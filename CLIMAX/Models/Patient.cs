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
        [MaxLength(20,ErrorMessage="Maximum of 20 characters")]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$",ErrorMessage="Numbers and some special characters are not allowed")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters")]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters")]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string LastName { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string CivilStatus { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
   
        //Address
        [Display(Name = "House No")]
        [RegularExpression("^[1-9]{1}[0-9]{0,3}$", ErrorMessage = "Please input positive numbers not greater than 4 digits")]
        public int HomeNo { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        //Contact Details
        [Display(Name="Landline No")]
        [RegularExpression("^[0-9]{1,2}-[0-9]{7}-[0-9]{3}$",ErrorMessage="Invalid landline number")]
        public string LandlineNo { get; set; }
        [RegularExpression("^09[0-9]{9}$", ErrorMessage = "Invalid cellphone number")]
        [Display(Name = "Cellphone No")]
        public string CellphoneNo { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
 
        //Profession Details
        public string Occupation { get; set; }

        //Emergency Details
        [Display(Name = "Emergency Contact Phone No")]
        [RegularExpression("^09[0-9]{9}$", ErrorMessage = "Invalid cellphone number")]
        public string EmergencyContactNo { get; set; }
        [Display(Name = "Emergency Contact First Name")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters")]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string EmergencyContactFName { get; set; }
        [Display(Name = "Emergency Contact Middle Name")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters")]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string EmergencyContactMName { get; set; }
        [Display(Name = "Emergency Contact Last Name")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters")]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string EmergencyContactLName { get; set; }
       
        public int BranchID { get; set; }
        public virtual Branch branch { get; set; }

        public bool isEnabled { get; set; }

        public string FullName
        {
            get { return FirstName + " " + MiddleName + " " + LastName; }
        }


        public string getColumns()
        {
            return "PatientID,FirstName,MiddleName,LastName,BirthDate,Gender,CivilStatus,Height,Weight,HomeNo,Street,City,LandlineNo,CellphoneNo,EmailAddress,Occupation,EmergencyContactNo,EmergencyContactFName,EmergencyContactMName,EmergencyContactLName,BranchID";
        }
    }
}