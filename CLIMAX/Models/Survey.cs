using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Survey
    {
        public int SurveyID { get; set; }
        [Display(Name="First Name")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters")]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters")]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters")]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string LastName { get; set; }
        public int StarRating { get; set; }
        public string Comment { get; set; }
        [ForeignKey("Treatments")]
        public int TreatmentID { get; set; }
        public virtual Treatments Treatments { get; set; }

        public string getColumns()
        {
            return "SurveyID,FirstName,MiddleName,LastName,StarRating,Comment,TreatmentID";
        }
    }
}