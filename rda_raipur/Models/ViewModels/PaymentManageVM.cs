// using rda_raipur.Models.AllotyRegistration;  <-- Is line ko chahe toh delete kar sakte hain ab

namespace rda_raipur.Models.ViewModels
{
    public class PaymentManageVM
    {
        // Yahan par humne pura path de diya hai taaki namespace aur class ka confusion khatam ho jaye
        public rda_raipur.Models.AllotyRegistration.AllotyRegistration AlloteeInfo { get; set; }

        public AlloteePaymentDetail PaymentInfo { get; set; }
    }
}