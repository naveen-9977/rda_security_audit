using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Mobile Number is required")]
        public string MobileNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        // OTP verification field
        public string? Otp { get; set; }
    }
}