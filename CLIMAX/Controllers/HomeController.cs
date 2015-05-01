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
    
        public ActionResult Index()
        {
          return View();
        }
    }
}