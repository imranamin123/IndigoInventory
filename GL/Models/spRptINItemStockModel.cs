using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINItemStockModel
    {
        public long ProjectID { get; set; }
        public long ItemID { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public string CategoryName { get; set; }
        public string SizeName { get; set; }
        public string UOM { get; set; }
        public string ProjectName { get; set; }
        public string CompanyName { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }

        public decimal Rate { get; set; }

        public decimal OpeningQty { get; set; }
        public decimal TransferQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal IssuedQty { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal ClosingQty { get; set; }
        public decimal ClosingAmount { get; set; }
    }
}