IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINGetGRNItemRow')
	DROP PROC spINGetGRNItemRow
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 14-Apr-2025
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spINGetGRNItemRow]
	@ItemID bigint,
	@RequestDetailID bigint
	
AS
BEGIN
      SET NOCOUNT ON;

      SELECT TOP 1
        pd.RequestDetailID,
              i.ItemID,
              i.ItemCode,
        u.Name AS Unit,
        s.Name AS Size,
        ISNULL(pd.ApprovedQty, 0) AS ApprovedQty
    FROM INPurchaseRequisitionDetail pd
              INNER JOIN INPurchaseRequisition pr ON pd.RequestID = pr.RequestID
              INNER JOIN INItem i ON pd.ItemID = i.ItemID
              INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
              LEFT JOIN INSize s ON i.SizeID = s.SizeID
              LEFT JOIN  INProjectItem p ON p.ItemID = i.ItemID AND p.ProjectID = pr.ProjectID
    WHERE
        pd.RequestDetailID = @RequestDetailID
        AND i.ItemID = @ItemID
        AND pd.Balance <> 0
        AND pr.SubmitedByKPO > 0;
END
GO

exec spINGetGRNItemRow 1369, 1814

