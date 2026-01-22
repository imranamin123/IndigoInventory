using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Setup
{
    public class FiscalYearSetupViewModel
    {
        private FiscalYearSetup _FiscalYearSetup { get; set; }
        public FiscalYearSetup FiscalYearSetup
        {
            get
            {
                if (_FiscalYearSetup == null)
                    _FiscalYearSetup = new FiscalYearSetup();
                return _FiscalYearSetup;
            }
            set
            {
                _FiscalYearSetup = value;
            }
        }
    }
}