namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActionTypes",
                c => new
                    {
                        ActionTypesID = c.Int(nullable: false, identity: true),
                        AffectedRecord = c.String(),
                        Action = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ActionTypesID);
            
            CreateTable(
                "dbo.AuditTrails",
                c => new
                    {
                        AuditTrailID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        ActionDetail = c.String(),
                        RecordID = c.Int(nullable: false),
                        ActionTypeID = c.Int(nullable: false),
                        DateTimeOfAction = c.DateTime(nullable: false),
                        actionType_ActionTypesID = c.Int(),
                    })
                .PrimaryKey(t => t.AuditTrailID)
                .ForeignKey("dbo.ActionTypes", t => t.actionType_ActionTypesID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID)
                .Index(t => t.actionType_ActionTypesID);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        RoleTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.RoleTypes", t => t.RoleTypeID, cascadeDelete: true)
                .Index(t => t.RoleTypeID);
            
            CreateTable(
                "dbo.RoleTypes",
                c => new
                    {
                        RoleTypeId = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.RoleTypeId);
            
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        BranchID = c.Int(nullable: false, identity: true),
                        BranchName = c.String(),
                        Location = c.String(),
                        ContactNo = c.String(),
                    })
                .PrimaryKey(t => t.BranchID);
            
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        BirthDate = c.DateTime(nullable: false),
                        Gender = c.Boolean(nullable: false),
                        CivilStatus = c.String(),
                        Height = c.String(),
                        Weight = c.String(),
                        HomeNo = c.Int(nullable: false),
                        Street = c.String(),
                        City = c.String(),
                        LandlineNo = c.String(),
                        CellphoneNo = c.String(),
                        EmailAddress = c.String(),
                        Occupation = c.String(),
                        CompanyID = c.Int(nullable: false),
                        EmergencyContactNo = c.String(),
                        EmergencyContactFName = c.String(),
                        EmergencyContactMName = c.String(),
                        EmergencyContactLName = c.String(),
                        BranchID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PatientID)
                .ForeignKey("dbo.Branches", t => t.BranchID, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .Index(t => t.CompanyID)
                .Index(t => t.BranchID);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyID = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        CompanyAddress = c.String(),
                        CompanyNo = c.String(),
                    })
                .PrimaryKey(t => t.CompanyID);
            
            CreateTable(
                "dbo.ChargeSlips",
                c => new
                    {
                        ChargeSlipID = c.Int(nullable: false, identity: true),
                        DateTimePurchased = c.DateTime(nullable: false),
                        ModeOfPayment = c.String(),
                        AmtPayment = c.Double(nullable: false),
                        GiftCertificateAmt = c.Double(nullable: false),
                        GiftCertificateNo = c.String(),
                        CheckNo = c.String(),
                        PatientID = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChargeSlipID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientID, cascadeDelete: true)
                .Index(t => t.PatientID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.Histories",
                c => new
                    {
                        HistoryID = c.Int(nullable: false, identity: true),
                        SessionNo = c.Int(nullable: false),
                        PatientID = c.Int(nullable: false),
                        TreatmentID = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                        DateTimeStart = c.DateTime(nullable: false),
                        DateTimeEnd = c.DateTime(nullable: false),
                        treatment_TreatmentsID = c.Int(),
                    })
                .PrimaryKey(t => t.HistoryID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientID, cascadeDelete: true)
                .ForeignKey("dbo.Treatments", t => t.treatment_TreatmentsID)
                .Index(t => t.PatientID)
                .Index(t => t.EmployeeID)
                .Index(t => t.treatment_TreatmentsID);
            
            CreateTable(
                "dbo.Treatments",
                c => new
                    {
                        TreatmentsID = c.Int(nullable: false, identity: true),
                        TreatmentName = c.String(),
                    })
                .PrimaryKey(t => t.TreatmentsID);
            
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        InventoryID = c.Int(nullable: false, identity: true),
                        MaterialID = c.Int(nullable: false),
                        QtyInStock = c.Int(nullable: false),
                        LastDateUpdated = c.DateTime(nullable: false),
                        BranchID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryID)
                .ForeignKey("dbo.Branches", t => t.BranchID, cascadeDelete: true)
                .ForeignKey("dbo.Materials", t => t.MaterialID, cascadeDelete: true)
                .Index(t => t.MaterialID)
                .Index(t => t.BranchID);
            
            CreateTable(
                "dbo.Materials",
                c => new
                    {
                        MaterialID = c.Int(nullable: false, identity: true),
                        MaterialName = c.String(),
                        Description = c.String(),
                        Price = c.Double(nullable: false),
                        UnitTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaterialID)
                .ForeignKey("dbo.UnitTypes", t => t.UnitTypeID, cascadeDelete: true)
                .Index(t => t.UnitTypeID);
            
            CreateTable(
                "dbo.UnitTypes",
                c => new
                    {
                        UnitTypeID = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.UnitTypeID);
            
            CreateTable(
                "dbo.MaterialLists",
                c => new
                    {
                        MaterialListID = c.Int(nullable: false, identity: true),
                        TreatmentID = c.Int(nullable: false),
                        MaterialID = c.Int(nullable: false),
                        Qty = c.Int(nullable: false),
                        treatment_TreatmentsID = c.Int(),
                    })
                .PrimaryKey(t => t.MaterialListID)
                .ForeignKey("dbo.Materials", t => t.MaterialID, cascadeDelete: true)
                .ForeignKey("dbo.Treatments", t => t.treatment_TreatmentsID)
                .Index(t => t.MaterialID)
                .Index(t => t.treatment_TreatmentsID);
            
            CreateTable(
                "dbo.Medicine_ChargeSlip",
                c => new
                    {
                        Medicine_ChargeSlipID = c.Int(nullable: false, identity: true),
                        Qty = c.Int(nullable: false),
                        ChargeSlipID = c.Int(nullable: false),
                        MaterialID_MaterialID = c.Int(),
                    })
                .PrimaryKey(t => t.Medicine_ChargeSlipID)
                .ForeignKey("dbo.ChargeSlips", t => t.ChargeSlipID, cascadeDelete: true)
                .ForeignKey("dbo.Materials", t => t.MaterialID_MaterialID)
                .Index(t => t.ChargeSlipID)
                .Index(t => t.MaterialID_MaterialID);
            
            CreateTable(
                "dbo.Procedures",
                c => new
                    {
                        ProcedureID = c.Int(nullable: false, identity: true),
                        ProcedureName = c.String(),
                        TreatmentID = c.Int(nullable: false),
                        StepNo = c.Int(nullable: false),
                        treatment_TreatmentsID = c.Int(),
                    })
                .PrimaryKey(t => t.ProcedureID)
                .ForeignKey("dbo.Treatments", t => t.treatment_TreatmentsID)
                .Index(t => t.treatment_TreatmentsID);
            
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        ReportsID = c.Int(nullable: false, identity: true),
                        ReportTypeID = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                        DateStartOfReport = c.DateTime(nullable: false),
                        DateEndOfReport = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReportsID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.ReportTypes", t => t.ReportTypeID, cascadeDelete: true)
                .Index(t => t.ReportTypeID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.ReportTypes",
                c => new
                    {
                        ReportTypeID = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.ReportTypeID);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        ReservationID = c.Int(nullable: false, identity: true),
                        TreatmentID = c.Int(nullable: false),
                        ReservationType = c.Boolean(nullable: false),
                        DateTimeReserved = c.DateTime(nullable: false),
                        Notes = c.String(),
                        PatientID = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                        treatment_TreatmentsID = c.Int(),
                    })
                .PrimaryKey(t => t.ReservationID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientID, cascadeDelete: true)
                .ForeignKey("dbo.Treatments", t => t.treatment_TreatmentsID)
                .Index(t => t.PatientID)
                .Index(t => t.EmployeeID)
                .Index(t => t.treatment_TreatmentsID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Session_ChargeSlip",
                c => new
                    {
                        Session_ChargeSlipID = c.Int(nullable: false, identity: true),
                        TreatmentID = c.Int(nullable: false),
                        ChargeSlipID = c.Int(nullable: false),
                        treatment_TreatmentsID = c.Int(),
                    })
                .PrimaryKey(t => t.Session_ChargeSlipID)
                .ForeignKey("dbo.ChargeSlips", t => t.ChargeSlipID, cascadeDelete: true)
                .ForeignKey("dbo.Treatments", t => t.treatment_TreatmentsID)
                .Index(t => t.ChargeSlipID)
                .Index(t => t.treatment_TreatmentsID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Session_ChargeSlip", "treatment_TreatmentsID", "dbo.Treatments");
            DropForeignKey("dbo.Session_ChargeSlip", "ChargeSlipID", "dbo.ChargeSlips");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Reservations", "treatment_TreatmentsID", "dbo.Treatments");
            DropForeignKey("dbo.Reservations", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.Reservations", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Reports", "ReportTypeID", "dbo.ReportTypes");
            DropForeignKey("dbo.Reports", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Procedures", "treatment_TreatmentsID", "dbo.Treatments");
            DropForeignKey("dbo.Medicine_ChargeSlip", "MaterialID_MaterialID", "dbo.Materials");
            DropForeignKey("dbo.Medicine_ChargeSlip", "ChargeSlipID", "dbo.ChargeSlips");
            DropForeignKey("dbo.MaterialLists", "treatment_TreatmentsID", "dbo.Treatments");
            DropForeignKey("dbo.MaterialLists", "MaterialID", "dbo.Materials");
            DropForeignKey("dbo.Inventories", "MaterialID", "dbo.Materials");
            DropForeignKey("dbo.Materials", "UnitTypeID", "dbo.UnitTypes");
            DropForeignKey("dbo.Inventories", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.Histories", "treatment_TreatmentsID", "dbo.Treatments");
            DropForeignKey("dbo.Histories", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.Histories", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.ChargeSlips", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.ChargeSlips", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Patients", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Patients", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.AuditTrails", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Employees", "RoleTypeID", "dbo.RoleTypes");
            DropForeignKey("dbo.AuditTrails", "actionType_ActionTypesID", "dbo.ActionTypes");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Session_ChargeSlip", new[] { "treatment_TreatmentsID" });
            DropIndex("dbo.Session_ChargeSlip", new[] { "ChargeSlipID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Reservations", new[] { "treatment_TreatmentsID" });
            DropIndex("dbo.Reservations", new[] { "EmployeeID" });
            DropIndex("dbo.Reservations", new[] { "PatientID" });
            DropIndex("dbo.Reports", new[] { "EmployeeID" });
            DropIndex("dbo.Reports", new[] { "ReportTypeID" });
            DropIndex("dbo.Procedures", new[] { "treatment_TreatmentsID" });
            DropIndex("dbo.Medicine_ChargeSlip", new[] { "MaterialID_MaterialID" });
            DropIndex("dbo.Medicine_ChargeSlip", new[] { "ChargeSlipID" });
            DropIndex("dbo.MaterialLists", new[] { "treatment_TreatmentsID" });
            DropIndex("dbo.MaterialLists", new[] { "MaterialID" });
            DropIndex("dbo.Materials", new[] { "UnitTypeID" });
            DropIndex("dbo.Inventories", new[] { "BranchID" });
            DropIndex("dbo.Inventories", new[] { "MaterialID" });
            DropIndex("dbo.Histories", new[] { "treatment_TreatmentsID" });
            DropIndex("dbo.Histories", new[] { "EmployeeID" });
            DropIndex("dbo.Histories", new[] { "PatientID" });
            DropIndex("dbo.ChargeSlips", new[] { "EmployeeID" });
            DropIndex("dbo.ChargeSlips", new[] { "PatientID" });
            DropIndex("dbo.Patients", new[] { "BranchID" });
            DropIndex("dbo.Patients", new[] { "CompanyID" });
            DropIndex("dbo.Employees", new[] { "RoleTypeID" });
            DropIndex("dbo.AuditTrails", new[] { "actionType_ActionTypesID" });
            DropIndex("dbo.AuditTrails", new[] { "EmployeeID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Session_ChargeSlip");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Reservations");
            DropTable("dbo.ReportTypes");
            DropTable("dbo.Reports");
            DropTable("dbo.Procedures");
            DropTable("dbo.Medicine_ChargeSlip");
            DropTable("dbo.MaterialLists");
            DropTable("dbo.UnitTypes");
            DropTable("dbo.Materials");
            DropTable("dbo.Inventories");
            DropTable("dbo.Treatments");
            DropTable("dbo.Histories");
            DropTable("dbo.ChargeSlips");
            DropTable("dbo.Companies");
            DropTable("dbo.Patients");
            DropTable("dbo.Branches");
            DropTable("dbo.RoleTypes");
            DropTable("dbo.Employees");
            DropTable("dbo.AuditTrails");
            DropTable("dbo.ActionTypes");
        }
    }
}
