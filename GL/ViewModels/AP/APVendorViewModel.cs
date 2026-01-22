using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace GL.ViewModels.AP
{
    public class APVendorViewModel
    {
        private List<APVendor> _APVendorList { get; set; }
        public List<APVendor> APVendorList
        {
            get
            {
                if (_APVendorList == null)
                    _APVendorList = new List<APVendor>();
                return _APVendorList;
            }
            set
            {
                _APVendorList = value;
            }
        }
        
        private APVendor _APVendor { get; set; }
        public APVendor APVendor
        {
            get
            {
                if (_APVendor == null)
                    _APVendor = new APVendor();
                return _APVendor;
            }
            set
            {
                _APVendor = value;
            }
        }
        
    }
}