using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    // This attribute secures the entire controller
    [Authorize(Roles = "Admin")]
    public class Scheme_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Scheme_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Scheme_Master
        public async Task<IActionResult> Index()
        {
            var schemes = await _context.Scheme_Master.ToListAsync();
            // Explicitly pointing to the new folder location
            return View("~/Views/AdminDashboard/Master/Scheme_Master/Index.cshtml", schemes);
        }

        // GET: Scheme_Master/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheme_Master = await _context.Scheme_Master
                .FirstOrDefaultAsync(m => m.scheme_id == id);
            if (scheme_Master == null)
            {
                return NotFound();
            }

            return View("~/Views/AdminDashboard/Master/Scheme_Master/Details.cshtml", scheme_Master);
        }

        // GET: Scheme_Master/Create
        public IActionResult Create()
        {
            return View("~/Views/AdminDashboard/Master/Scheme_Master/Create.cshtml");
        }

        // POST: Scheme_Master/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("scheme_id,scheme_name_en,scheme_name_hi,scheme_sort_name_en,scheme_sort_name_hi,scheme_rera_no,create_date,updated_date,updated_by,IsActive,IsDeleted")] Scheme_Master scheme_Master)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scheme_Master);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/Scheme_Master/Create.cshtml", scheme_Master);
        }

        // GET: Scheme_Master/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheme_Master = await _context.Scheme_Master.FindAsync(id);
            if (scheme_Master == null)
            {
                return NotFound();
            }
            return View("~/Views/AdminDashboard/Master/Scheme_Master/Edit.cshtml", scheme_Master);
        }

        // POST: Scheme_Master/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("scheme_id,scheme_name_en,scheme_name_hi,scheme_sort_name_en,scheme_sort_name_hi,scheme_rera_no,create_date,updated_date,updated_by,IsActive,IsDeleted")] Scheme_Master scheme_Master)
        {
            if (id != scheme_Master.scheme_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheme_Master);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Scheme_MasterExists(scheme_Master.scheme_id))
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
            return View("~/Views/AdminDashboard/Master/Scheme_Master/Edit.cshtml", scheme_Master);
        }

        // GET: Scheme_Master/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheme_Master = await _context.Scheme_Master
                .FirstOrDefaultAsync(m => m.scheme_id == id);
            if (scheme_Master == null)
            {
                return NotFound();
            }

            return View("~/Views/AdminDashboard/Master/Scheme_Master/Delete.cshtml", scheme_Master);
        }

        // POST: Scheme_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheme_Master = await _context.Scheme_Master.FindAsync(id);
            if (scheme_Master != null)
            {
                _context.Scheme_Master.Remove(scheme_Master);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Scheme_MasterExists(int id)
        {
            return _context.Scheme_Master.Any(e => e.scheme_id == id);
        }
    }
}