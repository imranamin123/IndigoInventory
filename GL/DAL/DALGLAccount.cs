using GL.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace GL.DAL
{
    public class DALGLAccount
    {
        private GLEntities db = new GLEntities();

        #region GLAccount
        public List<GLAccount> GLAccountList(int CompanyID)
        {
            try
            {
                var GLAccountList = db.GLAccounts.Where(x => x.CompanyID == CompanyID).ToList();
                return GLAccountList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public GLAccount GLAccountGet(Int64 GLAccountID)
        {
            try
            {
                var GLAccount = db.GLAccounts.Where(x => x.GLAccountID == GLAccountID).FirstOrDefault();
                return GLAccount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsDuplicateGLAccount(int CompanyID, int GLAccountID, string GLAccountNo)
        {
            try
            {
                bool IsDuplicate = false;
                var GLAccount = db.GLAccounts.Where(x => x.CompanyID == CompanyID &&  x.GLAccountID != GLAccountID && x.GLAccountNo == GLAccountNo).FirstOrDefault();
                if(GLAccount != null)
                    IsDuplicate = true;
                return IsDuplicate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GLAccountDelete(Int64 GLAccountID)
        {
            try
            {
                var GLAccount = db.GLAccounts.Where(x => x.GLAccountID == GLAccountID).FirstOrDefault();
                db.GLAccounts.Remove(GLAccount);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool GLAccountSave(GLAccount GLAccount)
        {
            try
            {
                
                if (GLAccount != null)
                {
                    db.GLAccounts.AddOrUpdate(GLAccount);
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