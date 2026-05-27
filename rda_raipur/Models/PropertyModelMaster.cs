using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    [Table("Property_Model_Master")]
    public class PropertyModelMaster
    {
        [Key]
        public int Property_Id { get; set; }

        // Foreign Key IDs
        public int scheme_id { get; set; }
        public int sector_id { get; set; }
        public int block_id { get; set; }

        // Property Names
        public string Property_Type_Name_en { get; set; }
        public string Property_Type_Name_hi { get; set; }

        // Audit Fields (🔥 Nullable string / DateTime to prevent mapping crash)
        public string? created_by { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? updated_Date { get; set; }
        public string? updated_by { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        // Navigation Properties

        [ForeignKey("scheme_id")]
        public virtual Scheme_Master? Scheme_Master { get; set; }

        [ForeignKey("sector_id")]
        public virtual Sector_Master? Sector_Master { get; set; }

        [ForeignKey("block_id")]
        public virtual Block_Master? Block_Master { get; set; }
    }
}