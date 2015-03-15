using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CLIMAX.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int EmployeeID { get; set; }
        public virtual Employee employee { get; set; }

        public bool isActive { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       public DbSet<AuditTrail> AuditTrail { get; set; }
       public DbSet<ActionTypes> ActionTypes { get; set; }
       public DbSet<Branch> Branches { get; set; }
       public DbSet<ChargeSlip> ChargeSlips { get; set; }
       public DbSet<Employee> Employees { get; set; }
       public DbSet<History> History { get; set; }
       public DbSet<Inventory> Inventories { get; set; }
       public DbSet<MaterialList> MaterialList { get; set; }
       public DbSet<Materials> Materials { get; set; }
       public DbSet<Medicine_ChargeSlip> Medicine_ChargeSlip { get; set; }
       public DbSet<Patient> Patients { get; set; }
       public DbSet<Procedure> Procedure { get; set; }
       public DbSet<Reports> Reports { get; set; }
       public DbSet<ReportType> ReportTypes { get; set; }
       public DbSet<Reservation> Reservations { get; set; }
       public DbSet<RoleType> RoleType { get; set; }
       public DbSet<Session_ChargeSlip> Session_ChargeSlip { get; set; }
       public DbSet<Treatments> Treatments { get; set; }
       public DbSet<UnitType> UnitTypes { get; set; }
       public DbSet<Survey> Surveys { get; set; }

       protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);

           modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
           modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

         //  modelBuilder.Entity<ChargeSlip>()
         //   .HasRequired(r => r.Patient)
         //   .WithOptional()
         //   .WillCascadeOnDelete(false);

         //  modelBuilder.Entity<ChargeSlip>()
         //  .HasRequired(r => r.Employee)
         //  .WithOptional()
         //  .WillCascadeOnDelete(false);

         //  modelBuilder.Entity<History>()
         //   .HasRequired(r => r.patient)
         //   .WithOptional()
         //   .WillCascadeOnDelete(false);

         //  modelBuilder.Entity<History>()
         //.HasRequired(r => r.employee)
         //.WithOptional()
         //.WillCascadeOnDelete(false);
  

       }

      

       public class IdentityManager
       {
           RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(
           new RoleStore<ApplicationRole>(new ApplicationDbContext()));

           UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>(
               new UserStore<ApplicationUser>(new ApplicationDbContext()));

           ApplicationDbContext _db = new ApplicationDbContext();

           public bool RoleExists(string name)
           {
               return _roleManager.RoleExists(name);
           }

           public bool CreateRole(string name, string description)
           {
               // Swap ApplicationRole for IdentityRole:
               var idResult = _roleManager.Create(new ApplicationRole(name, description));
               return idResult.Succeeded;
           }


           public void ClearUserRoles(string userId)
           {
               var user = _userManager.FindById(userId);
               var currentRoles = new List<IdentityUserRole>();

               currentRoles.AddRange(user.Roles);

               foreach (var role in currentRoles)
               {
                   _userManager.RemoveFromRole(userId, _db.Roles.Find(role.RoleId).Name);
               }

           }
       }
   }
}