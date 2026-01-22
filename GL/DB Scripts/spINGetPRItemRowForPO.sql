IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINGetPRItemRowForPO')
	DROP PROC spINGetPRItemRowForPO
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 11-Apr-2025
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spINGetPRItemRowForPO]
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
        ISNULL(pd.RequestedQty, 0) AS RequestedQty
    FROM INPurchaseRequisitionDetail pd
		INNER JOIN INPurchaseRequisition pr ON pd.RequestID = pr.RequestID
		INNER JOIN INItem i ON pd.ItemID = i.ItemID
		INNER JOIN INProjectItem p ON p.ItemID = i.ItemID AND p.ProjectID = pr.ProjectID  
		INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
		LEFT JOIN INSize s ON i.SizeID = s.SizeID
    WHERE
        pd.RequestDetailID = @RequestDetailID
        AND i.ItemID = @ItemID
        AND pd.Balance <> 0
        AND pr.SubmitedByKPO > 0;

END

GO

exec spINGetGRNItemRow 1369, 1814
select * from INPurchaseRequisition where RequestID = 267
select * from INPurchaseRequisitionDetail where RequestID = 267
select * from INItem where ItemID in(select ItemID from INPurchaseRequisitionDetail where RequestID = 267)

select * from INPurchaseOrder
select * from INPurchaseOrderDetail
