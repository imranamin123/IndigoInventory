IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINStoreIssueNoteHistoryData')
	DROP PROC spRptINStoreIssueNoteHistoryData
go


-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 11-Feb-2026
-- Description:	
-- ==========================================================================================

CREATE procedure spRptINStoreIssueNoteHistoryData
	@CompanyID int,
	@ProjectID int,
	@FromDate date = NULL,
	@ToDate date = NULL
AS
BEGIN

	SELECT 
		c.Name AS Company, 
		p.ProjectName, 
		grp.Name AS [Group], 
		cat.Name AS Category, 
		sinnd.ItemID, 
		i.Description AS Item, 
		s.Name AS Size, 
		uom.Name AS UOM, 
		prji.LastRate,
		SUM(ISNULL(sinnd.IssuedQty,0)) 'IssuedQty' 	
						 
	FROM INStoreIssueNote sinn  INNER JOIN
			INStoreIssueNoteDetail sinnd ON sinn.StoreIssueNoteID = sinnd.StoreIssueNoteID INNER JOIN
			INItem i ON sinnd.ItemID = i.ItemID INNER JOIN
			INCategory cat ON i.CategoryID = cat.CategoryID LEFT JOIN
			INGroup grp ON cat.GroupID = grp.GroupID LEFT JOIN
			INSize s ON i.SizeID = s.SizeID LEFT JOIN
			INUnitOfMeasurement uom ON i.UOMID = uom.UOMID INNER JOIN                          
			Company c ON sinn.CompanyID = c.CompanyID INNER JOIN
			INProject p ON sinn.ProjectID = p.ProjectID INNER JOIN
			INProjectItem prji ON sinn.ProjectID = prji.ProjectID ANd sinnd.ItemID = prji.ItemID
	WHERE 
		sinn.CompanyID = @CompanyID AND 
		sinn.ProjectID = @ProjectID AND
		(CAST(sinn.StoreIssueNoteDate as date) >= @FromDate OR @FromDate IS NULL) AND
		(CAST(sinn.StoreIssueNoteDate as date) <= @ToDate OR @ToDate IS NULL)
	GROUP BY
		c.Name,
		p.ProjectName,
		grp.Name,
		cat.Name,
		sinnd.ItemID,
		i.Description,
		s.Name,
		uom.Name,
		prji.LastRate
ORDER BY 
	i.Description	
		--c.Name, p.ProjectName, [Group], Category, Item, Size, UOM
END

go

exec spRptINStoreIssueNoteHistoryData 1,1 --, '2025-11-25', '2025-11-26'



