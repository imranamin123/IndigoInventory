using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINPORevertHistoryDataModel
    {
        public long PORevertHistoryID { get; set; }
        public long PurchaseOrderID { get; set; }
        public int ProjectID { get; set; }
        public string Status { get; set; }
        public System.DateTime PurchaseOrderDate { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public int SrNo { get; set; }
        public long ItemID { get; set; }
        public string ItemDescription { get; set; }
        public string Size { get; set; }
        public string UOM { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public System.DateTime RevertedDate { get; set; }
        public decimal RevertedQty { get; set; }
        public decimal RevertedRate { get; set; }
        public decimal RevertedDiscount { get; set; }
        public decimal RevertedTotalAmount { get; set; }
        public string ProjectName { get; set; }
    }
}