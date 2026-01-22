using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptGeneralLedgerOBModel
    {
        public int CompanyID { get; set; }
        public string Company { get; set; }
        public string GLAccountNo { get; set; }
        public string Description { get; set; }
        public System.DateTime VoucherDate { get; set; }
        public string VTYPE { get; set; }
        public string VoucherNumber { get; set; }
        public string GLNarration { get; set; }
        public string type { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public decimal OB { get; set; }
    }
}