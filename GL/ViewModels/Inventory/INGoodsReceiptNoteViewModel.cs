using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Inventory
{
    public class INGoodsReceiptNoteViewModel
    {
        public int RoleID = 0;
        public int SubmitedByKPO = 0;
        public int SubmitedByMD = 0;
        public int IsNew = 0;
        public string username=string.Empty;

        private List<spINGoodsReceiptNoteSearchList_Result> _ININGoodsReceiptNoteSearchList { get; set; }
        public List<spINGoodsReceiptNoteSearchList_Result> INGoodsReceiptNoteSearchList
        {
            get
            {
                if (_ININGoodsReceiptNoteSearchList == null)
                    _ININGoodsReceiptNoteSearchList = new List<spINGoodsReceiptNoteSearchList_Result>();
                return _ININGoodsReceiptNoteSearchList;
            }
            set
            {
                _ININGoodsReceiptNoteSearchList = value;
            }
        }

        private INGoodsReceiptNote _INGoodsReceiptNote { get; set; }
        public INGoodsReceiptNote INGoodsReceiptNote
        {
            get
            {
                if (_INGoodsReceiptNote == null)
                    _INGoodsReceiptNote = new INGoodsReceiptNote();
                return _INGoodsReceiptNote;
            }
            set
            {
                _INGoodsReceiptNote = value;
            }
        }

        private List<spINGoodsReceiptNoteDetailRows_Result> _INGoodsReceiptNoteDetailRows { get; set; }
        public List<spINGoodsReceiptNoteDetailRows_Result> INGoodsReceiptNoteDetailRows
        {
            get
            {
                if (_INGoodsReceiptNoteDetailRows == null)
                    _INGoodsReceiptNoteDetailRows = new List<spINGoodsReceiptNoteDetailRows_Result>();
                return _INGoodsReceiptNoteDetailRows;
            }
            set
            {
                _INGoodsReceiptNoteDetailRows = value;
            }   
        }

    }
}
