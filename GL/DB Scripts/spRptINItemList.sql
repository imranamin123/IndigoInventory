IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINItemList')
	DROP PROC spRptINItemList
go


-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 12-Feb-2026
-- Description:	
-- ==========================================================================================

CREATE procedure spRptINItemList
	@ComapnyID int,
	@FromDate date = NULL,
	@ToDate date = NULL
AS
BEGIN


SELECT 
	c.Name 'Company',
	grp.Name AS 'Group', 
	cat.Name AS 'Category', 	
	i.ItemID,
	i.Description AS 'ItemDescription', 
	s.Name AS 'Size', 
	uom.Name AS 'UOM',
	i.CreatedAt 

FROM    INItem i INNER JOIN
		INCategory cat ON i.CategoryID = cat.CategoryID LEFT JOIN
        INGroup grp ON cat.GroupID = grp.GroupID LEFT JOIN
        INSize s ON i.SizeID = s.SizeID LEFT JOIN
        INUnitOfMeasurement uom ON i.UOMID = uom.UOMID INNER JOIN
		Company c ON i.CompanyID = c.CompanyID
WHERE 
	i.CompanyID = @ComapnyID AND
	(CAST(i.CreatedAt as date) >= @FromDate OR @FromDate IS NULL) AND
	(CAST(i.CreatedAt as date) <= @ToDate OR @ToDate IS NULL) 

ORDER BY 
	c.Name,
	grp.Name, 
	cat.Name,
	i.Description,
	s.Name, 
	uom.Name
END

go



exec spRptINItemList 1--, '2025-01-01', '2025-12-30'




