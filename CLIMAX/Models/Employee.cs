using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        [Display(Name="Last Name")]
        [MaxLength(20)]
        [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string LastName { get; set; }
         [Display(Name = "First Name")]
         [MaxLength(20)]
         [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]    
        public string FirstName { get; set; }
         [Display(Name = "Middle Name")]
         [MaxLength(20)]
         [RegularExpression("^([ \u00c0-\u01ffa-zA-Z'-])+$", ErrorMessage = "Numbers and some special characters are not allowed")]
        public string MiddleName { get; set; }
        public int RoleTypeID { get; set; }
        public virtual RoleType roleType { get; set; }

        public int BranchID { get; set; }
        public virtual Branch Branch { get; set; }

        public bool isEnabled { get; set; }


        public string FullName
        {
            get { return FirstName + " " + MiddleName + " " + LastName; }
        }

        public string getColumns()
        {
            return "EmployeeID,LastName,FirstName,MiddleName,RoleTypeID,BranchID,isEnabled";
        }
    }

    public class EmployeeTransactionsViewModel
    {
        public string Patient { get; set; }
        [Display(Name="Date & Time of Transaction")]
        public string DateTimeOfTransaction { get; set; }
        public List<string> Treatments { get; set; }
    }
}