using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    [Table("Block_Master")]
    public class Block_Master
    {
        [Key]
        public int block_id { get; set; }

        public int scheme_id { get; set; }

        public int sector_id { get; set; }

        [Required]
        public string block_name_hi { get; set; }

        [Required]
        public string block_name_en { get; set; }

        public string? created_by { get; set; } // Added ? to prevent ModelState errors

        public DateTime? Create_Date { get; set; }

        public DateTime? updated_Date { get; set; }

        public string? updated_by { get; set; } // Added ? to prevent ModelState errors

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        [ForeignKey("scheme_id")]
        public Scheme_Master? Scheme_Master { get; set; }

        [ForeignKey("sector_id")]
        public Sector_Master? Sector_Master { get; set; }
    }
}