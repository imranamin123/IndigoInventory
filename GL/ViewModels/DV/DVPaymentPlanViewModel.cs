using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.DV
{
    public class DVPaymentPlanViewModel
    {
        
        public int rowCount { get; set; }
        private spDVPaymentPlanHead_Result _DVPaymentPlanHead { get; set; }
        public spDVPaymentPlanHead_Result DVPaymentPlanHead
        {
            get
            {
                if (_DVPaymentPlanHead == null)
                    _DVPaymentPlanHead = new spDVPaymentPlanHead_Result();
                return _DVPaymentPlanHead;
            }
            set
            {
                _DVPaymentPlanHead = value;
            }
        }

        private List<spDVPaymentPlanDetail_Result> _DVPaymentPlanDetailRows { get; set; }
        public List<spDVPaymentPlanDetail_Result> DVPaymentPlanDetailRows
        {
            get
            {
                if (_DVPaymentPlanDetailRows == null)
                {
                    _DVPaymentPlanDetailRows = new List<spDVPaymentPlanDetail_Result>();
                    _DVPaymentPlanDetailRows.Add(new spDVPaymentPlanDetail_Result());
                }
                return _DVPaymentPlanDetailRows;
            }
            set
            {
                _DVPaymentPlanDetailRows = value;
            }
        }


    }
}