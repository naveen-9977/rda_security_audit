using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace rda_raipur.Models.site
{
    [Table("SiteNews")] // यह सुनिश्चित करता है कि EF Core सही टेबल से कनेक्ट हो
    public class SiteNews
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "English text is required")]
        public string NewsText_en { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hindi text is required")]
        public string NewsText_hi { get; set; } = string.Empty;

        public string? LinkUrl { get; set; }

        public string? PdfFilePath { get; set; }

        // यह डेटाबेस में सेव नहीं होगा, सिर्फ फाइल अपलोड के लिए है
        [NotMapped]
        public IFormFile? PdfFile { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Audit Fields (ये कंट्रोलर की लॉजिक के लिए ज़रूरी हैं)
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? created_by { get; set; }
        public DateTime? updated_Date { get; set; }
        public string? updated_by { get; set; }
    }
}