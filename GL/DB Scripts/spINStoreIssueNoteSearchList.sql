
IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINStoreIssueNoteSearchList')
	DROP PROC spINStoreIssueNoteSearchList
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 12-Jun-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spINStoreIssueNoteSearchList]
	@CompanyID int,
	@UserID int = NULL,
	@FromDate date = NULL,
	@ToDate date = NULL,
	@ItemID bigint = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT
		note.StoreIssueNoteID,
		note.StoreIssueNoteDate,
		p.ProjectName,
		note.Remarks,
		note.IsPosted,
		note.CompanyID
	FROM INStoreIssueNote note
	INNER JOIN INStoreIssueNoteDetail det ON note.StoreIssueNoteID = det.StoreIssueNoteID 
	INNER JOIN INProject p ON note.ProjectID = p.ProjectID

	WHERE note.CompanyID = @CompanyID AND
		(note.CreatedBy = @UserID OR @UserID IS NULL) AND
		(CAST(note.StoreIssueNoteDate as date) >= @FromDate OR @FromDate IS NULL) AND
		(CAST(note.StoreIssueNoteDate as date) <= @ToDate OR @ToDate IS NULL) AND
		(det.ItemID = @ItemID OR @ItemID IS NULL)
	ORDER BY note.StoreIssueNoteDate DESC
END

GO

exec spINStoreIssueNoteSearchList 1




