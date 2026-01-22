IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDashboardUnitMatrixProjectWise')
	DROP PROC spDashboardUnitMatrixProjectWise
go
-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 24-Jun-25
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author:		
-- Create date: 
-- Description:	
-- ------------------------------------------------------------------------------------------

CREATE PROCEDURE spDashboardUnitMatrixProjectWise

	@ProjectID int
	
AS
BEGIN

select *
from 
(
select 
	u.ProjectID,
	--u.FloorNo,  
	SUBSTRING(isnull(Matrix,''),1, CHARINDEX('-',isnull(Matrix,''))-1) 'Floor',			
	SUBSTRING(Matrix, CHARINDEX('-', Matrix) + 1, LEN(Matrix)) Unit, 
	
	CASE WHEN u.Onhold = 0 THEN 
		t.DVArrangementDesc + '-' + ap.Name 
	ELSE
		'Onhold' + '-' 
	END	AS Applicant  

	--t.DVArrangementDesc + '-' + ap.Name AS Applicant  
from DVUnit u 
	left join DVApplicationForm f on u.UnitID = f.UnitID
	left join DVApplicant ap on f.ApplicationFormID = ap.ApplicationFormID
	left join DVArrangementType t on u.DVArrangementTypeID = t.DVArrangementTypeID
where 
	u.ProjectID = 1 and	u.Cancelled = 0 
) as SourceTable
PIVOT(
	MAX(Applicant) FOR Unit IN ([A],[B],[C],[D],[E],[F],[G],[H],[I],[J],[K],[L],[M],[N],[O],[P],[Q]) 
) AS PivotTable

END

go

EXEC spDashboardUnitMatrixProjectWise 1


select * from DVUnit
select * from DVArrangementType

