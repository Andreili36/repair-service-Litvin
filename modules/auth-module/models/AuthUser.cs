namespace RepairService.Modules.AuthModule.Models
{
    public class AuthUser
    {
        public long Id { get; set; }
        public string Login { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public long RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public long UserTypeId { get; set; }
        public string UserTypeCode { get; set; } = "";
        public long? EmployeeId { get; set; }
        public bool ChangePassword { get; set; }
    }
}