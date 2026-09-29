using RepairService.Db;
using RepairService.Modules.AuthModule.Models;
using RepairService.Shared.Models;

namespace RepairService.Modules.AuthModule.Domain
{
    public class AuthService
    {
        private readonly DbUnibridge _db;

        public AuthService(DbUnibridge db)
        {
            _db = db;
        }

        public AuthUser? Authenticate(string login, string password, bool captchaComplete, out string error)
        {
            error = "";

            var row = _db.SqlQuery(
                @"SELECT u.id, u.login, u.last_name, u.first_name, u.middle_name,
                         u.password, u.access_banned, u.access_ban_end, u.last_auth_date,
                         u.count_wrong_pass_entries, u.change_password,
                         u.role_id, r.name AS role_name,
                         u.user_type_id, ut.code AS user_type_code,
                         u.employee_id, u.user_setting_id
                  FROM users u
                  JOIN roles r         ON r.id = u.role_id
                  JOIN user_types ut   ON ut.id = u.user_type_id
                  WHERE u.login = @l",
                r => new
                {
                    Id = r.GetInt64(0),
                    Login = r.GetString(1),
                    LastName = r.GetString(2),
                    FirstName = r.GetString(3),
                    MiddleName = r.IsDBNull(4) ? "" : r.GetString(4),
                    Password = r.GetString(5),
                    AccessBanned = r.GetBoolean(6),
                    AccessBanEnd = r.IsDBNull(7) ? (DateTime?)null : r.GetDateTime(7),
                    LastAuthDate = r.IsDBNull(8) ? (DateTime?)null : r.GetDateTime(8),
                    CountWrongPass = r.GetInt32(9),
                    ChangePassword = r.GetBoolean(10),
                    RoleId = r.GetInt64(11),
                    RoleName = r.GetString(12),
                    UserTypeId = r.GetInt64(13),
                    UserTypeCode = r.GetString(14),
                    EmployeeId = r.IsDBNull(15) ? (long?)null : r.GetInt64(15),
                    UserSettingId = r.IsDBNull(16) ? (long?)null : r.GetInt64(16)
                },
                new Dictionary<string, object?> { ["@l"] = login })
                .FirstOrDefault();

            if (row is null)
            {
                error = "Вы ввели неверный логин или пароль. Пожалуйста, проверьте ещё раз введенные данные.";
                return null;
            }

            if (row.AccessBanned)
            {
                if (row.AccessBanEnd.HasValue && row.AccessBanEnd.Value <= DateTime.Now)
                {
                    Unblock(row.Id);
                }
                else
                {
                    error = "Вы заблокированы. Обратитесь к администратору.";
                    return null;
                }
            }

            if (row.LastAuthDate.HasValue &&
                row.LastAuthDate.Value < DateTime.Now.AddMonths(-1))
            {
                Block(row.Id);
                error = "Вы заблокированы. Обратитесь к администратору.";
                return null;
            }

            if (row.Password != password || !captchaComplete)
            {
                var newCount = row.CountWrongPass + 1;
                var (maxAttempts, autoUnblock) = GetSettings(row.UserSettingId);

                if (newCount >= maxAttempts)
                {
                    Block(row.Id, autoUnblock);
                    error = "Вы заблокированы. Обратитесь к администратору.";
                }
                else
                {
                    _db.SqlExecute(
                        "UPDATE users SET count_wrong_pass_entries = @c WHERE id = @id",
                        new Dictionary<string, object?>
                        {
                            ["@c"] = newCount,
                            ["@id"] = row.Id
                        });
                    error = "Вы ввели неверный логин или пароль. Пожалуйста, проверьте ещё раз введенные данные.";
                }
                return null;
            }

            var now = DateTime.Now;
            _db.SqlExecute(
                @"UPDATE users
                  SET count_wrong_pass_entries = 0,
                      last_auth_date = @now,
                      first_auth_date = COALESCE(first_auth_date, @now)
                  WHERE id = @id",
                new Dictionary<string, object?>
                {
                    ["@now"] = now,
                    ["@id"] = row.Id
                });

            return new AuthUser
            {
                Id = row.Id,
                Login = row.Login,
                LastName = row.LastName,
                FirstName = row.FirstName,
                MiddleName = row.MiddleName,
                RoleId = row.RoleId,
                RoleName = row.RoleName,
                UserTypeId = row.UserTypeId,
                UserTypeCode = row.UserTypeCode,
                EmployeeId = row.EmployeeId,
                ChangePassword = row.ChangePassword
            };
        }

        public List<UserClaim> GetClaims(long roleId)
        {
            return _db.SqlQuery(
                @"SELECT object_code, can_view, can_create, can_edit, can_delete
                  FROM object_claims
                  WHERE role_id = @r",
                r => new UserClaim
                {
                    ObjectCode = r.GetString(0),
                    CanView = r.GetBoolean(1),
                    CanCreate = r.GetBoolean(2),
                    CanEdit = r.GetBoolean(3),
                    CanDelete = r.GetBoolean(4)
                },
                new Dictionary<string, object?> { ["@r"] = roleId });
        }

        public User GetUserData(long userId)
        {
            return _db.SqlQuery(
                @"SELECT u.id, u.code, u.login, u.last_name, u.first_name, u.middle_name,
                         u.role_id, r.name AS role_name, u.employee_id
                  FROM users u
                  JOIN roles r         ON r.id = u.role_id
                  WHERE u.id = @id",
                r => new User
                {
                    Id = r.GetInt64(0),
                    Code = r.IsDBNull(1) ? "" : r.GetString(1),
                    Login = r.GetString(2),
                    LastName = r.GetString(3),
                    FirstName = r.GetString(4),
                    MiddleName = r.IsDBNull(5) ? "" : r.GetString(5),
                    RoleId = r.GetInt64(6),
                    RoleName = r.GetString(7),
                    EmployeeId = r.IsDBNull(8) ? (long?)null : r.GetInt64(8),
                },
                new Dictionary<string, object?> { ["@id"] = userId }).First();
        }

        public void ChangePassword(long userId, string newPassword)
        {
            _db.SqlExecute(
                @"UPDATE users
                  SET password = @p,
                      change_password = false,
                      date_change_password = @now
                  WHERE id = @id",
                new Dictionary<string, object?>
                {
                    ["@p"] = newPassword,
                    ["@now"] = DateTime.Now,
                    ["@id"] = userId
                });
        }

        private void Block(long userId, int autoUnblockMinutes = 0)
        {
            DateTime? banEnd = autoUnblockMinutes > 0
                ? DateTime.Now.AddMinutes(autoUnblockMinutes)
                : (DateTime?)null;

            _db.SqlExecute(
                @"UPDATE users
                  SET access_banned = true,
                      access_ban_end = @end
                  WHERE id = @id",
                new Dictionary<string, object?>
                {
                    ["@end"] = banEnd,
                    ["@id"] = userId
                });
        }

        private void Unblock(long userId)
        {
            _db.SqlExecute(
                @"UPDATE users
                  SET access_banned = false,
                      access_ban_end = null,
                      count_wrong_pass_entries = 0
                  WHERE id = @id",
                new Dictionary<string, object?> { ["@id"] = userId });
        }

        private (int maxAttempts, int autoUnblockMinutes) GetSettings(long? userSettingId)
        {
            if (userSettingId is null)
                return (3, 0);

            var row = _db.SqlQuery(
                @"SELECT max_wrong_attempts, auto_unblock_minutes
                  FROM user_settings WHERE id = @id",
                r => (Max: r.GetInt32(0), Unblock: r.GetInt32(1)),
                new Dictionary<string, object?> { ["@id"] = userSettingId.Value })
                .FirstOrDefault();

            return row == default ? (3, 0) : (row.Max, row.Unblock);
        }
    }
}