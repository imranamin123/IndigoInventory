using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Inventory
{
    public class INStoreIssueNoteViewModel
    {
        public int RoleID = 0;
        public int SubmitedByKPO = 0;
        public int SubmitedByMD = 0;
        public int IsNew = 0;

        private List<spINStoreIssueNoteSearchList_Result> _INStoreIssueNoteSearchList { get; set; }
        public List<spINStoreIssueNoteSearchList_Result> INStoreIssueNoteSearchList
        {
            get
            {
                if (_INStoreIssueNoteSearchList == null)
                    _INStoreIssueNoteSearchList = new List<spINStoreIssueNoteSearchList_Result>();
                return _INStoreIssueNoteSearchList;
            }
            set
            {
                _INStoreIssueNoteSearchList = value;
            }
        }

        private INStoreIssueNote _INStoreIssueNote { get; set; }
        public INStoreIssueNote INStoreIssueNote
        {
            get
            {
                if (_INStoreIssueNote == null)
                    _INStoreIssueNote = new INStoreIssueNote();
                return _INStoreIssueNote;
            }
            set
            {
                _INStoreIssueNote = value;
            }
        }

        private List<spINStoreIssueNoteDetailRows_Result> _INStoreIssueNoteDetailRows { get; set; }
        public List<spINStoreIssueNoteDetailRows_Result> INStoreIssueNoteDetailRows
        {
            get
            {
                if (_INStoreIssueNoteDetailRows == null)
                    _INStoreIssueNoteDetailRows = new List<spINStoreIssueNoteDetailRows_Result>();
                return _INStoreIssueNoteDetailRows;
            }
            set
            {
                _INStoreIssueNoteDetailRows = value;
            }
        }

    }
}