// Path: Models/Document_Verification/DocumentVerificationLog.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models.Document_Verification
{
    public class DocumentVerificationLog
    {
        [Key]
        public int VerificationLogId { get; set; }

        [ForeignKey("PropertyBooking")]
        public int BookingId { get; set; }
        public virtual PropertyBooking? PropertyBooking { get; set; }

        public int ReviewerId { get; set; }
        public string? ReviewerRole { get; set; }
        public string? ActionTaken { get; set; }
        public string? Remarks { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.Now;
    }
}