using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GL.DAL;
using GL.EF;
using GL.Models;
using GL.Reports;
using GL.ReportsWebForms;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace GL.Controllers
{
    public class ReportsController : Controller
    {
        private object srj;

        // GET: Reports
        public ActionResult TrialBalance()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.fiscalYears = dal.FiscalYearGetForDropdown(LoginUser.CompanyID);
            ViewBag.CompanyID = LoginUser.CompanyID;
            return View();
        }



        public ActionResult DownloadExcelItem(int CompanyID, int ProjectID, int? ItemID, DateTime? FromDate, DateTime? ToDate )
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Report");

                var result = new DALInventory().GetRptINItemStock(CompanyID, ProjectID, ItemID, FromDate, ToDate);

                var db = new GLEntities();

                //////var result =
                //////    (
                //////        from i in db.INItems.Include(x => x.INGroup).Include(x => x.INCategory)

                //////        join size in db.INSizes on i.SizeID equals size.SizeID into sizej
                //////        from size in sizej.DefaultIfEmpty()

                //////        join uom in db.INUnitOfMeasurements on i.UOMID equals uom.UOMID into uomj
                //////        from uom in uomj.DefaultIfEmpty()

                //////            // INNER JOIN INProjectItems
                //////        join pi in db.INProjectItems
                //////            .Where(x => x.ProjectID == ProjectID)
                //////            on i.ItemID equals pi.ItemID
                //////        //join pi in db.INProjectItems
                //////        //    .Where(x => x.ProjectID == ProjectID)
                //////        //    on i.ItemID equals pi.ItemID into pij
                //////        //from pi in pij.DefaultIfEmpty()

                //////        join mv in
                //////        (
                //////            (
                //////                // GRN
                //////                from d in db.INGoodsReceiptNoteDetails
                //////                join m in db.INGoodsReceiptNotes on d.GoodsReceiptNoteID equals m.GoodsReceiptNoteID
                //////                where m.ProjectID == ProjectID
                //////                      && m.GoodsReceiptNotesDate <= ToDate
                //////                      && (!ItemID.HasValue || d.ItemID == ItemID)
                //////                select new
                //////                {
                //////                    d.ItemID,
                //////                    TranDate = m.GoodsReceiptNotesDate,
                //////                    ReceivedQty = d.ReceivedQty ?? 0m,
                //////                    IssuedQty = 0m,
                //////                    ReturnQty = 0m,
                //////                    TransferInQty = 0m,
                //////                    TransferOutQty = 0m
                //////                }
                //////            )

                //////            .Concat(
                //////                // SIN
                //////                from d in db.INStoreIssueNoteDetails
                //////                join m in db.INStoreIssueNotes on d.StoreIssueNoteID equals m.StoreIssueNoteID
                //////                where m.ProjectID == ProjectID
                //////                      && m.StoreIssueNoteDate <= ToDate
                //////                      && (!ItemID.HasValue || d.ItemID == ItemID)
                //////                select new
                //////                {
                //////                    d.ItemID,
                //////                    TranDate = m.StoreIssueNoteDate,
                //////                    ReceivedQty = 0m,
                //////                    IssuedQty = d.IssuedQty ?? 0m,
                //////                    ReturnQty = 0m,
                //////                    TransferInQty = 0m,
                //////                    TransferOutQty = 0m
                //////                }
                //////            )

                //////            .Concat(
                //////                // SRN
                //////                from d in db.INStoreReturnNoteDetails
                //////                join m in db.INStoreReturnNotes on d.StoreReturnNoteID equals m.StoreReturnNoteID
                //////                where m.ProjectID == ProjectID
                //////                      && m.IsPosted == true
                //////                      && m.StoreReturnNoteDate <= ToDate
                //////                      && (!ItemID.HasValue || d.ItemID == ItemID)
                //////                select new
                //////                {
                //////                    d.ItemID,
                //////                    TranDate = m.StoreReturnNoteDate,
                //////                    ReceivedQty = 0m,
                //////                    IssuedQty = 0m,
                //////                    ReturnQty = d.ReturnQty ?? 0m,
                //////                    TransferInQty = 0m,
                //////                    TransferOutQty = 0m
                //////                }
                //////            )

                //////            .Concat(
                //////                // STN IN
                //////                from d in db.INStoreTransferNoteDetails
                //////                join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //////                where m.ToProjectID == ProjectID
                //////                      && m.ReceivedByID != null
                //////                      && m.StoreTransferNoteDate <= ToDate
                //////                      && (!ItemID.HasValue || d.ItemID == ItemID)
                //////                select new
                //////                {
                //////                    d.ItemID,
                //////                    TranDate = m.StoreTransferNoteDate,
                //////                    ReceivedQty = 0m,
                //////                    IssuedQty = 0m,
                //////                    ReturnQty = 0m,
                //////                    TransferInQty = d.TransferQty ?? 0m,
                //////                    TransferOutQty = 0m
                //////                }
                //////            )

                //////            .Concat(
                //////                // STN OUT
                //////                from d in db.INStoreTransferNoteDetails
                //////                join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //////                where m.FromProjectID == ProjectID
                //////                      && m.ReceivedByID != null
                //////                      && m.StoreTransferNoteDate <= ToDate
                //////                      && (!ItemID.HasValue || d.ItemID == ItemID)
                //////                select new
                //////                {
                //////                    d.ItemID,
                //////                    TranDate = m.StoreTransferNoteDate,
                //////                    ReceivedQty = 0m,
                //////                    IssuedQty = 0m,
                //////                    ReturnQty = 0m,
                //////                    TransferInQty = 0m,
                //////                    TransferOutQty = d.TransferQty ?? 0m
                //////                }
                //////            )

                //////        ) on i.ItemID equals mv.ItemID into mvj
                //////        from mv in mvj.DefaultIfEmpty()

                //////        where i.CompanyID == CompanyID
                //////              && (!ItemID.HasValue || i.ItemID == ItemID)

                //////        group new { i, size, uom, pi, mv } by new
                //////        {
                //////            i.ItemID,
                //////            i.Description,
                //////            GroupName = i.INGroup.Name,
                //////            CategoryName = i.INCategory.Name,
                //////            SizeName = size != null ? size.Name : "",
                //////            UOM = uom != null ? uom.Name : "",
                //////            Rate = pi != null ? pi.LastRate ?? 0m : 0m,
                //////            Opening = pi != null ? pi.OpeningQty ?? 0m : 0m
                //////        }
                //////        into g

                //////        let OpeningQty =
                //////            (g.Where(x => x.mv != null && x.mv.TranDate < FromDate)
                //////             .Sum(x => (decimal?)(
                //////                 x.mv.ReceivedQty + x.mv.ReturnQty + x.mv.TransferInQty
                //////                 - x.mv.IssuedQty - x.mv.TransferOutQty)) ?? 0m)
                //////            + g.Key.Opening

                //////        let ReceivedQty =
                //////            g.Where(x => x.mv != null && x.mv.TranDate >= FromDate && x.mv.TranDate <= ToDate)
                //////             .Sum(x => (decimal?)x.mv.ReceivedQty) ?? 0m

                //////        let IssuedQty =
                //////            g.Where(x => x.mv != null && x.mv.TranDate >= FromDate && x.mv.TranDate <= ToDate)
                //////             .Sum(x => (decimal?)x.mv.IssuedQty) ?? 0m

                //////        let ReturnQty =
                //////            g.Where(x => x.mv != null && x.mv.TranDate >= FromDate && x.mv.TranDate <= ToDate)
                //////             .Sum(x => (decimal?)x.mv.ReturnQty) ?? 0m

                //////        let TransferQty =
                //////            g.Where(x => x.mv != null && x.mv.TranDate >= FromDate && x.mv.TranDate <= ToDate)
                //////             .Sum(x => (decimal?)(
                //////                 x.mv.TransferInQty - x.mv.TransferOutQty)) ?? 0m

                //////        let ClosingQty = OpeningQty + ReceivedQty + ReturnQty + TransferQty - IssuedQty

                //////        select new spRptINItemStockModel
                //////        {
                //////            ProjectID = ProjectID,
                //////            ItemID = g.Key.ItemID,
                //////            Description = g.Key.Description,
                //////            SizeName = g.Key.SizeName,
                //////            UOM = g.Key.UOM,
                //////            Rate = g.Key.Rate,
                //////            FromDate = FromDate.Value,
                //////            ToDate = ToDate.Value,
                //////            GroupName = g.Key.GroupName,
                //////            CategoryName = g.Key.CategoryName,
                //////            OpeningQty = OpeningQty,
                //////            ReceivedQty = ReceivedQty,
                //////            IssuedQty = IssuedQty,
                //////            ReturnQty = ReturnQty,
                //////            TransferQty = TransferQty,
                //////            ClosingQty = ClosingQty,
                //////            ClosingAmount = ClosingQty * g.Key.Rate
                //////        }
                //////    )
                //////.Where(x =>
                //////    //x.OpeningQty > 0 ||
                //////    //x.ReceivedQty > 0 ||
                //////    //x.IssuedQty > 0 ||
                //////    //x.ReturnQty > 0 |
                //////    //x.TransferQty > 0 ||
                //////    //x.ClosingQty > 0 ||
                //////    //x.Rate > 0)
                //////    //Math.Abs((double)x.OpeningQty) > 0.0001 &&
                //////    //Math.Abs((double)x.ReceivedQty) > 0.0001 &&
                //////    //Math.Abs((double)x.IssuedQty) > 0.0001 &&
                //////    //Math.Abs((double)x.ReturnQty) > 0.0001 &&
                //////    //Math.Abs((double)x.TransferQty) > 0.0001 &&
                //////    //Math.Abs((double)x.ClosingQty) > 0.0001 &&
                //////    //Math.Abs((double)x.Rate) > 0.0001)
                //////    !(
                //////    x.OpeningQty == 0 &&
                //////    x.ReceivedQty == 0 &&
                //////    x.IssuedQty == 0 &&
                //////    x.ReturnQty == 0 &&
                //////    x.TransferQty == 0 &&
                //////    x.ClosingQty == 0))// &&
                //////    //x.Rate == 0))
                //////    //.Where(x =>
                //////    //    x.OpeningQty != 0 ||
                //////    //    x.ReceivedQty != 0 ||
                //////    //    x.IssuedQty != 0 ||
                //////    //    x.ReturnQty != 0 ||
                //////    //    x.TransferQty != 0 ||
                //////    //    x.ClosingQty != 0 ||
                //////    //    x.Rate != 0)
                //////    .ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


                using (var excelPackage = new ExcelPackage())
                {
                    if (result.Count > 0)
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("INItemStockReportExcel");
                        var rowNo = 1;

                        //workSheet.Cells[rowNo, 1].Value = INItemStockData.Max(x => x.CompanyName);
                        //workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        //workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        //workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                        //workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;

                        //rowNo++;
                        //rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "Item Stock Report";
                        workSheet.Cells[rowNo, 1, rowNo, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 13].Merge = true;
                        workSheet.Cells[rowNo, 1, rowNo, 13].Style.Font.Bold = true;



                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Project";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 2].Value = db.INProjects.Where(x => x.ProjectID == ProjectID).FirstOrDefault().ProjectName;// INItemStockData.Max(x => x.ProjectName).ToString();

                        workSheet.Cells[rowNo, 11].Value = "From Date";
                        workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 12].Value = Convert.ToDateTime(result.Max(x => x.FromDate)).ToString("dd-MMM-yyyy");

                        rowNo++;
                        workSheet.Cells[rowNo, 11].Value = "To Date";
                        workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 12].Value = Convert.ToDateTime(result.Max(x => x.ToDate)) .ToString("dd-MMM-yyyy");


                        rowNo++;
                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Sr No";
                        workSheet.Cells[rowNo, 2].Value = "Group";
                        workSheet.Cells[rowNo, 3].Value = "Category";
                        workSheet.Cells[rowNo, 4].Value = "Item ID";
                        workSheet.Cells[rowNo, 5].Value = "Item Description";
                        workSheet.Cells[rowNo, 6].Value = "Size";
                        workSheet.Cells[rowNo, 7].Value = "UOM";
                        workSheet.Cells[rowNo, 8].Value = "Opening";
                        workSheet.Cells[rowNo, 9].Value = "GRN";
                        workSheet.Cells[rowNo, 10].Value = "STN";
                        workSheet.Cells[rowNo, 11].Value = "SIN";
                        workSheet.Cells[rowNo, 12].Value = "SRN";
                        workSheet.Cells[rowNo, 13].Value = "Closing";
                        workSheet.Cells[rowNo, 14].Value = "Rate";
                        workSheet.Cells[rowNo, 15].Value = "Total Amount";

                        workSheet.Cells[rowNo, 1, rowNo, 15].Style.Font.Bold = true;

                        Int64 SrNo = 0;
                        foreach (var row in result)
                        {
                            rowNo++;
                            //balance += (row.DebitAmount ?? 0) - (row.CreditAmount ?? 0);
                            SrNo++;
                            workSheet.Cells[rowNo, 1].Value = SrNo;
                            // worksheet.Cells[rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[rowNo, 2].Value = row.GroupName;
                            workSheet.Cells[rowNo, 3].Value = row.CategoryName;
                            workSheet.Cells[rowNo, 4].Value = row.ItemID;
                            workSheet.Cells[rowNo, 5].Value = row.Description;
                            workSheet.Cells[rowNo, 6].Value = row.SizeName;
                            workSheet.Cells[rowNo, 7].Value = row.UOM;
                            workSheet.Cells[rowNo, 8].Value = row.OpeningQty;
                            workSheet.Cells[rowNo, 9].Value = row.ReceivedQty;
                            workSheet.Cells[rowNo, 10].Value = row.TransferQty;
                            workSheet.Cells[rowNo, 11].Value = row.IssuedQty;
                            workSheet.Cells[rowNo, 12].Value = row.ReturnQty;
                            workSheet.Cells[rowNo, 13].Value = row.ClosingQty;
                            workSheet.Cells[rowNo, 14].Value = row.Rate;
                            workSheet.Cells[rowNo, 15].Value = row.ClosingAmount;


                        }
                        workSheet.Cells[5, 8, rowNo, 12].Style.Numberformat.Format = "#,##0";
                        //worksheet.Cells[8, 1, rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(2).Width = 20;
                        workSheet.Column(3).Width = 40;
                        workSheet.Column(5).Width = 50;
                    }
                    else
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("INItemStockReportExcel");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = "Record not found";
                        workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;
                    }
                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "StockReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }

        public ActionResult DownloadExcel(int CompanyID, int ProjectID, DateTime? FromDate, DateTime? ToDate)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Report");

                var db = new GLEntities();

                var INItemStockData =
                (
                    from i in db.INItems
                    where i.CompanyID == CompanyID

                    join pi in db.INProjectItems
                        .Where(x => x.CompanyID == CompanyID && x.ProjectID == ProjectID)
                        on i.ItemID equals pi.ItemID into pij
                    from pi in pij.DefaultIfEmpty()

                    join mv in
                    (
                        // GRN
                        (
                            from d in db.INGoodsReceiptNoteDetails
                            join m in db.INGoodsReceiptNotes
                                on d.GoodsReceiptNoteID equals m.GoodsReceiptNoteID
                            where m.ProjectID == ProjectID
                                  && m.GoodsReceiptNotesDate <= ToDate
                            select new
                            {
                                d.ItemID,
                                TranDate = m.GoodsReceiptNotesDate,
                                ReceivedQty = d.ReceivedQty ?? 0m,
                                IssuedQty = 0m,
                                ReturnQty = 0m,
                                TransferInQty = 0m,
                                TransferOutQty = 0m,
                                NetQty = d.ReceivedQty ?? 0m
                            }
                        )

                        .Union(

                        // SIN
                        from d in db.INStoreIssueNoteDetails
                        join m in db.INStoreIssueNotes
                            on d.StoreIssueNoteID equals m.StoreIssueNoteID
                        where m.ProjectID == ProjectID
                              && m.StoreIssueNoteDate <= ToDate
                        select new
                        {
                            d.ItemID,
                            TranDate = m.StoreIssueNoteDate,
                            ReceivedQty = 0m,
                            IssuedQty = d.IssuedQty ?? 0m,
                            ReturnQty = 0m,
                            TransferInQty = 0m,
                            TransferOutQty = 0m,
                            NetQty = -(d.IssuedQty ?? 0m)
                        })

                        .Union(

                        // SRN
                        from d in db.INStoreReturnNoteDetails
                        join m in db.INStoreReturnNotes
                            on d.StoreReturnNoteID equals m.StoreReturnNoteID
                        where m.ProjectID == ProjectID
                              && m.IsPosted == true
                              && m.StoreReturnNoteDate <= ToDate
                        select new
                        {
                            d.ItemID,
                            TranDate = m.StoreReturnNoteDate,
                            ReceivedQty = 0m,
                            IssuedQty = 0m,
                            ReturnQty = d.ReturnQty ?? 0m,
                            TransferInQty = 0m,
                            TransferOutQty = 0m,
                            NetQty = d.ReturnQty ?? 0m
                        })

                        .Union(

                        // STN IN
                        from d in db.INStoreTransferNoteDetails
                        join m in db.INStoreTransferNotes
                            on d.StoreTransferNoteID equals m.StoreTransferNoteID
                        where m.ToProjectID == ProjectID
                              && m.ReceivedByID != null
                              && m.StoreTransferNoteDate <= ToDate
                        select new
                        {
                            d.ItemID,
                            TranDate = m.StoreTransferNoteDate,
                            ReceivedQty = 0m,
                            IssuedQty = 0m,
                            ReturnQty = 0m,
                            TransferInQty = d.TransferQty ?? 0m,
                            TransferOutQty = 0m,
                            NetQty = d.TransferQty ?? 0m
                        })

                        .Union(

                        // STN OUT
                        from d in db.INStoreTransferNoteDetails
                        join m in db.INStoreTransferNotes
                            on d.StoreTransferNoteID equals m.StoreTransferNoteID
                        where m.FromProjectID == ProjectID
                              && m.ReceivedByID != null
                              && m.StoreTransferNoteDate <= ToDate
                        select new
                        {
                            d.ItemID,
                            TranDate = m.StoreTransferNoteDate,
                            ReceivedQty = 0m,
                            IssuedQty = 0m,
                            ReturnQty = 0m,
                            TransferInQty = 0m,
                            TransferOutQty = d.TransferQty ?? 0m,
                            NetQty = -(d.TransferQty ?? 0m)
                        })
                    )
                    on i.ItemID equals mv.ItemID into mvj
                    from mv in mvj.DefaultIfEmpty()

                    group new { i, pi, mv } by new
                    {
                        i.ItemID,
                        i.Description,
                        GroupName = i.INGroup.Name,
                        CategoryName = i.INCategory.Name,
                        SizeName = i.SizeID != null
                            ? db.INSizes.FirstOrDefault(x => x.SizeID == i.SizeID).Name
                            : "",
                        UOM = db.INUnitOfMeasurements
                            .FirstOrDefault(x => x.UOMID == i.UOMID).Name,
                        Rate = pi != null ? pi.LastRate : 0m,
                        ProjectOpeningQty = pi != null ? pi.OpeningQty : 0m
                    }
                    into g

                    let OpeningQty =
                        (g.Where(x => x.mv != null && x.mv.TranDate < FromDate)
                          .Sum(x => (decimal?)x.mv.NetQty) ?? 0m)
                        + (g.Key.ProjectOpeningQty ?? 0m)

                    let ReceivedQty =
                        g.Where(x => x.mv != null
                                  && x.mv.TranDate >= FromDate
                                  && x.mv.TranDate <= ToDate)
                         .Sum(x => (decimal?)x.mv.ReceivedQty) ?? 0m

                    let IssuedQty =
                        g.Where(x => x.mv != null
                                  && x.mv.TranDate >= FromDate
                                  && x.mv.TranDate <= ToDate)
                         .Sum(x => (decimal?)x.mv.IssuedQty) ?? 0m

                    let ReturnQty =
                        g.Where(x => x.mv != null
                                  && x.mv.TranDate >= FromDate
                                  && x.mv.TranDate <= ToDate)
                         .Sum(x => (decimal?)x.mv.ReturnQty) ?? 0m

                    let TransferQty =
                        (g.Where(x => x.mv != null
                                   && x.mv.TranDate >= FromDate
                                   && x.mv.TranDate <= ToDate)
                          .Sum(x => (decimal?)x.mv.TransferInQty) ?? 0m)
                        -
                        (g.Where(x => x.mv != null
                                   && x.mv.TranDate >= FromDate
                                   && x.mv.TranDate <= ToDate)
                          .Sum(x => (decimal?)x.mv.TransferOutQty) ?? 0m)

                    let ClosingQty =
                        OpeningQty + ReceivedQty + ReturnQty + TransferQty - IssuedQty

                    select new spRptINItemStockModel
                    {
                        ProjectID = ProjectID,
                        ItemID = g.Key.ItemID,
                        Description = g.Key.Description,
                        GroupName = g.Key.GroupName,
                        CategoryName = g.Key.CategoryName,
                        SizeName = g.Key.SizeName,
                        UOM = g.Key.UOM,
                        FromDate = FromDate.Value,
                        ToDate = ToDate.Value,
                        Rate = g.Key.Rate ?? 0m,
                        OpeningQty = OpeningQty,
                        ReceivedQty = ReceivedQty,
                        IssuedQty = IssuedQty,
                        TransferQty = TransferQty,
                        ReturnQty = ReturnQty,
                        ClosingQty = ClosingQty,
                        ClosingAmount = ClosingQty * (g.Key.Rate ?? 0m)
                    }
                )
                .Where(x =>
                    x.OpeningQty != 0 ||
                    x.ReceivedQty != 0 ||
                    x.IssuedQty != 0 ||
                    x.TransferQty != 0 ||
                    x.ReturnQty != 0 ||
                    x.ClosingQty != 0 ||
                    x.Rate != 0)
                .ToList();

                //// ======================
                //// 1. OPENING BALANCES
                //// ======================

                //var grnData =
                //    from d in db.INGoodsReceiptNoteDetails
                //    join m in db.INGoodsReceiptNotes on d.GoodsReceiptNoteID equals m.GoodsReceiptNoteID
                //    where m.GoodsReceiptNotesDate < FromDate && m.ProjectID == ProjectID
                //    group d by new { d.ItemID, m.ProjectID } into g
                //    select new
                //    {
                //        g.Key.ItemID,
                //        g.Key.ProjectID,
                //        Qty = g.Sum(x => x.ReceivedQty ?? 0)
                //    };

                //var sinData =
                //    from d in db.INStoreIssueNoteDetails
                //    join m in db.INStoreIssueNotes on d.StoreIssueNoteID equals m.StoreIssueNoteID
                //    where m.StoreIssueNoteDate < FromDate && m.ProjectID == ProjectID
                //    group d by new { d.ItemID, m.ProjectID } into g
                //    select new
                //    {
                //        g.Key.ItemID,
                //        g.Key.ProjectID,
                //        Qty = g.Sum(x => x.IssuedQty ?? 0)
                //    };

                //var srnData =
                //    from d in db.INStoreReturnNoteDetails
                //    join m in db.INStoreReturnNotes on d.StoreReturnNoteID equals m.StoreReturnNoteID
                //    where m.StoreReturnNoteDate < FromDate && m.ProjectID == ProjectID && m.IsPosted == true
                //    group d by new { d.ItemID, m.ProjectID } into g
                //    select new
                //    {
                //        g.Key.ItemID,
                //        g.Key.ProjectID,
                //        Qty = g.Sum(x => x.ReturnQty ?? 0)
                //    };

                //var stnInData =
                //    (from d in db.INStoreTransferNoteDetails
                //    join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //    where m.StoreTransferNoteDate < FromDate
                //          && m.ToProjectID == ProjectID
                //          && m.ReceivedByID != null && m.ReceivedByID > 0
                //    group d by new { d.ItemID, m.ToProjectID } into g
                //    select new
                //    {
                //        g.Key.ItemID,
                //        ProjectID = g.Key.ToProjectID,
                //        Qty = g.Sum(x => x.TransferQty ?? 0)
                //    });

                //var stnOutData =
                //    (from d in db.INStoreTransferNoteDetails
                //    join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //    where m.StoreTransferNoteDate < FromDate
                //          && m.FromProjectID == ProjectID
                //          && m.ReceivedByID != null && m.ReceivedByID > 0
                //    group d by new { d.ItemID, m.FromProjectID } into g
                //    select new
                //    {
                //        g.Key.ItemID,
                //        ProjectID = g.Key.FromProjectID,
                //        Qty = g.Sum(x => x.TransferQty ?? 0)
                //    });

                //var openingQty =
                //(
                //    from g in grnData
                //    join s in sinData on new { g.ItemID, g.ProjectID } equals new { s.ItemID, s.ProjectID } into sj
                //    from s in sj.DefaultIfEmpty()
                //    join r in srnData on new { g.ItemID, g.ProjectID } equals new { r.ItemID, r.ProjectID } into rj
                //    from r in rj.DefaultIfEmpty()
                //    join tin in stnInData on new { g.ItemID, g.ProjectID } equals new { tin.ItemID, tin.ProjectID } into tij
                //    from tin in tij.DefaultIfEmpty()
                //    join tout in stnOutData on new { g.ItemID, g.ProjectID } equals new { tout.ItemID, tout.ProjectID } into toj
                //    from tout in toj.DefaultIfEmpty()
                //    select new
                //    {
                //        g.ItemID,
                //        g.ProjectID,
                //        OpeningQty =
                //            (g.Qty + (r == null ? 0 : r.Qty) + (tin == null ? 0 : tin.Qty))
                //          - ((s == null ? 0 : s.Qty) + (tout == null ? 0 : tout.Qty))
                //    }
                //    //    OpeningQty =
                //    //        (g.Qty + (r?.Qty ?? 0) + (tin?.Qty ?? 0))
                //    //      - ((s?.Qty ?? 0) + (tout?.Qty ?? 0))
                //    //}
                //)
                //.Union
                //(
                //    from s in sinData
                //    join g in grnData on new { s.ItemID, s.ProjectID } equals new { g.ItemID, g.ProjectID } into gj
                //    from g in gj.DefaultIfEmpty()
                //    join r in srnData on new { s.ItemID, s.ProjectID } equals new { r.ItemID, r.ProjectID } into rj
                //    from r in rj.DefaultIfEmpty()
                //    join tin in stnInData on new { s.ItemID, s.ProjectID } equals new { tin.ItemID, tin.ProjectID } into tij
                //    from tin in tij.DefaultIfEmpty()
                //    join tout in stnOutData on new { s.ItemID, s.ProjectID } equals new { tout.ItemID, tout.ProjectID } into toj
                //    from tout in toj.DefaultIfEmpty()
                //    select new
                //    {
                //        s.ItemID,
                //        s.ProjectID,
                //        OpeningQty =
                //            ((g == null ? 0 : g.Qty) + (r == null ? 0 : r.Qty) + (tin == null ? 0 : tin.Qty))
                //          - (s.Qty + (tout == null ? 0 : tout.Qty))
                //    }
                //);


                //// ======================
                //// 2. PERIOD TRANSACTIONS
                //// ======================

                //var resultGRN =
                //    (from d in db.INGoodsReceiptNoteDetails
                //     join m in db.INGoodsReceiptNotes on d.GoodsReceiptNoteID equals m.GoodsReceiptNoteID
                //     where m.GoodsReceiptNotesDate >= FromDate && m.GoodsReceiptNotesDate <= ToDate && m.ProjectID == ProjectID
                //     group d by new { m.ProjectID, d.ItemID } into g
                //     select new
                //     {
                //         g.Key.ProjectID,
                //         g.Key.ItemID,
                //         TotalReceivedQty = g.Sum(x => x.ReceivedQty ?? 0)
                //     });

                //var resultSIN =
                //    (from d in db.INStoreIssueNoteDetails
                //     join m in db.INStoreIssueNotes on d.StoreIssueNoteID equals m.StoreIssueNoteID
                //     where m.StoreIssueNoteDate >= FromDate && m.StoreIssueNoteDate <= ToDate && m.ProjectID == ProjectID
                //     group d by new { m.ProjectID, d.ItemID } into g
                //     select new
                //     {
                //         g.Key.ProjectID,
                //         g.Key.ItemID,
                //         TotalIssuedQty = g.Sum(x => x.IssuedQty ?? 0)
                //     });

                //var resultSRN =
                //    (from d in db.INStoreReturnNoteDetails
                //     join m in db.INStoreReturnNotes on d.StoreReturnNoteID equals m.StoreReturnNoteID
                //     where m.StoreReturnNoteDate >= FromDate && m.StoreReturnNoteDate <= ToDate && m.ProjectID == ProjectID && m.IsPosted == true
                //     group d by new { m.ProjectID, d.ItemID } into g
                //     select new
                //     {
                //         g.Key.ProjectID,
                //         g.Key.ItemID,
                //         TotalReturnQty = g.Sum(x => x.ReturnQty ?? 0)
                //     });

                //var resultSTNIn =
                //    (from d in db.INStoreTransferNoteDetails
                //    join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //    where m.StoreTransferNoteDate >= FromDate && m.StoreTransferNoteDate <= ToDate
                //          && m.ToProjectID == ProjectID && m.ReceivedByID != null
                //    group d by new { d.ItemID, m.ToProjectID } into g
                //    select new
                //    {
                //        ProjectID = g.Key.ToProjectID,
                //        ItemID = g.Key.ItemID,
                //        TotalReceivedQty = g.Sum(x => x.TransferQty ?? 0)
                //    }).ToList();

                //var resultSTNOut =
                //    (from d in db.INStoreTransferNoteDetails
                //    join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //    where m.StoreTransferNoteDate >= FromDate && m.StoreTransferNoteDate <= ToDate
                //          && m.FromProjectID == ProjectID && m.ReceivedByID != null
                //    group d by new { d.ItemID, m.FromProjectID } into g
                //    select new
                //    {
                //        ProjectID = g.Key.FromProjectID,
                //        ItemID = g.Key.ItemID,
                //        TotalIssuedQty = g.Sum(x => x.TransferQty ?? 0)
                //    }).ToList();

                //// ======================
                //// 3. FINAL STOCK REPORT
                //// ======================

                //var items = db.INItems.Where(x => x.CompanyID == CompanyID).ToList();
                //var projectItems = db.INProjectItems.Where(x => x.CompanyID == CompanyID && x.ProjectID == ProjectID).ToList();

                //var spRptINItemStockModelList =
                //(
                //    from i in items
                //    join pi in projectItems on i.ItemID equals pi.ItemID into pij
                //    from pi in pij.DefaultIfEmpty()
                //    join grn in resultGRN on i.ItemID equals grn.ItemID into gj
                //    from grn in gj.DefaultIfEmpty()
                //    join sin in resultSIN on i.ItemID equals sin.ItemID into sj
                //    from sin in sj.DefaultIfEmpty()
                //    join srn in resultSRN on i.ItemID equals srn.ItemID into srj
                //    from srn in srj.DefaultIfEmpty()
                //    join stnIn in resultSTNIn on i.ItemID equals stnIn.ItemID into tij
                //    from stnIn in tij.DefaultIfEmpty()
                //    join stnOut in resultSTNOut on i.ItemID equals stnOut.ItemID into toj
                //    from stnOut in toj.DefaultIfEmpty()
                //    select new spRptINItemStockModel
                //    {
                //        ProjectID = ProjectID,
                //        ItemID = i.ItemID,
                //        Description = i.Description,
                //        GroupName = i.INGroup.Name,
                //        CategoryName = i.INCategory.Name,
                //        SizeName = i.SizeID != null ? db.INSizes.FirstOrDefault(x => x.SizeID == i.SizeID).Name : "",
                //        UOM = db.INUnitOfMeasurements.FirstOrDefault(x => x.UOMID == i.UOMID).Name,
                //        ProjectName = db.INProjects.FirstOrDefault(x => x.ProjectID == ProjectID).ProjectName,
                //        CompanyName = db.Companies.FirstOrDefault(x => x.CompanyID == i.CompanyID).Name,
                //        FromDate = FromDate.GetValueOrDefault(),
                //        ToDate = ToDate.GetValueOrDefault(),
                //        Rate = pi?.LastRate ?? 0,
                //        OpeningQty = (openingQty.FirstOrDefault(o => o.ItemID == i.ItemID && o.ProjectID == ProjectID)?.OpeningQty ?? 0)
                //                     + (pi?.OpeningQty ?? 0),
                //        ReceivedQty = (grn?.TotalReceivedQty ?? 0),// + (srn?.TotalReturnQty ?? 0) + (stnIn?.TotalReceivedQty ?? 0),
                //        IssuedQty = (sin?.TotalIssuedQty ?? 0), // + (stnOut?.TotalIssuedQty ?? 0),
                //        TransferQty = (stnIn?.TotalReceivedQty ?? 0) - (stnOut?.TotalIssuedQty ?? 0),
                //        ReturnQty = (srn?.TotalReturnQty ?? 0), // - (srn?.TotalReturnQty ?? 0),
                //        ClosingQty =
                //            (openingQty.FirstOrDefault(o => o.ItemID == i.ItemID && o.ProjectID == ProjectID)?.OpeningQty ?? 0)
                //            + (pi?.OpeningQty ?? 0)
                //            + (grn?.TotalReceivedQty ?? 0)
                //            + (srn?.TotalReturnQty ?? 0)
                //            + (stnIn?.TotalReceivedQty ?? 0)
                //            - (sin?.TotalIssuedQty ?? 0)
                //            - (stnOut?.TotalIssuedQty ?? 0),
                //        ClosingAmount =
                //            (
                //                (openingQty.FirstOrDefault(o => o.ItemID == i.ItemID && o.ProjectID == ProjectID)?.OpeningQty ?? 0)
                //                + (pi?.OpeningQty ?? 0)
                //                + (grn?.TotalReceivedQty ?? 0)
                //                + (srn?.TotalReturnQty ?? 0)
                //                + (stnIn?.TotalReceivedQty ?? 0)
                //                - (sin?.TotalIssuedQty ?? 0)
                //                - (stnOut?.TotalIssuedQty ?? 0)
                //            ) * (pi?.LastRate ?? 0)
                //    }
                //);

                //var INItemStockData = spRptINItemStockModelList //.ToList();
                //    .Where(x => x.TransferQty != 0 || x.OpeningQty != 0 || x.ReceivedQty != 0 || x.TransferQty != 0 || x.IssuedQty != 0 || x.ReturnQty != 0 || x.ClosingQty != 0 || x.Rate != 0)
                //    .ToList();

                #region 
                //////// ======================
                //////// 1. Opening Balances
                //////// ======================
                //////var grnData =
                //////    from d in db.INGoodsReceiptNoteDetails
                //////    join m in db.INGoodsReceiptNotes on d.GoodsReceiptNoteID equals m.GoodsReceiptNoteID
                //////    where m.GoodsReceiptNotesDate < FromDate && m.ProjectID == ProjectID
                //////    group d by new { d.ItemID, m.ProjectID } into g
                //////    select new
                //////    {
                //////        g.Key.ItemID,
                //////        g.Key.ProjectID,
                //////        TotalReceivedQty = g.Sum(x => x.ReceivedQty ?? 0)
                //////    };

                //////var sinData =
                //////    from d in db.INStoreIssueNoteDetails
                //////    join m in db.INStoreIssueNotes on d.StoreIssueNoteID equals m.StoreIssueNoteID
                //////    where m.StoreIssueNoteDate < FromDate && m.ProjectID == ProjectID
                //////    group d by new { d.ItemID, m.ProjectID } into g
                //////    select new
                //////    {
                //////        g.Key.ItemID,
                //////        g.Key.ProjectID,
                //////        TotalIssuedQty = g.Sum(x => x.IssuedQty ?? 0)
                //////    };

                //////var srnData =
                //////    from d in db.INStoreReturnNoteDetails
                //////    join m in db.INStoreReturnNotes on d.StoreReturnNoteID equals m.StoreReturnNoteID
                //////    where m.StoreReturnNoteDate < FromDate && m.ProjectID == ProjectID
                //////    group d by new { d.ItemID, m.ProjectID } into g
                //////    select new
                //////    {
                //////        g.Key.ItemID,
                //////        g.Key.ProjectID,
                //////        TotalReturnQty = g.Sum(x => x.ReturnQty ?? 0)
                //////    };

                //////var stnInData =
                //////    from d in db.INStoreTransferNoteDetails
                //////    join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //////    where m.StoreTransferNoteDate < FromDate
                //////          && m.ToProjectID == ProjectID
                //////          && m.ReceivedByID != null && m.ReceivedByID > 0   // ✅ Include only if transfer is received
                //////    group d by new { d.ItemID, m.ToProjectID } into g
                //////    select new
                //////    {
                //////        g.Key.ItemID,
                //////        ProjectID = g.Key.ToProjectID,
                //////        TotalReceivedQty = g.Sum(x => x.TransferQty ?? 0)
                //////    };

                //////// Store Transfer Out (before FromDate)
                //////var stnOutData =
                //////    from d in db.INStoreTransferNoteDetails
                //////    join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //////    where m.StoreTransferNoteDate < FromDate
                //////          && m.FromProjectID == ProjectID
                //////          && m.ReceivedByID != null && m.ReceivedByID > 0  // ✅ Include only if transfer is received
                //////    group d by new { d.ItemID, m.FromProjectID } into g
                //////    select new
                //////    {
                //////        g.Key.ItemID,
                //////        ProjectID = g.Key.FromProjectID,
                //////        TotalIssuedQty = g.Sum(x => x.TransferQty ?? 0)
                //////    };

                //////// Opening quantity = GRN + STN In – (SIN + STN Out)
                ////////Opening quantity = GRN + STN In – (SIN + STN Out)
                //////var openingQty =
                //////(from g in grnData

                ////// join s in sinData on new { g.ItemID, g.ProjectID } equals new { s.ItemID, s.ProjectID } into gj
                ////// from s in gj.DefaultIfEmpty()

                ////// join si in stnInData on new { g.ItemID, g.ProjectID } equals new { si.ItemID, si.ProjectID } into sij
                ////// from stnIn in sij.DefaultIfEmpty()

                ////// join so in stnOutData on new { g.ItemID, g.ProjectID } equals new { so.ItemID, so.ProjectID } into soj
                ////// from stnOut in soj.DefaultIfEmpty()
                ////// select new
                ////// {
                //////     g.ItemID,
                //////     g.ProjectID,
                //////     OpeningQty =
                //////         (g.TotalReceivedQty + (stnIn == null ? 0 : stnIn.TotalReceivedQty))
                //////       - ((s == null ? 0 : s.TotalIssuedQty) + (sr == null ? 0 : sr.TotalReturnQty) + (stnOut == null ? 0 : stnOut.TotalIssuedQty))
                ////// })
                //////.Union(
                ////// from s in sinData
                ////// join g in grnData on new { s.ItemID, s.ProjectID } equals new { g.ItemID, g.ProjectID } into sj
                ////// from g in sj.DefaultIfEmpty()
                ////// // by me

                ////// join si in stnInData on new { s.ItemID, s.ProjectID } equals new { si.ItemID, si.ProjectID } into sij
                ////// from stnIn in sij.DefaultIfEmpty()
                ////// join so in stnOutData on new { s.ItemID, s.ProjectID } equals new { so.ItemID, so.ProjectID } into soj
                ////// from stnOut in soj.DefaultIfEmpty()
                ////// select new
                ////// {
                //////     s.ItemID,
                //////     s.ProjectID,
                //////     OpeningQty =
                //////         ((g == null ? 0 : g.TotalReceivedQty) + (stnIn == null ? 0 : stnIn.TotalReceivedQty))
                //////       - (s.TotalIssuedQty + (stnOut == null ? 0 : stnOut.TotalIssuedQty))
                ////// })
                //////.ToList();

                //////// ======================
                //////// 2. Period Transactions
                //////// ======================
                //////var resultGRN = db.INGoodsReceiptNoteDetails
                //////    .Join(db.INGoodsReceiptNotes,
                //////        d => d.GoodsReceiptNoteID,
                //////        m => m.GoodsReceiptNoteID,
                //////        (d, m) => new { d, m })
                //////    .Where(x => x.m.GoodsReceiptNotesDate >= FromDate
                //////                && x.m.GoodsReceiptNotesDate <= ToDate
                //////                && x.m.ProjectID == ProjectID)
                //////    .GroupBy(x => new { x.m.ProjectID, x.d.ItemID })
                //////    .Select(g => new
                //////    {
                //////        ProjectID = g.Key.ProjectID,
                //////        ItemID = g.Key.ItemID,
                //////        TotalReceivedQty = g.Sum(x => x.d.ReceivedQty ?? 0)
                //////    })
                //////    .ToList();

                //////var resultSIN = db.INStoreIssueNoteDetails
                //////    .Join(db.INStoreIssueNotes,
                //////        d => d.StoreIssueNoteID,
                //////        m => m.StoreIssueNoteID,
                //////        (d, m) => new { d, m })
                //////    .Where(x => x.m.StoreIssueNoteDate >= FromDate
                //////                && x.m.StoreIssueNoteDate <= ToDate
                //////                && x.m.ProjectID == ProjectID)
                //////    .GroupBy(x => new { x.m.ProjectID, x.d.ItemID })
                //////    .Select(g => new
                //////    {
                //////        ProjectID = g.Key.ProjectID,
                //////        ItemID = g.Key.ItemID,
                //////        TotalIssuedQty = g.Sum(x => x.d.IssuedQty ?? 0)
                //////    })
                //////    .ToList();

                //////var resultSRN = db.INStoreReturnNoteDetails
                //////    .Join(db.INStoreReturnNotes,
                //////        d => d.StoreReturnNoteID,
                //////        m => m.StoreReturnNoteID,
                //////        (d, m) => new { d, m })
                //////    .Where(x => x.m.StoreReturnNoteDate >= FromDate
                //////                && x.m.StoreReturnNoteDate <= ToDate
                //////                && x.m.ProjectID == ProjectID)
                //////    .GroupBy(x => new { x.m.ProjectID, x.d.ItemID })
                //////    .Select(g => new
                //////    {
                //////        ProjectID = g.Key.ProjectID,
                //////        ItemID = g.Key.ItemID,
                //////        TotalReturnQty = g.Sum(x => x.d.ReturnQty ?? 0)
                //////    })
                //////    .ToList();


                //////// Store Transfer In (during period)
                //////var resultSTNIn =
                //////    from d in db.INStoreTransferNoteDetails
                //////    join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //////    where m.StoreTransferNoteDate >= FromDate
                //////          && m.StoreTransferNoteDate <= ToDate
                //////          && m.ToProjectID == ProjectID
                //////          && m.ReceivedByID != null   // ✅ Include only if transfer is received
                //////    group d by new { d.ItemID, m.ToProjectID } into g
                //////    select new
                //////    {
                //////        ProjectID = g.Key.ToProjectID,
                //////        ItemID = g.Key.ItemID,
                //////        TotalReceivedQty = g.Sum(x => x.TransferQty ?? 0)
                //////    };

                //////// Store Transfer Out (during period)
                //////var resultSTNOut =
                //////    from d in db.INStoreTransferNoteDetails
                //////    join m in db.INStoreTransferNotes on d.StoreTransferNoteID equals m.StoreTransferNoteID
                //////    where m.StoreTransferNoteDate >= FromDate
                //////          && m.StoreTransferNoteDate <= ToDate
                //////          && m.FromProjectID == ProjectID
                //////          && m.ReceivedByID != null   // ✅ Include only if transfer is received
                //////    group d by new { d.ItemID, m.FromProjectID } into g
                //////    select new
                //////    {
                //////        ProjectID = g.Key.FromProjectID,
                //////        ItemID = g.Key.ItemID,
                //////        TotalIssuedQty = g.Sum(x => x.TransferQty ?? 0)
                //////    };


                //////// ======================
                //////// 3. Final Stock Report
                //////// ======================

                //////var items = db.INItems.Where(x => x.CompanyID == CompanyID).ToList();

                //////// Join INProjectItem to get OpeningQty, QtyInHand, and LastRate
                //////var projectItems = db.INProjectItems
                //////    .Where(x => x.CompanyID == CompanyID && x.ProjectID == ProjectID)
                //////    .ToList();

                //////var spRptINItemStockModelList =
                //////    (from i in items
                //////     join pi in projectItems on i.ItemID equals pi.ItemID into pij
                //////     from pi in pij.DefaultIfEmpty()

                //////     join grn in resultGRN on i.ItemID equals grn.ItemID into gj
                //////     from grn in gj.DefaultIfEmpty()

                //////     join sin in resultSIN on i.ItemID equals sin.ItemID into sj
                //////     from sin in sj.DefaultIfEmpty()


                //////     join srn in resultSRN on i.ItemID equals srn.ItemID into sr
                //////     from srn in sr.DefaultIfEmpty()


                //////     join stnIn in resultSTNIn on i.ItemID equals stnIn.ItemID into stnIj
                //////     from stnIn in stnIj.DefaultIfEmpty()

                //////     join stnOut in resultSTNOut on i.ItemID equals stnOut.ItemID into stnOj
                //////     from stnOut in stnOj.DefaultIfEmpty()

                //////     select new spRptINItemStockModel
                //////     {
                //////         ProjectID = ProjectID,
                //////         ItemID = i.ItemID,
                //////         Description = i.Description,
                //////         GroupName = i.INGroup.Name,
                //////         CategoryName = i.INCategory.Name,
                //////         SizeName = (i.SizeID != null)
                //////             ? db.INSizes.Where(x => x.SizeID == i.SizeID).FirstOrDefault().Name
                //////             : string.Empty,
                //////         UOM = db.INUnitOfMeasurements.Where(x => x.UOMID == i.UOMID).FirstOrDefault().Name,
                //////         ProjectName = db.INProjects.Where(x => x.ProjectID == ProjectID).FirstOrDefault().ProjectName,
                //////         CompanyName = db.Companies.Where(x => x.CompanyID == i.CompanyID).FirstOrDefault().Name,
                //////         FromDate = FromDate.GetValueOrDefault(DateTime.MinValue),
                //////         ToDate = ToDate.GetValueOrDefault(DateTime.MinValue),

                //////         // ✅ Updated to use INProjectItem only
                //////         Rate = pi != null ? pi.LastRate.GetValueOrDefault(0) : 0,

                //////         // Combine external openingQty list + INProjectItem.OpeningQty
                //////         OpeningQty = (openingQty.FirstOrDefault(o => o.ItemID == i.ItemID && o.ProjectID == ProjectID)?.OpeningQty ?? 0)
                //////                        + (pi != null ? pi.OpeningQty.GetValueOrDefault(0) : 0),

                //////         ReceivedQty = (grn == null ? 0 : grn.TotalReceivedQty),
                //////         IssuedQty = (sin == null ? 0 : sin.TotalIssuedQty),
                //////         ReturnQty = (srn == null ? 0 : srn.TotalReturnQty),
                //////         // Net transfers (In - Out)
                //////         TransferQty = (stnIn == null ? 0 : stnIn.TotalReceivedQty)
                //////                     - (stnOut == null ? 0 : stnOut.TotalIssuedQty),

                //////         ClosingQty = ((grn == null ? 0 : grn.TotalReceivedQty)
                //////                     + (stnIn == null ? 0 : stnIn.TotalReceivedQty))
                //////                     - ((sin == null ? 0 : sin.TotalIssuedQty)
                //////                     + (stnOut == null ? 0 : stnOut.TotalIssuedQty)),

                //////         // ✅ Use new Rate for closing amount
                //////         ClosingAmount = (
                //////             ((grn == null ? 0 : grn.TotalReceivedQty)
                //////            + (stnIn == null ? 0 : stnIn.TotalReceivedQty))
                //////           - ((sin == null ? 0 : sin.TotalIssuedQty)
                //////           + (srn == null ? 0 : srn.TotalReturnQty)
                //////            + (stnOut == null ? 0 : stnOut.TotalIssuedQty))
                //////         ) * (pi != null ? pi.LastRate.GetValueOrDefault(0) : 0)
                //////     })
                //////     //.Where(x => x.OpeningQty != 0 || x.ReceivedQty != 0 || x.TransferQty != 0 || x.IssuedQty != 0 || x.ClosingQty != 0 || x.Rate != 0 || x.ClosingAmount != 0)
                //////     .ToList();



                //////// ====== Final projection ======
                //////var INItemStockData = (
                //////   from v in spRptINItemStockModelList
                //////   select new spRptINItemStockModel
                //////   {
                //////       CompanyName = v.CompanyName,
                //////       ProjectName = v.ProjectName,
                //////       GroupName = v.GroupName,
                //////       CategoryName = v.CategoryName,
                //////       ItemID = v.ItemID,
                //////       Description = v.Description,
                //////       SizeName = v.SizeName,
                //////       UOM = v.UOM,
                //////       FromDate = v.FromDate,
                //////       ToDate = v.ToDate,

                //////       OpeningQty = v.OpeningQty,
                //////       TransferQty = v.TransferQty,
                //////       ReceivedQty = v.ReceivedQty,
                //////       IssuedQty = v.IssuedQty,
                //////       ReturnQty = v.ReturnQty,
                //////       Rate = v.Rate,

                //////       // ✅ Correct closing quantity formula
                //////       ClosingQty = v.OpeningQty
                //////                    + v.ReceivedQty
                //////                    + v.TransferQty
                //////                    - v.IssuedQty
                //////                    + v.ReturnQty,

                //////       // ✅ Correct closing amount
                //////       ClosingAmount = (v.OpeningQty
                //////               + v.ReceivedQty
                //////               + v.TransferQty
                //////               + v.ReturnQty
                //////               - v.IssuedQty) * v.Rate

                //////       ////ClosingQty = v.OpeningQty + v.ClosingQty,
                //////       ////ClosingAmount = (v.OpeningQty + v.ClosingQty) * v.Rate
                //////   }).ToList();
                #endregion

                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


                using (var excelPackage = new ExcelPackage())
                {
                    if (INItemStockData.Count > 0)
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("INItemStockReportExcel");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = INItemStockData.Max(x => x.CompanyName);
                        workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;

                        rowNo++;
                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "Item Stock Report";
                        workSheet.Cells[rowNo, 1, rowNo, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 13].Merge = true;
                        workSheet.Cells[rowNo, 1, rowNo, 13].Style.Font.Bold = true;



                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Project";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 2].Value = "";// INItemStockData.Max(x => x.ProjectName).ToString();

                        workSheet.Cells[rowNo, 11].Value = "From Date";
                        workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 12].Value = INItemStockData.Max(x => x.FromDate).ToString("dd-MMM-yyyy");

                        rowNo++;
                        workSheet.Cells[rowNo, 11].Value = "To Date";
                        workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 12].Value = INItemStockData.Max(x => x.ToDate).ToString("dd-MMM-yyyy");


                        rowNo++;
                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Sr No";
                        workSheet.Cells[rowNo, 2].Value = "Group";
                        workSheet.Cells[rowNo, 3].Value = "Category";
                        workSheet.Cells[rowNo, 4].Value = "Item ID";
                        workSheet.Cells[rowNo, 5].Value = "Item Description";
                        workSheet.Cells[rowNo, 6].Value = "Size";
                        workSheet.Cells[rowNo, 7].Value = "UOM";
                        workSheet.Cells[rowNo, 8].Value = "Opening";
                        workSheet.Cells[rowNo, 9].Value = "GRN";
                        workSheet.Cells[rowNo, 10].Value = "STN";
                        workSheet.Cells[rowNo, 11].Value = "SIN";
                        workSheet.Cells[rowNo, 12].Value = "SRN";
                        workSheet.Cells[rowNo, 13].Value = "Closing";
                        workSheet.Cells[rowNo, 14].Value = "Rate";
                        workSheet.Cells[rowNo, 15].Value = "Total Amount";

                        workSheet.Cells[rowNo, 1, rowNo, 15].Style.Font.Bold = true;

                        Int64 SrNo = 0;
                        foreach (var row in INItemStockData)
                        {
                            rowNo++;
                            //balance += (row.DebitAmount ?? 0) - (row.CreditAmount ?? 0);
                            SrNo++;
                            workSheet.Cells[rowNo, 1].Value = SrNo;
                            // worksheet.Cells[rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[rowNo, 2].Value = row.GroupName;
                            workSheet.Cells[rowNo, 3].Value = row.CategoryName;
                            workSheet.Cells[rowNo, 4].Value = row.ItemID;
                            workSheet.Cells[rowNo, 5].Value = row.Description;
                            workSheet.Cells[rowNo, 6].Value = row.SizeName;
                            workSheet.Cells[rowNo, 7].Value = row.UOM;
                            workSheet.Cells[rowNo, 8].Value = row.OpeningQty;
                            workSheet.Cells[rowNo, 9].Value = row.ReceivedQty;
                            workSheet.Cells[rowNo, 10].Value = row.TransferQty;
                            workSheet.Cells[rowNo, 11].Value = row.IssuedQty;
                            workSheet.Cells[rowNo, 12].Value = row.ReturnQty;
                            workSheet.Cells[rowNo, 13].Value = row.ClosingQty;
                            workSheet.Cells[rowNo, 14].Value = row.Rate;
                            workSheet.Cells[rowNo, 15].Value = row.ClosingAmount;


                        }
                        workSheet.Cells[8, 8, rowNo, 12].Style.Numberformat.Format = "#,##0";
                        //worksheet.Cells[8, 1, rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(2).Width = 20;
                        workSheet.Column(3).Width = 40;
                        workSheet.Column(5).Width = 50;
                    }
                    else
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("INItemStockReportExcel");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = "Record not found";
                        workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;
                    }
                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "StockReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }


        public ActionResult ApplicationFormReport()
        {
            //var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            //DALDropdowns dal = new DALDropdowns();
            //ViewBag.GLAccounts = dal.GLAccountGetForDropdown(LoginUser.CompanyID);
            //ViewBag.CompanyID = LoginUser.CompanyID;
            return View();
        }

        public ActionResult DVUnitLedgerReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            ViewBag.Units = new List<DVUnit>();
            ViewBag.LedgerDate = DateTime.Now.ToString("dd-MMM-yyyy");
            return View();
        }

        public ActionResult APPartyLedgerReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            //ViewBag.APVendors = dal.GLAPVendorsListWithCode(LoginUser.CompanyID);
            var APVendors = dal.GLAPVendorsListWithCode(LoginUser.CompanyID);
            spAPVendorsListWithCode_Result all = new spAPVendorsListWithCode_Result { APVendorID = 0, DisplayText = "All" };
            APVendors.Insert(0, all);
            ViewBag.APVendors = APVendors;
            ViewBag.ToDate = DateTime.Now.ToString("dd-MMM-yyyy");
            return View();
        }

        public ActionResult DVUnitLedgerReportDownload(int ProjectID, int UnitID, DateTime LedgerDate)
        {
            var report = new rptDVUnitLedger();

            var spRptDVUnitLedgerList = new GLEntities().spRptDVUnitLedger(ProjectID, UnitID, LedgerDate).ToList();

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

            report.SetDataSource(DVReceiptReportData);
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

        [HttpGet]
        private ActionResult INPurchaseRequisitionReportDownload(int RequestID)
        {
            // CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            var report = new rptINPurchaseRequisition();
            //Nullable<int> RequestID = Convert.ToInt32(Request.QueryString["RequestID"]);

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
                    Cancelled = v.Cancelled

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

            //  reportQueue.Enqueue(report);

            //report.SetDataSource(PurchaseRequisitionData);
            //CrystalReportViewer1.ReportSource = report;
            //CrystalReportViewer1.RefreshReport();
        }

        public ActionResult APPartyLedgerReportDownload(int APVendorID, DateTime ToDate)
        {
            var report = new rptAPPartyLedger();
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            var RptAPPartyLedgerList = new GLEntities().spRptAPPartyLedger(LoginUser.CompanyID, ToDate, APVendorID).ToList(); // spRptAPPartyLedger(APVendorID, ToDate).ToList();

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

            report.SetDataSource(APPartyLedgerReporData);
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

        public ActionResult GeneralLedgerReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.GLAccounts = dal.GLAccountGetForDropdown(LoginUser.CompanyID);
            ViewBag.CompanyID = LoginUser.CompanyID;
            ViewBag.LoginUser = LoginUser;
            return View();
        }

        public ActionResult DVGeneralLedgerReportDownload(DateTime? StartDate, DateTime? EndDate, string GLAccountNoFrom, string GLAccountNoTo)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            var report = new rptGeneralLedger();
            Nullable<int> CompanyID = LoginUser.CompanyID;
            string GLAccountNoStart = GLAccountNoFrom == "" ? null : GLAccountNoFrom;
            string GLAccountNoEnd = GLAccountNoTo == "" ? null : GLAccountNoTo;

            var GeneralLedgerReportModel = new GLEntities().spRptGeneralLedger(CompanyID, StartDate, EndDate, GLAccountNoStart, GLAccountNoEnd).ToList();

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
                    GLNarration = v.GLNarration ?? "",
                    VoucherNumber = v.VoucherNumber ?? "",
                    Debit = v.Debit ?? 0,
                    Credit = v.Credit ?? 0,
                    Balance = v.Balance ?? 0,
                }).ToList();

            report.SetDataSource(GeneralLedgerReportData);

            ReportDocument reportDocument = report;// new ReportDocument();
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


        public ActionResult DVGeneralLedgerReportDownloadOB(DateTime? StartDate, DateTime? EndDate, string GLAccountNoFrom, string GLAccountNoTo)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            var report = new rptGeneralLedgerOB();
            Nullable<int> CompanyID = LoginUser.CompanyID;
            string GLAccountNoStart = GLAccountNoFrom == "" ? null : GLAccountNoFrom;
            string GLAccountNoEnd = GLAccountNoTo == "" ? null : GLAccountNoTo;

            //if (StartDate == null)
            //{
            //    StartDate = new DateTime(1900, 01, 01);
            //}
            //if (EndDate == null)
            //{
            //    EndDate = DateTime.Now;
            //}

            var GeneralLedgerReportModel = new GLEntities().spRptGeneralLedgerOB(CompanyID, StartDate, EndDate, GLAccountNoStart, GLAccountNoEnd).ToList();

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

            report.SetDataSource(ReportData);

            ReportDocument reportDocument = report;// new ReportDocument();
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


        public ActionResult DVReceiptByIDReportDownload(int DVReceiptID)
        {
            var report = new RptDVReceipt();

            var spRptDVReceipt = new GLEntities().spRptDVReceipt(DVReceiptID);

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

            report.SetDataSource(rptPaymentPlanData);
            ReportDocument reportDocument = report;// new ReportDocument();
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

        [HttpGet]
        public ActionResult StoreTransferNoteReportDownload(int StoreTransferNoteID)
        {


            var report = new rptStoreTransferNote();
            //Nullable<int> INStoreTransferNoteID = Convert.ToInt32(Request.QueryString["INStoreTransferNoteID"]);

            var INStoreTransferNote = new GLEntities().spRptINStoreTransferNote(StoreTransferNoteID);

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
                    TransferQty = v.TransferQty.GetValueOrDefault(0),
                    Remarks = v.Remarks,

                    CreatedBy = v.CreatedBy,
                    CreatedAt = v.CreatedAt.GetValueOrDefault(DateTime.Now),
                    ApprovedBy = v.ApprovedBy,
                    ApprovedByAt = v.ApprovedByAt.GetValueOrDefault(DateTime.Now),
                    ReceivedBy = v.ReceivedBy,
                    ReceivedByAt = v.ReceivedByAt.GetValueOrDefault(DateTime.Now)

                }).ToList();

            report.SetDataSource(INStoreTransferNoteData);
            ReportDocument reportDocument = report;// new ReportDocument();
            // Export to a memory stream
            Stream pdfStream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

            // Read the Stream into a MemoryStream
            MemoryStream memoryStream = new MemoryStream();
            pdfStream.CopyTo(memoryStream);

            // Close the original Stream
            pdfStream.Close();

            // Set the response for the browser to download the file
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=StoreTransferNote.pdf");
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


        public ActionResult DVMemberPaymentPlanStatusReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.DVProjectsList(LoginUser.CompanyID);
            ViewBag.Units = new List<DVUnit>();
            ViewBag.LedgerDate = DateTime.Now.ToString("dd-MMM-yyyy");
            return View();
        }

        public ActionResult DVMemberPaymentPlanStatusReport_AllUnits()
        {

            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.DVProjectsList(LoginUser.CompanyID);
            ViewBag.Units = new List<DVUnit>();
            return View();
        }


        public ActionResult DVMemberPaymentPlanStatusReportAllUnitsDownload(int ProjectID, DateTime StatusDate)
        {
            //CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            var report = new rptMemberPaymentPlanStatus_AllUnits();


            var spRptMemberPaymentPlanStatus_AllUnits = new GLEntities().spRptMemberPaymentPlanStatus_AllUnits(ProjectID, StatusDate).ToList();

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



            report.SetDataSource(ReporData);
            //report.FileName = "MemberPaymentPlanStatus";
            ReportDocument reportDocument = report;// new ReportDocument();
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



        public ActionResult DVMemberPaymentPlanStatusReportDownload(int ProjectID, int UnitID, DateTime StatusDate)
        {
            //    CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            var report = new rptMemberPaymentPlanStatus();
            var db = new GLEntities();


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
                    spRptMemberPaymentPlan.Balance = spRptMemberPaymentPlan.DueAmount.GetValueOrDefault(0) - downPaymentAmountTotal;
                    spRptMemberPaymentPlan.ReceivedAmount = downPaymentAmountTotal;
                }
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

            report.SetDataSource(ReporData);
            //report.FileName = "MemberPaymentPlanStatus";
            ReportDocument reportDocument = report;// new ReportDocument();
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

        public ActionResult DVMemberPaymentPlanStatusReportAllUnits(int ProjectID, DateTime StatusDate)
        {
            var report = new rptMemberPaymentPlanStatus_AllUnits();

            var spRptMemberPaymentPlanStatus_AllUnits = new GLEntities().spRptMemberPaymentPlanStatus_AllUnits(ProjectID, StatusDate).ToList();

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


            report.SetDataSource(ReporData);
            //report.FileName = "MemberPaymentPlanStatus";
            ReportDocument reportDocument = report;// new ReportDocument();
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

        #region Inventory Reports
        public ActionResult INItemStockReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            ViewBag.CompanyID = LoginUser.CompanyID;
            //ViewBag.Units = new List<DVUnit>();
            ViewBag.Items = dal.INItemsList(LoginUser.CompanyID);
            ViewBag.FromDate = DateTime.UtcNow.ToString("dd-MMM-yyyy");
            ViewBag.ToDate = DateTime.UtcNow.ToString("dd-MMM-yyyy");
            return View();
        }

        public ActionResult INGoodsReceiptNoteHistoryDataReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            ViewBag.Items = dal.INItemsList(LoginUser.CompanyID);
            ViewBag.CompanyID = LoginUser.CompanyID;
            //ViewBag.Units = new List<DVUnit>();
            ViewBag.FromDate = null;// DateTime.UtcNow.ToString("dd-MMM-yyyy");
            ViewBag.ToDate = null;// DateTime.UtcNow.ToString("dd-MMM-yyyy");
            return View();
        }

        public ActionResult DownloadINGoodsReceiptNoteHistoryReportExcel(int ProjectID, long? ItemID, DateTime? FromDate, DateTime? ToDate)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Report");

                var db = new GLEntities();


                var spRptINGoodsReceiptNoteHistoryDataList = db.spRptINGoodsReceiptNoteHistoryData(ProjectID, ItemID, FromDate, ToDate).ToList();
                // ====== Final projection ======
                var spRptINGoodsReceiptNoteHistoryDataModel = (
                   from v in spRptINGoodsReceiptNoteHistoryDataList
                   select new spRptINGoodsReceiptNoteHistoryDataModel
                   {
                       Amount = v.Amount.GetValueOrDefault(0),
                       ApprovedQty = v.ApprovedQty.GetValueOrDefault(0),
                       APVendorName = v.APVendorName,
                       Category = v.Category,
                       Company = v.Company,
                       GoodsReceiptNoteID = v.GoodsReceiptNoteID,
                       GoodsReceiptNotesDate = v.GoodsReceiptNotesDate.GetValueOrDefault(DateTime.Now),
                       Group = v.Group,
                       Item = v.Item,
                       ItemID = v.ItemID.GetValueOrDefault(0),
                       ProjectName = v.ProjectName,
                       Rate = v.Rate.GetValueOrDefault(0),
                       ReceivedQty = v.ReceivedQty.GetValueOrDefault(0),
                       RejectedQty = v.RejectedQty.GetValueOrDefault(0),
                       Size = v.Size,
                       Status = v.Status,
                       UOM = v.UOM,

                   }).ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.

                using (var excelPackage = new ExcelPackage())
                {
                    var workSheet = excelPackage.Workbook.Worksheets.Add("INGoodsReceiptNoteHistoryReportExcel");
                    var rowNo = 1;

                    workSheet.Cells[rowNo, 1].Value = "Indigo Developers";// spRptINGoodsReceiptNoteHistoryDataModel.Max(x => x.CompanyName);
                    workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                    workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;

                    rowNo++;
                    rowNo++;

                    workSheet.Cells[rowNo, 1].Value = "Goods Receipt Note History Report";
                    workSheet.Cells[rowNo, 1, rowNo, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowNo, 1, rowNo, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[rowNo, 1, rowNo, 13].Merge = true;
                    workSheet.Cells[rowNo, 1, rowNo, 13].Style.Font.Bold = true;



                    rowNo++;
                    workSheet.Cells[rowNo, 1].Value = "Project";
                    workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                    workSheet.Cells[rowNo, 2].Value = spRptINGoodsReceiptNoteHistoryDataModel.Max(x => x.ProjectName).ToString();

                    workSheet.Cells[rowNo, 11].Value = "From Date";
                    workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                    if (FromDate != null)
                        workSheet.Cells[rowNo, 12].Value = FromDate.Value.ToString("dd-MMM-yyyy");

                    rowNo++;

                    workSheet.Cells[rowNo, 11].Value = "To Date";
                    workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                    if (ToDate != null)
                        workSheet.Cells[rowNo, 12].Value = ToDate.Value.ToString("dd-MMM-yyyy");


                    rowNo++;
                    rowNo++;
                    workSheet.Cells[rowNo, 1].Value = "Sr No";
                    workSheet.Cells[rowNo, 2].Value = "Group";
                    workSheet.Cells[rowNo, 3].Value = "Category";
                    workSheet.Cells[rowNo, 4].Value = "Date";
                    workSheet.Cells[rowNo, 5].Value = "GRN ID";
                    workSheet.Cells[rowNo, 6].Value = "Vendor";
                    workSheet.Cells[rowNo, 7].Value = "Status";
                    workSheet.Cells[rowNo, 8].Value = "Item ID";
                    workSheet.Cells[rowNo, 9].Value = "Item Description";
                    workSheet.Cells[rowNo, 10].Value = "Size";
                    workSheet.Cells[rowNo, 11].Value = "UOM";
                    workSheet.Cells[rowNo, 12].Value = "Approved Qty";
                    workSheet.Cells[rowNo, 13].Value = "Received Qty";
                    workSheet.Cells[rowNo, 14].Value = "Rejected Qty";
                    workSheet.Cells[rowNo, 15].Value = "Rate";
                    workSheet.Cells[rowNo, 16].Value = "Total Amount";


                    workSheet.Cells[rowNo, 1, rowNo, 16].Style.Font.Bold = true;

                    Int64 SrNo = 0;
                    foreach (var row in spRptINGoodsReceiptNoteHistoryDataModel)
                    {
                        rowNo++;
                        //balance += (row.DebitAmount ?? 0) - (row.CreditAmount ?? 0);
                        SrNo++;
                        workSheet.Cells[rowNo, 1].Value = SrNo;
                        // worksheet.Cells[rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 2].Value = row.Group;
                        workSheet.Cells[rowNo, 3].Value = row.Category;
                        workSheet.Cells[rowNo, 4].Value = row.GoodsReceiptNotesDate.ToString("dd-MM-yyyy");
                        workSheet.Cells[rowNo, 5].Value = Convert.ToString(row.GoodsReceiptNoteID);
                        workSheet.Cells[rowNo, 6].Value = row.APVendorName;
                        workSheet.Cells[rowNo, 7].Value = row.Status;
                        workSheet.Cells[rowNo, 8].Value = Convert.ToString(row.ItemID);
                        workSheet.Cells[rowNo, 9].Value = row.Item;
                        workSheet.Cells[rowNo, 10].Value = row.Size;
                        workSheet.Cells[rowNo, 11].Value = row.UOM;
                        workSheet.Cells[rowNo, 12].Value = row.ApprovedQty;
                        workSheet.Cells[rowNo, 13].Value = row.ReceivedQty;
                        workSheet.Cells[rowNo, 14].Value = row.RejectedQty;
                        workSheet.Cells[rowNo, 15].Value = row.Rate;
                        workSheet.Cells[rowNo, 16].Value = row.Amount;


                    }
                    workSheet.Cells[8, 16, rowNo, 16].Style.Numberformat.Format = "#,##0";
                    //worksheet.Cells[8, 1, rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Column(2).Width = 20;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 20;
                    workSheet.Column(5).Width = 10;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 25;
                    workSheet.Column(8).Width = 10;
                    workSheet.Column(9).Width = 40;

                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "GoodsReceiptNoteHistoryReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }

        public ActionResult INStoreIssueNoteHistoryDataReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            ViewBag.CompanyID = LoginUser.CompanyID;
            //ViewBag.Units = new List<DVUnit>();
            ViewBag.FromDate = null;// DateTime.UtcNow.ToString("dd-MMM-yyyy");
            ViewBag.ToDate = null;// DateTime.UtcNow.ToString("dd-MMM-yyyy");
            return View();
        }
        public ActionResult DownloadINStoreIssueNoteHistoryReportExcel(int ProjectID, DateTime? FromDate, DateTime? ToDate)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Store Issue Note History");

                var db = new GLEntities();

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }

                var spRptINStoreIssueNoteHistoryDataList = db.spRptINStoreIssueNoteHistoryData(LoginUser.CompanyID, ProjectID, FromDate, ToDate).ToList();
                // ====== Final projection ======
                var spRptINStoreIssueNoteHistoryDataModel = (
                   from v in spRptINStoreIssueNoteHistoryDataList
                   select new spRptINStoreIssueNoteHistoryDataModel
                   {
                       Company = v.Company,
                       ProjectName = v.ProjectName,
                       Group = v.Group,
                       Category = v.Category,
                       ItemID = v.ItemID.GetValueOrDefault(0),
                       Item = v.Item,
                       Size = v.Size,
                       UOM = v.UOM,
                       LastRate = v.LastRate.GetValueOrDefault(0),
                       IssuedQty = v.IssuedQty.GetValueOrDefault(0),
                       TotalAmount = v.LastRate.GetValueOrDefault(0) * v.IssuedQty.GetValueOrDefault(0)
                   }).ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.

                using (var excelPackage = new ExcelPackage())
                {
                    if (spRptINStoreIssueNoteHistoryDataModel.Count > 0)
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Store Issue Note History");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = spRptINStoreIssueNoteHistoryDataList.Max(x => x.Company).ToString();
                        workSheet.Cells[rowNo, 1, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 10].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 10].Style.Font.Bold = true;

                        rowNo++;
                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "Item Issuance Report";
                        workSheet.Cells[rowNo, 1, rowNo, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 10].Merge = true;
                        workSheet.Cells[rowNo, 1, rowNo, 10].Style.Font.Bold = true;



                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Project";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 2].Value = spRptINStoreIssueNoteHistoryDataModel.Max(x => x.ProjectName).ToString();

                        workSheet.Cells[rowNo, 9].Value = "From Date";
                        workSheet.Cells[rowNo, 9].Style.Font.Bold = true;
                        if (FromDate != null)
                        {
                            workSheet.Cells[rowNo, 10].Value = FromDate.Value.ToString("dd-MMM-yyyy");
                            rowNo++;
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 10].Value = "N/A";
                            rowNo++;
                        }

                        workSheet.Cells[rowNo, 9].Value = "To Date";
                        workSheet.Cells[rowNo, 9].Style.Font.Bold = true;
                        if (ToDate != null)
                        {
                            workSheet.Cells[rowNo, 10].Value = ToDate.Value.ToString("dd-MMM-yyyy");
                            rowNo++;
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 10].Value = "N/A";
                            rowNo++;
                        }


                        rowNo++;
                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Sr No";
                        workSheet.Cells[rowNo, 2].Value = "Group";
                        workSheet.Cells[rowNo, 3].Value = "Category";
                        workSheet.Cells[rowNo, 4].Value = "Item ID";
                        workSheet.Cells[rowNo, 5].Value = "Item Description";
                        workSheet.Cells[rowNo, 6].Value = "Size";
                        workSheet.Cells[rowNo, 7].Value = "UOM";
                        workSheet.Cells[rowNo, 8].Value = "IssuedQty";
                        workSheet.Cells[rowNo, 9].Value = "Rate";
                        workSheet.Cells[rowNo, 10].Value = "Total Amount";


                        workSheet.Cells[rowNo, 1, rowNo, 10].Style.Font.Bold = true;

                        Int64 SrNo = 0;
                        foreach (var row in spRptINStoreIssueNoteHistoryDataModel)
                        {
                            rowNo++;
                            SrNo++;
                            workSheet.Cells[rowNo, 1].Value = SrNo;
                            //worksheet.Cells[rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[rowNo, 2].Value = row.Group;
                            workSheet.Cells[rowNo, 3].Value = row.Category;
                            workSheet.Cells[rowNo, 4].Value = row.ItemID;
                            workSheet.Cells[rowNo, 5].Value = row.Item;
                            workSheet.Cells[rowNo, 6].Value = row.Size;
                            workSheet.Cells[rowNo, 7].Value = row.UOM;
                            workSheet.Cells[rowNo, 8].Value = row.IssuedQty;
                            workSheet.Cells[rowNo, 9].Value = row.LastRate;
                            workSheet.Cells[rowNo, 10].Value = row.TotalAmount;
                        }

                        workSheet.Cells[8, 10, rowNo, 10].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[8, 10, rowNo, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(2).Width = 20;
                        workSheet.Column(3).Width = 20;
                        workSheet.Column(4).Width = 20;
                        workSheet.Column(5).Width = 20;
                        workSheet.Column(6).Width = 20;
                        workSheet.Column(7).Width = 20;
                        workSheet.Column(8).Width = 20;
                        workSheet.Column(9).Width = 20;
                        workSheet.Column(10).Width = 20;
                    }
                    else
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Store Issue Note History");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = "Record not found";
                        workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;
                    }


                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "StoreIssueNoteHistoryReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }

        public ActionResult INPurchaseRequisitionHistoryDataReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            ViewBag.CompanyID = LoginUser.CompanyID;
            //ViewBag.Units = new List<DVUnit>();
            ViewBag.FromDate = null;// DateTime.UtcNow.ToString("dd-MMM-yyyy");
            ViewBag.ToDate = null;//DateTime.UtcNow.ToString("dd-MMM-yyyy");
            return View();
        }
        public ActionResult DownloadPurchaseRequisitionHistoryReportExcel(int ProjectID, DateTime? FromDate, DateTime? ToDate)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Report");

                var db = new GLEntities();


                var spRptINPurchaseRequisitionHistoryDataList = db.spRptINPurchaseRequisitionHistoryData(ProjectID, FromDate, ToDate).ToList();
                // ====== Final projection ======
                var spRptINPurchaseRequisitionHistoryDataModel = (
                   from v in spRptINPurchaseRequisitionHistoryDataList
                   select new spRptINPurchaseRequisitionHistoryDataModel
                   {
                       ItemDescription = v.ItemDescription,
                       RequestDate = v.RequestDate.GetValueOrDefault(DateTime.Now),
                       RequestDetailID = v.RequestDetailID,
                       RequestedQty = v.RequestedQty.GetValueOrDefault(0),
                       RequestTypeDesc = v.RequestTypeDesc,
                       RequestID = v.RequestID,
                       ApprovedQty = v.ApprovedQty.GetValueOrDefault(0),
                       Category = v.Category,
                       Company = v.Company,
                       Group = v.Group,
                       ItemID = v.ItemID.GetValueOrDefault(0),
                       ProjectName = v.ProjectName,
                       ReceivedQty = v.ReceivedQty.GetValueOrDefault(0),
                       Size = v.Size,
                       UOM = v.UOM,

                   }).ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.

                using (var excelPackage = new ExcelPackage())
                {
                    var workSheet = excelPackage.Workbook.Worksheets.Add("INPurchaseRequisitionHistoryReportExcel");
                    var rowNo = 1;

                    workSheet.Cells[rowNo, 1].Value = spRptINPurchaseRequisitionHistoryDataModel.Max(x => x.Company);
                    workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                    workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;

                    rowNo++;
                    rowNo++;

                    workSheet.Cells[rowNo, 1].Value = "Purchase Requisition History Report";
                    workSheet.Cells[rowNo, 1, rowNo, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowNo, 1, rowNo, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[rowNo, 1, rowNo, 13].Merge = true;
                    workSheet.Cells[rowNo, 1, rowNo, 13].Style.Font.Bold = true;



                    rowNo++;
                    workSheet.Cells[rowNo, 1].Value = "Project";
                    workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                    workSheet.Cells[rowNo, 2].Value = spRptINPurchaseRequisitionHistoryDataModel.Max(x => x.ProjectName).ToString();

                    workSheet.Cells[rowNo, 11].Value = "From Date";
                    workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                    if (FromDate != null)
                        workSheet.Cells[rowNo, 12].Value = FromDate.Value.ToString("dd-MMM-yyyy");

                    rowNo++;
                    workSheet.Cells[rowNo, 11].Value = "To Date";
                    workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                    if (ToDate != null)
                        workSheet.Cells[rowNo, 12].Value = ToDate.Value.ToString("dd-MMM-yyyy");


                    rowNo++;
                    rowNo++;
                    workSheet.Cells[rowNo, 1].Value = "Sr No";
                    workSheet.Cells[rowNo, 2].Value = "Group";
                    workSheet.Cells[rowNo, 3].Value = "Category";
                    workSheet.Cells[rowNo, 4].Value = "Date";
                    workSheet.Cells[rowNo, 5].Value = "PR ID";
                    workSheet.Cells[rowNo, 6].Value = "Item ID";
                    workSheet.Cells[rowNo, 7].Value = "Item Description";
                    workSheet.Cells[rowNo, 8].Value = "Size";
                    workSheet.Cells[rowNo, 9].Value = "UOM";
                    workSheet.Cells[rowNo, 10].Value = "Requested Qty";
                    workSheet.Cells[rowNo, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowNo, 11].Value = "Approved Qty";
                    workSheet.Cells[rowNo, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowNo, 12].Value = "Received Qty";
                    workSheet.Cells[rowNo, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;


                    workSheet.Cells[rowNo, 1, rowNo, 16].Style.Font.Bold = true;

                    Int64 SrNo = 0;
                    foreach (var row in spRptINPurchaseRequisitionHistoryDataModel)
                    {
                        rowNo++;
                        //balance += (row.DebitAmount ?? 0) - (row.CreditAmount ?? 0);
                        SrNo++;
                        workSheet.Cells[rowNo, 1].Value = SrNo;
                        // worksheet.Cells[rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 2].Value = row.Group;
                        workSheet.Cells[rowNo, 3].Value = row.Category;
                        workSheet.Cells[rowNo, 4].Value = row.RequestDate.ToString("dd-MM-yyyy");
                        workSheet.Cells[rowNo, 5].Value = Convert.ToString(row.RequestID);
                        workSheet.Cells[rowNo, 6].Value = Convert.ToString(row.ItemID);
                        workSheet.Cells[rowNo, 7].Value = row.ItemDescription;
                        workSheet.Cells[rowNo, 8].Value = row.Size;
                        workSheet.Cells[rowNo, 9].Value = row.UOM;
                        workSheet.Cells[rowNo, 10].Value = row.RequestedQty;
                        workSheet.Cells[rowNo, 11].Value = row.ApprovedQty;
                        workSheet.Cells[rowNo, 12].Value = row.ReceivedQty;


                    }
                    //workSheet.Cells[8, 16, rowNo, 16].Style.Numberformat.Format = "#,##0";
                    //worksheet.Cells[8, 1, rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 20;
                    workSheet.Column(5).Width = 10;
                    workSheet.Column(6).Width = 10;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 20;
                    workSheet.Column(9).Width = 20;

                    workSheet.Column(10).Width = 15;
                    workSheet.Column(11).Width = 15;
                    workSheet.Column(12).Width = 15;

                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "PurchaseRequisitionHistoryReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }

        public ActionResult INPORevertHistoryDataReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            ViewBag.CompanyID = LoginUser.CompanyID;
            //ViewBag.Units = new List<DVUnit>();
            ViewBag.FromDate = null;// DateTime.UtcNow.ToString("dd-MMM-yyyy");
            ViewBag.ToDate = null;// DateTime.UtcNow.ToString("dd-MMM-yyyy");
            return View();
        }

        public ActionResult DownloadPORevertHistoryReportExcel(int ProjectID, DateTime? FromDate, DateTime? ToDate)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Report");

                var db = new GLEntities();


                var spRptINPORevertHistoryDataList = db.spRptINPORevertHistoryData(ProjectID, FromDate, ToDate).ToList();
                // ====== Final projection ======
                var spRptINPurchaseRequisitionHistoryDataModel = (
                   from v in spRptINPORevertHistoryDataList
                   select new spRptINPORevertHistoryDataModel
                   {
                       PurchaseOrderID = v.PurchaseOrderID.GetValueOrDefault(0),
                       PurchaseOrderDate = v.PurchaseOrderDate.GetValueOrDefault(new DateTime(1900, 01, 01)),
                       ProjectID = v.ProjectID.GetValueOrDefault(0),
                       PORevertHistoryID = v.PORevertHistoryID,
                       Discount = v.Discount.GetValueOrDefault(0),
                       Qty = v.Qty.GetValueOrDefault(0),
                       Rate = v.Rate.GetValueOrDefault(0),
                       RevertedDate = v.RevertedDate.GetValueOrDefault(new DateTime(1900, 01, 01)),
                       RevertedDiscount = v.RevertedDiscount.GetValueOrDefault(0),
                       RevertedQty = v.RevertedQty.GetValueOrDefault(0),
                       RevertedRate = v.RevertedRate.GetValueOrDefault(0),
                       Category = v.Category,
                       Group = v.Group,
                       SrNo = v.SrNo.GetValueOrDefault(0),
                       ItemDescription = v.ItemDescription,
                       Status = v.Status,
                       ItemID = v.ItemID.GetValueOrDefault(0),
                       ProjectName = v.ProjectName,
                       RevertedTotalAmount = v.RevertedTotalAmount.GetValueOrDefault(0),
                       Size = v.Size,
                       TotalAmount = v.TotalAmount.GetValueOrDefault(0),
                       UOM = v.UOM,


                   }).ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.

                var project = db.DVProjects.Where(x => x.ProjectID == ProjectID).FirstOrDefault();
                var company = db.Companies.Where(x => x.CompanyID == project.CompanyID).FirstOrDefault();

                using (var excelPackage = new ExcelPackage())
                {
                    var workSheet = excelPackage.Workbook.Worksheets.Add("INPORevertHistoryReportExcel");
                    var rowNo = 1;

                    workSheet.Cells[rowNo, 1].Value = company.Name;
                    workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                    workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;

                    rowNo++;
                    rowNo++;

                    workSheet.Cells[rowNo, 1].Value = "PO Revert History History Report";
                    workSheet.Cells[rowNo, 1, rowNo, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowNo, 1, rowNo, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[rowNo, 1, rowNo, 13].Merge = true;
                    workSheet.Cells[rowNo, 1, rowNo, 13].Style.Font.Bold = true;



                    rowNo++;
                    workSheet.Cells[rowNo, 1].Value = "Project";
                    workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                    workSheet.Cells[rowNo, 2].Value = project.ProjectName;// spRptINPurchaseRequisitionHistoryDataModel.Max(x => x.ProjectName).ToString();

                    workSheet.Cells[rowNo, 11].Value = "From Date";
                    workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                    if (FromDate != null)
                        workSheet.Cells[rowNo, 12].Value = FromDate.Value.ToString("dd-MMM-yyyy");
                    else
                        workSheet.Cells[rowNo, 12].Value = "N/A";

                    rowNo++;
                    workSheet.Cells[rowNo, 11].Value = "To Date";
                    workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                    if (ToDate != null)
                        workSheet.Cells[rowNo, 12].Value = ToDate.Value.ToString("dd-MMM-yyyy");
                    else
                        workSheet.Cells[rowNo, 12].Value = "N/A";

                    rowNo++;
                    rowNo++;
                    workSheet.Cells[rowNo, 1].Value = "Sr No";
                    workSheet.Cells[rowNo, 2].Value = "Project";
                    workSheet.Cells[rowNo, 3].Value = "PO ID";
                    workSheet.Cells[rowNo, 4].Value = "PO Date";
                    workSheet.Cells[rowNo, 5].Value = "Group";
                    workSheet.Cells[rowNo, 6].Value = "Category";
                    workSheet.Cells[rowNo, 7].Value = "Version";
                    workSheet.Cells[rowNo, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowNo, 8].Value = "Status";
                    workSheet.Cells[rowNo, 9].Value = "Item ID";
                    workSheet.Cells[rowNo, 10].Value = "Item Description";
                    workSheet.Cells[rowNo, 11].Value = "Size";
                    workSheet.Cells[rowNo, 12].Value = "UOM";

                    workSheet.Cells[rowNo, 13].Value = "Qty";
                    workSheet.Cells[rowNo, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    workSheet.Cells[rowNo, 14].Value = "Rate";
                    workSheet.Cells[rowNo, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    workSheet.Cells[rowNo, 15].Value = "Discount";
                    workSheet.Cells[rowNo, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    workSheet.Cells[rowNo, 16].Value = "Total Amount";
                    workSheet.Cells[rowNo, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    workSheet.Cells[rowNo, 17].Value = "Reverted Date";
                    workSheet.Cells[rowNo, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    workSheet.Cells[rowNo, 18].Value = "Reverted Time";
                    workSheet.Cells[rowNo, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    workSheet.Cells[rowNo, 19].Value = "Reverted Qty";
                    workSheet.Cells[rowNo, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    workSheet.Cells[rowNo, 20].Value = "Reverted Rate";
                    workSheet.Cells[rowNo, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    workSheet.Cells[rowNo, 21].Value = "Reverted Discount";
                    workSheet.Cells[rowNo, 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    workSheet.Cells[rowNo, 22].Value = "Reverted Total Amount";
                    workSheet.Cells[rowNo, 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    workSheet.Cells[rowNo, 1, rowNo, 22].Style.Font.Bold = true;

                    Int64 SrNo = 0;
                    foreach (var row in spRptINPurchaseRequisitionHistoryDataModel)
                    {
                        if (row.RevertedDiscount > 0 || row.RevertedQty > 0 || row.RevertedRate > 0 || row.Status == "Added" || row.Status == "Deleted")
                        {
                            rowNo++;
                            //balance += (row.DebitAmount ?? 0) - (row.CreditAmount ?? 0);
                            SrNo++;
                            workSheet.Cells[rowNo, 1].Value = SrNo;
                            worksheet.Cells[rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[rowNo, 2].Value = row.ProjectName;
                            workSheet.Cells[rowNo, 3].Value = Convert.ToString(row.PurchaseOrderID);
                            workSheet.Cells[rowNo, 4].Value = row.PurchaseOrderDate == new DateTime(1900, 01, 01) ? "" : row.PurchaseOrderDate.ToString("dd-MM-yyyy");
                            workSheet.Cells[rowNo, 5].Value = row.Group;
                            workSheet.Cells[rowNo, 6].Value = row.Category;
                            workSheet.Cells[rowNo, 7].Value = row.SrNo;
                            workSheet.Cells[rowNo, 8].Value = row.Status;
                            workSheet.Cells[rowNo, 9].Value = Convert.ToString(row.ItemID);
                            workSheet.Cells[rowNo, 10].Value = row.ItemDescription;
                            workSheet.Cells[rowNo, 11].Value = row.Size;

                            workSheet.Cells[rowNo, 12].Value = row.UOM;
                            workSheet.Cells[rowNo, 13].Value = row.Qty;
                            workSheet.Cells[rowNo, 14].Value = row.Rate;
                            workSheet.Cells[rowNo, 15].Value = row.Discount;
                            workSheet.Cells[rowNo, 16].Value = row.TotalAmount;

                            worksheet.Cells[rowNo, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            workSheet.Cells[rowNo, 17].Value = row.RevertedDate == new DateTime(1900, 01, 01) ? "" : row.RevertedDate.ToString("dd-MM-yyyy");

                            worksheet.Cells[rowNo, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            workSheet.Cells[rowNo, 18].Value = row.RevertedDate == new DateTime(1900, 01, 01) ? "" : row.RevertedDate.ToString("hh:mm tt");


                            workSheet.Cells[rowNo, 19].Value = row.RevertedQty;
                            workSheet.Cells[rowNo, 20].Value = row.RevertedRate;
                            workSheet.Cells[rowNo, 21].Value = row.RevertedDiscount;
                            workSheet.Cells[rowNo, 22].Value = row.RevertedTotalAmount;
                        }



                    }
                    //workSheet.Cells[8, 16, rowNo, 16].Style.Numberformat.Format = "#,##0";
                    //worksheet.Cells[8, 1, rowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Column(2).Width = 10;
                    workSheet.Column(3).Width = 10;
                    workSheet.Column(4).Width = 10;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 10;
                    workSheet.Column(8).Width = 20;
                    workSheet.Column(9).Width = 20;

                    workSheet.Column(10).Width = 50;
                    workSheet.Column(11).Width = 20;
                    workSheet.Column(12).Width = 15;

                    workSheet.Column(13).Width = 20;
                    workSheet.Column(14).Width = 20;
                    workSheet.Column(15).Width = 20;
                    workSheet.Column(16).Width = 20;
                    workSheet.Column(17).Width = 20;
                    workSheet.Column(18).Width = 20;
                    workSheet.Column(19).Width = 20;
                    workSheet.Column(20).Width = 20;
                    workSheet.Column(21).Width = 20;
                    workSheet.Column(22).Width = 20;

                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "PORevertHistoryReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }

        public ActionResult APVendorListReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            ViewBag.CompanyID = LoginUser.CompanyID;
            ViewBag.FromDate = null;
            ViewBag.ToDate = null;
            return View();
        }
        public ActionResult DownloadAPVendorListReportExcel(DateTime? FromDate, DateTime? ToDate)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Vendor List Report");

                var db = new GLEntities();

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }

                var spRptVendorList = db.spRptVendorList(LoginUser.CompanyID, FromDate, ToDate).ToList();
                // ====== Final projection ======
                var spRptVendorListModel = (
                   from v in spRptVendorList
                   select new spRptVendorListModel
                   {
                       Company = v.Company,
                       APVendorName = v.APVendorName,
                       Address = v.Address,
                       APVendorCategoryName = v.APVendorCategoryName,
                       BankDetails = v.BankDetails,
                       ContactNumber = v.ContactNumber,
                       ContactPerson = v.ContactPerson,
                       Email = v.Email,
                       CreatedAt = v.CreatedAt.Value,
                   }).ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.

                using (var excelPackage = new ExcelPackage())
                {
                    if (spRptVendorListModel.Count > 0)
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Vendor List History");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = spRptVendorListModel.Max(x => x.Company).ToString();
                        workSheet.Cells[rowNo, 1, 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 9].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 9].Style.Font.Bold = true;

                        rowNo++;
                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "Vendor List Report";
                        workSheet.Cells[rowNo, 1, rowNo, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 9].Merge = true;
                        workSheet.Cells[rowNo, 1, rowNo, 9].Style.Font.Bold = true;

                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "From Date";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        if (FromDate != null)
                        {
                            workSheet.Cells[rowNo, 2].Value = FromDate.Value.ToString("dd-MMM-yyyy");
                            rowNo++;
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 2].Value = "N/A";
                            rowNo++;
                        }

                        workSheet.Cells[rowNo, 1].Value = "To Date";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        if (ToDate != null)
                        {
                            workSheet.Cells[rowNo, 2].Value = ToDate.Value.ToString("dd-MMM-yyyy");
                            rowNo++;
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 2].Value = "N/A";
                            rowNo++;
                        }


                        rowNo++;
                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Sr No";
                        workSheet.Cells[rowNo, 2].Value = "Created Date";
                        workSheet.Cells[rowNo, 3].Value = "Vendor Name";
                        workSheet.Cells[rowNo, 4].Value = "Contact Person";
                        workSheet.Cells[rowNo, 5].Value = "Bank Details";
                        workSheet.Cells[rowNo, 6].Value = "Category";
                        workSheet.Cells[rowNo, 7].Value = "Contact No";
                        workSheet.Cells[rowNo, 8].Value = "Email";
                        workSheet.Cells[rowNo, 9].Value = "Address";

                        workSheet.Cells[rowNo, 1, rowNo, 9].Style.Font.Bold = true;

                        Int64 SrNo = 0;
                        foreach (var row in spRptVendorListModel)
                        {
                            rowNo++;
                            SrNo++;
                            workSheet.Cells[rowNo, 1].Value = SrNo;
                            workSheet.Cells[rowNo, 2].Value = row.CreatedAt.ToString("dd-MMM-yyyy");
                            workSheet.Cells[rowNo, 3].Value = row.APVendorName;
                            workSheet.Cells[rowNo, 4].Value = row.ContactPerson;
                            workSheet.Cells[rowNo, 5].Value = row.BankDetails;
                            workSheet.Cells[rowNo, 6].Value = row.APVendorCategoryName;
                            workSheet.Cells[rowNo, 7].Value = row.ContactNumber;
                            workSheet.Cells[rowNo, 8].Value = row.Email;
                            workSheet.Cells[rowNo, 9].Value = row.Address;
                        }

                        //worksheet.Cells[8, 10, rowNo, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(2).Width = 20;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 30;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 30;
                        workSheet.Column(8).Width = 30;
                        workSheet.Column(9).Width = 40;
                    }
                    else
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Vendor List Report");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = "Record not found";
                        workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;
                    }


                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "VendorListReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }

        public ActionResult INItemListReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.FromDate = null;
            ViewBag.ToDate = null;
            return View();
        }
        public ActionResult DownloadINItemListReportExcel(DateTime? FromDate, DateTime? ToDate)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Item List Report");

                var db = new GLEntities();

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }

                var spRptINItemList = db.spRptINItemList(LoginUser.CompanyID, FromDate, ToDate).ToList();
                // ====== Final projection ======
                var spRptINItemListData = (
                   from v in spRptINItemList
                   select new spRptINItemListModel
                   {
                       Company = v.Company,
                       Category = v.Category,
                       CreatedAt = v.CreatedAt.Value,
                       Group = v.Group,
                       ItemDescription = v.ItemDescription,
                       ItemID = v.ItemID,
                       Size = v.Size,
                       UOM = v.UOM

                   }).ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.

                using (var excelPackage = new ExcelPackage())
                {
                    if (spRptINItemListData.Count > 0)
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Item List");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = spRptINItemListData.Max(x => x.Company).ToString();
                        workSheet.Cells[rowNo, 1, 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 8].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 8].Style.Font.Bold = true;

                        rowNo++;
                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "Item List Report";
                        workSheet.Cells[rowNo, 1, rowNo, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 8].Merge = true;
                        workSheet.Cells[rowNo, 1, rowNo, 8].Style.Font.Bold = true;

                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "From Date";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        if (FromDate != null)
                        {
                            workSheet.Cells[rowNo, 2].Value = FromDate.Value.ToString("dd-MMM-yyyy");
                            rowNo++;
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 2].Value = "N/A";
                            rowNo++;
                        }

                        workSheet.Cells[rowNo, 1].Value = "To Date";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        if (ToDate != null)
                        {
                            workSheet.Cells[rowNo, 2].Value = ToDate.Value.ToString("dd-MMM-yyyy");
                            rowNo++;
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 2].Value = "N/A";
                            rowNo++;
                        }

                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Sr No";
                        workSheet.Cells[rowNo, 2].Value = "Created Date";
                        workSheet.Cells[rowNo, 3].Value = "Group";
                        workSheet.Cells[rowNo, 4].Value = "Category";
                        workSheet.Cells[rowNo, 5].Value = "Item ID";
                        workSheet.Cells[rowNo, 6].Value = "Item Description";
                        workSheet.Cells[rowNo, 7].Value = "Size";
                        workSheet.Cells[rowNo, 8].Value = "UOM";

                        workSheet.Cells[rowNo, 1, rowNo, 8].Style.Font.Bold = true;

                        Int64 SrNo = 0;
                        foreach (var row in spRptINItemListData)
                        {
                            rowNo++;
                            SrNo++;
                            workSheet.Cells[rowNo, 1].Value = SrNo;
                            workSheet.Cells[rowNo, 2].Value = row.CreatedAt.ToString("dd-MMM-yyyy");
                            workSheet.Cells[rowNo, 3].Value = row.Group;
                            workSheet.Cells[rowNo, 4].Value = row.Category;
                            workSheet.Cells[rowNo, 5].Value = row.ItemID.ToString();
                            workSheet.Cells[rowNo, 6].Value = row.ItemDescription;
                            workSheet.Cells[rowNo, 7].Value = row.Size;
                            workSheet.Cells[rowNo, 8].Value = row.UOM;
                        }

                        workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(2).Width = 20;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 30;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 10;
                        workSheet.Column(8).Width = 10;
                    }
                    else
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Vendor List Report");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = "Record not found";
                        workSheet.Cells[rowNo, 1, 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 13].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 13].Style.Font.Bold = true;
                    }


                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "ItemListReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }


        public ActionResult INPendingCompleteDemandsReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            ViewBag.FromDate = null;
            ViewBag.ToDate = null;
            return View();
        }
        public ActionResult DownloadINPendingCompleteDemandsExcel(int? ProjectID, string Status, DateTime? FromDate, DateTime? ToDate)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Demands Status");

                var db = new GLEntities();

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }

                var data = db.spRptPendingCompleteDemands(LoginUser.CompanyID, ProjectID, FromDate, ToDate).ToList();
                var spRptPendingCompleteDemandsList = data.Where(x => x.Status == Status).ToList();
                // ====== Final projection ======
                var PendingCompleteDemandsData = (
                       from v in spRptPendingCompleteDemandsList
                       select new spRptPendingCompleteDemandsModel
                       {
                           Company = v.Company,
                           ProjectName = v.ProjectName,
                           RequestDate = v.RequestDate.Value,
                           RequestedQty = v.RequestedQty.GetValueOrDefault(0),
                           RequestID = v.RequestID,
                           ApprovedQty = v.ApprovedQty.GetValueOrDefault(0),
                           Category = v.Category,
                           Group = v.Group,
                           Item = v.Item,
                           ItemID = v.ItemID.GetValueOrDefault(0),
                           ReceivedQty = v.ReceivedQty.GetValueOrDefault(0),
                           RequestDetailID = v.RequestDetailID,
                           Size = v.Size,
                           Status = v.Status,
                           UOM = v.UOM

                       }).ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.

                using (var excelPackage = new ExcelPackage())
                {
                    if (PendingCompleteDemandsData.Count > 0)
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Demands Status");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = PendingCompleteDemandsData.Max(x => x.Company).ToString();
                        workSheet.Cells[rowNo, 1, 2, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 12].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 12].Style.Font.Bold = true;

                        rowNo++;
                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "Pending Completed Demands List Report";
                        workSheet.Cells[rowNo, 1, rowNo, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 12].Merge = true;
                        workSheet.Cells[rowNo, 1, rowNo, 12].Style.Font.Bold = true;

                        rowNo++;

                        workSheet.Cells[rowNo, 11].Value = "From Date";
                        workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                        if (FromDate != null)
                        {
                            workSheet.Cells[rowNo, 12].Value = FromDate.Value.ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 12].Value = "N/A";
                        }

                        workSheet.Cells[rowNo, 1].Value = "Status";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 2].Value = Status;

                        rowNo++;

                        workSheet.Cells[rowNo, 11].Value = "To Date";
                        workSheet.Cells[rowNo, 11].Style.Font.Bold = true;
                        if (ToDate != null)
                        {
                            workSheet.Cells[rowNo, 12].Value = ToDate.Value.ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 12].Value = "N/A";
                        }


                        workSheet.Cells[rowNo, 1].Value = "Project";
                        workSheet.Cells[rowNo, 2].Value = PendingCompleteDemandsData.Max(x => x.ProjectName).ToString();
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        rowNo++;

                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Sr No";
                        workSheet.Cells[rowNo, 2].Value = "Group";
                        workSheet.Cells[rowNo, 3].Value = "Category";
                        workSheet.Cells[rowNo, 4].Value = "Date";
                        workSheet.Cells[rowNo, 5].Value = "PR ID";
                        //workSheet.CCells[rowNo, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        workSheet.Cells[rowNo, 6].Value = "Item Description";
                        workSheet.Cells[rowNo, 7].Value = "Size";
                        workSheet.Cells[rowNo, 8].Value = "UOM";
                        workSheet.Cells[rowNo, 9].Value = "Requested Qty";
                        workSheet.Cells[rowNo, 10].Value = "Approved Qty";
                        workSheet.Cells[rowNo, 11].Value = "Received Qty";
                        workSheet.Cells[rowNo, 12].Value = "Status";

                        workSheet.Cells[rowNo, 1, rowNo, 12].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 9, rowNo, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        Int64 SrNo = 0;
                        foreach (var row in PendingCompleteDemandsData)
                        {
                            rowNo++;
                            SrNo++;
                            workSheet.Cells[rowNo, 1].Value = SrNo;
                            workSheet.Cells[rowNo, 2].Value = row.Group;
                            workSheet.Cells[rowNo, 3].Value = row.Category;
                            workSheet.Cells[rowNo, 4].Value = row.RequestDate.ToString("dd-MMM-yyyy");
                            workSheet.Cells[rowNo, 5].Value = row.RequestID.ToString();
                            workSheet.Cells[rowNo, 6].Value = row.Item;
                            workSheet.Cells[rowNo, 7].Value = row.Size;
                            workSheet.Cells[rowNo, 8].Value = row.UOM;
                            workSheet.Cells[rowNo, 9].Value = row.RequestedQty;
                            workSheet.Cells[rowNo, 10].Value = row.ApprovedQty;
                            workSheet.Cells[rowNo, 11].Value = row.ReceivedQty;
                            workSheet.Cells[rowNo, 12].Value = row.Status;

                        }

                        workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 40;
                        workSheet.Column(4).Width = 20;
                        workSheet.Column(5).Width = 10;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(8).Width = 10;
                        workSheet.Column(9).Width = 20;
                        workSheet.Column(10).Width = 20;
                        workSheet.Column(11).Width = 20;
                        workSheet.Column(12).Width = 20;

                    }
                    else
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Pending Completed Demands List Report");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = "Record not found";
                        workSheet.Cells[rowNo, 1, 2, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 12].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 12].Style.Font.Bold = true;
                    }


                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "PendingCompletedDemandsReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }


        public ActionResult INPendingPOReport()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            ViewBag.FromDate = null;
            ViewBag.ToDate = null;

            return View();
        }
        public ActionResult DownloadINPendingPOExcel(int? ProjectID, string Status, DateTime? FromDate, DateTime? ToDate)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Pending PO");

                var db = new GLEntities();

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }

                var data = db.spRptINPendingPOList(LoginUser.CompanyID, ProjectID, FromDate, ToDate).ToList();
                var spRptPendingPOList = data.Where(x => x.Status == Status).ToList();
                // ====== Final projection ======
                var PendingCompleteDemandsData = (
                       from v in spRptPendingPOList
                       select new spRptPendingPOModel
                       {
                           Company = v.Company,
                           GoodsReceiptNotesDate = v.GoodsReceiptNotesDate.Value,
                           GoodsReceiptNoteID = v.GoodsReceiptNoteID,
                           APVendorName = v.APVendorName,
                           Amount = v.Amount.GetValueOrDefault(0),
                           Rate = v.Rate.GetValueOrDefault(0),
                           ProjectName = v.ProjectName,
                           Category = v.Category,
                           Group = v.Group,
                           ItemName = v.ItemName,
                           ItemID = v.ItemID.GetValueOrDefault(0),
                           ReceivedQty = v.ReceivedQty.GetValueOrDefault(0),
                           Size = v.Size,
                           Status = v.Status,
                           UOM = v.UOM

                       }).ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.

                using (var excelPackage = new ExcelPackage())
                {
                    if (PendingCompleteDemandsData.Count > 0)
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Pending PO Status");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = PendingCompleteDemandsData.Max(x => x.Company).ToString();
                        workSheet.Cells[rowNo, 1, 2, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 11].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 11].Style.Font.Bold = true;

                        rowNo++;
                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "Pending PO Report";
                        workSheet.Cells[rowNo, 1, rowNo, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 11].Merge = true;
                        workSheet.Cells[rowNo, 1, rowNo, 11].Style.Font.Bold = true;

                        rowNo++;

                        workSheet.Cells[rowNo, 10].Value = "From Date";
                        workSheet.Cells[rowNo, 10].Style.Font.Bold = true;
                        if (FromDate != null)
                        {
                            workSheet.Cells[rowNo, 11].Value = FromDate.Value.ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 11].Value = "N/A";
                        }

                        workSheet.Cells[rowNo, 1].Value = "Status";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 2].Value = Status;

                        rowNo++;

                        workSheet.Cells[rowNo, 10].Value = "To Date";
                        workSheet.Cells[rowNo, 10].Style.Font.Bold = true;
                        if (ToDate != null)
                        {
                            workSheet.Cells[rowNo, 11].Value = ToDate.Value.ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            workSheet.Cells[rowNo, 11].Value = "N/A";
                        }


                        workSheet.Cells[rowNo, 1].Value = "Project";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 2].Value = PendingCompleteDemandsData.Max(x => x.ProjectName).ToString();

                        rowNo++;

                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Sr No";
                        workSheet.Cells[rowNo, 2].Value = "Date";
                        workSheet.Cells[rowNo, 3].Value = "GRN ID";
                        workSheet.Cells[rowNo, 4].Value = "PO Status";
                        workSheet.Cells[rowNo, 5].Value = "Vendor";
                        workSheet.Cells[rowNo, 6].Value = "Item Description";
                        workSheet.Cells[rowNo, 7].Value = "Size";
                        workSheet.Cells[rowNo, 8].Value = "UOM";
                        workSheet.Cells[rowNo, 9].Value = "Received Qty";
                        workSheet.Cells[rowNo, 10].Value = "Rate";
                        workSheet.Cells[rowNo, 11].Value = "Total Amount";

                        workSheet.Cells[rowNo, 1, rowNo, 11].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 9, rowNo, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        Int64 SrNo = 0;
                        foreach (var row in PendingCompleteDemandsData)
                        {
                            rowNo++;
                            SrNo++;
                            workSheet.Cells[rowNo, 1].Value = SrNo;
                            workSheet.Cells[rowNo, 2].Value = row.GoodsReceiptNotesDate.ToString("dd-MMM-yyyy");
                            workSheet.Cells[rowNo, 3].Value = row.GoodsReceiptNoteID.ToString();
                            workSheet.Cells[rowNo, 4].Value = row.Status;
                            workSheet.Cells[rowNo, 5].Value = row.APVendorName;
                            workSheet.Cells[rowNo, 6].Value = row.ItemName;
                            workSheet.Cells[rowNo, 7].Value = row.Size;
                            workSheet.Cells[rowNo, 8].Value = row.UOM;
                            workSheet.Cells[rowNo, 9].Value = row.ReceivedQty;
                            workSheet.Cells[rowNo, 10].Value = row.Rate;
                            workSheet.Cells[rowNo, 11].Value = row.Amount;

                        }
                        workSheet.Cells[8, 8, rowNo, 11].Style.Numberformat.Format = "#,##0";

                        workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(2).Width = 20;
                        workSheet.Column(3).Width = 20;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 40;
                        workSheet.Column(6).Width = 20;
                        workSheet.Column(7).Width = 20;
                        workSheet.Column(8).Width = 10;
                        workSheet.Column(9).Width = 20;
                        workSheet.Column(10).Width = 20;
                        workSheet.Column(11).Width = 20;

                    }
                    else
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Pending PO Report");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = "Record not found";
                        workSheet.Cells[rowNo, 1, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 10].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 10].Style.Font.Bold = true;
                    }


                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "PendingPOReport.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }

        public ActionResult INItemRateComparison()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            DALDropdowns dal = new DALDropdowns();
            ViewBag.Projects = dal.INProjectsList(LoginUser.CompanyID);
            return View();
        }

        public ActionResult DownloadINItemRateComparisonExcel(int? ProjectID)
        {
            // EPPlus license context (required in newer versions)
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.


            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Item Rate Comparison");

                var db = new GLEntities();

                var LoginUser = (spLoginUser_Result)Session["LoginUser"];
                if (LoginUser == null)
                {
                    return RedirectToAction("Login", "Security");
                }

                var data = db.spRptINItemRateComparisonList(LoginUser.CompanyID, ProjectID).ToList();
                var INItemRateComparisonList = data.Where(x => x.LastRate != null).ToList();
                // ====== Final projection ======
                var INItemRateComparisonListData = (
                       from v in INItemRateComparisonList
                       select new spRptINItemRateComparisonListModel
                       {
                           Company = v.Company,
                           ProjectName = v.ProjectName,
                           Category = v.Category,
                           Group = v.Group,
                           ItemName = v.ItemName,
                           ItemID = v.ItemID.GetValueOrDefault(0),
                           Size = v.Size,
                           UOM = v.UOM,
                           LastRate = v.LastRate.GetValueOrDefault(0),
                           LastRateDate = v.LastRateDate.GetValueOrDefault(DateTime.MinValue),
                           LastRate2 = v.LastRate2.GetValueOrDefault(0),
                           LastRateDate2 = v.LastRateDate2.GetValueOrDefault(DateTime.MinValue),
                           LastRate3 = v.LastRate3.GetValueOrDefault(0),
                           LastRateDate3 = v.LastRateDate3.GetValueOrDefault(DateTime.MinValue),
                           LastRate4 = v.LastRate4.GetValueOrDefault(0),
                           LastRateDate4 = v.LastRateDate4.GetValueOrDefault(DateTime.MinValue)

                       }).ToList();


                ExcelPackage.License.SetNonCommercialPersonal("Indigo"); //This will also set the Author property to the name provided in the argument.

                using (var excelPackage = new ExcelPackage())
                {
                    if (INItemRateComparisonListData.Count > 0)
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Item Rate Comparison");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = INItemRateComparisonListData.Max(x => x.Company).ToString();
                        workSheet.Cells[rowNo, 1, 2, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 11].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 11].Style.Font.Bold = true;

                        rowNo++;
                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "Pending PO Report";
                        workSheet.Cells[rowNo, 1, rowNo, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, rowNo, 11].Merge = true;
                        workSheet.Cells[rowNo, 1, rowNo, 11].Style.Font.Bold = true;

                        rowNo++;

                        workSheet.Cells[rowNo, 1].Value = "Project";
                        workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                        workSheet.Cells[rowNo, 2].Value = INItemRateComparisonListData.Max(x => x.ProjectName).ToString();

                        rowNo++;

                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = "Sr No";
                        workSheet.Cells[rowNo, 2].Value = "Group";
                        workSheet.Cells[rowNo, 3].Value = "Category";
                        workSheet.Cells[rowNo, 4].Value = "ItemID";
                        workSheet.Cells[rowNo, 5].Value = "ItemName";
                        workSheet.Cells[rowNo, 6].Value = "Size";
                        workSheet.Cells[rowNo, 7].Value = "UOM";

                        workSheet.Cells[rowNo, 8].Value = "Last Purchase Rate";
                        workSheet.Cells[rowNo, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        workSheet.Cells[rowNo, 9].Value = "2nd Last Purchase Rate";
                        workSheet.Cells[rowNo, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        //workSheet.Cells[rowNo, 11].Value = "2nd Last Purchase Date";
                        //workSheet.Cells[rowNo, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowNo, 10].Value = "3rd Last Purchase Rate";
                        workSheet.Cells[rowNo, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        //workSheet.Cells[rowNo, 13].Value = "3rd Last Purchase Date";
                        //workSheet.Cells[rowNo, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowNo, 11].Value = "4th Last Purchase Rate";
                        workSheet.Cells[rowNo, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        //workSheet.Cells[rowNo, 15].Value = "4th Last Purchase Date";
                        //workSheet.Cells[rowNo, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowNo, 12].Value = "Last Purchase Date";
                        workSheet.Cells[rowNo, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowNo, 1, rowNo, 12].Style.Font.Bold = true;
                        //workSheet.Cells[rowNo, 9, rowNo, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        Int64 SrNo = 0;
                        foreach (var row in INItemRateComparisonListData)
                        {
                            rowNo++;
                            SrNo++;
                            workSheet.Cells[rowNo, 1].Value = SrNo;
                            workSheet.Cells[rowNo, 2].Value = row.Group;
                            workSheet.Cells[rowNo, 3].Value = row.Category;
                            workSheet.Cells[rowNo, 4].Value = row.ItemID.ToString();
                            workSheet.Cells[rowNo, 5].Value = row.ItemName;
                            workSheet.Cells[rowNo, 6].Value = row.Size;
                            workSheet.Cells[rowNo, 7].Value = row.UOM;

                            workSheet.Cells[rowNo, 8].Value = row.LastRate > 0 ? row.LastRate : (object)null;
                            //workSheet.Cells[rowNo, 9].Value = row.LastRateDate != DateTime.MinValue ? row.LastRateDate.ToString("dd-MMM-yyyy") : "";
                            //workSheet.Cells[rowNo, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[rowNo, 9].Value = row.LastRate2 > 0 ? row.LastRate2 : (object)null;
                            //workSheet.Cells[rowNo, 11].Value = row.LastRateDate2 != DateTime.MinValue ? row.LastRateDate2.ToString("dd-MMM-yyyy"): "";
                            //workSheet.Cells[rowNo, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[rowNo, 10].Value = row.LastRate3 > 0 ? row.LastRate3 : (object)null;
                            //workSheet.Cells[rowNo, 13].Value = row.LastRateDate3 != DateTime.MinValue ? row.LastRateDate3.ToString("dd-MMM-yyyy") : "";
                            //workSheet.Cells[rowNo, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[rowNo, 11].Value = row.LastRate4 > 0 ? row.LastRate4 : (object)null;
                            //workSheet.Cells[rowNo, 15].Value = row.LastRateDate4 != DateTime.MinValue ? row.LastRateDate4.ToString("dd-MMM-yyyy") : "";
                            //workSheet.Cells[rowNo, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[rowNo, 12].Value = row.LastRateDate != DateTime.MinValue ? row.LastRateDate.ToString("dd-MMM-yyyy") : "";
                            workSheet.Cells[rowNo, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                        }
                        workSheet.Cells[8, 8, rowNo, 11].Style.Numberformat.Format = "#,##0";

                        workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Column(2).Width = 20;
                        workSheet.Column(3).Width = 20;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 40;
                        workSheet.Column(6).Width = 20;
                        workSheet.Column(7).Width = 20;

                        workSheet.Column(8).Width = 25;
                        workSheet.Column(9).Width = 25;
                        workSheet.Column(10).Width = 25;
                        workSheet.Column(11).Width = 25;
                        workSheet.Column(12).Width = 25;
                        //workSheet.Column(13).Width = 25;
                        //workSheet.Column(14).Width = 25;
                        //workSheet.Column(15).Width = 25;

                    }
                    else
                    {
                        var workSheet = excelPackage.Workbook.Worksheets.Add("Item Rate Comparison");
                        var rowNo = 1;

                        workSheet.Cells[rowNo, 1].Value = "Record not found";
                        workSheet.Cells[rowNo, 1, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowNo, 1, 2, 10].Merge = true;
                        workSheet.Cells[rowNo, 1, 2, 10].Style.Font.Bold = true;
                    }


                    // Export as Excel file
                    var stream = new MemoryStream();
                    excelPackage.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "ItemRateComparison.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream, contentType, fileName);

                }

            }
        }


        #endregion



    }
}

