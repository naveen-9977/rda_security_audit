using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rda_raipur.Models.Document_Verification;

namespace rda_raipur.Models
{
    public class PropertyBooking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("UserProfileDetail")]
        public int? UserProfileId { get; set; }
        public virtual UserProfileDetail? UserProfileDetail { get; set; }

        [ForeignKey("BookingProfileDetail")]
        public int BookingProfileId { get; set; }
        public virtual BookingProfileDetail? BookingProfileDetail { get; set; }

        [Required]
        public string? PropertyId { get; set; }

        public decimal? BidAmount { get; set; }

        // 🔥 NEW PAYMENT FIELDS FOR BIFURCATION 🔥
        public decimal? ApplicationFee { get; set; } // आवेदन शुल्क (Non-refundable)
        public decimal? EMDAmount { get; set; }      // अमानत राशि (Refundable/Adjustable)

        public DateTime BookingDate { get; set; } = DateTime.Now;

        // पेमेंट गेटवे और स्टेटस
        public string PaymentStatus { get; set; } = "Pending"; // Success/Failed
        public string? TransactionId { get; set; }              // गेटवे ट्रांजेक्शन ID

        public string BookingStatus { get; set; } = "Pending";

        // वेरिफिकेशन और ट्रैकिंग
        public string VerificationStatus { get; set; } = "Pending";
        public int? CurrentlyAssignedEmployeeId { get; set; }

        public virtual ICollection<DocumentVerificationLog> VerificationLogs { get; set; } = new List<DocumentVerificationLog>();

        public string? CancelRemarks { get; set; }
        public string? ApplicationNo { get; set; }

        // पेमेंट डिटेल्स का पूरा इतिहास (ICollection)
        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

        // टेंडर और अलॉटमेंट डेट्स
        public DateTime? TenderOpenDate { get; set; }
        public DateTime? AllotmentDate { get; set; }
    }
}