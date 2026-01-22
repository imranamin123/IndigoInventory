IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINStoreTransferNote')
	DROP PROC spRptINStoreTransferNote
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 24-Sep-2025
-- Description:	
-- ==========================================================================================

CREATE procedure spRptINStoreTransferNote
	@StoreTransferNoteID int
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		c.Name 'Company',
		t.StoreTransferNoteID,
		t.StoreTransferNoteDate,
		pTo.ProjectName 'ToProject',
		pFr.ProjectName 'FromProject',
		t.Remarks 'MRemarks',
		d.StoreTransferNoteDetailID,
		d.ItemID,
		i.Description 'Item',
		s.Name 'Size',
		uom.Name 'Unit',		  
		d.TransferQty,
		d.Remarks,
		cr.Name 'CreatedBy',
		t.CreatedAt 'CreatedAt',
		ap.Name 'ApprovedBy',
		t.ApprovedByAt 'ApprovedByAt',
		rec.Name 'ReceivedBy',
		t.ReceivedByAt 'ReceivedByAt'
		
	FROM INStoreTransferNote t
		INNER JOIN INStoreTransferNoteDetail d ON t.StoreTransferNoteID = d.StoreTransferNoteID
		INNER JOIN INItem i ON d.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement uom ON i.UOMID = uom.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
		INNER JOIN DVProject pTo ON pTo.ProjectID = t.ToProjectID
		INNER JOIN DVProject pFr ON pFr.ProjectID = t.FromProjectID
		INNER JOIN Company c ON t.CompanyID = c.CompanyID
		LEFT JOIN SecUsers cr ON t.CreatedBy = cr.UsersID
		LEFT JOIN SecUsers ap ON t.ApprovedByID = ap.UsersID
		LEFT JOIN SecUsers rec ON t.ReceivedByID = rec.UsersID
  WHERE 
	
	t.StoreTransferNoteID = @StoreTransferNoteID

END

go

exec spRptINStoreTransferNote 11

--select * from INStoreTransferNote
--select * from INStoreTransferNoteDetail

--exec spRptINStoreTransferNote 1

--select * from INGroup
--select * from INCategory
--select * from INSize
--select * from INUnitOfMeasurement


