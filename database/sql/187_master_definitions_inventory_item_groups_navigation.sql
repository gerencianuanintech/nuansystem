/*
    Registra Grupos de articulos en:
    Modulo de configuracion -> Definiciones -> Inventario.

    Solo NuanSystem_Master. Conserva el FormKey item-groups y no modifica
    configuraciones ni activa workers de sincronizacion.
*/
USE [NuanSystem_Master];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME()<>N'NuanSystem_Master' THROW 51187,'Migration 187 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.SecurityMenus',N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityForms',N'U') IS NULL
    THROW 51187,'Security menus and forms are required before migration 187.',1;
IF OBJECT_ID(N'dbo.Permissions',N'U') IS NULL OR OBJECT_ID(N'dbo.RolePermissions',N'U') IS NULL
    THROW 51187,'API permission tables are required before migration 187.',1;
IF OBJECT_ID(N'dbo.Modules',N'U') IS NULL OR OBJECT_ID(N'dbo.Roles',N'U') IS NULL
    THROW 51187,'Modules and Roles are required before migration 187.',1;
IF OBJECT_ID(N'dbo.SecurityOperations',N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityRoleFormOperations',N'U') IS NULL
    THROW 51187,'Form operation tables are required before migration 187.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51187,'MasterSchemaHistory is required before migration 187.',1;
GO

BEGIN TRY
 BEGIN TRANSACTION;

 DECLARE @ModuleId int=(SELECT TOP(1) Id FROM dbo.Modules WHERE Code=N'GENERALINVENTORY');
 DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
 DECLARE @ConfigurationId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.CONFIGURATION' AND IsDeleted=0);
 DECLARE @DefinitionsId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code IN(N'MENU.CONFIGURATION.DEFINITION',N'MENU.DEFINITIONS') AND IsDeleted=0 ORDER BY CASE WHEN Code=N'MENU.CONFIGURATION.DEFINITION' THEN 0 ELSE 1 END);
 DECLARE @InventoryId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.DEFINITIONS.INVENTORY' AND IsDeleted=0);

 IF @ModuleId IS NULL THROW 51187,'GENERALINVENTORY module is required.',1;
 IF @ConfigurationId IS NULL OR @DefinitionsId IS NULL OR @InventoryId IS NULL THROW 51187,'Configuration > Definitions > Inventory from migration 185 is required.',1;
 IF @AdminRoleId IS NULL THROW 51187,'Active ADMIN role is required.',1;

 DECLARE @Permissions table(Code nvarchar(120),Name nvarchar(160),Description nvarchar(300));
 INSERT @Permissions VALUES
  (N'GENERALINVENTORY.ITEMGROUPS.READ',N'Ver grupos de articulos',N'Consultar el maestro de grupos de articulos.'),
  (N'GENERALINVENTORY.ITEMGROUPS.MANAGE',N'Gestionar grupos de articulos',N'Crear, editar y eliminar grupos de articulos.');

 INSERT dbo.Permissions(ModuleId,Code,Name,Description)
 SELECT @ModuleId,p.Code,p.Name,p.Description FROM @Permissions p
 WHERE NOT EXISTS(SELECT 1 FROM dbo.Permissions x WHERE x.Code=p.Code);
 UPDATE target SET ModuleId=@ModuleId,Name=source.Name,Description=source.Description,IsActive=1,UpdatedAt=SYSUTCDATETIME()
 FROM dbo.Permissions target JOIN @Permissions source ON source.Code=target.Code;
 INSERT dbo.RolePermissions(RoleId,PermissionId)
 SELECT @AdminRoleId,p.Id FROM dbo.Permissions p WHERE p.Code IN(SELECT Code FROM @Permissions)
 AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions rp WHERE rp.RoleId=@AdminRoleId AND rp.PermissionId=p.Id);

 DECLARE @FormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms WHERE (Code=N'FORM.GENERALINVENTORY.ITEMGROUPS' OR FormKey=N'item-groups') AND IsDeleted=0 ORDER BY CASE WHEN Code=N'FORM.GENERALINVENTORY.ITEMGROUPS' THEN 0 ELSE 1 END);
 IF @FormId IS NULL BEGIN
  INSERT dbo.SecurityForms(Code,Name,Description,FormKey,FormType,IsVisible,IsActive,CreatedByUserName,CreatedAt)
  VALUES(N'FORM.GENERALINVENTORY.ITEMGROUPS',N'Grupos de articulos',N'Mantenimiento de grupos de articulos',N'item-groups',1,1,1,N'Sistema',SYSUTCDATETIME());
  SET @FormId=CONVERT(int,SCOPE_IDENTITY());
 END;
 UPDATE dbo.SecurityForms SET Code=N'FORM.GENERALINVENTORY.ITEMGROUPS',Name=N'Grupos de articulos',Description=N'Mantenimiento de grupos de articulos',
  FormKey=N'item-groups',FormType=1,IsVisible=1,IsActive=1,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@FormId;

 DECLARE @MenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE (Code IN(N'MENU.GENERALINVENTORY.ITEMGROUPS',N'MENU.DEFINITIONS.INVENTORY.ITEMGROUPS') OR FormKey=N'item-groups') AND IsDeleted=0 ORDER BY CASE WHEN Code=N'MENU.DEFINITIONS.INVENTORY.ITEMGROUPS' THEN 0 ELSE 1 END);
 DECLARE @PreviousRoleAccess table(RoleId int NOT NULL PRIMARY KEY,IsAllowed bit NOT NULL);
 IF @MenuId IS NOT NULL
  INSERT @PreviousRoleAccess(RoleId,IsAllowed)
  SELECT RoleId,CONVERT(bit,MAX(CONVERT(int,IsAllowed))) FROM dbo.SecurityRoleMenus WHERE MenuId=@MenuId AND IsDeleted=0 GROUP BY RoleId;
 IF @MenuId IS NULL BEGIN
  INSERT dbo.SecurityMenus(ParentId,Code,Name,Description,MenuType,FormId,FormKey,IconLarge,IconSmall,DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt)
  VALUES(@InventoryId,N'MENU.DEFINITIONS.INVENTORY.ITEMGROUPS',N'Grupos de articulos',N'Mantenimiento de grupos de articulos',3,@FormId,N'item-groups',
   N'Accordion/inventario_32.svg',N'Accordion/inventario_16.svg',20,1,1,N'Sistema',SYSUTCDATETIME());
  SET @MenuId=CONVERT(int,SCOPE_IDENTITY());
 END;
 UPDATE dbo.SecurityMenus SET ParentId=@InventoryId,Code=N'MENU.DEFINITIONS.INVENTORY.ITEMGROUPS',Name=N'Grupos de articulos',Description=N'Mantenimiento de grupos de articulos',
  MenuType=3,FormId=@FormId,FormKey=N'item-groups',DisplayOrder=20,IsVisible=1,IsActive=1,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@MenuId;

 DECLARE @RequiredRoleMenus table(RoleId int NOT NULL,MenuId int NOT NULL,IsAllowed bit NOT NULL,PRIMARY KEY(RoleId,MenuId));
 INSERT @RequiredRoleMenus(RoleId,MenuId,IsAllowed)
 SELECT RoleId,@MenuId,IsAllowed FROM @PreviousRoleAccess;
 INSERT @RequiredRoleMenus(RoleId,MenuId,IsAllowed)
 SELECT access.RoleId,ancestor.MenuId,1 FROM @PreviousRoleAccess access
 CROSS JOIN(SELECT @ConfigurationId MenuId UNION ALL SELECT @DefinitionsId UNION ALL SELECT @InventoryId) ancestor
 WHERE access.IsAllowed=1 AND NOT EXISTS(SELECT 1 FROM @RequiredRoleMenus existing WHERE existing.RoleId=access.RoleId AND existing.MenuId=ancestor.MenuId);
 UPDATE @RequiredRoleMenus SET IsAllowed=1 WHERE RoleId=@AdminRoleId;
 INSERT @RequiredRoleMenus(RoleId,MenuId,IsAllowed)
 SELECT @AdminRoleId,menu.Id,1 FROM dbo.SecurityMenus menu WHERE menu.Id IN(@ConfigurationId,@DefinitionsId,@InventoryId,@MenuId)
 AND NOT EXISTS(SELECT 1 FROM @RequiredRoleMenus existing WHERE existing.RoleId=@AdminRoleId AND existing.MenuId=menu.Id);

 MERGE dbo.SecurityRoleMenus target USING(SELECT RoleId,MenuId,IsAllowed FROM @RequiredRoleMenus) source
 ON target.RoleId=source.RoleId AND target.MenuId=source.MenuId
 WHEN MATCHED THEN UPDATE SET IsAllowed=source.IsAllowed,IsDeleted=0,DeletedAt=NULL,DeletedByUserId=NULL,DeletedByUserName=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
 WHEN NOT MATCHED THEN INSERT(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt) VALUES(source.RoleId,source.MenuId,source.IsAllowed,N'Sistema',SYSUTCDATETIME());

 INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt)
 SELECT @AdminRoleId,@FormId,o.Id,1,N'Sistema',SYSUTCDATETIME() FROM dbo.SecurityOperations o
 WHERE o.IsDeleted=0 AND o.IsActive=1 AND LOWER(LTRIM(RTRIM(o.ActionKey))) IN
 (N'refresh',N'create',N'update',N'delete',N'consult',N'history',N'copy',N'customize-columns',N'customizecolumns',N'export-excel',N'exportexcel',N'export-pdf',N'exportpdf',N'export-json',N'exportjson',N'export-xml',N'exportxml')
 AND NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleFormOperations x WHERE x.RoleId=@AdminRoleId AND x.FormId=@FormId AND x.OperationId=o.Id);

 IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260811.187')
  INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260811.187',N'Registra Grupos de articulos en Configuracion > Definiciones > Inventario');

 COMMIT;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
GO
