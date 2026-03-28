IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spRptINItemRateComparisonList')
	DROP PROC spRptINItemRateComparisonList
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 16-Feb-2026
-- Description:	
-- ==========================================================================================
CREATE PROCEDURE spRptINItemRateComparisonList
	@CompanyID int,
	@ProjectID int
AS
BEGIN
	SET NOCOUNT ON;

	WITH RateCTE AS
	(
		SELECT 
			c.Name AS Company,
			p.ProjectName,
			grp.Name AS [Group], 
			cat.Name AS Category, 
			grnd.ItemID,
			i.Description AS ItemName,
			s.Name AS Size,
			uom.Name AS UOM,
			grnd.Rate,
			grn.GoodsReceiptNotesDate,
			ROW_NUMBER() OVER
			(
				PARTITION BY grnd.ItemID 
				ORDER BY grn.GoodsReceiptNotesDate DESC
			) AS RN
		FROM INGoodsReceiptNote grn
			INNER JOIN INGoodsReceiptNoteDetail grnd 
				ON grn.GoodsReceiptNoteID = grnd.GoodsReceiptNoteID
			INNER JOIN INItem i 
				ON grnd.ItemID = i.ItemID
			INNER JOIN Company c 
				ON grn.CompanyID = c.CompanyID
			INNER JOIN INProject p 
				ON grn.ProjectID = p.ProjectID
			INNER JOIN INCategory cat 
				ON i.CategoryID = cat.CategoryID
			LEFT JOIN INGroup grp 
				ON cat.GroupID = grp.GroupID
			LEFT JOIN INSize s 
				ON i.SizeID = s.SizeID
			LEFT JOIN INUnitOfMeasurement uom 
				ON i.UOMID = uom.UOMID
		WHERE grn.CompanyID = @CompanyID 
		AND grn.ProjectID = @ProjectID
	)

	SELECT
		Company,
		ProjectName,
		[Group],
		Category,
		ItemID,
		ItemName,
		Size,
		UOM,

		MAX(CASE WHEN RN = 1 THEN Rate END) AS LastRate,
		MAX(CASE WHEN RN = 1 THEN GoodsReceiptNotesDate END) AS LastRateDate,
		
		MAX(CASE WHEN RN = 2 THEN Rate END) AS LastRate2,
		MAX(CASE WHEN RN = 2 THEN GoodsReceiptNotesDate END) AS LastRateDate2,
		
		MAX(CASE WHEN RN = 3 THEN Rate END) AS LastRate3,
		MAX(CASE WHEN RN = 3 THEN GoodsReceiptNotesDate END) AS LastRateDate3,
		
		MAX(CASE WHEN RN = 4 THEN Rate END) AS LastRate4,
		MAX(CASE WHEN RN = 4 THEN GoodsReceiptNotesDate END) AS LastRateDate4
		

	FROM RateCTE
	WHERE RN <= 4
	GROUP BY
		Company,
		ProjectName,
		[Group],
		Category,
		ItemID,
		ItemName,
		Size,
		UOM
	ORDER BY ItemID

END
GO

exec spRptINItemRateComparisonList 1,1
select * from INGoodsReceiptNoteDetail where ItemID = 1076 and GoodsReceiptNoteID in  (select GoodsReceiptNoteID from INGoodsReceiptNote where ProjectID = 1)
order by GoodsReceiptNoteDetailID desc
