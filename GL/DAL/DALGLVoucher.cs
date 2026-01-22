using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;
using GL.EF;
using GL.Models;

namespace GL.DAL
{
    public class DALGLVoucher
    {

        private GLEntities db = new GLEntities();

        #region GLVoucher   

        public List<spGLVoucherSearchList_Result> GetGLVoucherSearchList(SearchModel search)
        {
            try
            {
                List<spGLVoucherSearchList_Result> GLVoucherSearchList = db.spGLVoucherSearchList(search.CompanyID, search.StatusTypeID).ToList();

                return GLVoucherSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spGLVoucherDetailRows_Result> GetGLVoucherDetailRows(int CompanyID, int GLVoucherID)
        {
            try
            {
                List<spGLVoucherDetailRows_Result> GLVoucherSearchList = db.spGLVoucherDetailRows(CompanyID, GLVoucherID).ToList();

                return GLVoucherSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLVoucher> GLVoucherList(int companyID)
        {
            try
            {
                var GLVoucherList = db.GLVouchers.Where(x=> x.CompanyID == companyID).ToList();
                return GLVoucherList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GLVoucher GLVoucherGet(Int64 GLVoucherID)
        {
            try
            {
                var GLVoucher = db.GLVouchers.Where(x => x.GLVoucherID == GLVoucherID).FirstOrDefault();
                return GLVoucher;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spRptGLVoucherGetByID_Result> GLRptVoucherReportGetByID(int GLVoucherID)
        {
            try
            {
                var GLVoucher = db.spRptGLVoucherGetByID(GLVoucherID).ToList(); // db.GLVouchers.Where(x => x.GLVoucherID == GLVoucherID).FirstOrDefault();
                return GLVoucher;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GLVoucherDelete(Int64 GLVoucherID)
        {
            try
            {
                var GLVoucher = db.GLVouchers.Where(x => x.GLVoucherID == GLVoucherID).FirstOrDefault();
                db.GLVouchers.Remove(GLVoucher);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GLVoucherDetailDelete(Int64 GLVoucherDetailID)
        {
            try
            {
                var GLVoucherDetail = db.GLVoucherDetails.Where(x => x.GLVoucherDetailID == GLVoucherDetailID).FirstOrDefault();
                db.GLVoucherDetails.Remove(GLVoucherDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GLVoucherSave(GLVoucher GLVoucher)
        {
            try
            {

                if (GLVoucher != null)
                {
                    db.GLVouchers.AddOrUpdate(GLVoucher);
                    db.SaveChanges();

                    var voucherType = db.VoucherTypes.Where(x => x.VoucherTypeID == GLVoucher.VoucherTypeID).FirstOrDefault();

                    if (voucherType != null)
                    {
                        GLVoucher.VoucherNumber = voucherType.VTYPE + "-" + GLVoucher.GLVoucherID;
                    }

                    db.GLVouchers.AddOrUpdate(GLVoucher);
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

        //public bool GLVoucherSave(GLVoucher GLVoucher)
        //{
        //    try
        //    {

        //        if (GLVoucher != null)
        //        {
        //            db.GLVouchers.AddOrUpdate(GLVoucher);
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

        public GLVoucherDetail GLVoucherDetailSave(GLVoucherDetail glVoucherDetail)
        {
            try
            {

                db.GLVoucherDetails.AddOrUpdate(glVoucherDetail);
                db.SaveChanges();
                return glVoucherDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}