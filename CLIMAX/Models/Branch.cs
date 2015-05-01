using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Branch
    {
        public int BranchID { get; set; }
        [Required]
        [Display(Name="Branch Name")]
        public string BranchName { get; set; }
        public string Location { get; set; }
        
        [Display(Name = "Contact No")]
        [Phone]
        public string ContactNo { get; set; }
        public virtual List<Patient> Patients { get; set; }

        public bool isEnabled { get; set; }
    }
}