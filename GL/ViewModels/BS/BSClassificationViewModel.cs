using GL.EF;
using GL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.ViewModels.BS
{
    public class BSClassificationViewModel
    {
        public int BSClassificationID { get; set; }

        private spBSClassificationHeader_Result _BSClassificationHeader { get; set; }
        public spBSClassificationHeader_Result BSClassificationHeader
        {
            get
            {
                if (_BSClassificationHeader == null)
                    _BSClassificationHeader = new spBSClassificationHeader_Result();
                return _BSClassificationHeader;
            }
            set
            {
                _BSClassificationHeader = value;
            }
        }

        private BSBankStatement _BSBankStatement { get; set; }
        public BSBankStatement BSBankStatement
        {
            get
            {
                if (_BSBankStatement == null)
                    _BSBankStatement = new BSBankStatement();
                return _BSBankStatement;
            }
            set
            {
                _BSBankStatement = value;
            }
        }

        private BSClassification _BSClassification { get; set; }
        public BSClassification BSClassification
        {
            get
            {
                if (_BSClassification == null)
                    _BSClassification = new BSClassification();
                return _BSClassification;
            }
            set
            {
                _BSClassification = value;
            }
        }

        private List< spBSBankStatementPendingUnpendingList_Result> _BSBankStatementPendingUnpendingList { get; set; }
        public List<spBSBankStatementPendingUnpendingList_Result> BSBankStatementPendingUnpendingList
        {
            get
            {
                if (_BSBankStatementPendingUnpendingList == null)
                    _BSBankStatementPendingUnpendingList = new List<spBSBankStatementPendingUnpendingList_Result>();
                return _BSBankStatementPendingUnpendingList;
            }
            set
            {
                _BSBankStatementPendingUnpendingList = value;
            }
        }

        private List<spBSClassificationList_Result> _BSClassificationListModel { get; set; }
        public List<spBSClassificationList_Result> BSClassificationListModel
{
            get
            {
                if (_BSClassificationListModel == null)
                    _BSClassificationListModel = new List<spBSClassificationList_Result>();
                return _BSClassificationListModel;
            }
            set
            {
                _BSClassificationListModel = value;
            }
        }


// Search list page
        private List<spBSClassificationSearchList_Result> _BSClassificationSearchList { get; set; }
        public List<spBSClassificationSearchList_Result> BSClassificationSearchList
        {
            get
            {
                if (_BSClassificationSearchList == null)
                    _BSClassificationSearchList = new List<spBSClassificationSearchList_Result>();
                return _BSClassificationSearchList;
            }
            set
            {
                _BSClassificationSearchList = value;
            }
        }

    }
}