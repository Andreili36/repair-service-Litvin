using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class Order
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public long EquipmentId { get; set; }
        public long StatusId { get; set; }
        public long PriorityId { get; set; }
        public DateTime AcceptedAt { get; set; }
        public DateTime? PlannedEndAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal? TotalCost { get; set; }
        public string? Description { get; set; }
        public long CreatedByUserId { get; set; }
        public long? AssignedEngineerId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}