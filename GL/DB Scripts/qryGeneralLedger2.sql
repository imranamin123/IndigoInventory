declare	
	@CompanyID int = 2,
	@EndDate date=null,
	@EndGLAccountNo varchar(50)='27-50-01-0102'
	@StartDate date='2025-05-01',
	@StartGLAccountNo varchar(50)='27-50-01-0101',
	
	select v.CompanyID,c.Name 'Company',a.GLAccountNo, a.Description,'' VoucherDate,'' VTYPE,'' VoucherNumber,'' GLNarration,
			'ob' type, 0 Debit, 0 Credit, 0 'Balance', SUM(isnull(d.Debit,0)) - SUM(isnull(d.Credit,0)) 'OB'
	
		from GLVoucher v 
		inner join GLVoucherDetail d on v.GLVoucherID = d.GLVoucherID
		inner join GLAccount a ON d.GLAccountID = a.GLAccountID
		inner join Company c on v.CompanyID = c.CompanyID ---- between '11-20-03-0101' and '11-20-03-0101'
		where v.VoucherDate < @StartDate and LTRIM(RTRIM(a.GLAccountNo)) >= @StartGLAccountNo and LTRIM(RTRIM(a.GLAccountNo)) <= @EndGLAccountNo and v.CompanyID = @CompanyID
		--where d.GLAccountID =  884 and v.VoucherDate < '2024-11-04' --and a.GLAccountNo = '11-11-10-0101' --and '11-1-10-0101'
		group by v.CompanyID,c.Name,a.GLAccountNo,a.Description

union all

	SELECT        
		Company.CompanyID, 
		Company.Name 'Company', 
		GLAccount.GLAccountNo 'GLAccountNo', 
		GLAccount.Description 'Description', 
		GLVoucher.VoucherDate,
		VoucherType.VTYPE,
		GLVoucher.VoucherNumber,
		GLVoucherDetail.GLNarration,
		'trans' type,
		GLVoucherDetail.Debit 'Debit', 
		GLVoucherDetail.Credit 'Credit',
		(GLVoucherDetail.Debit - GLVoucherDetail.Credit) Balance,
		0 'OB'
	  
	FROM  GLVoucherDetail INNER JOIN
		  GLVoucher ON GLVoucherDetail.GLVoucherID = GLVoucher.GLVoucherID INNER JOIN
		  GLAccount ON GLVoucherDetail.GLAccountID = GLAccount.GLAccountID INNER JOIN
		  Company ON GLVoucher.CompanyID = Company.CompanyID AND GLAccount.CompanyID = Company.CompanyID INNER JOIN 
		  VoucherType ON GLVoucher.VoucherTypeID = VoucherType.VoucherTypeID

WHERE        
		Company.CompanyID = @CompanyID AND 

		GLVoucher.VoucherDate < @StartDate and LTRIM(RTRIM(GLAccount.GLAccountNo)) >= @StartGLAccountNo and LTRIM(RTRIM(GLAccount.GLAccountNo)) <= @EndGLAccountNo and GLVoucher.CompanyID = @CompanyID
		--(
		--	(GLAccount.GLAccountNo between @StartGLAccountNo and @EndGLAccountNo)-- OR (@StartGLAccountNo IS NULL AND @EndGLAccountNo IS NULL) 			
		--) AND
		--(
		--	(GLVoucher.VoucherDate between @StartDate and @EndDate) --OR (@StartDate IS NULL AND @EndDate IS NULL)
		--)
