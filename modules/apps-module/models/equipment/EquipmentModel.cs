using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class EquipmentModel
    {
        public long Id { get; set; }
        public long BrandId { get; set; }
        public long TypeId { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}