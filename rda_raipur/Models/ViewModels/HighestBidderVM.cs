using System;

namespace rda_raipur.Models.ViewModels
{
    public class HighestBidderVM
    {
        public string Tender_Property_Id { get; set; }
        public int BookingId { get; set; }

        public string SchemeName { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string FlatName { get; set; }
        public string Category { get; set; }
        public string PropertyType { get; set; }
        public string AllotmentType { get; set; }
        public string House_No { get; set; }
        public string ApplicationNo { get; set; }

        public string ApplicantName { get; set; }
        public string FatherHusbandName { get; set; }
        public string Gender { get; set; }
        public string UserCategory { get; set; }
        public string Address { get; set; }

        public decimal BidAmount { get; set; }
        public DateTime? BookingDate { get; set; }
        public int TotalBids { get; set; }

        public string PropertyStatus { get; set; }
        public string ApplicantBookingStatus { get; set; }
        public string CancelRemarks { get; set; }

        // 🔥 NEW: TenderOpenDate is now tracking per booking 🔥
        public DateTime? TenderOpenDate { get; set; }
        // इसे हटाकर नीचे वाला लिखें
        public bool IsTenderOpened { get; set; }
   
        public DateTime? AllotmentDate { get; set; }
    }
}