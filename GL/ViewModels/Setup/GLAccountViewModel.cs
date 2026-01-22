using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Setup
{
    public class GLAccountViewModel
    {
        public List<GLAccount> _GLAccountList { get; set; }
        public List<GLAccount> GLAccountList
        {
            get
            {
                if (_GLAccountList == null)
                    _GLAccountList = new List<GLAccount>();
                return _GLAccountList;
            }
            set
            {
                _GLAccountList = value;
            }
        }

        private GLAccount _GLAccount { get; set; }
        public GLAccount GLAccount
        {
            get
            {
                if (_GLAccount == null)
                    _GLAccount = new GLAccount();
                return _GLAccount;
            }
            set
            {
                _GLAccount = value;
            }
        }
    }
}