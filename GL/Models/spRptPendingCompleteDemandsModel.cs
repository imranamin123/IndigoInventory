using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptPendingCompleteDemandsModel
    {
        public string Company { get; set; }
        public string ProjectName { get; set; }
        public long RequestID { get; set; }
        public int ProjectID { get; set; }
        public System.DateTime RequestDate { get; set; }
        public decimal RequestedQty { get; set; }
        public decimal Balance { get; set; }
    }
}