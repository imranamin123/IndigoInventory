IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINGetItemRow')
	DROP PROC spINGetItemRow
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
CREATE PROCEDURE [dbo].[spINGetItemRow]
	@ProjectID int,
	@ItemID bigint
	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		i.ItemID, 
		p.LastRate,
		p.QtyInHand,

		u.Name 'Unit',
		s.Name 'Size'

	FROM INItem i 
	INNER JOIN INProjectItem p ON i.ItemID = p.ItemID 
	INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
	LEFT JOIN INSize s ON i.SizeID = s.SizeID
	WHERE
		i.ItemID = @ItemID AND
		p.ProjectID = @ProjectID
END

go

exec spINGetItemRow 1,2937

select * from INPurchaseRequisition where RequestID = 269
select * from INPurchaseRequisitionDetail where RequestID = 269








