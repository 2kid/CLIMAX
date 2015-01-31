using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class ActionTypes
    {
        public int ActionTypesID { get; set; }
        public string AffectedRecord { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }  
    }
}