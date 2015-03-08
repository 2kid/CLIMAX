using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Procedure
    {
        public int ProcedureID { get; set; }
        [Required]
        [Display(Name = "Procedure Name")]
        public string ProcedureName { get; set; }
        [ForeignKey("treatment")]
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }
        [Display(Name = "Step No")]
        public int StepNo { get; set; }

    }
}