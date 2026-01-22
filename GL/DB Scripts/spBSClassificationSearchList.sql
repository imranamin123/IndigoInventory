

IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spBSClassificationSearchList')
	DROP PROC spBSClassificationSearchList
go


-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 18-Oct-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE spBSClassificationSearchList
	@CompanyID int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT        
		c.BSClassificationID,
		b.BankID, 
		b.BankName, 
		c.BSClassificationDate, 
		c.BSBankStatementBalance,
		b.CompanyID 
	FROM  BSClassification c
		INNER JOIN  BKBank b ON b.BankID = c.BKBankID
	WHERE
		c.CompanyID = @CompanyID

END
GO

exec spBSClassificationSearchList 1



