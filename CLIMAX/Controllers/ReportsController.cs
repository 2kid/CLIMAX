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
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult GeneratePDFPatients()
        {
            return new Rotativa.ActionAsPdf("Index");
        }
       
        
        // GET: Reports
        public ActionResult Index(Reports report, FormCollection form)
        {
            if (report.reportType.Type == "Transactional")
            {
                var reports = db.Reports.Include(r => r.employee).Include(r => r.reportType);

            }
            else if (report.reportType.Type == "Patients")
            {
                var patients = db.Patients.Include(p => p.branch).Include(p => p.company).ToList();

                return View(patients);
            }

            return View();
            //var reports = db.Reports.Include(r => r.employee).Include(r => r.reportType);

            //string employeeId = form["employeeId"];
            //string reportTypeId = form["reportTypeId"];
            //return View(reports.ToList());
        }


        // GET: Reports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reports reports = db.Reports.Find(id);
            if (reports == null)
            {
                return HttpNotFound();
            }
            return View(reports);
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName");
            ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "Type");

            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportsID,ReportTypeID,EmployeeID")] Reports reports,FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string startDate = form["start"];
                string endDate = form["end"];
                if(!string.IsNullOrEmpty(startDate))
                    reports.DateStartOfReport = DateTime.Parse(startDate);
                if (!string.IsNullOrEmpty(endDate))
                    reports.DateEndOfReport = DateTime.Parse(endDate);

                reports.EmployeeID = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.EmployeeID).SingleOrDefault();
                db.Reports.Add(reports);
                db.SaveChanges();
                return RedirectToAction("Index", new { Report = reports });
                // return RedirectToAction("Index"); //details
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", reports.EmployeeID);
            ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "Type", reports.ReportTypeID);
            return View(reports);
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reports reports = db.Reports.Find(id);
            if (reports == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", reports.EmployeeID);
            ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "reportType", reports.ReportTypeID);
            return View(reports);
        }

        // POST: Reports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReportsID,ReportTypeID,EmployeeID,DateTimeGenerated")] Reports reports)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reports).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", reports.EmployeeID);
            ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "reportType", reports.ReportTypeID);
            return View(reports);
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
