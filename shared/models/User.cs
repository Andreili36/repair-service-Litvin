using System;

namespace RepairService.Shared.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public long? EmployeeId { get; set; }
        public string Comment { get; set; }
        public byte[]? Avatar { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public long? CreateUserId { get; set; }
        public long? ModificationUserId { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}