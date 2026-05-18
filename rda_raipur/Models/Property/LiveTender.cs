using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models.Property
{
    // 1. MASTER TABLE (Sirf ek baar Dates aur Files yahan save hongi)
    public class Tender_Live_Master
    {
        [Key]
        public int MasterId { get; set; }
        public string Scheme_Name { get; set; } // Kis scheme ka master hai

        public DateTime Tender_Start_Date { get; set; }
        public DateTime Tender_End_Date { get; set; }
        public DateTime Tender_Opening_Date { get; set; }
        public string Status { get; set; } // Active / Inactive

        public string? CoverImage { get; set; }
        public string? BrochurePdf { get; set; }
        public string? LayoutPdf { get; set; }

        public DateTime Created_Date { get; set; }

        // Navigation (Is master me kitni properties hain)
        public List<Tender_Live_Mapping> LiveProperties { get; set; } = new();
    }

    // 2. MAPPING TABLE (Yahan pata chalega ki is Master me kaun-kaun si property judi hai)
    public class Tender_Live_Mapping
    {
        [Key]
        public int MappingId { get; set; }

        // Link to Master
        public int MasterId { get; set; }
        [ForeignKey("MasterId")]
        public Tender_Live_Master TenderLiveMaster { get; set; }

        // Link to Property Inventory
        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public TenderPropertyCreation Property { get; set; }
    }
}