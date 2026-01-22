USE [GL]
GO
/****** Object:  StoredProcedure [dbo].[spBKBankTransSearchList]    Script Date: 7/12/2024 5:36:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDVReceiptSearchListSGO')
	DROP PROC spDVReceiptSearchListSGO
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 3-Dec-2024
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spDVReceiptSearchListSGO]
	@CompanyID int,
	@ProjectID int=null,
	@UnitID int=null
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT        
		dbo.DVReceipt.DVReceiptID, 
		dbo.DVReceipt.DVReceiptDate, 
		dbo.DVProject.ProjectName, 
		dbo.DVUnit.UnitNo, 
		dbo.DVReceipt.IsPosted,
		SUM(dbo.DVReceiptDetail.Amount) Amount

		--dbo.DVProject.ProjectID, 
		--dbo.DVUnit.UnitID
	FROM dbo.DVReceipt
		INNER JOIN dbo.DVApplicationForm ON dbo.DVReceipt.ApplicationFormID = dbo.DVApplicationForm.ApplicationFormID 
		INNER JOIN dbo.DVProject ON dbo.DVApplicationForm.ProjectID = dbo.DVProject.ProjectID 
		INNER JOIN dbo.DVUnit ON dbo.DVApplicationForm.UnitID = dbo.DVUnit.UnitID AND dbo.DVProject.ProjectID = dbo.DVUnit.ProjectID 
		left JOIN dbo.DVReceiptDetail ON dbo.DVReceipt.DVReceiptID = dbo.DVReceiptDetail.DVReceiptID


	WHERE 
		dbo.DVReceipt.CompanyID = @CompanyID AND
		(dbo.DVApplicationForm.ProjectID = @ProjectID OR @ProjectID IS NULL) AND
		(dbo.DVApplicationForm.UnitID = @UnitID OR @UnitID IS NULL) 
	GROUP BY
		dbo.DVReceipt.DVReceiptID, 
		dbo.DVReceipt.DVReceiptDate, 
		dbo.DVProject.ProjectName, 
		dbo.DVUnit.UnitNo,
		dbo.DVReceipt.IsPosted
	ORDER BY 
		dbo.DVReceipt.DVReceiptDate 

END

GO

exec spDVReceiptSearchList 1,1


