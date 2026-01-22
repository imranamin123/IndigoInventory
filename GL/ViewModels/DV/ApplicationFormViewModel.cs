using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.DV
{
    public class ApplicationFormViewModel
    {
        public int RoleID = 0;
        private spDVGetApplicationForm_Result _spDVGetApplicationForm { get; set; }

        public spDVGetApplicationForm_Result spDVGetApplicationForm
        {
            get
            {
                if (_spDVGetApplicationForm == null)
                    _spDVGetApplicationForm = new spDVGetApplicationForm_Result();
                return _spDVGetApplicationForm;
            }
            set
            {
                _spDVGetApplicationForm = value;
            }
        }

        private List<spDVApplicantRows_Result> _DVApplicantRows{ get; set; }

        public List<spDVApplicantRows_Result> DVApplicantRows
        {
            get 
            { 
                if(_DVApplicantRows == null)
                    _DVApplicantRows = new List<spDVApplicantRows_Result>();
                return _DVApplicantRows;
            }
            set 
            { 
                _DVApplicantRows = value;
            }
        }

        private List<spDVApplicationFormSearchList_Result> _DVApplicationFormSearchList { get; set; }
        public List<spDVApplicationFormSearchList_Result> DVApplicationFormSearchList
        {
            get
            {
                if (_DVApplicationFormSearchList == null)
                    _DVApplicationFormSearchList = new List<spDVApplicationFormSearchList_Result>();
                return _DVApplicationFormSearchList;
            }
            set
            {
                _DVApplicationFormSearchList = value;
            }
        }


    }
}