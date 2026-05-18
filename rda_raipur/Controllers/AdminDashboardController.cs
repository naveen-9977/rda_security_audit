using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace rda_raipur.Controllers
{
    // 🔥 UPDATE: Ab 'Admin' aur 'Employee' dono is dashboard ko access kar sakte hain.
    // (Baki pages ki security humara naya CheckPermissionAttribute handle karega)
    [Authorize(Roles = "Admin,Employee")]
    public class AdminDashboardController : Controller
    {
        // Main Admin Dashboard Overview
        public IActionResult Index()
        {
            ViewBag.Message = "Welcome to the Secure Admin Dashboard!";
            return View();
        }

        // Secure route for Employee Management
        // URL: /AdminDashboard/Employees
        public IActionResult Employees()
        {
            // Extra safety check: redirect to login if session or identity is lost
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // Explicitly returns the view from your specific folder structure
            return View("~/Views/Admin/EmployeeDetails/Index.cshtml");
        }
    }
}