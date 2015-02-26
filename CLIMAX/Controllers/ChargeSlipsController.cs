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

        // GET: ChargeSlips/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChargeSlips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ChargeSlipID,DateTimePurchased,ModeOfPayment,AmtOfPayment,ApprovalNo,GiftCertificateAmt,GiftCertificateNo,CheckNo")] ChargeSlip chargeSlip)
        {
            if (ModelState.IsValid)
            {
                db.ChargeSlips.Add(chargeSlip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(chargeSlip);
        }

        // GET: ChargeSlips/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: ChargeSlips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChargeSlipID,DateTimePurchased,ModeOfPayment,AmtOfPayment,ApprovalNo,GiftCertificateAmt,GiftCertificateNo,CheckNo")] ChargeSlip chargeSlip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chargeSlip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chargeSlip);
        }

        // GET: ChargeSlips/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: ChargeSlips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChargeSlip chargeSlip = db.ChargeSlips.Find(id);
            db.ChargeSlips.Remove(chargeSlip);
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
