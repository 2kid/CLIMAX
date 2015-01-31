using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIMAX.Models
{
    public class Inventory
    {
        public int InventoryID { get; set; }
        public int MaterialID { get; set; }
        public virtual Materials material { get; set; }
        public int QtyInStock { get; set; }
        public DateTime LastDateUpdated { get; set; }
        public int BranchID { get; set; }
        public Branch branch { get; set; }
    }
}