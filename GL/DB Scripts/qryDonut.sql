


select COUNT(*) unit, 'sold' status from DVUnit 
where ProjectID = 1
and  Cancelled = 0
and IsActive = 0

union all

select COUNT(*) status, 'unsold' status from DVUnit 
where ProjectID = 1
and  Cancelled = 0
and IsActive = 1

select * from DVUnit


