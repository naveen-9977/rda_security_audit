using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    [Table("ip_ii_old_data")]
    public class IpIiOldData
    {
        [Key]
        public short House_No { get; set; } 

        public string Scheme_name_en { get; set; }
        public string scheme_sort_name_en { get; set; }
        public string sector_name_en { get; set; }
        public string block_name_en { get; set; }
        public string Flat_name_en { get; set; }
        public string Property_Type { get; set; }
        public string Flat_Type { get; set; }


        public decimal? Super_Buildup_Area { get; set; }
        public decimal? Buildup_Area { get; set; }
        public decimal? Carpet_Area { get; set; }
    }
}