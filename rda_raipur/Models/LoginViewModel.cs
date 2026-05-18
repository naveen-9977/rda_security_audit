using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(10, ErrorMessage = "Username cannot exceed 10 characters.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only numbers allowed.")]
        [Display(Name = "Mobile Number")]
        public string Username { get; set; } = string.Empty; // <-- Fixed

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty; // <-- Fixed

        [Display(Name = "Enter OTP")]
        public string? Otp { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}