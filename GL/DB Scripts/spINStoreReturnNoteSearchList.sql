
IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINStoreReturnNoteSearchList')
	DROP PROC spINStoreReturnNoteSearchList
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 15-Jan-2026
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spINStoreReturnNoteSearchList]
	@CompanyID int,
	@UserID int = NULL,
	@FromDate date = NULL,
	@ToDate date = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		note.StoreReturnNoteID,
		note.StoreReturnNoteDate,
		p.ProjectName,
		note.Remarks,
		note.IsPosted,
		note.CompanyID
	FROM dbo.INStoreReturnNote note
	INNER JOIN INProject p ON note.ProjectID = p.ProjectID

	WHERE note.CompanyID = @CompanyID AND
		(note.CreatedBy = @UserID OR @UserID IS NULL) AND
		(CAST(note.StoreReturnNoteDate as date) >= @FromDate OR @FromDate IS NULL) AND
		(CAST(note.StoreReturnNoteDate as date) <= @ToDate OR @ToDate IS NULL)
	ORDER BY note.StoreReturnNoteDate DESC
END

GO

exec spINStoreReturnNoteSearchList 1,59




