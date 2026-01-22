use GL
go

IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDVPaymentPlanDetail')
	DROP PROC spDVPaymentPlanDetail
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
CREATE PROCEDURE spDVPaymentPlanDetail
	@PaymentPlanID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		d.PaymentPlanDetailID,
		d.PaymentPlanID,
		d.PaymentPlanDetailDate,
		d.Percentage,
		d.Amount,
		d.PaymentPlanTypeID,
		t.PaymentPlanTypeCode,
		t.PaymentPlanTypeDesc,
		d.CreatedBy,
		d.CreatedAt,
		d.ModifiedBy,
		d.ModifiedAt,
		d.CompanyID
	FROM DVPaymentPlanDetail d
		INNER JOIN DVPaymentPlanType t ON d.PaymentPlanTypeID = t.PaymentPlanTypeID
	WHERE 
		d.PaymentPlanID = @PaymentPlanID
	ORDER BY 
		d.PaymentPlanDetailDate,
		d.PaymentPlanID
END
GO

exec spDVPaymentPlanDetail 1

select * from DVPaymentPlanType
select * from DVPaymentPlanDetail
