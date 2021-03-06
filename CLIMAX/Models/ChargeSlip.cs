﻿using System;
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
        [Display(Name = "Card Type")]
        public string CardType { get; set; }

        public int PatientID { get; set; }
        public virtual Patient Patient { get; set; }
        public int EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }

        public string getColumns()
        {
            return "ChargeSlipID,DateTimePurchased,DiscountRate,AmtDiscount,AmtDue,ModeOfPayment,AmtPayment,GiftCertificateAmt,GiftCertificateNo,CheckNo,CardType,PatientID,EmployeeID";
        }
    }

    public class ChargeSlipContainerViewModel
    {
        public string Patient { get; set; }
        public int? ChargeSlipID { get; set; }
        public DateTime DateTimePurchased { get; set; }
        public List<ChargeSlipViewModel> items { get; set; }
        public double Total { get; set; }
        public double? GiftCertificateAmt { get; set; }
        public double? DiscountAmount { get; set; }
        public string Therapist { get; set; }
    }

    public class ChargeSlipViewModel
    {      
       // public int SessionNo { get; set; }
        public string Treatment { get; set; }
        public int? TreatmentQty { get; set; }
        public double? TreatmentAmount { get; set; }
        public string Medicine { get; set; }
        public int? MedicineQty { get; set; }
        public double? MedicineAmount { get; set; }     
        public double AmountDue { get; set; }
    }

    public class SurveyCode
    {
        [Key]
        public int SurveyCodeID { get; set; }
        public int ChargeSlipID { get; set; }
        public string Code { get; set; }
        public bool isUsed { get; set; }
    }

    public class SummaryReportContainerViewModel
    {
        public int CardTypeCount { get; set; }
        public double TotalGrossAmount { get; set; }
        public double TotalNet { get; set; }
        public List<SummaryReportViewModel> items { get; set; }
    }
    public class SummaryReportViewModel
    {
        public string Patient { get; set; }
        public string CardType { get; set; }
        public double GrossAmount { get; set; }
        public double Net { get; set; }
    }
}