using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using rda_raipur.Models.otp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        // 🔥 SEPARATE MASTER OTPS 🔥
        private const string USER_MASTER_OTP = "123456";  // For Public (Allottee, Booking, Register)
        private const string ADMIN_MASTER_OTP = "987654"; // For Admins and Employees

        private string GetClientIp()
        {
            // Agar app Proxy/Load Balancer/Cloudflare ke piche hai, toh 'X-Forwarded-For' header use karein
            var remoteIp = Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(remoteIp))
            {
                remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            return remoteIp ?? "Unknown";
        }

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 🔥 SMART REDIRECT LOGIC (SECURITY AUDIT SAFE) 🔥
        // =========================================================================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            // Agar returnUrl me koi Admin/Employee wale page ka naam hai, toh seedha EmployeeLogin par bhejo
            if (!string.IsNullOrEmpty(returnUrl))
            {
                // Admin/Staff routes ke keywords
                string[] staffKeywords = { "Master", "Tender", "Admin", "Dashboard", "Lottery", "Propertie", "Payment", "Employee", "Verification" };

                bool isStaffRoute = staffKeywords.Any(keyword => returnUrl.Contains(keyword, StringComparison.OrdinalIgnoreCase));

                if (isStaffRoute)
                {
                    return RedirectToAction("EmployeeLogin");
                }
            }

            // Default fallback public users (Allottees) ke liye
            return RedirectToAction("AllotteeLogin");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            return RedirectToAction("AllotteeLogin");
        }
        // =========================================================================


        #region ALLOTTEE_LOGIN_PROCESS

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AllotteeLogin()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllotteeLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    var userRole = user.Role ?? "User";

                    // 🔥 STRICT SECURITY CHECK: Sirf 'User' role wale hi Allottee Portal me login kar sakte hain 🔥
                    if (!userRole.Equals("User", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(string.Empty, "Access Denied: Officials/Admins must use the Staff Login Portal.");
                        return View(model);
                    }

                    // 🔥 USER MASTER OTP (123456) OR REAL OTP LOGIC 🔥
                    bool isMasterOtp = !string.IsNullOrEmpty(model.Otp) && model.Otp.Trim() == USER_MASTER_OTP;
                    bool isRealOtpValid = !string.IsNullOrEmpty(user.Otp) && !string.IsNullOrEmpty(model.Otp) && user.Otp.Trim() == model.Otp.Trim() && user.OtpExpiry >= DateTime.Now;

                    if (isMasterOtp || isRealOtpValid)
                    {
                        var log = await _context.OtpLogs
                            .OrderByDescending(l => l.Id)
                            .FirstOrDefaultAsync(l => l.MobileNo == model.Username && l.OtpCode == model.Otp && !l.IsUsed);

                        if (log != null) log.IsUsed = true;

                        user.Otp = null;
                        user.OtpExpiry = null;
                        await _context.SaveChangesAsync();

                        var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, "User"),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid or expired OTP.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Mobile Number or password.");
                }
            }
            return View(model);
        }

        #endregion

        #region EMPLOYEE_LOGIN_PROCESS

        [HttpGet]
        [AllowAnonymous]
        public IActionResult EmployeeLogin()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    var userRole = user.Role ?? "User";

                    // 🔥 SECURITY CHECK: Public Users cannot login from Employee Portal 🔥
                    if (!userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) && !userRole.Equals("Employee", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(string.Empty, "Access Denied: Public users must use the Allottee Portal.");
                        return View(model);
                    }

                    string dbOtp = !string.IsNullOrEmpty(user.Otp) ? user.Otp.Trim() : "EMPTY_IN_DB";
                    string formOtp = !string.IsNullOrEmpty(model.Otp) ? model.Otp.Trim() : "EMPTY_IN_FORM";

                    // 🔥 ADMIN MASTER OTP (987654) OR REAL OTP LOGIC 🔥
                    bool isMasterOtp = formOtp == ADMIN_MASTER_OTP;
                    bool isOtpMatch = (dbOtp == formOtp);
                    bool isNotExpired = user.OtpExpiry.HasValue && user.OtpExpiry.Value >= DateTime.Now;

                    if (isMasterOtp || (isOtpMatch && isNotExpired && dbOtp != "EMPTY_IN_DB"))
                    {
                        var log = await _context.OtpLogs
                            .OrderByDescending(l => l.Id)
                            .FirstOrDefaultAsync(l => l.MobileNo == model.Username && l.OtpCode == model.Otp && !l.IsUsed);

                        if (log != null) log.IsUsed = true;

                        user.Otp = null;
                        user.OtpExpiry = null;
                        await _context.SaveChangesAsync();

                        var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, char.ToUpper(userRole[0]) + userRole.Substring(1).ToLower()),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Index", "AdminDashboard");
                    }
                    else
                    {
                        // 🔴 ADVANCED ERROR REPORTING 🔴
                        if (!isOtpMatch && !isMasterOtp)
                        {
                            ModelState.AddModelError(string.Empty, $"❌ OTP FAILED: Invalid OTP entered.");
                        }
                        else if (!isNotExpired && !isMasterOtp)
                        {
                            ModelState.AddModelError(string.Empty, $"❌ TIME EXPIRED: OTP has expired. Please request a new one.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "❌ Invalid or expired OTP.");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid credentials.");
                }
            }
            return View(model);
        }

        #endregion

        #region OTP_GENERATION (Shared)

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateOtp(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    return Json(new { status = false, message = "Invalid credentials or inactive account." });

                // Hamesha original random OTP generate hoga aur DB mein save hoga
                string otp = new Random().Next(111111, 999999).ToString();
                DateTime expiry = DateTime.Now.AddMinutes(5);

                user.Otp = otp;
                user.OtpExpiry = expiry;

                _context.OtpLogs.Add(new OtpLog
                {
                    MobileNo = username,
                    OtpCode = otp,
                    GeneratedAt = DateTime.Now,
                    ExpiresAt = expiry,
                    OtpType = "Login",
                    IpAddress = GetClientIp()
                });

                await _context.SaveChangesAsync();

                // Hamesha user ke mobile par Original OTP hi SMS hoga
                string message = $"Your OTP for RDA-online payment system is {otp}. Expires in 5 minutes. RDA";
                bool isSent = await SendSmsApi(username, message, "1707169113723849694");

                return Json(new { status = isSent, message = isSent ? "OTP sent successfully." : "Failed to send SMS." });
            }
            catch (Exception ex) { return Json(new { status = false, message = "An error occurred while generating OTP." }); }
        }

        #endregion

        #region REGISTRATION_PROCESS

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.MobileNo);

                // 🔥 USER MASTER OTP (123456) OR REAL OTP LOGIC 🔥
                bool isMasterOtp = !string.IsNullOrEmpty(model.Otp) && model.Otp.Trim() == USER_MASTER_OTP;
                bool isRealOtpValid = user != null && !string.IsNullOrEmpty(user.Otp) && !string.IsNullOrEmpty(model.Otp) && user.Otp.Trim() == model.Otp.Trim() && user.OtpExpiry >= DateTime.Now;

                if (user != null && (isMasterOtp || isRealOtpValid))
                {
                    var log = await _context.OtpLogs
                        .OrderByDescending(l => l.Id)
                        .FirstOrDefaultAsync(l => l.MobileNo == model.MobileNo && l.OtpCode == model.Otp && !l.IsUsed);

                    if (log != null) log.IsUsed = true;

                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    user.Email = model.Email;
                    user.MobileNo = model.MobileNo;
                    user.IsActive = true;
                    user.Otp = null;
                    user.OtpExpiry = null;

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "आपका खाता सफलतापूर्वक बन गया है! कृपया लॉगिन करें।";

                    return RedirectToAction("AllotteeLogin", "Account");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid or expired OTP.");
                }
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateRegistrationOtp(string mobileNo)
        {
            if (string.IsNullOrEmpty(mobileNo) || mobileNo.Length != 10)
                return Json(new { status = false, message = "Valid mobile number is required." });

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == mobileNo);
                if (user == null)
                {
                    user = new rda_raipur.Models.User { Username = mobileNo, IsActive = false, Role = "User" };
                    _context.Users.Add(user);
                }

                string otp = new Random().Next(111111, 999999).ToString();
                DateTime expiry = DateTime.Now.AddMinutes(5);

                user.Otp = otp;
                user.OtpExpiry = expiry;

                _context.OtpLogs.Add(new OtpLog
                {
                    MobileNo = mobileNo,
                    OtpCode = otp,
                    GeneratedAt = DateTime.Now,
                    ExpiresAt = expiry,
                    OtpType = "Registration",
                    IpAddress = GetClientIp()
                });

                await _context.SaveChangesAsync();

                string message = $"Your OTP for RDA-online payment system is {otp}. Expires in 5 minutes. RDA";
                bool isSent = await SendSmsApi(mobileNo, message, "1707169113723849694");

                return Json(new { status = isSent, message = isSent ? "OTP sent successfully." : "SMS Provider Error." });
            }
            catch (Exception ex) { return Json(new { status = false, message = "An error occurred." }); }
        }

        #endregion

        #region BOOKING_FORM_OTP_AND_PROFILE_FETCH

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateBookingOtp(string mobileNo)
        {
            if (string.IsNullOrEmpty(mobileNo) || mobileNo.Length != 10)
                return Json(new { status = false, message = "Valid mobile number is required." });

            try
            {
                string otp = new Random().Next(111111, 999999).ToString();
                DateTime expiry = DateTime.Now.AddMinutes(5);

                _context.OtpLogs.Add(new OtpLog
                {
                    MobileNo = mobileNo,
                    OtpCode = otp,
                    GeneratedAt = DateTime.Now,
                    ExpiresAt = expiry,
                    OtpType = "BookingVerify",
                    IpAddress = GetClientIp()
                });

                await _context.SaveChangesAsync();

                string message = $"Your OTP for RDA-online payment system is {otp}. Expires in 5 minutes. RDA";
                bool isSent = await SendSmsApi(mobileNo, message, "1707169113723849694");

                return Json(new { status = isSent, message = isSent ? "OTP sent successfully." : "SMS Provider Error." });
            }
            catch (Exception ex) { return Json(new { status = false, message = "An error occurred." }); }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyBookingOtpAndFetchData(string mobileNo, string otp)
        {
            // 🔥 USER MASTER OTP (123456) OR REAL OTP LOGIC 🔥
            bool isMasterOtp = !string.IsNullOrEmpty(otp) && otp.Trim() == USER_MASTER_OTP;

            var log = await _context.OtpLogs
                .OrderByDescending(l => l.Id)
                .FirstOrDefaultAsync(l => l.MobileNo == mobileNo && l.OtpCode == otp && !l.IsUsed && l.OtpType == "BookingVerify");

            if (!isMasterOtp && (log == null || log.ExpiresAt < DateTime.Now))
            {
                return Json(new { status = false, message = "Invalid or expired OTP." });
            }

            if (log != null) log.IsUsed = true;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == mobileNo);
            if (user == null)
            {
                user = new rda_raipur.Models.User
                {
                    Username = mobileNo,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user@123"),
                    MobileNo = mobileNo,
                    IsActive = true,
                    Role = "User"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var profile = await _context.UserProfileDetails.FirstOrDefaultAsync(p => p.MobileNo == mobileNo);

            if (profile != null)
            {
                return Json(new { status = true, isExisting = true, data = profile, message = "Profile fetched successfully. Auto-filling details." });
            }
            else
            {
                return Json(new { status = true, isExisting = false, data = new { mobileNo = mobileNo }, message = "Mobile verified. Please fill in your application details." });
            }
        }

        #endregion

        #region FORGOT_PASSWORD

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateForgotPwdOtp(string mobileNo)
        {
            if (string.IsNullOrEmpty(mobileNo) || mobileNo.Length != 10)
                return Json(new { status = false, message = "Valid mobile number is required." });

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == mobileNo);
                if (user == null || !user.IsActive)
                    return Json(new { status = false, message = "Mobile number not registered or inactive." });

                string otp = new Random().Next(111111, 999999).ToString();
                DateTime expiry = DateTime.Now.AddMinutes(5);

                user.Otp = otp;
                user.OtpExpiry = expiry;

                _context.OtpLogs.Add(new OtpLog
                {
                    MobileNo = mobileNo,
                    OtpCode = otp,
                    GeneratedAt = DateTime.Now,
                    ExpiresAt = expiry,
                    OtpType = "ForgotPassword",
                    IpAddress = GetClientIp()
                });

                await _context.SaveChangesAsync();

                string message = $"Your OTP for RDA-online payment system is {otp}. Expires in 5 minutes. RDA";
                bool isSent = await SendSmsApi(mobileNo, message, "1707169113723849694");

                return Json(new { status = isSent, message = isSent ? "OTP sent successfully." : "Failed to send SMS." });
            }
            catch (Exception ex) { return Json(new { status = false, message = "An error occurred." }); }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string mobileNo, string otp, string newPassword)
        {
            if (string.IsNullOrEmpty(mobileNo) || string.IsNullOrEmpty(otp) || string.IsNullOrEmpty(newPassword))
                return Json(new { status = false, message = "All fields are required." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == mobileNo);
            if (user == null) return Json(new { status = false, message = "User not found." });

            // 🔥 USER MASTER OTP (123456) OR REAL OTP LOGIC 🔥
            bool isMasterOtp = otp.Trim() == USER_MASTER_OTP;
            bool isRealOtpValid = !string.IsNullOrEmpty(user.Otp) && user.Otp.Trim() == otp.Trim() && user.OtpExpiry >= DateTime.Now;

            if (isMasterOtp || isRealOtpValid)
            {
                var log = await _context.OtpLogs
                    .OrderByDescending(l => l.Id)
                    .FirstOrDefaultAsync(l => l.MobileNo == mobileNo && l.OtpCode == otp && !l.IsUsed && l.OtpType == "ForgotPassword");

                if (log != null) log.IsUsed = true;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.Otp = null;
                user.OtpExpiry = null;

                await _context.SaveChangesAsync();
                return Json(new { status = true, message = "Password reset successfully. You can now login." });
            }

            return Json(new { status = false, message = "Invalid or expired OTP." });
        }

        #endregion

        #region HELPERS

        private async Task<bool> SendSmsApi(string mobile, string message, string templateId)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string> {
                    { "username", "rdaraipur" }, { "password", "rdaraipur@" },
                    { "senderid", "RDArpr" }, { "message", message },
                    { "numbers", mobile }, { "unicode", "0" }, { "template_id", templateId }
                };
                var content = new FormUrlEncodedContent(values);
                try
                {
                    var response = await client.PostAsync("https://merasandesh.com/api/sendsms", content);
                    return response.IsSuccessStatusCode;
                }
                catch { return false; }
            }
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}