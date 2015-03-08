using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Session_ChargeSlip
    {
        public int Session_ChargeSlipID { get; set; }
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }

        public int ChargeSlipID { get; set; }
        public virtual ChargeSlip chargeSlip { get; set; }
    }
}