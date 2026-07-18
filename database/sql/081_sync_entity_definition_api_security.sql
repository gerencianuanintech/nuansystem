/*
    Ejecutar en NuanSystem_Master.

    Seguridad del mantenimiento administrativo de definiciones de entidades Sync.
    Registra permisos API y operaciones del futuro formulario sync-entities.
    No publica una opcion en SecurityMenus hasta que exista la pantalla WinForms.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
BEGIN
    THROW 51081, 'Este script debe ejecutarse en NuanSystem_Master.', 1;
END;
GO

IF OBJECT_ID(N'dbo.Modules', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Permissions', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Roles', N'U') IS NULL
   OR OBJECT_ID(N'dbo.RolePermissions', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityRoleFormOperations', N'U') IS NULL
BEGIN
    THROW 51082, 'Falta instalar la infraestructura de seguridad Master requerida.', 1;
END;
GO

DECLARE @RequiredOperations table (Code nvarchar(80) NOT NULL PRIMARY KEY);
INSERT INTO @RequiredOperations (Code)
VALUES
    (N'ACTION.REFRESH'),
    (N'ACTION.CONSULT'),
    (N'ACTION.CREATE'),
    (N'ACTION.UPDATE'),
    (N'ACTION.DELETE'),
    (N'ACTION.CUSTOMIZE_COLUMNS'),
    (N'ACTION.HISTORY');

IF EXISTS
(
    SELECT 1
    FROM @RequiredOperations required
    LEFT JOIN dbo.SecurityOperations operation
        ON operation.Code = required.Code
       AND operation.IsDeleted = 0
       AND operation.IsActive = 1
    WHERE operation.Id IS NULL
)
BEGIN
    THROW 51083, 'Faltan operaciones de seguridad requeridas; ejecute primero 072_sync_configuration_winforms_security.sql.', 1;
END;

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SYNC')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'SYNC', N'Sincronizacion Master/Sucursal', 70);
END;

DECLARE @SyncModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'SYNC');

MERGE dbo.Permissions AS target
USING
(
    VALUES
        (@SyncModuleId, N'SYNC.ENTITIES.VIEW', N'Sync Entities View', N'Consultar el catalogo de definiciones de entidades Sync.'),
        (@SyncModuleId, N'SYNC.ENTITIES.CREATE', N'Sync Entities Create', N'Crear definiciones administrables de entidades Sync.'),
        (@SyncModuleId, N'SYNC.ENTITIES.EDIT', N'Sync Entities Edit', N'Editar definiciones administrables de entidades Sync.'),
        (@SyncModuleId, N'SYNC.ENTITIES.DELETE', N'Sync Entities Delete', N'Eliminar definiciones no sistemicas y sin referencias Sync.')
) AS source (ModuleId, Code, Name, Description)
    ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        ModuleId = source.ModuleId,
        Name = source.Name,
        Description = source.Description,
        IsActive = 1,
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT (ModuleId, Code, Name, Description, IsActive)
    VALUES (source.ModuleId, source.Code, source.Name, source.Description, 1);

MERGE dbo.SecurityForms AS target
USING
(
    SELECT
        N'FORM.ADMINISTRATION.SYNC.ENTITIES' AS Code,
        N'Entidades de sincronizacion' AS Name,
        N'Catalogo tecnico administrable de entidades para perfiles Maestro-Sucursal.' AS Description,
        N'sync-entities' AS FormKey,
        CAST(1 AS tinyint) AS FormType
) AS source
    ON target.Code = source.Code OR target.FormKey = source.FormKey
WHEN MATCHED THEN
    UPDATE SET
        Code = source.Code,
        Name = source.Name,
        Description = source.Description,
        FormKey = source.FormKey,
        FormType = source.FormType,
        HasListView = 1,
        HasEditView = 1,
        IsVisible = 1,
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
        Code, Name, Description, FormKey, FormType, HasListView, HasEditView,
        IsVisible, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        source.Code, source.Name, source.Description, source.FormKey, source.FormType, 1, 1,
        1, 1, N'Sistema', SYSUTCDATETIME()
    );

DECLARE @AdminRoleId int =
(
    SELECT TOP (1) Id
    FROM dbo.Roles
    WHERE Code = N'ADMIN'
      AND IsDeleted = 0
);
DECLARE @SyncEntitiesFormId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityForms
    WHERE FormKey = N'sync-entities'
      AND IsDeleted = 0
);

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
    SELECT @AdminRoleId, permission.Id
    FROM dbo.Permissions permission
    WHERE permission.Code IN
    (
        N'SYNC.ENTITIES.VIEW',
        N'SYNC.ENTITIES.CREATE',
        N'SYNC.ENTITIES.EDIT',
        N'SYNC.ENTITIES.DELETE'
    )
      AND permission.IsActive = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.RolePermissions existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.PermissionId = permission.Id
      );

    MERGE dbo.SecurityRoleFormOperations AS target
    USING
    (
        SELECT @SyncEntitiesFormId AS FormId, operation.Id AS OperationId
        FROM dbo.SecurityOperations operation
        INNER JOIN @RequiredOperations required ON required.Code = operation.Code
        WHERE operation.IsDeleted = 0
          AND operation.IsActive = 1
          AND @SyncEntitiesFormId IS NOT NULL
    ) AS source
        ON target.RoleId = @AdminRoleId
       AND target.FormId = source.FormId
       AND target.OperationId = source.OperationId
    WHEN MATCHED THEN
        UPDATE SET
            IsAllowed = 1,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserName = N'Sistema',
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, source.FormId, source.OperationId, 1, N'Sistema', SYSUTCDATETIME());
END;

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260715.081')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description, AppliedAt)
    VALUES (N'20260715.081', N'Seguridad API del catalogo de entidades Sync', SYSUTCDATETIME());
END;

COMMIT TRANSACTION;
GO
