IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('INPurchaseOrderDetail') AND name = 'Tax')
	ALTER TABLE INPurchaseOrderDetail ADD Tax decimal(18, 4) NULL
ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('INPurchaseOrderDetail') AND name = 'Tax' AND (precision <> 18 OR scale <> 4))
	ALTER TABLE INPurchaseOrderDetail ALTER COLUMN Tax decimal(18, 4) NULL
go
