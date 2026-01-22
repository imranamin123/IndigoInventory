-- Get Calander Month

declare @CompanyID int = 1
declare @FiscalYear varchar(10) = '2025-1'
declare @CalanderMonth date 

declare @Year int
declare @Month int

--select @FiscalYear 
select @Year = CONVERT(int, SUBSTRING(@FiscalYear,0,5))
select @Month = CONVERT(int, SUBSTRING(@FiscalYear,6,5))
--select @Year
--select @Month 


select * 
into #FiscalYearSetup
from FiscalYearSetup
where FiscalYear = @Year and FiscalPeriod = @Month AND CompanyID = @CompanyID

select @CalanderMonth = CalanderMonth  from #FiscalYearSetup
drop table #FiscalYearSetup

--select @CalanderMonth 'CalanderMonth'
-- End Get Calander Month

--SELECT c.CompanyID, c.Name, a.GLAccountNo,a.Description, v.VoucherDate, v.FiscalYear, vd.Debit, vd.Credit
--FROM Company c 
--	INNER JOIN GLVoucher v ON c.CompanyID = v.CompanyID
--	INNER JOIN GLVoucherDetail vd ON v.CompanyID = vd.CompanyID AND v.GLVoucherID = vd.GLVoucherID
--	INNER JOIN GLAccount a ON vd.CompanyID = a.CompanyID AND vd.GLVoucherID = a.GLAccountID
--WHERE 
--	c.CompanyID = 1 --and v.VoucherDate <= @CalanderMonth -- '2024-07-31' -- '2024-07-04' --AND v.VoucherDate <= @CalanderMonth
--order by v.VoucherDate

select @CalanderMonth

SELECT c.CompanyID, c.Name, a.GLAccountNo,a.Description,v.GLVoucherID, v.VoucherDate, v.FiscalYear, vd.Debit, vd.Credit
FROM Company c 
	INNER JOIN GLVoucher v ON c.CompanyID = v.CompanyID
	INNER JOIN GLVoucherDetail vd ON v.CompanyID = vd.CompanyID AND v.GLVoucherID = vd.GLVoucherID
	INNER JOIN GLAccount a ON vd.CompanyID = a.CompanyID AND vd.GLVoucherID = a.GLAccountID
WHERE 
	c.CompanyID = 1 and v.VoucherDate <= @CalanderMonth
order by v.VoucherDate



SELECT c.CompanyID, c.Name, a.GLAccountNo,a.Description, Sum(vd.Debit) 'Debit', SUM(vd.Credit) 'Credit', (Sum(vd.Debit) - Sum(vd.Credit)) 'Total'
FROM Company c 
	INNER JOIN GLVoucher v ON c.CompanyID = v.CompanyID
	INNER JOIN GLVoucherDetail vd ON v.CompanyID = vd.CompanyID AND v.GLVoucherID = vd.GLVoucherID
	INNER JOIN GLAccount a ON vd.CompanyID = a.CompanyID AND vd.GLAccountID = a.GLAccountID
WHERE 
	c.CompanyID = 1 --and v.GLVoucherID = 22 --and v.VoucherDate <= @CalanderMonth
group by c.CompanyID, c.Name, a.GLAccountNo, a.Description
order by a.GLAccountNo



-----------------------------------------------------------

--select * from GLVoucher where GLVoucherID = 22
--select * from GLVoucherDetail where GLVoucherID = 22
--select * from GLAccount where CompanyID = 1 
--select * from GLVoucherDetail where CompanyID = 1

SELECT c.CompanyID,c.Name,
a.GLAccountNo,a.Description, 
v.GLVoucherID, v.VoucherDate, v.FiscalYear
,vd.GLAccountID, vd.Debit, vd.Credit
FROM GLVoucher v 
	INNER JOIN GLVoucherDetail vd ON v.CompanyID = vd.CompanyID AND v.GLVoucherID = vd.GLVoucherID
	INNER JOIN Company c ON v.CompanyID = c.CompanyID
	INNER JOIN GLAccount a ON vd.CompanyID = a.CompanyID AND vd.GLAccountID = a.GLAccountID
where v.CompanyID = 1 and a.GLAccountNo = '11-10-10-0101' -- v.GLVoucherID in(22,23,24)
order by a.GLAccountNo






	
