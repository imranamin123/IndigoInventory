using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;
using GL.EF;
using GL.Models;

namespace GL.DAL
{
    public class DALDVReceipt
    {
        private GLEntities db = new GLEntities();

        #region DVReceipt
        public List<spDVReceiptHead_Result> GetDVReceiptHead(int ApplicationFormID)
        {
            try
            {
                List<spDVReceiptHead_Result> DVReceiptHeadSearchList = db.spDVReceiptHead(ApplicationFormID).ToList();

                return DVReceiptHeadSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<spDVReceiptDetail_Result> GetDVReceiptDetailRows(int DVReceiptID)
        {
            try
            {
                List<spDVReceiptDetail_Result> DVReceiptDetailList = db.spDVReceiptDetail(DVReceiptID).ToList();

                return DVReceiptDetailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spDVReceiptHeadByReceiptID_Result> GetDVReceiptHeadByReceiptID(int ReceiptID)
        {
            try
            {
                var DVReceiptHeadByReceiptIDSearchList = db.spDVReceiptHeadByReceiptID(ReceiptID).ToList();

                return DVReceiptHeadByReceiptIDSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<spDVReceiptDetailByReceiptID_Result> GetDVReceiptDetailByReceiptIDRows(int DVReceiptID)
        {
            try
            {
                var DVReceiptDetailByReceiptIDList = db.spDVReceiptDetailByReceiptID(DVReceiptID).ToList();

                return DVReceiptDetailByReceiptIDList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DVReceiptDetailDelete(int DVReceiptDetailID)
        {
            try
            {
                var result = true;
                var DVReceiptDetail = db.DVReceiptDetails.Where(x => x.DVReceiptDetailID == DVReceiptDetailID).FirstOrDefault();
                if (DVReceiptDetail != null)
                {
                    db.DVReceiptDetails.Remove(DVReceiptDetail);
                    db.SaveChanges();
                }
                else
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DVReceiptSave(DVReceipt DVReceipt)
        {
            try
            {

                if (DVReceipt != null)
                {
                    // to check who changed varified and locked 

                    var receptdDB = db.DVReceipts.Find(DVReceipt.DVReceiptID);
                    if (receptdDB != null)
                    {

                        if ( receptdDB.Varified != DVReceipt.Varified && DVReceipt.Varified == true)
                        {
                            DVReceipt.VarifiedBy = DVReceipt.ModifiedBy;
                        }
                        else if (receptdDB.Varified != DVReceipt.Varified && DVReceipt.Varified == false)
                        {
                            DVReceipt.VarifiedBy = 0;
                        }

                        if (receptdDB.Locked != DVReceipt.Locked && DVReceipt.Locked == true)
                        {
                            DVReceipt.LockedBy = DVReceipt.ModifiedBy;
                        }
                        else if (receptdDB.Locked != DVReceipt.Locked && DVReceipt.Locked == false)
                        {
                            DVReceipt.LockedBy = 0;
                        }

                    }
                    db.DVReceipts.AddOrUpdate(DVReceipt);
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


        public DVReceiptDetail DVReceiptDetailSave(DVReceiptDetail DVReceiptDetail)
        {
            try
            {
                db.DVReceiptDetails.AddOrUpdate(DVReceiptDetail);
                db.SaveChanges();
                return DVReceiptDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spDVReceiptSearchList_Result> GetDVDVReceiptSearchList(SearchModel search)
        {
            try
            {
                List<spDVReceiptSearchList_Result> DVReceiptSearchList = db.spDVReceiptSearchList(search.CompanyID, search.ProjectID, search.UnitID).ToList();

                return DVReceiptSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spDVReceiptSearchListSGO_Result> GetDVDVReceiptSearchListSGO(SearchModel search)
        {
            try
            {
                List<spDVReceiptSearchListSGO_Result> DVReceiptSearchListSGO = db.spDVReceiptSearchListSGO(search.CompanyID, search.ProjectID, search.UnitID).ToList();

                return DVReceiptSearchListSGO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}