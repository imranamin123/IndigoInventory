--SELECT 

--	FROM 
--		DVPaymentPlanDetail pd
--			INNER JOIN DVPaymentPlan pm	ON pd.PaymentPlanID = pm.PaymentPlanID
--			INNER JOIN DVApplicationForm f ON pm.ApplicationFormID = f.ApplicationFormID
--			INNER JOIN DVPaymentPlanType pt ON pd.PaymentPlanTypeID = pt.PaymentPlanTypeID
--			INNER JOIN Company c ON pm.CompanyID = c.CompanyID
--			INNER JOIN DVUnit u ON f.UnitID = u.UnitID
--			INNER JOIN DVUnitType ut ON u.UnitTypeID = ut.UnitTypeID
--			INNER JOIN DVProject prj ON f.ProjectID = prj.ProjectID
--	WHERE 
--		f.ApplicationFormID = @ApplicationFormID AND
--		(pd.PaymentPlanDetailDate <= @StatusDate OR @StatusDate IS NULL)
--	ORDER BY 
--		pd.PaymentPlanTypeID, 
--		pd.PaymentPlanDetailDate

--select * from DVProject 

select * from DVApplicationForm where ProjectID = 1
select * from DVPaymentPlan where ApplicationFormID in (select ApplicationFormID from DVApplicationForm where ProjectID = 1)
select * from DVPaymentPlanDetail where PaymentPlanID in (select PaymentPlanID from DVPaymentPlan where ApplicationFormID in (select ApplicationFormID from DVApplicationForm where ProjectID = 1))


select f.ProjectID, f.UnitID, ppd.PaymentPlanTypeID, ppt.PaymentPlanTypeCode ,ppd.Amount
	from DVApplicationForm f 
		inner join DVPaymentPlan pp on f.ApplicationFormID = pp.ApplicationFormID
		inner join DVPaymentPlanDetail ppd on pp.PaymentPlanID = ppd.PaymentPlanID
		inner join DVPaymentPlanType ppt on ppd.PaymentPlanTypeID = ppt.PaymentPlanTypeID
	where
		f.ProjectID = 1

select * from DVPaymentPlanType

