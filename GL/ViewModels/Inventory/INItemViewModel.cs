using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Inventory
{
    public class INItemViewModel
    {
        private List<spINItemSearchList_Result> _INItemSearchList { get; set; }
        public List<spINItemSearchList_Result> INItemSearchList
        {
            get
            {
                if (_INItemSearchList == null)
                    _INItemSearchList = new List<spINItemSearchList_Result>();
                return _INItemSearchList;
            }
            set
            {
                _INItemSearchList = value;
            }
        }
        private INItem _INItem { get; set; }
        public INItem INItem
        {
            get
            {
                if (_INItem == null)
                    _INItem = new INItem();
                return _INItem;
            }
            set
            {
                _INItem = value;
            }
        }

    }
}