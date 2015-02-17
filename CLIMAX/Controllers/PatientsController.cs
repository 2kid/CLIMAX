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
    public class PatientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Patients
        public ActionResult Index(FormCollection form)
        {
            var patients = db.Patients.Include(p => p.branch).Include(p => p.company).ToList();

            string search = form["searchValue"];
            if (!string.IsNullOrEmpty(search))
            {
                patients = patients.Where(r => r.FullName.ToLower().Contains(search.ToLower())).ToList();
            }

            return View(patients);
        }

        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // GET: Patients/Create
        public ActionResult Create()
        {
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "CompanyName");

            IEnumerable<SelectListItem> item = new List<SelectListItem>()
            {
                new SelectListItem(){
                    Text = "Female", Value = "false"},
              new SelectListItem(){
                    Text = "Male", Value = "true"}   
            };

            ViewBag.Gender = new SelectList(item,"Value","Text");
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatientID,FirstName,MiddleName,LastName,BirthDate,Gender,CivilStatus,Height,Weight,HomeNo,Street,City,LandlineNo,CellphoneNo,EmailAddress,Occupation,CompanyID,EmergencyContactNo,EmergencyContactFName,EmergencyContactMName,EmergencyContactLName,BranchID")] Patient patient)
        {

            if (ModelState.IsValid)
            {
                db.Patients.Add(patient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "CompanyName", patient.CompanyID);
            IEnumerable<SelectListItem> item = new List<SelectListItem>()
            {
                new SelectListItem(){
                    Text = "Female", Value = "false"},
              new SelectListItem(){
                    Text = "Male", Value = "true"}   
            };

            ViewBag.Gender = new SelectList(item, "Value", "Text");
            return View(patient);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "CompanyName", patient.CompanyID);
            IEnumerable<SelectListItem> item = new List<SelectListItem>()
            {
                new SelectListItem(){
                    Text = "Female", Value = "false"},
              new SelectListItem(){
                    Text = "Male", Value = "true"}   
            };

            ViewBag.Gender = new SelectList(item, "Value", "Text");
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientID,FirstName,MiddleName,LastName,BirthDate,Gender,CivilStatus,Height,Weight,HomeNo,Street,City,LandlineNo,CellphoneNo,EmailAddress,Occupation,CompanyID,EmergencyContactNo,EmergencyContactFName,EmergencyContactMName,EmergencyContactLName,BranchID")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "CompanyName", patient.CompanyID);
            IEnumerable<SelectListItem> item = new List<SelectListItem>()
            {
                new SelectListItem(){
                    Text = "Female", Value = "false"},
              new SelectListItem(){
                    Text = "Male", Value = "true"}   
            };

            ViewBag.Gender = new SelectList(item, "Value", "Text");
            return View(patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
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
