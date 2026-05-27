using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class Scheme_Master
    {
        [Key]   
        public int scheme_id { get; set; }

        [Required]
        [StringLength(200)]

        public string scheme_name_en { get; set; }

        [Required]
        public string scheme_name_hi { get; set; }

        [Required]
        public string scheme_sort_name_en { get; set; }

        [Required]
        public string scheme_sort_name_hi { get; set; }

        [Required]
        public string scheme_rera_no { get; set; }
        public string? created_by { get; set; }
        public DateTime? create_date { get; set; }
        public DateTime? updated_date { get; set; }

        
        public string? updated_by { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
