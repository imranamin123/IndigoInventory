IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINGoodsReceiptNote')
	DROP PROC spRptINGoodsReceiptNote
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 07-May-2025
-- Description:	
-- ==========================================================================================

CREATE procedure spRptINGoodsReceiptNote
	@GoodsReceiptNoteID bigint
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		grn.GoodsReceiptNoteID,
		grn.GoodsReceiptNotesDate,
		grn.ProjectID,
		p.ProjectName,
		grn.APVendorID,
		v.APVendorName,
		grn.DCINVNO,
		grn.DCINVDate,
		grn.IGPNO,
		grn.PurchaseOrderID,
		grn.VehicleNo,
		grn.Remarks 'mRemarks',
		grn.IsPosted,
		grnd.GoodsReceiptNoteDetailID,
		grnd.RequestDetailID,
		grnd.ItemID,
		i.Description 'Item',
		s.Name 'Size',
		uom.Name 'Unit',
		grnd.ApprovedQty,
		grnd.RejectedQty,
		grnd.ReceivedQty,
		grnd.Rate,
		grnd.VAT,
		grnd.Amount,
		grnd.Remarks 'dRemarks',
		c.Name 'Company',
		u.Name 'ReceivedBy'

	FROM INGoodsReceiptNote grn
		INNER JOIN INGoodsReceiptNoteDetail grnd ON grn.GoodsReceiptNoteID = grnd.GoodsReceiptNoteID
		INNER JOIN INItem i ON grnd.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement uom ON i.UOMID = uom.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
		INNER JOIN INProject p ON grn.ProjectID = p.ProjectID
		LEFT JOIN APVendor v ON grn.APVendorID = v.APVendorID
		INNER JOIN Company c ON grn.CompanyID = c.CompanyID
		INNER JOIN SecUsers u ON grn.CreatedBy = u.UsersID

	WHERE 
		grn.GoodsReceiptNoteID = @GoodsReceiptNoteID

END

go

exec spRptINGoodsReceiptNote 294

select * from INGoodsReceiptNote where purchaseOrderID is null

update INGoodsReceiptNote set purchaseOrderID = 0  where GoodsReceiptNoteID <> 294







