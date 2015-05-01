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
        [Authorize]
        public ActionResult Index(FormCollection form)
        {
            var treatments = db.Treatments.Where(r=>r.isEnabled).ToList();
            string search = form["searchValue"];
            if(!string.IsNullOrEmpty(search))
            {
                treatments = treatments.Where(r => r.TreatmentName.ToLower().Contains(search.ToLower())).ToList();
            }
            return View(treatments);
        }

        // GET: Treatments/Details/5
        [Authorize]
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
            if (!treatments.isEnabled)
            {
                return HttpNotFound();
            }
            ViewBag.MaterialList = db.MaterialList.Include(a => a.material).Where(r => r.TreatmentID == treatments.TreatmentsID).Select(u=>new MaterialsViewModel() { MaterialName = u.material.MaterialName, Qty = u.Qty, unitType = u.material.unitType.Type}).ToList();
            return View(treatments);
        }

        static List<MaterialsViewModel> materialList;
        // GET: Treatments/Create
        [Authorize(Roles = "Auditor,Admin")]
        public ActionResult Create()
        {
            ViewBag.MaterialsList = materialList = new List<MaterialsViewModel>();
            ViewBag.Medicines = new SelectList(db.Materials.Where(r=>r.isEnabled).ToList(), "MaterialID", "MaterialName");
            return View();
        }

        // POST: Treatments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Auditor,Admin")]
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
                    treatments.isEnabled = true;
                    db.Treatments.Add(treatments);
                    int auditId =  Audit.CreateAudit(treatments.TreatmentName, "Create", "Treatment", User.Identity.Name);
                    db.SaveChanges();
                    Audit.CompleteAudit(auditId, treatments.TreatmentsID);

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
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
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
                        if(materialQty < 1)
                        {
                            ModelState.AddModelError("", "That material quantity is invalid");
                            ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                            ViewBag.MaterialList = materialList;
                            return View(treatments);
                        }
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
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");            
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
                    ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");

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
        [Authorize(Roles = "Auditor,Admin")]
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
            if(!treatments.isEnabled)
            {
                return HttpNotFound();
            }
            ViewBag.MaterialsList = db.MaterialList.Include(a => a.treatment).Include(a => a.material).Where(r => r.TreatmentID == treatments.TreatmentsID).Select(u => new MaterialsViewModel() { MaterialName = u.material.MaterialName, unitType = u.material.unitType.Type, Qty = u.Qty, TotalPrice = (u.material.Price * u.Qty) }).ToList();
            ViewBag.Medicines = new SelectList(db.Materials.Where(r=>r.isEnabled).ToList(), "MaterialID", "MaterialName");

            return View(treatments);
        }

        // POST: Treatments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Auditor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TreatmentsID,TreatmentName,TreatmentPrice")] Treatments treatments,FormCollection form)
        {
            if (materialList == null)
            {
                materialList = new List<MaterialsViewModel>();
            }

            if (form["submit"] == "Create")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(treatments).State = EntityState.Modified;
                    int auditId = Audit.CreateAudit(treatments.TreatmentName, "Edit", "Treatment", User.Identity.Name);
                    Audit.CompleteAudit(auditId, treatments.TreatmentsID);
                    db.SaveChanges();

                    List<MaterialList> removeList = db.MaterialList.Where(r => r.TreatmentID == treatments.TreatmentsID).ToList();
                    db.MaterialList.RemoveRange(removeList);

                    foreach (MaterialsViewModel item in materialList)
                    {
                        MaterialList treatment_medicine_List = new MaterialList()
                        {
                            MaterialID = item.MaterialID,
                            TreatmentID = treatments.TreatmentsID,
                            Qty = item.Qty
                        };
                        db.MaterialList.Add(treatment_medicine_List);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }

                List<int> materialIDs = materialList.Select(u => u.MaterialID).ToList();
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
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
                        if (materialQty < 1)
                        {
                            ModelState.AddModelError("", "That material quantity is invalid");
                            ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
                            ViewBag.MaterialList = materialList;
                            return View(treatments);
                        }
                        MaterialsViewModel material = db.Materials.Include(a => a.unitType).Where(r => r.MaterialID == materialID).Select(u => new MaterialsViewModel() { MaterialID = u.MaterialID, MaterialName = u.MaterialName, unitType = u.unitType.Type, Qty = materialQty, TotalPrice = u.Price * materialQty }).SingleOrDefault();
                        materialList.Add(material);
                        //remove already put materials from the options
                        materialIDs.Add(materialID);
                    }
                }
                else
                {
                    if (materialID == 0)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    else
                        ModelState.AddModelError("", "Please input the quantity.");
                }
                ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");
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
                    ViewBag.Medicines = new SelectList(db.Materials.Where(r => !materialIDs.Contains(r.MaterialID) && r.isEnabled).ToList(), "MaterialID", "MaterialName");

                    return View(treatments);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

//            ViewBag.Medicines = new SelectList(db.Materials, "MaterialID", "MaterialName");

         
  //          return View(treatments);
        }

        // GET: Treatments/Disable/5
        [Authorize(Roles = "Auditor,Admin")]
        public ActionResult Disable(int? id)
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

        // POST: Treatments/Disable/5
        [Authorize(Roles = "Auditor,Admin")]
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            Treatments treatments = db.Treatments.Find(id);
            treatments.isEnabled = false;
            db.Entry(treatments).State = EntityState.Modified;
            int auditId = Audit.CreateAudit(treatments.TreatmentName, "Disable", "Treatment", User.Identity.Name);
            Audit.CompleteAudit(auditId, treatments.TreatmentsID);
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
