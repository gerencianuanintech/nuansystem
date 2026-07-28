/*
    Migracion 149 - Acciones Ribbon para el Monitor SRI.

    Vincula Actualizar, Consultar, Filtro y Descargar XML al formulario
    sri-document-monitor según los permisos API ya concedidos. No concede
    permisos API, menus ni acceso a roles nuevos.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51149, 'Migration 149 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL
    THROW 51149, 'MasterSchemaHistory is required before migration 149.', 1;
IF OBJECT_ID(N'dbo.SecurityForms',N'U') IS NULL
    THROW 51149, 'SecurityForms is required before migration 149.', 1;
IF OBJECT_ID(N'dbo.SecurityOperations',N'U') IS NULL
    THROW 51149, 'SecurityOperations is required before migration 149.', 1;
IF OBJECT_ID(N'dbo.SecurityRoleFormOperations',N'U') IS NULL
    THROW 51149, 'SecurityRoleFormOperations is required before migration 149.', 1;
IF OBJECT_ID(N'dbo.RolePermissions',N'U') IS NULL
    THROW 51149, 'RolePermissions is required before migration 149.', 1;
IF OBJECT_ID(N'dbo.Permissions',N'U') IS NULL
    THROW 51149, 'Permissions is required before migration 149.', 1;
GO

BEGIN TRY
BEGIN TRANSACTION;

DECLARE @FormId int=
(
    SELECT TOP (1) Id
    FROM dbo.SecurityForms
    WHERE FormKey=N'sri-document-monitor'
      AND IsActive=1
      AND IsDeleted=0
);

IF @FormId IS NULL
    THROW 51149, 'The sri-document-monitor form is required before migration 149.', 1;

DECLARE @RefreshOperationId int=
(
    SELECT TOP (1) Id FROM dbo.SecurityOperations
    WHERE Code=N'ACTION.REFRESH' AND IsActive=1 AND IsDeleted=0
);
DECLARE @ConsultOperationId int=
(
    SELECT TOP (1) Id FROM dbo.SecurityOperations
    WHERE Code=N'ACTION.CONSULT' AND IsActive=1 AND IsDeleted=0
);
DECLARE @FilterOperationId int=
(
    SELECT TOP (1) Id FROM dbo.SecurityOperations
    WHERE Code=N'ACTION.FILTER' AND IsActive=1 AND IsDeleted=0
);
DECLARE @DownloadOperationId int=
(
    SELECT TOP (1) Id FROM dbo.SecurityOperations
    WHERE Code=N'ACTION.DOWNLOAD_XML' AND IsActive=1 AND IsDeleted=0
);

IF @RefreshOperationId IS NULL OR @ConsultOperationId IS NULL OR @FilterOperationId IS NULL OR @DownloadOperationId IS NULL
    THROW 51149, 'Required Monitor SRI operations are missing.', 1;

UPDATE dbo.SecurityOperations
SET Name=N'Descargar XML',
    Description=N'Descargar el XML autorizado seleccionado.',
    ActionKey=N'download-xml',
    RibbonPageName=N'Inicio',
    RibbonGroupName=N'Acciones',
    IconLarge=N'Operaciones/xml_32.svg',
    IconSmall=N'Operaciones/xml_16.svg',
    DisplayOrder=30,
    IsActive=1,
    IsDeleted=0,
    DeletedByUserId=NULL,
    DeletedByUserName=NULL,
    DeletedAt=NULL,
    UpdatedByUserName=N'Sistema',
    UpdatedAt=SYSUTCDATETIME()
WHERE Id=@DownloadOperationId;

DECLARE @RoleOperations table
(
    RoleId int NOT NULL,
    OperationId int NOT NULL,
    PRIMARY KEY(RoleId,OperationId)
);

INSERT @RoleOperations(RoleId,OperationId)
SELECT DISTINCT rolePermission.RoleId,@RefreshOperationId
FROM dbo.RolePermissions rolePermission
INNER JOIN dbo.Permissions permission ON permission.Id=rolePermission.PermissionId
WHERE permission.Code=N'SRI.DOCUMENTS.VIEW' AND permission.IsActive=1
UNION
SELECT DISTINCT rolePermission.RoleId,@ConsultOperationId
FROM dbo.RolePermissions rolePermission
INNER JOIN dbo.Permissions permission ON permission.Id=rolePermission.PermissionId
WHERE permission.Code=N'SRI.DOCUMENTS.VIEW' AND permission.IsActive=1
UNION
SELECT DISTINCT rolePermission.RoleId,@FilterOperationId
FROM dbo.RolePermissions rolePermission
INNER JOIN dbo.Permissions permission ON permission.Id=rolePermission.PermissionId
WHERE permission.Code=N'SRI.DOCUMENTS.VIEW' AND permission.IsActive=1
UNION
SELECT DISTINCT rolePermission.RoleId,@DownloadOperationId
FROM dbo.RolePermissions rolePermission
INNER JOIN dbo.Permissions permission ON permission.Id=rolePermission.PermissionId
WHERE permission.Code=N'SRI.DOCUMENTS.DOWNLOAD_XML' AND permission.IsActive=1;

INSERT dbo.SecurityRoleFormOperations
(
    RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt
)
SELECT source.RoleId,@FormId,source.OperationId,1,N'Sistema',SYSUTCDATETIME()
FROM @RoleOperations source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SecurityRoleFormOperations existing
    WHERE existing.RoleId=source.RoleId
      AND existing.FormId=@FormId
      AND existing.OperationId=source.OperationId
);

IF NOT EXISTS
(
    SELECT 1 FROM dbo.MasterSchemaHistory
    WHERE Version=N'20260728.149'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version,Description)
    VALUES(N'20260728.149',N'Acciones Ribbon y filtros modales del Monitor SRI');
END;

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
