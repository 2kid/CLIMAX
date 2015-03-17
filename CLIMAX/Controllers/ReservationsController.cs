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
            ViewBag.EmployeeID = new SelectList(db.Employees.Include(a => a.roleType).Where(r => r.roleType.Type == "Therapist"), "EmployeeID", "FullName");
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
                if (!Regex.IsMatch(reservation.DateTimeReserved.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeReserved", "The field Date Reserved is invalid");
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
                if (reservation.DateTimeReserved.CompareTo(DateTime.Now) == -1)
                {
                    ModelState.AddModelError("", "Date Reserved cannot be before today");
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
                string patient = db.Patients.Find(reservation.PatientID).FullName;
                Audit.CreateAudit(patient, "Create", "Reservation", reservation.ReservationID, User.Identity.Name);

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
                if (!Regex.IsMatch(reservation.DateTimeReserved.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeReserved", "The field Date Reserved is invalid");
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
                if (reservation.DateTimeReserved.CompareTo(DateTime.Now) == -1)
                {
                    ModelState.AddModelError("", "Date Reserved cannot be before today");
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
                string patient = db.Patients.Find(reservation.PatientID).FullName;
                Audit.CreateAudit(patient, "Edit", "Reservation", reservation.ReservationID, User.Identity.Name);

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
            string patient = db.Patients.Find(reservation.PatientID).FullName;
            Audit.CreateAudit(patient, "Delete", "Reservation", reservation.ReservationID, User.Identity.Name);

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Calendar()
        {
            return View();
        }

        public JsonResult GetEvents(double start, double end)
        {       
            var fromDate = ConvertFromUnixTimestamp(start);
            var toDate = ConvertFromUnixTimestamp(end);
            var events = db.Reservations.Where(r => fromDate.CompareTo(r.DateTimeReserved) == -1 && toDate.CompareTo(r.DateTimeReserved) == 1).ToList();//repository.GetEvents(fromDate, toDate);

            var clientList = new List<object>();
            foreach (var e in events)
            {
                clientList.Add(
                    new
                    {
                        id = e.ReservationID,
                        title = e.ReservationType.ToString(),
                        description = "Reserved for: "+ e.patient.FullName + "\n" + e.Notes,
                        start = e.DateTimeReserved,
                        end = e.DateTimeReserved.AddHours(1),
                    });
            }
            return Json(clientList.ToArray(), JsonRequestBehavior.AllowGet);

        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
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
