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
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Text;

namespace CLIMAX.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static int employeeID;
        private static string employeeName;

        public ActionResult GenerateExcelReports(string reportType)//, int BranchID)//Reports report)
        {
            GridView gv = new GridView();
            if (reportType == "Sales Report")
            {
                gv.DataSource = chargeSlip;
            }
            else if (reportType == "Summary Report")
            {
                gv.DataSource = summaryReport;
            }
            else
            {
                gv.DataSource = iReport;
            }

            if (gv.DataSource != null)
            {
                gv.DataBind();

                System.Web.HttpContext.Current.Response.ClearContent();
                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.AddHeader(
               "content-disposition", string.Format("attachment; filename={0}", "Report.xls"));
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                System.Web.HttpContext.Current.Response.Charset = "UTF-8";
                System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                System.Web.HttpContext.Current.Response.Charset = "";

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                        gv.RenderControl(htw);
                        System.Web.HttpContext.Current.Response.Output.Write(sw.ToString());
                        System.Web.HttpContext.Current.Response.Flush();

                        StringBuilder Content = new StringBuilder();
                        double totalSales = 0;
                        double totalDiscount = 0;
                        double totalGiftCert = 0;
                        double totalRevenueTreatment = 0;
                        double totalRevenueMedicines = 0;
                        foreach (ChargeSlipContainerViewModel container in chargeSlip)
                        {
                            ChargeSlip cs = db.ChargeSlips.Find(container.ChargeSlipID);
                            Content.Append("<br /><table><tr><td>Patient: </td><td>" + cs.Patient.FullName + "</td><td>" + cs.DateTimePurchased + "</td></tr><tr><td>Therapist: </td><td>" + cs.Employee.FullName + "</td><td></td></tr>");

                            if (cs.GiftCertificateAmt != null)
                                totalGiftCert += cs.GiftCertificateAmt.Value;
                            totalSales += cs.AmtDue;
                            if (cs.AmtDiscount != null)
                                totalDiscount += cs.AmtDiscount.Value;
                            double subTotal = 0;
                            Content.Append("<tr><th>Quantity</th><th>Item</th><th>Amount</th></tr>");
                            foreach (Session_ChargeSlip session in db.Session_ChargeSlip.Include(a => a.treatment).Where(r => r.ChargeSlipID == cs.ChargeSlipID).ToList())
                            {
                                subTotal += session.treatment.TreatmentPrice * session.Qty;
                                Content.Append("<tr><td>" + session.Qty + "</td><td>" + session.treatment.TreatmentName + "</td><td>" + session.treatment.TreatmentPrice * session.Qty + "</td></tr>");
                                totalRevenueTreatment += (session.treatment.TreatmentPrice * session.Qty);
                            }
                            foreach (Medicine_ChargeSlip medicine in db.Medicine_ChargeSlip.Include(a => a.Materials).Where(r => r.ChargeSlipID == cs.ChargeSlipID).ToList())
                            {
                                subTotal += medicine.Materials.Price * medicine.Qty;
                                Content.Append("<tr><td>" + medicine.Qty + "</td><td>" + medicine.Materials.MaterialName + "</td><td>" + medicine.Materials.Price * medicine.Qty + "</td></tr>");
                                totalRevenueMedicines += (medicine.Materials.Price * medicine.Qty);
                            }
                            Content.Append("<tr><td>Sub-Total</td><td></td><td>" + subTotal + "</td></tr>");
                            Content.Append("<tr><td>Discount</td><td></td><td>" + cs.AmtDiscount + "</td></tr>");
                            Content.Append("<tr><td>Gift Certificate</td><td></td><td>" + cs.GiftCertificateAmt + "</td></tr>");
                            Content.Append("<tr><th>Total</th><th></th><th>" + cs.AmtDue + "</th></tr>");
                            Content.Append("<tr><th>Payment</th><th></th><th></th></tr>");
                            if (cs.ModeOfPayment == "Cash")
                            {
                                Content.Append("<tr><th>Cash</th><th></th><th>" + cs.AmtPayment + "</th></tr>");
                            }
                            else if (cs.ModeOfPayment == "Check")
                            {
                                Content.Append("<tr><td>CheckNo</td><td>" + cs.CheckNo + "</td><td></td></tr>");
                                Content.Append("<tr><th>Check Amount</th><th></th><th>" + cs.AmtPayment + "</th></tr>");
                            }
                            else
                            {
                                Content.Append("<tr><td>Card Type</td><td>" + cs.CardType + "</td><td></td></tr>");
                                Content.Append("<tr><td>Credit Amount</td><td></td><td>" + cs.AmtPayment + "</td></tr>");
                            }
                            Content.Append("<tr><td>Change</td><td></td><td>" + (cs.AmtPayment - cs.AmtDue) + "</th></tr>");
                        }
                        System.Web.HttpContext.Current.Response.Write("<h1>Dermstrata</h1><br />Total Revenue Treatments: " + totalRevenueTreatment + "<br />Total Revenue Medicines: " + totalRevenueMedicines + "<br />Total Discount: " + totalDiscount + "<br />Total Gift Certificate: " + totalGiftCert + "<br />Total Sales: " + totalSales);

                        System.Web.HttpContext.Current.Response.Write(Content);

                        System.Web.HttpContext.Current.Response.End();
                    }
                }

                return new EmptyResult();
            }

            return View();
        }
        static List<ChargeSlipContainerViewModel> chargeSlip;
        static SummaryReportContainerViewModel summaryReport;
        static List<InventoryReportsViewModel> iReport;
        public ActionResult Index(string reportString, int BranchID, FormCollection form)
        {

            Reports report = JsonConvert.DeserializeObject<Reports>(reportString);
            int auditId;
            string reportType = db.ReportTypes.Find(report.ReportTypeID).Type;
            chargeSlip = null;
            summaryReport = null;
            iReport = null;

            report.EmployeeID = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.EmployeeID).SingleOrDefault();
            employeeID = report.EmployeeID;
            auditId = Audit.CreateAudit(reportType, "Create", "Reports", User.Identity.Name);

            try
            {
                db.Reports.Add(report);
                db.SaveChanges();
            }
            catch
            {
                //error
                RedirectToAction("Create");
            }
            if (User.IsInRole("OIC"))
            {
                BranchID = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee.BranchID).Single();
            }
         
            Audit.CompleteAudit(auditId, report.ReportsID);
         
            ViewBag.Type = reportType;
            ViewBag.Branch = db.Branches.Find(BranchID).BranchName;
            ViewBag.Employee = db.Employees.Find(report.EmployeeID).FullName;
            if (reportType == "Sales Report")
            {
                chargeSlip = new List<ChargeSlipContainerViewModel>();

                double totalTreatmentAmount = 0;
                double totalMedicineAmount = 0;
                double totalRevenue = 0;
                double totalDiscountAmount = 0;
                double totalGiftCert = 0;
                List<ChargeSlipViewModel> items = new List<ChargeSlipViewModel>();

                foreach (ChargeSlip model in db.ChargeSlips.Where(r => report.DateStartOfReport.CompareTo(r.DateTimePurchased) == -1 && report.DateEndOfReport.CompareTo(r.DateTimePurchased) == 1).ToList())
                {

                    foreach (Session_ChargeSlip session in db.Session_ChargeSlip.Include(a => a.treatment).Where(r => r.ChargeSlipID == model.ChargeSlipID).ToList())
                    {
                        totalTreatmentAmount += session.treatment.TreatmentPrice * session.Qty;
                        items.Add(new ChargeSlipViewModel() { Treatment = session.treatment.TreatmentName, TreatmentQty = session.Qty, TreatmentAmount = session.treatment.TreatmentPrice * session.Qty });
                    }
                    foreach (Medicine_ChargeSlip medicine in db.Medicine_ChargeSlip.Include(a => a.Materials).Where(r => r.ChargeSlipID == model.ChargeSlipID).ToList())
                    {
                        totalMedicineAmount += medicine.Materials.Price * medicine.Qty;
                        items.Add(new ChargeSlipViewModel() { Medicine = medicine.Materials.MaterialName, MedicineQty = medicine.Qty, MedicineAmount = medicine.Materials.Price * medicine.Qty });
                    }

                    chargeSlip.Add(new ChargeSlipContainerViewModel() { ChargeSlipID = model.ChargeSlipID, Patient = model.Patient.FullName, DateTimePurchased = model.DateTimePurchased, Therapist = model.Employee.FullName, DiscountAmount = model.AmtDiscount, Total = model.AmtDue, items = items , GiftCertificateAmt = model.GiftCertificateAmt});
                    items = new List<ChargeSlipViewModel>();
                    if(model.GiftCertificateAmt !=null)
                    totalGiftCert += model.GiftCertificateAmt.Value;
                    totalRevenue += model.AmtDue;
                    if (model.AmtDiscount != null)
                    totalDiscountAmount += model.AmtDiscount.Value;
                }

                ViewBag.TotalRevenueTreatments = totalTreatmentAmount;
                ViewBag.TotalRevenueMedicines = totalMedicineAmount;
                ViewBag.TotalDiscount = totalDiscountAmount;
                ViewBag.TotalGCAmount = totalGiftCert;
                ViewBag.TotalSales = totalRevenue;
                ViewBag.Transactions = chargeSlip;
            }
            else if (reportType == "Summary Report")
            {
                summaryReport = new SummaryReportContainerViewModel();

                double totalGross = 0;
                double totalRevenue = 0;
                int cardCount = 0;
                List<SummaryReportViewModel> sr = new List<SummaryReportViewModel>();
                foreach (ChargeSlip model in db.ChargeSlips.Where(r => report.DateStartOfReport.CompareTo(r.DateTimePurchased) == -1 && report.DateEndOfReport.CompareTo(r.DateTimePurchased) == 1).ToList())
                {
                    if (model.CardType != "")
                        cardCount++;

                    sr.Add(new SummaryReportViewModel()
                    {
                        Patient = model.Patient.FullName,
                        CardType = model.CardType,
                        GrossAmount = model.AmtDue + model.AmtDiscount.Value,
                        Net = model.AmtDue
                    });

                    totalGross += model.AmtDue + model.AmtDiscount.Value;
                    totalRevenue += model.AmtDue;
                }
                summaryReport.items = sr;
                summaryReport.TotalGrossAmount = totalGross;
                summaryReport.TotalNet = totalRevenue;
                summaryReport.CardTypeCount = cardCount;

                ViewBag.Summary = summaryReport;

            }
            else if (reportType == "Inventory Report")
            {
                iReport = new List<InventoryReportsViewModel>();

                foreach (Materials item in db.Materials.ToList())
                {

                    int? currentQty = db.Inventories.Where(r => r.MaterialID == item.MaterialID && r.BranchID == BranchID).Select(u => u.QtyInStock).SingleOrDefault();
                    if (currentQty != null)
                        iReport.Add(new InventoryReportsViewModel() { MaterialID = item.MaterialID, Medicine = item.MaterialName, Balance = currentQty.Value });
                }

                foreach (InventoryReportsViewModel item in iReport)
                {
                    int sold = 0;
                    foreach (AuditTrail audit in db.AuditTrail.Include(a => a.actionType).Where(r => report.DateStartOfReport.CompareTo(r.DateTimeOfAction) == -1 && r.actionType.AffectedRecord == "ChargeSlip" && report.DateEndOfReport.CompareTo(r.DateTimeOfAction) == 1).ToList())
                    {
                        foreach (Session_ChargeSlip session in db.Session_ChargeSlip.Where(r => r.ChargeSlipID == audit.RecordID).ToList())
                        {
                            MaterialList b = db.MaterialList.Where(r => r.TreatmentID == session.TreatmentID && r.MaterialID == item.MaterialID).SingleOrDefault();
                            if (b != null)
                            {
                                sold += b.Qty * session.Qty;
                            }
                        }

                        foreach (int c in db.Medicine_ChargeSlip.Where(r => r.ChargeSlipID == audit.RecordID && r.MaterialID == item.MaterialID).Select(u => u.Qty).ToList())
                        {
                            sold += c;
                        }
                    }
                    item.Sold = sold;
                    //   }

                    //     foreach (InventoryReportsViewModel item in iReport)
                    //    {
                    int add = 0;
                    int removed = 0;
                    foreach (AuditTrail audit in db.AuditTrail.Include(a => a.actionType).Where(r => report.DateStartOfReport.CompareTo(r.DateTimeOfAction) == -1 && r.actionType.AffectedRecord == "Inventory").OrderBy(b => b.DateTimeOfAction).ToList())
                    {
                        if (audit.actionType.Action == "Edit")
                        {

                            Inventory inv = db.Inventories.Find(audit.RecordID);
                            string[] array = audit.ActionDetail.Split('-');
                            if (array[2] == item.Medicine)
                            {
                                if (array[1] == "Added")
                                {
                                    add += int.Parse(array[0]);
                                }
                                else if (array[1] == "Removed")
                                {
                                    removed += int.Parse(array[0]);
                                }
                            }
                        }
                    }
                    item.Add = add;
                    item.Remove = removed;
                    item.Control = item.Balance + removed - add + item.Sold;
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
                ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName");
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
                DateTime startDate;// = form["start"];
                DateTime endDate;// = form["end"];
                if (DateTime.TryParse(form["start"], out startDate))
                    reports.DateStartOfReport = startDate;
                if (DateTime.TryParse(form["end"], out endDate))
                {
                    endDate = endDate.AddDays(1);
                    endDate = endDate.Subtract(new TimeSpan(1));
                    reports.DateEndOfReport = endDate;
                }

                if (!Regex.IsMatch(reports.DateStartOfReport.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("", "The field Date Start is invalid");
                    ViewBag.ReportTypeID = new SelectList(db.ReportTypes, "ReportTypeID", "Type", reports.ReportTypeID);
                    return View(reports);
                }

                if (!Regex.IsMatch(reports.DateEndOfReport.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("", "The field Date End is invalid");
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
