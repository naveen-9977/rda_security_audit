using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class BookingProfileDetail
    {
        [Key]
        public int Id { get; set; }

        // Booking के समय की पर्सनल डिटेल्स
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

        // 🔥 NEW PROPERTIES ADDED 🔥
        public string? EmailId { get; set; }        // रिपोर्ट के लिए ईमेल
        public string? PresentAddress { get; set; }  // रिपोर्ट के लिए एड्रेस
        public string? Address { get; set; }         // पुराना एड्रेस कॉलम (बैकअप के लिए रखें)

        // 🔥 BANK DETAILS (For Refunds) 🔥
        public string? BankName { get; set; }
        public string? BankAccountNo { get; set; }
        public string? IfscCode { get; set; }

        // अपलोड किए गए डॉक्यूमेंट्स का पाथ
        public string? PhotoPath { get; set; }
        public string? SignaturePath { get; set; }
        public string? AadharPath { get; set; }
        public string? PanPath { get; set; }
        public string? CastePath { get; set; }
        public string? IncomePath { get; set; }
        public string? DomicilePath { get; set; }
        public string? AffidavitPath { get; set; }

        // 🔥 NEW: BANK PASSBOOK PATH 🔥
        public string? BankPassbookPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}