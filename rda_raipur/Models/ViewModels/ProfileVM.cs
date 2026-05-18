using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models.ViewModels
{
    public class ProfileVM
    {
        // === Read-Only Info (Display on Left Card) ===
        public string EmpCode { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string ImagePath { get; set; }

        // === Editable Profile Info ===
        [Required(ErrorMessage = "First Name is required")]
        public string Fname { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string Lname { get; set; }

        public string Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string MaritalStatus { get; set; }
        public string BloodGroup { get; set; }
        public string Address { get; set; }

        // 🔥 Naye Fields (Taaki Employee khud select kar sake) 🔥
        public int? DptId { get; set; }
        public int? Designation_Id { get; set; }
        public int? EmpTypeId { get; set; } // Agar aapke EmployeeDetails model me ye hai

        // Photo upload karne ke liye
        public IFormFile ProfileImage { get; set; }

        // === Change Password Fields ===
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}