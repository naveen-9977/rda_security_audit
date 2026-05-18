using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models.otp
{
    public class OtpLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MobileNo { get; set; } = string.Empty;

        [Required]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        public string? OtpType { get; set; } // "Login" ya "Registration"
    }
}