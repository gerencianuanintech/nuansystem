USE [NuanSystem_Master];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
IF DB_NAME()<>N'NuanSystem_Master' THROW 51209, 'Migration 209 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51209,'MasterSchemaHistory is required.',1;
IF OBJECT_ID(N'dbo.SecurityOperations',N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityFormOperations',N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityRoleFormOperations',N'U') IS NULL THROW 51209,'Form operation tables are required.',1;
GO
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @ModuleId int=(SELECT TOP(1) Id FROM dbo.Modules WHERE Code=N'GENERALINVENTORY');
 DECLARE @ConfigurationMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.CONFIGURATION' AND IsDeleted=0);
 DECLARE @DefinitionsMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code IN(N'MENU.CONFIGURATION.DEFINITION',N'MENU.DEFINITIONS') AND IsDeleted=0 ORDER BY CASE WHEN Code=N'MENU.CONFIGURATION.DEFINITION' THEN 0 ELSE 1 END);
 DECLARE @InventoryMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code IN(N'MENU.DEFINITIONS.INVENTORY',N'MENU.CONFIGURATION,DEFINITION.INVENTORY') AND IsDeleted=0 ORDER BY CASE WHEN Code=N'MENU.DEFINITIONS.INVENTORY' THEN 0 ELSE 1 END);
 DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
 IF @ModuleId IS NULL OR @ConfigurationMenuId IS NULL OR @DefinitionsMenuId IS NULL OR @InventoryMenuId IS NULL OR @AdminRoleId IS NULL THROW 51209,'Required navigation parents are missing.',1;
 IF (SELECT COUNT(1) FROM dbo.SecurityForms WHERE (Code IN(N'FORM.GENERALINVENTORY.ITEMORIGINS',N'FORM.DEFINITIONS.INVENTORY.ITEMORIGINS') OR FormKey=N'item-origins') AND IsDeleted=0)>1 THROW 51209,'Multiple active forms identify ItemOrigins.',1;
 IF (SELECT COUNT(1) FROM dbo.SecurityMenus WHERE (Code IN(N'MENU.GENERALINVENTORY.ITEMORIGINS',N'MENU.DEFINITIONS.INVENTORY.ITEMORIGINS') OR FormKey=N'item-origins') AND IsDeleted=0)>1 THROW 51209,'Multiple active menus identify ItemOrigins.',1;
 DECLARE @Permissions table(Code nvarchar(120) PRIMARY KEY,Name nvarchar(160),Description nvarchar(300));
 INSERT @Permissions VALUES
 (N'GENERALINVENTORY.ITEMORIGINS.READ',N'Ver origenes de articulos',N'Consultar el maestro de origenes de articulos.'),
 (N'GENERALINVENTORY.ITEMORIGINS.MANAGE',N'Gestionar origenes de articulos',N'Crear, editar y eliminar origenes de articulos.');
 INSERT dbo.Permissions(ModuleId,Code,Name,Description)
 SELECT @ModuleId,p.Code,p.Name,p.Description FROM @Permissions p WHERE NOT EXISTS(SELECT 1 FROM dbo.Permissions x WHERE x.Code=p.Code);
 UPDATE target SET ModuleId=@ModuleId,Name=source.Name,Description=source.Description,IsActive=1,UpdatedAt=SYSUTCDATETIME()
 FROM dbo.Permissions target JOIN @Permissions source ON source.Code=target.Code;
 INSERT dbo.RolePermissions(RoleId,PermissionId)
 SELECT @AdminRoleId,p.Id FROM dbo.Permissions p WHERE p.Code IN(SELECT Code FROM @Permissions)
 AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions x WHERE x.RoleId=@AdminRoleId AND x.PermissionId=p.Id);
 DECLARE @FormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms WHERE Code IN(N'FORM.GENERALINVENTORY.ITEMORIGINS',N'FORM.DEFINITIONS.INVENTORY.ITEMORIGINS') OR FormKey=N'item-origins' ORDER BY IsDeleted,Id);
 IF @FormId IS NULL
 BEGIN
  INSERT dbo.SecurityForms(Code,Name,Description,FormKey,FormType,HasListView,HasEditView,IsVisible,IsActive,CreatedByUserName,CreatedAt)
  VALUES(N'FORM.GENERALINVENTORY.ITEMORIGINS',N'Origenes de articulos',N'Mantenimiento de origenes de articulos',N'item-origins',1,1,1,1,1,N'Sistema',SYSUTCDATETIME());
  SET @FormId=CONVERT(int,SCOPE_IDENTITY());
 END;
 UPDATE dbo.SecurityForms SET Code=N'FORM.GENERALINVENTORY.ITEMORIGINS',Name=N'Origenes de articulos',Description=N'Mantenimiento de origenes de articulos',
  FormKey=N'item-origins',FormType=1,HasListView=1,HasEditView=1,IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@FormId;
 DECLARE @MenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code IN(N'MENU.GENERALINVENTORY.ITEMORIGINS',N'MENU.DEFINITIONS.INVENTORY.ITEMORIGINS') OR FormKey=N'item-origins' ORDER BY IsDeleted,Id);
 IF @MenuId IS NULL
 BEGIN
  INSERT dbo.SecurityMenus(ParentId,Code,Name,Description,MenuType,FormId,FormKey,IconLarge,IconSmall,DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt)
  VALUES(@InventoryMenuId,N'MENU.DEFINITIONS.INVENTORY.ITEMORIGINS',N'Origenes de articulos',N'Mantenimiento de origenes de articulos',3,@FormId,N'item-origins',N'Accordion/inventario_32.svg',N'Accordion/inventario_16.svg',55,1,1,N'Sistema',SYSUTCDATETIME());
  SET @MenuId=CONVERT(int,SCOPE_IDENTITY());
 END;
 UPDATE dbo.SecurityMenus SET ParentId=@InventoryMenuId,Code=N'MENU.DEFINITIONS.INVENTORY.ITEMORIGINS',Name=N'Origenes de articulos',Description=N'Mantenimiento de origenes de articulos',
  MenuType=3,FormId=@FormId,FormKey=N'item-origins',DisplayOrder=55,IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
  UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@MenuId;
 UPDATE dbo.SecurityRoleMenus SET IsAllowed=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
  UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
 WHERE RoleId=@AdminRoleId AND MenuId IN(@ConfigurationMenuId,@DefinitionsMenuId,@InventoryMenuId,@MenuId);
 INSERT dbo.SecurityRoleMenus(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt)
 SELECT @AdminRoleId,parent.Id,1,N'Sistema',SYSUTCDATETIME() FROM dbo.SecurityMenus parent
 WHERE parent.Id IN(@ConfigurationMenuId,@DefinitionsMenuId,@InventoryMenuId)
 AND NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleMenus x WHERE x.RoleId=@AdminRoleId AND x.MenuId=parent.Id);
 INSERT dbo.SecurityRoleMenus(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt)
 SELECT @AdminRoleId,@MenuId,1,N'Sistema',SYSUTCDATETIME()
 WHERE NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleMenus x WHERE x.RoleId=@AdminRoleId AND x.MenuId=@MenuId);
 DECLARE @ApplicableOperations table(OperationId int PRIMARY KEY);
 INSERT @ApplicableOperations(OperationId)
 SELECT operation.Id FROM dbo.SecurityOperations operation
 WHERE operation.IsDeleted=0 AND operation.IsActive=1 AND operation.Code IN(
  N'ACTION.REFRESH',N'ACTION.CONSULT',N'ACTION.CREATE',N'ACTION.UPDATE',N'ACTION.DELETE',N'ACTION.COPY',N'ACTION.HISTORY',
  N'ACTION.CUSTOMIZE_COLUMNS',N'ACTION.EXPORT_EXCEL',N'ACTION.EXPORT_PDF',N'ACTION.EXPORT_JSON',N'ACTION.EXPORT_XML');
 IF (SELECT COUNT(*) FROM @ApplicableOperations)<>12 THROW 51209,'The twelve canonical CRUD operations are required.',1;
 UPDATE target SET IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
 FROM dbo.SecurityFormOperations target JOIN @ApplicableOperations source ON source.OperationId=target.OperationId WHERE target.FormId=@FormId;
 INSERT dbo.SecurityFormOperations(FormId,OperationId,IsActive,CreatedByUserName,CreatedAt)
 SELECT @FormId,source.OperationId,1,N'Sistema',SYSUTCDATETIME() FROM @ApplicableOperations source
 WHERE NOT EXISTS(SELECT 1 FROM dbo.SecurityFormOperations target WHERE target.FormId=@FormId AND target.OperationId=source.OperationId);
 UPDATE target SET IsAllowed=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
 FROM dbo.SecurityRoleFormOperations target JOIN @ApplicableOperations source ON source.OperationId=target.OperationId
 WHERE target.RoleId=@AdminRoleId AND target.FormId=@FormId;
 INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt)
 SELECT @AdminRoleId,@FormId,source.OperationId,1,N'Sistema',SYSUTCDATETIME() FROM @ApplicableOperations source
 WHERE NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleFormOperations target WHERE target.RoleId=@AdminRoleId AND target.FormId=@FormId AND target.OperationId=source.OperationId);
 IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.209')
  INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260813.209',N'Registers Orígenes de artículos navigation and security');
 COMMIT;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
GO
