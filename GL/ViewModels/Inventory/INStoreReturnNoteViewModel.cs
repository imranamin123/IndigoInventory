using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Inventory
{
    public class INStoreReturnNoteViewModel
    {
        public int RoleID = 0;
        public int SubmitedByKPO = 0;
        public int SubmitedByMD = 0;
        public int IsNew = 0;

        private List<spINStoreReturnNoteSearchList_Result> _INStoreReturnNoteSearchList { get; set; }
        public List<spINStoreReturnNoteSearchList_Result> INStoreReturnNoteSearchList
        {
            get
            {
                if (_INStoreReturnNoteSearchList == null)
                    _INStoreReturnNoteSearchList = new List<spINStoreReturnNoteSearchList_Result>();
                return _INStoreReturnNoteSearchList;
            }
            set
            {
                _INStoreReturnNoteSearchList = value;
            }
        }

        private INStoreReturnNote _INStoreReturnNote { get; set; }
        public INStoreReturnNote INStoreReturnNote
        {
            get
            {
                if (_INStoreReturnNote == null)
                    _INStoreReturnNote = new INStoreReturnNote();
                return _INStoreReturnNote;
            }
            set
            {
                _INStoreReturnNote = value;
            }
        }

        private List<spINStoreReturnNoteDetailRows_Result> _INStoreReturnNoteDetailRows { get; set; }
        public List<spINStoreReturnNoteDetailRows_Result> INStoreReturnNoteDetailRows
        {
            get
            {
                if (_INStoreReturnNoteDetailRows == null)
                    _INStoreReturnNoteDetailRows = new List<spINStoreReturnNoteDetailRows_Result>();
                return _INStoreReturnNoteDetailRows;
            }
            set
            {
                _INStoreReturnNoteDetailRows = value;
            }
        }
    }
}