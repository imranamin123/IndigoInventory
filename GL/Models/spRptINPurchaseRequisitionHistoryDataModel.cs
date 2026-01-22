using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINPurchaseRequisitionHistoryDataModel
    {
        public string Company { get; set; }
        public string ProjectName { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public System.DateTime RequestDate { get; set; }
        public long RequestID { get; set; }
        public long RequestDetailID { get; set; }
        public long ItemID { get; set; }
        public string ItemDescription { get; set; }
        public string Size { get; set; }
        public string UOM { get; set; }
        public string RequestTypeDesc { get; set; }
        public decimal RequestedQty { get; set; }
        public decimal ApprovedQty { get; set; }
        public decimal ReceivedQty { get; set; }
    }
}