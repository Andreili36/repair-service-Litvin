using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class OrderRow
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public long EquipmentId { get; set; }
        public long StatusId { get; set; }
        public long PriorityId { get; set; }
        public string ClientName { get; set; } = "";
        public string StatusName { get; set; } = "";
        public string PriorityName { get; set; } = "";
        public decimal? TotalCost { get; set; }
        public DateTime AcceptedAt { get; set; }
    }
}