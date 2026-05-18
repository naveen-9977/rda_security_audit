using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{

    [Table("UserProfileDetails")]
    public class UserProfileDetail
    {
        [Key]
        public int Id { get; set; }
        public string? MobileNo { get; set; }
        public string? ApplicantName { get; set; }
        public string? FatherHusbandName { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Category { get; set; }
        public string? AadharNo { get; set; }
        public string? PanNo { get; set; }
        public string? CasteNo { get; set; }
        public string? IncomeNo { get; set; }
        public string? DomicileNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNo { get; set; }
        public string? IfscCode { get; set; }

        // Upload Paths (Latest)
        public string? PhotoPath { get; set; }
        public string? SignaturePath { get; set; }
        public string? AadharPath { get; set; }
        public string? PanPath { get; set; }
        public string? CastePath { get; set; }
        public string? IncomePath { get; set; }
        public string? DomicilePath { get; set; }
        public string? AffidavitPath { get; set; }
        public string? BankPassbookPath { get; set; }


        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}