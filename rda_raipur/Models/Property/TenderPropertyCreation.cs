using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models.Property
{
    [Table("Tender_Property_Creattion")]
    public class TenderPropertyCreation
    {
        // 🔥 UPDATE 1: Purana Property_Id ab sirf 'Id' ban gaya hai
        [Key]
        public int Id { get; set; }

        // --- IDs ---
        public int? scheme_id { get; set; }
        public int? sector_id { get; set; }
        public int? block_id { get; set; }
        public int? Flat_id { get; set; }
        public int? res_category_id { get; set; }
        public int? allotement_type_id { get; set; }
        public int? Model_Id { get; set; }

        // 🔥 Isme '?' lagana zaroori hai taki DB ka NULL handle ho sake
        public string? Property_Classification { get; set; }

        // 🔥 NAYE FIELDS PLOT KE LIYE 🔥
        public decimal? width_plot { get; set; }
        public decimal? depth_plot { get; set; }
        public int? direction_id { get; set; }
        public int? better_location_id { get; set; }

        // --- Names (English & Hindi) ---
        public string? Model_name_en { get; set; }
        public string? House_No { get; set; }
        public string? res_category_name_en { get; set; }
        public string? res_category_name_hi { get; set; }
        public string? Scheme_name_en { get; set; }
        public string? Scheme_name_hi { get; set; }
        public string? scheme_sort_name_en { get; set; }
        public string? sector_name_en { get; set; }
        public string? sector_name_hi { get; set; }
        public string? block_name_en { get; set; }
        public string? block_name_hi { get; set; }
        public string? Flat_name_en { get; set; }
        public string? Flat_name_hi { get; set; }
        public string? allotement_type_name_en { get; set; }
        public string? Property_Type_Name_en { get; set; }
        public string? direction_name_en { get; set; }

        // 🔥 UPDATE 2: Purana Tender_Property_Id ab 'Property_Id' ban gaya hai 🔥
        public string? Property_Id { get; set; }

        // --- Measurements & Pricing ---
        public decimal? Super_Buildup_Area { get; set; }
        public decimal? Buildup_Area { get; set; }
        public decimal? Carpet_Area { get; set; }
        public decimal? offset_Price { get; set; }
        public decimal? EMD_Amount { get; set; }

        // --- Tracking & Status ---
        public string? created_by { get; set; }
        public DateTime? updated_Date { get; set; }
        public string? updated_by { get; set; }
        public DateTime? Create_Date { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int? No_of_Time_Appear { get; set; }
        public string? Booking_Status { get; set; }
        public bool IsAlloted { get; internal set; }
    }
}