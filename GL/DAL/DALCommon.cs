using DAL;
using GL.EF;
using GL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace GL.DAL
{
    public class DALCommon
    {
        GLEntities db = new GLEntities();
        public FiscalYearSetup GetFiscalYearSetup(int CompanyID, int year, int month)
        {
            var fiscalYearSetup = db.FiscalYearSetups.Where(x => x.CompanyID == CompanyID && x.CalanderMonth.Value.Year == year && x.CalanderMonth.Value.Month == month).FirstOrDefault();                ;
            return fiscalYearSetup;
        }

        public DVPaymentPackage GetDVPaymentPackage(int CompanyID, int? PaymentPlanID=null)
        {
            if (PaymentPlanID != null && PaymentPlanID != 0)
            { 
                var paymentPlanDetails = db.DVPaymentPlanDetails.Where(x => x.PaymentPlanID == PaymentPlanID).ToList();
                if (paymentPlanDetails != null)
                {
                    db.DVPaymentPlanDetails.RemoveRange(paymentPlanDetails);
                    db.SaveChanges();
                }
            }

            var DVPaymentPackage = db.DVPaymentPackages.Where(x => x.CompanyID == CompanyID).FirstOrDefault();

            return DVPaymentPackage; 
        }



        #region "Dashboard"

        public List<spDashboardGetUnitStatusProjectWise_Result> GetDashboardUnitStatusProjectWise(int CompanyID, int ProjectID)
        {
            var DashboardUnitStatusProjectWise = db.spDashboardGetUnitStatusProjectWise(CompanyID, ProjectID).ToList();
            return DashboardUnitStatusProjectWise;
        }

        public List<spDashboardUnitMatrixProjectWise_Result> GetDashboardUnitMatrixProjectWise(int ProjectID) {
            var data = db.spDashboardUnitMatrixProjectWise(ProjectID).ToList();
            return data;
        }

        //public List<DVUnit> GetDashboardvwFloorUnit()
        //{
        //    var result = db.Database.SqlQuery<DVUnit>("SELECT * FROM DVUnit").ToList();
                        
        //    return result;
        //}

        #endregion

        //public static void saveImages(int id, PictureBox pictureBox, string caller)
        //{
        //    DALSetup dal = new DALSetup();
        //    string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //    string exeDir = System.IO.Path.GetDirectoryName(exePath);
        //    DirectoryInfo binDir = System.IO.Directory.GetParent(exeDir);
        //    DirectoryInfo appFolderDirInfo = System.IO.Directory.GetParent(binDir.ToString());
        //    string appFolder = appFolderDirInfo.ToString();
        //    string fileName = string.Empty;
        //    string extension = ".png";


        //    // first remove old picture if exist

        //        Student student = dal.StudentGet(id);
        //        if (student.Picture != null && student.Picture != string.Empty)
        //        {
        //            fileName = student.Picture;
        //            string ExistingFilename = appFolder + "\\Images\\" + fileName;
        //            if (File.Exists(ExistingFilename))
        //            {
        //                File.Delete(ExistingFilename);
        //            }
        //        }

        //        fileName = "STD_" + DateTime.Now.ToString("yymmssfff") + extension;
        //        string name = appFolder + "\\Images\\" + fileName;
        //        pictureBox.Image.Save(name);

        //        student.Picture = fileName;
        //        dal.StudentSave(student);


        //}

    }
}