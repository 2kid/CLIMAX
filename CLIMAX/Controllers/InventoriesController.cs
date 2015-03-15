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
    public class InventoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Inventories
        public ActionResult Index()
        {
            Employee currentUser = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee).SingleOrDefault();
            if (currentUser != null && currentUser.BranchID != 0)
            {
                 return View(db.Inventories.Where(r => r.BranchID == currentUser.BranchID).ToList());
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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

        [Authorize(Roles="OIC")]
        // GET: Inventories/Create
        public ActionResult Create()
        {
              Employee currentUser = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee).SingleOrDefault();
              List<int> materialIDs = db.Inventories.Where(r => r.BranchID == currentUser.BranchID).Select(u => u.MaterialID).ToList();
            ViewBag.Materials = new SelectList(db.Materials.Where(r=> !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "OIC")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InventoryID,MaterialID,QtyInStock,QtyToAlert")] Inventory inventory, FormCollection form)
        {
            Employee currentUser = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee).SingleOrDefault();
            List<int> materialIDs = db.Inventories.Where(r => r.BranchID == currentUser.BranchID).Select(u => u.MaterialID).ToList();
              
            if (ModelState.IsValid)
            {  
                if (currentUser.BranchID != 0 && currentUser.roleType.Type == "Officer in Charge")
                {
                    if(materialIDs.Contains(inventory.MaterialID))
                    {
                        ModelState.AddModelError("", "This branch already contains that material.");
                        ViewBag.Materials = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");
                        return View(inventory);
                    }
                    inventory.BranchID = currentUser.BranchID;
                    inventory.LastDateUpdated = DateTime.Now;
                    db.Inventories.Add(inventory);
                    db.SaveChanges();
                    string material = db.Materials.Find(inventory.MaterialID).MaterialName;
                    Audit.CreateAudit(material, "Create", "Inventory", inventory.InventoryID, User.Identity.Name);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "You need to be the Officer in Charge to add an inventory item.");
                }
            }
            ViewBag.Materials = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");
            return View(inventory);
        }

        private static int QtyInStock;
        // GET: Inventories/Edit/5
        [Authorize(Roles = "OIC")]
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
            QtyInStock = inventory.QtyInStock;

            ViewBag.Materials = new SelectList(db.Materials, "MaterialID", "MaterialName");
            ViewBag.Branch = new SelectList(db.Branches, "BranchID", "BranchName");
          
            return View(inventory);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "OIC")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InventoryID,MaterialID,BranchID,QtyToAlert")] Inventory inventory, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                Employee currentUser = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee).SingleOrDefault();
                if (currentUser.BranchID != 0 && currentUser.roleType.Type == "Officer in Charge")
                {
                    int previousQty = db.Inventories.Find(inventory.InventoryID).QtyInStock;
                    int addQty;
                    int subtractQty;
                    if(int.TryParse(form["addQty"],out addQty))
                    {
                        inventory.QtyInStock = QtyInStock = QtyInStock + addQty;
                    }
                    if (int.TryParse(form["subtractQty"], out subtractQty))
                    {
                        inventory.QtyInStock = QtyInStock = QtyInStock - subtractQty;
                    }
                    inventory.BranchID = currentUser.BranchID;
                    inventory.LastDateUpdated = DateTime.Now;
                    db.Entry(inventory).State = EntityState.Modified;
                    string material = db.Materials.Find(inventory.MaterialID).MaterialName;
                    if (previousQty > inventory.QtyInStock)
                    {
                        Audit.CreateAudit((addQty-subtractQty)+"-Removed-"+material, "Edit", "Inventory", inventory.InventoryID, User.Identity.Name);
                    }
                    else if(previousQty < inventory.QtyInStock)
                    {
                        Audit.CreateAudit((addQty - subtractQty) + "-Added-" + material, "Edit", "Inventory", inventory.InventoryID, User.Identity.Name);
                    }
                    else
                    {
                        Audit.CreateAudit(0+"-Same-"+material, "Edit", "Inventory", inventory.InventoryID, User.Identity.Name);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "You need to be the Officer in Charge to add an inventory item.");
                }
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
            string material = db.Materials.Find(inventory.MaterialID).MaterialName;
            Audit.CreateAudit(material, "Delete", "Inventory", inventory.InventoryID, User.Identity.Name);

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
