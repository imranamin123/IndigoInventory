IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINPurchaseOrderDetailRows')
	DROP PROC spINPurchaseOrderDetailRows
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 30-Apr-2025
-- Description:	
-- ==========================================================================================

CREATE procedure spINPurchaseOrderDetailRows
	@INPurchaseOrderID int
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		pod.PurchaseOrderDetailID,
		pod.PurchaseOrderID,
		pod.RequestDetailID,
		pod.ItemID,
		i.Description 'Item',
		s.Name 'Size',
		u.Name 'Unit',		  
		pod.RequestedQty,
		pod.ApprovedQty,
		pod.UnitPrice,
		pod.DiscountedPrice,
		pod.Tax,
		pod.Amount,
		pod.CreatedBy,
		pod.CreatedAt,
		pod.ModifiedBy,
		pod.ModifiedAt,
		pod.CompanyID
	FROM 
		INPurchaseOrderDetail pod
		INNER JOIN INItem i ON pod.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
	WHERE 

		pod.PurchaseOrderID = @INPurchaseOrderID
END

go


exec spINPurchaseOrderDetailRows 1






