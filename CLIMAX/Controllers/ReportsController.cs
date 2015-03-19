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
using System.Text.RegularExpressions;

namespace CLIMAX.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static int employeeID;
        private static string employeeName;
        private static bool isPDF = false;
        public ActionResult GeneratePDFPatients(string reportString, int BranchID)//Reports report)
        {
            isPDF = true;
            employeeName = db.Employees.Find(employeeID).FullName;
            return new Rotativa.ActionAsPdf("Index", new { reportString = reportString, BranchID = BranchID });
        }

        [AllowAnonymous]
        public ActionResult Index(string reportString, int BranchID, FormCollection form)
        {
           
            Reports report = JsonConvert.DeserializeObject<Reports>(reportString);
            int auditId;
            string reportType = db.ReportTypes.Find(report.ReportTypeID).Type;
            if (isPDF)
            {
                report.EmployeeID = employeeID;
                //add audit
                auditId = Audit.CreateAudit("Generated For " + employeeName, "PDF", "Reports", User.Identity.Name);
            }
            else
            {
                report.EmployeeID = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.EmployeeID).SingleOrDefault();
                employeeID = report.EmployeeID;
                isPDF = false;
                auditId = Audit.CreateAudit(reportType, "Create", "Reports", User.Identity.Name);
       
            }
            db.Reports.Add(report);
            db.SaveChanges();
            if (User.IsInRole("OIC"))
            {
                BranchID = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee.BranchID).Single();
            }
            if (isPDF)
            {
                Audit.CompleteAudit(auditId,report.ReportsID);
                employeeID = 0;
                employeeName = null;
                ViewBag.isPDF = isPDF;
                isPDF = false;
            }
            else
            {
                Audit.CompleteAudit(auditId, report.ReportsID);
                ViewBag.isPDF = isPDF;             
            }
            ViewBag.Type = reportType;
            ViewBag.Branch = db.Branches.Find(BranchID).BranchName;
            ViewBag.Employee = db.Employees.Find(report.EmployeeID).FullName;
            if (reportType == "Sales Report")
            {
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


                    chargeSlip.First(r => r.ChargeSlipID == model.ChargeSlipID).Therapist = model.Employee.FullName;
                    chargeSlip.First(r => r.ChargeSlipID == model.ChargeSlipID).Patient = model.Patient.FullName;
                    chargeSlip.Last().Total = individualTotalAmount;
                    chargeSlip.Last().DiscountAmount = model.AmtDiscount;
                    chargeSlip.Last().AmountDue = model.AmtDue;
                    totalRevenue += model.AmtDue;

                }
                chargeSlip.Add(new ChargeSlipViewModel() { TreatmentAmount = totalTreatmentAmount, MedicineAmount = totalMedicineAmount, Total = totalTreatmentAmount + totalMedicineAmount, AmountDue = totalRevenue });
                ViewBag.Transactions = chargeSlip;
            }
            else if (reportType == "Summary Report")
            {
                List<SummaryReportViewModel> chargeSlip = new List<SummaryReportViewModel>();

                double totalGross = 0;
                double totalRevenue = 0;
                foreach (ChargeSlip model in db.ChargeSlips.Where(r => report.DateStartOfReport.CompareTo(r.DateTimePurchased) == -1 && report.DateEndOfReport.CompareTo(r.DateTimePurchased) == 1).ToList())
                {
                    chargeSlip.Add(new SummaryReportViewModel()
                    {
                        Patient = model.Patient.FullName,
                        CardType = model.CardType,
                        GrossAmount = model.AmtDue + model.AmtDiscount.Value,
                        Net = model.AmtDue
                    });

                    totalGross += model.AmtDue + model.AmtDiscount.Value;
                    totalRevenue += model.AmtDue;
                }
                chargeSlip.Add(new SummaryReportViewModel() { GrossAmount = totalGross, Net = totalRevenue });
                ViewBag.Summary = chargeSlip;
            }
            else if (reportType == "Inventory Report")
            {
                List<InventoryReportsViewModel> iReport = new List<InventoryReportsViewModel>();

                foreach (Materials item in db.Materials.ToList())
                {

                    int? currentQty = db.Inventories.Where(r => r.MaterialID == item.MaterialID && r.BranchID == BranchID).Select(u => u.QtyInStock).SingleOrDefault();
                    if (currentQty != null)
                        iReport.Add(new InventoryReportsViewModel() { MaterialID = item.MaterialID, Medicine = item.MaterialName, Balance = currentQty.Value });
                }          
                
                    foreach (InventoryReportsViewModel item in iReport)
                    {
                        int sold = 0;
                        foreach (AuditTrail audit in db.AuditTrail.Include(a => a.actionType).Where(r => report.DateStartOfReport.CompareTo(r.DateTimeOfAction) == -1 && r.actionType.AffectedRecord == "ChargeSlip").ToList()) //&& report.DateEndOfReport.CompareTo(r.DateTimeOfAction) == 1).ToList())
                        {
                            foreach (Session_ChargeSlip session in db.Session_ChargeSlip.Where(r => r.ChargeSlipID == audit.RecordID).ToList())
                            {
                                MaterialList b = db.MaterialList.Where(r => r.TreatmentID == session.TreatmentID && r.MaterialID == item.MaterialID).SingleOrDefault();
                                if (b != null)
                                {
                                    sold += b.Qty * session.Qty;
                                    if (report.DateEndOfReport.CompareTo(audit.DateTimeOfAction) == 1)
                                    {
                                        if (item.Control == null)
                                        {
                                            item.Control = item.Balance;
                                        }
                                        item.Control += b.Qty * session.Qty;
                                    }
                                    else if (report.DateEndOfReport.CompareTo(audit.DateTimeOfAction) == -1)
                                    {
                                        item.Balance += b.Qty * session.Qty;
                                    }
                                }
                            }

                            foreach (int c in db.Medicine_ChargeSlip.Where(r => r.ChargeSlipID == audit.RecordID && r.MaterialID == item.MaterialID).Select(u => u.Qty).ToList())
                            {
                                sold += c;
                                if (report.DateEndOfReport.CompareTo(audit.DateTimeOfAction) == 1)
                                {
                                    if (item.Control == null)
                                    {
                                        item.Control = item.Balance;
                                    }
                                    item.Control += c;
                                }
                                else if (report.DateEndOfReport.CompareTo(audit.DateTimeOfAction) == -1)
                                {
                                    item.Balance += c;
                                }
                            }                          
                        }
                        item.Sold = sold;
                    }
                 
                    foreach (InventoryReportsViewModel item in iReport)
                    {
                        int add = 0;
                        int removed = 0;
                        foreach (AuditTrail audit in db.AuditTrail.Include(a => a.actionType).Where(r => report.DateStartOfReport.CompareTo(r.DateTimeOfAction) == -1 && r.actionType.AffectedRecord == "Inventory").OrderBy(b=>b.DateTimeOfAction).ToList())
                        {
                            if (audit.actionType.Action == "Edit")
                            {

                                Inventory inv = db.Inventories.Find(audit.RecordID);
                                string[] array = audit.ActionDetail.Split('-');
                                if (array[1] == "Added")
                                {
                                    add += int.Parse(array[0]);
                                    if (report.DateEndOfReport.CompareTo(audit.DateTimeOfAction) == 1)
                                    {
                                        if (item.Control == null)
                                        {
                                            item.Control = item.Balance;
                                        }
                                        item.Control -= int.Parse(array[0]);
                                    }
                                    else if (report.DateEndOfReport.CompareTo(audit.DateTimeOfAction) == -1)
                                    {
                                        item.Balance -= int.Parse(array[0]);
                                    }

                                }
                                else if (array[1] == "Removed")
                                {
                                    removed += int.Parse(array[0]);
                                    if (report.DateEndOfReport.CompareTo(audit.DateTimeOfAction) == 1)
                                    {
                                        if (item.Control == null)
                                        {
                                            item.Control = item.Balance;
                                        }
                                        item.Control += int.Parse(array[0]);
                                    }
                                    else if (report.DateEndOfReport.CompareTo(audit.DateTimeOfAction) == -1)
                                    {
                                        item.Balance += int.Parse(array[0]);
                                    }
                                }
                            }
                        }
                        item.Add = add;
                        item.Remove = removed;
                    }
                ViewBag.Inventories = iReport;
            }
            return View(report);
        }


        static int BranchID;
        // GET: Reports/Create
        public ActionResult Create()
        {
            BranchID = 0;
            ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "Type");
            if (User.IsInRole("OIC"))
            {
                ApplicationUser user = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Single();
                BranchID = user.employee.BranchID;
                ViewBag.Branch = user.employee.Branch.BranchName;
            }
            else
            {
                ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            }

            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportsID,ReportTypeID")] Reports reports, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string startDate = form["start"];
                string endDate = form["end"];
                if (!string.IsNullOrEmpty(startDate))
                    reports.DateStartOfReport = DateTime.Parse(startDate);
                if (!string.IsNullOrEmpty(endDate))
                    reports.DateEndOfReport = DateTime.Parse(endDate);

                if (!Regex.IsMatch(reports.DateStartOfReport.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateStartOfReport", "The field Date Start is invalid");
                    ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "Type", reports.ReportTypeID);
                    return View(reports);
                }

                if (!Regex.IsMatch(reports.DateEndOfReport.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateEndOfReport", "The field Date End is invalid");
                    ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "Type", reports.ReportTypeID);
                    return View(reports);
                }

                if (reports.DateEndOfReport.CompareTo(reports.DateStartOfReport) == -1)
                {
                    ModelState.AddModelError("", "Date End cannot be before Date Start");
                    ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "Type", reports.ReportTypeID);
                    return View(reports);
                }

                int branchId;
                if (int.TryParse(form["BranchID"], out branchId))
                {
                    return RedirectToAction("Index", new { reportString = JsonConvert.SerializeObject(reports), BranchID = branchId });
                }
                else
                {
                    if (User.IsInRole("OIC"))
                    {
                        return RedirectToAction("Index", new { reportString = JsonConvert.SerializeObject(reports), BranchID = BranchID });
                    }
                    ModelState.AddModelError("", "Invalid Branch");
                }
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
