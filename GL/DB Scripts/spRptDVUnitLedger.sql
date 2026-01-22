
IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptDVUnitLedger')
	DROP PROC spRptDVUnitLedger
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 13-Sep-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spRptDVUnitLedger]
	@ProjectID int,
	@UnitID int,
	@ToDate date
AS
BEGIN
	SET NOCOUNT ON;

SELECT 

	f.ApplicationFormID,
	ap.Name 'Customer',
	RefNo,
	prj.ProjectName,
	f.ApplicationFormNo,
	ISNULL(f.ApplicationFormDate,getdate()) ApplicationFormDate,
	u.UnitNo,
	ut.UnitTypeName,
	u.FloorNo,
	ISNULL(u.SQFT,0) SQFT,
	ISNULL(f.Price,0) Price,
	ISNULL((u.SQFT * f.Price),0) UnitAmount,
	ISNULL(f.TokenMoney,0) TokenMoney,	
	GETDATE() LedgerDate,
	main.Company,
	main.UnitID,
	main.Date, 
	main.PaymentPlanTypeDesc,
	main.DVPaymentMethodDesc,	
	main.Narration, 
	ISNULL(main.Debit,0) 'Debit', 
	ISNULL(main.Credit,0) 'Credit',
	ISNULL(main.Debit,0) - ISNULL(main.Credit,0) Balance
FROM (
	SELECT 
		c.Name 'Company',
		f.ApplicationFormID,
		'' 'RefNo',
		f.UnitID,
		d.PaymentPlanDetailDate as 'Date', 
		t.PaymentPlanTypeDesc,
		'' DVPaymentMethodDesc,
		'' as 'Narration', 
		ISNULL(d.Amount,0) 'Debit', 
		0 'Credit'
	FROM DVPaymentPlanDetail d
		INNER JOIN DVPaymentPlan p ON d.PaymentPlanID = p.PaymentPlanID
		INNER JOIN DVApplicationForm f ON p.ApplicationFormID = f.ApplicationFormID
		INNER JOIN Company c ON p.CompanyID = c.CompanyID
		INNER JOIN DVPaymentPlanType t ON d.PaymentPlanTypeID = t.PaymentPlanTypeID
	WHERE 
		f.ProjectID = @ProjectID 
		AND	f.UnitID = @UnitID 
		AND d.PaymentPlanDetailDate <= @ToDate

	UNION ALL

	SELECT 
		c.Name 'Company',
		f.ApplicationFormID,
		CAST(r.DVReceiptID as varchar) 'RefNo',
		f.UnitID,
		r.DVReceiptDate as 'Date', 
		t.PaymentPlanTypeDesc,
		m.DVPaymentMethodDesc,
		d.Narration as 'Narration',
		CASE WHEN ISNULL(d.Amount,0) < 0 THEN  ABS(ISNULL(d.Amount,0)) END 'Debit', 
		CASE WHEN ISNULL(d.Amount,0) >= 0 THEN ISNULL(d.Amount,0) END 'Credit'
	FROM dbo.DVReceiptDetail d
		INNER JOIn DVReceipt r ON d.DVReceiptID = r.DVReceiptID
		INNER JOIN DVApplicationForm f ON r.ApplicationFormID = f.ApplicationFormID
		INNER JOIN Company c ON r.CompanyID = c.CompanyID
		INNER JOIN DVPaymentPlanType t ON d.PaymentPlanTypeID = t.PaymentPlanTypeID
		INNER JOIN DVPaymentMethod m ON d.DVPaymentMethodID = m.DVPaymentMethodID
	WHERE 
		f.ProjectID = @ProjectID 
		AND f.UnitID = @UnitID 
		AND r.DVReceiptDate <= @ToDate
		) main

		INNER JOIN DVApplicationForm f ON main.ApplicationFormID = f.ApplicationFormID
		OUTER APPLY (
			SELECT STRING_AGG(Name, ' / ') AS Name
			FROM DVApplicant
			WHERE ApplicationFormID = main.ApplicationFormID
		) ap
		INNER JOIN DVUnit u ON f.UnitID = u.UnitID
		INNER JOIN DVUnitType ut ON u.UnitTypeID = ut.UnitTypeID
		INNER JOIN DVProject prj ON f.ProjectID = prj.ProjectID
	ORDER BY 
		main.Date

END

GO

exec spRptDVUnitLedger 1, 86, '2025-07-30 00:00:00.000'








