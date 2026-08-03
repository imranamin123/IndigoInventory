using GL.DAL;
using GL.EF;
using GL.ReportsWebForms;
using GL.ViewModels.DV;
using GL.ViewModels.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace GL.Controllers
{
    public class DVPaymentPlanController : Controller
    {
        [HttpGet]
        public ActionResult DVPaymentPlan(int? ApplicationFormID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                DVPaymentPlanViewModel model = new DVPaymentPlanViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.DVPaymentPlanTypeList = dalDropdowns.DVPaymentPlanTypeList(LoginUser.CompanyID);

                if (ApplicationFormID != null || ApplicationFormID > 0)
                {
                    var dal = new DALDVPaymentPlan();
                    model.DVPaymentPlanHead = dal.GetPaymentPlanHead(ApplicationFormID.Value).FirstOrDefault();
                    if(model.DVPaymentPlanHead.PaymentPlanID == 0)
                    {
                        model.DVPaymentPlanHead.ApplicationFormDate= DateTime.Now;
                        model.DVPaymentPlanHead.PaymentPlanDate = DateTime.Now;
                        model.DVPaymentPlanHead.CreatedAt = DateTime.Now;
                        model.DVPaymentPlanHead.ModifiedAt = DateTime.Now;  
                    }

                        model.DVPaymentPlanDetailRows = dal.GetDVPaymentPlanDetailRows(model.DVPaymentPlanHead.PaymentPlanID).ToList();
                }
                else
                {
                    model.DVPaymentPlanHead = new  spDVPaymentPlanHead_Result();
                    model.DVPaymentPlanHead.Amount = 0;
                    model.DVPaymentPlanHead.ProjectName = string.Empty;
                    model.DVPaymentPlanHead.PaymentPlanDate = DateTime.Now;
                    model.DVPaymentPlanHead.PaymentPlanID = 0;
                    model.DVPaymentPlanHead.ApplicationFormID = ApplicationFormID.Value;
                    model.DVPaymentPlanDetailRows = new List<spDVPaymentPlanDetail_Result>();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult DVPaymentPlanSave(DVPaymentPlan DVPaymentPlan, List<DVPaymentPlanDetail> DVPaymentPlanDetail)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }

                GL.Models.response res = new GL.Models.response();

                if (DVPaymentPlan.PaymentPlanID == 0)
                {
                    DVPaymentPlan.CompanyID = LoginUser.CompanyID;
                    DVPaymentPlan.CreatedAt = DateTime.Now;
                    DVPaymentPlan.CreatedBy = LoginUser.UsersID;
                }

                DVPaymentPlan.ModifiedAt = DateTime.Now;
                DVPaymentPlan.ModifiedBy = LoginUser.UsersID;


                bool result = new DALDVPaymentPlan().DVPaymentPlanSave(DVPaymentPlan);


                if (result == true)
                {
                    // save details
                    if (DVPaymentPlanDetail != null && DVPaymentPlanDetail.Count > 0)
                    {
                        foreach (var item in DVPaymentPlanDetail)
                        {
                            item.PaymentPlanID = DVPaymentPlan.PaymentPlanID;

                            if (item.PaymentPlanDetailID == 0)
                            {
                                item.CreatedBy = LoginUser.UsersID;
                                item.CreatedAt = DateTime.Now;
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                                item.CompanyID = LoginUser.CompanyID;
                            }
                            else
                            {
                                item.CompanyID = LoginUser.CompanyID;
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                            }
                            new DALDVPaymentPlan().DVPaymentPlanDetailSave(item);
                        }
                    }
                    // end save details

                    res.resObj = DVPaymentPlan;
                    res.status = true;
                    res.resMessage = "Record saved successfully!";
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public JsonResult DVPaymentPackage(int? PaymentPlanID)
        {
            try 
            {
                GL.Models.response res = new GL.Models.response();
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                var result = new DALCommon().GetDVPaymentPackage(LoginUser.CompanyID, PaymentPlanID);

                if (result != null)
                {
                    res.status = true;
                    res.resMessage = "Record found";
                    res.resObj = result;
                }
                else 
                { 
                    res.status = false;
                    res.resMessage = "Record not found";
                    res.resObj= null;
                }

                return Json(res,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) 
            {
                throw ex;
            }
            
        }

        [HttpGet]
        public JsonResult DVPaymentPlanDetailDelete(int PaymentPlanDetailID)
        {
            try
            {
                GL.Models.response res = new GL.Models.response();
                var result = new DALDVPaymentPlan().DVPaymentPlanDelete(PaymentPlanDetailID);
                if(result == true)
                {
                    res.status = true;
                    res.resMessage = "Row deleted successfully!";
                }
                else
                {
                    res.status = false;
                    res.resMessage = "There is some error to delete this row!";
                }

                return Json(res,JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}