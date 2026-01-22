using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace GL.ViewModels.Setup
{
    public class APCompanyViewModel
    {
        public List<Company> _CompanyList { get; set; }
        public List<Company> CompanyList
        {
            get
            {
                if (_CompanyList == null)
                    _CompanyList = new List<Company>();
                return _CompanyList;
            }
            set
            {
                _CompanyList = value;
            }
        }
        
        private Company _Company { get; set; }
        public Company Company
        {
            get
            {
                if (_Company == null)
                    _Company = new Company();
                return _Company;
            }
            set
            {
                _Company = value;
            }
        }
        
    }
}