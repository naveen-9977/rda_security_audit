using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models.PermissionModel
{
    public class AppModule
    {
        [Key]
        public int ModuleId { get; set; }

        [Required(ErrorMessage = "Module Name is required")]
        public string ModuleName { get; set; }

        [Required(ErrorMessage = "Controller Name is required")]
        public string ControllerName { get; set; }

        // ? lagane se NULL handle ho jayega
        public string? Category { get; set; }

        public bool IsActive { get; set; } = true;
    }
}