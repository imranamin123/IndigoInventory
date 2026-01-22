IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINStoreReturnNoteItemRow')
	DROP PROC spINStoreReturnNoteItemRow
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 16-Jan-2026
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spINStoreReturnNoteItemRow]
	@ItemID bigint
	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		i.ItemID, 
		u.Name 'Unit',
		s.Name 'Size'

	FROM INItem i 
	INNER JOIN INUnitOfMeasurement u ON i.UOMID = u.UOMID
	LEFT JOIN INSize s ON i.SizeID = s.SizeID
	WHERE
		i.ItemID = @ItemID
	AND i.Freeze = 0
END

GO

exec spINStoreReturnNoteItemRow 1350

select * from INItem where ItemID = 1350

