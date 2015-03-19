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

        public string DownloadAuditTrails()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(new AuditTrail().getCoulmns());

            foreach (var item in db.AuditTrail.ToList())
            {
                List<string> fields = new List<string>();
                fields.Add(item.AuditTrailID.ToString());
                fields.Add(item.EmployeeID.ToString());
                fields.Add(item.ActionDetail);
                fields.Add(item.RecordID.ToString());
                fields.Add(item.ActionTypeID.ToString());
                fields.Add(item.DateTimeOfAction.ToString());

                sb.AppendLine(string.Join(",", fields));
            }
            return sb.ToString();       
        }
    }
}