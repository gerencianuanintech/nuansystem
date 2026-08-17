USE [NuanSystem_Master];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
IF DB_NAME()<>N'NuanSystem_Master' THROW 51227, 'Migration 227 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51227, 'MasterSchemaHistory is required.', 1;
IF OBJECT_ID(N'dbo.SecurityFormOperations',N'U') IS NULL THROW 51227, 'SecurityFormOperations is required.', 1;
IF OBJECT_ID(N'dbo.SecurityRoleFormOperations',N'U') IS NULL THROW 51227, 'SecurityRoleFormOperations is required.', 1;
GO
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @ModuleId int=(SELECT TOP(1) Id FROM dbo.Modules WHERE Code=N'GENERALINVENTORY' AND IsActive=1);
 DECLARE @InventoryMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.DEFINITIONS.INVENTORY' AND IsDeleted=0);
 DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
 IF @ModuleId IS NULL OR @InventoryMenuId IS NULL OR @AdminRoleId IS NULL THROW 51227,'Required navigation parents are missing.',1;
 DECLARE @Permissions table(Code nvarchar(120) PRIMARY KEY,Name nvarchar(160));
 INSERT @Permissions VALUES(N'GENERALINVENTORY.SALESCHANNELS.READ',N'Ver Canales de venta'),(N'GENERALINVENTORY.SALESCHANNELS.MANAGE',N'Gestionar Canales de venta');
 INSERT dbo.Permissions(ModuleId,Code,Name,Description)
 SELECT @ModuleId,p.Code,p.Name,p.Name FROM @Permissions p WHERE NOT EXISTS(SELECT 1 FROM dbo.Permissions x WHERE x.Code=p.Code);
 UPDATE target SET ModuleId=@ModuleId,Name=source.Name,Description=source.Name,IsActive=1,UpdatedAt=SYSUTCDATETIME()
 FROM dbo.Permissions target JOIN @Permissions source ON source.Code=target.Code;
 INSERT dbo.RolePermissions(RoleId,PermissionId)
 SELECT @AdminRoleId,p.Id FROM dbo.Permissions p WHERE p.Code IN(SELECT Code FROM @Permissions)
 AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions x WHERE x.RoleId=@AdminRoleId AND x.PermissionId=p.Id);
 DECLARE @FormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms
  WHERE FormKey IN(N'sales-channels',N'inventory-sales-channels')
     OR Code IN(N'FORM.DEFINITIONS.INVENTORY.SalesChannels',N'FORM.GENERALINVENTORY.SALESCHANNELS')
  ORDER BY IsDeleted,Id);
 IF @FormId IS NULL
 BEGIN
  INSERT dbo.SecurityForms(Code,Name,Description,FormKey,FormType,HasListView,HasEditView,IsVisible,IsActive,CreatedByUserName,CreatedAt)
  VALUES(N'FORM.DEFINITIONS.INVENTORY.SalesChannels',N'Canales de venta',N'Mantenimiento de Canales de venta',N'sales-channels',1,1,1,1,1,N'Sistema',SYSUTCDATETIME());
  SET @FormId=CONVERT(int,SCOPE_IDENTITY());
 END;
 UPDATE dbo.SecurityForms SET Code=N'FORM.DEFINITIONS.INVENTORY.SalesChannels',Name=N'Canales de venta',Description=N'Mantenimiento de Canales de venta',
  FormKey=N'sales-channels',FormType=1,HasListView=1,HasEditView=1,IsVisible=1,IsActive=1,IsDeleted=0,
  DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@FormId;
 DECLARE @MenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus
  WHERE FormKey IN(N'sales-channels',N'inventory-sales-channels')
     OR Code IN(N'MENU.DEFINITIONS.INVENTORY.SALESCHANNELS',N'MENU.GENERALINVENTORY.SALESCHANNELS')
  ORDER BY IsDeleted,Id);
 IF @MenuId IS NULL
 BEGIN
  INSERT dbo.SecurityMenus(ParentId,Code,Name,Description,MenuType,FormId,FormKey,DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt)
  VALUES(@InventoryMenuId,N'MENU.DEFINITIONS.INVENTORY.SALESCHANNELS',N'Canales de venta',N'Mantenimiento de Canales de venta',3,@FormId,N'sales-channels',100,1,1,N'Sistema',SYSUTCDATETIME());
  SET @MenuId=CONVERT(int,SCOPE_IDENTITY());
 END;
 UPDATE dbo.SecurityMenus SET ParentId=@InventoryMenuId,Code=N'MENU.DEFINITIONS.INVENTORY.SALESCHANNELS',Name=N'Canales de venta',Description=N'Mantenimiento de Canales de venta',
  MenuType=3,FormId=@FormId,FormKey=N'sales-channels',DisplayOrder=100,IsVisible=1,IsActive=1,IsDeleted=0,
  DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@MenuId;
 UPDATE dbo.SecurityRoleMenus SET IsAllowed=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
  UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE RoleId=@AdminRoleId AND MenuId=@MenuId;
 INSERT dbo.SecurityRoleMenus(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt)
 SELECT @AdminRoleId,@MenuId,1,N'Sistema',SYSUTCDATETIME()
 WHERE NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleMenus x WHERE x.RoleId=@AdminRoleId AND x.MenuId=@MenuId);
 DECLARE @ApplicableOperations table(OperationId int PRIMARY KEY);
 INSERT @ApplicableOperations(OperationId)
 SELECT o.Id FROM dbo.SecurityOperations o
 WHERE o.IsDeleted=0 AND o.IsActive=1 AND o.Code IN(
  N'ACTION.REFRESH',N'ACTION.CONSULT',N'ACTION.CREATE',N'ACTION.UPDATE',N'ACTION.DELETE',N'ACTION.COPY',N'ACTION.HISTORY',
  N'ACTION.CUSTOMIZE_COLUMNS',N'ACTION.EXPORT_EXCEL',N'ACTION.EXPORT_PDF',N'ACTION.EXPORT_JSON',N'ACTION.EXPORT_XML');
 IF (SELECT COUNT(*) FROM @ApplicableOperations)<>12 THROW 51227,'The twelve canonical CRUD operations are required.',1;
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
 IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260817.227')
  INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260817.227',N'Registers Canales de venta navigation and security');
 COMMIT;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
GO

