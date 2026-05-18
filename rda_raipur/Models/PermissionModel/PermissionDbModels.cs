using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models.PermissionModel
{
    // Yahan se AppModule HATA diya gaya hai kyunki wo pehle se AppModule.cs me hai.
   
    public class EmployeePermission
    {
        [Key]
        public int PermissionId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("AppModule")]
        public int ModuleId { get; set; }
        public virtual AppModule AppModule { get; set; }

        public bool CanView { get; set; } = false;
        public bool CanAdd { get; set; } = false;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        
        
    }

}