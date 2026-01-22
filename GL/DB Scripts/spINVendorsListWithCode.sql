
IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINVendorsListWithCode')
	DROP PROC spINVendorsListWithCode
go
-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 06-Nov-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spINVendorsListWithCode]
	@CountryID int
AS
BEGIN
	SELECT 
		APVendorID, 
		APVendorCode + '    ' + APVendorName DisplayText 
	FROM
		APVendor
WHERE 
	CompanyID =  @CountryID
ORDER BY APVendorName
		
END

