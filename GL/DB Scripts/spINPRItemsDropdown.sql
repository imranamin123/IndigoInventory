IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINPRItemsDropdown')
	DROP PROC spINPRItemsDropdown
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 2-Apr-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spINPRItemsDropdown]
	@RequestID bigint
AS
BEGIN

	SET NOCOUNT On;
	SELECT 
		CAST(PRDet.RequestDetailID as varchar(50)) + ',' + CAST(PRDet.ItemID as varchar(50)) 'ItemID', 
		i.Description + ' = ' + CAST(PRDet.Balance as varchar(500)) 'Description',
		PRDet.Balance
		
	FROM INPurchaseRequisitionDetail PRDet 
	INNER JOIN INPurchaseRequisition PR ON PRDet.RequestID = PR.RequestID
	INNER JOIN INItem i ON PRDet.ItemID = i.ItemID
	
	WHERE 
		PRDet.RequestID = @RequestID
	ORDER BY i.Description
END

go 

exec spINPRItemsDropdown 268
select * from INPurchaseOrderDetail
select * from INPurchaseOrder
