using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spINStoreTransferNoteSearchList_Result_Model
    {
        public long StoreTransferNoteID { get; set; }
        public Nullable<System.DateTime> StoreTransferNoteDate { get; set; }
        public string ProjectFrom { get; set; }
        public string ProjectTo { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CompanyID { get; set; }
    }
}