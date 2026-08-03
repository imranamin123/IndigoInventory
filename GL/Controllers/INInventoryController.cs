using Azure.Core;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.CrystalReports.TSLV;
using CrystalDecisions.Shared;
using CsvHelper;
using DAL;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.Reports;
using GL.ReportsWebForms;
using GL.ViewModels.Inventory;
using GL.ViewModels.Setup;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace GL.Controllers
{
    public class INInventoryController : Controller
    {

        #region Item


        [HttpPost]
        public ActionResult MergeFromINItemToProjectItem()
        {
            var db = new GLEntities();

            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var items = db.INItems.ToList();
                        foreach (var item in items)
                        {
                            var projectItemISA = new INProjectItem()
                            {
                                CompanyID = item.CompanyID,
                                ProjectID = 1,
                                ItemID = item.ItemID,
                            };
                            var projectItemIBA = new INProjectItem()
                            {
                                CompanyID = item.CompanyID,
                                ProjectID = 2,
                                ItemID = item.ItemID,
                            };

                            db.INProjectItems.AddOrUpdate(projectItemISA);
                            db.INProjectItems.AddOrUpdate(projectItemIBA);
                            db.SaveChanges();

                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        TempData["Msg"] = "ERROR: Data not imported successfully!";
                    }
                    TempData["Msg"] = "Data imported successfully!";
                }


            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToAction("Item");
            //return View();
        }

        [HttpPost]
        public ActionResult STN(HttpPostedFileBase file)
        {
            var db = new GLEntities();

            if (file == null || file.ContentLength == 0)
            {
                TempData["Msg"] = "Please select a CSV file.";
                return RedirectToAction("Item");
                //return View();
            }

            try
            {

                using (var reader = new StreamReader(file.InputStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var itemsCSV = csv.GetRecords<INItemModel>().ToList();
                    var itemsCSVWithOB = itemsCSV.ToList();
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {

                            foreach (var item in itemsCSVWithOB)
                            {

                                //STN

                                ////var stnd = new INStoreTransferNoteDetail()
                                ////{
                                ////    CompanyID = 1,
                                ////    CreatedAt = DateTime.Now,
                                ////    CreatedBy = 59,
                                ////    QtyInHand = 0,
                                ////    ItemID = item.ItemID,
                                ////    ModifiedAt = DateTime.Now,
                                ////    ModifiedBy = 59,
                                ////    Remarks = "STN Transfer Requested from IBA to ISA By Code 24-Mar-2026",
                                ////    RequestDetailID = db.INPurchaseRequisitionDetails.Where(x => x.RequestID == 10979 && x.ItemID == item.ItemID).FirstOrDefault().RequestDetailID,
                                ////    RequestedQty = item.STN,
                                ////    TransferQty = item.STN,
                                ////    StoreTransferNoteID = stn.StoreTransferNoteID,
                                ////    StoreTransferNoteDetailID = 0
                                ////};
                                ////db.INStoreTransferNoteDetails.AddOrUpdate(stnd);

                                ////// demand

                                ////var prd = new INPurchaseRequisitionDetail()
                                ////{
                                ////    RequestID = pr.RequestID,
                                ////    ItemID = item.ItemID,
                                ////    RequestedQty = item.STN,
                                ////    ApprovedQty = item.STN,
                                ////    QtyInHand = 0,
                                ////    LastRate = 0,
                                ////    Remarks = "STN Transfer Requested from IBA to ISA By Code 24-Mar-2026",
                                ////    Balance = 0,
                                ////    CreatedAt = DateTime.Now,
                                ////    CreatedBy = 59,
                                ////    ModifiedAt = DateTime.Now,
                                ////    ModifiedBy = 59
                                ////};

                                //prds.Add(prd);
                                //  prds.
                                ////db.INPurchaseRequisitionDetails.AddOrUpdate(prd);
                                ////db.SaveChanges();

                                //var purchaseRequisitionDetail = db.INPurchaseRequisitionDetails.Where(x => x.RequestDetailID == stn.RequestDetailID).FirstOrDefault();
                                //if (purchaseRequisitionDetail != null)
                                //{
                                //    purchaseRequisitionDetail.Balance += item.STN;
                                //    purchaseRequisitionDetail.ModifiedBy = 59;
                                //    purchaseRequisitionDetail.ModifiedAt = DateTime.Now;
                                //    db.INPurchaseRequisitionDetails.AddOrUpdate(purchaseRequisitionDetail);
                                //    db.SaveChanges();
                                //}


                                    //var projectItemTo = db.INProjectItems.Where(x => x.ItemID == item.ItemID && x.ProjectID == 1).FirstOrDefault();
                                    //if (projectItemTo != null)
                                    //{
                                    //    projectItemTo.QtyInHand -= item.STN;
                                    //    db.INProjectItems.AddOrUpdate(projectItemTo);
                                    //    db.SaveChanges();
                                    //}

                                    //var projectItemFrom = db.INProjectItems.Where(x => x.ItemID == item.ItemID && x.ProjectID == 2).FirstOrDefault();
                                    //if (projectItemFrom != null)
                                    //{
                                    //    projectItemFrom.QtyInHand += item.STN;
                                    //    db.INProjectItems.AddOrUpdate(projectItemFrom);
                                    //    db.SaveChanges();
                                    //}

                                }

                            ////pr.SubmitedAtMD = DateTime.Now;
                            ////pr.SubmitedByMD = 40;
                            ////db.INPurchaseRequisitions.AddOrUpdate(pr);
                            ////db.SaveChanges();

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            TempData["Msg"] = "ERROR: Data not imported successfully!";
                        }
                        TempData["Msg"] = "Data imported successfully!";
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToAction("Item");
        }


        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            var db = new GLEntities();

            if (file == null || file.ContentLength == 0)
            {
                TempData["Msg"] = "Please select a CSV file.";
                return RedirectToAction("Item");
                //return View();
            }

            try
            {

                using (var reader = new StreamReader(file.InputStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var itemsCSV = csv.GetRecords<INItemModel>().ToList();
                    var itemsCSVWithOB = itemsCSV.ToList();
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in itemsCSVWithOB)
                            {

                                var inItem = db.INProjectItems.Where(x => x.ItemID == item.ItemID && x.ProjectID == 1).FirstOrDefault();
                                if (inItem != null)
                                {
                                    inItem.LastRate = item.LastRate;
                                    db.INProjectItems.AddOrUpdate(inItem);
                                    db.SaveChanges();
                                }

                            }
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            TempData["Msg"] = "ERROR: Data not imported successfully!";
                        }
                        TempData["Msg"] = "Data imported successfully!";
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToAction("Item");
        }



        [HttpGet]
        public ActionResult ItemList()
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INItemViewModel model = new INItemViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.INGroupList = new DALDropdowns().INGroupList(LoginUser.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ItemSearchList(string ItemCode, string Name, int? GroupID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            INItemViewModel model = new INItemViewModel();


            model.INItemSearchList = new DALInventory().GetINItemSearchList(ItemCode, Name, GroupID);
            return View("_ItemSearchListRows", model);

        }

        [HttpGet]
        public ActionResult Item(Int64? id)
        {
            try
            {
                INItemViewModel model = new INItemViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }

                ViewBag.RollID = LoginUser.RoleID;
                ViewBag.username = LoginUser.username;


                ViewBag.INGroupList = new DALDropdowns().INGroupList(LoginUser.CompanyID);
                ViewBag.INCategoryList = null;// new DALDropdowns().INCategoryList(LoginUser.CompanyID);
                ViewBag.INSizeList = new DALDropdowns().INSizeList(LoginUser.CompanyID);
                ViewBag.INUnitOfMeasurementList = new DALDropdowns().INUnitOfMeasurementList(LoginUser.CompanyID);


                if (id != null || id > 0)
                {
                    model.INItem = new DALInventory().INItemGet(id.GetValueOrDefault(0));
                    ViewBag.INCategoryList = new DALDropdowns().INCategoryList(model.INItem.GroupID.GetValueOrDefault(0));
                }
                else
                {
                    model.INItem = new INItem();

                    model.INItem.ItemID = 0;
                    model.INItem.ItemCode = 0;

                    model.INItem.UOMID = 0;
                    model.INItem.Freeze = false;
                    model.INItem.CreatedAt = DateTime.Now;
                    model.INItem.CreatedBy = 0;
                    model.INItem.ModifiedAt = DateTime.Now;
                    model.INItem.ModifiedBy = 0;
                    model.INItem.CompanyID = 1;

                    ViewBag.INCategoryList = new List<INCategory>();
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult INItemSave(INItem INItem)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                response res = new response();
                DALInventory dal = new DALInventory();

                if (INItem.ItemID == 0)
                {
                    INItem.CreatedAt = DateTime.Now;
                    INItem.CreatedBy = LoginUser.UsersID;
                    INItem.CompanyID = LoginUser.CompanyID;
                }

                INItem.ModifiedAt = DateTime.Now;
                INItem.ModifiedBy = LoginUser.UsersID;
                if (INItem.ItemCode == 0)
                {
                    INItem.ItemCode = dal.NextItemCodeByGroup(INItem.GroupID.GetValueOrDefault(0));
                }

                //////////// duplicate group and size ///////////
                bool IsDuplicateGroupSize = dal.IsDuplicateGroupSize(INItem.CompanyID.GetValueOrDefault(0), INItem.GroupID.GetValueOrDefault(0), INItem.SizeID.GetValueOrDefault(0), INItem.ItemID);
                bool IsDuplicateSizeDescription = dal.IsDuplicateSizeDescription(INItem);
                //IsDuplicateGroupSize = true;
                //if (IsDuplicateGroupSize)
                //{
                //    res.status = false;
                //    res.resObj = null;
                //    res.resMessage = "Size already exists for the selected Group!";
                //    return Json(res, JsonRequestBehavior.AllowGet);
                //}
                if (IsDuplicateSizeDescription)
                {
                    res.status = false;
                    res.resObj = null;
                    res.resMessage = "Size already exists for the given description!";
                    return Json(res, JsonRequestBehavior.AllowGet);

                }
                //////////// end duplicate group and size ///////////

                bool result = dal.INItemSave(INItem);
                if (result == true)
                {
                    res.status = true;
                    res.id = INItem.ItemID;
                    res.resMessage = "Record saved successfully!";
                }
                else
                {
                    res.status = false;
                    res.resObj = null;
                    res.resMessage = "Record not saved!";
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult GetCategoryListByGroupID(int GroupID)
        {
            try
            {
                DALDropdowns dal = new DALDropdowns();
                var categories = dal.INCategoryList(GroupID).Select(x => new { CategoryID = x.CategoryID, Name = x.Name });

                return Json(categories, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region PurchaseRequisition


        [HttpGet]
        public ActionResult PurchaseRequisitionList()
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INPurchaseRequisitionViewModel model = new INPurchaseRequisitionViewModel();

                ViewBag.roleid = LoginUser.RoleID;
                ViewBag.username = LoginUser.username;
                ViewBag.INItems = new DALDropdowns().INItemsList(LoginUser.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult PurchaseRequisitionSearchList(DateTime? RequestDateFrom, DateTime? RequestDateTo, long? ItemID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            INPurchaseRequisitionViewModel model = new INPurchaseRequisitionViewModel();

            if (LoginUser.username == "ind.umer" || LoginUser.username == "ind.shafeeq")
            {
                model.INPurchaseRequisitionSearchList = new DALInventory().GetINPurchaseRequisitionSearchPurchaseHeadList(LoginUser.CompanyID, null, RequestDateFrom, RequestDateTo, ItemID);
            }
            else
            {
                model.INPurchaseRequisitionSearchList = new DALInventory().GetINPurchaseRequisitionSearchList(LoginUser.CompanyID, LoginUser.UsersID, RequestDateFrom, RequestDateTo,ItemID);
            }
            model.RoleID = LoginUser.RoleID;
            return View("_PurchaseRequisitionSearchListRows", model);

        }

        [HttpGet]
        public ActionResult PurchaseRequisition(int? id)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INPurchaseRequisitionViewModel model = new INPurchaseRequisitionViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.Projects = dalDropdowns.INProjectsList(LoginUser.CompanyID);
                ViewBag.INRequestTypes = dalDropdowns.INRequestTypesList(LoginUser.CompanyID);
                ViewBag.INStores = dalDropdowns.INStoreList(LoginUser.CompanyID);
                ViewBag.INItems = dalDropdowns.INItemsList(LoginUser.CompanyID);


                if (id != null || id > 0)
                {
                    model.INPurchaseRequisition = new DALInventory().INPurchaseRequisitionGet(id.GetValueOrDefault(0));

                    model.INPurchaseRequisitionDetailRows = new DALInventory().GetINPurchaseRequisitionDetailRows(id.GetValueOrDefault(0));
                }
                else
                {
                    model.INPurchaseRequisition = new INPurchaseRequisition();
                    model.INPurchaseRequisition.RequestID = 0;
                    model.INPurchaseRequisition.RequestDate = DateTime.Now;
                    model.INPurchaseRequisition.ProjectID = 0;
                    model.INPurchaseRequisition.Remarks = string.Empty;
                    model.INPurchaseRequisition.RequestTypeID = 0;
                    model.INPurchaseRequisition.CreatedAt = DateTime.Now;
                    model.INPurchaseRequisition.CreatedBy = 0;
                    model.INPurchaseRequisition.ModifiedAt = DateTime.Now;
                    model.INPurchaseRequisition.ModifiedBy = 0;
                    model.INPurchaseRequisition.CancelledAt = DateTime.Now;
                    model.INPurchaseRequisition.CancelledBy = 0;
                    model.INPurchaseRequisition.CompanyID = 0;

                    model.IsNew = 1;

                    model.INPurchaseRequisitionDetailRows = new List<spINPurchaseRequisitionDetailRows_Result>();
                }
                model.RoleID = LoginUser.RoleID;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        private ActionResult INPurchaseRequisitionReportDownload(int RequestID)
        {
            var report = new rptINPurchaseRequisition();
            var PurchaseRequisitions = new GLEntities().spRptINPurchaseRequisition(RequestID);

            var PurchaseRequisitionData = (
                from v in PurchaseRequisitions
                select new spRptINPurchaseRequisitionModel
                {
                    Company = v.Company,
                    mRemarks = v.mRemarks,
                    DocumentNo = v.DocumentNo ?? "",
                    ProjectName = v.ProjectName,
                    RequestDate = v.RequestDate.GetValueOrDefault(DateTime.Now),
                    RequestID = v.RequestID,

                    RequestDetailID = v.RequestDetailID,
                    ItemID = v.ItemID.GetValueOrDefault(0),
                    Item = v.Item ?? "",
                    ItemCode = v.ItemCode.GetValueOrDefault(0),
                    Unit = v.Unit ?? "",
                    Size = v.Size ?? "",
                    RequestedQty = v.RequestedQty.GetValueOrDefault(0),
                    ApprovedQty = v.ApprovedQty.GetValueOrDefault(0),
                    dRemarks = v.dRemarks,
                    SubmittedByKPO = v.SubmittedByKPO,
                    SubmitedAtKPO = v.SubmitedAtKPO.GetValueOrDefault(DateTime.Now),

                    SubmittedByMD = v.SubmittedByMD,
                    SubmitedAtMD = v.SubmitedAtMD.GetValueOrDefault(DateTime.Now),

                    LastRate = v.LastRate.GetValueOrDefault(0),
                    QtyInHand = v.QtyInHand.GetValueOrDefault(0),

                }).ToList();


            report.SetDataSource(PurchaseRequisitionData);
            ReportDocument reportDocument = report;

            // Export to a memory stream
            Stream pdfStream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

            // Read the Stream into a MemoryStream
            MemoryStream memoryStream = new MemoryStream();
            pdfStream.CopyTo(memoryStream);

            // Close the original Stream
            pdfStream.Close();

            // Set the response for the browser to download the file
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Report.pdf");
            Response.Buffer = true;
            Response.Clear();

            // Write the MemoryStream to the response
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();

            // Cleanup
            reportDocument.Close();
            reportDocument.Dispose();
            return new EmptyResult();

        }

        [HttpPost]
        public JsonResult INPurchaseRequisitionSubmit(int RequestID, string Option)
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
                if(Option=="Submit")
                {
                    result = new DALInventory().INPurchaseRequisitionSubmit(RequestID, LoginUser.UsersID, LoginUser.RoleID);
                }
                else if (Option=="Cancell")
                {
                    result = new DALInventory().INPurchaseRequisitionCancell(RequestID, LoginUser.UsersID, LoginUser.RoleID);
                }
                    
                res.id = RequestID;
                res.status = true;
                res.resMessage = "Record submitted successfully!";

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult INPurchaseRequisitionUnSubmitByGM(int id)
        {
            try
            {
                bool result = new DALInventory().INPurchaseRequisitionUnSubmitByGM(id);

                return RedirectToAction("PurchaseRequisitionList");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult INPurchaseRequisitionObjectSave(INPurchaseRequisition INPurchaseRequisition, List<INPurchaseRequisitionDetail> INPurchaseRequisitionDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();
                var dal = new DALInventory();
                if (INPurchaseRequisition.RequestID == 0)
                {
                    INPurchaseRequisition.CompanyID = LoginUser.CompanyID;
                    INPurchaseRequisition.CreatedAt = DateTime.Now;
                    INPurchaseRequisition.CreatedBy = LoginUser.UsersID;
                    INPurchaseRequisition.ModifiedAt = DateTime.Now;
                    INPurchaseRequisition.ModifiedBy = LoginUser.UsersID;
                }
                else
                {
                    INPurchaseRequisition.ModifiedAt = DateTime.Now;
                    INPurchaseRequisition.ModifiedBy = LoginUser.UsersID;
                }


                if (INPurchaseRequisition.DocumentNo.GetValueOrDefault(0) == 0)
                {
                    INPurchaseRequisition.DocumentNo = dal.NextDocumentNumberByProject(INPurchaseRequisition.ProjectID.GetValueOrDefault(0));
                    var ProjectName = dal.GetProjectNameByID(INPurchaseRequisition.ProjectID.GetValueOrDefault(0)).ProjectName;
                    INPurchaseRequisition.ProjectDocumentNo = ProjectName + "-" + INPurchaseRequisition.DocumentNo.ToString().PadLeft(4, '0');
                }

                bool result = new DALInventory().INPurchaseRequisitionSave(INPurchaseRequisition);


                if (result == true)
                {
                    // save details
                    if (INPurchaseRequisitionDetailList != null && INPurchaseRequisitionDetailList.Count > 0)
                    {
                        foreach (var item in INPurchaseRequisitionDetailList)
                        {
                            item.RequestID = INPurchaseRequisition.RequestID;

                            if (item.RequestDetailID == 0)
                            {
                                item.CreatedBy = LoginUser.UsersID;
                                item.CreatedAt = DateTime.Now;
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                                item.CompanyID = LoginUser.CompanyID;
                            }
                            else
                            {
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                            }
                            item.Balance = item.ApprovedQty.GetValueOrDefault(0);
                            new DALInventory().INPurchaseRequisitionDetailSave(item);
                        }
                    }
                    // end save details

                    res.id = INPurchaseRequisition.RequestID;
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


        [HttpGet]
        public ActionResult INPurchaseRequisitionDetailDelete(long id)
        {

            try
            {

                new DALInventory().INPurchaseRequisitionDetailDelete(id);
                return RedirectToAction("PurchaseRequisition");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetINGetItemRow(int ProjectID, long ItemID)
        {
            try
            {
                var ItemRow = new DALInventory().GetINGetItemRow(ProjectID, ItemID);
                return Json(ItemRow, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region GoodsReceiptNote

        [HttpGet]
        public ActionResult GoodsReceiptNote(int? id)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INGoodsReceiptNoteViewModel model = new INGoodsReceiptNoteViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.Projects = dalDropdowns.INProjectsList(LoginUser.CompanyID);
                ViewBag.INRequestTypes = dalDropdowns.INRequestTypesList(LoginUser.CompanyID);
                ViewBag.INStores = dalDropdowns.INStoreList(LoginUser.CompanyID);

                List<spINGRNItemsDropdown_Result> INItems = new List<spINGRNItemsDropdown_Result>();
                ViewBag.APVendors = dalDropdowns.GLAPVendorsListWithCode(LoginUser.CompanyID);
                List<spINGRNItemsDropdown_Result> remainingINItems = new List<spINGRNItemsDropdown_Result>();
                if (id != null || id > 0)
                {
                    model.INGoodsReceiptNote = new DALInventory().INGoodsReceiptNote(id.GetValueOrDefault(0));

                    model.INGoodsReceiptNoteDetailRows = new DALInventory().GetINGoodsReceiptNoteDetailRows(id.GetValueOrDefault(0));

                    INItems = dalDropdowns.GetINGRItemsDropdown(LoginUser.CompanyID, model.INGoodsReceiptNote.ProjectID.GetValueOrDefault(0));

                    foreach (var item in INItems)
                    {
                        var itemId = item.ItemID;
                        string[] parts = itemId.Split(',');
                        string requestDetailID = parts.Length > 0 ? parts[0] : null;
                        string itemID = parts.Length > 1 ? parts[1] : null;

                        foreach (var grn in model.INGoodsReceiptNoteDetailRows)
                        {
                            if (grn.RequestDetailID == Convert.ToInt64(requestDetailID) && grn.ItemID == Convert.ToInt64(itemID))
                            {
                                remainingINItems.Add(item);

                            }
                            if (grn.RequestDetailID != Convert.ToInt64(requestDetailID) && grn.ItemID != Convert.ToInt64(itemID) && item.Balance > 0)
                            {
                                remainingINItems.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    model.INGoodsReceiptNote = new INGoodsReceiptNote();
                    model.INGoodsReceiptNote.GoodsReceiptNoteID = 0;
                    model.INGoodsReceiptNote.GoodsReceiptNotesDate = DateTime.Now;
                    model.INGoodsReceiptNote.ProjectID = 0;
                    model.INGoodsReceiptNote.DCINVNO = string.Empty;
                    model.INGoodsReceiptNote.DCINVDate = DateTime.Now;
                    model.INGoodsReceiptNote.IGPNO = 0;
                    model.INGoodsReceiptNote.VehicleNo = string.Empty;
                    model.INGoodsReceiptNote.Remarks = string.Empty;
                    model.INGoodsReceiptNote.IsPosted = false;
                    model.INGoodsReceiptNote.CreatedBy = 0;
                    model.INGoodsReceiptNote.CreatedAt = DateTime.Now;
                    model.INGoodsReceiptNote.ModifiedBy = 0;
                    model.INGoodsReceiptNote.ModifiedAt = DateTime.Now;
                    model.INGoodsReceiptNote.CompanyID = LoginUser.CompanyID;


                    model.IsNew = 1;

                    model.INGoodsReceiptNoteDetailRows = new List<spINGoodsReceiptNoteDetailRows_Result>();
                }
                ViewBag.INItems = remainingINItems.Count == 0 ? INItems.Where(x => x.Balance > 0).ToList() : remainingINItems;
                model.RoleID = LoginUser.RoleID;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult GoodsReceiptNoteDetailDelete(long id)
        {
            try
            {
                new DALInventory().INGoodsReceiptNoteDetailDelete(id);
                return RedirectToAction("PurchaseRequisition");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult GoodsReceiptNoteList(int? Pending=0)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INGoodsReceiptNoteViewModel model = new INGoodsReceiptNoteViewModel();

                ViewBag.roleid = LoginUser.RoleID;
                ViewBag.username = LoginUser.username;
                ViewBag.Pending = Pending;
                ViewBag.INItems = new DALDropdowns().INItemsList(LoginUser.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GoodsReceiptNoteSearchList(DateTime? FromDate, DateTime? ToDate, long? ItemID, int? Pending=0)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            INGoodsReceiptNoteViewModel model = new INGoodsReceiptNoteViewModel();
            
            if (LoginUser.username == "ind.umer" || LoginUser.username == "ind.shafeeq")
            {
                model.INGoodsReceiptNoteSearchList = new DALInventory().GetINGoodsReceiptNotePurchaseHeadSearchList(LoginUser.CompanyID, null, FromDate, ToDate,ItemID, Pending);
                
                if(Pending == 1)
                    model.username = LoginUser.username;
                else
                    model.username = string.Empty;
            }
            else
            {
                model.INGoodsReceiptNoteSearchList = new DALInventory().GetINGoodsReceiptNoteSearchList(LoginUser.CompanyID, LoginUser.UsersID, FromDate, ToDate, ItemID);
            }

            model.RoleID = LoginUser.RoleID;

            

            return View("_GoodsReceiptNoteSearchListRows", model);

        }


        [HttpGet]
        public ActionResult PurchaseOrderForGRN(int? id)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var modelGRN = new INGoodsReceiptNoteViewModel();
                var PO = new INPurchaseOrderViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.Projects = dalDropdowns.INProjectsList(LoginUser.CompanyID);
                ViewBag.INPOStatusList = dalDropdowns.INPOStatusList(LoginUser.CompanyID);
                ViewBag.INStores = dalDropdowns.INStoreList(LoginUser.CompanyID);

                List<spINGRNItemsForPODropdown_Result> INItems = new List<spINGRNItemsForPODropdown_Result>();
                //List<spINGRNItemsDropdown_Result> INItems = new List<spINGRNItemsDropdown_Result>();

                ViewBag.APVendors = dalDropdowns.GLAPVendorsListWithCode(LoginUser.CompanyID);
                List<spINGRNItemsForPODropdown_Result> remainingINItems = new List<spINGRNItemsForPODropdown_Result>();
                if (id != null || id > 0)
                {

                    modelGRN.INGoodsReceiptNote = new DALInventory().INGoodsReceiptNote(id.GetValueOrDefault(0));
                    modelGRN.INGoodsReceiptNoteDetailRows = new DALInventory().GetINGoodsReceiptNoteDetailRows(id.GetValueOrDefault(0));
                    PO = GetPOForGRN(modelGRN);

                    INItems = dalDropdowns.GetINGRNItemsForPODropdown(id);

                    ViewBag.INItems = INItems.ToList();
                    modelGRN.RoleID = LoginUser.RoleID;

                    
                }
                return View(PO);
                //return Content(JsonConvert.SerializeObject(PO));
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private INPurchaseOrderViewModel GetPOForGRN(INGoodsReceiptNoteViewModel grn)
        {
            var model = new INPurchaseOrderViewModel();

            model.INPurchaseOrder = new INPurchaseOrder()
            {
                ApprovedAt = DateTime.Now,
                ApprovedBy = 0,
                APVendorID = grn.INGoodsReceiptNote.APVendorID,
                CompanyID = grn.INGoodsReceiptNote.CompanyID,
                CreatedAt = DateTime.Now,
                CreatedBy = 0,
                GoodsReceiptNoteID = grn.INGoodsReceiptNote.GoodsReceiptNoteID,
                ModifiedAt = DateTime.Now,
                ModifiedBy = 0,
                PaymentTerms = "",
                POStatusID = 0,
                ProjectID = grn.INGoodsReceiptNote.ProjectID,
                PurchaseOrderDate = DateTime.Now,
                PurchaseOrderID = 0,
                Remarks = "",
                RequestID = 0
            };

            model.INPurchaseOrderDetailRows = new List<spINPurchaseOrderDetailRows_Result>();

            foreach(var item in grn.INGoodsReceiptNoteDetailRows)
            {
                var spINPurchaseOrderDetailRow = new spINPurchaseOrderDetailRows_Result();
                spINPurchaseOrderDetailRow.Amount = item.Amount;

                spINPurchaseOrderDetailRow.PurchaseOrderDetailID = 0;
                spINPurchaseOrderDetailRow.PurchaseOrderID = 0;
                spINPurchaseOrderDetailRow.RequestDetailID = item.RequestDetailID;
                spINPurchaseOrderDetailRow.ItemID = item.ItemID;
                spINPurchaseOrderDetailRow.Item = "";
                spINPurchaseOrderDetailRow.Size = item.Size;
                spINPurchaseOrderDetailRow.Unit = item.Unit;
                spINPurchaseOrderDetailRow.RequestedQty = 0;
                spINPurchaseOrderDetailRow.ApprovedQty = item.ReceivedQty;
                spINPurchaseOrderDetailRow.UnitPrice = item.Rate;
                spINPurchaseOrderDetailRow.CompanyID = item.CompanyID;

                model.INPurchaseOrderDetailRows.Add(spINPurchaseOrderDetailRow);

            }

            return model;

        }

        [HttpGet]
        public JsonResult GetINGRDropdownItems(int CompanyID, int ProjectID)
        {
            try
            {
                List<spINGRNItemsDropdown_Result> INGRDropdownItems = new DALDropdowns().GetINGRItemsDropdown(CompanyID, ProjectID).Where(x => x.Balance > 0).ToList(); ;
                return Json(INGRDropdownItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public JsonResult GetINGRNItemRow(long ItemID, long RequestDetailID)
        {
            try
            {
                var ItemGRNRow = new DALInventory().GetINGRNItemRow(ItemID, RequestDetailID);
                return Json(ItemGRNRow, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult INGoodsReceiptNoteUnPost(long id)
        {
            try
            {

                bool result = new DALInventory().INGoodsReceiptNoteUnPost(id);

                return RedirectToAction("GoodsReceiptNoteList");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult INGoodsReceiptNoteSave(INGoodsReceiptNote INGoodsReceiptNote, List<INGoodsReceiptNoteDetail> INGoodsReceiptNoteDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();
                var dal = new DALInventory();
                bool result = false;
                if (INGoodsReceiptNote.GoodsReceiptNoteID == 0)
                {
                    INGoodsReceiptNote.CompanyID = LoginUser.CompanyID;
                    INGoodsReceiptNote.CreatedAt = DateTime.Now;
                    INGoodsReceiptNote.CreatedBy = LoginUser.UsersID;
                    INGoodsReceiptNote.ModifiedAt = DateTime.Now;
                    INGoodsReceiptNote.ModifiedBy = LoginUser.UsersID;
                }
                else
                {
                    INGoodsReceiptNote.ModifiedAt = DateTime.Now;
                    INGoodsReceiptNote.ModifiedBy = LoginUser.UsersID;
                }

                result = new DALInventory().INGoodsReceiptNoteSave(INGoodsReceiptNote);

                if (result == true)
                {
                    // save details
                    if (INGoodsReceiptNoteDetailList != null && INGoodsReceiptNoteDetailList.Count > 0)
                    {
                        foreach (var item in INGoodsReceiptNoteDetailList)
                        {
                            item.GoodsReceiptNoteID = INGoodsReceiptNote.GoodsReceiptNoteID;

                            if (item.GoodsReceiptNoteDetailID == 0)
                            {
                                item.CreatedBy = LoginUser.UsersID;
                                item.CreatedAt = DateTime.Now;
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                                item.CompanyID = LoginUser.CompanyID;
                            }
                            else
                            {
                                //item.CompanyID = LoginUser.CompanyID;
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                            }
                            new DALInventory().INGoodsReceiptNoteDetailSave(item);
                        }
                    }
                    // end save details

                    //res.resObj = glVoucher;
                    res.id = INGoodsReceiptNote.GoodsReceiptNoteID;
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

        #region StoreIssueNote


        [HttpGet]
        public ActionResult StoreIssueNoteList()
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INStoreIssueNoteViewModel model = new INStoreIssueNoteViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.INItems = new DALDropdowns().INItemsList(LoginUser.CompanyID);
                ViewBag.INGroupList = new DALDropdowns().INGroupList(LoginUser.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult StoreIssueNoteSearchList(DateTime? FromDate, DateTime? ToDate, long? ItemID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            INStoreIssueNoteViewModel model = new INStoreIssueNoteViewModel();

            model.INStoreIssueNoteSearchList = new DALInventory().GetINStoreIssueNoteSearchList(LoginUser.CompanyID, LoginUser.UsersID, FromDate, ToDate, ItemID);
            model.RoleID = LoginUser.RoleID;
            return View("_StoreIssueNoteSearchListRows", model);

        }

        [HttpGet]
        public ActionResult StoreIssueNote(int? id)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INStoreIssueNoteViewModel model = new INStoreIssueNoteViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.Projects = dalDropdowns.INProjectsList(LoginUser.CompanyID);
                ViewBag.INRequestTypes = dalDropdowns.INRequestTypesList(LoginUser.CompanyID);
                ViewBag.INStores = dalDropdowns.INStoreList(LoginUser.CompanyID);

                List<spINStoreIssueNoteItemsDropdown_Result> INItems = new List<spINStoreIssueNoteItemsDropdown_Result>(); //dalDropdowns.StoreIssueNoteItemsDropdownItemsDropdown(LoginUser.CompanyID);
                List<spINStoreIssueNoteItemsDropdown_Result> remainingINItems = new List<spINStoreIssueNoteItemsDropdown_Result>();

                if (id != null || id > 0)
                {
                    model.INStoreIssueNote = new DALInventory().INStoreIssueNote(id.GetValueOrDefault(0));

                    model.INStoreIssueNoteDetailRows = new DALInventory().GetINStoreIssueNoteDetailRows(id.GetValueOrDefault(0));

                    INItems = dalDropdowns.StoreIssueNoteItemsDropdownItemsDropdown(model.INStoreIssueNote.ProjectID.GetValueOrDefault(0), LoginUser.CompanyID);

                    // spINStoreIssueNoteItemsDropdown only returns items with remaining balance > 0,
                    // so an item this note already issued down to zero balance would be missing from
                    // INItems and its name would fail to show in the dropdown. Add it back in using
                    // the name already stored on the detail row.
                    remainingINItems = INItems.ToList();
                    var dropdownItemIds = new HashSet<long>(remainingINItems.Select(x => x.ItemID));
                    foreach (var issue in model.INStoreIssueNoteDetailRows)
                    {
                        if (issue.ItemID.HasValue && dropdownItemIds.Add(issue.ItemID.Value))
                        {
                            remainingINItems.Add(new spINStoreIssueNoteItemsDropdown_Result
                            {
                                ItemID = issue.ItemID.Value,
                                Description = issue.Item,
                                Balance = 0
                            });
                        }
                    }
                }
                else
                {
                    model.INStoreIssueNote = new INStoreIssueNote();
                    model.INStoreIssueNote.StoreIssueNoteID = 0;
                    model.INStoreIssueNote.StoreIssueNoteDate = DateTime.Now;
                    model.INStoreIssueNote.ProjectID = 0;
                    model.INStoreIssueNote.Remarks = string.Empty;
                    model.INStoreIssueNote.IsPosted = false;
                    model.INStoreIssueNote.CreatedBy = 0;
                    model.INStoreIssueNote.CreatedAt = DateTime.Now;
                    model.INStoreIssueNote.ModifiedBy = 0;
                    model.INStoreIssueNote.ModifiedAt = DateTime.Now;
                    model.INStoreIssueNote.CompanyID = LoginUser.CompanyID;


                    model.IsNew = 1;

                    model.INStoreIssueNoteDetailRows = new List<spINStoreIssueNoteDetailRows_Result>();
                }
                ViewBag.INItems = remainingINItems;
                model.RoleID = LoginUser.RoleID;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public JsonResult GetStoreIssueNoteDropdownItems(int CompanyID, int ProjectID)
        {
            try
            {
                List<spINStoreIssueNoteItemsDropdown_Result> INStoreIssueNoteDropdownItems = new DALDropdowns().GetINStoreIssueNoteItemsDropdown(ProjectID, CompanyID);
                return Json(INStoreIssueNoteDropdownItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetStoreIssueNoteItemRow(long ItemID)
        {
            try
            {
                var ItemRow = new DALInventory().GetStoreIssueNoteItemRow(ItemID);
                return Json(ItemRow, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult INStoreIssueNoteSubmit(int RequestID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();

                bool result = new DALInventory().INPurchaseRequisitionSubmit(RequestID, LoginUser.UsersID, LoginUser.RoleID);

                //res.resObj = glVoucher;
                res.id = RequestID;
                res.status = true;
                res.resMessage = "Record submitted successfully!";

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult INStoreIssueNoteSave(INStoreIssueNote INStoreIssueNote, List<INStoreIssueNoteDetail> INStoreIssueNoteDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();
                var dal = new DALInventory();
                bool result = false;
                if (INStoreIssueNote.StoreIssueNoteID == 0)
                {
                    INStoreIssueNote.CompanyID = LoginUser.CompanyID;
                    INStoreIssueNote.CreatedAt = DateTime.Now;
                    INStoreIssueNote.CreatedBy = LoginUser.UsersID;
                    INStoreIssueNote.ModifiedAt = DateTime.Now;
                    INStoreIssueNote.ModifiedBy = LoginUser.UsersID;
                }
                else
                {
                    INStoreIssueNote.CompanyID = LoginUser.CompanyID;
                    INStoreIssueNote.ModifiedAt = DateTime.Now;
                    INStoreIssueNote.ModifiedBy = LoginUser.UsersID;
                }

                result = new DALInventory().INStoreIssueNoteSave(INStoreIssueNote);


                if (result == true)
                {
                    //save details
                    if (INStoreIssueNoteDetailList != null && INStoreIssueNoteDetailList.Count > 0)
                    {
                        foreach (var item in INStoreIssueNoteDetailList)
                        {
                            item.StoreIssueNoteID = INStoreIssueNote.StoreIssueNoteID;

                            if (item.StoreIssueNoteDetailID == 0)
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
                            new DALInventory().INStoreIssueNoteDetailSave(item);
                        }
                    }
                    //end save details

                    res.resObj = INStoreIssueNote;
                    res.id = INStoreIssueNote.StoreIssueNoteID;
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


        [HttpGet]
        public ActionResult INStoreIssueNoteDetailDelete(long id)
        {
            try
            {
                //GLVoucherViewModel model = new GLVoucherViewModel();

                new DALInventory().INStoreIssueNoteDetailDelete(id);
                return RedirectToAction("StoreIssueNote");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult INStoreIssueNotePost(long id)
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

                dal.StoreIssueNotePost(id);

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


        [HttpGet]
        public ActionResult INStoreIssueNoteUnPost(long id)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                if (LoginUser.RoleID != 5)
                {
                    return new HttpStatusCodeResult(403, "Only GM can revert posting.");
                }

                GL.Models.response res = new GL.Models.response();
                bool result = false;
                DALInventory dal = new DALInventory();

                dal.StoreIssueNoteUnPost(id);

                return RedirectToAction("StoreIssueNoteList");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region StoreReturnNote


        [HttpGet]
        public ActionResult StoreReturnNoteList()
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INStoreReturnNoteViewModel model = new INStoreReturnNoteViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.INGroupList = new DALDropdowns().INGroupList(LoginUser.CompanyID);
                ViewBag.INItems = new DALDropdowns().INItemsList(LoginUser.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult StoreReturnNoteSearchList(DateTime? FromDate, DateTime? ToDate, long? ItemID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            INStoreReturnNoteViewModel model = new INStoreReturnNoteViewModel();

            model.INStoreReturnNoteSearchList = new DALInventory().GetINStoreReturnNoteSearchList(LoginUser.CompanyID, LoginUser.UsersID, FromDate, ToDate, ItemID );
            model.RoleID = LoginUser.RoleID;
            return View("_StoreReturnNoteSearchListRows", model);

        }

        [HttpGet]
        public ActionResult StoreReturnNote(int? id)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INStoreReturnNoteViewModel model = new INStoreReturnNoteViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.Projects = dalDropdowns.INProjectsList(LoginUser.CompanyID);
                ViewBag.INStores = dalDropdowns.INStoreList(LoginUser.CompanyID);
                ViewBag.INItems = dalDropdowns.INItemsList(LoginUser.CompanyID);

                if (id != null || id > 0)
                {
                    model.INStoreReturnNote = new DALInventory().INStoreReturnNote(id.GetValueOrDefault(0));
                    model.INStoreReturnNoteDetailRows = new DALInventory().GetINStoreReturnNoteDetailRows(id.GetValueOrDefault(0));

                }
                else
                {
                    model.INStoreReturnNote = new INStoreReturnNote();
                    model.INStoreReturnNote.StoreReturnNoteID = 0;
                    model.INStoreReturnNote.StoreReturnNoteDate = DateTime.Now;
                    model.INStoreReturnNote.ProjectID = 0;
                    model.INStoreReturnNote.Remarks = string.Empty;
                    model.INStoreReturnNote.IsPosted = false;
                    model.INStoreReturnNote.CreatedBy = 0;
                    model.INStoreReturnNote.CreatedAt = null;
                    model.INStoreReturnNote.ModifiedBy = 0;
                    model.INStoreReturnNote.ModifiedAt = null;
                    model.INStoreReturnNote.CompanyID = LoginUser.CompanyID;


                    model.IsNew = 1;

                    model.INStoreReturnNoteDetailRows = new List<spINStoreReturnNoteDetailRows_Result>();
                }
                model.RoleID = LoginUser.RoleID;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetStoreReturnNoteItemRow(long ItemID)
        {
            try
            {
                var ItemRow = new DALInventory().GetStoreReturnNoteItemRow(ItemID);
                return Json(ItemRow, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult INStoreReturnNoteSubmit(int RequestID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();

                bool result = new DALInventory().INPurchaseRequisitionSubmit(RequestID, LoginUser.UsersID, LoginUser.RoleID);

                //res.resObj = glVoucher;
                res.id = RequestID;
                res.status = true;
                res.resMessage = "Record submitted successfully!";

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult INStoreReturnNoteSave(INStoreReturnNote INStoreReturnNote, List<INStoreReturnNoteDetail> INStoreReturnNoteDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();
                var dal = new DALInventory();
                bool result = false;
                if (INStoreReturnNote.StoreReturnNoteID == 0)
                {
                    INStoreReturnNote.CompanyID = LoginUser.CompanyID;
                    INStoreReturnNote.CreatedAt = DateTime.Now;
                    INStoreReturnNote.CreatedBy = LoginUser.UsersID;
                    INStoreReturnNote.ModifiedAt = DateTime.Now;
                    INStoreReturnNote.ModifiedBy = LoginUser.UsersID;
                }
                else
                {
                    INStoreReturnNote.CompanyID = LoginUser.CompanyID;
                    INStoreReturnNote.ModifiedAt = DateTime.Now;
                    INStoreReturnNote.ModifiedBy = LoginUser.UsersID;
                }

                result = new DALInventory().INStoreReturnNoteSave(INStoreReturnNote);


                if (result == true)
                {
                    //save details
                    if (INStoreReturnNoteDetailList != null && INStoreReturnNoteDetailList.Count > 0)
                    {
                        foreach (var item in INStoreReturnNoteDetailList)
                        {
                            item.StoreReturnNoteID = INStoreReturnNote.StoreReturnNoteID;

                            if (item.StoreReturnNoteDetailID == 0)
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
                            new DALInventory().INStoreReturnNoteDetailSave(item);
                        }
                    }
                    //end save details

                    res.resObj = INStoreReturnNote;
                    res.id = INStoreReturnNote.StoreReturnNoteID;
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


        [HttpGet]
        public ActionResult INStoreReturnNoteDetailDelete(long id)
        {
            try
            {
                //GLVoucherViewModel model = new GLVoucherViewModel();

                new DALInventory().INStoreReturnNoteDetailDelete(id);
                return RedirectToAction("StoreReturnNote");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult INStoreReturnNotePost(long id)
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

                dal.StoreReturnNotePost(id);

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


        [HttpGet]
        public ActionResult INStoreReturnNoteUnPost(long id)
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

                dal.StoreReturnNoteUnPost(id);

                return RedirectToAction("StoreReturnNoteList");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion


        #region StoreTransferNote

        [HttpPost]
        public JsonResult StoreTransferNoteApprove(int StoreTransferNoteID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();

                bool result = new DALInventory().INStoreTransferNoteApprove(StoreTransferNoteID, LoginUser.UsersID, LoginUser.RoleID);

                //res.resObj = glVoucher;
                res.id = StoreTransferNoteID;
                res.status = true;
                res.resMessage = "Record approved successfully!";

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult StoreTransferNoteReceive(int StoreTransferNoteID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();

                bool result = new DALInventory().INStoreTransferNoteReceive(StoreTransferNoteID, LoginUser.UsersID);

                res.id = StoreTransferNoteID;
                res.status = true;
                res.resMessage = "Received successfully!";

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult StoreTransferNoteDetailDelete(long id)
        {
            try
            {
                //GLVoucherViewModel model = new GLVoucherViewModel();

                new DALInventory().INStoreTransferNoteDetailDelete(id);
                return RedirectToAction("StoreTransferNote");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult StoreTransferNoteList()
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INStoreTransferNoteViewModel model = new INStoreTransferNoteViewModel();
                ViewBag.INItems = new DALDropdowns().INItemsList(LoginUser.CompanyID);
                // DALDropdowns dalDropdowns = new DALDropdowns();

                // ViewBag.INGroupList = new DALDropdowns().INGroupList(LoginUser.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult StoreTransferNoteSearchList(DateTime? FromDate, DateTime? ToDate, long? ItemID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            INStoreTransferNoteViewModel model = new INStoreTransferNoteViewModel();

            //model.INStoreTransferNoteSearchList = new DALInventory().GetINStoreTransferNoteSearchList(LoginUser.CompanyID, LoginUser.UsersID, FromDate, ToDate, ItemID);
            model.INStoreTransferNoteSearchList = new DALInventory().GetINStoreTransferNoteSearchList(LoginUser.CompanyID, null, FromDate, ToDate, ItemID);
            model.RoleID = LoginUser.RoleID;
            return View("_StoreTransferNoteSearchListRows", model);

        }

        [HttpGet]
        public ActionResult StoreTransferNote(int? id)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INStoreTransferNoteViewModel model = new INStoreTransferNoteViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.Projects = dalDropdowns.INProjectsList(LoginUser.CompanyID);
                ViewBag.INRequestTypes = dalDropdowns.INRequestTypesList(LoginUser.CompanyID);
                ViewBag.INStores = dalDropdowns.INStoreList(LoginUser.CompanyID);

                List<spINGRNItemsDropdown_Result> INItems = new List<spINGRNItemsDropdown_Result>();
                List<spINGRNItemsDropdown_Result> remainingINItems = new List<spINGRNItemsDropdown_Result>();
                if (id != null || id > 0)
                {
                    model.INStoreTransferNote = new DALInventory().INStoreTransferNote(id.GetValueOrDefault(0));

                    model.INStoreTransferNoteDetailRows = new DALInventory().GetINStoreTransferNoteDetailRows(id.GetValueOrDefault(0));

                    INItems = dalDropdowns.GetINGRItemsDropdown(LoginUser.CompanyID, model.INStoreTransferNote.ToProjectID.GetValueOrDefault(0));

                    foreach (var item in INItems)
                    {
                        var itemId = item.ItemID;
                        string[] parts = itemId.Split(',');
                        string requestDetailID = parts.Length > 0 ? parts[0] : null;
                        string itemID = parts.Length > 1 ? parts[1] : null;

                        foreach (var grn in model.INStoreTransferNoteDetailRows)
                        {
                            if (grn.RequestDetailID == Convert.ToInt64(requestDetailID) && grn.ItemID == Convert.ToInt64(itemID))
                            {
                                remainingINItems.Add(item);

                            }
                            if (grn.RequestDetailID != Convert.ToInt64(requestDetailID) && grn.ItemID != Convert.ToInt64(itemID) && item.Balance > 0)
                            {
                                remainingINItems.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    model.INStoreTransferNote = new INStoreTransferNote();
                    model.INStoreTransferNote.StoreTransferNoteID = 0;
                    model.INStoreTransferNote.StoreTransferNoteDate = DateTime.Now;
                    model.INStoreTransferNote.FromProjectID = 0;
                    model.INStoreTransferNote.ToProjectID = 0;
                    model.INStoreTransferNote.RefOGPNo = string.Empty;
                    model.INStoreTransferNote.Remarks = string.Empty;
                    model.INStoreTransferNote.CreatedBy = 0;
                    model.INStoreTransferNote.CreatedAt = DateTime.Now;

                    model.INStoreTransferNote.ModifiedBy = 0;
                    model.INStoreTransferNote.ModifiedAt = DateTime.Now;
                    model.INStoreTransferNote.ApprovedByID = 0;
                    model.INStoreTransferNote.ApprovedByAt = DateTime.Now;

                    model.INStoreTransferNote.CompanyID = LoginUser.CompanyID;


                    model.IsNew = 1;

                    model.INStoreTransferNoteDetailRows = new List<spINStoreTransferNoteDetailRows_Result>();
                }
                ViewBag.INItems = remainingINItems.Count == 0 ? INItems.Where(x => x.Balance > 0).ToList() : remainingINItems;
                model.RoleID = LoginUser.RoleID;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult INStoreTransferNoteSave(INStoreTransferNote INStoreTransferNote, List<INStoreTransferNoteDetail> INStoreTransferNoteDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();
                var dal = new DALInventory();
                bool result = false;
                if (INStoreTransferNote.StoreTransferNoteID == 0)
                {
                    INStoreTransferNote.CompanyID = LoginUser.CompanyID;
                    INStoreTransferNote.CreatedAt = DateTime.Now;
                    INStoreTransferNote.CreatedBy = LoginUser.UsersID;
                    INStoreTransferNote.ModifiedAt = DateTime.Now;
                    INStoreTransferNote.ModifiedBy = LoginUser.UsersID;
                }
                else
                {
                    INStoreTransferNote.ModifiedAt = DateTime.Now;
                    INStoreTransferNote.ModifiedBy = LoginUser.UsersID;
                }

                result = new DALInventory().INStoreTransferNoteSave(INStoreTransferNote);

                if (result == true)
                {
                    // save details
                    if (INStoreTransferNoteDetailList != null && INStoreTransferNoteDetailList.Count > 0)
                    {
                        foreach (var item in INStoreTransferNoteDetailList)
                        {
                            item.StoreTransferNoteID = INStoreTransferNote.StoreTransferNoteID;

                            if (item.StoreTransferNoteDetailID == 0)
                            {
                                item.CreatedBy = LoginUser.UsersID;
                                item.CreatedAt = DateTime.Now;
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                                item.CompanyID = LoginUser.CompanyID;
                            }
                            else
                            {
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                            }
                            new DALInventory().INStoreTransferNoteDetailSave(item);
                        }
                    }
                    // end save details

                    res.id = INStoreTransferNote.StoreTransferNoteID;
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


        #region PurchaseOrder


        [HttpGet]
        public ActionResult INPurchaseOrderDetailDelete(long id)
        {
            try
            {
                //GLVoucherViewModel model = new GLVoucherViewModel();

                new DALInventory().PurchaseOrderDetailDelete(id);
                return RedirectToAction("PurchaseOrder");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult PurchaseOrderList()
        {
            try
            {

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                INPurchaseOrderViewModel model = new INPurchaseOrderViewModel();
                // DALDropdowns dalDropdowns = new DALDropdowns();

                // ViewBag.INGroupList = new DALDropdowns().INGroupList(LoginUser.CompanyID);
                ViewBag.username = LoginUser.username;
                ViewBag.INItems = new DALDropdowns().INItemsList(LoginUser.CompanyID);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PurchaseOrderSearchList(DateTime? FromDate, DateTime? ToDate, long? ItemID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            var model = new INPurchaseOrderViewModel();

            if(LoginUser.username == "ind.umer" || LoginUser.username == "ind.shafeeq")
            {
                ViewBag.username = "ind.shafeeq";
                //model.INPurchaseOrderSearchList = new DALInventory().GetINPurchaseOrderSearchList(LoginUser.CompanyID, LoginUser.UsersID, FromDate, ToDate);
                model.INPurchaseOrderSearchList = new DALInventory().GetINPurchaseOrderSearchList(LoginUser.CompanyID, LoginUser.UsersID, FromDate, ToDate, ItemID);
            }
            else
            {
                model.INPurchaseOrderSearchList = new DALInventory().GetINPurchaseOrderSearchListForInventory(LoginUser.CompanyID, null, FromDate, ToDate, ItemID).Where(x => x.Approved > 0).ToList();
            }
            
            model.RoleID = LoginUser.RoleID;
            ViewBag.INItems = new DALDropdowns().INItemsList(LoginUser.CompanyID);
            return View("_PurchaseOrderListRows", model);
        }

        [HttpGet]
        public ActionResult PurchaseOrder(Int64? PurchaseOrderID)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }
                var model = new INPurchaseOrderViewModel();
                DALDropdowns dalDropdowns = new DALDropdowns();

                ViewBag.Projects = dalDropdowns.INProjectsList(LoginUser.CompanyID);
                ViewBag.INPOStatusList = dalDropdowns.INPOStatusList(LoginUser.CompanyID);
                ViewBag.INStores = dalDropdowns.INStoreList(LoginUser.CompanyID);

                //List<spINPRItemsDropdown_Result> INItems = new List<spINPRItemsDropdown_Result>();
                List<spINGRNItemsDropdown_Result> INItems = new List<spINGRNItemsDropdown_Result>();

                ViewBag.APVendors = dalDropdowns.GLAPVendorsListWithCode(LoginUser.CompanyID);
                List<spINGRNItemsDropdown_Result> remainingINItems = new List<spINGRNItemsDropdown_Result>();
                if (PurchaseOrderID != null || PurchaseOrderID > 0)
                {
                    model.INPurchaseOrder = new DALInventory().INPurchaseOrder(PurchaseOrderID.GetValueOrDefault(0));

                    model.INPurchaseOrderDetailRows = new DALInventory().GetINPurchaseOrderDetailRows(PurchaseOrderID.GetValueOrDefault(0));

                    INItems = dalDropdowns.GetINGRItemsDropdown(LoginUser.CompanyID, model.INPurchaseOrder.ProjectID.GetValueOrDefault(0));
                    //INItems = dalDropdowns.GetINPRItemsDropdown(model.INPurchaseOrder.RequestID.GetValueOrDefault(0));

                    foreach (var item in INItems)
                    {
                        var itemId = item.ItemID;
                        string[] parts = itemId.Split(',');
                        string requestDetailID = parts.Length > 0 ? parts[0] : null;
                        string itemID = parts.Length > 1 ? parts[1] : null;

                        foreach (var grn in model.INPurchaseOrderDetailRows)
                        {
                            if (grn.RequestDetailID == Convert.ToInt64(requestDetailID) && grn.ItemID == Convert.ToInt64(itemID))
                            {
                                remainingINItems.Add(item);

                            }
                            if (grn.RequestDetailID != Convert.ToInt64(requestDetailID) && grn.ItemID != Convert.ToInt64(itemID) && item.Balance > 0)
                            {
                                remainingINItems.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    model.INPurchaseOrder = new INPurchaseOrder()
                    {
                        //ApprovedAt = DateTime.Now,
                        //ApprovedBy = 0,
                        APVendorID = 0,
                        CompanyID = LoginUser.CompanyID,
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoginUser.UsersID,
                        GoodsReceiptNoteID = 0,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = LoginUser.UsersID,
                        POStatusID = 0,
                        ProjectID = 0,
                        PurchaseOrderDate = DateTime.Now,
                        PurchaseOrderID = 0,
                        Remarks = string.Empty,
                        RequestID = 0,
                        FreightCharges = 0

                    };

                    model.IsNew = 1;

                    model.INPurchaseOrderDetailRows = new List<spINPurchaseOrderDetailRows_Result>();
                }
                ViewBag.INItems = remainingINItems.Count == 0 ? INItems.Where(x => x.Balance > 0).ToList() : remainingINItems;
                model.RoleID = LoginUser.RoleID;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult INGetPurchaseRequestForPO(long RequestID)
        {
            try
            {
                GL.Models.response res = new GL.Models.response();
                var PurchaseRequisition = new DALInventory().INPurchaseRequisitionGet(RequestID);

                INPurchaseRequisitionModel purchaseRequisitionModel = null;
                if (PurchaseRequisition != null)
                {
                    purchaseRequisitionModel = new INPurchaseRequisitionModel()
                    {
                        RequestID = PurchaseRequisition.RequestID,
                        DocumentNo = PurchaseRequisition.DocumentNo,
                        ManualDemandNo = PurchaseRequisition.ManualDemandNo,
                        ProjectDocumentNo = PurchaseRequisition.ProjectDocumentNo,
                        ProjectID = PurchaseRequisition.ProjectID,
                        Remarks = PurchaseRequisition.Remarks,

                        CompanyID = PurchaseRequisition.CompanyID,
                        CreatedAt = PurchaseRequisition.CreatedAt,
                        CreatedBy = PurchaseRequisition.CreatedBy,

                        ModifiedAt = PurchaseRequisition.ModifiedAt,
                        ModifiedBy = PurchaseRequisition.ModifiedBy,
                        RequestDate = PurchaseRequisition.RequestDate,

                        RequestTypeID = PurchaseRequisition.RequestTypeID,
                        SubmitedAtKPO = PurchaseRequisition.SubmitedAtKPO,
                        SubmitedAtMD = PurchaseRequisition.SubmitedAtMD,
                        SubmitedByKPO = PurchaseRequisition.SubmitedByKPO,
                        SubmitedByMD = PurchaseRequisition.SubmitedByMD

                    };

                }

                res.status = true;
                res.resObj = purchaseRequisitionModel;
                res.resMessage = "Request found";

                return Content(JsonConvert.SerializeObject(res), "application/json");


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult INGetPurchaseRequestDetailRowsForPO(int RequestID)
        {
            try
            {
                GL.Models.response res = new GL.Models.response();
                var INPurchaseRequisitionDetailRows = new DALInventory().GetINPurchaseRequisitionDetailRows(RequestID);

                res.status = true;
                res.resObj = INPurchaseRequisitionDetailRows;
                res.resMessage = "Request found";

                return Content(JsonConvert.SerializeObject(res), "application/json");


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetINPRDropdownItems(long RequestID)
        {
            try
            {
                List<spINPRItemsDropdown_Result> INPRDropdownItems = new DALDropdowns().GetINPRItemsDropdown(RequestID).ToList();
                return Json(INPRDropdownItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetPRItemRowForPO(long ItemID, long RequestDetailID)
        {
            try
            {
                var PRItemRowForPO = new DALInventory().GetPRItemRowForPO(ItemID, RequestDetailID);
                return Json(PRItemRowForPO, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult INPurchaseOrderSave(INPurchaseOrder INPurchaseOrder, List<INPurchaseOrderDetail> INPurchaseOrderDetailList)
        {
            try
            {
                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
                }
                GL.Models.response res = new GL.Models.response();
                var dal = new DALInventory();
                bool result = false;
                if (INPurchaseOrder.PurchaseOrderID == 0)
                {
                    INPurchaseOrder.CompanyID = LoginUser.CompanyID;
                    INPurchaseOrder.CreatedAt = DateTime.Now;
                    INPurchaseOrder.CreatedBy = LoginUser.UsersID;
                    INPurchaseOrder.POStatusID = 1;
                }
                else
                {
                    INPurchaseOrder.ModifiedAt = DateTime.Now;
                    INPurchaseOrder.ModifiedBy = LoginUser.UsersID;
                }
                //INPurchaseRequisition.CompanyID = LoginUser.CompanyID;

                result = new DALInventory().INPurchaseOrderSave(INPurchaseOrder);

                if (result == true)
                {
                    // save details
                    if (INPurchaseOrderDetailList != null && INPurchaseOrderDetailList.Count > 0)
                    {
                        foreach (var item in INPurchaseOrderDetailList)
                        {
                            item.PurchaseOrderID = INPurchaseOrder.PurchaseOrderID;

                            if (item.PurchaseOrderDetailID == 0)
                            {
                                item.CreatedBy = LoginUser.UsersID;
                                item.CreatedAt = DateTime.Now;
                                item.CompanyID = LoginUser.CompanyID;
                            }
                            else
                            {
                                //item.CompanyID = LoginUser.CompanyID;
                                item.ModifiedBy = LoginUser.UsersID;
                                item.ModifiedAt = DateTime.Now;
                            }
                            //item.Balance
                            new DALInventory().INPurchaseOrderDetailSave(item, INPurchaseOrder.ProjectID.GetValueOrDefault(0));
                        }
                    }
                    // end save details

                    //res.resObj = glVoucher;
                    res.id = INPurchaseOrder.PurchaseOrderID;
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

        [HttpPost]
        public ActionResult GetPurchaseOrderMaster(long PurchaseOrderID)
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

                var INPurchaseOrder = dal.GetPurchaseOrderMaster(PurchaseOrderID);


                if (INPurchaseOrder != null)
                {
                    INPurchaseOrder.CancelledBy = INPurchaseOrder.CancelledBy == null ? 0 : INPurchaseOrder.CancelledBy;
                    if (INPurchaseOrder.CancelledBy <= 0) {
                        res.resMessage = "Record Found!";
                        res.status = true;
                    }
                    else
                    {
                        res.resMessage = "This PO is already cancelled!";
                        res.status = false;

                    }
                    res.resObj = INPurchaseOrder;
                    //res.resMessage = "Record Found!";
                }
                else
                {
                    res.resObj = null;
                    res.status = false;
                    res.resMessage = "Record not found!";

                }

                return Content(JsonConvert.SerializeObject(res), "application/json");
                //return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult POUnApprove(long id)
        {
            try
            {

                bool result = new DALInventory().POUnApprove(id);

               

                return RedirectToAction("PurchaseOrderList");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        [HttpPost]
        public JsonResult POCancel(Int64 INPurchaseOrderID)
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

                result = dal.POCancel(INPurchaseOrderID, LoginUser.UsersID);

                if (result == true)
                {
                    res.id = INPurchaseOrderID;
                    res.status = true;
                    res.resMessage = "PO cancelled successfully!";
                }
                else
                {
                    res.resObj = null;
                    res.status = false;
                    res.resMessage = "Record was not cancelled. Please contact to the admin!";

                }

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult POApprove(Int64 INPurchaseOrderID)
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

                result = dal.POApprove(INPurchaseOrderID, LoginUser.UsersID);

                if (result == true)
                {
                    res.id = INPurchaseOrderID;
                    res.status = true;
                    res.resMessage = "Record approved successfully!";
                }
                else
                {
                    res.resObj = null;
                    res.status = false;
                    res.resMessage = "Record was not approved. Please contact to the admin!";

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