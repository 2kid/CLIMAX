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
using Newtonsoft.Json;

namespace CLIMAX.Controllers
{
    [Authorize(Roles="OIC,Auditor")]
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

        public void setDropdownLists()
        {
            ViewBag.EmployeeID = new SelectList(db.Employees.Include(a => a.roleType).Where(r => r.roleType.Type == "Therapist"), "EmployeeID", "FullName");
            ViewBag.PatientID = new SelectList(db.Patients.Where(r=>r.isEnabled).ToList(), "PatientID", "FullName");
            ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text");
            ViewBag.ReservationTime = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "9AM", Value = "9"},  new SelectListItem(){
                                Text = "10AM", Value = "10"},  new SelectListItem(){
                                Text = "11AM", Value = "11"},  new SelectListItem(){
                                Text = "12PM", Value = "12"},  new SelectListItem(){
                                Text = "1PM", Value = "13"},  new SelectListItem(){
                                Text = "2PM", Value = "14"},  new SelectListItem(){
                                Text = "3PM", Value = "15"},  new SelectListItem(){
                                Text = "4PM", Value = "16"},  new SelectListItem(){
                                Text = "5PM", Value = "17"},  new SelectListItem(){
                                Text = "6PM", Value = "18"},  new SelectListItem(){
                                Text = "7PM", Value = "19"}, new SelectListItem(){
                                Text = "8PM", Value = "20"}, new SelectListItem(){
                                Text = "9PM", Value = "21"}, new SelectListItem(){
                                Text = "10PM", Value = "22"}, new SelectListItem(){
                                Text = "11PM", Value = "23"}}, "Value", "Text");

            ViewBag.Treatments = new SelectList(db.Treatments.Where(r=>r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
    
        }

        // GET: Reservations/Create
        public ActionResult Create()
        {
            setDropdownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReservationID,TreatmentID,ReservationType,DateTimeReserved,Notes,PatientID,EmployeeID")] Reservation reservation, FormCollection form)
        {

            if (ModelState.IsValid)
            {
                reservation.DateTimeReserved = DateTime.Parse(form["DateTimeReserved"]);
                double hours = double.Parse(form["ReservationTime"]);
                reservation.DateTimeReserved = reservation.DateTimeReserved.AddHours(hours);

                if (!Regex.IsMatch(reservation.DateTimeReserved.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeReserved", "The field Date Reserved is invalid");
                    setDropdownLists();
                    return View(reservation);
                }

                if (reservation.DateTimeReserved.CompareTo(DateTime.Now) == -1)
                {
                    ModelState.AddModelError("", "Date Reserved cannot be before today");
                    setDropdownLists();
                    return View(reservation);
                }

                if (reservation.ReservationType && reservation.EmployeeID == null)
                {
                    ModelState.AddModelError("", "Please specify who will perform the surgery.");
                    setDropdownLists();
                    return View(reservation);
                }
                db.Reservations.Add(reservation);
                string patient = db.Patients.Find(reservation.PatientID).FullName;
                int auditId = Audit.CreateAudit(patient, "Create", "Reservation", User.Identity.Name);
                db.SaveChanges();
                Audit.CompleteAudit(auditId, reservation.ReservationID);
                return RedirectToAction("Index");
            }

            setDropdownLists();
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
            setDropdownLists();
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservationID,TreatmentID,ReservationType,Notes,PatientID,EmployeeID")] Reservation reservation,FormCollection form)
        {
            if (ModelState.IsValid)
            {
                reservation.DateTimeReserved = DateTime.Parse(form["DateTimeReserved"]);
                double hours = double.Parse(form["ReservationTime"]);
                reservation.DateTimeReserved = reservation.DateTimeReserved.AddHours(hours);

                if (!Regex.IsMatch(reservation.DateTimeReserved.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeReserved", "The field Date Reserved is invalid");
                    ViewBag.EmployeeID = new SelectList(db.Employees.Where(r=>r.isEnabled).ToList(), "EmployeeID", "FullName", reservation.EmployeeID);
                    ViewBag.PatientID = new SelectList(db.Patients.Where(r=>r.isEnabled).ToList(), "PatientID", "FullName", reservation.PatientID);

                    ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text");
                    ViewBag.Treatments = new SelectList(db.Treatments.Where(r=>r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                    return View(reservation);
                }
                if (reservation.DateTimeReserved.CompareTo(DateTime.Now) == -1)
                {
                    ModelState.AddModelError("", "Date Reserved cannot be before today");
                    ViewBag.EmployeeID = new SelectList(db.Employees.Where(r=>r.isEnabled).ToList(), "EmployeeID", "FullName", reservation.EmployeeID);
                    ViewBag.PatientID = new SelectList(db.Patients.Where(r=>r.isEnabled).ToList(), "PatientID", "FullName", reservation.PatientID);

                    ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text");
                    ViewBag.Treatments = new SelectList(db.Treatments.Where(r=>r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                    return View(reservation);
                }

                if (reservation.ReservationType && reservation.EmployeeID == null)
                {
                    ModelState.AddModelError("", "Please specify who will perform the surgery.");
                    ViewBag.EmployeeID = new SelectList(db.Employees.Where(r=>r.isEnabled).ToList(), "EmployeeID", "FullName", reservation.EmployeeID);
                    ViewBag.PatientID = new SelectList(db.Patients.Where(r=>r.isEnabled).ToList(), "PatientID", "FullName", reservation.PatientID);

                    ViewBag.ReservationType = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Treatment", Value = "false"},
                          new SelectListItem(){
                                Text = "Surgical", Value = "true"}   
                        }, "Value", "Text");
                    ViewBag.Treatments = new SelectList(db.Treatments.Where(r=>r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                    return View(reservation);
                }
                db.Entry(reservation).State = EntityState.Modified;
                string patient = db.Patients.Find(reservation.PatientID).FullName;
                int auditId = Audit.CreateAudit(patient, "Edit", "Reservation", User.Identity.Name);
                db.SaveChanges();
                Audit.CompleteAudit(auditId, reservation.ReservationID);
                return RedirectToAction("Index");
            }
            setDropdownLists();
            return View(reservation);
        }

        // GET: Reservations/Disable/5
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

        // POST: Reservations/Disable/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
            string patient = db.Patients.Find(reservation.PatientID).FullName;
            int auditId = Audit.CreateAudit(patient, "Disable", "Reservation", User.Identity.Name);
            db.SaveChanges();
            Audit.CompleteAudit(auditId, reservation.ReservationID);
            return RedirectToAction("Index");
        }


        public ActionResult Calendar()
        {
            var events = db.Reservations.ToList();

            var clientList = new List<object>();
            foreach (var e in events)
            {
                string type = "";
                if (e.ReservationType)
                {
                    type = "surgical";
                }
                else
                {
                    type = "treatment";
                }
                clientList.Add(
                    new
                    {
                        id = e.ReservationID.ToString(),
                        title = type,
                        description = "Reserved for: " + e.patient.FullName,// + "\n" + e.Notes,
                        start = e.DateTimeReserved.ToString(),
                        end = e.DateTimeReserved.AddHours(1).ToString()
                    });
            }

            ViewBag.Reservations = JsonConvert.SerializeObject(clientList);
            return View();
        }

        [HttpGet]
        public JsonResult GetEvents(double start, double end)
        {
            var fromDate = ConvertFromUnixTimestamp(start);
            var toDate = ConvertFromUnixTimestamp(end);
            //var epoch = new DateTime(1970, 1, 1);
            //var fromDate = epoch.AddMilliseconds(start);
            //var toDate = epoch.AddMilliseconds(end);
            var events = db.Reservations.Where(r => fromDate.CompareTo(r.DateTimeReserved) == -1 && toDate.CompareTo(r.DateTimeReserved) == 1).ToList();//repository.GetEvents(fromDate, toDate);

            var clientList = new List<object>();
            foreach (var e in events)
            {
                string type = "";
                if(e.ReservationType)
                {
                   type = "surgical";
                }
                else
                {
                    type = "treatment";
                }
                clientList.Add(
                    new
                    {
                        id = e.ReservationID,
                        title = type,
                        description = "Reserved for: "+ e.patient.FullName + "\n" + e.Notes,
                        start = e.DateTimeReserved,
                        end = e.DateTimeReserved.AddHours(1),
                        allDay = false
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
