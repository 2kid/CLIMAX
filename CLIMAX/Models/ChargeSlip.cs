using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class ChargeSlip
    {
        public int ChargeSlipID { get; set; }
        public DateTime DateTimePurchased { get; set; }
        public string ModeOfPayment { get; set; }
        public double AmtOfPayment { get; set; }
        public int ApprovalNo { get; set; }
        public double GiftCertificateAmt { get; set; }
        public string GiftCertificateNo { get; set; }
        public string CheckNo { get; set; }
    }
}