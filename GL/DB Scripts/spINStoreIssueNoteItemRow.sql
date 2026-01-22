IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spINStoreIssueNoteItemRow')
	DROP PROC spINStoreIssueNoteItemRow
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 13-Jun-2025
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spINStoreIssueNoteItemRow]
	@ItemID bigint
	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		i.ItemID, 
		i.ItemCode,
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

exec spINStoreIssueNoteItemRow 1350

select * from INItem where ItemID = 1350

