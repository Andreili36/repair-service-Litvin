namespace RepairService.Shared.Models
{
    public class UserClaim
    {
        public string ObjectCode { get; set; } = "";
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}