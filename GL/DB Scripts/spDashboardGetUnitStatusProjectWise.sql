IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDashboardGetUnitStatusProjectWise')
	DROP PROC spDashboardGetUnitStatusProjectWise
go
-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 17-Jun-25
-- Description:	This stored procedure returns the count of sold and unsold units by project
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author:		
-- Create date: 
-- Description:	
-- ------------------------------------------------------------------------------------------

CREATE PROCEDURE spDashboardGetUnitStatusProjectWise
	@CompanyID int,
	@ProjectID int
	
AS
BEGIN

	-- Unsold
	SELECT 
		'Unsold' 'Status', 
		COUNT(*) 'Units' ,
		ISNULL(SUM(u.UnitPrice * u.SQFT),0) 'Amount',
		SUM(u.SQFT) 'SQFT'
	FROM 
		DVUnit u 
			LEFT JOIN DVApplicationForm f ON u.UnitID = f.UnitID

	WHERE 
		u.CompanyID = @CompanyID
		and u.ProjectID = @ProjectID
		and u.Cancelled = 0
		--and u.Onhold = 0
		and u.IsActive = 1
		

	UNION ALL


	-- Sold
	SELECT 
		'Sold' 'Status', 
		COUNT(u.UnitID) 'Units' ,
		ISNULL(SUM(f.Price * u.SQFT),0) 'Amount',
		SUM(u.SQFT) 'SQFT'
	FROM 
		DVUnit u 
			LEFT JOIN DVApplicationForm f ON u.UnitID = f.UnitID
			LEFT JOIN DVArrangementType t ON u.DVArrangementTypeID = t.DVArrangementTypeID

	WHERE 
		u.CompanyID = @CompanyID
		and u.ProjectID = @ProjectID
		and u.Cancelled = 0
		and u.Onhold = 0
		and u.IsActive = 0
		and t.DVArrangementTypeID = 1 -- sold

UNION ALL

	-- Adjustment
	SELECT 
		'Adjustment' 'Status', 
		COUNT(u.UnitID) 'Units' ,
		ISNULL(SUM(f.Price * u.SQFT),0) 'Amount',
		SUM(u.SQFT) 'SQFT'
	FROM 
		DVUnit u 
			LEFT JOIN DVApplicationForm f ON u.UnitID = f.UnitID
			LEFT JOIN DVArrangementType t ON u.DVArrangementTypeID = t.DVArrangementTypeID

	WHERE 
		u.CompanyID = @CompanyID
		and u.ProjectID = @ProjectID
		and u.Cancelled = 0
		and u.Onhold = 0
		and u.IsActive = 0
		and t.DVArrangementTypeID =2 -- sold

		UNION ALL

	-- Barter
	SELECT 
		'Barter' 'Status', 
		COUNT(u.UnitID) 'Units' ,
		ISNULL(SUM(f.Price * u.SQFT),0) 'Amount',
		SUM(u.SQFT) 'SQFT'
	FROM 
		DVUnit u 
			LEFT JOIN DVApplicationForm f ON u.UnitID = f.UnitID
			LEFT JOIN DVArrangementType t ON u.DVArrangementTypeID = t.DVArrangementTypeID

	WHERE 
		u.CompanyID = @CompanyID
		and u.ProjectID = @ProjectID
		and u.Cancelled = 0
		and u.Onhold = 0
		and u.IsActive = 0
		and t.DVArrangementTypeID = 3 -- Barter


END

go

exec spDashboardGetUnitStatusProjectWise 1,1



--select * from DVUnit
--select * from DVArrangementType
--select * from DVUnit

--update DVUnit set unitPrice = 26000

--select f.ApplicationFormID, u.UnitNo 
--from DVApplicationForm f left join DVUnit u on f.UnitID = u.UnitID
--where u.Cancelled = 0 and u.IsActive = 1



