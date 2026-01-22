-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 29-Jul-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
ALTER PROCEDURE [dbo].[spMenuPagesRightsList]
	
	@CompanyID int,
	@UserID int
		
AS

BEGIN

SELECT 
	u.CompanyID, u.UsersID, u.username,s.Name 'Company', SUM(ISNULL(p.UserID,0)) 'Assigned'
FROM 
	SecUsers u 
INNER JOIN Company s ON 	s.CompanyID = u.CompanyID 
LEFT JOIN SecPagesRights p ON p.CompanyID = u.CompanyID AND p.UserID = u.UsersID
WHERE 
	(s.CompanyID = @CompanyID OR @CompanyID IS NULL) AND
	(u.UsersID = @UserID  OR @UserID IS NULL)
GROUP BY 
	u.CompanyID,u.UsersID, u.username,s.Name 

END




