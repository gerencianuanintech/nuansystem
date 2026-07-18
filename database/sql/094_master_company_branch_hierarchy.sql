-- 094: Expone la jerarquia Master/Sucursal en el mantenimiento administrativo de companias.
-- Idempotente: actualiza columnas, indices y procedimientos existentes.

IF COL_LENGTH('dbo.Companies', 'DisplayOrder') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD DisplayOrder int NOT NULL CONSTRAINT DF_Companies_DisplayOrder DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Companies', 'IsDefault') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD IsDefault bit NOT NULL CONSTRAINT DF_Companies_IsDefault DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Companies', 'Address') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD Address nvarchar(300) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'Phone') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD Phone nvarchar(30) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'Email') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD Email nvarchar(256) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'LogoImage') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD LogoImage varbinary(max) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'LogoImageContentType') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD LogoImageContentType nvarchar(80) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'LogoImageFileName') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD LogoImageFileName nvarchar(260) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'TimeZoneId') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD TimeZoneId nvarchar(80) NOT NULL CONSTRAINT DF_Companies_TimeZoneId DEFAULT N'America/Guayaquil';
END;
GO

IF COL_LENGTH('dbo.Companies', 'CultureCode') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD CultureCode nvarchar(20) NOT NULL CONSTRAINT DF_Companies_CultureCode DEFAULT N'es-EC';
END;
GO

IF COL_LENGTH('dbo.Companies', 'CurrencyCode') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD CurrencyCode nvarchar(3) NOT NULL CONSTRAINT DF_Companies_CurrencyCode DEFAULT N'USD';
END;
GO

IF COL_LENGTH('dbo.Companies', 'IsMaster') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD IsMaster bit NOT NULL CONSTRAINT DF_Companies_IsMaster DEFAULT 1;
END;
GO

IF COL_LENGTH('dbo.Companies', 'ParentCompanyId') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD ParentCompanyId int NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'BranchCode') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD BranchCode nvarchar(50) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'SyncEnabled') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD SyncEnabled bit NOT NULL CONSTRAINT DF_Companies_SyncEnabled DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Companies', 'CreatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD CreatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'CreatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD CreatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'UpdatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD UpdatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'UpdatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD UpdatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD IsDeleted bit NOT NULL CONSTRAINT DF_Companies_IsDeleted DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Companies', 'DeletedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD DeletedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'DeletedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD DeletedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD DeletedAt datetime2(0) NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Companies_IsDeleted_DisplayOrder' AND object_id = OBJECT_ID(N'dbo.Companies'))
BEGIN
    CREATE INDEX IX_Companies_IsDeleted_DisplayOrder ON dbo.Companies (IsDeleted, DisplayOrder, CommercialName);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Companies_Code_Active' AND object_id = OBJECT_ID(N'dbo.Companies'))
BEGIN
    CREATE UNIQUE INDEX UX_Companies_Code_Active ON dbo.Companies (Code) WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Companies_Default_Active' AND object_id = OBJECT_ID(N'dbo.Companies'))
BEGIN
    CREATE UNIQUE INDEX UX_Companies_Default_Active ON dbo.Companies (IsDefault) WHERE IsDefault = 1 AND IsDeleted = 0 AND IsActive = 1;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Companies_Parent_BranchCode_Active' AND object_id = OBJECT_ID(N'dbo.Companies'))
BEGIN
    CREATE UNIQUE INDEX UX_Companies_Parent_BranchCode_Active
        ON dbo.Companies (ParentCompanyId, BranchCode)
        WHERE ParentCompanyId IS NOT NULL AND BranchCode IS NOT NULL AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COMPANIACONFIGURACIONLISTAR
AS
BEGIN
    SELECT
        Id, Code, CommercialName, LegalName, TaxIdentification, Address, Phone, Email,
        LogoImage, LogoImageContentType, LogoImageFileName,
        DatabaseEngine, [Server], Port, DatabaseName, DatabaseUser, IsActive,
        SapIntegrationMode, DisplayOrder, IsDefault, TimeZoneId, CultureCode, CurrencyCode,
        IsMaster, ParentCompanyId, BranchCode, SyncEnabled,
        CreatedByUserId, CreatedByUserName, CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt,
        DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.Companies
    WHERE IsDeleted = 0
    ORDER BY DisplayOrder, CommercialName;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COMPANIACONFIGURACIONBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        Id, Code, CommercialName, LegalName, TaxIdentification, Address, Phone, Email,
        LogoImage, LogoImageContentType, LogoImageFileName,
        DatabaseEngine, [Server], Port, DatabaseName, DatabaseUser, IsActive,
        SapIntegrationMode, DisplayOrder, IsDefault, TimeZoneId, CultureCode, CurrencyCode,
        IsMaster, ParentCompanyId, BranchCode, SyncEnabled,
        CreatedByUserId, CreatedByUserName, CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt,
        DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.Companies
    WHERE Id = @Id
      AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COMPANIACONFIGURACIONBUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.Companies
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_COMPANIACONFIGURACIONCREAR
    @Code nvarchar(50),
    @CommercialName nvarchar(200),
    @LegalName nvarchar(250) = NULL,
    @TaxIdentification nvarchar(50) = NULL,
    @Address nvarchar(300) = NULL,
    @Phone nvarchar(30) = NULL,
    @Email nvarchar(256) = NULL,
    @LogoImage varbinary(max) = NULL,
    @LogoImageContentType nvarchar(80) = NULL,
    @LogoImageFileName nvarchar(260) = NULL,
    @DatabaseEngine int = 1,
    @Server nvarchar(200),
    @Port int = NULL,
    @DatabaseName nvarchar(128),
    @DatabaseUser nvarchar(128),
    @DatabasePasswordEncrypted nvarchar(max),
    @IsActive bit = 1,
    @SapIntegrationMode int = 0,
    @DisplayOrder int = 0,
    @IsDefault bit = 0,
    @TimeZoneId nvarchar(80) = N'America/Guayaquil',
    @CultureCode nvarchar(20) = N'es-EC',
    @CurrencyCode nvarchar(3) = N'USD',
    @IsMaster bit = 1,
    @ParentCompanyId int = NULL,
    @BranchCode nvarchar(50) = NULL,
    @SyncEnabled bit = 0,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    IF (@IsMaster = 1 AND (@ParentCompanyId IS NOT NULL OR NULLIF(LTRIM(RTRIM(@BranchCode)), N'') IS NOT NULL))
       OR (@IsMaster = 0 AND (@ParentCompanyId IS NULL OR NULLIF(LTRIM(RTRIM(@BranchCode)), N'') IS NULL))
    BEGIN
        THROW 51011, 'La jerarquia de la compania no es valida.', 1;
    END;

    IF @IsMaster = 0 AND NOT EXISTS
    (
        SELECT 1
        FROM dbo.Companies
        WHERE Id = @ParentCompanyId
          AND IsMaster = 1
          AND IsActive = 1
          AND IsDeleted = 0
    )
    BEGIN
        THROW 51011, 'La empresa padre debe existir, estar activa y ser maestra.', 1;
    END;

    IF @IsDefault = 1
    BEGIN
        UPDATE dbo.Companies SET IsDefault = 0 WHERE IsDefault = 1 AND IsDeleted = 0;
    END;

    INSERT INTO dbo.Companies
    (
        Code, CommercialName, LegalName, TaxIdentification, Address, Phone, Email,
        LogoImage, LogoImageContentType, LogoImageFileName,
        DatabaseEngine, [Server], Port, DatabaseName, DatabaseUser, DatabasePasswordEncrypted,
        IsActive, SapIntegrationMode, DisplayOrder, IsDefault, TimeZoneId, CultureCode, CurrencyCode,
        IsMaster, ParentCompanyId, BranchCode, SyncEnabled,
        CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @Code, @CommercialName, @LegalName, @TaxIdentification, @Address, @Phone, @Email,
        @LogoImage, @LogoImageContentType, @LogoImageFileName,
        @DatabaseEngine, @Server, @Port, @DatabaseName, @DatabaseUser, @DatabasePasswordEncrypted,
        @IsActive, @SapIntegrationMode, @DisplayOrder, @IsDefault, @TimeZoneId, @CultureCode, @CurrencyCode,
        @IsMaster, @ParentCompanyId, @BranchCode, @SyncEnabled,
        @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME()
    );

    DECLARE @Id int = CAST(SCOPE_IDENTITY() AS int);

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'ConfigurationCompanies', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
    FROM
    (
        VALUES
            (N'Code', CONVERT(nvarchar(max), @Code)),
            (N'CommercialName', CONVERT(nvarchar(max), @CommercialName)),
            (N'LegalName', CONVERT(nvarchar(max), @LegalName)),
            (N'TaxIdentification', CONVERT(nvarchar(max), @TaxIdentification)),
            (N'Address', CONVERT(nvarchar(max), @Address)),
            (N'Phone', CONVERT(nvarchar(max), @Phone)),
            (N'Email', CONVERT(nvarchar(max), @Email)),
            (N'LogoImageFileName', CONVERT(nvarchar(max), @LogoImageFileName)),
            (N'DatabaseEngine', CONVERT(nvarchar(max), @DatabaseEngine)),
            (N'Server', CONVERT(nvarchar(max), @Server)),
            (N'Port', CONVERT(nvarchar(max), @Port)),
            (N'DatabaseName', CONVERT(nvarchar(max), @DatabaseName)),
            (N'DatabaseUser', CONVERT(nvarchar(max), @DatabaseUser)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
            (N'SapIntegrationMode', CONVERT(nvarchar(max), @SapIntegrationMode)),
            (N'DisplayOrder', CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsDefault', CONVERT(nvarchar(max), CONVERT(int, @IsDefault))),
            (N'TimeZoneId', CONVERT(nvarchar(max), @TimeZoneId)),
            (N'CultureCode', CONVERT(nvarchar(max), @CultureCode)),
            (N'CurrencyCode', CONVERT(nvarchar(max), @CurrencyCode)),
            (N'IsMaster', CONVERT(nvarchar(max), CONVERT(int, @IsMaster))),
            (N'ParentCompanyId', CONVERT(nvarchar(max), @ParentCompanyId)),
            (N'BranchCode', CONVERT(nvarchar(max), @BranchCode)),
            (N'SyncEnabled', CONVERT(nvarchar(max), CONVERT(int, @SyncEnabled)))
    ) AS Changes(FieldName, NewValue)
    WHERE NewValue IS NOT NULL;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_COMPANIACONFIGURACIONACTUALIZAR
    @Id int,
    @Code nvarchar(50),
    @CommercialName nvarchar(200),
    @LegalName nvarchar(250) = NULL,
    @TaxIdentification nvarchar(50) = NULL,
    @Address nvarchar(300) = NULL,
    @Phone nvarchar(30) = NULL,
    @Email nvarchar(256) = NULL,
    @LogoImage varbinary(max) = NULL,
    @LogoImageContentType nvarchar(80) = NULL,
    @LogoImageFileName nvarchar(260) = NULL,
    @DatabaseEngine int = 1,
    @Server nvarchar(200),
    @Port int = NULL,
    @DatabaseName nvarchar(128),
    @DatabaseUser nvarchar(128),
    @DatabasePasswordEncrypted nvarchar(max) = NULL,
    @IsActive bit = 1,
    @SapIntegrationMode int = 0,
    @DisplayOrder int = 0,
    @IsDefault bit = 0,
    @TimeZoneId nvarchar(80) = N'America/Guayaquil',
    @CultureCode nvarchar(20) = N'es-EC',
    @CurrencyCode nvarchar(3) = N'USD',
    @IsMaster bit = 1,
    @ParentCompanyId int = NULL,
    @BranchCode nvarchar(50) = NULL,
    @SyncEnabled bit = 0,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    DECLARE
        @OldCode nvarchar(50),
        @OldCommercialName nvarchar(200),
        @OldLegalName nvarchar(250),
        @OldTaxIdentification nvarchar(50),
        @OldAddress nvarchar(300),
        @OldPhone nvarchar(30),
        @OldEmail nvarchar(256),
        @OldLogoImageFileName nvarchar(260),
        @OldDatabaseEngine int,
        @OldServer nvarchar(200),
        @OldPort int,
        @OldDatabaseName nvarchar(128),
        @OldDatabaseUser nvarchar(128),
        @OldIsActive bit,
        @OldSapIntegrationMode int,
        @OldDisplayOrder int,
        @OldIsDefault bit,
        @OldTimeZoneId nvarchar(80),
        @OldCultureCode nvarchar(20),
        @OldCurrencyCode nvarchar(3),
        @OldIsMaster bit,
        @OldParentCompanyId int,
        @OldBranchCode nvarchar(50),
        @OldSyncEnabled bit;

    SELECT
        @OldCode = Code,
        @OldCommercialName = CommercialName,
        @OldLegalName = LegalName,
        @OldTaxIdentification = TaxIdentification,
        @OldAddress = Address,
        @OldPhone = Phone,
        @OldEmail = Email,
        @OldLogoImageFileName = LogoImageFileName,
        @OldDatabaseEngine = DatabaseEngine,
        @OldServer = [Server],
        @OldPort = Port,
        @OldDatabaseName = DatabaseName,
        @OldDatabaseUser = DatabaseUser,
        @OldIsActive = IsActive,
        @OldSapIntegrationMode = SapIntegrationMode,
        @OldDisplayOrder = DisplayOrder,
        @OldIsDefault = IsDefault,
        @OldTimeZoneId = TimeZoneId,
        @OldCultureCode = CultureCode,
        @OldCurrencyCode = CurrencyCode,
        @OldIsMaster = IsMaster,
        @OldParentCompanyId = ParentCompanyId,
        @OldBranchCode = BranchCode,
        @OldSyncEnabled = SyncEnabled
    FROM dbo.Companies
    WHERE Id = @Id
      AND IsDeleted = 0;

    IF @OldCode IS NULL
    BEGIN
        SELECT 0;
        RETURN;
    END;

    IF @OldIsMaster <> @IsMaster
    BEGIN
        THROW 51011, 'El tipo maestra/sucursal no puede modificarse.', 1;
    END;

    IF (@IsMaster = 1 AND (@ParentCompanyId IS NOT NULL OR NULLIF(LTRIM(RTRIM(@BranchCode)), N'') IS NOT NULL))
       OR (@IsMaster = 0 AND (@ParentCompanyId IS NULL OR NULLIF(LTRIM(RTRIM(@BranchCode)), N'') IS NULL))
    BEGIN
        THROW 51011, 'La jerarquia de la compania no es valida.', 1;
    END;

    IF @IsMaster = 0 AND NOT EXISTS
    (
        SELECT 1
        FROM dbo.Companies
        WHERE Id = @ParentCompanyId
          AND IsMaster = 1
          AND IsActive = 1
          AND IsDeleted = 0
    )
    BEGIN
        THROW 51011, 'La empresa padre debe existir, estar activa y ser maestra.', 1;
    END;

    IF @IsDefault = 1
    BEGIN
        UPDATE dbo.Companies SET IsDefault = 0 WHERE Id <> @Id AND IsDefault = 1 AND IsDeleted = 0;
    END;

    UPDATE dbo.Companies
    SET
        Code = @Code,
        CommercialName = @CommercialName,
        LegalName = @LegalName,
        TaxIdentification = @TaxIdentification,
        Address = @Address,
        Phone = @Phone,
        Email = @Email,
        LogoImage = @LogoImage,
        LogoImageContentType = @LogoImageContentType,
        LogoImageFileName = @LogoImageFileName,
        DatabaseEngine = @DatabaseEngine,
        [Server] = @Server,
        Port = @Port,
        DatabaseName = @DatabaseName,
        DatabaseUser = @DatabaseUser,
        DatabasePasswordEncrypted = COALESCE(@DatabasePasswordEncrypted, DatabasePasswordEncrypted),
        IsActive = @IsActive,
        SapIntegrationMode = @SapIntegrationMode,
        DisplayOrder = @DisplayOrder,
        IsDefault = @IsDefault,
        TimeZoneId = @TimeZoneId,
        CultureCode = @CultureCode,
        CurrencyCode = @CurrencyCode,
        IsMaster = @IsMaster,
        ParentCompanyId = @ParentCompanyId,
        BranchCode = @BranchCode,
        SyncEnabled = @SyncEnabled,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'ConfigurationCompanies', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM
    (
        VALUES
            (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
            (N'CommercialName', CONVERT(nvarchar(max), @OldCommercialName), CONVERT(nvarchar(max), @CommercialName)),
            (N'LegalName', CONVERT(nvarchar(max), @OldLegalName), CONVERT(nvarchar(max), @LegalName)),
            (N'TaxIdentification', CONVERT(nvarchar(max), @OldTaxIdentification), CONVERT(nvarchar(max), @TaxIdentification)),
            (N'Address', CONVERT(nvarchar(max), @OldAddress), CONVERT(nvarchar(max), @Address)),
            (N'Phone', CONVERT(nvarchar(max), @OldPhone), CONVERT(nvarchar(max), @Phone)),
            (N'Email', CONVERT(nvarchar(max), @OldEmail), CONVERT(nvarchar(max), @Email)),
            (N'LogoImageFileName', CONVERT(nvarchar(max), @OldLogoImageFileName), CONVERT(nvarchar(max), @LogoImageFileName)),
            (N'DatabaseEngine', CONVERT(nvarchar(max), @OldDatabaseEngine), CONVERT(nvarchar(max), @DatabaseEngine)),
            (N'Server', CONVERT(nvarchar(max), @OldServer), CONVERT(nvarchar(max), @Server)),
            (N'Port', CONVERT(nvarchar(max), @OldPort), CONVERT(nvarchar(max), @Port)),
            (N'DatabaseName', CONVERT(nvarchar(max), @OldDatabaseName), CONVERT(nvarchar(max), @DatabaseName)),
            (N'DatabaseUser', CONVERT(nvarchar(max), @OldDatabaseUser), CONVERT(nvarchar(max), @DatabaseUser)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
            (N'SapIntegrationMode', CONVERT(nvarchar(max), @OldSapIntegrationMode), CONVERT(nvarchar(max), @SapIntegrationMode)),
            (N'DisplayOrder', CONVERT(nvarchar(max), @OldDisplayOrder), CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsDefault', CONVERT(nvarchar(max), CONVERT(int, @OldIsDefault)), CONVERT(nvarchar(max), CONVERT(int, @IsDefault))),
            (N'TimeZoneId', CONVERT(nvarchar(max), @OldTimeZoneId), CONVERT(nvarchar(max), @TimeZoneId)),
            (N'CultureCode', CONVERT(nvarchar(max), @OldCultureCode), CONVERT(nvarchar(max), @CultureCode)),
            (N'CurrencyCode', CONVERT(nvarchar(max), @OldCurrencyCode), CONVERT(nvarchar(max), @CurrencyCode)),
            (N'IsMaster', CONVERT(nvarchar(max), CONVERT(int, @OldIsMaster)), CONVERT(nvarchar(max), CONVERT(int, @IsMaster))),
            (N'ParentCompanyId', CONVERT(nvarchar(max), @OldParentCompanyId), CONVERT(nvarchar(max), @ParentCompanyId)),
            (N'BranchCode', CONVERT(nvarchar(max), @OldBranchCode), CONVERT(nvarchar(max), @BranchCode)),
            (N'SyncEnabled', CONVERT(nvarchar(max), CONVERT(int, @OldSyncEnabled)), CONVERT(nvarchar(max), CONVERT(int, @SyncEnabled)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    IF @DatabasePasswordEncrypted IS NOT NULL
    BEGIN
        INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES (N'ConfigurationCompanies', CONVERT(nvarchar(80), @Id), N'UPDATE', N'DatabasePassword', N'********', N'********', @UpdatedByUserId, @UpdatedByUserName);
    END;

    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_COMPANIACONFIGURACIONELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.UserCompanies WHERE CompanyId = @Id AND IsActive = 1)
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.Companies
    SET
        IsDeleted = 1,
        IsActive = 0,
        IsDefault = 0,
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
        VALUES (N'ConfigurationCompanies', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);
    END;

    SELECT @AffectedRows;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.CONFIGURATION.COMPANIES')
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.CONFIGURATION.COMPANIES', N'Companias', N'Mantenimiento de companias',
        N'configuration-companies', 1, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityForms
SET Name = N'Companias',
    Description = N'Mantenimiento de companias',
    FormKey = N'configuration-companies',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.CONFIGURATION.COMPANIES';
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION')
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.CONFIGURATION', N'Configuracion', N'Modulo de configuracion',
        1, NULL, NULL, N'Accordion/configuracion_32.svg', N'Accordion/configuracion_16.svg',
        20, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

DECLARE @ConfigurationMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION');
DECLARE @CompaniesFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.CONFIGURATION.COMPANIES');

IF @ConfigurationMenuId IS NOT NULL AND @CompaniesFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.COMPANIES')
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @ConfigurationMenuId, N'MENU.CONFIGURATION.COMPANIES', N'Companias',
            N'Administrar companias',
            3, @CompaniesFormId, N'configuration-companies',
            N'Accordion/companies_32.svg', N'Accordion/companies_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET FormId = @CompaniesFormId,
        FormKey = N'configuration-companies',
        IconLarge = N'Accordion/companies_32.svg',
        IconSmall = N'Accordion/companies_16.svg',
        DisplayOrder = 10,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.CONFIGURATION.COMPANIES';
END;
GO

DECLARE @CompaniesModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'COMPANIES');

IF @CompaniesModuleId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'COMPANIES.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@CompaniesModuleId, N'COMPANIES.MANAGE', N'Gestionar companias', N'Crear, editar, eliminar y consultar companias');
END;
GO

DECLARE @AdminRoleIdForAccess int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');
DECLARE @CompaniesMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.COMPANIES');
DECLARE @CompaniesFormIdForAccess int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.CONFIGURATION.COMPANIES');
DECLARE @CompaniesPermissionId int = (SELECT TOP (1) Id FROM dbo.Permissions WHERE Code = N'COMPANIES.MANAGE');

IF @AdminRoleIdForAccess IS NOT NULL AND @CompaniesPermissionId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleIdForAccess AND PermissionId = @CompaniesPermissionId)
    BEGIN
        INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
        VALUES (@AdminRoleIdForAccess, @CompaniesPermissionId);
    END;
END;

IF @AdminRoleIdForAccess IS NOT NULL AND @CompaniesMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleIdForAccess AND MenuId = @CompaniesMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleIdForAccess, @CompaniesMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleIdForAccess IS NOT NULL AND @CompaniesFormIdForAccess IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleIdForAccess, @CompaniesFormIdForAccess, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.Code IN
    (
        N'ACTION.REFRESH',
        N'ACTION.CREATE',
        N'ACTION.NEW',
        N'ACTION.COPY',
        N'ACTION.UPDATE',
        N'ACTION.EDIT',
        N'ACTION.DELETE',
        N'ACTION.CONSULT',
        N'ACTION.HISTORY'
    )
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleIdForAccess
            AND existing.FormId = @CompaniesFormIdForAccess
            AND existing.OperationId = operation.Id
      );
END;
GO


IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260716.094')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260716.094', N'Jerarquia Master/Sucursal en configuracion administrativa de companias');
END;
GO
