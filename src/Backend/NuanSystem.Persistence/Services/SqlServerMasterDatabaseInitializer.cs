using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Persistence.Options;

namespace NuanSystem.Persistence.Services;

public sealed class SqlServerMasterDatabaseInitializer(
    IConfiguration configuration,
    IOptions<MasterDatabaseOptions> options) : IMasterDatabaseInitializer
{
    private static readonly Regex ValidDatabaseName = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var serverConnectionString = configuration.GetConnectionString("SqlServerAdmin");
        if (string.IsNullOrWhiteSpace(serverConnectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:SqlServerAdmin no esta configurado.");
        }

        var databaseName = options.Value.DatabaseName;
        if (!ValidDatabaseName.IsMatch(databaseName))
        {
            throw new InvalidOperationException($"El nombre de base de datos master '{databaseName}' no es valido.");
        }

        await using var serverConnection = new SqlConnection(serverConnectionString);
        await serverConnection.OpenAsync(cancellationToken);

        await CreateDatabaseIfMissingAsync(serverConnection, databaseName, cancellationToken);

        var masterConnectionString = BuildDatabaseConnectionString(serverConnectionString, databaseName);
        await using var masterConnection = new SqlConnection(masterConnectionString);
        await masterConnection.OpenAsync(cancellationToken);

        await CreateSchemaObjectsAsync(masterConnection, cancellationToken);
        await CreateStoredProceduresAsync(masterConnection, cancellationToken);
    }

    private static async Task CreateDatabaseIfMissingAsync(
        SqlConnection connection,
        string databaseName,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
IF DB_ID(@databaseName) IS NULL
BEGIN
    DECLARE @sql nvarchar(max) = N'CREATE DATABASE {QuoteIdentifier(databaseName)}';
    EXEC sys.sp_executesql @sql;
END
""";
        command.Parameters.Add("@databaseName", SqlDbType.NVarChar, 128).Value = databaseName;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task CreateSchemaObjectsAsync(
        SqlConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
IF OBJECT_ID(N'dbo.SystemParameters', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SystemParameters
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SystemParameters PRIMARY KEY,
        [Key] nvarchar(120) NOT NULL CONSTRAINT UQ_SystemParameters_Key UNIQUE,
        [Value] nvarchar(max) NULL,
        Description nvarchar(300) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SystemParameters_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.SystemParameters WHERE [Key] = N'System.Name')
BEGIN
    INSERT INTO dbo.SystemParameters ([Key], [Value], Description)
    VALUES (N'System.Name', N'NuanSystem', N'Nombre logico del sistema');
END;

IF OBJECT_ID(N'dbo.Companies', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Companies
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Companies PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        CommercialName nvarchar(200) NOT NULL,
        LegalName nvarchar(250) NULL,
        TaxIdentification nvarchar(50) NULL,
        DatabaseEngine int NOT NULL CONSTRAINT DF_Companies_DatabaseEngine DEFAULT 1,
        [Server] nvarchar(200) NOT NULL,
        Port int NULL,
        DatabaseName nvarchar(128) NOT NULL,
        DatabaseUser nvarchar(128) NOT NULL,
        DatabasePasswordEncrypted nvarchar(max) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Companies_IsActive DEFAULT 1,
        SapIntegrationMode int NOT NULL CONSTRAINT DF_Companies_SapIntegrationMode DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Companies_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT UQ_Companies_Code UNIQUE (Code),
        CONSTRAINT CK_Companies_DatabaseEngine CHECK (DatabaseEngine IN (1, 2)),
        CONSTRAINT CK_Companies_SapIntegrationMode CHECK (SapIntegrationMode IN (0, 1, 2))
    );
END;

IF OBJECT_ID(N'dbo.SapCompanySettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapCompanySettings
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapCompanySettings PRIMARY KEY,
        CompanyId int NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_SapCompanySettings_IsEnabled DEFAULT 0,
        IntegrationMode int NOT NULL CONSTRAINT DF_SapCompanySettings_IntegrationMode DEFAULT 0,
        ServiceLayerUrl nvarchar(500) NULL,
        SapCompanyDb nvarchar(128) NULL,
        SapUser nvarchar(128) NULL,
        SapPasswordEncrypted nvarchar(max) NULL,
        DiApiServer nvarchar(200) NULL,
        LicenseServer nvarchar(200) NULL,
        Language nvarchar(20) NULL,
        HanaServer nvarchar(200) NULL,
        HanaPort int NULL,
        HanaSchema nvarchar(128) NULL,
        HanaUser nvarchar(128) NULL,
        HanaPasswordEncrypted nvarchar(max) NULL,
        MaxRetryCount int NOT NULL CONSTRAINT DF_SapCompanySettings_MaxRetryCount DEFAULT 3,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapCompanySettings_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_SapCompanySettings_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT UQ_SapCompanySettings_CompanyId UNIQUE (CompanyId),
        CONSTRAINT CK_SapCompanySettings_IntegrationMode CHECK (IntegrationMode IN (0, 1, 2))
    );
END;

IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaServer') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaServer nvarchar(200) NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaPort') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaPort int NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaSchema') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaSchema nvarchar(128) NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaUser') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaUser nvarchar(128) NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaPasswordEncrypted') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaPasswordEncrypted nvarchar(max) NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'MaxRetryCount') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD MaxRetryCount int NOT NULL CONSTRAINT DF_SapCompanySettings_MaxRetryCount DEFAULT 3;

IF OBJECT_ID(N'dbo.CompanyParameters', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CompanyParameters
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CompanyParameters PRIMARY KEY,
        CompanyId int NOT NULL,
        [Key] nvarchar(120) NOT NULL,
        [Value] nvarchar(max) NULL,
        Description nvarchar(300) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_CompanyParameters_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_CompanyParameters_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT UQ_CompanyParameters_Company_Key UNIQUE (CompanyId, [Key])
    );
END;

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        UserName nvarchar(120) NOT NULL,
        NormalizedUserName nvarchar(120) NOT NULL,
        Email nvarchar(256) NULL,
        NormalizedEmail nvarchar(256) NULL,
        DisplayName nvarchar(200) NOT NULL,
        PasswordHash nvarchar(max) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
        IsLocked bit NOT NULL CONSTRAINT DF_Users_IsLocked DEFAULT 0,
        MustChangePassword bit NOT NULL CONSTRAINT DF_Users_MustChangePassword DEFAULT 0,
        LockoutEndAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_Users_IsDeleted DEFAULT 0,
        FailedAccessCount int NOT NULL CONSTRAINT DF_Users_FailedAccessCount DEFAULT 0,
        LastLoginAt datetime2(0) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT UQ_Users_NormalizedUserName UNIQUE (NormalizedUserName)
    );
END;

IF COL_LENGTH('dbo.Users', 'IsLocked') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD IsLocked bit NOT NULL CONSTRAINT DF_Users_IsLocked DEFAULT 0;
END;

IF COL_LENGTH('dbo.Users', 'MustChangePassword') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD MustChangePassword bit NOT NULL CONSTRAINT DF_Users_MustChangePassword DEFAULT 0;
END;

IF COL_LENGTH('dbo.Users', 'LockoutEndAt') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD LockoutEndAt datetime2(0) NULL;
END;

IF COL_LENGTH('dbo.Users', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD IsDeleted bit NOT NULL CONSTRAINT DF_Users_IsDeleted DEFAULT 0;
END;

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        Code nvarchar(80) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Roles_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT UQ_Roles_Code UNIQUE (Code)
    );
END;

IF OBJECT_ID(N'dbo.Modules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Modules
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Modules PRIMARY KEY,
        Code nvarchar(80) NOT NULL,
        Name nvarchar(120) NOT NULL,
        DisplayOrder int NOT NULL CONSTRAINT DF_Modules_DisplayOrder DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_Modules_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Modules_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT UQ_Modules_Code UNIQUE (Code)
    );
END;

IF OBJECT_ID(N'dbo.Permissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permissions
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Permissions PRIMARY KEY,
        ModuleId int NOT NULL,
        Code nvarchar(120) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Permissions_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Permissions_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_Permissions_Modules FOREIGN KEY (ModuleId) REFERENCES dbo.Modules(Id),
        CONSTRAINT UQ_Permissions_Code UNIQUE (Code)
    );
END;

IF OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityOperations
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityOperations PRIMARY KEY,
        Name nvarchar(120) NOT NULL,
        Code nvarchar(80) NULL,
        Description nvarchar(300) NULL,
        RibbonPageName nvarchar(80) NULL,
        RibbonGroupName nvarchar(80) NULL,
        ActionKey nvarchar(120) NULL,
        IconLarge nvarchar(200) NULL,
        IconSmall nvarchar(200) NULL,
        DisplayOrder int NOT NULL CONSTRAINT DF_SecurityOperations_DisplayOrder DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityOperations_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityOperations_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityOperations_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT UQ_SecurityOperations_Name UNIQUE (Name)
    );
END;

IF COL_LENGTH('dbo.SecurityOperations', 'Code') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD Code nvarchar(80) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'Description') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD Description nvarchar(300) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'RibbonPageName') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD RibbonPageName nvarchar(80) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'RibbonGroupName') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD RibbonGroupName nvarchar(80) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'ActionKey') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD ActionKey nvarchar(120) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'IconLarge') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD IconLarge nvarchar(200) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'IconSmall') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD IconSmall nvarchar(200) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'DisplayOrder') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD DisplayOrder int NOT NULL CONSTRAINT DF_SecurityOperations_DisplayOrder DEFAULT 0;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'IsActive') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD IsActive bit NOT NULL CONSTRAINT DF_SecurityOperations_IsActive DEFAULT 1;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'CreatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD CreatedByUserId int NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'CreatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD CreatedByUserName nvarchar(120) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityOperations_CreatedAt DEFAULT SYSUTCDATETIME();
END;

IF COL_LENGTH('dbo.SecurityOperations', 'UpdatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD UpdatedByUserId int NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'UpdatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD UpdatedByUserName nvarchar(120) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'UpdatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD UpdatedAt datetime2(0) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD IsDeleted bit NOT NULL CONSTRAINT DF_SecurityOperations_IsDeleted DEFAULT 0;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'DeletedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD DeletedByUserId int NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'DeletedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD DeletedByUserName nvarchar(120) NULL;
END;

IF COL_LENGTH('dbo.SecurityOperations', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityOperations ADD DeletedAt datetime2(0) NULL;
END;

EXEC sys.sp_executesql N'
UPDATE dbo.SecurityOperations
SET
    Code = COALESCE(Code, UPPER(REPLACE(Name, N'' '', N''.''))),
    RibbonPageName = COALESCE(RibbonPageName, N''Inicio''),
    RibbonGroupName = COALESCE(RibbonGroupName, N''Acciones''),
    ActionKey = COALESCE(ActionKey, LOWER(REPLACE(Name, N'' '', N''.'')))
WHERE Code IS NULL
   OR RibbonPageName IS NULL
   OR RibbonGroupName IS NULL
   OR ActionKey IS NULL;';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_SecurityOperations_Code' AND object_id = OBJECT_ID(N'dbo.SecurityOperations'))
BEGIN
    CREATE UNIQUE INDEX UQ_SecurityOperations_Code ON dbo.SecurityOperations (Code);
END;

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

IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserRoles
    (
        UserId int NOT NULL,
        RoleId int NOT NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_UserRoles_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
END;

IF OBJECT_ID(N'dbo.RolePermissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RolePermissions
    (
        RoleId int NOT NULL,
        PermissionId int NOT NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_RolePermissions_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_RolePermissions PRIMARY KEY (RoleId, PermissionId),
        CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id),
        CONSTRAINT FK_RolePermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES dbo.Permissions(Id)
    );
END;

IF OBJECT_ID(N'dbo.UserCompanies', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserCompanies
    (
        UserId int NOT NULL,
        CompanyId int NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_UserCompanies_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_UserCompanies_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_UserCompanies PRIMARY KEY (UserId, CompanyId),
        CONSTRAINT FK_UserCompanies_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_UserCompanies_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id)
    );
END;

IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshTokens
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_RefreshTokens PRIMARY KEY,
        UserId int NOT NULL,
        TokenHash nvarchar(300) NOT NULL,
        ExpiresAt datetime2(0) NOT NULL,
        RevokedAt datetime2(0) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByIp nvarchar(64) NULL,
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT UQ_RefreshTokens_TokenHash UNIQUE (TokenHash)
    );
END;

IF OBJECT_ID(N'dbo.AuditLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditLogs
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLogs PRIMARY KEY,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        CompanyCode nvarchar(50) NULL,
        HttpMethod nvarchar(12) NOT NULL,
        [Path] nvarchar(500) NOT NULL,
        QueryString nvarchar(1000) NULL,
        StatusCode int NOT NULL,
        IpAddress nvarchar(64) NULL,
        UserAgent nvarchar(500) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditLogs_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AuditLogs_CreatedAt ON dbo.AuditLogs (CreatedAt DESC);
    CREATE INDEX IX_AuditLogs_UserId ON dbo.AuditLogs (UserId, CreatedAt DESC);
END;

IF OBJECT_ID(N'dbo.AuditErrorLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditErrorLogs
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditErrorLogs PRIMARY KEY,
        [Source] nvarchar(30) NOT NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        CompanyCode nvarchar(50) NULL,
        ModuleKey nvarchar(120) NULL,
        FormName nvarchar(180) NULL,
        ActionName nvarchar(120) NULL,
        HttpMethod nvarchar(12) NULL,
        [Path] nvarchar(500) NULL,
        QueryString nvarchar(1000) NULL,
        StatusCode int NULL,
        ErrorMessage nvarchar(2000) NOT NULL,
        ExceptionType nvarchar(300) NULL,
        StackTrace nvarchar(max) NULL,
        TraceId nvarchar(120) NULL,
        IpAddress nvarchar(64) NULL,
        MachineName nvarchar(120) NULL,
        UserAgent nvarchar(500) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditErrorLogs_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AuditErrorLogs_CreatedAt ON dbo.AuditErrorLogs (CreatedAt DESC);
    CREATE INDEX IX_AuditErrorLogs_UserId ON dbo.AuditErrorLogs (UserId, CreatedAt DESC);
    CREATE INDEX IX_AuditErrorLogs_Source_CreatedAt ON dbo.AuditErrorLogs ([Source], CreatedAt DESC);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SECURITY')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'SECURITY', N'Seguridad', 10);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'COMPANIES')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'COMPANIES', N'Empresas', 20);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'CATALOG')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'CATALOG', N'Catalogos', 30);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SALES')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'SALES', N'Ventas', 40);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SAP')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'SAP', N'Integracion SAP', 50);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SETTINGS')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'SETTINGS', N'Configuracion', 60);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Code = N'ADMIN')
BEGIN
    INSERT INTO dbo.Roles (Code, Name, Description)
    VALUES (N'ADMIN', N'Administrador', N'Rol con permisos administrativos del sistema');
END;

DECLARE @securityModuleId int = (SELECT Id FROM dbo.Modules WHERE Code = N'SECURITY');
DECLARE @companiesModuleId int = (SELECT Id FROM dbo.Modules WHERE Code = N'COMPANIES');
DECLARE @catalogModuleId int = (SELECT Id FROM dbo.Modules WHERE Code = N'CATALOG');
DECLARE @salesModuleId int = (SELECT Id FROM dbo.Modules WHERE Code = N'SALES');
DECLARE @sapModuleId int = (SELECT Id FROM dbo.Modules WHERE Code = N'SAP');
DECLARE @settingsModuleId int = (SELECT Id FROM dbo.Modules WHERE Code = N'SETTINGS');

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SECURITY.USERS.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@securityModuleId, N'SECURITY.USERS.MANAGE', N'Gestionar usuarios', N'Crear, editar, activar e inactivar usuarios');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SECURITY.ROLES.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@securityModuleId, N'SECURITY.ROLES.MANAGE', N'Gestionar roles', N'Crear roles y asignar permisos');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SECURITY.ACCESS.BYPASS')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@securityModuleId, N'SECURITY.ACCESS.BYPASS', N'Omitir permisos de formulario', N'Permite omitir la autorizacion dinamica por formulario y operacion');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SECURITY.AUDIT.READ')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@securityModuleId, N'SECURITY.AUDIT.READ', N'Consultar auditoria', N'Consultar trazabilidad de acciones del sistema');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'COMPANIES.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@companiesModuleId, N'COMPANIES.MANAGE', N'Gestionar empresas', N'Crear y configurar empresas');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'CATALOG.CUSTOMERS.READ')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@catalogModuleId, N'CATALOG.CUSTOMERS.READ', N'Consultar clientes', N'Listar y consultar clientes');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'CATALOG.CUSTOMERS.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@catalogModuleId, N'CATALOG.CUSTOMERS.MANAGE', N'Gestionar clientes', N'Crear, editar y eliminar clientes');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'CATALOG.ITEMS.READ')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@catalogModuleId, N'CATALOG.ITEMS.READ', N'Consultar articulos', N'Listar y consultar articulos');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'CATALOG.ITEMS.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@catalogModuleId, N'CATALOG.ITEMS.MANAGE', N'Gestionar articulos', N'Crear, editar y eliminar articulos');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SALES.DOCUMENTS.READ')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@salesModuleId, N'SALES.DOCUMENTS.READ', N'Consultar documentos', N'Listar y consultar documentos comerciales');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SALES.DOCUMENTS.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@salesModuleId, N'SALES.DOCUMENTS.MANAGE', N'Gestionar documentos', N'Crear documentos comerciales');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SAP.SYNC.READ')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@sapModuleId, N'SAP.SYNC.READ', N'Consultar sincronizacion SAP', N'Consultar logs de sincronizacion SAP');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SAP.SYNC.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@sapModuleId, N'SAP.SYNC.MANAGE', N'Gestionar sincronizacion SAP', N'Enviar documentos a SAP');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SETTINGS.PARAMETERS.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@settingsModuleId, N'SETTINGS.PARAMETERS.MANAGE', N'Gestionar parametros', N'Crear y editar parametros por empresa');
END;

DECLARE @adminRoleId int = (SELECT Id FROM dbo.Roles WHERE Code = N'ADMIN');

INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
SELECT @adminRoleId, p.Id
FROM dbo.Permissions p
WHERE p.Code IN (
    N'SECURITY.USERS.MANAGE',
    N'SECURITY.ROLES.MANAGE',
    N'SECURITY.ACCESS.BYPASS',
    N'SECURITY.AUDIT.READ',
    N'COMPANIES.MANAGE',
    N'CATALOG.CUSTOMERS.READ',
    N'CATALOG.CUSTOMERS.MANAGE',
    N'CATALOG.ITEMS.READ',
    N'CATALOG.ITEMS.MANAGE',
    N'SALES.DOCUMENTS.READ',
    N'SALES.DOCUMENTS.MANAGE',
    N'SAP.SYNC.READ',
    N'SAP.SYNC.MANAGE',
    N'SETTINGS.PARAMETERS.MANAGE')
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.RolePermissions rp
      WHERE rp.RoleId = @adminRoleId
        AND rp.PermissionId = p.Id
  );

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterSchemaHistory
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_MasterSchemaHistory PRIMARY KEY,
        Version nvarchar(50) NOT NULL,
        Description nvarchar(300) NOT NULL,
        AppliedAt datetime2(0) NOT NULL CONSTRAINT DF_MasterSchemaHistory_AppliedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_MasterSchemaHistory_Version UNIQUE (Version)
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260427.02')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260427.02', N'Fase 2: tablas base multiempresa');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260427.03')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260427.03', N'Fase 3: tablas base de seguridad');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260428.18')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260428.18', N'Fase 18: permisos operativos por modulo');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260428.19')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260428.19', N'Fase 19: auditoria operativa');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260429.01')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260429.01', N'Mantenimiento de operaciones de seguridad');
END;
""";

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task CreateStoredProceduresAsync(
        SqlConnection connection,
        CancellationToken cancellationToken)
    {
        var scriptPath = FindDatabaseScriptPath("005_master_security_operations_audit.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("006_master_security_menus.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("007_master_security_forms.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("008_master_role_access.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("009_master_security_users.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("010_master_security_roles.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("019_master_inventory_items_security.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("042_master_inventory_items_granular_permissions.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("045_master_general_inventory_auxiliary_security.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("011_master_configuration_companies.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("012_master_configuration_settings.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("049_master_sap_sync_worker.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("052_master_security_document_series.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }

        scriptPath = FindDatabaseScriptPath("054_master_operational_catalog_security.sql");
        if (scriptPath is not null)
        {
            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }
    }

    private static async Task ExecuteScriptFileAsync(
        SqlConnection connection,
        string scriptPath,
        CancellationToken cancellationToken)
    {
        var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        foreach (var batch in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(batch))
            {
                continue;
            }

            await using var command = connection.CreateCommand();
            command.CommandText = batch;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private static string? FindDatabaseScriptPath(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(directory.FullName, "database", "sql", fileName);
            if (File.Exists(path))
            {
                return path;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static string BuildDatabaseConnectionString(string baseConnectionString, string databaseName)
    {
        var builder = new SqlConnectionStringBuilder(baseConnectionString)
        {
            InitialCatalog = databaseName
        };

        return builder.ConnectionString;
    }

    private static string QuoteIdentifier(string value)
    {
        return $"[{value.Replace("]", "]]", StringComparison.Ordinal)}]";
    }
}
