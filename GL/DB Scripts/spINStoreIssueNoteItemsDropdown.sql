IF EXISTS (SELECT * FROM sysObjects o WHERE o.name = 'spINStoreIssueNoteItemsDropdown')
	DROP PROC spINStoreIssueNoteItemsDropdown
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 22-Jul-2025
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spINStoreIssueNoteItemsDropdown]
	@ProjectID int,
	@CompanyID int
AS
BEGIN

	SET NOCOUNT On;

	SELECT *
    FROM
    (
        SELECT
            i.ItemID,

            -- Description column
            -- QtyInHand is already the item's full current balance (see spRptINItemStock,
            -- which folds OpeningQty into the running ClosingQty it computes). Adding
            -- OpeningQty again here double-counts it and can show stock that isn't really there.
            i.Description + ' = ' + CAST(ISNULL(p.QtyInHand, 0) AS VARCHAR(50)) AS Description,

            -- Balance column
            ISNULL(p.QtyInHand, 0) AS Balance

        FROM INItem i
        LEFT JOIN INProjectItem p 
            ON i.ItemID = p.ItemID
        WHERE
            i.CompanyID = @CompanyID
            AND p.ProjectID = @ProjectID
    ) main
    WHERE main.Balance > 0
    ORDER BY main.Description;

	--SELECT * FROM(
	--	SELECT 
	--	i.ItemID,
	--	i.Description  + ' = ' + CAST((isnull(p.QtyInHand,0)) as varchar(500)) 'Description',	
	--	--i.Description  + ' = ' + CAST((isnull(p.QtyInHand,0) + isnull(p.OpeningQty,0)) as varchar(500)) 'Description',	
	--	--isnull(p.QtyInHand,0) + isnull(p.OpeningQty,0)	'Balance'		
	--	isnull(p.QtyInHand,0) 'Balance'		

	--FROM INItem i
	--LEFT  JOIN INProjectItem p ON i.ItemID = p.ItemID 
	--WHERE 
	--	i.CompanyID = @CompanyID 
	--	AND p.ProjectID = @ProjectID 
	--) main 

	--WHERE main.Balance > 0 
	--order by main.Description


END

go 

exec spINStoreIssueNoteItemsDropdown 5,1

select * from INProjectItem where ItemID = 1225 and ProjectID = 5

--select * from INItem


