using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.DVO
{
    public class DVOBillingViewModel
    {

        private DVOBilling _DVOBilling { get; set; }
        public DVOBilling DVOBilling
        {
            get
            {
                if (_DVOBilling == null)
                    _DVOBilling = new DVOBilling();
                return _DVOBilling;
            }
            set
            {
                _DVOBilling = value;
            }
        }

        private List<spDVOBillingSearchList_Result> _DVOBillingSearchListRows { get; set; }
        public List<spDVOBillingSearchList_Result> DVOBillingSearchListRows
        {
            get
            {
                if (_DVOBillingSearchListRows == null)
                {
                    _DVOBillingSearchListRows = new List<spDVOBillingSearchList_Result>();
                }
                return _DVOBillingSearchListRows;
            }
            set
            {
                _DVOBillingSearchListRows = value;
            }
        }

        private List<spDVOBillingDetailRows_Result> _DVOBillingDetailRows { get; set; }
        public List<spDVOBillingDetailRows_Result> DVOBillingDetailRows
        {
            get
            {
                if (_DVOBillingDetailRows == null)
                {
                    _DVOBillingDetailRows = new List<spDVOBillingDetailRows_Result>();
                }
                return _DVOBillingDetailRows;
            }
            set
            {
                _DVOBillingDetailRows = value;
            }
        }
    }
}