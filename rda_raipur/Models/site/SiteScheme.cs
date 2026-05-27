using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models.site
{
    public class SiteScheme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SchemeName { get; set; }

        // इन तीनों में '?' लगा दिया ताकि पुराने रिकॉर्ड्स का NULL हैंडल हो सके
        public string? ImagePath { get; set; }
        public string? Location { get; set; }
        public string? MapLocationUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // 🔥 यहाँ भी '?' लगा दिया ताकि पुराने NULL रिकॉर्ड्स एरर न दें
        public bool? IsDeleted { get; set; } = false;
    }
}