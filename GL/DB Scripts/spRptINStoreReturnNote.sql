IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINStoreReturnNote')
	DROP PROC spRptINStoreReturnNote
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 16-Jan-2026
-- Description:	
-- ==========================================================================================

CREATE procedure spRptINStoreReturnNote
	@StoreReturnNoteID int
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		c.Name 'Company',
		n.StoreReturnNoteID,
		n.StoreReturnNoteDate,
		p.ProjectName,
		n.Remarks 'mRemarks',
		d.StoreReturnNoteDetailID,
		d.ItemID,
		i.Description 'Item',
		s.Name 'Size',
		uom.Name 'Unit',		  
		d.ReturnQty,
		u.Name 'ReturnBy',
		n.CreatedAt 'ReturnAt'
		
	FROM INStoreReturnNote n
		INNER JOIN INStoreReturnNoteDetail d ON n.StoreReturnNoteID = d.StoreReturnNoteID
		INNER JOIN INItem i ON d.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement uom ON i.UOMID = uom.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
		INNER JOIN INProject p ON p.ProjectID = n.ProjectID
		INNER JOIN Company c ON n.CompanyID = c.CompanyID
		INNER JOIN SecUsers u ON n.CreatedBy = u.UsersID
  WHERE 
	
	n.StoreReturnNoteID = @StoreReturnNoteID

END

go

exec spRptINStoreReturnNote 1



