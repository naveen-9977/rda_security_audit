using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models.site
{
    public class SiteContact
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Office Name (English) is required")]
        [MaxLength(200)]
        [Display(Name = "Office Name (English)")]
        public string OfficeName { get; set; }

        [Required(ErrorMessage = "Office Name (Hindi) is required")]
        [MaxLength(200)]
        [Display(Name = "Office Name (Hindi)")]
        public string OfficeNameHi { get; set; }

        [Required(ErrorMessage = "Address (English) is required")]
        [MaxLength(500)]
        [Display(Name = "Address (English)")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Address (Hindi) is required")]
        [MaxLength(500)]
        [Display(Name = "Address (Hindi)")]
        public string AddressHi { get; set; }

        [MaxLength(50)]
        public string? Phone1 { get; set; }

        [MaxLength(50)]
        public string? Phone2 { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Display(Name = "Google Maps Embed URL")]
        [MaxLength(2000)]
        public string? MapEmbedUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // 🔥 ADDED FIELDS (Do not remove these)
        public bool IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}