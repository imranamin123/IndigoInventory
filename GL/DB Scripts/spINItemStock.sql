IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINItemStock')
    DROP PROC spRptINItemStock
go
-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 22-Aug-2025
-- Description:	
-- ==========================================================================================

CREATE PROCEDURE spRptINItemStock
    @CompanyID int,
    @ProjectID int,
    @FromDate date = NULL,
    @ToDate date = NULL
AS
BEGIN

    SET NOCOUNT ON;

    SELECT i.ItemID,
           i.Description,
           g.Name AS GroupName,
           c.Name AS CategoryName,
           s.Name AS SizeName,
		   uom.Name 'UOM',
           p.ProjectName,
		   comp.Name 'CompanyName',
		   @FromDate 'FromDate',
		   @ToDate 'ToDate',
		   CASE
				WHEN	@ProjectID = 1 THEN
					ISNULL(ISALastRate,0)
				WHEN @ProjectID = 2 THEN
					ISNULL(IBALastRate,0)
		  END 'Rate',


           -- Opening Balance
           ISNULL(
           (
               SELECT SUM(ISNULL(grnd2.ReceivedQty, 0))
               FROM INGoodsReceiptNoteDetail grnd2
                   INNER JOIN INGoodsReceiptNote grn2
                       ON grnd2.GoodsReceiptNoteID = grn2.GoodsReceiptNoteID
               WHERE grnd2.ItemID = i.ItemID
                     AND grnd2.CompanyID = @CompanyID
                     AND grn2.ProjectID = @ProjectID
                     AND (
                             @FromDate IS NOT NULL
                             AND CAST(grn2.GoodsReceiptNotesDate AS DATE) < @FromDate
                         )
           ),
           0
                 ) - ISNULL(
                     (
                         SELECT SUM(ISNULL(sinnd2.IssuedQty, 0))
                         FROM INStoreIssueNoteDetail sinnd2
                             INNER JOIN INStoreIssueNote sinn2
                                 ON sinnd2.StoreIssueNoteID = sinn2.StoreIssueNoteID
                         WHERE sinnd2.ItemID = i.ItemID
                               AND sinnd2.CompanyID = @CompanyID
                               AND sinn2.ProjectID = @ProjectID
                               AND (
                                       @FromDate IS NOT NULL
                                       AND CAST(sinn2.StoreIssueNoteDate AS DATE) < @FromDate
                                   )
                     ),
                     0
                           ) AS OpeningQty,

           -- Received during period
           ISNULL(SUM(grnd.ReceivedQty), 0) AS ReceivedQty,

           -- Issued during period
           ISNULL(SUM(sinnd.IssuedQty), 0) AS IssuedQty,

           -- Closing Quantity
           (ISNULL(
            (
                SELECT SUM(ISNULL(grnd2.ReceivedQty, 0))
                FROM INGoodsReceiptNoteDetail grnd2
                    INNER JOIN INGoodsReceiptNote grn2
                        ON grnd2.GoodsReceiptNoteID = grn2.GoodsReceiptNoteID
                WHERE grnd2.ItemID = i.ItemID
                      AND grnd2.CompanyID = @CompanyID
                      AND grn2.ProjectID = @ProjectID
                      AND (
                              @FromDate IS NOT NULL
                              AND CAST(grn2.GoodsReceiptNotesDate AS DATE) < @FromDate
                          )
            ),
            0
                  ) - ISNULL(
                      (
                          SELECT SUM(ISNULL(sinnd2.IssuedQty, 0))
                          FROM INStoreIssueNoteDetail sinnd2
                              INNER JOIN INStoreIssueNote sinn2
                                  ON sinnd2.StoreIssueNoteID = sinn2.StoreIssueNoteID
                          WHERE sinnd2.ItemID = i.ItemID
                                AND sinnd2.CompanyID = @CompanyID
                                AND sinn2.ProjectID = @ProjectID
                                AND (
                                        @FromDate IS NOT NULL
                                        AND CAST(sinn2.StoreIssueNoteDate AS DATE) < @FromDate
                                    )
                      ),
                      0
                            ) + ISNULL(SUM(grnd.ReceivedQty), 0) - ISNULL(SUM(sinnd.IssuedQty), 0)
           ) AS ClosingQty,

           -- Closing Amount based on ProjectID
           ((ISNULL(
             (
                 SELECT SUM(ISNULL(grnd2.ReceivedQty, 0))
                 FROM INGoodsReceiptNoteDetail grnd2
                     INNER JOIN INGoodsReceiptNote grn2
                         ON grnd2.GoodsReceiptNoteID = grn2.GoodsReceiptNoteID
                 WHERE grnd2.ItemID = i.ItemID
                       AND grnd2.CompanyID = @CompanyID
                       AND grn2.ProjectID = @ProjectID
                       AND (
                               @FromDate IS NOT NULL
                               AND CAST(grn2.GoodsReceiptNotesDate AS DATE) < @FromDate
                           )
             ),
             0
                   ) - ISNULL(
                       (
                           SELECT SUM(ISNULL(sinnd2.IssuedQty, 0))
                           FROM INStoreIssueNoteDetail sinnd2
                               INNER JOIN INStoreIssueNote sinn2
                                   ON sinnd2.StoreIssueNoteID = sinn2.StoreIssueNoteID
                           WHERE sinnd2.ItemID = i.ItemID
                                 AND sinnd2.CompanyID = @CompanyID
                                 AND sinn2.ProjectID = @ProjectID
                                 AND (
                                         @FromDate IS NOT NULL
                                         AND CAST(sinn2.StoreIssueNoteDate AS DATE) < @FromDate
                                     )
                       ),
                       0
                             ) + ISNULL(SUM(grnd.ReceivedQty), 0) - ISNULL(SUM(sinnd.IssuedQty), 0)
            ) * (CASE
                     WHEN @ProjectID = 1 THEN
                         ISNULL(i.ISALastRate, 0)
                     ELSE
                         ISNULL(i.IBALastRate, 0)
                 END
                )
           ) AS ClosingAmount
    FROM INItem i
		INNER JOIN INUnitOfMeasurement uom 
			ON i.UOMID = uom.UOMID
        INNER JOIN INGroup g		
            ON i.GroupID = g.GroupID
        INNER JOIN INCategory c
            ON i.CategoryID = c.CategoryID
        INNER JOIN INSize s
            ON i.SizeID = s.SizeID
		INNER JOIN Company comp ON comp.CompanyID = i.CompanyID

        -- GRN joins for current period
        LEFT JOIN INGoodsReceiptNoteDetail grnd
            ON i.ItemID = grnd.ItemID
               AND grnd.CompanyID = @CompanyID
        LEFT JOIN INGoodsReceiptNote grn
            ON grnd.GoodsReceiptNoteID = grn.GoodsReceiptNoteID
               AND grn.ProjectID = @ProjectID
               AND (
                       @FromDate IS NULL
                       OR CAST(grn.GoodsReceiptNotesDate AS DATE) >= @FromDate
                   )
               AND (
                       @ToDate IS NULL
                       OR CAST(grn.GoodsReceiptNotesDate AS DATE) <= @ToDate
                   )

        -- SINN joins for current period
        LEFT JOIN INStoreIssueNoteDetail sinnd
            ON i.ItemID = sinnd.ItemID
               AND sinnd.CompanyID = @CompanyID
        LEFT JOIN INStoreIssueNote sinn
            ON sinnd.StoreIssueNoteID = sinn.StoreIssueNoteID
               AND sinn.ProjectID = @ProjectID
               AND (
                       @FromDate IS NULL
                       OR CAST(sinn.StoreIssueNoteDate AS DATE) >= @FromDate
                   )
               AND (
                       @ToDate IS NULL
                       OR CAST(sinn.StoreIssueNoteDate AS DATE) <= @ToDate
                   )
        LEFT JOIN DVProject p
            ON p.ProjectID = @ProjectID
    WHERE i.CompanyID = @CompanyID
    GROUP BY i.ItemID,
             i.Description,
             g.Name,
             c.Name,
             s.Name,
			 uom.Name,
			 comp.Name,
             p.ProjectName,
             i.ISALastRate,
             i.IBALastRate
    ORDER BY g.Name,
             c.Name,
             i.Description;
END

go

exec spRptINItemStock 1, 2, '2025-08-01','2025-08-27'

