/*
    Registra Familias de articulos en:
    Modulo de configuracion -> Definiciones -> Inventario.

    Solo NuanSystem_Master. Conserva FormKey item-families, migra los accesos
    existentes del leaf y crea permisos API propios. No activa workers ni
    modifica perfiles o rutas Matriz-Sucursal.
*/

USE [NuanSystem_Master];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() <> N'NuanSystem_Master' THROW 51189, 'Migration 189 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SecurityMenus', N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityRoleMenus', N'U') IS NULL
    THROW 51189, 'Security menus and role menus are required before migration 189.', 1;
IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL THROW 51189, 'SecurityForms is required before migration 189.', 1;
IF OBJECT_ID(N'dbo.Permissions', N'U') IS NULL OR OBJECT_ID(N'dbo.RolePermissions', N'U') IS NULL
    THROW 51189, 'API permission tables are required before migration 189.', 1;
IF OBJECT_ID(N'dbo.Modules', N'U') IS NULL OR OBJECT_ID(N'dbo.Roles', N'U') IS NULL
    THROW 51189, 'Modules and Roles are required before migration 189.', 1;
IF OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityRoleFormOperations', N'U') IS NULL
    THROW 51189, 'Form operation tables are required before migration 189.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL THROW 51189, 'MasterSchemaHistory is required before migration 189.', 1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @ModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'GENERALINVENTORY');
    DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
    DECLARE @ConfigurationMenuId int =
        (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION' AND IsDeleted = 0);
    DECLARE @DefinitionsMenuId int =
    (
        SELECT TOP (1) Id FROM dbo.SecurityMenus
        WHERE Code IN (N'MENU.CONFIGURATION.DEFINITION', N'MENU.DEFINITIONS') AND IsDeleted = 0
        ORDER BY CASE WHEN Code = N'MENU.CONFIGURATION.DEFINITION' THEN 0 ELSE 1 END
    );
    DECLARE @InventoryMenuId int =
    (
        SELECT TOP (1) Id FROM dbo.SecurityMenus
        WHERE Code IN (N'MENU.DEFINITIONS.INVENTORY', N'MENU.CONFIGURATION,DEFINITION.INVENTORY')
          AND IsDeleted = 0
        ORDER BY CASE WHEN Code = N'MENU.DEFINITIONS.INVENTORY' THEN 0 ELSE 1 END
    );

    IF @ModuleId IS NULL THROW 51189, 'GENERALINVENTORY module is required.', 1;
    IF @AdminRoleId IS NULL THROW 51189, 'Active ADMIN role is required.', 1;
    IF @ConfigurationMenuId IS NULL OR @DefinitionsMenuId IS NULL OR @InventoryMenuId IS NULL
        THROW 51189, 'Configuration > Definitions > Inventory from migration 185 is required.', 1;

    IF
    (
        SELECT COUNT(1) FROM dbo.SecurityForms
        WHERE (Code IN (N'FORM.GENERALINVENTORY.ITEMFAMILIES', N'FORM.DEFINITIONS.INVENTORY.ITEMFAMILIES')
               OR FormKey = N'item-families')
          AND IsDeleted = 0
    ) > 1
        THROW 51189, 'Multiple active SecurityForms identify item-families; reconcile them before migration 189.', 1;

    IF
    (
        SELECT COUNT(1) FROM dbo.SecurityMenus
        WHERE (Code IN (N'MENU.GENERALINVENTORY.ITEMFAMILIES', N'MENU.DEFINITIONS.INVENTORY.ITEMFAMILIES')
               OR FormKey = N'item-families')
          AND IsDeleted = 0
    ) > 1
        THROW 51189, 'Multiple active SecurityMenus identify item-families; reconcile them before migration 189.', 1;

    DECLARE @Permissions table
    (
        Code nvarchar(120) NOT NULL PRIMARY KEY,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NOT NULL
    );

    INSERT @Permissions(Code, Name, Description)
    VALUES
    (N'GENERALINVENTORY.ITEMFAMILIES.READ', N'Ver familias de articulos', N'Consultar el maestro de familias de articulos.'),
    (N'GENERALINVENTORY.ITEMFAMILIES.MANAGE', N'Gestionar familias de articulos', N'Crear, editar y eliminar familias de articulos.');

    INSERT dbo.Permissions(ModuleId, Code, Name, Description)
    SELECT @ModuleId, source.Code, source.Name, source.Description
    FROM @Permissions source
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Permissions target WHERE target.Code = source.Code);

    UPDATE target
    SET ModuleId = @ModuleId,
        Name = source.Name,
        Description = source.Description,
        IsActive = 1,
        UpdatedAt = SYSUTCDATETIME()
    FROM dbo.Permissions target
    INNER JOIN @Permissions source ON source.Code = target.Code;

    INSERT dbo.RolePermissions(RoleId, PermissionId)
    SELECT @AdminRoleId, permission.Id
    FROM dbo.Permissions permission
    WHERE permission.Code IN (SELECT Code FROM @Permissions)
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.RolePermissions existing
          WHERE existing.RoleId = @AdminRoleId AND existing.PermissionId = permission.Id
      );

    /* El mantenimiento legacy se autorizaba con CATALOG.ITEMS.READ/MANAGE.
       Se copian esos grants a los permisos propios sin retirar los anteriores. */
    INSERT dbo.RolePermissions(RoleId, PermissionId)
    SELECT DISTINCT legacyGrant.RoleId, targetPermission.Id
    FROM dbo.RolePermissions legacyGrant
    INNER JOIN dbo.Permissions legacyPermission
        ON legacyPermission.Id = legacyGrant.PermissionId
    INNER JOIN dbo.Permissions targetPermission
        ON targetPermission.Code = CASE legacyPermission.Code
            WHEN N'CATALOG.ITEMS.READ' THEN N'GENERALINVENTORY.ITEMFAMILIES.READ'
            WHEN N'CATALOG.ITEMS.MANAGE' THEN N'GENERALINVENTORY.ITEMFAMILIES.MANAGE'
        END
    WHERE legacyPermission.Code IN (N'CATALOG.ITEMS.READ', N'CATALOG.ITEMS.MANAGE')
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.RolePermissions existing
          WHERE existing.RoleId = legacyGrant.RoleId
            AND existing.PermissionId = targetPermission.Id
      );

    DECLARE @FormId int =
    (
        SELECT TOP (1) Id
        FROM dbo.SecurityForms
        WHERE (Code IN (N'FORM.GENERALINVENTORY.ITEMFAMILIES', N'FORM.DEFINITIONS.INVENTORY.ITEMFAMILIES')
               OR FormKey = N'item-families')
          AND IsDeleted = 0
        ORDER BY CASE WHEN FormKey = N'item-families' THEN 0 ELSE 1 END
    );

    IF @FormId IS NULL
    BEGIN
        INSERT dbo.SecurityForms
        (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
        VALUES
        (N'FORM.GENERALINVENTORY.ITEMFAMILIES', N'Familias de articulos',
         N'Mantenimiento de familias de articulos', N'item-families', 1, 1, 1, N'Sistema', SYSUTCDATETIME());
        SET @FormId = CONVERT(int, SCOPE_IDENTITY());
    END;

    UPDATE dbo.SecurityForms
    SET Code = N'FORM.GENERALINVENTORY.ITEMFAMILIES',
        Name = N'Familias de articulos',
        Description = N'Mantenimiento de familias de articulos',
        FormKey = N'item-families', FormType = 1,
        IsVisible = 1, IsActive = 1, IsDeleted = 0,
        DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL,
        UpdatedByUserName = N'Sistema', UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @FormId;

    DECLARE @MenuId int =
    (
        SELECT TOP (1) Id
        FROM dbo.SecurityMenus
        WHERE (Code IN
              (N'MENU.GENERALINVENTORY.ITEMFAMILIES', N'MENU.DEFINITIONS.INVENTORY.ITEMFAMILIES')
               OR FormKey = N'item-families')
          AND IsDeleted = 0
        ORDER BY CASE WHEN FormKey = N'item-families' THEN 0 ELSE 1 END
    );

    DECLARE @PreviousRoleAccess table
    (
        RoleId int NOT NULL PRIMARY KEY,
        IsAllowed bit NOT NULL
    );

    IF @MenuId IS NOT NULL
        INSERT @PreviousRoleAccess(RoleId, IsAllowed)
        SELECT RoleId, CONVERT(bit, MAX(CONVERT(int, IsAllowed)))
        FROM dbo.SecurityRoleMenus
        WHERE MenuId = @MenuId AND IsDeleted = 0
        GROUP BY RoleId;

    IF @MenuId IS NULL
    BEGIN
        INSERT dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @InventoryMenuId, N'MENU.DEFINITIONS.INVENTORY.ITEMFAMILIES',
            N'Familias de articulos', N'Mantenimiento de familias de articulos',
            3, @FormId, N'item-families',
            N'Accordion/inventario_32.svg', N'Accordion/inventario_16.svg',
            30, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
        SET @MenuId = CONVERT(int, SCOPE_IDENTITY());
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @InventoryMenuId,
        Code = N'MENU.DEFINITIONS.INVENTORY.ITEMFAMILIES',
        Name = N'Familias de articulos',
        Description = N'Mantenimiento de familias de articulos',
        MenuType = 3, FormId = @FormId, FormKey = N'item-families',
        DisplayOrder = 30, IsVisible = 1, IsActive = 1, IsDeleted = 0,
        DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL,
        UpdatedByUserName = N'Sistema', UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @MenuId;

    DECLARE @RequiredRoleMenus table
    (
        RoleId int NOT NULL,
        MenuId int NOT NULL,
        IsAllowed bit NOT NULL,
        PRIMARY KEY(RoleId, MenuId)
    );

    INSERT @RequiredRoleMenus(RoleId, MenuId, IsAllowed)
    SELECT RoleId, @MenuId, IsAllowed FROM @PreviousRoleAccess;

    INSERT @RequiredRoleMenus(RoleId, MenuId, IsAllowed)
    SELECT access.RoleId, ancestor.MenuId, 1
    FROM @PreviousRoleAccess access
    CROSS JOIN
    (
        SELECT @ConfigurationMenuId AS MenuId
        UNION ALL SELECT @DefinitionsMenuId
        UNION ALL SELECT @InventoryMenuId
    ) ancestor
    WHERE access.IsAllowed = 1
      AND NOT EXISTS
      (
          SELECT 1 FROM @RequiredRoleMenus existing
          WHERE existing.RoleId = access.RoleId AND existing.MenuId = ancestor.MenuId
      );

    UPDATE @RequiredRoleMenus SET IsAllowed = 1 WHERE RoleId = @AdminRoleId;

    INSERT @RequiredRoleMenus(RoleId, MenuId, IsAllowed)
    SELECT @AdminRoleId, menu.Id, 1
    FROM dbo.SecurityMenus menu
    WHERE menu.Id IN (@ConfigurationMenuId, @DefinitionsMenuId, @InventoryMenuId, @MenuId)
      AND NOT EXISTS
      (
          SELECT 1 FROM @RequiredRoleMenus existing
          WHERE existing.RoleId = @AdminRoleId AND existing.MenuId = menu.Id
      );

    MERGE dbo.SecurityRoleMenus target
    USING (SELECT RoleId, MenuId, IsAllowed FROM @RequiredRoleMenus) source
       ON target.RoleId = source.RoleId AND target.MenuId = source.MenuId
    WHEN MATCHED THEN
        UPDATE SET IsAllowed = source.IsAllowed, IsDeleted = 0,
                   DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL,
                   UpdatedByUserName = N'Sistema', UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT(RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES(source.RoleId, source.MenuId, source.IsAllowed, N'Sistema', SYSUTCDATETIME());

    INSERT dbo.SecurityRoleFormOperations
    (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, @FormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.IsDeleted = 0 AND operation.IsActive = 1
      AND LOWER(LTRIM(RTRIM(operation.ActionKey))) IN
      (
          N'refresh', N'create', N'update', N'delete', N'consult', N'history', N'copy',
          N'customize-columns', N'customizecolumns',
          N'export-excel', N'exportexcel', N'export-pdf', N'exportpdf',
          N'export-json', N'exportjson', N'export-xml', N'exportxml'
      )
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = @FormId
            AND existing.OperationId = operation.Id
      );

    IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260812.189')
        INSERT dbo.MasterSchemaHistory(Version, Description)
        VALUES(N'20260812.189', N'Registra Familias de articulos en Configuracion > Definiciones > Inventario');

    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    THROW;
END CATCH;
GO
