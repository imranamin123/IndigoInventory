using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptPaymentPlan
    {
        public string Company { get; set; }
        public System.DateTime PaymentPlanDate { get; set; }
        public System.DateTime PaymentStartDate { get; set; }
        public int PaymentPlanID { get; set; }
        public int PaymentPlanTypeID { get; set; }
        public int ApplicationFormID { get; set; }
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
        public long PaymentPlanDetailID { get; set; }
        public System.DateTime PaymentPlanDetailDate { get; set; }
        public int PaymentPlanTypeID1 { get; set; }
        public decimal Percentage { get; set; }
        public string PaymentPlanTypeCode { get; set; }
        public string PaymentPlanTypeDesc { get; set; }
        public decimal Amount { get; set; }
    }
}