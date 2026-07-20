/*
    Fase 5.2 - Permisos API para la cola de documentos autorizados SRI.
    Ejecutar exclusivamente en NuanSystem_Master. No registra formulario ni menu.
*/
SET NOCOUNT ON;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SRI')
BEGIN
    INSERT dbo.Modules(Code, Name, DisplayOrder)
    VALUES(N'SRI', N'SRI', 70);
END;
ELSE
BEGIN
    UPDATE dbo.Modules SET Name=N'SRI', DisplayOrder=70, IsActive=1, UpdatedAt=SYSUTCDATETIME()
    WHERE Code=N'SRI';
END;
GO

DECLARE @ModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code=N'SRI');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code=N'ADMIN');
DECLARE @Permissions table(Code nvarchar(120) PRIMARY KEY, Name nvarchar(160), Description nvarchar(300));

INSERT @Permissions(Code, Name, Description)
VALUES
 (N'SRI.DOCUMENTS.VIEW', N'Ver consultas SRI', N'Consultar la cola y los intentos de documentos autorizados SRI.'),
 (N'SRI.DOCUMENTS.ENQUEUE', N'Encolar consultas SRI', N'Registrar una clave de acceso para consulta posterior al SRI.'),
 (N'SRI.DOCUMENTS.CANCEL', N'Cancelar consultas SRI', N'Cancelar consultas SRI pendientes o programadas para reintento.'),
 (N'SRI.DOCUMENTS.REPROCESS', N'Reprocesar consultas SRI', N'Reactivar consultas SRI fallidas o enviadas a Dead Letter.'),
 (N'SRI.DOCUMENTS.VIEW_PAYLOAD', N'Ver contenido SRI', N'Ver el contenido autorizado recuperado del SRI cuando la fase de almacenamiento este habilitada.'),
 (N'SRI.DOCUMENTS.DOWNLOAD_XML', N'Descargar XML SRI', N'Descargar XML autorizado cuando la fase de almacenamiento este habilitada.');

MERGE dbo.Permissions AS target
USING @Permissions AS source ON target.Code=source.Code
WHEN MATCHED THEN UPDATE SET ModuleId=@ModuleId, Name=source.Name, Description=source.Description, IsActive=1, UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(ModuleId, Code, Name, Description) VALUES(@ModuleId, source.Code, source.Name, source.Description);

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT dbo.RolePermissions(RoleId, PermissionId)
    SELECT @AdminRoleId, permission.Id
    FROM dbo.Permissions permission
    WHERE permission.Code IN (SELECT Code FROM @Permissions)
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.RolePermissions existing
          WHERE existing.RoleId=@AdminRoleId AND existing.PermissionId=permission.Id
      );
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260720.116')
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES(N'20260720.116', N'Permisos API para cola de documentos autorizados SRI');
GO
