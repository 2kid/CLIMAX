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
    public class EmployeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Employees
        public ActionResult Index(FormCollection form)
        {
            if (User.IsInRole("OIC"))
            {
                int branchId = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee).Select(u => u.BranchID).SingleOrDefault();
                var employees = db.Employees.Include(a => a.roleType).Where(r => r.roleType.Type != "Administrator" && r.roleType.Type != "Auditor" && r.BranchID == branchId).ToList();

                string search = form["searchValue"];
                if (!string.IsNullOrEmpty(search))
                {
                    employees = employees.Where(r => r.FullName.ToLower().Contains(search.ToLower())).ToList();
                }
                return View(employees);

            }
            else
            {
                var employees = db.Employees.Include(a => a.roleType).ToList();

                string search = form["searchValue"];
                if (!string.IsNullOrEmpty(search))
                {
                    employees = employees.Where(r => r.FullName.ToLower().Contains(search.ToLower())).ToList();
                }
                return View(employees);
            }
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "OIC,Auditor")]
        public ActionResult Create()
        {
            ViewBag.Roles = new SelectList(db.RoleType.Where(r => r.Type == "Beauty Consultant" || r.Type == "Therapist").ToList(), "RoleTypeId", "Type");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles="OIC,Auditor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,LastName,FirstName,MiddleName,RoleTypeID")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee currentUser = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee).SingleOrDefault();
                if (currentUser.BranchID != 0)
                {
                    employee.BranchID = currentUser.BranchID;
                    db.Employees.Add(employee);
                    int auditId =  Audit.CreateAudit(employee.FullName, "Create", "Employee",  User.Identity.Name);
                    db.SaveChanges();
                    Audit.CompleteAudit(auditId, employee.EmployeeID);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "You need to be the Officer in Charge to add an employee.");
                }
            }

            ViewBag.Roles = new SelectList(db.RoleType.Where(r => r.Type == "Beauty Consultant" || r.Type == "Therapist").ToList(), "RoleTypeId", "Type");
            return View(employee);
        }


        // GET: Employees/Edit/5
        [Authorize(Roles = "OIC,Auditor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.Roles = new SelectList(db.RoleType.Where(r => r.Type == "Beauty Consultant" || r.Type == "Therapist").ToList(), "RoleTypeId", "Type");
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "OIC,Auditor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,LastName,FirstName,MiddleName,RoleTypeID")] Employee employee)
        {
            if (ModelState.IsValid)
            { 
                Employee currentUser = db.Users.Include(a => a.employee).Where(r => r.UserName == User.Identity.Name).Select(u => u.employee).SingleOrDefault();
                if (currentUser.BranchID != 0)
                {
                    employee.BranchID = currentUser.BranchID;
                    db.Entry(employee).State = EntityState.Modified;
                    int auditId = Audit.CreateAudit(employee.FullName, "Edit", "Employee", User.Identity.Name);
                    Audit.CompleteAudit(auditId, employee.EmployeeID);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Roles = new SelectList(db.RoleType.Where(r => r.Type == "Beauty Consultant" || r.Type == "Therapist").ToList(), "RoleTypeId", "Type");
            return View(employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "OIC,Auditor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [Authorize(Roles = "OIC,Auditor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            int auditId = Audit.CreateAudit(employee.FullName, "Delete", "Employee", User.Identity.Name);
            Audit.CompleteAudit(auditId, employee.EmployeeID);
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
