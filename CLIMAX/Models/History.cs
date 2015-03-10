using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class History
    {
        public int HistoryID { get; set; }
        //which session is it
        public int SessionNo { get; set; }
      
        public int PatientID { get; set; }
        public virtual Patient patient { get; set; }
      
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }
        
        public int EmployeeID { get; set; }
        public virtual Employee employee { get; set; }
        
        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeEnd { get; set; }
    
    }

}