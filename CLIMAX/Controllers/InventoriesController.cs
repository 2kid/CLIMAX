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
    public class InventoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Inventories
        public ActionResult Index()
        {
            return View(db.Inventories.ToList());
        }

        // GET: Inventories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // GET: Inventories/Create
        public ActionResult Create()
        {
            ViewBag.Materials = new SelectList(db.Materials, "MaterialID", "MaterialName");
            ViewBag.Branch = new SelectList(db.Branches, "BranchID", "BranchName");
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InventoryID,MaterialID,QtyInStock,BranchID")] Inventory inventory,FormCollection form)
        {
            if (ModelState.IsValid)
            {
                inventory.LastDateUpdated = DateTime.Now;
                db.Inventories.Add(inventory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Marterials = new SelectList(db.Materials, "MaterialID", "MaterialName");
            ViewBag.Branch = new SelectList(db.Branches, "BranchID", "BranchName");
          
            return View(inventory);
        }

        // GET: Inventories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.Materials = new SelectList(db.Materials, "MaterialID", "MaterialName");
            ViewBag.Branch = new SelectList(db.Branches, "BranchID", "BranchName");
          
            return View(inventory);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InventoryID,MaterialID,QtyInStock,BranchID")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                inventory.LastDateUpdated = DateTime.Now;
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Materials = new SelectList(db.Materials, "MaterialID", "MaterialName");
            ViewBag.Branch = new SelectList(db.Branches, "BranchID", "BranchName");
          
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventory inventory = db.Inventories.Find(id);
            db.Inventories.Remove(inventory);
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
