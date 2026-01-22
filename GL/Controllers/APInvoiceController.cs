using Azure;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.ViewModels.AP;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace GL.Controllers
{
    public class APInvoiceController : Controller
    {
        #region APInvoice
        [HttpGet]
        public ActionResult APInvoiceList()
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new APInvoiceViewModel();

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult APInvoiceSearchList(SearchModel search)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new APInvoiceViewModel();

                search.CompanyID = LoginUser.CompanyID;

                model.APInvoiceSearchList = new DALAPAccountsPayable().GetAPInvoiceSearchList(search);
                return View("_APInvoiceSearchList", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult APInvoice(long? APInvoiceID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                APInvoiceViewModel model = new APInvoiceViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.APVendors = dalDropdowns.GLAPVendorsListWithCode(LoginUser.CompanyID);
                ViewBag.GLAccounts = dalDropdowns.GLAccountGetForDropdown(LoginUser.CompanyID); 

                if (APInvoiceID != null || APInvoiceID > 0)
                {
                    model.APInvoice = new DALAPAccountsPayable().APInvoiceGet(APInvoiceID.GetValueOrDefault(0));                   

                    model.APInvoiceDetailRows = new DALAPAccountsPayable().GetAPInvoiceDetailRows(APInvoiceID.Value);
                }
                else
                {
                    model.APInvoice = new APInvoice();
                    
                    model.APInvoice.APInvoiceID = 0;
                    model.APInvoice.APVendorID = 0;                        
                    model.APInvoice.APInvoiceDate = DateTime.Now;
                    model.APInvoice.APInvoiceNo = string.Empty;
                    model.APInvoice.APInvoiceAmount = 0;
                    model.APInvoice.APTaxAmount=0;
                    model.APInvoice.IsPosted= false;
                    model.APInvoice.CreatedAt = DateTime.Now;
                    model.APInvoice.CreatedBy = 0;
                    model.APInvoice.ModifiedAt = DateTime.Now;
                    model.APInvoice.ModifiedBy=0;
                    model.APInvoice.CompanyID = 0;
                    model.APInvoiceDetailRows = new List<spAPInvoiceDetailRows_Result>();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        [HttpGet]
        public ActionResult APInvoiceDetailDelete(Int64 id)
        {
            try
            {
                APInvoiceViewModel model = new APInvoiceViewModel();

                new DALAPAccountsPayable().APInvoiceDetailDelete(id);
                return RedirectToAction("APInvoice");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult APInvoiceSave(APInvoice APInvoice, List<APInvoiceDetail> APInvoiceDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                GL.Models.response res = new GL.Models.response();
                if (APInvoice.APInvoiceID == 0)
                {
                    APInvoice.CompanyID = LoginUser.CompanyID;
                    APInvoice.CreatedAt = DateTime.Now;
                    APInvoice.CreatedBy = LoginUser.UsersID;
                    APInvoice.ModifiedAt = DateTime.Now;
                    APInvoice.ModifiedBy = LoginUser.UsersID;
                }
                else
                {
                    //APInvoice.CompanyID = LoginUser.CompanyID;
                    APInvoice.ModifiedAt = DateTime.Now;
                    APInvoice.ModifiedBy = LoginUser.UsersID;
                }


                bool result = new DALAPAccountsPayable().APInvoiceSave(APInvoice);


                if (result == true)
                {
                    // save details
                    if (APInvoiceDetailList != null && APInvoiceDetailList.Count > 0)
                    {
                        foreach (var item in APInvoiceDetailList)
                        {
                            item.APInvoiceID = APInvoice.APInvoiceID;

                            if (item.APInvoiceDetailID == 0)
                            {
                                item.CreatedBy = LoginUser.UsersID;
                                item.CreatedAt = DateTime.Now;
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                                item.CompanyID = LoginUser.CompanyID;
                            }
                            else
                            {
                                //item.CompanyID = LoginUser.CompanyID;
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                            }
                            new DALAPAccountsPayable().APInvoiceDetailSave(item);
                        }
                    }
                    // end save details

                    res.resObj = APInvoice;
                    res.status = true;
                    res.resMessage = "Record save successfully!";
                }
                else
                {
                    res.resObj = null;
                    res.status = false;
                    res.resMessage = "Record was not saved. Please contact to the admin!";

                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region Utility Methods

        //public string GetGLAccountDescription(int id)
        //{
        //    var glAccount = new DALGLAccount().GLAccountGet(id);
        //    return glAccount.Description;
        //}

        //public string GetFiscalYearSetup(DateTime VoucherDate)
        //{
        //    string FiscalPeriod = string.Empty; 
        //    var LoginUser = (spLoginUser_Result)Session["LoginUser"];
        //    var FiscalYearSetup = new DALCommon().GetFiscalYearSetup(LoginUser.CompanyID, VoucherDate.Year, VoucherDate.Month);
        //    if (FiscalYearSetup != null)
        //    {
        //        FiscalPeriod = FiscalYearSetup.FiscalYear.ToString() + "-" + FiscalYearSetup.FiscalPeriod.ToString();
        //    }
        //    return FiscalPeriod;

        //}
        #endregion
    }
}