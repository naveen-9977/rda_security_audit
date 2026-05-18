using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Added for IFormFile

namespace rda_raipur.Models.site
{
    public class SiteNews
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "English text is required")]
        public string NewsText_en { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hindi text is required")]
        public string NewsText_hi { get; set; } = string.Empty;

        public string? LinkUrl { get; set; }

        // PDF Path stored in database
        public string? PdfFilePath { get; set; }

        // Not mapped to DB, used only for receiving the uploaded file from form
        [NotMapped]
        public IFormFile? PdfFile { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}