-- ==========================================================================================
-- INDocumentPrintLog  +  spINRegisterDocumentPrint
-- Print-status tracking for PR / PO / GRN report generation.
--
-- Run this whole file against the application database (e.g. GL) to install or update the
-- feature. It is idempotent and safe to re-run:
--   * fresh server            -> creates the table + proc
--   * server with an older     -> migrates the table in place (adds columns, collapses the
--     one-row-per-print build     old history rows to one row per user, swaps the index) and
--                                 recreates the proc
--
-- Model: ONE ROW per (DocumentType, DocumentID, PrintedBy).
--   * first time that user prints the document  -> INSERT a row (PrintCount = 1)
--   * every later print by the same user        -> UPDATE that row (PrintCount + 1) and
--                                                  refresh PrintLabel for the new turn
--
-- PrintLabel by that user's turn (PrintCount):
--   * user IS the document creator (owner):  turn 1 -> 'Original' ; turn N -> 'Reprinted {N-1}'
--   * user is NOT the creator (or UserID 0): turn N -> 'Copy {N}'
--
-- Every report generation counts as one print. No historical backfill.
-- NOTE: migrating from the previous build re-normalises labels to this per-user model
--       (a row that read e.g. 'Copy 3' under the old combined counter becomes 'Copy 1'
--        if that was that user's first print).
-- ==========================================================================================

SET NOCOUNT ON
GO

-- ------------------------------------------------------------------------------------------
-- 1. Table - fresh install
-- ------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE [name] = 'INDocumentPrintLog' AND [type] = 'U')
BEGIN
    CREATE TABLE dbo.INDocumentPrintLog
    (
        PrintLogID     BIGINT       IDENTITY(1,1) NOT NULL,
        DocumentType   VARCHAR(4)   NOT NULL,   -- 'PR' | 'PO' | 'GRN'
        DocumentID     BIGINT       NOT NULL,
        PrintedBy      INT          NOT NULL,   -- SecUser.UsersID ; 0 = unidentified user
        PrintCount     INT          NOT NULL CONSTRAINT DF_INDocumentPrintLog_PrintCount     DEFAULT (1),
        FirstPrintedAt DATETIME     NOT NULL CONSTRAINT DF_INDocumentPrintLog_FirstPrintedAt DEFAULT (GETDATE()),
        PrintedAt      DATETIME     NOT NULL CONSTRAINT DF_INDocumentPrintLog_PrintedAt      DEFAULT (GETDATE()),  -- last print
        IsOwnerPrint   BIT          NOT NULL CONSTRAINT DF_INDocumentPrintLog_IsOwnerPrint   DEFAULT (0),
        PrintLabel     VARCHAR(50)  NOT NULL CONSTRAINT DF_INDocumentPrintLog_PrintLabel     DEFAULT (''),
        CONSTRAINT PK_INDocumentPrintLog PRIMARY KEY CLUSTERED (PrintLogID)
    )

    CREATE UNIQUE INDEX UX_INDocumentPrintLog_DocUser
        ON dbo.INDocumentPrintLog (DocumentType, DocumentID, PrintedBy)

    PRINT 'INDocumentPrintLog: created.'
END
ELSE
    PRINT 'INDocumentPrintLog: already exists - checking for migration.'
GO

-- ------------------------------------------------------------------------------------------
-- 2. Migrate an existing table (one-row-per-print -> one-row-per-user)
-- ------------------------------------------------------------------------------------------
IF COL_LENGTH('dbo.INDocumentPrintLog', 'PrintCount') IS NULL
BEGIN
    ALTER TABLE dbo.INDocumentPrintLog
        ADD PrintCount INT NOT NULL CONSTRAINT DF_INDocumentPrintLog_PrintCount DEFAULT (1)
    PRINT 'INDocumentPrintLog: added column PrintCount.'
END
GO

IF COL_LENGTH('dbo.INDocumentPrintLog', 'FirstPrintedAt') IS NULL
BEGIN
    ALTER TABLE dbo.INDocumentPrintLog ADD FirstPrintedAt DATETIME NULL
    PRINT 'INDocumentPrintLog: added column FirstPrintedAt.'
END
GO

UPDATE dbo.INDocumentPrintLog SET FirstPrintedAt = PrintedAt WHERE FirstPrintedAt IS NULL
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.INDocumentPrintLog')
                                       AND name = 'FirstPrintedAt' AND is_nullable = 1)
BEGIN
    ALTER TABLE dbo.INDocumentPrintLog ALTER COLUMN FirstPrintedAt DATETIME NOT NULL
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_INDocumentPrintLog_FirstPrintedAt')
        ALTER TABLE dbo.INDocumentPrintLog
            ADD CONSTRAINT DF_INDocumentPrintLog_FirstPrintedAt DEFAULT (GETDATE()) FOR FirstPrintedAt
END
GO

-- Collapse duplicate rows into the surviving (latest) row per (DocumentType, DocumentID, PrintedBy)
IF EXISTS (
    SELECT 1 FROM dbo.INDocumentPrintLog
    GROUP BY DocumentType, DocumentID, PrintedBy
    HAVING COUNT(*) > 1
)
BEGIN
    ;WITH grp AS (
        SELECT DocumentType, DocumentID, PrintedBy,
               COUNT(*)            AS Cnt,
               MAX(PrintLogID)     AS KeepID,
               MIN(FirstPrintedAt) AS FirstAt,
               MAX(PrintedAt)      AS LastAt
        FROM dbo.INDocumentPrintLog
        GROUP BY DocumentType, DocumentID, PrintedBy
        HAVING COUNT(*) > 1
    )
    UPDATE l
    SET l.PrintCount     = g.Cnt,
        l.FirstPrintedAt = g.FirstAt,
        l.PrintedAt      = g.LastAt
    FROM dbo.INDocumentPrintLog l
    JOIN grp g ON g.KeepID = l.PrintLogID

    DELETE l
    FROM dbo.INDocumentPrintLog l
    JOIN (
        SELECT DocumentType, DocumentID, PrintedBy, MAX(PrintLogID) AS KeepID
        FROM dbo.INDocumentPrintLog
        GROUP BY DocumentType, DocumentID, PrintedBy
    ) k ON k.DocumentType = l.DocumentType
       AND k.DocumentID   = l.DocumentID
       AND k.PrintedBy    = l.PrintedBy
    WHERE l.PrintLogID <> k.KeepID

    PRINT 'INDocumentPrintLog: collapsed duplicate rows to one per (document, user).'
END
GO

-- Re-normalise every label to the per-user turn count
UPDATE dbo.INDocumentPrintLog
SET PrintLabel = CASE
        WHEN IsOwnerPrint = 1 AND PrintCount <= 1 THEN 'Original'
        WHEN IsOwnerPrint = 1                     THEN 'Reprinted ' + CAST(PrintCount - 1 AS VARCHAR(10))
        ELSE 'Copy ' + CAST(PrintCount AS VARCHAR(10))
    END
GO

-- Swap the old non-unique index for the unique (DocumentType, DocumentID, PrintedBy) one
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_INDocumentPrintLog_Doc'
                                       AND object_id = OBJECT_ID('dbo.INDocumentPrintLog'))
    DROP INDEX IX_INDocumentPrintLog_Doc ON dbo.INDocumentPrintLog
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_INDocumentPrintLog_DocUser'
                                           AND object_id = OBJECT_ID('dbo.INDocumentPrintLog'))
BEGIN
    CREATE UNIQUE INDEX UX_INDocumentPrintLog_DocUser
        ON dbo.INDocumentPrintLog (DocumentType, DocumentID, PrintedBy)
    PRINT 'INDocumentPrintLog: created unique index UX_INDocumentPrintLog_DocUser.'
END
GO

-- ------------------------------------------------------------------------------------------
-- 3. Proc
-- ------------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE [name] = 'spINRegisterDocumentPrint' AND [type] = 'P')
    DROP PROCEDURE dbo.spINRegisterDocumentPrint
GO

CREATE PROCEDURE dbo.spINRegisterDocumentPrint
    @DocumentType    VARCHAR(4),   -- 'PR' | 'PO' | 'GRN'
    @DocumentID      BIGINT,
    @PrintedByUserID INT           -- SecUser.UsersID ; pass 0 when the user cannot be identified
AS
BEGIN
    SET NOCOUNT ON
    SET XACT_ABORT ON

    DECLARE @CreatedBy INT
    DECLARE @IsOwner   BIT
    DECLARE @Label     VARCHAR(50)
    DECLARE @Count     INT
    DECLARE @RowID     BIGINT
    DECLARE @Now       DATETIME = GETDATE()

    -- Resolve the document creator for the given type
    IF @DocumentType = 'PR'
        SELECT @CreatedBy = CreatedBy FROM dbo.INPurchaseRequisition WHERE RequestID         = @DocumentID
    ELSE IF @DocumentType = 'PO'
        SELECT @CreatedBy = CreatedBy FROM dbo.INPurchaseOrder       WHERE PurchaseOrderID   = @DocumentID
    ELSE IF @DocumentType = 'GRN'
        SELECT @CreatedBy = CreatedBy FROM dbo.INGoodsReceiptNote    WHERE GoodsReceiptNoteID = @DocumentID

    SET @IsOwner = CASE
                       WHEN @PrintedByUserID <> 0 AND @CreatedBy IS NOT NULL AND @PrintedByUserID = @CreatedBy
                       THEN 1 ELSE 0
                   END

    BEGIN TRAN

        -- One row per (document, user). Lock it (or the key range) for the duration.
        SELECT @RowID = PrintLogID, @Count = PrintCount
        FROM dbo.INDocumentPrintLog WITH (UPDLOCK, HOLDLOCK)
        WHERE DocumentType = @DocumentType
          AND DocumentID   = @DocumentID
          AND PrintedBy    = @PrintedByUserID

        IF @RowID IS NULL
        BEGIN
            SET @Count = 1
            INSERT INTO dbo.INDocumentPrintLog
                (DocumentType, DocumentID, PrintedBy, PrintCount, FirstPrintedAt, PrintedAt, IsOwnerPrint, PrintLabel)
            VALUES
                (@DocumentType, @DocumentID, @PrintedByUserID, 1, @Now, @Now, @IsOwner, '')
            SET @RowID = SCOPE_IDENTITY()
        END
        ELSE
        BEGIN
            SET @Count = @Count + 1
            UPDATE dbo.INDocumentPrintLog
            SET PrintCount   = @Count,
                PrintedAt    = @Now,
                IsOwnerPrint = @IsOwner
            WHERE PrintLogID = @RowID
        END

        SET @Label = CASE
                         WHEN @IsOwner = 1 AND @Count <= 1 THEN 'Original'
                         WHEN @IsOwner = 1                 THEN 'Reprinted ' + CAST(@Count - 1 AS VARCHAR(10))
                         ELSE 'Copy ' + CAST(@Count AS VARCHAR(10))
                     END

        UPDATE dbo.INDocumentPrintLog
        SET PrintLabel = @Label
        WHERE PrintLogID = @RowID

    COMMIT TRAN

    SELECT
        @Label AS PrintLabel,
        ISNULL((SELECT u.Name FROM dbo.SecUsers u WHERE u.UsersID = @PrintedByUserID), '(unknown)') AS PrintedByName
END
GO

PRINT 'spINRegisterDocumentPrint: (re)created.'
GO
