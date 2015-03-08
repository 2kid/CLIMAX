﻿using System;
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

namespace CLIMAX.Controllers
{
    public class ChargeSlipsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ChargeSlips
        public ActionResult Index()
        {
            return View(db.ChargeSlips.ToList());
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

        static List<TreatmentsViewModel> treatmentOrders;
        static List<MaterialsViewModel> materialOrders;
        // GET: ChargeSlips/Create
        public ActionResult Create()
        {
            ViewBag.PaymentMethod = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "Cash", Value = "Cash"},
                new SelectListItem(){Text = "Check", Value = "Check"},
                new SelectListItem(){Text = "Credit Card", Value = "Credit Card"}
            };
            ViewBag.Patients = new SelectList(db.Patients, "PatientID", "FullName");
            ViewBag.Therapists = new SelectList(db.Employees.Include(a => a.roleType).Where(r => r.roleType.Type == "Therapist").ToList(), "EmployeeID", "FullName");
            ViewBag.Treatments = new SelectList(db.Treatments, "TreatmentsID", "TreatmentName");
            ViewBag.Medicines = new SelectList(db.Materials, "MaterialID", "MaterialName");
            ViewBag.TreatmentOrders = treatmentOrders = new List<TreatmentsViewModel>();
            ViewBag.MaterialOrders = materialOrders = new List<MaterialsViewModel>();
            return View();
        }

        // POST: ChargeSlips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ChargeSlipID,PatientID,EmployeeID,DiscountRate,AmtDiscount,AmtDue,ModeOfPayment,AmtPayment,GiftCertificateAmt,GiftCertificateNo,CheckNo")] ChargeSlip chargeSlip, FormCollection form)
        {
            //  List<MaterialsViewModel> materialList = JsonConvert.DeserializeObject<List<MaterialsViewModel>>(form["Materials"]);
            // List<Treatments> TreatmentList = JsonConvert.DeserializeObject<List<Treatments>>(form["TreatmentOrders"]);
            ViewBag.PaymentMethod = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "Cash", Value = "Cash"},
                new SelectListItem(){Text = "Check", Value = "Check"},
                new SelectListItem(){Text = "Credit Card", Value = "Credit Card"}
            };
            ViewBag.Patients = new SelectList(db.Patients, "PatientID", "FullName");
            ViewBag.Therapists = new SelectList(db.Employees.Include(a => a.roleType).Where(r => r.roleType.Type == "Therapist").ToList(), "EmployeeID", "FullName");
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
                    chargeSlip.DateTimePurchased = DateTime.Now;
                    db.ChargeSlips.Add(chargeSlip);
                    db.SaveChanges();
                    foreach (TreatmentsViewModel item in treatmentOrders)
                    {
                        Session_ChargeSlip session = new Session_ChargeSlip()
                        {
                            TreatmentID = item.TreatmentsID,
                            ChargeSlipID = chargeSlip.ChargeSlipID
                        };
                        db.Session_ChargeSlip.Add(session);
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
                        TreatmentsViewModel treatment = db.Treatments.Where(r => r.TreatmentsID == TreatmentID).Select(u => new TreatmentsViewModel() { TreatmentName = u.TreatmentName, TreatmentsID = u.TreatmentsID, Qty = TreatmentQty, TotalPrice = TreatmentQty * u.TreatmentPrice }).Single();
                        treatmentOrders.Add(treatment);
                        //remove already put treatments from the options
                        treatmentIDs.Add(TreatmentID);
                    }
                }
                ViewBag.MaterialOrders = materialOrders;
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");
                ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID)).ToList(), "TreatmentsID", "TreatmentName");
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
                        MaterialsViewModel material = db.Materials.Include(a => a.unitType).Where(r => r.MaterialID == materialID).Select(u => new MaterialsViewModel() { MaterialID = u.MaterialID, MaterialName = u.MaterialName, unitType = u.unitType.Type, Qty = materialQty, TotalPrice = u.Price * materialQty }).SingleOrDefault();
                        materialOrders.Add(material);
                        //remove already put materials from the options
                        materialIDs.Add(materialID);
                    }
                }
                ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID)).ToList(), "TreatmentsID", "TreatmentName");
                ViewBag.TreatmentOrders = treatmentOrders;
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");
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
                    ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");
                    ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID)).ToList(), "TreatmentsID", "TreatmentName");
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
                    ViewBag.Treatments = new SelectList(db.Treatments.Where(r => !treatmentIDs.Contains(r.TreatmentsID)).ToList(), "TreatmentsID", "TreatmentName");
                    ViewBag.MaterialOrders = materialOrders;
                    ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");
                    return View(chargeSlip);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            #endregion
            return View(chargeSlip);
        }

        //// GET: ChargeSlips/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ChargeSlip chargeSlip = db.ChargeSlips.Find(id);
        //    if (chargeSlip == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(chargeSlip);
        //}

        //// POST: ChargeSlips/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ChargeSlipID,DateTimePurchased,ModeOfPayment,AmtOfPayment,ApprovalNo,GiftCertificateAmt,GiftCertificateNo,CheckNo")] ChargeSlip chargeSlip)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(chargeSlip).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(chargeSlip);
        //}

        //// GET: ChargeSlips/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ChargeSlip chargeSlip = db.ChargeSlips.Find(id);
        //    if (chargeSlip == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(chargeSlip);
        //}

        //// POST: ChargeSlips/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    ChargeSlip chargeSlip = db.ChargeSlips.Find(id);
        //    db.ChargeSlips.Remove(chargeSlip);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
