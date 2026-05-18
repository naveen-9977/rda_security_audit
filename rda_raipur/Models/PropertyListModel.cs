using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // 🔥 YEH LINE ZAROORI HAI

namespace rda_raipur.Models
{
    // 🔥 YAHAN APNI SQL TABLE KA ASLI NAAM BATA DIJIYE (Bina 's' ke ya jo bhi DB me hai)
    [Table("PropertyListModel")]
    public class PropertyListModel
    {
        [Key]
        public int PropertyId { get; set; }

        public string? Tender_Property_Id { get; set; }

        public bool IsSelected { get; set; }

        public string Scheme_Name { get; set; }
        public required string Property_Type { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string Property_Number { get; set; }

        public string UserCategory { get; set; }

        public decimal Offset_Price { get; set; }
        public decimal EMD_Amount { get; set; }

        public string Property_Model { get; set; }
        public string Property_Size { get; set; }

        public string Property_Status { get; set; }
    }
}