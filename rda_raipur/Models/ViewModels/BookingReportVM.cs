using System;

namespace rda_raipur.Models.ViewModels
{
    public class BookingReportVM
    {
        // Property Details
        public string PropertyId { get; set; }
        public string SchemeName { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string FlatName { get; set; }
        public string HouseNo { get; set; }

        // Applicant Details
        public string ApplicantName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Category { get; set; }

        // Booking & Payment (Updated)
        public DateTime BookingDate { get; set; }

        // 🔥 Bifurcated Payment Fields
        public decimal ApplicationFee { get; set; }
        public decimal EMDAmount { get; set; }
        public decimal TotalPaid { get; set; }

        public string PaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public string VerificationStatus { get; set; }
    }
}