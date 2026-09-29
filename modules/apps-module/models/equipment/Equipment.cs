using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class Equipment
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public long ModelId { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}