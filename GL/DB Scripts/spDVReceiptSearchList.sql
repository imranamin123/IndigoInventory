USE [GL]
GO
/****** Object:  StoredProcedure [dbo].[spBKBankTransSearchList]    Script Date: 7/12/2024 5:36:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spDVReceiptSearchList')
	DROP PROC spDVReceiptSearchList
go

-- ==========================================================================================
-- Created By:	Imran Amin
-- Create date: 04-Sep-2024
-- Description:	
-- ==========================================================================================

CREATE procedure [dbo].[spDVReceiptSearchList]
	@CompanyID int,
	@ProjectID int=null,
	@UnitID int=null
AS
BEGIN
	
	SET NOCOUNT ON;

SELECT 
		DVReceipt.CompanyID,
		DVApplicationForm.ProjectID,
		DVApplicationForm.UnitID,
		DVApplicationForm.ApplicationFormID,
		DVReceipt.DVReceiptDate,
		DVReceipt.DVReceiptID,
		DVProject.ProjectName,
		DVUnit.UnitNo,
		DVReceipt.IsPosted,
		SUM(DVReceiptDetail.Amount) AmountTotal
		

FROM DVApplicationForm
	inner join DVProject ON DVApplicationForm.ProjectID = DVProject.ProjectID
	inner join DVUnit oN DVApplicationForm.UnitID = DVUnit.UnitID
	inner join DVReceipt ON DVApplicationForm.[ApplicationFormID] = DVReceipt.ApplicationFormID
	left join DVReceiptDetail ON DVReceipt.DVReceiptID = DVReceiptDetail.DVReceiptID
	
GROUP BY
		DVReceipt.CompanyID,
		DVApplicationForm.ProjectID,
		DVApplicationForm.UnitID,
		DVApplicationForm.ApplicationFormID,
		DVReceipt.DVReceiptDate,
		DVReceipt.DVReceiptID,
		DVProject.ProjectName,
		DVUnit.UnitNo,
		DVReceipt.IsPosted
	HAVING 
		DVReceipt.CompanyID = @CompanyID AND
		(DVApplicationForm.ProjectID = @ProjectID OR @ProjectID IS NULL) AND
		(DVApplicationForm.UnitID = @UnitID OR @UnitID IS NULL) 


	ORDER BY 
		DVReceipt.CompanyID,
		DVApplicationForm.ProjectID,
		DVApplicationForm.UnitID,
		DVReceipt.DVReceiptDate

END

GO

exec spDVReceiptSearchList 3,2,194



select * from GLAccount where CompanyID = 2
