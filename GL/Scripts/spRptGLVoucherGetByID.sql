
-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 20-Jul-2024
-- Description:	This sp is used in voucher report
-- ==========================================================================================



alter procedure [dbo].[spRptGLVoucherGetByID]
	@VoucherID int
AS
BEGIN

	SET	NOCOUNT ON;

	SELECT v.GLVoucherID
		  ,ISNULL(v.VoucherTypeID,0) 'VoucherTypeID'
		  ,ISNULL(v.VoucherNumber,'') 'VoucherNumber'
		  ,ISNULL(vt.Type,'') 'VoucherType'
		  ,ISNULL(u.Name,'') 'User'
		  ,ISNULL(c.Name,'') 'Company'
		  ,ISNULL(v.VoucherDate,'1900-10-01') 'VoucherDate'
		  ,ISNULL(v.FiscalYear,'') 'FiscalYear'
		  ,ISNULL(d.GLVoucherDetailID,'') 'GLVoucherDetailID'
		  ,ISNULL(d.GLAccountID,'') 'GLAccountID'
		  ,ISNULL(a.GLAccountNo,'') 'GLAccountNo'
		  ,ISNULL(a.Description,'') 'Description'
		  ,ISNULL(d.GLNarration,'') 'GLNarration'
		  ,ISNULL(d.Debit,0) 'Debit'
		  ,ISNULL(d.Credit,0) 'Credit'
		  ,ISNULL(v.CreatedBy,'0') 'CreatedBy'
		  ,ISNULL(v.CreatedAt,'1900-10-01') 'CreatedAt'
		  ,ISNULL(v.ModifiedBy,'0') 'ModifiedBy'
		  ,ISNULL(v.ModifiedAt,'1900-10-01') 'ModifiedAt'
		  ,ISNULL(v.CompanyID,'0') 'CompanyID'
		  
	FROM GLVoucher v 
		INNER JOIN GLVoucherDetail d ON v.GLVoucherID = d.GLVoucherID
		INNER JOIN VoucherType vt ON v.VoucherTypeID = vt.VoucherTypeID
		INNER JOIN SecUsers u ON v.CreatedBy = u.UsersID
		INNER JOIN Company c ON v.CompanyID = c.CompanyID
		INNER JOIN GLAccount a ON d.GLAccountID = a.GLAccountID
	WHERE v.GLVoucherID = @VoucherID

END

go

exec [spRptGLVoucherGetByID] 18