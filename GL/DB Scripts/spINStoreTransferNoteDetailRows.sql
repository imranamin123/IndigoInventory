IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINStoreTransferNoteDetailRows')
	DROP PROC spINStoreTransferNoteDetailRows
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 30-Apr-2025
-- Description:	
-- ==========================================================================================

CREATE procedure spINStoreTransferNoteDetailRows
	@StoreTransferNoteID int
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		stnd.StoreTransferNoteDetailID,
		stnd.StoreTransferNoteID,
		stnd.RequestDetailID,
		stnd.ItemID,
		--cast(stnd.RequestDetailID as varchar) + '-' + cast(stnd.ItemID as varchar) 'ItemID',
		i.Description 'Item',
		s.Name 'Size',
		u.Name 'Unit',	
		stnd.QtyInHand,
		stnd.RequestedQty,
		stnd.TransferQty,	  
		stnd.Remarks,
		stnd.CreatedBy,
		stnd.CreatedAt,
		stnd.ModifiedBy,
		stnd.ModifiedAt,
		stnd.CompanyID
	FROM 
		INStoreTransferNoteDetail stnd
		INNER JOIN INItem i ON stnd.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
	WHERE 

		stnd.StoreTransferNoteID = @StoreTransferNoteID
END

go

exec spINStoreTransferNoteDetailRows 1

select * from INStoreTransferNote

select * from INStoreTransferNoteDetail


