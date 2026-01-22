IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptINPORevertHistoryData')
	DROP PROC spRptINPORevertHistoryData
go


-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 26-Dec-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spRptINPORevertHistoryData]
	@ProjectID int,
	@FromDate date = NULL,
	@ToDate date = NULL
AS
BEGIN


SELECT 
	his.PORevertHistoryID,
	--c.Name AS Company, 
	his.PurchaseOrderID,
	his.ProjectID,
	p.ProjectName, 
	his.PurchaseOrderDate,
	grp.Name AS [Group], 
	cat.Name AS Category, 	
	his.Status,
	his.ItemID, 
	his.SrNo,
	i.Description AS ItemDescription, 
	s.Name AS Size, 
	uom.Name AS UOM, 
    his.Qty,
	his.Rate,
	his.Discount,
	his.TotalAmount,
	his.RevertedDate,
	his.RevertedQty,
	his.RevertedRate,
	his.RevertedDiscount,
	his.RevertedTotalAmount
	

FROM    INPORevertHistory his INNER JOIN 
		INItem i ON his.ItemID = i.ItemID INNER JOIN
		INCategory cat ON i.CategoryID = cat.CategoryID LEFT JOIN
        INGroup grp ON cat.GroupID = grp.GroupID LEFT JOIN
        INSize s ON i.SizeID = s.SizeID LEFT JOIN
        INUnitOfMeasurement uom ON i.UOMID = uom.UOMID INNER JOIN                          
      --  Company c ON his.CompanyID = c.CompanyID INNER JOIN
        INProject p ON his.ProjectID = p.ProjectID
WHERE 
	his.ProjectID = @ProjectID AND
	(CAST(his.PurchaseOrderDate as date) >= @FromDate OR @FromDate IS NULL) AND
	(CAST(his.PurchaseOrderDate as date) <= @ToDate OR @ToDate IS NULL) 

ORDER BY 
	p.ProjectName, 
	his.PurchaseOrderID,
	--[Group], 
	--Category,
	--Size, 
	--UOM,
	SrNo
	--ItemDescription



END

go



exec spRptINPORevertHistoryData 1, '2025-01-01', '2025-12-30'

select * from INPurchaseOrderDetail where PurchaseOrderID = 10021
select * from INPORevertHistory

--delete from INPORevertHistory



