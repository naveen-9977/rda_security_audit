using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models.AllotyInstallmentDueDetails
{
    [Table("Alloty_Payment_Details")]
    public class AllotyPaymentDetails
    {
        [Key]
        public int Id { get; set; }

        public string Alloty_Unique_Id { get; set; }

        public DateTime? Due_Date { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Paid_Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Due_Amount { get; set; }
        public decimal? Total_Property_Cost { get; set; }
    }
}
