using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.DTOs;
using rda_raipur.Models;
using rda_raipur.Filters;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class EmployeeManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EmployeeManagementController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ==========================================
        // सहायक मेथड: मास्टर्स से ड्रॉपडाउन डेटा लोड करना
        // ==========================================
        private async Task PopulateMastersAsync()
        {
            var depts = await _context.Department_Masters.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
            var desigs = await _context.Designation_Masters.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
            var types = await _context.EmpType_Masters.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();

            ViewBag.DeptList = depts.Select(x => new {
                id = x.dpt_id,
                name = $"{x.dpt_name_en} ({x.dpt_name_hi})"
            }).ToList();

            ViewBag.DesigList = desigs.Select(x => new {
                id = x.designation_id,
                name = $"{x.designation_name_en} ({x.designation_name_hi})"
            }).ToList();

            ViewBag.TypeList = types.Select(x => new {
                id = x.emp_type_id,
                name = $"{x.emp_type_name_en} ({x.emp_type_name_hi})"
            }).ToList();
        }

        // ==========================================
        // 1. INDEX PAGE
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index(int? departmentId)
        {
            var departments = await _context.Department_Masters
                .Where(d => d.IsActive && !d.IsDeleted)
                .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = d.dpt_id.ToString(),
                    Text = d.dpt_name_en
                }).ToListAsync();

            ViewBag.Departments = departments;
            ViewBag.SelectedDepartment = departmentId;

            var designations = await _context.Designation_Masters
                .Where(d => d.IsActive && !d.IsDeleted)
                .ToDictionaryAsync(d => d.designation_id, d => d.designation_name_en);

            ViewBag.DesignationDict = designations;

            var query = _context.EmployeeDetails.Where(e => e.Status == 1);
            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(e => e.DptId == departmentId.Value);
            }

            var employees = await query.OrderByDescending(e => e.Created_At).ToListAsync();
            return View(employees);
        }

        // ==========================================
        // 2. DETAILS PAGE
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var employee = await _context.EmployeeDetails.FirstOrDefaultAsync(e => e.EmpId == id);
            if (employee == null || employee.Status == 0) return NotFound();
            return View(employee);
        }

        // ==========================================
        // 3. CREATE PAGE (GET)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateMastersAsync();
            return View();
        }

        // ==========================================
        // 4. CREATE ACTION (POST)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateMastersAsync();
                return View(dto);
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Mobile_One || u.MobileNo == dto.Mobile_One);

            if (existingUser != null)
            {
                TempData["Error"] = "Mobile number already exists.";
                await PopulateMastersAsync();
                return View(dto);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string? imagePath = null;
                if (dto.Image != null && dto.Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads/profiles");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.Image.CopyToAsync(fileStream);
                    }
                    imagePath = "/uploads/profiles/" + uniqueFileName;
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword("Employee@123");
                var newUser = new rda_raipur.Models.User
                {
                    Username = dto.Mobile_One,
                    MobileNo = dto.Mobile_One,
                    Email = dto.Email_Id,
                    PasswordHash = hashedPassword,
                    Role = "Employee",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var newEmployee = new EmployeeDetails
                {
                    Fname = dto.Fname,
                    Lname = dto.Lname,
                    Designation_Id = dto.Designation_Id,
                    Emp_Type_Id = dto.Emp_Type_Id,
                    DptId = dto.DptId,
                    Mobile_One = dto.Mobile_One,
                    Email_Id = dto.Email_Id,
                    Gender = dto.Gender,
                    Dob = dto.Dob,
                    Marital_Status = dto.Marital_Status,
                    Blood_Group = dto.Blood_Group,
                    District_Id = dto.District_Id,
                    State_Id = dto.State_Id,
                    Address = dto.Address,
                    Pr_DateOf_Joining = dto.Pr_DateOf_Joining,
                    Image = imagePath,
                    UserId = newUser.Id,
                    Status = 1,
                    Created_At = DateTime.Now,
                    Created_By = 1 // Default Admin
                };

                _context.EmployeeDetails.Add(newEmployee);
                await _context.SaveChangesAsync();

                newEmployee.Emp_Code = "RDA-EMP-" + newEmployee.EmpId.ToString("D3");
                _context.EmployeeDetails.Update(newEmployee);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                TempData["Success"] = $"Employee {newEmployee.Emp_Code} created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Error: " + ex.Message;
                await PopulateMastersAsync();
                return View(dto);
            }
        }

        // ==========================================
        // 5. EDIT PAGE (GET)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var employee = await _context.EmployeeDetails.FindAsync(id);
            if (employee == null || employee.Status == 0) return NotFound();
            await PopulateMastersAsync();
            return View(employee);
        }

        // ==========================================
        // 6. EDIT ACTION (POST) - 🔥 UPDATED FOR MOBILE CHANGE 🔥
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateEmployeeDto dto)
        {
            var emp = await _context.EmployeeDetails.FindAsync(id);
            if (emp == null) return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 🔥 logic: Agar Mobile Number badla gaya hai
                if (emp.Mobile_One != dto.Mobile_One)
                {
                    // 1. Check karein ki naya number kisi aur user ke paas to nahi hai
                    var duplicateUser = await _context.Users.AnyAsync(u => (u.Username == dto.Mobile_One || u.MobileNo == dto.Mobile_One) && u.Id != emp.UserId);
                    if (duplicateUser)
                    {
                        TempData["Error"] = "The new mobile number is already in use by another account.";
                        await PopulateMastersAsync();
                        return View(emp);
                    }

                    // 2. User table me login credentials (Username aur Mobile) update karein
                    var user = await _context.Users.FindAsync(emp.UserId);
                    if (user != null)
                    {
                        user.Username = dto.Mobile_One;
                        user.MobileNo = dto.Mobile_One;
                        _context.Users.Update(user);
                    }
                }

                // Image logic
                if (dto.Image != null && dto.Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads/profiles");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.Image.CopyToAsync(fileStream);
                    }
                    emp.Image = "/uploads/profiles/" + uniqueFileName;
                }

                // Update Employee Table
                emp.Fname = dto.Fname;
                emp.Lname = dto.Lname;
                emp.Designation_Id = dto.Designation_Id;
                emp.Emp_Type_Id = dto.Emp_Type_Id;
                emp.DptId = dto.DptId;
                emp.Mobile_One = dto.Mobile_One; // <--- Ab ye number update ho jayega
                emp.Email_Id = dto.Email_Id;
                emp.Gender = dto.Gender;
                emp.Dob = dto.Dob;
                emp.Marital_Status = dto.Marital_Status;
                emp.Blood_Group = dto.Blood_Group;
                emp.District_Id = dto.District_Id;
                emp.State_Id = dto.State_Id;
                emp.Address = dto.Address;
                emp.Pr_DateOf_Joining = dto.Pr_DateOf_Joining;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Employee record and login credentials updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Error updating employee: " + ex.Message;
                await PopulateMastersAsync();
                return View(emp);
            }
        }

        // ==========================================
        // 7. DELETE ACTION
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.EmployeeDetails.FindAsync(id);
            if (employee == null) return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                employee.Status = 0;
                var user = await _context.Users.FindAsync(employee.UserId);
                if (user != null) user.IsActive = false;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["Success"] = "Employee deleted successfully.";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "An error occurred while deleting.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}