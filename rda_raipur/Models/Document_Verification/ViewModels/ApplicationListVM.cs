using rda_raipur.Models; // PropertyBooking ke liye

// 🔥 Dhyan dein: Namespace mein naye folder ka naam hona chahiye
namespace rda_raipur.Models.Document_Verification.ViewModels
{
    public class ApplicationListVM
    {
        public PropertyBooking Booking { get; set; }
        public string SectorName { get; set; }
    }
}