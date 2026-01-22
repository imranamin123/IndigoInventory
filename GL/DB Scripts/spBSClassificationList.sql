IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spBSClassificationList')
	DROP PROC spBSClassificationList
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
CREATE PROCEDURE spBSClassificationList
	@TransactionID int
AS
BEGIN
	SET NOCOUNT ON;


	SELECT 
	 d.BSClassificationDetailID
	,d.BSClassificationID
	,d.BSClassificationTypeID
	,d.TransactionID
	,d.Balance
	,d.CreatedAt
	,d.CreatedBy
	,d.ModifiedAt
	,d.ModifiedBy
	,d.CompanyID
 
FROM 
	BSClassificationDetail d 
		inner join BSClassificationType t ON d.BSClassificationTypeID = t.BSClassificationTypeID
		 
WHERE
	d.TransactionID = @TransactionID
END

GO

exec spBSClassificationList 1


