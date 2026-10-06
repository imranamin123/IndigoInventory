using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINPurchaseOrderReportModel
    {
        public long PurchaseOrderID { get; set; }
        public System.DateTime PurchaseOrderDate { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public string PaymentTerms { get; set; }
        public int APVendorID { get; set; }
        public System.DateTime ApprovedAt { get; set; }
        public string mRemarks { get; set; }
        public string APVendorName { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string VendorAddress { get; set; }
        public string BankDetails { get; set; }
        public string Email { get; set; }
        public long ItemID { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public decimal ApprovedQty { get; set; }
        public decimal FreightCharges { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal Amount { get; set; }
        public string Company { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string ApprovedBy { get; set; }
        public int CancelledBy { get; set; }
        public string GoodsReceiptNoteID { get; set; }
        public System.DateTime CancelledAt { get; set; }
        public string Cancelled { get; set; }

        // Print tracking (populated by ReportForm.aspx.cs before SetDataSource)
        public string PrintStatus { get; set; }   // "Original" | "Reprinted 1" | "Copy 2" ...
        public string PrintedBy { get; set; }     // name of the user who generated the printout
    }
}
