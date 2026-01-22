using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spBSClassificationList_Result_Model
    {
        public int BSClassificationDetailID { get; set; }
        public int BSClassificationID { get; set; }
        public int BSClassificationTypeID { get; set; }
        public int TransactionID { get; set; }
        public decimal Balance { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public int CompanyID { get; set; }
    }
}