using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class ReportType
    {
        public int ReportTypeID { get; set; }
        [Required]
        public string Type { get; set; }
    }
}