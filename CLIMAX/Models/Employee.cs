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
        public int EmployeeID { get; set; }
        [Display(Name="Last Name")]
        public string LastName { get; set; }
         [Display(Name = "First Name")]
        public string FirstName { get; set; }
         [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        public int RoleTypeID { get; set; }
        public virtual RoleType roleType { get; set; }

        public string FullName
        {
            get { return FirstName + " " + MiddleName + " " + LastName; }
        }
    }
}