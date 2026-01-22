IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINGoodsReceiptNoteDetailRows')
	DROP PROC spINGoodsReceiptNoteDetailRows
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 30-Apr-2025
-- Description:	
-- ==========================================================================================

CREATE procedure spINGoodsReceiptNoteDetailRows
	@GoodsReceiptNoteID int
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		grn.GoodsReceiptNoteDetailID,
		grn.GoodsReceiptNoteID,
		grn.RequestDetailID,
		grn.ItemID,
		--cast(grn.RequestDetailID as varchar) + '-' + cast(grn.ItemID as varchar) 'ItemID',
		i.Description 'Item',
		s.Name 'Size',
		u.Name 'Unit',		  
		grn.ApprovedQty,
		grn.ReceivedQty,
		grn.RejectedQty,
		grn.Rate,
		grn.VAT,
		grn.Amount,
		grn.Remarks,
		grn.CreatedBy,
		grn.CreatedAt,
		grn.ModifiedBy,
		grn.ModifiedAt,
		grn.CompanyID
	FROM 
		INGoodsReceiptNoteDetail grn
		INNER JOIN INItem i ON grn.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
	WHERE 

		grn.GoodsReceiptNoteID = @GoodsReceiptNoteID
END

go

exec spINGoodsReceiptNoteDetailRows 3

select * from INPurchaseRequisition




