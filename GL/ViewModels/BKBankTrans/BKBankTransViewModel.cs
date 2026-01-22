using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.BKBankTrans
{
    public class BKBankTransViewModel
    {
        private BKBankTran _BKBankTran { get; set; }
        public BKBankTran BKBankTran
        {
            get
            {
                if (_BKBankTran == null)
                    _BKBankTran = new BKBankTran();
                return _BKBankTran;
            }
            set
            {
                _BKBankTran = value;
            }
        }

        private List<spBKBankTransDetailRows_Result> _BKBankTransDetailRows { get; set; }
        public List<spBKBankTransDetailRows_Result> GLBKBankTransDetailRows
        {
            get
            {
                if (_BKBankTransDetailRows == null)
                    _BKBankTransDetailRows = new List<spBKBankTransDetailRows_Result>();
                return _BKBankTransDetailRows;
            }
            set
            {
                _BKBankTransDetailRows = value;
            }
        }


        private List<spBKBankTransSearchList_Result> _BKBankTransSearchList { get; set; }
        public List<spBKBankTransSearchList_Result> BKBankTransSearchList
        {
            get
            {
                if (_BKBankTransSearchList == null)
                    _BKBankTransSearchList = new List<spBKBankTransSearchList_Result>();
                return _BKBankTransSearchList;
            }
            set
            {
                _BKBankTransSearchList = value;
            }
        }


    }
}