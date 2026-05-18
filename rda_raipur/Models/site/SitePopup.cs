using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace rda_raipur.Models.site
{
    public class SitePopup
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100)]
        public string Title { get; set; }

        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? ImageUpload { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}