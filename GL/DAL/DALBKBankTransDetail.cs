using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;

namespace GL.DAL
{
    public class DALBKBankTransDetail
    {
        private GLEntities db = new GLEntities();

        #region BKBankTransDetails    
        public List<BKBankTransDetail> BKBankTransDetailList( int BKBankTransID)
        {
            try
            {
                var BKBankTransDetails = db.BKBankTransDetails.Where(x =>  x.BankTransID == BKBankTransID).ToList();
                return BKBankTransDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public BKBankTransDetail BKBankTransDetailGet(Int64 BKBankTransDetailID)
        {
            try
            {
                var BKBankTransDetail = db.BKBankTransDetails.Where(x => x.BankTransDetID == BKBankTransDetailID).FirstOrDefault();
                return BKBankTransDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool BKBankTransDetailDelete(Int64 BKBankTransDetailID)
        {
            try
            {
                var BKBankTransDetail = db.BKBankTransDetails.Where(x => x.BankTransDetID == BKBankTransDetailID).FirstOrDefault();
                if(BKBankTransDetail != null)
                {
                    db.BKBankTransDetails.Remove(BKBankTransDetail);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool BKBankTransDetailSave(BKBankTransDetail BKBankTransDetail)
        {
            try
            {

                if (BKBankTransDetail != null)
                {
                    db.BKBankTransDetails.AddOrUpdate(BKBankTransDetail);
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