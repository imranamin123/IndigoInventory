using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptDVUnitLedger
    {
        public int ApplicationFormID { get; set; }
        public string RefNo { get; set; }
        public string Customer { get; set; }
        public string ProjectName { get; set; }
        public string ApplicationFormNo { get; set; }
        public System.DateTime ApplicationFormDate { get; set; }
        public string UnitNo { get; set; }
        public string UnitTypeName { get; set; }
        public string FloorNo { get; set; }
        public int SQFT { get; set; }
        public decimal Price { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal TokenMoney { get; set; }
        public System.DateTime LedgerDate { get; set; }
        public string Company { get; set; }
        public int ApplicationFormID1 { get; set; }
        public int UnitID { get; set; }
        public System.DateTime Date { get; set; }
        public string PaymentPlanTypeDesc { get; set; }
        public string DVPaymentMethodDesc { get; set; }
        public string Narration { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}