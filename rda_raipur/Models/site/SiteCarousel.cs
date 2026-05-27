using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace rda_raipur.Models.site
{
    public class SiteCarousel
    {
        [Key]
        public int Id { get; set; }

        public string? Caption_en { get; set; }
        public string? Caption_hi { get; set; }

        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? ImageUpload { get; set; }

        public int DisplayOrder { get; set; } = 1;
        public string? Url { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Audit Fields
        public string? created_by { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? updated_by { get; set; }
        public DateTime? updated_Date { get; set; }
    }
}