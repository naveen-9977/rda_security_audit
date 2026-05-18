using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;

namespace rda_raipur.Controllers
{
    public class PropertyModelTypeMastersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyModelTypeMastersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PropertyModelTypeMasters
        public async Task<IActionResult> Index()
        {
            //return View(await _context.PropertyModelTypeMasters.ToListAsync());


            var data = await _context.PropertyModelTypeMasters
                       .Where(x => x.IsDeleted == false)
                       .ToListAsync();

            return View(data);
        }

        // GET: PropertyModelTypeMasters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyModelTypeMaster = await _context.PropertyModelTypeMasters
                .FirstOrDefaultAsync(m => m.Model_Id == id);
            if (propertyModelTypeMaster == null)
            {
                return NotFound();
            }

            return View(propertyModelTypeMaster);
        }

        // GET: PropertyModelTypeMasters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PropertyModelTypeMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Model_Id,Model_name_en,Model_name_hi,created_by,Create_Date,updated_Date,updated_by,IsActive,IsDeleted")] PropertyModelTypeMaster propertyModelTypeMaster)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(propertyModelTypeMaster);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(propertyModelTypeMaster);
        //}


        public async Task<IActionResult> Create(PropertyModelTypeMaster model)
        {
            if (ModelState.IsValid)
            {
                model.Create_Date = DateTime.Now;
                model.created_by = "Admin";
                model.IsDeleted = false;

                _context.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: PropertyModelTypeMasters/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var propertyModelTypeMaster = await _context.PropertyModelTypeMasters.FindAsync(id);
        //    if (propertyModelTypeMaster == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(propertyModelTypeMaster);
        //}

        public async Task<IActionResult> Edit(int id)
        {
            var data = await _context.PropertyModelTypeMasters
                        .FirstOrDefaultAsync(x => x.Model_Id == id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }


        // POST: PropertyModelTypeMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Model_Id,Model_name_en,Model_name_hi,created_by,Create_Date,updated_Date,updated_by,IsActive,IsDeleted")] PropertyModelTypeMaster propertyModelTypeMaster)
        //{
        //    if (id != propertyModelTypeMaster.Model_Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(propertyModelTypeMaster);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PropertyModelTypeMasterExists(propertyModelTypeMaster.Model_Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(propertyModelTypeMaster);
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyModelTypeMaster model)
        {
            if (id != model.Model_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var data = await _context.PropertyModelTypeMasters
                            .FirstOrDefaultAsync(x => x.Model_Id == id);

                if (data != null)
                {
                    data.Model_name_en = model.Model_name_en;
                    data.Model_name_hi = model.Model_name_hi;
                    data.IsActive = model.IsActive;
                    data.updated_Date = DateTime.Now;
                    data.updated_by = "Admin";

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // GET: PropertyModelTypeMasters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyModelTypeMaster = await _context.PropertyModelTypeMasters
                .FirstOrDefaultAsync(m => m.Model_Id == id);
            if (propertyModelTypeMaster == null)
            {
                return NotFound();
            }

            return View(propertyModelTypeMaster);
        }

        // POST: PropertyModelTypeMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyModelTypeMaster = await _context.PropertyModelTypeMasters.FindAsync(id);
            if (propertyModelTypeMaster != null)
            {
                _context.PropertyModelTypeMasters.Remove(propertyModelTypeMaster);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyModelTypeMasterExists(int id)
        {
            return _context.PropertyModelTypeMasters.Any(e => e.Model_Id == id);
        }
    }
}
