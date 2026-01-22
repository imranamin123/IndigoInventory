
IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spAPInvoiceDetailRows')
	DROP PROC spAPInvoiceDetailRows
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 12-Feb-2025
-- Description:	
-- ==========================================================================================

CREATE procedure dbo.spAPInvoiceDetailRows
	@APInvoiceID bigint
AS
BEGIN

	SET	NOCOUNT ON;
	
	SELECT APInvoiceDetailID
      ,APInvoiceID
      ,GLAccountID
	  ,C1
	  ,C2
	  ,A3
      ,Amount
      ,CreatedAt
      ,CreatedBy
      ,ModifiedAt
      ,ModifiedBy
      ,CompanyID
  FROM 
	dbo.APInvoiceDetail
  WHERE 
	APInvoiceID = @APInvoiceID
ORDER BY 
	  APInvoiceDetailID
	  
END
GO

exec spAPInvoiceDetailRows 1

