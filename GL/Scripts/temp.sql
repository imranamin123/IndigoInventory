-- Get Calander Month

declare @CompanyID int = 1
declare @FiscalYear varchar(10) = '2024-10'
declare @CalanderMonth date 

declare @Year int
declare @Month int

select @FiscalYear 
select @Year = CONVERT(int, SUBSTRING(@FiscalYear,0,5))
select @Month = CONVERT(int, SUBSTRING(@FiscalYear,6,5))
select @Year
select @Month 


	select * 
	into #FiscalYearSetup
	from FiscalYearSetup
	where FiscalYear = @Year and FiscalPeriod = @Month AND CompanyID = @CompanyID

select @CalanderMonth = CalanderMonth  from #FiscalYearSetup

select @CalanderMonth 'CalanderMonth'
-- End Get Calander Month

SELECT c.CompanyID, c.Name, a.GLAccountNo,a.Description, vd.Debit, vd.Credit
FROM Company c 
	INNER JOIN GLVoucher v ON c.CompanyID = v.CompanyID
	INNER JOIN GLVoucherDetail vd ON v.CompanyID = vd.CompanyID AND v.GLVoucherID = vd.GLVoucherID
	INNER JOIN GLAccount a ON vd.CompanyID = a.CompanyID AND vd.GLVoucherID = a.GLAccountID
WHERE 
	c.CompanyID = 1 AND v.VoucherDate <= @CalanderMonth




drop table #FiscalYearSetup







