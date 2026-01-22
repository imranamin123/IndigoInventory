using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportAppServer;
using CrystalDecisions.Web;
using GL.EF;
using GL.Models;
using GL.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GL.Common;
using static GL.Common.Enumeration;
using System.Collections;
using System.IO;
using System.Web.Mvc;
using CrystalDecisions.Shared.Json;
using static System.Data.Entity.Infrastructure.Design.Executor;

using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO.Packaging;


namespace GL.ReportsWebForms
{

    public enum PaymentPlanTypeEnum
    {
        TokenMoney = 1,
        DownPayment = 2,
        Installment = 3,
        Possession = 4,
        Adjustment = 5,
        CancellationCharges = 6,
        ExtraArea = 7,
        ExtraWork = 8,
        MeterCharges = 9,
        IBALegacyPayments = 10

    }

    public partial class ReportForm : System.Web.UI.Page
    {
        protected static Queue reportQueue = new Queue();

        private GLEntities db = new GLEntities();
        ReportDocument report = new ReportDocument();


        protected void Page_Load(object sender, EventArgs e)
        {


            var ReportName = Convert.ToString(Request.QueryString["ReportName"]);

            if (ReportName == "GeneralLedger")
            {
                GeneralLedgerReport();
            }
            else if (ReportName == "GeneralLedgerOB")
            {
                GeneralLedgerReportOB();
            }
            else if (ReportName == "ApplicationForm")
            {
                ApplicationFormReport();
            }
            else if (ReportName == "DVReceiptByID")
            {
                DVReceiptByIDReport();
            }
            else if (ReportName == "DVReceiptByIDSGO")
            {
                DVReceiptByIDReportSGO();
            }
            else if (ReportName == "DVPaymentPlan")
            {
                DVPaymentPlanReport();
            }
            else if (ReportName == "UnitLedger")
            {
                DVUnitLedgerReport();
            }
            else if (ReportName == "ApplicationForm")
            {
                ApplicationFormReport();
            }
            else if (ReportName == "DVMemberPaymentPlanStatusReport")
            {
                DVMemberPaymentPlanStatusReport();
            }
            else if (ReportName == "DVMemberPaymentPlanStatusReportAllUnits")
            {
                DVMemberPaymentPlanStatusReportAllUnits();
            }
            else if (ReportName == "APPartyLedgerReport")
            {
                APPartyLedgerReport();
            }
            else if (ReportName == "INPurchaseRequisitionReport")
            {
                INPurchaseRequisitionReport();
            }
            else if (ReportName == "INGoodsReceiptNoteReport")
            {
                INGoodsReceiptNoteReport();
            }
            else if (ReportName == "INPurchaseRequisitionReportDownload")
            {
                INPurchaseRequisitionReportDownload();
            }
            else if (ReportName == "INGoodsReceiptNoteReportDownload")
            {
                INGoodsReceiptNoteReportDownload();
            }
            else if (ReportName == "INStoreIssueNoteReportDownload")
            {
                INStoreIssueNoteReportDownload();
            }
            else if (ReportName == "INStoreReturnNoteReportDownload")
            {
                INStoreReturnNoteReportDownload();
            }            
            else if (ReportName == "INStoreTransferNoteReportDownload")
            {
                INStoreTransferNoteReportDownload();
            }

            else if (ReportName == "INPurchaseOrderReportDownload")
            {
                INPurchaseOrderReportDownload();
            }

        }

        private void INPurchaseRequisitionReportDownload()
        {
            var report = new rptINPurchaseRequisition();
            Nullable<int> RequestID = Convert.ToInt32(Request.QueryString["RequestID"]);

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
                    ManualDemandNo = v.ManualDemandNo.GetValueOrDefault(0),
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
        }

        private void INStoreIssueNoteReportDownload()
        {
            //CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptINStoreIssueNote();
            Nullable<int> INStoreIssueNoteID = Convert.ToInt32(Request.QueryString["INStoreIssueNoteID"]);

            var INStoreIssueNote = new GLEntities().spRptINStoreIssueNote(INStoreIssueNoteID);

            var INStoreIssueNoteData = (

                from v in INStoreIssueNote
                select new spRptINStoreIssueNoteReportModel
                {
                    Company = v.Company,
                    StoreIssueNoteID = v.StoreIssueNoteID,
                    StoreIssueNoteDate = v.StoreIssueNoteDate.GetValueOrDefault(DateTime.Now),
                    ProjectName = v.ProjectName,
                    StoreName = v.StoreName,
                    mRemarks = v.mRemarks,
                    StoreIssueNoteDetailID = v.StoreIssueNoteDetailID,
                    ItemID = v.ItemID.GetValueOrDefault(0),
                    Item = v.Item,
                    Size = v.Size,
                    Unit = v.Unit,
                    IssuedQty = v.IssuedQty.GetValueOrDefault(0),
                    IssuedBy = v.IssuedBy,

                }).ToList();

            reportQueue.Enqueue(report);


            report.SetDataSource(INStoreIssueNoteData);
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
            Response.AddHeader("content-disposition", "attachment;filename=StoreIssueNoteReport.pdf");
            Response.Buffer = true;
            Response.Clear();

            // Write the MemoryStream to the response
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();

            // Cleanup
            reportDocument.Close();
            reportDocument.Dispose();

        }


        private void INStoreReturnNoteReportDownload()
        {
            //CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptINStoreReturnNote();
            Nullable<int> INStoreReturnNoteID = Convert.ToInt32(Request.QueryString["INStoreReturnNoteID"]);

            var INStoreReturnNote = new GLEntities().spRptINStoreReturnNote(INStoreReturnNoteID);

            var INStoreReturnNoteData = (

                from v in INStoreReturnNote
                select new spRptINStoreReturnNoteReportModel
                {
                    Company = v.Company,
                    StoreReturnNoteID = v.StoreReturnNoteID,
                    StoreReturnNoteDate = v.StoreReturnNoteDate.GetValueOrDefault(DateTime.Now),
                    ProjectName = v.ProjectName,
                    mRemarks = v.mRemarks,
                    StoreReturnNoteDetailID = v.StoreReturnNoteDetailID,
                    ItemID = v.ItemID.GetValueOrDefault(0),
                    Item = v.Item,
                    Size = v.Size,
                    Unit = v.Unit,
                    ReturnQty = v.ReturnQty.GetValueOrDefault(0),
                    ReturnBy = v.ReturnBy,
                    ReturnAt =v.ReturnAt.Value
                }).ToList();

            reportQueue.Enqueue(report);


            report.SetDataSource(INStoreReturnNoteData);
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
            Response.AddHeader("content-disposition", "attachment;filename=StoreReturnNoteReport.pdf");
            Response.Buffer = true;
            Response.Clear();

            // Write the MemoryStream to the response
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();

            // Cleanup
            reportDocument.Close();
            reportDocument.Dispose();

        }

        private void INStoreTransferNoteReportDownload()
        {
            //CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptINStoreIssueNote();
            Nullable<int> INStoreTransferNoteID = Convert.ToInt32(Request.QueryString["INStoreTransferNoteID"]);

            var INStoreTransferNote = new GLEntities().spRptINStoreTransferNote(INStoreTransferNoteID);

            var INStoreTransferNoteData = (

                from v in INStoreTransferNote
                select new spRptINStoreTransferNoteModel
                {
                    Company = v.Company,
                    StoreTransferNoteID = v.StoreTransferNoteID,
                    StoreTransferNoteDate = v.StoreTransferNoteDate.GetValueOrDefault(DateTime.Now),
                    ToProject = v.ToProject,
                    FromProject = v.FromProject,
                    MRemarks = v.MRemarks,
                    StoreTransferNoteDetailID = v.StoreTransferNoteDetailID,
                    ItemID = v.ItemID.GetValueOrDefault(0),
                    Item = v.Item,
                    Size = v.Size,
                    Unit = v.Unit,
                    TransferQty=v.TransferQty.GetValueOrDefault(0),
                    Remarks = v.Remarks,
                    
                    CreatedBy = v.CreatedBy,
                    CreatedAt = v.CreatedAt.GetValueOrDefault(DateTime.Now),
                    ApprovedBy = v.ApprovedBy,
                    ApprovedByAt=v.ApprovedByAt.GetValueOrDefault(DateTime.Now),
                    ReceivedBy = v.ReceivedBy,
                    ReceivedByAt=v.ReceivedByAt.GetValueOrDefault(DateTime.Now)

                }).ToList();

            reportQueue.Enqueue(report);


            report.SetDataSource(INStoreTransferNoteData);
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
            Response.AddHeader("content-disposition", "attachment;filename=StoreTransferNoteReport.pdf");
            Response.Buffer = true;
            Response.Clear();

            // Write the MemoryStream to the response
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();

            // Cleanup
            reportDocument.Close();
            reportDocument.Dispose();

        }

        private void INItemStockReportDownloadPdf()
        {
            //CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new RptINItemStock();
            int CompanyID = Convert.ToInt32(Request.QueryString["CompanyID"]);
            int ProjectID = Convert.ToInt32(Request.QueryString["ProjectID"]);
            DateTime FromDate = Convert.ToDateTime(Request.QueryString["FromDate"]);
            DateTime ToDate = Convert.ToDateTime(Request.QueryString["ToDate"]);

            var INItemStockList = new GLEntities().spRptINItemStock(CompanyID, ProjectID, FromDate, ToDate);

            var INItemStockData = (

                from v in INItemStockList
                select new spRptINItemStockModel
                {
                    CompanyName = v.CompanyName,
                    ProjectName = v.ProjectName,
                    GroupName = v.GroupName,
                    CategoryName = v.CategoryName,
                    ItemID = v.ItemID,
                    Description = v.Description,
                    SizeName = v.SizeName,
                    UOM = v.UOM,
                    OpeningQty = v.OpeningQty.GetValueOrDefault(0),
                    ReceivedQty = v.ReceivedQty,
                    Rate = v.Rate.GetValueOrDefault(0),
                    ClosingQty = v.ClosingQty.GetValueOrDefault(0),
                    ClosingAmount = v.ClosingAmount.GetValueOrDefault(0)
                }).ToList();


            reportQueue.Enqueue(report);


            report.SetDataSource(INItemStockData);
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
            Response.AddHeader("content-disposition", "attachment;filename=ItemStockReport.pdf");
            Response.Buffer = true;
            Response.Clear();

            // Write the MemoryStream to the response
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();

            // Cleanup
            reportDocument.Close();
            reportDocument.Dispose();

        }

        private void INPurchaseOrderReportDownload()
        {
            //CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptINPurchaseOrder();
            Nullable<int> PurchaseOrderID = Convert.ToInt32(Request.QueryString["PurchaseOrderID"]);

            var INPurchaseOrder = new GLEntities().spRptINPurchaseOrder(PurchaseOrderID);

            var PurchaseOrderData = (

                from v in INPurchaseOrder
                select new spRptINPurchaseOrderReportModel
                {
                    ProjectName= v.ProjectName,
                    ProjectID=v.ProjectID.GetValueOrDefault(0),
                    Unit=v.Unit,
                    ItemID=v.ItemID.GetValueOrDefault(0),
                    APVendorID=v.APVendorID.GetValueOrDefault(0),
                    Amount=v.Amount.GetValueOrDefault(0),
                    Status=v.Status,
                    ApprovedQty=v.ApprovedQty.GetValueOrDefault(0),
                    APVendorName=v.APVendorName,
                    BankDetails =v.BankDetails,
                    Company=v.Company,
                    ContactNumber=v.ContactNumber,
                    ContactPerson=v.ContactPerson,
                    Email=v.Email,
                    Item=v.Item,
                    mRemarks=v.mRemarks,
                    PaymentTerms=v.PaymentTerms,
                    PurchaseOrderDate=v.PurchaseOrderDate.GetValueOrDefault(DateTime.Now),
                    PurchaseOrderID=v.PurchaseOrderID,
                    Size=v.Size,
                    UnitPrice=v.UnitPrice.GetValueOrDefault(0),
                    DiscountedPrice = v.DiscountedPrice.GetValueOrDefault(0),
                    VendorAddress = v.VendorAddress,
                    CreatedBy = v.CreatedBy,
                    CreatedAt = v.CreatedAt.GetValueOrDefault(DateTime.Now),
                    ApprovedBy=v.ApprovedBy,
                    ApprovedAt = v.ApprovedAt.GetValueOrDefault(DateTime.Now)

                }).ToList();

            reportQueue.Enqueue(report);


            report.SetDataSource(PurchaseOrderData);
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
            Response.AddHeader("content-disposition", "attachment;filename=PurhaseOrderReport.pdf");
            Response.Buffer = true;
            Response.Clear();

            // Write the MemoryStream to the response
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();

            // Cleanup
            reportDocument.Close();
            reportDocument.Dispose();


            //CrystalReportViewer1.ReportSource = report;
            //CrystalReportViewer1.RefreshReport();
        }

        private void INGoodsReceiptNoteReportDownload()
        {
            //CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptINGoodsReceiptNote();
            Nullable<int> GoodsReceiptNoteID = Convert.ToInt32(Request.QueryString["GoodsReceiptNoteID"]);

            var INGoodsReceiptNote = new GLEntities().spRptINGoodsReceiptNote(GoodsReceiptNoteID);

            var GoodsReceiptNoteData = (

                from v in INGoodsReceiptNote
                select new spRptINGoodsReceiptNoteReportModel
                {
                    GoodsReceiptNoteID = v.GoodsReceiptNoteID,
                    GoodsReceiptNotesDate = v.GoodsReceiptNotesDate.GetValueOrDefault(DateTime.Now),
                    ProjectID = v.ProjectID.GetValueOrDefault(0),
                    ProjectName = v.ProjectName,
                    APVendorID = v.APVendorID.GetValueOrDefault(0),
                    APVendorName = v.APVendorName,
                    DCINVNO = v.DCINVNO,
                    DCINVDate = v.DCINVDate.GetValueOrDefault(DateTime.Now),
                    IGPNO = v.IGPNO.GetValueOrDefault(0),
                    VehicleNo = v.VehicleNo,
                    mRemarks = v.mRemarks,
                    IsPosted = v.IsPosted.GetValueOrDefault(false),
                    GoodsReceiptNoteDetailID = v.GoodsReceiptNoteDetailID,
                    PurchaseOrderID = Convert.ToInt32(v.PurchaseOrderID),
                    RequestDetailID = v.RequestDetailID.GetValueOrDefault(0),
                    ItemID = v.ItemID.GetValueOrDefault(0),
                    Item = v.Item,
                    Size = v.Size,
                    Unit = v.Unit,
                    ApprovedQty = v.ApprovedQty.GetValueOrDefault(0),
                    ReceivedQty = v.ReceivedQty.GetValueOrDefault(0),
                    Rate = v.Rate.GetValueOrDefault(0),
                    VAT = v.VAT.GetValueOrDefault(0),
                    Amount = v.Amount.GetValueOrDefault(0),
                    dRemarks = v.dRemarks,
                    Company = v.Company,
                    ReceivedBy = v.ReceivedBy

                }).ToList();

            reportQueue.Enqueue(report);


            report.SetDataSource(GoodsReceiptNoteData);
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
            Response.AddHeader("content-disposition", "attachment;filename=GoodsReceiptReport.pdf");
            Response.Buffer = true;
            Response.Clear();

            // Write the MemoryStream to the response
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();

            // Cleanup
            reportDocument.Close();
            reportDocument.Dispose();


            //CrystalReportViewer1.ReportSource = report;
            //CrystalReportViewer1.RefreshReport();
        }

        private void INPurchaseRequisitionReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptINPurchaseRequisition();
            Nullable<int> RequestID = Convert.ToInt32(Request.QueryString["RequestID"]);

            var PurchaseRequisitions = db.spRptINPurchaseRequisition(RequestID);

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

            reportQueue.Enqueue(report);

            report.SetDataSource(PurchaseRequisitionData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();
        }

        private void INGoodsReceiptNoteReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptINGoodsReceiptNote();
            Nullable<int> GoodsReceiptNoteID = Convert.ToInt32(Request.QueryString["GoodsReceiptNoteID"]);

            var INGoodsReceiptNote = db.spRptINGoodsReceiptNote(GoodsReceiptNoteID);

            var GoodsReceiptNoteData = (

                from v in INGoodsReceiptNote
                select new spRptINGoodsReceiptNoteReportModel
                {
                    GoodsReceiptNoteID = v.GoodsReceiptNoteID,
                    GoodsReceiptNotesDate = v.GoodsReceiptNotesDate.GetValueOrDefault(DateTime.Now),
                    ProjectID = v.ProjectID.GetValueOrDefault(0),
                    ProjectName = v.ProjectName,
                    APVendorID = v.APVendorID.GetValueOrDefault(0),
                    APVendorName = v.APVendorName,
                    DCINVNO = v.DCINVNO,
                    DCINVDate = v.DCINVDate.GetValueOrDefault(DateTime.Now),
                    IGPNO = v.IGPNO.GetValueOrDefault(0),
                    VehicleNo = v.VehicleNo,
                    mRemarks = v.mRemarks,
                    IsPosted = v.IsPosted.GetValueOrDefault(false),
                    GoodsReceiptNoteDetailID = v.GoodsReceiptNoteDetailID,
                    RequestDetailID = v.RequestDetailID.GetValueOrDefault(0),
                    ItemID = v.ItemID.GetValueOrDefault(0),
                    Item = v.Item,
                    Size = v.Size,
                    Unit = v.Unit,
                    ApprovedQty = v.ApprovedQty.GetValueOrDefault(0),
                    ReceivedQty = v.ReceivedQty.GetValueOrDefault(0),
                    Rate = v.Rate.GetValueOrDefault(0),
                    VAT = v.VAT.GetValueOrDefault(0),
                    Amount = v.Amount.GetValueOrDefault(0),
                    dRemarks = v.dRemarks,
                    Company = v.Company,
                    ReceivedBy = v.ReceivedBy,

                }).ToList();

            reportQueue.Enqueue(report);

            report.SetDataSource(GoodsReceiptNoteData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();
        }

        private void ApplicationFormReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptApplicationForm();
            var ApplicationFormID = Convert.ToInt32(Request.QueryString["ApplicationFormID"]);
            //var GeneralLedgerReportModel = db.sprpt(CompanyID, StartDate, EndDate, GLAccountNoStart, GLAccountNoEnd).ToList();
            var ApplicationFormReportModel = db.spRptDVApplication(ApplicationFormID);

            var ApplicationFormReportData = (
                           from v in ApplicationFormReportModel
                           select new
                           {
                               ApplicationFormID = v.ApplicationFormID,
                               ProjectID = v.ProjectID.GetValueOrDefault(0),
                               ApplicationFormNo = v.ApplicationFormNo ?? "",
                               ApplicationFormDate = v.ApplicationFormDate.GetValueOrDefault(DateTime.Now),
                               UnitID = v.UnitID.GetValueOrDefault(0),
                               Price = v.Price.GetValueOrDefault(0),
                               CreatedAt = v.CreatedAt.GetValueOrDefault(DateTime.Now),
                               CreatedBy = v.CreatedBy.GetValueOrDefault(0),
                               ModifiedAt = v.ModifiedAt.GetValueOrDefault(DateTime.Now),
                               ModifiedBy = v.ModifiedBy.GetValueOrDefault(0),
                               CompanyID = v.CompanyID,
                               UnitNo = v.UnitNo ?? "",
                               UnitTypeID = v.UnitTypeID.GetValueOrDefault(0),
                               FloorNo = v.FloorNo ?? "",
                               SQFT = v.SQFT.GetValueOrDefault(0),
                               TokenMoney = v.TokenMoney.GetValueOrDefault(0),
                               UnitTypeName = v.UnitTypeName ?? "",
                               ProjectName = v.ProjectName ?? "",
                               ProjectDescription = v.ProjectDescription ?? "",
                               ProjectAddress = v.ProjectAddress ?? "",
                               ProjectLogoPath = v.ProjectLogoPath ?? "",
                               IsActive = v.IsActive.GetValueOrDefault(false),

                               ApplicationFormID_1 = v.ApplicationFormID_1,
                               Name_1 = v.Name_1 ?? "",
                               GenderID_1 = v.GenderID_1,
                               HusbandFather_1 = v.HusbandFather_1 ?? "",
                               NICOP_CNIC_1 = v.NICOP_CNIC_1 ?? "",
                               Passport_1 = v.Passport_1 ?? "",
                               HouseNo_1 = v.HouseNo_1 ?? "",
                               StreetArea_1 = v.StreetArea_1,
                               Area_1 = v.Area_1 ?? "",
                               City_1 = v.City_1 ?? "",
                               District_1 = v.District_1 ?? "",
                               Division_1 = v.Division_1 ?? "",
                               Province_1 = v.Province_1 ?? "",
                               Country_1 = v.Country_1 ?? "",
                               CorrespondenceAdressHouseNo_1 = v.CorrespondenceAdressHouseNo_1 ?? "",
                               StreetArea2_1 = v.StreetArea2_1 ?? "",
                               City2_1 = v.City2_1 ?? "",
                               Country2_1 = v.Country2_1 ?? "",
                               PhoneNo_1 = v.PhoneNo_1 ?? "",
                               Phone2_1 = v.Phone2_1 ?? "",
                               Phone3_1 = v.Phone3_1 ?? "",
                               Email_1 = v.Email_1 ?? "",
                               Profession_1 = v.Profession_1 ?? "",
                               SubProfession_1 = v.SubProfession_1 ?? "",
                               Living_1 = v.Living_1 ?? "",
                               Citizen_1 = v.Citizen_1 ?? "",
                               Tax_1 = v.Tax_1 ?? "",
                               NomioneeName_1 = v.NomioneeName_1 ?? "",
                               NomineeFatherHusbandName_1 = v.NomineeFatherHusbandName_1 ?? "",
                               Nominee_NICOP_CNIC_1 = v.Nominee_NICOP_CNIC_1 ?? "",
                               Relation_1 = v.Relation_1 ?? "",
                               ImagePath_1 = v.ImagePath_1 ?? "",
                               CreatedAt_1 = v.CreatedAt_1,
                               CreatedBy_1 = v.CreatedBy_1,
                               ModifiedAt_1 = v.ModifiedAt_1,
                               ModifiedBy_1 = v.ModifiedBy_1,
                               CompanyID_1 = v.CompanyID_1,

                               ApplicationFormID_2 = v.ApplicationFormID_2,
                               Name_2 = v.Name_2 ?? "",
                               GenderID_2 = v.GenderID_2,
                               HusbandFather_2 = v.HusbandFather_2 ?? "",
                               NICOP_CNIC_2 = v.NICOP_CNIC_2 ?? "",
                               Passport_2 = v.Passport_2 ?? "",
                               HouseNo_2 = v.HouseNo_2 ?? "",
                               StreetArea_2 = v.StreetArea_2 ?? "",
                               Area_2 = v.Area_2 ?? "",
                               City_2 = v.City_2 ?? "",
                               District_2 = v.District_2 ?? "",
                               Division_2 = v.Division_2 ?? "",
                               Province_2 = v.Province_2 ?? "",
                               Country_2 = v.Country_2 ?? "",
                               CorrespondenceAdressHouseNo_2 = v.CorrespondenceAdressHouseNo_2 ?? "",
                               StreetArea2_2 = v.StreetArea2_2 ?? "",
                               City2_2 = v.ApplicationFormID,
                               Country2_2 = v.Country2_2 ?? "",
                               PhoneNo_2 = v.PhoneNo_2 ?? "",
                               Phone2_2 = v.Phone2_2 ?? "",
                               Phone3_2 = v.Phone3_2 ?? "",
                               Email_2 = v.Email_2 ?? "",
                               Profession_2 = v.Profession_2 ?? "",
                               SubProfession_2 = v.SubProfession_2 ?? "",
                               Living_2 = v.Living_2 ?? "",
                               Citizen_2 = v.Citizen_2 ?? "",
                               Tax_2 = v.Tax_2 ?? "",
                               NomioneeName_2 = v.NomioneeName_2 ?? "",
                               NomineeFatherHusbandName_2 = v.NomineeFatherHusbandName_2 ?? "",
                               Nominee_NICOP_CNIC_2 = v.Nominee_NICOP_CNIC_2 ?? "",
                               Relation_2 = v.Relation_2 ?? "",
                               ImagePath_2 = v.ImagePath_2 ?? "",
                               CreatedAt_2 = v.CreatedAt_2,
                               CreatedBy_2 = v.CreatedBy_2,
                               ModifiedAt_2 = v.ModifiedAt_2,
                               ModifiedBy_2 = v.ModifiedBy_2,
                               CompanyID_2 = v.CompanyID_2,

                           }).ToList();

            reportQueue.Enqueue(report);

            report.SetDataSource(ApplicationFormReportData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();
        }

        private void GeneralLedgerReportOB()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptGeneralLedgerOB();

            Nullable<int> CompanyID = Convert.ToInt32(Request.QueryString["CompanyID"]);
            
            DateTime? StartDate;
            if (!string.IsNullOrEmpty(Request.QueryString["StartDate"]))
                StartDate = Convert.ToDateTime(Request.QueryString["StartDate"]);
            else
                StartDate = null;

            DateTime? EndDate;
            if (!string.IsNullOrEmpty(Request.QueryString["EndDate"]))
                EndDate = Convert.ToDateTime(Request.QueryString["EndDate"]);
            else
                EndDate = null;

            string GLAccountNoStart = Convert.ToString(Request.QueryString["GLAccountNoStart"]) == "" ? null : Convert.ToString(Request.QueryString["GLAccountNoStart"]);
            string GLAccountNoEnd = Convert.ToString(Request.QueryString["GLAccountNoTo"]) == "" ? null : Convert.ToString(Request.QueryString["GLAccountNoTo"]);

            var GeneralLedgerReportModel = db.spRptGeneralLedgerOB(CompanyID, StartDate, EndDate, GLAccountNoStart, GLAccountNoEnd).ToList();
            var ReportData = (

                         from v in GeneralLedgerReportModel
                         select new spRptGeneralLedgerOBModel
                         {

                             CompanyID = v.CompanyID.GetValueOrDefault(0),
                             Company = v.Company,
                             GLAccountNo = v.GLAccountNo ?? "",
                             Description = v.Description ?? "",
                             VoucherDate = v.VoucherDate.GetValueOrDefault(DateTime.Now),
                             VTYPE = v.VTYPE == string.Empty ? "" : v.VTYPE,
                             GLNarration = v.GLNarration == string.Empty ? "" : v.GLNarration,
                             VoucherNumber = v.VoucherNumber == string.Empty ? "" : v.VoucherNumber,
                             Debit = v.Debit ?? 0,
                             Credit = v.Credit ?? 0,
                             Balance = v.Balance ?? 0,
                             type = v.type ?? "",
                             OB = v.OB ?? 0
                         }).ToList();


            //var GeneralLedgerReportData = (
            //    from v in GeneralLedgerReportModel
            //    select new
            //    {
            //        CompanyID = v.CompanyID,
            //        Company = v.Company,
            //        GLAccountNo = v.GLAccountNo ?? "",
            //        Description = v.Description ?? "",
            //        VoucherDate = v.VoucherDate ?? DateTime.MinValue,
            //        VTYPE = v.VTYPE ?? "",
            //        VoucherNumber = v.VoucherNumber ?? "",
            //        GLNarration = v.GLNarration ?? "",
            //        Debit = v.Debit ?? 0,
            //        Credit = v.Credit ?? 0,
            //        Balance = v.Balance ?? 0,
            //    }).ToList();

            //ReportDocument reportPdf = new ReportDocument();
            //report.Load("/pdf/" + report);
            ////report.Load("pdfYourReportPath.rpt");
            //report.ExportToDisk(ExportFormatType.PortableDocFormat, "/pdf/" + report);


            reportQueue.Enqueue(report);

            report.SetDataSource(ReportData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();


        }

        private void GeneralLedgerReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptGeneralLedger();

            Nullable<int> CompanyID = Convert.ToInt32(Request.QueryString["CompanyID"]);
            DateTime? StartDate;
            if (!string.IsNullOrEmpty(Request.QueryString["StartDate"]))
                StartDate = Convert.ToDateTime(Request.QueryString["StartDate"]);
            else
                StartDate = null;

            DateTime? EndDate;
            if (!string.IsNullOrEmpty(Request.QueryString["EndDate"]))
                EndDate = Convert.ToDateTime(Request.QueryString["EndDate"]);
            else
                EndDate = null;

            string GLAccountNoStart = Convert.ToString(Request.QueryString["GLAccountNoStart"]) == "" ? null : Convert.ToString(Request.QueryString["GLAccountNoStart"]);
            string GLAccountNoEnd = Convert.ToString(Request.QueryString["GLAccountNoTo"]) == "" ? null : Convert.ToString(Request.QueryString["GLAccountNoTo"]);

            var GeneralLedgerReportModel = db.spRptGeneralLedger(CompanyID, StartDate, EndDate, GLAccountNoStart, GLAccountNoEnd).ToList();


            var GeneralLedgerReportData = (
                from v in GeneralLedgerReportModel
                select new
                {
                    CompanyID = v.CompanyID,
                    Company = v.Company,
                    GLAccountNo = v.GLAccountNo ?? "",
                    Description = v.Description ?? "",
                    VoucherDate = v.VoucherDate ?? DateTime.MinValue,
                    VTYPE = v.VTYPE ?? "",
                    VoucherNumber = v.VoucherNumber ?? "",
                    GLNarration = v.GLNarration ?? "",
                    Debit = v.Debit ?? 0,
                    Credit = v.Credit ?? 0,
                    Balance = v.Balance ?? 0,
                }).ToList();

            //ReportDocument reportPdf = new ReportDocument();
            //report.Load("/pdf/" + report);
            ////report.Load("pdfYourReportPath.rpt");
            //report.ExportToDisk(ExportFormatType.PortableDocFormat, "/pdf/" + report);


            reportQueue.Enqueue(report);

            report.SetDataSource(GeneralLedgerReportData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();


        }

        private void DVReceiptByIDReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new RptDVReceipt();
            Nullable<int> DVReceiptID = Convert.ToInt32(Request.QueryString["DVReceiptID"]);

            var spRptDVReceipt = db.spRptDVReceipt(DVReceiptID);

            var rptPaymentPlanData = (

                from v in spRptDVReceipt
                select new spRptDVReceiptReport
                {
                    Company = v.Company,
                    PaymentPlanTypeID = v.PaymentPlanTypeID,
                    ApplicationFormID = v.ApplicationFormID,
                    ProjectName = v.ProjectName,
                    Applicant = v.Applicant,
                    ApplicationFormNo = v.ApplicationFormNo,
                    ApplicationFormDate = v.ApplicationFormDate,
                    UnitNo = v.UnitNo,
                    UnitTypeName = v.UnitTypeName,
                    FloorNo = v.FloorNo,
                    SQFT = v.SQFT,
                    Price = v.Price,
                    UnitAmount = v.UnitAmount,
                    TokenMoney = v.TokenMoney,
                    PaymentPlanTypeCode = v.PaymentPlanTypeCode,
                    PaymentPlanTypeDesc = v.PaymentPlanTypeDesc,
                    Amount = v.Amount,
                    DVPaymentMethodID = v.DVPaymentMethodID,
                    DVPaymentMethodDesc = v.DVPaymentMethodDesc,
                    DVReceiptDate = v.DVReceiptDate,
                    DVReceiptDetailID = v.DVReceiptDetailID.Value,
                    DVReceiptID = v.DVReceiptID,
                    Locked = v.Locked,
                    LockedBy = v.LockedBy,
                    LockedByName = v.LockedByName,
                    Narration = v.Narration,
                    ReceiptID = v.ReceiptID,
                    Varified = v.Varified,
                    VarifiedBy = v.VarifiedBy,
                    VarifiedByName = v.VarifiedByName


                }).ToList();

            reportQueue.Enqueue(report);

            report.SetDataSource(rptPaymentPlanData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();
        }

        private void DVReceiptByIDReportSGO()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new RptDVReceiptSGO();
            Nullable<int> DVReceiptID = Convert.ToInt32(Request.QueryString["DVReceiptID"]);

            var spRptDVReceipt = db.spRptDVReceipt(DVReceiptID);

            var ReportData = (

                from v in spRptDVReceipt
                select new spRptDVReceiptReport
                {
                    Company = v.Company,
                    PaymentPlanTypeID = v.PaymentPlanTypeID,
                    ApplicationFormID = v.ApplicationFormID,
                    ProjectName = v.ProjectName,
                    Applicant = v.Applicant,
                    ApplicationFormNo = v.ApplicationFormNo,
                    ApplicationFormDate = v.ApplicationFormDate,
                    UnitNo = v.UnitNo,
                    UnitTypeName = v.UnitTypeName,
                    FloorNo = v.FloorNo,
                    SQFT = v.SQFT,
                    Price = v.Price,
                    UnitAmount = v.UnitAmount,
                    TokenMoney = v.TokenMoney,
                    PaymentPlanTypeCode = v.PaymentPlanTypeCode,
                    PaymentPlanTypeDesc = v.PaymentPlanTypeDesc,
                    Amount = v.Amount,
                    DVPaymentMethodID = v.DVPaymentMethodID,
                    DVPaymentMethodDesc = v.DVPaymentMethodDesc,
                    DVReceiptDate = v.DVReceiptDate,
                    DVReceiptDetailID = v.DVReceiptDetailID.Value,
                    DVReceiptID = v.DVReceiptID,
                    Locked = v.Locked,
                    LockedBy = v.LockedBy,
                    LockedByName = v.LockedByName,
                    Narration = v.Narration,
                    ReceiptID = v.ReceiptID,
                    Varified = v.Varified,
                    VarifiedBy = v.VarifiedBy,
                    VarifiedByName = v.VarifiedByName


                }).ToList();

            reportQueue.Enqueue(report);

            report.SetDataSource(ReportData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();
        }

        private void DVPaymentPlanReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptDVPaymentPlan();
            //DVReceiptReport report = new DVReceiptReport();
            Nullable<int> PaymentPlanID = Convert.ToInt32(Request.QueryString["PaymentPlanID"]);


            var spRptDVPaymentPlan = db.spRptPaymentPlan(PaymentPlanID).ToList();


            var DVReceiptReporData = (

                from v in spRptDVPaymentPlan
                select new spRptPaymentPlan
                {
                    Company = v.Company,
                    PaymentPlanDate = v.PaymentPlanDate,
                    PaymentStartDate = v.PaymentStartDate,
                    PaymentPlanID = v.PaymentPlanID,
                    PaymentPlanTypeID = v.PaymentPlanTypeID,
                    ApplicationFormID = v.ApplicationFormID,
                    ProjectName = v.ProjectName,
                    ApplicationFormNo = v.ApplicationFormNo,
                    ApplicationFormDate = v.ApplicationFormDate,
                    UnitNo = v.UnitNo,
                    UnitTypeName = v.UnitTypeName,
                    FloorNo = v.FloorNo,
                    SQFT = v.SQFT,
                    Price = v.Price,
                    UnitAmount = v.UnitAmount,
                    TokenMoney = v.TokenMoney,
                    PaymentPlanDetailID = v.PaymentPlanDetailID.GetValueOrDefault(0),
                    PaymentPlanTypeCode = v.PaymentPlanTypeCode,
                    PaymentPlanTypeDesc = v.PaymentPlanTypeDesc,
                    Amount = v.Amount,
                    Percentage = v.Percentage.Value,
                    PaymentPlanDetailDate = v.PaymentPlanDetailDate.Value

                }).ToList();

            reportQueue.Enqueue(report);

            report.SetDataSource(DVReceiptReporData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();
        }

        private void DVMemberPaymentPlanStatusReportAllUnits()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptMemberPaymentPlanStatus_AllUnits();

            int ProjectID = Convert.ToInt32(Request.QueryString["ProjectID"]);
            //int UnitID = Convert.ToInt32(Request.QueryString["UnitID"]);

            var StatusDate = Convert.ToDateTime(Request.QueryString["StatusDate"]);

            var spRptMemberPaymentPlanStatus_AllUnits = db.spRptMemberPaymentPlanStatus_AllUnits(ProjectID, StatusDate).ToList();

            var ReporData = (

                from v in spRptMemberPaymentPlanStatus_AllUnits
                select new spRptMemberPaymentPlanStatus_AllUnits_Model
                {
                    Company = v.Company ?? "",
                    Applicant = v.Applicant ?? "",
                    UnitNo = v.UnitNo ?? "",
                    PossessionStatus = v.PossessionStatus ?? "",
                    AD = v.AD.GetValueOrDefault(0),
                    DP = v.DP.GetValueOrDefault(0),
                    EX = v.EX.GetValueOrDefault(0),
                    IN = v.IN.GetValueOrDefault(0),
                    MC = v.MC.GetValueOrDefault(0),
                    PS = v.PS.GetValueOrDefault(0),
                    TK = v.TK.GetValueOrDefault(0)
                }).ToList();

            if (ReporData.Count <= 0)
            {
                Response.Write("<h1>No record found</h1>");
            }
            else
            {
                reportQueue.Enqueue(report);

                report.SetDataSource(ReporData);
                CrystalReportViewer1.ReportSource = report;
                CrystalReportViewer1.RefreshReport();
            }
        }

        private void DVMemberPaymentPlanStatusReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptMemberPaymentPlanStatus();

            int ProjectID = Convert.ToInt32(Request.QueryString["ProjectID"]);
            int UnitID = Convert.ToInt32(Request.QueryString["UnitID"]);

            var StatusDate = Convert.ToDateTime(Request.QueryString["StatusDate"]);
            var EndDate = Convert.ToDateTime(Request.QueryString["StatusDate"]);


            var spRptMemberPaymentPlanStatus_PaymentPlan_List = db.spRptMemberPaymentPlanStatus_PaymentPlan(ProjectID, UnitID, StatusDate).ToList();
            var spRptMemberPaymentPlanStatus_Receipt_List = db.spRptMemberPaymentPlanStatus_Receipt(ProjectID, UnitID).ToList();

            decimal downPaymentAmountTotal = 0;
            decimal possessionAmountTotal = 0;
            decimal installmentAmountTotal = 0;


            decimal adjustmentTotal = 0;
            decimal cancellationChargesTotal = 0;
            decimal extraAreaTotal = 0;
            decimal extraWorkTotal = 0;
            decimal meterChargesTotal = 0;
            decimal ibaLegacyPaymentsTotal = 0;

            foreach (var spRptMemberPaymentPlanStatus_Receipt in spRptMemberPaymentPlanStatus_Receipt_List)
            {
                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.TokenMoney))
                {
                    spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID = Convert.ToInt32(PaymentPlanTypeEnum.DownPayment);

                }

                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.DownPayment))
                {
                    downPaymentAmountTotal += spRptMemberPaymentPlanStatus_Receipt.Amount;
                }

                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.Possession))
                {
                    possessionAmountTotal += spRptMemberPaymentPlanStatus_Receipt.Amount;
                }

                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.Installment))
                {
                    installmentAmountTotal += spRptMemberPaymentPlanStatus_Receipt.Amount;
                }

                //////////////////////////
                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.Adjustment))
                {
                    adjustmentTotal += spRptMemberPaymentPlanStatus_Receipt.Amount;
                }
                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.CancellationCharges))
                {
                    cancellationChargesTotal += spRptMemberPaymentPlanStatus_Receipt.Amount;
                }
                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.ExtraArea))
                {
                    extraAreaTotal += spRptMemberPaymentPlanStatus_Receipt.Amount;
                }
                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.ExtraWork))
                {
                    extraWorkTotal += spRptMemberPaymentPlanStatus_Receipt.Amount;
                }
                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.MeterCharges))
                {
                    meterChargesTotal += spRptMemberPaymentPlanStatus_Receipt.Amount;
                }
                if (spRptMemberPaymentPlanStatus_Receipt.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.IBALegacyPayments))
                {
                    ibaLegacyPaymentsTotal += spRptMemberPaymentPlanStatus_Receipt.Amount;
                }

            }

            foreach (var spRptMemberPaymentPlan in spRptMemberPaymentPlanStatus_PaymentPlan_List)
            {
                var dueAmount = spRptMemberPaymentPlan.DueAmount;
                if (spRptMemberPaymentPlan.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.DownPayment))
                {
                    if (downPaymentAmountTotal > dueAmount)
                    {
                        spRptMemberPaymentPlan.ReceivedAmount = dueAmount.Value;
                        downPaymentAmountTotal = downPaymentAmountTotal - dueAmount.Value;
                        spRptMemberPaymentPlan.Balance = 0;
                    }
                    else
                    {
                        spRptMemberPaymentPlan.ReceivedAmount = downPaymentAmountTotal;
                        downPaymentAmountTotal = 0;
                        spRptMemberPaymentPlan.Balance = dueAmount.GetValueOrDefault(0) - spRptMemberPaymentPlan.ReceivedAmount;
                    }
                }
                //{
                //    spRptMemberPaymentPlan.Balance = spRptMemberPaymentPlan.DueAmount.GetValueOrDefault(0) - downPaymentAmountTotal;
                //    spRptMemberPaymentPlan.ReceivedAmount = downPaymentAmountTotal;
                //}
                else if (spRptMemberPaymentPlan.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.Possession))
                {
                    spRptMemberPaymentPlan.Balance = spRptMemberPaymentPlan.DueAmount.GetValueOrDefault(0);
                    spRptMemberPaymentPlan.ReceivedAmount = possessionAmountTotal;
                }

                else if (spRptMemberPaymentPlan.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.ExtraArea))
                {
                    spRptMemberPaymentPlan.Balance = spRptMemberPaymentPlan.DueAmount.GetValueOrDefault(0);// - downPaymentAmountTotal;
                    spRptMemberPaymentPlan.ReceivedAmount = extraAreaTotal;
                }

                else if (spRptMemberPaymentPlan.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.MeterCharges))
                {
                    spRptMemberPaymentPlan.Balance = spRptMemberPaymentPlan.DueAmount.GetValueOrDefault(0);// - downPaymentAmountTotal;
                    spRptMemberPaymentPlan.ReceivedAmount = extraAreaTotal;
                }

                else if (spRptMemberPaymentPlan.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.IBALegacyPayments))
                {
                    spRptMemberPaymentPlan.Balance = spRptMemberPaymentPlan.DueAmount.GetValueOrDefault(0);// - downPaymentAmountTotal;
                    spRptMemberPaymentPlan.ReceivedAmount = ibaLegacyPaymentsTotal;
                }

                else if (spRptMemberPaymentPlan.PaymentPlanTypeID == Convert.ToInt32(PaymentPlanTypeEnum.Installment))
                {
                    if (installmentAmountTotal > dueAmount)
                    {
                        spRptMemberPaymentPlan.ReceivedAmount = dueAmount.Value;
                        installmentAmountTotal = installmentAmountTotal - dueAmount.Value;
                        spRptMemberPaymentPlan.Balance = 0;
                    }
                    else
                    {
                        spRptMemberPaymentPlan.ReceivedAmount = installmentAmountTotal;
                        installmentAmountTotal = 0;
                        spRptMemberPaymentPlan.Balance = dueAmount.GetValueOrDefault(0) - spRptMemberPaymentPlan.ReceivedAmount;
                    }
                }
            }


            var ReporData = (

                from v in spRptMemberPaymentPlanStatus_PaymentPlan_List
                select new spRptMemberPaymentPlanStatus_PaymentPlan
                {
                    ApplicationFormDate = v.ApplicationFormDate.Date,
                    ApplicationFormID = v.ApplicationFormID,
                    ApplicationFormNo = v.ApplicationFormNo ?? "",
                    Company = v.Company ?? "",
                    DueAmount = v.DueAmount.GetValueOrDefault(0),
                    FloorNo = v.FloorNo ?? "",
                    PaymentPlanDetailDate = v.PaymentPlanDetailDate.Value,
                    PaymentPlanType = v.PaymentPlanType ?? "",
                    Price = v.Price,
                    ProjectName = v.ProjectName ?? "",
                    ReceivedAmount = v.ReceivedAmount,
                    SQFT = v.SQFT,
                    TokenMoney = v.TokenMoney,
                    UnitAmount = v.UnitAmount,
                    UnitID = v.UnitID.GetValueOrDefault(0),
                    UnitNo = v.UnitNo ?? "",
                    UnitTypeName = v.UnitTypeName ?? "",
                    Balance = v.DueAmount.GetValueOrDefault(0) - v.ReceivedAmount //v.Balance

                }).ToList();

            reportQueue.Enqueue(report);

            report.SetDataSource(ReporData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();

        }

        private void DVUnitLedgerReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptDVUnitLedger();

            int ProjectID = Convert.ToInt32(Request.QueryString["ProjectID"]);
            int UnitID = Convert.ToInt32(Request.QueryString["UnitID"]);
            DateTime LedgerDate = Convert.ToDateTime(Request.QueryString["LedgerDate"]);


            var spRptDVUnitLedgerList = db.spRptDVUnitLedger(ProjectID, UnitID, LedgerDate).ToList();


            var DVReceiptReportData = (

                from v in spRptDVUnitLedgerList
                select new spRptDVUnitLedger
                {
                    Company = v.Company,
                    ApplicationFormID = v.ApplicationFormID,
                    ProjectName = v.ProjectName,
                    Customer = v.Customer,
                    ApplicationFormNo = v.ApplicationFormNo,
                    ApplicationFormDate = v.ApplicationFormDate,
                    UnitNo = v.UnitNo,
                    UnitTypeName = v.UnitTypeName,
                    FloorNo = v.FloorNo,
                    SQFT = v.SQFT,
                    Price = v.Price,
                    UnitAmount = v.UnitAmount,
                    TokenMoney = v.TokenMoney,
                    PaymentPlanTypeDesc = v.PaymentPlanTypeDesc,
                    Credit = v.Credit,
                    Balance = v.Balance.GetValueOrDefault(0),
                    Date = v.Date.GetValueOrDefault(DateTime.Now),
                    Debit = v.Debit,
                    DVPaymentMethodDesc = v.DVPaymentMethodDesc,
                    LedgerDate = v.LedgerDate,
                    Narration = v.Narration,
                    RefNo = v.RefNo,
                    UnitID = v.UnitID.GetValueOrDefault(0),


                }).ToList();

            reportQueue.Enqueue(report);

            report.SetDataSource(DVReceiptReportData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();
        }

        private void APPartyLedgerReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptAPPartyLedger();
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];

            int? APVendorID = Convert.ToInt32(Request.QueryString["APVendorID"]);
            DateTime ToDate = Convert.ToDateTime(Request.QueryString["ToDate"]);


            var RptAPPartyLedgerList = db.spRptAPPartyLedger(LoginUser.CompanyID, ToDate, APVendorID).ToList();


            var APPartyLedgerReporData = (

                from v in RptAPPartyLedgerList
                select new spRptAPPartyLedgerReport
                {
                    //InvNo,TransDate,APVendorName,Narration,Dr, Cr,(Dr + Cr) Balance,Company
                    InvNo = v.InvNo,
                    TransDate = v.TransDate.GetValueOrDefault(DateTime.Now),
                    APVendorName = v.APVendorName,
                    Narration = v.Narration,
                    Company = v.Company ?? "",
                    Dr = v.Dr.GetValueOrDefault(0),
                    Cr = v.Cr,
                    Balance = v.Balance.GetValueOrDefault(0),

                }).ToList();

            reportQueue.Enqueue(report);

            report.SetDataSource(APPartyLedgerReporData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();
        }



        private void DVApplicationFormReport()
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            report = new rptApplicationForm();

            int ApplicationFormID = Convert.ToInt32(Request.QueryString["ApplicationFormID"]);

            var spRptDVApplicationList = db.spRptDVApplication(ApplicationFormID).ToList();


            var DVReceiptReporData = (

                from v in spRptDVApplicationList
                select new spRptDVApplication
                {
                    ApplicationFormDate = v.ApplicationFormDate.GetValueOrDefault(DateTime.Now),
                    ApplicationFormID = ApplicationFormID,
                    ApplicationFormNo = v.ApplicationFormNo ?? "",
                    SQFT = v.SQFT.GetValueOrDefault(0),
                    UnitTypeName = v.UnitTypeName ?? "",
                    UnitTypeID = v.UnitTypeID.GetValueOrDefault(0),
                    FloorNo = v.FloorNo ?? "",
                    TokenMoney = v.TokenMoney.GetValueOrDefault(0),
                    UnitID = v.UnitID.GetValueOrDefault(0),
                    UnitNo = v.UnitNo ?? "",
                    CompanyID = v.CompanyID,
                    CreatedAt = v.CreatedAt.GetValueOrDefault(DateTime.Now),
                    IsActive = v.IsActive.GetValueOrDefault(false),
                    ModifiedAt = v.ModifiedAt.GetValueOrDefault(DateTime.Now),
                    ModifiedBy = v.ModifiedBy.GetValueOrDefault(0),
                    Price = v.Price.GetValueOrDefault(0),
                    ProjectID = v.ProjectID.GetValueOrDefault(0),
                    ProjectName = v.ProjectName ?? "",



                    Area_1 = v.Area_1 ?? "",
                    Citizen_1 = v.Citizen_1 ?? "",
                    City2_1 = v.City2_1 ?? "",
                    City_1 = v.City_1 ?? "",
                    Country2_1 = v.Country2_1 ?? "",
                    Country_1 = v.Country_1 ?? "",
                    District_1 = v.District_1 ?? "",
                    CorrespondenceAdressHouseNo_1 = v.CorrespondenceAdressHouseNo_1 ?? "",
                    CreatedBy = v.CreatedBy.GetValueOrDefault(0),
                    Division_1 = v.Division_1 ?? "",
                    Email_1 = v.Email_1 ?? "",
                    GenderID_1 = v.GenderID_1,
                    HouseNo_1 = v.HouseNo_1 ?? "",
                    HusbandFather_1 = v.HusbandFather_1 ?? "",
                    ImagePath_1 = v.ImagePath_1 ?? "",
                    Living_1 = v.Living_1 ?? "",
                    Name_1 = v.Name_1 ?? "",
                    NICOP_CNIC_1 = v.NICOP_CNIC_1 ?? "",
                    NomineeFatherHusbandName_1 = v.NomineeFatherHusbandName_1 ?? "",
                    Nominee_NICOP_CNIC_1 = v.Nominee_NICOP_CNIC_1 ?? "",
                    NomioneeName_1 = v.NomioneeName_1 ?? "",
                    Phone2_1 = v.Phone2_1 ?? "",
                    Phone3_1 = v.Phone3_1 ?? "",
                    PhoneNo_1 = v.PhoneNo_1 ?? "",
                    Profession_1 = v.Profession_1 ?? "",
                    Relation_1 = v.Relation_1 ?? "",
                    StreetArea2_1 = v.StreetArea2_1 ?? "",
                    StreetArea_1 = v.StreetArea_1 ?? "",
                    SubProfession_1 = v.SubProfession_1 ?? "",
                    Tax_1 = v.Tax_1 ?? "",



                    Area_2 = v.Area_2 ?? "",
                    Citizen_2 = v.Citizen_2 ?? "",
                    City2_2 = v.City2_2 ?? "",
                    City_2 = v.City_2 ?? "",
                    Country2_2 = v.Country2_2 ?? "",
                    Country_2 = v.Country_2 ?? "",
                    District_2 = v.District_2 ?? "",
                    CorrespondenceAdressHouseNo_2 = v.CorrespondenceAdressHouseNo_2 ?? "",
                    Division_2 = v.Division_2 ?? "",
                    Email_2 = v.Email_2 ?? "",
                    GenderID_2 = v.GenderID_2,
                    HouseNo_2 = v.HouseNo_2 ?? "",
                    HusbandFather_2 = v.HusbandFather_2 ?? "",
                    ImagePath_2 = v.ImagePath_2 ?? "",
                    Living_2 = v.Living_2 ?? "",
                    Name_2 = v.Name_2 ?? "",
                    NICOP_CNIC_2 = v.NICOP_CNIC_2 ?? "",
                    NomineeFatherHusbandName_2 = v.NomineeFatherHusbandName_2 ?? "",
                    Nominee_NICOP_CNIC_2 = v.Nominee_NICOP_CNIC_2 ?? "",
                    NomioneeName_2 = v.NomioneeName_2 ?? "",
                    Phone2_2 = v.Phone2_2 ?? "",
                    Phone3_2 = v.Phone3_2 ?? "",
                    PhoneNo_2 = v.PhoneNo_2 ?? "",
                    Profession_2 = v.Profession_2 ?? "",
                    Relation_2 = v.Relation_2 ?? "",
                    StreetArea2_2 = v.StreetArea2_2 ?? "",
                    StreetArea_2 = v.StreetArea_2 ?? "",
                    SubProfession_2 = v.SubProfession_2 ?? "",
                    Tax_2 = v.Tax_2 ?? "",

                }).ToList();

            reportQueue.Enqueue(report);

            report.SetDataSource(DVReceiptReporData);
            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            int cnd = reportQueue.Count;
            if (reportQueue.Count > 7)
            {
                ((ReportClass)reportQueue.Dequeue()).Dispose();
            }

            if (report != null)
            {

                report.Close();
                report.Dispose();
            }
        }


    }
}