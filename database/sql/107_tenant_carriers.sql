/* Maestro tenant independiente de Transportistas. No depende de BusinessPartners, SAP ni Sync. */
IF OBJECT_ID(N'dbo.Carriers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Carriers
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Carriers PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(150) NOT NULL,
        IdentificationTypeCode nvarchar(2) NOT NULL,
        IdentificationNumber nvarchar(30) NOT NULL,
        Description nvarchar(500) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Carriers_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Carriers_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_Carriers_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT CK_Carriers_IdentificationTypeCode CHECK (IdentificationTypeCode IN (N'04', N'05', N'06')),
        CONSTRAINT CK_Carriers_IdentificationNumber_NotBlank CHECK (LEN(LTRIM(RTRIM(IdentificationNumber))) > 0)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Carriers') AND name = N'UX_Carriers_Code_ActiveRecord')
    CREATE UNIQUE INDEX UX_Carriers_Code_ActiveRecord ON dbo.Carriers (Code) WHERE IsDeleted = 0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIERS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Code, Name, IdentificationTypeCode, IdentificationNumber, Description, IsActive,
           CreatedByUserId, CreatedByUserName, CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.Carriers
    WHERE IsDeleted = 0
    ORDER BY Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIERS_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Code, Name, IsActive
    FROM dbo.Carriers
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIERS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Code, Name, IdentificationTypeCode, IdentificationNumber, Description, IsActive,
           CreatedByUserId, CreatedByUserName, CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.Carriers
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIERSBUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.Carriers
    WHERE Code = @Code AND IsDeleted = 0 AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIERS_HISTORIAL
    @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName, CreatedAt
    FROM dbo.AuditCatalogChanges
    WHERE EntityName = N'Carrier' AND RecordId = CONVERT(nvarchar(80), @Id)
    ORDER BY CreatedAt DESC, Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CARRIERS_CREAR
    @Code nvarchar(50), @Name nvarchar(150), @IdentificationTypeCode nvarchar(2),
    @IdentificationNumber nvarchar(30), @Description nvarchar(500) = NULL, @IsActive bit,
    @AuditUserId int = NULL, @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @IdentificationTypeCode NOT IN (N'04', N'05', N'06') THROW 51000, 'Tipo de identificacion no permitido.', 1;
    IF LEN(LTRIM(RTRIM(@IdentificationNumber))) = 0 THROW 51001, 'La identificacion es obligatoria.', 1;

    BEGIN TRANSACTION;
    INSERT dbo.Carriers (Code, Name, IdentificationTypeCode, IdentificationNumber, Description, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt)
    VALUES (@Code, @Name, @IdentificationTypeCode, @IdentificationNumber, @Description, @IsActive, @AuditUserId, @AuditUserName, SYSUTCDATETIME());

    DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());
    INSERT dbo.AuditCatalogChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'Carrier', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @AuditUserId, @AuditUserName
    FROM (VALUES
        (N'Code', CONVERT(nvarchar(max), @Code)),
        (N'Name', CONVERT(nvarchar(max), @Name)),
        (N'IdentificationTypeCode', CONVERT(nvarchar(max), @IdentificationTypeCode)),
        (N'IdentificationNumber', CONVERT(nvarchar(max), @IdentificationNumber)),
        (N'Description', CONVERT(nvarchar(max), @Description)),
        (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))) changes(FieldName, NewValue);
    COMMIT TRANSACTION;
    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CARRIERS_ACTUALIZAR
    @Id int, @Code nvarchar(50), @Name nvarchar(150), @IdentificationTypeCode nvarchar(2),
    @IdentificationNumber nvarchar(30), @Description nvarchar(500) = NULL, @IsActive bit,
    @AuditUserId int = NULL, @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @IdentificationTypeCode NOT IN (N'04', N'05', N'06') THROW 51000, 'Tipo de identificacion no permitido.', 1;
    IF LEN(LTRIM(RTRIM(@IdentificationNumber))) = 0 THROW 51001, 'La identificacion es obligatoria.', 1;

    DECLARE @OldCode nvarchar(50), @OldName nvarchar(150), @OldType nvarchar(2), @OldNumber nvarchar(30), @OldDescription nvarchar(500), @OldIsActive bit;
    SELECT @OldCode = Code, @OldName = Name, @OldType = IdentificationTypeCode, @OldNumber = IdentificationNumber, @OldDescription = Description, @OldIsActive = IsActive
    FROM dbo.Carriers WHERE Id = @Id AND IsDeleted = 0;
    IF @OldCode IS NULL BEGIN SELECT 0; RETURN; END;

    BEGIN TRANSACTION;
    UPDATE dbo.Carriers
    SET Code = @Code, Name = @Name, IdentificationTypeCode = @IdentificationTypeCode,
        IdentificationNumber = @IdentificationNumber, Description = @Description, IsActive = @IsActive,
        UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    INSERT dbo.AuditCatalogChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'Carrier', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @AuditUserId, @AuditUserName
    FROM (VALUES
        (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
        (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
        (N'IdentificationTypeCode', CONVERT(nvarchar(max), @OldType), CONVERT(nvarchar(max), @IdentificationTypeCode)),
        (N'IdentificationNumber', CONVERT(nvarchar(max), @OldNumber), CONVERT(nvarchar(max), @IdentificationNumber)),
        (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
        (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))) changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');
    COMMIT TRANSACTION;
    SELECT 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_CARRIERS_ELIMINAR
    @Id int, @AuditUserId int = NULL, @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.Carriers WHERE Id = @Id AND IsDeleted = 0) BEGIN SELECT 0; RETURN; END;

    BEGIN TRANSACTION;
    UPDATE dbo.Carriers
    SET IsActive = 0, IsDeleted = 1, DeletedByUserId = @AuditUserId,
        DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    INSERT dbo.AuditCatalogChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    VALUES
        (N'Carrier', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsActive', N'1', N'0', @AuditUserId, @AuditUserName),
        (N'Carrier', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @AuditUserId, @AuditUserName);
    COMMIT TRANSACTION;
    SELECT 1;
END;
GO
