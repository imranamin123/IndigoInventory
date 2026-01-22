using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Common
{
    public class Enumeration
    {
       public enum PaymentPlanTypeEnum
        {
            TokenMoney=1,
            DownPayment=2,
            Installment=3,
            Possession=4,
            Adjustment=5,
            CancellationCharges=6,
            ExtraArea=7,
            ExtraWork=8,
            MeterCharges=9
        }

    }
}