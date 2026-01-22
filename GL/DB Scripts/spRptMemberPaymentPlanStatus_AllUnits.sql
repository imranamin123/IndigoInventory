IF EXISTS
(
    SELECT *
    FROM sysObjects o
    WHERE o.[name] = 'spRptMemberPaymentPlanStatus_AllUnits'
)
    DROP PROC spRptMemberPaymentPlanStatus_AllUnits
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 5-Oct-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spRptMemberPaymentPlanStatus_AllUnits]
    @ProjectID int,
	@StatusDate date=null
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM
    (
        SELECT main.UnitNo,
               CASE main.PossessionStatus
                   WHEN 1 THEN
                       'Yes'
                   ELSE
                       'No'
               END PossessionStatus,
               main.Applicant,
               main.PaymentPlanTypeCode,
			   main.Company,
			   main.ProjectDescription,
               SUM(main.Amount) Amount
        FROM
        (
            SELECT dbo.DVUnit.UnitNo,
                   dbo.DVApplicationForm.PossessionStatus,
                   (
                       select top 1
                           ISNULL(Name, '')
                       from dbo.DVApplicant
                       where ApplicationFormID = dbo.DVApplicationForm.ApplicationFormID
                   ) Applicant,
                   dbo.DVPaymentPlanType.PaymentPlanTypeCode,
                   -dbo.DVReceiptDetail.Amount 'Amount',
				   dbo.Company.Name 'Company',
				   dbo.DVProject.ProjectDescription
            FROM dbo.DVReceiptDetail
                INNER JOIN dbo.DVReceipt
                    ON dbo.DVReceiptDetail.DVReceiptID = dbo.DVReceipt.DVReceiptID
                INNER JOIN dbo.DVApplicationForm
                    ON dbo.DVReceipt.ApplicationFormID = dbo.DVApplicationForm.ApplicationFormID
                INNER JOIN dbo.DVUnit
                    ON dbo.DVApplicationForm.UnitID = dbo.DVUnit.UnitID
                INNER JOIN dbo.DVPaymentPlanType
                    ON dbo.DVReceiptDetail.PaymentPlanTypeID = dbo.DVPaymentPlanType.PaymentPlanTypeID
				INNER JOIN dbo.Company ON dbo.DVReceiptDetail.CompanyID = dbo.Company.CompanyID
				INNER JOIN dbo.DVProject ON dbo.DVApplicationForm.ProjectID = dbo.DVProject.ProjectID
            WHERE dbo.DVApplicationForm.ProjectID = @ProjectID
                  AND dbo.DVReceipt.DVReceiptDate <= @StatusDate 

            UNION ALL

            SELECT dbo.DVUnit.UnitNo,
                   dbo.DVApplicationForm.PossessionStatus,
                   (
                       select top 1
                           ISNULL(Name, '')
                       from dbo.DVApplicant
                       where ApplicationFormID = dbo.DVApplicationForm.ApplicationFormID
                   ) Applicant,
                   dbo.DVPaymentPlanType.PaymentPlanTypeCode,
                   dbo.DVPaymentPlanDetail.Amount 'Amount',
				   dbo.Company.Name 'Company',
				   dbo.DVProject.ProjectDescription
            FROM dbo.DVPaymentPlan
                INNER JOIN dbo.DVPaymentPlanDetail
                    ON dbo.DVPaymentPlan.PaymentPlanID = dbo.DVPaymentPlanDetail.PaymentPlanID
                INNER JOIN dbo.DVApplicationForm
                    ON dbo.DVPaymentPlan.ApplicationFormID = dbo.DVApplicationForm.ApplicationFormID
                INNER JOIN dbo.DVUnit
                    ON dbo.DVApplicationForm.UnitID = dbo.DVUnit.UnitID
                INNER JOIN dbo.DVPaymentPlanType
                    ON dbo.DVPaymentPlanDetail.PaymentPlanTypeID = dbo.DVPaymentPlanType.PaymentPlanTypeID
				INNER JOIN dbo.Company ON dbo.DVPaymentPlan.CompanyID = dbo.Company.CompanyID
				INNER JOIN dbo.DVProject ON dbo.DVApplicationForm.ProjectID = dbo.DVProject.ProjectID
            WHERE dbo.DVApplicationForm.ProjectID = @ProjectID
                  AND dbo.DVPaymentPlanDetail.PaymentPlanDetailDate <= @StatusDate 
        ) main
        GROUP BY main.UnitNo,
                 main.PossessionStatus,
                 main.Applicant,
                 main.PaymentPlanTypeCode,
				 main.Company,
				 main.ProjectDescription

    ) AS SourceTable
    PIVOT
    (
        MAX(Amount)
        FOR PaymentPlanTypeCode IN ([TK], [DP], [IN], [PS], [AD], [EX], [MC])
    ) AS PivotTable;

END


go

exec spRptMemberPaymentPlanStatus_AllUnits 1, '2024-11-01'






