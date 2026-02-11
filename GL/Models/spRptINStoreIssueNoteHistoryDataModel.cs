using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINStoreIssueNoteHistoryDataModel
    {
        public string Company { get; set; }
        public string ProjectName { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public long ItemID { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
        public string UOM { get; set; }
        public decimal LastRate { get; set; }
        public decimal IssuedQty { get; set; }
        public decimal TotalAmount { get; set; }
    }
}