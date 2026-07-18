SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL
    THROW 51105,'Ejecute primero 080_sync_entity_definitions.sql.',1;

UPDATE dbo.SyncEntityDefinitions
SET SupportsInsert=1,
    SupportsUpdate=1,
    SupportsDeactivate=1,
    SupportsIncremental=CASE WHEN Code=N'PurchaseOrder' THEN 1 ELSE SupportsIncremental END,
    Description=CASE Code
        WHEN N'PurchaseOrder' THEN N'Documento operativo SAP importado en Master y enrutado a una unica sucursal por bodega.'
        ELSE N'Catalogo de referencia con productor Full y aplicador idempotente Master-Sucursal.'
    END,
    UpdatedByUserName=N'Fase9Pilot',
    UpdatedAt=SYSUTCDATETIME()
WHERE Code IN(N'Tax',N'UnitOfMeasure',N'PriceList',N'PurchaseOrder')
  AND IsDeleted=0;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NOT NULL
   AND NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260718.105')
BEGIN
    INSERT dbo.MasterSchemaHistory(Version,Description)
    VALUES(N'20260718.105',N'Activa catalogos de referencia y ordenes en sincronizacion Master-Sucursal');
END;
GO
