using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    [Table("Master_Direction")]
    public class MasterDirection
    {
        [Key]
        public int direction_id { get; set; }
        public string? direction_name_en { get; set; }
        public string? direction_name_hi { get; set; }
        public string? direction_code { get; set; } //
        public bool IsDeleted { get; set; }
    }
}