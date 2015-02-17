using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    //for materials in a dropdown
    public class Materials
    {
        [Key]
        public int MaterialID { get; set; }
        [Required]
        [Display(Name = "Material Name")]
        public string MaterialName { get; set; }
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int UnitTypeID { get; set; }
        public virtual UnitType unitType { get; set; }
    }

    //For Materials in a list
    public class MaterialsViewModel
    {
        public int MaterialID { get; set; }
        public string MaterialName { get; set; }
        public int Qty { get; set; }
        public double TotalPrice { get; set; }
        public string unitType { get; set; }

        public string units { get { return Qty + " " + unitType; } }
    }
}