using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLIMAX.Models;
using Newtonsoft.Json;

namespace CLIMAX.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult GeneratePDFPatients()
        {
            return new Rotativa.ActionAsPdf("Index");
        }

     
        // GET: Reports
        public ActionResult Index(string reportString, FormCollection form)
        {
            Reports report = JsonConvert.DeserializeObject<Reports>(reportString);
            report.EmployeeID = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.EmployeeID).SingleOrDefault();
            db.Reports.Add(report);
            db.SaveChanges();
            string reportType = db.ReportTypes.Find(report.ReportTypeID).Type;
            Audit.CreateAudit(reportType, "Create", "Reports", report.ReportsID, User.Identity.Name);

            if (reportType == "Transactional")
            {
                ViewBag.Type = "Sales Report";
                List<ChargeSlipViewModel> chargeSlip = new List<ChargeSlipViewModel>();

                double totalTreatmentAmount = 0;
                double totalMedicineAmount = 0;
                double totalRevenue = 0;
                foreach (ChargeSlip model in db.ChargeSlips.Where(r => report.DateStartOfReport.CompareTo(r.DateTimePurchased) == -1 && report.DateEndOfReport.CompareTo(r.DateTimePurchased) == 1).ToList())
                {
                    double individualTotalAmount = 0;
      
                    foreach (Session_ChargeSlip session in db.Session_ChargeSlip.Include(a => a.treatment).Where(r => r.ChargeSlipID == model.ChargeSlipID).ToList())
                    {
                        totalTreatmentAmount += session.treatment.TreatmentPrice * session.Qty;
                        individualTotalAmount += session.treatment.TreatmentPrice * session.Qty;
                        chargeSlip.Add(new ChargeSlipViewModel() { ChargeSlipID = session.ChargeSlipID, Treatment = session.treatment.TreatmentName, TreatmentQty = session.Qty, TreatmentAmount = session.treatment.TreatmentPrice * session.Qty });
                    }
                    foreach (Medicine_ChargeSlip medicine in db.Medicine_ChargeSlip.Include(a => a.Materials).Where(r => r.ChargeSlipID == model.ChargeSlipID).ToList())
                    {
                        totalMedicineAmount += medicine.Materials.Price * medicine.Qty;
                        individualTotalAmount += medicine.Materials.Price * medicine.Qty;
                        chargeSlip.Add(new ChargeSlipViewModel() { ChargeSlipID = medicine.ChargeSlipID, Medicine = medicine.Materials.MaterialName, MedicineQty = medicine.Qty, MedicineAmount = medicine.Materials.Price * medicine.Qty });
                    }

                    //chargeSlip.AddRange(db.Session_ChargeSlip.Include(a => a.treatment).Where(r => r.ChargeSlipID == model.ChargeSlipID).Select(u => new ChargeSlipViewModel() { ChargeSlipID = u.ChargeSlipID, Treatment = u.treatment.TreatmentName, TreatmentQty = u.Qty, TreatmentAmount = u. }).ToList());
                    //                    chargeSlip.AddRange(db.Medicine_ChargeSlip.Include(a => a.Materials).Where(r => r.ChargeSlipID == model.ChargeSlipID).Select(u => new ChargeSlipViewModel() { ChargeSlipID = u.ChargeSlipID, Medicine = u.Materials.MaterialName, MedicineQty = u.Qty, MedicineAmount = MedicineAmount }).ToList());
                    chargeSlip.First(r => r.ChargeSlipID == model.ChargeSlipID).Therapist = model.Employee.FullName;
                    chargeSlip.Last().Total = individualTotalAmount;
                    chargeSlip.Last().DiscountAmount = model.AmtDiscount;
                    chargeSlip.Last().AmountDue = model.AmtDue;
                    totalRevenue += model.AmtDue;
                }
                chargeSlip.Add(new ChargeSlipViewModel() { TreatmentAmount = totalTreatmentAmount, MedicineAmount = totalMedicineAmount, Total = totalTreatmentAmount + totalMedicineAmount, AmountDue = totalRevenue });
                ViewBag.Transactions = chargeSlip;
            }
            else if (reportType == "Patients")
            {
                //ViewBag.Type = "Treatment Report";
                //var patients = db.Patients.Include(p => p.branch).Include(p => p.company).ToList();

                //return View(patients);
            }

            return View(report);
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
            ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "Type");
            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportsID,ReportTypeID")] Reports reports,FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string startDate = form["start"];
                string endDate = form["end"];
                if (!string.IsNullOrEmpty(startDate))
                    reports.DateStartOfReport = DateTime.Parse(startDate);
                if (!string.IsNullOrEmpty(endDate))
                    reports.DateEndOfReport = DateTime.Parse(endDate);
 
                return RedirectToAction("Index", new { reportString = JsonConvert.SerializeObject(reports) });
                // return RedirectToAction("Index"); //details
            }

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
