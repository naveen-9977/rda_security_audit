using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using rda_raipur.Models.site;
using System.Diagnostics;

namespace rda_raipur.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Popups = await _context.SitePopups
                                .Where(x => x.IsActive && !x.IsDeleted)
                                .OrderBy(x => x.DisplayOrder).ToListAsync();

            ViewBag.LatestNews = await _context.SiteNews
                                .Where(x => x.IsActive && !x.IsDeleted)
                                .OrderByDescending(x => x.CreatedDate).ToListAsync();

            return View();
        }

        // 🔥 Sabhi Schemes dikhane ke liye 🔥
        public async Task<IActionResult> AllSchemes()
        {
            var schemes = await _context.SiteSchemes
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();

            return View(schemes);
        }

        // 🔴 ID ki jagah Name (Slug) se detail dikhane ke liye
        public async Task<IActionResult> SchemeDetails(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return NotFound();
            }

            // URL mein '-' hoga, usko wapas space ' ' mein convert karein
            var originalName = name.Replace("-", " ");

            var scheme = await _context.SiteSchemes
                .FirstOrDefaultAsync(s => s.SchemeName.ToLower() == originalName.ToLower() && s.IsActive);

            if (scheme == null)
            {
                return NotFound();
            }

            return View(scheme);
        }

        public IActionResult Privacy() => View();
        public IActionResult Leadership() => View();
        public IActionResult Organization() => View();
        public IActionResult Rti() => View();

        public async Task<IActionResult> Gallery()
        {
            var folders = await _context.GalleryFolders.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
            ViewBag.AllImages = await _context.SiteGalleries.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
            return View(folders);
        }

        public async Task<IActionResult> Brochure()
        {
            var brochures = await _context.Brochures.Where(x => x.IsActive).ToListAsync();
            return View(brochures);
        }

        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            ViewBag.ContactInfo = await _context.SiteContacts.FirstOrDefaultAsync(x => x.IsActive);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(rda_raipur.Models.site.ContactQuery model)
        {
            if (ModelState.IsValid)
            {
                _context.ContactQueries.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Message sent successfully!";
                return RedirectToAction(nameof(Contact));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            return LocalRedirect(returnUrl ?? "/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}