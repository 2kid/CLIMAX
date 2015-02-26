using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Reports
    {
        public int ReportsID { get; set; }
        
        public int ReportTypeID { get; set; }
        public virtual ReportType reportType { get; set; }
     
        public int EmployeeID { get; set; }
        public virtual Employee employee { get; set; }
      
        public DateTime DateTimeGenerated { get; set; }

    }
}