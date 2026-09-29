using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class ServiceItem
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}