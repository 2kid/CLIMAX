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
            var patients = db.Patients.Where(r => r.isEnabled).Include(p => p.branch).ToList();

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
            if (!patient.isEnabled)
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

            List<SelectListItem> areaCode = new List<SelectListItem>();
            for (int i = 2; i < 90; i++)
            {
                areaCode.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            ViewBag.areaCode = areaCode;
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
                ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.BranchName != "*" && r.isEnabled).ToList(), "BranchID", "BranchName");
                return View();
            }


        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatientID,FirstName,MiddleName,LastName,Gender,CivilStatus,Height,Weight,HomeNo,Street,City,LandlineNo,CellphoneNo,EmailAddress,Occupation,EmergencyContactNo,EmergencyContactFName,EmergencyContactMName,EmergencyContactLName,BranchID")] Patient patient, FormCollection form)
        {
            IEnumerable<SelectListItem> civilStatus = new List<SelectListItem>()
                        {
                            new SelectListItem(){ Text = "Single", Value = "Single"},
                            new SelectListItem(){Text = "Married", Value = "Married"},
                            new SelectListItem(){Text = "Widowed", Value = "Widowed"},
                            new SelectListItem(){Text = "Divorced", Value = "Divorced"},
                            new SelectListItem(){Text = "Separated", Value = "Separated"}
                        };
         
            List<SelectListItem> areaCode = new List<SelectListItem>();
            for (int i = 2; i < 90; i++)
            {
                areaCode.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            ViewBag.areaCode = new SelectList(areaCode, "Value", "Text", form["areaCode"]);

            if (ModelState.IsValid)
            {

                DateTime bdate;
                    if(!DateTime.TryParse(form["BirthDate"],out bdate))
                    {
                        ModelState.AddModelError("BirthDate", "The field BirthDate is invalid");
                        ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName", patient.BranchID);
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
                    patient.BirthDate = bdate;

                if (!Regex.IsMatch(patient.BirthDate.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("BirthDate", "The field BirthDate is invalid");
                    ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName", patient.BranchID);
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
                    ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName", patient.BranchID);
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
                patient.isEnabled = true;
                db.Patients.Add(patient);
                int auditId = Audit.CreateAudit(patient.FullName, "Create", "Patient", User.Identity.Name);
                db.SaveChanges();
                Audit.CompleteAudit(auditId, patient.PatientID);
                return RedirectToAction("Index");
            }

            branchId = null;
            if (User.IsInRole("OIC"))
            {
                branchId = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee.BranchID).Single();
                patient.BranchID = branchId.Value;
            }
            else
            {
                ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.BranchName != "*" && r.isEnabled).ToList(), "BranchID", "BranchName");
            }

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

            if (!patient.isEnabled)
            {
                return HttpNotFound();
            }

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

            //if (patient.LandlineNo != "00-0000000-000")
            //{
            //    string[] array = patient.LandlineNo.Split('-');
            //    List<SelectListItem> areaCode = new List<SelectListItem>();
            //    for (int i = 2; i < 90; i++)
            //    {
            //        if (array[0] == i.ToString())
            //        {
            //            areaCode.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
            //        }
            //        else
            //        {
            //            areaCode.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            //        }
            //    }

            //    ViewBag.areaCode = areaCode;
            //    if (array[0] != "")
            //    {
            //        ViewBag.landlineNo = array[1];
            //        ViewBag.extCode = array[2];
            //    }
            //    else
            //    {
            //        ViewBag.landlineNo = "";
            //        ViewBag.extCode = "";
            //    }
            //}
            //else
            //{
            List<SelectListItem> areaCode = new List<SelectListItem>();
            for (int i = 2; i < 90; i++)
            {
                areaCode.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            }
            ViewBag.areaCode = areaCode;
            //    ViewBag.landlineNo = "";
            //    ViewBag.extCode = "";
            //}
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientID,FirstName,MiddleName,LastName,Gender,CivilStatus,Height,Weight,HomeNo,Street,City,LandlineNo,CellphoneNo,EmailAddress,Occupation,EmergencyContactNo,EmergencyContactFName,EmergencyContactMName,EmergencyContactLName")] Patient patient, FormCollection form)
        {
            IEnumerable<SelectListItem> civilStatus = new List<SelectListItem>()
                        {
                            new SelectListItem(){ Text = "Single", Value = "Single"},
                            new SelectListItem(){Text = "Married", Value = "Married"},
                            new SelectListItem(){Text = "Widowed", Value = "Widowed"},
                            new SelectListItem(){Text = "Divorced", Value = "Divorced"},
                            new SelectListItem(){Text = "Separated", Value = "Separated"}
                        };
            List<SelectListItem> areaCode = new List<SelectListItem>();
            for (int i = 2; i < 90; i++)
            {
                areaCode.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            ViewBag.areaCode = new SelectList(areaCode, "Value", "Text", form["areaCode"]);
            if (ModelState.IsValid)
            {
                DateTime bdate;
                if (!DateTime.TryParse(form["BirthDate"], out bdate))
                {
                    ModelState.AddModelError("BirthDate", "The field BirthDate is invalid");
                    ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName", patient.BranchID);
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

                patient.BirthDate = bdate;
                if (!Regex.IsMatch(patient.BirthDate.ToString("yyyy-MM-dd"), "^((19|20|21)\\d\\d)-(0?[1-9]|1[012])-(0?[1-9]|(1|2)[0-9]|3[01])+$"))
                {
                    ModelState.AddModelError("BirthDate", "The field BirthDate is invalid");
                    ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName", patient.BranchID);
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
                    ViewBag.BranchID = new SelectList(db.Branches.Where(r => r.isEnabled).ToList(), "BranchID", "BranchName", patient.BranchID);
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

                patient.BranchID = db.Patients.Where(r=>r.PatientID == patient.PatientID).Select(u=>u.BranchID).Single();
                //  patient.LandlineNo = form["areaCode"]+form[""]
                patient.isEnabled = true;
                db.Entry(patient).State = EntityState.Modified;
                int auditId = Audit.CreateAudit(patient.FullName, "Edit", "Patient", User.Identity.Name);
                Audit.CompleteAudit(auditId, patient.PatientID);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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

        // GET: Patients/Disable/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Patient patient = db.Patients.Find(id);
        //    if (patient == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(patient);
        //}

        //// POST: Patients/Disable/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DisableConfirmed(int id)
        //{
        //    Patient patient = db.Patients.Find(id);
        //    patient.isEnabled = false;
        //    db.Entry(patient).State = EntityState.Modified;

        //    int auditId = Audit.CreateAudit(patient.FullName, "Disable", "Patient", User.Identity.Name);
        //    Audit.CompleteAudit(auditId, patient.PatientID);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
