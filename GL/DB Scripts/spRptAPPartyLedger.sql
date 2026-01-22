
IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptAPPartyLedger')
	DROP PROC spRptAPPartyLedger
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 26-Feb-2025
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spRptAPPartyLedger]
	@CompanyID int,
	@ToDate date,
	@APVendorID int = 0
	
AS
BEGIN
	SET NOCOUNT ON;


SELECT 
	InvNo,
	TransDate,
	APVendorName,
	Narration,
	Dr, 
	Cr,
	(Cr - Dr) Balance,
	Company
FROM (

		SELECT        
			dbo.APInvoice.APInvoiceNo 'InvNo', 
			dbo.APInvoice.APInvoiceDate 'TransDate', 
			dbo.APVendor.APVendorName,
			dbo.APInvoice.APInvoiceNo + ' - ' + dbo.APVendor.APVendorName 'Narration',
			0 'Dr',
			ISNULL(dbo.APInvoiceDetail.Amount,0)  'Cr',
			dbo.Company.Name 'Company'
		FROM            
			dbo.APInvoice 
			INNER JOIN dbo.APInvoiceDetail ON dbo.APInvoice.APInvoiceID = dbo.APInvoiceDetail.APInvoiceID 
			INNER JOIN dbo.APVendor ON dbo.APInvoice.APVendorID = dbo.APVendor.APVendorID
			INNER JOIN dbo.Company ON dbo.APInvoice.CompanyID = dbo.Company.CompanyID
		WHERE 
			dbo.APInvoice.CompanyID = @CompanyID AND
			CAST(dbo.APInvoice.APInvoiceDate AS date) < = @ToDate
			AND (dbo.APInvoice.APVendorID = @APVendorID OR @APVendorID = 0)
	

		UNION ALL

		SELECT        
			'' 'InvNo',
			dbo.BKBankTransDetail.CreatedAt 'TransDate', 
			dbo.APVendor.APVendorName,
			dbo.BKBankTransDetail.DocumentRef 'Narration',
			ISNULL(dbo.BKBankTransDetail.Dr,0) 'Dr',
			ISNULL(dbo.BKBankTransDetail.Cr,0) * -1 'Cr', 
			
			-- following is the old logic changed at 6-Aug-2025
			--ISNULL(dbo.BKBankTransDetail.Cr,0) * -1 'Dr', 
			--0 'Cr',
			dbo.Company.Name 'Comapny'
		FROM            	
			dbo.BKBankTransDetail
			INNER JOIN dbo.APVendor ON dbo.BKBankTransDetail.APVendorID = dbo.APVendor.APVendorID
			INNER JOIN dbo.Company ON dbo.BKBankTransDetail.CompanyID = dbo.Company.CompanyID
		WHERE 
			dbo.BKBankTransDetail.CompanyID = @CompanyID AND
			CAST(dbo.BKBankTransDetail.CreatedAt AS date) < = @ToDate AND
			(dbo.BKBankTransDetail.APVendorID = @APVendorID OR @APVendorID = 0)

	) main 
	
	order by TransDate
END


go


exec spRptAPPartyLedger 2, '2025-10-06', 1199


select dbo.APInvoiceDetail.* 	FROM            
			dbo.APInvoice 
			INNER JOIN dbo.APInvoiceDetail ON dbo.APInvoice.APInvoiceID = dbo.APInvoiceDetail.APInvoiceID 
	where APInvoice.APVendorID = 1199
go
select * from BKBankTransDetail where APVendorID = 1199
select * from SecUsers


