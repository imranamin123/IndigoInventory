using GL.Common;
using GL.EF;
using GL.Models;
using GL.Reports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using WebGrease.Css.Ast;

namespace GL.DAL
{
    public class DALInventory
    {
        GLEntities db = new GLEntities();

        //  public bool StoreItemEntryAllowed { get; private set; }

        #region Item
        public List<spINItemSearchList_Result> GetINItemSearchList(string ItemCode, string Name, int? GroupID)
        {
            try
            {
                List<spINItemSearchList_Result> INItemGetSearchList = db.spINItemSearchList(ItemCode, GroupID, Name).ToList();

                return INItemGetSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Int64 NextItemCodeByGroup(int GroupID)
        {
            try
            {
                long? maxItemID = db.INItems.Where(x => x.GroupID == GroupID).Max(x => (long?)x.ItemID);
                long NextItemCode = 0;
                var maxItem = maxItemID.HasValue ? db.INItems.FirstOrDefault(x => x.ItemID == maxItemID) : null;
                if (maxItem != null)
                {
                    NextItemCode = maxItem.ItemCode.GetValueOrDefault(0) + 1;
                }
                else
                {
                    NextItemCode = 1;
                }

                return NextItemCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsDuplicateGroupSize(int CompanyID, int GroupID, int SizeID, long ItemID)
        {
            try
            {
                INItem item = null;
                if (ItemID == 0)
                {
                    item = db.INItems.Where(x => x.CompanyID == CompanyID && x.SizeID == SizeID && x.GroupID == GroupID).FirstOrDefault();
                    if (item == null)
                        return false;
                    else
                        return true;

                }
                else
                {
                    item = db.INItems.Where(x => x.CompanyID == CompanyID && x.SizeID == SizeID && x.GroupID == GroupID && x.ItemID != ItemID).FirstOrDefault();
                    if (item == null)
                        return false;
                    else
                        return true;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public INItem INItemGet(long ItemID)
        {
            try
            {
                var NTItem = db.INItems.Where(x => x.ItemID == ItemID).FirstOrDefault();
                return NTItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INItemSave(INItem INItem)
        {
            try
            {
                if (INItem != null)
                {
                    db.INItems.AddOrUpdate(INItem);
                    db.SaveChanges();

                    // insert project items 
                    var projectItems = new List<INProjectItem>();
                    var projects = db.INProjects.ToList();

                    foreach (var project in projects)
                    {
                        var projectItem = db.INProjectItems.Where(x => x.ProjectID == project.ProjectID && x.ItemID == INItem.ItemID).FirstOrDefault();
                        if (projectItem == null)
                        {
                            projectItem = new INProjectItem();
                            projectItem.ProjectID = project.ProjectID;
                            projectItem.ItemID = INItem.ItemID;
                            projectItem.OpeningQty = 0;
                            projectItem.QtyInHand = 0;
                            projectItem.LastRate = 0;
                            projectItem.CompanyID = INItem.CompanyID;

                            projectItems.Add(projectItem);

                        }
                    }
                    if (projectItems.Count > 0)
                    {
                        db.INProjectItems.AddRange(projectItems);
                        db.SaveChanges();
                    }
                    //
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool CopyFromINItemToProjectItem()
        {
            try
            {
                var items = db.INItems.ToList();
                foreach (var item in items)
                {
                    var projectItemISA = new INProjectItem()
                    {
                        CompanyID = item.CompanyID,
                        ItemID = item.ItemID,
                        ////LastRate=item.ISALastRate,
                        ////OpeningQty=item.ISAOpeningQty,
                        ////QtyInHand=item.ISAQtyInHand,
                    };
                    var projectItemIBA = new INProjectItem()
                    {
                        CompanyID = item.CompanyID,
                        ItemID = item.ItemID,
                        ////LastRate = item.IBALastRate,
                        ////OpeningQty = 0,
                        ////QtyInHand = item.IBAQtyInHand
                    };

                    db.INProjectItems.AddOrUpdate(projectItemISA);
                    db.INProjectItems.AddOrUpdate(projectItemIBA);
                    db.SaveChanges();

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsDuplicateSizeDescription(INItem INItem)
        {
            try
            {
                INItem item = null;
                bool IsDuplicate = false;
                //if (INItem.ItemID == 0)
                //{
                item = db.INItems.Where(x => x.CompanyID == INItem.CompanyID && x.SizeID == INItem.SizeID && x.Description == INItem.Description && x.ItemID != INItem.ItemID).FirstOrDefault();
                if (item == null)
                    IsDuplicate = false;
                else
                    IsDuplicate = true;

                //}

                return IsDuplicate;

                //else
                //{
                //    item = db.INItems.Where(x => x.CompanyID == CompanyID && x.SizeID == SizeID && x.GroupID == GroupID && x.ItemID != ItemID).FirstOrDefault();
                //    if (item == null)
                //        return false;
                //    else
                //        return true;

                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region PurchaseRequisition


        public INProject GetProjectNameByID(int ProjectID)
        {
            try
            {
                var Project = db.INProjects.Where(x => x.ProjectID == ProjectID).FirstOrDefault();//  db.INPurchaseRequisitions.Where(x => x.ProjectID == ProjectID).Max(x => (long?)x.DocumentNo);

                return Project;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Int64 NextDocumentNumberByProject(int ProjectID)
        {
            try
            {
                var maxNo = db.INPurchaseRequisitions.Where(x => x.ProjectID == ProjectID).Max(x => (long?)x.DocumentNo);
                long NextNo = 0;
                var maxItem = maxNo.HasValue ? db.INPurchaseRequisitions.FirstOrDefault(x => x.DocumentNo == maxNo) : null;
                if (maxItem != null)
                {
                    NextNo = maxItem.DocumentNo.GetValueOrDefault(0) + 1;
                }
                else
                {
                    NextNo = 1;
                }

                return NextNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINPurchaseRequisitionSearchList_Result> GetINPurchaseRequisitionSearchList(int CompanyID, int? UserID, DateTime? RequestDateFrom, DateTime? RequestDateTo, long? ItemID)
        {
            try
            {
                var user = db.SecUsers.Where(x => x.CompanyID == CompanyID && x.RoleID == 5 && x.UsersID == UserID).FirstOrDefault();

                if (user != null && user.RoleID == 5) UserID = null;

                List<spINPurchaseRequisitionSearchList_Result> INPurchaseRequisitionGetSearchList = db.spINPurchaseRequisitionSearchList(CompanyID, UserID, RequestDateFrom, RequestDateTo, ItemID).ToList();

                return INPurchaseRequisitionGetSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINPurchaseRequisitionSearchList_Result> GetINPurchaseRequisitionSearchPurchaseHeadList(int CompanyID, int? UserID, DateTime? RequestDateFrom, DateTime? RequestDateTo, long? ItemID)
        {
            try
            {

                List<spINPurchaseRequisitionSearchList_Result> INPurchaseRequisitionGetSearchList = db.spINPurchaseRequisitionSearchList(CompanyID, UserID, RequestDateFrom, RequestDateTo, ItemID).Where(x => x.SubmitedByKPO > 0).ToList();

                return INPurchaseRequisitionGetSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public INPurchaseRequisition INPurchaseRequisitionGet(long PurchaseRequisitionID)
        {
            try
            {
                var NTPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == PurchaseRequisitionID).FirstOrDefault();
                return NTPurchaseRequisition;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINPurchaseRequisitionDetailRows_Result> GetINPurchaseRequisitionDetailRows(int requestID)
        {
            try
            {
                var INPurchaseRequisitionRows = db.spINPurchaseRequisitionDetailRows(requestID).ToList();

                return INPurchaseRequisitionRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINPurchaseOrderDetailRows_Result> GetINPurchaseOrderDetailRows(Int64 requestID)
        {
            try
            {
                var INPurchaseOrderDetailRows = db.spINPurchaseOrderDetailRows(Convert.ToInt32(requestID)).ToList();

                return INPurchaseOrderDetailRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public spINGetItemRow_Result GetINGetItemRow(int ProjectID, Int64 ItemID)
        {
            try
            {
                spINGetItemRow_Result INGetItemRow = db.spINGetItemRow(ProjectID, ItemID).FirstOrDefault();

                return INGetItemRow;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public spINGetGRNItemRow_Result GetINGRNItemRow(Int64 ItemID, Int64 RequestDetailID)
        {
            try
            {
                var INGetGRNItemRow = db.spINGetGRNItemRow(ItemID, RequestDetailID).FirstOrDefault();

                return INGetGRNItemRow;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<spRptINItemStock_Result> GetRptINItemStock(int CompanyID, int ProjectID, int? ItemID, DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                var RptINItemStock = db.spRptINItemStock(CompanyID, ProjectID, ItemID, FromDate, ToDate).ToList();

                return RptINItemStock;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region "Dashboard"

        public spRptINDashboardKPI_Result GetDashboardKPI(int CompanyID, int ProjectID, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return db.spRptINDashboardKPI(CompanyID, ProjectID, FromDate, ToDate).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spRptINDashboardStockByCategory_Result> GetDashboardStockByCategory(int CompanyID, int ProjectID)
        {
            try
            {
                return db.spRptINDashboardStockByCategory(CompanyID, ProjectID).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spRptINDashboardTopItemsByValue_Result> GetDashboardTopItemsByValue(int CompanyID, int ProjectID, int TopN)
        {
            try
            {
                return db.spRptINDashboardTopItemsByValue(CompanyID, ProjectID, TopN).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spRptINDashboardItemMovement_Result> GetDashboardItemMovement(int CompanyID, int ProjectID, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return db.spRptINDashboardItemMovement(CompanyID, ProjectID, FromDate, ToDate).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spRptINDashboardMonthlyTrend_Result> GetDashboardMonthlyTrend(int CompanyID, int ProjectID, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return db.spRptINDashboardMonthlyTrend(CompanyID, ProjectID, FromDate, ToDate).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spRptINDashboardRecentActivity_Result> GetDashboardRecentActivity(int CompanyID, int ProjectID, int TopN)
        {
            try
            {
                return db.spRptINDashboardRecentActivity(CompanyID, ProjectID, TopN).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public spINGetPRItemRowForPO_Result GetPRItemRowForPO(Int64 ItemID, Int64 RequestDetailID)
        {
            try
            {
                var PRItemRowForPO = db.spINGetPRItemRowForPO(ItemID, RequestDetailID).FirstOrDefault();

                return PRItemRowForPO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public spINStoreIssueNoteItemRow_Result GetStoreIssueNoteItemRow(Int64 ItemID)
        {
            try
            {
                var ItemRow = db.spINStoreIssueNoteItemRow(ItemID).FirstOrDefault();

                return ItemRow;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INPurchaseRequisitionSubmit(int RequestID, int UserID, int RoleID)
        {
            try
            {
                var INPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == RequestID).FirstOrDefault();
                var role = db.SecRoles.Where(x => x.RoleID == RoleID).FirstOrDefault();
                if (role.RoleID == 4 || role.RoleID == 5)
                {
                    if (role.RoleID == 4) // KPO
                    {
                        INPurchaseRequisition.SubmitedByKPO = UserID;
                        INPurchaseRequisition.SubmitedAtKPO = DateTime.Now;
                    }
                    else if (role.RoleID == 5)
                    {
                        INPurchaseRequisition.SubmitedByMD = UserID;
                        INPurchaseRequisition.SubmitedAtMD = DateTime.Now;
                    }
                    db.INPurchaseRequisitions.AddOrUpdate(INPurchaseRequisition);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INPurchaseRequisitionCancell(int RequestID, int UserID, int RoleID)
        {
            try
            {
                var INPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == RequestID).FirstOrDefault();
                var role = db.SecRoles.Where(x => x.RoleID == RoleID).FirstOrDefault();
                if (role.RoleID == 4 || role.RoleID == 5)
                {
                    if (role.RoleID == 5)
                    {
                        INPurchaseRequisition.CancelledBy = UserID;
                        INPurchaseRequisition.CancelledAt = DateTime.Now;
                    }
                    db.INPurchaseRequisitions.AddOrUpdate(INPurchaseRequisition);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INPurchaseRequisitionSave(INPurchaseRequisition INPurchaseRequisition)
        {
            try
            {
                if (INPurchaseRequisition != null)
                {
                    db.INPurchaseRequisitions.AddOrUpdate(INPurchaseRequisition);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INPurchaseRequisitionDetailSave(INPurchaseRequisitionDetail INPurchaseRequisitionDetail)
        {
            try
            {
                if (INPurchaseRequisitionDetail != null)
                {
                    db.INPurchaseRequisitionDetails.AddOrUpdate(INPurchaseRequisitionDetail);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INPurchaseRequisitionDetailDelete(long RequestDetailID)
        {
            try
            {
                var INPurchaseRequisitionDetail = db.INPurchaseRequisitionDetails.Where(x => x.RequestDetailID == RequestDetailID).FirstOrDefault();
                db.INPurchaseRequisitionDetails.Remove(INPurchaseRequisitionDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool INPurchaseRequisitionUnSubmitByGM(long id)
        {
            try
            {
                var INPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == id).FirstOrDefault();
                if(INPurchaseRequisition.CancelledBy > 0)
                {
                    INPurchaseRequisition.CancelledBy = 0;
                    INPurchaseRequisition.CancelledAt = null;
                }
                if(INPurchaseRequisition.SubmitedByMD > 0)
                {
                    INPurchaseRequisition.SubmitedByMD = 0;
                    INPurchaseRequisition.SubmitedAtMD = null;
                }
                
                db.INPurchaseRequisitions.AddOrUpdate(INPurchaseRequisition);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GoodsReceiptNote

        public bool INGoodsReceiptNoteUnPost(long id)
        {
            try
            {
                var INPurchaseRequisition = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == id).FirstOrDefault();
                INPurchaseRequisition.IsPosted = false;
                db.INGoodsReceiptNotes.AddOrUpdate(INPurchaseRequisition);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<spINGoodsReceiptNoteSearchList_Result> GetINGoodsReceiptNoteSearchList(int CompanyID, int? UserID, DateTime? FromDate, DateTime? ToDate, long? ItemID)
        {
            try
            {
                var user = db.SecUsers.Where(x => x.CompanyID == CompanyID && x.UsersID == UserID).FirstOrDefault();

                if (user != null && user.RoleID == 5) UserID = null;

                List<spINGoodsReceiptNoteSearchList_Result> INGoodsReceiptNoteSearchList = db.spINGoodsReceiptNoteSearchList(CompanyID, UserID, FromDate, ToDate, ItemID).ToList();

                return INGoodsReceiptNoteSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<spINGoodsReceiptNoteSearchList_Result> GetINGoodsReceiptNotePurchaseHeadSearchList(int CompanyID, int? UserID, DateTime? FromDate, DateTime? ToDate,long? ItemID, int? Pending = 0)
        {
            try
            {
                var INGoodsReceiptNoteSearchList = new List<spINGoodsReceiptNoteSearchList_Result>();
                if (Pending == 0)
                {
                    INGoodsReceiptNoteSearchList = db.spINGoodsReceiptNoteSearchList(CompanyID, UserID, FromDate, ToDate, ItemID).Where(x => x.IsPosted == true).ToList();
                }
                else
                {
                    INGoodsReceiptNoteSearchList = GetINGoodsReceiptNoteWithoutPOSearchList(CompanyID, UserID, FromDate, ToDate).ToList();
                }
                return INGoodsReceiptNoteSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<spINGoodsReceiptNoteSearchList_Result> GetINGoodsReceiptNoteWithoutPOSearchList(int CompanyID, int? UserID, DateTime? FromDate, DateTime? ToDate, int? Pending = 0)
        {
            try
            {
                var withoutPOList = db.spINGoodsReceiptNoteWithoutPOSearchList(CompanyID, UserID, FromDate, ToDate).ToList();

                List<spINGoodsReceiptNoteSearchList_Result> INGoodsReceiptNoteSearchList = withoutPOList.Select(x => new spINGoodsReceiptNoteSearchList_Result
                {
                    GoodsReceiptNoteID = x.GoodsReceiptNoteID,
                    GoodsReceiptNotesDate = x.GoodsReceiptNotesDate,
                    APVendorID = x.APVendorID,
                    ProjectName = x.ProjectName,
                    Remarks = x.Remarks,
                    IsPosted = x.IsPosted,
                    CompanyID = x.CompanyID,

                    // PurchaseOrderID does not exist → set null
                    PurchaseOrderID = null
                }).ToList();

                return INGoodsReceiptNoteSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public INGoodsReceiptNote INGoodsReceiptNote(long GoodsReceiptNoteID)
        {
            try
            {
                var INGoodsReceiptNote = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == GoodsReceiptNoteID).FirstOrDefault();
                return INGoodsReceiptNote;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GRNPost(List<INGoodsReceiptNoteDetail> INGoodsReceiptNoteDetails)
        {
            try
            {
                foreach (var grnDetailItem in INGoodsReceiptNoteDetails)
                {


                    var INPurchaseRequisitionDetail = db.INPurchaseRequisitionDetails.Where(x => x.RequestDetailID == grnDetailItem.RequestDetailID && x.ItemID == grnDetailItem.ItemID).FirstOrDefault();
                    var INPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == INPurchaseRequisitionDetail.RequestID).FirstOrDefault();

                    INPurchaseRequisitionDetail.Balance = INPurchaseRequisitionDetail.Balance - grnDetailItem.ReceivedQty;

                    var INGoodsReceiptNote = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == grnDetailItem.GoodsReceiptNoteID).FirstOrDefault();
                    INGoodsReceiptNote.IsPosted = true;

                    db.SaveChanges();

                    // StoreItem

                    var goodsReceiptNote = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == grnDetailItem.GoodsReceiptNoteID).FirstOrDefault();

                    var projectItem = db.INProjectItems.Where(x => x.ItemID == grnDetailItem.ItemID && x.ProjectID == goodsReceiptNote.ProjectID).FirstOrDefault();
                    if (projectItem != null)
                    {
                        projectItem.QtyInHand += grnDetailItem.ReceivedQty;
                        projectItem.LastRate = grnDetailItem.Rate;
                        db.INProjectItems.AddOrUpdate(projectItem);
                        db.SaveChanges();
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public INPurchaseOrder GetPurchaseOrderMaster(long PurchaseOrderID)
        {
            try
            {
                var INPurchaseOrder = db.INPurchaseOrders.Where(x => x.PurchaseOrderID == PurchaseOrderID).FirstOrDefault();
                return INPurchaseOrder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool GRNUnPost(long id)
        {
            try
            {
                var INGoodsReceiptNoteDetails = db.INGoodsReceiptNoteDetails.Where(x => x.GoodsReceiptNoteID == id).ToList();
                foreach (var grnDetailItem in INGoodsReceiptNoteDetails)
                {

                    var INPurchaseRequisitionDetail = db.INPurchaseRequisitionDetails.Where(x => x.RequestDetailID == grnDetailItem.RequestDetailID && x.ItemID == grnDetailItem.ItemID).FirstOrDefault();
                    var INPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == INPurchaseRequisitionDetail.RequestID).FirstOrDefault();

                    INPurchaseRequisitionDetail.Balance = INPurchaseRequisitionDetail.Balance + grnDetailItem.ReceivedQty;

                    var INGoodsReceiptNote = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == grnDetailItem.GoodsReceiptNoteID).FirstOrDefault();
                    INGoodsReceiptNote.IsPosted = false;

                    db.SaveChanges();

                    // StoreItem
                    var goodsReceiptNote = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == grnDetailItem.GoodsReceiptNoteID).FirstOrDefault();

                    var projectItem = db.INProjectItems.Where(x => x.ItemID == grnDetailItem.ItemID && x.ProjectID == goodsReceiptNote.ProjectID).FirstOrDefault();
                    if (projectItem != null)
                    {
                        projectItem.QtyInHand -= grnDetailItem.ReceivedQty;
                        db.INProjectItems.AddOrUpdate(projectItem);
                        db.SaveChanges();
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public bool INStoreIssueNoteRevert(long id)
        //{
        //    try
        //    {
        //        var INGoodsReceiptNoteDetails = db.INGoodsReceiptNoteDetails.Where(x => x.GoodsReceiptNoteID == id).ToList();
        //        foreach (var grnDetailItem in INGoodsReceiptNoteDetails)
        //        {

        //            var INPurchaseRequisitionDetail = db.INPurchaseRequisitionDetails.Where(x => x.RequestDetailID == grnDetailItem.RequestDetailID && x.ItemID == grnDetailItem.ItemID).FirstOrDefault();
        //            var INPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == INPurchaseRequisitionDetail.RequestID).FirstOrDefault();

        //            INPurchaseRequisitionDetail.Balance = INPurchaseRequisitionDetail.Balance + grnDetailItem.ReceivedQty;

        //            var INGoodsReceiptNote = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == grnDetailItem.GoodsReceiptNoteID).FirstOrDefault();
        //            INGoodsReceiptNote.IsPosted = false;

        //            db.SaveChanges();

        //            // StoreItem
        //            var goodsReceiptNote = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == grnDetailItem.GoodsReceiptNoteID).FirstOrDefault();

        //            var projectItem = db.INProjectItems.Where(x => x.ItemID == grnDetailItem.ItemID && x.ProjectID == goodsReceiptNote.ProjectID).FirstOrDefault();
        //            if (projectItem != null)
        //            {
        //                projectItem.QtyInHand -= grnDetailItem.ReceivedQty;
        //                db.INProjectItems.AddOrUpdate(projectItem);
        //                db.SaveChanges();
        //            }

        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        public List<spINGoodsReceiptNoteDetailRows_Result> GetINGoodsReceiptNoteDetailRows(int GoodsReceiptNoteID)
        {
            try
            {
                var INGoodsReceiptNoteDetailRows = db.spINGoodsReceiptNoteDetailRows(GoodsReceiptNoteID).ToList();

                return INGoodsReceiptNoteDetailRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INGoodsReceiptNoteSave(INGoodsReceiptNote INGoodsReceiptNote)
        {
            try
            {
                if (INGoodsReceiptNote != null)
                {
                    db.INGoodsReceiptNotes.AddOrUpdate(INGoodsReceiptNote);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INGoodsReceiptNoteDetailSave(INGoodsReceiptNoteDetail INGoodsReceiptNoteDetail)
        {
            try
            {
                if (INGoodsReceiptNoteDetail != null)
                {
                    db.INGoodsReceiptNoteDetails.AddOrUpdate(INGoodsReceiptNoteDetail);
                    db.SaveChanges();

                    ////// StoreItem
                    ////if (StoreItemEntryAllowed)
                    ////{
                    ////    var goodsReceiptNote = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == INGoodsReceiptNoteDetail.GoodsReceiptNoteID).FirstOrDefault();

                    ////    var projectItem = db.INProjectItems.Where(x => x.ItemID == INGoodsReceiptNoteDetail.ItemID && x.ProjectID == goodsReceiptNote.ProjectID).FirstOrDefault();
                    ////    if (projectItem != null)
                    ////    {
                    ////        projectItem.QtyInHand += INGoodsReceiptNoteDetail.ReceivedQty;
                    ////        db.INProjectItems.AddOrUpdate(projectItem);
                    ////        db.SaveChanges();
                    ////    }

                    ////}
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public spINGetItemRow_Result GetINGetItemRow(Int64 ItemID)
        //{
        //    try
        //    {
        //        spINGetItemRow_Result INGetItemRow = db.spINGetItemRow(ItemID).FirstOrDefault();

        //        return INGetItemRow;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool INPurchaseRequisitionSubmit(int RequestID, int UserID, int RoleID)
        //{
        //    try
        //    {
        //        var INPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == RequestID).FirstOrDefault();
        //        var role = db.SecRoles.Where(x => x.RoleID == RoleID).FirstOrDefault();
        //        if (role.RoleID == 4 || role.RoleID == 5)
        //        {
        //            if (role.RoleID == 4) // KPO
        //            {
        //                INPurchaseRequisition.SubmitedByKPO = UserID;
        //                INPurchaseRequisition.SubmitedAtKPO = DateTime.Now;
        //            }
        //            else if (role.RoleID == 5)
        //            {
        //                INPurchaseRequisition.SubmitedByMD = UserID;
        //                INPurchaseRequisition.SubmitedAtMD = DateTime.Now;
        //            }
        //            db.INPurchaseRequisitions.AddOrUpdate(INPurchaseRequisition);
        //            db.SaveChanges();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        //public bool INPurchaseRequisitionSave(INPurchaseRequisition INPurchaseRequisition)
        //{
        //    try
        //    {
        //        if (INPurchaseRequisition != null)
        //        {
        //            db.INPurchaseRequisitions.AddOrUpdate(INPurchaseRequisition);
        //            db.SaveChanges();
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool INPurchaseRequisitionDetailSave(INPurchaseRequisitionDetail INPurchaseRequisitionDetail)
        //{
        //    try
        //    {
        //        if (INPurchaseRequisitionDetail != null)
        //        {
        //            db.INPurchaseRequisitionDetails.AddOrUpdate(INPurchaseRequisitionDetail);
        //            db.SaveChanges();
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public bool INGoodsReceiptNoteDetailDelete(long GoodsReceiptNoteDetailID)
        {
            try
            {
                var INGoodsReceiptNoteDetail = db.INGoodsReceiptNoteDetails.Where(x => x.GoodsReceiptNoteDetailID == GoodsReceiptNoteDetailID).FirstOrDefault();
                db.INGoodsReceiptNoteDetails.Remove(INGoodsReceiptNoteDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool PurchaseOrderDetailDelete(long PurchaseOrderDetailID)
        {
            try
            {
                var PurchaseOrderDetail = db.INPurchaseOrderDetails.Where(x => x.PurchaseOrderDetailID == PurchaseOrderDetailID).FirstOrDefault();
                db.INPurchaseOrderDetails.Remove(PurchaseOrderDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region StoreIssueNote

        public List<spINStoreIssueNoteSearchList_Result> GetINStoreIssueNoteSearchList(int CompanyID, int? UserID, DateTime? FromDate, DateTime? ToDate, long? ItemID)
        {
            try
            {
                var user = db.SecUsers.Where(x => x.CompanyID == CompanyID && x.UsersID == UserID).FirstOrDefault();

                if (user != null) UserID = null;

                List<spINStoreIssueNoteSearchList_Result> INStoreIssueNoteSearchList = db.spINStoreIssueNoteSearchList(CompanyID, UserID, FromDate, ToDate, ItemID).ToList();

                return INStoreIssueNoteSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public INStoreIssueNote INStoreIssueNote(long StoreIssueNoteID)
        {
            try
            {
                var INStoreIssueNote = db.INStoreIssueNotes.Where(x => x.StoreIssueNoteID == StoreIssueNoteID).FirstOrDefault();
                return INStoreIssueNote;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool StoreIssueNotePost(long id)
        {
            try
            {

                var StoreIssueNote = db.INStoreIssueNotes.Where(x => x.StoreIssueNoteID == id).FirstOrDefault();
                StoreIssueNote.IsPosted = true;
                db.INStoreIssueNotes.AddOrUpdate(StoreIssueNote);
                
                var INStoreIssueNoteDetails = db.INStoreIssueNoteDetails.Where(x => x.StoreIssueNoteID == id).ToList();

                foreach (var item in INStoreIssueNoteDetails)
                {
                    // StoreItem
                    var projectItem = db.INProjectItems.Where(x => x.ItemID == item.ItemID && x.ProjectID == StoreIssueNote.ProjectID).FirstOrDefault();
                    if (projectItem != null)
                    {
                        projectItem.QtyInHand -= item.IssuedQty;
                        db.INProjectItems.AddOrUpdate(projectItem);
                        //db.SaveChanges();
                    }

                }
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool StoreIssueNoteUnPost(long id)
        {
            try
            {

                var StoreIssueNote = db.INStoreIssueNotes.Where(x => x.StoreIssueNoteID == id).FirstOrDefault();
                StoreIssueNote.IsPosted = false;
                db.INStoreIssueNotes.AddOrUpdate(StoreIssueNote);

                var INStoreIssueNoteDetails = db.INStoreIssueNoteDetails.Where(x => x.StoreIssueNoteID == id).ToList();

                foreach (var item in INStoreIssueNoteDetails)
                {
                    // StoreItem
                    var projectItem = db.INProjectItems.Where(x => x.ItemID == item.ItemID && x.ProjectID == StoreIssueNote.ProjectID).FirstOrDefault();
                    if (projectItem != null)
                    {
                        projectItem.QtyInHand += item.IssuedQty;
                        db.INProjectItems.AddOrUpdate(projectItem);
                        //db.SaveChanges();
                    }

                }
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINStoreIssueNoteDetailRows_Result> GetINStoreIssueNoteDetailRows(int StoreIssueNoteID)
        {
            try
            {
                var INStoreIssueNoteDetailRows = db.spINStoreIssueNoteDetailRows(StoreIssueNoteID).ToList();

                return INStoreIssueNoteDetailRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INStoreIssueNoteSave(INStoreIssueNote INStoreIssueNote)
        {
            try
            {
                if (INStoreIssueNote != null)
                {
                    db.INStoreIssueNotes.AddOrUpdate(INStoreIssueNote);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INStoreIssueNoteDetailSave(INStoreIssueNoteDetail INStoreIssueNoteDetail)
        {
            try
            {
                if (INStoreIssueNoteDetail != null)
                {
                    db.INStoreIssueNoteDetails.AddOrUpdate(INStoreIssueNoteDetail);
                    db.SaveChanges();

                    ////// StoreItem
                    ////if (StoreItemEntryAllowed)
                    ////{
                    ////    var storeIssueNote = db.INStoreIssueNotes.Where(x => x.StoreIssueNoteID == INStoreIssueNoteDetail.StoreIssueNoteID).FirstOrDefault();

                    ////    var projectItem = db.INProjectItems.Where(x => x.ItemID == INStoreIssueNoteDetail.ItemID && x.ProjectID == storeIssueNote.ProjectID).FirstOrDefault();
                    ////    if (projectItem != null)
                    ////    {
                    ////        projectItem.QtyInHand -= INStoreIssueNoteDetail.IssuedQty;
                    ////        db.INProjectItems.AddOrUpdate(projectItem);
                    ////        db.SaveChanges();
                    ////    }

                    ////}
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INStoreIssueNoteDetailDelete(long StoreIssueNoteDetailID)
        {
            try
            {
                var INStoreIssueNoteDetail = db.INStoreIssueNoteDetails.Where(x => x.StoreIssueNoteDetailID == StoreIssueNoteDetailID).FirstOrDefault();
                db.INStoreIssueNoteDetails.Remove(INStoreIssueNoteDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region StoreReturnNote

        public List<spINStoreReturnNoteSearchList_Result> GetINStoreReturnNoteSearchList(int CompanyID, int? UserID, DateTime? FromDate, DateTime? ToDate, long? ItemID)
        {
            try
            {
                var user = db.SecUsers.Where(x => x.CompanyID == CompanyID && x.UsersID == UserID).FirstOrDefault();

                if (user != null) UserID = null;

                List<spINStoreReturnNoteSearchList_Result> INStoreReturnNoteSearchList = db.spINStoreReturnNoteSearchList(CompanyID, UserID, FromDate, ToDate, ItemID).ToList();

                return INStoreReturnNoteSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public INStoreReturnNote INStoreReturnNote(long StoreReturnNoteID)
        {
            try
            {
                var INStoreReturnNote = db.INStoreReturnNotes.Where(x => x.StoreReturnNoteID == StoreReturnNoteID).FirstOrDefault();
                return INStoreReturnNote;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool StoreReturnNotePost(long id)
        {
            try
            {

                var StoreReturnNote = db.INStoreReturnNotes.Where(x => x.StoreReturnNoteID == id).FirstOrDefault();
                StoreReturnNote.IsPosted = true;
                db.INStoreReturnNotes.AddOrUpdate(StoreReturnNote);

                var INStoreReturnNoteDetails = db.INStoreReturnNoteDetails.Where(x => x.StoreReturnNoteID == id).ToList();

                foreach (var item in INStoreReturnNoteDetails)
                {
                    // StoreItem
                    var projectItem = db.INProjectItems.Where(x => x.ItemID == item.ItemID && x.ProjectID == StoreReturnNote.ProjectID).FirstOrDefault();
                    if (projectItem != null)
                    {
                        projectItem.QtyInHand += item.ReturnQty;
                        db.INProjectItems.AddOrUpdate(projectItem);
                        //db.SaveChanges();
                    }

                }
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool StoreReturnNoteUnPost(long id)
        {
            try
            {

                var StoreReturnNote = db.INStoreReturnNotes.Where(x => x.StoreReturnNoteID == id).FirstOrDefault();
                StoreReturnNote.IsPosted = false;
                db.INStoreReturnNotes.AddOrUpdate(StoreReturnNote);

                var INStoreReturnNoteDetails = db.INStoreReturnNoteDetails.Where(x => x.StoreReturnNoteID == id).ToList();

                foreach (var item in INStoreReturnNoteDetails)
                {
                    // StoreItem
                    var projectItem = db.INProjectItems.Where(x => x.ItemID == item.ItemID && x.ProjectID == StoreReturnNote.ProjectID).FirstOrDefault();
                    if (projectItem != null)
                    {
                        projectItem.QtyInHand += item.ReturnQty;
                        db.INProjectItems.AddOrUpdate(projectItem);
                        //db.SaveChanges();
                    }

                }
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINStoreReturnNoteDetailRows_Result> GetINStoreReturnNoteDetailRows(int StoreReturnNoteID)
        {
            try
            {
                var INStoreReturnNoteDetailRows = db.spINStoreReturnNoteDetailRows(StoreReturnNoteID).ToList();

                return INStoreReturnNoteDetailRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INStoreReturnNoteSave(INStoreReturnNote INStoreReturnNote)
        {
            try
            {
                if (INStoreReturnNote != null)
                {
                    db.INStoreReturnNotes.AddOrUpdate(INStoreReturnNote);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INStoreReturnNoteDetailSave(INStoreReturnNoteDetail INStoreReturnNoteDetail)
        {
            try
            {
                if (INStoreReturnNoteDetail != null)
                {
                    db.INStoreReturnNoteDetails.AddOrUpdate(INStoreReturnNoteDetail);
                    db.SaveChanges();

                    ////// StoreItem
                    ////if (StoreItemEntryAllowed)
                    ////{
                    ////    var storeReturnNote = db.INStoreReturnNotes.Where(x => x.StoreReturnNoteID == INStoreReturnNoteDetail.StoreReturnNoteID).FirstOrDefault();

                    ////    var projectItem = db.INProjectItems.Where(x => x.ItemID == INStoreReturnNoteDetail.ItemID && x.ProjectID == storeReturnNote.ProjectID).FirstOrDefault();
                    ////    if (projectItem != null)
                    ////    {
                    ////        projectItem.QtyInHand -= INStoreReturnNoteDetail.ReturndQty;
                    ////        db.INProjectItems.AddOrUpdate(projectItem);
                    ////        db.SaveChanges();
                    ////    }

                    ////}
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INStoreReturnNoteDetailDelete(long StoreReturnNoteDetailID)
        {
            try
            {
                var INStoreReturnNoteDetail = db.INStoreReturnNoteDetails.Where(x => x.StoreReturnNoteDetailID == StoreReturnNoteDetailID).FirstOrDefault();
                db.INStoreReturnNoteDetails.Remove(INStoreReturnNoteDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public spINStoreReturnNoteItemRow_Result GetStoreReturnNoteItemRow(Int64 ItemID)
        {
            try
            {
                var ItemRow = db.spINStoreReturnNoteItemRow(ItemID).FirstOrDefault();

                return ItemRow;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion


        #region StoreTransferNote
        public bool INStoreTransferNoteApprove(int StoreTransferNoteID, int UserID, int RoleID)
        {
            try
            {
                var inStoreTransferNote = db.INStoreTransferNotes.Where(x => x.StoreTransferNoteID == StoreTransferNoteID).FirstOrDefault();
                inStoreTransferNote.ApprovedByID = UserID;
                inStoreTransferNote.ApprovedByAt = DateTime.Now;
                db.INStoreTransferNotes.AddOrUpdate(inStoreTransferNote);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool INStoreTransferNoteReceive(long StoreTransferNoteID, int UserID)
        {
            try
            {
                var storeTransferNote = db.INStoreTransferNotes.Where(x => x.StoreTransferNoteID == StoreTransferNoteID).FirstOrDefault();
                storeTransferNote.ReceivedByID = UserID;
                storeTransferNote.ReceivedByAt = DateTime.Now;
                //db.INStoreTransferNotes.AddOrUpdate(storeTransferNote);
                //db.SaveChanges();

                //  var storeTransferNote = db.INStoreTransferNotes.Where(x => x.StoreTransferNoteID == StoreTransferNoteID).FirstOrDefault();
                var storeTransferNoteDetails = db.INStoreTransferNoteDetails.Where(x => x.StoreTransferNoteID == StoreTransferNoteID).ToList();

                foreach (var stn in storeTransferNoteDetails)
                {
                    var purchaseRequisitionDetail = db.INPurchaseRequisitionDetails.Where(x => x.RequestDetailID == stn.RequestDetailID).FirstOrDefault();
                    if (purchaseRequisitionDetail != null)
                    {
                        purchaseRequisitionDetail.Balance -= stn.TransferQty;
                        purchaseRequisitionDetail.ModifiedBy = UserID;
                        purchaseRequisitionDetail.ModifiedAt = DateTime.Now;
                        db.INPurchaseRequisitionDetails.AddOrUpdate(purchaseRequisitionDetail);
                        db.SaveChanges();

                        // StoreItem

                        var StoreTransferNotes = db.INStoreTransferNotes.Where(x => x.StoreTransferNoteID == stn.StoreTransferNoteID).FirstOrDefault();

                        var projectItemTo = db.INProjectItems.Where(x => x.ItemID == stn.ItemID && x.ProjectID == StoreTransferNotes.ToProjectID).FirstOrDefault();
                        if (projectItemTo != null)
                        {
                            projectItemTo.QtyInHand += stn.TransferQty;
                            db.INProjectItems.AddOrUpdate(projectItemTo);
                            db.SaveChanges();
                        }

                        var projectItemFrom = db.INProjectItems.Where(x => x.ItemID == stn.ItemID && x.ProjectID == StoreTransferNotes.FromProjectID).FirstOrDefault();
                        if (projectItemFrom != null)
                        {
                            projectItemFrom.QtyInHand -= stn.TransferQty;
                            db.INProjectItems.AddOrUpdate(projectItemFrom);
                            db.SaveChanges();
                        }



                        ////var INItem = db.INItems.Where(x => x.ItemID == stn.ItemID).FirstOrDefault();

                        ////if (storeTransferNote.ToProjectID == 1)
                        ////{
                        ////    INItem.ISAQtyInHand += stn.TransferQty;
                        ////    INItem.IBAQtyInHand -= stn.TransferQty;
                        ////}
                        ////else if (storeTransferNote.ToProjectID == 2)
                        ////{
                        ////    INItem.IBAQtyInHand += stn.TransferQty;
                        ////    INItem.ISAQtyInHand -= stn.TransferQty;
                        ////}

                        ////INItem.ModifiedBy = UserID;
                        ////INItem.ModifiedAt = DateTime.Now;

                        ////db.INItems.AddOrUpdate(INItem);
                        ////db.SaveChanges();
                    }
                }

                db.INStoreTransferNotes.AddOrUpdate(storeTransferNote);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<spINStoreTransferNoteSearchList_Result> GetINStoreTransferNoteSearchList(int CompanyID, int? UserID, DateTime? FromDate, DateTime? ToDate, long? ItemID)
        {
            try
            {
                var user = db.SecUsers.Where(x => x.CompanyID == CompanyID && x.UsersID == UserID).FirstOrDefault();

                if (user != null && user.RoleID == 5) UserID = null;

                List<spINStoreTransferNoteSearchList_Result> INStoreTransferNoteSearchList = db.spINStoreTransferNoteSearchList(CompanyID, UserID, FromDate, ToDate, ItemID).ToList();

                return INStoreTransferNoteSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public INStoreTransferNote INStoreTransferNote(long StoreTransferNoteID)
        {
            try
            {
                var INStoreTransferNote = db.INStoreTransferNotes.Where(x => x.StoreTransferNoteID == StoreTransferNoteID).FirstOrDefault();
                return INStoreTransferNote;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public bool GRNPost(List<INStoreTransferNoteDetail> INStoreTransferNoteDetails)
        //{
        //    try
        //    {
        //        foreach (var grnDetailItem in INStoreTransferNoteDetails)
        //        {


        //            var INPurchaseRequisitionDetail = db.INPurchaseRequisitionDetails.Where(x => x.RequestDetailID == grnDetailItem.RequestDetailID && x.ItemID == grnDetailItem.ItemID).FirstOrDefault();
        //            var INPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == INPurchaseRequisitionDetail.RequestID).FirstOrDefault();

        //            INPurchaseRequisitionDetail.Balance = INPurchaseRequisitionDetail.Balance - grnDetailItem.ReceivedQty;
        //            var INItem = db.INItems.Where(x => x.ItemID == INPurchaseRequisitionDetail.ItemID).First();

        //            if (INPurchaseRequisition.ProjectID == 1)
        //            {
        //                INItem.ISAQtyInHand = INItem.ISAQtyInHand.GetValueOrDefault(0) + grnDetailItem.ReceivedQty;
        //                INItem.ISALastRate = grnDetailItem.Rate.GetValueOrDefault(0);
        //            }
        //            else if (INPurchaseRequisition.ProjectID == 2)
        //            {
        //                INItem.IBAQtyInHand = INItem.IBAQtyInHand.GetValueOrDefault(0) + grnDetailItem.ReceivedQty;
        //                INItem.IBALastRate = grnDetailItem.Rate.GetValueOrDefault(0);
        //            }

        //            var INStoreTransferNote = db.INStoreTransferNotes.Where(x => x.StoreTransferNoteID == grnDetailItem.StoreTransferNoteID).FirstOrDefault();
        //            INStoreTransferNote.IsPosted = true;

        //            db.SaveChanges();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool GRNUnPost(long id)
        //{
        //    try
        //    {
        //        var INStoreTransferNoteDetails = db.INStoreTransferNoteDetails.Where(x => x.StoreTransferNoteID == id).ToList();
        //        foreach (var grnDetailItem in INStoreTransferNoteDetails)
        //        {

        //            var INPurchaseRequisitionDetail = db.INPurchaseRequisitionDetails.Where(x => x.RequestDetailID == grnDetailItem.RequestDetailID && x.ItemID == grnDetailItem.ItemID).FirstOrDefault();
        //            var INPurchaseRequisition = db.INPurchaseRequisitions.Where(x => x.RequestID == INPurchaseRequisitionDetail.RequestID).FirstOrDefault();

        //            INPurchaseRequisitionDetail.Balance = INPurchaseRequisitionDetail.Balance + grnDetailItem.ReceivedQty;
        //            var INItem = db.INItems.Where(x => x.ItemID == INPurchaseRequisitionDetail.ItemID).First();

        //            if (INPurchaseRequisition.ProjectID == 1)
        //            {
        //                INItem.ISAQtyInHand = INItem.ISAQtyInHand.GetValueOrDefault(0) - grnDetailItem.ReceivedQty;
        //                INItem.ISALastRate = grnDetailItem.Rate.GetValueOrDefault(0);
        //            }
        //            else if (INPurchaseRequisition.ProjectID == 2)
        //            {
        //                INItem.IBAQtyInHand = INItem.IBAQtyInHand.GetValueOrDefault(0) - grnDetailItem.ReceivedQty;
        //                INItem.IBALastRate = grnDetailItem.Rate.GetValueOrDefault(0);
        //            }


        //            var INStoreTransferNote = db.INStoreTransferNotes.Where(x => x.StoreTransferNoteID == grnDetailItem.StoreTransferNoteID).FirstOrDefault();
        //            INStoreTransferNote.IsPosted = false;

        //            db.SaveChanges();
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        public List<spINStoreTransferNoteDetailRows_Result> GetINStoreTransferNoteDetailRows(int StoreTransferNoteID)
        {
            try
            {
                var INStoreTransferNoteDetailRows = db.spINStoreTransferNoteDetailRows(StoreTransferNoteID).ToList();

                return INStoreTransferNoteDetailRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INStoreTransferNoteSave(INStoreTransferNote INStoreTransferNote)
        {
            try
            {
                if (INStoreTransferNote != null)
                {
                    db.INStoreTransferNotes.AddOrUpdate(INStoreTransferNote);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INStoreTransferNoteDetailSave(INStoreTransferNoteDetail INStoreTransferNoteDetail)
        {
            try
            {
                if (INStoreTransferNoteDetail != null)
                {
                    db.INStoreTransferNoteDetails.AddOrUpdate(INStoreTransferNoteDetail);
                    db.SaveChanges();



                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INStoreTransferNoteDetailDelete(long StoreTransferNoteDetailID)
        {
            try
            {
                var INStoreTransferNoteDetail = db.INStoreTransferNoteDetails.Where(x => x.StoreTransferNoteID == StoreTransferNoteDetailID).FirstOrDefault();
                db.INStoreTransferNoteDetails.Remove(INStoreTransferNoteDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion



        #region PurchaseOrder

        public List<spINPurchaseOrderSearchList_Result> GetINPurchaseOrderSearchListForInventory(int CompanyID, int? UserID, DateTime? FromDate, DateTime? ToDate, long? ItemID )
        {
            try
            {

                UserID = null;

                List<spINPurchaseOrderSearchList_Result> INPurchaseOrderSearchList = db.spINPurchaseOrderSearchList(CompanyID, UserID, FromDate, ToDate, ItemID).ToList();

                return INPurchaseOrderSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<spINPurchaseOrderSearchList_Result> GetINPurchaseOrderSearchList(int CompanyID, int? UserID, DateTime? FromDate, DateTime? ToDate, long? ItemID)
        {
            try
            {
                var user = db.SecUsers.Where(x => x.CompanyID == CompanyID && x.UsersID == UserID).FirstOrDefault();

                if (user != null && user.RoleID == 5) UserID = null;

                List<spINPurchaseOrderSearchList_Result> INPurchaseOrderSearchList = db.spINPurchaseOrderSearchList(CompanyID, UserID, FromDate, ToDate, ItemID).ToList();

                return INPurchaseOrderSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public INPurchaseOrder INPurchaseOrder(long PurchaseOrderID)
        {
            try
            {
                var INPurchaseOrder = db.INPurchaseOrders.Where(x => x.PurchaseOrderID == PurchaseOrderID).FirstOrDefault();
                return INPurchaseOrder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public bool INPurchaseOrderSave(INPurchaseOrder INPurchaseOrder)
        {
            try
            {
                if (INPurchaseOrder != null)
                {
                    db.INPurchaseOrders.AddOrUpdate(INPurchaseOrder);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INPurchaseOrderDetailSave(INPurchaseOrderDetail INPurchaseOrderDetail, int ProjectID)
        {
            try
            {
                if (INPurchaseOrderDetail != null)
                {
                    db.INPurchaseOrderDetails.AddOrUpdate(INPurchaseOrderDetail);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool INPurchaseOrderDetailDelete(long INPurchaseOrderDetailID)
        {
            try
            {
                var INGoodsReceiptNoteDetail = db.INPurchaseOrderDetails.Where(x => x.PurchaseOrderDetailID == INPurchaseOrderDetailID).FirstOrDefault();
                db.INPurchaseOrderDetails.Remove(INGoodsReceiptNoteDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool POCancel(Int64 PurchaseOrderID, int userid)
        {
            try
            {
                var INPurchaseOrder = db.INPurchaseOrders.Where(x => x.PurchaseOrderID == PurchaseOrderID).FirstOrDefault();

                INPurchaseOrder.CancelledBy = userid;
                INPurchaseOrder.CancelledAt = DateTime.Now;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool POApprove(Int64 PurchaseOrderID, int userid)
        {
            try
            {

                var INPurchaseOrder = db.INPurchaseOrders.Where(x => x.PurchaseOrderID == PurchaseOrderID).FirstOrDefault();

                INPurchaseOrder.ApprovedBy = userid;
                INPurchaseOrder.POStatusID = 2; // Approved 
                INPurchaseOrder.ApprovedAt = DateTime.Now;
                db.SaveChanges();

                // GRN
                if (INPurchaseOrder.GoodsReceiptNoteID.GetValueOrDefault(0) > 0)
                {
                    var grn = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == INPurchaseOrder.GoodsReceiptNoteID).FirstOrDefault();
                    grn.PurchaseOrderID = INPurchaseOrder.PurchaseOrderID;
                    db.SaveChanges();
                }

                /// Revert Logic
                var srNo = db.INPORevertHistories.Where(x => x.PurchaseOrderID == PurchaseOrderID).Max(x => x.SrNo).GetValueOrDefault(0);

                if (srNo > 0)
                {
                    var pod = db.INPurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();

                    var reverts = db.INPORevertHistories.Where(x => x.PurchaseOrderID == PurchaseOrderID && x.SrNo == srNo).ToList();  //new List<INPORevertHistory>();

                    foreach (var revertedItem in reverts)
                    {
                        var podItem = pod.Where(x => x.ItemID == revertedItem.ItemID).FirstOrDefault();
                        if(podItem != null)
                        {
                            revertedItem.RevertedDiscount = revertedItem.Discount.GetValueOrDefault(0) == podItem.DiscountedPrice.GetValueOrDefault(0) ? 0: podItem.DiscountedPrice.GetValueOrDefault(0);
                            revertedItem.RevertedRate = revertedItem.Rate == podItem.UnitPrice ? 0 : podItem.UnitPrice;
                            revertedItem.RevertedQty = revertedItem.Qty == podItem.ApprovedQty ? 0 : podItem.ApprovedQty;
                            revertedItem.RevertedTotalAmount = revertedItem.TotalAmount == podItem.Amount ? 0 : podItem.Amount;
                            revertedItem.ProjectID = INPurchaseOrder.ProjectID;
                            revertedItem.RevertedDate = DateTime.Now;
                            revertedItem.Status = "Updated";
                            db.INPORevertHistories.AddOrUpdate(revertedItem);
                        }
                        else
                        {
                            revertedItem.Status = "Deleted";
                            db.INPORevertHistories.AddOrUpdate(revertedItem);
                        }                            
                    };

                    foreach(var addedItem in pod)
                    {
                        var newItem = reverts.Where(x => x.PurchaseOrderID == PurchaseOrderID && x.ItemID == addedItem.ItemID).FirstOrDefault();
                        if(newItem == null )
                        {
                            var revert = new INPORevertHistory() { 
                                CompanyID = addedItem.CompanyID,
                                ProjectID= INPurchaseOrder.ProjectID,
                                ItemID = addedItem.ItemID,
                                Discount = addedItem.DiscountedPrice,
                                PurchaseOrderDate=addedItem.CreatedAt,
                                Qty = addedItem.ApprovedQty,
                                Rate =addedItem.UnitPrice,
                                PurchaseOrderID = addedItem.PurchaseOrderID,
                                SrNo = srNo,
                                TotalAmount=addedItem.Amount,
                                Status = "Added"
                            };
                            db.INPORevertHistories.AddOrUpdate(revert);
                        }
                    }
                    db.SaveChanges();

                }
                
                /// 

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool POUnApprove(Int64 PurchaseOrderID)
        {
            try
            {

                var INPurchaseOrder = db.INPurchaseOrders.Where(x => x.PurchaseOrderID == PurchaseOrderID).FirstOrDefault();

                if(INPurchaseOrder.CancelledBy > 0 )
                {
                    INPurchaseOrder.CancelledBy = null;
                    INPurchaseOrder.CancelledAt = null;
                    db.SaveChanges();
                    return true;
                }

                INPurchaseOrder.ApprovedBy = null;
                INPurchaseOrder.POStatusID = 1; // Pending Approval
                INPurchaseOrder.ApprovedAt = null;
                db.SaveChanges();

                // GRN
                if (INPurchaseOrder.GoodsReceiptNoteID.GetValueOrDefault(0) > 0)
                {
                    var grn = db.INGoodsReceiptNotes.Where(x => x.GoodsReceiptNoteID == INPurchaseOrder.GoodsReceiptNoteID).FirstOrDefault();
                    grn.PurchaseOrderID = null;
                    db.SaveChanges();
                }

                /// Revert Logic
                var srNo = db.INPORevertHistories.Where(x => x.PurchaseOrderID== PurchaseOrderID).Max(x => x.SrNo).GetValueOrDefault(0);
                
                if (srNo > 0) 
                {
                    var pod = db.INPurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();
                    var reverts = new List<INPORevertHistory>();

                    foreach (var item in pod)
                    {
                        var revert = new INPORevertHistory()
                        {
                            SrNo = srNo + 1,
                            Status = "Updated",
                            PurchaseOrderID = item.PurchaseOrderID,
                            Discount = item.DiscountedPrice,
                            ItemID = item.ItemID,
                            Qty = item.ApprovedQty,
                            Rate = item.UnitPrice,
                            PurchaseOrderDate = INPurchaseOrder.PurchaseOrderDate,
                            TotalAmount = item.Amount,
                            ProjectID = INPurchaseOrder.ProjectID,
                            CompanyID = INPurchaseOrder.CompanyID
                        };

                        reverts.Add(revert);
                    }
                    ;

                    db.INPORevertHistories.AddRange(reverts);
                    db.SaveChanges();

                }
                else
                {
                    var pod = db.INPurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();
                    var reverts = new List<INPORevertHistory>();

                    foreach(var item in pod)
                    {
                        var revert = new INPORevertHistory()
                        {
                            SrNo = 1,
                            Status = "Reverted",
                            PurchaseOrderID = item.PurchaseOrderID,
                            Discount = item.DiscountedPrice,
                            ItemID = item.ItemID,
                            Qty = item.ApprovedQty,
                            Rate = item.UnitPrice,
                            PurchaseOrderDate = INPurchaseOrder.PurchaseOrderDate,
                            TotalAmount = item.Amount,
                            ProjectID = INPurchaseOrder.ProjectID,                            
                            CompanyID = INPurchaseOrder.CompanyID
                        };

                        reverts.Add(revert);
                    };

                    db.INPORevertHistories.AddRange(reverts);
                    db.SaveChanges();
                }

                    
                /// 

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


    }
}