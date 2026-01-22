

IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spSecurityGetModuleIdsByUser')
	DROP PROC spSecurityGetModuleIdsByUser
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 12-Dec-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE spSecurityGetModuleIdsByUser
	@UserID int
AS
BEGIN
	SET NOCOUNT ON;

	
	SELECT 	DISTINCT
		SecModules.ModuleID,
		SecModules.ModuleNo,
		SecModules.ModuleName,
		--SecModules.IsViewAllowed,
		SecPagesRights.ModuleOrderNo
	FROM 
		SecPagesRights INNER JOIN 
			SecModules ON SecPagesRights.ModuleNo = SecModules.ModuleNo
	WHERE
		SecPagesRights.IsViewAllowed = 1 AND
		UserID = @UserID
END

GO

exec spSecurityGetModuleIdsByUser 13


