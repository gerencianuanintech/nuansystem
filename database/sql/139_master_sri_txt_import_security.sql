/*
    SRI TXT Import - permisos API Master.

    No registra WinForms, formulario ni menu.
    No concede permisos automaticamente a ningun rol existente.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Modules', N'U') IS NULL
    THROW 51139, 'Modules is required before migration 139.', 1;
IF OBJECT_ID(N'dbo.Permissions', N'U') IS NULL
    THROW 51139, 'Permissions is required before migration 139.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51139, 'MasterSchemaHistory is required before migration 139.', 1;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SRI')
BEGIN
    INSERT dbo.Modules(Code, Name, DisplayOrder)
    VALUES(N'SRI', N'SRI', 70);
END
ELSE
BEGIN
    UPDATE dbo.Modules
    SET Name = N'SRI',
        DisplayOrder = 70,
        IsActive = 1,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Code = N'SRI';
END;
GO

DECLARE @ModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'SRI');
DECLARE @Permissions table
(
    Code nvarchar(120) PRIMARY KEY,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT @Permissions(Code, Name, Description)
VALUES
 (N'SRI.TXT_IMPORTS.VIEW', N'Consultar cargas TXT SRI', N'Consultar cargas TXT SRI y sus resumenes.'),
 (N'SRI.TXT_IMPORTS.UPLOAD', N'Cargar TXT SRI', N'Registrar y validar un archivo TXT SRI.'),
 (N'SRI.TXT_IMPORTS.VALIDATE', N'Validar cargas TXT SRI', N'Ejecutar validacion autorizada de una carga TXT SRI.'),
 (N'SRI.TXT_IMPORTS.ENQUEUE', N'Encolar cargas TXT SRI', N'Cambiar documentos preparados de Staged a Pending.'),
 (N'SRI.TXT_IMPORTS.REPROCESS', N'Reprocesar cargas TXT SRI', N'Reprocesar una carga TXT SRI bajo reglas autorizadas.'),
 (N'SRI.TXT_IMPORTS.DELETE', N'Eliminar cargas TXT SRI', N'Aplicar eliminacion logica a una carga elegible.'),
 (N'SRI.TXT_IMPORTS.VIEW_ERRORS', N'Ver errores TXT SRI', N'Consultar errores saneados de filas TXT SRI.'),
 (N'SRI.TXT_IMPORTS.VIEW_HISTORY', N'Ver historial TXT SRI', N'Consultar auditoria de cargas TXT SRI.'),
 (N'SRI.TXT_IMPORTS.VIEW_SAP_STATUS', N'Ver estado SAP de TXT SRI', N'Permiso reservado para la futura integracion SAP aprobada.');

UPDATE target
SET ModuleId = @ModuleId,
    Name = source.Name,
    Description = source.Description,
    IsActive = 1,
    UpdatedAt = SYSUTCDATETIME()
FROM dbo.Permissions target
INNER JOIN @Permissions source ON source.Code = target.Code;

INSERT dbo.Permissions(ModuleId, Code, Name, Description)
SELECT @ModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Permissions target
    WHERE target.Code = source.Code
);
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260727.139'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260727.139',
        N'Permisos API SRI TXT Import sin concesiones automaticas a roles'
    );
END;
GO
