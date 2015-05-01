namespace CLIMAX.Migrations
{
    using CLIMAX.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CLIMAX.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(CLIMAX.Models.ApplicationDbContext context)
        {
            context.Branches.AddOrUpdate(
            new Branch() { BranchID = 1, BranchName = "*", isEnabled = true },
            new Branch() { BranchID = 2, BranchName = "Sta. Mesa Branch", isEnabled = true },
            new Branch() { BranchID = 3, BranchName = "MOA Branch", isEnabled = true });

            context.ReportTypes.AddOrUpdate(
                new ReportType() { ReportTypeID = 1, Type = "Sales Report"},
                new ReportType() { ReportTypeID = 2, Type = "Summary Report"},
                new ReportType() { ReportTypeID = 3, Type = "Inventory Report"}
                );

            if (context.RoleType.Count() == 0)
            {
                context.RoleType.AddOrUpdate(
                new RoleType() { RoleTypeId = 1, Type = "Beauty Consultant" , isEnabled = true},
                new RoleType() { RoleTypeId = 2, Type = "Therapist" , isEnabled = true},
                new RoleType() { RoleTypeId = 3, Type = "Officer in Charge", isEnabled =true },
                new RoleType() { RoleTypeId = 4, Type = "Auditor", isEnabled = true },
                new RoleType() { RoleTypeId = 5, Type = "Administrator", isEnabled = true });
            }

            context.UnitTypes.AddOrUpdate(
            new UnitType() { UnitTypeID = 1, Type = "Grams", isEnabled = true },
            new UnitType() { UnitTypeID = 2, Type = "MilliLiters", isEnabled = true });

            if (context.ActionTypes.Count() == 0)
            {
                context.ActionTypes.AddOrUpdate(
                new ActionTypes() { ActionTypesID = 1, AffectedRecord = "Patient", Action = "Create", Description = "A new patient record was created. - " },
                new ActionTypes() { ActionTypesID = 2, AffectedRecord = "Patient", Action = "Disable", Description = "A patient record was disabled. - " },
                new ActionTypes() { ActionTypesID = 3, AffectedRecord = "Patient", Action = "Enable", Description = "A patient record was enabled. - " },
                new ActionTypes() { ActionTypesID = 4, AffectedRecord = "Patient", Action = "Edit", Description = "A patient record was updated. - " },
                new ActionTypes() { ActionTypesID = 5, AffectedRecord = "Employee", Action = "Create", Description = "A new employee record was created. - " },
                new ActionTypes() { ActionTypesID = 6, AffectedRecord = "Employee", Action = "Disable", Description = "An employee record was disabled. - " },
                new ActionTypes() { ActionTypesID = 7, AffectedRecord = "Employee", Action = "Enable", Description = "An employee record was enabled. - " },
                new ActionTypes() { ActionTypesID = 8, AffectedRecord = "Employee", Action = "Edit", Description = "An employee record was updated. - " },
                new ActionTypes() { ActionTypesID = 9, AffectedRecord = "RoleType", Action = "Create", Description = "A new role was created. - " },
                new ActionTypes() { ActionTypesID = 10, AffectedRecord = "RoleType", Action = "Disable", Description = "A role was disabled. - " },
                new ActionTypes() { ActionTypesID = 11, AffectedRecord = "RoleType", Action = "Enable", Description = "A role was enabled. - " },
                new ActionTypes() { ActionTypesID = 12, AffectedRecord = "RoleType", Action = "Edit", Description = "A role was updated. - " },
                new ActionTypes() { ActionTypesID = 13, AffectedRecord = "UnitType", Action = "Create", Description = "A new unit was created. - " },
                new ActionTypes() { ActionTypesID = 14, AffectedRecord = "UnitType", Action = "Disable", Description = "A unit was disabled. - " },
                new ActionTypes() { ActionTypesID = 15, AffectedRecord = "UnitType", Action = "Enable", Description = "A unit was enabled. - " },
                new ActionTypes() { ActionTypesID = 16, AffectedRecord = "UnitType", Action = "Edit", Description = "A unit was updated. - " },
                new ActionTypes() { ActionTypesID = 17, AffectedRecord = "Treatment", Action = "Create", Description = "A new treatment record was created. - " },
                new ActionTypes() { ActionTypesID = 18, AffectedRecord = "Treatment", Action = "Disable", Description = "A treatment record was disabled. - " },
                new ActionTypes() { ActionTypesID = 19, AffectedRecord = "Treatment", Action = "Enable", Description = "A treatment record was enabled. - " },
                new ActionTypes() { ActionTypesID = 20, AffectedRecord = "Treatment", Action = "Edit", Description = "A treatment record was updated. - " },
                new ActionTypes() { ActionTypesID = 21, AffectedRecord = "Procedure", Action = "Create", Description = "A new procedure was created. - " },
                new ActionTypes() { ActionTypesID = 22, AffectedRecord = "Procedure", Action = "Disable", Description = "A procedure was disabled. - " },
                new ActionTypes() { ActionTypesID = 23, AffectedRecord = "Procedure", Action = "Enable", Description = "A procedure was enabled. - " },
                new ActionTypes() { ActionTypesID = 24, AffectedRecord = "Procedure", Action = "Edit", Description = "A procedure was updated. - " },
                new ActionTypes() { ActionTypesID = 25, AffectedRecord = "History", Action = "Create", Description = "A new history record was created. - " },
                new ActionTypes() { ActionTypesID = 26, AffectedRecord = "History", Action = "Disable", Description = "A history record was disabled. - " },
                new ActionTypes() { ActionTypesID = 27, AffectedRecord = "History", Action = "Enable", Description = "A history record was enabled. - " },
                new ActionTypes() { ActionTypesID = 28, AffectedRecord = "History", Action = "Edit", Description = "A history record was updated. - " },
                new ActionTypes() { ActionTypesID = 29, AffectedRecord = "Material", Action = "Create", Description = "A new material record was created. - " },
                new ActionTypes() { ActionTypesID = 30, AffectedRecord = "Material", Action = "Disable", Description = "A material record was disabled. - " },
                new ActionTypes() { ActionTypesID = 31, AffectedRecord = "Material", Action = "Enable", Description = "A material record was enabled. - " },
                new ActionTypes() { ActionTypesID = 32, AffectedRecord = "Material", Action = "Edit", Description = "A material record was updated. - " },
                new ActionTypes() { ActionTypesID = 33, AffectedRecord = "Inventory", Action = "Create", Description = "A new inventory record was created. - " },
                new ActionTypes() { ActionTypesID = 34, AffectedRecord = "Inventory", Action = "Disable", Description = "An inventory record was disabled. - " },
                new ActionTypes() { ActionTypesID = 35, AffectedRecord = "Inventory", Action = "Enable", Description = "An inventory record was enabled. - " },
                new ActionTypes() { ActionTypesID = 36, AffectedRecord = "Inventory", Action = "Edit", Description = "An inventory record was updated. - " },
                new ActionTypes() { ActionTypesID = 37, AffectedRecord = "Branch", Action = "Create", Description = "A new branch record was created. - " },
                new ActionTypes() { ActionTypesID = 38, AffectedRecord = "Branch", Action = "Disable", Description = "A branch record was disabled. - " },
                new ActionTypes() { ActionTypesID = 39, AffectedRecord = "Branch", Action = "Enable", Description = "A branch record was enabled. - " },
                new ActionTypes() { ActionTypesID = 40, AffectedRecord = "Branch", Action = "Edit", Description = "A branch record was updated. - " },
                new ActionTypes() { ActionTypesID = 41, AffectedRecord = "Reservation", Action = "Create", Description = "A new reservation record was created. - " },
                new ActionTypes() { ActionTypesID = 42, AffectedRecord = "Reservation", Action = "Delete", Description = "A reservation record was deleted. - " },
                new ActionTypes() { ActionTypesID = 43, AffectedRecord = "Reservation", Action = "Edit", Description = "A reservation record was updated. - " },
                new ActionTypes() { ActionTypesID = 44, AffectedRecord = "Reports", Action = "Create", Description = "A new report record was created. - " },
                new ActionTypes() { ActionTypesID = 45, AffectedRecord = "Reports", Action = "PDF", Description = "A report was converted to PDF. - " },
                new ActionTypes() { ActionTypesID = 46, AffectedRecord = "ChargeSlip", Action = "Create", Description = "A new chargeslip transaction record was created. - " },
                new ActionTypes() { ActionTypesID = 47, AffectedRecord = "Account", Action = "Create", Description = "A new account was created. - " },
                new ActionTypes() { ActionTypesID = 48, AffectedRecord = "Account", Action = "Disable", Description = "An account has been deactivated. - " },
                new ActionTypes() { ActionTypesID = 49, AffectedRecord = "Account", Action = "Enable", Description = "An account has been reactivated. - " },
                new ActionTypes() { ActionTypesID = 50, AffectedRecord = "None", Action = "Send", Description = "A sms message was sent. - " }
                );
            }
            context.SaveChanges();

            if (context.Materials.Count() == 0)
            {
                context.Materials.AddOrUpdate(
                new Materials() { MaterialName = "Lotion", Price = 12.00, UnitTypeID = 1, Description = "Desc", isEnabled = true },
                new Materials() { MaterialName = "Cream", Price = 30.00, UnitTypeID = 1, Description = "Desc", isEnabled = true });
            }

            if (context.Employees.Count() == 0)
            {
                context.Employees.AddOrUpdate(
                   new Employee() { BranchID = 1, FirstName = "sa", RoleTypeID = 5, isEnabled = true }
                   );
            }

            context.SaveChanges();

            bool success = false;
            string[] roles = { "OIC", "Auditor", "Admin" };
            var idManager = new CLIMAX.Models.ApplicationDbContext.IdentityManager();
            success = idManager.CreateRole(roles[0], "Officer in Charge");
            success = idManager.CreateRole(roles[1], "Auditor");
            success = idManager.CreateRole(roles[2], "Administrator");

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
            string password = "12345678";
            var newUser = new ApplicationUser()
            {
                UserName = "admin@yahoo.com",
                Email = "admin@yahoo.com",
                EmployeeID = 1,
                isActive = true
            };
            try
            {
                var adminresult = UserManager.Create(newUser, password);
                // Add User Admin to Role Admin
                IdentityResult result = UserManager.AddToRole(newUser.Id, "Admin");
            }
            catch
            {
                //do nothing
            }
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
