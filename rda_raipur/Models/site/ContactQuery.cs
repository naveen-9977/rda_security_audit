using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models.site
{
    public class ContactQuery
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [MaxLength(20)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid 10-digit phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter your message")]
        public string Message { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}