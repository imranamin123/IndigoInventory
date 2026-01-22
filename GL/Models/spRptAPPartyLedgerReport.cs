using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptAPPartyLedgerReport
    {
        public string InvNo { get; set; }
        public System.DateTime TransDate { get; set; }
        public string APVendorName { get; set; }
        public string Company { get; set; }
        public string Narration { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public decimal Balance { get; set; }
    }
}