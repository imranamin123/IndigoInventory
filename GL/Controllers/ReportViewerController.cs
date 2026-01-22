using Azure.Core;
using static Azure.Core.HttpHeader;
using System.Collections.Generic;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;

72 % of storage used … If you run out of space, you can't save to Drive, back up Google Photos, or use Gmail. Get 30 GB of storage for Rs 189.00 Rs 99.00/month for 6 months.
﻿using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuickErp.Models.EF;
using QuickErpCSharp.Models;
using QuickErp.Models.DTO;
using QuickErp.Models.EF;
using QuickErp.Models.Helpers;
using QuickErp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;
using QuickErp.Controllers;
using QuickErp.Models.Attributes;

namespace QuickErpMvc.Controllers
{

    [Authorize(), MenuSecurity(MenuID = 10004)]
    public class ReportViewerController : QuickErpMvcController
    {
        public log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        QuickErpEF db = new QuickErpEF();

        #region "ActionMethods"

        public ActionResult Index()
        {
            ReportViewerViewModel reportViewerViewModel = new ReportViewerViewModel();

            try
            {
                log.Info("Started");
                Boolean isSaveDateRange = false;
                Boolean isSaveReportFormatting = false;
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                Boolean borderOnCompanyLogo = true;
                Boolean newPageOnGroup = true;
                Boolean repeatHeaderOnEachPage = true;
                var db = new QuickErpEF();

                var warehouse = db.InvWarehouses
                    .Where(w => w.CompanyID == Common.LoginData.ImpersonateCompany.CompanyID && w.WarehouseID == Common.LoginData.ImpersonateUser.WarehouseID)
                    .FirstOrDefault();
                Boolean.TryParse(AppSetting.GetSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "IsSaveDateRange"), out isSaveDateRange);
                Boolean.TryParse(AppSetting.GetSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "IsSaveReportFormatting"), out isSaveReportFormatting);

                reportViewerViewModel.ItemType = "Both";
                reportViewerViewModel.WarehouseID = Common.LoginData.ImpersonateUser.WarehouseID;
                if (warehouse != null)
                {
                    reportViewerViewModel.WarehouseCode = warehouse.WarehouseCode;
                    reportViewerViewModel.WarehouseName = warehouse.WarehouseName;
                }

                if (isSaveDateRange)
                {
                    DateTime.TryParse(AppSetting.GetSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.StartDate"), out startDate);
                    DateTime.TryParse(AppSetting.GetSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.EndDate"), out endDate);

                    if (startDate != DateTime.MinValue) reportViewerViewModel.StartDate = startDate;
                    if (endDate != DateTime.MinValue) reportViewerViewModel.EndDate = endDate;
                }
                if (isSaveReportFormatting)
                {
                    Boolean.TryParse(AppSetting.GetSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.BorderOnCompanyLogo"), out borderOnCompanyLogo);
                    Boolean.TryParse(AppSetting.GetSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.NewPageOnGroup"), out newPageOnGroup);
                    Boolean.TryParse(AppSetting.GetSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.RepeatHeaderOnEachPage"), out repeatHeaderOnEachPage);

                    reportViewerViewModel.BorderOnCompanyLogo = borderOnCompanyLogo;
                    reportViewerViewModel.NewPageOnGroup = newPageOnGroup;
                    reportViewerViewModel.RepeatHeaderOnEachPage = repeatHeaderOnEachPage;
                }

                return View(reportViewerViewModel);

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return View(reportViewerViewModel);
            }
        }

        [HttpPost()]
        public async Task<ActionResult> Index(ReportViewerViewModel model)
        {
            ActionResultJson<string> _ActionResultJson = new ActionResultJson<string>();
            CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            System.IO.Stream IOStream = default(System.IO.Stream);
            string contentType = "application/vnd.ms-excel";
            string extension = null;
            string _ReportGuid = null;
            DateTime _StartTime = DateTime.UtcNow;
            ActionResult actionResult = null;

            try
            {
                log.Info("Started");

                model.ActionType = model.ActionType ?? "download";
                model.ExportFormat = model.ExportFormat ?? "pdf";

                #region "Save Settings"

                AppSetting.SaveSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.StartDate", model.StartDate.HasValue ? model.StartDate.ToString() : "", "Report Viewer - Start Date");
                AppSetting.SaveSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.EndDate", model.EndDate.HasValue ? model.EndDate.ToString() : "", "Report Viewer - End Date");
                AppSetting.SaveSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.BorderOnCompanyLogo", model.BorderOnCompanyLogo.ToString(), "Report Viewer - BorderOnCompanyLogo");
                AppSetting.SaveSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.NewPageOnGroup", model.NewPageOnGroup.ToString(), "Report Viewer - NewPageOnGroup");
                AppSetting.SaveSetting(Common.LoginData.ImpersonateCompany.CompanyID, 0, "ReportViewer.RepeatHeaderOnEachPage", model.RepeatHeaderOnEachPage.ToString(), "Report Viewer - RepeatHeaderOnEachPage");

                #endregion

                if (IsValid(model) && model.ExportFormat.ToLower() != "xls2")
                {
                    MapValues(model, ref reportDocument);


                    if (BrokenRules.Count == 0)
                    {

                        if (model.ActionType.ToLower() == "download")
                        {
                            if (model.ExportFormat == null)
                                model.ExportFormat = string.Empty;
                            //when some other form calls report

                            switch (model.ExportFormat.ToLower())
                            {
                                case "pdf":

                                    IOStream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                    contentType = "application/pdf";
                                    extension = ".pdf";

                                    break;
                                case "xls":

                                    IOStream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                                    contentType = "application/vnd.ms-excel";
                                    extension = ".xls";

                                    break;
                                default:

                                    IOStream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                    contentType = "application/pdf";
                                    extension = ".pdf";

                                    break;
                            }

                            reportDocument.Close();
                            reportDocument.Dispose();

                            model.Result = null;

                            actionResult = File(IOStream, contentType, model.ReportName + DateTime.Now.ToString(" dd-MMM-yy") + extension);

                            actionResult = actionResult;
                        }
                        else
                        {
                            _ReportGuid = Guid.NewGuid().ToString();
                            Session[_ReportGuid] = reportDocument;

                            actionResult = Redirect("/Pages/ReportViewer.aspx?id=" + _ReportGuid);

                        }
                    }
                }
                else
                {
                    var db = new QuickErpEF();
                    var sp = new SP();
                    var report = db.Base_Report.Where(w => w.ReportID == model.ReportID).FirstOrDefault();
                    model.ReportName = report != null ? report.ReportName : "";

                    using (var excelPackage = new ExcelPackage())
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add(model.ReportName);
                        var rowNo = 1;

                        switch (model.ReportID)
                        {
                            case 5: // General Ledger
                                #region "General Ledger"
                                var glModel = sp.spReportGeneralLedger(Common.LoginData.ImpersonateCompany.CompanyID, model.ChartOfAccountID, model.StartDate, model.EndDate);
                                decimal balance = 0;

                                workSheet.Cells[rowNo, 1].Value = "General Ledger";
                                workSheet.Cells[1, 1, 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[1, 1, 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                workSheet.Cells[1, 1, 2, 4].Merge = true;
                                workSheet.Cells[1, 1, 2, 6].Style.Font.Bold = true;
                                workSheet.Cells[rowNo, 5].Value = "Start Date";
                                workSheet.Cells[rowNo, 6].Value = model.StartDate.HasValue ? model.StartDate.Value.ToString("dd-MMM-yyyy") : "";

                                rowNo++;
                                workSheet.Cells[rowNo, 5].Value = "End Date";
                                workSheet.Cells[rowNo, 6].Value = model.EndDate.HasValue ? model.EndDate.Value.ToString("dd-MMM-yyyy") : "";
                                workSheet.Cells[rowNo, 6].Style.Font.Bold = true;

                                rowNo++;
                                workSheet.Cells[rowNo, 1].Value = "Voucher No";
                                workSheet.Cells[rowNo, 2].Value = "Voucher Date";
                                workSheet.Cells[rowNo, 3].Value = "Narration";
                                workSheet.Cells[rowNo, 4].Value = "Debit";
                                workSheet.Cells[rowNo, 5].Value = "Credit";
                                workSheet.Cells[rowNo, 6].Value = "Balance";
                                workSheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;

                                foreach (var row in glModel)
                                {
                                    rowNo++;
                                    balance += (row.DebitAmount ?? 0) - (row.CreditAmount ?? 0);

                                    workSheet.Cells[rowNo, 1].Value = row.VoucherNumber;
                                    workSheet.Cells[rowNo, 2].Value = row.VoucherDate;
                                    workSheet.Cells[rowNo, 3].Value = row.Narration;
                                    workSheet.Cells[rowNo, 4].Value = row.DebitAmount;
                                    workSheet.Cells[rowNo, 5].Value = row.CreditAmount;
                                    workSheet.Cells[rowNo, 6].Value = balance;
                                }

                                workSheet.Cells[1, 2, rowNo, 2].Style.Numberformat.Format = "dd-MMM-yyyy";
                                workSheet.Cells[1, 4, rowNo, 4].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 5, rowNo, 5].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 6, rowNo, 6].Style.Numberformat.Format = "#,##0";
                                workSheet.Column(1).Width = 20;
                                workSheet.Column(2).Width = 15;
                                workSheet.Column(3).Width = 50;
                                workSheet.Column(4).Width = 15;
                                workSheet.Column(5).Width = 15;
                                workSheet.Column(6).Width = 15;

                                break;
                            #endregion
                            case 7:
                                #region "Trial Balance"
                                var tbModel = sp.spReportTrialBalance(Common.LoginData.ImpersonateCompany.CompanyID, model.StartDate, model.EndDate);

                                workSheet.Cells[rowNo, 1].Value = "Trial Balance";
                                workSheet.Cells[1, 1, 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[1, 1, 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                workSheet.Cells[1, 1, 1, 4].Merge = true;
                                workSheet.Cells[1, 1, 2, 6].Style.Font.Bold = true;
                                workSheet.Cells[rowNo, 5].Value = "Start Date";
                                workSheet.Cells[rowNo, 6].Value = model.StartDate.HasValue ? model.StartDate.Value.ToString("dd-MMM-yyyy") : "";

                                rowNo++;
                                workSheet.Cells[rowNo, 5].Value = "End Date";
                                workSheet.Cells[rowNo, 6].Value = model.EndDate.HasValue ? model.EndDate.Value.ToString("dd-MMM-yyyy") : "";
                                workSheet.Cells[rowNo, 6].Style.Font.Bold = true;

                                workSheet.Cells[rowNo, 1].Value = "Code";
                                workSheet.Cells[rowNo, 2].Value = "Chart of Account";
                                workSheet.Cells[rowNo, 3].Value = "Opening";
                                workSheet.Cells[rowNo, 4].Value = "Debit";
                                workSheet.Cells[rowNo, 5].Value = "Credit";
                                workSheet.Cells[rowNo, 6].Value = "Closing";
                                workSheet.Cells[1, 1, 1, 7].Style.Font.Bold = true;

                                foreach (var row in tbModel)
                                {
                                    rowNo++;

                                    workSheet.Cells[rowNo, 1].Value = row.ChartOfAccountCode;
                                    workSheet.Cells[rowNo, 2].Value = row.ChartOfAccountName;
                                    workSheet.Cells[rowNo, 3].Value = row.OpeningDebit - row.OpeningCredit;
                                    workSheet.Cells[rowNo, 4].Value = row.Debit;
                                    workSheet.Cells[rowNo, 5].Value = row.Credit;
                                    workSheet.Cells[rowNo, 6].Value = row.ClosingDebit - row.ClosingCredit;
                                }

                                workSheet.Cells[1, 3, rowNo, 3].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 4, rowNo, 4].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 5, rowNo, 5].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 6, rowNo, 6].Style.Numberformat.Format = "#,##0";
                                workSheet.Column(1).Width = 20;
                                workSheet.Column(2).Width = 50;
                                workSheet.Column(3).Width = 15;
                                workSheet.Column(4).Width = 15;
                                workSheet.Column(5).Width = 15;
                                workSheet.Column(6).Width = 15;

                                break;
                            #endregion
                            case 31:
                                #region "Year Budget"
                                var ybModel = sp.spReportYearBudget(Common.LoginData.ImpersonateCompany.CompanyID, null);

                                workSheet.Cells[rowNo, 1].Value = "Year Budget";
                                workSheet.Cells[1, 1, 1, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[1, 1, 1, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                workSheet.Cells[1, 1, 1, 15].Merge = true;

                                rowNo++;
                                workSheet.Cells[rowNo, 1].Value = "Chart of Account";
                                workSheet.Cells[rowNo, 2].Value = "Buget";
                                workSheet.Cells[rowNo, 3].Value = "Jan";
                                workSheet.Cells[rowNo, 4].Value = "Feb";
                                workSheet.Cells[rowNo, 5].Value = "Mar";
                                workSheet.Cells[rowNo, 6].Value = "Apr";
                                workSheet.Cells[rowNo, 7].Value = "May";
                                workSheet.Cells[rowNo, 8].Value = "Jun";
                                workSheet.Cells[rowNo, 9].Value = "Jul";
                                workSheet.Cells[rowNo, 10].Value = "Aug";
                                workSheet.Cells[rowNo, 11].Value = "Sep";
                                workSheet.Cells[rowNo, 12].Value = "Oct";
                                workSheet.Cells[rowNo, 13].Value = "Nov";
                                workSheet.Cells[rowNo, 14].Value = "Dec";
                                workSheet.Cells[rowNo, 15].Value = "Balance";

                                workSheet.Cells[1, 1, 2, 15].Style.Font.Bold = true;

                                foreach (var row in ybModel)
                                {
                                    rowNo++;

                                    workSheet.Cells[rowNo, 1].Value = row.ChartOfAccountName;
                                    workSheet.Cells[rowNo, 2].Value = row.BudgetAmount;
                                    workSheet.Cells[rowNo, 3].Value = row.Jan;
                                    workSheet.Cells[rowNo, 4].Value = row.Feb;
                                    workSheet.Cells[rowNo, 5].Value = row.Mar;
                                    workSheet.Cells[rowNo, 6].Value = row.Apr;
                                    workSheet.Cells[rowNo, 7].Value = row.May;
                                    workSheet.Cells[rowNo, 8].Value = row.Jun;
                                    workSheet.Cells[rowNo, 9].Value = row.Jul;
                                    workSheet.Cells[rowNo, 10].Value = row.Aug;
                                    workSheet.Cells[rowNo, 11].Value = row.Sep;
                                    workSheet.Cells[rowNo, 12].Value = row.Oct;
                                    workSheet.Cells[rowNo, 13].Value = row.Nov;
                                    workSheet.Cells[rowNo, 14].Value = row.Dec;
                                    workSheet.Cells[rowNo, 15].Value = Convert.ToDecimal(row.BudgetAmount) - row.Jan - row.Feb - row.Mar - row.Apr - row.May - row.Jun - row.Jul - row.Aug - row.Sep - row.Oct - row.Nov - row.Dec;
                                }

                                workSheet.Cells[1, 2, rowNo, 2].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 3, rowNo, 3].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 4, rowNo, 4].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 5, rowNo, 5].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 6, rowNo, 6].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 7, rowNo, 7].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 8, rowNo, 8].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 9, rowNo, 9].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 10, rowNo, 10].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 11, rowNo, 11].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 12, rowNo, 12].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 13, rowNo, 13].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 14, rowNo, 14].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 15, rowNo, 15].Style.Numberformat.Format = "#,##0";
                                workSheet.Column(1).Width = 50;
                                workSheet.Column(2).Width = 20;
                                workSheet.Column(3).Width = 15;
                                workSheet.Column(4).Width = 15;
                                workSheet.Column(5).Width = 15;
                                workSheet.Column(6).Width = 15;
                                workSheet.Column(7).Width = 15;
                                workSheet.Column(8).Width = 15;
                                workSheet.Column(9).Width = 15;
                                workSheet.Column(10).Width = 15;
                                workSheet.Column(11).Width = 15;
                                workSheet.Column(12).Width = 15;
                                workSheet.Column(13).Width = 15;
                                workSheet.Column(14).Width = 15;
                                workSheet.Column(15).Width = 20;

                                break;
                            #endregion
                            case 32:
                                #region "Budget"
                                var budgetModel = sp.spReportBudget(Common.LoginData.ImpersonateCompany.CompanyID, null);

                                workSheet.Cells[rowNo, 1].Value = "Budget";
                                workSheet.Cells[1, 1, 1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[1, 1, 1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                workSheet.Cells[1, 1, 1, 4].Merge = true;

                                rowNo++;
                                workSheet.Cells[rowNo, 1].Value = "Chart of Account";
                                workSheet.Cells[rowNo, 2].Value = "Buget";
                                workSheet.Cells[rowNo, 3].Value = "Consumed";
                                workSheet.Cells[rowNo, 4].Value = "Balance";

                                workSheet.Cells[1, 1, 2, 4].Style.Font.Bold = true;

                                foreach (var row in budgetModel)
                                {
                                    rowNo++;

                                    workSheet.Cells[rowNo, 1].Value = row.ChartOfAccountName;
                                    workSheet.Cells[rowNo, 2].Value = row.BudgetAmount;
                                    workSheet.Cells[rowNo, 3].Value = row.ConsumedAmount;
                                    workSheet.Cells[rowNo, 4].Value = Convert.ToDecimal(row.BudgetAmount) - row.ConsumedAmount;
                                }

                                workSheet.Cells[1, 2, rowNo, 2].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 3, rowNo, 3].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[1, 4, rowNo, 4].Style.Numberformat.Format = "#,##0";
                                workSheet.Column(1).Width = 50;
                                workSheet.Column(2).Width = 20;
                                workSheet.Column(3).Width = 20;
                                workSheet.Column(4).Width = 20;

                                break;
                            #endregion

                            default:
                                this.BrokenRules.Add("This report does not support excel file without formatting");
                                break;
                        }

                        var fileName = Server.MapPath(@"\Files\UserFiles\" + Common.LoginData.ImpersonateCompany.CompanyID);
                        if (!System.IO.Directory.Exists(fileName)) System.IO.Directory.CreateDirectory(fileName);
                        fileName += @"\" + Guid.NewGuid().ToString() + ".xlsx";
                        excelPackage.SaveAs(new System.IO.FileInfo(fileName));

                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        extension = ".xlsx";
                        actionResult = File(fileName, contentType, model.ReportName + DateTime.Now.ToString(" dd-MMM-yy HHmm") + extension);
                    }
                }

                if (BrokenRules.Count > 0)
                {
                    _ActionResultJson.broken_rules = this.BrokenRules;
                    _ActionResultJson.http_code = System.Net.HttpStatusCode.BadRequest;
                    _ActionResultJson.message = "Please correct following errors:";
                    model.Result = _ActionResultJson;

                }

                if (actionResult == null)
                    actionResult = View("Index", model);

                log.Debug("Completed - Report '" + model.ReportName + "' generated in " + DateTime.UtcNow.Subtract(_StartTime).TotalMinutes.ToString("##.#") + " mins");
                log.UserAction(string.Format("Company: {0} - User: {1}({2}) - Completed Report '{3}' generated in {4} secs", Common.LoginData.ImpersonateCompany.CompanyName, Common.LoginData.ImpersonateUser.UserName, Common.LoginData.ImpersonateUser.UserFullName, model.ReportName, DateTime.UtcNow.Subtract(_StartTime).TotalSeconds.ToString()));

                return await Task.Run(() => actionResult);

            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.BrokenRules.Add(ex.Message);
                model.Result = new ActionResultJson<string>
                {
                    http_code = System.Net.HttpStatusCode.InternalServerError,
                    message = ex.Message,
                    broken_rules = this.BrokenRules
                };
                return Json(new ActionResultJson<string>
                {
                    http_code = HttpStatusCode.InternalServerError,
                    message = ex.Message,
                    broken_rules = this.BrokenRules
                });
                //return View("Index", model);
            }
        }

        #endregion

        #region "Methods"

        private void MapValues(ReportViewerViewModel _ReportViewModel, ref CrystalDecisions.CrystalReports.Engine.ReportDocument _reportDocument)
        {
            try
            {
                log.Info("Started");

                LoadReport(_ReportViewModel, ref _reportDocument);
                SetParameterFields(_ReportViewModel, ref _reportDocument);
                SetFormulaFields(_ReportViewModel, _reportDocument);
                SetConnection(_reportDocument);

                foreach (CrystalDecisions.CrystalReports.Engine.ReportDocument _SubReport in _reportDocument.Subreports)
                {
                    //SetParameterFields(_ReportViewModel, _SubReport)
                    SetFormulaFields(_ReportViewModel, _SubReport);
                    //SetConnection(_SubReport)

                }

                log.Debug("Completed");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        private bool IsValid(ReportViewerViewModel _Request)
        {
            try
            {
                log.Info("Started");

                if (_Request == null)
                {
                    this.BrokenRules.Add("Invalid data format");

                }
                else
                {
                    if (_Request.ReportID == 0)
                    {
                        this.BrokenRules.Add("Invalid report, select valid report");
                    }

                }

                log.Debug("Completed");

                return this.BrokenRules.Count == 0;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        //public JsonResult GetReports(Nullable<Int32> ReportID)
        //{
        //    try
        //    {
        //        var sp = new SP();
        //        System.Data.Entity.Infrastructure.DbRawSqlQuery<Base_Report> _BaseReport = default(System.Data.Entity.Infrastructure.DbRawSqlQuery<Base_Report>);
        //        List<ReportViewerViewModel> reports = new List<ReportViewerViewModel>();
        //        ReportViewerViewModel report = default(ReportViewerViewModel);
        //        string _ReportSessionKey = null;

        //        log.Info("Started");

        //        _ReportSessionKey = "Report_" + ReportID.ToString() + "_" + Common.LoginData.ImpersonateCompany.CompanyID.ToString() + "_" + Common.LoginData.ImpersonateUser.UserID.ToString();

        //        if (Session[_ReportSessionKey] == null)
        //        {
        //            log.Debug("Fetching from database");

        //            _BaseReport = (new Base_Report()).GetActiveReports(ReportID);
        //            var model = sp.spBaseReportGetActiveByParentIDUserAutoID(ReportID, Common.LoginData.ImpersonateUser.UserAutoID);


        //            foreach (var row in model)
        //            {
        //                    report = new ReportViewerViewModel();

        //                    report.ReportID = row.ReportID;
        //                    report.ReportName = row.ReportName;
        //                    //report.HasChildren = row.HasChildren();

        //                    reports.Add(report);

        //            }

        //            Session[_ReportSessionKey] = reports;

        //        }
        //        else
        //        {
        //            log.Debug("Fetching from session");

        //            reports = (List<ReportViewerViewModel>)Session[_ReportSessionKey];

        //        }

        //        log.Debug("Completed");

        //        return Json(reports, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        return Json(null);
        //    }
        //}

        public JsonResult GetReportsForTreeView()
        {
            try
            {
                var treeView = GetReport(null);

                return Json(treeView, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public void SetConnection(CrystalDecisions.CrystalReports.Engine.ReportDocument ReportDocument)
        {
            try
            {
                CrystalDecisions.CrystalReports.Engine.ReportDocument SubReportDocument = default(CrystalDecisions.CrystalReports.Engine.ReportDocument);
                System.Data.SqlClient.SqlConnectionStringBuilder ConnectionString = default(System.Data.SqlClient.SqlConnectionStringBuilder);
                CrystalDecisions.Shared.ConnectionInfo ConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();

                log.Debug("Started");

                ConnectionString = new System.Data.SqlClient.SqlConnectionStringBuilder(System.Configuration.ConfigurationManager.ConnectionStrings["QuickErpAdoNet"].ConnectionString);

                ConnectionInfo.DatabaseName = ConnectionString.InitialCatalog;
                ConnectionInfo.ServerName = ConnectionString.DataSource;
                ConnectionInfo.UserID = ConnectionString.UserID;
                ConnectionInfo.Password = ConnectionString.Password;

                foreach (CrystalDecisions.CrystalReports.Engine.Table Table in ReportDocument.Database.Tables)
                {
                    CrystalDecisions.Shared.TableLogOnInfo TableLogOnInfo = Table.LogOnInfo;

                    TableLogOnInfo.ConnectionInfo = ConnectionInfo;
                    Table.ApplyLogOnInfo(TableLogOnInfo);

                }

                for (Int32 I = 0; I <= ReportDocument.Subreports.Count - 1; I++)
                {
                    SubReportDocument = ReportDocument.OpenSubreport(ReportDocument.Subreports[I].Name);

                    foreach (CrystalDecisions.CrystalReports.Engine.Table Table in SubReportDocument.Database.Tables)
                    {
                        CrystalDecisions.Shared.TableLogOnInfo TableLogOnInfo = Table.LogOnInfo;

                        TableLogOnInfo.ConnectionInfo = ConnectionInfo;
                        Table.ApplyLogOnInfo(TableLogOnInfo);

                    }

                }

                foreach (CrystalDecisions.Shared.IConnectionInfo connection in ReportDocument.DataSourceConnections)
                {
                    connection.SetConnection(ConnectionInfo.ServerName, ConnectionInfo.DatabaseName, ConnectionInfo.UserID, ConnectionInfo.Password);
                }

                log.Debug("Completed");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }

        }

        public void LoadReport(ReportViewerViewModel reportViewModel, ref CrystalDecisions.CrystalReports.Engine.ReportDocument _reportDocument)
        {
            try
            {
                Base_Report _Report = default(Base_Report);

                log.Info("Started");

                using (QuickErpEF _DB = new QuickErpEF())
                {
                    _Report = _DB.Base_Report.Where(w => w.ReportID == reportViewModel.ReportID).FirstOrDefault();

                }

                if (_Report != null && !string.IsNullOrEmpty(_Report.ReportPath))
                {
                    reportViewModel.ReportName = _Report.ReportName;
                    if (Common.LoginData.ImpersonateCompany.CompanyID == 26)    //Have to hardcode this for now unless it is handled by security and different formats.
                    {
                        _Report.ReportPath = _Report.ReportPath.Replace("TrialBalance.rpt", "TrialBalanceFormat2.rpt");
                    }
                    _reportDocument.Load(Server.MapPath(_Report.ReportPath));
                }
                else
                {
                    log.Error(string.Format("Report ID '{0}' could not be found in the database", reportViewModel.ReportID));
                }

                log.Debug("Completed - Loaded report: " + _reportDocument.FileName);

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public void SetParameterFields(ReportViewerViewModel reportViewModel, ref CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument)
        {
            try
            {
                log.Info("Started");


                foreach (CrystalDecisions.Shared.ParameterField reportParameter in reportDocument.ParameterFields)
                {
                    switch (reportParameter.Name.ToLower())
                    {
                        case "@warehouseid":
                            reportDocument.SetParameterValue("@WarehouseID", reportViewModel.WarehouseID);

                            break;
                        case "@companyid":
                            reportDocument.SetParameterValue("@CompanyID", Common.LoginData.ImpersonateCompany.CompanyID);

                            break;
                        case "@israwmaterial":
                            if (reportViewModel.ItemType != null && reportViewModel.ItemType.ToLower() == "rawmaterial")
                            {
                                reportDocument.SetParameterValue("@IsRawMaterial", true);

                            }
                            else
                            {
                                reportDocument.SetParameterValue("@IsRawMaterial", null);

                            }

                            break;
                        case "@isfinishedgood":
                            if (reportViewModel.ItemType != null && reportViewModel.ItemType.ToLower() == "finishedgood")
                            {
                                reportDocument.SetParameterValue("@IsFinishedGood", true);
                            }
                            else
                            {
                                reportDocument.SetParameterValue("@IsFinishedGood", null);
                            }

                            break;
                        case "@showzerovaluerows":
                            reportDocument.SetParameterValue("@ShowZeroValueRows", reportViewModel.ShowZeroRows);

                            break;
                        case "@partyid":
                            if (reportViewModel.PartyID.HasValue)
                            {
                                reportDocument.SetParameterValue("@PartyID", reportViewModel.PartyID);
                            }
                            else
                            {
                                reportDocument.SetParameterValue("@PartyID", null);
                            }

                            break;
                        case "@itemid":
                            if (reportViewModel.ItemID.HasValue)
                            {
                                reportDocument.SetParameterValue("@ItemID", reportViewModel.ItemID);
                            }
                            else
                            {
                                reportDocument.SetParameterValue("@ItemID", null);
                            }

                            break;
                        case "@chartofaccountid":
                            if (reportViewModel.ChartOfAccountID.HasValue)
                            {
                                reportDocument.SetParameterValue("@ChartOfAccountID", reportViewModel.ChartOfAccountID);
                            }
                            else
                            {
                                reportDocument.SetParameterValue("@ChartOfAccountID", null);
                            }

                            break;
                        case "@startdate":
                            if (reportViewModel.StartDate.HasValue)
                            {
                                reportDocument.SetParameterValue("@StartDate", reportViewModel.StartDate);
                            }
                            else
                            {
                                reportDocument.SetParameterValue("@StartDate", null);
                            }

                            break;
                        case "@enddate":
                            if (reportViewModel.EndDate.HasValue)
                            {
                                reportDocument.SetParameterValue("@EndDate", reportViewModel.EndDate);
                            }
                            else
                            {
                                reportDocument.SetParameterValue("@EndDate", null);
                            }

                            break;
                        case "@saleid":
                            if (reportViewModel.SaleID.HasValue)
                            {
                                reportDocument.SetParameterValue("@SaleID", reportViewModel.SaleID);
                            }
                            else
                            {
                                reportDocument.SetParameterValue("@SaleID", null);
                            }

                            break;
                        case "@voucherid":
                            if (reportViewModel.VoucherID.HasValue)
                            {
                                reportDocument.SetParameterValue("@VoucherID", reportViewModel.VoucherID);
                            }
                            else
                            {
                                reportDocument.SetParameterValue("@VoucherID", null);
                            }

                            break;
                        case "@vouchertypeid":
                            if (reportViewModel.VoucherTypeID.HasValue)
                            {
                                reportDocument.SetParameterValue("@VoucherTypeID", reportViewModel.VoucherTypeID);
                            }
                            else
                            {
                                reportDocument.SetParameterValue("@VoucherTypeID", null);
                            }

                            break;
                        case "@includesubcompanies":
                            reportDocument.SetParameterValue("@IncludeSubCompanies", Common.LoginData.ImpersonateCompany.CompanyHasChildren);

                            break;
                        case "@orderid":
                            reportDocument.SetParameterValue("@OrderID", reportViewModel.OrderID);

                            break;
                        case "@year":
                            reportDocument.SetParameterValue("@Year", null);

                            break;
                        case "@budgetid":
                            reportDocument.SetParameterValue("@BudgetID", null);

                            break;
                        case "@showcustomers":
                            reportDocument.SetParameterValue("@ShowCustomers", reportViewModel.ShowCustomers);

                            break;
                        case "@showsuppliers":
                            reportDocument.SetParameterValue("@ShowSuppliers", reportViewModel.ShowSuppliers);

                            break;
                        case "@processid":
                            reportDocument.SetParameterValue("@ProcessID", reportViewModel.ProcessID);

                            break;
                        case "@transferstockid":
                            reportDocument.SetParameterValue("@TransferStockID", reportViewModel.TransferStockID);

                            break;
                        case "@grnid":
                            reportDocument.SetParameterValue("@GrnID", reportViewModel.GrnID);

                            break;
                        case "@grnreturnid":
                            reportDocument.SetParameterValue("@GrnReturnID", reportViewModel.GrnReturnID);

                            break;
                        default:
                            log.Warn("Report ID: " + reportViewModel.ReportID.ToString() + " expects parameter " + reportParameter.Name + " which is not supplied");

                            break;
                    }
                }

                log.Debug("Completed");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public void SetFormulaFields(ReportViewerViewModel _ReportViewModel, CrystalDecisions.CrystalReports.Engine.ReportDocument _reportDocument)
        {
            try
            {
                log.Info("Started");

                foreach (CrystalDecisions.CrystalReports.Engine.FormulaFieldDefinition FormulaField in _reportDocument.DataDefinition.FormulaFields)
                {
                    switch (FormulaField.Name.ToLower())
                    {
                        case "logopath":
                            if (!string.IsNullOrEmpty(Common.LoginData.ImpersonateCompany.CompanyLogoPath))
                            {
                                FormulaField.Text = "\"" + System.Web.Hosting.HostingEnvironment.MapPath(Common.LoginData.ImpersonateCompany.CompanyLogoPath) + "\"";

                            }

                            break;
                        case "warehousename":
                            FormulaField.Text = "\"" + Common.LoginData.ImpersonateCompany.WarehouseName + "\"";

                            break;
                        case "companyname":
                            FormulaField.Text = "\"" + Common.LoginData.ImpersonateCompany.CompanyName + "\"";

                            break;
                        case "companyaddress":
                            FormulaField.Text = "\"" + Common.LoginData.ImpersonateCompany.Address + "\"";

                            break;
                        case "companyphone":
                            FormulaField.Text = "\"" + Common.LoginData.ImpersonateCompany.LandlineNumber + " " + Common.LoginData.ImpersonateCompany.MobileNumber + "\"";

                            break;
                        case "companyemail":
                            FormulaField.Text = "\"" + Common.LoginData.ImpersonateCompany.EmailAddress + "\"";

                            break;
                        case "companywebsite":
                            FormulaField.Text = "\"" + Common.LoginData.ImpersonateCompany.CompanyWebsiteUrl + "\"";

                            break;
                        case "companylogopath":
                            if (!string.IsNullOrEmpty(Common.LoginData.ImpersonateCompany.CompanyLogoPath))
                            {
                                FormulaField.Text = "\"" + System.Web.Hosting.HostingEnvironment.MapPath(Common.LoginData.ImpersonateCompany.CompanyLogoPath) + "\"";
                            }

                            break;
                        case "warehousecount":
                            if (!Common.LoginData.ImpersonateUser.WarehouseID.HasValue)
                            {
                                var db = new QuickErpEF();
                                FormulaField.Text = db.InvWarehouses
                                    .Where(w => w.CompanyID == Common.LoginData.ImpersonateCompany.CompanyID)
                                    .Count().ToString();
                            }
                            else
                            {
                                FormulaField.Text = "1";
                            }

                            break;
                        case "generatedby":
                            FormulaField.Text = "\"" + Common.LoginData.ImpersonateUser.UserName + "\"";

                            break;
                        case "reportname":
                            FormulaField.Text = "\"" + _ReportViewModel.ReportName + "\"";

                            break;
                        case "generalledgerurl":
                            FormulaField.Text = "\"" + Url.Action("GeneralLedger", "Report", new { Area = "" }, Request.Url.Scheme) + "\"";

                            break;
                        case "chartofaccounturl":
                            FormulaField.Text = "\"" + Url.Action("Create", "ChartOfAccount", new { Area = "Finance" }, Request.Url.Scheme) + "\"";

                            break;
                        case "voucherurl":
                            FormulaField.Text = "\"" + Url.Action("JournalVoucherCreate", "Voucher", new { Area = "" }, Request.Url.Scheme) + "\"";

                            break;
                        case "itemurl":
                            FormulaField.Text = "\"" + Url.Action("Create", "Item", new { Area = "Inventory" }, Request.Url.Scheme) + "\"";

                            break;
                        case "simplevoucherurl":
                            FormulaField.Text = "\"" + Url.Action("Create", "SimpleVoucher", new { Area = "Finance" }, Request.Url.Scheme) + "\"";

                            break;
                        case "poweredby":
                            FormulaField.Text = "\"" + "Powered by " + Common.Brand.Name + "\"";

                            break;
                        case "repeatheader":
                            FormulaField.Text = _ReportViewModel.RepeatHeaderOnEachPage.ToString();

                            break;
                        case "newpageongroup":
                            FormulaField.Text = _ReportViewModel.NewPageOnGroup.ToString();

                            break;
                        case "borderoncompanylogo":
                            FormulaField.Text = _ReportViewModel.BorderOnCompanyLogo.ToString();

                            break;
                    }

                }

                log.Debug("Completed");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public List<TreeViewViewModel> GetReport(int? parentReportID)
        {
            try
            {
                log.Debug("Started");
                var sp = new SP();

                //var nodes = db.Base_Report
                //    .Where(w => w.ParentReportID == parentReportID)
                //    .Select(s => new TreeViewViewModel() { nodeId = s.ReportID, text = s.ReportName, href = "node-" + s.ReportID.ToString() })
                //    .ToList();
                var nodes = sp.spBaseReportGetActiveByParentIDUserAutoID(parentReportID, Common.LoginData.ImpersonateUser.UserAutoID)
                    .Select(s => new TreeViewViewModel() { nodeId = s.ReportID, text = s.ReportName, href = "node-" + s.ReportID.ToString() })
                    .ToList();

                foreach (var node in nodes)
                {
                    node.nodes = GetReport(node.nodeId);
                }

                if (nodes.Count > 0)
                {
                    return nodes;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        #endregion

    }
}