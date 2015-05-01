using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLIMAX.Models;
using System.IO;

namespace CLIMAX.Controllers
{
    public class MaterialsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Materials
        [Authorize]
        public ActionResult Index()
        {
            var materials = db.Materials.Where(r=>r.isEnabled).Include(m => m.unitType);

            return View(materials.Where(r=>r.isEnabled).ToList());
        }
         
   
        // GET: Materials/Create
        [Authorize(Roles = "Auditor,Admin")]
        public ActionResult Create()
        {
            ViewBag.UnitTypeID = new SelectList(db.UnitTypes.Where(r=>r.isEnabled).ToList(), "UnitTypeID", "Type");
            return View();
        }

        // POST: Materials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Auditor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaterialID,MaterialName,Description,Price,UnitTypeID")] Materials materials)
        {
            if (ModelState.IsValid)
            {
                materials.isEnabled = true;
                db.Materials.Add(materials);
                int auditId = Audit.CreateAudit(materials.MaterialName, "Create", "Material", User.Identity.Name);
                db.SaveChanges();
                Audit.CompleteAudit(auditId, materials.MaterialID);
                return RedirectToAction("Index");
    }

            ViewBag.UnitTypeID = new SelectList(db.UnitTypes.Where(r=>r.isEnabled).ToList(), "UnitTypeID", "Type", materials.UnitTypeID);
            return View(materials);
        }

        // GET: Materials/Edit/5
        [Authorize(Roles = "Auditor,Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Materials materials = db.Materials.Find(id);
            if (materials == null)
            {
                return HttpNotFound();
            }
            if (!materials.isEnabled)
            {
                return HttpNotFound();
            }
            ViewBag.UnitTypeID = new SelectList(db.UnitTypes.Where(r=>r.isEnabled).ToList(), "UnitTypeID", "Type", materials.UnitTypeID);
            return View(materials);
        }

        // POST: Materials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Auditor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaterialID,MaterialName,Description,Price,UnitTypeID")] Materials materials)
        {
            if (ModelState.IsValid)
            {
                materials.isEnabled = true;
                db.Entry(materials).State = EntityState.Modified;
                int auditId = Audit.CreateAudit(materials.MaterialName, "Edit", "Material", User.Identity.Name);
                Audit.CompleteAudit(auditId, materials.MaterialID);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UnitTypeID = new SelectList(db.UnitTypes.Where(r=>r.isEnabled).ToList(), "UnitTypeID", "Type", materials.UnitTypeID);
            return View(materials);
        }

        // GET: Materials/Disable/5
        [Authorize(Roles = "Auditor,Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Materials materials = db.Materials.Find(id);
            if (materials == null)
            {
                return HttpNotFound();
            }
            return View(materials);
        }

        // POST: Materials/Disable/5
        [Authorize(Roles = "Auditor,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            Materials materials = db.Materials.Find(id);
            materials.isEnabled = false;
            db.Entry(materials).State = EntityState.Modified;
            int auditId = Audit.CreateAudit(materials.MaterialName, "Disable", "Material", User.Identity.Name);
            Audit.CompleteAudit(auditId, materials.MaterialID);
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
