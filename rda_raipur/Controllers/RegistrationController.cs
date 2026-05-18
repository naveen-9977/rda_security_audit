using Microsoft.AspNetCore.Mvc;
using rda_raipur.Data;
using rda_raipur.Models;

namespace rda_raipur.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IWebHostEnvironment env;
        private readonly ApplicationDbContext _context;

        public IActionResult Index()
        {
            return View();
        }

        public RegistrationController(IWebHostEnvironment _env, ApplicationDbContext context)
        {
            env = _env;
            _context = context;
        }
        public IActionResult Step1()
        {
            return View("Step1_Application");
        }

        [HttpPost]
        public IActionResult Step1(ApplicantModel model)
        {


            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            if (ModelState.IsValid)
            {
                _context.Applicants.Add(model);
                _context.SaveChanges();

                TempData["ApplicantId"] = model.ApplicantId;

                return RedirectToAction("Step2");
            }

            return View("Step1_Application", model);
        }

        // STEP 2
        public IActionResult Step2()
        {
            return View("Step2_DocumentUpload");
        }

        [HttpPost]
        public IActionResult Step2(DocumentUploadModel model)
        {
            string path = Path.Combine(env.WebRootPath, "uploads");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (model.Photo != null)
            {
                string photoPath = Path.Combine(path, model.Photo.FileName);

                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    model.Photo.CopyTo(stream);
                }
            }

            return RedirectToAction("Step3");
        }

        // STEP 3
        public IActionResult Step3()
        {
            return View("Step3_Payment");
        }

        [HttpPost]
        public IActionResult Step3(PaymentModel model)
        {
            // Save payment
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

    }
}
