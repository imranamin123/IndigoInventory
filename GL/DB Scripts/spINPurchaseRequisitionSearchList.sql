
IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINPurchaseRequisitionSearchList')
	DROP PROC spINPurchaseRequisitionSearchList
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 11-Apr-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spINPurchaseRequisitionSearchList]
	@CompanyID int,
	@UserID int = NULL,
	@RequestDateFrom date = NULL,
	@RequestDateTo date = NULL
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT 
		pr.RequestID, 
		pr.RequestDate, 
		p.ProjectName,
		pr.ProjectDocumentNo,
		pr.Remarks,
		ISNULL(pr.SubmitedByKPO,0) 'SubmitedByKPO',
		ISNULL(pr.SubmitedAtKPO,0) 'SubmitedAtKPO',
		ISNULL(pr.SubmitedByMD,0) 'SubmitedByMD',
		ISNULL(pr.SubmitedAtMD,0) 'SubmitedAtMD'
	FROM INPurchaseRequisition pr
--	INNER JOIN INStore s ON pr.ToStoreID = s.StoreID
	INNER JOIN INProject p ON pr.ProjectID = p.ProjectID
	WHERE pr.CompanyID = @CompanyID AND
		(pr.CreatedBy = @UserID OR @UserID IS NULL) AND
		(CAST(pr.RequestDate as date) >= @RequestDateFrom OR @RequestDateFrom IS NULL) AND
		(CAST(pr.RequestDate as date) <= @RequestDateTo OR @RequestDateTo IS NULL)
	ORDER BY pr.RequestDate DESC
END

GO

exec spINPurchaseRequisitionSearchList 1


