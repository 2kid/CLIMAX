using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CLIMAX.Models;
using System.Data.Entity;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Mail;

namespace CLIMAX.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public ActionResult AdminIndex()
        {
            return View(db.Users.ToList());
        }

        public ActionResult Disable(string email)
        {
            ApplicationUser user = db.Users.Where(r => r.UserName == email).Single();
            user.isActive = false;
            db.Entry(user).State = EntityState.Modified;
            int auditId = Audit.CreateAudit(user.UserName, "Disable", "Account", User.Identity.Name);
            db.SaveChanges();
            return RedirectToAction("AdminIndex");
        }

        public ActionResult Enable(string email)
        {
            ApplicationUser user = db.Users.Where(r => r.UserName == email).Single();
            user.isActive = true;
            db.Entry(user).State = EntityState.Modified;
            int auditId = Audit.CreateAudit(user.UserName, "Enable", "Account", User.Identity.Name);
            db.SaveChanges();
            return RedirectToAction("AdminIndex");
        }

        public ActionResult EditAccount(string email)
        {
            return View(new SetPasswordViewModel() { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Email == "admin@yahoo.com")
                {
                    ModelState.AddModelError("", "You cannot change the password of this account.");
                    return View(model);
                }

                string userId = db.Users.Where(r => r.UserName == model.Email).Select(u => u.Id).Single();
                var result = UserManager.RemovePassword(userId);
                result = UserManager.AddPassword(userId, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("AdminIndex");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser user = db.Users.Include(a=>a.employee).Where(r => r.UserName == model.Email).SingleOrDefault();
            if (user != null && user.EmailConfirmed)
            {
                if (user.isActive)
                {
                        var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                        switch (result)
                        {
                            case SignInStatus.Success:
                                string message = "";

                                if (user.employee.roleType.Type == "Officer in Charge")//User.IsInRole("OIC"))
                                {
                                    int branchId = db.Employees.Where(r => r.EmployeeID == user.EmployeeID).Select(u => u.BranchID).Single();

                                    if (branchId != 1)
                                    {
                                        List<Inventory> inventory = db.Inventories.Include(a => a.material).Where(r => r.BranchID == branchId).ToList();
                                        //check if stocks is low
                                        List<InventoryMessageViewModel> items = new List<InventoryMessageViewModel>();
                                        foreach (Inventory material in inventory)
                                        {
                                            if (material.isLowInStock)
                                                items.Add(new InventoryMessageViewModel() { Inventory = material.material.MaterialName, QtyLeft = material.QtyInStock.ToString() });
                                        }

                                        message = JsonConvert.SerializeObject(items);
                                    }
                                }

                                if (returnUrl != null)
                                {
                                    return (returnUrl.Contains('?')) ? RedirectToLocal(returnUrl, "&", message) : RedirectToLocal(returnUrl, "?", message);
                                }
                                else
                                {
                                    return RedirectToLocal(returnUrl, "?", message);
                                }
                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                            case SignInStatus.Failure:
                            default:
                                ModelState.AddModelError("", "Invalid login attempt.");
                                return View(model);
                        }
                    }
                     ModelState.AddModelError("", "Account has been deactivated.");
            }
                else
                  ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);       
            }   

        //
        // GET: /Account/Register

        public ActionResult AddAccount()
        {
            ViewBag.RoleType = new SelectList(db.RoleType.Where(r => r.Type == "Officer in Charge" || r.Type == "Administrator" || r.Type == "Auditor").ToList(), "RoleTypeId", "Type");
            ViewBag.Branch = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName");
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAccount(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string role = db.RoleType.Find(model.RoleTypeID).Type;
                string branch = db.Branches.Find(model.BranchID).BranchName;
                if (role == "Officer in Charge" && branch == "*")
                {
                    ModelState.AddModelError("", "Officer in Charge must be assigned to a branch");
                    ViewBag.RoleType = new SelectList(db.RoleType.Where(r => r.Type == "Officer in Charge" || r.Type == "Administrator" || r.Type == "Auditor").ToList(), "RoleTypeId", "Type");
                    ViewBag.Branch = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName");
                    return View(model);
                }

                Employee employee = new Employee()
                {
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    RoleTypeID = model.RoleTypeID,
                    BranchID = model.BranchID
                };

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, employee = employee, isActive = true };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    switch (role)
                    {
                        case "Officer in Charge":
                            result = UserManager.AddToRole(user.Id, "OIC");
                            break;
                        case "Auditor":
                            result = UserManager.AddToRole(user.Id, "Auditor");
                            break;
                        case "Administrator":
                            result = UserManager.AddToRole(user.Id, "Admin");
                            break;
                    }
                    int auditId = Audit.CreateAudit(user.UserName, "Create", "Account", User.Identity.Name);

                    System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                    new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["EMAIL"], "Web Registration"),
                    new System.Net.Mail.MailAddress(user.Email));
                    m.Subject = "Email confirmation";
                    m.Body = string.Format("Dear {0} ,<BR/> your email has been to registered to Dermstrata's Clinic Management System (CLIMAX) as an {2}. Please click on the below link to confirm your email: <a href=\"{1}\" title=\"User Email Confirm\">{1}</a>",
                    user.UserName, Url.Action("ConfirmEmail", "Account",
                    new { Token = user.Id, Email = user.Email }, Request.Url.Scheme), role);
                    m.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com");
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EMAIL"], ConfigurationManager.AppSettings["EMAIL_PASSWORD"]);
                    smtp.EnableSsl = true;
                    smtp.Send(m);

                    return RedirectToAction("AdminIndex", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            ViewBag.RoleType = new SelectList(db.RoleType.Where(r => r.Type == "Officer in Charge" || r.Type == "Administrator" || r.Type == "Auditor").ToList(), "RoleTypeId", "Type");
            ViewBag.Branch = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName");
            return View(model);
        }


        //
        // POST: /Account/LogOff
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Confirm(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string Token, string Email)
        {
            ApplicationUser user = this.UserManager.FindById(Token);
            if (user != null)
            {
                if (user.Email == Email && user.EmailConfirmed == false)
                {
                    user.EmailConfirmed = true;
                    await UserManager.UpdateAsync(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return View();
                }
                else
                {
                    return RedirectToAction("Confirm", "Account", new { Email = user.Email });
                }
            }
            else
            {
                return RedirectToAction("Confirm", "Account", new { Email = "" });
            }
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                //generate password token
                var token = UserManager.GeneratePasswordResetToken(user.Id);
                //create url with above token
                var resetLink = Url.Action("ResetPassword", "Account", new { un = model.Email, rt = token }, protocol: Request.Url.Scheme);

                //send mail
                string subject = "Password Reset";
                //string body = "<b>Please find the Password Reset Token</b><br/>" + resetLink;
                string body = string.Format("Dear {0}<BR/>Someone has requested to reset your password, please click on the link below to complete the password reset: <a href=\"" + resetLink + "\">Reset Password</a>", user.UserName);
                try
                {
                    SmtpClient client = new SmtpClient();
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;

                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EMAIL"], ConfigurationManager.AppSettings["EMAIL_PASSWORD"]);
                    client.UseDefaultCredentials = false;
                    client.Credentials = credentials;

                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress(ConfigurationManager.AppSettings["EMAIL"]);
                    msg.To.Add(new MailAddress(user.Email));

                    msg.Subject = subject;
                    msg.IsBodyHtml = true;
                    msg.Body = body;

                    client.Send(msg);
                }
                catch
                {

                }
            }
            return View("ForgotPasswordConfirmation");
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string un, string rt)
        {
            ApplicationUser user = this.UserManager.FindByEmail(un);
            if (user != null)
            {
                return rt == null ? View("Error") : View(new ResetPasswordViewModel { Email = un, Code = rt });
            }
            return View("Error");

        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl, string symbol, string message)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                if (!returnUrl.Contains("/Account/LogOff"))
                    return Redirect(returnUrl + symbol + "message=" + message);
            }
            return RedirectToAction("Index", "Patients", new { message = message });
        }
        #endregion
    }
}