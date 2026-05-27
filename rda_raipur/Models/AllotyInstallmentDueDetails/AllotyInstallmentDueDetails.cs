using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models.AllotyInstallmentDueDetails
{

    [Table("Alloty_Installment_Due_Details")]
    public class AllotyInstallmentDueDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Alloty_Unique_Id { get; set; }

        public DateTime? Last_Due_Installment_Date { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total_Paid_Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total_Due_Amount { get; set; }

        [StringLength(20)]
        public string Payment_Status { get; set; }  // Completed / Pending

        public DateTime Created_Date { get; set; } = DateTime.Now;

        public decimal? Total_Property_Cost { get; set; }
        public decimal GST_Percent_Property { get; set; }
        public decimal Total_Property_Cost_With_GST { get; set; }
        public decimal GST_Percent_Paid { get; set; }
        public decimal Total_Paid_Amount_With_GST { get; set; }
        public decimal GST_Percent_Due { get; set; }
        public decimal Total_Due_Amount_With_GST { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Maintainance_Amount { get; set; }

        [StringLength(10)]
        public string Paid_Yes_NO { get; set; }  // Yes / No
        public string installment_type { get; set; }
    }
}


 