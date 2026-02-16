IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptPendingCompleteDemands')
	DROP PROC spRptPendingCompleteDemands
go


-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 12-Feb-2026
-- Description:	
-- ==========================================================================================

CREATE procedure spRptPendingCompleteDemands
	@ComapnyID int,
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
			pr.RequestID, 
			pr.RequestDate, 
			prd.RequestDetailID,
			prd.ItemID, 
			i.Description AS Item, 
			s.Name AS Size, 
			uom.Name AS UOM, 
			ISNULL(prd.RequestedQty,0) 'RequestedQty',
			ISNULL(prd.ApprovedQty,0) 'ApprovedQty',
			
			CASE
				WHEN prd.ApprovedQty = SUM(ISNULL(grnd.ReceivedQty,0)) THEN
					'Completed'
				WHEN SUM(ISNULL(grnd.ReceivedQty,0)) > 0 AND SUM(ISNULL(grnd.ReceivedQty,0)) < prd.ApprovedQty THEN
					'Incomplete'
				WHEN SUM(ISNULL(grnd.ReceivedQty,0)) = 0 THEN
					'Pending'
			END 'Status',

			SUM(ISNULL(grnd.ReceivedQty,0)) ReceivedQty
			

		FROM INPurchaseRequisition pr INNER JOIN 
			INPurchaseRequisitionDetail prd ON pr.RequestID = prd.RequestID	LEFT JOIN
			INGoodsReceiptNoteDetail grnd ON prd.RequestDetailID = grnd.RequestDetailID LEFT JOIN	
			INItem i ON prd.ItemID = i.ItemID INNER JOIN
			INCategory cat ON i.CategoryID = cat.CategoryID LEFT JOIN
			INGroup grp ON cat.GroupID = grp.GroupID LEFT JOIN
			INSize s ON i.SizeID = s.SizeID LEFT JOIN
			INUnitOfMeasurement uom ON i.UOMID = uom.UOMID INNER JOIN                          
			Company c ON pr.CompanyID = c.CompanyID INNER JOIN
			INProject p ON pr.ProjectID = p.ProjectID
		WHERE 
			pr.CompanyID = @ComapnyID AND
			pr.ProjectID = @ProjectID AND
			(CAST(pr.RequestDate as date) >= @FromDate OR @FromDate IS NULL) AND
			(CAST(pr.RequestDate as date) <= @ToDate OR @ToDate IS NULL)

		GROUP BY 
			c.Name, 
			p.ProjectName, 
			grp.Name, 
			cat.Name, 
			pr.RequestID, 
			pr.RequestDate, 
			prd.RequestDetailID,
			prd.ItemID, 
			i.Description, 
			s.Name, 
			uom.Name, 
			prd.RequestedQty,
			prd.ApprovedQty
		ORDER BY prd.RequestDetailID

END

GO

exec spRptPendingCompleteDemands 1,2 --, '2026-01-01','2026-02-20'


