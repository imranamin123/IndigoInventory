using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptMemberPaymentPlanStatus_PaymentPlan
    {
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
        public string Company { get; set; }
        public int UnitID { get; set; }
        public string PaymentPlanType { get; set; }
        public int PaymentPlanTypeID { get; set; }
        public System.DateTime PaymentPlanDetailDate { get; set; }
        public decimal DueAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal Balance { get; set; }
    }
}