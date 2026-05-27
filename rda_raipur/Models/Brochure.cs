using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class Brochure
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "English Title is required")]
        public string Title_En { get; set; }

        [Required(ErrorMessage = "Hindi Title is required")]
        public string Title_Hi { get; set; }

        // Description ऐड कर दी ताकि एरर न आए
        public string? Description { get; set; }

        public string? ImagePath { get; set; }
        public string? PdfPath { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Audit Fields
        public string? created_by { get; set; }
        public string? updated_by { get; set; }
        public DateTime? updated_Date { get; set; }
    }
}