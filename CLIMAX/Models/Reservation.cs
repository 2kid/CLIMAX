using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }
        public int ReservationTypeID { get; set; }
        public virtual ReservationType reservationType { get; set; }
        public DateTime DateTimeReserved { get; set; }
        public string  Notes { get; set; }
        public int PatientID { get; set; }
        public virtual Patient patient { get; set; }
        public int EmployeeID { get; set; }
        public virtual Employee employee { get; set; }



    }
}