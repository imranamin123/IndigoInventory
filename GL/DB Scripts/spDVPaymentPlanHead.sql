use GL
go

IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDVPaymentPlanHead')
	DROP PROC spDVPaymentPlanHead
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 21-Aug-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spDVPaymentPlanHead]
	@ApplicationFormID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT distinct
		f.ApplicationFormID,
		ISNULL(p.PaymentPlanID,0) PaymentPlanID,
		prj.ProjectName,
		f.ApplicationFormNo,
		f.ApplicationFormDate,
		u.UnitNo,
		ut.UnitTypeName,
		u.FloorNo,
		u.SQFT,
		f.Price,
		(u.SQFT * f.Price) Amount,
		f.TokenMoney,		
		p.PaymentPlanDate,
		p.CreatedBy,
		p.CreatedAt,
		p.ModifiedBy,
		p.ModifiedAt,
		p.CompanyID
	from DVApplicationForm f
		left join DVPaymentPlan p ON p.ApplicationFormID = f.ApplicationFormID
		left join DVUnit u ON f.UnitID = u.UnitID
		left join DVUnitType ut ON u.UnitTypeID = ut.UnitTypeID
		left join DVProject prj ON f.ProjectID = prj.ProjectID
	WHERE f.ApplicationFormID = @ApplicationFormID

END

GO

exec spDVPaymentPlanHead 3


