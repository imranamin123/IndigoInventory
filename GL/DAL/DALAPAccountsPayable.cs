using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Data.Entity.Migrations;

using GL.EF;
using GL.Models;

namespace GL.DAL
{
    public class DALAPAccountsPayable
    {

        private GLEntities db = new GLEntities();

        #region APVendor


        public bool IsAPVendorCodeExist(APVendor APVendor)
        {
            try
            {
                bool isExist = false;
                var APVendorCodeDb = db.APVendors.Where(x => x.CompanyID == APVendor.CompanyID && x.APVendorCode == APVendor.APVendorCode && x.APVendorID != APVendor.APVendorID).FirstOrDefault();
                if (APVendorCodeDb != null)
                {
                    isExist = true;
                }
                return isExist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<APVendor> APVendorList(int CompanyID)
        {
            try
            {
                var APVendorList = db.APVendors.Where(x => x.CompanyID == CompanyID).ToList();
                return APVendorList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public APVendor APVendorGet(Int64 APVendorID)
        {
            try
            {
                var APVendor = db.APVendors.Where(x => x.APVendorID == APVendorID).FirstOrDefault();
                return APVendor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool APVendorDelete(Int64 APVendorID)
        {
            try
            {
                var APVendor = db.APVendors.Where(x => x.APVendorID == APVendorID).FirstOrDefault();
                db.APVendors.Remove(APVendor);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool APVendorSave(APVendor APVendor)
        {
            try
            {
            //    if (APVendor.APVendorID == 0)
            //    {
            //        APVendor.APVendorID = db.APVendors.Max(x => x.APVendorID) + 1;
            //    }

                if (APVendor != null)
                {
                    db.APVendors.AddOrUpdate(APVendor);
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

        #region APInvoice

        public bool APInvoiceDetailDelete(Int64 id)
        {
            try
            {
                var APInvoiceDetail = db.APInvoiceDetails.Where(x => x.APInvoiceDetailID == id).FirstOrDefault();
                db.APInvoiceDetails.Remove(APInvoiceDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool APInvoiceSave(APInvoice APInvoice)
        {
            try
            {

                if (APInvoice != null)
                {
                    db.APInvoices.AddOrUpdate(APInvoice);
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

        public List<spAPInvoiceSearchList_Result> GetAPInvoiceSearchList(SearchModel search)
        {
            try
            {
                List<spAPInvoiceSearchList_Result> APInvoiceSearchList = db.spAPInvoiceSearchList(search.CompanyID).ToList();

                return APInvoiceSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spAPInvoiceDetailRows_Result> GetAPInvoiceDetailRows(Int64 APInvoiceID)
        {
            try
            {
                List<spAPInvoiceDetailRows_Result> APInvoiceDetailRows = db.spAPInvoiceDetailRows(APInvoiceID).ToList();

                return APInvoiceDetailRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public APInvoice APInvoiceGet(long APInvoiceID)
        {
            try
            {
                var APInvoice = db.APInvoices.Where(x => x.APInvoiceID== APInvoiceID).FirstOrDefault();
                return APInvoice;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public APInvoiceDetail APInvoiceDetailSave(APInvoiceDetail APInvoiceDetail)
        {
            try
            {

                db.APInvoiceDetails.AddOrUpdate(APInvoiceDetail);
                db.SaveChanges();
                return APInvoiceDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}