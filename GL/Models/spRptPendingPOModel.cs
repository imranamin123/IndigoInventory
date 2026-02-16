using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptPendingPOModel
    {
        public string Company { get; set; }
        public long GoodsReceiptNoteID { get; set; }
        public System.DateTime GoodsReceiptNotesDate { get; set; }
        public string ProjectName { get; set; }
        public string APVendorName { get; set; }
        public string Status { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public long ItemID { get; set; }
        public string ItemName { get; set; }
        public string Size { get; set; }
        public string UOM { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}