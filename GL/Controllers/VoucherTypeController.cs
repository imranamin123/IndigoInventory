using GL.DAL;
using GL.EF;
using GL.ViewModels.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GL.Controllers
{
    public class VoucherTypeController : Controller
    {
        #region VoucherType
        [HttpGet]
        public ActionResult VoucherTypeList()
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new VoucherTypeViewModel();
                model.VoucherTypeList = new DALVoucherType().VoucherTypeList();
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult VoucherType(Int64? id)
        {
            try
            {
                VoucherTypeViewModel model = new VoucherTypeViewModel();
                if (id != null || id > 0)
                {
                    model.VoucherType = new DALVoucherType().VoucherTypeGet(id.Value);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult VoucherTypeDelete(Int64 id)
        {
            try
            {
                VoucherTypeViewModel model = new VoucherTypeViewModel();
                Response res = new Response();
                new DALVoucherType().VoucherTypeDelete(id);
                return RedirectToAction("VoucherTypeList");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult VoucherTypeSave(VoucherType VoucherType)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                Response res = new Response();
                if (VoucherType.VoucherTypeID == 0)
                {
                    VoucherType.CompanyID = LoginUser.CompanyID;
                    VoucherType.CreatedAt = DateTime.UtcNow;
                    VoucherType.CreatedBy = LoginUser.UsersID;

                }

                VoucherType.VoucherTypeNo = VoucherType.A1 + "-" + VoucherType.A2 + "-" + VoucherType.A3 + "-" + VoucherType.A4;

                bool result = new DALVoucherType().VoucherTypeSave(VoucherType);
                if (result == true)
                {
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
    }
}