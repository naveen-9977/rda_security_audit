using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models.site
{
    public class GalleryFolder
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Folder Name is required")]
        [Display(Name = "Folder Name")]
        public string FolderName { get; set; }

        public string? CoverImagePath { get; set; }

        [NotMapped]
        public IFormFile? CoverImageUpload { get; set; }

        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}