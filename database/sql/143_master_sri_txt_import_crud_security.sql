/*
    Migracion 143 - SRI TXT Import, formulario, menu y operaciones Master.

    No concede permisos, menus ni operaciones a ningun rol existente.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51143, 'MasterSchemaHistory is required before migration 143.', 1;
IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL
    THROW 51143, 'SecurityForms is required before migration 143.', 1;
IF OBJECT_ID(N'dbo.SecurityMenus', N'U') IS NULL
    THROW 51143, 'SecurityMenus is required before migration 143.', 1;
IF OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL
    THROW 51143, 'SecurityOperations is required before migration 143.', 1;
GO

BEGIN TRY
BEGIN TRANSACTION;

DECLARE @ParentId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityMenus
    WHERE Code = N'MENU.ADMINISTRATION'
      AND IsDeleted = 0
);

IF @ParentId IS NULL
    THROW 51143, 'MENU.ADMINISTRATION is required before migration 143.', 1;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.REFRESH'
      AND IsActive = 1
      AND IsDeleted = 0
)
    THROW 51143, 'ACTION.REFRESH is required before migration 143.', 1;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.CONSULT'
      AND IsActive = 1
      AND IsDeleted = 0
)
    THROW 51143, 'ACTION.CONSULT is required before migration 143.', 1;

DECLARE @FormId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityForms
    WHERE FormKey = N'sri-txt-imports'
       OR Code = N'FORM.ADMINISTRATION.SRI.TXT_IMPORTS'
    ORDER BY CASE WHEN FormKey = N'sri-txt-imports' THEN 0 ELSE 1 END
);

IF @FormId IS NULL
BEGIN
    INSERT dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType,
        HasListView, HasEditView, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.ADMINISTRATION.SRI.TXT_IMPORTS',
        N'Importaciones TXT SRI',
        N'Consulta paginada y encolado explicito de importaciones TXT SRI.',
        N'sri-txt-imports',
        3, 1, 0, 1, 1,
        N'Sistema', SYSUTCDATETIME()
    );
    SET @FormId = CONVERT(int, SCOPE_IDENTITY());
END
ELSE
BEGIN
    UPDATE dbo.SecurityForms
    SET Code = N'FORM.ADMINISTRATION.SRI.TXT_IMPORTS',
        Name = N'Importaciones TXT SRI',
        Description = N'Consulta paginada y encolado explicito de importaciones TXT SRI.',
        FormKey = N'sri-txt-imports',
        FormType = 3,
        HasListView = 1,
        HasEditView = 0,
        IsVisible = 1,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @FormId;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SecurityMenus
    WHERE Code = N'MENU.ADMINISTRATION.SRI.TXT_IMPORTS'
      AND IsDeleted = 0
)
BEGIN
    INSERT dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType,
        FormId, FormKey, IconLarge, IconSmall, DisplayOrder,
        IsVisible, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @ParentId,
        N'MENU.ADMINISTRATION.SRI.TXT_IMPORTS',
        N'Importaciones TXT SRI',
        N'Consulta y encolado de archivos TXT SRI.',
        3,
        @FormId,
        N'sri-txt-imports',
        N'Accordion/document_32.svg',
        N'Accordion/document_16.svg',
        41,
        1, 1, N'Sistema', SYSUTCDATETIME()
    );
END
ELSE
BEGIN
    UPDATE dbo.SecurityMenus
    SET ParentId = @ParentId,
        Name = N'Importaciones TXT SRI',
        Description = N'Consulta y encolado de archivos TXT SRI.',
        MenuType = 3,
        FormId = @FormId,
        FormKey = N'sri-txt-imports',
        DisplayOrder = 41,
        IsVisible = 1,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Code = N'MENU.ADMINISTRATION.SRI.TXT_IMPORTS';
END;

DECLARE @Operations table
(
    Code nvarchar(80) NOT NULL PRIMARY KEY,
    Name nvarchar(120) NOT NULL,
    Description nvarchar(300) NOT NULL,
    ActionKey nvarchar(120) NOT NULL,
    DisplayOrder int NOT NULL
);

INSERT @Operations(Code, Name, Description, ActionKey, DisplayOrder)
VALUES
    (N'ACTION.SRI_TXT_IMPORTS.ENQUEUE', N'Encolar TXT SRI', N'Cambiar filas preparadas a pendientes.', N'enqueue', 30),
    (N'ACTION.SRI_TXT_IMPORTS.OPEN_QUEUE', N'Abrir cola SRI', N'Abrir la cola SRI vinculada.', N'open-queue', 40);

IF EXISTS
(
    SELECT 1
    FROM @Operations source
    INNER JOIN dbo.SecurityOperations target ON target.Name = source.Name
    WHERE target.Code <> source.Code
)
    THROW 51143, 'A required SRI TXT operation name belongs to another operation.', 1;

MERGE dbo.SecurityOperations AS target
USING @Operations AS source
ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        Name = source.Name,
        Description = source.Description,
        ActionKey = source.ActionKey,
        DisplayOrder = source.DisplayOrder,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        Code, Name, Description, ActionKey, DisplayOrder,
        IsActive, CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.Code, source.Name, source.Description, source.ActionKey, source.DisplayOrder,
        1, N'Sistema', SYSUTCDATETIME(), 0
    );

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260727.143'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260727.143',
        N'Formulario y navegacion SRI TXT Import sin concesiones automaticas'
    );
END;

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
