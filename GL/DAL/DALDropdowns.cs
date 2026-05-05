using GL.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace GL.DAL
{
    public class DALDropdowns
    {
        GLEntities db = new GLEntities();

        public List<BKBankTransType> BKBankTransTypeList()
        {
            var BKBankTransTypesList = db.BKBankTransTypes.ToList();
            return BKBankTransTypesList;
        }

        public List<spINGRNItemsForPODropdown_Result> GetINGRNItemsForPODropdown(Int64? GoodsReceiptNoteID)
        {
            try
            {
                var INGRNItemsForPODropdown = db.spINGRNItemsForPODropdown(GoodsReceiptNoteID).ToList();
                return INGRNItemsForPODropdown;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINGRNItemsDropdown_Result> GetINGRItemsDropdown(int CompanyID, int ProjectID)
        {
            try
            {
                var INGRNItemsDropdown = db.spINGRNItemsDropdown(CompanyID,ProjectID).ToList();
                return INGRNItemsDropdown;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINPRItemsDropdown_Result> GetINPRItemsDropdown(long RequestID)
        {
            try
            {
                var INPRItemsDropdown = db.spINPRItemsDropdown(RequestID).ToList();
                return INPRItemsDropdown;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINStoreIssueNoteItemsDropdown_Result> GetINStoreIssueNoteItemsDropdown(int ProjectID, int CompanyID)
        {
            try
            {
                var INStoreIssueNoteItemsDropdown = db.spINStoreIssueNoteItemsDropdown(ProjectID, CompanyID).Where(x => x.Balance > 0 ).ToList();
                return INStoreIssueNoteItemsDropdown;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINItemsDropdown_Result> GetINItemsDropdown(int CompanyID)
        {
            try
            {
                var INItemsDropdown = db.spINItemsDropdown(CompanyID).ToList();
                return INItemsDropdown;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spINStoreIssueNoteItemsDropdown_Result> StoreIssueNoteItemsDropdownItemsDropdown(int ProjectID, int CompanyID)
        {
            try
            {
                var INStoreIssueNoteItemsDropdown = db.spINStoreIssueNoteItemsDropdown(ProjectID,CompanyID).ToList();
                return INStoreIssueNoteItemsDropdown;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<APVendorCategory> GetAPVendorCategoryDropdown(int CompanyID)
        {
            try
            {
                var APVendorCategoryDropdown = db.APVendorCategories.Where(x => x.CompanyID == CompanyID).ToList();// db.spINGRNItemsDropdown(CompanyID).ToList();
                return APVendorCategoryDropdown;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<INItem> GetINGRItemsAllDropdown(int CompanyID)
        {
            try
            {
                var INGRNItemsAllDropdown = db.INItems.Where(x => x.CompanyID == CompanyID).ToList();// db.spINGRNItemsDropdown(CompanyID).ToList();
                return INGRNItemsAllDropdown;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DVArrangementType> GetDVArrangementTypesDropdown(int CompanyID)
        {
            try
            {
                var DVArrangementTypes = db.DVArrangementTypes.Where(x => x.CompanyID == CompanyID).ToList();// db.spINGRNItemsDropdown(CompanyID).ToList();
                return DVArrangementTypes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<INCategory> INCategoryList(int GroupID)
        {
            var CategoryList = db.INCategories.Where(x => x.GroupID == GroupID).ToList();
            return CategoryList;
        }

        public List<INGroup> INGroupList (int CompanyID)
        {
            var INGroupList = db.INGroups.Where(x => x.CompanyID == CompanyID).OrderBy(x => x.Name).ToList();
            return INGroupList;
        }

        public List<INSize> INSizeList(int CompanyID)
        {
            var INSizeList = db.INSizes.Where(x => x.CompanyID == CompanyID).ToList();
            return INSizeList;
        }

        public List<INUnitOfMeasurement> INUnitOfMeasurementList(int CompanyID)
        {
            var INUnitOfMeasurementList = db.INUnitOfMeasurements.Where(x => x.CompanyID == CompanyID).ToList();
            return INUnitOfMeasurementList;
        }

        
        public List<SecUser> UserList(int CompanyID)
        {
            var UserList = db.SecUsers.Where(x => x.CompanyID == CompanyID).ToList();
            return UserList;
        }

        public List<DVPaymentPlanType> DVPaymentPlanTypeList(int CompanyID)
        {
            var PaymentPlanTypesList = db.DVPaymentPlanTypes.Where(x => x.CompanyID == CompanyID).ToList();
            return PaymentPlanTypesList;
        }

        public List<DVDocumentType> DVDocumentTypeList(int CompanyID)
        {
            var DVDocumentTypes = db.DVDocumentTypes.Where(x => x.CompanyID == CompanyID).ToList();
            return DVDocumentTypes;
        }

        public List<DVPaymentMethod> DVPaymentMethodList(int CompanyID)
        {
            var DVPaymentMethodList = db.DVPaymentMethods.Where(x => x.CompanyID == CompanyID).ToList();
            return DVPaymentMethodList;
        }

        public List<DVPaymentPlanType> DVPaymentPlanDetailsList(int CompanyID)
        {
            var DVPaymentPlanDetailsList = db.DVPaymentPlanTypes.Where(x => x.CompanyID == CompanyID).ToList();
            return DVPaymentPlanDetailsList;
        }


        public List<BKBank> BKBankList(int CompanyID)
        {
            var BKBanksList = db.BKBanks.Where(x => x.CompanyID == CompanyID).OrderBy(x => x.BankName).ToList();
            return BKBanksList;
        }

        public List<DVProject> DVProjectsList(int CompanyID)
        {
            var DVProjectsList = db.DVProjects.Where(x => x.CompanyID == CompanyID).ToList();
            return DVProjectsList;
        }


        public List<INProject> INProjectsList(int CompanyID)
        {
            var ProjectsList = db.INProjects.Where(x => x.CompanyID == CompanyID).ToList();
            return ProjectsList;
        }

        public List<INRequestType> INRequestTypesList(int CompanyID)
        {
            var INRequestTypeList = db.INRequestTypes.Where(x => x.CompanyID == CompanyID).ToList();
            return INRequestTypeList;
        }

        public List<INPOStatu> INPOStatusList(int CompanyID)
        {
            var INPOStatusList = db.INPOStatus.Where(x => x.CompanyID == CompanyID).ToList();
            return INPOStatusList;
        }

        public List<spINItemsDropdown_Result> INItemsList(int CompanyID)
        {
            var INItemsList = db.spINItemsDropdown(CompanyID).ToList();// db.INItems.Where(x => x.CompanyID == CompanyID).ToList();
            return INItemsList;
        }

        public List<INStore> INStoreList(int CompanyID)
        {
            var INStoreList = db.INStores.Where(x => x.CompanyID == CompanyID).ToList();
            return INStoreList;
        }

        public List<DVUnit> DVUnitsList(spDVGetApplicationForm_Result applicationForm)
        {
            var units = db.DVUnits.Where(x => x.ProjectID == applicationForm.ProjectID && x.IsActive == true || x.UnitID == applicationForm.UnitID).ToList();
            return units;
        }

        public List<GL.Models.Unit> DVUnitsListByProjectID(int ProjectID, bool AllUnits)
        {
           List< GL.Models.Unit> units = null;
            //IncludeAll == true will get all the units to get the report otherwise for entry screen 
            if (AllUnits == false)
            {
                //units = db.spDVUnitsListByProjectID(ProjectID, AllUnits).ToList();
                units = db.DVUnits.Where(x => x.ProjectID == ProjectID && x.IsActive == true && x.Cancelled == false && x.Onhold == false).Select(x => new GL.Models.Unit { UnitID = x.UnitID, UnitNo = x.UnitNo }).OrderBy(x => x.UnitNo).ToList();
            }
            else
            {
                units = db.DVUnits.Where(x => x.ProjectID == ProjectID).Select(x => new GL.Models.Unit { UnitID = x.UnitID, UnitNo = x.UnitNo }).OrderBy(x => x.UnitNo).ToList();
            }

            return units;
        }


        public bool CancelUnit(int UnitID)
        {
            var result = false;
            var unit = db.DVUnits.Where(x => x.UnitID == UnitID).FirstOrDefault();
            if (unit != null)
            {

                var newUnit = new DVUnit()
                {
                    UnitID = 0,
                    Cancelled = false,
                    CompanyID = unit.CompanyID,
                    FloorNo = unit.FloorNo,
                    IsActive = true,
                    SQFT = unit.SQFT,
                    ProjectID = unit.ProjectID,
                    UnitNo = unit.UnitNo ,
                    UnitTypeID = unit.UnitTypeID
                };

                db.DVUnits.AddOrUpdate(newUnit);

                unit.Cancelled = true;
                unit.UnitNo += " [Cancelled]";
                db.DVUnits.AddOrUpdate(unit);

                db.SaveChanges();
                result = true;
            }

            return result;
        }


        public List<GL.Models.Unit> DVOccupiedUnitsListByProjectID(int ProjectID, bool IsOccupied)
        {
            List<GL.Models.Unit> units = null;
            if (IsOccupied == true)
            {
                units = db.DVUnits.Where(x => x.ProjectID == ProjectID && x.IsActive == false).Select(x => new GL.Models.Unit { UnitID = x.UnitID, UnitNo = x.UnitNo }).OrderBy(x => x.UnitNo).ToList();
            }
            else
            {
                units = db.DVUnits.Where(x => x.ProjectID == ProjectID && x.IsActive == true).Select(x => new GL.Models.Unit { UnitID = x.UnitID, UnitNo = x.UnitNo }).OrderBy(x => x.UnitNo).ToList();
            }

            return units;
        }

        public List<GL.Models.Unit> DVUnitsListByProjectID(int ProjectID, bool AllUnits, int unitID)
        {
            List<GL.Models.Unit> units = null;
            //IncludeAll == true will get all the units to get the report otherwise for entry screen 
            if (AllUnits == false)
            {
                //units = db.spDVUnitsListByProjectID(ProjectID, AllUnits).ToList();
                units = db.DVUnits.Where(x => (x.ProjectID == ProjectID && x.IsActive == true && x.Cancelled == false) || x.UnitID == unitID).Select(x => new GL.Models.Unit { UnitID = x.UnitID, UnitNo = x.UnitNo }).OrderBy(x => x.UnitNo).ToList();
            }
            else
            {
                units = db.DVUnits.Where(x => x.ProjectID == ProjectID).Select(x => new GL.Models.Unit { UnitID = x.UnitID, UnitNo = x.UnitNo }).OrderBy(x => x.UnitNo).ToList();
            }

            return units;
        }


        public List<DVUnitType> DVUnitTypeList(int CompanyID)
        {
            var DVUnitList = db.DVUnitTypes.Where(x => x.CompanyID == CompanyID).ToList();
            return DVUnitList;
        }

        public List<spDVUnitsRows_Result> GetDVUnitsRows(int unitID)
        {
            try
            {
                List<spDVUnitsRows_Result> DVUnitsRowsList = null;
                DVUnitsRowsList = db.spDVUnitsRows(unitID).ToList();

                return DVUnitsRowsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      

        public List<Gender> GenderList()
        {
            var GenderList = db.Genders.ToList();
            return GenderList;
        }

        public List<Province> ProvinceList()
        {
            var ProvinceList = db.Provinces.ToList();
            return ProvinceList;
        }


        public List<Company> CompanyList()
        {
            var CompanyList = db.Companies.Where(x => x.CompanyID != 4).ToList();
            return CompanyList;
        }

        public List<SecUser> UsersList(int CompanyID)
        {
            var UsersList = db.SecUsers.Where(x => x.CompanyID == CompanyID).ToList();
            return UsersList;
        }

        public List<StatusType> StatusTypeList()
        {
            var StatusTypeList = db.StatusTypes.ToList();
            return StatusTypeList;
        }

        public List<BSClassificationType> BSClassificationTypeList(int CompanyID)
        {
            var BSClassificationTypeList = db.BSClassificationTypes.Where(x => x.CompanyID == CompanyID).ToList();
            return BSClassificationTypeList;
        }

        public List<VoucherType> VoucherTypeList(int CompanyID)
        {
            var VoucherTypeList = db.VoucherTypes.Where(x => x.CompanyID == CompanyID).ToList();
            return VoucherTypeList;
        }

        public List<APVendor> APVendorList(int CompanyID)
        {
            var APVendorsList = db.APVendors.Where(x => x.CompanyID == CompanyID).ToList();
            return APVendorsList;
        }

        public List<GLAccount> GLAccountList(int CompanyID)
        {
            var GLAccountList = db.GLAccounts.Where(x => x.CompanyID == CompanyID).OrderBy(x => x.GLAccountNo).ToList();
            return GLAccountList;
        }

        public List<spGLAccountsForDropdown_Result> GLAccountGetForDropdown(int CompanyID)
        {
            try
            {
                var GLAccount = db.spGLAccountsForDropdown(CompanyID).ToList();// db.GLAccounts.Where(x => x.GLAccountID == GLAccountID).FirstOrDefault();
                return GLAccount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spAPVendorsListWithCode_Result> GLAPVendorsListWithCode(int CompanyID)
        {
            try
            {
                var APVendors = db.spAPVendorsListWithCode(CompanyID).ToList();
                return APVendors;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<spFiscalYearSetupGetList_Result> FiscalYearGetForDropdown(int CompanyID)
        {
            try
            {
                var model = db.spFiscalYearSetupGetList(CompanyID).ToList();    
                return model;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SecRole> RolesList()
        {
            var RollsList = db.SecRoles.ToList();
            return RollsList;
        }

        public List<Month> MonthList()
        {
            var MonthsList = db.Months.ToList();
            return MonthsList;
        }

    }
}