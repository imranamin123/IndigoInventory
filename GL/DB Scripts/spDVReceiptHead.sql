use GL
go

IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDVReceiptHead')
	DROP PROC spDVReceiptHead
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 30-Aug-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spDVReceiptHead]
	@ApplicationFormID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		f.ApplicationFormID,
		0 'ReceiptID',
		prj.ProjectName,
		f.ApplicationFormNo,
		f.ApplicationFormDate,
		u.UnitNo,
		ut.UnitTypeName,
		u.FloorNo,
		u.SQFT,
		f.Price,
		(u.SQFT * f.Price) Amount,
		f.TokenMoney,		
		GETDATE() 'DVReceiptDate',
		CAST(0 as bit) 'Varified',
		0 'VarifiedBy',		
		'' 'VarifiedByName',
		CAST(0 as bit) 'Locked',
		0 'LockedBy',
		'' 'LockedByName',
		0 'CreatedBy',
		GETDATE() 'CreatedAt',
		0 ModifiedBy,
		GETDATE() ModifiedAt,
		0 CompanyID
	FROM DVApplicationForm f
		left join DVUnit u ON f.UnitID = u.UnitID
		left join DVUnitType ut ON u.UnitTypeID = ut.UnitTypeID
		left join DVProject prj ON f.ProjectID = prj.ProjectID
	WHERE f.ApplicationFormID = @ApplicationFormID
	--SELECT
	--	f.ApplicationFormID,
	--	ISNULL(r.DVReceiptID,0) ReceiptID,
	--	prj.ProjectName,
	--	f.ApplicationFormNo,
	--	f.ApplicationFormDate,
	--	u.UnitNo,
	--	ut.UnitTypeName,
	--	u.FloorNo,
	--	u.SQFT,
	--	f.Price,
	--	(u.SQFT * f.Price) Amount,
	--	f.TokenMoney,		
	--	r.DVReceiptDate,
	--	r.Varified,
	--	r.VarifiedBy,		
	--	uvarifiedBy.Name 'VarifiedByName',
	--	r.Locked,
	--	r.LockedBy,
	--	ulockedBy.Name 'LockedByName',
	--	r.CreatedBy,
	--	ISNULL(r.CreatedAt,GETDATE()) CreatedAt,
	--	r.ModifiedBy,
	--	ISNULL(r.ModifiedAt,GETDATE()) ModifiedAt,
	--	r.CompanyID
	--from DVApplicationForm f
	--	left join DVReceipt r ON r.ApplicationFormID = f.ApplicationFormID
	--	left join SecUsers ulockedBy ON r.LockedBy = ulockedBy.UsersID 
	--	left join SecUsers uvarifiedBy ON r.VarifiedBy = uvarifiedBy.UsersID
	--	left join DVUnit u ON f.UnitID = u.UnitID
	--	left join DVUnitType ut ON u.UnitTypeID = ut.UnitTypeID
	--	left join DVProject prj ON f.ProjectID = prj.ProjectID
	--WHERE f.ApplicationFormID = @ApplicationFormID

END

GO

exec spDVReceiptHead 8




