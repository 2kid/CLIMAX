using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class ReportType
    {
        public int ReportTypeID { get; set; }
        public string reportType { get; set; }
        public string reportDescription { get; set; }

        public virtual List<Reports> reports { get; set; }
    }
}