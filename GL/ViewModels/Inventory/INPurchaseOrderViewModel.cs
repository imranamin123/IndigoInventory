using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Inventory
{
    public class INPurchaseOrderViewModel
    {
        public int RoleID = 0;
        public int SubmitedByKPO = 0;
        public int SubmitedByMD = 0;
        public int IsNew = 0;

        private List<spINPurchaseOrderSearchList_Result> _INPurchaseOrderSearchList { get; set; }
        public List<spINPurchaseOrderSearchList_Result> INPurchaseOrderSearchList
        {
            get
            {
                if (_INPurchaseOrderSearchList == null)
                    _INPurchaseOrderSearchList = new List<spINPurchaseOrderSearchList_Result>();
                return _INPurchaseOrderSearchList;
            }
            set
            {
                _INPurchaseOrderSearchList = value;
            }
        }

        private INPurchaseOrder _INPurchaseOrder { get; set; }
        public INPurchaseOrder INPurchaseOrder
        {
            get
            {
                if (_INPurchaseOrder == null)
                    _INPurchaseOrder = new INPurchaseOrder();
                return _INPurchaseOrder;
            }
            set
            {
                _INPurchaseOrder = value;
            }
        }

        private List<spINPurchaseOrderDetailRows_Result> _INPurchaseOrderDetailRows { get; set; }
        public List<spINPurchaseOrderDetailRows_Result> INPurchaseOrderDetailRows
        {
            get
            {
                if (_INPurchaseOrderDetailRows == null)
                    _INPurchaseOrderDetailRows = new List<spINPurchaseOrderDetailRows_Result>();
                return _INPurchaseOrderDetailRows;
            }
            set
            {
                _INPurchaseOrderDetailRows = value;
            }
        }
    }
}