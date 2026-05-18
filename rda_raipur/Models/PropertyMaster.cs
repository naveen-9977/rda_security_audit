using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    [Table("Properties")] // 🔥 Your table name (change if different)
    public class PropertyMaster
    {
        [Key]
        public int Id { get; set; }

        public string? Scheme_Name { get; set; }

        public string? Scheme_Short_Name { get; set; }

        public string? Sector { get; set; }

        public string? Block { get; set; }

        public string? Flat_Type { get; set; }

        public string? Property_Type { get; set; }

        public string? Flat_Name { get; set; }

        public string? Property_No { get; set; }


        // Database me ye columns NULL allow karte hain, isliye hum 'decimal?' (nullable decimal) use karenge.

        public decimal? Super_Buildup_Area { get; set; }
        public decimal? Buildup_Area { get; set; }
        public decimal? Carpet_Area { get; set; }
    }
}
