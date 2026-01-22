IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spIntDVReceiptCash')
	DROP PROC spIntDVReceiptCash
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 24-Dec-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE spIntDVReceiptCash
	@DVReceiptID int
AS

BEGIN
	SET NOCOUNT ON;

	SELECT 
		r.DVReceiptID, 
		r.DVReceiptDate,
		d.PaymentPlanTypeID, 
		d.DVPaymentMethodID, 
		pm.A1,
		pm.A2, 
		pm.A3, 
		pm.A4, 
		pm.VoucherTypeID,
		vt.VTYPE,
		'' VoucherNumber
	
	FROM DVReceipt r
	INNER JOIN DVReceiptDetail d ON r.DVReceiptID = d.DVReceiptID
	INNER JOIN DVPaymentMethod pm ON d.DVPaymentMethodID = pm.DVPaymentMethodID
	INNER JOIN VoucherType vt ON pm.VoucherTypeID = vt.VoucherTypeID
	WHERE
		r.DVReceiptID = @DVReceiptID 
		--AND d.DVPaymentMethodID = 1 -- Cash
END

GO

exec spIntDVReceiptCash 414

