using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;

namespace CLIMAX.Controllers
{
    public class SmsController : Controller
    {
        //
        // GET: /Sms/
       
        
        [Authorize(Roles = "Auditor,OIC")]     
        [HttpPost]
        public ActionResult SMS(FormCollection form)
        {
            string number = form["Number"]; //"+639153890655"; 
            string message = form["Message"] ; //"Testing 101";
            string AccountSid = "ACd74470b401f572522fb3d6471119e2b4";
            string AuthToken = "76921e307fba754a29647827fcedc937";
            var twilio = new TwilioRestClient(AccountSid, AuthToken);
            var msg = twilio.SendSmsMessage("+12053012883","+63" + number , message);
            Audit.CreateAudit(form["Message"], "Send", "None", User.Identity.Name);

            return View(form);           
        }

        public ActionResult SMS()
        {
            return View();
        }
        
	}
}