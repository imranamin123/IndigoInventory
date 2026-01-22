IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINGoodsReceiptNoteHistoryData')
	DROP PROC spRptINGoodsReceiptNoteHistoryData
go


-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 11-Dec-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spRptINGoodsReceiptNoteHistoryData]
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
	grn.GoodsReceiptNoteID, 
    grn.GoodsReceiptNotesDate, 
	v.APVendorName,

	grnd.ItemID, 
	i.Description AS Item, 
	s.Name AS Size, 
	uom.Name AS UOM, 
    grnd.ApprovedQty, 
	grnd.RejectedQty, 
	grnd.ReceivedQty, 
	grnd.Rate, 
	grnd.Amount, 
    v.APVendorName,
	CASE 
		WHEN grn.PurchaseOrderID IS NULL THEN
			'Pending Approval'
		WHEN grn.PurchaseOrderID IS NOT NULL THEN
			'Approved'
		ELSE 
			'Undefined'
	END 'Status'
						 
FROM INGoodsReceiptNote grn  INNER JOIN
        INGoodsReceiptNoteDetail grnd ON grn.GoodsReceiptNoteID = grnd.GoodsReceiptNoteID INNER JOIN
        APVendor v ON grn.APVendorID = v.APVendorID INNER JOIN
		INItem i ON grnd.ItemID = i.ItemID INNER JOIN
		INCategory cat ON i.CategoryID = cat.CategoryID LEFT JOIN
        INGroup grp ON cat.GroupID = grp.GroupID LEFT JOIN
        INSize s ON i.SizeID = s.SizeID LEFT JOIN
        INUnitOfMeasurement uom ON i.UOMID = uom.UOMID INNER JOIN                          
        Company c ON grn.CompanyID = c.CompanyID INNER JOIN
        INProject p ON grn.ProjectID = p.ProjectID
WHERE 
	grn.ProjectID = @ProjectID AND
	(CAST(grn.GoodsReceiptNotesDate as date) >= @FromDate OR @FromDate IS NULL) AND
	(CAST(grn.GoodsReceiptNotesDate as date) <= @ToDate OR @ToDate IS NULL)
ORDER BY p.ProjectName, [Group], Category, grn.GoodsReceiptNotesDate, Item, Size, UOM



END

go



exec spRptINGoodsReceiptNoteHistoryData 2, '2025-11-25', '2025-11-26'

select * from INGoodsReceiptNote where ProjectID = 1
