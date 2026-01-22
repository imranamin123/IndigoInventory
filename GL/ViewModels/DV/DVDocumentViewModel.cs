using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.DV
{
    public class DVDocumentViewModel
    {
        public int DocumentID = 0;
        private DVDocument _DVDocument { get; set; }
        public DVDocument DVDocument
        {
            get 
            {
                if(_DVDocument == null)
                    _DVDocument = new DVDocument();
                return _DVDocument;
            }
            set 
            {
                _DVDocument = value;
            } 
        }

        private List<spDocumentListByApplication_Result> _DocumentListRows { get; set; }
        public List<spDocumentListByApplication_Result> DocumentListRows
        {
            get
            {
                if (_DocumentListRows == null)
                {
                    _DocumentListRows = new List<spDocumentListByApplication_Result>();
                }
                return _DocumentListRows;
            }
            set
            {
                _DocumentListRows = value;
            }
        }
    }
}