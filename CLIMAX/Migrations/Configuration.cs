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
         new Branch() { BranchName = "*" },
         new Branch() { BranchName = "branch1" },
         new Branch() { BranchName = "branch2" });

            context.RoleType.AddOrUpdate(
         new RoleType() { Type = "Beauty Consultant" },
         new RoleType() { Type = "Therapist" },
         new RoleType() { Type = "Officer in Charge" },
         new RoleType() { Type = "Auditor" },
         new RoleType() { Type = "Administrator" });

            context.UnitTypes.AddOrUpdate(
         new UnitType() { Type = "Grams" },
         new UnitType() { Type = "MilliLiters" });


            context.ActionTypes.AddOrUpdate(
         new ActionTypes() { AffectedRecord = "Patient", Action = "Create" , Description = "A new patient record was created. - "},
         new ActionTypes() { AffectedRecord = "Patient", Action = "Delete", Description = "A patient record was deleted. - " },
         new ActionTypes() { AffectedRecord = "Patient", Action = "Edit", Description = "A patient record was updated. - " },
         new ActionTypes() { AffectedRecord = "Employee", Action = "Create", Description = "A new employee record was created. - " },
         new ActionTypes() { AffectedRecord = "Employee", Action = "Delete", Description = "An employee record was deleted. - " },
         new ActionTypes() { AffectedRecord = "Employee", Action = "Edit", Description = "An employee record was updated. - " },
         new ActionTypes() { AffectedRecord = "Treatment", Action = "Create", Description = "A new treatment record was created. - " },
         new ActionTypes() { AffectedRecord = "Treatment", Action = "Delete" , Description = "A treatment record was deleted. - " },
         new ActionTypes() { AffectedRecord = "Treatment", Action = "Edit", Description = "A treatment record was updated. - " },
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
         new ActionTypes() { AffectedRecord = "ChargeSlip", Action = "Create", Description = "A new chargeslip transaction record was created. - " }
         );

            context.SaveChanges();

            context.Materials.AddOrUpdate(
         new Materials() { MaterialName = "Material", Price = 12, UnitTypeID = 1, Description = "Desc" },
         new Materials() { MaterialName = "Cream", Price = 30, UnitTypeID = 1, Description = "Desc" });

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
                Email = "admin@yahoo.com"
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
