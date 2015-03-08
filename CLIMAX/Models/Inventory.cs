using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Inventory
    {
        public int InventoryID { get; set; }
        public int MaterialID { get; set; }
        public virtual Materials material { get; set; }
        [Display(Name = "Quantity in Stock")]
        public int QtyInStock { get; set; }
        [Display(Name = "Last Date Updated")]
        public DateTime LastDateUpdated { get; set; }
        public int BranchID { get; set; }
        public Branch branch { get; set; }
    }
}