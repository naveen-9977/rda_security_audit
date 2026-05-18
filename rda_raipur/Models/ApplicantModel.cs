using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class ApplicantModel
    {
        [Key]   // Primary Key
        public int ApplicantId { get; set; }
       
        [Required]
        public string? ApplicantName { get; set; }
        public string? FatherName { get; set; }
        public DateTime? DOB { get; set; }
        
        [Required]
        public string Mobile { get; set; }
        //public string Email { get; set; }

        public string CorrespondenceAddress { get; set; }
        //public string PermanentAddress { get; set; }

        //public string Profession { get; set; }

        //public decimal AnnualIncomeSelf { get; set; }
        //public decimal AnnualIncomeFamily { get; set; }

        //public string Category { get; set; }
    }
}
