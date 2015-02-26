using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLIMAX.Controllers
{
    public class HomeController : Controller
    {
        SerialPort SP = new SerialPort();
        public ActionResult SMS(FormCollection form)
        {
            string number = form["Number"];
            string message = form["Message"];
            SP.PortName = "COM7";
            SP.Open();
            string ph_no;
            ph_no = Char.ConvertFromUtf32(34) + number + Char.ConvertFromUtf32(34);
            SP.Write("AT+CMGF=1" + Char.ConvertFromUtf32(13));
            SP.Write("AT+CMGS=" + ph_no + Char.ConvertFromUtf32(13));
            SP.Write( message + Char.ConvertFromUtf32(26) + Char.ConvertFromUtf32(13));
            SP.Close();
            return View(form);
        }
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