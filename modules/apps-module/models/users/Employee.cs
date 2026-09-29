using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class Employee
    {
        public long Id { get; set; }
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string? MiddleName { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}