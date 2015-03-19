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

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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
            return View(new SetPasswordViewModel() { Email = email});
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

                string userId = db.Users.Where(r=>r.UserName == model.Email).Select(u=>u.Id).Single();
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
           
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (db.Users.Where(r => r.UserName == model.Email).Select(u=>u.isActive).Single())
                    return RedirectToLocal(returnUrl);
                    else
                          ModelState.AddModelError("", "Account has been deactivated.");
                    return View(model);
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


        //
        // GET: /Account/Register
        
        public ActionResult AddAccount()
        {
            ViewBag.RoleType = new SelectList(db.RoleType.Where(r => r.Type == "Officer in Charge" || r.Type == "Administrator" || r.Type == "Auditor").ToList(), "RoleTypeId", "Type");
            ViewBag.Branch = new SelectList(db.Branches, "BranchID", "BranchName");
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
                if(role == "Officer in Charge" && branch == "*")
                {
                    ModelState.AddModelError("", "Officer in Charge must be assigned to a branch");
                    ViewBag.RoleType = new SelectList(db.RoleType.Where(r => r.Type == "Officer in Charge" || r.Type == "Administrator" || r.Type == "Auditor").ToList(), "RoleTypeId", "Type");
                    ViewBag.Branch = new SelectList(db.Branches, "BranchID", "BranchName");
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

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, employee = employee, isActive = true};
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
                    return RedirectToAction("AdminIndex", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            ViewBag.RoleType = new SelectList(db.RoleType.Where(r => r.Type == "Officer in Charge" || r.Type == "Administrator" || r.Type == "Auditor").ToList(), "RoleTypeId", "Type");
            ViewBag.Branch = new SelectList(db.Branches, "BranchID", "BranchName");
            return View(model);
        }

    
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Patients");
        }
        #endregion
    }
}