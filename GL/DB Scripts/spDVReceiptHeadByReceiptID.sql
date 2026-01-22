
IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDVReceiptHeadByReceiptID')
	DROP PROC spDVReceiptHeadByReceiptID
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 05-Sep-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spDVReceiptHeadByReceiptID]
	@DVReceiptID int
AS
BEGIN
	SET NOCOUNT ON;
SELECT
	f.ApplicationFormID,
	ISNULL(r.DVReceiptID,0) ReceiptID,
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
	r.DVReceiptDate,
	r.Varified,
	r.IsPosted,
	r.VarifiedBy,		
	uvarifiedBy.Name 'VarifiedByName',
	r.Locked,
	r.LockedBy,
	ulockedBy.Name 'LockedByName',
	r.CreatedBy,
	ISNULL(r.CreatedAt,GETDATE()) CreatedAt,
	r.ModifiedBy,
	ISNULL(r.ModifiedAt,GETDATE()) ModifiedAt,
	r.CompanyID
FROM DVApplicationForm f
	left join DVReceipt r ON r.ApplicationFormID = f.ApplicationFormID
	left join SecUsers ulockedBy ON r.LockedBy = ulockedBy.UsersID 
	left join SecUsers uvarifiedBy ON r.VarifiedBy = uvarifiedBy.UsersID
	left join DVUnit u ON f.UnitID = u.UnitID
	left join DVUnitType ut ON u.UnitTypeID = ut.UnitTypeID
	left join DVProject prj ON f.ProjectID = prj.ProjectID
WHERE r.DVReceiptID = @DVReceiptID

END

GO

exec spDVReceiptHeadByReceiptID 11