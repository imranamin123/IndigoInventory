using Newtonsoft;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.ViewModels.BKBankTrans;
using GL.ViewModels.BS;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace GL.Controllers
{
    public class BSBankStatementController : Controller
    {
        // GET: BSBankStatement
        public ActionResult BSClassification()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.BKBanks = dal.BKBankList(LoginUser.CompanyID);
            return View();
        }


        #region BSClassification
        [HttpGet]
        public ActionResult BSClassificationList()
        {
            try
            {
                var loginUser = (spLoginUser_Result)Session["LoginUser"];
                if (loginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new BSClassificationViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.Banks = dalDropdowns.BKBankList(loginUser.CompanyID);

                model.BSClassification = new BSClassification();

                model.BSClassification.BSClassificationID = 0;
                model.BSClassification.BSClassificationDate = DateTime.Now;
                model.BSClassification.TransactionID = 0;
                model.BSClassification.BSBankStatementBalance = 0;
                model.BSClassification.BKBankID = 0;

                model.BSClassification.CreatedAt = DateTime.Now;
                model.BSClassification.CreatedBy = loginUser.UsersID;
                model.BSClassification.ModifiedAt = DateTime.Now;
                model.BSClassification.ModifiedBy = loginUser.UsersID;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult BSBankStatementPendingUnpendingSearchList(int BKBankID, bool IsPending, DateTime ValueDateFrom, DateTime ValueDateTo)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new BSClassificationViewModel();

                model.BSBankStatementPendingUnpendingList = new DALBSClassification().GetBSBankStatementPendingUnpendingList(LoginUser.CompanyID, BKBankID, IsPending, ValueDateFrom, ValueDateTo);
                return View("_BSBankStatementPendingUnpendingList", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult BSClassificationSearchList(SearchModel search)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new BSClassificationViewModel();
                //DALDropdowns dalDropdowns = new DALDropdowns();

                //ViewBag.VoucherTypes = dalDropdowns.VoucherTypeList();

                model.BSClassificationSearchList = new DALBSClassification().GetBSClassificationSearchList(LoginUser.CompanyID);
                return View("_BSClassificationSearchList", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult BSClassification(int? TransactionID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                BSClassificationViewModel model = new BSClassificationViewModel();

                DALDropdowns dalDropdowns = new DALDropdowns();
                DALBSClassification dalClassification = new DALBSClassification();

                ViewBag.BSClassificationTypes = dalDropdowns.BSClassificationTypeList(LoginUser.CompanyID);

                if (TransactionID != null || TransactionID > 0)
                {
                    model.BSClassificationHeader = new DALBSClassification().GetBSClassificationHeader(TransactionID.Value);
                    model.BSBankStatement = new DALBSClassification().GetBSBankStatement(TransactionID.Value);
                    model.BSClassificationListModel = dalClassification.GetBSClassificationList(TransactionID.Value);
                }
                else
                {

                    model.BSClassificationHeader.TransactionID = 0;
                    model.BSClassificationHeader.BKBankID = 0;
                    model.BSClassificationHeader.TransactionDate = DateTime.Now;
                    model.BSClassificationHeader.ValueDate = DateTime.Now;
                    model.BSClassificationHeader.TransactionReferenceNo = "";
                    model.BSClassificationHeader.Description = "";
                    model.BSClassificationHeader.Debit = 0;
                    model.BSClassificationHeader.Credit = 0;
                    model.BSClassificationHeader.Balance = 0;
                    model.BSClassificationHeader.Amount = 0;
                    model.BSClassificationHeader.BSClassificationID = 0;
                    model.BSClassificationHeader.BSClassificationDate= DateTime.Now;
                    model.BSClassificationHeader.BSBankStatementBalance = 0;
                    model.BSClassificationHeader.CreatedAt = DateTime.Now;
                    model.BSClassificationHeader.CreatedBy = LoginUser.UsersID;
                    model.BSClassificationHeader.ModifiedAt = DateTime.Now;
                    model.BSClassificationHeader.ModifiedBy = LoginUser.UsersID;
                    model.BSClassificationHeader.CompanyID =LoginUser.CompanyID;


                    model.BSClassificationListModel = new List<spBSClassificationList_Result>();
                    model.BSBankStatementPendingUnpendingList = new List<spBSBankStatementPendingUnpendingList_Result>();

                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult BSClassificationSave(BSClassification BSClassification, List<BSClassificationDetail> BSClassificationDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                DALBSClassification dal = new DALBSClassification();

                GL.Models.response res = new GL.Models.response();

                if (BSClassification.BSClassificationID == 0)
                {
                    BSClassification.CompanyID = LoginUser.CompanyID;
                    BSClassification.CreatedAt = DateTime.Now;
                    BSClassification.CreatedBy = LoginUser.UsersID;
                }

                BSClassification.ModifiedAt = DateTime.Now;
                BSClassification.ModifiedBy = LoginUser.UsersID;


                bool result = new DALBSClassification().BSClassificationSave(BSClassification);


                if (result == true)
                {
                    // save details
                    if (BSClassificationDetailList != null && BSClassificationDetailList.Count > 0)
                    {
                        foreach (var item in BSClassificationDetailList)
                        {
                            item.BSClassificationID = BSClassification.BSClassificationID;

                            if (item.BSClassificationDetailID == 0)
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
                            new DALBSClassification().BSClassificationDetailSave(item);
                        }
                    }
                    // end save details

                    res.resObj = BSClassification;
                    res.id = BSClassification.TransactionID.Value;
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

        [HttpPost]
        public JsonResult GetBSClassification(int TransactionID)
        {
            try
            {
                DALBSClassification dal = new DALBSClassification();
                BSClassificationModel model = new BSClassificationModel();
                var details = dal.GetBSClassification(TransactionID);
                if (details != null)
                {

                    model.BKBankID = details.BKBankID.GetValueOrDefault(0);
                    model.BSBankStatementBalance = details.BSBankStatementBalance.GetValueOrDefault(0); ;
                    model.BSClassificationDate = details.BSClassificationDate.GetValueOrDefault(new DateTime());
                    model.BSClassificationID = details.BSClassificationID;
                    model.TransactionID = details.TransactionID.GetValueOrDefault(0); ;
                    model.ValueDate = details.ValueDate.GetValueOrDefault(new DateTime());
                    model.CompanyID = details.CompanyID.GetValueOrDefault(0);
                    model.CreatedAt = details.CreatedAt.GetValueOrDefault(new DateTime());
                    model.CreatedBy = details.CreatedBy.GetValueOrDefault(0);
                    model.ModifiedAt = details.ModifiedAt.GetValueOrDefault(new DateTime());
                    model.ModifiedBy = details.ModifiedBy.GetValueOrDefault(0);

                }


                //return this.Json(Results , JsonRequestBehavior.AllowGet);
                return this.Json(Newtonsoft.Json.JsonConvert.SerializeObject(model), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GetBSClassificationList(int TransactionID)
        {
            try
            {
                DALBSClassification dal = new DALBSClassification();
                List<BSClassificationDetailModel> modelList = new List<BSClassificationDetailModel>();
                GL.Models.response res = new GL.Models.response();

                var details = dal.GetBSClassificationDetailList(TransactionID);

                if (details != null && details.Count > 0)
                {
                    foreach (var item in details)
                    {
                        var det = new BSClassificationDetailModel();

                        det.BSClassificationDetailID = item.BSClassificationDetailID;
                        det.BSClassificationID = item.BSClassificationID;
                        det.BSClassificationTypeID = item.BSClassificationTypeID;
                        det.TransactionID = item.TransactionID;
                        det.Balance = item.Balance;
                        det.CompanyID = item.CompanyID;
                        det.CreatedAt = item.CreatedAt;
                        det.CreatedBy = item.CreatedBy;
                        det.ModifedAt = item.ModifiedAt;
                        det.ModifiedBy = item.ModifiedBy;

                        modelList.Add(det);

                    }

                }

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(modelList), "application/json");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BSClassificationDetailDelete(int id)
        {
            try
            {
                new DALBSClassification().BSClassificationDetailDelete(id);// DALBSClassificationDetail().BSClassificationDetailDelete(id);
                return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}