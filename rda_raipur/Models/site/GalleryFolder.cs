using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace rda_raipur.Models.site
{
    [Table("GalleryFolders")] // यह सुनिश्चित करता है कि EF Core सही टेबल से कनेक्ट हो
    public class GalleryFolder
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Folder Name is required")]
        [StringLength(250)]
        [Display(Name = "Folder Name")]
        public string FolderName { get; set; } = string.Empty;

        public string? CoverImagePath { get; set; }

        // डेटाबेस में सेव नहीं होगा, सिर्फ फाइल अपलोड के लिए है
        [NotMapped]
        public IFormFile? CoverImageUpload { get; set; }

        // Status & Audit Fields
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? created_by { get; set; }

        public DateTime? updated_Date { get; set; }
        public string? updated_by { get; set; }
    }
}