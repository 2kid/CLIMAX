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
    [Authorize]
    public class UnitTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UnitTypes
        public ActionResult Index()
        {
            return View(db.UnitTypes.ToList());
        }

        // GET: UnitTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitType unitType = db.UnitTypes.Find(id);
            if (unitType == null)
            {
                return HttpNotFound();
            }
            return View(unitType);
        }

        // GET: UnitTypes/Create
        [Authorize(Roles = "Auditor,Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: UnitTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Auditor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UnitTypeID,Type")] UnitType unitType)
        {
            if (ModelState.IsValid)
            {
                db.UnitTypes.Add(unitType);
                int auditId =  Audit.CreateAudit(unitType.Type, "Create", "UnitType",User.Identity.Name);
                db.SaveChanges();
                Audit.CompleteAudit(auditId, unitType.UnitTypeID);
                return RedirectToAction("Index");
            }

            return View(unitType);
        }

        // GET: UnitTypes/Edit/5
        [Authorize(Roles = "Auditor,Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitType unitType = db.UnitTypes.Find(id);
            if (unitType == null)
            {
                return HttpNotFound();
            }
            return View(unitType);
        }

        // POST: UnitTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Auditor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UnitTypeID,Type")] UnitType unitType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unitType).State = EntityState.Modified;
                int auditId =  Audit.CreateAudit(unitType.Type, "Edit", "UnitType",User.Identity.Name);
                Audit.CompleteAudit(auditId, unitType.UnitTypeID);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unitType);
        }

        // GET: UnitTypes/Delete/5
        [Authorize(Roles = "Auditor,Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitType unitType = db.UnitTypes.Find(id);
            if (unitType == null)
            {
                return HttpNotFound();
            }
            return View(unitType);
        }

        // POST: UnitTypes/Delete/5
        [Authorize(Roles = "Auditor,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UnitType unitType = db.UnitTypes.Find(id);
            db.UnitTypes.Remove(unitType);
            int auditId =  Audit.CreateAudit(unitType.Type, "Delete", "UnitType",  User.Identity.Name);
            Audit.CompleteAudit(auditId, unitType.UnitTypeID);
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
