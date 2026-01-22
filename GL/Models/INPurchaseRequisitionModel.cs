using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class INPurchaseRequisitionModel
    {
        public long RequestID { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<long> DocumentNo { get; set; }
        public Nullable<long> ManualDemandNo { get; set; }
        public string ProjectDocumentNo { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> SubmitedByKPO { get; set; }
        public Nullable<System.DateTime> SubmitedAtKPO { get; set; }
        public Nullable<int> SubmitedByMD { get; set; }
        public Nullable<System.DateTime> SubmitedAtMD { get; set; }
        public Nullable<int> RequestTypeID { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> CompanyID { get; set; }
    }
}