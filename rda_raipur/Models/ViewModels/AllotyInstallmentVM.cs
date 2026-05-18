namespace rda_raipur.Models.ViewModels
{
    public class AllotyInstallmentVM
    {
        public string Alloty_Unique_Id { get; set; }

        public DateTime? Last_Due_Installment_Date { get; set; }

        public decimal Total_Property_Cost { get; set; }
        public decimal GST_Percent_Property { get; set; }
        public decimal Total_Property_Cost_With_GST { get; set; }

        public decimal Total_Paid_Amount { get; set; }
        public decimal GST_Percent_Paid { get; set; }
        public decimal Total_Paid_Amount_With_GST { get; set; }

        public decimal Total_Due_Amount { get; set; }
        public decimal GST_Percent_Due { get; set; }
        public decimal Total_Due_Amount_With_GST { get; set; }

        public string Payment_Status { get; set; }
        public decimal Maintainance_Amount { get; set; }
        public string Paid_Yes_NO { get; set; }
    }
}
