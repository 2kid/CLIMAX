using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Procedure
    {
        public int ProcedureID { get; set; }
        public string ProcedureName { get; set; }
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }
        public int StepNo { get; set; }

    }
}