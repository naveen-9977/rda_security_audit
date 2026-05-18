using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Filters;
using rda_raipur.Models.Property;
using rda_raipur.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace rda_raipur.Controllers.PropertyModule
{
    // 🔥 PERMISSION SECURITY ADDED: Sirf Admin aur Employee allow honge
    [Authorize(Roles = "Admin,Employee")]
    [Route("TenderLive")]
    public class TenderLiveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TenderLiveController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ========================================================
        // 🛡️ PERMISSION HELPER FUNCTION
        // ========================================================
        private async Task<bool> HasPermission(string actionType)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin") return true;

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return false;

            // Database me ControllerName 'TenderLive' check hoga
            var permission = await _context.EmployeePermissions
                .Include(p => p.AppModule)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.AppModule.ControllerName == "TenderLive");

            if (permission == null) return false;

            return actionType switch
            {
                "View" => permission.CanView == true,
                "Add" => permission.CanAdd == true,
                "Edit" => permission.CanEdit == true,
                "Delete" => permission.CanDelete == true,
                _ => false
            };
        }

        // ==========================================
        // 📊 1. LIVE DASHBOARD (List of Master Tenders)
        // ==========================================
        [HttpGet]
        [Route("LiveDashboard")]
        [Route("")]
        [Route("Index")]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> LiveDashboard()
        {
            var masters = await _context.Tender_Live_Masters
                            .Include(m => m.LiveProperties)
                            .OrderByDescending(m => m.Created_Date)
                            .ToListAsync();
            return View(masters);
        }

        // ==========================================
        // ⚙️ 2. MANAGE MASTER (Edit Details & Manage Mapping)
        // ==========================================
        [HttpGet]
        [Route("Manage/{masterId}")]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Manage(int masterId)
        {
            var master = await _context.Tender_Live_Masters
                            .Include(m => m.LiveProperties)
                                .ThenInclude(mapping => mapping.Property)
                            .FirstOrDefaultAsync(m => m.MasterId == masterId);

            if (master == null) return RedirectToAction("LiveDashboard");

            return View(master);
        }

        // ==========================================
        // 💾 3. UPDATE MASTER (Updates Dates & Documents)
        // ==========================================
        [HttpPost]
        [Route("UpdateMaster")]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> UpdateMaster(int MasterId, Tender_Live_Master model, IFormFile? TenderImage, IFormFile? TenderBrochure, IFormFile? TenderLayout)
        {
            var existing = await _context.Tender_Live_Masters.FindAsync(MasterId);
            if (existing == null) return NotFound();

            existing.Tender_Start_Date = model.Tender_Start_Date;
            existing.Tender_End_Date = model.Tender_End_Date;
            existing.Tender_Opening_Date = model.Tender_Opening_Date;
            existing.Status = model.Status;

            if (TenderImage != null) existing.CoverImage = await SaveFile(TenderImage, "images");
            if (TenderBrochure != null) existing.BrochurePdf = await SaveFile(TenderBrochure, "brochures");
            if (TenderLayout != null) existing.LayoutPdf = await SaveFile(TenderLayout, "layouts");

            _context.Tender_Live_Masters.Update(existing);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Master Tender configuration updated successfully.";
            return RedirectToAction("Manage", new { masterId = MasterId });
        }

        // ==========================================
        // ➕ 4. ADD PROPERTY TO MASTER (Mapping)
        // ==========================================
        [HttpPost]
        [Route("AddPropertyToMaster")]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> AddPropertyToMaster(int masterId, int propertyId)
        {
            var exists = await _context.Tender_Live_Mappings
                                .AnyAsync(x => x.MasterId == masterId && x.PropertyId == propertyId);

            if (!exists)
            {
                var newMapping = new Tender_Live_Mapping
                {
                    MasterId = masterId,
                    PropertyId = propertyId
                };
                _context.Tender_Live_Mappings.Add(newMapping);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Property added to this Live Master.";
            }

            return RedirectToAction("Manage", new { masterId = masterId });
        }

        // ==========================================
        // 🗑️ 5. REMOVE PROPERTY FROM MASTER (Soft Remove)
        // ==========================================
        [HttpPost]
        [Route("RemoveProperty")]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> RemoveProperty(int mappingId, int masterId)
        {
            var mapping = await _context.Tender_Live_Mappings.FindAsync(mappingId);
            if (mapping != null)
            {
                _context.Tender_Live_Mappings.Remove(mapping);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Property removed from Live listing.";
            }
            return RedirectToAction("Manage", new { masterId = masterId });
        }

        // ==========================================
        // ➕ 6. CREATE: Search & Initial Batch Publish
        // ==========================================
        [HttpGet]
        [Route("Create")]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public IActionResult Create()
        {
            ViewBag.Schemes = new SelectList(_context.Scheme_Master.Where(x => !x.IsDeleted), "scheme_id", "scheme_name_en");
            return View(new TenderLiveCreateVM());
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Create(TenderLiveCreateVM vm, string submitType, IFormFile? TenderImage, IFormFile? TenderBrochure, IFormFile? TenderLayout)
        {
            if (submitType == "Filter")
            {
                var query = _context.TenderProperties.Where(x => x.IsDeleted == false || x.IsDeleted == null);
                if (vm.SearchSchemeId.HasValue) query = query.Where(x => x.scheme_id == vm.SearchSchemeId);
                if (vm.SearchSectorId.HasValue) query = query.Where(x => x.sector_id == vm.SearchSectorId);
                if (!string.IsNullOrEmpty(vm.BookingStatus)) query = query.Where(x => x.Booking_Status == vm.BookingStatus);

                vm.Properties = await query.Select(x => new TenderPropertyItemVM
                {
                    PropertyId = x.Id,
                    Tender_Property_Id = x.Property_Id,
                    Scheme_Name = x.Scheme_name_en,
                    Sector = x.sector_name_en,
                    Block = x.block_name_en,
                    Property_Number = x.House_No,
                    UserCategory = x.res_category_name_en,
                    Offset_Price = x.offset_Price ?? 0,
                    Property_Model = x.Model_name_en,
                    Property_Size = x.Super_Buildup_Area.ToString(),
                    Buildup_Area = x.Buildup_Area.ToString(),
                    Carpet_Area = x.Carpet_Area.ToString()
                }).ToListAsync();

                ViewBag.Schemes = new SelectList(_context.Scheme_Master.Where(x => !x.IsDeleted), "scheme_id", "scheme_name_en");
                return View(vm);
            }

            if (submitType == "Save")
            {
                var selected = vm.Properties.Where(x => x.IsSelected).ToList();
                if (!selected.Any()) { TempData["Error"] = "Please select properties."; return RedirectToAction("Create"); }
                if (!vm.TenderStartDate.HasValue || !vm.TenderEndDate.HasValue) { TempData["Error"] = "Dates required."; return RedirectToAction("Create"); }

                // A. Create one Master Record
                var newMaster = new Tender_Live_Master
                {
                    Scheme_Name = selected.First().Scheme_Name ?? "Unassigned",
                    Tender_Start_Date = vm.TenderStartDate.Value,
                    Tender_End_Date = vm.TenderEndDate.Value,
                    Tender_Opening_Date = vm.TenderOpeningDate ?? DateTime.Now,
                    Status = vm.Status ?? "Active",
                    CoverImage = await SaveFile(TenderImage, "images"),
                    BrochurePdf = await SaveFile(TenderBrochure, "brochures"),
                    LayoutPdf = await SaveFile(TenderLayout, "layouts"),
                    Created_Date = DateTime.Now
                };
                _context.Tender_Live_Masters.Add(newMaster);
                await _context.SaveChangesAsync();

                // B. Map selected properties to Master
                foreach (var item in selected)
                {
                    _context.Tender_Live_Mappings.Add(new Tender_Live_Mapping
                    {
                        MasterId = newMaster.MasterId,
                        PropertyId = item.PropertyId
                    });
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Master Tender & Properties published successfully.";
                return RedirectToAction("LiveDashboard");
            }

            return View(vm);
        }

        // ==========================================
        // ⚡ 7. AJAX HELPERS (For Modals & Cascading)
        // (Inko ServiceFilter ki zarurat nahi kyunki ye View load hone ke baad call hote hain)
        // ==========================================
        [HttpGet]
        [Route("GetVacantPropertiesForScheme")]
        public async Task<JsonResult> GetVacantPropertiesForScheme(string scheme)
        {
            var alreadyMappedIds = await _context.Tender_Live_Mappings
                                         .Select(x => x.PropertyId)
                                         .ToListAsync();

            var vacantProps = await _context.TenderProperties
                .Where(x => x.Scheme_name_en == scheme
                            && x.Booking_Status == "Vacant"
                            && x.IsDeleted == false
                            && !alreadyMappedIds.Contains(x.Id))
                .Select(x => new {
                    id = x.Id,
                    text = $"Sec: {x.sector_name_en} | Blk: {x.block_name_en} | No: {x.House_No} ({x.Property_Id})"
                }).ToListAsync();

            return Json(vacantProps);
        }

        [HttpGet]
        [Route("GetSectors")]
        public JsonResult GetSectors(int schemeId)
        {
            return Json(_context.Sector_Master.Where(x => x.scheme_id == schemeId && !x.IsDeleted).Select(x => new { x.sector_id, x.sector_name_en }).ToList());
        }

        private async Task<string?> SaveFile(IFormFile? file, string subFolder)
        {
            if (file == null || file.Length == 0) return null;
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create)) { await file.CopyToAsync(stream); }
            return $"/uploads/{subFolder}/{fileName}";
        }
    }
}