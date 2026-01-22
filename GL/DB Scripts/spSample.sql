IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptDVUnitLedger')
	DROP PROC spRptDVUnitLedger
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 13-Sep-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spRptDVUnitLedger]
	@ApplicationFormID int
AS
BEGIN
	SET NOCOUNT ON;
END

