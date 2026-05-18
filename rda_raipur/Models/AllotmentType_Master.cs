using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace rda_raipur.Models
{
    [Table("Allotement_type_Master")]
    public class AllotmentType_Master
    {
        [Key]
        public int allotement_type_id { get; set; }

        [Required]
        public string allotement_type_name_hi { get; set; }

        [Required]
        public string allotement_type_name_en { get; set; }

        public string created_by { get; set; }

        public DateTime? Create_Date { get; set; }

        public DateTime? updated_Date { get; set; }

        public string updated_by { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
