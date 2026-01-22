using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class BSClassificationModel
    {
        public int BSClassificationID { get; set; }
        public int BKBankID { get; set; }
        public int TransactionID { get; set; }
        public System.DateTime ValueDate { get; set; }
        public System.DateTime BSClassificationDate { get; set; }
        public decimal BSBankStatementBalance { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public int CompanyID { get; set; }

    }
}