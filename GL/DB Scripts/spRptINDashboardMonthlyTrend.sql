

IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINDashboardMonthlyTrend')
	DROP PROC spRptINDashboardMonthlyTrend
go

-- ==========================================================================================
-- Created By:	Claude
-- Create date: 01-Aug-2026
-- Description:	Inventory Dashboard - purchases vs issues, grouped by month, within the
--              selected period.
-- ==========================================================================================

--exec spRptINDashboardMonthlyTrend 1, 2, '2026-01-01', '2026-08-01'

CREATE OR ALTER PROCEDURE dbo.spRptINDashboardMonthlyTrend
(
    @CompanyID INT,
    @ProjectID INT,
    @FromDate DATE,
    @ToDate DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Purchases AS
    (
        SELECT
            CAST(DATEFROMPARTS(YEAR(m.GoodsReceiptNotesDate), MONTH(m.GoodsReceiptNotesDate), 1) AS DATE) AS PeriodMonth,
            SUM(ISNULL(d.ReceivedQty, 0)) AS PurchaseQty,
            SUM(ISNULL(d.Amount, 0)) AS PurchaseValue
        FROM INGoodsReceiptNoteDetail d
        INNER JOIN INGoodsReceiptNote m ON m.GoodsReceiptNoteID = d.GoodsReceiptNoteID
        WHERE m.ProjectID = @ProjectID
          AND m.CompanyID = @CompanyID
          AND m.GoodsReceiptNotesDate >= @FromDate
          AND m.GoodsReceiptNotesDate <= @ToDate
        GROUP BY
            DATEFROMPARTS(YEAR(m.GoodsReceiptNotesDate), MONTH(m.GoodsReceiptNotesDate), 1)
    ),

    Issues AS
    (
        SELECT
            CAST(DATEFROMPARTS(YEAR(m.StoreIssueNoteDate), MONTH(m.StoreIssueNoteDate), 1) AS DATE) AS PeriodMonth,
            SUM(ISNULL(d.IssuedQty, 0)) AS IssueQty,
            SUM(ISNULL(d.IssuedQty, 0) * ISNULL(pi.LastRate, 0)) AS IssueValue
        FROM INStoreIssueNoteDetail d
        INNER JOIN INStoreIssueNote m ON m.StoreIssueNoteID = d.StoreIssueNoteID
        LEFT JOIN INProjectItem pi ON pi.ItemID = d.ItemID AND pi.ProjectID = m.ProjectID
        WHERE m.ProjectID = @ProjectID
          AND m.CompanyID = @CompanyID
          AND m.StoreIssueNoteDate >= @FromDate
          AND m.StoreIssueNoteDate <= @ToDate
        GROUP BY
            DATEFROMPARTS(YEAR(m.StoreIssueNoteDate), MONTH(m.StoreIssueNoteDate), 1)
    )

    SELECT
        ISNULL(p.PeriodMonth, i.PeriodMonth) AS PeriodMonth,
        ISNULL(p.PurchaseQty, 0) AS PurchaseQty,
        ISNULL(p.PurchaseValue, 0) AS PurchaseValue,
        ISNULL(i.IssueQty, 0) AS IssueQty,
        ISNULL(i.IssueValue, 0) AS IssueValue

    FROM Purchases p
    FULL OUTER JOIN Issues i ON i.PeriodMonth = p.PeriodMonth

    ORDER BY
        PeriodMonth;

END
GO
