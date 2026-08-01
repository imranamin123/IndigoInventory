

IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINDashboardItemMovement')
	DROP PROC spRptINDashboardItemMovement
go

-- ==========================================================================================
-- Created By:	Claude
-- Create date: 01-Aug-2026
-- Description:	Inventory Dashboard - issued quantity per item within the selected period,
--              for items that are stocked in the project. Ordered fastest-moving first;
--              the same result set is used to derive both the fast-moving and the
--              slow/non-moving item widgets on the client (top N / bottom N of this list).
-- ==========================================================================================

--exec spRptINDashboardItemMovement 1, 2, '2026-01-01', '2026-08-01'

CREATE OR ALTER PROCEDURE dbo.spRptINDashboardItemMovement
(
    @CompanyID INT,
    @ProjectID INT,
    @FromDate DATE,
    @ToDate DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Issued AS
    (
        SELECT
            d.ItemID,
            SUM(ISNULL(d.IssuedQty, 0)) AS IssuedQty
        FROM INStoreIssueNoteDetail d
        INNER JOIN INStoreIssueNote m ON m.StoreIssueNoteID = d.StoreIssueNoteID
        WHERE m.ProjectID = @ProjectID
          AND m.CompanyID = @CompanyID
          AND m.StoreIssueNoteDate >= @FromDate
          AND m.StoreIssueNoteDate <= @ToDate
        GROUP BY
            d.ItemID
    )

    SELECT
        i.ItemID,
        i.ItemCode,
        i.Description,
        ISNULL(ig.Name, '') AS GroupName,
        ISNULL(ic.Name, '') AS CategoryName,
        ISNULL(pi.QtyInHand, 0) AS QtyInHand,
        ISNULL(iss.IssuedQty, 0) AS IssuedQty

    FROM INProjectItem pi
    INNER JOIN INItem i ON i.ItemID = pi.ItemID
    LEFT JOIN INGroup ig ON ig.GroupID = i.GroupID
    LEFT JOIN INCategory ic ON ic.CategoryID = i.CategoryID
    LEFT JOIN Issued iss ON iss.ItemID = i.ItemID

    WHERE pi.ProjectID = @ProjectID
      AND pi.CompanyID = @CompanyID

    ORDER BY
        IssuedQty DESC;

END
GO
