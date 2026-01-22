-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 13-Aug-2024
-- Description:	
-- ==========================================================================================

alter procedure [dbo].[spDVUnitsRows]
	@UnitID int	
AS
BEGIN

	SET	NOCOUNT ON;

	SELECT u.UnitID
		  ,u.ProjectID
		  ,u.UnitNo
		  ,u.UnitTypeID
		  ,u.FloorNo
		  ,u.SQFT
		  ,u.IsActive
		  ,u.CompanyID
		  ,t.UnitTypeName

	  FROM DVUnit u inner join DVUnitType t on u.UnitTypeID = t.UnitTypeID
	  WHERE u.UnitID = @UnitID

END


