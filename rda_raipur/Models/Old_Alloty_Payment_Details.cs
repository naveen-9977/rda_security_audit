using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    [Table("Old_Alloty_Payment_Details")]
    public class Old_Alloty_Payment_Details
    {
        [Key]   // ✅
        public int Payment_Id { get; set; }

        public string? Alloty_Unique_Id { get; set; }
        public string? SAP_Id { get; set; }

        public decimal? TotalOffsetValue { get; set; }
        public decimal? EMD_Amount { get; set; }
        public DateTime? EMD_Receipt_Date { get; set; }
        public string? EMD_Receipt_No { get; set; }

        public string? Payment_Mode { get; set; }
        public int? NoOfInstallment { get; set; }

        public DateTime? Possession_Date { get; set; }
        public string? Possession_No { get; set; }

        public DateTime? First_Installment_Date { get; set; }
        public decimal? First_Installment_Amount { get; set; }
        public decimal? First_Installment_GST { get; set; }

        public decimal? Total_Installment_Amount_Due { get; set; }
        public decimal? Total_Installment_Gst_Due_Amount { get; set; }

        public decimal? Additional_Amount { get; set; }
        public decimal? Sum_Total_of_Installment { get; set; }

        public decimal? Total_Installment_Amount_Received { get; set; }
        public decimal? Total_Installment_Gst_Received_Amount { get; set; }
        public decimal? Additional_Received_Amount { get; set; }
        public decimal? Sum_Total_of_Received_Amount { get; set; }

        public decimal? Total_Surcharge_Amount_Due { get; set; }
        public decimal? Total_Surcharge_amount_on_GST { get; set; }

        public decimal? Discount_of_Surcharge_Amount { get; set; }
        public decimal? Discount_of_Gst_Amount { get; set; }

        public string? Remarks { get; set; }


     
    }



    //public class Old_Alloty_PaymentVM
    //{
    //    public Old_Alloty_Payment_Details Payment { get; set; }

    //    public string Alloty_Unique_Id { get; set; }
    //    public string SAP_Id { get; set; }

    //    // Dropdown
    //    public List<SelectListItem> PaymentModes { get; set; }
    //}
}
