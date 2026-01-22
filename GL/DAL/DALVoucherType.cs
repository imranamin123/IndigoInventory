using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;

namespace GL.DAL
{
    
    public class DALVoucherType
    {
        private GLEntities db = new GLEntities();

        #region VoucherType    
        public List<VoucherType> VoucherTypeList()
        {
            try
            {
                var VoucherTypeList = db.VoucherTypes.ToList();
                return VoucherTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public VoucherType VoucherTypeGet(Int64 VoucherTypeID)
        {
            try
            {
                var VoucherType = db.VoucherTypes.Where(x => x.VoucherTypeID == VoucherTypeID).FirstOrDefault();
                return VoucherType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool VoucherTypeDelete(Int64 VoucherTypeID)
        {
            try
            {
                var VoucherType = db.VoucherTypes.Where(x => x.VoucherTypeID == VoucherTypeID).FirstOrDefault();
                db.VoucherTypes.Remove(VoucherType);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool VoucherTypeSave(VoucherType VoucherType)
        {
            try
            {

                if (VoucherType != null)
                {
                    db.VoucherTypes.AddOrUpdate(VoucherType);
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