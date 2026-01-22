using DAL;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.ViewModels.DV;
using GL.ViewModels.AP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using GL.ViewModels.DVO;

namespace GL.Controllers
{
    public class DVODevOperationsController : Controller
    {
        [HttpGet]
        public ActionResult DVOBillingList()
        {
            try
            {
                
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];

                ViewBag.Projects = new DALDropdowns().DVProjectsList(1);
                ViewBag.Units = new List<DVUnit>();


                var model = new DVOBillingViewModel();
                model.DVOBilling.BillingDate = DateTime.Now;
                //ViewBag.Projects = new DALDropdowns().DVProjectsList(LoginUser.CompanyID);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult DVOBillingSearchList( DateTime StartDate, DateTime EndDate, int? ProjectID, int? UnitID)
        {
            try
            {
                
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new DVOBillingViewModel();


                //search.CompanyID = LoginUser.CompanyID;

                model.DVOBillingSearchListRows = new DALDVOBilling().GetDVOBillingSearchList(LoginUser.CompanyID,StartDate,EndDate,ProjectID, UnitID);
                return View("_DVOBillingSearchList", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult DVOBilling(int? BillingID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                DVOBillingViewModel model = new DVOBillingViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();
 
                ViewBag.Projects = dalDropdowns.DVProjectsList(1);
                ViewBag.Units = new List<DVUnit>();
                ViewBag.DVPaymentPlanTypeList = dalDropdowns.DVPaymentPlanTypeList(LoginUser.CompanyID);

                if (BillingID != null || BillingID > 0)
                {
                    model.DVOBilling = new DALDVOBilling().GetDVOBilling(BillingID.Value);
                    ViewBag.Units = dalDropdowns.DVOccupiedUnitsListByProjectID(model.DVOBilling.ProjectID.GetValueOrDefault(0),true);
                    model.DVOBillingDetailRows = new DALDVOBilling().GetDVOBillingDetailRows(BillingID.Value);
                }
                else
                {
                    model.DVOBilling = new DVOBilling();
                    model.DVOBilling.BillingID = 0;
                    model.DVOBilling.BillingDate = DateTime.Now;
                    model.DVOBilling.ApplicationFormID = 0;
                    model.DVOBilling.CreatedAt = DateTime.Now;
                    model.DVOBilling.CreatedBy=LoginUser.UsersID;
                    model.DVOBilling.ModifiedAt = DateTime.Now;
                    model.DVOBilling.ModifiedBy=LoginUser.UsersID;
                    model.DVOBilling.CompanyID = LoginUser.CompanyID;
                    
                    model.DVOBillingDetailRows = new List<spDVOBillingDetailRows_Result>();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult DVOBillingSave(DVOBilling DVOBilling, List<DVOBillingDetail> DVOBillingDetail)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];

                GL.Models.response res = new GL.Models.response();  

                if (DVOBilling.BillingID == 0)
                {
                    DVOBilling.CompanyID = LoginUser.CompanyID;
                    DVOBilling.CreatedAt = DateTime.Now;
                    DVOBilling.CreatedBy = LoginUser.UsersID;
                }

                DVOBilling.ModifiedAt = DateTime.Now;
                DVOBilling.ModifiedBy = LoginUser.UsersID;



                bool result = new DALDVOBilling().DVBillingSave(DVOBilling);


                if (result == true)
                {
                    // save details
                    if (DVOBillingDetail != null && DVOBillingDetail.Count > 0)
                    {
                        foreach (var item in DVOBillingDetail)
                        {
                            item.BillingID = DVOBilling.BillingID;

                            if (item.BillingDetailID == 0)
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
                            new DALDVOBilling().DVOBillingDetailSave(item);
                        }
                    }
                    // end save details

                    res.resObj = DVOBilling;
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


        public ActionResult DVOBillingDetailDelete(int BillingDetailID)
        {
            try
            {
                APInvoiceViewModel model = new APInvoiceViewModel();

                new DALDVOBilling().DVOBillingDetailDelete(BillingDetailID);
                return RedirectToAction("DVOBilling");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost]
        //public JsonResult DVOBillingDetailDelete(int BillingDetailID)
        //{
        //    try
        //    {
        //        GL.Models.response res = new GL.Models.response();
        //        var result = new DALDVReceipt().DVReceiptDetailDelete(BillingDetailID);
        //        if (result == true)
        //        {
        //            res.status = true;
        //            res.resMessage = "Row deleted successfully!";
        //        }
        //        else
        //        {
        //            res.status = false;
        //            res.resMessage = "There is some error to delete this row!";
        //        }

        //        return Json(res, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


    }


}