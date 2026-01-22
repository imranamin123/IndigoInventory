using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINStoreIssueNoteReportModel
    {
        public string Company { get; set; }
        public long StoreIssueNoteID { get; set; }
        public System.DateTime StoreIssueNoteDate { get; set; }
        public string ProjectName { get; set; }
        public string StoreName { get; set; }
        public string mRemarks { get; set; }
        public long StoreIssueNoteDetailID { get; set; }
        public long ItemID { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public decimal IssuedQty { get; set; }
        public string IssuedBy { get; set; }
    }
}