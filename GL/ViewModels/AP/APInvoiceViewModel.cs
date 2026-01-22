using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.AP
{
    public class APInvoiceViewModel
    {
        private APInvoice _APInvoice { get; set; }
        public APInvoice APInvoice
        {
            get
            {
                if (_APInvoice == null)
                    _APInvoice = new APInvoice();
                return _APInvoice;
            }
            set
            {
                _APInvoice = value;
            }
        }

        private List<spAPInvoiceDetailRows_Result> _APInvoiceDetailRows { get; set; }
        public List<spAPInvoiceDetailRows_Result> APInvoiceDetailRows
        {
            get
            {
                if (_APInvoiceDetailRows == null)
                    _APInvoiceDetailRows = new List<spAPInvoiceDetailRows_Result>();
                return _APInvoiceDetailRows;
            }
            set
            {
                _APInvoiceDetailRows = value;
            }
        }


        public List<spAPInvoiceSearchList_Result> _APInvoiceSearchList { get; set; }
        public List<spAPInvoiceSearchList_Result> APInvoiceSearchList
        {
            get
            {
                if (_APInvoiceSearchList == null)
                    _APInvoiceSearchList = new List<spAPInvoiceSearchList_Result>();
                return _APInvoiceSearchList;
            }
            set
            {
                _APInvoiceSearchList = value;
            }
        }


    }
}