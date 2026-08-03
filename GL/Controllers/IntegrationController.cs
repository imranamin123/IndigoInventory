using Azure;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.ReportsWebForms;
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
    public class IntegrationController : Controller
    {


        [HttpPost]
        public JsonResult IntDVReceiptVoucher(int DVReceiptID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
            }

            GL.Models.response res = new GL.Models.response();
            bool result = false;
            DALIngegration dal = new DALIngegration();
            result = dal.IntDVReceiptVoucher(DVReceiptID, LoginUser.CompanyID);

            if (result == true)
            {
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

        [HttpPost]
        public JsonResult IntBankTransactionVoucher(int bkBankTransID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
            }

            GL.Models.response res = new GL.Models.response();
            bool result = false;
            DALIngegration dal = new DALIngegration();
            result = dal.IntBankTransactionVoucher(bkBankTransID, LoginUser.CompanyID);

            if (result == true)
            {
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

        [HttpPost]
        public JsonResult IntAPInvoice(Int64 APInvoiceID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
            }

            GL.Models.response res = new GL.Models.response();
            bool result = false;
            DALIngegration dal = new DALIngegration();
            result = dal.IntAPInvoice(APInvoiceID, LoginUser.CompanyID);

            if (result == true)
            {
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


        [HttpPost]
        public JsonResult INGoodRreceiptNotePost(List<INGoodsReceiptNoteDetail> INGoodsReceiptNoteDetails)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }

                GL.Models.response res = new GL.Models.response();
                bool result = false;
                DALInventory dal = new DALInventory();

                result = dal.GRNPost(INGoodsReceiptNoteDetails);

                if (result == true)
                {
                    res.status = true;
                    res.resMessage = "Record posted successfully!";
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

        //[HttpPost]
        //public JsonResult INStoreIssueNotePost(List<INStoreIssueNoteDetail> INStoreIssueNoteDetails)
        //{
        //    try
        //    {
        //        var LoginUser = (spLoginUser_Result)Session["LoginUser"];

        //        GL.Models.response res = new GL.Models.response();
        //        bool result = false;
        //        DALInventory dal = new DALInventory();

        //        dal.StoreIssueNotePost(INStoreIssueNoteDetails);

        //        if (result == true)
        //        {
        //            res.status = true;
        //            res.resMessage = "Record posted successfully!";
        //        }
        //        else
        //        {
        //            res.resObj = null;
        //            res.status = false;
        //            res.resMessage = "Record was not saved. Please contact to the admin!";

        //        }

        //        return Json(res, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet]
        public ActionResult INGoodRreceiptNoteUnPost(long id)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }

                GL.Models.response res = new GL.Models.response();
                bool result = false;
                DALInventory dal = new DALInventory();

                dal.GRNUnPost(id);

                //if (result == true)
                //{
                //    res.status = true;
                //    res.resMessage = "Record posted successfully!";
                //}
                //else
                //{
                //    res.resObj = null;
                //    res.status = false;
                //    res.resMessage = "Record was not saved. Please contact to the admin!";

                //}
                return RedirectToAction("GoodsReceiptNoteList","INInventory");
                //return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}