using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;

namespace GL.DAL
{
    public class DALGLVoucherDetailDetail
    {
        private GLEntities db = new GLEntities();

        #region GLVoucherDetail    
        public List<GLVoucherDetail> GLVoucherDetailList()
        {
            try
            {
                var GLVoucherDetailList = db.GLVoucherDetails.ToList();
                return GLVoucherDetailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public GLVoucherDetail GLVoucherDetailGet(Int64 GLVoucherDetailID)
        {
            try
            {
                var GLVoucherDetail = db.GLVoucherDetails.Where(x => x.GLVoucherDetailID == GLVoucherDetailID).FirstOrDefault();
                return GLVoucherDetail;
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
        public bool GLVoucherDetailSave(GLVoucherDetail GLVoucherDetail)
        {
            try
            {

                if (GLVoucherDetail != null)
                {
                    db.GLVoucherDetails.AddOrUpdate(GLVoucherDetail);
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