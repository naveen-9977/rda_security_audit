using System.Collections.Generic;

namespace rda_raipur.Models.PermissionModel
{
    // ==========================================
    // 1. Index Page (Employees List) ke liye ViewModel
    // ==========================================
    public class EmployeeListVM
    {
        public int UserId { get; set; }
        public string EmpCode { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public string Department { get; set; }
        public string ImagePath { get; set; }
    }

    // ==========================================
    // 2. Manage Permissions Page ke liye ViewModel
    // ==========================================
    public class ManagePermissionVM
    {
        public int UserId { get; set; }
        public string EmployeeName { get; set; }

        // 🔥 ये रहीं वो 4 नई लाइनें जो मैंने ऐड कर दी हैं 🔥
        public string EmpCode { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string ImagePath { get; set; }

        public List<ModulePermissionVM> Modules { get; set; } = new List<ModulePermissionVM>();
    }

    // ==========================================
    // 3. Module Permissions ki list ke liye
    // ==========================================
    public class ModulePermissionVM
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }

        public string Category { get; set; } // Parent-Child / Grouping ke liye

        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}