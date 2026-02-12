IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptPendingCompleteDemands')
	DROP PROC spRptPendingCompleteDemands
go


-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 12-Feb-2026
-- Description:	
-- ==========================================================================================

CREATE procedure spRptPendingCompleteDemands
	@ComapnyID int,
	@ProjectID int,
	@FromDate date = NULL,
	@ToDate date = NULL
AS
BEGIN
	SELECT 
		c.Name 'Company',
		p.ProjectName,
		pr.RequestID, 
		pr.ProjectID, 
		pr.RequestDate, 
		SUM(prd.RequestedQty) RequestedQty, 
		SUM(prd.Balance) Balance
	FROM 
		INPurchaseRequisition pr INNER JOIN 
		INPurchaseRequisitionDetail prd ON pr.RequestID = prd.RequestID INNER JOIN 
		INProject p ON pr.ProjectID = p.ProjectID INNER JOIN 
		Company c ON pr.CompanyID = c.CompanyID
	WHERE 
		pr.CompanyID = @ComapnyID AND
		pr.ProjectID = @ProjectID AND
		(CAST(pr.RequestDate as date) >= @FromDate OR @FromDate IS NULL) AND
		(CAST(pr.RequestDate as date) <= @ToDate OR @ToDate IS NULL) 

	GROUP BY 
		c.Name,
		p.ProjectName,
		pr.RequestID, 
		pr.ProjectID, 
		pr.RequestDate
	
END

GO

exec spRptPendingCompleteDemands 1,2


