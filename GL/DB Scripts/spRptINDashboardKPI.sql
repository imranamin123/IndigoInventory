

IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINDashboardKPI')
	DROP PROC spRptINDashboardKPI
go

-- ==========================================================================================
-- Created By:	Claude
-- Create date: 01-Aug-2026
-- Description:	Inventory Dashboard - single row KPI summary (item/category/group counts,
--              current stock qty/value, purchases & issues within the selected period).
-- ==========================================================================================

--exec spRptINDashboardKPI 1, 2, '2026-01-01', '2026-08-01'

CREATE OR ALTER PROCEDURE dbo.spRptINDashboardKPI
(
    @CompanyID INT,
    @ProjectID INT,
    @FromDate DATE,
    @ToDate DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT

        (SELECT COUNT(*) FROM INItem WHERE CompanyID = @CompanyID AND ISNULL(Freeze, 0) = 0) AS TotalItems,

        (SELECT COUNT(*) FROM INCategory WHERE CompanyID = @CompanyID) AS TotalCategories,

        (SELECT COUNT(*) FROM INGroup WHERE CompanyID = @CompanyID) AS TotalGroups,

        ISNULL((SELECT SUM(pi.QtyInHand)
                FROM INProjectItem pi
                WHERE pi.ProjectID = @ProjectID AND pi.CompanyID = @CompanyID), 0) AS TotalStockQty,

        ISNULL((SELECT SUM(ISNULL(pi.QtyInHand, 0) * ISNULL(pi.LastRate, 0))
                FROM INProjectItem pi
                WHERE pi.ProjectID = @ProjectID AND pi.CompanyID = @CompanyID), 0) AS TotalStockValue,

        ISNULL((SELECT SUM(ISNULL(d.Amount, 0))
                FROM INGoodsReceiptNoteDetail d
                INNER JOIN INGoodsReceiptNote m ON m.GoodsReceiptNoteID = d.GoodsReceiptNoteID
                WHERE m.ProjectID = @ProjectID AND m.CompanyID = @CompanyID
                  AND m.GoodsReceiptNotesDate >= @FromDate AND m.GoodsReceiptNotesDate <= @ToDate), 0) AS TotalPurchaseValue,

        ISNULL((SELECT COUNT(DISTINCT m.GoodsReceiptNoteID)
                FROM INGoodsReceiptNote m
                WHERE m.ProjectID = @ProjectID AND m.CompanyID = @CompanyID
                  AND m.GoodsReceiptNotesDate >= @FromDate AND m.GoodsReceiptNotesDate <= @ToDate), 0) AS TotalPurchaseCount,

        ISNULL((SELECT SUM(ISNULL(d.IssuedQty, 0) * ISNULL(pi.LastRate, 0))
                FROM INStoreIssueNoteDetail d
                INNER JOIN INStoreIssueNote m ON m.StoreIssueNoteID = d.StoreIssueNoteID
                LEFT JOIN INProjectItem pi ON pi.ItemID = d.ItemID AND pi.ProjectID = m.ProjectID
                WHERE m.ProjectID = @ProjectID AND m.CompanyID = @CompanyID
                  AND m.StoreIssueNoteDate >= @FromDate AND m.StoreIssueNoteDate <= @ToDate), 0) AS TotalIssueValue,

        ISNULL((SELECT COUNT(DISTINCT m.StoreIssueNoteID)
                FROM INStoreIssueNote m
                WHERE m.ProjectID = @ProjectID AND m.CompanyID = @CompanyID
                  AND m.StoreIssueNoteDate >= @FromDate AND m.StoreIssueNoteDate <= @ToDate), 0) AS TotalIssueCount;

END
GO
