using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Treatments
    {
        public int TreatmentsID { get; set; }
        [Required]
        [Display(Name="Treatment Name")]
        public string TreatmentName { get; set; }
        [Display(Name = "Treatment Price")]
        public double TreatmentPrice { get; set; }
    }

    //for treatmentOrders
    public class TreatmentsViewModel
    {
        public int TreatmentsID { get; set; }
        public string TreatmentName { get; set; }
        public int Qty { get; set; }
        public double TotalPrice { get; set; }
    }
}