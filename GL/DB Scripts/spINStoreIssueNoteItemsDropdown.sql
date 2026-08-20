IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINStoreIssueNoteItemsDropdown')
	DROP PROC spINStoreIssueNoteItemsDropdown
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 22-Jul-2025
-- Description:
-- ==========================================================================================

CREATE procedure [dbo].[spINStoreIssueNoteItemsDropdown]
	@ProjectID int,
	@CompanyID int
AS
BEGIN

	SET NOCOUNT On;

	/*
		Balance is computed from actual posted transaction history
		(same formula as spRptINItemStock's ClosingQty) rather than
		INProjectItem.QtyInHand -- QtyInHand only started being
		maintained by GRN/SIN/SRN/Transfer posting on 1-Jul-2026, so
		it is 0 for items whose stock came entirely from postings
		before that date, even though they have real stock on hand.
	*/

	;WITH GRN AS
	(
		SELECT d.ItemID, SUM(ISNULL(d.ReceivedQty, 0)) AS ReceivedQty
		FROM INGoodsReceiptNoteDetail d
		INNER JOIN INGoodsReceiptNote m ON m.GoodsReceiptNoteID = d.GoodsReceiptNoteID
		WHERE m.ProjectID = @ProjectID AND m.IsPosted = 1
		GROUP BY d.ItemID
	),
	SIN AS
	(
		SELECT d.ItemID, SUM(ISNULL(d.IssuedQty, 0)) AS IssuedQty
		FROM INStoreIssueNoteDetail d
		INNER JOIN INStoreIssueNote m ON m.StoreIssueNoteID = d.StoreIssueNoteID
		WHERE m.ProjectID = @ProjectID AND m.IsPosted = 1
		GROUP BY d.ItemID
	),
	SRN AS
	(
		SELECT d.ItemID, SUM(ISNULL(d.ReturnQty, 0)) AS ReturnQty
		FROM INStoreReturnNoteDetail d
		INNER JOIN INStoreReturnNote m ON m.StoreReturnNoteID = d.StoreReturnNoteID
		WHERE m.ProjectID = @ProjectID AND m.IsPosted = 1
		GROUP BY d.ItemID
	),
	TransferIn AS
	(
		SELECT d.ItemID, SUM(ISNULL(d.TransferQty, 0)) AS TransferQty
		FROM INStoreTransferNoteDetail d
		INNER JOIN INStoreTransferNote m ON m.StoreTransferNoteID = d.StoreTransferNoteID
		WHERE m.ToProjectID = @ProjectID AND m.ReceivedByID IS NOT NULL
		GROUP BY d.ItemID
	),
	TransferOut AS
	(
		SELECT d.ItemID, SUM(ISNULL(d.TransferQty, 0)) AS TransferQty
		FROM INStoreTransferNoteDetail d
		INNER JOIN INStoreTransferNote m ON m.StoreTransferNoteID = d.StoreTransferNoteID
		WHERE m.FromProjectID = @ProjectID AND m.ReceivedByID IS NOT NULL
		GROUP BY d.ItemID
	)

	SELECT *
    FROM
    (
        SELECT
            i.ItemID,

            -- Description column
            i.Description + ' = ' + CAST
            (
                ISNULL(p.OpeningQty, 0)
                + ISNULL(grn.ReceivedQty, 0)
                - ISNULL(sin.IssuedQty, 0)
                + ISNULL(srn.ReturnQty, 0)
                + ISNULL(ti.TransferQty, 0)
                - ISNULL(to1.TransferQty, 0)
                AS VARCHAR(50)
            ) AS Description,

            -- Balance column
            (
                ISNULL(p.OpeningQty, 0)
                + ISNULL(grn.ReceivedQty, 0)
                - ISNULL(sin.IssuedQty, 0)
                + ISNULL(srn.ReturnQty, 0)
                + ISNULL(ti.TransferQty, 0)
                - ISNULL(to1.TransferQty, 0)
            ) AS Balance

        FROM INItem i
        INNER JOIN INProjectItem p
            ON p.ItemID = i.ItemID AND p.ProjectID = @ProjectID
        LEFT JOIN GRN grn ON grn.ItemID = i.ItemID
        LEFT JOIN SIN sin ON sin.ItemID = i.ItemID
        LEFT JOIN SRN srn ON srn.ItemID = i.ItemID
        LEFT JOIN TransferIn ti ON ti.ItemID = i.ItemID
        LEFT JOIN TransferOut to1 ON to1.ItemID = i.ItemID
        WHERE
            i.CompanyID = @CompanyID
    ) main
    WHERE main.Balance > 0
    ORDER BY main.Description;


END

go

exec spINStoreIssueNoteItemsDropdown 5,1

select * from INProjectItem where ItemID = 1225 and ProjectID = 5

--select * from INItem

