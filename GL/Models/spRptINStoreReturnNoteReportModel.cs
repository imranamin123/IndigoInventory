using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINStoreReturnNoteReportModel
    {
        public string Company { get; set; }
        public long StoreReturnNoteID { get; set; }
        public System.DateTime StoreReturnNoteDate { get; set; }
        public string ProjectName { get; set; }
        public string StoreName { get; set; }
        public string mRemarks { get; set; }
        public long StoreReturnNoteDetailID { get; set; }
        public long ItemID { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public decimal ReturnQty { get; set; }
        public string ReturnBy { get; set; }
        public System.DateTime ReturnAt { get; set; }
    }
}