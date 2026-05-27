using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{


    [Table("OldAllotyInstallmentMaster")]
    public class OldAllotyInstallmentMaster
    {

        public int Id { get; set; }

        public string Alloty_Unique_Id { get; set; }

        public decimal Sale_Value { get; set; }

        public decimal Total_Installment_Amount { get; set; }

        public decimal Total_Paid_Amount { get; set; }

        public decimal Total_Due_Amount { get; set; }

        public decimal Total_Interest { get; set; }

        public decimal Total_Maintenance { get; set; }
        public DateTime Created_Date { get; set; } = DateTime.Now;

        public List<OldAllotyInstallmentDetail> Details { get; set; }

        public decimal Total_Property_Cost { get; set; }


        public decimal Interest_Waiver { get; set; }

        public decimal GST_On_Interest { get; set; }

        public decimal GST_Waiver { get; set; }

        public decimal Final_Payable { get; set; }

        public decimal? Basic_Property_Price { get; set; }
        public decimal? Final_Property_Cost { get; set; }
        public decimal? Additional_Property_Cost { get; set; }
    }
    [Table("OldAllotyInstallmentDetail")] 
    public class OldAllotyInstallmentDetail
    {
        public int Id { get; set; }

        public int MasterId { get; set; }

        public string Installment_No { get; set; }

        public DateTime? Due_Date { get; set; }

        public decimal Installment_Amount { get; set; }

        public decimal GST_Percent { get; set; }

        public decimal GST_Amount { get; set; }

        public decimal Total_Amount { get; set; }

        public decimal Paid_Amount { get; set; }

        public DateTime? Paid_Date { get; set; }

        public decimal Interest_Amount { get; set; }

        public string Paid_Status { get; set; } // Yes/No

        // Maintainance COLUMNS
        public decimal? Maint_Charge { get; set; }
        public decimal? Maint_GST_Percent { get; set; }
        public decimal? Maint_GST_Amount { get; set; }

        public string Installment_Type { get; set; }

        [Column("Is_Excess")]
        public bool Is_Excess { get; set; }

       

        public OldAllotyInstallmentMaster Master { get; set; }

      
    }
}
