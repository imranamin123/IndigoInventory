
-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 29-Jul-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
create PROCEDURE [dbo].[spMenuPages]
	
	@CompanyID SMALLINT,
	@IsAdmin BIT = null
AS

BEGIN

SELECT 
	u.UsersID, u.username,s.Name 'CompanyName', SUM(ISNULL(p.UserID,0)) 'Assigned'
FROM 
	SecUsers u 
INNER JOIN Company s ON 	s.CompanyID = u.CompanyID 
LEFT JOIN SecPagesRights p ON p.CompanyID = u.CompanyID AND p.UserID = u.UsersID
WHERE 
	(@IsAdmin = 1 OR s.CompanyID = @CompanyID )
GROUP BY 
	u.UsersID, u.username,s.Name 

END





