using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.Inventory
{
    public class INStoreTransferNoteViewModel
    {
        public int RoleID = 0;
        public int SubmitedByKPO = 0;
        public int SubmitedByMD = 0;
        public int IsNew = 0;

        private List<spINStoreTransferNoteSearchList_Result> _INStoreTransferNoteSearchList { get; set; }
        public List<spINStoreTransferNoteSearchList_Result> INStoreTransferNoteSearchList
        {
            get
            {
                if (_INStoreTransferNoteSearchList == null)
                    _INStoreTransferNoteSearchList = new List<spINStoreTransferNoteSearchList_Result>();
                return _INStoreTransferNoteSearchList;
            }
            set
            {
                _INStoreTransferNoteSearchList = value;
            }
        }

        private INStoreTransferNote _INStoreTransferNote { get; set; }
        public INStoreTransferNote INStoreTransferNote
        {
            get
            {
                if (_INStoreTransferNote == null)
                    _INStoreTransferNote = new INStoreTransferNote();
                return _INStoreTransferNote;
            }
            set
            {
                _INStoreTransferNote = value;
            }
        }

        private List<spINStoreTransferNoteDetailRows_Result> _INStoreTransferNoteDetailRows { get; set; }
        public List<spINStoreTransferNoteDetailRows_Result> INStoreTransferNoteDetailRows
        {
            get
            {
                if (_INStoreTransferNoteDetailRows == null)
                    _INStoreTransferNoteDetailRows = new List<spINStoreTransferNoteDetailRows_Result>();
                return _INStoreTransferNoteDetailRows;
            }
            set
            {
                _INStoreTransferNoteDetailRows = value;
            }
        }
    }
}