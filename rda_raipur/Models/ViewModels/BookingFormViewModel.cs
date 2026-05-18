using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models.ViewModels
{
    public class BookingFormViewModel
    {
        // --- व्यक्तिगत विवरण (Personal Details) ---
        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid Mobile Number")]
        public string? MobileNo { get; set; }

        [Required(ErrorMessage = "Applicant name is required")]
        public string? ApplicantName { get; set; }

        public string? FatherHusbandName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        public int? Age { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }

        // --- 🏦 बैंक विवरण (Bank Details for Refund) ---
        public string? BankAccountNo { get; set; }

        public string? IfscCode { get; set; }

        public string? BankName { get; set; }

        // --- दस्तावेज़ नंबर (Document Numbers) ---
        public string? AadharNo { get; set; }

        public string? PanNo { get; set; }

        public string? CasteNo { get; set; }

        public string? IncomeNo { get; set; }

        public string? DomicileNo { get; set; }

        // --- प्रॉपर्टी विवरण (Property Details) ---
        public string? PropertyId { get; set; } // यह Asset No / Tender Property ID के लिए है

        public decimal BidAmount { get; set; }

        // --- फ़ाइल अपलोड (File Uploads) ---
        public IFormFile? PhotoUpload { get; set; }
        public IFormFile? SignatureUpload { get; set; }
        public IFormFile? AadharUpload { get; set; }
        public IFormFile? PanUpload { get; set; }
        public IFormFile? CasteUpload { get; set; }
        public IFormFile? IncomeUpload { get; set; }
        public IFormFile? DomicileUpload { get; set; }
        public IFormFile? AffidavitUpload { get; set; }

        // 🔥 Naya Field: Bank Passbook ya Cancelled Cheque upload karne ke liye 🔥
        public IFormFile? BankPassbookUpload { get; set; }
    }
}