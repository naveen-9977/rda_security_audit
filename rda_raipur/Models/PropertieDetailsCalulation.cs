using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace rda_raipur.Models
{
    //public class PropertieDetailsCalulation
    //{
    //    public int Id { get; set; }
    //    [Required(ErrorMessage = "Scheme is required")]
    //    public int? Scheme_Id { get; set; }
    //    [Required(ErrorMessage = "Sector is required")]
    //    public int? Sector_Id { get; set; }
    //    public int? Block_Id { get; set; }
    //    [Required(ErrorMessage = "Property Type is required")]
    //    public string PropertyType { get; set; }

    //    [Column("better_location_id")]
    //    [Required(ErrorMessage = "Better Location is required")]
    //    public int? BetterLocationId { get; set; }

    //    [Required(ErrorMessage = "Direction is required")]
    //    public string Direction { get; set; }
    //    [Required(ErrorMessage = "Length is required")]
    //    [Column(TypeName = "decimal(18, 2)")]
    //    public decimal? Length { get; set; }
    //    [Required(ErrorMessage = "Breadth is required")]
    //    [Column(TypeName = "decimal(18, 2)")]
    //    public decimal? Breadth { get; set; }
    //    [Required(ErrorMessage = "TotalSize is required")]

    //    [Column(TypeName = "decimal(18, 2)")]
    //    public decimal? SuperBuildupArea { get; set; }
    //    [Column(TypeName = "decimal(18, 2)")]
    //    public decimal? BuildupArea { get; set; }
    //    [Column(TypeName = "decimal(18, 2)")]
    //    public decimal? CarpetArea { get; set; }
    //    [Column(TypeName = "decimal(18, 2)")]
    //    public decimal? TotalSize { get; set; }
    //    [Required(ErrorMessage = "Property Number is required")]
    //    //public string PropertyNumber { get; set; }



    //    [Remote(action: "IsPropertyNumberAvailable",
    //    controller: "PropertieDetailsCalculation",
    //    AdditionalFields = "Scheme_Id,Sector_Id",
    //    ErrorMessage = "This Property Number already exists in the selected Scheme/Sector.")]
    //    public string PropertyNumber { get; set; }

    //    [Required(ErrorMessage = "Property Classification is required")]
    //    public string Classification { get; set; }

    //    [Required(ErrorMessage = "Property Rate is required")]
    //    [Column(TypeName = "decimal(18, 2)")]
    //    public decimal? PropertyRate { get; set; }
    //    [Required(ErrorMessage = "Measurement Type is required")]
    //    public string MeasurmentType { get; set; }
    //    public string Remark { get; set; }

    //    [Column("CreatDate")]
    //    public DateTime? CreatDate { get; set; }

    //    public string? CreatedBy { get; set; }
    //    public DateTime? UpdateDate { get; set; }
    //    public string? UpdateBy { get; set; }

    //    public string? IsActive { get; set; }
    //    public string? IsDeleted { get; set; }

    //    public string? ConstructionStatus { get; set; }


    //    [Required(ErrorMessage = "Model Type is required")]
    //    public int? Model_Id { get; set; } // Matches your DB column

    //    public int? Flat_Id { get; set; }


    //}



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
    }
}

