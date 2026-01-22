using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptDVReceiptReport
    {
        public string Company { get; set; }
        public int ApplicationFormID { get; set; }
        public int ReceiptID { get; set; }
        public string ProjectName { get; set; }
        public string Applicant { get; set; }        
        public string ApplicationFormNo { get; set; }
        public DateTime ApplicationFormDate { get; set; }
        public string UnitNo { get; set; }
        public string UnitTypeName { get; set; }
        public string FloorNo { get; set; }
        public int SQFT { get; set; }
        public decimal Price { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal TokenMoney { get; set; }
        public System.DateTime DVReceiptDate { get; set; }
        public bool Varified { get; set; }
        public int VarifiedBy { get; set; }
        public string VarifiedByName { get; set; }
        public bool Locked { get; set; }
        public int LockedBy { get; set; }
        public string LockedByName { get; set; }
        public long DVReceiptDetailID { get; set; }
        public int DVReceiptID { get; set; }
        public string PaymentPlanTypeCode { get; set; }
        public string PaymentPlanTypeDesc { get; set; }
        public string DVPaymentMethodDesc { get; set; }
        public int DVPaymentMethodID { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public int PaymentPlanTypeID { get; set; }
    }
}