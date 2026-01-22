IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spDVUnitsListByProjectID')
	DROP PROC spDVUnitsListByProjectID
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 30-Oct-2024
-- Description:	
-- ==========================================================================================

create procedure [dbo].[spDVUnitsListByProjectID]
	@ProjectID int,
	@AllUnits bit
AS
BEGIN

	SET	NOCOUNT ON;
	IF @AllUnits = 1
	BEGIN
		SELECT 
			u.ProjectID,
			u.IsActive,
			u.UnitID, 
			u.UnitNo, 
			(SELECT TOP 1 NAME FROM DVApplicant WHERE ApplicationFormID = f.ApplicationFormID) Applicant,
			u.UnitNo + ' ' + ISNULL((SELECT TOP 1 NAME FROM DVApplicant WHERE ApplicationFormID = f.ApplicationFormID),'') UnitApplicant
			--u.UnitNo  + '  ' + ISNULL((SELECT TOP 1 NAME FROM DVApplicant WHERE ApplicationFormID = f.ApplicationFormID),'') UnitApplicant
		from 
			DVUnit u
		left JOIN DVApplicationForm f ON u.UnitID = f.UnitID AND u.ProjectID = f.ProjectID
		WHERE u.ProjectID = @ProjectID
	END
	ELSE
	BEGIN
		SELECT 
			u.ProjectID,
			u.IsActive,
			u.UnitID, 
			u.UnitNo, 
			(SELECT TOP 1 NAME FROM DVApplicant WHERE ApplicationFormID = f.ApplicationFormID) Applicant,
			u.UnitNo + ' ' + ISNULL((SELECT TOP 1 NAME FROM DVApplicant WHERE ApplicationFormID = f.ApplicationFormID),'') UnitApplicant

			--u.UnitNo + '  ' + (SELECT TOP 1 NAME FROM DVApplicant WHERE ApplicationFormID = f.ApplicationFormID) UnitApplicant
		from 
			DVUnit u
		left JOIN DVApplicationForm f ON u.UnitID = f.UnitID AND u.ProjectID = f.ProjectID
		WHERE u.ProjectID = @ProjectID AND u.IsActive = 1 -- @AllUnits
	
	END

END
go

exec spDVUnitsListByProjectID 1,1
exec spDVUnitsListByProjectID 1,0



  