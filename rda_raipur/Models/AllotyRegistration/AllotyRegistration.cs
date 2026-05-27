using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models.AllotyRegistration
{
    [Table("Alloty_Registration")]
    public class AllotyRegistration
    {
        public int Id { get; set; }

        public string Alloty_Unique_Id { get; set; }

        public string? SAP_Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Alloty_Name { get; set; }
        [Required(ErrorMessage = "Father/Husband Name is required")]
        public string Father_Husband_Name { get; set; }
        public int? Age { get; set; }
        public DateTime? AllotyDOB { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }
        [Required(ErrorMessage = "Alloty Address is required")]

        public string Alloty_Address { get; set; } // Current Address
        public string? Permanent_Address { get; set; } // Permanent Address

        [Required(ErrorMessage = "User Category is required")]
        public int? res_category_id { get; set; }   // for dropdown value (ID)
        public string? res_category_name_en { get; set; } // optional (store name)

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter valid 10-digit mobile number")]
        public string? Mobile { get; set; }

        public string? Scheme { get; set; }
        public string? Sector { get; set; }
        public string? Block { get; set; }

        [Required(ErrorMessage = "Scheme is required")]
        public int? SchemeId { get; set; }

        [Required(ErrorMessage = "Sector is required")]
        public int? SectorId { get; set; }
        [Required(ErrorMessage = "Block is required")]
        public int? BlockId { get; set; }

        [Required(ErrorMessage = "Property No is required")]
        public string? Property_No { get; set; }

        [Required(ErrorMessage = "Plot Size is required")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Enter valid plot size")]
        public string? PlotSize { get; set; }
        public decimal? CarpetArea { get; set; }

        public string Allotment_Yes_No { get; set; }
        public string Allotment_No { get; set; }
        public DateTime? Allotment_Date { get; set; }
        [Required(ErrorMessage = "Allotment Type is required")]
        public string Allotment_Type { get; set; }

        public string Property_Registry { get; set; }
        public DateTime? Registry_date { get; set; }
        public string? Registry_No { get; set; }

        public string? Sale_No { get; set; }
        public DateTime? Sale_Date { get; set; }

        public string? Possession_No { get; set; }
        public DateTime? Possession_Date { get; set; }

        public string? NOC_Yes_No { get; set; }
        public string? NOC_No { get; set; }
        public DateTime? NOC_Date { get; set; }

        public DateTime? Agreement_Date_From { get; set; }
        public DateTime? Agreement_Date_To { get; set; }

        [Required(ErrorMessage = "Property Cost is required")]
        [Range(0, 999999999, ErrorMessage = "Enter valid amount")]
        public decimal? Property_Cost { get; set; }

        public string? BatterLocation { get; set; }
        public int? better_location_id { get; set; }
        public decimal? BatterLocation_Charges { get; set; }

        public int Property_id { get; set; }

        public string? Property_Type_Name_en { get; set; }

        public int? Running_No { get; set; }

        public string? Flat_Type { get; set; }
        public string? Flat_Name { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string? Registration_Status { get; set; }

        // 🔥 BAS YE EK LINE ADD KARNI HAI SOFT DELETE KE LIYE
        public bool IsDeleted { get; set; } = false;
    }
}