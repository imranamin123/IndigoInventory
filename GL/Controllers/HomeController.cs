using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GL;
using GL.DAL;
using GL.EF;
using GL.Models;

namespace GL.Controllers
{
    public class HomeController : Controller
    {
        #region "Dashboard"
        //[Authorize]`  
        public ActionResult Dashboard()
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
            DALDropdowns dalDropdowns = new DALDropdowns();
            ViewBag.Projects = dalDropdowns.DVProjectsList(LoginUser.CompanyID);
            return View();
        }

        [HttpPost]
        public JsonResult GetUnitSaleDataJson(int ProjectID)
        {
            var LoginUser = (spLoginUser_Result)Session["LoginUser"];
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