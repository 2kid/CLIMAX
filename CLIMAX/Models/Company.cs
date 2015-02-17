using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Company
    {
        public int CompanyID { get; set; }
        [Display(Name="Company Name")]
        public string CompanyName { get; set; }
          [Display(Name = "Company Address")]
        public string CompanyAddress { get; set; }
          [Display(Name = "Company Phone Number")]
        //changed from int
        public string CompanyNo { get; set; }

        public virtual List<Patient> Patients { get; set; }
    }
}