IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptMemberPaymentPlanStatus_PaymentPlan_AllUnits')
	DROP PROC spRptMemberPaymentPlanStatus_PaymentPlan_AllUnits
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
CREATE PROCEDURE [dbo].[spRptMemberPaymentPlanStatus_PaymentPlan_AllUnits]
	@ProjectID int,
	@StartDate date=null,
	@EndDate date=null
AS
BEGIN
	SET NOCOUNT ON;

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
		--f.ApplicationFormID = @ApplicationFormID AND
		f.ProjectID = @ProjectID AND
		(
			(pd.PaymentPlanDetailDate >= @StartDate AND pd.PaymentPlanDetailDate <= @EndDate) OR 
			(@StartDate IS NULL AND @EndDate IS NULL) OR
			(pd.PaymentPlanDetailDate >= @StartDate) OR 
			(pd.PaymentPlanDetailDate <= @EndDate) 
		)
	ORDER BY 
		f.ApplicationFormID
		--pd.PaymentPlanTypeID, 
		--pd.PaymentPlanDetailDate

END

GO

exec spRptMemberPaymentPlanStatus_PaymentPlan_AllUnits 1

