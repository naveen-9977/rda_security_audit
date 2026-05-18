using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models.ViewModels;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfileController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==========================================
        // 1. VIEW PROFILE PAGE
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var employee = await _context.EmployeeDetails.FirstOrDefaultAsync(e => e.UserId == userId);
            var model = new ProfileVM();

            if (employee != null)
            {
                var dept = await _context.Department_Masters.FirstOrDefaultAsync(d => d.dpt_id == employee.DptId);
                var desig = await _context.Designation_Masters.FirstOrDefaultAsync(d => d.designation_id == employee.Designation_Id);

                // Read-Only Data (Safe String Conversion)
                model.EmpCode = employee.Emp_Code?.ToString() ?? "N/A";
                model.MobileNo = employee.Mobile_One?.ToString() ?? "N/A";
                model.Email = employee.Email_Id?.ToString() ?? "N/A";
                model.DepartmentName = dept?.dpt_name_en?.ToString() ?? "Not Assigned";
                model.DesignationName = desig?.designation_name_en?.ToString() ?? "Not Assigned";
                model.ImagePath = employee.Image?.ToString();

                // Editable Data
                model.Fname = employee.Fname?.ToString();
                model.Lname = employee.Lname?.ToString();
                model.Gender = employee.Gender?.ToString();
                model.Dob = employee.Dob;
                model.MaritalStatus = employee.Marital_Status?.ToString();
                model.BloodGroup = employee.Blood_Group?.ToString();
                model.Address = employee.Address?.ToString();
            }
            else
            {
                // For Admin User
                var user = await _context.Users.FindAsync(userId);
                model.Fname = "System";
                model.Lname = "Administrator";
                model.EmpCode = "ADMIN";
                model.MobileNo = user?.MobileNo?.ToString() ?? "N/A";
                model.Email = user?.Email?.ToString() ?? "N/A";
                model.DepartmentName = "System";
                model.DesignationName = "Admin";
            }

            ViewBag.ActiveTab = TempData["ActiveTab"]?.ToString() ?? "profile";
            return View(model);
        }

        // ==========================================
        // 2. UPDATE PROFILE LOGIC (SECURED)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileVM model)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var employee = await _context.EmployeeDetails.FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee != null)
            {
                // 🔥 Image Upload Logic (Allowed)
                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfileImage.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(fileStream);
                    }

                    if (!string.IsNullOrEmpty(employee.Image))
                    {
                        string oldPath = Path.Combine(_env.WebRootPath, employee.Image.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    employee.Image = "/uploads/profiles/" + uniqueFileName;
                }

                // 🔥 Update Only Editable Personal Fields 🔥
                employee.Dob = model.Dob;
                employee.Address = model.Address;
                employee.Gender = model.Gender;
                employee.Blood_Group = model.BloodGroup;

                // Safe conversion for Marital Status
                if (int.TryParse(model.MaritalStatus, out int maritalId))
                {
                    employee.Marital_Status = maritalId;
                }

                // ⚠️ Fname, Lname, Mobile, Email, Dept, Desig are intentionally NOT updated here.

                _context.Update(employee);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Personal details and profile picture updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Admin profile details cannot be updated from here.";
            }

            TempData["ActiveTab"] = "profile";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 3. CHANGE PASSWORD LOGIC
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ProfileVM model)
        {
            TempData["ActiveTab"] = "password";

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                TempData["ErrorMessage"] = "All password fields are required.";
                return RedirectToAction(nameof(Index));
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "New password and Confirm password do not match.";
                return RedirectToAction(nameof(Index));
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash);
            if (!isPasswordValid)
            {
                TempData["ErrorMessage"] = "Incorrect Current Password. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your password has been changed successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}