using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Medicine_ChargeSlip
    {
        public int Medicine_ChargeSlipID { get; set; }
        public Materials MaterialID { get; set; }
        public int Qty { get; set; }
        public int ChargeSlipID { get; set; }
        public virtual ChargeSlip chargeSlip { get; set; }

    }
}