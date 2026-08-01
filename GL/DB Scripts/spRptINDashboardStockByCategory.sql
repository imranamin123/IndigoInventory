

IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINDashboardStockByCategory')
	DROP PROC spRptINDashboardStockByCategory
go

-- ==========================================================================================
-- Created By:	Claude
-- Create date: 01-Aug-2026
-- Description:	Inventory Dashboard - current stock quantity & value grouped by category.
-- ==========================================================================================

--exec spRptINDashboardStockByCategory 1, 2

CREATE OR ALTER PROCEDURE dbo.spRptINDashboardStockByCategory
(
    @CompanyID INT,
    @ProjectID INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ISNULL(ic.CategoryID, 0) AS CategoryID,
        ISNULL(ic.Name, 'Uncategorized') AS CategoryName,
        SUM(ISNULL(pi.QtyInHand, 0)) AS TotalQty,
        SUM(ISNULL(pi.QtyInHand, 0) * ISNULL(pi.LastRate, 0)) AS TotalValue

    FROM INProjectItem pi
    INNER JOIN INItem i ON i.ItemID = pi.ItemID
    LEFT JOIN INCategory ic ON ic.CategoryID = i.CategoryID

    WHERE pi.ProjectID = @ProjectID
      AND pi.CompanyID = @CompanyID

    GROUP BY
        ic.CategoryID,
        ic.Name

    HAVING SUM(ISNULL(pi.QtyInHand, 0)) <> 0
        OR SUM(ISNULL(pi.QtyInHand, 0) * ISNULL(pi.LastRate, 0)) <> 0

    ORDER BY
        TotalValue DESC;

END
GO
