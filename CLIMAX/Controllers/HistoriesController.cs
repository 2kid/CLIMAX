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
    public class HistoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Histories
        public ActionResult Index(int id)
        {
            var history = db.History.Include(h => h.employee).Include(h => h.patient).Where(r=>r.PatientID == id);
            ViewBag.PatientID = id;
            return View(history.ToList());
        }

        // GET: Histories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            History history = db.History.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            return View(history);
        }

        static int patientID;
        static int adminEmpID;
        // GET: Histories/Create
        //[Authorize(Roles="OIC,Auditor")]
        //public ActionResult Create(int id)
        //{
        //    adminEmpID = db.Users.Where(r=>r.UserName == "admin@yahoo.com").Select(u=>u.EmployeeID).Single();
        //    ViewBag.EmployeeID = new SelectList(db.Employees.Where(r=>r.EmployeeID != adminEmpID).ToList(), "EmployeeID", "LastName");
        //    ViewBag.TreatmentID = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName");
        //    ViewBag.Patient = db.Patients.Find(id).FullName;
        //    patientID = id;
        //    return View();
        //}

        //// POST: Histories/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "OIC,Auditor")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "HistoryID,SessionNo,TreatmentID,EmployeeID,DateTimeStart,DateTimeEnd")] History history)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        adminEmpID = db.Users.Where(r => r.UserName == "admin@yahoo.com").Select(u => u.EmployeeID).Single();
        //        history.PatientID = patientID;
        //        if (!Regex.IsMatch(history.DateTimeStart.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
        //        {
        //            ModelState.AddModelError("DateTimeStart", "The field Date & Time Start is invalid");
        //            ViewBag.EmployeeID = new SelectList(db.Employees.Where(r => r.EmployeeID != adminEmpID).ToList(), "EmployeeID", "LastName", history.EmployeeID);
        //            ViewBag.TreatmentID = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName", history.TreatmentID);
        //            ViewBag.Patient = db.Patients.Find(patientID).FullName; 
        //            return View(history);
        //        }

        //        if (!Regex.IsMatch(history.DateTimeEnd.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
        //        {
        //            ModelState.AddModelError("DateTimeEnd", "The field Date & Time End is invalid");
        //            ViewBag.EmployeeID = new SelectList(db.Employees.Where(r => r.EmployeeID != adminEmpID).ToList(), "EmployeeID", "LastName", history.EmployeeID);
        //            ViewBag.TreatmentID = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName", history.TreatmentID);
        //            ViewBag.Patient = db.Patients.Find(patientID).FullName;
        //            return View(history);
        //        }
        //        db.History.Add(history);
        //        String patient = db.Patients.Find(history.PatientID).FullName;
        //        int auditId = Audit.CreateAudit(patient, "Create", "History", User.Identity.Name);
        //        db.SaveChanges();
        //        Audit.CompleteAudit(auditId, history.HistoryID);
        //        return RedirectToAction("Index", "Histories", new { id = history.PatientID });
        //    }

        //    ViewBag.EmployeeID = new SelectList(db.Employees.Where(r => r.EmployeeID != adminEmpID).ToList(), "EmployeeID", "LastName", history.EmployeeID);
        //    ViewBag.TreatmentID = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName", history.TreatmentID);
        //    ViewBag.Patient = db.Patients.Find(patientID).FullName;
        //    return View(history);
        //}

      
        // GET: Histories/Edit/5
        [Authorize(Roles = "OIC,Auditor")]
        public ActionResult Confirm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            History history = db.History.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            patientID = history.PatientID;
            //adminEmpID = db.Users.Where(r => r.UserName == "admin@yahoo.com").Select(u => u.EmployeeID).Single();
            //patientID = id.Value;
            //ViewBag.EmployeeID = new SelectList(db.Employees.Where(r => r.EmployeeID != adminEmpID).ToList(), "EmployeeID", "LastName", history.EmployeeID);
            //ViewBag.TreatmentID = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName",history.TreatmentID);
            //ViewBag.Patient = db.Patients.Find(patientID).FullName;
            return View(history);
        }

        // POST: Histories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "OIC,Auditor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm([Bind(Include = "HistoryID,TreatmentID,EmployeeID,ChargeSlipID,DateTimeStart")] History history)
        {
            if (ModelState.IsValid)
            {
                 history.PatientID = patientID;
                 history.DateTimeEnd = DateTime.Now;
                db.Entry(history).State = EntityState.Modified;
                String patient = db.Patients.Find(history.PatientID).FullName;
                int auditId = Audit.CreateAudit(patient, "Edit", "History", User.Identity.Name);
                Audit.CompleteAudit(auditId, history.HistoryID);
                 db.SaveChanges();
                 return RedirectToAction("Index", "Histories", new { id = history.PatientID });
            }
            return View(history);
        }

        //// GET: Histories/Disable/5
        //[Authorize(Roles = "OIC,Auditor")]
        //public ActionResult Disable(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    History history = db.History.Find(id);
        //    if (history == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(history);
        //}

        //// POST: Histories/Disable/5
        //[Authorize(Roles = "OIC,Auditor")]
        //[HttpPost, ActionName("Disable")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DisableConfirmed(int id)
        //{
        //    History history = db.History.Find(id);
        //    db.History.Remove(history);
        //    String patient = db.Patients.Find(history.PatientID).FullName;
        //    int auditId = Audit.CreateAudit(patient, "Disable", "History", User.Identity.Name);
        //    Audit.CompleteAudit(auditId, history.HistoryID);
        //    db.SaveChanges();
        //    return RedirectToAction("Index", "Histories", new { id = history.PatientID });
        //}

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
