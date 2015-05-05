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
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text;

namespace CLIMAX.Controllers
{
    [Authorize]
    public class ChargeSlipsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ChargeSlips
        public ActionResult Index(string paymentMethod, string searchValue, string DateTimeStart, string DateTimeEnd)
        {
            List<SelectListItem> paymentMethods = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "Cash", Value = "Cash"},
                new SelectListItem(){Text = "Check", Value = "Check"},
                new SelectListItem(){Text = "Credit Card", Value = "Credit Card"}
            };

            ViewBag.PaymentMethod = new SelectList(paymentMethods, "Value", "Text", paymentMethod);
            List<ChargeSlip> chargeslip = db.ChargeSlips.ToList();
            DateTime start;
            DateTime end;

            if (!string.IsNullOrEmpty(paymentMethod))
            {
                chargeslip = chargeslip.Where(r => r.ModeOfPayment == paymentMethod).ToList();
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                chargeslip = chargeslip.Where(r => r.GiftCertificateNo != null && r.GiftCertificateNo.Contains(searchValue)).ToList();

                ViewBag.GCNumber = searchValue;
            }

            if ((DateTime.TryParse(DateTimeStart, out start) && DateTime.TryParse(DateTimeEnd, out end)))
            {

                end = end.AddDays(1);
                end = end.Subtract(new TimeSpan(1));

                ViewBag.startDate = start.ToString("yyyy-MM-dd");
                ViewBag.endDate = end.ToString("yyyy-MM-dd");

                if (!Regex.IsMatch(start.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeStart", "The field Date start is invalid");
                    return View(chargeslip);
                }


                if (!Regex.IsMatch(end.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeEnd", "The field Date end is invalid");
                    return View(chargeslip);
                }

                if (end.CompareTo(start) == -1)
                {
                    ModelState.AddModelError("", "Date Start cannot be after Date End");
                    return View(chargeslip);
                }
                chargeslip = chargeslip.Where(r => end.CompareTo(r.DateTimePurchased) == 1 && r.DateTimePurchased.CompareTo(start) == 1).ToList();         
            }

            return View(chargeslip);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int id, FormCollection form)
        {
            List<SelectListItem> paymentMethods = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "Cash", Value = "Cash"},
                new SelectListItem(){Text = "Check", Value = "Check"},
                new SelectListItem(){Text = "Credit Card", Value = "Credit Card"}
            };

            ViewBag.PaymentMethod = new SelectList(paymentMethods, "Value", "Text", form["PaymentMethod"]);
            List<ChargeSlip> chargeslip = db.ChargeSlips.ToList();
            DateTime start;
            DateTime end;

            if (!string.IsNullOrEmpty(form["PaymentMethod"]))
            {
                chargeslip = chargeslip.Where(r => r.ModeOfPayment == form["PaymentMethod"]).ToList();
            }

            if (!string.IsNullOrEmpty(form["searchValue"]))
            {
                chargeslip = chargeslip.Where(r => r.GiftCertificateNo != null && r.GiftCertificateNo.Contains(form["searchValue"])).ToList();

                ViewBag.GCNumber = form["searchValue"];
            }

            if ((DateTime.TryParse(form["DateTimeStart"], out start) && DateTime.TryParse(form["DateTimeEnd"], out end)))
            {
                if (!Regex.IsMatch(start.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeStart", "The field Date start is invalid");
                    return View(chargeslip);
                }

                end = end.AddDays(1);
                end = end.Subtract(new TimeSpan(1));

                if (!Regex.IsMatch(end.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("DateTimeEnd", "The field Date end is invalid");
                    return View(chargeslip);
                }

                if (end.CompareTo(start) == -1)
                {
                    ModelState.AddModelError("", "Date Start cannot be after Date End");
                    return View(chargeslip);
                }
                chargeslip = chargeslip.Where(r => end.CompareTo(r.DateTimePurchased) == 1 && r.DateTimePurchased.CompareTo(start) == 1).ToList();

                ViewBag.startDate = start.ToString("yyyy-MM-dd");
                ViewBag.endDate = end.ToString("yyyy-MM-dd");
            }

            ChargeSlip cs = db.ChargeSlips.Find(id);          
                SurveyCode code = db.SurveyCode.Where(r => r.ChargeSlipID == cs.ChargeSlipID).Single();

                System.Web.HttpContext.Current.Response.ClearContent();
                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.AddHeader(
               "content-disposition", string.Format("attachment; filename={0}", "Reports-"+DateTime.Now.ToString("yyyy-MM-dd")+".xls"));
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                System.Web.HttpContext.Current.Response.Charset = "UTF-8";
                System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                System.Web.HttpContext.Current.Response.Charset = "";

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                       // gv.RenderControl(htw);
                        System.Web.HttpContext.Current.Response.Output.Write(sw.ToString());
                        System.Web.HttpContext.Current.Response.Flush();
                        System.Web.HttpContext.Current.Response.Write("Dermstrata");
                        System.Web.HttpContext.Current.Response.Write("&nbsp;&nbsp;&nbsp;<span>" + cs.DateTimePurchased + "</span><br>Patient: " + cs.Patient.FullName + "<br>Therapist:" + cs.Employee.FullName);

                        double subTotal = 0;
                        StringBuilder Content = new StringBuilder();
                        Content.Append("<table><tr><th>Quantity</th><th>Item</th><th>Amount</th></tr>");
                        foreach (Session_ChargeSlip session in db.Session_ChargeSlip.Include(a => a.treatment).Where(r => r.ChargeSlipID == id).ToList())
                        {
                            subTotal += session.treatment.TreatmentPrice * session.Qty;
                           Content.Append("<tr><td>"+ session.Qty +"</td><td>"+session.treatment.TreatmentName+"</td><td>"+ session.treatment.TreatmentPrice * session.Qty+"</td></tr>");
                        }
                        foreach (Medicine_ChargeSlip medicine in db.Medicine_ChargeSlip.Include(a => a.Materials).Where(r => r.ChargeSlipID == id).ToList())
                        {
                           subTotal += medicine.Materials.Price * medicine.Qty;
                            Content.Append("<tr><td>" + medicine.Qty + "</td><td>" + medicine.Materials.MaterialName + "</td><td>" + medicine.Materials.Price * medicine.Qty + "</td></tr>");
                        }
                        Content.Append("<tr><td>Sub-Total</td><td></td><td>"+ subTotal + "</td></tr>");
                        Content.Append("<tr><td>Discount</td><td></td><td>" + cs.AmtDiscount + "</td></tr>");
                        Content.Append("<tr><td>Gift Certificate</td><td></td><td>" + cs.GiftCertificateAmt + "</td></tr>");
                        Content.Append("<tr><th>Total</th><th></th><th>" + cs.AmtDue + "</th></tr>");                   
                        Content.Append("<tr><th>Payment</th><th></th><th></th></tr>");
                        if (cs.ModeOfPayment == "Cash")
                        {
                            Content.Append("<tr><th>Cash</th><th></th><th>" + cs.AmtPayment + "</th></tr>");
                        }
                        else if(cs.ModeOfPayment == "Check")
                        {
                            Content.Append("<tr><td>CheckNo</td><td>"+cs.CheckNo+"</td><td></td></tr>");
                            Content.Append("<tr><th>Check Amount</th><th></th><th>" + cs.AmtPayment + "</th></tr>");
                        }
                        else
                        {
                            Content.Append("<tr><td>Card Type</td><td>" + cs.CardType + "</td><td></td></tr>");
                            Content.Append("<tr><td>Credit Amount</td><td></td><td>" + cs.AmtPayment + "</td></tr>");
                        }
                        Content.Append("<tr><td>Change</td><td></td><td>" + (cs.AmtPayment - cs.AmtDue) + "</th></tr>");

                        System.Web.HttpContext.Current.Response.Write(Content);
                        System.Web.HttpContext.Current.Response.Write("<br>Number:" + code.SurveyCodeID + "&nbsp;&nbsp;&nbsp; Security Code:" + code.Code);
                        System.Web.HttpContext.Current.Response.Write(@"<br><Table><tr><td><img src=""https://fbcdn-sphotos-c-a.akamaihd.net/hphotos-ak-xpa1/v/t1.0-9/11209466_1613806435529994_6640960158631838574_n.jpg?oh=cc41af819a8323c5a5300ef64b7d4843&oe=55CFE1FE&__gda__=1440035476_d351330cfc1a374f945f62f68735be3a"" \></td></tr></Table>");                    
                        System.Web.HttpContext.Current.Response.End();
                    }
                }

            //    return new EmptyResult();
            //}

            return View(chargeslip);
        }

        // GET: ChargeSlips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChargeSlip chargeSlip = db.ChargeSlips.Find(id);
            if (chargeSlip == null)
            {
                return HttpNotFound();
            }
            return View(chargeSlip);
        }


        void SetChoices()
        {
            ViewBag.PaymentMethod = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "Cash", Value = "Cash"},
                new SelectListItem(){Text = "Check", Value = "Check"},
                new SelectListItem(){Text = "Credit Card", Value = "Credit Card"}
            };

            ViewBag.CardType = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "Mastercard", Value = "Mastercard"},
                new SelectListItem(){Text = "Visa", Value = "Visa"},
            };

            ViewBag.Patients = new SelectList(db.Patients.Where(r => r.isEnabled).ToList(), "PatientID", "FullName");

            List<int> histories = db.History.Where(r => r.DateTimeEnd == null).Select(u=>u.EmployeeID).ToList();
            ViewBag.Therapists = new SelectList(db.Employees.Include(a => a.roleType).Where(r => r.roleType.Type == "Therapist" && !histories.Contains(r.EmployeeID)).ToList(), "EmployeeID", "FullName");
          
           
        }

        static List<TreatmentsViewModel> treatmentOrders;
        static List<MaterialsViewModel> materialOrders;

        [Authorize(Roles = "OIC")]
        public ActionResult Create()
        {
            SetChoices();
            ViewBag.Treatments = new SelectList(db.Treatments.Where(r => r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
            ViewBag.Medicines = new SelectList(db.Materials.Where(r => r.isEnabled).ToList(), "MaterialID", "MaterialName");
            ViewBag.TreatmentOrders = treatmentOrders = new List<TreatmentsViewModel>();
            ViewBag.MaterialOrders = materialOrders = new List<MaterialsViewModel>();
            return View();
        }

        // POST: ChargeSlips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "OIC")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ChargeSlipID,PatientID,EmployeeID,DiscountRate,AmtDiscount,AmtDue,ModeOfPayment,AmtPayment,GiftCertificateAmt,GiftCertificateNo,CheckNo,CardType")] ChargeSlip chargeSlip, FormCollection form)
        {
            SetChoices();

            if (materialOrders == null)
            {
                materialOrders = new List<MaterialsViewModel>();
            }
            if (treatmentOrders == null)
            {
                treatmentOrders = new List<TreatmentsViewModel>();
            }

            List<int> treatmentIDs = treatmentOrders.Select(u => u.TreatmentsID).ToList();
            List<int> materialIDs = materialOrders.Select(u => u.MaterialID).ToList();

            #region Create Chargeslip
            if (form["submit"] == "Create")
            {
                if (ModelState.IsValid)
                {
                    if (chargeSlip.GiftCertificateNo != null)
                    {
                        ChargeSlip duplicate = db.ChargeSlips.Where(r => r.GiftCertificateNo == chargeSlip.GiftCertificateNo).SingleOrDefault();
                        if (duplicate != null)
                        {
                            ModelState.AddModelError("", "The Gift Certificate has already been taken.");
                            ViewBag.Treatments = new SelectList(db.Treatments.Where(r => r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                            ViewBag.Medicines = new SelectList(db.Materials.Where(r => r.isEnabled).ToList(), "MaterialID", "MaterialName");
                            ViewBag.TreatmentOrders = treatmentOrders;
                            ViewBag.MaterialOrders = materialOrders;
                            return View(chargeSlip);
                        }
                    }

                    if (chargeSlip.AmtPayment < chargeSlip.AmtDue)
                    {
                        ModelState.AddModelError("", "The amount paid cannot be less than the amount due");
                        ViewBag.Treatments = new SelectList(db.Treatments.Where(r => r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                        ViewBag.Medicines = new SelectList(db.Materials.Where(r => r.isEnabled).ToList(), "MaterialID", "MaterialName");
                        ViewBag.TreatmentOrders = treatmentOrders;
                        ViewBag.MaterialOrders = materialOrders;
                        return View(chargeSlip);
                    }

                    else if (chargeSlip.ModeOfPayment != "Cash" && chargeSlip.AmtPayment != chargeSlip.AmtDue)
                    {
                        ModelState.AddModelError("", "Check & Credit Payments must be exactly the same as amount due");
                        ViewBag.Treatments = new SelectList(db.Treatments.Where(r => r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                        ViewBag.Medicines = new SelectList(db.Materials.Where(r => r.isEnabled).ToList(), "MaterialID", "MaterialName");
                        ViewBag.TreatmentOrders = treatmentOrders;
                        ViewBag.MaterialOrders = materialOrders;
                        return View(chargeSlip);
                    }

                    else if (chargeSlip.ModeOfPayment == "Check" && chargeSlip.CheckNo.Trim() == null)
                    {
                        ModelState.AddModelError("", "Check Payments must have Check No");
                        ViewBag.Treatments = new SelectList(db.Treatments.Where(r => r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                        ViewBag.Medicines = new SelectList(db.Materials.Where(r => r.isEnabled).ToList(), "MaterialID", "MaterialName");
                        ViewBag.TreatmentOrders = treatmentOrders;
                        ViewBag.MaterialOrders = materialOrders;
                        return View(chargeSlip);
                    }

                    if (chargeSlip.ModeOfPayment == "Cash" || chargeSlip.ModeOfPayment == "Check")
                    {
                        chargeSlip.CardType = "";
                    }
                    if (treatmentOrders.Count == 0 && materialOrders.Count == 0)
                    {
                        ModelState.AddModelError("", "You are not buying anything.");
                        ViewBag.Treatments = new SelectList(db.Treatments.Where(r => r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                        ViewBag.Medicines = new SelectList(db.Materials.Where(r => r.isEnabled).ToList(), "MaterialID", "MaterialName");
                        ViewBag.TreatmentOrders = treatmentOrders;
                        ViewBag.MaterialOrders = materialOrders;
                        return View(chargeSlip);
                    }
                    chargeSlip.DateTimePurchased = DateTime.Now;
                    db.ChargeSlips.Add(chargeSlip);
                    string patient = db.Patients.Find(chargeSlip.PatientID).FullName;
                    int auditId = Audit.CreateAudit(patient, "Create", "ChargeSlip", User.Identity.Name);
                    db.SaveChanges();
                    Audit.CompleteAudit(auditId, chargeSlip.ChargeSlipID);

                    int branchId = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee.BranchID).SingleOrDefault();
                    if (branchId != 0)
                    {
                        foreach (TreatmentsViewModel item in treatmentOrders)
                        {
                            Session_ChargeSlip session = new Session_ChargeSlip()
                            {
                                TreatmentID = item.TreatmentsID,
                                ChargeSlipID = chargeSlip.ChargeSlipID,
                                Qty = item.Qty
                            };
                            db.Session_ChargeSlip.Add(session);
                            await db.SaveChangesAsync();

                            List<MaterialList> materialList = db.MaterialList.Where(r => r.TreatmentID == item.TreatmentsID).ToList();
                            foreach (MaterialList material_treatment in materialList)
                            {
                                Inventory inventory = db.Inventories.Where(r => r.MaterialID == material_treatment.MaterialID && r.BranchID == branchId).SingleOrDefault();
                                if (inventory != null)
                                {
                                    inventory.QtyInStock -= material_treatment.Qty;
                                    db.Entry(inventory).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }

                            History newHistory = new History()
                            {
                                EmployeeID = chargeSlip.EmployeeID,
                                PatientID = chargeSlip.PatientID,
                                TreatmentID = item.TreatmentsID,
                                DateTimeStart = chargeSlip.DateTimePurchased,
                                ChargeSlipID = chargeSlip.ChargeSlipID
                            };

                            db.History.Add(newHistory);
                            auditId = Audit.CreateAudit(patient, "Create", "History", User.Identity.Name);
                            await db.SaveChangesAsync();

                            Audit.CompleteAudit(auditId, newHistory.HistoryID);
                            await db.SaveChangesAsync();
                        }

                        foreach (MaterialsViewModel item in materialOrders)
                        {
                            Medicine_ChargeSlip medicine = new Medicine_ChargeSlip()
                                        {
                                            ChargeSlipID = chargeSlip.ChargeSlipID,
                                            MaterialID = item.MaterialID,
                                            Qty = item.Qty
                                        };
                            db.Medicine_ChargeSlip.Add(medicine);
                            await db.SaveChangesAsync();

                            Inventory inventory = db.Inventories.Where(r => r.MaterialID == medicine.MaterialID && r.BranchID == branchId).SingleOrDefault();
                            if (inventory != null)
                            {
                                inventory.QtyInStock -= medicine.Qty;
                                db.Entry(inventory).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }

                        SurveyCode code = new SurveyCode();
                        code.ChargeSlipID = chargeSlip.ChargeSlipID;
                        code.Code = Guid.NewGuid().ToString().Substring(0, 8);
                        code.isUsed = false;
                        db.SurveyCode.Add(code);
                        await db.SaveChangesAsync();


                    }
                    return RedirectToAction("Index");
                }
            }
            #endregion

            #region Add Treatment
            else if (form["submit"] == "Add Treatment")
            {
                int TreatmentID;
                int TreatmentQty;
                if (int.TryParse(form["Treatments"], out TreatmentID) && int.TryParse(form["TreatmentQty"], out TreatmentQty))
                {
                    if (treatmentIDs.Contains(TreatmentID))
                    {
                        ModelState.AddModelError("", "That treatment has already been added.");
                    }
                    else
                    {
                        int branchId = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee.BranchID).SingleOrDefault();
                        if (branchId != 0)
                        {
                            TreatmentsViewModel treatment = db.Treatments.Where(r => r.TreatmentsID == TreatmentID).Select(u => new TreatmentsViewModel() { TreatmentName = u.TreatmentName, TreatmentsID = u.TreatmentsID, Qty = TreatmentQty, TotalPrice = TreatmentQty * u.TreatmentPrice }).Single();

                            int qtyRequested = 0;
                            List<MaterialList> materialList = db.MaterialList.Where(r => r.TreatmentID == treatment.TreatmentsID).ToList();
                            foreach (MaterialList material_treatment in materialList)
                            {
                                Inventory inventory = db.Inventories.Include(a => a.material).Where(r => r.MaterialID == material_treatment.MaterialID && r.BranchID == branchId).SingleOrDefault();
                                if (inventory != null)
                                {
                                    qtyRequested = treatment.Qty * material_treatment.Qty;
                                    MaterialsViewModel additionalMaterial = materialOrders.Where(r => r.MaterialID == material_treatment.MaterialID).SingleOrDefault();
                                    if (additionalMaterial != null)
                                        qtyRequested += additionalMaterial.Qty;

                                    if (inventory.QtyInStock < qtyRequested)
                                    {
                                        ModelState.AddModelError("", "Cannot add treatment. The branch has insufficient stock of \"" + inventory.material.MaterialName + "\".");
                                        ViewBag.MaterialOrders = materialOrders;
                                        ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                                        ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID) && r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                                        ViewBag.TreatmentOrders = treatmentOrders;
                                        return View(chargeSlip);
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Cannot add item. This treatment requires \"" + material_treatment.material.MaterialName + "\" which has not been added in this branch's inventory.");
                                    ViewBag.MaterialOrders = materialOrders;
                                    ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                                    ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID) && r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                                    ViewBag.TreatmentOrders = treatmentOrders;
                                    return View(chargeSlip);
                                }
                            }
                            treatmentOrders.Add(treatment);
                            //remove already put treatments from the options
                            treatmentIDs.Add(TreatmentID);
                        }
                    }
                }

                ViewBag.MaterialOrders = materialOrders;
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID) && r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                ViewBag.TreatmentOrders = treatmentOrders;
                return View(chargeSlip);
            }
            #endregion

            #region Add Medicine
            else if (form["submit"] == "Add Medicine")
            {
                int materialID;
                int materialQty;
                if (int.TryParse(form["Medicines"], out materialID) && int.TryParse(form["MedicineQty"], out materialQty))
                {
                    if (materialIDs.Contains(materialID))
                    {
                        ModelState.AddModelError("", "That material has already been added.");
                    }
                    else
                    {

                        int branchId = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee.BranchID).SingleOrDefault();
                        if (branchId != 0)
                        {
                            MaterialsViewModel material = db.Materials.Include(a => a.unitType).Where(r => r.MaterialID == materialID).Select(u => new MaterialsViewModel() { MaterialID = u.MaterialID, MaterialName = u.MaterialName, unitType = u.unitType.Type, Qty = materialQty, TotalPrice = u.Price * materialQty }).SingleOrDefault();

                            int qtyRequested = 0;
                            Inventory inventory = db.Inventories.Include(a => a.material).Where(r => r.MaterialID == material.MaterialID && r.BranchID == branchId).SingleOrDefault();
                            if (inventory != null)
                            {
                                qtyRequested = material.Qty;

                                foreach (TreatmentsViewModel treatment in treatmentOrders)
                                {
                                    List<MaterialList> materialList = db.MaterialList.Where(r => r.TreatmentID == treatment.TreatmentsID && r.MaterialID == material.MaterialID).ToList();
                                    foreach (MaterialList material_treatment in materialList)
                                        qtyRequested += (treatment.Qty * material_treatment.Qty);
                                }

                                if (inventory.QtyInStock < qtyRequested)
                                {
                                    ModelState.AddModelError("", "Cannot add item. The branch has insufficient stock of \"" + inventory.material.MaterialName + "\".");
                                    ViewBag.MaterialOrders = materialOrders;
                                    ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                                    ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID) && r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                                    ViewBag.TreatmentOrders = treatmentOrders;
                                    return View(chargeSlip);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Cannot add item. This item has not been added in this branch's inventory.");
                                ViewBag.MaterialOrders = materialOrders;
                                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                                ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID) && r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                                ViewBag.TreatmentOrders = treatmentOrders;
                                return View(chargeSlip);
                            }

                            materialOrders.Add(material);
                            //remove already put materials from the options
                            materialIDs.Add(materialID);
                        }
                    }
                }
                ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID) && r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                ViewBag.TreatmentOrders = treatmentOrders;
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                ViewBag.MaterialOrders = materialOrders;
                return View(chargeSlip);
            }
            #endregion

            #region Remove Medicine / Treatment
            else if (form["submit"].StartsWith("RemoveM-"))
            {
                string[] array = form["submit"].Split('-');
                int index;
                if (int.TryParse(array[1], out index) && index < materialOrders.Count)
                {
                    materialOrders.RemoveAt(index);
                    materialIDs.RemoveAt(index);
                    ViewBag.MaterialOrders = materialOrders;
                    ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                    ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID) && r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                    ViewBag.TreatmentOrders = treatmentOrders;
                    return View(chargeSlip);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (form["submit"].StartsWith("RemoveT-"))
            {
                string[] array = form["submit"].Split('-');
                int index;
                if (int.TryParse(array[1], out index) && index < treatmentOrders.Count)
                {
                    treatmentOrders.RemoveAt(index);
                    treatmentIDs.RemoveAt(index);
                    ViewBag.TreatmentOrders = treatmentOrders;
                    ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID) && r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
                    ViewBag.MaterialOrders = materialOrders;
                    ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                    return View(chargeSlip);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            #endregion


            ViewBag.Treatments = new SelectList(db.Treatments.Where(r => r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
            ViewBag.Medicines = new SelectList(db.Materials.Where(r => r.isEnabled).ToList(), "MaterialID", "MaterialName");
            ViewBag.TreatmentOrders = treatmentOrders;
            ViewBag.MaterialOrders = materialOrders;
            return View(chargeSlip);
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
