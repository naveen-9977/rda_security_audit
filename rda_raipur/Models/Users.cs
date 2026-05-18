using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } 

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty; // <-- Fixed

        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; } = string.Empty; // <-- Fixed

        public string? MobileNo { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Role { get; set; } = "Admin";

        public string? Otp { get; set; }

        public DateTime? OtpExpiry { get; set; }
    }
}