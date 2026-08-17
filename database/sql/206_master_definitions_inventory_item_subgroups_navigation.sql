/*
    Registra Subgrupos de articulos en Configuracion > Definiciones > Inventario.
    Reutiliza las identidades legacy para conservar grants, denegaciones y
    operaciones. No activa sincronizacion ni integraciones externas.
*/
USE [NuanSystem_Master];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME()<>N'NuanSystem_Master' THROW 51206,'Migration 206 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.SecurityMenus',N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityRoleMenus',N'U') IS NULL THROW 51206,'Security menus and role menus are required.',1;
IF OBJECT_ID(N'dbo.SecurityForms',N'U') IS NULL THROW 51206,'SecurityForms is required.',1;
IF OBJECT_ID(N'dbo.Permissions',N'U') IS NULL OR OBJECT_ID(N'dbo.RolePermissions',N'U') IS NULL THROW 51206,'API permission tables are required.',1;
IF OBJECT_ID(N'dbo.Modules',N'U') IS NULL OR OBJECT_ID(N'dbo.Roles',N'U') IS NULL THROW 51206,'Modules and Roles are required.',1;
IF OBJECT_ID(N'dbo.SecurityOperations',N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityRoleFormOperations',N'U') IS NULL THROW 51206,'Form operation tables are required.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51206,'MasterSchemaHistory is required.',1;
GO

BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @ModuleId int=(SELECT TOP(1) Id FROM dbo.Modules WHERE Code=N'GENERALINVENTORY');
 DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
 DECLARE @ConfigurationMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.CONFIGURATION' AND IsDeleted=0);
 DECLARE @DefinitionsMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code IN(N'MENU.CONFIGURATION.DEFINITION',N'MENU.DEFINITIONS') AND IsDeleted=0 ORDER BY CASE WHEN Code=N'MENU.CONFIGURATION.DEFINITION' THEN 0 ELSE 1 END);
 DECLARE @InventoryMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code IN(N'MENU.DEFINITIONS.INVENTORY',N'MENU.CONFIGURATION,DEFINITION.INVENTORY') AND IsDeleted=0 ORDER BY CASE WHEN Code=N'MENU.DEFINITIONS.INVENTORY' THEN 0 ELSE 1 END);
 IF @ModuleId IS NULL OR @AdminRoleId IS NULL THROW 51206,'GENERALINVENTORY module and ADMIN role are required.',1;
 IF @ConfigurationMenuId IS NULL OR @DefinitionsMenuId IS NULL OR @InventoryMenuId IS NULL THROW 51206,'Configuration > Definitions > Inventory from migration 185 is required.',1;

 IF (SELECT COUNT(1) FROM dbo.SecurityForms WHERE (Code IN(N'FORM.GENERALINVENTORY.ITEMSUBGROUPS',N'FORM.DEFINITIONS.INVENTORY.ITEMSUBGROUPS') OR FormKey IN(N'inventory-item-subgroups',N'item-subgroups')) AND IsDeleted=0)>1
    THROW 51206,'Multiple active forms identify ItemSubgroups; reconcile before migration 206.',1;
 IF (SELECT COUNT(1) FROM dbo.SecurityMenus WHERE (Code IN(N'MENU.GENERALINVENTORY.ITEMSUBGROUPS',N'MENU.DEFINITIONS.INVENTORY.ITEMSUBGROUPS') OR FormKey IN(N'inventory-item-subgroups',N'item-subgroups')) AND IsDeleted=0)>1
    THROW 51206,'Multiple active menus identify ItemSubgroups; reconcile before migration 206.',1;

 DECLARE @Permissions table(Code nvarchar(120) PRIMARY KEY,Name nvarchar(160),Description nvarchar(300));
 INSERT @Permissions VALUES
 (N'GENERALINVENTORY.ITEMSUBGROUPS.READ',N'Ver subgrupos de articulos',N'Consultar el maestro de subgrupos de articulos.'),
 (N'GENERALINVENTORY.ITEMSUBGROUPS.MANAGE',N'Gestionar subgrupos de articulos',N'Crear, editar y eliminar subgrupos de articulos.');
 INSERT dbo.Permissions(ModuleId,Code,Name,Description)
 SELECT @ModuleId,p.Code,p.Name,p.Description FROM @Permissions p WHERE NOT EXISTS(SELECT 1 FROM dbo.Permissions x WHERE x.Code=p.Code);
 UPDATE target SET ModuleId=@ModuleId,Name=source.Name,Description=source.Description,IsActive=1,UpdatedAt=SYSUTCDATETIME()
 FROM dbo.Permissions target JOIN @Permissions source ON source.Code=target.Code;
 INSERT dbo.RolePermissions(RoleId,PermissionId)
 SELECT @AdminRoleId,p.Id FROM dbo.Permissions p WHERE p.Code IN(SELECT Code FROM @Permissions)
 AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions rp WHERE rp.RoleId=@AdminRoleId AND rp.PermissionId=p.Id);

 /* Compatibilidad con instalaciones previas a 045. */
 INSERT dbo.RolePermissions(RoleId,PermissionId)
 SELECT DISTINCT legacy.RoleId,target.Id FROM dbo.RolePermissions legacy
 JOIN dbo.Permissions legacyPermission ON legacyPermission.Id=legacy.PermissionId
 JOIN dbo.Permissions target ON target.Code=CASE legacyPermission.Code
      WHEN N'CATALOG.ITEMS.READ' THEN N'GENERALINVENTORY.ITEMSUBGROUPS.READ'
      WHEN N'CATALOG.ITEMS.MANAGE' THEN N'GENERALINVENTORY.ITEMSUBGROUPS.MANAGE' END
 WHERE legacyPermission.Code IN(N'CATALOG.ITEMS.READ',N'CATALOG.ITEMS.MANAGE')
 AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions x WHERE x.RoleId=legacy.RoleId AND x.PermissionId=target.Id);

 DECLARE @FormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms
  WHERE (Code IN(N'FORM.GENERALINVENTORY.ITEMSUBGROUPS',N'FORM.DEFINITIONS.INVENTORY.ITEMSUBGROUPS') OR FormKey IN(N'inventory-item-subgroups',N'item-subgroups')) AND IsDeleted=0
  ORDER BY CASE WHEN FormKey=N'item-subgroups' THEN 0 WHEN FormKey=N'inventory-item-subgroups' THEN 1 ELSE 2 END);
 IF @FormId IS NULL
 BEGIN
  INSERT dbo.SecurityForms(Code,Name,Description,FormKey,FormType,IsVisible,IsActive,CreatedByUserName,CreatedAt)
  VALUES(N'FORM.GENERALINVENTORY.ITEMSUBGROUPS',N'Subgrupos de articulos',N'Mantenimiento de subgrupos de articulos',N'item-subgroups',1,1,1,N'Sistema',SYSUTCDATETIME());
  SET @FormId=CONVERT(int,SCOPE_IDENTITY());
 END;
 UPDATE dbo.SecurityForms SET Code=N'FORM.GENERALINVENTORY.ITEMSUBGROUPS',Name=N'Subgrupos de articulos',Description=N'Mantenimiento de subgrupos de articulos',
  FormKey=N'item-subgroups',FormType=1,IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
 WHERE Id=@FormId;

 DECLARE @MenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus
  WHERE (Code IN(N'MENU.GENERALINVENTORY.ITEMSUBGROUPS',N'MENU.DEFINITIONS.INVENTORY.ITEMSUBGROUPS') OR FormKey IN(N'inventory-item-subgroups',N'item-subgroups')) AND IsDeleted=0
  ORDER BY CASE WHEN FormKey=N'item-subgroups' THEN 0 WHEN FormKey=N'inventory-item-subgroups' THEN 1 ELSE 2 END);
 DECLARE @PreviousRoleAccess table(RoleId int PRIMARY KEY,IsAllowed bit NOT NULL);
 IF @MenuId IS NOT NULL INSERT @PreviousRoleAccess SELECT RoleId,CONVERT(bit,MAX(CONVERT(int,IsAllowed))) FROM dbo.SecurityRoleMenus WHERE MenuId=@MenuId AND IsDeleted=0 GROUP BY RoleId;
 IF @MenuId IS NULL
 BEGIN
  INSERT dbo.SecurityMenus(ParentId,Code,Name,Description,MenuType,FormId,FormKey,IconLarge,IconSmall,DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt)
  VALUES(@InventoryMenuId,N'MENU.DEFINITIONS.INVENTORY.ITEMSUBGROUPS',N'Subgrupos de articulos',N'Mantenimiento de subgrupos de articulos',3,@FormId,N'item-subgroups',
         N'Accordion/inventario_32.svg',N'Accordion/inventario_16.svg',70,1,1,N'Sistema',SYSUTCDATETIME());
  SET @MenuId=CONVERT(int,SCOPE_IDENTITY());
 END;
 UPDATE dbo.SecurityMenus SET ParentId=@InventoryMenuId,Code=N'MENU.DEFINITIONS.INVENTORY.ITEMSUBGROUPS',Name=N'Subgrupos de articulos',Description=N'Mantenimiento de subgrupos de articulos',
  MenuType=3,FormId=@FormId,FormKey=N'item-subgroups',DisplayOrder=70,IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
  UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@MenuId;

 DECLARE @RequiredRoleMenus table(RoleId int,MenuId int,IsAllowed bit,PRIMARY KEY(RoleId,MenuId));
 INSERT @RequiredRoleMenus SELECT RoleId,@MenuId,IsAllowed FROM @PreviousRoleAccess;
 INSERT @RequiredRoleMenus SELECT access.RoleId,ancestor.MenuId,1 FROM @PreviousRoleAccess access
 CROSS JOIN(SELECT @ConfigurationMenuId MenuId UNION ALL SELECT @DefinitionsMenuId UNION ALL SELECT @InventoryMenuId) ancestor
 WHERE access.IsAllowed=1 AND NOT EXISTS(SELECT 1 FROM @RequiredRoleMenus x WHERE x.RoleId=access.RoleId AND x.MenuId=ancestor.MenuId);
 UPDATE @RequiredRoleMenus SET IsAllowed=1 WHERE RoleId=@AdminRoleId;
 INSERT @RequiredRoleMenus SELECT @AdminRoleId,m.Id,1 FROM dbo.SecurityMenus m WHERE m.Id IN(@ConfigurationMenuId,@DefinitionsMenuId,@InventoryMenuId,@MenuId)
 AND NOT EXISTS(SELECT 1 FROM @RequiredRoleMenus x WHERE x.RoleId=@AdminRoleId AND x.MenuId=m.Id);
 MERGE dbo.SecurityRoleMenus target USING(SELECT RoleId,MenuId,IsAllowed FROM @RequiredRoleMenus) source ON target.RoleId=source.RoleId AND target.MenuId=source.MenuId
 WHEN MATCHED THEN UPDATE SET IsAllowed=source.IsAllowed,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
 WHEN NOT MATCHED THEN INSERT(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt) VALUES(source.RoleId,source.MenuId,source.IsAllowed,N'Sistema',SYSUTCDATETIME());

 /* Solo completa ADMIN. Las operaciones de otros roles se conservan. */
 INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt)
 SELECT @AdminRoleId,@FormId,o.Id,1,N'Sistema',SYSUTCDATETIME() FROM dbo.SecurityOperations o
 WHERE o.IsDeleted=0 AND o.IsActive=1 AND LOWER(LTRIM(RTRIM(o.ActionKey))) IN
 (N'refresh',N'create',N'update',N'delete',N'consult',N'history',N'copy',N'customize-columns',N'customizecolumns',N'export-excel',N'exportexcel',N'export-pdf',N'exportpdf',N'export-json',N'exportjson',N'export-xml',N'exportxml')
 AND NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleFormOperations x WHERE x.RoleId=@AdminRoleId AND x.FormId=@FormId AND x.OperationId=o.Id);

 IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.206')
  INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260813.206',N'Mueve Subgrupos de articulos a Configuracion > Definiciones > Inventario');
 COMMIT;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
GO
