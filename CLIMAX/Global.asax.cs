using CLIMAX.Migrations;
using CLIMAX.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CLIMAX
{
    public class MvcApplication : System.Web.HttpApplication
    {
        ApplicationDbContext db = new ApplicationDbContext(); 
        protected void Application_Start()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            db.Database.Initialize(false);
           // Database.SetInitializer(new DatabaseInitializer());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);         
        }
    }
    //public class DatabaseInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    //{
    //    protected override void Seed(CLIMAX.Models.ApplicationDbContext context)
    //    {

    //        // Seed code here
    //    }
    //}
}
