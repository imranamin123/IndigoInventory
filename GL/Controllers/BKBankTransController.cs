using Azure;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.ViewModels.BKBankTrans;
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
    public class BKBankTransController : Controller
    {
        #region BKBankTrans
        [HttpGet]
        public ActionResult BKBankTranList()
        {
            try
            {
                var loginUser = (spLoginUser_Result)Session["LoginUser"];
                if (loginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new BKBankTransViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult BKBankTranSearchList(SearchModel search)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new BKBankTransViewModel();
                //DALDropdowns dalDropdowns = new DALDropdowns();

                //ViewBag.VoucherTypes = dalDropdowns.VoucherTypeList();

                model.BKBankTransSearchList = new DALBKBankTrans().GetBKBankTransSearchList(LoginUser.CompanyID);
                return View("_BKBankTranSearchList", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult BKBankTran(int? BKBankTranID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                BKBankTransViewModel model = new BKBankTransViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                if (LoginUser.RoleID == 1)
                {
                    ViewBag.Companies = dalDropdowns.CompanyList();
                }
                else
                {
                    ViewBag.companies = dalDropdowns.CompanyList().Where(x => x.CompanyID == LoginUser.CompanyID);
                }


                if (LoginUser.UsersID == 1 || LoginUser.UsersID == 12 || LoginUser.UsersID == 41 || LoginUser.UsersID == 50 || LoginUser.UsersID == 51) {
                    ViewBag.IsVendor = 1;
                }
                


                ViewBag.APVendors = dalDropdowns.GLAPVendorsListWithCode(LoginUser.CompanyID);
                ViewBag.Banks = dalDropdowns.BKBankList(LoginUser.CompanyID);
                ViewBag.BKBankTransTypes = dalDropdowns.BKBankTransTypeList();
                ViewBag.IsPost = new List<SelectListItem>
                                {
                                    new SelectListItem { Text = "Post", Value = "1" },
                                    new SelectListItem { Text = "Unpost", Value = "0" },
                                };


                if (BKBankTranID != null || BKBankTranID > 0)
                {
                    model.BKBankTran = new DALBKBankTrans().GetBKBankTrans(BKBankTranID.Value);
                    model.GLBKBankTransDetailRows = new DALBKBankTrans().GetBKBankTransDetailRows(BKBankTranID.Value);
                    int count = 0;
                    foreach (var item in model.GLBKBankTransDetailRows)
                    {
                        if (item.IsPost == 1)
                        {
                            count++;
                        }
                    }
                    ViewBag.IsPostCount = count;
                }
                else
                {
                    model.BKBankTran = new BKBankTran();
                    model.BKBankTran.BankTransCode = string.Empty;
                    model.BKBankTran.TransDate = DateTime.Now;
                    model.BKBankTran.BankTransID = 0;
                    model.BKBankTran.IsPosted = false;
                    
                    //ViewBag.IsPosted = false ;
                    model.GLBKBankTransDetailRows = new List<spBKBankTransDetailRows_Result>();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult IsBKBankTranExist(int CompanyID,int BankTransID, int BankID, DateTime TransDate) {
            
            GL.Models.response res = new GL.Models.response();
            var model = new DALBKBankTrans().GetBKBankTransIfExist(CompanyID, BankTransID, BankID, TransDate);

            if (model != null)
            {
                res.id = model.BankTransID;
                res.status = true;
                res.resMessage = "Record found!";
            }

            else
            {
                res.id = 0;
                res.status = false;
                res.resMessage = "Record not found!";

            }

            return Json(res, JsonRequestBehavior.AllowGet);

        }


    [HttpGet]
        public ActionResult BKBankTranDelete(int BKBankTranID)
        {
            try
            {
                //BKBankTransViewModel model = new BKBankTransViewModel();

                new DALBKBankTrans().BKBankTranDelete(BKBankTranID);
                return RedirectToAction("BKBankTranList");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BKBankTransDetailDelete(int id)
        {
            try
            {
                new DALBKBankTransDetail().BKBankTransDetailDelete(id);
                return RedirectToAction("BKBankTran");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult BKBankTranSave(BKBankTran BKBankTrans, List<BKBankTransDetail> BKBankTransDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();
                if (BKBankTrans.BankTransID == 0)
                {

                    BKBankTrans.CreatedAt = DateTime.Now;
                    BKBankTrans.CreatedBy = LoginUser.UsersID;
                    BKBankTrans.ModifiedAt = DateTime.Now;
                    BKBankTrans.ModifiedBy = LoginUser.UsersID;
                    BKBankTrans.CompanyID = LoginUser.CompanyID;
                }

                
                BKBankTrans.ModifiedAt = DateTime.Now;
                BKBankTrans.ModifiedBy = LoginUser.UsersID;


                bool result = new DALBKBankTrans().BKBankTranSave(BKBankTrans);


                if (result == true)
                {
                    // save details
                    if (BKBankTransDetailList != null && BKBankTransDetailList.Count > 0)
                    {
                        foreach (var item in BKBankTransDetailList)
                        {
                            item.BankTransID = BKBankTrans.BankTransID;

                            if (item.BankTransDetID == 0)
                            {
                                item.CreatedBy = LoginUser.UsersID;
                                item.CreatedAt = DateTime.Now;
                                item.CompanyID = LoginUser.CompanyID;
                            }

                            
                            item.ModifiedBy = LoginUser.UsersID;
                            item.ModifiedAt = DateTime.Now;


                            new DALBKBankTransDetail().BKBankTransDetailSave(item);
                        }
                    }
                    // end save details

                    //res.resObj = BKBankTrans;
                    res.id = BKBankTrans.BankTransID;
                    res.status = true;
                    res.resMessage = "Record save successfully!";
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion

        #region functions

        public string GetBankTransTypeDescription(int id)
        {
            var BankTransTypeDescription = new DALDropdowns().BKBankTransTypeList().Where(x => x.BankTransTypeID == id).FirstOrDefault().BankTransTypeDescription;
            return BankTransTypeDescription;
        }

        #endregion
    }
}