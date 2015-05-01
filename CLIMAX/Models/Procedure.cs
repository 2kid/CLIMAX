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
        [Display(Name = "Procedure")]
        [MaxLength(100)]
        public string ProcedureName { get; set; }
        [ForeignKey("treatment")]
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }
        [Display(Name = "Step No")]
        [RegularExpression("^[0-9]{1,3}$",ErrorMessage="Number must be between 0 and 999")]
        public int StepNo { get; set; }

        public bool isEnabled { get; set; }

        public string getColumns()
        {
            return "ProcedureID,ProcedureName,TreatmentID,StepNo";
        }
    }
}