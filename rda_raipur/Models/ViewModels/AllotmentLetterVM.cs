namespace rda_raipur.Models.ViewModels
{
    public class AllotmentLetterVM
    {
        public int BookingId { get; set; }

        // Applicant Details
        public string ApplicantName { get; set; }
        public string FatherName { get; set; }
        public string Address { get; set; }
        public string MobileNo { get; set; }

        // Booking Details
        public string ApplicationNo { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? AllotmentDate { get; set; }

        // Property Details
        public string SchemeName { get; set; }
        public string SectorName { get; set; }

        public string block_name_en { get; set; }
        public string Flat_name_en { get; set; }

        public string Property_Type_Name_en { get; set; }
      


        public string Model_name_en { get; set; }

        public string PropertyType { get; set; }
        public string PropertyNo { get; set; }
        public string Direction { get; set; }

        

        public decimal? BetterLocationAmount { get; set; }

        public decimal? BetterLocationGST { get; set; }

        public string? BetterLocation { get; set; }



        public string allotement_type_name_en { get; set; }


        public decimal? SuperBuildupArea { get; set; }
        public decimal? BuildupArea { get; set; }
        public decimal? CarpetArea { get; set; }

        public decimal? Width { get; set; }
        public decimal? Depth { get; set; }

        // Financial Details
        public decimal? BidAmount { get; set; }
        public decimal BidAmountGST { get; set; }
        public decimal? EmdAmount { get; set; }
        public decimal? ApplicationFee { get; set; }

        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; }


        public decimal RegistrationAmount { get; set; }

        public decimal FirstInstallmentAmount { get; set; }

        public decimal SecondInstallmentAmount { get; set; }

        public decimal MaintenanceCharge { get; set; }
       
        public decimal RegistrationGST { get; set; }

        public decimal FirstInstallmentGST { get; set; }

        public decimal SecondInstallmentGST { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public DateTime? FirstInstallmentDate { get; set; }

        public DateTime? SecondInstallmentDate { get; set; }

        
    }
}
