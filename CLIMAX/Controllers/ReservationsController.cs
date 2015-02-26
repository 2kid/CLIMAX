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
        public ActionResult Index(FormCollection form)
        {
            var reservations = db.Reservations.Include(r => r.employee).Include(r => r.patient).Include(r => r.reservationType);

            string name = form["nameValue"];
            if(!string.IsNullOrEmpty(name))
            {
                reservations = reservations.Where(r => r.patient.FullName.ToLower().Contains(name));
            }

            string date = form["dateReserved"];
            //if (!string.IsNullOrEmpty(date))
            //{
                DateTime dateReserved;
                if (DateTime.TryParse(date, out dateReserved))
                {
                    reservations = reservations.Where(r => r.DateTimeReserved == dateReserved);
                }
            //}

            return View(reservations.ToList());
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
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName");
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName");
            ViewBag.ReservationTypeID = new SelectList(db.ReservationTypes, "ReservationTypeID", "reservationType");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReservationID,TreatmentID,ReservationTypeID,DateTimeReserved,Notes,PatientID,EmployeeID")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", reservation.EmployeeID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", reservation.PatientID);
            ViewBag.ReservationTypeID = new SelectList(db.ReservationTypes, "ReservationTypeID", "reservationType", reservation.ReservationTypeID);
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
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", reservation.EmployeeID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", reservation.PatientID);
            ViewBag.ReservationTypeID = new SelectList(db.ReservationTypes, "ReservationTypeID", "reservationType", reservation.ReservationTypeID);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservationID,TreatmentID,ReservationTypeID,DateTimeReserved,Notes,PatientID,EmployeeID")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", reservation.EmployeeID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", reservation.PatientID);
            ViewBag.ReservationTypeID = new SelectList(db.ReservationTypes, "ReservationTypeID", "reservationType", reservation.ReservationTypeID);
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
