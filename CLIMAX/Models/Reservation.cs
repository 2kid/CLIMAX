using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        [Display(Name="Treatment")]
        [ForeignKey("treatment")]
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }
         [Display(Name = "Reservation Type")]
        public bool ReservationType { get; set; }
         [Display(Name = "Date & Time Reserved")]
         [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
         [DataType(DataType.DateTime)]
        public DateTime DateTimeReserved { get; set; }
        public string  Notes { get; set; }
        [Display(Name="Patient")]
        public int PatientID { get; set; }
        public virtual Patient patient { get; set; }
        [Display(Name = "Therapist")]
        public int? EmployeeID { get; set; }
        public virtual Employee employee { get; set; }

        public string getColumns()
        {
            return "ReservationID,TreatmentID,ReservationType,DateTimeReserved,Notes,PatientID,EmployeeID";
        }
    }
}