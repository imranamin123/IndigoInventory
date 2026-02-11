IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptVendorList')
	DROP PROC spRptVendorList
go


-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 11-Feb-2026
-- Description:	
-- ==========================================================================================

CREATE procedure spRptVendorList
	@CompanyID int,
	@FromDate date = NULL,
	@ToDate date = NULL
AS
BEGIN

	SELECT 
		c.Name 'Company',
		v.APVendorID,
		v.APVendorName,
		v.ContactPerson,
		v.BankDetails,
		cat.APVendorCategoryName,
		v.ContactNumber,
		v.Email,
		v.Address,
		v.CreatedAt
						 
	FROM 
		APVendor v INNER JOIN 
		APVendorCategory cat ON v.APVendorCategoryID = cat.APVendorCategoryID INNER JOIN
		Company c ON v.CompanyID = c.CompanyID

	WHERE 
		v.CompanyID = @CompanyID AND 
		(CAST(v.CreatedAt as date) >= @FromDate OR @FromDate IS NULL) AND
		(CAST(v.CreatedAt as date) <= @ToDate OR @ToDate IS NULL)
ORDER BY 
	v.CreatedAt	

END

go

exec spRptVendorList 1



