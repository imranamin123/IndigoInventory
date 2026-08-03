using GL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;

using System.Web.Security;
using GL.DAL;
using GL.Security.ViewModels;
using GL.Models;


namespace GL.Controllers
{
    //[Authorize]
    public class SecurityController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(SecUser model)
        {
            try
            {
                //if(IsExpired())
                //{
                //    ModelState.AddModelError("TrialExpired","Your trial period is expired. Please contact to the vendor");
                //    return View(model);
                //}                

                DALSecurity dal = new DALSecurity();
                SecUser user = dal.ValidateUser(model.username, model.password);

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.username, false);

                    spLoginUser_Result LoginUser = dal.GetLoginUser(user.CompanyID.Value, user.UsersID);

                    Session["LoginUser"] = LoginUser;// new CommonDAL().GetLoginUser(user.UsersID);
                    Session["MenuModules"] = new DALSecurity().GetMenuModules(LoginUser.CompanyID, LoginUser.UsersID).Where(x => x.IsViewAllowed != null).ToList(); // new DALUserManagement().GetModules();
                    Session["MenuPages"] = new DALSecurity().GetMenuPages(LoginUser.CompanyID, LoginUser.UsersID);

                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "invalid email or password");
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Signout()
        {
            try
            {
                FormsAuthentication.SignOut();
                Session["LoginUser"] = null;
                return RedirectToAction("Login", "Security");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult GetPartialLayoutHeaderView()
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    throw new InvalidOperationException("Session expired. Please login again.");
                }
                return PartialView("_headerView", LoginUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region PageRights
        public ActionResult MenuRightsView()
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                MenuViewModel model = new MenuViewModel();
                DALDropdowns dal = new DALDropdowns();
                model.MenuPagesRightsList = new List<spMenuPagesRightsList_Result>();
                var dalDropdowns = new DAL.DALDropdowns();
                if (LoginUser.RoleID == 1)
                {
                    ViewBag.Companies = dalDropdowns.CompanyList().Where(x => x.CompanyID == LoginUser.CompanyID || LoginUser.RoleID == 1).ToList();
                }
                else if (LoginUser.RoleID == 2)
                {
                    ViewBag.Companies = dalDropdowns.CompanyList().Where(x => x.CompanyID == LoginUser.CompanyID).ToList();
                }
                else
                    ViewBag.Companies = new List<Company>();

                //ViewBag.Users = dalDropdowns.UsersList(LoginUser.SchoolID);

                return View("MenuRightsView", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult MenuRightsRowsView(SearchModel searchModel)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            MenuViewModel model = new MenuViewModel();

            model.CompanyID = searchModel.CompanyID.Value;
            model.UserID = searchModel.UserID;
            model.MenuPagesRightsList = new DALSecurity().GetMenuPageRightsList(searchModel.CompanyID.Value, searchModel.UserID);
            return View("_MenuRightsViewRows", model);

        }

        [HttpGet]
        public ActionResult CreateUserPageRights(SearchModel searchModel)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            MenuViewModel model = new MenuViewModel();
            DALDropdowns dal = new DALDropdowns();
            ViewBag.CompanyName = dal.CompanyList().Where(x => x.CompanyID == searchModel.CompanyID).FirstOrDefault().Name;
            ViewBag.Login = dal.UsersList(searchModel.CompanyID.Value).Where(x => x.UsersID == searchModel.UserID).FirstOrDefault().username;
            ViewBag.UserName = dal.UsersList(searchModel.CompanyID.Value).Where(x => x.UsersID == searchModel.UserID).FirstOrDefault().Name;
            model.UserPagesRights = new DALSecurity().GetUserPagesRights(Convert.ToInt16(searchModel.CompanyID.Value), searchModel.UserID);
            //foreach (var item in model.UserPagesRights)
            //{
            //    if (item.PageRightsID == null)
            //        item.PageRightsID = 0;
            //}
            //model.UserPagesRights = new DALSecurity().GetUserPagesRights2(searchModel.SchoolID, searchModel.UserID);
            return View("CreateUserPageRights", model);
        }

        [HttpGet]
        public ActionResult myPost()
        {
            response res = new response();
            res.status = true;
            res.resMessage = "Records Saved Successfully!";
            res.resObj = null;
            return Json(res, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult CreateUserPageRightsSave(List<SecPagesRight> SecPagesRights)
        {
            try
            {
                response res = new response();
                new DALSecurity().UserPagesRightsSave(SecPagesRights);
                res.status = true;
                res.resMessage = "Records Saved Successfully!";
                res.resObj = null;
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region Common

        [HttpGet]
        public string RefreshSession() {
            return "Refresh Called";
        }
        
        #endregion


    }
}