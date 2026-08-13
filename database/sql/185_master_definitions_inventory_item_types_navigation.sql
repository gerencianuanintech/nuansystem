/*
    Target: NuanSystem_Master
    Purpose: Move the existing Item Types maintenance to
             Configuración > Definiciones > Inventario > Tipos de ítem.

    This script preserves the existing SecurityForms row, FormKey, menu row Id,
    role assignments and permissions. It does not grant access to roles that did
    not already have access to the Item Types leaf menu.
*/

USE [NuanSystem_Master];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51185, 'Migration 185 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SecurityMenus', N'U') IS NULL
    THROW 51185, 'SecurityMenus is required before migration 185.', 1;
IF OBJECT_ID(N'dbo.SecurityRoleMenus', N'U') IS NULL
    THROW 51185, 'SecurityRoleMenus is required before migration 185.', 1;
IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL
    THROW 51185, 'SecurityForms is required before migration 185.', 1;
GO

BEGIN TRANSACTION;

BEGIN TRY

DECLARE @ConfigurationMenuId int =
(
    SELECT TOP (1) Id FROM dbo.SecurityMenus
    WHERE Code = N'MENU.CONFIGURATION' AND IsDeleted = 0
);
DECLARE @DefinitionsMenuId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityMenus
    WHERE Code IN
    (
        N'MENU.CONFIGURATION.DEFINITION',
        N'MENU.DEFINITIONS'
    )
      AND IsDeleted = 0
    ORDER BY CASE WHEN Code = N'MENU.CONFIGURATION.DEFINITION' THEN 0 ELSE 1 END
);
DECLARE @InventoryDefinitionsMenuId int;
DECLARE @ItemTypesMenuId int =
(
    SELECT TOP (1) Id FROM dbo.SecurityMenus
    WHERE Code IN
    (
        N'MENU.GENERALINVENTORY.ITEMTYPES',
        N'MENU.DEFINITIONS.INVENTORY.ITEMTYPES'
    )
    AND IsDeleted = 0
    ORDER BY CASE WHEN Code = N'MENU.DEFINITIONS.INVENTORY.ITEMTYPES' THEN 0 ELSE 1 END
);
DECLARE @ItemTypesFormId int =
(
    SELECT TOP (1) Id FROM dbo.SecurityForms
    WHERE FormKey = N'inventory-item-types' AND IsDeleted = 0
);

IF @ConfigurationMenuId IS NULL
    THROW 51185, 'MENU.CONFIGURATION is required before migration 185.', 1;
IF @DefinitionsMenuId IS NULL
    THROW 51185, 'The active Configuration > Definitions menu is required before migration 185.', 1;
IF @ItemTypesMenuId IS NULL
    THROW 51185, 'The existing Item Types menu is required before migration 185.', 1;
IF @ItemTypesFormId IS NULL
    THROW 51185, 'FormKey inventory-item-types is required before migration 185.', 1;
IF EXISTS
(
    SELECT 1
    FROM dbo.SecurityMenus canonical
    INNER JOIN dbo.SecurityMenus legacy
        ON legacy.Code = N'MENU.CONFIGURATION,DEFINITION.INVENTORY'
       AND legacy.IsDeleted = 0
       AND legacy.Id <> canonical.Id
    WHERE canonical.Code = N'MENU.DEFINITIONS.INVENTORY'
      AND canonical.IsDeleted = 0
)
    THROW 51185, 'Canonical and legacy Inventory definition menus identify different active records.', 1;

DECLARE @ExistingRoleAccess table
(
    RoleId int NOT NULL PRIMARY KEY,
    IsAllowed bit NOT NULL
);

INSERT @ExistingRoleAccess (RoleId, IsAllowed)
SELECT RoleId, CONVERT(bit, MAX(CONVERT(int, IsAllowed)))
FROM dbo.SecurityRoleMenus
WHERE MenuId = @ItemTypesMenuId
  AND IsDeleted = 0
GROUP BY RoleId;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.SecurityMenus
    WHERE Code IN
    (
        N'MENU.DEFINITIONS.INVENTORY',
        N'MENU.CONFIGURATION,DEFINITION.INVENTORY'
    )
      AND IsDeleted = 0
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
        @DefinitionsMenuId, N'MENU.DEFINITIONS.INVENTORY', N'Inventario',
        N'Maestros auxiliares de inventario', 2, NULL,
        N'Accordion/inventario_32.svg', N'Accordion/inventario_16.svg',
        20, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

SET @InventoryDefinitionsMenuId =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityMenus
    WHERE Code IN
    (
        N'MENU.DEFINITIONS.INVENTORY',
        N'MENU.CONFIGURATION,DEFINITION.INVENTORY'
    )
      AND IsDeleted = 0
    ORDER BY CASE WHEN Code = N'MENU.DEFINITIONS.INVENTORY' THEN 0 ELSE 1 END
);

UPDATE dbo.SecurityMenus
SET ParentId = @ConfigurationMenuId,
    Name = N'Definiciones',
    Description = N'Maestros y definiciones generales',
    MenuType = 2,
    FormId = NULL,
    FormKey = NULL,
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @DefinitionsMenuId;

UPDATE dbo.SecurityMenus
SET ParentId = @DefinitionsMenuId,
    Code = N'MENU.DEFINITIONS.INVENTORY',
    Name = N'Inventario',
    Description = N'Maestros auxiliares de inventario',
    MenuType = 2,
    FormId = NULL,
    FormKey = NULL,
    DisplayOrder = 20,
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @InventoryDefinitionsMenuId;

UPDATE dbo.SecurityMenus
SET ParentId = @InventoryDefinitionsMenuId,
    Code = N'MENU.DEFINITIONS.INVENTORY.ITEMTYPES',
    Name = N'Tipos de ítem',
    Description = N'Mantenimiento de tipos de ítem',
    MenuType = 3,
    FormId = @ItemTypesFormId,
    FormKey = N'inventory-item-types',
    DisplayOrder = 10,
    IsVisible = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @ItemTypesMenuId;

MERGE dbo.SecurityRoleMenus AS target
USING
(
    /* El leaf conserva incluso una denegación explícita. */
    SELECT access.RoleId, @ItemTypesMenuId AS MenuId, access.IsAllowed
    FROM @ExistingRoleAccess access
    UNION ALL
    /* Los ancestros solo se habilitan para quien sí puede ver el leaf.
       Una denegación del leaf nunca debe revocar acceso compartido. */
    SELECT access.RoleId, required.MenuId, CONVERT(bit, 1) AS IsAllowed
    FROM @ExistingRoleAccess access
    CROSS JOIN
    (
        SELECT @ConfigurationMenuId AS MenuId
        UNION ALL SELECT @DefinitionsMenuId
        UNION ALL SELECT @InventoryDefinitionsMenuId
    ) required
    WHERE access.IsAllowed = 1
) AS source
    ON target.RoleId = source.RoleId
   AND target.MenuId = source.MenuId
WHEN MATCHED THEN
    UPDATE SET
        IsAllowed = source.IsAllowed,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
    VALUES (source.RoleId, source.MenuId, source.IsAllowed, N'Sistema', SYSUTCDATETIME());

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260811.185'
   )
BEGIN
    INSERT dbo.MasterSchemaHistory (Version, Description)
    VALUES
    (
        N'20260811.185',
        N'Reubica Tipos de ítem en Configuración > Definiciones > Inventario'
    );
END;

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
BEGIN
    UPDATE dbo.MasterSchemaHistory
    SET Description = N'Reubica Tipos de ítem en Configuración > Definiciones > Inventario'
    WHERE Version = N'20260811.185'
      AND Description <> N'Reubica Tipos de ítem en Configuración > Definiciones > Inventario';
END;

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
