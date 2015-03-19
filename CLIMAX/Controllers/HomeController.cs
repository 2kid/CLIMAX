using CLIMAX.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CLIMAX.Controllers
{
    public class HomeController : Controller
    {
        CLIMAX.Models.ApplicationDbContext.ClimaxDbContext ClimaxDb = new CLIMAX.Models.ApplicationDbContext.ClimaxDbContext();
        ApplicationDbContext db = new ApplicationDbContext();  
        SerialPort SP = new SerialPort();
        [Authorize(Roles="Auditor,OIC")]
        public ActionResult SMS(FormCollection form)
        {
            string number = form["Number"];
            string message = form["Message"];

            try
            {
                SP.PortName = "COM7";
                SP.Open();
                string ph_no;
                ph_no = Char.ConvertFromUtf32(34) + number + Char.ConvertFromUtf32(34);
                SP.Write("AT+CMGF=1" + Char.ConvertFromUtf32(13));
                SP.Write("AT+CMGS=" + ph_no + Char.ConvertFromUtf32(13));
                SP.Write(message + Char.ConvertFromUtf32(26) + Char.ConvertFromUtf32(13));
                SP.Close();
                int auditId = Audit.CreateAudit(message, "Send", "None", User.Identity.Name);

            }
            catch
            {
               
            }

            return View(form);
        }


        [Authorize]
        public ActionResult UploadDatabase()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadDatabase(HttpPostedFileBase FileUpload)
        {
            try
            {
                DatabaseToCsv();
                string[] Tables = { "AuditTrails", "Patients", "ChargeSlips", "Employees", "History", "Inventories", "Materials", "Reports", "Reservations", "Treatments", "MaterialList", "UnitTypes", "Session_ChargeSlip", "Medicine_ChargeSlip", "Procedures" };
                foreach(string filename in Tables)
                {
                    try
                    {
                        Read(filename);
                    }
                    catch
                    {
                        continue;
                    }
                }

                ViewData["Feedback"] = "Upload Complete";

            }
            catch (Exception ex)
            {
                ViewData["Feedback"] = ex.Message;                
            }

            return View("UploadDatabase", ViewData["Feedback"]);
        }

        void DatabaseToCsv()
        {
            if (!Directory.Exists(Server.MapPath(@"Backup")))
            {
                Directory.CreateDirectory(Server.MapPath(@"Backup"));
            }

            var path = Path.Combine(Server.MapPath(@"~/Backup"), "AuditTrails.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadAuditTrails());

            path = Path.Combine(Server.MapPath(@"~/Backup"), "Patients.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadPatient());


            path = Path.Combine(Server.MapPath(@"~/Backup"), "ChargeSlips.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadCharSlips());


            path = Path.Combine(Server.MapPath(@"~/Backup"), "Employees.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadEmployees());


            path = Path.Combine(Server.MapPath(@"~/Backup"), "History.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadHistory());


            path = Path.Combine(Server.MapPath(@"~/Backup"), "Inventories.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadInventories());


            path = Path.Combine(Server.MapPath(@"~/Backup"), "Materials.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadMaterialLists());


            path = Path.Combine(Server.MapPath(@"~/Backup"), "Reports.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadReports());


            path = Path.Combine(Server.MapPath(@"~/Backup"), "Reservations.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadReservations());


            path = Path.Combine(Server.MapPath(@"~/Backup"), "Treatments.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadTreatments());
        

            path = Path.Combine(Server.MapPath(@"~/Backup"), "MaterialList.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadMaterialLists());


            //path = Path.Combine(Server.MapPath(@"~/Backup"), "ReportTypes.csv");
            //System.IO.File.WriteAllText(path, new Backup().DownloadAuditTrails());

            path = Path.Combine(Server.MapPath(@"~/Backup"), "UnitTypes.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadUnitTypes());

             path = Path.Combine(Server.MapPath(@"~/Backup"), "Session_ChargeSlip.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadSessionChargeSlip());

             path = Path.Combine(Server.MapPath(@"~/Backup"), "Medicine_ChargeSlip.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadMedicineChargeSlip());

             path = Path.Combine(Server.MapPath(@"~/Backup"), "Procedures.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadProcedures());
        }

        public void Read(string filename)
        {
            var path = Path.Combine(Server.MapPath(@"~/Backup"), filename + ".csv");
            StreamReader oStreamReader = new StreamReader(path);

            DataTable oDataTable = null;
            int RowCount = 0;
            string[] ColumnNames = null;
            string[] oStreamDataValues = null;
            //using while loop read the stream data till end
            while (!oStreamReader.EndOfStream)
            {
                String oStreamRowData = oStreamReader.ReadLine().Trim();
                if (oStreamRowData.Length > 0)
                {
                    oStreamDataValues = oStreamRowData.Split(',');
                    //Bcoz the first row contains column names, we will poluate 
                    //the column name by
                    //reading the first row and RowCount-0 will be true only once
                    if (RowCount == 0)
                    {
                        RowCount = 1;
                        ColumnNames = oStreamRowData.Split(',');
                        oDataTable = new DataTable();

                        //using foreach looping through all the column names
                        foreach (string csvcolumn in ColumnNames)
                        {
                            DataColumn oDataColumn = new DataColumn(csvcolumn.ToUpper(), typeof(string));

                            //setting the default value of empty.string to newly created column
                            oDataColumn.DefaultValue = string.Empty;

                            //adding the newly created column to the table
                            oDataTable.Columns.Add(oDataColumn);
                        }
                    }
                    else
                    {
                        //creates a new DataRow with the same schema as of the oDataTable            
                        DataRow oDataRow = oDataTable.NewRow();

                        //using foreach looping through all the column names
                        for (int i = 0; i < ColumnNames.Length; i++)
                        {
                            oDataRow[ColumnNames[i]] = oStreamDataValues[i] == null ? string.Empty : oStreamDataValues[i].ToString();
                        }

                        //adding the newly created row with data to the oDataTable       
                        oDataTable.Rows.Add(oDataRow);
                    }
                }
            }
            //close the oStreamReader object
            oStreamReader.Close();
            //release all the resources used by the oStreamReader object
            oStreamReader.Dispose();

            for (int a = 0; a < oDataTable.Rows.Count; a++)
            {
                #region Inserting Data
                //inside this loop create your object/entity for example :
                switch (filename)
                {
                    case "ChargeSlips":
                        ChargeSlip chargeslip = new ChargeSlip();

                        int chargeSlipId;
                        DateTime DateTimePurchased;
                        int DiscountRate;
                        double AmtDiscount;
                        double AmtDue;
                        double AmtPayment;
                        double GiftCertAmt;
                        int PatientID;
                        int EmployeeID;

                        if (int.TryParse(oDataTable.Rows[a][0].ToString(), out chargeSlipId))
                            chargeslip.ChargeSlipID = chargeSlipId;
                        if (DateTime.TryParse(oDataTable.Rows[a][1].ToString(), out DateTimePurchased))
                            chargeslip.DateTimePurchased = DateTimePurchased;
                        if (int.TryParse(oDataTable.Rows[a][2].ToString(), out DiscountRate))
                            chargeslip.DiscountRate = DiscountRate;
                        if (double.TryParse(oDataTable.Rows[a][3].ToString(), out AmtDiscount))
                            chargeslip.AmtDiscount = AmtDiscount;
                        if (double.TryParse(oDataTable.Rows[a][4].ToString(), out AmtDue))
                            chargeslip.AmtDue = AmtDue;
                        chargeslip.ModeOfPayment = oDataTable.Rows[a][5].ToString();
                        if (double.TryParse(oDataTable.Rows[a][6].ToString(), out AmtPayment))
                            chargeslip.AmtPayment = AmtPayment;
                        if (double.TryParse(oDataTable.Rows[a][7].ToString(), out GiftCertAmt))
                            chargeslip.GiftCertificateAmt = GiftCertAmt;
                        chargeslip.GiftCertificateNo = oDataTable.Rows[a][8].ToString();
                        chargeslip.CheckNo = oDataTable.Rows[a][9].ToString();
                        chargeslip.CardType = oDataTable.Rows[a][10].ToString();
                        if (int.TryParse(oDataTable.Rows[a][11].ToString(), out PatientID))
                            chargeslip.PatientID = PatientID;
                        if (int.TryParse(oDataTable.Rows[a][12].ToString(), out EmployeeID))
                            chargeslip.EmployeeID = EmployeeID;
                        if (Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.ChargeSlips.Add(chargeslip);
                            ClimaxDb.SaveChanges();
                            db.ChargeSlips.Remove(chargeslip);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet.");
                        }
                        break;

                    case "Patients":
                        Patient newPatient = new Patient();
                        DateTime Birthdate;
                        int HomeNo;
                        int BranchID;
                        if(int.TryParse(oDataTable.Rows[a][0].ToString(),out PatientID))
                        {
                            newPatient.PatientID = PatientID;
                        }
                        newPatient.FirstName = oDataTable.Rows[a][1].ToString();
                        newPatient.MiddleName = oDataTable.Rows[a][2].ToString();
                        newPatient.LastName = oDataTable.Rows[a][3].ToString();

                        if(DateTime.TryParse(oDataTable.Rows[a][4].ToString(),out Birthdate))
                        newPatient.BirthDate = Birthdate;
                        if (oDataTable.Rows[a][5].ToString() == "TRUE")
                            newPatient.Gender = true;
                        else
                            newPatient.Gender = false;
                        newPatient.CivilStatus = oDataTable.Rows[a][6].ToString();
                        newPatient.Height = oDataTable.Rows[a][7].ToString();
                        newPatient.Weight = oDataTable.Rows[a][8].ToString();
                        if (int.TryParse(oDataTable.Rows[a][9].ToString(), out HomeNo))
                        {
                            newPatient.HomeNo = HomeNo;
                        }
                        newPatient.Street = oDataTable.Rows[a][10].ToString();
                        newPatient.City = oDataTable.Rows[a][11].ToString();
                        newPatient.LandlineNo = oDataTable.Rows[a][12].ToString();
                        newPatient.CellphoneNo = oDataTable.Rows[a][13].ToString();
                        newPatient.EmailAddress = oDataTable.Rows[a][14].ToString();
                        newPatient.Occupation = oDataTable.Rows[a][15].ToString();
                        newPatient.EmergencyContactNo = oDataTable.Rows[a][16].ToString();
                        newPatient.EmergencyContactFName = oDataTable.Rows[a][17].ToString();
                        newPatient.EmergencyContactMName = oDataTable.Rows[a][18].ToString();
                        newPatient.EmergencyContactLName = oDataTable.Rows[a][19].ToString();
                        if (int.TryParse(oDataTable.Rows[a][20].ToString(), out BranchID))
                        {
                            newPatient.BranchID = BranchID;
                        }
                        if (Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.Patients.Add(newPatient);
                            ClimaxDb.SaveChanges();
                            db.Patients.Remove(newPatient);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet.");
                        }
                        break;

                    case "Employees":
                        Employee emp = new Employee();
                        int RoleTypeId;
                        int BranchId;
                        if (int.TryParse(oDataTable.Rows[a][0].ToString(), out EmployeeID))
                            emp.EmployeeID = EmployeeID;
                        emp.LastName = oDataTable.Rows[a][1].ToString();
                        emp.FirstName = oDataTable.Rows[a][2].ToString();
                        emp.MiddleName = oDataTable.Rows[a][3].ToString();
                        if(int.TryParse(oDataTable.Rows[a][4].ToString(),out RoleTypeId))
                        emp.RoleTypeID = RoleTypeId;
                        if (int.TryParse(oDataTable.Rows[a][5].ToString(), out BranchId))
                            emp.BranchID = BranchId;
                        if (Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.Employees.Add(emp);
                            ClimaxDb.SaveChanges();
                            db.Employees.Remove(emp);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet.");
                        }
                        break;

                    case "Inventories":
                        Inventory inventory = new Inventory();
                        int InventoryID;
                        int MaterialID;
                        int qtyInStock;
                        int qtyToAlert;
                        DateTime lastUpdate;
        
                        if (int.TryParse(oDataTable.Rows[a][0].ToString(), out InventoryID))
                            inventory.InventoryID = InventoryID;
                        if (int.TryParse(oDataTable.Rows[a][1].ToString(), out MaterialID))
                            inventory.MaterialID = MaterialID;
                        if (int.TryParse(oDataTable.Rows[a][2].ToString(), out qtyInStock))
                            inventory.QtyInStock = qtyInStock;
                        if (int.TryParse(oDataTable.Rows[a][3].ToString(), out qtyToAlert))
                            inventory.QtyToAlert = qtyToAlert;
                        if (DateTime.TryParse(oDataTable.Rows[a][4].ToString(), out lastUpdate))
                            inventory.LastDateUpdated = lastUpdate;
                        if (int.TryParse(oDataTable.Rows[a][5].ToString(), out BranchID))
                            inventory.BranchID = BranchID;
                        if (Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.Inventories.Add(inventory);
                            ClimaxDb.SaveChanges();
                            db.Inventories.Remove(inventory);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet.");
                        }
                        break;

                        case "MaterialList":
                        int MaterialListID;
                        int TreatmentID;
                        int Qty;
                        MaterialList mList = new MaterialList();

                        if (int.TryParse(oDataTable.Rows[a][0].ToString(), out MaterialListID))
                            mList.MaterialListID = MaterialListID;
                        if (int.TryParse(oDataTable.Rows[a][1].ToString(), out TreatmentID))
                            mList.TreatmentID = TreatmentID;
                        if (int.TryParse(oDataTable.Rows[a][2].ToString(), out MaterialID))
                            mList.MaterialID = MaterialID;
                        if (int.TryParse(oDataTable.Rows[a][3].ToString(), out Qty))
                            mList.Qty = Qty;
                        if (Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.MaterialList.Add(mList);
                            ClimaxDb.SaveChanges();
                            db.MaterialList.Remove(mList);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet.");
                        }         
                            break;

                    case "History":
                            History history = new History();
                            int HistoryID;
                            int SessionNo;
                            DateTime Start;
                            DateTime End;
                            if (int.TryParse(oDataTable.Rows[a][0].ToString(), out HistoryID))
                                history.HistoryID = HistoryID;
                            if (int.TryParse(oDataTable.Rows[a][1].ToString(), out SessionNo))
                                history.SessionNo = SessionNo;
                            if (int.TryParse(oDataTable.Rows[a][2].ToString(), out PatientID))
                                history.PatientID = PatientID;
                            if (int.TryParse(oDataTable.Rows[a][3].ToString(), out TreatmentID))
                                history.TreatmentID = TreatmentID;
                            if (int.TryParse(oDataTable.Rows[a][4].ToString(), out EmployeeID))
                                history.EmployeeID = EmployeeID;
                            if (DateTime.TryParse(oDataTable.Rows[a][5].ToString(), out Start))
                                history.DateTimeStart = Start;
                            if (DateTime.TryParse(oDataTable.Rows[a][6].ToString(), out End))
                                history.DateTimeEnd = End;
                            if (Backup.CheckForInternetConnection())
                            {
                                ClimaxDb.History.Add(history);
                                ClimaxDb.SaveChanges();
                                db.History.Remove(history);
                                db.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet.");
                            }    
                            break;

                    case "AuditTrails":
                            AuditTrail audit = new AuditTrail();
                            int AuditTrailID;
                            int RecordID;
                            int ActionTypeID;
                            DateTime DateTimeLog;
                            if (int.TryParse(oDataTable.Rows[a][0].ToString(), out AuditTrailID))
                                audit.AuditTrailID = AuditTrailID;
                            if (int.TryParse(oDataTable.Rows[a][1].ToString(), out EmployeeID))
                                audit.EmployeeID = EmployeeID;
                            audit.ActionDetail = oDataTable.Rows[a][2].ToString();
                            if (int.TryParse(oDataTable.Rows[a][3].ToString(), out RecordID))
                                audit.RecordID = RecordID;
                            if (int.TryParse(oDataTable.Rows[a][4].ToString(), out ActionTypeID))
                                audit.ActionTypeID = ActionTypeID;
                            if (DateTime.TryParse(oDataTable.Rows[a][5].ToString(), out DateTimeLog))
                                audit.DateTimeOfAction = DateTimeLog;
                            if (Backup.CheckForInternetConnection())
                            {
                                ClimaxDb.AuditTrail.Add(audit);
                                ClimaxDb.SaveChanges();
                                db.AuditTrail.Remove(audit);
                                db.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet.");
                            }    
                            break;
      case "UnitTypes":
                        UnitType unittype = new UnitType();
                        int unitTypeId;
                        if(int.TryParse(oDataTable.Rows[a][0].ToString(),out unitTypeId))
                        {unittype.UnitTypeID = unitTypeId;}
                        unittype.Type = oDataTable.Rows[a][1].ToString();
                        if (Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.UnitTypes.Add(unittype);
                            ClimaxDb.SaveChanges();
                            db.UnitTypes.Remove(unittype);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet connection.");
                        }
                        break;

                    case "Treatments":
                        Treatments treatment = new Treatments();
                        if(int.TryParse(oDataTable.Rows[a][0].ToString(),out TreatmentID))
                        {
                            treatment.TreatmentsID = TreatmentID;
                        }
                        treatment.TreatmentName = oDataTable.Rows[a][1].ToString();
                        double TreatmentPrice;
                        if(double.TryParse(oDataTable.Rows[a][2].ToString(), out TreatmentPrice))
                        {treatment.TreatmentPrice = TreatmentPrice;}
                        if(Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.Treatments.Add(treatment);
                            ClimaxDb.SaveChanges();
                            db.Treatments.Remove(treatment);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet connection.");
                        }
                        break;

                    case "Materials":
                        Materials material = new Materials();
                        double Price;
                        int UnitTypeID;
                        if(int.TryParse(oDataTable.Rows[a][0].ToString(), out MaterialID))
                        {
                            material.MaterialID = MaterialID;
                        }
                        material.MaterialName = oDataTable.Rows[a][1].ToString();
                        material.Description = oDataTable.Rows[a][2].ToString();
                        if(double.TryParse(oDataTable.Rows[a][3].ToString(), out Price))
                        {
                            material.Price = Price;
                        }
                        if(int.TryParse(oDataTable.Rows[a][4].ToString(), out UnitTypeID))
                        {
                            material.UnitTypeID = UnitTypeID;
                        }
                        if(Backup.CheckForInternetConnection()){
                            ClimaxDb.Materials.Add(material);
                            ClimaxDb.SaveChanges();
                            db.Materials.Remove(material);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet connection.");
                        }
                        break;

                    case "Session_ChargeSlip":
                        Session_ChargeSlip sessionchargeslip = new Session_ChargeSlip();
                        int Session_ChargeSlipID;
                        int ChargeSlipID;
                        if(int.TryParse(oDataTable.Rows[a][0].ToString(), out Session_ChargeSlipID))
                        {
                            sessionchargeslip.Session_ChargeSlipID = Session_ChargeSlipID;
                        }
                        if(int.TryParse(oDataTable.Rows[a][1].ToString(), out TreatmentID))
                        {
                            sessionchargeslip.TreatmentID = TreatmentID;
                        }
                        if(int.TryParse(oDataTable.Rows[a][2].ToString(), out Qty))
                        {
                            sessionchargeslip.Qty = Qty;
                        }
                        if(int.TryParse(oDataTable.Rows[a][3].ToString(), out ChargeSlipID))
                        {
                            sessionchargeslip.ChargeSlipID = ChargeSlipID;
                        }
                        if(Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.Session_ChargeSlip.Add(sessionchargeslip);
                            ClimaxDb.SaveChanges();
                            db.Session_ChargeSlip.Remove(sessionchargeslip);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet connection.");
                        }
                        break;

                    case "Medicine_ChargeSlip":
                        Medicine_ChargeSlip medschargeslip = new Medicine_ChargeSlip();
                        int Medicine_ChargeSlipID;
                        if(int.TryParse(oDataTable.Rows[a][0].ToString(), out Medicine_ChargeSlipID))
                        {
                            medschargeslip.Medicine_ChargeSlipID = Medicine_ChargeSlipID;
                        }
                        if(int.TryParse(oDataTable.Rows[a][1].ToString(), out MaterialID))
                        {
                            medschargeslip.MaterialID = MaterialID;
                        }
                        if(int.TryParse(oDataTable.Rows[a][2].ToString(), out Qty))
                        {
                            medschargeslip.Qty = Qty;
                        }
                        if(int.TryParse(oDataTable.Rows[a][3].ToString(), out ChargeSlipID))
                        {
                            medschargeslip.ChargeSlipID = ChargeSlipID;
                        }
                        if(Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.Medicine_ChargeSlip.Add(medschargeslip);
                            ClimaxDb.SaveChanges();
                            db.Medicine_ChargeSlip.Remove(medschargeslip);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet connection.");
                        }
                        break;

                    case "Procedures":
                        Procedure procedure = new Procedure();
                        int ProcedureID;
                        int StepNo;
                        if(int.TryParse(oDataTable.Rows[a][0].ToString(), out ProcedureID))
                        {
                            procedure.ProcedureID = ProcedureID;
                        }
                        procedure.ProcedureName = oDataTable.Rows[a][1].ToString();
                        if(int.TryParse(oDataTable.Rows[a][2].ToString(), out TreatmentID))
                        {
                            procedure.TreatmentID = TreatmentID;
                        }
                        if(int.TryParse(oDataTable.Rows[a][3].ToString(), out StepNo))
                        {
                            procedure.StepNo = StepNo;
                        }
                        if(Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.Procedure.Add(procedure);
                            ClimaxDb.SaveChanges();
                            db.Procedure.Remove(procedure);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet connection.");
                        }
                        break;

                    case "Reports":
                        Reports report = new Reports();
                        int ReportsID;
                        int ReportTypeID;
                        DateTime DateStartOfReport;
                        DateTime DateEndOfReport;
                        if(int.TryParse(oDataTable.Rows[a][0].ToString(), out ReportsID))
                        {
                            report.ReportsID = ReportsID;
                        }
                        if(int.TryParse(oDataTable.Rows[a][1].ToString(), out ReportTypeID))
                        {
                            report.ReportTypeID = ReportTypeID;
                        }
                        if(int.TryParse(oDataTable.Rows[a][2].ToString(), out EmployeeID))
                        {
                            report.EmployeeID = EmployeeID;
                        }
                        if(DateTime.TryParse(oDataTable.Rows[a][3].ToString(), out DateStartOfReport))
                        {
                            report.DateStartOfReport = DateStartOfReport;
                        }
                        if(DateTime.TryParse(oDataTable.Rows[a][4].ToString(), out DateEndOfReport))
                        {
                            report.DateEndOfReport = DateEndOfReport;
                        }
                        if(Backup.CheckForInternetConnection()){
                            ClimaxDb.Reports.Add(report);
                            ClimaxDb.SaveChanges();
                            db.Reports.Remove(report);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet connection.");
                        }
                        break;

                    case "Reservations":
                        Reservation reservation = new Reservation();
                        int ReservationID;
                        DateTime DateTimeReserved;
                        if(int.TryParse(oDataTable.Rows[a][0].ToString(), out ReservationID))
                        {
                            reservation.ReservationID = ReservationID;
                        }
                        if(int.TryParse(oDataTable.Rows[a][1].ToString(), out TreatmentID))
                        {
                            reservation.TreatmentID = TreatmentID;
                        }
                        if(oDataTable.Rows[a][2].ToString() =="TRUE")
                        {
                            reservation.ReservationType = true;
                        }
                        else
                        {
                            reservation.ReservationType = false;
                        }
                        if(DateTime.TryParse(oDataTable.Rows[a][3].ToString(), out DateTimeReserved))
                        {
                            reservation.DateTimeReserved = DateTimeReserved;
                        }
                        reservation.Notes = oDataTable.Rows[a][4].ToString();
                        if(int.TryParse(oDataTable.Rows[a][5].ToString(), out PatientID))
                        {
                            reservation.PatientID = PatientID;
                        }
                        if(int.TryParse(oDataTable.Rows[a][6].ToString(), out EmployeeID))
                        {
                            reservation.EmployeeID = EmployeeID;
                        }
                      
                        if(Backup.CheckForInternetConnection())
                        {
                            ClimaxDb.Reservations.Add(reservation);
                            ClimaxDb.SaveChanges();
                            db.Reservations.Remove(reservation);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Sync Failed. Please check your internet connection.");
                        }
                        break;
                }

           
                //ADD LOGIC HERE FOR CHECKING OF INTERNET CONNECTIVITY

                //if connected to internet INSERT DATA TO CENTRAL DATABASE

                //CREATE ANOTHER INSTANCE of DATABASE CONTEXT  pointing to the CENTRAL DATABASE / database deployed on the cloud

               // db.Employees.Add(myEmployee);
                //db.SaveChanges();

                //ALSO I suggest, after insertion of data on the local host to the central database, delete all data on the localhost.
                //
                #endregion
            }
        
        }
    
        public ActionResult Index()
        {
     
            return View();
        }

        public ActionResult MainMenu()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}