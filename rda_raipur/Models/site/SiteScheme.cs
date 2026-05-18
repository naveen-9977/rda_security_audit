using System;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models.site
{
    public class SiteScheme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SchemeName { get; set; }

        public string ImagePath { get; set; }

        public string Location { get; set; }

        public string MapLocationUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; } 
        public string? UpdatedBy { get; set; } 
        public DateTime? UpdatedDate { get; set; } 
    }
}