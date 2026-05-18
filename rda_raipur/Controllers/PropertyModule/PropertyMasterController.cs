using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Filters;
using rda_raipur.Models.Property;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace rda_raipur.Controllers.PropertyModule
{
    // 🔥 PERMISSION SECURITY ADDED: Sirf Admin aur Employee allow honge
    [Authorize(Roles = "Admin,Employee")]
    public class PropertyMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyMasterController(ApplicationDbContext context)
        {
            _context = context;
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

            var permission = await _context.EmployeePermissions
                .Include(p => p.AppModule)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.AppModule.ControllerName == "PropertyMaster");

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

        // Helper Method to get Hardcoded Direction Names
        private string GetDirectionName(int? id)
        {
            return id switch
            {
                1 => "NORTH",
                2 => "SOUTH",
                3 => "EAST",
                4 => "WEST",
                5 => "NORTH-EAST",
                6 => "NORTH-WEST",
                7 => "SOUTH-EAST",
                8 => "SOUTH-WEST",
                _ => null
            };
        }

        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))]
        public async Task<IActionResult> SchemeDashboard()
        {
            var data = await _context.TenderProperties.Where(x => x.IsDeleted == false).ToListAsync();
            return View(data);
        }

        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))]
        public async Task<IActionResult> Index(string scheme)
        {
            var query = _context.TenderProperties.Where(x => x.IsDeleted == false);
            if (!string.IsNullOrEmpty(scheme))
            {
                query = scheme == "Unassigned" ? query.Where(x => string.IsNullOrEmpty(x.Scheme_name_en)) : query.Where(x => x.Scheme_name_en == scheme);
                ViewBag.CurrentScheme = scheme;
            }
            var data = await query.OrderByDescending(x => x.Create_Date).ToListAsync();
            return View(data);
        }

        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))]
        public IActionResult Create()
        {
            LoadDropdownsForCreate();
            return View();
        }

        private void LoadDropdownsForCreate(string selectedType = null)
        {
            ViewBag.Schemes = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en");
            ViewBag.AllotmentList = new SelectList(_context.AllotementTypeMasters.Where(x => x.IsDeleted == false), "allotement_type_id", "allotement_type_name_en");
            ViewBag.BetterLocations = new SelectList(_context.Better_Location, "better_location_id", "better_location_name_en");

            // Hardcoded Directions
            var hardcodedDirections = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "NORTH" },
                new SelectListItem { Value = "2", Text = "SOUTH" },
                new SelectListItem { Value = "3", Text = "EAST" },
                new SelectListItem { Value = "4", Text = "WEST" },
                new SelectListItem { Value = "5", Text = "NORTH-EAST" },
                new SelectListItem { Value = "6", Text = "NORTH-WEST" },
                new SelectListItem { Value = "7", Text = "SOUTH-EAST" },
                new SelectListItem { Value = "8", Text = "SOUTH-WEST" }
            };
            ViewBag.Directions = new SelectList(hardcodedDirections, "Value", "Text");

            var pTypes = _context.PropertyModelMasters
                .Where(x => x.IsDeleted == false && x.IsActive == true && !string.IsNullOrEmpty(x.Property_Type_Name_en))
                .Select(x => x.Property_Type_Name_en.Trim()).Distinct().ToList();
            ViewBag.PropertyTypes = new SelectList(pTypes, selectedType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))]
        public async Task<IActionResult> Create(TenderPropertyCreation model,
            List<string> HouseNumbers, List<int> CategoryIds,
            List<decimal?> Widths, List<decimal?> Depths,
            List<int?> DirectionIds, List<int?> BetterLocationIds,
            List<decimal?> SuperAreas, List<decimal?> BuildupAreas, List<decimal?> CarpetAreas,
            List<decimal?> OffsetPrices, List<decimal?> EmdAmounts)
        {
            ModelState.Remove("House_No");
            ModelState.Remove("Property_Id");
            ModelState.Remove("res_category_id");
            ModelState.Remove("width_plot");
            ModelState.Remove("depth_plot");
            ModelState.Remove("direction_id");
            ModelState.Remove("better_location_id");
            ModelState.Remove("Super_Buildup_Area");
            ModelState.Remove("offset_Price");
            ModelState.Remove("EMD_Amount");

            if (ModelState.IsValid && HouseNumbers != null && HouseNumbers.Count > 0)
            {
                HouseNumbers = HouseNumbers.Select(h => h?.Trim()).ToList();

                var duplicateQuery = _context.TenderProperties.Where(p => p.sector_id == model.sector_id && p.IsDeleted == false);

                if (model.block_id.HasValue) duplicateQuery = duplicateQuery.Where(p => p.block_id == model.block_id);
                if (model.Model_Id.HasValue) duplicateQuery = duplicateQuery.Where(p => p.Model_Id == model.Model_Id);
                if (model.Flat_id.HasValue) duplicateQuery = duplicateQuery.Where(p => p.Flat_id == model.Flat_id);

                var existingHouses = await duplicateQuery.Select(p => p.House_No).ToListAsync();
                var dbDuplicates = HouseNumbers.Where(h => !string.IsNullOrWhiteSpace(h)).Intersect(existingHouses, StringComparer.OrdinalIgnoreCase).ToList();

                if (dbDuplicates.Any())
                {
                    TempData["Error"] = $"Duplicate Found! Unit No(s): {string.Join(", ", dbDuplicates)} already exist.";
                    LoadDropdownsForCreate(model.Property_Type_Name_en);
                    return View(model);
                }

                var scheme = await _context.Scheme_Master.FirstOrDefaultAsync(x => x.scheme_id == model.scheme_id);
                var sector = await _context.Sector_Master.FirstOrDefaultAsync(x => x.sector_id == model.sector_id);

                var block = model.block_id.HasValue ? await _context.Block_Masters.FirstOrDefaultAsync(x => x.block_id == model.block_id) : null;
                var flat = model.Flat_id.HasValue ? await _context.FlatTypeMasters.FirstOrDefaultAsync(x => x.Flat_id == model.Flat_id) : null;
                var modelType = model.Model_Id.HasValue ? await _context.PropertyModelTypeMasters.FirstOrDefaultAsync(x => x.Model_Id == model.Model_Id) : null;
                var allotment = await _context.AllotementTypeMasters.FirstOrDefaultAsync(x => x.allotement_type_id == model.allotement_type_id);

                var allCategories = await _context.UserCategoryMasters.ToListAsync();

                for (int i = 0; i < HouseNumbers.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(HouseNumbers[i])) continue;

                    string hNo = HouseNumbers[i];

                    int catId = CategoryIds.Count > i ? CategoryIds[i] : 0;
                    var resCategory = allCategories.FirstOrDefault(x => x.res_category_id == catId);

                    int? dirId = DirectionIds.Count > i ? DirectionIds[i] : null;

                    string dirNameEn = GetDirectionName(dirId);

                    var newProperty = new TenderPropertyCreation
                    {
                        scheme_id = model.scheme_id,
                        sector_id = model.sector_id,
                        block_id = model.block_id,
                        Model_Id = model.Model_Id,
                        Flat_id = model.Flat_id,
                        Property_Type_Name_en = model.Property_Type_Name_en,
                        Property_Classification = model.Property_Classification,
                        allotement_type_id = model.allotement_type_id,

                        House_No = hNo,
                        res_category_id = catId,
                        res_category_name_en = resCategory?.res_category_name_en,
                        res_category_name_hi = resCategory?.res_category_name_hi,
                        width_plot = Widths.Count > i ? Widths[i] : null,
                        depth_plot = Depths.Count > i ? Depths[i] : null,
                        direction_id = dirId,
                        direction_name_en = dirNameEn,
                        better_location_id = BetterLocationIds.Count > i ? BetterLocationIds[i] : null,
                        Super_Buildup_Area = SuperAreas.Count > i ? SuperAreas[i] : null,
                        Buildup_Area = BuildupAreas.Count > i ? BuildupAreas[i] : null,
                        Carpet_Area = CarpetAreas.Count > i ? CarpetAreas[i] : null,
                        offset_Price = OffsetPrices.Count > i ? OffsetPrices[i] : null,
                        EMD_Amount = EmdAmounts.Count > i ? EmdAmounts[i] : null,

                        Scheme_name_en = scheme?.scheme_name_en,
                        Scheme_name_hi = scheme?.scheme_name_hi,
                        scheme_sort_name_en = scheme?.scheme_sort_name_en,
                        sector_name_en = sector?.sector_name_en,
                        sector_name_hi = sector?.sector_name_hi,
                        block_name_en = block?.block_name_en,
                        block_name_hi = block?.block_name_hi,
                        Flat_name_en = flat?.Flat_name_en,
                        Flat_name_hi = flat?.Flat_name_hi,
                        allotement_type_name_en = allotment?.allotement_type_name_en,
                        Model_name_en = modelType?.Model_name_en,

                        No_of_Time_Appear = 1,
                        Booking_Status = "Vacant",
                        IsActive = true,
                        IsDeleted = false,
                        Create_Date = DateTime.Now,
                        created_by = "Admin",
                        updated_by = "Admin"
                    };

                    string classCode = !string.IsNullOrEmpty(newProperty.Property_Classification)
                                       ? newProperty.Property_Classification.Substring(0, 3).ToUpper()
                                       : "";

                    var parts = new System.Collections.Generic.List<string>
                    {
                        newProperty.scheme_sort_name_en,
                        newProperty.sector_name_en,
                        newProperty.block_name_en,
                        classCode,
                        newProperty.Property_Type_Name_en,
                        newProperty.Model_name_en,
                        newProperty.Flat_name_en,
                        newProperty.House_No
                    };

                    newProperty.Property_Id = string.Join("-", parts.Where(p => !string.IsNullOrWhiteSpace(p))).ToUpper();

                    _context.Add(newProperty);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"{HouseNumbers.Count(h => !string.IsNullOrWhiteSpace(h))} entries processed successfully!";
                return RedirectToAction(nameof(SchemeDashboard));
            }

            LoadDropdownsForCreate(model.Property_Type_Name_en);
            return View(model);
        }

        // --- AJAX Helpers (Inpe permission zaruri nahi hoti form load hone ke baad) ---
        [HttpGet]
        public async Task<JsonResult> GetUserCategory()
        {
            var categories = await _context.UserCategoryMasters
                .Where(x => x.IsDeleted == false && x.IsActive == true)
                .Select(x => new { id = x.res_category_id, name = x.res_category_name_en }).ToListAsync();
            return Json(categories);
        }

        [HttpGet]
        public async Task<IActionResult> CheckDuplicate(int sectorId, string houseNo, int? blockId, int? modelId, int? flatId)
        {
            var query = _context.TenderProperties.Where(p => p.sector_id == sectorId && p.House_No == houseNo && p.IsDeleted == false);

            if (blockId.HasValue) query = query.Where(p => p.block_id == blockId);
            if (modelId.HasValue) query = query.Where(p => p.Model_Id == modelId);
            if (flatId.HasValue) query = query.Where(p => p.Flat_id == flatId);

            bool isDuplicate = await query.AnyAsync();
            return Json(!isDuplicate);
        }

        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var property = await _context.TenderProperties.FindAsync(id);
            if (property == null) return NotFound();

            ViewBag.Schemes = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en", property.scheme_id);
            ViewBag.AllotmentList = new SelectList(_context.AllotementTypeMasters.Where(x => x.IsDeleted == false), "allotement_type_id", "allotement_type_name_en", property.allotement_type_id);
            ViewBag.BetterLocations = new SelectList(_context.Better_Location, "better_location_id", "better_location_name_en", property.better_location_id);

            var hardcodedDirections = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "NORTH" },
                new SelectListItem { Value = "2", Text = "SOUTH" },
                new SelectListItem { Value = "3", Text = "EAST" },
                new SelectListItem { Value = "4", Text = "WEST" },
                new SelectListItem { Value = "5", Text = "NORTH-EAST" },
                new SelectListItem { Value = "6", Text = "NORTH-WEST" },
                new SelectListItem { Value = "7", Text = "SOUTH-EAST" },
                new SelectListItem { Value = "8", Text = "SOUTH-WEST" }
            };
            ViewBag.Directions = new SelectList(hardcodedDirections, "Value", "Text", property.direction_id);

            var pTypes = _context.PropertyModelMasters
                .Where(x => x.IsDeleted == false && x.IsActive == true && !string.IsNullOrEmpty(x.Property_Type_Name_en))
                .Select(x => x.Property_Type_Name_en.Trim()).Distinct().ToList();
            ViewBag.PropertyTypes = new SelectList(pTypes, property.Property_Type_Name_en);

            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))]
        public async Task<IActionResult> Edit(int id, TenderPropertyCreation model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.TenderProperties.FirstOrDefaultAsync(x => x.Id == id);
                    if (existing == null) return NotFound();

                    existing.House_No = model.House_No;

                    existing.res_category_id = model.res_category_id;
                    if (model.res_category_id != null)
                    {
                        var resCategory = await _context.UserCategoryMasters.FirstOrDefaultAsync(x => x.res_category_id == model.res_category_id);
                        existing.res_category_name_en = resCategory?.res_category_name_en;
                        existing.res_category_name_hi = resCategory?.res_category_name_hi;
                    }

                    existing.offset_Price = model.offset_Price;
                    existing.EMD_Amount = model.EMD_Amount;
                    existing.Super_Buildup_Area = model.Super_Buildup_Area;
                    existing.Buildup_Area = model.Buildup_Area;
                    existing.Carpet_Area = model.Carpet_Area;

                    existing.Property_Type_Name_en = model.Property_Type_Name_en;
                    existing.Property_Classification = model.Property_Classification;

                    existing.width_plot = model.width_plot;
                    existing.depth_plot = model.depth_plot;
                    existing.better_location_id = model.better_location_id;
                    existing.direction_id = model.direction_id;

                    existing.direction_name_en = GetDirectionName(model.direction_id);

                    existing.updated_by = "Admin";
                    existing.updated_Date = DateTime.Now;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TenderPropertyExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Index", new { scheme = model.Scheme_name_en });
            }
            return View(model);
        }

        private bool TenderPropertyExists(int id)
        {
            return _context.TenderProperties.Any(e => e.Id == id);
        }

        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var property = await _context.TenderProperties.FirstOrDefaultAsync(m => m.Id == id);
            if (property == null) return NotFound();
            return View(property);
        }

        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var property = await _context.TenderProperties.FirstOrDefaultAsync(m => m.Id == id);
            if (property == null) return NotFound();
            return View(property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.TenderProperties.FirstOrDefaultAsync(x => x.Id == id);
            if (property != null)
            {
                property.IsDeleted = true;
                property.updated_Date = DateTime.Now;
                property.updated_by = "Admin";
                _context.Update(property);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { scheme = property.Scheme_name_en });
            }
            return RedirectToAction(nameof(SchemeDashboard));
        }

        // --- Dropdown API Helpers ---
        [HttpGet]
        public JsonResult GetSectors(int schemeId)
        {
            return Json(_context.Sector_Master.Where(x => x.scheme_id == schemeId && x.IsDeleted == false).Select(x => new { x.sector_id, x.sector_name_en }).ToList());
        }

        [HttpGet]
        public JsonResult GetBlocks(int sectorId)
        {
            return Json(_context.Block_Masters.Where(x => x.sector_id == sectorId && x.IsDeleted == false).Select(x => new { x.block_id, x.block_name_en }).ToList());
        }

        [HttpGet]
        public JsonResult GetFlats()
        {
            return Json(_context.FlatTypeMasters.Where(x => x.IsDeleted == false).Select(x => new { x.Flat_id, x.Flat_name_en }).ToList());
        }

        [HttpGet]
        public async Task<JsonResult> GetModels()
        {
            return Json(await _context.PropertyModelTypeMasters.Where(x => x.IsDeleted == false).Select(x => new { id = x.Model_Id, name = x.Model_name_en }).ToListAsync());
        }

        [HttpGet]
        public JsonResult GeneratePropertyCode(int schemeId, int? sectorId, int? blockId, int? flatId, int? modelId, string houseNo, string classification)
        {
            var scheme = _context.Scheme_Master.FirstOrDefault(x => x.scheme_id == schemeId);
            var sector = _context.Sector_Master.FirstOrDefault(x => x.sector_id == sectorId);
            var block = _context.Block_Masters.FirstOrDefault(x => x.block_id == blockId);
            var flat = _context.FlatTypeMasters.FirstOrDefault(x => x.Flat_id == flatId);
            var modelType = _context.PropertyModelTypeMasters.FirstOrDefault(x => x.Model_Id == modelId);

            string classCode = !string.IsNullOrEmpty(classification) ? classification.Substring(0, 3).ToUpper() : "";

            var parts = new System.Collections.Generic.List<string>
            {
                scheme?.scheme_sort_name_en,
                sector?.sector_name_en,
                block?.block_name_en,
                classCode,
                modelType?.Model_name_en,
                flat?.Flat_name_en,
                houseNo
            };

            string code = string.Join("-", parts.Where(p => !string.IsNullOrWhiteSpace(p))).ToUpper();
            return Json(code);
        }
    }
}