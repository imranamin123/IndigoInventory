IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINStoreIssueNoteDetailRows')
	DROP PROC spINStoreIssueNoteDetailRows
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 12-Jun-2025
-- Description:	
-- ==========================================================================================

CREATE procedure spINStoreIssueNoteDetailRows
	@StoreIssueNoteID int
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		d.StoreIssueNoteDetailID,
		d.StoreIssueNoteID,
		d.ItemID,
		i.ItemCode,
		i.Description 'Item',
		s.Name 'Size',
		u.Name 'Unit',		  
		d.IssuedQty,
		d.CreatedBy,
		d.CreatedAt,
		d.ModifiedBy,
		d.ModifiedAt,
		d.CompanyID
	FROM 
		INStoreIssueNoteDetail d
		INNER JOIN INItem i ON d.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
	WHERE 

		d.StoreIssueNoteID = @StoreIssueNoteID
END

go

exec spINStoreIssueNoteDetailRows 1








