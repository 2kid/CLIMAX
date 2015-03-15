using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLIMAX.Models;
using System.Text.RegularExpressions;

namespace CLIMAX.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //  private string Audit
        // GET: Patients
        public ActionResult Index(FormCollection form)
        {
            var patients = db.Patients.Include(p => p.branch).ToList();

            string search = form["searchValue"];
            if (!string.IsNullOrEmpty(search))
            {
                patients = patients.Where(r => r.FullName.ToLower().Contains(search.ToLower())).ToList();
            }

            return View(patients);
        }

        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        private static int? branchId;
        // GET: Patients/Create
        public ActionResult Create()
        {
            IEnumerable<SelectListItem> item = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Female", Value = "false"},
                new SelectListItem(){Text = "Male", Value = "true"}   
            };

            IEnumerable<SelectListItem> civilStatus = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Single", Value = "Single"},
                new SelectListItem(){Text = "Married", Value = "Married"},
                new SelectListItem(){Text = "Widowed", Value = "Widowed"},
                new SelectListItem(){Text = "Divorced", Value = "Divorced"},
                new SelectListItem(){Text = "Separated", Value = "Separated"}
            };


            ViewBag.Gender = new SelectList(item, "Value", "Text");
            ViewBag.CivilStatus = civilStatus;

            branchId = null;
            if (User.IsInRole("OIC"))
            {
                branchId = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee.BranchID).Single();
                return View(new Patient() { BranchID = branchId.Value });
            }
            else
            {
                ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.BranchName != "*").ToList(), "BranchID", "BranchName");
                return View();
            }


        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatientID,FirstName,MiddleName,LastName,BirthDate,Gender,CivilStatus,Height,Weight,HomeNo,Street,City,LandlineNo,CellphoneNo,EmailAddress,Occupation,EmergencyContactNo,EmergencyContactFName,EmergencyContactMName,EmergencyContactLName,BranchID")] Patient patient)
        {
            IEnumerable<SelectListItem> civilStatus = new List<SelectListItem>()
                        {
                            new SelectListItem(){ Text = "Single", Value = "Single"},
                            new SelectListItem(){Text = "Married", Value = "Married"},
                            new SelectListItem(){Text = "Widowed", Value = "Widowed"},
                            new SelectListItem(){Text = "Divorced", Value = "Divorced"},
                            new SelectListItem(){Text = "Separated", Value = "Separated"}
                        }; 
            
            if (ModelState.IsValid)
            {
                if (!Regex.IsMatch(patient.BirthDate.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("BirthDate", "The field BirthDate is invalid");
                    ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);            
                    ViewBag.Gender = new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Female", Value = "false"},
                          new SelectListItem(){
                                Text = "Male", Value = "true"}   
                        };

                    ViewBag.CivilStatus = civilStatus;
                    return View(patient);
                }

              if(patient.BirthDate.CompareTo(DateTime.Now) == 1)
                {
                    ModelState.AddModelError("", "Date of Birth cannot be after today");
                    ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);
                    ViewBag.Gender = new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Female", Value = "false"},
                          new SelectListItem(){
                                Text = "Male", Value = "true"}   
                        };

                    ViewBag.CivilStatus = civilStatus;
                    return View(patient);
                }

                if (User.IsInRole("OIC"))
                    patient.BranchID = branchId.Value;

                db.Patients.Add(patient);
                db.SaveChanges();
                Audit.CreateAudit(patient.FullName, "Create", "Patient", patient.PatientID, User.Identity.Name);
                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);
            ViewBag.Gender = new List<SelectListItem>()
            {
                new SelectListItem(){
                    Text = "Female", Value = "false"},
                new SelectListItem(){
                    Text = "Male", Value = "true"}   
            };

            ViewBag.CivilStatus = civilStatus;
            return View(patient);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);

            ViewBag.Gender = new List<SelectListItem>()
            {
                new SelectListItem(){
                    Text = "Female", Value = "false"},
              new SelectListItem(){
                    Text = "Male", Value = "true", Selected = patient.Gender}   
            };

            ViewBag.CivilStatus = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Single", Value = "Single"},
                new SelectListItem(){Text = "Married", Value = "Married"},
                new SelectListItem(){Text = "Widowed", Value = "Widowed"},
                new SelectListItem(){Text = "Divorced", Value = "Divorced"},
                new SelectListItem(){Text = "Separated", Value = "Separated"}
            };
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientID,FirstName,MiddleName,LastName,BirthDate,Gender,CivilStatus,Height,Weight,HomeNo,Street,City,LandlineNo,CellphoneNo,EmailAddress,Occupation,EmergencyContactNo,EmergencyContactFName,EmergencyContactMName,EmergencyContactLName,BranchID")] Patient patient)
        {
            IEnumerable<SelectListItem> civilStatus = new List<SelectListItem>()
                        {
                            new SelectListItem(){ Text = "Single", Value = "Single"},
                            new SelectListItem(){Text = "Married", Value = "Married"},
                            new SelectListItem(){Text = "Widowed", Value = "Widowed"},
                            new SelectListItem(){Text = "Divorced", Value = "Divorced"},
                            new SelectListItem(){Text = "Separated", Value = "Separated"}
                        };

            if (ModelState.IsValid)
            {
                if (!Regex.IsMatch(patient.BirthDate.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("BirthDate", "The field BirthDate is invalid");
                    ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);
                    ViewBag.Gender = new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Female", Value = "false"},
                          new SelectListItem(){
                                Text = "Male", Value = "true"}   
                        };

                    ViewBag.CivilStatus = civilStatus;
                    return View(patient);
                }

                if (patient.BirthDate.CompareTo(DateTime.Now) == 1)
                {
                    ModelState.AddModelError("", "Date of Birth cannot be after today");
                    ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);
                    ViewBag.Gender = new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Female", Value = "false"},
                          new SelectListItem(){
                                Text = "Male", Value = "true"}   
                        };

                    ViewBag.CivilStatus = civilStatus;
                    return View(patient);
                }

                if (User.IsInRole("OIC"))
                    patient.BranchID = branchId.Value;

                db.Entry(patient).State = EntityState.Modified;
                Audit.CreateAudit(patient.FullName, "Edit", "Patient", patient.PatientID, User.Identity.Name);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", patient.BranchID);
            IEnumerable<SelectListItem> item = new List<SelectListItem>()
            {
                new SelectListItem(){
                    Text = "Female", Value = "false"},
              new SelectListItem(){
                    Text = "Male", Value = "true"}   
            };

            ViewBag.Gender = new SelectList(item, "Value", "Text");
            ViewBag.CivilStatus = civilStatus;
            return View(patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
            Audit.CreateAudit(patient.FullName, "Delete", "Patient", patient.PatientID, User.Identity.Name);
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
