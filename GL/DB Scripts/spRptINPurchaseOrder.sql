IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINPurchaseOrder')
	DROP PROC spRptINPurchaseOrder
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 17-Nov-2025
-- Description:	
-- ==========================================================================================

CREATE procedure spRptINPurchaseOrder
	@PurchaseOrderID bigint
AS
BEGIN

	SET	NOCOUNT ON;
	

	SELECT 
		po.PurchaseOrderID,
		po.PurchaseOrderDate,
		po.ProjectID,
		p.ProjectName,
		po.PaymentTerms,
	    st.Description 'Status',
		po.APVendorID,
		po.ApprovedAt,
		po.Remarks 'mRemarks',
		v.APVendorName,
		v.ContactNumber,
		v.ContactPerson,
		v.Address 'VendorAddress',
		v.BankDetails,
		v.Email,
		pod.ItemID,
		i.Description 'Item',
		s.Name 'Size',
		uom.Name 'Unit',
		pod.ApprovedQty,
		pod.UnitPrice,
		pod.DiscountedPrice,
		pod.Amount,
		c.Name 'Company',
		uCreate.Name 'CreatedBy',
		po.CreatedAt,
		uApprove.Name 'ApprovedBy',
		po.CancelledBy,
		po.CancelledAt,
		po.FreightCharges,
		CASE 
			WHEN po.GoodsReceiptNoteID <= 0 OR po.GoodsReceiptNoteID IS NULL  THEN
				'Pending'
			ELSE 
			  CAST(po.GoodsReceiptNoteID as varchar(50))
		END GoodsReceiptNoteID,
		CASE
			WHEN po.CancelledBy > 0 THEN
				'Cancelled'
		END Cancelled		

	FROM INPurchaseOrder po
		INNER JOIN INPurchaseOrderDetail pod ON po.PurchaseOrderID = pod.PurchaseOrderID
		INNER JOIN INItem i ON pod.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement uom ON i.UOMID = uom.UOMID
		INNER JOIN INPOStatus st ON po.POStatusID = st.POStatusID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
		INNER JOIN INProject p ON po.ProjectID = p.ProjectID
		LEFT JOIN APVendor v ON po.APVendorID = v.APVendorID
		INNER JOIN Company c ON po.CompanyID = c.CompanyID
		LEFT JOIN SecUsers uCreate ON po.CreatedBy = uCreate.UsersID
		LEFT JOIN SecUsers uApprove ON po.ApprovedBy = uApprove.UsersID

	WHERE 
		po.PurchaseOrderID = @PurchaseOrderID
	
END

go

exec spRptINPurchaseOrder 157









