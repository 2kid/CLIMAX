using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Session_ChargeSlip
    {
        public int Session_ChargeSlipID { get; set; }
        [ForeignKey("treatment")]
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }
        public int Qty { get; set; }
        public int ChargeSlipID { get; set; }
        public virtual ChargeSlip chargeSlip { get; set; }
    }
}