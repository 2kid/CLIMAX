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
    public class ReportTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /ReportType/
        public ActionResult Index()
        {
            return View(db.ReportTypes.ToList());
        }

        // GET: /ReportType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportType reporttype = db.ReportTypes.Find(id);
            if (reporttype == null)
            {
                return HttpNotFound();
            }
            return View(reporttype);
        }

        // GET: /ReportType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ReportType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ReportTypeID,Type")] ReportType reporttype)
        {
            if (ModelState.IsValid)
            {
                db.ReportTypes.Add(reporttype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reporttype);
        }

        // GET: /ReportType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportType reporttype = db.ReportTypes.Find(id);
            if (reporttype == null)
            {
                return HttpNotFound();
            }
            return View(reporttype);
        }

        // POST: /ReportType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ReportTypeID,Type")] ReportType reporttype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reporttype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reporttype);
        }

        // GET: /ReportType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportType reporttype = db.ReportTypes.Find(id);
            if (reporttype == null)
            {
                return HttpNotFound();
            }
            return View(reporttype);
        }

        // POST: /ReportType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReportType reporttype = db.ReportTypes.Find(id);
            db.ReportTypes.Remove(reporttype);
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
