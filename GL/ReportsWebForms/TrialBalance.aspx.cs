using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;
using GL.EF;
using GL.Models;
using GL.Reports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GL.ReportsWebForms
{
    public partial class TrialBalance : System.Web.UI.Page
    {
        protected static Queue reportQueue = null;
        private GLEntities db = new GLEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            reportQueue= new Queue();

            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            rptTrialBalance report = new rptTrialBalance();

            Nullable<int> CompanyID = null;
            string FiscalYear = "";
            if (!string.IsNullOrEmpty(Request.QueryString["CompanyID"]))
            {
                CompanyID = Convert.ToInt32(Request.QueryString["CompanyID"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["FiscalYear"]))
            {
                FiscalYear = Convert.ToString(Request.QueryString["FiscalYear"]);
            }

            var TrialBalanceModel = db.spRptGLTrialBalance(CompanyID,FiscalYear).ToList();


            var TrialBalanceData = (
                from v in TrialBalanceModel
                select new
                {
                    CompanyID = v.CompanyID,
                    Company = v.Company ?? "",
                    FiscalYear = v.FiscalYear ?? "",
                    GLAccountNo = v.GLAccountNo ?? "",
                    Description = v.Description ?? "",
                    Debit = v.Debit,
                    Credit = v.Credit,
                    Total = v.Total,
                }).ToList();



            report.SetDataSource(TrialBalanceData);

            reportQueue.Enqueue(report);

            CrystalReportViewer1.ReportSource = report;
            CrystalReportViewer1.RefreshReport();


        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            int cnd = reportQueue.Count;
            ((ReportClass)reportQueue.Dequeue()).Dispose();

        }
    }
}