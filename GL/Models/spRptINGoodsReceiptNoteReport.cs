using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINGoodsReceiptNoteReportModel
    {
        public long GoodsReceiptNoteID { get; set; }
        public System.DateTime GoodsReceiptNotesDate { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int APVendorID { get; set; }
        public string APVendorName { get; set; }
        public string DCINVNO { get; set; }
        public System.DateTime DCINVDate { get; set; }
        public long IGPNO { get; set; }
        public string VehicleNo { get; set; }
        public string mRemarks { get; set; }
        public bool IsPosted { get; set; }
        public long GoodsReceiptNoteDetailID { get; set; }
        public long RequestDetailID { get; set; }
        public long PurchaseOrderID { get; set; }
        
        public long ItemID { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public decimal ApprovedQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal Rate { get; set; }
        public decimal VAT { get; set; }
        public decimal Amount { get; set; }
        public string dRemarks { get; set; }
        public string Company { get; set; }
        public string ReceivedBy { get; set; }

        // Print tracking (populated by ReportForm.aspx.cs before SetDataSource)
        public string PrintStatus { get; set; }   // "Original" | "Reprinted 1" | "Copy 2" ...
        public string PrintedBy { get; set; }     // name of the user who generated the printout
    }
}