using GL.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace GL.DAL
{
    public class DALDVOBilling
    {
        private GLEntities db = new GLEntities();


        public List<spDVOBillingSearchList_Result> GetDVOBillingSearchList(int CompanyID, DateTime StartDate, DateTime EndDate, int? ProjectID, int? UnitID)
        {
            try
            {
                var DVOBillingSearchList = db.spDVOBillingSearchList(CompanyID, StartDate, EndDate, ProjectID, UnitID).ToList();
                return DVOBillingSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DVOBilling GetDVOBilling(Int64 BillingID)
        {
            try
            {
                var DVOBilling = db.DVOBillings.Where(x => x.BillingID == BillingID).FirstOrDefault();
                return DVOBilling;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<spDVOBillingDetailRows_Result> GetDVOBillingDetailRows(int BillingID)
        {
            try
            {
                var DVOBillingDetailRows = db.spDVOBillingDetailRows(BillingID).ToList();

                return DVOBillingDetailRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DVBillingSave(DVOBilling DVOBilling)
        {
            try
            {
                if (DVOBilling != null)
                {
                    if(DVOBilling.ApplicationFormID == 0)
                    {
                        var dbBilling = db.DVApplicationForms.Where(x => x.ProjectID == DVOBilling.ProjectID && x.UnitID == DVOBilling.UnitID).FirstOrDefault();
                        DVOBilling.ApplicationFormID = dbBilling.ApplicationFormID;
                    }


                    db.DVOBillings.AddOrUpdate(DVOBilling);
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

        public DVOBillingDetail DVOBillingDetailSave(DVOBillingDetail DVOBillingDetail)
        {
            try
            {
                db.DVOBillingDetails.AddOrUpdate(DVOBillingDetail);
                db.SaveChanges();
                return DVOBillingDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DVOBillingDetailDelete(int DVOBillingDetailID)
        {
            try
            {
                var result = true;
                var DVReceiptDetail = db.DVOBillingDetails.Where(x => x.BillingDetailID == DVOBillingDetailID).FirstOrDefault();
                if (DVReceiptDetail != null)
                {
                    db.DVOBillingDetails.Remove(DVReceiptDetail);
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
    }
}