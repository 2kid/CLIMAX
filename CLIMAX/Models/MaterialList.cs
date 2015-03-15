using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class MaterialList
    {
        public int MaterialListID { get; set; }
        [ForeignKey("treatment")]
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }
        public int MaterialID { get; set; }
        public virtual Materials material { get; set; }
        [RegularExpression("^[1-9]{1}[0-9]{0,3}$", ErrorMessage = "Please input positive numbers not greater than 4 digits")]
        public int Qty { get; set; }
    }
}