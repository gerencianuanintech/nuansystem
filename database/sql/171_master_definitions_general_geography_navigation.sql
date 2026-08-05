/*
    Reubica los catálogos geográficos en Definiciones -> General.
    Conserva FormKey, códigos de formulario, permisos y accesos existentes.
    Ejecutar solo en NuanSystem_Master después de 035.
*/
USE [NuanSystem_Master];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51171, 'Migration 171 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SecurityMenus', N'U') IS NULL
    THROW 51171, 'SecurityMenus is required before migration 171.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51171, 'MasterSchemaHistory is required before migration 171.', 1;
IF NOT EXISTS
(
    SELECT 1 FROM dbo.SecurityMenus
    WHERE Code = N'MENU.GEOGRAPHY' AND IsDeleted = 0
)
    THROW 51171, 'Migration 035 geography menu is required before migration 171.', 1;
GO

BEGIN TRANSACTION;

DECLARE @DefinitionsMenuId int;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.SecurityMenus
    WHERE Code = N'MENU.DEFINITIONS' AND IsDeleted = 0
)
BEGIN
    INSERT dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.DEFINITIONS', N'Definiciones',
        N'Maestros y definiciones generales', 1, NULL,
        N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg',
        27, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET ParentId = NULL,
    Name = N'Definiciones',
    Description = N'Maestros y definiciones generales',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 27,
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Code = N'MENU.DEFINITIONS'
  AND IsDeleted = 0;

SET @DefinitionsMenuId =
(
    SELECT TOP (1) Id FROM dbo.SecurityMenus
    WHERE Code = N'MENU.DEFINITIONS' AND IsDeleted = 0
);

/* Conserva exactamente el alcance existente: solo los roles que ya podían
   ver Geografía reciben el nuevo ancestro requerido para renderizar el árbol. */
IF OBJECT_ID(N'dbo.SecurityRoleMenus', N'U') IS NOT NULL
BEGIN
    INSERT dbo.SecurityRoleMenus
    (
        RoleId, MenuId, IsAllowed,
        CreatedByUserName, CreatedAt
    )
    SELECT DISTINCT
        existing.RoleId, @DefinitionsMenuId, 1,
        N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityRoleMenus existing
    INNER JOIN dbo.SecurityMenus geography
        ON geography.Id = existing.MenuId
       AND geography.Code = N'MENU.GEOGRAPHY'
       AND geography.IsDeleted = 0
    WHERE existing.IsDeleted = 0
      AND existing.IsAllowed = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleMenus target
          WHERE target.RoleId = existing.RoleId
            AND target.MenuId = @DefinitionsMenuId
      );
END;

UPDATE dbo.SecurityMenus
SET ParentId = @DefinitionsMenuId,
    Name = N'General',
    Description = N'Países, provincias y ciudades',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 10,
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Code = N'MENU.GEOGRAPHY'
  AND IsDeleted = 0;

UPDATE dbo.SecurityMenus
SET ParentId =
    (
        SELECT TOP (1) Id FROM dbo.SecurityMenus
        WHERE Code = N'MENU.GEOGRAPHY' AND IsDeleted = 0
    ),
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Code IN
(
    N'MENU.GEOGRAPHY.COUNTRIES',
    N'MENU.GEOGRAPHY.PROVINCES',
    N'MENU.GEOGRAPHY.CITIES'
)
AND IsDeleted = 0;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260804.171'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES(N'20260804.171', N'Reubica Geografía en Definiciones > General');
END;

COMMIT TRANSACTION;
GO
