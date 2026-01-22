using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Setup
{
    public class SecUserViewModel
    {
        private List<spSecUserGetSearchList_Result> _SecUserSearchList { get; set; }
        public List<spSecUserGetSearchList_Result> SecUserSearchList
        {
            get
            {
                if (_SecUserSearchList == null)
                    _SecUserSearchList = new List<spSecUserGetSearchList_Result>();
                return _SecUserSearchList;
            }
            set
            {
                _SecUserSearchList = value;
            }
        }
        private SecUser _SecUser { get; set; }
        public SecUser SecUser
        {
            get
            {
                if (_SecUser == null)
                    _SecUser = new SecUser();
                return _SecUser;
            }
            set
            {
                _SecUser = value;
            }
        }

    }
}