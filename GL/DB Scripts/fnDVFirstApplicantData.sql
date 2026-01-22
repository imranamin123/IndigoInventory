	


Alter FUNCTION fnDVFirstApplicantData(
	@ApplicationFormID INT
)
    RETURNS @DVApplicantFirst TABLE (
		[ApplicantID] int,
		[ApplicationFormID] [int] NULL,
		[Name] [varchar](500) NULL,
		[GenderID] [int] NULL,
		[HusbandFather] [varchar](500) NULL,
		[NICOP_CNIC] [varchar](100) NULL,
		[Passport] [varchar](100) Null,
		[HouseNo] [varchar](50) NULL,
		[StreetArea] [varchar](500) NULL,
		[Area] [varchar](100) NULL,
		[City] [varchar](50) NULL,
		[District] [varchar](50) NULL,
		[Division] [varchar](50) NULL,
		[Province] [varchar](50) NULL,
		[Country] [varchar](50) NULL,
		[CorrespondenceAdressHouseNo] [varchar](500) NULL,
		[StreetArea2] [varchar](500) NULL,
		[City2] [varchar](50) NULL,
		[Country2] [varchar](50) NULL,
		[PhoneNo] [varchar](50) NULL,
		[Phone2] [varchar](50) NULL,
		[Phone3] [varchar](50) NULL,
		[Email] [varchar](50) NULL,
		[Profession] [varchar](50) NULL,
		[SubProfession] [varchar](50) NULL,
		[Living] [varchar](50) NULL,
		[Citizen] [varchar](50) NULL,
		[Tax] [varchar](50) NULL,
		[NomioneeName] [varchar](100) NULL,
		[NomineeFatherHusbandName] [varchar](100) NULL,
		[Nominee_NICOP_CNIC] [varchar](50) NULL,
		[Relation] [varchar](50) NULL,
		[ImagePath] [varchar](2000) NULL,
		[CreatedAt] [datetime] NULL,
		[CreatedBy] [int] NULL,
		[ModifiedAt] [datetime] NULL,
		[ModifiedBy] [int] NULL,
		[CompanyID] [int] NULL
		
	)

AS
BEGIN

	INSERT INTO @DVApplicantFirst
		SELECT [ApplicantID]
		  ,[ApplicationFormID]
		  ,[Name]
		  ,[GenderID]
		  ,[HusbandFather]
		  ,[NICOP_CNIC]
		  ,[Passport]
		  ,[HouseNo]
		  ,[StreetArea]
		  ,[Area]
		  ,[City]
		  ,[District]
		  ,[Division]
		  ,Province.ProvinceName 'Province'
		  ,[Country]
		  ,[CorrespondenceAdressHouseNo]
		  ,[StreetArea2]
		  ,[City2]
		  ,[Country2]
		  ,[PhoneNo]
		  ,[Phone2]
		  ,[Phone3]
		  ,[Email]
		  ,[Profession]
		  ,[SubProfession]
		  ,[Living]
		  ,[Citizen]
		  ,[Tax]
		  ,[NomioneeName]
		  ,[NomineeFatherHusbandName]
		  ,[Nominee_NICOP_CNIC]
		  ,[Relation]
		  ,[ImagePath]
		  ,[CreatedAt]
		  ,[CreatedBy]
		  ,[ModifiedAt]
		  ,[ModifiedBy]
		  ,[CompanyID]
	  FROM DVApplicant left join Province ON DVApplicant.ProvinceID = Province.ProvinceID
	  WHERE ApplicantID=(SELECT Min(ApplicantID) FROM DVApplicant WHERE ApplicationFormID = @ApplicationFormID)
    
    RETURN;

END

go	
select * from dbo.fnDVFirstApplicantData(10)
	
