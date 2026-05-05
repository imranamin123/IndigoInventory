using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINItemRateComparisonListModel
    {
        public string Company { get; set; }
        public string ProjectName { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public long ItemID { get; set; }
        public string ItemName { get; set; }
        public string Size { get; set; }
        public string UOM { get; set; }
        public decimal LastRate { get; set; }
        public System.DateTime LastRateDate { get; set; }
        public decimal LastRate2 { get; set; }
        public System.DateTime LastRateDate2 { get; set; }
        public decimal LastRate3 { get; set; }
        public System.DateTime LastRateDate3 { get; set; }
        public decimal LastRate4 { get; set; }
        public System.DateTime LastRateDate4 { get; set; }

    }
}