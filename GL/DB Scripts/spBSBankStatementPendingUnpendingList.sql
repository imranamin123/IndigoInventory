
IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spBSBankStatementPendingUnpendingList')
	DROP PROC spBSBankStatementPendingUnpendingList
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
CREATE PROCEDURE spBSBankStatementPendingUnpendingList
	@CompanyID int,
	@BKBankID int,
	@IsPending bit=1,
	@ValueDateFrom date,
	@ValueDateTo date
	
AS
BEGIN
	SET NOCOUNT ON;
	IF @IsPending  = 1
	BEGIN
		SELECT stmt.TransactionID
			,stmt.TransactionDate
			,stmt.BKBankID
			,b.BankCode
			,b.BankName
			,@IsPending 'IsPending'
			,stmt.ValueDate
			,stmt.TransactionReferenceNo
			,stmt.Description
			,ISNULL(stmt.Debit,0) 'Debit'
			,ISNULL(stmt.Credit,0) 'Credit'
			,(ISNULL(stmt.Credit,0) - ISNULL(stmt.Debit,0)) 'Amount'
			,stmt.Balance
			,stmt.CreatedAt
			,stmt.CreatedBy
			,stmt.CompanyID
		FROM BKBank b 
			inner join BSBankStatement stmt ON b.BankID = stmt.BKBankID
		WHERE 
			(stmt.CompanyID = @CompanyID) AND
			(stmt.BKBankID = @BKBankID) AND
			(stmt.ValueDate >= @ValueDateFrom AND stmt.ValueDate <= @ValueDateTo) AND
			(stmt.TransactionID not in ( 
				SELECT 
					TransactionID 
				FROM 
					BSClassification 
				WHERE 
					CompanyID = @CompanyID AND 
					BKBankID= @BKBankID AND 
					ValueDate >= @ValueDateFrom AND ValueDate <= @ValueDateTo
				))

	END
	ELSE
	BEGIN
		SELECT stmt.TransactionID
			,stmt.TransactionDate
			,stmt.BKBankID
			,b.BankCode
			,b.BankName
			,@IsPending 'IsPending'
			,stmt.ValueDate
			,stmt.TransactionReferenceNo
			,stmt.Description
			,ISNULL(stmt.Debit,0) 'Debit'
			,ISNULL(stmt.Credit,0) 'Credit'
			,(ISNULL(stmt.Credit,0) - ISNULL(stmt.Debit,0)) 'Amount'
			,stmt.Balance
			,stmt.CreatedAt
			,stmt.CreatedBy
			,stmt.CompanyID
		FROM BKBank b 
			inner join BSBankStatement stmt ON b.BankID = stmt.BKBankID
		WHERE 
			(stmt.CompanyID = @CompanyID) AND
			(stmt.BKBankID = @BKBankID) and
			(stmt.ValueDate >= @ValueDateFrom AND stmt.ValueDate <= @ValueDateTo) 
	END	
END
GO


exec spBSBankStatementPendingUnpendingList 1,1,0,'2024-10-23','2024-10-23'
exec spBSBankStatementPendingUnpendingList 1,1,1,'2024-10-23','2024-10-23'



