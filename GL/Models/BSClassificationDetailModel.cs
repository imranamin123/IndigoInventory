using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class BSClassificationDetailModel
    {
        public int BSClassificationDetailID { get; set; }
        public Nullable<int> BSClassificationID { get; set; }
        public Nullable<int> BSClassificationTypeID { get; set; }
        public Nullable<int> TransactionID { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifedAt { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> CompanyID { get; set; }
    }
}