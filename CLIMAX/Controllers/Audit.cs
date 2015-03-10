using CLIMAX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLIMAX.Controllers
{
    public static class Audit 
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        public static void CreateAudit(string actionDetail, string action, string affectedTable, int recordID, string user)
        {
            AuditTrail newAudit = new AuditTrail();
            newAudit.ActionDetail = actionDetail;
            newAudit.ActionTypeID = db.ActionTypes.Where(r => r.Action == action && r.AffectedRecord == affectedTable).Select(u => u.ActionTypesID).Single();
            newAudit.DateTimeOfAction = DateTime.Now;
            newAudit.EmployeeID = db.Users.Where(r => r.UserName == user).Select(u => u.EmployeeID).Single();
            newAudit.RecordID = recordID;
            db.AuditTrail.Add(newAudit);
            db.SaveChanges();
        }
    }
}