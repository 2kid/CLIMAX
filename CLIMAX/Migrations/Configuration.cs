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
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CLIMAX.Models.ApplicationDbContext context)
        {
            context.Branches.AddOrUpdate(
            new Branch() { BranchID = 1, BranchName = "*" },
            new Branch() { BranchID = 2, BranchName = "branch1" },
            new Branch() { BranchID = 3, BranchName = "branch2" });

            context.ReportTypes.AddOrUpdate(
                new ReportType() { ReportTypeID = 1, Type = "Transactional"},
                new ReportType() { ReportTypeID = 2, Type = "Patients"}      
                );


            context.RoleType.AddOrUpdate(
            new RoleType() { RoleTypeId = 1, Type = "Beauty Consultant" },
            new RoleType() { RoleTypeId = 2, Type = "Therapist" },
            new RoleType() { RoleTypeId = 3, Type = "Officer in Charge" },
            new RoleType() { RoleTypeId = 4, Type = "Auditor" },
            new RoleType() { RoleTypeId = 5, Type = "Administrator" });

            context.UnitTypes.AddOrUpdate(
            new UnitType() { UnitTypeID = 1, Type = "Grams" },
            new UnitType() { UnitTypeID = 2, Type = "MilliLiters" });

            context.ActionTypes.AddOrUpdate(
            new ActionTypes() { ActionTypesID = 1, AffectedRecord = "Patient", Action = "Create", Description = "A new patient record was created. - " },
            new ActionTypes() { ActionTypesID = 2, AffectedRecord = "Patient", Action = "Delete", Description = "A patient record was deleted. - " },
            new ActionTypes() { ActionTypesID = 3, AffectedRecord = "Patient", Action = "Edit", Description = "A patient record was updated. - " },
            new ActionTypes() { AffectedRecord = "Employee", Action = "Create", Description = "A new employee record was created. - " },
            new ActionTypes() { AffectedRecord = "Employee", Action = "Delete", Description = "An employee record was deleted. - " },
            new ActionTypes() { AffectedRecord = "Employee", Action = "Edit", Description = "An employee record was updated. - " },
            new ActionTypes() { AffectedRecord = "RoleType", Action = "Create", Description = "A new role was created. - " },
            new ActionTypes() { AffectedRecord = "RoleType", Action = "Delete", Description = "A role was deleted. - " },
            new ActionTypes() { AffectedRecord = "RoleType", Action = "Edit", Description = "A role was updated. - " },
            new ActionTypes() { AffectedRecord = "UnitType", Action = "Create", Description = "A new unit was created. - " },
            new ActionTypes() { AffectedRecord = "UnitType", Action = "Delete", Description = "A unit was deleted. - " },
            new ActionTypes() { AffectedRecord = "UnitType", Action = "Edit", Description = "A unit was updated. - " },
            new ActionTypes() { AffectedRecord = "Treatment", Action = "Create", Description = "A new treatment record was created. - " },
            new ActionTypes() { AffectedRecord = "Treatment", Action = "Delete", Description = "A treatment record was deleted. - " },
            new ActionTypes() { AffectedRecord = "Treatment", Action = "Edit", Description = "A treatment record was updated. - " },
            new ActionTypes() { AffectedRecord = "Procedure", Action = "Create", Description = "A new procedure was created. - " },
            new ActionTypes() { AffectedRecord = "Procedure", Action = "Delete", Description = "A procedure was deleted. - " },
            new ActionTypes() { AffectedRecord = "Procedure", Action = "Edit", Description = "A procedure was updated. - " },
            new ActionTypes() { AffectedRecord = "History", Action = "Create", Description = "A new history record was created. - " },
            new ActionTypes() { AffectedRecord = "History", Action = "Delete", Description = "A history record was deleted. - " },
            new ActionTypes() { AffectedRecord = "History", Action = "Edit", Description = "A history record was updated. - " },
            new ActionTypes() { AffectedRecord = "Material", Action = "Create", Description = "A new material record was created. - " },
            new ActionTypes() { AffectedRecord = "Material", Action = "Delete", Description = "A material record was deleted. - " },
            new ActionTypes() { AffectedRecord = "Material", Action = "Edit", Description = "A material record was updated. - " },
            new ActionTypes() { AffectedRecord = "Inventory", Action = "Create", Description = "A new inventory record was created. - " },
            new ActionTypes() { AffectedRecord = "Inventory", Action = "Delete", Description = "An inventory record was deleted. - " },
            new ActionTypes() { AffectedRecord = "Inventory", Action = "Edit", Description = "An inventory record was updated. - " },
            new ActionTypes() { AffectedRecord = "Branch", Action = "Create", Description = "A new branch record was created. - " },
            new ActionTypes() { AffectedRecord = "Branch", Action = "Delete", Description = "A branch record was deleted. - " },
            new ActionTypes() { AffectedRecord = "Branch", Action = "Edit", Description = "A branch record was updated. - " },
            new ActionTypes() { AffectedRecord = "Company", Action = "Create", Description = "A new company record was created. - " },
            new ActionTypes() { AffectedRecord = "Company", Action = "Delete", Description = "A company record was deleted. - " },
            new ActionTypes() { AffectedRecord = "Company", Action = "Edit", Description = "A company record was updated. - " },
            new ActionTypes() { AffectedRecord = "Reservation", Action = "Create", Description = "A new reservation record was created. - " },
            new ActionTypes() { AffectedRecord = "Reservation", Action = "Delete", Description = "A reservation record was deleted. - " },
            new ActionTypes() { AffectedRecord = "Reservation", Action = "Edit", Description = "A reservation record was updated. - " },
            new ActionTypes() { AffectedRecord = "Reports", Action = "Create", Description = "A new report record was created. - " },
            new ActionTypes() { AffectedRecord = "ChargeSlip", Action = "Create", Description = "A new chargeslip transaction record was created. - " },
            new ActionTypes() { AffectedRecord = "Account", Action = "Create", Description = "A new account was created. - " },
            new ActionTypes() { AffectedRecord = "None", Action = "Send", Description = "A sms message was sent. - " }
            );

            context.SaveChanges();
          
            context.Materials.AddOrUpdate(
            new Materials() { MaterialName = "Material", Price = 12, UnitTypeID = 1, Description = "Desc" },
            new Materials() { MaterialName = "Cream", Price = 30, UnitTypeID = 1, Description = "Desc" });

            context.Employees.AddOrUpdate(
               new Employee() { BranchID = 1, EmployeeID = 1, FirstName = "sa" , RoleTypeID = 5 }
               );

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
                EmployeeID = 1
            };
            try
            {
                var adminresult = UserManager.Create(newUser, password);
                // Add User Admin to Role Admin
                //if (adminresult.Succeeded)
                //{
                IdentityResult result = UserManager.AddToRoles(newUser.Id, roles);
                //}
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
