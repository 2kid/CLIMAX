using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Medicine_ChargeSlip
    {
        public int Medicine_ChargeSlipID { get; set; }
        public int MaterialID { get; set; }
        public Materials Materials { get; set; }
        [RegularExpression("^[1-9]{1}[0-9]{0,3}$", ErrorMessage = "Please input positive numbers not greater than 4 digits")]
        public int Qty { get; set; }
        public int ChargeSlipID { get; set; }
        public virtual ChargeSlip chargeSlip { get; set; }

        public string getCoulmns()
        {
            return "Medicine_ChargeSlipID,MaterialID,Qty,ChargeSlipID";
        }
    }
}