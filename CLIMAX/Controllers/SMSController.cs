using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLIMAX.Controllers
{
    public class SMSController : Controller
    {
        //
        // GET: /SMS/
        SerialPort SP = new SerialPort();
        public ActionResult SMS(FormCollection form)
        {
            string number = form["Number"];
            string message = form["Message"];

            
            SP.PortName = "COM5";
            SP.Open();
            string ph_no;
            ph_no = Char.ConvertFromUtf32(34) + number + Char.ConvertFromUtf32(34);
            SP.Write("AT+CMGF=1" + Char.ConvertFromUtf32(13));
            SP.Write("AT+CMGS=" + ph_no + Char.ConvertFromUtf32(13));
            SP.Write(message + Char.ConvertFromUtf32(26) + Char.ConvertFromUtf32(13));
            SP.Close();
            Audit.CreateAudit(message, "Send", "None", 0, User.Identity.Name);     

            return View(form);
        }
	}
}