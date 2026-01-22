using GL.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace GL.DAL
{
    public class DALBSClassification
    {
        private GLEntities db = new GLEntities();

        #region BSClassification
        public List<spBSClassificationSearchList_Result> GetBSClassificationSearchList(int CompanyID)
        {
            try
            {
                var BSClassificationSearchList = db.spBSClassificationSearchList(CompanyID).ToList();
                return BSClassificationSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public BSClassification GetBSClassification(int TransactionID)
        //{
        //    try
        //    {
        //        var BSClassification = db.BSClassifications.Where(x => x.BSClassificationID == TransactionID).FirstOrDefault();
        //        return BSClassification;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        
        //public List<spBSClassificationSearchList_Result> GetBSClassificationSearchList(int CompanyID)
        //{
        //    try
        //    {
        //        var BSClassificationSearchList = db.spBSClassificationSearchList(CompanyID).ToList();
        //        return BSClassificationSearchList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public List<spBSBankStatementPendingUnpendingList_Result> GetBSBankStatementPendingUnpendingList(int CompanyID,int BKBankID,bool IsPending, DateTime ValueDateFrom, DateTime ValueDateTo)
        {
            try
            {
                var BSClassificationSearchList = db.spBSBankStatementPendingUnpendingList(CompanyID, BKBankID, IsPending, ValueDateFrom, ValueDateTo).ToList();
                return BSClassificationSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spBSClassificationList_Result> GetBSClassificationList(int TransactionID)
        {
            try
            {
                var BSClassificationList = db.spBSClassificationList(TransactionID).ToList();
                return BSClassificationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BSClassificationDetail> GetBSClassificationDetailList(int TransactionID)
        {
            try
            {
                var BSClassificationDetailList = db.BSClassificationDetails.Where(x => x.TransactionID == TransactionID).ToList();
                return BSClassificationDetailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BSClassification GetBSClassification(int TransactionID)
        {
            try
            {
                var BSClassification = db.BSClassifications.Where(x => x.TransactionID == TransactionID).FirstOrDefault();
                return BSClassification;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public spBSClassificationHeader_Result GetBSClassificationHeader(int TransactionID)
        {
            try
            {
                var BSClassificationHeader = db.spBSClassificationHeader(TransactionID).FirstOrDefault();
                return BSClassificationHeader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BSBankStatement GetBSBankStatement(int TransactionID)
        {
            try
            {
                var BSBankStatement = db.BSBankStatements.Where(x => x.TransactionID == TransactionID).FirstOrDefault();
                return BSBankStatement;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool BSClassificationDetailDelete(Int64 BSClassificationDetailID)
        {
            try
            {
                var BSClassificationDetail = db.BSClassificationDetails.Where(x => x.BSClassificationDetailID== BSClassificationDetailID).FirstOrDefault();
                if (BSClassificationDetail != null)
                {
                    var BSClassificationID = BSClassificationDetail.BSClassificationID;
                    db.BSClassificationDetails.Remove(BSClassificationDetail);
                    db.SaveChanges();
                    var count = db.BSClassificationDetails.Where(x => x.BSClassificationID == BSClassificationID).Count();
                    if (count <= 0) {
                        var BSClassification = db.BSClassifications.Where(x => x.BSClassificationID == BSClassificationID).FirstOrDefault();
                        db.BSClassifications.Remove(BSClassification);
                        db.SaveChanges();
                    }
                    

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool BSClassificationSave(BSClassification BSClassification)
        {
            try
            {

                if (BSClassification != null)
                {
                    // to check who changed varified and locked 
                    db.BSClassifications.AddOrUpdate(BSClassification);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public BSClassificationDetail BSClassificationDetailSave(BSClassificationDetail BSClassificationDetail)
        {
            try
            {
                db.BSClassificationDetails.AddOrUpdate(BSClassificationDetail);
                db.SaveChanges();
                return BSClassificationDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public BKBankTran GetBKBankTransIfExist(int CompanyID, int BankID, DateTime TransDate)
        //{
        //    try
        //    {
        //        //var BKBankTran = db.BKBankTrans.Where(x => x.CompanyID == CompanyID && x.BankID == BankID && x.TransDate == TransDate.Date).FirstOrDefault();

        //        var BKBankTran = db.BKBankTrans.Where(x => x.CompanyID == CompanyID && x.BankID == BankID &&
        //            x.TransDate.Value.Year == TransDate.Year &&
        //            x.TransDate.Value.Month == TransDate.Month &&
        //            x.TransDate.Value.Day == TransDate.Day).FirstOrDefault();


        //        return BKBankTran;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public List<spBKBankTransDetailRows_Result> GetBKBankTransDetailRows(int BankTransID)
        //{
        //    try
        //    {
        //        List<spBKBankTransDetailRows_Result> BKBankTransDetailRows = db.spBKBankTransDetailRows(BankTransID).ToList();

        //        return BKBankTransDetailRows;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool IsDuplicateBKBankTrans(int CompanyID, int BankTransID, DateTime TransDate)
        //{
        //    try
        //    {
        //        bool IsDuplicate = false;
        //        var BKBankTran = db.BKBankTrans.Where(x => x.CompanyID == CompanyID && x.BankTransID != BankTransID && x.TransDate == TransDate).FirstOrDefault();
        //        if (BKBankTran != null)
        //            IsDuplicate = true;
        //        return IsDuplicate;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool BKBankTranDelete(Int64 BankTransID)
        //{
        //    try
        //    {
        //        var BKBankTran = db.BKBankTrans.Where(x => x.BankTransID == BankTransID).FirstOrDefault();
        //        if (BKBankTran != null)
        //        {
        //            db.BKBankTrans.Remove(BKBankTran);
        //            db.SaveChanges();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public bool BKBankTranSave(BKBankTran BKBankTran)
        //{
        //    try
        //    {

        //        if (BKBankTran != null)
        //        {
        //            var bank = db.BKBanks.Where(x => x.BankID == BKBankTran.BankID).FirstOrDefault();
        //            if (bank != null)
        //            {
        //                BKBankTran.BankTransCode = bank.BankCode + "-" + BKBankTran.TransDate.Value.ToString("dd-MMM-yyyy");
        //            }


        //            db.BKBankTrans.AddOrUpdate(BKBankTran);
        //            db.SaveChanges();
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

    }
}