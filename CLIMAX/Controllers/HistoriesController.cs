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

        // GET: Histories/Create
        public ActionResult Create(int id)
        {
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName");
            ViewBag.Patient = db.Patients.Find(id).FullName;
            return View();
        }

        // POST: Histories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HistoryID,SessionNo,PatientID,TreatmentID,EmployeeID,DateTimeStart,DateTimeEnd")] History history)
        {
            if (ModelState.IsValid)
            {
                if (!Regex.IsMatch(history.DateTimeStart.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeStart", "The field Date & Time Start is invalid");
                    ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", history.EmployeeID);
                    ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", history.PatientID);
                    return View(history);
                }

                if (!Regex.IsMatch(history.DateTimeEnd.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeEnd", "The field Date & Time End is invalid");
                    ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", history.EmployeeID);
                    ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", history.PatientID);
                    return View(history);
                }
                db.History.Add(history);
                db.SaveChanges();
                String patient = db.Patients.Find(history.PatientID).FullName;
                Audit.CreateAudit(patient, "Create", "History", history.HistoryID, User.Identity.Name);
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", history.EmployeeID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", history.PatientID);
            return View(history);
        }

        // GET: Histories/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", history.EmployeeID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", history.PatientID);
            return View(history);
        }

        // POST: Histories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HistoryID,SessionNo,PatientID,TreatmentID,EmployeeID,DateTimeStart,DateTimeEnd")] History history)
        {
            if (ModelState.IsValid)
            {
                if (!Regex.IsMatch(history.DateTimeStart.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeStart", "The field Date & Time Start is invalid");
                    ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", history.EmployeeID);
                    ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", history.PatientID);
                    return View(history);
                }

                if (!Regex.IsMatch(history.DateTimeEnd.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeEnd", "The field Date & Time End is invalid");
                    ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", history.EmployeeID);
                    ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", history.PatientID);
                    return View(history);
                }
                db.Entry(history).State = EntityState.Modified;
                String patient = db.Patients.Find(history.PatientID).FullName;
                Audit.CreateAudit(patient, "Edit", "History", history.HistoryID, User.Identity.Name);
                 db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", history.EmployeeID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", history.PatientID);
            return View(history);
        }

        // GET: Histories/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Histories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            History history = db.History.Find(id);
            db.History.Remove(history);
            String patient = db.Patients.Find(history.PatientID).FullName;
            Audit.CreateAudit(patient, "Delete", "History", history.HistoryID, User.Identity.Name);

            db.SaveChanges();
            return RedirectToAction("Index");
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
