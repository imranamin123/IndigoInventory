IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('INPurchaseOrderDetail') AND name = 'Tax')
	ALTER TABLE INPurchaseOrderDetail ADD Tax decimal(18, 2) NULL
go
