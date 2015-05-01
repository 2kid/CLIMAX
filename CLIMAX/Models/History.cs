using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class History
    {
        public int HistoryID { get; set; }
        //which session is it
        //public int SessionNo { get; set; }
       
        public int PatientID { get; set; }
        public virtual Patient patient { get; set; }
      
        [ForeignKey("treatment")]
        public int TreatmentID { get; set; }
        public virtual Treatments treatment { get; set; }
       
        public int EmployeeID { get; set; }
        public virtual Employee employee { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DateTimeStart { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? DateTimeEnd { get; set; }

        public int ChargeSlipID { get; set; }

        public string getColumns()
        {
            return "HistoryID,PatientID,TreatmentID,EmployeeID,DateTimeStart,DateTimeEnd";
        }
    }
}