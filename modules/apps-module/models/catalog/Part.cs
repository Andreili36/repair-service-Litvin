using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class Part
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string? Sku { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinStock { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}