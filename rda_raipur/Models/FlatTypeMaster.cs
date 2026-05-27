using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    [Table("Flat_Type_Master")]
    public class FlatTypeMaster
    {
        [Key]
        public int Flat_id { get; set; }

        [Required]
        public string Flat_name_en { get; set; }

        public string? Flat_name_hi { get; set; }

        public string? created_by { get; set; }

        public DateTime? Create_Date { get; set; }

        public DateTime? updated_Date { get; set; }

        public string? updated_by { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDeleted { get; set; }
    }
}