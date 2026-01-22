IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spAPInvoiceSearchList')
	DROP PROC spAPInvoiceSearchList
go


-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 12-Feb-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spAPInvoiceSearchList]
	@CompanyID int=null
AS
BEGIN

	SET	NOCOUNT ON;

	SELECT 
		inv.APInvoiceID, 
		inv.APInvoiceDate, 
		inv.APInvoiceAmount,
		v.APVendorName,
		ISNULL(inv.IsPosted,0) IsPosted
	FROM APInvoice inv 
		INNER JOIN APVendor v ON inv.APVendorID = v.APVendorID
	WHERE inv.CompanyID = @CompanyID 

END

go


spAPInvoiceSearchList 1
