/*
    Ejecutar en NuanSystem_Master.
    Seed de seguridad para configuracion y ejecuciones Sync Maestro/Sucursal WinForms.

    Reglas:
    - FormKeys: sync-profiles, sync-executions.
    - El frontend consume unicamente endpoints bajo /api/sync/configuration.
    - No crea tablas funcionales ni toca SyncOutbox/SyncInbox/worker.
*/

SET NOCOUNT ON;
GO

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @AdministrationMenuId int;
DECLARE @IntegrationsMenuId int;
DECLARE @SyncProfilesFormId int;
DECLARE @SyncExecutionsFormId int;
DECLARE @SyncProfilesMenuId int;
DECLARE @SyncExecutionsMenuId int;

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

SET @AdministrationMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION' AND IsDeleted = 0);

IF @AdministrationMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION.INTEGRATIONS' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @AdministrationMenuId, N'MENU.ADMINISTRATION.INTEGRATIONS', N'Integraciones',
            N'Configuracion de integraciones administrativas', 2, NULL, NULL,
            N'Accordion/sync_32.svg', N'Accordion/sync_16.svg',
            40, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @AdministrationMenuId,
        Name = N'Integraciones',
        Description = N'Configuracion de integraciones administrativas',
        MenuType = 2,
        FormId = NULL,
        FormKey = NULL,
        DisplayOrder = 40,
        IsVisible = 1,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Code = N'MENU.ADMINISTRATION.INTEGRATIONS';
END;

SET @IntegrationsMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION.INTEGRATIONS' AND IsDeleted = 0);

DECLARE @Forms table
(
    Code nvarchar(80) NOT NULL PRIMARY KEY,
    Name nvarchar(120) NOT NULL,
    Description nvarchar(300) NOT NULL,
    FormKey nvarchar(120) NOT NULL,
    FormType int NOT NULL,
    HasEditView bit NOT NULL
);

INSERT INTO @Forms (Code, Name, Description, FormKey, FormType, HasEditView)
VALUES
    (N'FORM.ADMINISTRATION.SYNC.PROFILES', N'Perfiles de sincronizacion', N'Configuracion Maestro - Sucursales.', N'sync-profiles', 1, 1),
    (N'FORM.ADMINISTRATION.SYNC.EXECUTIONS', N'Ejecuciones de sincronizacion', N'Monitoreo administrativo de ejecuciones Sync.', N'sync-executions', 3, 0);

MERGE dbo.SecurityForms AS target
USING @Forms AS source
    ON target.Code = source.Code OR target.FormKey = source.FormKey
WHEN MATCHED THEN
    UPDATE SET
        Code = source.Code,
        Name = source.Name,
        Description = source.Description,
        FormKey = source.FormKey,
        FormType = source.FormType,
        HasListView = 1,
        HasEditView = source.HasEditView,
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
        source.Code, source.Name, source.Description, source.FormKey, source.FormType, 1, source.HasEditView,
        1, 1, N'Sistema', SYSUTCDATETIME()
    );

SET @SyncProfilesFormId = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE FormKey = N'sync-profiles' AND IsDeleted = 0);
SET @SyncExecutionsFormId = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE FormKey = N'sync-executions' AND IsDeleted = 0);

IF @IntegrationsMenuId IS NOT NULL
BEGIN
    DECLARE @Menus table
    (
        Code nvarchar(80) NOT NULL PRIMARY KEY,
        Name nvarchar(120) NOT NULL,
        Description nvarchar(300) NOT NULL,
        FormId int NULL,
        FormKey nvarchar(120) NOT NULL,
        DisplayOrder int NOT NULL
    );

    INSERT INTO @Menus (Code, Name, Description, FormId, FormKey, DisplayOrder)
    VALUES
        (N'MENU.ADMINISTRATION.INTEGRATIONS.SYNC.PROFILES', N'Perfiles de sincronizacion', N'Configuracion Maestro - Sucursales.', @SyncProfilesFormId, N'sync-profiles', 10),
        (N'MENU.ADMINISTRATION.INTEGRATIONS.SYNC.EXECUTIONS', N'Ejecuciones', N'Monitoreo administrativo de ejecuciones Sync.', @SyncExecutionsFormId, N'sync-executions', 20);

    MERGE dbo.SecurityMenus AS target
    USING @Menus AS source
        ON target.Code = source.Code OR target.FormKey = source.FormKey
    WHEN MATCHED THEN
        UPDATE SET
            ParentId = @IntegrationsMenuId,
            Code = source.Code,
            Name = source.Name,
            Description = source.Description,
            MenuType = 3,
            FormId = source.FormId,
            FormKey = source.FormKey,
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
            @IntegrationsMenuId, source.Code, source.Name, source.Description, 3, source.FormId, source.FormKey,
            N'Accordion/sync_32.svg', N'Accordion/sync_16.svg', source.DisplayOrder, 1, 1,
            N'Sistema', SYSUTCDATETIME()
        );
END;

SET @SyncProfilesMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE FormKey = N'sync-profiles' AND IsDeleted = 0);
SET @SyncExecutionsMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE FormKey = N'sync-executions' AND IsDeleted = 0);

DECLARE @Operations table
(
    Code nvarchar(80) NOT NULL PRIMARY KEY,
    Name nvarchar(120) NOT NULL,
    ActionKey nvarchar(120) NOT NULL,
    IconLarge nvarchar(200) NULL,
    IconSmall nvarchar(200) NULL,
    DisplayOrder int NOT NULL
);

INSERT INTO @Operations (Code, Name, ActionKey, IconLarge, IconSmall, DisplayOrder)
VALUES
    (N'ACTION.REFRESH', N'Actualizar', N'refresh', NULL, NULL, 10),
    (N'ACTION.CREATE', N'Crear', N'create', N'Ribbon/nuevo_32.svg', N'Ribbon/nuevo_16.svg', 20),
    (N'ACTION.UPDATE', N'Actualizar registro', N'update', N'Ribbon/editar_32.svg', N'Ribbon/editar_16.svg', 30),
    (N'ACTION.DELETE', N'Eliminar', N'delete', NULL, NULL, 40),
    (N'ACTION.CONSULT', N'Consultar', N'consult', NULL, NULL, 50),
    (N'ACTION.COPY', N'Copiar', N'copy', NULL, NULL, 52),
    (N'ACTION.ACTIVATE', N'Activar', N'activate', N'Ribbon/activar_toggle_on_32.svg', N'Ribbon/activar_toggle_on_16.svg', 55),
    (N'ACTION.DEACTIVATE', N'Desactivar', N'deactivate', N'Ribbon/desactivar_toggle_off_32.svg', N'Ribbon/desactivar_toggle_off_16.svg', 56),
    (N'ACTION.EXECUTE', N'Ejecutar', N'execute', N'Ribbon/ejecutar_play_circulo_32.svg', N'Ribbon/ejecutar_play_circulo_16.svg', 60),
    (N'ACTION.VIEWEXECUTIONS', N'Ver ejecuciones', N'view-executions', N'Ribbon/ver_ejecuciones_lista_32.svg', N'Ribbon/ver_ejecuciones_lista_16.svg', 65),
    (N'ACTION.CANCEL', N'Anular', N'cancel', NULL, NULL, 70),
    (N'ACTION.RETRY', N'Reintentar', N'retry', NULL, NULL, 80),
    (N'ACTION.FILTER', N'Filtro', N'filter', N'Ribbon/filtro_32.svg', N'Ribbon/filtro_16.svg', 84),
    (N'ACTION.CUSTOMIZE_COLUMNS', N'Columnas', N'customize-columns', NULL, NULL, 85),
    (N'ACTION.VALIDATE', N'Validar', N'validate', N'Ribbon/validar_cuadro_check_32.svg', N'Ribbon/validar_cuadro_check_16.svg', 90),
    (N'ACTION.HISTORY', N'Historial', N'history', NULL, NULL, 95);

MERGE dbo.SecurityOperations AS target
USING @Operations AS source
    ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        Name = source.Name,
        Description = source.Name,
        ActionKey = source.ActionKey,
        IconLarge = COALESCE(source.IconLarge, target.IconLarge),
        IconSmall = COALESCE(source.IconSmall, target.IconSmall),
        DisplayOrder = source.DisplayOrder,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT (Code, Name, Description, ActionKey, IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt, IsDeleted)
    VALUES (source.Code, source.Name, source.Name, source.ActionKey, source.IconLarge, source.IconSmall, source.DisplayOrder, 1, N'Sistema', SYSUTCDATETIME(), 0);

IF @AdminRoleId IS NOT NULL
BEGIN
    MERGE dbo.SecurityRoleMenus AS target
    USING
    (
        SELECT Id AS MenuId
        FROM dbo.SecurityMenus
        WHERE Id IN (@AdministrationMenuId, @IntegrationsMenuId, @SyncProfilesMenuId, @SyncExecutionsMenuId)
          AND Id IS NOT NULL
          AND IsDeleted = 0
    ) AS source
    ON target.RoleId = @AdminRoleId AND target.MenuId = source.MenuId
    WHEN MATCHED THEN
        UPDATE SET IsAllowed = 1, IsDeleted = 0, UpdatedByUserName = N'Sistema', UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, source.MenuId, 1, N'Sistema', SYSUTCDATETIME());

    MERGE dbo.SecurityRoleFormOperations AS target
    USING
    (
        SELECT @SyncProfilesFormId AS FormId, Id AS OperationId
        FROM dbo.SecurityOperations
        WHERE Code IN (N'ACTION.REFRESH', N'ACTION.CREATE', N'ACTION.UPDATE', N'ACTION.DELETE', N'ACTION.CONSULT', N'ACTION.COPY', N'ACTION.ACTIVATE', N'ACTION.DEACTIVATE', N'ACTION.EXECUTE', N'ACTION.VIEWEXECUTIONS', N'ACTION.FILTER', N'ACTION.CUSTOMIZE_COLUMNS', N'ACTION.VALIDATE', N'ACTION.HISTORY')
          AND @SyncProfilesFormId IS NOT NULL
          AND IsDeleted = 0
        UNION ALL
        SELECT @SyncExecutionsFormId AS FormId, Id AS OperationId
        FROM dbo.SecurityOperations
        WHERE Code IN (N'ACTION.REFRESH', N'ACTION.CONSULT', N'ACTION.CANCEL', N'ACTION.RETRY')
          AND @SyncExecutionsFormId IS NOT NULL
          AND IsDeleted = 0
    ) AS source
    ON target.RoleId = @AdminRoleId
       AND target.FormId = source.FormId
       AND target.OperationId = source.OperationId
    WHEN MATCHED THEN
        UPDATE SET IsAllowed = 1, IsDeleted = 0, UpdatedByUserName = N'Sistema', UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, source.FormId, source.OperationId, 1, N'Sistema', SYSUTCDATETIME());
END;

IF @AdminRoleId IS NOT NULL
BEGIN
    DECLARE @PermissionCodes table (Code nvarchar(120) NOT NULL PRIMARY KEY);
    INSERT INTO @PermissionCodes (Code)
    VALUES
        (N'SYNC.CONFIGURATION.VIEW'),
        (N'SYNC.CONFIGURATION.CREATE'),
        (N'SYNC.CONFIGURATION.EDIT'),
        (N'SYNC.CONFIGURATION.DELETE'),
        (N'SYNC.CONFIGURATION.ACTIVATE'),
        (N'SYNC.CONFIGURATION.VALIDATE'),
        (N'SYNC.CONFIGURATION.EXECUTE'),
        (N'SYNC.CONFIGURATION.VIEWEXECUTIONS'),
        (N'SYNC.CONFIGURATION.CANCEL'),
        (N'SYNC.CONFIGURATION.RETRY');

    INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
    SELECT @AdminRoleId, permission.Id
    FROM dbo.Permissions permission
    INNER JOIN @PermissionCodes codes ON codes.Code = permission.Code
    WHERE permission.IsActive = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.RolePermissions existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.PermissionId = permission.Id
      );
END;

GO
