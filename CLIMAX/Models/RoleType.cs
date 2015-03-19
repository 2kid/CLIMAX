using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class RoleType
    {
        public int RoleTypeId { get; set; }
        [Required]
        [Display(Name="Role Type")]
        public string Type { get; set; }

        public string getCoulmns()
        {
            return "RoleTypeId,Type";
        }
    }
}