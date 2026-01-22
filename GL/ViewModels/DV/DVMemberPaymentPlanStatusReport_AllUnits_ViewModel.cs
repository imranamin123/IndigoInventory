using GL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.DV
{
    public class DVMemberPaymentPlanStatusReport_AllUnits_ViewModel
    {
        private List<MemberPaymentStatus_AllUnits_List_Model> _MemberPaymentStatus_AllUnits_Rows { get; set; }

        public List<MemberPaymentStatus_AllUnits_List_Model> MemberPaymentStatus_AllUnits_Rows
        {
            get
            {
                if (_MemberPaymentStatus_AllUnits_Rows == null)
                    _MemberPaymentStatus_AllUnits_Rows = new List<MemberPaymentStatus_AllUnits_List_Model>();
                return _MemberPaymentStatus_AllUnits_Rows;
            }
            set
            {
                _MemberPaymentStatus_AllUnits_Rows = value;
            }
        }
    }
}