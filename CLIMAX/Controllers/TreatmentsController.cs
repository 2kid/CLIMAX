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

namespace CLIMAX.Controllers
{
    public class TreatmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Treatments
        public ActionResult Index(FormCollection form)
        {
            var treatments = db.Treatments.ToList();
            string search = form["searchValue"];
            if(!string.IsNullOrEmpty(search))
            {
                treatments = treatments.Where(r => r.TreatmentName.Contains(search)).ToList();
            }
            return View(treatments);
        }

        // GET: Treatments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Treatments treatments = db.Treatments.Find(id);
            if (treatments == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaterialList = db.MaterialList.Include(a => a.material).Where(r => r.TreatmentID == treatments.TreatmentsID).Select(u=>new MaterialsViewModel() { MaterialName = u.material.MaterialName, Qty = u.Qty, unitType = u.material.unitType.Type}).ToList();
            return View(treatments);
        }

        static List<MaterialsViewModel> materialList;
        // GET: Treatments/Create
        public ActionResult Create()
        {
            ViewBag.MaterialsList = materialList = new List<MaterialsViewModel>();
            ViewBag.Medicines = new SelectList(db.Materials, "MaterialID", "MaterialName");
            return View();
        }

        // POST: Treatments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TreatmentsID,TreatmentName,TreatmentPrice")] Treatments treatments, FormCollection form)
        {
            if (materialList == null)
            {
                materialList = new List<MaterialsViewModel>();
            }
            if (form["submit"] == "Create")
            {
                if (ModelState.IsValid)
                {             
                    db.Treatments.Add(treatments);
                    db.SaveChanges();

                    foreach (MaterialsViewModel item in materialList)
                    {
                        MaterialList treatment_medicine_List = new MaterialList()
                        {
                            MaterialID = item.MaterialID,
                            TreatmentID = treatments.TreatmentsID,
                            Qty = item.Qty
                        };
                        db.MaterialList.Add(treatment_medicine_List);
                        await db.SaveChangesAsync();
                    }

                    return RedirectToAction("Index");
                }
                List<int> materialIDs = materialList.Select(u => u.MaterialID).ToList();
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");
                ViewBag.MaterialList = materialList;
                return View(treatments);
            }
            else if (form["submit"] == "Add Material")
            {
                int materialID;
                int materialQty;
                List<int> materialIDs = materialList.Select(u => u.MaterialID).ToList();
                if (int.TryParse(form["Medicines"], out materialID) && int.TryParse(form["MedicineQty"], out materialQty))
                {                  
                    if (materialIDs.Contains(materialID))
                    {
                        ModelState.AddModelError("", "That material has already been added.");
                    }
                    else
                    {
                        MaterialsViewModel material = db.Materials.Include(a => a.unitType).Where(r => r.MaterialID == materialID).Select(u => new MaterialsViewModel() { MaterialID = u.MaterialID, MaterialName = u.MaterialName, unitType = u.unitType.Type, Qty = materialQty, TotalPrice = u.Price * materialQty }).SingleOrDefault();
                        materialList.Add(material);
                        //remove already put materials from the options
                        materialIDs.Add(materialID);
                    }
                }
                else
                {
                    if(materialID==0)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    else
                        ModelState.AddModelError("", "Please input the quantity.");
                }
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");            
                ViewBag.MaterialList = materialList;
                return View(treatments);
            }
            else if (form["submit"].StartsWith("Remove-"))
            {
                string[] array = form["submit"].Split('-');
                int index;
                if (int.TryParse(array[1], out index) && index < materialList.Count)
                {
                    materialList.RemoveAt(index);
                    ViewBag.MaterialList = materialList;
                    List<int> materialIDs = materialList.Select(u => u.MaterialID).ToList();
                    ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID)).ToList(), "MaterialID", "MaterialName");

                    return View(treatments);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // GET: Treatments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Treatments treatments = db.Treatments.Find(id);
            if (treatments == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaterialsList = db.MaterialList.Include(a => a.treatment).Include(a => a.material).Where(r => r.TreatmentID == treatments.TreatmentsID).Select(u => new MaterialsViewModel() { MaterialName = u.material.MaterialName, unitType = u.material.unitType.Type, Qty = u.Qty, TotalPrice = (u.material.Price * u.Qty) }).ToList();
            ViewBag.Medicines = new SelectList(db.Materials, "MaterialID", "MaterialName");

            return View(treatments);
        }

        // POST: Treatments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TreatmentsID,TreatmentName,TreatmentPrice")] Treatments treatments)
        {
            ViewBag.Medicines = new SelectList(db.Materials, "MaterialID", "MaterialName");

            if (ModelState.IsValid)
            {
                db.Entry(treatments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(treatments);
        }

        // GET: Treatments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Treatments treatments = db.Treatments.Find(id);
            if (treatments == null)
            {
                return HttpNotFound();
            }
            return View(treatments);
        }

        // POST: Treatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Treatments treatments = db.Treatments.Find(id);
            db.Treatments.Remove(treatments);
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
