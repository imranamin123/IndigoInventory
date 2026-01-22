IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptMemberPaymentPlanStatus_Receipt')
	DROP PROC spRptMemberPaymentPlanStatus_Receipt
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
CREATE PROCEDURE [dbo].[spRptMemberPaymentPlanStatus_Receipt]
	@ProjectID int,
	@UnitID int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ApplicationFormID int
	DECLARE @PaymentPlanID INT

	SELECT @ApplicationFormID = ApplicationFormID FROM DVApplicationForm WHERE ProjectID = @ProjectID AND UnitID = @UnitID
	
	SELECT 	 
		d.PaymentPlanTypeID, 
		ISNULL(SUM(d.Amount),0) Amount
	FROM DVReceiptDetail d 
		INNER JOIN DVReceipt m ON d.DVReceiptID = m.DVReceiptID
	WHERE m.ApplicationFormID =  @ApplicationFormID
	group by d.PaymentPlanTypeID

END

GO

exec spRptMemberPaymentPlanStatus_Receipt  1,143
exec spRptMemberPaymentPlanStatus_PaymentPlan 1,143
