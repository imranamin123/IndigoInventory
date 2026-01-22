using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Inventory
{
    public class INPurchaseRequisitionViewModel
    {
        public int RoleID = 0;
        public int SubmitedByKPO = 0;
        public int SubmitedByMD = 0;
        public int IsNew = 0;

        private List<spINPurchaseRequisitionSearchList_Result> _INPurchaseRequisitionSearchList { get; set; }
        public List<spINPurchaseRequisitionSearchList_Result> INPurchaseRequisitionSearchList
        {
            get
            {
                if (_INPurchaseRequisitionSearchList == null)
                    _INPurchaseRequisitionSearchList = new List<spINPurchaseRequisitionSearchList_Result>();
                return _INPurchaseRequisitionSearchList;
            }
            set
            {
                _INPurchaseRequisitionSearchList = value;
            }
        }
        
        private INPurchaseRequisition _INPurchaseRequisition { get; set; }
        public INPurchaseRequisition INPurchaseRequisition
        {
            get
            {
                if (_INPurchaseRequisition == null)
                    _INPurchaseRequisition = new INPurchaseRequisition();
                return _INPurchaseRequisition;
            }
            set
            {
                _INPurchaseRequisition = value;
            }
        }

        private List<spINPurchaseRequisitionDetailRows_Result> _INPurchaseRequisitionDetailRows { get; set; }
        public List<spINPurchaseRequisitionDetailRows_Result> INPurchaseRequisitionDetailRows
        {
            get
            {
                if (_INPurchaseRequisitionDetailRows == null)
                    _INPurchaseRequisitionDetailRows = new List<spINPurchaseRequisitionDetailRows_Result>();
                return _INPurchaseRequisitionDetailRows;
            }
            set
            {
                _INPurchaseRequisitionDetailRows = value;
            }
        }

    }
}