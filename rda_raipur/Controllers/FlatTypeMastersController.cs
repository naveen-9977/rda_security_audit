using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // <-- Added Authorization
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;

namespace rda_raipur.Controllers
{
    [Authorize] // <-- Added to secure the page
    public class FlatTypeMastersController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Naya View Path set kar diya
        private readonly string viewPath = "~/Views/AdminDashboard/Master/FlatTypeMasters/";

        public FlatTypeMastersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var data = await _context.FlatTypeMasters
                        .Where(x => x.IsDeleted == false)
                        .ToListAsync();

            // Explicit view path
            return View(viewPath + "Index.cshtml", data);
        }

        // GET: FlatTypeMasters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flatTypeMaster = await _context.FlatTypeMasters
                .FirstOrDefaultAsync(m => m.Flat_id == id);
            if (flatTypeMaster == null)
            {
                return NotFound();
            }

            return View(viewPath + "Details.cshtml", flatTypeMaster);
        }

        // GET: FlatTypeMasters/Create
        public IActionResult Create()
        {
            return View(viewPath + "Create.cshtml");
        }

        // POST: FlatTypeMasters/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // <-- Security ke liye add kiya
        public async Task<IActionResult> Create(FlatTypeMaster model)
        {
            if (ModelState.IsValid)
            {
                model.Create_Date = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsActive = true;
                model.IsDeleted = false;

                _context.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(viewPath + "Create.cshtml", model);
        }

        // GET: FlatTypeMasters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flatTypeMaster = await _context.FlatTypeMasters.FindAsync(id);
            if (flatTypeMaster == null)
            {
                return NotFound();
            }
            return View(viewPath + "Edit.cshtml", flatTypeMaster);
        }

        // POST: FlatTypeMasters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FlatTypeMaster model)
        {
            if (ModelState.IsValid)
            {
                model.updated_Date = DateTime.Now;
                model.updated_by = User.Identity.Name ?? "Admin";

                _context.Update(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(viewPath + "Edit.cshtml", model);
        }

        // GET: FlatTypeMasters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flatTypeMaster = await _context.FlatTypeMasters
                .FirstOrDefaultAsync(m => m.Flat_id == id);
            if (flatTypeMaster == null)
            {
                return NotFound();
            }

            return View(viewPath + "Delete.cshtml", flatTypeMaster);
        }

        // POST: FlatTypeMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flatTypeMaster = await _context.FlatTypeMasters.FindAsync(id);
            if (flatTypeMaster != null)
            {
                // Hard delete (_context.Remove) ko hata kar Soft delete lagaya hai
                flatTypeMaster.IsDeleted = true;
                flatTypeMaster.updated_Date = DateTime.Now;
                flatTypeMaster.updated_by = User.Identity.Name ?? "Admin";

                _context.Update(flatTypeMaster);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlatTypeMasterExists(int id)
        {
            return _context.FlatTypeMasters.Any(e => e.Flat_id == id);
        }
    }
}