using RepairService.Modules.AppsModule.Components;
using RepairService.Modules.AppsModule.Models;
using RepairService.Shared.Contracts;
using System.Collections.Generic;

namespace RepairService.Modules.AppsModule.Catalogs
{
    public static class AppCatalogs
    {
        public static IEnumerable<CatalogEntry> Descriptors()
        {
            yield return UsersEntry();
            yield return RolesEntry();
            yield return UserTypesEntry();
            yield return UserSettingsEntry();
            yield return EmployeesEntry();
            yield return ObjectClaimsEntry();
        }

        private static CatalogEntry UsersEntry() => new()
        {
            ObjectCode = "users",
            Title = "Пользователи",
            DescriptorFactory = UsersDescriptor
        };

        private static CatalogEntry RolesEntry() => new()
        {
            ObjectCode = "roles",
            Title = "Роли",
            DescriptorFactory = RolesDescriptor
        };

        private static CatalogEntry UserTypesEntry() => new()
        {
            ObjectCode = "user_types",
            Title = "Типы пользователей",
            DescriptorFactory = UserTypesDescriptor
        };

        private static CatalogEntry UserSettingsEntry() => new()
        {
            ObjectCode = "user_settings",
            Title = "Настройки пользователей",
            DescriptorFactory = UserSettingsDescriptor
        };

        private static CatalogEntry EmployeesEntry() => new()
        {
            ObjectCode = "employees",
            Title = "Сотрудники",
            DescriptorFactory = EmployeesDescriptor
        };

        private static CatalogEntry ObjectClaimsEntry() => new()
        {
            ObjectCode = "object_claims",
            Title = "Права доступа",
            DescriptorFactory = ObjectClaimsDescriptor
        };

        private static SectionDescriptor UsersDescriptor() => new()
        {
            ObjectCode = "users",
            Title = "Пользователи",
            LoadData = db => db.SqlQuery(
                @"SELECT id, login, last_name, first_name, middle_name, email, phone,
                         access_banned, last_auth_date
                  FROM users ORDER BY login",
                r => new UserRow
                {
                    Id = r.GetInt64(0),
                    Login = r.GetString(1),
                    LastName = r.GetString(2),
                    FirstName = r.GetString(3),
                    MiddleName = r.IsDBNull(4) ? null : r.GetString(4),
                    Email = r.IsDBNull(5) ? null : r.GetString(5),
                    Phone = r.IsDBNull(6) ? null : r.GetString(6),
                    AccessBanned = r.GetBoolean(7),
                    LastAuthDate = r.IsDBNull(8) ? (DateTime?)null : r.GetDateTime(8)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",            Header = "ID",           Type = ColumnType.Number },
                new() { Key = "Login",         Header = "Логин",        Type = ColumnType.Text },
                new() { Key = "LastName",      Header = "Фамилия",      Type = ColumnType.Text },
                new() { Key = "FirstName",     Header = "Имя",          Type = ColumnType.Text },
                new() { Key = "Email",         Header = "Email",        Type = ColumnType.Text },
                new() { Key = "Phone",         Header = "Телефон",      Type = ColumnType.Text },
                new() { Key = "AccessBanned",  Header = "Заблокирован", Type = ColumnType.Bool }
            },
            FormFields = new List<FormField>
            {
                new() { Key = "Login",       Label = "Логин" },
                new() { Key = "Password",    Label = "Пароль", Kind = FieldKind.Password },
                new() { Key = "LastName",    Label = "Фамилия" },
                new() { Key = "FirstName",   Label = "Имя" },
                new() { Key = "MiddleName",  Label = "Отчество" },
                new() { Key = "Email",       Label = "Email" },
                new() { Key = "Phone",       Label = "Телефон" },
                new() { Key = "AccessBanned",  Label = "Заблокирован", Kind = FieldKind.Bool }
            },
            OnCreate = (db, data) => db.SqlExecute(
                @"INSERT INTO users
                    (login, password, last_name, first_name, middle_name, email, phone, role_id, user_type_id)
                  VALUES (@l, @p, @ln, @fn, @mn, @em, @ph, 2, 1)",
                new Dictionary<string, object?>
                {
                    ["@l"] = data.GetValueOrNull("Login"),
                    ["@p"] = data.GetValueOrNull("Password"),
                    ["@ln"] = data.GetValueOrNull("LastName"),
                    ["@fn"] = data.GetValueOrNull("FirstName"),
                    ["@mn"] = data.GetValueOrNull("MiddleName"),
                    ["@em"] = data.GetValueOrNull("Email"),
                    ["@ph"] = data.GetValueOrNull("Phone")
                }),
            OnUpdate = (db, row, data) => db.SqlExecute(
                @"UPDATE users
                  SET last_name = @ln, first_name = @fn, middle_name = @mn,
                      email = @em, phone = @ph, access_banned = @ab
                  WHERE login = @l",
                new Dictionary<string, object?>
                {
                    ["@ln"] = data.GetValueOrNull("LastName"),
                    ["@fn"] = data.GetValueOrNull("FirstName"),
                    ["@mn"] = data.GetValueOrNull("MiddleName"),
                    ["@em"] = data.GetValueOrNull("Email"),
                    ["@ph"] = data.GetValueOrNull("Phone"),
                    ["@ab"] = data.GetValueOrNull("AccessBanned"),
                    ["@l"] = GetValue(row, "Login")
                }),
            OnDelete = (db, row) => db.SqlExecute(
                "DELETE FROM users WHERE login = @l",
                new Dictionary<string, object?>
                {
                    ["@l"] = GetValue(row, "Login")
                })
        };

        private static SectionDescriptor RolesDescriptor() => new()
        {
            ObjectCode = "roles",
            Title = "Роли",
            LoadData = db => db.SqlQuery(
                "SELECT id, name FROM roles ORDER BY id",
                r => new Role
                {
                    Id = r.GetInt64(0),
                    Name = r.GetString(1)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",   Header = "ID",           Type = ColumnType.Number },
                new() { Key = "Name", Header = "Наименование", Type = ColumnType.Text }
            },
            FormFields = new List<FormField>
            {
                new() { Key = "Name", Label = "Наименование" }
            },
            OnCreate = (db, data) => db.SqlExecute(
                "INSERT INTO roles (name) VALUES (@n)",
                new Dictionary<string, object?> { ["@n"] = data.GetValueOrNull("Name") }),
            OnUpdate = (db, row, data) => db.SqlExecute(
                "UPDATE roles SET name = @n WHERE id = @id",
                new Dictionary<string, object?>
                {
                    ["@n"] = data.GetValueOrNull("Name"),
                    ["@id"] = GetValue(row, "Id")
                }),
            OnDelete = (db, row) => db.SqlExecute(
                "DELETE FROM roles WHERE id = @id",
                new Dictionary<string, object?> { ["@id"] = GetValue(row, "Id") })
        };

        private static SectionDescriptor UserTypesDescriptor() => new()
        {
            ObjectCode = "user_types",
            Title = "Типы пользователей",
            LoadData = db => db.SqlQuery(
                "SELECT id, code, name FROM user_types ORDER BY id",
                r => new UserType
                {
                    Id = r.GetInt64(0),
                    Code = r.GetString(1),
                    Name = r.GetString(2)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",   Header = "ID",           Type = ColumnType.Number },
                new() { Key = "Code", Header = "Код",          Type = ColumnType.Text },
                new() { Key = "Name", Header = "Наименование", Type = ColumnType.Text }
            },
            FormFields = new List<FormField>()
        };

        private static SectionDescriptor UserSettingsDescriptor() => new()
        {
            ObjectCode = "user_settings",
            Title = "Настройки пользователей",
            LoadData = db => db.SqlQuery(
                @"SELECT id, name, can_reset_password, block_on_wrong_attempts,
                         max_wrong_attempts, auto_unblock_minutes, password_min_length,
                         password_digit_length, password_upper_length, password_special_length
                  FROM user_settings ORDER BY id",
                r => new UserSetting
                {
                    Id = r.GetInt64(0),
                    Name = r.GetString(1),
                    CanResetPassword = r.GetBoolean(2),
                    BlockOnWrongAttempts = r.GetBoolean(3),
                    MaxWrongAttempts = r.GetInt32(4),
                    AutoUnblockMinutes = r.GetInt32(5),
                    PasswordMinLength = r.GetInt32(6),
                    PasswordDigitLength = r.GetInt32(7),
                    PasswordUpperLength = r.GetInt32(8),
                    PasswordSpecialLength = r.GetInt32(9)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",                       Header = "ID",                Type = ColumnType.Number },
                new() { Key = "Name",                     Header = "Наименование",      Type = ColumnType.Text },
                new() { Key = "CanResetPassword",         Header = "Сброс пароля",      Type = ColumnType.Bool },
                new() { Key = "BlockOnWrongAttempts",     Header = "Блокировка",        Type = ColumnType.Bool },
                new() { Key = "MaxWrongAttempts",         Header = "Попыток",           Type = ColumnType.Number },
                new() { Key = "AutoUnblockMinutes",       Header = "Авторазблок (мин)", Type = ColumnType.Number },
                new() { Key = "PasswordMinLength",        Header = "Мин. длина",        Type = ColumnType.Number }
            },
            FormFields = new List<FormField>
            {
                new() { Key = "Name",                     Label = "Наименование" },
                new() { Key = "CanResetPassword",         Label = "Сброс пароля",         Kind = FieldKind.Bool },
                new() { Key = "BlockOnWrongAttempts",     Label = "Блокировка",           Kind = FieldKind.Bool },
                new() { Key = "MaxWrongAttempts",         Label = "Попыток до блокировки", Kind = FieldKind.Int, Value = 3 },
                new() { Key = "AutoUnblockMinutes",       Label = "Авторазблок (мин)",    Kind = FieldKind.Int, Value = 60 },
                new() { Key = "PasswordMinLength",        Label = "Мин. длина пароля",    Kind = FieldKind.Int, Value = 8 },
                new() { Key = "PasswordDigitLength",      Label = "Мин. цифр",            Kind = FieldKind.Int, Value = 1 },
                new() { Key = "PasswordUpperLength",      Label = "Мин. заглавных",       Kind = FieldKind.Int, Value = 0 },
                new() { Key = "PasswordSpecialLength",    Label = "Мин. спецсимволов",    Kind = FieldKind.Int, Value = 0 }
            },
            OnCreate = (db, data) => db.SqlExecute(
                @"INSERT INTO user_settings
                    (name, can_reset_password, block_on_wrong_attempts, max_wrong_attempts,
                     auto_unblock_minutes, password_min_length, password_digit_length,
                     password_upper_length, password_special_length)
                  VALUES (@n, @cr, @bw, @mw, @au, @pl, @dl, @ul, @sl)",
                new Dictionary<string, object?>
                {
                    ["@n"] = data.GetValueOrNull("Name"),
                    ["@cr"] = data.GetValueOrNull("CanResetPassword"),
                    ["@bw"] = data.GetValueOrNull("BlockOnWrongAttempts"),
                    ["@mw"] = data.GetValueOrNull("MaxWrongAttempts"),
                    ["@au"] = data.GetValueOrNull("AutoUnblockMinutes"),
                    ["@pl"] = data.GetValueOrNull("PasswordMinLength"),
                    ["@dl"] = data.GetValueOrNull("PasswordDigitLength"),
                    ["@ul"] = data.GetValueOrNull("PasswordUpperLength"),
                    ["@sl"] = data.GetValueOrNull("PasswordSpecialLength")
                }),
            OnUpdate = (db, row, data) => db.SqlExecute(
                @"UPDATE user_settings
                  SET name = @n, can_reset_password = @cr, block_on_wrong_attempts = @bw,
                      max_wrong_attempts = @mw, auto_unblock_minutes = @au,
                      password_min_length = @pl, password_digit_length = @dl,
                      password_upper_length = @ul, password_special_length = @sl
                  WHERE id = @id",
                new Dictionary<string, object?>
                {
                    ["@n"] = data.GetValueOrNull("Name"),
                    ["@cr"] = data.GetValueOrNull("CanResetPassword"),
                    ["@bw"] = data.GetValueOrNull("BlockOnWrongAttempts"),
                    ["@mw"] = data.GetValueOrNull("MaxWrongAttempts"),
                    ["@au"] = data.GetValueOrNull("AutoUnblockMinutes"),
                    ["@pl"] = data.GetValueOrNull("PasswordMinLength"),
                    ["@dl"] = data.GetValueOrNull("PasswordDigitLength"),
                    ["@ul"] = data.GetValueOrNull("PasswordUpperLength"),
                    ["@sl"] = data.GetValueOrNull("PasswordSpecialLength"),
                    ["@id"] = GetValue(row, "Id")
                }),
            OnDelete = (db, row) => db.SqlExecute(
                "DELETE FROM user_settings WHERE id = @id",
                new Dictionary<string, object?> { ["@id"] = GetValue(row, "Id") })
        };

        private static SectionDescriptor EmployeesDescriptor() => new()
        {
            ObjectCode = "employees",
            Title = "Сотрудники",
            LoadData = db => db.SqlQuery(
                @"SELECT id, last_name, first_name, middle_name, position, phone, email, hire_date
                  FROM employees ORDER BY last_name",
                r => new Employee
                {
                    Id = r.GetInt64(0),
                    LastName = r.GetString(1),
                    FirstName = r.GetString(2),
                    MiddleName = r.IsDBNull(3) ? null : r.GetString(3),
                    Position = r.IsDBNull(4) ? null : r.GetString(4),
                    Phone = r.IsDBNull(5) ? null : r.GetString(5),
                    Email = r.IsDBNull(6) ? null : r.GetString(6),
                    HireDate = r.IsDBNull(7) ? (DateTime?)null : r.GetDateTime(7)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",         Header = "ID",          Type = ColumnType.Number },
                new() { Key = "LastName",   Header = "Фамилия",     Type = ColumnType.Text },
                new() { Key = "FirstName",  Header = "Имя",         Type = ColumnType.Text },
                new() { Key = "Position",   Header = "Должность",   Type = ColumnType.Text },
                new() { Key = "Phone",      Header = "Телефон",     Type = ColumnType.Text },
                new() { Key = "Email",      Header = "Email",       Type = ColumnType.Text },
                new() { Key = "HireDate",   Header = "Дата приёма", Type = ColumnType.Date }
            },
            FormFields = new List<FormField>
            {
                new() { Key = "LastName",   Label = "Фамилия" },
                new() { Key = "FirstName",  Label = "Имя" },
                new() { Key = "MiddleName", Label = "Отчество" },
                new() { Key = "Position",   Label = "Должность" },
                new() { Key = "Phone",      Label = "Телефон" },
                new() { Key = "Email",      Label = "Email" },
                new() { Key = "HireDate",   Label = "Дата приёма", Kind = FieldKind.Date }
            },
            OnCreate = (db, data) => db.SqlExecute(
                @"INSERT INTO employees (last_name, first_name, middle_name, position, phone, email, hire_date)
                  VALUES (@ln, @fn, @mn, @p, @ph, @em, @hd)",
                new Dictionary<string, object?>
                {
                    ["@ln"] = data.GetValueOrNull("LastName"),
                    ["@fn"] = data.GetValueOrNull("FirstName"),
                    ["@mn"] = data.GetValueOrNull("MiddleName"),
                    ["@p"] = data.GetValueOrNull("Position"),
                    ["@ph"] = data.GetValueOrNull("Phone"),
                    ["@em"] = data.GetValueOrNull("Email"),
                    ["@hd"] = data.GetValueOrNull("HireDate")
                }),
            OnUpdate = (db, row, data) => db.SqlExecute(
                @"UPDATE employees
                  SET last_name = @ln, first_name = @fn, middle_name = @mn,
                      position = @p, phone = @ph, email = @em, hire_date = @hd
                  WHERE id = @id",
                new Dictionary<string, object?>
                {
                    ["@ln"] = data.GetValueOrNull("LastName"),
                    ["@fn"] = data.GetValueOrNull("FirstName"),
                    ["@mn"] = data.GetValueOrNull("MiddleName"),
                    ["@p"] = data.GetValueOrNull("Position"),
                    ["@ph"] = data.GetValueOrNull("Phone"),
                    ["@em"] = data.GetValueOrNull("Email"),
                    ["@hd"] = data.GetValueOrNull("HireDate"),
                    ["@id"] = GetValue(row, "Id")
                }),
            OnDelete = (db, row) => db.SqlExecute(
                "DELETE FROM employees WHERE id = @id",
                new Dictionary<string, object?> { ["@id"] = GetValue(row, "Id") })
        };

        private static SectionDescriptor ObjectClaimsDescriptor() => new()
        {
            ObjectCode = "object_claims",
            Title = "Права доступа",
            LoadData = db => db.SqlQuery(
                @"SELECT oc.id, oc.role_id, r.name AS role_name, oc.object_code,
                         oc.can_view, oc.can_create, oc.can_edit, oc.can_delete
                  FROM object_claims oc
                  JOIN roles r ON r.id = oc.role_id
                  ORDER BY r.name, oc.object_code",
                r => new ObjectClaimRow
                {
                    Id = r.GetInt64(0),
                    RoleId = r.GetInt64(1),
                    RoleName = r.GetString(2),
                    ObjectCode = r.GetString(3),
                    CanView = r.GetBoolean(4),
                    CanCreate = r.GetBoolean(5),
                    CanEdit = r.GetBoolean(6),
                    CanDelete = r.GetBoolean(7)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",         Header = "ID",        Type = ColumnType.Number },
                new() { Key = "RoleName",   Header = "Роль",      Type = ColumnType.Text },
                new() { Key = "ObjectCode", Header = "Объект",    Type = ColumnType.Text },
                new() { Key = "CanView",    Header = "Просмотр",  Type = ColumnType.Bool },
                new() { Key = "CanCreate",  Header = "Создание",  Type = ColumnType.Bool },
                new() { Key = "CanEdit",    Header = "Изменение", Type = ColumnType.Bool },
                new() { Key = "CanDelete",  Header = "Удаление",  Type = ColumnType.Bool }
            },
            FormFields = new List<FormField>
            {
                new() { Key = "RoleId",     Label = "ID роли",   Kind = FieldKind.Int },
                new() { Key = "ObjectCode", Label = "Объект" },
                new() { Key = "CanView",    Label = "Просмотр",  Kind = FieldKind.Bool },
                new() { Key = "CanCreate",  Label = "Создание",  Kind = FieldKind.Bool },
                new() { Key = "CanEdit",    Label = "Изменение", Kind = FieldKind.Bool },
                new() { Key = "CanDelete",  Label = "Удаление",  Kind = FieldKind.Bool }
            },
            OnCreate = (db, data) => db.SqlExecute(
                @"INSERT INTO object_claims (role_id, object_code, can_view, can_create, can_edit, can_delete)
                  VALUES (@r, @o, @v, @c, @e, @d)",
                new Dictionary<string, object?>
                {
                    ["@r"] = data.GetValueOrNull("RoleId"),
                    ["@o"] = data.GetValueOrNull("ObjectCode"),
                    ["@v"] = data.GetValueOrNull("CanView"),
                    ["@c"] = data.GetValueOrNull("CanCreate"),
                    ["@e"] = data.GetValueOrNull("CanEdit"),
                    ["@d"] = data.GetValueOrNull("CanDelete")
                }),
            OnUpdate = (db, row, data) => db.SqlExecute(
                @"UPDATE object_claims
                  SET can_view = @v, can_create = @c, can_edit = @e, can_delete = @d
                  WHERE id = @id",
                new Dictionary<string, object?>
                {
                    ["@v"] = data.GetValueOrNull("CanView"),
                    ["@c"] = data.GetValueOrNull("CanCreate"),
                    ["@e"] = data.GetValueOrNull("CanEdit"),
                    ["@d"] = data.GetValueOrNull("CanDelete"),
                    ["@id"] = GetValue(row, "Id")
                }),
            OnDelete = (db, row) => db.SqlExecute(
                "DELETE FROM object_claims WHERE id = @id",
                new Dictionary<string, object?> { ["@id"] = GetValue(row, "Id") })
        };

        private static object? GetValue(object row, string key)
        {
            if (row is IDictionary<string, object?> dict)
                return dict.GetValueOrNull(key);
            var prop = row.GetType().GetProperty(key);
            return prop?.GetValue(row);
        }
    }
}