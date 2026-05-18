using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Filters;
using rda_raipur.Models.PermissionModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace rda_raipur.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class PermissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PermissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================
        // 1. EMPLOYEES LIST (WITH DEPARTMENT FILTER & DATATABLES) 🔥
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Index(int? departmentId)
        {
            // Dropdown ke liye Department List load karna
            var departments = await _context.Department_Masters
                .Where(d => d.IsActive && !d.IsDeleted)
                .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = d.dpt_id.ToString(),
                    Text = d.dpt_name_en
                }).ToListAsync();

            ViewBag.Departments = departments;
            ViewBag.SelectedDepartment = departmentId;

            // Sirf Active Staff (Status == 1)
            var query = _context.EmployeeDetails.Where(e => e.Status == 1);

            // Agar Dropdown se Department select kiya gaya hai, toh filter karein
            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(e => e.DptId == departmentId.Value);
            }

            var employeesDb = await query.OrderByDescending(e => e.Created_At).ToListAsync();

            // Table me Designation ka naam dikhane ke liye Dictionary
            var designationsData = await _context.Designation_Masters
                .Where(d => d.IsActive && !d.IsDeleted)
                .ToListAsync();
            var designationDict = designationsData.ToDictionary(d => d.designation_id, d => d.designation_name_en);

            var employees = employeesDb.Select(e => new EmployeeListVM
            {
                UserId = e.UserId,
                EmpCode = e.Emp_Code ?? "N/A",
                FullName = e.Fname + " " + e.Lname,
                MobileNo = e.Mobile_One,
                Department = designationDict.ContainsKey(e.Designation_Id) ? designationDict[e.Designation_Id] : "N/A",
                ImagePath = e.Image
            }).ToList();

            return View(employees);
        }

        // =====================================
        // 2. MANAGE MODULE PERMISSIONS
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Manage(int userId)
        {
            var employee = await _context.EmployeeDetails.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction(nameof(Index));
            }

            // 🔥 NAYA LOGIC: Department aur Designation ka naam database se nikalna 🔥
            var department = await _context.Department_Masters.FirstOrDefaultAsync(d => d.dpt_id == employee.DptId);
            var designation = await _context.Designation_Masters.FirstOrDefaultAsync(d => d.designation_id == employee.Designation_Id);

            var model = new ManagePermissionVM
            {
                UserId = userId,
                EmployeeName = employee.Fname + " " + employee.Lname,

                // Naye fields assign kiye gaye taaki Dossier Summary mein dikh sake
                EmpCode = employee.Emp_Code ?? "N/A",
                ImagePath = employee.Image,
                DepartmentName = department?.dpt_name_en ?? "N/A",
                DesignationName = designation?.designation_name_en ?? "N/A",

                Modules = new List<ModulePermissionVM>()
            };

            // Database se saare active modules fetch karna
            var allModules = await _context.AppModules.Where(m => m.IsActive).ToListAsync();

            // Is specific user ke purane permissions fetch karna
            var userPermissions = await _context.EmployeePermissions.Where(p => p.UserId == userId).ToListAsync();

            foreach (var module in allModules)
            {
                var currentPerm = userPermissions.FirstOrDefault(p => p.ModuleId == module.ModuleId);

                model.Modules.Add(new ModulePermissionVM
                {
                    ModuleId = module.ModuleId,
                    ModuleName = module.ModuleName,
                    Category = string.IsNullOrEmpty(module.Category) ? "General System" : module.Category,
                    CanView = currentPerm?.CanView ?? false,
                    CanAdd = currentPerm?.CanAdd ?? false,
                    CanEdit = currentPerm?.CanEdit ?? false,
                    CanDelete = currentPerm?.CanDelete ?? false
                });
            }

            // Category aur Name ke hisaab se sorting
            model.Modules = model.Modules
                .OrderBy(m => m.Category)
                .ThenBy(m => m.ModuleName)
                .ToList();

            return View(model);
        }

        // =====================================
        // 3. SAVE PERMISSIONS
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePermissions(ManagePermissionVM model)
        {
            if (model == null || model.Modules == null)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var mod in model.Modules)
            {
                var existingPerm = await _context.EmployeePermissions
                    .FirstOrDefaultAsync(p => p.UserId == model.UserId && p.ModuleId == mod.ModuleId);

                if (existingPerm != null)
                {
                    // Update existing
                    existingPerm.CanView = mod.CanView;
                    existingPerm.CanAdd = mod.CanAdd;
                    existingPerm.CanEdit = mod.CanEdit;
                    existingPerm.CanDelete = mod.CanDelete;
                    _context.Update(existingPerm);
                }
                else
                {
                    // Create new if any permission is checked
                    if (mod.CanView || mod.CanAdd || mod.CanEdit || mod.CanDelete)
                    {
                        var newPerm = new EmployeePermission
                        {
                            UserId = model.UserId,
                            ModuleId = mod.ModuleId,
                            CanView = mod.CanView,
                            CanAdd = mod.CanAdd,
                            CanEdit = mod.CanEdit,
                            CanDelete = mod.CanDelete
                        };
                        _context.EmployeePermissions.Add(newPerm);
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Permissions updated successfully!";

            return RedirectToAction(nameof(Manage), new { userId = model.UserId });
        }
    }
}