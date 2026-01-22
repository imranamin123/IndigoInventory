using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINPurchaseRequisitionModel
    {
        public string Company { get; set; }
        public long RequestID { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string ProjectName { get; set; }
        public string DocumentNo { get; set; }
        public long ManualDemandNo { get; set; }
        public string mRemarks { get; set; }
        public long RequestDetailID { get; set; }
        public long ItemID { get; set; }
        public long ItemCode { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public decimal RequestedQty { get; set; }
        public decimal ApprovedQty { get; set; }
        public decimal QtyInHand { get; set; }
        public decimal LastRate { get; set; }
        public string dRemarks { get; set; }

        public string SubmittedByKPO { get; set; }
        public System.DateTime SubmitedAtKPO { get; set; }
        public string SubmittedByMD { get; set; }
        public System.DateTime SubmitedAtMD { get; set; }

    }
}