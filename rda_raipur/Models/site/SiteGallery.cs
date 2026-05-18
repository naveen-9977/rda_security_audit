using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace rda_raipur.Models.site // <-- Added .site here
{
    public class SiteGallery
    {
        [Key]
        public int Id { get; set; }

        public string? Title_en { get; set; }
        public string? Title_hi { get; set; }

        public string? ImagePath { get; set; } // <-- Added ? to fix warning

        [NotMapped]
        public IFormFile? ImageUpload { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Inside your existing SiteGallery class, add these lines:
        [Required(ErrorMessage = "Please select a folder")]
        public int FolderId { get; set; }

        [ForeignKey("FolderId")]
        public virtual GalleryFolder? GalleryFolder { get; set; }
    }
}