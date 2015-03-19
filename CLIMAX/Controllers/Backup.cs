using CLIMAX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CLIMAX.Controllers
{
    public class Backup : Controller
    {


        static ApplicationDbContext db = new ApplicationDbContext();
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        //
        // GET: /Backup/
        public string DownloadPatient()
        {
            //bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            // if (alert == true)
            // {


            sb.AppendLine(new Patient().getColumns());

            foreach (var item in db.Patients.ToList())
            {
                List<string> fields = new List<string>();
                fields.Add(item.PatientID.ToString());
                fields.Add(item.FirstName);
                fields.Add(item.MiddleName);
                fields.Add(item.LastName);
                fields.Add(item.BirthDate.ToString("yyyy-MM-dd"));
                fields.Add(item.Gender.ToString());
                fields.Add(item.CivilStatus);
                fields.Add(item.Height);
                fields.Add(item.Weight);
                fields.Add(item.HomeNo.ToString());
                fields.Add(item.Street);
                fields.Add(item.City);
                fields.Add(item.LandlineNo);
                fields.Add(item.CellphoneNo);
                fields.Add(item.EmailAddress);
                fields.Add(item.Occupation);
                fields.Add(item.EmergencyContactNo);
                fields.Add(item.EmergencyContactFName);
                fields.Add(item.EmergencyContactMName);
                fields.Add(item.EmergencyContactLName);
                fields.Add(item.BranchID.ToString());
                  string.Concat("\"", fields.ToString().Replace("\"", "\"\""), "\"");
                sb.AppendLine(string.Join(",", fields));
            }

            // }
            // else if (alert == false)
            // {

            // }
            return sb.ToString();
        }

        public string DownloadAuditTrails()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new AuditTrail().getColumns());

                foreach (var item in db.AuditTrail.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.AuditTrailID.ToString());
                    fields.Add(item.EmployeeID.ToString());
                    fields.Add(item.ActionDetail);
                    fields.Add(item.RecordID.ToString());
                    fields.Add(item.ActionTypeID.ToString());
                    fields.Add(item.DateTimeOfAction.ToString());

                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadCharSlips()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {
                sb.AppendLine(new ChargeSlip().getColumns());

                foreach (var item in db.ChargeSlips.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.DateTimePurchased.ToString());
                    fields.Add(item.DiscountRate.ToString());
                    fields.Add(item.AmtDiscount.ToString());
                    fields.Add(item.AmtDue.ToString());
                    fields.Add(item.ModeOfPayment.ToString());
                    fields.Add(item.AmtPayment.ToString());
                    fields.Add(item.GiftCertificateAmt.ToString());
                    fields.Add(item.GiftCertificateNo);
                    fields.Add(item.CheckNo);
                    fields.Add(item.CardType);
                    fields.Add(item.PatientID.ToString());
                    fields.Add(item.EmployeeID.ToString());

                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }
        public string DownloadEmployees()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new Employee().getColumns());

                foreach (var item in db.Employees.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.EmployeeID.ToString());
                    fields.Add(item.LastName);
                    fields.Add(item.FirstName);
                    fields.Add(item.MiddleName);
                    fields.Add(item.roleType.ToString());
                    fields.Add(item.BranchID.ToString());                    

                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadInventories()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new Inventory().getColumns());

                foreach (var item in db.Inventories.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.InventoryID.ToString());
                    fields.Add(item.MaterialID.ToString());
                    fields.Add(item.QtyInStock.ToString());
                    fields.Add(item.QtyToAlert.ToString());
                    fields.Add(item.LastDateUpdated.ToString());
                    fields.Add(item.BranchID.ToString());

                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadMaterialLists()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new MaterialList().getColumns());

                foreach (var item in db.MaterialList.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.MaterialListID.ToString());
                    fields.Add(item.MaterialID.ToString());
                    fields.Add(item.TreatmentID.ToString());
                    fields.Add(item.Qty.ToString());                    

                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadTreatments()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new Treatments().getColumns());

                foreach (var item in db.Treatments.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.TreatmentsID.ToString());
                    fields.Add(item.TreatmentName);
                    fields.Add(item.TreatmentPrice.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadReservations()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new Reservation().getColumns());

                foreach (var item in db.Reservations.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.ReservationID.ToString());
                    fields.Add(item.TreatmentID.ToString());
                    fields.Add(item.ReservationType.ToString());
                    fields.Add(item.DateTimeReserved.ToString());
                    fields.Add(item.Notes);
                    fields.Add(item.PatientID.ToString());
                    fields.Add(item.EmployeeID.ToString());

                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }
        //public string DownloadReportTypes()
        //{
        //    bool alert = CheckForInternetConnection();
        //    StringBuilder sb = new StringBuilder();
        //    if (alert == true)
        //    {


        //        sb.AppendLine(new ReportType().getColumns());
                
        //        foreach (var item in db.ReportTypes.ToList())
        //        {
        //            List<string> fields = new List<string>();
        //            fields.Add(item.ReportTypeID.ToString());
        //            fields.Add(item.Type.ToString());
                   

        //            sb.AppendLine(string.Join(",", fields));
        //        }

        //    }
        //    else if (alert == false)
        //    {

        //    }
        //    return sb.ToString();
        //}

        public string DownloadUnitTypes()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new UnitType().getColumns());

                foreach (var item in db.UnitTypes.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.UnitTypeID.ToString());
                    fields.Add(item.Type);


                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }
        public string DownloadReports()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new Reports().getColumns());

                foreach (var item in db.Reports.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.ReportsID.ToString());
                    fields.Add(item.ReportTypeID.ToString());
                    fields.Add(item.EmployeeID.ToString());
                    fields.Add(item.DateStartOfReport.ToString());
                    fields.Add(item.DateEndOfReport.ToString());


                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadHistory()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new History().getColumns());

                foreach (var item in db.History.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.HistoryID.ToString());
                    fields.Add(item.SessionNo.ToString());
                    fields.Add(item.PatientID.ToString());
                    fields.Add(item.TreatmentID.ToString());
                    fields.Add(item.EmployeeID.ToString());
                    fields.Add(item.DateTimeStart.ToString());
                    fields.Add(item.DateTimeEnd.ToString());
                    


                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadMaterials()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {
                sb.AppendLine(new Materials().getColumns());

                foreach (var item in db.Materials.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.MaterialID.ToString());
                    fields.Add(item.MaterialName);
                    fields.Add(item.Description);
                    fields.Add(item.Price.ToString());
                    fields.Add(item.UnitTypeID.ToString());

                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadSessionChargeSlip()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new Session_ChargeSlip().getColumns());

                foreach (var item in db.Session_ChargeSlip.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.Session_ChargeSlipID.ToString());
                    fields.Add(item.TreatmentID.ToString());
                    fields.Add(item.Qty.ToString());
                    fields.Add(item.ChargeSlipID.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadMedicineChargeSlip()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new Medicine_ChargeSlip().getColumns());

                foreach (var item in db.Medicine_ChargeSlip.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.Medicine_ChargeSlipID.ToString());
                    fields.Add(item.MaterialID.ToString());
                    fields.Add(item.Qty.ToString());
                    fields.Add(item.ChargeSlipID.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

        public string DownloadProcedures()
        {
            bool alert = CheckForInternetConnection();
            StringBuilder sb = new StringBuilder();
            if (alert == true)
            {


                sb.AppendLine(new Procedure().getColumns());

                foreach (var item in db.Procedure.ToList())
                {
                    List<string> fields = new List<string>();
                    fields.Add(item.ProcedureID.ToString());
                    fields.Add(item.ProcedureName);
                    fields.Add(item.TreatmentID.ToString());
                    fields.Add(item.StepNo.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

            }
            else if (alert == false)
            {

            }
            return sb.ToString();
        }

	}
}