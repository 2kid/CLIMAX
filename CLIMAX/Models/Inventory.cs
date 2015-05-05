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
        [RegularExpression("^[0-9]{0,4}$", ErrorMessage = "Please input positive numbers not greater than 4 digits")]
        public int QtyInStock { get; set; }
        [Display(Name = "Quantity to alert")]
        [RegularExpression("^[1-9][0-9]{0,3}$", ErrorMessage = "Please input positive numbers not greater than 4 digits")]
        public int? QtyToAlert { get; set; }
        [Display(Name = "Last Date Updated")]
        public DateTime LastDateUpdated { get; set; }
        public int BranchID { get; set; }
        public Branch branch { get; set; }

        public bool isEnabled { get; set; }

        public bool isLowInStock {
            get
            {
                if (QtyToAlert != null && QtyInStock <= QtyToAlert)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public string getColumns()
        {
            return "InventoryID,MaterialID,QtyInStock,QtyToAlert,LastDateUpdated,BranchID,isEnabled";
        }

    }


    public class InventoryReportsViewModel
    {
        public int MaterialID { get; set; }
        public string Medicine { get; set; }
        public int? Control { get; set; }
        public int Add { get; set; }
        public int Remove { get; set; }
        public int Sold { get; set; }
        public int Balance { get; set; }
    }
    public class InventoryMessageViewModel
    {
        public string Inventory { get; set; }
        public string QtyLeft { get; set; }
    }


}