using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string CivilStatus{ get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string HomeAddress { get; set; }
        public string EmailAddress { get; set; }
        public int HomeNo { get; set; }
        public int CellphoneNo { get; set; }
        public string Occupation { get; set; }
        public int CompanyID { get; set; }
        public virtual Company company { get; set; }
        public int EmergencyContactNo { get; set; }
        public string EmergencyContactName { get; set; }
        public int BranchID { get; set; }
        public virtual Branch branch { get; set; }

        public string FullName
        {
            get { return FirstName + " " + MiddleName + " " + LastName; }
        }
    }
}