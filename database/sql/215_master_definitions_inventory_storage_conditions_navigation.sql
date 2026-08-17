/* Migra seguridad/navegacion legacy de StorageConditions sin cambiar FormId, MenuId ni accesos existentes. */
USE [NuanSystem_Master];
GO
SET NOCOUNT ON; SET XACT_ABORT ON;
IF DB_NAME()<>N'NuanSystem_Master' THROW 51215,'Migration 215 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51215,'MasterSchemaHistory is required.',1;
GO
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @ModuleId int=(SELECT TOP(1) Id FROM dbo.Modules WHERE Code=N'GENERALINVENTORY');
 DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
 DECLARE @ConfigurationMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.CONFIGURATION' AND IsDeleted=0);
 DECLARE @DefinitionsMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code IN(N'MENU.CONFIGURATION.DEFINITION',N'MENU.DEFINITIONS') AND IsDeleted=0 ORDER BY CASE WHEN Code=N'MENU.CONFIGURATION.DEFINITION' THEN 0 ELSE 1 END);
 DECLARE @InventoryMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code IN(N'MENU.DEFINITIONS.INVENTORY',N'MENU.CONFIGURATION,DEFINITION.INVENTORY') AND IsDeleted=0 ORDER BY CASE WHEN Code=N'MENU.DEFINITIONS.INVENTORY' THEN 0 ELSE 1 END);
 IF @ModuleId IS NULL OR @AdminRoleId IS NULL OR @ConfigurationMenuId IS NULL OR @DefinitionsMenuId IS NULL OR @InventoryMenuId IS NULL THROW 51215,'Required security parents are missing.',1;
 IF (SELECT COUNT(1) FROM dbo.SecurityForms WHERE (Code IN(N'FORM.GENERALINVENTORY.STORAGECONDITIONS',N'FORM.DEFINITIONS.INVENTORY.STORAGECONDITIONS') OR FormKey IN(N'inventory-storage-conditions',N'storage-conditions')) AND IsDeleted=0)>1 THROW 51215,'Multiple active forms identify StorageConditions.',1;
 IF (SELECT COUNT(1) FROM dbo.SecurityMenus WHERE (Code IN(N'MENU.GENERALINVENTORY.STORAGECONDITIONS',N'MENU.DEFINITIONS.INVENTORY.STORAGECONDITIONS') OR FormKey IN(N'inventory-storage-conditions',N'storage-conditions')) AND IsDeleted=0)>1 THROW 51215,'Multiple active menus identify StorageConditions.',1;

 DECLARE @Permissions table(Code nvarchar(120) PRIMARY KEY,Name nvarchar(160),Description nvarchar(300));
 INSERT @Permissions VALUES(N'GENERALINVENTORY.STORAGECONDITIONS.READ',N'Ver condiciones de almacenamiento',N'Consultar el maestro de condiciones de almacenamiento.'),(N'GENERALINVENTORY.STORAGECONDITIONS.MANAGE',N'Gestionar condiciones de almacenamiento',N'Crear, editar y eliminar condiciones de almacenamiento.');
 INSERT dbo.Permissions(ModuleId,Code,Name,Description) SELECT @ModuleId,p.Code,p.Name,p.Description FROM @Permissions p WHERE NOT EXISTS(SELECT 1 FROM dbo.Permissions x WHERE x.Code=p.Code);
 UPDATE target SET ModuleId=@ModuleId,Name=source.Name,Description=source.Description,IsActive=1,UpdatedAt=SYSUTCDATETIME() FROM dbo.Permissions target JOIN @Permissions source ON source.Code=target.Code;
 INSERT dbo.RolePermissions(RoleId,PermissionId) SELECT @AdminRoleId,p.Id FROM dbo.Permissions p WHERE p.Code IN(SELECT Code FROM @Permissions) AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions x WHERE x.RoleId=@AdminRoleId AND x.PermissionId=p.Id);

 DECLARE @FormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms WHERE (Code IN(N'FORM.GENERALINVENTORY.STORAGECONDITIONS',N'FORM.DEFINITIONS.INVENTORY.STORAGECONDITIONS') OR FormKey IN(N'inventory-storage-conditions',N'storage-conditions')) AND IsDeleted=0 ORDER BY CASE WHEN FormKey=N'storage-conditions' THEN 0 ELSE 1 END);
 IF @FormId IS NULL BEGIN INSERT dbo.SecurityForms(Code,Name,Description,FormKey,FormType,IsVisible,IsActive,CreatedByUserName,CreatedAt) VALUES(N'FORM.GENERALINVENTORY.STORAGECONDITIONS',N'Condiciones de almacenamiento',N'Mantenimiento de condiciones de almacenamiento',N'storage-conditions',1,1,1,N'Sistema',SYSUTCDATETIME()); SET @FormId=CONVERT(int,SCOPE_IDENTITY()); END;
 UPDATE dbo.SecurityForms SET Code=N'FORM.GENERALINVENTORY.STORAGECONDITIONS',Name=N'Condiciones de almacenamiento',Description=N'Mantenimiento de condiciones de almacenamiento',FormKey=N'storage-conditions',FormType=1,IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@FormId;

 DECLARE @MenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE (Code IN(N'MENU.GENERALINVENTORY.STORAGECONDITIONS',N'MENU.DEFINITIONS.INVENTORY.STORAGECONDITIONS') OR FormKey IN(N'inventory-storage-conditions',N'storage-conditions')) AND IsDeleted=0 ORDER BY CASE WHEN FormKey=N'storage-conditions' THEN 0 ELSE 1 END);
 DECLARE @PreviousRoleAccess table(RoleId int PRIMARY KEY,IsAllowed bit);
 IF @MenuId IS NOT NULL INSERT @PreviousRoleAccess SELECT RoleId,CONVERT(bit,MAX(CONVERT(int,IsAllowed))) FROM dbo.SecurityRoleMenus WHERE MenuId=@MenuId AND IsDeleted=0 GROUP BY RoleId;
 IF @MenuId IS NULL BEGIN INSERT dbo.SecurityMenus(ParentId,Code,Name,Description,MenuType,FormId,FormKey,IconLarge,IconSmall,DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt) VALUES(@InventoryMenuId,N'MENU.DEFINITIONS.INVENTORY.STORAGECONDITIONS',N'Condiciones de almacenamiento',N'Mantenimiento de condiciones de almacenamiento',3,@FormId,N'storage-conditions',N'Accordion/inventario_32.svg',N'Accordion/inventario_16.svg',110,1,1,N'Sistema',SYSUTCDATETIME()); SET @MenuId=CONVERT(int,SCOPE_IDENTITY()); END;
 UPDATE dbo.SecurityMenus SET ParentId=@InventoryMenuId,Code=N'MENU.DEFINITIONS.INVENTORY.STORAGECONDITIONS',Name=N'Condiciones de almacenamiento',Description=N'Mantenimiento de condiciones de almacenamiento',MenuType=3,FormId=@FormId,FormKey=N'storage-conditions',DisplayOrder=110,IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@MenuId;
 INSERT dbo.SecurityRoleMenus(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt) SELECT @AdminRoleId,m.Id,1,N'Sistema',SYSUTCDATETIME() FROM dbo.SecurityMenus m WHERE m.Id IN(@ConfigurationMenuId,@DefinitionsMenuId,@InventoryMenuId,@MenuId) AND NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleMenus x WHERE x.RoleId=@AdminRoleId AND x.MenuId=m.Id AND x.IsDeleted=0);
 UPDATE access SET IsAllowed=previous.IsAllowed,IsDeleted=0,UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema' FROM dbo.SecurityRoleMenus access JOIN @PreviousRoleAccess previous ON previous.RoleId=access.RoleId WHERE access.MenuId=@MenuId;
 INSERT dbo.SecurityRoleMenus(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt) SELECT previous.RoleId,@MenuId,previous.IsAllowed,N'Sistema',SYSUTCDATETIME() FROM @PreviousRoleAccess previous WHERE NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleMenus access WHERE access.RoleId=previous.RoleId AND access.MenuId=@MenuId);

 INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt) SELECT @AdminRoleId,@FormId,o.Id,1,N'Sistema',SYSUTCDATETIME() FROM dbo.SecurityOperations o WHERE o.IsDeleted=0 AND o.IsActive=1 AND LOWER(LTRIM(RTRIM(o.ActionKey))) IN(N'refresh',N'create',N'update',N'delete',N'consult',N'history',N'copy',N'customize-columns',N'customizecolumns',N'export-excel',N'exportexcel',N'export-pdf',N'exportpdf',N'export-json',N'exportjson',N'export-xml',N'exportxml') AND NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleFormOperations x WHERE x.RoleId=@AdminRoleId AND x.FormId=@FormId AND x.OperationId=o.Id);
 IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.215') INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260813.215',N'Migra StorageConditions preservando formulario, menu y accesos legacy');
 COMMIT;
END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH;
GO


