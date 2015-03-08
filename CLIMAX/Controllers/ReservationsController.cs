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
    public class ReservationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reservations
        public ActionResult Index()
        {
            var reservations = db.Reservations.Include(r => r.employee).Include(r => r.patient).Include(r => r.treatment).ToList();
            return View(reservations);
        }

        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // GET: Reservations/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeID = new SelectList(db.Employees.Include(a=>a.roleType).Where(r=>r.roleType.Type == "Therapist"), "EmployeeID", "FullName");
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FullName");
            ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text");
            ViewBag.Treatments = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReservationID,TreatmentID,ReservationType,DateTimeReserved,Notes,PatientID,EmployeeID")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                if (reservation.ReservationType && reservation.EmployeeID == null)
                {
                    ModelState.AddModelError("", "Please specify who will perform the surgery.");
                    ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FullName", reservation.EmployeeID);
                    ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FullName", reservation.PatientID);

                    ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text");
                    ViewBag.Treatments = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName");
                    return View(reservation);
                }
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FullName", reservation.EmployeeID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FullName", reservation.PatientID);

            ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text"); 
            ViewBag.Treatments = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName");
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FullName", reservation.EmployeeID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FullName", reservation.PatientID);
            ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text"); 
            ViewBag.Treatments = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName");
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservationID,TreatmentID,ReservationType,DateTimeReserved,Notes,PatientID,EmployeeID")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                if (reservation.ReservationType && reservation.EmployeeID == null)
                {
                    ModelState.AddModelError("", "Please specify who will perform the surgery.");
                    ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FullName", reservation.EmployeeID);
                    ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FullName", reservation.PatientID);

                    ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text");
                    ViewBag.Treatments = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName");
                    return View(reservation);
                }
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FullName", reservation.EmployeeID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FullName", reservation.PatientID);
            ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text");
            ViewBag.Treatments = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName");
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
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
