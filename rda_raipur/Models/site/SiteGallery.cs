using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace rda_raipur.Models.site
{
    [Table("SiteGalleries")]
    public class SiteGallery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title_en { get; set; } = string.Empty;

        [Required]
        public string Title_hi { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? ImageUpload { get; set; }

        public int FolderId { get; set; }

        [ForeignKey("FolderId")]
        public virtual GalleryFolder? GalleryFolder { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Audit Fields (जो तुम्हें डेटाबेस में ऐड करने हैं)
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? created_by { get; set; }
        public DateTime? updated_Date { get; set; }
        public string? updated_by { get; set; }
    }
}