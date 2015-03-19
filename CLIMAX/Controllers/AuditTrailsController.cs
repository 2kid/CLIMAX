using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLIMAX.Models;

namespace CLIMAX.Controllers
{
    [Authorize]
    public class AuditTrailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AuditTrails
        public ActionResult Index()
        {
            var auditTrail = db.AuditTrail.Include(a => a.actionType).Include(a => a.employee);
            return View(auditTrail.ToList()); 
            
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
