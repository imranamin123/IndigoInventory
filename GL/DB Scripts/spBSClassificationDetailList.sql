
IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spBSClassificationDetailList')
	DROP PROC spBSClassificationDetailList
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 24-Oct-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE spBSClassificationDetailList
	@BSClassificationID int	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT BSClassificationDetailID
		,BSClassificationID
		,BSClassificationTypeID
		,TransactionID
		,Balance
		,CreatedAt
		,CreatedBy
		,ModifedAt
		,ModifiedBy
		,CompanyID
	FROM 
		BSClassificationDetail
	WHERE 
		BSClassificationID = @BSClassificationID
END
GO


exec spBSClassificationDetailList 1