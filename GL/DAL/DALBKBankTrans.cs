using GL.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;


namespace GL.DAL
{
    public class DALBKBankTrans
    {

        private GLEntities db = new GLEntities();

        #region BKBankTran
        public List<spBKBankTransSearchList_Result> GetBKBankTransSearchList(int CompanyID)
        {
            try
            {
                var BKBankTranList = db.spBKBankTransSearchList(CompanyID).ToList();
                return BKBankTranList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public BKBankTran GetBKBankTrans(Int64 BankTransID)
        {
            try
            {
                var BKBankTran = db.BKBankTrans.Where(x => x.BankTransID == BankTransID).FirstOrDefault(); 
                return BKBankTran;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BKBankTran GetBKBankTransIfExist(int CompanyID, int BankTransID, int BankID, DateTime TransDate)
        {
            try
            {
                //var BKBankTran = db.BKBankTrans.Where(x => x.CompanyID == CompanyID && x.BankID == BankID && x.TransDate == TransDate.Date).FirstOrDefault();

                var BKBankTran = db.BKBankTrans.Where(x => x.CompanyID == CompanyID && x.BankID == BankID && x.BankTransID != BankTransID &&
                    x.TransDate.Value.Year == TransDate.Year && 
                    x.TransDate.Value.Month == TransDate.Month && 
                    x.TransDate.Value.Day == TransDate.Day).FirstOrDefault();


                return BKBankTran;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spBKBankTransDetailRows_Result> GetBKBankTransDetailRows(int BankTransID)
        {
            try
            {
                List<spBKBankTransDetailRows_Result> BKBankTransDetailRows = db.spBKBankTransDetailRows(BankTransID).ToList();

                return BKBankTransDetailRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsDuplicateBKBankTrans(int CompanyID, int BankTransID, DateTime TransDate)
        {
            try
            {
                bool IsDuplicate = false;
                var BKBankTran = db.BKBankTrans.Where(x => x.CompanyID == CompanyID && x.BankTransID != BankTransID && x.TransDate == TransDate).FirstOrDefault();
                if (BKBankTran != null)
                    IsDuplicate = true;
                return IsDuplicate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool BKBankTranDelete(Int64 BankTransID)
        {
            try
            {
                var BKBankTran = db.BKBankTrans.Where(x => x.BankTransID == BankTransID).FirstOrDefault();
                if (BKBankTran != null)
                {
                    db.BKBankTrans.Remove(BKBankTran);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool BKBankTranSave(BKBankTran BKBankTran)
        {
            try
            {

                if (BKBankTran != null)
                {
                    var bank = db.BKBanks.Where(x => x.BankID == BKBankTran.BankID).FirstOrDefault();
                    if (bank != null) {
                        BKBankTran.BankTransCode = bank.BankCode + "-" + BKBankTran.TransDate.Value.ToString("dd-MMM-yyyy");
                    }


                    db.BKBankTrans.AddOrUpdate(BKBankTran);
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
        #endregion

    }
}