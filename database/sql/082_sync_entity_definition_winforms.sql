/*
    Ejecutar en NuanSystem_Master.

    Publica el mantenimiento WinForms de entidades de sincronizacion y expone
    el historial de auditoria del catalogo tecnico.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
BEGIN
    THROW 51091, 'Este script debe ejecutarse en NuanSystem_Master.', 1;
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
   OR OBJECT_ID(N'dbo.AuditSyncConfigurationChanges', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityMenus', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityRoleMenus', N'U') IS NULL
BEGIN
    THROW 51092, 'Falta instalar la infraestructura requerida por el mantenimiento de entidades Sync.', 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCENTITYDEFINITIONHISTORIAL
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        audit.Id,
        audit.EntityName,
        audit.RecordId,
        audit.[Action],
        audit.FieldName,
        audit.OldValue,
        audit.NewValue,
        audit.UserId,
        audit.UserName,
        audit.[Source],
        audit.CreatedAt
    FROM dbo.AuditSyncConfigurationChanges audit
    WHERE audit.EntityName = N'SyncEntityDefinitions'
      AND audit.RecordId = CONVERT(nvarchar(80), @Id)
    ORDER BY audit.CreatedAt DESC, audit.Id DESC;
END;
GO

DECLARE @IntegrationsMenuId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityMenus
    WHERE Code = N'MENU.ADMINISTRATION.INTEGRATIONS'
      AND IsDeleted = 0
);
DECLARE @SyncEntitiesFormId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityForms
    WHERE FormKey = N'sync-entities'
      AND IsDeleted = 0
);

IF @IntegrationsMenuId IS NULL OR @SyncEntitiesFormId IS NULL
BEGIN
    THROW 51093, 'No se encontro el menu Integraciones o el formulario sync-entities.', 1;
END;

BEGIN TRANSACTION;

MERGE dbo.SecurityMenus AS target
USING
(
    SELECT
        @IntegrationsMenuId AS ParentId,
        N'MENU.ADMINISTRATION.INTEGRATIONS.SYNC.ENTITIES' AS Code,
        N'Entidades de sincronizacion' AS Name,
        N'Catalogo tecnico de entidades disponibles para perfiles Maestro-Sucursal.' AS Description,
        @SyncEntitiesFormId AS FormId,
        N'sync-entities' AS FormKey,
        15 AS DisplayOrder
) AS source
    ON target.Code = source.Code OR target.FormKey = source.FormKey
WHEN MATCHED THEN
    UPDATE SET
        ParentId = source.ParentId,
        Code = source.Code,
        Name = source.Name,
        Description = source.Description,
        MenuType = 3,
        FormId = source.FormId,
        FormKey = source.FormKey,
        IconLarge = N'Accordion/sync_32.svg',
        IconSmall = N'Accordion/sync_16.svg',
        DisplayOrder = source.DisplayOrder,
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
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        source.ParentId, source.Code, source.Name, source.Description, 3, source.FormId, source.FormKey,
        N'Accordion/sync_32.svg', N'Accordion/sync_16.svg', source.DisplayOrder, 1, 1,
        N'Sistema', SYSUTCDATETIME()
    );

DECLARE @AdminRoleId int =
(
    SELECT TOP (1) Id
    FROM dbo.Roles
    WHERE Code = N'ADMIN'
      AND IsDeleted = 0
);
DECLARE @SyncEntitiesMenuId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityMenus
    WHERE FormKey = N'sync-entities'
      AND IsDeleted = 0
);

IF @AdminRoleId IS NOT NULL AND @SyncEntitiesMenuId IS NOT NULL
BEGIN
    MERGE dbo.SecurityRoleMenus AS target
    USING (SELECT @SyncEntitiesMenuId AS MenuId) AS source
        ON target.RoleId = @AdminRoleId AND target.MenuId = source.MenuId
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
        INSERT (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, source.MenuId, 1, N'Sistema', SYSUTCDATETIME());
END;

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260716.082')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description, AppliedAt)
    VALUES (N'20260716.082', N'Menu WinForms e historial de entidades Sync', SYSUTCDATETIME());
END;

COMMIT TRANSACTION;
GO
