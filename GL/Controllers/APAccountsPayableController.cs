using DAL;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.ViewModels.AP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace GL.Controllers
{
    public class APAccountsPayableController : Controller
    {

        #region Vendor
        [HttpGet]
        public ActionResult VendorList()
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new APVendorViewModel();
                model.APVendorList = new DALAPAccountsPayable().APVendorList(LoginUser.CompanyID);

                ViewBag.roleid = LoginUser.RoleID;
                ViewBag.username = LoginUser.username;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult Vendor(Int64? id)
        {
            try
            {
                APVendorViewModel model = new APVendorViewModel();
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                DALDropdowns dalDropdowns = new DALDropdowns();
                ViewBag.APVendorCategories = dalDropdowns.GetAPVendorCategoryDropdown(LoginUser.CompanyID).OrderBy(x => x.APVendorCategoryName).ToList();
                if (id != null || id > 0)
                {
                    model.APVendor = new DALAPAccountsPayable().APVendorGet(id.Value);
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult VendorDelete(Int64 id)
        {
            try
            {
                APVendorViewModel model = new APVendorViewModel();
                response res = new response();
                new DALAPAccountsPayable().APVendorDelete(id);
                return RedirectToAction("APVendorList");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult VendorSave(APVendor Vendor)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                response res = new response();

                DALAPAccountsPayable dal = new DALAPAccountsPayable();

                if (dal.IsAPVendorCodeExist(Vendor) == false)
                {
                    if (Vendor.APVendorID == 0)
                    {
                        Vendor.CreatedBy = LoginUser.UsersID;
                        Vendor.CreatedAt = DateTime.Now;
                        Vendor.CompanyID = LoginUser.CompanyID;
                    }

                    Vendor.ModifiedBy = LoginUser.UsersID;
                    Vendor.ModifiedAt = DateTime.Now;

                    bool result = new DALAPAccountsPayable().APVendorSave(Vendor);
                    if (result == true)
                    {
                        res.status = true;
                        res.resMessage = "Record saved successfully!";
                    }
                }
                else
                {
                    res.status = false;
                    res.resMessage = "Vendor code already exists";
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