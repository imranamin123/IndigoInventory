
IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spDVOBillingDetailRows')
	DROP PROC spDVOBillingDetailRows
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 29-Nov-2024
-- Description:	
-- ==========================================================================================

create procedure dbo.spDVOBillingDetailRows
	@BillingID int
AS
BEGIN

	SET NOCOUNT ON;

	SELECT 
		d.BillingDetailID
      ,	d.BillingID
	  ,	d.PaymentPlanTypeID
	  , t.PaymentPlanTypeCode    
      ,	d.Amount
	  ,	d.Narration
      ,	d.CreatedAt
      ,	d.CreatedBy
      ,	d.ModifiedAt
      ,	d.ModifiedBy
      ,	d.CompanyID
	FROM	
		DVOBillingDetail d 
	INNER JOIN DVPaymentPlanType t ON d.PaymentPlanTypeID = t.PaymentPlanTypeID
	WHERE d.BillingID = @BillingID

END
GO

exec spDVOBillingDetailRows  1


