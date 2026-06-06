/*
    Ejecutar este script en NuanSystem_Master.
    Agrega indicadores visuales independientes de FormType:
    HasListView y HasEditView.
*/

IF COL_LENGTH(N'dbo.SecurityForms', N'HasListView') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityForms
    ADD HasListView bit NOT NULL CONSTRAINT DF_SecurityForms_HasListView DEFAULT 1;
END;
GO

IF COL_LENGTH(N'dbo.SecurityForms', N'HasEditView') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityForms
    ADD HasEditView bit NOT NULL CONSTRAINT DF_SecurityForms_HasEditView DEFAULT 1;
END;
GO

UPDATE dbo.SecurityForms
SET HasListView = 1,
    HasEditView = CASE WHEN FormType IN (1, 2) THEN 1 ELSE HasEditView END,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE IsDeleted = 0
  AND (HasListView = 0 OR (FormType IN (1, 2) AND HasEditView = 0));
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_FORMULARIOSEGURIDADLISTAR
AS
BEGIN
    SELECT
        Id, Code, Name, Description, FormKey, CAST(FormType AS int) AS FormType,
        CASE FormType
            WHEN 1 THEN N'Mantenimiento'
            WHEN 2 THEN N'Transaccional'
            WHEN 3 THEN N'Reporte'
            WHEN 4 THEN N'Dialogo'
            WHEN 5 THEN N'Proceso'
            ELSE N'Desconocido'
        END AS FormTypeName,
        HasListView,
        HasEditView,
        IsVisible, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt,
        DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.SecurityForms
    WHERE IsDeleted = 0
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_FORMULARIOSEGURIDADBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        Id, Code, Name, Description, FormKey, CAST(FormType AS int) AS FormType,
        CASE FormType
            WHEN 1 THEN N'Mantenimiento'
            WHEN 2 THEN N'Transaccional'
            WHEN 3 THEN N'Reporte'
            WHEN 4 THEN N'Dialogo'
            WHEN 5 THEN N'Proceso'
            ELSE N'Desconocido'
        END AS FormTypeName,
        HasListView,
        HasEditView,
        IsVisible, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt,
        DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.SecurityForms
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_FORMULARIOSEGURIDADCREAR
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @FormKey nvarchar(120),
    @FormType int,
    @HasListView bit = 1,
    @HasEditView bit = 1,
    @IsVisible bit = 1,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, HasListView, HasEditView, IsVisible, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @Code, @Name, @Description, @FormKey, @FormType, @HasListView, @HasEditView, @IsVisible, @IsActive,
        @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME()
    );

    DECLARE @Id int = CAST(SCOPE_IDENTITY() AS int);

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityForms', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
    FROM
    (
        VALUES
            (N'Code', CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @Description)),
            (N'FormKey', CONVERT(nvarchar(max), @FormKey)),
            (N'FormType', CONVERT(nvarchar(max), @FormType)),
            (N'HasListView', CONVERT(nvarchar(max), CONVERT(int, @HasListView))),
            (N'HasEditView', CONVERT(nvarchar(max), CONVERT(int, @HasEditView))),
            (N'IsVisible', CONVERT(nvarchar(max), CONVERT(int, @IsVisible))),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, NewValue)
    WHERE NewValue IS NOT NULL;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_FORMULARIOSEGURIDADACTUALIZAR
    @Id int,
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @FormKey nvarchar(120),
    @FormType int,
    @HasListView bit = 1,
    @HasEditView bit = 1,
    @IsVisible bit = 1,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    DECLARE
        @OldCode nvarchar(80),
        @OldName nvarchar(120),
        @OldDescription nvarchar(300),
        @OldFormKey nvarchar(120),
        @OldFormType int,
        @OldHasListView bit,
        @OldHasEditView bit,
        @OldIsVisible bit,
        @OldIsActive bit;

    SELECT
        @OldCode = Code,
        @OldName = Name,
        @OldDescription = Description,
        @OldFormKey = FormKey,
        @OldFormType = FormType,
        @OldHasListView = HasListView,
        @OldHasEditView = HasEditView,
        @OldIsVisible = IsVisible,
        @OldIsActive = IsActive
    FROM dbo.SecurityForms
    WHERE Id = @Id AND IsDeleted = 0;

    IF @OldCode IS NULL
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.SecurityForms
    SET
        Code = @Code,
        Name = @Name,
        Description = @Description,
        FormKey = @FormKey,
        FormType = @FormType,
        HasListView = @HasListView,
        HasEditView = @HasEditView,
        IsVisible = @IsVisible,
        IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityForms', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM
    (
        VALUES
            (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
            (N'FormKey', CONVERT(nvarchar(max), @OldFormKey), CONVERT(nvarchar(max), @FormKey)),
            (N'FormType', CONVERT(nvarchar(max), @OldFormType), CONVERT(nvarchar(max), @FormType)),
            (N'HasListView', CONVERT(nvarchar(max), CONVERT(int, @OldHasListView)), CONVERT(nvarchar(max), CONVERT(int, @HasListView))),
            (N'HasEditView', CONVERT(nvarchar(max), CONVERT(int, @OldHasEditView)), CONVERT(nvarchar(max), CONVERT(int, @HasEditView))),
            (N'IsVisible', CONVERT(nvarchar(max), CONVERT(int, @OldIsVisible)), CONVERT(nvarchar(max), CONVERT(int, @IsVisible))),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    SELECT @AffectedRows;
END;
GO
