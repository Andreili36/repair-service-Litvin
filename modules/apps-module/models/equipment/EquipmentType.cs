using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class EquipmentType
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}