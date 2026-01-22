using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class MemberPaymentStatus_AllUnits_List_Model
    {
        public string Unit { get; set; }
        public string PaymentPlanType { get; set; }
        public string Month { get; set; }
        public decimal ReceivedAmount { get; set; }
    }
}