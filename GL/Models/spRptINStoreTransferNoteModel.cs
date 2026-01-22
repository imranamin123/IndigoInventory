using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINStoreTransferNoteModel
    {
        public string Company { get; set; }
        public long StoreTransferNoteID { get; set; }
        public System.DateTime StoreTransferNoteDate { get; set; }
        public string ToProject { get; set; }
        public string FromProject { get; set; }
        public string MRemarks { get; set; }
        public long StoreTransferNoteDetailID { get; set; }
        public long ItemID { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public string Remarks { get; set; }
        public decimal TransferQty { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedAt { get; set; }

        public string ApprovedBy { get; set; }
        public System.DateTime ApprovedByAt { get; set; }
        public string ReceivedBy { get; set; }
        public System.DateTime ReceivedByAt { get; set; }

    }
}