using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Setup
{
    public class GLVoucherViewModel
    {
        private GLVoucher _GLVoucher { get; set; }
        public GLVoucher GLVoucher
        {
            get
            {
                if (_GLVoucher == null)
                    _GLVoucher = new GLVoucher();
                return _GLVoucher;
            }
            set
            {
                _GLVoucher = value;
            }
        }

        private List<spGLVoucherDetailRows_Result> _GLVoucherDetailRows { get; set; }
        public List<spGLVoucherDetailRows_Result> GLVoucherDetailRows
        {
            get
            {
                if (_GLVoucherDetailRows == null)
                    _GLVoucherDetailRows = new List<spGLVoucherDetailRows_Result>();
                return _GLVoucherDetailRows;
            }
            set
            {
                _GLVoucherDetailRows = value;
            }
        }


        public List<spGLVoucherSearchList_Result> _GLVoucherList { get; set; }
        public List<spGLVoucherSearchList_Result> GLVoucherList
        {
            get
            {
                if (_GLVoucherList == null)
                    _GLVoucherList = new List<spGLVoucherSearchList_Result>();
                return _GLVoucherList;
            }
            set
            {
                _GLVoucherList = value;
            }
        }


    }
}