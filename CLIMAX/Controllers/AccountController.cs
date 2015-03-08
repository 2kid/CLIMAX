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

namespace CLIMAX.Controllers
{
    [Authorize]
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
                    return RedirectToLocal(returnUrl);
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
        [Authorize(Roles="Admin")]
        public ActionResult Register()
        {
            ViewBag.RoleType = new SelectList(db.RoleType.Where(r => r.Type == "Officer in Charge" || r.Type == "Administrator" || r.Type == "Auditor").ToList(), "RoleTypeId", "Type");
            ViewBag.Branch = new SelectList(db.Branches, "BranchID", "BranchName");
            return View();
        }

        //
        // POST: /Account/Register
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
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

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, employee = employee};
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
               
                    return RedirectToAction("Index", "Home");
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
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}