IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptMemberPaymentPlanStatus_PaymentPlan')
	DROP PROC spRptMemberPaymentPlanStatus_PaymentPlan
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 25-Sep-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spRptMemberPaymentPlanStatus_PaymentPlan]
	@ProjectID int,
	@UnitID int,
	@StatusDate date=null
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ApplicationFormID int
	DECLARE @PaymentPlanID INT

	SELECT @ApplicationFormID = ApplicationFormID FROM DVApplicationForm WHERE ProjectID = @ProjectID AND UnitID = @UnitID
	SELECT @PaymentPlanID = PaymentPlanID FROM DVPaymentPlan WHERE @ApplicationFormID = @ApplicationFormID
	SELECT 
		f.ApplicationFormID,
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
		c.Name 'Company',
		f.UnitID,
		pt.PaymentPlanTypeDesc 'PaymentPlanType', 
		pd.PaymentPlanTypeID,
		pd.PaymentPlanDetailDate,
		pd.Amount 'DueAmount',
		0.00 'ReceivedAmount',
		0.00 'Balance'
	FROM 
		DVPaymentPlanDetail pd
			INNER JOIN DVPaymentPlan pm	ON pd.PaymentPlanID = pm.PaymentPlanID
			INNER JOIN DVApplicationForm f ON pm.ApplicationFormID = f.ApplicationFormID
			INNER JOIN DVPaymentPlanType pt ON pd.PaymentPlanTypeID = pt.PaymentPlanTypeID
			INNER JOIN Company c ON pm.CompanyID = c.CompanyID
			INNER JOIN DVUnit u ON f.UnitID = u.UnitID
			INNER JOIN DVUnitType ut ON u.UnitTypeID = ut.UnitTypeID
			INNER JOIN DVProject prj ON f.ProjectID = prj.ProjectID
	WHERE 
		f.ApplicationFormID = @ApplicationFormID AND
		(pd.PaymentPlanDetailDate <= @StatusDate OR @StatusDate IS NULL)
	ORDER BY 
		pd.PaymentPlanTypeID, 
		pd.PaymentPlanDetailDate

END

GO

spRptMemberPaymentPlanStatus_PaymentPlan 1, 69, '2025-1-2'



