/* Master 236: navigation/security for SAP integration settings. */
IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityMenus', N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL OR OBJECT_ID(N'dbo.SecurityFormOperations', N'U') IS NULL
    THROW 51236, 'Security schema is required before migration 236.', 1;
GO
BEGIN TRY
BEGIN TRANSACTION;
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @ParentMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.ADMINISTRATION.INTEGRATIONS' AND IsDeleted = 0);
IF @AdminRoleId IS NULL OR @ParentMenuId IS NULL THROW 51236, 'ADMIN role and Integrations menu are required.', 1;
MERGE dbo.SecurityForms AS target
USING (SELECT N'FORM.ADMINISTRATION.SAP.INTEGRATION_SETTINGS' Code, N'Configuración general SAP' Name, N'Conexión y opciones generales de SAP.' Description, N'sap-integration-settings' FormKey) source
ON target.Code = source.Code OR target.FormKey = source.FormKey
WHEN MATCHED THEN UPDATE SET Code=source.Code, Name=source.Name, Description=source.Description, FormKey=source.FormKey, FormType=2, HasListView=0, HasEditView=1, IsVisible=1, IsActive=1, IsDeleted=0, UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(Code,Name,Description,FormKey,FormType,HasListView,HasEditView,IsVisible,IsActive,CreatedByUserName,CreatedAt) VALUES(source.Code,source.Name,source.Description,source.FormKey,2,0,1,1,1,N'Sistema',SYSUTCDATETIME());
DECLARE @FormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE FormKey=N'sap-integration-settings' AND IsDeleted=0);
MERGE dbo.SecurityMenus AS target
USING (SELECT N'MENU.ADMINISTRATION.INTEGRATIONS.SAP.INTEGRATION_SETTINGS' Code, N'Configuración general SAP' Name, @FormId FormId) source
ON target.Code=source.Code OR target.FormKey=N'sap-integration-settings'
WHEN MATCHED THEN UPDATE SET ParentId=@ParentMenuId,Name=source.Name,FormId=source.FormId,FormKey=N'sap-integration-settings',MenuType=3,DisplayOrder=49,IsVisible=1,IsActive=1,IsDeleted=0,UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(ParentId,Code,Name,Description,MenuType,FormId,FormKey,IconLarge,IconSmall,DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt) VALUES(@ParentMenuId,source.Code,source.Name,N'Configuración general de la integración SAP.',3,source.FormId,N'sap-integration-settings',N'Accordion/sync_32.svg',N'Accordion/sync_16.svg',49,1,1,N'Sistema',SYSUTCDATETIME());
DECLARE @MenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE FormKey=N'sap-integration-settings' AND IsDeleted=0);
MERGE dbo.SecurityRoleMenus AS target USING (SELECT @AdminRoleId RoleId,@MenuId MenuId) source ON target.RoleId=source.RoleId AND target.MenuId=source.MenuId
WHEN MATCHED THEN UPDATE SET IsAllowed=1,IsDeleted=0,UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt) VALUES(source.RoleId,source.MenuId,1,N'Sistema',SYSUTCDATETIME());
INSERT dbo.SecurityFormOperations(FormId,OperationId,IsActive,CreatedAt)
SELECT @FormId,o.Id,1,SYSUTCDATETIME() FROM dbo.SecurityOperations o WHERE o.Code IN (N'ACTION.CONSULT',N'ACTION.UPDATE') AND o.IsActive=1 AND o.IsDeleted=0 AND NOT EXISTS (SELECT 1 FROM dbo.SecurityFormOperations x WHERE x.FormId=@FormId AND x.OperationId=o.Id);
INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt)
SELECT @AdminRoleId,@FormId,o.Id,1,N'Sistema',SYSUTCDATETIME() FROM dbo.SecurityOperations o WHERE o.Code IN (N'ACTION.CONSULT',N'ACTION.UPDATE') AND o.IsActive=1 AND o.IsDeleted=0 AND NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleFormOperations x WHERE x.RoleId=@AdminRoleId AND x.FormId=@FormId AND x.OperationId=o.Id);
COMMIT TRANSACTION;
END TRY
BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK TRANSACTION; THROW; END CATCH;
GO
