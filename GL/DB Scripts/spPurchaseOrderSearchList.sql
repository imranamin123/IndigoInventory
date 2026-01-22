
IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINPurchaseOrderSearchList')
	DROP PROC spINPurchaseOrderSearchList
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 06-Nov-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spINPurchaseOrderSearchList]
	@CompanyID int,
	@UserID int = NULL,
	@FromDate date = NULL,
	@ToDate date = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		po.PurchaseOrderID,
		po.PurchaseOrderDate,
		p.ProjectName,
		po.Remarks,
		CASE 
			WHEN ISNULL(po.ApprovedBy,0) > 0 THEN
				1
			ELSE 0
		END 'Approved', 
		po.CompanyID
	FROM dbo.INPurchaseOrder po
	INNER JOIN INProject p ON po.ProjectID = p.ProjectID

	WHERE po.CompanyID = @CompanyID AND
		(po.CreatedBy = @UserID OR @UserID IS NULL) AND
		(CAST(po.PurchaseOrderDate as date) >= @FromDate OR @FromDate IS NULL) AND
		(CAST(po.PurchaseOrderDate as date) <= @ToDate OR @ToDate IS NULL)
	ORDER BY po.PurchaseOrderDate DESC
END

GO

exec spINPurchaseOrderSearchList 1,1,'2025-11-06','2025-11-07'





