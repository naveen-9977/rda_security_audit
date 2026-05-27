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
    public class Block_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Block_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Block_Master
        public async Task<IActionResult> Index()
        {
            var data = await _context.Block_Masters
                                .Include(b => b.Scheme_Master)
                                .Include(b => b.Sector_Master)
                                .Where(x => x.IsDeleted == false)
                                .ToListAsync();

            return View("~/Views/AdminDashboard/Master/Block_Master/Index.cshtml", data);
        }

        // GET: Block_Master/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var block_Master = await _context.Block_Masters
                .Include(b => b.Scheme_Master)
                .Include(b => b.Sector_Master)
                .FirstOrDefaultAsync(m => m.block_id == id);

            if (block_Master == null) return NotFound();

            return View("~/Views/AdminDashboard/Master/Block_Master/Details.cshtml", block_Master);
        }

        // GET: Block_Master/Create
        public IActionResult Create()
        {
            ViewBag.SchemeList = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en");
            ViewBag.SectorList = new SelectList(_context.Sector_Master.Where(x => x.IsDeleted == false), "sector_id", "sector_name_en");
            return View("~/Views/AdminDashboard/Master/Block_Master/Create.cshtml");
        }

        // POST: Block_Master/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("block_id,scheme_id,sector_id,block_name_hi,block_name_en,IsActive,IsDeleted")] Block_Master blockMaster)
        {
            if (ModelState.IsValid)
            {
                // 🔥 BACKEND SECURITY: Audit Fields
                blockMaster.Create_Date = DateTime.Now;
                blockMaster.created_by = User.Identity.Name ?? "Admin";

                _context.Add(blockMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SchemeList = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en", blockMaster.scheme_id);
            ViewBag.SectorList = new SelectList(_context.Sector_Master.Where(x => x.IsDeleted == false), "sector_id", "sector_name_en", blockMaster.sector_id);
            return View("~/Views/AdminDashboard/Master/Block_Master/Create.cshtml", blockMaster);
        }

        // GET: Block_Master/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var block_Master = await _context.Block_Masters.FindAsync(id);
            if (block_Master == null) return NotFound();

            ViewBag.SchemeList = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en", block_Master.scheme_id);
            ViewBag.SectorList = new SelectList(_context.Sector_Master.Where(x => x.IsDeleted == false), "sector_id", "sector_name_en", block_Master.sector_id);
            return View("~/Views/AdminDashboard/Master/Block_Master/Edit.cshtml", block_Master);
        }

        // POST: Block_Master/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("block_id,scheme_id,sector_id,block_name_hi,block_name_en,IsActive,IsDeleted")] Block_Master block_Master)
        {
            if (id != block_Master.block_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔥 FETCH EXISTING RECORD (To keep old CreatedBy/Date intact)
                    var existing = await _context.Block_Masters.FindAsync(id);
                    if (existing == null) return NotFound();

                    // Update only allowed fields
                    existing.scheme_id = block_Master.scheme_id;
                    existing.sector_id = block_Master.sector_id;
                    existing.block_name_en = block_Master.block_name_en;
                    existing.block_name_hi = block_Master.block_name_hi;
                    existing.IsActive = block_Master.IsActive;
                    existing.IsDeleted = block_Master.IsDeleted;

                    // 🔥 BACKEND SECURITY: Audit Fields
                    existing.updated_by = User.Identity.Name ?? "Admin";
                    existing.updated_Date = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Block_MasterExists(block_Master.block_id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SchemeList = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en", block_Master.scheme_id);
            ViewBag.SectorList = new SelectList(_context.Sector_Master.Where(x => x.IsDeleted == false), "sector_id", "sector_name_en", block_Master.sector_id);
            return View("~/Views/AdminDashboard/Master/Block_Master/Edit.cshtml", block_Master);
        }

        // GET: Block_Master/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var block_Master = await _context.Block_Masters
                .Include(b => b.Scheme_Master)
                .Include(b => b.Sector_Master)
                .FirstOrDefaultAsync(m => m.block_id == id);

            if (block_Master == null) return NotFound();

            return View("~/Views/AdminDashboard/Master/Block_Master/Delete.cshtml", block_Master);
        }

        // POST: Block_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var block = await _context.Block_Masters.FindAsync(id);
            if (block != null)
            {
                // 🔥 Soft Delete with Audit
                block.IsDeleted = true;
                block.IsActive = false;
                block.updated_Date = DateTime.Now;
                block.updated_by = User.Identity.Name ?? "Admin";

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool Block_MasterExists(int id)
        {
            return _context.Block_Masters.Any(e => e.block_id == id);
        }

        [HttpGet]
        public JsonResult GetSectorsByScheme(int schemeId)
        {
            var sectors = _context.Sector_Master
                .Where(x => x.scheme_id == schemeId && x.IsDeleted == false)
                .Select(x => new { x.sector_id, x.sector_name_en })
                .ToList();
            return Json(sectors);
        }
    }
}