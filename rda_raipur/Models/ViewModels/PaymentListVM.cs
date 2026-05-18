namespace rda_raipur.Models.ViewModels
{
    public class PaymentListVM
    {
        public int Id { get; set; }
        public string Alloty_Unique_Id { get; set; }
        public string SAP_Id { get; set; } // 🔥 Naya add kiya
        public string Alloty_Name { get; set; }
        public string Mobile { get; set; }
        public string PropertyInfo { get; set; }

        public decimal TotalWaterChargePaid { get; set; }
        public decimal TotalWaterChargeOutstanding { get; set; }
        public decimal TotalSurchargePaid { get; set; }
        public decimal TotalSurchargeRemaining { get; set; }
    }
}