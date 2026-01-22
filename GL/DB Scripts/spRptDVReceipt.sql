	

IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptDVReceipt')
	DROP PROC spRptDVReceipt
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 06-Sep-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spRptDVReceipt]
	@DVReceiptID int
AS
BEGIN
	SET NOCOUNT ON;	
DECLARE @Applicant1 varchar(500)
DECLARE @Applicant2 varchar(500)
declare @Applicant varchar(500)

select top 1 @Applicant1 = Name from DVApplicant where ApplicationFormID = (select ApplicationFormID from DVReceipt where DVReceiptID = @DVReceiptID )
order by ApplicantID


select top 1 @Applicant2 = Name from DVApplicant where ApplicationFormID = (select ApplicationFormID from DVReceipt where DVReceiptID = @DVReceiptID )
order by ApplicantID desc 


IF @Applicant1 = @Applicant2
begin
	SET @Applicant = @Applicant1
end
ELSE
begin
	SET @Applicant = @Applicant1 + ' / ' + @Applicant2
end

	
	SELECT
		c.Name 'Company',
		f.ApplicationFormID,
		ISNULL(r.DVReceiptID,0) ReceiptID,
		prj.ProjectName,
		@Applicant 'Applicant',
		f.ApplicationFormNo,
		ISNULL(f.ApplicationFormDate,getdate()) ApplicationFormDate,
		u.UnitNo,
		ut.UnitTypeName,
		u.FloorNo,
		ISNULL(u.SQFT,0) SQFT,
		ISNULL(f.Price,0) Price,
		ISNULL((u.SQFT * f.Price),0) UnitAmount,
		ISNULL(f.TokenMoney,0) TokenMoney,		
		ISNULL(r.DVReceiptDate,0) DVReceiptDate,
		ISNULL(r.Varified,0) Varified,
		ISNULL(r.VarifiedBy,0) VarifiedBy,		
		uvarifiedBy.Name 'VarifiedByName',
		ISNULL(r.Locked,0) Locked,
		ISNULL(r.LockedBy,0) LockedBy,
		ulockedBy.Name 'LockedByName',
		d.DVReceiptDetailID,
		ISNULL(d.DVReceiptID,0) DVReceiptID,
		t.PaymentPlanTypeCode,
		t.PaymentPlanTypeDesc,
		m.DVPaymentMethodDesc,
		ISNULL(d.DVPaymentMethodID,0) DVPaymentMethodID,
		d.Narration,
		ISNULL(d.Amount,0) Amount,
		ISNULL(d.PaymentPlanTypeID,0) PaymentPlanTypeID
	FROM DVReceipt r
		left JOIN DVReceiptDetail d ON r.DVReceiptID = d.DVReceiptID
		INNER JOIN DVApplicationForm f ON r.ApplicationFormID = f.ApplicationFormID
		INNER JOIN DVPaymentPlanType t ON d.PaymentPlanTypeID = t.PaymentPlanTypeID
		INNER JOIN DVPaymentMethod m ON d.DVPaymentMethodID = m.DVPaymentMethodID
		left JOIN SecUsers ulockedBy ON r.LockedBy = ulockedBy.UsersID 
		left JOIN SecUsers uvarifiedBy ON r.VarifiedBy = uvarifiedBy.UsersID
		INNER JOIN DVUnit u ON f.UnitID = u.UnitID
		INNER JOIN DVUnitType ut ON u.UnitTypeID = ut.UnitTypeID
		INNER JOIN DVProject prj ON f.ProjectID = prj.ProjectID

		INNER JOIN Company c  ON r.CompanyID = c.CompanyID
	WHERE r.DVReceiptID = @DVReceiptID

END

GO


exec spRptDVReceipt 192

