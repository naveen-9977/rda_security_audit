using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace rda_raipur.Models
{
    
    public class PropertieDetailsCalulation
    {
        public int Id { get; set; }
        public string? PropertyType { get; set; }

        // Change these to nullable (decimal?)
        public decimal? Length { get; set; }
        public decimal? Breadth { get; set; }
        public decimal? TotalSize { get; set; }
        public decimal? PropertyRate { get; set; }
        public decimal? SuperBuildupArea { get; set; }
        public decimal? BuildupArea { get; set; }
        public decimal? CarpetArea { get; set; }

        // Change these to nullable (int?)
        
        public int? Scheme_Id { get; set; }
        public int? Sector_Id { get; set; }
        public int? res_category_id { get; set; }
        public int? Block_Id { get; set; }
        public int? better_location_id { get; set; }
        public int? Model_Id { get; set; }
        public int? Flat_id { get; set; }

        // Change these to nullable (string?)
        public string? BetterLocationType { get; set; }
        public string? Direction { get; set; }
        public string? PropertyNumber { get; set; }
        public string? ConstructionStatus { get; set; }
        public string? Classification { get; set; }
        public string? MeasurmentType { get; set; }
        public string? Remark { get; set; }
        public string? short_name { get; set; }

        // Audit fields
        public DateTime? CreatDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }

        public string? IsActive { get; set; }
        public string? IsDeleted { get; set; }
         // Add this to PropertieDetailsCalulation.cs
       [Column("allotment_type_id")] // Tells EF to look for the exact DB column name
        public int? allotment_type_id { get; set; }

        // for Rawabhat Transport Shop Floor Type
        public string? Floor { get; set; }
    }
}

