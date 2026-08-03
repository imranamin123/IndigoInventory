using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GL;
using GL.DAL;
using GL.EF;
using GL.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GL.Controllers
{
    public class HomeController : Controller
    {
        #region "Dashboard"
        //[Authorize]`
        public ActionResult Dashboard()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }

            DALDropdowns dalDropdowns = new DALDropdowns();
            ViewBag.Projects = dalDropdowns.INProjectsList(LoginUser.CompanyID);
            ViewBag.DefaultToDate = DateTime.Today;
            ViewBag.DefaultFromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);

            return View();
        }

        [HttpPost]
        public JsonResult GetDashboardKPIJson(int ProjectID, DateTime FromDate, DateTime ToDate)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                DALInventory dal = new DALInventory();
                var kpi = dal.GetDashboardKPI(LoginUser.CompanyID, ProjectID, FromDate, ToDate);
                return Json(kpi, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetDashboardStockByCategoryJson(int ProjectID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                DALInventory dal = new DALInventory();
                var list = dal.GetDashboardStockByCategory(LoginUser.CompanyID, ProjectID);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<spRptINDashboardStockByCategory_Result>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetDashboardTopItemsJson(int ProjectID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                DALInventory dal = new DALInventory();
                var list = dal.GetDashboardTopItemsByValue(LoginUser.CompanyID, ProjectID, 10);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<spRptINDashboardTopItemsByValue_Result>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetDashboardItemMovementJson(int ProjectID, DateTime FromDate, DateTime ToDate)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                DALInventory dal = new DALInventory();
                var list = dal.GetDashboardItemMovement(LoginUser.CompanyID, ProjectID, FromDate, ToDate);

                var fastMoving = list.Where(x => x.IssuedQty.GetValueOrDefault(0) > 0)
                                      .OrderByDescending(x => x.IssuedQty)
                                      .Take(10)
                                      .ToList();

                var slowMoving = list.Where(x => x.IssuedQty.GetValueOrDefault(0) == 0 && x.QtyInHand.GetValueOrDefault(0) > 0)
                                      .OrderByDescending(x => x.QtyInHand)
                                      .Take(10)
                                      .ToList();

                return Json(new { fastMoving, slowMoving }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { fastMoving = new List<spRptINDashboardItemMovement_Result>(), slowMoving = new List<spRptINDashboardItemMovement_Result>() }, JsonRequestBehavior.AllowGet);
            }
        }

        // Uses JsonConvert (not the default Json() serializer) so DateTime fields
        // serialize as ISO strings the client's `new Date(...)` can parse, instead
        // of the default "/Date(ms)/" format.
        [HttpPost]
        public ActionResult GetDashboardMonthlyTrendJson(int ProjectID, DateTime FromDate, DateTime ToDate)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            try
            {
                DALInventory dal = new DALInventory();
                var list = dal.GetDashboardMonthlyTrend(LoginUser.CompanyID, ProjectID, FromDate, ToDate);
                return Content(JsonConvert.SerializeObject(list), "application/json");
            }
            catch (Exception)
            {
                return Content(JsonConvert.SerializeObject(new List<spRptINDashboardMonthlyTrend_Result>()), "application/json");
            }
        }

        [HttpPost]
        public ActionResult GetDashboardRecentActivityJson(int ProjectID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }
            try
            {
                DALInventory dal = new DALInventory();
                var list = dal.GetDashboardRecentActivity(LoginUser.CompanyID, ProjectID, 15);
                return Content(JsonConvert.SerializeObject(list), "application/json");
            }
            catch (Exception)
            {
                return Content(JsonConvert.SerializeObject(new List<spRptINDashboardRecentActivity_Result>()), "application/json");
            }
        }

        [HttpPost]
        public JsonResult GetUnitSaleDataJson(int ProjectID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return Json(new GL.Models.response { status = false, resMessage = "Session expired. Please login again." }, JsonRequestBehavior.AllowGet);
            }
            var pieChart = new PieChart { };
            DALCommon dal = new DALCommon();
            //List<PieChart> pieCharts = new List<PieChart>();
            List<spDashboardGetUnitStatusProjectWise_Result> list = dal.GetDashboardUnitStatusProjectWise(LoginUser.CompanyID, ProjectID);
            if (list != null && list.Count > 0)
            {
                List<string> customLabels = new List<string>();
                List<Int64> data = new List<Int64>();
                foreach (var item in list)
                {
                    customLabels.Add(item.Status + ": " + item.Units + "  Rs: " + Convert.ToString(item.Amount.ToString("##,###,##0.##")) + " SQFT: " + item.SQFT );
                    data.Add(item.Units.GetValueOrDefault(0));
                }

                pieChart.customLabels = customLabels;
                pieChart.data = data;

            }
            return Json(pieChart,JsonRequestBehavior.AllowGet);

        }

        public ActionResult DownloadUnitStatusDataExcel(int ProjectID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            if (LoginUser == null)
            {
                return RedirectToAction("Login", "Security");
            }

            ExcelPackage.License.SetNonCommercialPersonal("Indigo");

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("UnitStatusData");
                var dal = new DALCommon();
                var result = dal.GetDashboardUnitStatusProjectWise(LoginUser.CompanyID, ProjectID);
                var projectName = new DALDropdowns().INProjectsList(LoginUser.CompanyID)
                    .FirstOrDefault(x => x.ProjectID == ProjectID)?.ProjectName ?? string.Empty;

                var now = DateTime.Now;
                var rowNo = 1;

                workSheet.Cells[rowNo, 1].Value = "Unit Status Data";
                workSheet.Cells[rowNo, 1, rowNo, 6].Merge = true;
                workSheet.Cells[rowNo, 1, rowNo, 6].Style.Font.Bold = true;
                workSheet.Cells[rowNo, 1, rowNo, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                workSheet.Cells[rowNo, 10].Value = "Print Date Time";
                workSheet.Cells[rowNo, 10].Style.Font.Bold = true;
                workSheet.Cells[rowNo, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[rowNo, 11].Value = now.ToString("dd-MMM-yyyy HH:mm:ss");
                workSheet.Cells[rowNo, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                rowNo++;
                workSheet.Cells[rowNo, 1].Value = "Selection Criteria";
                workSheet.Cells[rowNo, 1, rowNo, 6].Merge = true;
                workSheet.Cells[rowNo, 1, rowNo, 6].Style.Font.Bold = true;
                workSheet.Cells[rowNo, 1, rowNo, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                rowNo++;
                workSheet.Cells[rowNo, 1].Value = "Project";
                workSheet.Cells[rowNo, 1].Style.Font.Bold = true;
                workSheet.Cells[rowNo, 2].Value = projectName;
                workSheet.Cells[rowNo, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                rowNo += 2;
                workSheet.Cells[rowNo, 1].Value = "Status";
                workSheet.Cells[rowNo, 2].Value = "Units";
                workSheet.Cells[rowNo, 3].Value = "Amount";
                workSheet.Cells[rowNo, 4].Value = "SQFT";
                workSheet.Cells[rowNo, 1, rowNo, 4].Style.Font.Bold = true;
                workSheet.Cells[rowNo, 1, rowNo, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        rowNo++;
                        workSheet.Cells[rowNo, 1].Value = item.Status;
                        workSheet.Cells[rowNo, 2].Value = item.Units;
                        workSheet.Cells[rowNo, 3].Value = item.Amount;
                        workSheet.Cells[rowNo, 4].Value = item.SQFT;
                    }
                }
                else
                {
                    rowNo++;
                    workSheet.Cells[rowNo, 1].Value = "No data found.";
                    workSheet.Cells[rowNo, 1, rowNo, 4].Merge = true;
                    workSheet.Cells[rowNo, 1, rowNo, 4].Style.Font.Italic = true;
                }

                workSheet.Cells[1, 1, rowNo, 4].AutoFitColumns();
                workSheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Column(3).Style.Numberformat.Format = "#,##0.00";
                workSheet.Column(4).Style.Numberformat.Format = "#,##0";

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string fileName = $"UnitStatusData_{now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        [HttpPost]
        public JsonResult GetUnitMatrixDataJson(int ProjectID = 1)
        {
            DALCommon dal = new DALCommon();
            List<spDashboardUnitMatrixProjectWise_Result> list = dal.GetDashboardUnitMatrixProjectWise(ProjectID);

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        #endregion

        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}