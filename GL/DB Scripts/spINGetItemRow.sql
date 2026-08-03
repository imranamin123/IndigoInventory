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
		ISNULL(p.QtyInHand, 0) AS QtyInHand,

		u.Name 'Unit',
		s.Name 'Size'

	FROM INItem i
	LEFT JOIN INProjectItem p ON i.ItemID = p.ItemID AND p.ProjectID = @ProjectID
	INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
	LEFT JOIN INSize s ON i.SizeID = s.SizeID
	WHERE
		i.ItemID = @ItemID
END

go

exec spINGetItemRow 1,775



select * from INProjectItem where ItemID = 775 
select * from INItem where ItemID = 775

select * from INPurchaseRequisition where RequestID = 269
select * from INPurchaseRequisitionDetail where RequestID = 269








