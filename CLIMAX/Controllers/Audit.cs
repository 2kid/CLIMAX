using CLIMAX.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CLIMAX.Controllers
{
    public class Audit : Controller
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        public static int CreateAudit(string actionDetail, string action, string affectedTable, string user)
        {
            AuditTrail newAudit = new AuditTrail();
            newAudit.ActionDetail = actionDetail;
            newAudit.ActionTypeID = db.ActionTypes.Where(r => r.Action == action && r.AffectedRecord == affectedTable).Select(u => u.ActionTypesID).Single();
            newAudit.DateTimeOfAction = DateTime.Now;
            newAudit.EmployeeID = db.Users.Where(r => r.UserName == user).Select(u => u.EmployeeID).Single();
            db.AuditTrail.Add(newAudit);
            db.SaveChanges();
            return newAudit.AuditTrailID;
        }

        public static void CompleteAudit(int auditId, int recordID)
        {
            AuditTrail newAudit = db.AuditTrail.Find(auditId);
            newAudit.RecordID = recordID;
            db.Entry(newAudit).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}