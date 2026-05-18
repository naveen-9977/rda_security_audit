using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    public class PaymentDetail
    {
        [Key]
        public int PaymentId { get; set; }

        // ==========================================
        // USER RELATION (Naya Add Kiya Gaya)
        // ==========================================
        [ForeignKey("UserProfileDetail")]
        public int? UserProfileId { get; set; }
        public virtual UserProfileDetail? UserProfileDetail { get; set; }

        // ==========================================
        // BOOKING RELATION
        // ==========================================
        [ForeignKey("PropertyBooking")]
        public int BookingId { get; set; }
        public virtual rda_raipur.Models.PropertyBooking? PropertyBooking { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? TransactionId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string? OrderId { get; set; } // Gateway ki technical ID (ORD...) save karne ke liye

        // 🔥 YAHAN NAYA COLUMN ADD KIYA GAYA HAI (Bank UTR / RRN ke liye) 🔥
        public string? BankRefNo { get; set; }

        public string PaymentStatus { get; set; } = "Success";
    }
}