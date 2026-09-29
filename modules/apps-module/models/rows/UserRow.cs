using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class UserRow
    {
        public long Id { get; set; }
        public string Login { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool AccessBanned { get; set; }
        public DateTime? LastAuthDate { get; set; }
    }
}