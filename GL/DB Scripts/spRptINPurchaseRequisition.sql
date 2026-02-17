IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINPurchaseRequisition')
	DROP PROC spRptINPurchaseRequisition
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 21-Apr-2025
-- Description:	
-- ==========================================================================================

CREATE procedure spRptINPurchaseRequisition
	@RequestID int
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT 
		c.Name 'Company',
		m.RequestID,
		m.RequestDate,
		p.ProjectName,
		m.ProjectDocumentNo 'DocumentNo',
		m.ManualDemandNo,
		m.Remarks 'mRemarks',
		d.RequestDetailID,
		d.ItemID,
		i.ItemCode,
		i.Description 'Item',
		s.Name 'Size',
		uom.Name 'Unit',		  
		d.RequestedQty,
		d.ApprovedQty,
		d.QtyInHand,
		d.LastRate,
		d.Remarks 'dRemarks',
		u1.Name 'SubmittedByKPO',
		m.SubmitedAtKPO 'SubmitedAtKPO',
		u2.Name 'SubmittedByMD',		
		m.SubmitedAtMD 'SubmitedAtMD',
		CASE
			WHEN m.cancelledBy > 0 THEN
				'Cancelled'
		END Cancelled
		
		
	FROM INPurchaseRequisition m
		INNER JOIN INPurchaseRequisitionDetail d ON m.RequestID = d.RequestID
		INNER JOIN INItem i ON d.ItemID = i.ItemID
		INNER JOIN INUnitOfMeasurement uom ON i.UOMID = uom.UOMID
		LEFT JOIn INSize s ON i.SizeID = s.SizeID
		INNER JOIN INProject p ON p.ProjectID = m.ProjectID
		INNER JOIN Company c ON m.CompanyID = c.CompanyID
		LEFT JOIN SecUsers u1 ON m.SubmitedByKPO = u1.UsersID
		LEFT JOIN SecUsers u2 ON m.SubmitedByMD = u2.UsersID
  WHERE 
	
	d.RequestID = @RequestID

END

go

exec spRptINPurchaseRequisition 10777


--select * from secusers





