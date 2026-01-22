IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spBSClassificationHeader')
	DROP PROC spBSClassificationHeader
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 29-Oct-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE spBSClassificationHeader
	@TransactionID int
AS
BEGIN
	SET NOCOUNT ON;

		SELECT s.TransactionID
		,	s.BKBankID 
		,	s.TransactionDate
		,	s.ValueDate
		,	s.TransactionReferenceNo
		,	s.Description
		,	s.Debit
		,	s.Credit
		,	ISNULL(s.Balance,0) 'Balance'
		,	ISNULL(s.Credit,0) - ISNULL(s.Debit,0) Amount

		,	ISNULL(c.BSClassificationID,0) 'BSClassificationID'
		,	ISNULL(c.BSClassificationDate,getdate()) 'BSClassificationDate'
		,	ISNULL(c.BSBankStatementBalance,0) 'BSBankStatementBalance'
		,	ISNULL(c.CreatedAt,getdate()) 'CreatedAt'
		,	ISNULL(c.CreatedBy,0) 'CreatedBy'
		,	ISNULL(c.ModifiedAt,GETDATE()) 'ModifiedAt'
		,	ISNULL(c.ModifiedBy,0) 'ModifiedBy'
		,	ISNULL(c.CompanyID ,0) 'CompanyID'

		FROM BSBankStatement s 
			LEFT JOIN BSClassification c ON s.TransactionID = c.TransactionID
		WHERE
			s.TransactionID = @TransactionID

END

GO

exec spBSClassificationHeader 1
exec spBSClassificationHeader 2



 

