IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityForms
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityForms PRIMARY KEY,
        Code nvarchar(80) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Description nvarchar(300) NULL,
        FormKey nvarchar(120) NOT NULL,
        FormType tinyint NOT NULL,
        IsVisible bit NOT NULL CONSTRAINT DF_SecurityForms_IsVisible DEFAULT 1,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityForms_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityForms_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityForms_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT UQ_SecurityForms_Code UNIQUE (Code),
        CONSTRAINT UQ_SecurityForms_FormKey UNIQUE (FormKey),
        CONSTRAINT CK_SecurityForms_FormType CHECK (FormType IN (1, 2, 3, 4, 5))
    );
END;
GO

IF OBJECT_ID(N'dbo.AuditSecurityChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSecurityChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSecurityChanges PRIMARY KEY,
        EntityName nvarchar(120) NOT NULL,
        RecordId nvarchar(80) NOT NULL,
        [Action] nvarchar(20) NOT NULL,
        FieldName nvarchar(120) NOT NULL,
        OldValue nvarchar(max) NULL,
        NewValue nvarchar(max) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        Source nvarchar(60) NOT NULL CONSTRAINT DF_AuditSecurityChanges_Source DEFAULT N'API',
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditSecurityChanges_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AuditSecurityChanges_Entity_Record_CreatedAt ON dbo.AuditSecurityChanges (EntityName, RecordId, CreatedAt DESC);
    CREATE INDEX IX_AuditSecurityChanges_User_CreatedAt ON dbo.AuditSecurityChanges (UserId, CreatedAt DESC);
    CREATE INDEX IX_AuditSecurityChanges_CreatedAt ON dbo.AuditSecurityChanges (CreatedAt DESC);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_FORMULARIOSEGURIDADLISTAR
AS
BEGIN
    SELECT
        Id, Code, Name, Description, FormKey, CAST(FormType AS int) AS FormType,
        CASE FormType
            WHEN 1 THEN N'Listado'
            WHEN 2 THEN N'Edicion'
            WHEN 3 THEN N'Reporte'
            WHEN 4 THEN N'Dialogo'
            WHEN 5 THEN N'Proceso'
            ELSE N'Desconocido'
        END AS FormTypeName,
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
            WHEN 1 THEN N'Listado'
            WHEN 2 THEN N'Edicion'
            WHEN 3 THEN N'Reporte'
            WHEN 4 THEN N'Dialogo'
            WHEN 5 THEN N'Proceso'
            ELSE N'Desconocido'
        END AS FormTypeName,
        IsVisible, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt,
        DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.SecurityForms
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_FORMULARIOSEGURIDADBUSCARPORCODIGO
    @Code nvarchar(80),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.SecurityForms
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_FORMULARIOSEGURIDADBUSCARPORCLAVE
    @FormKey nvarchar(120),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.SecurityForms
    WHERE FormKey = @FormKey
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_FORMULARIOSEGURIDADCREAR
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @FormKey nvarchar(120),
    @FormType int,
    @IsVisible bit = 1,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, IsVisible, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @Code, @Name, @Description, @FormKey, @FormType, @IsVisible, @IsActive,
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
        @OldIsVisible bit,
        @OldIsActive bit;

    SELECT
        @OldCode = Code,
        @OldName = Name,
        @OldDescription = Description,
        @OldFormKey = FormKey,
        @OldFormType = FormType,
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
            (N'IsVisible', CONVERT(nvarchar(max), CONVERT(int, @OldIsVisible)), CONVERT(nvarchar(max), CONVERT(int, @IsVisible))),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_FORMULARIOSEGURIDADELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    UPDATE dbo.SecurityForms
    SET
        IsDeleted = 1,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    IF @AffectedRows > 0
    BEGIN
        INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES (N'SecurityForms', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);
    END;

    SELECT @AffectedRows;
END;
GO
