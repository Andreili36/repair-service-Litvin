using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class ObjectClaim
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public string ObjectCode { get; set; } = "";
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}