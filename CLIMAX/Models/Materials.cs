using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Materials
    {
        public int MaterialsID { get; set; }
        public string MaterialName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int UnitTypeID { get; set; }
        public virtual UnitType unitType { get; set; }



    }
}