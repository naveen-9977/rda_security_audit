using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.DTOs
{
    public class CreateEmployeeDto
    {
        [Required] public string Fname { get; set; }
        [Required] public string Lname { get; set; }
        [Required] public int Designation_Id { get; set; }
        [Required] public int Emp_Type_Id { get; set; }
        [Required] public int DptId { get; set; }
        [Required] public string Mobile_One { get; set; }

        public string? Email_Id { get; set; }
        public IFormFile? Image { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public int? Marital_Status { get; set; }
        public string? Blood_Group { get; set; }
        public int? District_Id { get; set; }
        public int? State_Id { get; set; }
        public string? Address { get; set; }
        public DateTime? Pr_DateOf_Joining { get; set; }
        public int? AdminId { get; set; }
    }
}