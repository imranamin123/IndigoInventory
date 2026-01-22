SELECT        
	Company.CompanyID, 
	Company.Name 'Company', 
	GLAccount.GLAccountNo 'GLAccountNo', 
	GLAccount.Description 'Description', 
	GLVoucher.VoucherDate,
	VoucherType.VTYPE,
	GLVoucherDetail.GLNarration,
	SUM(GLVoucherDetail.Debit) 'Debit', 
	Sum(GLVoucherDetail.Credit) 'Credit',  
	SUM(GLVoucherDetail.Debit)-Sum(GLVoucherDetail.Credit) 'Total'
FROM  GLVoucherDetail INNER JOIN
                         GLVoucher ON GLVoucherDetail.GLVoucherID = GLVoucher.GLVoucherID INNER JOIN
                         GLAccount ON GLVoucherDetail.GLAccountID = GLAccount.GLAccountID INNER JOIN
                         Company ON GLVoucher.CompanyID = Company.CompanyID AND GLAccount.CompanyID = Company.CompanyID INNER JOIN 
						 VoucherType ON GLVoucher.VoucherTypeID = VoucherType.VoucherTypeID
WHERE        
		Company.CompanyID = 1 -- and GLVoucher.VoucherDate <= @CalanderMonth
group by Company.CompanyID, Company.Name , GLAccount.GLAccountNo, GLAccount.Description
,GLVoucher.VoucherDate,VoucherType.VTYPE,GLVoucherDetail.GLNarration
--GLAccount.A1, GLAccount.A2, GLAccount.A3, GLAccount.A4

--order by GLAccount.GLAccountNo --  CONVERT(int, GLAccount.A1), CONVERT(int, GLAccount.A2), CONVERT(int, GLAccount.A3), CONVERT(int, GLAccount.A4)


select VoucherDate from GLVoucher

