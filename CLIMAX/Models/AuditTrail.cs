using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class AuditTrail
    {
        public int AuditTrailID { get; set; }
        public int EmployeeID { get; set; }
        public virtual Employee employee { get; set; }
     //What record was changed Username/Name of item
        public string ActionDetail { get; set; }
        public int? RecordID { get; set; }
        [ForeignKey("actionType")]
        public int ActionTypeID { get; set; }
        public virtual ActionTypes actionType { get; set; }
        public DateTime DateTimeOfAction { get; set; }

        public string getCoulmns()
        {
            return "AuditTrailID,EmployeeID,ActionDetail,RecordID,ActionTypeID,DateTimeOfAction";
        }
    }
}