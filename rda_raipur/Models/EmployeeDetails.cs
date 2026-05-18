using rda_raipur.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    [Table("EmployeeDetails")]
    public class EmployeeDetails
    {
        [Key]
        public int EmpId { get; set; }

        // 👇 यह नया कॉलम है जो RDA-EMP-001 फॉर्मेट को स्टोर करेगा
        public string? Emp_Code { get; set; }

        public string Fname { get; set; }
        public string Lname { get; set; }
        public int Designation_Id { get; set; }
        public int Emp_Type_Id { get; set; }
        public int DptId { get; set; }
        public string? Image { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public int? Marital_Status { get; set; }
        public string? Email_Id { get; set; }
        public string? Blood_Group { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int? District_Id { get; set; }
        public int? State_Id { get; set; }
        public string? Address { get; set; }
        public string Mobile_One { get; set; }
        public int Status { get; set; }
        public DateTime? Pr_DateOf_Joining { get; set; }
        public DateTime Created_At { get; set; }
        public int? Created_By { get; set; }
    }
}