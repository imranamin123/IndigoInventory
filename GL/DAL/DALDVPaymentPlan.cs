using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;
using GL.EF;
using GL.Models;

namespace GL.DAL
{
    public class DALDVPaymentPlan
    {
        private GLEntities db = new GLEntities();

        #region DVPaymentPlan   

        public List<spDVPaymentPlanHead_Result> GetPaymentPlanHead(int ApplicationFormID)
        {
            try
            {
                List<spDVPaymentPlanHead_Result> PaymentPlanHeadSearchList = db.spDVPaymentPlanHead(ApplicationFormID).ToList();

                return PaymentPlanHeadSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spDVPaymentPlanDetail_Result> GetDVPaymentPlanDetailRows(int PaymentPlanID)
        {
            try
            {
                List<spDVPaymentPlanDetail_Result> DVPaymentPlanDetailList = db.spDVPaymentPlanDetail(PaymentPlanID).ToList();

                return DVPaymentPlanDetailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DVPaymentPlanDelete(int PaymentPlanDetailID)
        {
            try
            {
                var DVPaymentPlanDetail = db.DVPaymentPlanDetails.Where(x => x.PaymentPlanDetailID == PaymentPlanDetailID).FirstOrDefault();
                db.DVPaymentPlanDetails.Remove(DVPaymentPlanDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DVPaymentPlanSave(DVPaymentPlan DVPaymentPlan)
        {
            try
            {

                if (DVPaymentPlan != null)
                {
                    db.DVPaymentPlans.AddOrUpdate(DVPaymentPlan);
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


        public DVPaymentPlanDetail DVPaymentPlanDetailSave(DVPaymentPlanDetail DVPaymentPlanDetail)
        {
            try
            {

                db.DVPaymentPlanDetails.AddOrUpdate(DVPaymentPlanDetail);
                db.SaveChanges();
                return DVPaymentPlanDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

       

    }
}