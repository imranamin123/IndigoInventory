

IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINDashboardRecentActivity')
	DROP PROC spRptINDashboardRecentActivity
go

-- ==========================================================================================
-- Created By:	Claude
-- Create date: 01-Aug-2026
-- Description:	Inventory Dashboard - latest N activities across Goods Receipt, Purchase
--              Order, Store Issue and Store Transfer documents.
-- ==========================================================================================

--exec spRptINDashboardRecentActivity 1, 2, 15

CREATE OR ALTER PROCEDURE dbo.spRptINDashboardRecentActivity
(
    @CompanyID INT,
    @ProjectID INT,
    @TopN INT = 15
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN) * FROM
    (
        SELECT
            'Goods Receipt' AS ActivityType,
            m.GoodsReceiptNoteID AS DocID,
            m.GoodsReceiptNotesDate AS ActivityDate,
            COUNT(d.GoodsReceiptNoteDetailID) AS ItemsCount,
            SUM(ISNULL(d.Amount, 0)) AS Amount,
            ISNULL(v.APVendorName, '') AS Reference,
            CASE WHEN m.IsPosted = 1 THEN 'Posted' ELSE 'Pending' END AS Status
        FROM INGoodsReceiptNote m
        INNER JOIN INGoodsReceiptNoteDetail d ON d.GoodsReceiptNoteID = m.GoodsReceiptNoteID
        LEFT JOIN APVendor v ON v.APVendorID = m.APVendorID
        WHERE m.ProjectID = @ProjectID AND m.CompanyID = @CompanyID
        GROUP BY m.GoodsReceiptNoteID, m.GoodsReceiptNotesDate, v.APVendorName, m.IsPosted

        UNION ALL

        SELECT
            'Purchase Order' AS ActivityType,
            po.PurchaseOrderID AS DocID,
            po.PurchaseOrderDate AS ActivityDate,
            COUNT(d.PurchaseOrderDetailID) AS ItemsCount,
            SUM(ISNULL(d.Amount, 0)) AS Amount,
            ISNULL(v.APVendorName, '') AS Reference,
            ISNULL(ps.Description, '') AS Status
        FROM INPurchaseOrder po
        INNER JOIN INPurchaseOrderDetail d ON d.PurchaseOrderID = po.PurchaseOrderID
        LEFT JOIN APVendor v ON v.APVendorID = po.APVendorID
        LEFT JOIN INPOStatus ps ON ps.POStatusID = po.POStatusID
        WHERE po.ProjectID = @ProjectID AND po.CompanyID = @CompanyID
        GROUP BY po.PurchaseOrderID, po.PurchaseOrderDate, v.APVendorName, ps.Description

        UNION ALL

        SELECT
            'Store Issue' AS ActivityType,
            m.StoreIssueNoteID AS DocID,
            m.StoreIssueNoteDate AS ActivityDate,
            COUNT(d.StoreIssueNoteDetailID) AS ItemsCount,
            SUM(ISNULL(d.IssuedQty, 0) * ISNULL(pi.LastRate, 0)) AS Amount,
            ISNULL(s.StoreName, '') AS Reference,
            CASE WHEN m.IsPosted = 1 THEN 'Posted' ELSE 'Pending' END AS Status
        FROM INStoreIssueNote m
        INNER JOIN INStoreIssueNoteDetail d ON d.StoreIssueNoteID = m.StoreIssueNoteID
        LEFT JOIN INProjectItem pi ON pi.ItemID = d.ItemID AND pi.ProjectID = m.ProjectID
        LEFT JOIN INStore s ON s.StoreID = m.StoreID
        WHERE m.ProjectID = @ProjectID AND m.CompanyID = @CompanyID
        GROUP BY m.StoreIssueNoteID, m.StoreIssueNoteDate, s.StoreName, m.IsPosted

        UNION ALL

        SELECT
            'Store Transfer' AS ActivityType,
            m.StoreTransferNoteID AS DocID,
            m.StoreTransferNoteDate AS ActivityDate,
            COUNT(d.StoreTransferNoteDetailID) AS ItemsCount,
            SUM(ISNULL(d.TransferQty, 0) * ISNULL(pi.LastRate, 0)) AS Amount,
            CASE WHEN m.FromProjectID = @ProjectID THEN 'To: ' + ISNULL(pTo.ProjectName, '')
                 ELSE 'From: ' + ISNULL(pFrom.ProjectName, '') END AS Reference,
            CASE WHEN m.ReceivedByID IS NOT NULL THEN 'Received'
                 WHEN m.ApprovedByID IS NOT NULL THEN 'Approved'
                 ELSE 'Pending' END AS Status
        FROM INStoreTransferNote m
        INNER JOIN INStoreTransferNoteDetail d ON d.StoreTransferNoteID = m.StoreTransferNoteID
        LEFT JOIN INProjectItem pi ON pi.ItemID = d.ItemID AND pi.ProjectID = m.FromProjectID
        LEFT JOIN INProject pFrom ON pFrom.ProjectID = m.FromProjectID
        LEFT JOIN INProject pTo ON pTo.ProjectID = m.ToProjectID
        WHERE (m.FromProjectID = @ProjectID OR m.ToProjectID = @ProjectID) AND m.CompanyID = @CompanyID
        GROUP BY m.StoreTransferNoteID, m.StoreTransferNoteDate, m.FromProjectID, m.ToProjectID,
                 pFrom.ProjectName, pTo.ProjectName, m.ReceivedByID, m.ApprovedByID

    ) activity

    ORDER BY
        ActivityDate DESC;

END
GO
