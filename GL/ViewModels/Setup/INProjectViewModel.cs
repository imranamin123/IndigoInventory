using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Setup
{
    public class INProjectViewModel
    {
        public List<INProject> _INProjectList { get; set; }
        public List<INProject> INProjectList
        {
            get
            {
                if (_INProjectList == null)
                    _INProjectList = new List<INProject>();
                return _INProjectList;
            }
            set
            {
                _INProjectList = value;
            }
        }

        private INProject _INProject { get; set; }
        public INProject INProject
        {
            get
            {
                if (_INProject == null)
                    _INProject = new INProject();
                return _INProject;
            }
            set
            {
                _INProject = value;
            }
        }

    }
}