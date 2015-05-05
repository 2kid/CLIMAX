using CLIMAX.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CLIMAX.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext offlineDb = new ApplicationDbContext();
        ClimaxDbContext onlineDb = new ClimaxDbContext();
        public ActionResult Index()
        {
            //onlineDb.Branches.Add(new Branch()
            //{
            //    BranchName = "GreenBelt Branch",
            //    ContactNo = "09228765432",
            //    isEnabled = false,
            //    Location = "Makati"
            //});
            //onlineDb.SaveChanges();
            return View();
        }


        [Authorize]
        public ActionResult UploadDatabase()
        {
            if (HttpContext.Request.Url.AbsoluteUri.StartsWith("http://localhost"))
            {
                return View();
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UploadDatabase(string thing)
        {
            StringBuilder message = new StringBuilder("Tables   -   Status");
            try
            {
                DatabaseToCsv();
                string[] Tables = { "Branch", "Patient", "ChargeSlip", "Employee", "History", "Inventory", "Material", "Reports", "Reservation", "Treatment", "UnitType", "Procedure" };

                foreach (string filename in Tables)
                {
                    try
                    {
                        await Read(filename);
                        message.Append("\n" + filename + " - Success");
                    }
                    catch (Exception ex)
                    {
                        message.Append("\n" + filename + " - Failed");
                        continue;
                    }
                }

                message.Append("\nUpload Complete");
                ViewData["Feedback"] = message;

            }
            catch (Exception ex)
            {
                ViewData["Feedback"] = ex.Message;
            }

            return View("UploadDatabase", ViewData["Feedback"]);
        }

        public async Task<bool> Read(string filename)
        {
            List<AuditTrail> onlineAudits = onlineDb.AuditTrail.Include(a => a.actionType).Where(r => r.actionType.AffectedRecord == filename).ToList();

            List<AuditTrail> audits = offlineDb.AuditTrail.Include(a => a.actionType).Where(r => r.actionType.AffectedRecord == filename).ToList();
            foreach (AuditTrail item in audits)
            {
                AuditTrail existingAudit = onlineAudits.Where(r => r.ActionTypeID == item.ActionTypeID && r.ActionDetail == item.ActionDetail && r.DateTimeOfAction == item.DateTimeOfAction).SingleOrDefault();
                if (existingAudit == null)
                {
                    offlineDb.Entry<AuditTrail>(item).State = EntityState.Detached;
                    onlineDb.AuditTrail.Add(item);
                    switch (filename)
                    {
                        case "Branch":
                            Branch branch = offlineDb.Branches.Where(r => r.BranchID == item.RecordID).Single();
                            offlineDb.Entry<Branch>(branch).State = EntityState.Detached;
                            branch.BranchID = 0;
                            if (Backup.CheckForInternetConnection())
                            {                              
                                onlineDb.Branches.Add(branch);
                                await onlineDb.SaveChangesAsync();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet.");
                            }
                            break;

                        case "ChargeSlip":
                            ChargeSlip chargeslip = offlineDb.ChargeSlips.Where(r => r.ChargeSlipID == item.RecordID).Single();

                            List<Medicine_ChargeSlip> medicines = offlineDb.Medicine_ChargeSlip.Where(r => r.ChargeSlipID == chargeslip.ChargeSlipID).ToList();

                            List<Session_ChargeSlip> sessions = offlineDb.Session_ChargeSlip.Where(r => r.ChargeSlipID == chargeslip.ChargeSlipID).ToList();

                            offlineDb.Entry<ChargeSlip>(chargeslip).State = EntityState.Detached;                        
                            chargeslip.ChargeSlipID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                               
                                onlineDb.ChargeSlips.Add(chargeslip);
                                await onlineDb.SaveChangesAsync();
                            
                                for (int i = 0; i < medicines.Count; i++)
                                {
                                    offlineDb.Entry<Medicine_ChargeSlip>(medicines[i]).State = EntityState.Detached;
                    
                                    medicines[i].Medicine_ChargeSlipID = 0;
                                    medicines[i].ChargeSlipID = chargeslip.ChargeSlipID;
                                }
                                for (int i = 0; i < sessions.Count; i++)
                                {
                                    offlineDb.Entry<Session_ChargeSlip>(sessions[i]).State = EntityState.Detached;
                        
                                    sessions[i].Session_ChargeSlipID = 0;
                                    sessions[i].ChargeSlipID = chargeslip.ChargeSlipID;
                                }

                                onlineDb.Medicine_ChargeSlip.AddRange(medicines);
                                onlineDb.Session_ChargeSlip.AddRange(sessions);
                               await onlineDb.SaveChangesAsync();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet.");
                            }
                            break;

                        case "Patient":
                            Patient newPatient = offlineDb.Patients.Where(r => r.PatientID == item.RecordID).Single();
                                offlineDb.Entry<Patient>(newPatient).State = EntityState.Detached;
                              
                            newPatient.PatientID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.Patients.Add(newPatient);
                                onlineDb.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet.");
                            }
                            break;

                        case "Employee":
                            Employee emp = offlineDb.Employees.Where(r => r.EmployeeID == item.RecordID).Single();
                                offlineDb.Entry<Employee>(emp).State = EntityState.Detached;
                             
                            emp.EmployeeID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.Employees.Add(emp);
                                onlineDb.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet.");
                            }
                            break;

                        case "Inventory":
                            Inventory inventory = offlineDb.Inventories.Where(r => r.InventoryID == item.RecordID).Single();
                            offlineDb.Entry<Inventory>(inventory).State = EntityState.Detached;
                             
                            inventory.InventoryID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.Inventories.Add(inventory);
                                onlineDb.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet.");
                            }
                            break;

                        case "History":
                            History history = offlineDb.History.Where(r => r.HistoryID == item.RecordID).Single();
                            offlineDb.Entry<History>(history).State = EntityState.Detached;
                            history.HistoryID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.History.Add(history);
                                onlineDb.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet.");
                            }
                            break;


                        case "UnitType":
                            UnitType unittype = offlineDb.UnitTypes.Where(r => r.UnitTypeID == item.RecordID).Single();

                            offlineDb.Entry<UnitType>(unittype).State = EntityState.Detached;
                            unittype.UnitTypeID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.UnitTypes.Add(unittype);
                                onlineDb.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet connection.");
                            }
                            break;

                        case "Treatment":
                            Treatments treatment = offlineDb.Treatments.Where(r => r.TreatmentsID == item.RecordID).Single();

                            List<MaterialList> medlist = offlineDb.MaterialList.Where(r => r.TreatmentID == treatment.TreatmentsID).ToList();
                            offlineDb.Entry<Treatments>(treatment).State = EntityState.Detached;
                             
                            treatment.TreatmentsID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.Treatments.Add(treatment);
                                onlineDb.SaveChanges();

                                for (int i = 0; i < medlist.Count; i++)
                                {
                                    offlineDb.Entry<MaterialList>(medlist[i]).State = EntityState.Detached;
                          
                                    medlist[i].MaterialListID = 0;
                                    medlist[i].TreatmentID = treatment.TreatmentsID;
                                }
                                onlineDb.MaterialList.AddRange(medlist);
                                onlineDb.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet connection.");
                            }
                            break;

                        case "Material":
                            Materials material = offlineDb.Materials.Where(r => r.MaterialID == item.RecordID).Single();
                            offlineDb.Entry<Materials>(material).State = EntityState.Detached;
                             
                            material.MaterialID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.Materials.Add(material);
                                onlineDb.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet connection.");
                            }
                            break;


                        case "Procedure":
                            Procedure procedure = offlineDb.Procedure.Where(r => r.ProcedureID == item.RecordID).Single();
                            offlineDb.Entry<Procedure>(procedure).State = EntityState.Detached;
                             
                            procedure.ProcedureID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.Procedure.Add(procedure);
                                onlineDb.SaveChanges();

                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet connection.");
                            }
                            break;

                        case "Reports":
                            Reports report = offlineDb.Reports.Where(r => r.ReportsID == item.RecordID).Single();
                            offlineDb.Entry<Reports>(report).State = EntityState.Detached;
                             
                            report.ReportsID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.Reports.Add(report);
                                onlineDb.SaveChanges();

                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet connection.");
                            }
                            break;

                        case "Reservation":
                            Reservation reservation = offlineDb.Reservations.Where(r => r.ReservationID == item.RecordID).Single();
                            offlineDb.Entry<Reservation>(reservation).State = EntityState.Detached;
                             
                            reservation.ReservationID = 0;
                            if (Backup.CheckForInternetConnection())
                            {
                                onlineDb.Reservations.Add(reservation);
                                onlineDb.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Sync Failed. Please check your internet connection.");
                            }
                            break;
                    }
                }
            }
            return true;
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

            path = Path.Combine(Server.MapPath(@"~/Backup"), "Branches.csv");
            System.IO.File.WriteAllText(path, new Backup().DownloadBranches());


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

    }
}