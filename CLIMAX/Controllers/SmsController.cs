using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLIMAX.Controllers
{
    public class SmsController : Controller
    {
        //
        // GET: /Sms/
        SerialPort SP = new SerialPort();
        [Authorize(Roles = "Auditor,OIC")]     
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
            SP.Write(message + Char.ConvertFromUtf32(26) + Char.ConvertFromUtf32(13));
            SP.Close();
            Audit.CreateAudit(message, "Send", "None", User.Identity.Name);

 
            return View(form);
        }
	}
}