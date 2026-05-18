using System;
using System.Collections.Generic;

namespace rda_raipur.Models.ViewModels
{
    public class TenderLiveCreateVM
    {
        // 🔍 SEARCH & FILTER FIELDS
        public int? SearchSchemeId { get; set; }
        public int? SearchSectorId { get; set; }
        public string? SearchPropertyType { get; set; } // e.g., Flat, Plot, Shop
        public string? BookingStatus { get; set; }       // e.g., "Vacant"

        // 📅 LIVE GLOBAL SETTINGS (Used for Step 2 Save process)
        public DateTime? TenderStartDate { get; set; }
        public DateTime? TenderEndDate { get; set; }
        public DateTime? TenderOpeningDate { get; set; }
        public string? Status { get; set; } // Active / Inactive

        // 📋 RESULT LIST
        public List<TenderPropertyItemVM> Properties { get; set; } = new List<TenderPropertyItemVM>();
    }

    public class TenderPropertyItemVM
    {
        public int PropertyId { get; set; }
        public string Tender_Property_Id { get; set; }
        public string Scheme_Name { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string Property_Number { get; set; }
        public string UserCategory { get; set; }
        public decimal Offset_Price { get; set; }
        public decimal EMD_Amount { get; set; }
        public string Property_Model { get; set; }
        public string Property_Type { get; set; }
        public string Flat_Type { get; set; }
        public string Allotment_Type { get; set; }

        public string Property_Classification { get; set; }

        // 🔥 ADD THESE FIELDS NOW 🔥
        public string Property_Size { get; set; } // Super Buildup
        public string Buildup_Area { get; set; }
        public string Carpet_Area { get; set; }

        // 🔥 NAYE FIELDS PLOT/SHOP KE LIYE (Dimensions & Location) 🔥
        public string? Width { get; set; }
        public string? Depth { get; set; }
        public string? Direction { get; set; }
        public int? BetterLocationId { get; set; }
        public string? BetterLocationName { get; set; }

        public string PropertyImage { get; set; }
        public string PropertyBrochure { get; set; }
        public string PropertyLayout { get; set; }
        public DateTime TenderEndDate { get; set; }
        public DateTime TenderOpeningDate { get; set; }
        public bool IsSelected { get; set; } // For checkbox
    }
}