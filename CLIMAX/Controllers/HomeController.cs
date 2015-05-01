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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CLIMAX.Controllers
{
    public class HomeController : Controller
    {
        CLIMAX.Models.ApplicationDbContext.ClimaxDbContext ClimaxDb = new CLIMAX.Models.ApplicationDbContext.ClimaxDbContext();
        ApplicationDbContext db = new ApplicationDbContext();
       
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