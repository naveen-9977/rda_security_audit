using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using rda_raipur.Models.ViewModels;
using rda_raipur.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using rda_raipur.Models.Property;
using Rotativa.AspNetCore;

namespace rda_raipur.Controllers
{
    public class ApplicationBookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        // LIVE CCAvenue Credentials
        private readonly string workingKey = "8813F1E230DBC32216F9AC6D2D1FA4C2";
        private readonly string accessCode = "AVQG90KG75BS04GQSB";
        private readonly string merchantId = "2680067";

        public ApplicationBookingController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ========================================================
        // 🚀 STEP 1: INITIATE BOOKING
        // ========================================================
        [HttpPost]
        public IActionResult InitiateBooking(int mappingId)
        {
            TempData["BookingMappingId"] = mappingId;
            return RedirectToAction("Form");
        }

        // ========================================================
        // 🚀 STEP 2: GET FORM
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Form()
        {
            var mappingIdObj = TempData["BookingMappingId"];
            if (mappingIdObj == null)
            {
                return RedirectToAction("Schemes", "PropertyBooking");
            }

            int mappingId = (int)mappingIdObj;
            TempData.Keep("BookingMappingId");

            var mapping = await _context.Tender_Live_Mappings
                .Include(x => x.TenderLiveMaster)
                .Include(x => x.Property)
                .FirstOrDefaultAsync(x => x.MappingId == mappingId);

            if (mapping == null) return NotFound("Tender Mapping details not found.");

            // 🔥 DATABASE SE PLOT AUR FLAT KI DETAILS NIKALNA 🔥
            var property = mapping.Property;

            ViewBag.Width = property.width_plot != null ? property.width_plot.ToString() : "NA";
            ViewBag.Depth = property.depth_plot != null ? property.depth_plot.ToString() : "NA";
            ViewBag.Direction = !string.IsNullOrEmpty(property.direction_name_en) ? property.direction_name_en : "NA";

            if (property.better_location_id != null && property.better_location_id > 0)
            {
                var loc = await _context.Better_Location.FirstOrDefaultAsync(b => b.better_location_id == property.better_location_id);
                ViewBag.BetterLocationName = loc != null && !string.IsNullOrEmpty(loc.better_location_name_en) ? loc.better_location_name_en : "NA";
            }
            else
            {
                ViewBag.BetterLocationName = "NA";
            }

            // 🔥 NEW: LIG/MIG and Free Hold/Lease Logic 🔥
            ViewBag.AllotmentType = !string.IsNullOrEmpty(property.allotement_type_name_en) ? property.allotement_type_name_en : "NA";
            ViewBag.FlatType = !string.IsNullOrEmpty(property.Flat_name_en) ? property.Flat_name_en : "NA";
            ViewBag.PropertyClassification = !string.IsNullOrEmpty(property.Property_Classification) ? property.Property_Classification : "Residential";

            ViewBag.Sector = property.sector_name_en;
            ViewBag.UnitNo = property.House_No;
            ViewBag.Category = property.Property_Type_Name_en;
            ViewBag.ReserveCategory = property.res_category_name_en;

            ViewBag.Area = property.Super_Buildup_Area;
            ViewBag.BuildupArea = property.Buildup_Area;
            ViewBag.CarpetArea = property.Carpet_Area;

            ViewBag.TotalPrice = property.offset_Price;
            ViewBag.EmdAmount = property.EMD_Amount;
            ViewBag.Location = property.Scheme_name_en;

            // ID pass kar rahe hain
            ViewBag.AssetNo = property.Property_Id; // String ID
            ViewBag.NumericPropertyId = property.Id; // Integer ID

            ViewBag.StartDate = mapping.TenderLiveMaster?.Tender_Start_Date;
            ViewBag.EndDate = mapping.TenderLiveMaster?.Tender_End_Date;

            ViewBag.IsEnglish = true;
            string resCat = property.res_category_name_en?.ToLower() ?? "";
            ViewBag.IsUnreserved = (resCat == "unreserved" || resCat == "general");

            // ========================================================
            // 🔥 REVERSE GST CALCULATION LOGIC (Total exact 1 Rupee) 🔥
            // ========================================================
            decimal totalAppFee = 1.00m; // Exact total amount required
            decimal baseAppFee = Math.Round(totalAppFee / 1.18m, 2); // Result: 0.85m
            decimal gstAmount = totalAppFee - baseAppFee; // Result: 0.15m

            ViewBag.AppFeeBase = baseAppFee;
            ViewBag.AppFeeGst = gstAmount;
            ViewBag.TotalAppFee = totalAppFee;

            return View();
        }

        // ========================================================
        // 🔥 AUTO-FETCH USER DETAILS VIA AJAX
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> GetUserDetailsByMobile(string mobileNo)
        {
            if (string.IsNullOrEmpty(mobileNo))
                return Json(new { success = false, message = "Mobile number is required." });

            var user = await _context.UserProfileDetails
                .Where(u => u.MobileNo == mobileNo)
                .Select(u => new
                {
                    applicantName = u.ApplicantName,
                    fatherHusbandName = u.FatherHusbandName,
                    email = u.Email,
                    address = u.Address,
                    aadharNo = u.AadharNo,
                    panNo = u.PanNo,
                    dob = u.DOB.HasValue ? u.DOB.Value.ToString("yyyy-MM-dd") : "",
                    age = u.Age,
                    gender = u.Gender,
                    casteNo = u.CasteNo,
                    incomeNo = u.IncomeNo,
                    domicileNo = u.DomicileNo,

                    // 🔥 Bank Details Auto Fetch 🔥
                    bankName = u.BankName,
                    bankAccountNo = u.BankAccountNo,
                    ifscCode = u.IfscCode,
                    bankPassbookPath = u.BankPassbookPath,

                    photoPath = u.PhotoPath,
                    signaturePath = u.SignaturePath,
                    aadharPath = u.AadharPath,
                    panPath = u.PanPath,
                    castePath = u.CastePath,
                    incomePath = u.IncomePath,
                    domicilePath = u.DomicilePath,
                    affidavitPath = u.AffidavitPath
                })
                .FirstOrDefaultAsync();

            if (user != null)
            {
                return Json(new { success = true, data = user });
            }

            return Json(new { success = false, message = "User not found. Fresh registration required." });
        }

        // ========================================================
        // 💾 SUBMIT FORM
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromForm] BookingFormViewModel model, [FromForm] decimal PaymentAmount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(model.MobileNo))
                    return Json(new { status = false, message = "Mobile number is required." });

                // 1. UPLOAD FILES
                string? photoPath = model.PhotoUpload != null ? await SaveFileAsync(model.PhotoUpload, "UserDocuments") : null;
                string? signaturePath = model.SignatureUpload != null ? await SaveFileAsync(model.SignatureUpload, "UserDocuments") : null;
                string? aadharPath = model.AadharUpload != null ? await SaveFileAsync(model.AadharUpload, "UserDocuments") : null;
                string? panPath = model.PanUpload != null ? await SaveFileAsync(model.PanUpload, "UserDocuments") : null;
                string? castePath = model.CasteUpload != null ? await SaveFileAsync(model.CasteUpload, "UserDocuments") : null;
                string? incomePath = model.IncomeUpload != null ? await SaveFileAsync(model.IncomeUpload, "UserDocuments") : null;
                string? domicilePath = model.DomicileUpload != null ? await SaveFileAsync(model.DomicileUpload, "UserDocuments") : null;
                string? affidavitPath = model.AffidavitUpload != null ? await SaveFileAsync(model.AffidavitUpload, "UserDocuments") : null;
                string? passbookPath = model.BankPassbookUpload != null ? await SaveFileAsync(model.BankPassbookUpload, "UserDocuments") : null;

                // 2. MAIN PROFILE SAVE/UPDATE
                var mainProfile = await _context.UserProfileDetails.FirstOrDefaultAsync(p => p.MobileNo == model.MobileNo);
                if (mainProfile == null)
                {
                    mainProfile = new UserProfileDetail { MobileNo = model.MobileNo, CreatedAt = DateTime.Now };
                    _context.UserProfileDetails.Add(mainProfile);
                }

                mainProfile.ApplicantName = model.ApplicantName;
                mainProfile.Email = model.Email;
                mainProfile.FatherHusbandName = model.FatherHusbandName;
                mainProfile.Address = model.Address;
                mainProfile.AadharNo = model.AadharNo;
                mainProfile.PanNo = model.PanNo?.ToUpper();
                mainProfile.DOB = model.DOB;
                mainProfile.Age = model.Age;
                mainProfile.Gender = model.Gender;

                mainProfile.BankName = model.BankName;
                mainProfile.BankAccountNo = model.BankAccountNo;
                mainProfile.IfscCode = model.IfscCode?.ToUpper();

                mainProfile.UpdatedAt = DateTime.Now;

                // Document Numbers Save Logic
                try
                {
                    var props = typeof(BookingFormViewModel).GetProperties();
                    if (props.Any(p => p.Name == "CasteNo")) mainProfile.CasteNo = (string)props.First(p => p.Name == "CasteNo").GetValue(model);
                    if (props.Any(p => p.Name == "IncomeNo")) mainProfile.IncomeNo = (string)props.First(p => p.Name == "IncomeNo").GetValue(model);
                    if (props.Any(p => p.Name == "DomicileNo")) mainProfile.DomicileNo = (string)props.First(p => p.Name == "DomicileNo").GetValue(model);
                }
                catch { }

                if (photoPath != null) mainProfile.PhotoPath = photoPath;
                if (signaturePath != null) mainProfile.SignaturePath = signaturePath;
                if (aadharPath != null) mainProfile.AadharPath = aadharPath;
                if (panPath != null) mainProfile.PanPath = panPath;
                if (castePath != null) mainProfile.CastePath = castePath;
                if (incomePath != null) mainProfile.IncomePath = incomePath;
                if (domicilePath != null) mainProfile.DomicilePath = domicilePath;
                if (affidavitPath != null) mainProfile.AffidavitPath = affidavitPath;
                if (passbookPath != null) mainProfile.BankPassbookPath = passbookPath;

                await _context.SaveChangesAsync();

                // 3. BOOKING PROFILE SNAPSHOT
                var bookingProfile = new BookingProfileDetail
                {
                    MobileNo = model.MobileNo,
                    ApplicantName = model.ApplicantName,
                    EmailId = model.Email,
                    PresentAddress = model.Address,
                    AadharNo = model.AadharNo,
                    PanNo = model.PanNo?.ToUpper(),
                    FatherHusbandName = model.FatherHusbandName,
                    DOB = model.DOB,
                    Age = model.Age,
                    Gender = model.Gender,

                    BankName = model.BankName,
                    BankAccountNo = model.BankAccountNo,
                    IfscCode = model.IfscCode?.ToUpper(),
                    BankPassbookPath = passbookPath ?? mainProfile.BankPassbookPath,

                    CasteNo = mainProfile.CasteNo,
                    IncomeNo = mainProfile.IncomeNo,
                    DomicileNo = mainProfile.DomicileNo,

                    PhotoPath = photoPath ?? mainProfile.PhotoPath,
                    SignaturePath = signaturePath ?? mainProfile.SignaturePath,
                    AadharPath = aadharPath ?? mainProfile.AadharPath,
                    PanPath = panPath ?? mainProfile.PanPath,
                    CastePath = castePath ?? mainProfile.CastePath,
                    IncomePath = incomePath ?? mainProfile.IncomePath,
                    DomicilePath = domicilePath ?? mainProfile.DomicilePath,
                    AffidavitPath = affidavitPath ?? mainProfile.AffidavitPath,
                    CreatedAt = DateTime.Now
                };

                _context.BookingProfileDetails.Add(bookingProfile);
                await _context.SaveChangesAsync();

                // =====================================================
                // 🔥 REVERSE GST CALCULATION LOGIC (Total exact 1 Rupee) 🔥
                // =====================================================
                decimal totalAppFee = 1.00m;
                decimal baseAppFee = Math.Round(totalAppFee / 1.18m, 2); // 0.85m
                decimal gstAmount = totalAppFee - baseAppFee; // 0.15m

                // EMD Amount calculation (Jo bhi Payment Amount UI se aayega usme se totalAppFee minus karenge)
                decimal emdAmount = (PaymentAmount > totalAppFee) ? (PaymentAmount - totalAppFee) : 0;

                // 4. PROPERTY BOOKING RECORD
                var booking = new rda_raipur.Models.PropertyBooking
                {
                    UserProfileId = mainProfile.Id,
                    BookingProfileId = bookingProfile.Id,
                    PropertyId = model.PropertyId,
                    BidAmount = model.BidAmount,
                    ApplicationFee = totalAppFee, // DataBase me 1.00 save hoga
                    EMDAmount = emdAmount,
                    BookingDate = DateTime.Now,
                    BookingStatus = "Pending",
                    PaymentStatus = "Pending",
                    VerificationStatus = "Pending"
                };

                _context.PropertyBookings.Add(booking);
                await _context.SaveChangesAsync();

                string actualPropertyCode = booking.PropertyId;
                if (int.TryParse(booking.PropertyId, out int parsedPropId))
                {
                    var propRecord = await _context.TenderProperties.FirstOrDefaultAsync(p => p.Id == parsedPropId);
                    if (propRecord != null && !string.IsNullOrEmpty(propRecord.Property_Id))
                    {
                        actualPropertyCode = propRecord.Property_Id;
                    }
                }

                string applicationNo = $"{actualPropertyCode}-{DateTime.Now:yyMM}-{booking.BookingId:D3}";

                booking.ApplicationNo = applicationNo;
                _context.Update(booking);
                await _context.SaveChangesAsync();

                string orderIdStr = "ORD" + DateTime.Now.Ticks.ToString().Substring(10);

                // 5. PAYMENT RECORD
                var payment = new PaymentDetail
                {
                    BookingId = booking.BookingId,
                    UserProfileId = mainProfile.Id,
                    Amount = PaymentAmount,
                    OrderId = orderIdStr,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Pending"
                };

                _context.PaymentDetails.Add(payment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 6. CCAVENUE REDIRECT
                string redirectUrl = Url.Action("PaymentResponse", "ApplicationBooking", null, Request.Scheme);
                string merchantData = $"merchant_id={merchantId}&order_id={orderIdStr}&currency=INR&amount={PaymentAmount}&redirect_url={redirectUrl}&cancel_url={redirectUrl}&language=EN&billing_name={model.ApplicantName}&billing_tel={model.MobileNo}&merchant_param1={applicationNo}";

                CCAvenueCrypto crypto = new CCAvenueCrypto();
                string encryptedData = crypto.Encrypt(merchantData, workingKey);

                return Json(new { status = true, encRequest = encryptedData, accessCode = accessCode });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                string realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { status = false, message = "DB Error: " + realError });
            }
        }

        // ========================================================
        // 💳 CCAVENUE RESPONSE HANDLER (With SMS Logic)
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> PaymentResponse([FromForm] string encResp)
        {
            try
            {
                CCAvenueCrypto crypto = new CCAvenueCrypto();
                string decryptedData = crypto.Decrypt(encResp, workingKey);

                var responseParams = new Dictionary<string, string>();
                foreach (var param in decryptedData.Split('&'))
                {
                    var pair = param.Split('=');
                    if (pair.Length == 2) responseParams[pair[0]] = pair[1];
                }

                string orderId = responseParams.ContainsKey("order_id") ? responseParams["order_id"] : "";
                string orderStatus = responseParams.ContainsKey("order_status") ? responseParams["order_status"] : "";
                string trackingId = responseParams.ContainsKey("tracking_id") ? responseParams["tracking_id"] : "";
                string amountStr = responseParams.ContainsKey("amount") ? responseParams["amount"] : "0";
                string bankRefNo = responseParams.ContainsKey("bank_ref_no") ? responseParams["bank_ref_no"] : "";

                var payment = await _context.PaymentDetails
                    .Include(p => p.PropertyBooking)
                        .ThenInclude(pb => pb.BookingProfileDetail)
                    .FirstOrDefaultAsync(p => p.OrderId == orderId);

                string applicationNo = "";

                if (payment != null)
                {
                    applicationNo = payment.PropertyBooking?.ApplicationNo;
                    string mobileNo = payment.PropertyBooking?.BookingProfileDetail?.MobileNo;

                    if (orderStatus == "Aborted")
                    {
                        var booking = payment.PropertyBooking;
                        if (booking != null)
                        {
                            var profileSnapshot = await _context.BookingProfileDetails.FindAsync(booking.BookingProfileId);
                            if (profileSnapshot != null) _context.BookingProfileDetails.Remove(profileSnapshot);
                            _context.PropertyBookings.Remove(booking);
                        }
                        _context.PaymentDetails.Remove(payment);
                        await _context.SaveChangesAsync();

                        TempData["Error"] = "Payment was cancelled. Application not saved.";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        payment.PaymentStatus = orderStatus;
                        payment.TransactionId = trackingId;
                        payment.BankRefNo = bankRefNo;

                        payment.PropertyBooking.PaymentStatus = orderStatus;
                        payment.PropertyBooking.TransactionId = trackingId;

                        if (orderStatus == "Success")
                            payment.PropertyBooking.BookingStatus = "Confirmed";
                        else
                            payment.PropertyBooking.BookingStatus = "Failed";

                        await _context.SaveChangesAsync();

                        // SMS LOGIC
                        if (!string.IsNullOrEmpty(mobileNo))
                        {
                            if (orderStatus == "Success")
                            {
                                string msg1 = $"Your Transaction successful ! Amount: {amountStr}, Date: {DateTime.Now:dd/MM/yyyy}, Reference: {trackingId}. RDA";
                                string msg2 = $"Application successfully submitted, your Application number is {applicationNo} RDA";

                                await SendSMSAsync(mobileNo, msg1, "1707169044024940654");
                                await SendSMSAsync(mobileNo, msg2, "1707172778355113015");
                            }
                            else if (orderStatus == "Failure" || orderStatus == "Failed")
                            {
                                string failMsg = "Your Transaction failed. Please try after some time. RDA";
                                await SendSMSAsync(mobileNo, failMsg, "1707169044398717079");
                            }
                        }
                    }
                }

                ViewBag.OrderStatus = orderStatus;
                ViewBag.ApplicationNo = applicationNo;
                ViewBag.OrderId = orderId;
                ViewBag.TrackingId = trackingId;
                ViewBag.Amount = amountStr;

                return View("PaymentResult");
            }
            catch (Exception ex)
            {
                ViewBag.OrderStatus = "Error";
                ViewBag.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return View("PaymentResult");
            }
        }

        // ========================================================
        // 🖨️ PDF DOWNLOAD LOGIC
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> DownloadReceipt(string orderId)
        {
            var paymentDetail = await _context.PaymentDetails
                .Include(p => p.PropertyBooking)
                    .ThenInclude(b => b.BookingProfileDetail)
                .FirstOrDefaultAsync(p => p.PropertyBooking.ApplicationNo == orderId || p.OrderId == orderId);

            if (paymentDetail == null) return Content("<h2>Payment Record Not Found!</h2>", "text/html");

            string bookingPropId = paymentDetail.PropertyBooking.PropertyId?.ToString() ?? "";
            bool isNumeric = int.TryParse(bookingPropId, out int parsedPropId);

            var property = await _context.TenderProperties
                .FirstOrDefaultAsync(x => (isNumeric && x.Id == parsedPropId) || x.Property_Id == bookingPropId);

            if (property != null)
            {
                ViewBag.Width = property.width_plot != null ? property.width_plot.ToString() : "NA";
                ViewBag.Depth = property.depth_plot != null ? property.depth_plot.ToString() : "NA";
                ViewBag.Direction = !string.IsNullOrEmpty(property.direction_name_en) ? property.direction_name_en : "NA";

                if (property.better_location_id != null && property.better_location_id > 0)
                {
                    var loc = await _context.Better_Location.FirstOrDefaultAsync(b => b.better_location_id == property.better_location_id);
                    ViewBag.BetterLocationName = loc != null && !string.IsNullOrEmpty(loc.better_location_name_en) ? loc.better_location_name_en : "NA";
                }
                else
                {
                    ViewBag.BetterLocationName = "NA";
                }

                ViewBag.AllotmentType = !string.IsNullOrEmpty(property.allotement_type_name_en) ? property.allotement_type_name_en : "NA";
                ViewBag.FlatType = !string.IsNullOrEmpty(property.Flat_name_en) ? property.Flat_name_en : "NA";
                ViewBag.PropertyClassification = !string.IsNullOrEmpty(property.Property_Classification) ? property.Property_Classification : "Residential";
            }

            ViewBag.SchemeName = property?.Scheme_name_en ?? "N/A";
            ViewBag.SectorName = property?.sector_name_en ?? "N/A";
            ViewBag.BlockName = property?.block_name_en ?? "N/A";
            ViewBag.PropertyType = property?.Property_Type_Name_en ?? "N/A";
            ViewBag.Area = property?.Super_Buildup_Area ?? property?.Buildup_Area ?? 0;
            ViewBag.OffsetPrice = property?.offset_Price ?? 0;
            ViewBag.ReserveCategory = property?.res_category_name_en ?? "General";

            var profile = paymentDetail.PropertyBooking.BookingProfileDetail;
            if (profile != null)
            {
                ViewBag.PhotoPath = profile.PhotoPath;
                ViewBag.SignPath = profile.SignaturePath;
            }

            return View("ReceiptTemplate", paymentDetail);
        }

        // ========================================================
        // 🖨️ PRINT FORM (HTML Backup)
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> PrintForm(string orderId)
        {
            var paymentDetail = await _context.PaymentDetails
                .Include(p => p.PropertyBooking)
                    .ThenInclude(b => b.BookingProfileDetail)
                .FirstOrDefaultAsync(p => p.PropertyBooking.ApplicationNo == orderId || p.TransactionId == orderId || p.OrderId == orderId);

            if (paymentDetail == null)
                return Content("<h2>Application Data Not Found!</h2>", "text/html");

            return View("~/Views/Home/PrintApplicationForm.cshtml", paymentDetail);
        }

        // ========================================================
        // 📁 HELPER: SAVE FILE TO FOLDER
        // ========================================================
        private async Task<string> SaveFileAsync(Microsoft.AspNetCore.Http.IFormFile file, string folderName)
        {
            string folderPath = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(folderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/uploads/" + folderName + "/" + uniqueFileName;
        }

        // ========================================================
        // 💬 HELPER: SEND SMS API
        // ========================================================
        private async Task SendSMSAsync(string mobileNumber, string messageText, string templateId)
        {
            try
            {
                using var client = new HttpClient();
                var values = new Dictionary<string, string>
                {
                    { "username", "rdaraipur" },
                    { "password", "rdaraipur@" },
                    { "senderid", "RDArpr" },
                    { "message", messageText },
                    { "numbers", mobileNumber },
                    { "unicode", "0" },
                    { "template_id", templateId }
                };

                var content = new FormUrlEncodedContent(values);
                await client.PostAsync("https://merasandesh.com/api/sendsms", content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SMS Sending Failed: " + ex.Message);
            }
        }
    }
}