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
        [MaxLength(100)]
        public string TreatmentName { get; set; }
        [Display(Name = "Treatment Price")]
        [RegularExpression("^[1-9][0-9]{0,4}([.][0-9]{1,2})?$", ErrorMessage = "Price must contain two decimal places and cannot exceed 99999.99.")]
        [DataType(DataType.Currency)]
        [Range(1, 99999.99)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public double TreatmentPrice { get; set; }

        public bool isEnabled { get; set; }
        public string getColumns()
        {
            return "TreatmentsID,TreatmentName,TreatmentPrice";
        }
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