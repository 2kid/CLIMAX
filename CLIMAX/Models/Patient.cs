using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)] 
       // [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string CivilStatus { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
   
        //Address
        [Display(Name = "House No")]
        public int HomeNo { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        //Contact Details
        [Display(Name="Landline No")]
        [Phone]
        public string LandlineNo { get; set; }
        [Phone]
        [Display(Name = "Cellphone No")]
        public string CellphoneNo { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
 
        //Profession Details
        public string Occupation { get; set; }
        public int? CompanyID { get; set; }
        public virtual Company company { get; set; }

        //Emergency Details
        [Display(Name = "Emergency Contact No")]
        public string EmergencyContactNo { get; set; }
        [Display(Name = "Emergency Contact First Name")]
        public string EmergencyContactFName { get; set; }
        [Display(Name = "Emergency Contact Middle Name")]
        public string EmergencyContactMName { get; set; }
        [Display(Name = "Emergency Contact Last Name")]
        public string EmergencyContactLName { get; set; }
       
        public int BranchID { get; set; }
        public virtual Branch branch { get; set; }

        public string FullName
        {
            get { return FirstName + " " + MiddleName + " " + LastName; }
        }
    }

    public class PatientViewModel
    {

    }

}