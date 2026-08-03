using Azure;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.ViewModels.Setup;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace GL.Controllers
{
    public class GLVoucherController : Controller
    {
        #region GLVoucher
        [HttpGet]
        public ActionResult GLVoucherList()
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new GLVoucherViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.VoucherTypes = dalDropdowns.VoucherTypeList(LoginUser.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GLVoucherSearchList(SearchModel search)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new GLVoucherViewModel();

                search.CompanyID = LoginUser.CompanyID;

                model.GLVoucherList = new DALGLVoucher().GetGLVoucherSearchList(search);
                return View("_GLVoucherSearchList", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult GLVoucher(int? GLVoucherID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                GLVoucherViewModel model = new GLVoucherViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();
                
                ViewBag.VoucherTypes = dalDropdowns.VoucherTypeList(LoginUser.CompanyID);
                ViewBag.GLAccounts = dalDropdowns.GLAccountGetForDropdown(LoginUser.CompanyID); //; dalDropdowns.GLAccountList(LoginUser.CompanyID);   

                if (GLVoucherID != null || GLVoucherID > 0)
                {
                    model.GLVoucher = new DALGLVoucher().GLVoucherGet(GLVoucherID.Value);
                   
                    model.GLVoucherDetailRows = new DALGLVoucher().GetGLVoucherDetailRows(LoginUser.CompanyID, GLVoucherID.Value);
                }
                else
                {
                    model.GLVoucher = new GLVoucher();
                    model.GLVoucher.VoucherNumber = string.Empty;
                    model.GLVoucher.VoucherDate = DateTime.Now;
                    model.GLVoucher.VoucherTypeID = 0;
                    model.GLVoucher.FiscalYear=string.Empty;
                    model.GLVoucherDetailRows = new List<spGLVoucherDetailRows_Result>();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult GLVoucherDelete(int GLVoucherID, int GLVoucherDetailID)
        {
            try
            {
                GLVoucherViewModel model = new GLVoucherViewModel();
                
                new DALGLVoucher().GLVoucherDelete(GLVoucherDetailID);
                return RedirectToAction("GLVoucher",GLVoucherID);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GLVoucherDetailDelete(int id)
        {
            try
            {
                GLVoucherViewModel model = new GLVoucherViewModel();
                
                new DALGLVoucher().GLVoucherDetailDelete(id);
                return RedirectToAction("GLVoucher");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GLVoucherSave(GLVoucher glVoucher, List<GLVoucherDetail> glVoucherDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();
                if (glVoucher.GLVoucherID == 0)
                {
                    glVoucher.CompanyID = LoginUser.CompanyID;
                    glVoucher.CreatedAt = DateTime.Now;
                    glVoucher.CreatedBy = LoginUser.UsersID;
                    glVoucher.ModifiedAt = DateTime.Now;
                    glVoucher.ModifiedBy = LoginUser.UsersID;
                }
                else
                {
                    glVoucher.CompanyID = LoginUser.CompanyID;
                    glVoucher.ModifiedAt = DateTime.Now;
                    glVoucher.ModifiedBy = LoginUser.UsersID;
                }


                bool result = new DALGLVoucher().GLVoucherSave(glVoucher);


                if (result == true)
                {
                    // save details
                    if (glVoucherDetailList != null && glVoucherDetailList.Count > 0)
                    {
                        foreach(var item in glVoucherDetailList)
                        {
                            item.GLVoucherID = glVoucher.GLVoucherID;

                            if(item.GLVoucherDetailID == 0)
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
                            new DALGLVoucher().GLVoucherDetailSave(item);
                        }
                    }
                    // end save details

                    res.resObj = glVoucher;
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

        public string GetGLAccountDescription(int id)
        {
            var glAccount = new DALGLAccount().GLAccountGet(id);
            return glAccount.Description;
        }

        public string GetFiscalYearSetup(DateTime VoucherDate)
        {
            string FiscalPeriod = string.Empty;
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                throw new InvalidOperationException("Session expired. Please login again.");
            }
            var FiscalYearSetup = new DALCommon().GetFiscalYearSetup(LoginUser.CompanyID, VoucherDate.Year, VoucherDate.Month);
            if (FiscalYearSetup != null)
            {
                FiscalPeriod = FiscalYearSetup.FiscalYear.ToString() + "-" + FiscalYearSetup.FiscalPeriod.ToString();
            }
            return FiscalPeriod;

        }
        #endregion
    }
}