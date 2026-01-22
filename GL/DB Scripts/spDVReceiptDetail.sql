use GL
go

IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDVReceiptDetail')
	DROP PROC spDVReceiptDetail
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 30-Aug-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE spDVReceiptDetail
	@DVReceiptID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		d.DVReceiptDetailID,
		d.DVReceiptID,
		t.PaymentPlanTypeCode,
		d.DVPaymentMethodID,
		d.Narration,
		d.Amount,
		d.PaymentPlanTypeID,
		d.CreatedBy,
		ISNULL(d.CreatedAt,GETDATE()) CreatedAt,
		d.ModifiedBy,
		ISNULL(d.ModifiedAt,GETDATE()) ModifiedAt,
		d.CompanyID

	FROM DVReceiptDetail d
		left JOIN DVPaymentPlanType t ON d.PaymentPlanTypeID = t.PaymentPlanTypeID
	WHERE d.DVReceiptID = @DVReceiptID
END
GO

exec spDVReceiptDetail 6



