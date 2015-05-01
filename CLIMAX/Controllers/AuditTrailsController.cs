using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLIMAX.Models;
using System.Text.RegularExpressions;

namespace CLIMAX.Controllers
{
    [Authorize]
    public class AuditTrailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AuditTrails
        List<AuditTrail> trails;
        public ActionResult Index(FormCollection form,string page = "1")
        {
            var auditTrail = db.AuditTrail.Include(a => a.actionType).Include(a => a.employee);
            DateTime start; 
            bool startIsDate = DateTime.TryParse(form["DateTimeStart"],out start);
            DateTime end; 
            bool endIsDate = DateTime.TryParse(form["DateTimeEnd"],out end);
            string logType = form["searchValue"];

            if (startIsDate && endIsDate)
            {
                if (!Regex.IsMatch(start.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("", "The field start date is invalid");
                    return View(auditTrail.ToList());
                }

                if (!Regex.IsMatch(end.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("", "The field end date is invalid");
                    return View(auditTrail.ToList());
                }

                if (start.CompareTo(end) == 1)
                {
                    ModelState.AddModelError("", "Date started cannot be after the date ended");
                    return View(auditTrail.ToList());
                }
               end = end.AddDays(1);
               end = end.Subtract(new TimeSpan(1));
               auditTrail = auditTrail.Where(r => r.DateTimeOfAction.CompareTo(start) == 1 && end.CompareTo(r.DateTimeOfAction) == 1);
                
            }

            if(!string.IsNullOrEmpty(logType))
            {
                auditTrail = auditTrail.Where(r => r.actionType.AffectedRecord.ToLower() == logType.ToLower());
            }

            trails = auditTrail.ToList();
            int trailsPage;
            const int takeCount = 10;
            if(int.TryParse(page,out trailsPage))
            {
                List<List<AuditTrail>> alltrails = new List<List<AuditTrail>>();
                while(trails.Count != 0)
                {
                    alltrails.Add(new List<AuditTrail>(trails.Take(takeCount).ToList()));
                    if (trails.Count >= takeCount)
                        trails.RemoveRange(0, takeCount);
                    else
                        trails.RemoveRange(0, trails.Count);
                }
                ViewBag.CPage = trailsPage;
                ViewBag.LastPage = alltrails.Count;
                return View(alltrails[trailsPage - 1]);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            
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
