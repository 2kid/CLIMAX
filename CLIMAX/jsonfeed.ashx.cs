using CLIMAX.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;


namespace CLIMAX
{
    /// <summary>
    /// Summary description for jsonfeed
    /// </summary>
    public class jsonfeed : IHttpHandler
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private List<object> list = new List<object>();
        public void ProcessRequest(HttpContext context)
        {
            var fromDate = ConvertFromUnixTimestamp(double.Parse(context.Request.QueryString["start"]));
            var toDate = ConvertFromUnixTimestamp(double.Parse(context.Request.QueryString["end"]));
//            var epoch = new DateTime(1970, 1, 1);
  //          var fromDate = epoch.AddMilliseconds(double.Parse(context.Request.QueryString["start"]));
    //        var toDate = epoch.AddMilliseconds(double.Parse(context.Request.QueryString["end"]));
         
            var events = db.Reservations.Where(r => fromDate.CompareTo(r.DateTimeReserved) == -1 && toDate.CompareTo(r.DateTimeReserved) == 1).ToList();//repository.GetEvents(fromDate, toDate);

            //var clientList = new List<object>();
            foreach (var e in events)
            {
                string type = "";
                if (e.ReservationType)
                {
                    type = "surgical";
                }
                else
                {
                    type = "treatment";
                }
                list.Add(
                    new
                    {
                        id = e.ReservationID,
                        title = type,
                        description = "Reserved for: " + e.patient.FullName + "\n" + e.Notes,
                        start = e.DateTimeReserved,
                        end = e.DateTimeReserved.AddHours(1),
                        allDay = false
                    });
            }
         
            //This is the important part!
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<object>));
            s.WriteObject(stream, list);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);

            context.Response.ContentType = "application/json";
            context.Response.Write(sr.ReadToEnd());
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class CalendarEvent
    {
        private string title;
        private string description;
        private string start;
        private string end;
        private string allday;

        public CalendarEvent(string title, string desc, string start, string end, string allDay)
        {
            this.title = title;
            this.description = desc;
            this.start = start;
            this.end = end;
            this.allday = allDay;

        }
    }
}