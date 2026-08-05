/*
    Repara la navegación de los catálogos geográficos en:
    Modulo de configuracion -> Definiciones -> General.

    Ejecutar solo en NuanSystem_Master después de 171.
    Conserva los Id, FormKey, permisos API y datos de las sucursales.
    Reactiva para ADMIN únicamente los menús y operaciones reales de estos CRUD.
*/
USE [NuanSystem_Master];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51178, 'Migration 178 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SecurityMenus', N'U') IS NULL
    THROW 51178, 'SecurityMenus is required before migration 178.', 1;
IF OBJECT_ID(N'dbo.SecurityRoleMenus', N'U') IS NULL
    THROW 51178, 'SecurityRoleMenus is required before migration 178.', 1;
IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL
    THROW 51178, 'SecurityForms is required before migration 178.', 1;
IF OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL
    THROW 51178, 'SecurityOperations is required before migration 178.', 1;
IF OBJECT_ID(N'dbo.SecurityRoleFormOperations', N'U') IS NULL
    THROW 51178, 'SecurityRoleFormOperations is required before migration 178.', 1;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
    THROW 51178, 'Roles is required before migration 178.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51178, 'MasterSchemaHistory is required before migration 178.', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260804.171')
    THROW 51178, 'Migration 171 is required before migration 178.', 1;
GO

BEGIN TRANSACTION;

DECLARE @ConfigurationMenuId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityMenus
    WHERE Code = N'MENU.CONFIGURATION' AND IsDeleted = 0
);
DECLARE @DefinitionsMenuId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityMenus
    WHERE Code = N'MENU.DEFINITIONS' AND IsDeleted = 0
);
DECLARE @GeneralMenuId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityMenus
    WHERE Code IN (N'MENU.GENERAL', N'MENU.GEOGRAPHY') AND IsDeleted = 0
    ORDER BY CASE WHEN Code = N'MENU.GENERAL' THEN 0 ELSE 1 END
);
DECLARE @AdminRoleId int =
(
    SELECT TOP (1) Id
    FROM dbo.Roles
    WHERE Code = N'ADMIN' AND IsDeleted = 0
);

IF @ConfigurationMenuId IS NULL
    THROW 51178, 'MENU.CONFIGURATION is required before migration 178.', 1;
IF @DefinitionsMenuId IS NULL
    THROW 51178, 'MENU.DEFINITIONS from migration 171 is required before migration 178.', 1;
IF @GeneralMenuId IS NULL
    THROW 51178, 'MENU.GEOGRAPHY or MENU.GENERAL is required before migration 178.', 1;
IF @AdminRoleId IS NULL
    THROW 51178, 'Active ADMIN role is required before migration 178.', 1;
IF EXISTS
(
    SELECT 1
    FROM dbo.SecurityMenus oldMenu
    INNER JOIN dbo.SecurityMenus newMenu
        ON newMenu.Code = N'MENU.GENERAL'
       AND newMenu.IsDeleted = 0
       AND newMenu.Id <> oldMenu.Id
    WHERE oldMenu.Code = N'MENU.GEOGRAPHY'
      AND oldMenu.IsDeleted = 0
)
    THROW 51178, 'MENU.GEOGRAPHY and MENU.GENERAL identify different active records.', 1;
IF EXISTS
(
    SELECT 1
    FROM dbo.SecurityMenus oldMenu
    INNER JOIN dbo.SecurityMenus newMenu
        ON newMenu.Code = REPLACE(oldMenu.Code, N'MENU.GEOGRAPHY.', N'MENU.GENERAL.')
       AND newMenu.IsDeleted = 0
       AND newMenu.Id <> oldMenu.Id
    WHERE oldMenu.Code IN
    (
        N'MENU.GEOGRAPHY.COUNTRIES',
        N'MENU.GEOGRAPHY.PROVINCES',
        N'MENU.GEOGRAPHY.CITIES'
    )
    AND oldMenu.IsDeleted = 0
)
    THROW 51178, 'Legacy and new General child menu codes identify different active records.', 1;

UPDATE dbo.SecurityMenus
SET ParentId = NULL,
    Name = N'Modulo de configuracion',
    MenuType = 1,
    FormKey = NULL,
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @ConfigurationMenuId;

UPDATE dbo.SecurityMenus
SET ParentId = @ConfigurationMenuId,
    Name = N'Definiciones',
    Description = N'Maestros y definiciones generales',
    MenuType = 2,
    FormKey = NULL,
    DisplayOrder = 20,
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @DefinitionsMenuId;

UPDATE dbo.SecurityMenus
SET ParentId = @DefinitionsMenuId,
    Code = N'MENU.GENERAL',
    Name = N'General',
    Description = N'Países, provincias y ciudades',
    MenuType = 2,
    FormKey = NULL,
    DisplayOrder = 10,
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @GeneralMenuId;

UPDATE dbo.SecurityMenus
SET ParentId = @GeneralMenuId,
    Code = CASE Code
        WHEN N'MENU.GEOGRAPHY.COUNTRIES' THEN N'MENU.GENERAL.COUNTRIES'
        WHEN N'MENU.GEOGRAPHY.PROVINCES' THEN N'MENU.GENERAL.PROVINCES'
        WHEN N'MENU.GEOGRAPHY.CITIES' THEN N'MENU.GENERAL.CITIES'
        ELSE Code
    END,
    MenuType = 3,
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Code IN
(
    N'MENU.GEOGRAPHY.COUNTRIES',
    N'MENU.GEOGRAPHY.PROVINCES',
    N'MENU.GEOGRAPHY.CITIES',
    N'MENU.GENERAL.COUNTRIES',
    N'MENU.GENERAL.PROVINCES',
    N'MENU.GENERAL.CITIES'
)
AND IsDeleted = 0;

DECLARE @RequiredMenus table (MenuId int NOT NULL PRIMARY KEY);
INSERT @RequiredMenus (MenuId)
SELECT Id
FROM dbo.SecurityMenus
WHERE Code IN
(
    N'MENU.CONFIGURATION',
    N'MENU.DEFINITIONS',
    N'MENU.GENERAL',
    N'MENU.GENERAL.COUNTRIES',
    N'MENU.GENERAL.PROVINCES',
    N'MENU.GENERAL.CITIES'
)
AND IsDeleted = 0;

IF (SELECT COUNT(1) FROM @RequiredMenus) <> 6
    THROW 51178, 'All six configuration geography menus are required before migration 178.', 1;

MERGE dbo.SecurityRoleMenus AS target
USING
(
    SELECT @AdminRoleId AS RoleId, MenuId
    FROM @RequiredMenus
) AS source
    ON target.RoleId = source.RoleId
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
    VALUES (source.RoleId, source.MenuId, 1, N'Sistema', SYSUTCDATETIME());

DECLARE @AllowedActions table (ActionKey nvarchar(120) NOT NULL PRIMARY KEY);
INSERT @AllowedActions (ActionKey)
VALUES
    (N'refresh'),
    (N'new'), (N'create'),
    (N'edit'), (N'update'),
    (N'delete'), (N'consult'), (N'copy'), (N'history'),
    (N'customize-columns'), (N'customizecolumns'),
    (N'export-excel'), (N'exportexcel'),
    (N'export-pdf'), (N'exportpdf'),
    (N'export-json'), (N'exportjson'),
    (N'export-xml'), (N'exportxml');

MERGE dbo.SecurityRoleFormOperations AS target
USING
(
    SELECT @AdminRoleId AS RoleId, form.Id AS FormId, operation.Id AS OperationId
    FROM dbo.SecurityForms form
    CROSS JOIN dbo.SecurityOperations operation
    INNER JOIN @AllowedActions allowed
        ON allowed.ActionKey = LOWER(LTRIM(RTRIM(operation.ActionKey)))
    WHERE form.FormKey IN (N'countries', N'provinces', N'cities')
      AND form.IsDeleted = 0
      AND form.IsActive = 1
      AND operation.IsDeleted = 0
      AND operation.IsActive = 1
) AS source
    ON target.RoleId = source.RoleId
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
    VALUES (source.RoleId, source.FormId, source.OperationId, 1, N'Sistema', SYSUTCDATETIME());

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260805.178'
)
BEGIN
    INSERT dbo.MasterSchemaHistory (Version, Description)
    VALUES
    (
        N'20260805.178',
        N'Repara Modulo de configuracion > Definiciones > General para geografia'
    );
END;

COMMIT TRANSACTION;
GO
