using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class EquipmentRow
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public long ModelId { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string ClientName { get; set; } = "";
        public string BrandName { get; set; } = "";
        public string ModelName { get; set; } = "";
    }
}