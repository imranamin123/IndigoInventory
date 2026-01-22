IF EXISTS (SELECT * FROM sysObjects o WHERE o.[name] = 'spRptMemberPaymentPlanStatus_Receipt_AllUnits')
	DROP PROC spRptMemberPaymentPlanStatus_Receipt_AllUnits
go

-- ==========================================================================================
-- Author:		Imran Amin
-- Create date: 5-Oct-2024
-- Description:	
-- ==========================================================================================
-- ------------ Modification History --------------------------------------------------------
-- Author	Date		Details
-- ------   ----        -------
-- ------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[spRptMemberPaymentPlanStatus_Receipt_AllUnits]
	@ProjectID int,
	@StartDate date=null,
	@EndDate date=null
AS
BEGIN
	SET NOCOUNT ON;


SELECT u.UnitNo,
       main.ApplicationFormID,
       main.PaymentPlanTypeID,
       SUM(main.Amount) 'Balance'
FROM
(
    SELECT dbo.DVReceipt.ApplicationFormID,
           dbo.DVReceipt.DVReceiptDate 'TransDate',
           dbo.DVReceiptDetail.PaymentPlanTypeID,
           -dbo.DVReceiptDetail.Amount 'Amount'
    FROM dbo.DVReceiptDetail
        LEFT OUTER JOIN dbo.DVReceipt
            ON dbo.DVReceiptDetail.DVReceiptID = dbo.DVReceipt.DVReceiptID
    WHERE dbo.DVReceipt.DVReceiptDate <= '2024-11-01'
    Union all
    SELECT dbo.DVPaymentPlan.ApplicationFormID,
           dbo.DVPaymentPlanDetail.PaymentPlanDetailDate 'TransDate',
           dbo.DVPaymentPlanDetail.PaymentPlanTypeID,
           dbo.DVPaymentPlanDetail.Amount 'Amount'
    --, 
    --'P' 'status'
    FROM dbo.DVPaymentPlan
        RIGHT OUTER JOIN dbo.DVPaymentPlanDetail
            ON dbo.DVPaymentPlan.PaymentPlanID = dbo.DVPaymentPlanDetail.PaymentPlanID
    WHERE dbo.DVPaymentPlanDetail.PaymentPlanDetailDate <= '2024-11-01'
) main
    INNER JOIN DVApplicationForm f
        on main.ApplicationFormID = f.ApplicationFormID
    INNER JOIN DVUnit u
        on f.UnitID = u.UnitID
GROUP BY u.UnitNo,
         main.ApplicationFormID,
         main.PaymentPlanTypeID
ORDER BY UnitNo,
         PaymentPlanTypeID

END

GO

--exec spRptMemberPaymentPlanStatus_Receipt_AllUnits  1, '2024-01-01', '2024-12-30'


