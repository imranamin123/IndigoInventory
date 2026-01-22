using GL.EF;
using GL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace GL.DAL
{
    public class DALDevelopment
    {
        private GLEntities db = new GLEntities();

        #region DVApplicationForm


        public bool IsA4Duplicate(int companyID, int projectID, int applicationID, string A4)
        {
            try
            {
                var dvApplicationForm = db.DVApplicationForms.Where(x => x.CompanyID == companyID && x.ProjectID == projectID && x.A4 == A4 && x.ApplicationFormID != applicationID ).FirstOrDefault();
                if (dvApplicationForm == null) 
                { 
                    return false;
                }
                else
                {
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DVApplicationForm GetDVApplicationForm(int ApplicationFormID)
        {
            try
            {
                var DVApplicationForm = db.DVApplicationForms.Where(x => x.ApplicationFormID== ApplicationFormID).FirstOrDefault();
                return DVApplicationForm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public spDVGetApplicationForm_Result spDVApplicationForm(int? ApplicationFormID)
        {
            try
            {
                var spDVApplicationForm = db.spDVGetApplicationForm(ApplicationFormID).FirstOrDefault(); //db.DVApplicants.Where(x => x.ApplicationFormID == ApplicationFormID).ToList();
                return spDVApplicationForm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DVUnit DVUnitData(int UnitID)
        {
            var DVUnit = db.DVUnits.Where(x => x.UnitID == UnitID).FirstOrDefault();

            return DVUnit;
        }

        public bool DVApplicationFormSave(DVApplicationForm DVApplicationForm, short? DVArrangementTypeID)
        {
            try
            {

                if (DVApplicationForm != null)
                {
                    

                    var oldForm = db.DVApplicationForms.Find(DVApplicationForm.ApplicationFormID);
                    if (oldForm != null)
                    {
                        if(oldForm.UnitID != DVApplicationForm.UnitID)
                        {
                            var oldUnit = db.DVUnits.Find(oldForm.UnitID);
                            oldUnit.IsActive = true;
                        }
                    }

                    db.DVApplicationForms.AddOrUpdate(DVApplicationForm);                    

                    var unit = db.DVUnits.Where(x => x.UnitID == DVApplicationForm.UnitID).FirstOrDefault();
                    if (unit != null)
                    {
                        unit.IsActive = false;
                        unit.DVArrangementTypeID = DVArrangementTypeID;
                    }
                    db.SaveChanges();

                    //// ApplicationFormNo /////
                    var project = db.DVProjects.Where(x => x.ProjectID == DVApplicationForm.ProjectID).FirstOrDefault();
                    DVApplicationForm.ApplicationFormNo =  project.ProjectName + "-" + unit.UnitNo + "-" + DVApplicationForm.ApplicationFormID;
                    db.DVApplicationForms.AddOrUpdate(DVApplicationForm);
                    db.SaveChanges();

                    /// end ///





                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DVApplicantSave(DVApplicant DVApplicant)
        {
            try
            {

                if (DVApplicant != null)
                {
                    db.DVApplicants.AddOrUpdate(DVApplicant);

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

        public List<spDVApplicantRows_Result> GetDVApplicants(int? ApplicationFormID)
        {
            try
            {
                var ApplicationForm = db.spDVApplicantRows(ApplicationFormID).ToList();
                return ApplicationForm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DVDocument GetDVDocument(int DocumentID)
        {
            try
            {
                var DVDocument = db.DVDocuments.Where(x => x.DocumentID == DocumentID).FirstOrDefault();
                return DVDocument;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetApplicationFormID(int ProjectID, int UnitID)
        {
            try
            {
                var ApplicationFormID = db.DVApplicationForms.Where(x => x.ProjectID == ProjectID && x.UnitID == UnitID).FirstOrDefault().ApplicationFormID;
                return ApplicationFormID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        public List<spDVApplicationFormSearchList_Result> GetDVApplicationFormSearchList(SearchModel search)
        {
            try
            {
                List<spDVApplicationFormSearchList_Result> DVApplicationFormSearchList = db.spDVApplicationFormSearchList(search.CompanyID).ToList();

                return DVApplicationFormSearchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool applicantImagePathSave(int ApplicantID, string ImagePath)
        {
            try
            {
                var applicant = db.DVApplicants.Find(ApplicantID);
                if (applicant != null)
                {
                    applicant.ImagePath = ImagePath;
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


        public bool documentImagePathSave(int DocumentID, string ImagePath)
        {
            try
            {
                var document = db.DVDocuments.Find(DocumentID);
                if (document != null)
                {
                    document.DocumentPath = ImagePath;
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

        public bool DVDocumentSave(DVDocument DVDocument)
        {
            try
            {
                if (DVDocument != null)
                {

                    db.DVDocuments.AddOrUpdate(DVDocument);
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


        public List<spRptMemberPaymentPlanStatus_PaymentPlan_AllUnits_Result> GetRptMemberPaymentPlanStatus_PaymentPlan_AllUnits_Report(int ProjectID, DateTime? StartDate, DateTime? EndDate)
        {
            try
            {
                var MemberPaymentPlanStatus_PaymentPlan_AllUnits = db.spRptMemberPaymentPlanStatus_PaymentPlan_AllUnits(ProjectID, StartDate, EndDate).ToList();
                return MemberPaymentPlanStatus_PaymentPlan_AllUnits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spRptMemberPaymentPlanStatus_Receipt_AllUnits_Result> GetspRptMemberPaymentPlanStatus_Receipt_AllUnits(int ProjectID, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var MemberPaymentPlanStatus_Receipt_AllUnits = db.spRptMemberPaymentPlanStatus_Receipt_AllUnits(ProjectID,StartDate,EndDate).ToList();
                return MemberPaymentPlanStatus_Receipt_AllUnits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<spDocumentListByApplication_Result> GetDocumentsByApplication(int? ApplicationFormID)
        {
            try
            {
                var Documents = db.spDocumentListByApplication(ApplicationFormID).ToList();
                return Documents;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DVDocumentDelete(int DVDocumentID)
        {
            try
            {
                var result = true;
                var DVDocument = db.DVDocuments.Where(x => x.DocumentID == DVDocumentID).FirstOrDefault();
                if (DVDocument != null)
                {
                    db.DVDocuments.Remove(DVDocument);
                    db.SaveChanges();

                    if (System.IO.File.Exists(DVDocument.DocumentPath))
                    {
                        System.IO.File.Delete(DVDocument.DocumentPath);
                    }
                }
                else
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}