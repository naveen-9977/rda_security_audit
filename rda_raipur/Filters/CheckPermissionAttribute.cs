using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using rda_raipur.Data;
using System.Linq;
using System.Security.Claims;

namespace rda_raipur.Filters
{
    public class CheckPermissionAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly string _permissionRequired;

        // 🔥 NAYA CONSTRUCTOR: Ab ye parameter receive kar sakta hai
        public CheckPermissionAttribute(ApplicationDbContext context, string permissionRequired = "")
        {
            _context = context;
            _permissionRequired = permissionRequired;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            // 1. Agar user login nahi hai toh login page par bhejein
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // 2. Agar user Admin hai, toh use sab kuch allow kar do (Full Access)
            if (user.IsInRole("Admin"))
            {
                base.OnActionExecuting(context);
                return;
            }

            // 3. Employee ke liye Permission Check karein
            var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                // Current Controller aur Action ka naam nikaalein
                var controllerName = context.RouteData.Values["controller"].ToString();
                var actionName = context.RouteData.Values["action"].ToString().ToLower();

                // Check karein ki is Controller (Module) ke liye is user ko kya permission hai
                var permission = _context.EmployeePermissions
                    .FirstOrDefault(p => p.UserId == userId && p.AppModule.ControllerName == controllerName);

                bool isAuthorized = false;

                if (permission != null)
                {
                    // A) Agar Controller se specific permission bheji gayi hai (e.g., Arguments = new object[] { "CanAdd" })
                    if (!string.IsNullOrEmpty(_permissionRequired))
                    {
                        switch (_permissionRequired)
                        {
                            case "CanView": isAuthorized = permission.CanView; break;
                            case "CanAdd": isAuthorized = permission.CanAdd; break;
                            case "CanEdit": isAuthorized = permission.CanEdit; break;
                            case "CanDelete": isAuthorized = permission.CanDelete; break;
                        }
                    }
                    // B) Agar koi argument nahi bheja, toh apke purane logic (Action Name) se auto-detect karega
                    else
                    {
                        if (actionName == "index" || actionName == "details")
                            isAuthorized = permission.CanView;

                        else if (actionName == "create")
                            isAuthorized = permission.CanAdd;

                        else if (actionName == "edit")
                            isAuthorized = permission.CanEdit;

                        else if (actionName == "delete" || actionName == "deleteconfirmed")
                            isAuthorized = permission.CanDelete;
                    }
                }

                // 4. Agar permission nahi hai, toh Access Denied page par bhej dein
                if (!isAuthorized)
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}