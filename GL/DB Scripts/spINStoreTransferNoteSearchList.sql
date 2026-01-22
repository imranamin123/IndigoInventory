
IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINStoreTransferNoteSearchList')
	DROP PROC spINStoreTransferNoteSearchList
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 17-Sep-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spINStoreTransferNoteSearchList]
	@CompanyID int,
	@UserID int = NULL,
	@FromDate date = NULL,
	@ToDate date = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		stn.StoreTransferNoteID,
		stn.StoreTransferNoteDate,
		pF.ProjectName 'PrjectFrom',
		pT.ProjectName 'ProjectTo',
		stn.Remarks ,		
		CASE
			WHEN ISNULL(stn.ApprovedByID,0) > 0 THEN
				1
			ELSE 
				0
		END 'Approved',
		CASE
			WHEN ISNULL(stn.ReceivedByID,0) > 0 THEN
				1
			ELSE 
				0
		END 'Received',

		--ISNULL(grn.IsPosted,0) 'IsPosted',
		stn.CompanyID
	FROM dbo.INStoreTransferNote stn
	INNER JOIN INProject pF ON stn.FromProjectID = pF.ProjectID
	INNER JOIN INProject pT ON stn.ToProjectID = pT.ProjectID

	WHERE stn.CompanyID = @CompanyID AND
		(stn.CreatedBy = @UserID OR @UserID IS NULL) AND
		(CAST(stn.StoreTransferNoteDate as date) >= @FromDate OR @FromDate IS NULL) AND
		(CAST(stn.StoreTransferNoteDate as date) <= @ToDate OR @ToDate IS NULL)
	ORDER BY stn.StoreTransferNoteDate DESC
END

GO

exec spINStoreTransferNoteSearchList 1,59
select * from INStoreTransferNote



