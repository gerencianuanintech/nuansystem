/*
    Migracion 147 - Acciones Ribbon para SRI TXT Import.

    Registra la accion de carga y vincula Filtro/Cargar al formulario para los
    roles que ya poseen el permiso API equivalente. No concede permisos API,
    menus ni acceso a roles que no los tengan.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51147, 'MasterSchemaHistory is required before migration 147.', 1;
IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL
    THROW 51147, 'SecurityForms is required before migration 147.', 1;
IF OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL
    THROW 51147, 'SecurityOperations is required before migration 147.', 1;
IF OBJECT_ID(N'dbo.SecurityRoleFormOperations', N'U') IS NULL
    THROW 51147, 'SecurityRoleFormOperations is required before migration 147.', 1;
IF OBJECT_ID(N'dbo.RolePermissions', N'U') IS NULL
    THROW 51147, 'RolePermissions is required before migration 147.', 1;
IF OBJECT_ID(N'dbo.Permissions', N'U') IS NULL
    THROW 51147, 'Permissions is required before migration 147.', 1;
GO

BEGIN TRY
BEGIN TRANSACTION;

DECLARE @FormId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityForms
    WHERE FormKey = N'sri-txt-imports'
      AND IsActive = 1
      AND IsDeleted = 0
);

IF @FormId IS NULL
    THROW 51147, 'The sri-txt-imports form is required before migration 147.', 1;

DECLARE @FilterOperationId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.FILTER'
      AND IsActive = 1
      AND IsDeleted = 0
);

IF @FilterOperationId IS NULL
    THROW 51147, 'ACTION.FILTER is required before migration 147.', 1;

IF EXISTS
(
    SELECT 1
    FROM dbo.SecurityOperations
    WHERE Name = N'Cargar TXT'
      AND Code <> N'ACTION.SRI_TXT_IMPORTS.UPLOAD'
      AND IsDeleted = 0
)
    THROW 51147, 'The Cargar TXT operation name belongs to another operation.', 1;

MERGE dbo.SecurityOperations AS target
USING
(
    SELECT
        N'ACTION.SRI_TXT_IMPORTS.UPLOAD' AS Code,
        N'Cargar TXT' AS Name,
        N'Cargar y validar un archivo TXT SRI sin encolarlo automaticamente.' AS Description,
        N'upload' AS ActionKey,
        N'Inicio' AS RibbonPageName,
        N'Acciones' AS RibbonGroupName,
        N'Operaciones/importar_32.svg' AS IconLarge,
        N'Operaciones/importar_16.svg' AS IconSmall,
        20 AS DisplayOrder
) AS source
ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        Name = source.Name,
        Description = source.Description,
        ActionKey = source.ActionKey,
        RibbonPageName = source.RibbonPageName,
        RibbonGroupName = source.RibbonGroupName,
        IconLarge = source.IconLarge,
        IconSmall = source.IconSmall,
        DisplayOrder = source.DisplayOrder,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        Code, Name, Description, ActionKey,
        RibbonPageName, RibbonGroupName, IconLarge, IconSmall, DisplayOrder,
        IsActive, CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.Code, source.Name, source.Description, source.ActionKey,
        source.RibbonPageName, source.RibbonGroupName, source.IconLarge, source.IconSmall, source.DisplayOrder,
        1, N'Sistema', SYSUTCDATETIME(), 0
    );

DECLARE @UploadOperationId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.SRI_TXT_IMPORTS.UPLOAD'
      AND IsActive = 1
      AND IsDeleted = 0
);

IF @UploadOperationId IS NULL
    THROW 51147, 'The SRI TXT upload operation could not be registered.', 1;

DECLARE @EnqueueOperationId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.SRI_TXT_IMPORTS.ENQUEUE'
      AND IsActive = 1
      AND IsDeleted = 0
);

IF @EnqueueOperationId IS NULL
    THROW 51147, 'ACTION.SRI_TXT_IMPORTS.ENQUEUE is required before migration 147.', 1;

DECLARE @OpenQueueOperationId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.SRI_TXT_IMPORTS.OPEN_QUEUE'
      AND IsActive = 1
      AND IsDeleted = 0
);

IF @OpenQueueOperationId IS NULL
    THROW 51147, 'ACTION.SRI_TXT_IMPORTS.OPEN_QUEUE is required before migration 147.', 1;

UPDATE dbo.SecurityOperations
SET RibbonPageName = N'Inicio',
    RibbonGroupName = N'Acciones',
    IconLarge = N'Ribbon/ejecutar_play_circulo_32.svg',
    IconSmall = N'Ribbon/ejecutar_play_circulo_16.svg',
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @EnqueueOperationId;

UPDATE dbo.SecurityOperations
SET RibbonPageName = N'Inicio',
    RibbonGroupName = N'Acciones',
    IconLarge = N'Operaciones/ver_detalle_32.svg',
    IconSmall = N'Operaciones/ver_detalle_16.svg',
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @OpenQueueOperationId;

DECLARE @RoleOperations table
(
    RoleId int NOT NULL,
    OperationId int NOT NULL,
    PRIMARY KEY (RoleId, OperationId)
);

INSERT @RoleOperations(RoleId, OperationId)
SELECT DISTINCT rolePermission.RoleId, @FilterOperationId
FROM dbo.RolePermissions rolePermission
INNER JOIN dbo.Permissions permission ON permission.Id = rolePermission.PermissionId
WHERE permission.Code = N'SRI.TXT_IMPORTS.VIEW'
  AND permission.IsActive = 1
UNION
SELECT DISTINCT rolePermission.RoleId, @UploadOperationId
FROM dbo.RolePermissions rolePermission
INNER JOIN dbo.Permissions permission ON permission.Id = rolePermission.PermissionId
WHERE permission.Code = N'SRI.TXT_IMPORTS.UPLOAD'
  AND permission.IsActive = 1
UNION
SELECT DISTINCT rolePermission.RoleId, @EnqueueOperationId
FROM dbo.RolePermissions rolePermission
INNER JOIN dbo.Permissions permission ON permission.Id = rolePermission.PermissionId
WHERE permission.Code = N'SRI.TXT_IMPORTS.ENQUEUE'
  AND permission.IsActive = 1
UNION
SELECT DISTINCT rolePermission.RoleId, @OpenQueueOperationId
FROM dbo.RolePermissions rolePermission
INNER JOIN dbo.Permissions permission ON permission.Id = rolePermission.PermissionId
WHERE permission.Code = N'SRI.DOCUMENTS.VIEW'
  AND permission.IsActive = 1;

INSERT dbo.SecurityRoleFormOperations
(
    RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt
)
SELECT
    source.RoleId,
    @FormId,
    source.OperationId,
    1,
    N'Sistema',
    SYSUTCDATETIME()
FROM @RoleOperations source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SecurityRoleFormOperations existing
    WHERE existing.RoleId = source.RoleId
      AND existing.FormId = @FormId
      AND existing.OperationId = source.OperationId
);

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260728.147'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260728.147',
        N'Acciones Ribbon y carga de archivos para SRI TXT Import'
    );
END;

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
