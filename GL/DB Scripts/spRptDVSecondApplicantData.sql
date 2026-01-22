

IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptDVFirstApplicantData')
	DROP PROC spRptDVFirstApplicantData
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 18-Aug-2024
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spRptDVFirstApplicantData]
	@ApplicationFormID int	
AS
BEGIN

	SET	NOCOUNT ON;
	
	
	SELECT        
		a.ApplicantID
		,ISNULL(a.Name,'') 'Name'
		,ISNULL(a.GenderID,0) 'GenderID'
		,ISNULL(a.HusbandFather,'') 'HusbandFather'
		,ISNULL(a.NICOP_CNIC,'') 'NICOP_CNIC'
		,ISNULL(a.Passport,'') 'Passport'
		,ISNULL(a.HouseNo,'') 'HouseNo'
		,ISNULL(a.StreetArea,'') 'StreetArea'
		,ISNULL(a.Area,'') 'Area'
		,ISNULL(a.City,'') 'City'
		,ISNULL(a.District,'') 'District'
		,ISNULL(a.Division,'') 'Division'
		,ISNULL(a.ProvinceID,0) 'ProvinceID'
		,ISNULL(a.Country,'') 'Country'
		,ISNULL(a.CorrespondenceAdressHouseNo,'') 'CorrespondenceAdressHouseNo'
		,ISNULL(a.StreetArea2,'') 'StreetArea2'
		,ISNULL(a.City2,'') 'City2'
		,ISNULL(a.Country2,'') 'Country2'
		,ISNULL(a.PhoneNo,'') 'PhoneNo'
		,ISNULL(a.Phone2,'') 'Phone2'
		,ISNULL(a.Phone3,'') 'Phone3'
		,ISNULL(a.Email,'') 'Email'
		,ISNULL(a.Profession,'') 'Profession'
		,ISNULL(a.SubProfession,'') 'SubProfession'
		,ISNULL(a.Living,'') 'Living'
		,ISNULL(a.Citizen,'') 'Citizen'
		,ISNULL(a.Tax,'') 'Tax'
		,ISNULL(a.NomioneeName,'') 'NomioneeName'
		,ISNULL(a.NomineeFatherHusbandName,'') 'NomineeFatherHusbandName'
		,ISNULL(a.Nominee_NICOP_CNIC,'') 'Nominee_NICOP_CNIC'
		,ISNULL(a.Relation,'') 'Relation'
		,ISNULL(a.ImagePath,'') 'ImagePath'

	FROM            DVApplicationForm f
		INNER JOIN DVApplicant a ON f.ApplicationFormID = a.ApplicationFormID
		INNER JOIN  DVProject p ON f.ProjectID = p.ProjectID 
		INNER JOIN  DVUnit u ON f.UnitID = u.UnitID AND p.ProjectID = u.ProjectID 
		INNER JOIN	DVUnitType ut  ON u.UnitTypeID = ut.UnitTypeID
	WHERE 
		--f.ApplicationFormID = 10
		a.ApplicantID = @ApplicantID
END

GO

exec spRptDVFirstApplicantData 2019

select * from DVApplicant
where ApplicationFormID = 10
