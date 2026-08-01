/*
    Migracion 160 - Navegacion y Ribbon WinForms para perfiles y ejecuciones SAP.

    Registra dos formularios independientes de Matriz-Sucursal bajo
    Administracion > Integraciones. Reutiliza permisos SAP creados por 152 y
    concede menus/operaciones solamente al rol ADMIN aprobado para desarrollo.
    No habilita perfiles, agendas, workers ni la ejecucion manual aun no expuesta.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51160, 'Migration 160 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityForms',N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityMenus',N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityOperations',N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityRoleMenus',N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityRoleFormOperations',N'U') IS NULL
    THROW 51160, 'Security schema is required before migration 160.', 1;
IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260730.152')
    THROW 51160, 'Migration 152 is required before migration 160.', 1;
GO

BEGIN TRY
BEGIN TRANSACTION;

DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
DECLARE @IntegrationsMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.ADMINISTRATION.INTEGRATIONS' AND IsDeleted=0);
IF @AdminRoleId IS NULL OR @IntegrationsMenuId IS NULL
    THROW 51160, 'ADMIN role and Integrations menu are required before migration 160.', 1;

DECLARE @Forms table(Code nvarchar(80) PRIMARY KEY,Name nvarchar(120),Description nvarchar(300),FormKey nvarchar(120),FormType int,HasEditView bit);
INSERT @Forms VALUES
 (N'FORM.ADMINISTRATION.SAP.SYNC_PROFILES',N'Perfiles SAP',N'Configuracion independiente de sincronizacion SAP Business One.',N'sap-sync-profiles',1,1),
 (N'FORM.ADMINISTRATION.SAP.SYNC_EXECUTIONS',N'Ejecuciones SAP',N'Monitor operativo de ejecuciones SAP Business One.',N'sap-sync-executions',3,0);

MERGE dbo.SecurityForms target USING @Forms source ON target.Code=source.Code OR target.FormKey=source.FormKey
WHEN MATCHED THEN UPDATE SET Code=source.Code,Name=source.Name,Description=source.Description,FormKey=source.FormKey,FormType=source.FormType,
 HasListView=1,HasEditView=source.HasEditView,IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(Code,Name,Description,FormKey,FormType,HasListView,HasEditView,IsVisible,IsActive,CreatedByUserName,CreatedAt)
 VALUES(source.Code,source.Name,source.Description,source.FormKey,source.FormType,1,source.HasEditView,1,1,N'Sistema',SYSUTCDATETIME());

DECLARE @ProfilesFormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms WHERE FormKey=N'sap-sync-profiles' AND IsDeleted=0);
DECLARE @ExecutionsFormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms WHERE FormKey=N'sap-sync-executions' AND IsDeleted=0);

DECLARE @Menus table(Code nvarchar(80) PRIMARY KEY,Name nvarchar(120),Description nvarchar(300),FormId int,FormKey nvarchar(120),DisplayOrder int);
INSERT @Menus VALUES
 (N'MENU.ADMINISTRATION.INTEGRATIONS.SAP.PROFILES',N'Perfiles SAP',N'Configuracion de perfiles SAP Business One.',@ProfilesFormId,N'sap-sync-profiles',30),
 (N'MENU.ADMINISTRATION.INTEGRATIONS.SAP.EXECUTIONS',N'Ejecuciones SAP',N'Monitoreo de ejecuciones SAP Business One.',@ExecutionsFormId,N'sap-sync-executions',40);

MERGE dbo.SecurityMenus target USING @Menus source ON target.Code=source.Code OR target.FormKey=source.FormKey
WHEN MATCHED THEN UPDATE SET ParentId=@IntegrationsMenuId,Code=source.Code,Name=source.Name,Description=source.Description,MenuType=3,FormId=source.FormId,FormKey=source.FormKey,
 DisplayOrder=source.DisplayOrder,IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(ParentId,Code,Name,Description,MenuType,FormId,FormKey,IconLarge,IconSmall,DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt)
 VALUES(@IntegrationsMenuId,source.Code,source.Name,source.Description,3,source.FormId,source.FormKey,N'Accordion/sync_32.svg',N'Accordion/sync_16.svg',source.DisplayOrder,1,1,N'Sistema',SYSUTCDATETIME());

DECLARE @ProfileMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE FormKey=N'sap-sync-profiles' AND IsDeleted=0);
DECLARE @ExecutionMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE FormKey=N'sap-sync-executions' AND IsDeleted=0);

MERGE dbo.SecurityRoleMenus target USING(SELECT Id MenuId FROM dbo.SecurityMenus WHERE Id IN(@IntegrationsMenuId,@ProfileMenuId,@ExecutionMenuId)) source
ON target.RoleId=@AdminRoleId AND target.MenuId=source.MenuId
WHEN MATCHED THEN UPDATE SET IsAllowed=1,IsDeleted=0,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt) VALUES(@AdminRoleId,source.MenuId,1,N'Sistema',SYSUTCDATETIME());

DECLARE @FormOperations table(FormId int,OperationCode nvarchar(80),PRIMARY KEY(FormId,OperationCode));
INSERT @FormOperations VALUES
 (@ProfilesFormId,N'ACTION.REFRESH'),(@ProfilesFormId,N'ACTION.CREATE'),(@ProfilesFormId,N'ACTION.UPDATE'),(@ProfilesFormId,N'ACTION.DELETE'),(@ProfilesFormId,N'ACTION.CONSULT'),
 (@ProfilesFormId,N'ACTION.CUSTOMIZE_COLUMNS'),(@ProfilesFormId,N'ACTION.FILTER'),(@ProfilesFormId,N'ACTION.VIEWEXECUTIONS'),
 (@ProfilesFormId,N'ACTION.SAP_SYNC_PROFILES.VALIDATE'),(@ProfilesFormId,N'ACTION.SAP_SYNC_PROFILES.ACTIVATE'),(@ProfilesFormId,N'ACTION.SAP_SYNC_PROFILES.DEACTIVATE'),
 (@ExecutionsFormId,N'ACTION.REFRESH'),(@ExecutionsFormId,N'ACTION.CONSULT'),(@ExecutionsFormId,N'ACTION.CUSTOMIZE_COLUMNS'),(@ExecutionsFormId,N'ACTION.FILTER'),
 (@ExecutionsFormId,N'ACTION.SAP_SYNC_EXECUTIONS.RETRY'),(@ExecutionsFormId,N'ACTION.SAP_SYNC_EXECUTIONS.CANCEL'),(@ExecutionsFormId,N'ACTION.SAP_SYNC_EXECUTIONS.RELEASE_EXPIRED_LOCK');

IF EXISTS(SELECT 1 FROM @FormOperations requested LEFT JOIN dbo.SecurityOperations operation ON operation.Code=requested.OperationCode AND operation.IsActive=1 AND operation.IsDeleted=0 WHERE operation.Id IS NULL)
    THROW 51160, 'A required SAP WinForms operation is missing.', 1;

MERGE dbo.SecurityRoleFormOperations target USING
(
 SELECT @AdminRoleId RoleId,requested.FormId,operation.Id OperationId
 FROM @FormOperations requested INNER JOIN dbo.SecurityOperations operation ON operation.Code=requested.OperationCode AND operation.IsActive=1 AND operation.IsDeleted=0
) source
ON target.RoleId=source.RoleId AND target.FormId=source.FormId AND target.OperationId=source.OperationId
WHEN MATCHED THEN UPDATE SET IsAllowed=1,IsDeleted=0,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt) VALUES(source.RoleId,source.FormId,source.OperationId,1,N'Sistema',SYSUTCDATETIME());

IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260731.160')
 INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260731.160',N'Navegacion y Ribbon WinForms independientes para perfiles y ejecuciones SAP');

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
GO
