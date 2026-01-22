
-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 29-Jul-2024
-- Description:	
-- ==========================================================================================



ALTER procedure [dbo].[spFiscalYearSetupGetList]
	@CompanyID int
AS
BEGIN
	SET	NOCOUNT ON;
	SELECT
		FiscalYearSetupID,
		FiscalYear,
		FiscalPeriod,
		CalanderMonth,
		CONVERT(varchar, FiscalYear) + '-' + CONVERT(varchar, FiscalPeriod) 'FiscalYearPeriod'
	FROM
		FiscalYearSetup 
	WHERE 
		CompanyID = @CompanyID

END

GO

EXEC spFiscalYearSetupGetList 1