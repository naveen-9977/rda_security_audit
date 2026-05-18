using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class AlloteePaymentDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Alloty_Unique_Id { get; set; } // Ye link karega AllotyRegistration table se

        public decimal TotalWaterChargePaid { get; set; } = 0;
        public decimal TotalWaterChargeOutstanding { get; set; } = 0;

        public decimal TotalSurchargePaid { get; set; } = 0;
        public decimal TotalSurchargeRemaining { get; set; } = 0;

        public decimal OtherChargesPaid { get; set; } = 0;
        public decimal OtherChargesRemaining { get; set; } = 0;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}