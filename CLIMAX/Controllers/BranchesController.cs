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
    public class BranchesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Branches
        public ActionResult Index()
        {
            
            return View(db.Branches.Where(r => r.isEnabled).ToList());
        }

        // GET: Branches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // GET: Branches/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BranchID,BranchName,Location,ContactNo")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                branch.isEnabled = true;
                db.Branches.Add(branch);
                int auditId = Audit.CreateAudit(branch.BranchName, "Create", "Branch", User.Identity.Name);
                db.SaveChanges();
                Audit.CompleteAudit(auditId, branch.BranchID);
                return RedirectToAction("Index");
            }

            return View(branch);
        }

        // GET: Branches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            if (branch.isEnabled == false)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BranchID,BranchName,Location,ContactNo")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                branch.isEnabled = true;
                db.Entry(branch).State = EntityState.Modified;
                int auditId = Audit.CreateAudit(branch.BranchName, "Edit", "Branch", User.Identity.Name);
                Audit.CompleteAudit(auditId, branch.BranchID);
                db.SaveChanges();               
                return RedirectToAction("Index");
            }
            return View(branch);
        }

        // GET: Branches/Disable/5
        public ActionResult Disable(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            Branch branch = db.Branches.Find(id);
            branch.isEnabled = false;
            int auditId = Audit.CreateAudit(branch.BranchName, "Disable", "Branch",User.Identity.Name);
            Audit.CompleteAudit(auditId, branch.BranchID);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Branches/Disable/5
        public ActionResult Enable(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Disable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            Branch branch = db.Branches.Find(id);
            branch.isEnabled = true;
            int auditId = Audit.CreateAudit(branch.BranchName, "Disable", "Branch", User.Identity.Name);
            Audit.CompleteAudit(auditId, branch.BranchID);
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
