

IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINDashboardTopItemsByValue')
	DROP PROC spRptINDashboardTopItemsByValue
go

-- ==========================================================================================
-- Created By:	Claude
-- Create date: 01-Aug-2026
-- Description:	Inventory Dashboard - top N items by current stock value.
-- ==========================================================================================

--exec spRptINDashboardTopItemsByValue 1, 2, 10

CREATE OR ALTER PROCEDURE dbo.spRptINDashboardTopItemsByValue
(
    @CompanyID INT,
    @ProjectID INT,
    @TopN INT = 10
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN)
        i.ItemID,
        i.ItemCode,
        i.Description,
        ISNULL(ig.Name, '') AS GroupName,
        ISNULL(ic.Name, '') AS CategoryName,
        ISNULL(pi.QtyInHand, 0) AS Qty,
        ISNULL(pi.LastRate, 0) AS Rate,
        (ISNULL(pi.QtyInHand, 0) * ISNULL(pi.LastRate, 0)) AS Value

    FROM INProjectItem pi
    INNER JOIN INItem i ON i.ItemID = pi.ItemID
    LEFT JOIN INGroup ig ON ig.GroupID = i.GroupID
    LEFT JOIN INCategory ic ON ic.CategoryID = i.CategoryID

    WHERE pi.ProjectID = @ProjectID
      AND pi.CompanyID = @CompanyID
      AND ISNULL(pi.QtyInHand, 0) > 0

    ORDER BY
        Value DESC;

END
GO
