using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.DV
{
    public class DVReceiptViewModel
    {
        public int RoleID = 0;
        public string username = string.Empty;
        public int rowCount { get; set; }
        private spDVReceiptHead_Result _DVReceiptHead { get; set; }
        public spDVReceiptHead_Result DVReceiptHead
        {
            get
            {
                if (_DVReceiptHead == null)
                    _DVReceiptHead = new spDVReceiptHead_Result();
                return _DVReceiptHead;
            }
            set
            {
                _DVReceiptHead = value;
            }
        }

        private List<spDVReceiptDetail_Result> _DVReceiptDetailRows { get; set; }
        public List<spDVReceiptDetail_Result> DVReceiptDetailRows
        {
            get
            {
                if (_DVReceiptDetailRows == null)
                {
                    _DVReceiptDetailRows = new List<spDVReceiptDetail_Result>();
                   // _DVReceiptDetailRows.Add(new spDVReceiptDetail_Result());
                }
                return _DVReceiptDetailRows;
            }
            set
            {
                _DVReceiptDetailRows = value;
            }
        }

        private List<spDVReceiptSearchList_Result> _DVReceiptList { get; set; }
        public List<spDVReceiptSearchList_Result> DVReceiptList
        {
            get
            {
                if (_DVReceiptList == null)
                    _DVReceiptList = new List<spDVReceiptSearchList_Result>();
                
                return _DVReceiptList;
            }
            set
            {
                _DVReceiptList = value;
            }
        }

        private List<spDVReceiptSearchListSGO_Result> _DVReceiptListSGO { get; set; }
        public List<spDVReceiptSearchListSGO_Result> DVReceiptListSGO
        {
            get
            {
                if (_DVReceiptListSGO == null)
                    _DVReceiptListSGO = new List<spDVReceiptSearchListSGO_Result>();

                return _DVReceiptListSGO;
            }
            set
            {
                _DVReceiptListSGO = value;
            }
        }


        //Edit

        private spDVReceiptHeadByReceiptID_Result _DVReceiptHeadByReceiptID { get; set; }
        public spDVReceiptHeadByReceiptID_Result DVReceiptHeadByReceiptID
        {
            get
            {
                if (_DVReceiptHeadByReceiptID == null)
                    _DVReceiptHeadByReceiptID = new spDVReceiptHeadByReceiptID_Result();
                return _DVReceiptHeadByReceiptID;
            }
            set
            {
                _DVReceiptHeadByReceiptID = value;
            }
        }

        private List<spDVReceiptDetailByReceiptID_Result> _DVReceiptDetailByReceiptIDRows { get; set; }
        public List<spDVReceiptDetailByReceiptID_Result> DVReceiptDetailByReceiptIDRows
        {
            get
            {
                if (_DVReceiptDetailByReceiptIDRows == null)
                {
                    _DVReceiptDetailByReceiptIDRows = new List<spDVReceiptDetailByReceiptID_Result>();
                    // _DVReceiptDetailRows.Add(new spDVReceiptDetail_Result());
                }
                return _DVReceiptDetailByReceiptIDRows;
            }
            set
            {
                _DVReceiptDetailByReceiptIDRows = value;
            }
        }


    }
}