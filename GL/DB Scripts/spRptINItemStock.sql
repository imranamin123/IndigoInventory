

IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINItemStock')
	DROP PROC spRptINItemStock
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 30-Jul-2026
-- Description:	
-- ==========================================================================================

--exec spRptINItemStock 1,2,null,'2026-01-01', '2026-07-01'

CREATE OR ALTER PROCEDURE dbo.spRptINItemStock
(
    @CompanyID INT,
    @ProjectID INT,
    @ItemID INT = NULL,
    @FromDate DATE,
    @ToDate DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    /*
        ============================================================
        1. Get only the items that belong to the requested project
        ============================================================
    */

    ;WITH ProjectItems AS
    (
        SELECT
            pi.ItemID,
            MAX(ISNULL(pi.LastRate, 0)) AS Rate,
            MAX(ISNULL(pi.OpeningQty, 0)) AS OpeningQty,
            MAX(ISNULL(pi.QtyInHand, 0)) AS QtyInHand
        FROM INProjectItem pi
        WHERE pi.ProjectID = @ProjectID
          AND (@ItemID IS NULL OR pi.ItemID = @ItemID)
        GROUP BY
            pi.ItemID
    ),

    /*
        ============================================================
        2. GRN - Aggregate BEFORE joining to Items
        ============================================================
    */

    GRN AS
    (
        SELECT
            d.ItemID,

            OpeningQty =
                SUM
                (
                    CASE
                        WHEN m.GoodsReceiptNotesDate < @FromDate
                        THEN ISNULL(d.ReceivedQty, 0)
                        ELSE 0
                    END
                ),

            ReceivedQty =
                SUM
                (
                    CASE
                        WHEN m.GoodsReceiptNotesDate >= @FromDate
                         AND m.GoodsReceiptNotesDate <= @ToDate
                        THEN ISNULL(d.ReceivedQty, 0)
                        ELSE 0
                    END
                )

        FROM INGoodsReceiptNoteDetail d
        INNER JOIN INGoodsReceiptNote m
            ON m.GoodsReceiptNoteID = d.GoodsReceiptNoteID

        WHERE m.ProjectID = @ProjectID
          AND m.IsPosted = 1
          AND m.GoodsReceiptNotesDate <= @ToDate
          AND (@ItemID IS NULL OR d.ItemID = @ItemID)

        GROUP BY
            d.ItemID
    ),

    /*
        ============================================================
        3. SIN - Aggregate BEFORE joining to Items
        ============================================================
    */

    SIN AS
    (
        SELECT
            d.ItemID,

            OpeningQty =
                SUM
                (
                    CASE
                        WHEN m.StoreIssueNoteDate < @FromDate
                        THEN ISNULL(d.IssuedQty, 0)
                        ELSE 0
                    END
                ),

            IssuedQty =
                SUM
                (
                    CASE
                        WHEN m.StoreIssueNoteDate >= @FromDate
                         AND m.StoreIssueNoteDate <= @ToDate
                        THEN ISNULL(d.IssuedQty, 0)
                        ELSE 0
                    END
                )

        FROM INStoreIssueNoteDetail d
        INNER JOIN INStoreIssueNote m
            ON m.StoreIssueNoteID = d.StoreIssueNoteID

        WHERE m.ProjectID = @ProjectID
          AND m.IsPosted = 1
          AND m.StoreIssueNoteDate <= @ToDate
          AND (@ItemID IS NULL OR d.ItemID = @ItemID)

        GROUP BY
            d.ItemID
    ),

    /*
        ============================================================
        4. SRN - Aggregate BEFORE joining to Items
        ============================================================
    */

    SRN AS
    (
        SELECT
            d.ItemID,

            OpeningQty =
                SUM
                (
                    CASE
                        WHEN m.StoreReturnNoteDate < @FromDate
                        THEN ISNULL(d.ReturnQty, 0)
                        ELSE 0
                    END
                ),

            ReturnQty =
                SUM
                (
                    CASE
                        WHEN m.StoreReturnNoteDate >= @FromDate
                         AND m.StoreReturnNoteDate <= @ToDate
                        THEN ISNULL(d.ReturnQty, 0)
                        ELSE 0
                    END
                )

        FROM INStoreReturnNoteDetail d
        INNER JOIN INStoreReturnNote m
            ON m.StoreReturnNoteID = d.StoreReturnNoteID

        WHERE m.ProjectID = @ProjectID
          AND m.IsPosted = 1
          AND m.StoreReturnNoteDate <= @ToDate
          AND (@ItemID IS NULL OR d.ItemID = @ItemID)

        GROUP BY
            d.ItemID
    ),

    /*
        ============================================================
        5. Transfer IN
        ============================================================
    */

    TransferIn AS
    (
        SELECT
            d.ItemID,

            OpeningQty =
                SUM
                (
                    CASE
                        WHEN m.StoreTransferNoteDate < @FromDate
                        THEN ISNULL(d.TransferQty, 0)
                        ELSE 0
                    END
                ),

            TransferQty =
                SUM
                (
                    CASE
                        WHEN m.StoreTransferNoteDate >= @FromDate
                         AND m.StoreTransferNoteDate <= @ToDate
                        THEN ISNULL(d.TransferQty, 0)
                        ELSE 0
                    END
                )

        FROM INStoreTransferNoteDetail d
        INNER JOIN INStoreTransferNote m
            ON m.StoreTransferNoteID = d.StoreTransferNoteID

        WHERE m.ToProjectID = @ProjectID
          AND m.ReceivedByID IS NOT NULL
          AND m.StoreTransferNoteDate <= @ToDate
          AND (@ItemID IS NULL OR d.ItemID = @ItemID)

        GROUP BY
            d.ItemID
    ),

    /*
        ============================================================
        6. Transfer OUT
        ============================================================
    */

    TransferOut AS
    (
        SELECT
            d.ItemID,

            OpeningQty =
                SUM
                (
                    CASE
                        WHEN m.StoreTransferNoteDate < @FromDate
                        THEN ISNULL(d.TransferQty, 0)
                        ELSE 0
                    END
                ),

            TransferQty =
                SUM
                (
                    CASE
                        WHEN m.StoreTransferNoteDate >= @FromDate
                         AND m.StoreTransferNoteDate <= @ToDate
                        THEN ISNULL(d.TransferQty, 0)
                        ELSE 0
                    END
                )

        FROM INStoreTransferNoteDetail d
        INNER JOIN INStoreTransferNote m
            ON m.StoreTransferNoteID = d.StoreTransferNoteID

        WHERE m.FromProjectID = @ProjectID
          AND m.ReceivedByID IS NOT NULL
          AND m.StoreTransferNoteDate <= @ToDate
          AND (@ItemID IS NULL OR d.ItemID = @ItemID)

        GROUP BY
            d.ItemID
    )

    /*
        ============================================================
        7. Final Result
        ============================================================
    */

    SELECT

        @ProjectID AS ProjectID,

        i.ItemID,

        i.Description,

        ISNULL(ig.Name, '') AS GroupName,

        ISNULL(ic.Name, '') AS CategoryName,

        ISNULL(sz.Name, '') AS SizeName,

        ISNULL(uom.Name, '') AS UOM,

        ISNULL(pi.Rate, 0) AS Rate,

        @FromDate AS FromDate,

        @ToDate AS ToDate,


        /*
            ========================================================
            Opening Quantity

            Project Opening
            + GRN
            - SIN
            + SRN
            + Transfer IN
            - Transfer OUT
            ========================================================
        */

        ISNULL(pi.OpeningQty, 0)

        + ISNULL(grn.OpeningQty, 0)

        - ISNULL(sin.OpeningQty, 0)

        + ISNULL(srn.OpeningQty, 0)

        + ISNULL(ti.OpeningQty, 0)

        - ISNULL(to1.OpeningQty, 0)

        AS OpeningQty,


        /*
            ========================================================
            Received
            ========================================================
        */

        ISNULL(grn.ReceivedQty, 0) AS ReceivedQty,


        /*
            ========================================================
            Issued
            ========================================================
        */

        ISNULL(sin.IssuedQty, 0) AS IssuedQty,


        /*
            ========================================================
            Return
            ========================================================
        */

        ISNULL(srn.ReturnQty, 0) AS ReturnQty,


        /*
            ========================================================
            Transfer
            ========================================================
        */

        ISNULL(ti.TransferQty, 0)

        - ISNULL(to1.TransferQty, 0)

        AS TransferQty,


        /*
            ========================================================
            Closing Quantity

            Opening + GRN - SIN + SRN + Transfer
            ========================================================
        */

        (
            ISNULL(pi.OpeningQty, 0)

            + ISNULL(grn.OpeningQty, 0)

            - ISNULL(sin.OpeningQty, 0)

            + ISNULL(srn.OpeningQty, 0)

            + ISNULL(ti.OpeningQty, 0)

            - ISNULL(to1.OpeningQty, 0)

            + ISNULL(grn.ReceivedQty, 0)

            + ISNULL(srn.ReturnQty, 0)

            + ISNULL(ti.TransferQty, 0)

            - ISNULL(to1.TransferQty, 0)

            - ISNULL(sin.IssuedQty, 0)
        )

        AS ClosingQty,


        /*
            ========================================================
            Closing Amount
            ========================================================
        */

        (
            ISNULL(pi.OpeningQty, 0)

            + ISNULL(grn.OpeningQty, 0)

            - ISNULL(sin.OpeningQty, 0)

            + ISNULL(srn.OpeningQty, 0)

            + ISNULL(ti.OpeningQty, 0)

            - ISNULL(to1.OpeningQty, 0)

            + ISNULL(grn.ReceivedQty, 0)

            + ISNULL(srn.ReturnQty, 0)

            + ISNULL(ti.TransferQty, 0)

            - ISNULL(to1.TransferQty, 0)

            - ISNULL(sin.IssuedQty, 0)
        )

        * ISNULL(pi.Rate, 0)

        AS ClosingAmount


    FROM INItem i

    /*
        IMPORTANT:
        INNER JOIN because your LINQ uses INNER JOIN
        ============================================================
    */

    INNER JOIN ProjectItems pi
        ON pi.ItemID = i.ItemID


    LEFT JOIN INGroup ig
        ON ig.GroupID = i.GroupID


    LEFT JOIN INCategory ic
        ON ic.CategoryID = i.CategoryID


    LEFT JOIN INSize sz
        ON sz.SizeID = i.SizeID


    LEFT JOIN INUnitOfMeasurement uom
        ON uom.UOMID = i.UOMID


    LEFT JOIN GRN grn
        ON grn.ItemID = i.ItemID


    LEFT JOIN SIN sin
        ON sin.ItemID = i.ItemID


    LEFT JOIN SRN srn
        ON srn.ItemID = i.ItemID


    LEFT JOIN TransferIn ti
        ON ti.ItemID = i.ItemID


    LEFT JOIN TransferOut to1
        ON to1.ItemID = i.ItemID


    WHERE i.CompanyID = @CompanyID

      AND (@ItemID IS NULL OR i.ItemID = @ItemID)


    /*
        ============================================================
        Remove rows with no positive activity

        Excludes the row only when Opening, GRN (received), SIN
        (issued), SRN (returned), Transfer (net), and Closing are
        ALL zero or negative -- i.e. nothing positive happened and
        there's nothing positive on hand. A row is kept as soon as
        any one of those is > 0.
        ============================================================
    */

    AND NOT
    (
        (
            ISNULL(pi.OpeningQty, 0)

            + ISNULL(grn.OpeningQty, 0)

            - ISNULL(sin.OpeningQty, 0)

            + ISNULL(srn.OpeningQty, 0)

            + ISNULL(ti.OpeningQty, 0)

            - ISNULL(to1.OpeningQty, 0)
        ) <= 0

        AND ISNULL(grn.ReceivedQty, 0) <= 0

        AND ISNULL(sin.IssuedQty, 0) <= 0

        AND ISNULL(srn.ReturnQty, 0) <= 0

        AND
        (
            ISNULL(ti.TransferQty, 0)
            - ISNULL(to1.TransferQty, 0)
        ) <= 0

        AND
        (
            ISNULL(pi.OpeningQty, 0)

            + ISNULL(grn.OpeningQty, 0)

            - ISNULL(sin.OpeningQty, 0)

            + ISNULL(srn.OpeningQty, 0)

            + ISNULL(ti.OpeningQty, 0)

            - ISNULL(to1.OpeningQty, 0)

            + ISNULL(grn.ReceivedQty, 0)

            + ISNULL(srn.ReturnQty, 0)

            + ISNULL(ti.TransferQty, 0)

            - ISNULL(to1.TransferQty, 0)

            - ISNULL(sin.IssuedQty, 0)
        ) <= 0

    )

    /*
        ============================================================
        Exclude rows with a negative Opening or Closing quantity
        entirely -- these represent posted issues/transfers that
        exceed posted receipts (over-consumption or a receiving
        document stuck unposted), not something this report can
        meaningfully display as a quantity.
        ============================================================
    */

    AND
    (
        ISNULL(pi.OpeningQty, 0)

        + ISNULL(grn.OpeningQty, 0)

        - ISNULL(sin.OpeningQty, 0)

        + ISNULL(srn.OpeningQty, 0)

        + ISNULL(ti.OpeningQty, 0)

        - ISNULL(to1.OpeningQty, 0)
    ) >= 0

    AND
    (
        ISNULL(pi.OpeningQty, 0)

        + ISNULL(grn.OpeningQty, 0)

        - ISNULL(sin.OpeningQty, 0)

        + ISNULL(srn.OpeningQty, 0)

        + ISNULL(ti.OpeningQty, 0)

        - ISNULL(to1.OpeningQty, 0)

        + ISNULL(grn.ReceivedQty, 0)

        + ISNULL(srn.ReturnQty, 0)

        + ISNULL(ti.TransferQty, 0)

        - ISNULL(to1.TransferQty, 0)

        - ISNULL(sin.IssuedQty, 0)
    ) >= 0;

END
GO