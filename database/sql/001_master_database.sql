IF DB_ID(N'NuanSystem_Master') IS NULL
BEGIN
    CREATE DATABASE [NuanSystem_Master];
END;
GO

USE [NuanSystem_Master];
GO

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
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SystemParameters WHERE [Key] = N'System.Name')
BEGIN
    INSERT INTO dbo.SystemParameters ([Key], [Value], Description)
    VALUES (N'System.Name', N'NuanSystem', N'Nombre logico del sistema');
END;
GO

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
GO

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
GO

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
GO

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
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        UserName nvarchar(120) NOT NULL,
        NormalizedUserName nvarchar(120) NOT NULL,
        Email nvarchar(256) NULL,
        NormalizedEmail nvarchar(256) NULL,
        PhoneNumber nvarchar(30) NULL,
        NormalizedPhoneNumber nvarchar(30) NULL,
        EmailConfirmed bit NOT NULL CONSTRAINT DF_Users_EmailConfirmed DEFAULT 0,
        PhoneNumberConfirmed bit NOT NULL CONSTRAINT DF_Users_PhoneNumberConfirmed DEFAULT 0,
        FirstName nvarchar(120) NULL,
        LastName nvarchar(120) NULL,
        DisplayName nvarchar(200) NOT NULL,
        PasswordHash nvarchar(max) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
        IsLocked bit NOT NULL CONSTRAINT DF_Users_IsLocked DEFAULT 0,
        CanUseWeb bit NOT NULL CONSTRAINT DF_Users_CanUseWeb DEFAULT 1,
        CanUseMobile bit NOT NULL CONSTRAINT DF_Users_CanUseMobile DEFAULT 1,
        FailedAccessCount int NOT NULL CONSTRAINT DF_Users_FailedAccessCount DEFAULT 0,
        LastLoginAt datetime2(0) NULL,
        MustChangePassword bit NOT NULL CONSTRAINT DF_Users_MustChangePassword DEFAULT 0,
        LockoutEndAt datetime2(0) NULL,
        TwoFactorEnabled bit NOT NULL CONSTRAINT DF_Users_TwoFactorEnabled DEFAULT 0,
        ProfileImageUrl nvarchar(500) NULL,
        ProfileImage varbinary(max) NULL,
        ProfileImageContentType nvarchar(100) NULL,
        ProfileImageFileName nvarchar(260) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_Users_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT UQ_Users_NormalizedUserName UNIQUE (NormalizedUserName)
    );
END;
GO

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
GO

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
GO

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
GO

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
GO

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
GO

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
GO

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
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SECURITY')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'SECURITY', N'Seguridad', 10);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'COMPANIES')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'COMPANIES', N'Empresas', 20);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Code = N'ADMIN')
BEGIN
    INSERT INTO dbo.Roles (Code, Name, Description)
    VALUES (N'ADMIN', N'Administrador', N'Rol con permisos administrativos del sistema');
END;
GO

DECLARE @securityModuleId int = (SELECT Id FROM dbo.Modules WHERE Code = N'SECURITY');
DECLARE @companiesModuleId int = (SELECT Id FROM dbo.Modules WHERE Code = N'COMPANIES');

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

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'COMPANIES.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@companiesModuleId, N'COMPANIES.MANAGE', N'Gestionar empresas', N'Crear y configurar empresas');
END;
GO

DECLARE @adminRoleId int = (SELECT Id FROM dbo.Roles WHERE Code = N'ADMIN');

INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
SELECT @adminRoleId, p.Id
FROM dbo.Permissions p
WHERE p.Code IN (N'SECURITY.USERS.MANAGE', N'SECURITY.ROLES.MANAGE', N'SECURITY.ACCESS.BYPASS', N'COMPANIES.MANAGE')
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.RolePermissions rp
      WHERE rp.RoleId = @adminRoleId
        AND rp.PermissionId = p.Id
  );
GO

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
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260427.03')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260427.03', N'Fase 3: tablas base de seguridad');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260427.02')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260427.02', N'Fase 2: tablas base multiempresa');
END;
GO
