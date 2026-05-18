using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace rda_raipur.Models.site // <-- Added .site here
{
    public class SiteCarousel
    {
        [Key]
        public int Id { get; set; }

        public string? Caption_en { get; set; }
        public string? Caption_hi { get; set; }

        public string? ImagePath { get; set; } // <-- Added ? to fix warning

        [NotMapped]
        public IFormFile? ImageUpload { get; set; }

        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}