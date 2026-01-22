IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINPurchaseRequisitionHistoryData')
	DROP PROC spRptINPurchaseRequisitionHistoryData
go


-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 12-Dec-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spRptINPurchaseRequisitionHistoryData]
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
	
    grn.RequestDate, 
	grn.RequestID, 
	grnd.RequestDetailID,
	grnd.ItemID, 
	i.Description AS ItemDescription, 
	s.Name AS Size, 
	uom.Name AS UOM, 
	t.RequestTypeDesc,
    grnd.RequestedQty, 
	grnd.ApprovedQty,
	(SELECT SUM(ISNULL(ReceivedQty,0)) FROM INGoodsReceiptNoteDetail m WHERE m.RequestDetailID = grnd.RequestDetailID) ReceivedQty

						 
FROM INPurchaseRequisition grn  INNER JOIN
        INPurchaseRequisitionDetail grnd ON grn.RequestID = grnd.RequestID INNER JOIN
       -- APVendor v ON grn.APVendorID = v.APVendorID INNER JOIN
		INItem i ON grnd.ItemID = i.ItemID INNER JOIN
		INCategory cat ON i.CategoryID = cat.CategoryID LEFT JOIN
        INGroup grp ON cat.GroupID = grp.GroupID LEFT JOIN
        INSize s ON i.SizeID = s.SizeID LEFT JOIN
        INUnitOfMeasurement uom ON i.UOMID = uom.UOMID INNER JOIN                          
        Company c ON grn.CompanyID = c.CompanyID INNER JOIN
        INProject p ON grn.ProjectID = p.ProjectID INNER JOIN
		INRequestType t ON grn.RequestTypeID = t.RequestTypeID

WHERE 
	grn.ProjectID = @ProjectID AND
	(CAST(grn.RequestDate as date) >= @FromDate OR @FromDate IS NULL) AND
	(CAST(grn.RequestDate as date) <= @ToDate OR @ToDate IS NULL) 
--	and	grnd.RequestDetailID = 2742
ORDER BY p.ProjectName, [Group], Category, grn.RequestDate, ItemDescription, Size, UOM



END

go



exec spRptINPurchaseRequisitionHistoryData 1--, '2025-10-18', '2025-11-26'


