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
        [StringLength(250)]
        public string Description { get; set; }
        [RegularExpression("^[1-9][0-9]{0,4}([.][0-9]{1,2})?$", ErrorMessage = "Price must contain two decimal places and cannot exceed 99999.99.")]
        [DataType(DataType.Currency)]
        [Range(1, 99999.99)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public double Price { get; set; }     
        public int UnitTypeID { get; set; }
        public virtual UnitType unitType { get; set; }
        
        public bool isEnabled { get; set; }

        public string getColumns()
        {
            return "MaterialID,MaterialName,Description,Price,UnitTypeID,isEnabled";
        }

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