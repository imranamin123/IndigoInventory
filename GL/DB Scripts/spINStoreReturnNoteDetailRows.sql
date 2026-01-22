IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINStoreReturnNoteDetailRows')
	DROP PROC spINStoreReturnNoteDetailRows
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 15-Jan-2026
-- Description:	
-- ==========================================================================================

CREATE procedure spINStoreReturnNoteDetailRows
	@StoreReturnNoteID int
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		d.StoreReturnNoteDetailID,
		d.StoreReturnNoteID,
		d.ItemID,
		i.ItemCode,
		i.Description 'Item',
		s.Name 'Size',
		u.Name 'Unit',		  
		d.ReturnQty,
		d.CreatedBy,
		d.CreatedAt,
		d.ModifiedBy,
		d.ModifiedAt,
		d.CompanyID
	FROM 
		INStoreReturnNoteDetail d
		INNER JOIN INItem i ON d.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
	WHERE 

		d.StoreReturnNoteID = @StoreReturnNoteID
END

go

exec spINStoreReturnNoteDetailRows 1








