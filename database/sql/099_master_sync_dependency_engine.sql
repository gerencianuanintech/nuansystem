/*
    Registra definiciones futuras y dependencias iniciales del motor
    Master-Sucursal. No crea productores/aplicadores ni activa perfiles.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NULL
BEGIN
    THROW 51111, 'Ejecute primero 080_sync_entity_definitions.sql.', 1;
END;
GO

DECLARE @Definitions TABLE
(
    Code nvarchar(80) NOT NULL,
    Name nvarchar(120) NOT NULL,
    Description nvarchar(500) NOT NULL,
    DefaultExecutionOrder int NOT NULL
);

INSERT INTO @Definitions (Code, Name, Description, DefaultExecutionOrder)
VALUES
    (N'Tax', N'Impuestos', N'Definicion planificada; requiere productor y aplicador antes de usarse en perfiles activos.', 110),
    (N'UnitOfMeasure', N'Unidades de medida', N'Definicion planificada; requiere productor y aplicador antes de usarse en perfiles activos.', 120),
    (N'PriceList', N'Listas de precios', N'Definicion planificada; requiere productor y aplicador antes de usarse en perfiles activos.', 230),
    (N'PurchaseOrder', N'Ordenes de compra', N'Documento operativo planificado; no se trata como CRUD ni se habilita sin sus maestros.', 300);

INSERT INTO dbo.SyncEntityDefinitions
(
    Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
    SupportsInsert, SupportsUpdate, SupportsDeactivate, DefaultKeyField,
    DefaultModifiedAtField, IsSystem, IsActive, CreatedByUserName
)
SELECT
    source.Code, source.Name, source.Description, source.DefaultExecutionOrder,
    0, 0, 0, 0, N'Code', N'UpdatedAt', 1, 1, N'Sistema'
FROM @Definitions AS source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SyncEntityDefinitions AS existing
    WHERE existing.Code = source.Code
);
GO

DECLARE @Dependencies TABLE
(
    EntityCode nvarchar(80) NOT NULL,
    DependsOnCode nvarchar(80) NOT NULL
);

INSERT INTO @Dependencies (EntityCode, DependsOnCode)
VALUES
    (N'PriceList', N'Currencies'),
    (N'PriceList', N'Item'),
    (N'PurchaseOrder', N'Currencies'),
    (N'PurchaseOrder', N'Tax'),
    (N'PurchaseOrder', N'UnitOfMeasure'),
    (N'PurchaseOrder', N'BusinessPartner'),
    (N'PurchaseOrder', N'Item'),
    (N'PurchaseOrder', N'Warehouse'),
    (N'PurchaseOrder', N'PriceList');

INSERT INTO dbo.SyncEntityDefinitionDependencies
(
    EntityDefinitionId, DependsOnEntityDefinitionId, CreatedByUserName
)
SELECT entity.Id, required.Id, N'Sistema'
FROM @Dependencies AS source
INNER JOIN dbo.SyncEntityDefinitions AS entity
    ON entity.Code = source.EntityCode AND entity.IsDeleted = 0
INNER JOIN dbo.SyncEntityDefinitions AS required
    ON required.Code = source.DependsOnCode AND required.IsDeleted = 0
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SyncEntityDefinitionDependencies AS existing
    WHERE existing.EntityDefinitionId = entity.Id
      AND existing.DependsOnEntityDefinitionId = required.Id
      AND existing.IsDeleted = 0
);
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260718.099')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260718.099', N'Motor inicial de dependencias de sincronizacion Master-Sucursal');
END;
GO
