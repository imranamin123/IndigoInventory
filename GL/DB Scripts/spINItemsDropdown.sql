IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINItemsDropdown')
	DROP PROC spINItemsDropdown
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 15-Jan-2026
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spINItemsDropdown]
	@CompanyID int
AS
BEGIN

	SET NOCOUNT On;

	SELECT 
		ItemID,
		Description
	FROM 
		INItem
	WHERE
		CompanyID = @CompanyID
	ORDER BY 
		[Description]
END

go 

exec spINItemsDropdown 1

--select * from INItem


