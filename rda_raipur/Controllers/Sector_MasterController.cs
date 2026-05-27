using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Sector_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Sector_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sector_Master
        public async Task<IActionResult> Index()
        {
            var data = _context.Sector_Master
                                .Include(s => s.Scheme_Master)
                                .Where(x => x.IsDeleted == false);

            return View("~/Views/AdminDashboard/Master/Sector_Master/Index.cshtml", await data.ToListAsync());
        }

        // GET: Sector_Master/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sector_Master = await _context.Sector_Master
                .Include(s => s.Scheme_Master)
                .FirstOrDefaultAsync(m => m.sector_id == id);

            if (sector_Master == null) return NotFound();

            return View("~/Views/AdminDashboard/Master/Sector_Master/Details.cshtml", sector_Master);
        }

        // GET: Sector_Master/Create
        public IActionResult Create()
        {
            ViewBag.SchemeList = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en");
            return View("~/Views/AdminDashboard/Master/Sector_Master/Create.cshtml");
        }

        // POST: Sector_Master/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("sector_id,scheme_id,sector_name_en,sector_name_hi,IsActive,IsDeleted")] Sector_Master sector)
        {
            if (ModelState.IsValid)
            {
                // 🔥 BACKEND SECURITY: Audit Fields
                sector.created_by = User.Identity.Name;
                sector.Create_Date = DateTime.Now;

                _context.Add(sector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SchemeList = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en", sector.scheme_id);
            return View("~/Views/AdminDashboard/Master/Sector_Master/Create.cshtml", sector);
        }

        // GET: Sector_Master/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sector_Master = await _context.Sector_Master.FindAsync(id);
            if (sector_Master == null) return NotFound();

            ViewBag.SchemeList = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en", sector_Master.scheme_id);
            return View("~/Views/AdminDashboard/Master/Sector_Master/Edit.cshtml", sector_Master);
        }

        // POST: Sector_Master/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("sector_id,scheme_id,sector_name_en,sector_name_hi,IsActive,IsDeleted")] Sector_Master sector_Master)
        {
            if (id != sector_Master.sector_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔥 FETCH EXISTING RECORD (To keep old CreatedBy/Date intact)
                    var existingSector = await _context.Sector_Master.FindAsync(id);
                    if (existingSector == null) return NotFound();

                    // Update fields
                    existingSector.scheme_id = sector_Master.scheme_id;
                    existingSector.sector_name_en = sector_Master.sector_name_en;
                    existingSector.sector_name_hi = sector_Master.sector_name_hi;
                    existingSector.IsActive = sector_Master.IsActive;
                    existingSector.IsDeleted = sector_Master.IsDeleted;

                    // 🔥 BACKEND SECURITY: Audit Fields
                    existingSector.updated_by = User.Identity.Name;
                    existingSector.updated_Date = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Sector_MasterExists(sector_Master.sector_id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SchemeList = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en", sector_Master.scheme_id);
            return View("~/Views/AdminDashboard/Master/Sector_Master/Edit.cshtml", sector_Master);
        }

        // GET: Sector_Master/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sector_Master = await _context.Sector_Master
                .Include(s => s.Scheme_Master)
                .FirstOrDefaultAsync(m => m.sector_id == id);

            if (sector_Master == null) return NotFound();

            return View("~/Views/AdminDashboard/Master/Sector_Master/Delete.cshtml", sector_Master);
        }

        // POST: Sector_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sector = await _context.Sector_Master.FindAsync(id);

            if (sector != null)
            {
                // 🔥 Soft Delete
                sector.IsDeleted = true;
                sector.IsActive = false;
                sector.updated_by = User.Identity.Name;
                sector.updated_Date = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool Sector_MasterExists(int id)
        {
            return _context.Sector_Master.Any(e => e.sector_id == id);
        }
    }
}