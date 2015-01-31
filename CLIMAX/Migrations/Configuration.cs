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

            context.SaveChanges();

            bool success = false;
            string[] roles = { "OIC", "Auditor", "Admin"};
            var idManager = new CLIMAX.Models.ApplicationDbContext.IdentityManager();
            success = idManager.CreateRole(roles[0], "Officer in Charge");
            success = idManager.CreateRole(roles[1], "Auditor");
            success = idManager.CreateRole(roles[2], "Administrator");
         
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

            string password = "12345678";
            var newUser = new ApplicationUser()
            {
                UserName = "sa",
                Email = "admin@yahoo.com"
            };

            try
            {
                var adminresult = UserManager.Create(newUser, password);

                // Add User Admin to Role Admin
                //if (adminresult.Succeeded)
                //{
                IdentityResult result = UserManager.AddToRoles(newUser.Id,roles);
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
