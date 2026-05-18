using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers.PropertyModule
{
    public class PropertyBookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyBookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // 🚀 1. SCHEMES
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Schemes(int? SearchSchemeId, decimal? budget, string? propertyType)
        {
            var today = DateTime.Now;

            var masterQuery = _context.Tender_Live_Masters
                .Include(m => m.LiveProperties)
                    .ThenInclude(map => map.Property)
                .Where(m => m.Status == "Active"
                       && m.Tender_Start_Date <= today
                       && m.Tender_End_Date >= today)
                .AsQueryable();

            var activeMasters = await masterQuery.ToListAsync();

            var schemeSummary = activeMasters.SelectMany(m => m.LiveProperties
                .Where(p => p.Property.Booking_Status == "Vacant")
                .GroupBy(p => p.Property.Property_Type_Name_en)
                .Select(g => new
                {
                    SchemeId = g.FirstOrDefault()?.Property.scheme_id ?? 0,
                    SchemeName = m.Scheme_Name,
                    PropertyType = g.Key,
                    TotalAssets = g.Count(),
                    MinPrice = g.Any() ? g.Min(p => p.Property.offset_Price ?? 0) : 0,
                    StartDate = m.Tender_Start_Date,
                    EndDate = m.Tender_End_Date,
                    OpeningDate = m.Tender_Opening_Date,
                    SchemeImage = m.CoverImage ?? "/uploads/images/No-image1.png",
                    Brochure = m.BrochurePdf,
                    Layout = m.LayoutPdf
                })
            ).ToList();

            if (SearchSchemeId.HasValue)
                schemeSummary = schemeSummary.Where(x => x.SchemeId == SearchSchemeId).ToList();

            if (!string.IsNullOrEmpty(propertyType))
                schemeSummary = schemeSummary.Where(x => x.PropertyType == propertyType).ToList();

            if (budget.HasValue && budget > 0)
                schemeSummary = schemeSummary.Where(x => x.MinPrice <= budget).ToList();

            ViewBag.SchemeSummary = schemeSummary;
            ViewBag.SchemeList = await _context.Scheme_Master.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.SelectedBudget = budget ?? 0;
            ViewBag.SelectedPropertyType = propertyType;

            return View(new TenderLiveCreateVM { SearchSchemeId = SearchSchemeId });
        }

        // =========================================================
        // 📋 2. UNITS: Fetching assets by Scheme & Type
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Units(string schemeName, string propertyType)
        {
            if (string.IsNullOrEmpty(schemeName)) return RedirectToAction(nameof(Schemes));

            var today = DateTime.Now;

            var query = _context.Tender_Live_Mappings
                .Include(x => x.TenderLiveMaster)
                .Include(x => x.Property)
                .Where(x => x.TenderLiveMaster.Scheme_Name == schemeName
                       && x.TenderLiveMaster.Status == "Active"
                       && x.TenderLiveMaster.Tender_Start_Date <= today
                       && x.TenderLiveMaster.Tender_End_Date >= today
                       && x.Property.Booking_Status == "Vacant")
                .AsQueryable();

            if (!string.IsNullOrEmpty(propertyType))
            {
                query = query.Where(x => x.Property.Property_Type_Name_en == propertyType);
            }

            var units = await query.Select(x => new TenderPropertyItemVM
            {
                PropertyId = x.MappingId,
                Tender_Property_Id = x.Property.Property_Id,
                Scheme_Name = x.Property.Scheme_name_en,
                Sector = x.Property.sector_name_en,
                Block = x.Property.block_name_en,
                Property_Number = x.Property.House_No,

                Property_Classification = x.Property.Property_Classification ?? "Residential",

                Flat_Type = x.Property.Flat_name_en,
                UserCategory = x.Property.res_category_name_en,
                Allotment_Type = x.Property.allotement_type_name_en,
                Property_Type = x.Property.Property_Type_Name_en,
                Offset_Price = x.Property.offset_Price ?? 0,
                EMD_Amount = x.Property.EMD_Amount ?? 0,
                Property_Size = x.Property.Super_Buildup_Area != null ? x.Property.Super_Buildup_Area.ToString() : "0",
                PropertyBrochure = x.TenderLiveMaster.BrochurePdf,
                PropertyLayout = x.TenderLiveMaster.LayoutPdf,
                TenderEndDate = x.TenderLiveMaster.Tender_End_Date,
                TenderOpeningDate = x.TenderLiveMaster.Tender_Opening_Date,

                // 🔥 NAYE UPDATE: PLOT DIMENSIONS & LOCATION MAP KIYE GAYE 🔥
                Width = x.Property.width_plot != null ? x.Property.width_plot.ToString() : "NA",
                Depth = x.Property.depth_plot != null ? x.Property.depth_plot.ToString() : "NA",
                Direction = !string.IsNullOrEmpty(x.Property.direction_name_en) ? x.Property.direction_name_en : "NA",
                BetterLocationId = x.Property.better_location_id

            }).ToListAsync();

            // 🔥 BETTER LOCATION NAME FETCH KARNE KA LOGIC 🔥
            var betterLocations = await _context.Better_Location.ToListAsync();
            foreach (var u in units)
            {
                if (u.BetterLocationId != null && u.BetterLocationId > 0)
                {
                    var loc = betterLocations.FirstOrDefault(b => b.better_location_id == u.BetterLocationId);
                    u.BetterLocationName = loc != null && !string.IsNullOrEmpty(loc.better_location_name_en) ? loc.better_location_name_en : "NA";
                }
                else
                {
                    u.BetterLocationName = "NA";
                }
            }

            if (units.Any())
            {
                ViewBag.MinPrice = units.Min(x => x.Offset_Price);
                var firstAsset = units.First();
                ViewBag.PropertyType = firstAsset.Property_Type;
                ViewBag.SectorLocation = firstAsset.Sector;
                ViewBag.EndDate = firstAsset.TenderEndDate;
                ViewBag.OfferDate = firstAsset.TenderOpeningDate;
                ViewBag.SchemeBrochure = firstAsset.PropertyBrochure;
                ViewBag.SchemeLayout = firstAsset.PropertyLayout;
            }
            else
            {
                ViewBag.MinPrice = 0;
            }

            ViewBag.SelectedScheme = schemeName;
            ViewBag.SelectedPropertyType = propertyType;
            return View(units);
        }

        [HttpPost]
        public IActionResult InitiateBooking(int mappingId)
        {
            TempData["BookingMappingId"] = mappingId;
            return RedirectToAction("Form", "ApplicationBooking");
        }
    }
}