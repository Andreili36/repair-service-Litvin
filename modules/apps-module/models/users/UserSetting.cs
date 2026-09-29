using System;

namespace RepairService.Modules.AppsModule.Models
{
    public class UserSetting
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public bool CanResetPassword { get; set; }
        public bool BlockOnWrongAttempts { get; set; }
        public int MaxWrongAttempts { get; set; }
        public int AutoUnblockMinutes { get; set; }
        public int PasswordMinLength { get; set; }
        public int PasswordDigitLength { get; set; }
        public int PasswordUpperLength { get; set; }
        public int PasswordSpecialLength { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}