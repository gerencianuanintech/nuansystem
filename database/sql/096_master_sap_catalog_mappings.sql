SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SapCatalogMappings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapCatalogMappings
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapCatalogMappings PRIMARY KEY,
        CompanyId int NOT NULL,
        MappingType nvarchar(40) NOT NULL,
        SapCode nvarchar(120) NOT NULL,
        NuanCode nvarchar(120) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_SapCatalogMappings_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_SapCatalogMappings_IsDeleted DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapCatalogMappings_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SapCatalogMappings_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_SapCatalogMappings_Type CHECK (MappingType IN (N'ItemGroup', N'UnitOfMeasure', N'Tax'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.SapCatalogMappings') AND name = N'UX_SapCatalogMappings_Company_Type_SapCode')
    CREATE UNIQUE INDEX UX_SapCatalogMappings_Company_Type_SapCode
        ON dbo.SapCatalogMappings (CompanyId, MappingType, SapCode)
        WHERE IsDeleted = 0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPCATALOGMAPPINGS_LISTARPOREMPRESA
    @CompanyId int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, MappingType, SapCode, NuanCode, IsActive, UpdatedAt
    FROM dbo.SapCatalogMappings
    WHERE CompanyId = @CompanyId AND IsDeleted = 0
    ORDER BY MappingType, SapCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SAPCATALOGMAPPINGS_REEMPLAZAR
    @CompanyId int,
    @MappingsJson nvarchar(max),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF ISJSON(@MappingsJson) <> 1 THROW 51000, 'MappingsJson no contiene JSON valido.', 1;

    DECLARE @Rows TABLE
    (
        MappingType nvarchar(40) NOT NULL,
        SapCode nvarchar(120) NOT NULL,
        NuanCode nvarchar(120) NOT NULL,
        IsActive bit NOT NULL,
        PRIMARY KEY (MappingType, SapCode)
    );

    INSERT INTO @Rows (MappingType, SapCode, NuanCode, IsActive)
    SELECT LTRIM(RTRIM(MappingType)), LTRIM(RTRIM(SapCode)), LTRIM(RTRIM(NuanCode)), IsActive
    FROM OPENJSON(@MappingsJson)
    WITH
    (
        MappingType nvarchar(40) '$.mappingType',
        SapCode nvarchar(120) '$.sapCode',
        NuanCode nvarchar(120) '$.nuanCode',
        IsActive bit '$.isActive'
    );

    BEGIN TRANSACTION;

    UPDATE target
    SET IsDeleted = 1, IsActive = 0, DeletedByUserId = @AuditUserId,
        DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
    FROM dbo.SapCatalogMappings target
    WHERE target.CompanyId = @CompanyId AND target.IsDeleted = 0
      AND NOT EXISTS (SELECT 1 FROM @Rows source WHERE source.MappingType = target.MappingType AND source.SapCode = target.SapCode);

    UPDATE target
    SET NuanCode = source.NuanCode, IsActive = source.IsActive,
        UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME()
    FROM dbo.SapCatalogMappings target
    INNER JOIN @Rows source ON source.MappingType = target.MappingType AND source.SapCode = target.SapCode
    WHERE target.CompanyId = @CompanyId AND target.IsDeleted = 0;

    INSERT INTO dbo.SapCatalogMappings
        (CompanyId, MappingType, SapCode, NuanCode, IsActive, CreatedByUserId, CreatedByUserName)
    SELECT @CompanyId, source.MappingType, source.SapCode, source.NuanCode, source.IsActive, @AuditUserId, @AuditUserName
    FROM @Rows source
    WHERE NOT EXISTS
    (
        SELECT 1 FROM dbo.SapCatalogMappings target
        WHERE target.CompanyId = @CompanyId AND target.MappingType = source.MappingType
          AND target.SapCode = source.SapCode AND target.IsDeleted = 0
    );

    COMMIT TRANSACTION;
END;
GO
