using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Branch
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string Location { get; set; }
        public int ContactNo { get; set; }
        public virtual List<Patient> Patients { get; set; }
    }
}