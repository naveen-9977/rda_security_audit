using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;

namespace rda_raipur.Controllers
{
    [Authorize]
    public class UserCategory_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Naya View Path ek variable mein rakh lete hain taaki baar-baar lamba path na likhna pade
        private readonly string viewPath = "~/Views/AdminDashboard/Master/UserCategory_Master/";

        public UserCategory_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserCategory_Master
        public async Task<IActionResult> Index()
        {
            var data = await _context.UserCategoryMasters
               .Where(x => x.IsDeleted == false)
               .ToListAsync();

            // Naya path yahan use ho raha hai
            return View(viewPath + "Index.cshtml", data);
        }

        // GET: UserCategory_Master/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCategory_Master = await _context.UserCategoryMasters
                .FirstOrDefaultAsync(m => m.res_category_id == id);
            if (userCategory_Master == null)
            {
                return NotFound();
            }

            return View(viewPath + "Details.cshtml", userCategory_Master);
        }

        // GET: UserCategory_Master/Create
        public IActionResult Create()
        {
            return View(viewPath + "Create.cshtml");
        }

        // POST: UserCategory_Master/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCategory_Master model)
        {
            if (ModelState.IsValid)
            {
                model.Create_Date = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(viewPath + "Create.cshtml", model);
        }

        // GET: UserCategory_Master/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCategory_Master = await _context.UserCategoryMasters.FindAsync(id);
            if (userCategory_Master == null)
            {
                return NotFound();
            }
            return View(viewPath + "Edit.cshtml", userCategory_Master);
        }

        // POST: UserCategory_Master/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("res_category_id,res_category_name_hi,res_category_name_en,created_by,Create_Date,updated_Date,updated_by,IsActive,IsDeleted")] UserCategory_Master userCategory_Master)
        {
            if (id != userCategory_Master.res_category_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userCategory_Master.updated_Date = DateTime.Now;
                    userCategory_Master.updated_by = User.Identity.Name ?? "Admin";

                    _context.Update(userCategory_Master);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserCategory_MasterExists(userCategory_Master.res_category_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Edit.cshtml", userCategory_Master);
        }

        // GET: UserCategory_Master/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCategory_Master = await _context.UserCategoryMasters
                .FirstOrDefaultAsync(m => m.res_category_id == id);
            if (userCategory_Master == null)
            {
                return NotFound();
            }

            return View(viewPath + "Delete.cshtml", userCategory_Master);
        }

        // POST: UserCategory_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userCategory_Master = await _context.UserCategoryMasters.FindAsync(id);
            if (userCategory_Master != null)
            {
                userCategory_Master.IsDeleted = true;
                userCategory_Master.updated_Date = DateTime.Now;
                userCategory_Master.updated_by = User.Identity.Name ?? "Admin";

                _context.Update(userCategory_Master);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserCategory_MasterExists(int id)
        {
            return _context.UserCategoryMasters.Any(e => e.res_category_id == id);
        }
    }
}