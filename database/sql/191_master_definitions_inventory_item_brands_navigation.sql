/*
    Registra Marcas de articulos en:
    Modulo de configuracion -> Definiciones -> Inventario.

    Solo NuanSystem_Master. Migra el FormKey legacy inventory-item-brands al
    FormKey item-brands sobre las mismas identidades de formulario/menu para
    conservar grants y denegaciones existentes. No activa sincronizacion.
*/

USE [NuanSystem_Master];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() <> N'NuanSystem_Master' THROW 51191, 'Migration 191 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SecurityMenus', N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityRoleMenus', N'U') IS NULL
    THROW 51191, 'Security menus and role menus are required.', 1;
IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL THROW 51191, 'SecurityForms is required.', 1;
IF OBJECT_ID(N'dbo.Permissions', N'U') IS NULL OR OBJECT_ID(N'dbo.RolePermissions', N'U') IS NULL
    THROW 51191, 'API permission tables are required.', 1;
IF OBJECT_ID(N'dbo.Modules', N'U') IS NULL OR OBJECT_ID(N'dbo.Roles', N'U') IS NULL
    THROW 51191, 'Modules and Roles are required.', 1;
IF OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityRoleFormOperations', N'U') IS NULL
    THROW 51191, 'Form operation tables are required.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL THROW 51191, 'MasterSchemaHistory is required.', 1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @ModuleId int=(SELECT TOP(1) Id FROM dbo.Modules WHERE Code=N'GENERALINVENTORY');
    DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
    DECLARE @ConfigurationMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.CONFIGURATION' AND IsDeleted=0);
    DECLARE @DefinitionsMenuId int=
    (
        SELECT TOP(1) Id FROM dbo.SecurityMenus
        WHERE Code IN(N'MENU.CONFIGURATION.DEFINITION',N'MENU.DEFINITIONS') AND IsDeleted=0
        ORDER BY CASE WHEN Code=N'MENU.CONFIGURATION.DEFINITION' THEN 0 ELSE 1 END
    );
    DECLARE @InventoryMenuId int=
    (
        SELECT TOP(1) Id FROM dbo.SecurityMenus
        WHERE Code IN(N'MENU.DEFINITIONS.INVENTORY',N'MENU.CONFIGURATION,DEFINITION.INVENTORY') AND IsDeleted=0
        ORDER BY CASE WHEN Code=N'MENU.DEFINITIONS.INVENTORY' THEN 0 ELSE 1 END
    );

    IF @ModuleId IS NULL THROW 51191, 'GENERALINVENTORY module is required.', 1;
    IF @AdminRoleId IS NULL THROW 51191, 'Active ADMIN role is required.', 1;
    IF @ConfigurationMenuId IS NULL OR @DefinitionsMenuId IS NULL OR @InventoryMenuId IS NULL
        THROW 51191, 'Configuration > Definitions > Inventory from migration 185 is required.', 1;

    IF
    (
        SELECT COUNT(1) FROM dbo.SecurityForms
        WHERE (Code IN(N'FORM.GENERALINVENTORY.ITEMBRANDS',N'FORM.DEFINITIONS.INVENTORY.ITEMBRANDS')
               OR FormKey IN(N'inventory-item-brands',N'item-brands')) AND IsDeleted=0
    ) > 1 THROW 51191, 'Multiple active forms identify ItemBrands; reconcile before migration 191.', 1;

    IF
    (
        SELECT COUNT(1) FROM dbo.SecurityMenus
        WHERE (Code IN(N'MENU.GENERALINVENTORY.ITEMBRANDS',N'MENU.DEFINITIONS.INVENTORY.ITEMBRANDS')
               OR FormKey IN(N'inventory-item-brands',N'item-brands')) AND IsDeleted=0
    ) > 1 THROW 51191, 'Multiple active menus identify ItemBrands; reconcile before migration 191.', 1;

    DECLARE @Permissions table(Code nvarchar(120) PRIMARY KEY,Name nvarchar(160),Description nvarchar(300));
    INSERT @Permissions VALUES
    (N'GENERALINVENTORY.ITEMBRANDS.READ',N'Ver marcas de articulos',N'Consultar el maestro de marcas de articulos.'),
    (N'GENERALINVENTORY.ITEMBRANDS.MANAGE',N'Gestionar marcas de articulos',N'Crear, editar y eliminar marcas de articulos.');

    INSERT dbo.Permissions(ModuleId,Code,Name,Description)
    SELECT @ModuleId,p.Code,p.Name,p.Description FROM @Permissions p
    WHERE NOT EXISTS(SELECT 1 FROM dbo.Permissions x WHERE x.Code=p.Code);

    UPDATE target SET ModuleId=@ModuleId,Name=source.Name,Description=source.Description,
           IsActive=1,UpdatedAt=SYSUTCDATETIME()
    FROM dbo.Permissions target JOIN @Permissions source ON source.Code=target.Code;

    INSERT dbo.RolePermissions(RoleId,PermissionId)
    SELECT @AdminRoleId,p.Id FROM dbo.Permissions p
    WHERE p.Code IN(SELECT Code FROM @Permissions)
      AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions rp WHERE rp.RoleId=@AdminRoleId AND rp.PermissionId=p.Id);

    /* Conserva compatibilidad con instalaciones anteriores a 045. */
    INSERT dbo.RolePermissions(RoleId,PermissionId)
    SELECT DISTINCT legacy.RoleId,target.Id
    FROM dbo.RolePermissions legacy
    JOIN dbo.Permissions legacyPermission ON legacyPermission.Id=legacy.PermissionId
    JOIN dbo.Permissions target ON target.Code=CASE legacyPermission.Code
        WHEN N'CATALOG.ITEMS.READ' THEN N'GENERALINVENTORY.ITEMBRANDS.READ'
        WHEN N'CATALOG.ITEMS.MANAGE' THEN N'GENERALINVENTORY.ITEMBRANDS.MANAGE' END
    WHERE legacyPermission.Code IN(N'CATALOG.ITEMS.READ',N'CATALOG.ITEMS.MANAGE')
      AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions x WHERE x.RoleId=legacy.RoleId AND x.PermissionId=target.Id);

    DECLARE @FormId int=
    (
        SELECT TOP(1) Id FROM dbo.SecurityForms
        WHERE (Code IN(N'FORM.GENERALINVENTORY.ITEMBRANDS',N'FORM.DEFINITIONS.INVENTORY.ITEMBRANDS')
               OR FormKey IN(N'inventory-item-brands',N'item-brands')) AND IsDeleted=0
        ORDER BY CASE WHEN FormKey=N'item-brands' THEN 0 WHEN FormKey=N'inventory-item-brands' THEN 1 ELSE 2 END
    );

    IF @FormId IS NULL
    BEGIN
        INSERT dbo.SecurityForms(Code,Name,Description,FormKey,FormType,IsVisible,IsActive,CreatedByUserName,CreatedAt)
        VALUES(N'FORM.GENERALINVENTORY.ITEMBRANDS',N'Marcas de articulos',N'Mantenimiento de marcas de articulos',
               N'item-brands',1,1,1,N'Sistema',SYSUTCDATETIME());
        SET @FormId=CONVERT(int,SCOPE_IDENTITY());
    END;

    UPDATE dbo.SecurityForms SET Code=N'FORM.GENERALINVENTORY.ITEMBRANDS',Name=N'Marcas de articulos',
           Description=N'Mantenimiento de marcas de articulos',FormKey=N'item-brands',FormType=1,
           IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
           UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    WHERE Id=@FormId;

    DECLARE @MenuId int=
    (
        SELECT TOP(1) Id FROM dbo.SecurityMenus
        WHERE (Code IN(N'MENU.GENERALINVENTORY.ITEMBRANDS',N'MENU.DEFINITIONS.INVENTORY.ITEMBRANDS')
               OR FormKey IN(N'inventory-item-brands',N'item-brands')) AND IsDeleted=0
        ORDER BY CASE WHEN FormKey=N'item-brands' THEN 0 WHEN FormKey=N'inventory-item-brands' THEN 1 ELSE 2 END
    );

    DECLARE @PreviousRoleAccess table(RoleId int PRIMARY KEY,IsAllowed bit NOT NULL);
    IF @MenuId IS NOT NULL
        INSERT @PreviousRoleAccess(RoleId,IsAllowed)
        SELECT RoleId,CONVERT(bit,MAX(CONVERT(int,IsAllowed))) FROM dbo.SecurityRoleMenus
        WHERE MenuId=@MenuId AND IsDeleted=0 GROUP BY RoleId;

    IF @MenuId IS NULL
    BEGIN
        INSERT dbo.SecurityMenus(ParentId,Code,Name,Description,MenuType,FormId,FormKey,IconLarge,IconSmall,
                                 DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt)
        VALUES(@InventoryMenuId,N'MENU.DEFINITIONS.INVENTORY.ITEMBRANDS',N'Marcas de articulos',
               N'Mantenimiento de marcas de articulos',3,@FormId,N'item-brands',
               N'Accordion/inventario_32.svg',N'Accordion/inventario_16.svg',40,1,1,N'Sistema',SYSUTCDATETIME());
        SET @MenuId=CONVERT(int,SCOPE_IDENTITY());
    END;

    UPDATE dbo.SecurityMenus SET ParentId=@InventoryMenuId,Code=N'MENU.DEFINITIONS.INVENTORY.ITEMBRANDS',
           Name=N'Marcas de articulos',Description=N'Mantenimiento de marcas de articulos',MenuType=3,
           FormId=@FormId,FormKey=N'item-brands',DisplayOrder=40,IsVisible=1,IsActive=1,IsDeleted=0,
           DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
           UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    WHERE Id=@MenuId;

    DECLARE @RequiredRoleMenus table(RoleId int,MenuId int,IsAllowed bit,PRIMARY KEY(RoleId,MenuId));
    INSERT @RequiredRoleMenus SELECT RoleId,@MenuId,IsAllowed FROM @PreviousRoleAccess;
    INSERT @RequiredRoleMenus
    SELECT access.RoleId,ancestor.MenuId,1 FROM @PreviousRoleAccess access
    CROSS JOIN(SELECT @ConfigurationMenuId MenuId UNION ALL SELECT @DefinitionsMenuId UNION ALL SELECT @InventoryMenuId) ancestor
    WHERE access.IsAllowed=1
      AND NOT EXISTS(SELECT 1 FROM @RequiredRoleMenus x WHERE x.RoleId=access.RoleId AND x.MenuId=ancestor.MenuId);

    UPDATE @RequiredRoleMenus SET IsAllowed=1 WHERE RoleId=@AdminRoleId;
    INSERT @RequiredRoleMenus
    SELECT @AdminRoleId,m.Id,1 FROM dbo.SecurityMenus m
    WHERE m.Id IN(@ConfigurationMenuId,@DefinitionsMenuId,@InventoryMenuId,@MenuId)
      AND NOT EXISTS(SELECT 1 FROM @RequiredRoleMenus x WHERE x.RoleId=@AdminRoleId AND x.MenuId=m.Id);

    MERGE dbo.SecurityRoleMenus target
    USING(SELECT RoleId,MenuId,IsAllowed FROM @RequiredRoleMenus) source
       ON target.RoleId=source.RoleId AND target.MenuId=source.MenuId
    WHEN MATCHED THEN UPDATE SET IsAllowed=source.IsAllowed,IsDeleted=0,DeletedByUserId=NULL,
         DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    WHEN NOT MATCHED THEN INSERT(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt)
         VALUES(source.RoleId,source.MenuId,source.IsAllowed,N'Sistema',SYSUTCDATETIME());

    INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt)
    SELECT @AdminRoleId,@FormId,o.Id,1,N'Sistema',SYSUTCDATETIME()
    FROM dbo.SecurityOperations o
    WHERE o.IsDeleted=0 AND o.IsActive=1
      AND LOWER(LTRIM(RTRIM(o.ActionKey))) IN
      (N'refresh',N'create',N'update',N'delete',N'consult',N'history',N'copy',
       N'customize-columns',N'customizecolumns',N'export-excel',N'exportexcel',N'export-pdf',N'exportpdf',
       N'export-json',N'exportjson',N'export-xml',N'exportxml')
      AND NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleFormOperations x
                     WHERE x.RoleId=@AdminRoleId AND x.FormId=@FormId AND x.OperationId=o.Id);

    IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260812.191')
        INSERT dbo.MasterSchemaHistory(Version,Description)
        VALUES(N'20260812.191',N'Mueve Marcas de articulos a Configuracion > Definiciones > Inventario');

    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK;
    THROW;
END CATCH;
GO
