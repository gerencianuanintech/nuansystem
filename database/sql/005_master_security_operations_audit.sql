IF COL_LENGTH('dbo.SecurityOperations', 'CreatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD CreatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.SecurityOperations', 'CreatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD CreatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.SecurityOperations', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityOperations_CreatedAt DEFAULT SYSUTCDATETIME();
END;
GO

IF COL_LENGTH('dbo.SecurityOperations', 'UpdatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD UpdatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.SecurityOperations', 'UpdatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD UpdatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.SecurityOperations', 'UpdatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD UpdatedAt datetime2(0) NULL;
END;
GO

IF COL_LENGTH('dbo.SecurityOperations', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD IsDeleted bit NOT NULL CONSTRAINT DF_SecurityOperations_IsDeleted DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.SecurityOperations', 'DeletedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD DeletedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.SecurityOperations', 'DeletedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD DeletedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.SecurityOperations', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD DeletedAt datetime2(0) NULL;
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERACIONSEGURIDADLISTAR
AS
BEGIN
    SELECT
        Id,
        Code,
        Name,
        Description,
        RibbonPageName,
        RibbonGroupName,
        ActionKey,
        IconLarge,
        IconSmall,
        DisplayOrder,
        IsActive,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        UpdatedByUserId,
        UpdatedByUserName,
        UpdatedAt,
        DeletedByUserId,
        DeletedByUserName,
        DeletedAt
    FROM dbo.SecurityOperations
    WHERE IsDeleted = 0
    ORDER BY RibbonPageName, RibbonGroupName, DisplayOrder, Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERACIONSEGURIDADBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        Id,
        Code,
        Name,
        Description,
        RibbonPageName,
        RibbonGroupName,
        ActionKey,
        IconLarge,
        IconSmall,
        DisplayOrder,
        IsActive,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        UpdatedByUserId,
        UpdatedByUserName,
        UpdatedAt,
        DeletedByUserId,
        DeletedByUserName,
        DeletedAt
    FROM dbo.SecurityOperations
    WHERE Id = @Id
      AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_OPERACIONSEGURIDADCREAR
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @RibbonPageName nvarchar(80) = NULL,
    @RibbonGroupName nvarchar(80) = NULL,
    @ActionKey nvarchar(120) = NULL,
    @IconLarge nvarchar(200) = NULL,
    @IconSmall nvarchar(200) = NULL,
    @DisplayOrder int = 0,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code,
        Name,
        Description,
        RibbonPageName,
        RibbonGroupName,
        ActionKey,
        IconLarge,
        IconSmall,
        DisplayOrder,
        IsActive,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt
    )
    VALUES
    (
        @Code,
        @Name,
        @Description,
        @RibbonPageName,
        @RibbonGroupName,
        @ActionKey,
        @IconLarge,
        @IconSmall,
        @DisplayOrder,
        @IsActive,
        @CreatedByUserId,
        @CreatedByUserName,
        SYSUTCDATETIME()
    );

    DECLARE @Id int = CAST(SCOPE_IDENTITY() AS int);

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityOperations', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
    FROM
    (
        VALUES
            (N'Code', CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @Description)),
            (N'RibbonPageName', CONVERT(nvarchar(max), @RibbonPageName)),
            (N'RibbonGroupName', CONVERT(nvarchar(max), @RibbonGroupName)),
            (N'ActionKey', CONVERT(nvarchar(max), @ActionKey)),
            (N'IconLarge', CONVERT(nvarchar(max), @IconLarge)),
            (N'IconSmall', CONVERT(nvarchar(max), @IconSmall)),
            (N'DisplayOrder', CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, NewValue)
    WHERE NewValue IS NOT NULL;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERACIONSEGURIDADBUSCARPORCODIGO
    @Code nvarchar(80),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.SecurityOperations
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERACIONSEGURIDADBUSCARPORNOMBRE
    @Name nvarchar(120),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.SecurityOperations
    WHERE Name = @Name
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_OPERACIONSEGURIDADACTUALIZAR
    @Id int,
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @RibbonPageName nvarchar(80) = NULL,
    @RibbonGroupName nvarchar(80) = NULL,
    @ActionKey nvarchar(120) = NULL,
    @IconLarge nvarchar(200) = NULL,
    @IconSmall nvarchar(200) = NULL,
    @DisplayOrder int = 0,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    DECLARE
        @OldCode nvarchar(80),
        @OldName nvarchar(120),
        @OldDescription nvarchar(300),
        @OldRibbonPageName nvarchar(80),
        @OldRibbonGroupName nvarchar(80),
        @OldActionKey nvarchar(120),
        @OldIconLarge nvarchar(200),
        @OldIconSmall nvarchar(200),
        @OldDisplayOrder int,
        @OldIsActive bit;

    SELECT
        @OldCode = Code,
        @OldName = Name,
        @OldDescription = Description,
        @OldRibbonPageName = RibbonPageName,
        @OldRibbonGroupName = RibbonGroupName,
        @OldActionKey = ActionKey,
        @OldIconLarge = IconLarge,
        @OldIconSmall = IconSmall,
        @OldDisplayOrder = DisplayOrder,
        @OldIsActive = IsActive
    FROM dbo.SecurityOperations
    WHERE Id = @Id
      AND IsDeleted = 0;

    IF @OldCode IS NULL
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.SecurityOperations
    SET
        Code = @Code,
        Name = @Name,
        Description = @Description,
        RibbonPageName = @RibbonPageName,
        RibbonGroupName = @RibbonGroupName,
        ActionKey = @ActionKey,
        IconLarge = @IconLarge,
        IconSmall = @IconSmall,
        DisplayOrder = @DisplayOrder,
        IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityOperations', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM
    (
        VALUES
            (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
            (N'RibbonPageName', CONVERT(nvarchar(max), @OldRibbonPageName), CONVERT(nvarchar(max), @RibbonPageName)),
            (N'RibbonGroupName', CONVERT(nvarchar(max), @OldRibbonGroupName), CONVERT(nvarchar(max), @RibbonGroupName)),
            (N'ActionKey', CONVERT(nvarchar(max), @OldActionKey), CONVERT(nvarchar(max), @ActionKey)),
            (N'IconLarge', CONVERT(nvarchar(max), @OldIconLarge), CONVERT(nvarchar(max), @IconLarge)),
            (N'IconSmall', CONVERT(nvarchar(max), @OldIconSmall), CONVERT(nvarchar(max), @IconSmall)),
            (N'DisplayOrder', CONVERT(nvarchar(max), @OldDisplayOrder), CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_OPERACIONSEGURIDADELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    UPDATE dbo.SecurityOperations
    SET
        IsDeleted = 1,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    IF @AffectedRows > 0
    BEGIN
        INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES (N'SecurityOperations', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);
    END;

    SELECT @AffectedRows;
END;
GO
