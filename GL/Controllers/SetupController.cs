using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GL.EF;
using GL.ViewModels;
using GL.ViewModels.Setup;
using DAL;
using GL.Models;
using GL.DAL;
using Azure;


namespace GL.Controllers
{
    public class SetupController : Controller
    {
        // GET: Setup
        public SetupController()
        {

        }

        #region Company
        [HttpGet]
        public ActionResult CompanyList()
        {
            try
            {               

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new APCompanyViewModel();
                model.CompanyList = new DALSetup().CompanyList();

                if (LoginUser.RoleID == 1)
                {
                    ViewBag.IsSuperAdmin = true;
                    ViewBag.activeSessionCount = SessionManager.GetActiveSessionCount();
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult Company(Int64? id)
        {
            try
            {
                var model = new APCompanyViewModel();
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                if (id != null || id > 0)
                {
                    model.Company = new DALSetup().CompanyGet(id.Value);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult CompanyDelete(Int64 id)
        {
            try
            {
                var model = new APCompanyViewModel();
                response res = new response();
                new DALSetup().CompanyDelete(id);
                return RedirectToAction("CompanyList");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult CompanySave(Company Company)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                response res = new response();
                //if (Company.CompanyID == 0)
                //{
                //    Company.CompanyID =  LoginUser.CompanyID;
                //}

                bool result = new DALSetup().CompanySave(Company);
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

        #region FiscalYearSetup

        [HttpGet]
        public ActionResult FiscalYearSetup(int? CompanyID)
        {
            try
            {
                
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                int companyID = 0;
                FiscalYearSetupViewModel model = new FiscalYearSetupViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();
                DALSetup dalSetup = new DALSetup();
                ViewBag.MonthList = new DALDropdowns().MonthList();
                ViewBag.RoleID = LoginUser.RoleID;
                ViewBag.IsUpdated = "0";

                if (CompanyID == null) { 
                    companyID = LoginUser.CompanyID;
                }
                else
                {
                    companyID = CompanyID.Value;
                }

                if (LoginUser.RoleID == 1)
                { // super admin
                    model.FiscalYearSetup = dalSetup.FiscalYearSetupInitialGet(companyID);
                    if (model.FiscalYearSetup.FiscalYearSetupID > 0)
                    {
                        ViewBag.IsUpdated = "1";
                    }
                    else
                    {
                        ViewBag.IsUpdated = "0";
                    }
                    
                    ViewBag.CompanyList = dalDropdowns.CompanyList().ToList();
                }
                else if (LoginUser.RoleID == 2)
                {
                    ViewBag.CompanyList = dalDropdowns.CompanyList().Where(x => x.CompanyID == LoginUser.CompanyID).ToList();
                }
                else
                    ViewBag.CompanyList = new List<Company>();

                ViewBag.CompanyID = companyID;
                
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult FiscalYearSetupLoad(FiscalYearSetup fiscalYearSetup)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                FiscalYearSetupViewModel model = new FiscalYearSetupViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();
                DALSetup dalSetup = new DALSetup();
                response res = new response();
                // ViewBag.MonthList = new DALDropdowns().MonthList();
                // ViewBag.RoleID = LoginUser.RoleID;

                if (LoginUser.RoleID == 1)
                { // super admin
                    model.FiscalYearSetup = dalSetup.FiscalYearSetupInitialGet(LoginUser.CompanyID);
                    ViewBag.CompanyList = dalDropdowns.CompanyList().ToList();
                    res.status = true;
                    res.resObj = model.FiscalYearSetup;
                }
                else if (LoginUser.RoleID == 2)
                {
                  //  ViewBag.CompanyList = dalDropdowns.CompanyList().Where(x => x.CompanyID == LoginUser.CompanyID).ToList();
                }
                else
                    ViewBag.CompanyList = new List<Company>();


                return Json(res); // iew(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ////[HttpGet]
        ////public ActionResult FiscalYearSetup()
        ////{
        ////    try
        ////    {
        ////        var LoginUser = (spLoginUser_Result)Session["LoginUser"];
        ////        FiscalYearSetupViewModel model = new FiscalYearSetupViewModel();
        ////        DALDropdowns dalDropdowns = new DALDropdowns();
        ////        DALSetup dalSetup = new DALSetup();
        ////        ViewBag.MonthList = new DALDropdowns().MonthList();
        ////        ViewBag.RoleID = LoginUser.RoleID;

        ////        if (LoginUser.RoleID == 1)
        ////        { // super admin
        ////            model.FiscalYearSetup = dalSetup.FiscalYearSetupInitialGet(LoginUser.CompanyID);
        ////            ViewBag.CompanyList = dalDropdowns.CompanyList().ToList();
        ////        }
        ////        else if (LoginUser.RoleID == 2)
        ////        {
        ////            ViewBag.CompanyList = dalDropdowns.CompanyList().Where(x => x.CompanyID == LoginUser.CompanyID).ToList();
        ////        }
        ////        else
        ////            ViewBag.CompanyList = new List<Company>();


        ////        return View(model);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw ex;
        ////    }
        ////}

        [HttpPost]
        public JsonResult FiscalYearSetupSave(FiscalYearSetup fiscalYearSetup)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                response res = new response();
                DALSetup dal = new DALSetup();

                if (dal.IsFiscalYearSetupExist(fiscalYearSetup) == false)
                {
                    //if (fiscalYearSetup.FiscalYearSetupID == 0)
                    //{
                    //    //SecUser.SchoolID = LoginUser.SchoolID;
                    //    SecUser.CreatedAt = DateTime.Now;
                    //    SecUser.CreatedBy = LoginUser.UsersID;
                    //}

                    fiscalYearSetup.CreatedAt = DateTime.Now;
                    fiscalYearSetup.CreatedBy = LoginUser.UsersID;
                    //fiscalYearSetup.CompanyID = LoginUser.CompanyID;

                    bool result = new DALSetup().FiscalYearSetupSave(fiscalYearSetup);
                    if (result == true)
                    {
                        res.status = true;
                        res.resMessage = "Record saved successfully!";
                    }
                }
                else
                {
                    res.status = false;
                    res.resMessage = "Username already exists";
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region SecUser
        [HttpGet]
        public ActionResult SecUserList()
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                SecUserViewModel model = new SecUserViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.StatusTypes = new DALDropdowns().StatusTypeList();
                if (LoginUser.RoleID == 1) // admin
                {
                    ViewBag.CompanyList = dalDropdowns.CompanyList();
                }
                else if (LoginUser.RoleID == 2)
                {
                    ViewBag.CompanyList = dalDropdowns.CompanyList().Where(x => x.CompanyID == LoginUser.CompanyID).ToList();
                }
                else
                    ViewBag.CompanyList = new List<Company>();
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult SecUserSearchList(SearchModel search)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            SecUserViewModel model = new SecUserViewModel();
            if (LoginUser.RoleID == 1 && search.CompanyID == null)
            {
                search.CompanyID = null;
            }
            else if(search.CompanyID == null)
            {
                search.CompanyID = LoginUser.CompanyID;
            }


            model.SecUserSearchList = new DALSetup().GetSecUserGetSearchList(search);
            return View("_SecUserSearchListRows", model);

        }

        [HttpGet]
        public ActionResult SecUser(Int64? id)
        {
            try
            {
                SecUserViewModel model = new SecUserViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }

                if (id != null || id > 0)
                {
                    model.SecUser = new DALSetup().SecUserGet(id.Value);
                }
                else
                {
                    model.SecUser.CreatedAt = DateTime.Now;
                }

                if (LoginUser.RoleID == 1)
                {
                    ViewBag.CompanyList = dalDropdowns.CompanyList().ToList();
                }
                else if (LoginUser.RoleID == 2)
                {
                    ViewBag.CompanyList = dalDropdowns.CompanyList().Where(x => x.CompanyID == LoginUser.CompanyID).ToList();
                }
                else
                    ViewBag.CompanyList = new List<Company>();

                ViewBag.StatusTypes = dalDropdowns.StatusTypeList();
                ViewBag.Rolls = dalDropdowns.RolesList();
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult SecUserDelete(Int64 id)
        {
            try
            {
                SecUserViewModel model = new SecUserViewModel();
                response res = new response();
                new DALSetup().SecUserDelete(id);
                return RedirectToAction("SecUserList");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult SecUserSave(SecUser SecUser)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                response res = new response();
                DALSetup dal = new DALSetup();

                if (dal.IsUserExist(SecUser) == false)
                {
                    if (SecUser.UsersID == 0)
                    {
                        //SecUser.SchoolID = LoginUser.SchoolID;
                        SecUser.CreatedAt = DateTime.Now;
                        SecUser.CreatedBy = LoginUser.UsersID;
                    }

                    bool result = new DALSetup().SecUserSave(SecUser);
                    if (result == true)
                    {
                        res.status = true;
                        res.resMessage = "Record saved successfully!";
                    }
                }
                else
                {
                    res.status = false;
                    res.resMessage = "Username already exists";
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult GetUserList(int? CompanyID)
        {
            try
            {

                DALDropdowns dal = new DALDropdowns();
                response res = new response();
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                if (LoginUser.RoleID != 1)
                    CompanyID = LoginUser.CompanyID;

                List<SecUser> UserList = new DALDropdowns().UserList(CompanyID.Value);
                if (UserList != null && UserList.Count > 0)
                {
                    res.status = true;
                    res.resMessage = "Records found";
                    res.resObj = (from a in UserList
                                  select new { UsersID = a.UsersID, username = a.username }).ToList();
                }
                else
                {
                    res.status = false;
                    res.resMessage = "Records not found";
                    res.resObj = null;

                }
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region INProject

        [HttpPost]
        public ActionResult INProjectPost(int ProjectID)
        {
            try
            {

                bool result = new DALSetup().INProjectPost(ProjectID);

                //res.resObj = glVoucher;
                return RedirectToAction("INProject", ProjectID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult INProjectList()
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new INProjectViewModel();
                model.INProjectList = new DALSetup().INProjectList();

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult INProject(Int64? id)
        {
            try
            {
                var model = new INProjectViewModel();
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                if (id != null || id > 0)
                {
                    model.INProject = new DALSetup().INProjectGet(id.Value);
                }
                else
                {
                    model.INProject.CompanyID = LoginUser.CompanyID;
                    model.INProject.ProjectID = 0;
                    model.INProject.ProjectName = string.Empty;
                    model.INProject.ProjectDescription = string.Empty;
                    model.INProject.Posted = false;

                }
                    return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult INProjectDelete(Int64 id)
        {
            try
            {
                var model = new INProjectViewModel();
                response res = new response();
                new DALSetup().INProjectDelete(id);
                return RedirectToAction("INProjectList");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult INProjectSave(INProject INProject)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                response res = new response();
                //if (INProject.INProjectID == 0)
                //{
                //    INProject.INProjectID =  LoginUser.INProjectID;
                //}

                bool result = new DALSetup().INProjectSave(INProject);
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