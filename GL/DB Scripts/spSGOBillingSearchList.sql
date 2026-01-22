USE [GL]
GO
/****** Object:  StoredProcedure [dbo].[spBKBankTransSearchList]    Script Date: 7/12/2024 5:36:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDVOBillingSearchList')
	DROP PROC spDVOBillingSearchList
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 6-Dec-2024
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spDVOBillingSearchList]
	@CompanyID int,  
	@ProjectID int=null,
	@UnitID int=null
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT        
		dbo.DVOBilling.BillingID, 
		dbo.DVOBilling.BillingDate, 
		dbo.DVProject.ProjectName, 
		dbo.DVUnit.UnitNo, 		
		SUM(dbo.DVOBillingDetail.Amount) Amount

	FROM dbo.DVOBilling LEFT JOIN
		 dbo.DVOBillingDetail ON dbo.DVOBilling.BillingID = dbo.DVOBillingDetail.BillingID INNER JOIN
         dbo.DVApplicationForm ON dbo.DVOBilling.ApplicationFormID = dbo.DVApplicationForm.ApplicationFormID INNER JOIN
         dbo.DVProject ON dbo.DVApplicationForm.ProjectID = dbo.DVProject.ProjectID INNER JOIN
         dbo.DVUnit ON dbo.DVApplicationForm.UnitID = dbo.DVUnit.UnitID AND dbo.DVProject.ProjectID = dbo.DVUnit.ProjectID 

	WHERE 
		dbo.DVOBilling.CompanyID = @CompanyID AND
		(dbo.DVApplicationForm.ProjectID = @ProjectID OR @ProjectID IS NULL) AND
		(dbo.DVApplicationForm.UnitID = @UnitID OR @UnitID IS NULL) 
	GROUP BY
		dbo.DVOBilling.BillingID, 
		dbo.DVOBilling.BillingDate, 
		dbo.DVProject.ProjectName, 
		dbo.DVUnit.UnitNo
	ORDER BY 
		dbo.DVOBilling.BillingDate 

END

GO

exec spDVOBillingSearchList 1


