using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Survey
    {
        public int SurveyID { get; set; }
        [Display(Name="First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public int StarRating { get; set; }
        public string Comment { get; set; }
        public int TeatmentID { get; set; }
        public virtual Treatments Treatments { get; set; }
    }
}