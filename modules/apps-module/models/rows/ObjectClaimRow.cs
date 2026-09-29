namespace RepairService.Modules.AppsModule.Models
{
    public class ObjectClaimRow
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public string ObjectCode { get; set; } = "";
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}