using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class ChargeSlip
    {
        public int ChargeSlipID { get; set; }
        [Display(Name = "Date & Time Purchased")]
        public DateTime DateTimePurchased { get; set; }
        [Display(Name = "Discount Rate")]
        public int? DiscountRate { get; set; }
        [Display(Name = "Discount Amount")]
        public double? AmtDiscount { get; set; }
        public double AmtDue { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string ModeOfPayment { get; set; }
        [Display(Name = "Amount")]
        public double AmtPayment { get; set; }
        [Display(Name = "GC Amount")]
        public double? GiftCertificateAmt { get; set; }
        [Display(Name = "GC Number")]
        public string GiftCertificateNo { get; set; }
        [Display(Name = "Check No")]
        public string CheckNo { get; set; }

        public int PatientID { get; set; }
        public virtual Patient Patient { get; set; }
        public int EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }
    }

    public class ChargeSlipViewModel
    {
        public int? ChargeSlipID { get; set; }
       // public int SessionNo { get; set; }
        public string Patient { get; set; }
        public string Treatment { get; set; }
        public int? TreatmentQty { get; set; }
        public double? TreatmentAmount { get; set; }
        public string Medicine { get; set; }
        public int? MedicineQty { get; set; }
        public double? MedicineAmount { get; set; }
        public double Total { get; set; }
        public double? DiscountAmount { get; set; }
        public double AmountDue { get; set; }
        public string Therapist { get; set; }
    }
}