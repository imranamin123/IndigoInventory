IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINPendingPOList')
	DROP PROC spRptINPendingPOList
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 16-Feb-2026
-- Description:	
-- ==========================================================================================

CREATE PROCEDURE spRptINPendingPOList
	@CompanyID int,
	@ProjectID int,
	@FromDate date = NULL,
	@ToDate date = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		c.Name 'Company',
		grn.GoodsReceiptNoteID,
		grn.GoodsReceiptNotesDate,
		p.ProjectName,
		v.APVendorName, 
		CASE
			WHEN ISNULL(grn.PurchaseOrderID,0) > 0 THEN
				'Approved'
			ELSE 
				'Pending'
		END Status,
		grp.Name AS [Group], 
		cat.Name AS Category, 
		grnd.ItemID, 
		i.Description AS ItemName, 
		s.Name AS Size, 
		uom.Name AS UOM,
		grnd.ReceivedQty,
		grnd.Rate,
		grnd.Amount

	FROM INGoodsReceiptNote grn
		INNER JOIN INGoodsReceiptNoteDetail grnd ON grn.GoodsReceiptNoteID = grnd.GoodsReceiptNoteID
		INNER JOIN INItem i ON grnd.ItemID = i.ItemID 
		INNER JOIN Company c ON grn.CompanyID = c.CompanyID 
		INNER JOIN INProject p ON grn.ProjectID = p.ProjectID
		INNER JOIN INCategory cat ON i.CategoryID = cat.CategoryID 
		LEFT JOIN INGroup grp ON cat.GroupID = grp.GroupID 
		LEFT JOIN INSize s ON i.SizeID = s.SizeID 
		LEFT JOIN INUnitOfMeasurement uom ON i.UOMID = uom.UOMID 
		LEFT JOIN APVendor v ON grn.APVendorID = v.APVendorID

	WHERE grn.CompanyID = @CompanyID AND
		grn.ProjectID = @ProjectID AND
		(CAST(grn.GoodsReceiptNotesDate as date) >= @FromDate OR @FromDate IS NULL) AND
		(CAST(grn.GoodsReceiptNotesDate as date) <= @ToDate OR @ToDate IS NULL)
	ORDER BY grn.GoodsReceiptNotesDate
END

go

exec spRptINPendingPOList 1,2
