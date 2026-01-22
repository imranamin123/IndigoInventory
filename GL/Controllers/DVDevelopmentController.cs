using DAL;
using GL.EF;
using GL.Models;
using GL.ReportsWebForms;
using GL.ViewModels.BKBankTrans;
using GL.ViewModels.DV;
using GL.ViewModels.Setup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GL.DAL
{
    public class DVDevelopmentController : Controller
    {

        #region ApplicationForm

        public ActionResult ApplicationForm(int? id)
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];

                var ApplicationForm = new ApplicationFormViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.Projects = dalDropdowns.DVProjectsList(LoginUser.CompanyID);
                ViewBag.UnitTypes = dalDropdowns.DVUnitTypeList(LoginUser.CompanyID);
                ViewBag.Genders = dalDropdowns.GenderList();
                ViewBag.Provinces = dalDropdowns.ProvinceList();
                ViewBag.DVArrangementTypes = dalDropdowns.GetDVArrangementTypesDropdown(LoginUser.CompanyID);
                ApplicationFormViewModel model = new ApplicationFormViewModel();

                spDVGetApplicationForm_Result applicationForm = new spDVGetApplicationForm_Result();
                DVUnit unit = new DVUnit();
                var applicants = new List<spDVApplicantRows_Result>();

                if (id != null)
                {
                    applicationForm = new DALDevelopment().spDVApplicationForm(id);
                    applicants = new DALDevelopment().GetDVApplicants(id).ToList();
                    ViewBag.Units = dalDropdowns.DVUnitsListByProjectID(applicationForm.ProjectID.Value, false, applicationForm.UnitID.Value);// dalDropdowns.DVUnitsList(applicationForm);
                }
                else
                {
                    applicationForm.ApplicationFormDate = DateTime.Now;
                    applicationForm.ApplicationFormID = 0;
                    applicationForm.ApplicationFormNo = string.Empty;
                    applicationForm.CompanyID = 0;
                    applicationForm.CreatedAt = DateTime.Now;
                    applicationForm.CreatedBy = 0;
                    applicationForm.FloorNo = string.Empty;
                    applicationForm.ModifiedAt = DateTime.Now;
                    applicationForm.ModifiedBy = 0;
                    applicationForm.Price = 0;
                    applicationForm.TokenMoney = 0;
                    applicationForm.ProjectID = 0;
                    applicationForm.ProjectName = string.Empty;
                    applicationForm.SQFT = 0;
                    applicationForm.UnitID = 0;
                    applicationForm.UnitNo = string.Empty;
                    applicationForm.UnitTypeID = 0;
                    applicationForm.UnitTypeName = string.Empty;
                    applicationForm.IsActive = true;
                    applicationForm.Cancelled = false;
                    applicationForm.A4=string.Empty;


                    ViewBag.Units = new List<DVUnit>();//  dalDropdowns.DVUnitsList(LoginUser.CompanyID, applicationForm.ApplicationFormID, applicationForm.UnitID.Value);
                }

                model.spDVGetApplicationForm = applicationForm;
                model.RoleID = new DALSecurity().GetLoginUser(LoginUser.CompanyID, LoginUser.UsersID).RoleID;
                model.DVApplicantRows = applicants;

                ViewBag.Amount = applicationForm.Price * applicationForm.SQFT;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetUnitData(int unitID)
        {
            try
            {
                DALDropdowns dal = new DALDropdowns();
                var unit = dal.GetDVUnitsRows(unitID);

                return Json(unit, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult IsA4Duplicate(int ProjectID, int ApplicationFormID, string A4)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var dal = new DALDevelopment();
                var IsDuplicate = dal.IsA4Duplicate(LoginUser.CompanyID, ProjectID, ApplicationFormID, A4);

                return Json(IsDuplicate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult GetUnitListByProjectID(int ProjectID, bool AllUnits = false)
        {
            try
            {
                DALDropdowns dal = new DALDropdowns();
                var unit = dal.DVUnitsListByProjectID(ProjectID, AllUnits);

                return Json(unit, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult DVOccupiedUnitsListByProjectID(int ProjectID, bool IsOccupied = false)
        {
            try
            {
                DALDropdowns dal = new DALDropdowns();
                var unit = dal.DVOccupiedUnitsListByProjectID(ProjectID, IsOccupied);

                return Json(unit, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult CancelUnit(int UnitID)
        {
            try
            {
                DALDropdowns dal = new DALDropdowns();
                var result = dal.CancelUnit(UnitID);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadApplicantPic()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    var applicantID = Convert.ToInt32(Request["ApplicantID"]);
                    var ApplicationFormID = Convert.ToInt32(Request["ApplicationFormID"]);

                    for (int i = 0; i < files.Count; i++)
                    {
                        // string path = AppDomain.CurrentDomain.BaseDirectory + "Images/applicants";  
                        string filename = Path.GetFileName(Request.Files[i].FileName);
                        string extension = Path.GetExtension(filename);

                        HttpPostedFileBase file = files[i];
                        string serverPath;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            serverPath = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            serverPath = "Applicant_" + ApplicationFormID + "_" + applicantID + "_" + DateTime.Now.ToString("yymmddssfff") + extension; //file.FileName;
                        }
                        // Get the complete folder path and store the file inside it.  
                        var ImagePath = "/Images/Applicants/PICS/" + serverPath;
                        serverPath = Path.Combine(Server.MapPath("/Images/Applicants/PICS/"), serverPath);

                        // delete the previous image file
                        var applicant = new DALDevelopment().GetDVApplicants(ApplicationFormID).Where(x => x.ApplicantID == applicantID).FirstOrDefault();
                        var oldServerPath = Server.MapPath(applicant.ImagePath);
                        if (System.IO.File.Exists(oldServerPath))
                        {
                            System.IO.File.Delete(oldServerPath);
                        }

                        //

                        new DALDevelopment().applicantImagePathSave(applicantID, ImagePath);
                        file.SaveAs(serverPath);
                    }
                    //return RedirectToRoute(ApplicationForm, 9);
                    return RedirectToRoute(new
                    {
                        controller = "DVDevelopment",
                        action = "ApplicationForm",
                        id = ApplicationFormID
                    });
                    // Returns message that successfully uploaded  
                    // return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public JsonResult GetApplicationFormID(int ProjectID, int UnitID)
        {
            try
            { 
                var FormApplicationFormID = new DALDevelopment().GetApplicationFormID(ProjectID, UnitID);
                return Json(FormApplicationFormID);
            }
            catch (Exception ex) { 
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult DVFormApplicationSave(DVApplicationForm DVApplicationForm, List<DVApplicant> DVApplicantList, short? DVArrangementTypeID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                GL.Models.response res = new GL.Models.response();
                if (DVApplicationForm.ApplicationFormID == 0)
                {

                    DVApplicationForm.CreatedAt = DateTime.Now;
                    DVApplicationForm.CreatedBy = LoginUser.UsersID;
                    DVApplicationForm.IsActive=true;
                }

                DVApplicationForm.CompanyID = LoginUser.CompanyID;
                DVApplicationForm.ModifiedAt = DateTime.Now;
                DVApplicationForm.ModifiedBy = LoginUser.UsersID;


                bool result = new DALDevelopment().DVApplicationFormSave(DVApplicationForm,DVArrangementTypeID);


                if (result == true)
                {
                    // save details
                    if (DVApplicantList != null && DVApplicantList.Count > 0)
                    {
                        foreach (var item in DVApplicantList)
                        {
                            item.ApplicationFormID = DVApplicationForm.ApplicationFormID;

                            if (item.ApplicantID == 0)
                            {
                                item.CreatedBy = LoginUser.UsersID;
                                item.CreatedAt = DateTime.Now;
                            }

                            item.CompanyID = LoginUser.CompanyID;
                            item.ModifiedBy = LoginUser.UsersID;
                            item.ModifiedAt = DateTime.Now;


                            new DALDevelopment().DVApplicantSave(item);
                            res.resObjList = new List<object>();
                            res.resObjList.Add(item);
                        }
                    }
                    // end save details

                }

                res.id = DVApplicationForm.ApplicationFormID;
                res.status = true;
                res.resMessage = "Record save successfully!";

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult DVFormApplicationList()
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new ApplicationFormViewModel();
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
        public ActionResult DVFormApplicationSearchList(SearchModel search)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new ApplicationFormViewModel();

                search.CompanyID = LoginUser.CompanyID;

                model.DVApplicationFormSearchList = new DALDevelopment().GetDVApplicationFormSearchList(search);
                return View("_DVFormApplicationSearchList", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Receipt
        [HttpGet]
        public ActionResult DVReceipt(int? ApplicationFormID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                DVReceiptViewModel model = new DVReceiptViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.DVPaymentPlanTypeList = dalDropdowns.DVPaymentPlanTypeList(LoginUser.CompanyID);
                ViewBag.DVPaymentMethodList = dalDropdowns.DVPaymentMethodList(LoginUser.CompanyID);

                if (ApplicationFormID != null || ApplicationFormID > 0)
                {
                    var dal = new DALDVReceipt();
                    model.DVReceiptHead = dal.GetDVReceiptHead(ApplicationFormID.Value).FirstOrDefault();

                    if (model.DVReceiptHead.ReceiptID == 0)
                    {
                        model.DVReceiptHead.ApplicationFormDate = DateTime.Now;
                        model.DVReceiptHead.DVReceiptDate = DateTime.Now;
                        model.DVReceiptHead.CreatedAt = DateTime.Now;
                        model.DVReceiptHead.ModifiedAt = DateTime.Now;
                    }
                    model.DVReceiptDetailRows = new List<spDVReceiptDetail_Result>();
                    //model.DVReceiptDetailRows = dal.GetDVReceiptDetailRows(model.DVReceiptHead.ReceiptID).ToList();
                }
                else
                {
                    model.DVReceiptHead = new spDVReceiptHead_Result();
                    model.DVReceiptHead.ApplicationFormID = 0;
                    model.DVReceiptHead.ReceiptID = 0;
                    model.DVReceiptHead.ProjectName = string.Empty;
                    model.DVReceiptHead.ApplicationFormNo = string.Empty;
                    model.DVReceiptHead.ApplicationFormDate = DateTime.Now;
                    model.DVReceiptHead.UnitNo = string.Empty;
                    model.DVReceiptHead.UnitTypeName = string.Empty;
                    model.DVReceiptHead.FloorNo = string.Empty;
                    model.DVReceiptHead.SQFT = 0;
                    model.DVReceiptHead.Price = 0;
                    model.DVReceiptHead.Amount = 0;
                    model.DVReceiptHead.TokenMoney = 0;
                    model.DVReceiptHead.DVReceiptDate = DateTime.Now;
                    model.DVReceiptHead.CreatedBy = 0;
                    model.DVReceiptHead.CreatedAt = DateTime.Now;
                    model.DVReceiptHead.ModifiedBy = 0;
                    model.DVReceiptHead.ModifiedAt = DateTime.Now;
                    model.DVReceiptHead.Varified = false;
                    model.DVReceiptHead.VarifiedBy = 0;
                    model.DVReceiptHead.VarifiedByName = string.Empty;
                    model.DVReceiptHead.Locked = false;
                    model.DVReceiptHead.LockedBy = 0;
                    model.DVReceiptHead.LockedByName = string.Empty;
                    model.DVReceiptHead.CompanyID = 0;
                    model.DVReceiptDetailRows = new List<spDVReceiptDetail_Result>();
                    model.DVReceiptDetailRows[0].CreatedAt = DateTime.Now;
                    model.DVReceiptDetailRows[0].ModifiedAt = DateTime.Now;

                }
                model.RoleID = new DALSecurity().GetLoginUser(LoginUser.CompanyID, LoginUser.UsersID).RoleID;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult DVReceiptSave(DVReceipt DVReceipt, List<DVReceiptDetail> DVReceiptDetail)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];

                GL.Models.response res = new GL.Models.response();

                if (DVReceipt.DVReceiptID == 0)
                {
                    DVReceipt.CompanyID = LoginUser.CompanyID;
                    DVReceipt.CreatedAt = DateTime.Now;
                    DVReceipt.CreatedBy = LoginUser.UsersID;
                }

                DVReceipt.ModifiedAt = DateTime.Now;
                DVReceipt.ModifiedBy = LoginUser.UsersID;

                
                


                bool result = new DALDVReceipt().DVReceiptSave(DVReceipt);


                if (result == true)
                {
                    // save details
                    if (DVReceiptDetail != null && DVReceiptDetail.Count > 0)
                    {
                        foreach (var item in DVReceiptDetail)
                        {
                            item.DVReceiptID = DVReceipt.DVReceiptID;

                            if (item.DVReceiptDetailID == 0)
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
                            new DALDVReceipt().DVReceiptDetailSave(item);
                        }
                    }
                    // end save details

                    res.resObj = DVReceipt;
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
        public JsonResult DVReceiptDetailDelete(int DVReceiptDetailID)
        {
            try
            {
                GL.Models.response res = new GL.Models.response();
                var result = new DALDVReceipt().DVReceiptDetailDelete(DVReceiptDetailID);
                if (result == true)
                {
                    res.status = true;
                    res.resMessage = "Row deleted successfully!";
                }
                else
                {
                    res.status = false;
                    res.resMessage = "There is some error to delete this row!";
                }

                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DVReceiptList

        [HttpGet]
        public ActionResult DVReceiptByID(int? ReceiptID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                DVReceiptViewModel model = new DVReceiptViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.DVPaymentPlanTypeList = dalDropdowns.DVPaymentPlanTypeList(LoginUser.CompanyID);
                ViewBag.DVPaymentMethodList = dalDropdowns.DVPaymentMethodList(LoginUser.CompanyID);

                if (ReceiptID != null || ReceiptID > 0)
                {
                    var dal = new DALDVReceipt();
                    model.DVReceiptHeadByReceiptID = dal.GetDVReceiptHeadByReceiptID(ReceiptID.Value).FirstOrDefault();

                    if (model.DVReceiptHeadByReceiptID.ReceiptID == 0)
                    {
                        model.DVReceiptHeadByReceiptID.ApplicationFormDate = DateTime.Now;
                        model.DVReceiptHeadByReceiptID.DVReceiptDate = DateTime.Now;
                        model.DVReceiptHeadByReceiptID.CreatedAt = DateTime.Now;
                        model.DVReceiptHeadByReceiptID.ModifiedAt = DateTime.Now;
                    }

                    model.DVReceiptDetailByReceiptIDRows = dal.GetDVReceiptDetailByReceiptIDRows(ReceiptID.Value).ToList();
                }
                else
                {
                    model.DVReceiptHeadByReceiptID = new spDVReceiptHeadByReceiptID_Result();

                    model.DVReceiptHeadByReceiptID.ApplicationFormID = 0;
                    model.DVReceiptHeadByReceiptID.ReceiptID = 0;
                    model.DVReceiptHeadByReceiptID.ProjectName = string.Empty;
                    model.DVReceiptHeadByReceiptID.ApplicationFormNo = string.Empty;
                    model.DVReceiptHeadByReceiptID.ApplicationFormDate = DateTime.Now;
                    model.DVReceiptHeadByReceiptID.UnitNo = string.Empty;
                    model.DVReceiptHeadByReceiptID.UnitTypeName = string.Empty;
                    model.DVReceiptHeadByReceiptID.FloorNo = string.Empty;
                    model.DVReceiptHeadByReceiptID.SQFT = 0;
                    model.DVReceiptHeadByReceiptID.Price = 0;
                    model.DVReceiptHeadByReceiptID.Amount = 0;
                    model.DVReceiptHeadByReceiptID.TokenMoney = 0;
                    model.DVReceiptHeadByReceiptID.DVReceiptDate = DateTime.Now;
                    model.DVReceiptHeadByReceiptID.Varified=false;
                    model.DVReceiptHeadByReceiptID.VarifiedBy = 0;
                    model.DVReceiptHeadByReceiptID.VarifiedByName = string.Empty;
                    model.DVReceiptHeadByReceiptID.Locked=false;
                    model.DVReceiptHeadByReceiptID.LockedBy = 0;
                    model.DVReceiptHeadByReceiptID.LockedByName = string.Empty;
                    model.DVReceiptHeadByReceiptID.CreatedBy = 0;
                    model.DVReceiptHeadByReceiptID.CreatedAt = DateTime.Now;
                    model.DVReceiptHeadByReceiptID.ModifiedBy = 0;
                    model.DVReceiptHeadByReceiptID.ModifiedAt = DateTime.Now;
                    model.DVReceiptHeadByReceiptID.CompanyID = 0;

                }
                model.RoleID = new DALSecurity().GetLoginUser(LoginUser.CompanyID, LoginUser.UsersID).RoleID;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpGet]
        public ActionResult DVReceiptList()
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new DVReceiptViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();
                ViewBag.Projects = dalDropdowns.DVProjectsList(LoginUser.CompanyID);
                ViewBag.Units = new List<DVUnit>();
                
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     
        [HttpPost]
        public ActionResult DVReceiptSearchList(SearchModel search)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new DVReceiptViewModel();


                search.CompanyID = LoginUser.CompanyID;

                model.DVReceiptList = new DALDVReceipt().GetDVDVReceiptSearchList(search);
                return View("_DVReceiptSearchList", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region DVReceiptSGO
      
        [HttpGet]
        public ActionResult DVReceiptListSGO()
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new DVReceiptViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();
                ViewBag.Projects = dalDropdowns.DVProjectsList(1);
                ViewBag.Units = new List<DVUnit>();

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult DVReceiptSearchListSGO(SearchModel search)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new DVReceiptViewModel();


                search.CompanyID = LoginUser.CompanyID;

                model.DVReceiptListSGO = new DALDVReceipt().GetDVDVReceiptSearchListSGO(search);
                return View("_DVReceiptSearchListSGO", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult DVReceiptSGO(int? ApplicationFormID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                DVReceiptViewModel model = new DVReceiptViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.DVPaymentPlanTypeList = dalDropdowns.DVPaymentPlanTypeList(LoginUser.CompanyID);
                ViewBag.DVPaymentMethodList = dalDropdowns.DVPaymentMethodList(LoginUser.CompanyID);

                if (ApplicationFormID != null || ApplicationFormID > 0)
                {
                    var dal = new DALDVReceipt();
                    model.DVReceiptHead = dal.GetDVReceiptHead(ApplicationFormID.Value).FirstOrDefault();

                    if (model.DVReceiptHead.ReceiptID == 0)
                    {
                        model.DVReceiptHead.ApplicationFormDate = DateTime.Now;
                        model.DVReceiptHead.DVReceiptDate = DateTime.Now;
                        model.DVReceiptHead.CreatedAt = DateTime.Now;
                        model.DVReceiptHead.ModifiedAt = DateTime.Now;
                    }
                    model.DVReceiptDetailRows = new List<spDVReceiptDetail_Result>();
                    //model.DVReceiptDetailRows = dal.GetDVReceiptDetailRows(model.DVReceiptHead.ReceiptID).ToList();
                }
                else
                {
                    model.DVReceiptHead = new spDVReceiptHead_Result();
                    model.DVReceiptHead.ApplicationFormID = 0;
                    model.DVReceiptHead.ReceiptID = 0;
                    model.DVReceiptHead.ProjectName = string.Empty;
                    model.DVReceiptHead.ApplicationFormNo = string.Empty;
                    model.DVReceiptHead.ApplicationFormDate = DateTime.Now;
                    model.DVReceiptHead.UnitNo = string.Empty;
                    model.DVReceiptHead.UnitTypeName = string.Empty;
                    model.DVReceiptHead.FloorNo = string.Empty;
                    model.DVReceiptHead.SQFT = 0;
                    model.DVReceiptHead.Price = 0;
                    model.DVReceiptHead.Amount = 0;
                    model.DVReceiptHead.TokenMoney = 0;
                    model.DVReceiptHead.DVReceiptDate = DateTime.Now;
                    model.DVReceiptHead.CreatedBy = 0;
                    model.DVReceiptHead.CreatedAt = DateTime.Now;
                    model.DVReceiptHead.ModifiedBy = 0;
                    model.DVReceiptHead.ModifiedAt = DateTime.Now;
                    model.DVReceiptHead.Varified = false;
                    model.DVReceiptHead.VarifiedBy = 0;
                    model.DVReceiptHead.VarifiedByName = string.Empty;
                    model.DVReceiptHead.Locked = false;
                    model.DVReceiptHead.LockedBy = 0;
                    model.DVReceiptHead.LockedByName = string.Empty;
                    model.DVReceiptHead.CompanyID = 0;
                    model.DVReceiptDetailRows = new List<spDVReceiptDetail_Result>();
                    model.DVReceiptDetailRows[0].CreatedAt = DateTime.Now;
                    model.DVReceiptDetailRows[0].ModifiedAt = DateTime.Now;

                }
                model.RoleID = new DALSecurity().GetLoginUser(LoginUser.CompanyID, LoginUser.UsersID).RoleID;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult DVReceiptByIDSGO(int? ReceiptID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                DVReceiptViewModel model = new DVReceiptViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.DVPaymentPlanTypeList = dalDropdowns.DVPaymentPlanTypeList(LoginUser.CompanyID);
                ViewBag.DVPaymentMethodList = dalDropdowns.DVPaymentMethodList(LoginUser.CompanyID);

                if (ReceiptID != null || ReceiptID > 0)
                {
                    var dal = new DALDVReceipt();
                    model.DVReceiptHeadByReceiptID = dal.GetDVReceiptHeadByReceiptID(ReceiptID.Value).FirstOrDefault();

                    if (model.DVReceiptHeadByReceiptID.ReceiptID == 0)
                    {
                        model.DVReceiptHeadByReceiptID.ApplicationFormDate = DateTime.Now;
                        model.DVReceiptHeadByReceiptID.DVReceiptDate = DateTime.Now;
                        model.DVReceiptHeadByReceiptID.CreatedAt = DateTime.Now;
                        model.DVReceiptHeadByReceiptID.ModifiedAt = DateTime.Now;
                    }

                    model.DVReceiptDetailByReceiptIDRows = dal.GetDVReceiptDetailByReceiptIDRows(ReceiptID.Value).ToList();
                }
                else
                {
                    model.DVReceiptHeadByReceiptID = new spDVReceiptHeadByReceiptID_Result();

                    model.DVReceiptHeadByReceiptID.ApplicationFormID = 0;
                    model.DVReceiptHeadByReceiptID.ReceiptID = 0;
                    model.DVReceiptHeadByReceiptID.ProjectName = string.Empty;
                    model.DVReceiptHeadByReceiptID.ApplicationFormNo = string.Empty;
                    model.DVReceiptHeadByReceiptID.ApplicationFormDate = DateTime.Now;
                    model.DVReceiptHeadByReceiptID.UnitNo = string.Empty;
                    model.DVReceiptHeadByReceiptID.UnitTypeName = string.Empty;
                    model.DVReceiptHeadByReceiptID.FloorNo = string.Empty;
                    model.DVReceiptHeadByReceiptID.SQFT = 0;
                    model.DVReceiptHeadByReceiptID.Price = 0;
                    model.DVReceiptHeadByReceiptID.Amount = 0;
                    model.DVReceiptHeadByReceiptID.TokenMoney = 0;
                    model.DVReceiptHeadByReceiptID.DVReceiptDate = DateTime.Now;
                    model.DVReceiptHeadByReceiptID.Varified = false;
                    model.DVReceiptHeadByReceiptID.VarifiedBy = 0;
                    model.DVReceiptHeadByReceiptID.VarifiedByName = string.Empty;
                    model.DVReceiptHeadByReceiptID.Locked = false;
                    model.DVReceiptHeadByReceiptID.LockedBy = 0;
                    model.DVReceiptHeadByReceiptID.LockedByName = string.Empty;
                    model.DVReceiptHeadByReceiptID.CreatedBy = 0;
                    model.DVReceiptHeadByReceiptID.CreatedAt = DateTime.Now;
                    model.DVReceiptHeadByReceiptID.ModifiedBy = 0;
                    model.DVReceiptHeadByReceiptID.ModifiedAt = DateTime.Now;
                    model.DVReceiptHeadByReceiptID.CompanyID = 0;

                }
                model.RoleID = new DALSecurity().GetLoginUser(LoginUser.CompanyID, LoginUser.UsersID).RoleID;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Document

        [HttpGet]
        public ActionResult DVDocument(int? ApplicationFormID, int? DocumentID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new DVDocumentViewModel();
                ViewBag.DVDocumentTypes = new DALDropdowns().DVDocumentTypeList(LoginUser.CompanyID).Select(x => new { DocumentTypeID = x.DocumentTypeID, DocumentTypeName=x.DocumentTypeName ?? "" } ).ToList();
                
                if(DocumentID != null)
                {
                    model.DVDocument = new DALDevelopment().GetDVDocument(DocumentID.Value);
                }
                else
                {
                    model.DVDocument = new DVDocument();
                    model.DVDocument.DocumentTypeID = 0; 
                    model.DVDocument.DocumentName = string.Empty;
                    model.DVDocument.DocumentID = 0;
                    model.DVDocument.DocumentPath = string.Empty;
                    model.DVDocument.DocumentDate = DateTime.Now;
                    model.DVDocument.ApplicationFormID = ApplicationFormID;
                 
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult DVDocumentList(int ApplicationFormID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                var model = new DVDocumentViewModel();
                var dal = new DALDevelopment();
                model.DocumentListRows = dal.GetDocumentsByApplication(ApplicationFormID);
                ViewBag.DVDocumentTypes = new DALDropdowns().DVDocumentTypeList(LoginUser.CompanyID);
                ViewBag.ApplicationFormID = ApplicationFormID;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult DownloadFile(string fileName)
        {
            // Define the path to your file
            var filePath = Path.Combine(Server.MapPath(fileName));

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            // Get the file's content type
            string contentType = MimeMapping.GetMimeMapping(fileName);

            // Return the file
            return File(filePath, contentType, fileName);
        }

        [HttpPost]
        public ActionResult UploadDocument()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    var DocumentID = Convert.ToInt32(Request["DocumentID"]);
                    var ApplicationFormID = Convert.ToInt32(Request["ApplicationFormID"]);

                    for (int i = 0; i < files.Count; i++)
                    {
                        // string path = AppDomain.CurrentDomain.BaseDirectory + "Images/applicants";  
                        string filename = Path.GetFileName(Request.Files[i].FileName);
                        string extension = Path.GetExtension(filename);

                        HttpPostedFileBase file = files[i];
                        string serverPath;
                        string fileName=string.Empty;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            serverPath = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            serverPath = "Document_" + ApplicationFormID + "_" + DocumentID + "_" + DateTime.Now.ToString("yymmddssfff") + extension; //file.FileName;
                            filename = serverPath;
                        }
                        // Get the complete folder path and store the file inside it.  
                        var ImagePath = "/Images/Documents/" + serverPath;
                        serverPath = Path.Combine(Server.MapPath("/Images/Documents/"), serverPath);

                        // delete the previous image file
                        var document = new DALDevelopment().GetDVDocument(DocumentID);
                        //var oldServerPath = document.DocumentPath;// Server.MapPath(document.DocumentPath);
                        var oldServerPath = Server.MapPath(document.DocumentPath);
                        if (System.IO.File.Exists(oldServerPath))
                        {
                            System.IO.File.Delete(oldServerPath);
                        }

                        //

                        new DALDevelopment().documentImagePathSave(DocumentID, ImagePath);
                        file.SaveAs(serverPath);
                        //var path = Server.MapPath("~/DEV/Documents/" + fileName);
                        //file.SaveAs(path);
                    }
                    //return RedirectToRoute(ApplicationForm, 9);
                    return RedirectToRoute(new
                    {
                        controller = "DVDevelopment",
                        action = "DVDocument",
                        DocumentID = DocumentID
                    });
                    // Returns message that successfully uploaded  
                    // return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public JsonResult DVDocumentSave(DVDocument DVDocument)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                GL.Models.response res = new GL.Models.response();
                if (DVDocument.DocumentID == 0)
                {

                    DVDocument.CreatedAt = DateTime.Now;
                    DVDocument.CreatedBy = LoginUser.UsersID;
                }

                DVDocument.CompanyID = LoginUser.CompanyID;
                DVDocument.ModifiedAt = DateTime.Now;
                DVDocument.ModifiedBy = LoginUser.UsersID;


                bool result = new DALDevelopment().DVDocumentSave(DVDocument);

                res.id = DVDocument.DocumentID;
                res.status = true;
                res.resMessage = "Record save successfully!";

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult DVDocumentDelete(int ApplicationFormID, int DocumentID)
        {
            try
            {
                GL.Models.response res = new GL.Models.response();
                var result = new DALDevelopment().DVDocumentDelete(DocumentID);
                if (result == true)
                {
                    res.status = true;
                    res.resMessage = "Row deleted successfully!";
                }
                else
                {
                    res.status = false;
                    res.resMessage = "There is some error to delete this row!";
                }

                return RedirectToRoute(new
                {
                    controller = "DVDevelopment",
                    action = "DVDocumentList",
                    ApplicationFormID = ApplicationFormID
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}