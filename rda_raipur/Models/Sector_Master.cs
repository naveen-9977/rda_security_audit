using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    public class Sector_Master
    {
        [Key]
        public int sector_id { get; set; }

        [Required]
        [Display(Name = "Scheme")]
        public int scheme_id { get; set; }

        [Required]
        [StringLength(200)]
        public string sector_name_en { get; set; } = string.Empty;

        [StringLength(200)]
        public string sector_name_hi { get; set; } = string.Empty;

        public string? created_by { get; set; }
        public DateTime? Create_Date { get; set; }

        public DateTime? updated_Date { get; set; }
        public string? updated_by { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // 🔥 Navigation Property
        [ForeignKey("scheme_id")]
        public Scheme_Master? Scheme_Master { get; set; }

    }
}
