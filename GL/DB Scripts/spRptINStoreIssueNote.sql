IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINStoreIssueNote')
	DROP PROC spRptINStoreIssueNote
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 28-Apr-2025
-- Description:	
-- ==========================================================================================

CREATE procedure spRptINStoreIssueNote
	@StoreIssueNoteID int
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		c.Name 'Company',
		n.StoreIssueNoteID,
		n.StoreIssueNoteDate,
		p.ProjectName,
		st.StoreName,
		n.Remarks 'mRemarks',
		d.StoreIssueNoteDetailID,
		d.ItemID,
		i.Description 'Item',
		s.Name 'Size',
		uom.Name 'Unit',		  
		d.IssuedQty,
		u.Name 'IssuedBy'
		
	FROM INStoreIssueNote n
		INNER JOIN INStoreIssueNoteDetail d ON n.StoreIssueNoteID = d.StoreIssueNoteID
		INNER JOIN INItem i ON d.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement uom ON i.UOMID = uom.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
		INNER JOIN INProject p ON p.ProjectID = n.ProjectID
		INNER JOIN Company c ON n.CompanyID = c.CompanyID
		INNER JOIN INStore st on n.StoreID = st.StoreID
		INNER JOIN SecUsers u ON n.CreatedBy = u.UsersID
  WHERE 
	
	n.StoreIssueNoteID = @StoreIssueNoteID

END

go

exec spRptINStoreIssueNote 1

exec spRptINStoreIssueNote 1


