using Microsoft.AspNetCore.Mvc.Rendering;

namespace rda_raipur.Models.ViewModels
{
    public class PropertySearchVM
    {
        public List<PropertyCardVM> Properties { get; set; }
      
        // Filters
        public string PropertyType { get; set; }
        public int? SchemeId { get; set; }
        
        public string? Scheme_name_en { get; set; }
        public string Configuration { get; set; }
        public string Category { get; set; }
        public decimal? Budget { get; set; }
        public string PropertyNumber { get; set; }


        // Dropdown Data
        public List<SelectListItem> PropertyTypes { get; set; }
        public List<SelectListItem> Schemes { get; set; }
        public List<SelectListItem> Configurations { get; set; }

        public List<CategoryCountVM> CategoryCounts { get; set; }
    }

    public class PropertyCardVM
    {
        public int PropertyId { get; set; }
        public int SchemeId { get; set; }
        public string SchemeName { get; set; }
        public string PropertyType { get; set; }
        public string Category { get; set; }
        
        public string Allotment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string PropertyImage { get; set; }
        public string FlatName { get; set; }

        public string PropertyBrochure { get; set; }
        public string PropertyLayout { get; set; }

        public string PropertyNumber { get; set; }

        public string Tender_Property_Id { get; set; }

    }

    public class CategoryCountVM
    {
        public string CategoryName { get; set; }
        public int TotalFlats { get; set; }
    }

}
