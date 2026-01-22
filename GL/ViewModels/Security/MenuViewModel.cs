using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GL.Security.ViewModels
{
    public class MenuViewModel
    {
        public int CompanyID { get; set; }
        public int? UserID { get; set; }

        private List<spMenuPagesRightsList_Result> _MenuPagesRightsList;
        public List<spMenuPagesRightsList_Result> MenuPagesRightsList
        {
            get
            {
                if (_MenuPagesRightsList == null)
                {
                    _MenuPagesRightsList = new List<spMenuPagesRightsList_Result>();
                }
                return _MenuPagesRightsList;
            }
            set
            {
                _MenuPagesRightsList = value;

            }
        }

        private List<spUserPagesRights_Result> _UserPagesRights;
        public List<spUserPagesRights_Result> UserPagesRights
        {
            get
            {
                if (_UserPagesRights == null)
                {
                    _UserPagesRights = new List<spUserPagesRights_Result>();
                }
                return _UserPagesRights;
            }
            set
            {
                _UserPagesRights = value;

            }
        }

    }
}