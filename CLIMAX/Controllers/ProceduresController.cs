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
    public class ProceduresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Procedures
        public ActionResult Index(int id)
        {
            ViewBag.TreatmentID = id;
            return View(db.Procedure.Include(a=> a.treatment).OrderBy(u=>u.StepNo).ToList());
        }

        // GET: Procedures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procedure procedure = db.Procedure.Find(id);
            if (procedure == null)
            {
                return HttpNotFound();
            }
            return View(procedure);
        }

        private static int TreatmentID;
        // GET: Procedures/Create
        public ActionResult Create(int id)
        {
            TreatmentID = id;
            return View();
        }

        // POST: Procedures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProcedureID,ProcedureName,StepNo")] Procedure procedure)
        {
            if (ModelState.IsValid)
            {
                procedure.TreatmentID = TreatmentID;
                db.Procedure.Add(procedure);
                db.SaveChanges();
                Audit.CreateAudit(procedure.ProcedureName, "Create", "Procedure", procedure.ProcedureID, User.Identity.Name);

                return RedirectToAction("Index", new { id = TreatmentID });
            }

            return View(procedure);
        }

        // GET: Procedures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procedure procedure = db.Procedure.Find(id);
            if (procedure == null)
            {
                return HttpNotFound();
            }
            return View(procedure);
        }

        // POST: Procedures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProcedureID,ProcedureName,TreatmentID,StepNo")] Procedure procedure)
        {
            if (ModelState.IsValid)
            {
                db.Entry(procedure).State = EntityState.Modified;
                Audit.CreateAudit(procedure.ProcedureName, "Edit", "Procedure", procedure.ProcedureID, User.Identity.Name);

                // db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(procedure);
        }

        // GET: Procedures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procedure procedure = db.Procedure.Find(id);
            if (procedure == null)
            {
                return HttpNotFound();
            }
            return View(procedure);
        }

        // POST: Procedures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Procedure procedure = db.Procedure.Find(id);
            db.Procedure.Remove(procedure);
            Audit.CreateAudit(procedure.ProcedureName, "Delete", "Procedure", procedure.ProcedureID, User.Identity.Name);

            //  db.SaveChanges();
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
