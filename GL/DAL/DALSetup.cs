using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Data.Entity.Migrations;

using GL.EF;
using GL.Models;


namespace DAL
{
    public class DALSetup
    {

        private GLEntities db = new GLEntities();

        #region Company
        public List<Company> CompanyList()
        {
            try
            {
                var CompanyList = db.Companies.ToList();
                return CompanyList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Company CompanyGet(Int64 CompanyID)
        {
            try
            {
                var Company = db.Companies.Where(x => x.CompanyID == CompanyID).FirstOrDefault();
                return Company;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool CompanyDelete(Int64 CompanyID)
        {
            try
            {
                var Company = db.Companies.Where(x => x.CompanyID == CompanyID).FirstOrDefault();
                db.Companies.Remove(Company);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool CompanySave(Company Company)
        {
            try
            {
                if (Company.CompanyID == 0)
                {
                    Company.CompanyID = db.Companies.Max(x => x.CompanyID) + 1;
                }

                if (Company != null)
                {
                    db.Companies.AddOrUpdate(Company);
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
        #endregion

        #region FiscalYearSetup
        public List<FiscalYearSetup> FiscalYearSetupList(int CompanyID)
        {
            try
            {
                var FiscalYearSetupList = db.FiscalYearSetups.Where(x => x.CompanyID == CompanyID).ToList();
                return FiscalYearSetupList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        public FiscalYearSetup FiscalYearSetupInitialGet( int CompanyID)
        {
            try
            {
                var MinYear = db.FiscalYearSetups.Where(x => x.CompanyID == CompanyID).Min(x => x.FiscalYear);
                if(MinYear == null)
                {
                    return null;
                }
                var CalanderMonth = db.FiscalYearSetups.Where( x => x.CompanyID == CompanyID && x.FiscalYear == MinYear ).Max(x => x.CalanderMonth);    
                FiscalYearSetup fiscalYearSetup = db.FiscalYearSetups.Where( x => x.CompanyID == CompanyID && x.FiscalYear == MinYear && x.CalanderMonth == CalanderMonth).FirstOrDefault();
                fiscalYearSetup.FiscalPeriod = fiscalYearSetup.CalanderMonth.Value.Month;
              
                return fiscalYearSetup;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool FiscalYearSetupDelete(Int64 FiscalYearSetupID)
        {
            try
            {
                var FiscalYearSetup = db.FiscalYearSetups.Where(x => x.FiscalYearSetupID == FiscalYearSetupID).FirstOrDefault();
                db.FiscalYearSetups.Remove(FiscalYearSetup);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool FiscalYearSetupSave(FiscalYearSetup fiscalYearSetup)
        {
            try
            {
                var lastDay = DateTime.DaysInMonth(fiscalYearSetup.FiscalYear.Value, fiscalYearSetup.FiscalPeriod.Value);
                var endCalanderMonthYear = new DateTime(fiscalYearSetup.FiscalYear.Value, fiscalYearSetup.FiscalPeriod.Value, lastDay);
                var startCalanderMonthYear = endCalanderMonthYear.AddYears(-1);
                var iteratingCalanderMonthYear = startCalanderMonthYear.AddMonths(1);

                for (int i = 1; i <= 12; i++)
                {
                    FiscalYearSetup fiscalYearSetupDB = new FiscalYearSetup();
                    fiscalYearSetupDB.FiscalYear = fiscalYearSetup.FiscalYear;
                    fiscalYearSetupDB.FiscalPeriod = fiscalYearSetup.FiscalPeriod;


                    var iteratinglastDay = DateTime.DaysInMonth(iteratingCalanderMonthYear.Year, iteratingCalanderMonthYear.Month);
                    fiscalYearSetupDB.CalanderMonth = new DateTime(iteratingCalanderMonthYear.Year, iteratingCalanderMonthYear.Month, iteratinglastDay);
                    iteratingCalanderMonthYear = new DateTime(iteratingCalanderMonthYear.Year, iteratingCalanderMonthYear.Month, iteratinglastDay);
                    fiscalYearSetupDB.CalanderMonth = iteratingCalanderMonthYear;
                    fiscalYearSetupDB.FiscalYearSetupID = 0;
                    fiscalYearSetupDB.FiscalPeriod = i;
                   
                    fiscalYearSetupDB.CreatedBy = fiscalYearSetup.CreatedBy;
                    fiscalYearSetupDB.CreatedAt=fiscalYearSetup.CreatedAt;
                    fiscalYearSetupDB.CompanyID=fiscalYearSetup.CompanyID;

                    db.FiscalYearSetups.AddOrUpdate(fiscalYearSetupDB); 
                    db.SaveChanges();
                    var newMonth = iteratingCalanderMonthYear.AddMonths(1);
                    iteratingCalanderMonthYear = newMonth;
                }
              

               
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsFiscalYearSetupExist(FiscalYearSetup fiscalYearSetup)
        {
            try
            {
                bool isExist = false;
                var fiscalYearSetupDb = db.FiscalYearSetups.Where(x => x.FiscalYear == fiscalYearSetup.FiscalYear && x.CompanyID == fiscalYearSetup.CompanyID).FirstOrDefault();
                if (fiscalYearSetupDb != null)
                {
                    isExist = true;
                }
                return isExist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region User
        public List<SecUser> SecUserList(Int64 CompanyID)
        {
            try
            {
                List<SecUser> SecUserList = null;
                SecUserList = db.SecUsers.Where(x => x.CompanyID == CompanyID).ToList();

                return SecUserList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spSecUserGetSearchList_Result> GetSecUserGetSearchList(SearchModel search)
        {
            try
            {
                List<spSecUserGetSearchList_Result> secUserGetSearchList = db.spSecUserGetSearchList(search.CompanyID, search.UserID, search.Name, search.username, search.StatusTypeID, search.RoleID).ToList();

                return secUserGetSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public SecUser SecUserGet(Int64 UserID)
        {
            try
            {
                var SecUser = db.SecUsers.Where(x => x.UsersID == UserID).FirstOrDefault();
                return SecUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SecUserDelete(Int64 UserID)
        {
            try
            {
                var SecUser = db.SecUsers.Where(x => x.UsersID == UserID).FirstOrDefault();
                db.SecUsers.Remove(SecUser);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SecUserSave(SecUser SecUser)
        {
            try
            {
                if (SecUser != null)
                {
                    db.SecUsers.AddOrUpdate(SecUser);
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

        public bool IsUserExist(SecUser secUser)
        {
            try
            {
                bool isExist = false;
                var user = db.SecUsers.Where(x => x.username == secUser.username && x.UsersID != secUser.UsersID).FirstOrDefault();
                if (user != null)
                {
                    isExist = true;
                }
                return isExist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Reports
        public List<spRptGeneralLedger_Result> GeneralLedgerReport(int? CompanyID,DateTime? StartDate, DateTime? EndDate, string GLAccountNoStart, string GLAccountNoEnd)
        {
            try
            {
                var GeneralLedgerReportData = db.spRptGeneralLedger(CompanyID, StartDate, EndDate, GLAccountNoStart, GLAccountNoEnd).ToList();

                return GeneralLedgerReportData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region INProject

        public bool INProjectPost(int id)
        {
            try
            {
                var INProject = db.INProjects.Where(x => x.ProjectID == id).FirstOrDefault();
                INProject.Posted = true;
                db.INProjects.AddOrUpdate(INProject);
                db.SaveChanges();

                // insert project items 
                var items = db.INItems.ToList();
                var projectItems = new List<INProjectItem>();
                
                foreach(var item in items)
                {
                    var projectItem = new INProjectItem();
                    projectItem.ProjectID = INProject.ProjectID;
                    projectItem.ItemID = item.ItemID;
                    projectItem.OpeningQty = 0;
                    projectItem.QtyInHand = 0;                    
                    projectItem.LastRate = 0;
                    projectItem.CompanyID = INProject.CompanyID;

                    projectItems.Add(projectItem);
                }
                
                db.INProjectItems.AddRange(projectItems);
                db.SaveChanges ();
                
                //

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<INProject> INProjectList()
        {
            try
            {
                var INProjectList = db.INProjects.ToList();
                return INProjectList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public INProject INProjectGet(Int64 INProjectID)
        {
            try
            {
                var INProject = db.INProjects.Where(x => x.ProjectID == INProjectID).FirstOrDefault();
                return INProject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool INProjectDelete(Int64 INProjectID)
        {
            try
            {
                var INProject = db.INProjects.Where(x => x.ProjectID == INProjectID).FirstOrDefault();
                db.INProjects.Remove(INProject);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool INProjectSave(INProject INProject)
        {
            try
            {
                if (INProject.ProjectID == 0)
                {
                    INProject.ProjectID = db.INProjects.Max(x => x.ProjectID) + 1;
                }

                if (INProject != null)
                {
                    db.INProjects.AddOrUpdate(INProject);
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
        #endregion


    }
}
