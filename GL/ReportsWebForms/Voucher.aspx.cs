using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;
using GL.DAL;
using GL.EF;
using GL.Reports;

namespace GL.ReportsWebForms
{
    public partial class wfVoucher : System.Web.UI.Page
    {
        protected static Queue reportQueue = null;
        private GLEntities db = new GLEntities();

        protected void Page_Load(object sender, EventArgs e)
        {
            
            reportQueue = new Queue();

            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            rptVoucher report = new rptVoucher();

            int GLVoucherID = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["GLVoucherID"]))
            {
                GLVoucherID = Convert.ToInt32(Request.QueryString["GLVoucherID"]);
            }
            
            
            var Vouchers = db.spRptGLVoucherGetByID(GLVoucherID).ToList(); 

            report.SetDataSource(Vouchers);
            
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