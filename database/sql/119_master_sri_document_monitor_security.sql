/* Fase 5.5: formulario, menu y operaciones del monitor SRI. Master only. */
SET NOCOUNT ON;
GO
DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
DECLARE @ParentId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.ADMINISTRATION' AND IsDeleted=0);
DECLARE @FormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms WHERE FormKey=N'sri-document-monitor' OR Code=N'FORM.ADMINISTRATION.SRI.MONITOR' ORDER BY CASE WHEN FormKey=N'sri-document-monitor' THEN 0 ELSE 1 END);
IF @FormId IS NULL
BEGIN
 INSERT dbo.SecurityForms(Code,Name,Description,FormKey,FormType,HasListView,HasEditView,IsVisible,IsActive,CreatedByUserName,CreatedAt)
 VALUES(N'FORM.ADMINISTRATION.SRI.MONITOR',N'Monitor de documentos SRI',N'Consulta y descarga protegida de XML autorizado.',N'sri-document-monitor',3,1,0,1,1,N'Sistema',SYSUTCDATETIME());
 SET @FormId=CONVERT(int,SCOPE_IDENTITY());
END
ELSE UPDATE dbo.SecurityForms SET Code=N'FORM.ADMINISTRATION.SRI.MONITOR',Name=N'Monitor de documentos SRI',Description=N'Consulta y descarga protegida de XML autorizado.',FormKey=N'sri-document-monitor',FormType=3,HasListView=1,HasEditView=0,IsVisible=1,IsActive=1,IsDeleted=0,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Id=@FormId;

IF NOT EXISTS(SELECT 1 FROM dbo.SecurityMenus WHERE Code=N'MENU.ADMINISTRATION.SRI.MONITOR' AND IsDeleted=0)
 INSERT dbo.SecurityMenus(ParentId,Code,Name,Description,MenuType,FormId,FormKey,IconLarge,IconSmall,DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt)
 VALUES(@ParentId,N'MENU.ADMINISTRATION.SRI.MONITOR',N'Monitor SRI',N'Documentos autorizados SRI.',3,@FormId,N'sri-document-monitor',N'Accordion/document_32.svg',N'Accordion/document_16.svg',40,1,1,N'Sistema',SYSUTCDATETIME());
ELSE UPDATE dbo.SecurityMenus SET ParentId=@ParentId,Name=N'Monitor SRI',Description=N'Documentos autorizados SRI.',MenuType=3,FormId=@FormId,FormKey=N'sri-document-monitor',DisplayOrder=40,IsVisible=1,IsActive=1,IsDeleted=0,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Code=N'MENU.ADMINISTRATION.SRI.MONITOR';

DECLARE @Ops table(Code nvarchar(80),Name nvarchar(120),Description nvarchar(300),ActionKey nvarchar(120),DisplayOrder int);
INSERT @Ops VALUES(N'ACTION.REFRESH',N'Actualizar',N'Recargar informacion.',N'refresh',10),(N'ACTION.CONSULT',N'Consultar',N'Consultar detalle e historial.',N'consult',20),(N'ACTION.DOWNLOAD_XML',N'Descargar XML',N'Descargar XML autorizado.',N'download-xml',30),(N'ACTION.FILTER',N'Filtro',N'Aplicar filtros al monitor.',N'filter',40);
MERGE dbo.SecurityOperations t USING @Ops s ON t.Code=s.Code WHEN MATCHED THEN UPDATE SET Name=s.Name,Description=s.Description,ActionKey=s.ActionKey,DisplayOrder=s.DisplayOrder,IsActive=1,IsDeleted=0,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHEN NOT MATCHED THEN INSERT(Code,Name,Description,ActionKey,DisplayOrder,IsActive,CreatedByUserName,CreatedAt,IsDeleted) VALUES(s.Code,s.Name,s.Description,s.ActionKey,s.DisplayOrder,1,N'Sistema',SYSUTCDATETIME(),0);
UPDATE dbo.SecurityOperations SET RibbonPageName=N'Inicio',RibbonGroupName=N'Acciones',IconLarge=N'Operaciones/xml_32.svg',IconSmall=N'Operaciones/xml_16.svg',UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Code=N'ACTION.DOWNLOAD_XML';
UPDATE dbo.SecurityOperations SET RibbonPageName=N'Inicio',RibbonGroupName=N'Personalizacion',IconLarge=N'Ribbon/filtro_32.svg',IconSmall=N'Ribbon/filtro_16.svg',UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE Code=N'ACTION.FILTER';
IF @AdminRoleId IS NOT NULL
BEGIN
 INSERT dbo.SecurityRoleMenus(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt) SELECT @AdminRoleId,m.Id,1,N'Sistema',SYSUTCDATETIME() FROM dbo.SecurityMenus m WHERE m.Code=N'MENU.ADMINISTRATION.SRI.MONITOR' AND NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleMenus x WHERE x.RoleId=@AdminRoleId AND x.MenuId=m.Id);
 INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt) SELECT @AdminRoleId,@FormId,o.Id,1,N'Sistema',SYSUTCDATETIME() FROM dbo.SecurityOperations o WHERE o.ActionKey IN(N'refresh',N'consult',N'download-xml',N'filter') AND NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleFormOperations x WHERE x.RoleId=@AdminRoleId AND x.FormId=@FormId AND x.OperationId=o.Id);
 INSERT dbo.RolePermissions(RoleId,PermissionId) SELECT @AdminRoleId,p.Id FROM dbo.Permissions p WHERE p.Code IN(N'SRI.DOCUMENTS.VIEW',N'SRI.DOCUMENTS.VIEW_PAYLOAD',N'SRI.DOCUMENTS.DOWNLOAD_XML') AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions x WHERE x.RoleId=@AdminRoleId AND x.PermissionId=p.Id);
END;
GO
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260721.119') INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260721.119',N'Seguridad y navegacion del monitor SRI');
GO
