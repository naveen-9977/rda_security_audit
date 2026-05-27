namespace rda_raipur.Models
{
    public class HighestBidderListModel
    {
        public int TenderPropertyId { get; set; }
        public string SchemeName { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string FlatName { get; set; }
        public string ResCategory { get; set; }
        public string PropertyType { get; set; }
        public string AllotmentType { get; set; }
        public string ApplicantName { get; set; }
        public string FatherHusbandName { get; set; }
        public string Gender { get; set; }
        public string UserCategory { get; set; }
        public string Address { get; set; }
        public decimal BidAmount { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; }
        public string VerificationStatus { get; set; }

    }
}
