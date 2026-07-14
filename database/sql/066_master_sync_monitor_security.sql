/*
    Ejecutar en NuanSystem_Master.
    Seed de seguridad para Monitor Sync Master/Sucursal WinForms.

    Reglas:
    - FormKey: sync-monitor.
    - Pantalla solo lectura; no registra acciones manuales de reintento ni liberacion.
    - El acceso funcional se gobierna por SYNC.OUTBOX.VIEW y SYNC.AUDIT.VIEW.
*/

SET NOCOUNT ON;
GO

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @AdministrationMenuId int;
DECLARE @SyncMenuId int;
DECLARE @SyncMonitorMenuId int;
DECLARE @SyncMonitorFormId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.ADMINISTRATION', N'Administracion', N'Administracion y monitoreo del sistema',
        1, NULL, NULL, N'Accordion/settings_32.svg', N'Accordion/settings_16.svg',
        15, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'Administracion',
    Description = N'Administracion y monitoreo del sistema',
    MenuType = 1,
    FormId = NULL,
    FormKey = NULL,
    DisplayOrder = 15,
    IsVisible = 1,
    IsActive = 1,
    IsDeleted = 0,
    DeletedByUserId = NULL,
    DeletedByUserName = NULL,
    DeletedAt = NULL,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Code = N'MENU.ADMINISTRATION';

SET @AdministrationMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION' AND IsDeleted = 0);

IF @AdministrationMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION.SYNC' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @AdministrationMenuId, N'MENU.ADMINISTRATION.SYNC', N'Sincronizacion',
            N'Monitoreo de sincronizacion Master/Sucursal', 2, NULL, NULL,
            N'Accordion/sync_32.svg', N'Accordion/sync_16.svg',
            30, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @AdministrationMenuId,
        Name = N'Sincronizacion',
        Description = N'Monitoreo de sincronizacion Master/Sucursal',
        MenuType = 2,
        FormId = NULL,
        FormKey = NULL,
        DisplayOrder = 30,
        IsVisible = 1,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Code = N'MENU.ADMINISTRATION.SYNC';
END;

SET @SyncMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION.SYNC' AND IsDeleted = 0);

SET @SyncMonitorFormId =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityForms
    WHERE IsDeleted = 0
      AND (FormKey = N'sync-monitor' OR Code = N'FORM.ADMINISTRATION.SYNC.MONITOR')
    ORDER BY CASE WHEN FormKey = N'sync-monitor' THEN 0 ELSE 1 END, Id
);

IF @SyncMonitorFormId IS NULL
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, HasListView, HasEditView,
        IsVisible, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.ADMINISTRATION.SYNC.MONITOR', N'Monitor Sync',
        N'Monitoreo solo lectura de Sync Master/Sucursal.', N'sync-monitor', 3, 1, 0,
        1, 1, N'Sistema', SYSUTCDATETIME()
    );

    SET @SyncMonitorFormId = CONVERT(int, SCOPE_IDENTITY());
END;
ELSE
BEGIN
    UPDATE dbo.SecurityForms
    SET Code = N'FORM.ADMINISTRATION.SYNC.MONITOR',
        Name = N'Monitor Sync',
        Description = N'Monitoreo solo lectura de Sync Master/Sucursal.',
        FormKey = N'sync-monitor',
        FormType = 3,
        HasListView = 1,
        HasEditView = 0,
        IsVisible = 1,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @SyncMonitorFormId;
END;

IF @SyncMenuId IS NOT NULL AND @SyncMonitorFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION.SYNC.MONITOR' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @SyncMenuId, N'MENU.ADMINISTRATION.SYNC.MONITOR', N'Monitor Sync',
            N'Monitoreo solo lectura de Sync Master/Sucursal.', 3, @SyncMonitorFormId, N'sync-monitor',
            N'Accordion/sync_32.svg', N'Accordion/sync_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @SyncMenuId,
        Name = N'Monitor Sync',
        Description = N'Monitoreo solo lectura de Sync Master/Sucursal.',
        MenuType = 3,
        FormId = @SyncMonitorFormId,
        FormKey = N'sync-monitor',
        DisplayOrder = 10,
        IsVisible = 1,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Code = N'MENU.ADMINISTRATION.SYNC.MONITOR';
END;

SET @SyncMonitorMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION.SYNC.MONITOR' AND IsDeleted = 0);

DECLARE @Operations table
(
    Code nvarchar(80) NOT NULL PRIMARY KEY,
    Name nvarchar(120) NOT NULL,
    Description nvarchar(300) NOT NULL,
    RibbonPageName nvarchar(80) NULL,
    RibbonGroupName nvarchar(80) NULL,
    ActionKey nvarchar(120) NOT NULL,
    DisplayOrder int NOT NULL
);

INSERT INTO @Operations (Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey, DisplayOrder)
VALUES
    (N'ACTION.REFRESH', N'Actualizar', N'Recargar informacion del formulario.', N'Inicio', N'Datos', N'refresh', 10),
    (N'ACTION.CONSULT', N'Consultar', N'Consultar registros del formulario.', N'Inicio', N'Acciones', N'consult', 20),
    (N'ACTION.EXPORT', N'Exportar', N'Exportar informacion.', N'Inicio', N'Datos', N'export', 110);

MERGE dbo.SecurityOperations AS target
USING @Operations AS source
    ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        Name = source.Name,
        Description = source.Description,
        RibbonPageName = source.RibbonPageName,
        RibbonGroupName = source.RibbonGroupName,
        ActionKey = source.ActionKey,
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
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        DisplayOrder, IsActive, CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.Code, source.Name, source.Description, source.RibbonPageName, source.RibbonGroupName, source.ActionKey,
        source.DisplayOrder, 1, N'Sistema', SYSUTCDATETIME(), 0
    );

IF @AdminRoleId IS NOT NULL
BEGIN
    MERGE dbo.SecurityRoleMenus AS target
    USING
    (
        SELECT Id AS MenuId
        FROM dbo.SecurityMenus
        WHERE Id IN (@AdministrationMenuId, @SyncMenuId, @SyncMonitorMenuId)
          AND Id IS NOT NULL
          AND IsDeleted = 0
    ) AS source
    ON target.RoleId = @AdminRoleId
       AND target.MenuId = source.MenuId
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

    MERGE dbo.SecurityRoleFormOperations AS target
    USING
    (
        SELECT @SyncMonitorFormId AS FormId, operation.Id AS OperationId
        FROM dbo.SecurityOperations operation
        WHERE @SyncMonitorFormId IS NOT NULL
          AND operation.IsDeleted = 0
          AND operation.IsActive = 1
          AND operation.ActionKey IN (N'refresh', N'consult', N'export')
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

    INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
    SELECT @AdminRoleId, permission.Id
    FROM dbo.Permissions permission
    WHERE permission.Code IN (N'SYNC.OUTBOX.VIEW', N'SYNC.AUDIT.VIEW')
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.RolePermissions existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.PermissionId = permission.Id
      );
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260709.06')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260709.06', N'Fase Frontend 1: seguridad y menu Monitor Sync Master/Sucursal');
END;
GO
