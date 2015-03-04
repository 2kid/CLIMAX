using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class UnitType
    {
        [Key]
        public int UnitTypeID { get; set; }
        [Required]
        [Display(Name = "Unit Type")]
        public string Type { get; set; }
    }
}