using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class AuditTrail
    {
        public int AuditTrailID { get; set; }
        public int EmployeeID { get; set; }
        public virtual Employee employee { get; set; }
        public int RecordID { get; set; }
        public int ActionTypeID { get; set; }
        public virtual ActionTypes actionType { get; set; }
    
    
    }
}