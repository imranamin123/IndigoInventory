using DAL;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.ViewModels.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GL.Controllers
{
    public class GLAccountController : Controller
    {
        #region GLAccount Action Methods
        [HttpGet]
        public ActionResult GLAccountList()
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new GLAccountViewModel();
                model.GLAccountList = new DALGLAccount().GLAccountList(LoginUser.CompanyID);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult GLAccount(Int64? id)
        {
            try
            {
                GLAccountViewModel model = new GLAccountViewModel();
                if (id != null || id > 0)
                {
                    model.GLAccount = new DALGLAccount().GLAccountGet(id.Value);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult GLAccountDelete(Int64 id)
        {
            try
            {
                GLAccountViewModel model = new GLAccountViewModel();
                response res = new response();
                new DALGLAccount().GLAccountDelete(id);
                return RedirectToAction("GLAccountList");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GLAccountSave(GLAccount GLAccount)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                response res = new response();
                if (GLAccount.GLAccountID == 0)
                {
                    GLAccount.CompanyID = LoginUser.CompanyID;
                    GLAccount.CreatedAt = DateTime.Now;
                    GLAccount.CreatedBy = LoginUser.UsersID;
              
                }
                else
                {
                    GLAccount.CompanyID = LoginUser.CompanyID;

                }

                GLAccount.GLAccountNo = GLAccount.A1 + "-" + GLAccount.A2 + "-" + GLAccount.A3 + "-" + GLAccount.A4;

                bool IsDuplicate = new DALGLAccount().IsDuplicateGLAccount(LoginUser.CompanyID, GLAccount.GLAccountID, GLAccount.GLAccountNo);
                if (IsDuplicate == true)
                {
                    res.status = false;
                    res.resMessage = "GL Account No already exists";
                }
                else
                {

                    bool result = new DALGLAccount().GLAccountSave(GLAccount);
                    if (result == true)
                    {
                        res.status = true;
                        res.resMessage = "Record save successfully!";
                    }
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