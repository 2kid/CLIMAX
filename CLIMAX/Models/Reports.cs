using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Display(Name="Date Start")]
        public DateTime DateStartOfReport { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "Date End")]
        public DateTime DateEndOfReport { get; set; }

    }
}