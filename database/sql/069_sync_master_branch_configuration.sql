SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncProfiles
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncProfiles PRIMARY KEY,
        CompanyId int NOT NULL,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(150) NOT NULL,
        Description nvarchar(500) NULL,
        Direction nvarchar(30) NOT NULL CONSTRAINT DF_SyncProfiles_Direction DEFAULT N'MasterToBranch',
        ExecutionMode nvarchar(20) NOT NULL CONSTRAINT DF_SyncProfiles_ExecutionMode DEFAULT N'Incremental',
        ConflictStrategy nvarchar(30) NOT NULL CONSTRAINT DF_SyncProfiles_ConflictStrategy DEFAULT N'MasterWins',
        BatchSize int NOT NULL CONSTRAINT DF_SyncProfiles_BatchSize DEFAULT 500,
        MaxRetries int NOT NULL CONSTRAINT DF_SyncProfiles_MaxRetries DEFAULT 3,
        RetryDelaySeconds int NOT NULL CONSTRAINT DF_SyncProfiles_RetryDelaySeconds DEFAULT 60,
        TimeoutMinutes int NOT NULL CONSTRAINT DF_SyncProfiles_TimeoutMinutes DEFAULT 30,
        IsActive bit NOT NULL CONSTRAINT DF_SyncProfiles_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncProfiles_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SyncProfiles_IsDeleted DEFAULT 0,
        CONSTRAINT FK_SyncProfiles_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_SyncProfiles_Direction CHECK (Direction IN (N'MasterToBranch')),
        CONSTRAINT CK_SyncProfiles_ExecutionMode CHECK (ExecutionMode IN (N'Incremental', N'Full', N'Manual')),
        CONSTRAINT CK_SyncProfiles_ConflictStrategy CHECK (ConflictStrategy IN (N'MasterWins')),
        CONSTRAINT CK_SyncProfiles_BatchSize CHECK (BatchSize BETWEEN 1 AND 10000),
        CONSTRAINT CK_SyncProfiles_MaxRetries CHECK (MaxRetries BETWEEN 0 AND 10),
        CONSTRAINT CK_SyncProfiles_RetryDelaySeconds CHECK (RetryDelaySeconds BETWEEN 0 AND 3600),
        CONSTRAINT CK_SyncProfiles_TimeoutMinutes CHECK (TimeoutMinutes BETWEEN 1 AND 1440)
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncProfileBranches', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncProfileBranches
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncProfileBranches PRIMARY KEY,
        SyncProfileId int NOT NULL,
        BranchCompanyId int NOT NULL,
        BatchSize int NULL,
        MaxRetries int NULL,
        IsActive bit NOT NULL CONSTRAINT DF_SyncProfileBranches_IsActive DEFAULT 1,
        LastSynchronizationAt datetime2(0) NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncProfileBranches_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SyncProfileBranches_IsDeleted DEFAULT 0,
        CONSTRAINT FK_SyncProfileBranches_Profile FOREIGN KEY (SyncProfileId) REFERENCES dbo.SyncProfiles(Id),
        CONSTRAINT FK_SyncProfileBranches_Companies FOREIGN KEY (BranchCompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_SyncProfileBranches_BatchSize CHECK (BatchSize IS NULL OR BatchSize BETWEEN 1 AND 10000),
        CONSTRAINT CK_SyncProfileBranches_MaxRetries CHECK (MaxRetries IS NULL OR MaxRetries BETWEEN 0 AND 10)
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncProfileEntities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncProfileEntities
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncProfileEntities PRIMARY KEY,
        SyncProfileId int NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        EntityName nvarchar(120) NOT NULL,
        ExecutionOrder int NOT NULL CONSTRAINT DF_SyncProfileEntities_ExecutionOrder DEFAULT 100,
        SyncMode nvarchar(20) NOT NULL CONSTRAINT DF_SyncProfileEntities_SyncMode DEFAULT N'Incremental',
        KeyField nvarchar(100) NULL,
        ModifiedAtField nvarchar(100) NULL,
        VersionField nvarchar(100) NULL,
        ActiveField nvarchar(100) NULL,
        AllowInsert bit NOT NULL CONSTRAINT DF_SyncProfileEntities_AllowInsert DEFAULT 1,
        AllowUpdate bit NOT NULL CONSTRAINT DF_SyncProfileEntities_AllowUpdate DEFAULT 1,
        AllowDeactivate bit NOT NULL CONSTRAINT DF_SyncProfileEntities_AllowDeactivate DEFAULT 1,
        ContinueOnError bit NOT NULL CONSTRAINT DF_SyncProfileEntities_ContinueOnError DEFAULT 0,
        BatchSize int NULL,
        IsActive bit NOT NULL CONSTRAINT DF_SyncProfileEntities_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncProfileEntities_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SyncProfileEntities_IsDeleted DEFAULT 0,
        CONSTRAINT FK_SyncProfileEntities_Profile FOREIGN KEY (SyncProfileId) REFERENCES dbo.SyncProfiles(Id),
        CONSTRAINT CK_SyncProfileEntities_SyncMode CHECK (SyncMode IN (N'Incremental', N'Full', N'Manual')),
        CONSTRAINT CK_SyncProfileEntities_BatchSize CHECK (BatchSize IS NULL OR BatchSize BETWEEN 1 AND 10000),
        CONSTRAINT CK_SyncProfileEntities_ExecutionOrder CHECK (ExecutionOrder >= 0),
        CONSTRAINT CK_SyncProfileEntities_EntityCode CHECK (EntityCode IN (
            N'Countries', N'Provinces', N'Cities', N'Currencies', N'BusinessPartnerPaymentTerms',
            N'SupplierGroups', N'SupplierClasses', N'EconomicActivities', N'Zones', N'SupplyMethods'))
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncProfileEntityBranches', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncProfileEntityBranches
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncProfileEntityBranches PRIMARY KEY,
        SyncProfileId int NOT NULL,
        SyncProfileEntityId int NOT NULL,
        SyncProfileBranchId int NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_SyncProfileEntityBranches_IsEnabled DEFAULT 1,
        BatchSize int NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncProfileEntityBranches_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SyncProfileEntityBranches_IsDeleted DEFAULT 0,
        CONSTRAINT FK_SyncProfileEntityBranches_Profile FOREIGN KEY (SyncProfileId) REFERENCES dbo.SyncProfiles(Id),
        CONSTRAINT CK_SyncProfileEntityBranches_BatchSize CHECK (BatchSize IS NULL OR BatchSize BETWEEN 1 AND 10000)
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncSchedules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncSchedules
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncSchedules PRIMARY KEY,
        SyncProfileId int NOT NULL,
        ScheduleType nvarchar(20) NOT NULL CONSTRAINT DF_SyncSchedules_ScheduleType DEFAULT N'Manual',
        IntervalMinutes int NULL,
        ExecutionTime time(0) NULL,
        TimeZoneId nvarchar(100) NOT NULL CONSTRAINT DF_SyncSchedules_TimeZoneId DEFAULT N'America/Guayaquil',
        PreventConcurrentExecutions bit NOT NULL CONSTRAINT DF_SyncSchedules_PreventConcurrentExecutions DEFAULT 1,
        IsActive bit NOT NULL CONSTRAINT DF_SyncSchedules_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncSchedules_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SyncSchedules_IsDeleted DEFAULT 0,
        CONSTRAINT FK_SyncSchedules_Profile FOREIGN KEY (SyncProfileId) REFERENCES dbo.SyncProfiles(Id),
        CONSTRAINT CK_SyncSchedules_Type CHECK (ScheduleType IN (N'Manual', N'Interval', N'Daily')),
        CONSTRAINT CK_SyncSchedules_IntervalMinutes CHECK (IntervalMinutes IS NULL OR IntervalMinutes BETWEEN 1 AND 1440),
        CONSTRAINT CK_SyncSchedules_Shape CHECK (
            (ScheduleType = N'Manual' AND IntervalMinutes IS NULL AND ExecutionTime IS NULL) OR
            (ScheduleType = N'Interval' AND IntervalMinutes IS NOT NULL AND ExecutionTime IS NULL) OR
            (ScheduleType = N'Daily' AND IntervalMinutes IS NULL AND ExecutionTime IS NOT NULL))
    );
END;
GO

IF OBJECT_ID(N'dbo.Modules', N'U') IS NOT NULL
    AND OBJECT_ID(N'dbo.Permissions', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SYNC')
    BEGIN
        INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
        VALUES (N'SYNC', N'Sincronizacion Master/Sucursal', 70);
    END;

    DECLARE @SyncModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'SYNC');

    MERGE dbo.Permissions AS target
    USING
    (
        VALUES
            (@SyncModuleId, N'SYNC.CONFIGURATION.VIEW', N'Sync Configuration View', N'Consultar configuracion Maestro-Sucursal.'),
            (@SyncModuleId, N'SYNC.CONFIGURATION.CREATE', N'Sync Configuration Create', N'Crear perfiles de configuracion Maestro-Sucursal.'),
            (@SyncModuleId, N'SYNC.CONFIGURATION.EDIT', N'Sync Configuration Edit', N'Editar perfiles de configuracion Maestro-Sucursal.'),
            (@SyncModuleId, N'SYNC.CONFIGURATION.DELETE', N'Sync Configuration Delete', N'Eliminar perfiles de configuracion Maestro-Sucursal sin historial operativo.'),
            (@SyncModuleId, N'SYNC.CONFIGURATION.ACTIVATE', N'Sync Configuration Activate', N'Activar o desactivar perfiles Maestro-Sucursal.'),
            (@SyncModuleId, N'SYNC.CONFIGURATION.VALIDATE', N'Sync Configuration Validate', N'Validar perfiles Maestro-Sucursal.')
    ) AS source(ModuleId, Code, Name, Description)
    ON target.Code = source.Code
    WHEN MATCHED THEN
        UPDATE SET ModuleId = source.ModuleId, Name = source.Name, Description = source.Description
    WHEN NOT MATCHED THEN
        INSERT (ModuleId, Code, Name, Description)
        VALUES (source.ModuleId, source.Code, source.Name, source.Description);

    DECLARE @SyncConfigurationAdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
    IF @SyncConfigurationAdminRoleId IS NOT NULL
    BEGIN
        INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
        SELECT @SyncConfigurationAdminRoleId, permission.Id
        FROM dbo.Permissions permission
        WHERE permission.Code IN
        (
            N'SYNC.CONFIGURATION.VIEW',
            N'SYNC.CONFIGURATION.CREATE',
            N'SYNC.CONFIGURATION.EDIT',
            N'SYNC.CONFIGURATION.DELETE',
            N'SYNC.CONFIGURATION.ACTIVATE',
            N'SYNC.CONFIGURATION.VALIDATE'
        )
          AND NOT EXISTS
          (
              SELECT 1
              FROM dbo.RolePermissions existing
              WHERE existing.RoleId = @SyncConfigurationAdminRoleId
                AND existing.PermissionId = permission.Id
          );
    END;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncProfiles_Company_Code_Active' AND object_id = OBJECT_ID(N'dbo.SyncProfiles'))
    CREATE UNIQUE INDEX UX_SyncProfiles_Company_Code_Active ON dbo.SyncProfiles (CompanyId, Code) WHERE IsDeleted = 0;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncProfiles_CompanyId' AND object_id = OBJECT_ID(N'dbo.SyncProfiles'))
    CREATE INDEX IX_SyncProfiles_CompanyId ON dbo.SyncProfiles (CompanyId, IsActive) WHERE IsDeleted = 0;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncProfileBranches_Profile_Branch_Active' AND object_id = OBJECT_ID(N'dbo.SyncProfileBranches'))
    CREATE UNIQUE INDEX UX_SyncProfileBranches_Profile_Branch_Active ON dbo.SyncProfileBranches (SyncProfileId, BranchCompanyId) WHERE IsDeleted = 0;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncProfileBranches_Id_Profile' AND object_id = OBJECT_ID(N'dbo.SyncProfileBranches'))
    CREATE UNIQUE INDEX UX_SyncProfileBranches_Id_Profile ON dbo.SyncProfileBranches (Id, SyncProfileId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncProfileEntities_Profile_Code_Active' AND object_id = OBJECT_ID(N'dbo.SyncProfileEntities'))
    CREATE UNIQUE INDEX UX_SyncProfileEntities_Profile_Code_Active ON dbo.SyncProfileEntities (SyncProfileId, EntityCode) WHERE IsDeleted = 0;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncProfileEntities_Id_Profile' AND object_id = OBJECT_ID(N'dbo.SyncProfileEntities'))
    CREATE UNIQUE INDEX UX_SyncProfileEntities_Id_Profile ON dbo.SyncProfileEntities (Id, SyncProfileId);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_SyncProfileEntityBranches_Entities_Profile')
    ALTER TABLE dbo.SyncProfileEntityBranches ADD CONSTRAINT FK_SyncProfileEntityBranches_Entities_Profile FOREIGN KEY (SyncProfileEntityId, SyncProfileId) REFERENCES dbo.SyncProfileEntities(Id, SyncProfileId);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_SyncProfileEntityBranches_Branches_Profile')
    ALTER TABLE dbo.SyncProfileEntityBranches ADD CONSTRAINT FK_SyncProfileEntityBranches_Branches_Profile FOREIGN KEY (SyncProfileBranchId, SyncProfileId) REFERENCES dbo.SyncProfileBranches(Id, SyncProfileId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncProfileEntityBranches_Entity_Branch_Active' AND object_id = OBJECT_ID(N'dbo.SyncProfileEntityBranches'))
    CREATE UNIQUE INDEX UX_SyncProfileEntityBranches_Entity_Branch_Active ON dbo.SyncProfileEntityBranches (SyncProfileEntityId, SyncProfileBranchId) WHERE IsDeleted = 0;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncSchedules_Profile_Active' AND object_id = OBJECT_ID(N'dbo.SyncSchedules'))
    CREATE UNIQUE INDEX UX_SyncSchedules_Profile_Active ON dbo.SyncSchedules (SyncProfileId) WHERE IsActive = 1 AND IsDeleted = 0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCPROFILELISTAR
    @CompanyId int = NULL,
    @IsActive bit = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        profile.Id,
        profile.CompanyId,
        company.Code AS CompanyCode,
        company.CommercialName AS CompanyName,
        profile.Code,
        profile.Name,
        profile.Description,
        profile.Direction,
        profile.ExecutionMode,
        profile.ConflictStrategy,
        profile.BatchSize,
        profile.MaxRetries,
        profile.RetryDelaySeconds,
        profile.TimeoutMinutes,
        profile.IsActive,
        (SELECT COUNT(1) FROM dbo.SyncProfileBranches branch WHERE branch.SyncProfileId = profile.Id AND branch.IsDeleted = 0) AS BranchCount,
        (SELECT COUNT(1) FROM dbo.SyncProfileEntities entity WHERE entity.SyncProfileId = profile.Id AND entity.IsDeleted = 0) AS EntityCount,
        profile.CreatedAt,
        profile.UpdatedAt
    FROM dbo.SyncProfiles profile
    INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
    WHERE profile.IsDeleted = 0
      AND (@CompanyId IS NULL OR profile.CompanyId = @CompanyId)
      AND (@IsActive IS NULL OR profile.IsActive = @IsActive)
    ORDER BY company.CommercialName, profile.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCPROFILEPAGINAR
    @Search nvarchar(150) = NULL,
    @CompanyId int = NULL,
    @IsActive bit = NULL,
    @ExecutionMode nvarchar(20) = NULL,
    @PageNumber int = 1,
    @PageSize int = 50,
    @UserId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber IS NULL OR @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize IS NULL OR @PageSize < 1 SET @PageSize = 50;
    IF @PageSize > 500 SET @PageSize = 500;

    DECLARE @Offset int = (@PageNumber - 1) * @PageSize;
    DECLARE @SearchPattern nvarchar(160) = CASE WHEN NULLIF(LTRIM(RTRIM(@Search)), N'') IS NULL THEN NULL ELSE N'%' + LTRIM(RTRIM(@Search)) + N'%' END;

    SELECT
        profile.Id,
        profile.Code,
        profile.Name,
        profile.CompanyId,
        company.CommercialName AS CompanyName,
        (SELECT COUNT(1) FROM dbo.SyncProfileBranches branch WHERE branch.SyncProfileId = profile.Id AND branch.IsDeleted = 0) AS BranchCount,
        (SELECT COUNT(1) FROM dbo.SyncProfileEntities entity WHERE entity.SyncProfileId = profile.Id AND entity.IsDeleted = 0) AS EntityCount,
        profile.Direction,
        profile.ExecutionMode,
        profile.ConflictStrategy,
        profile.BatchSize,
        profile.MaxRetries,
        profile.IsActive,
        CAST(NULL AS datetime2(0)) AS LastExecutionAt,
        CAST(NULL AS datetime2(0)) AS NextExecutionAt,
        profile.CreatedByUserId,
        profile.CreatedByUserName,
        profile.CreatedAt,
        profile.UpdatedByUserId,
        profile.UpdatedByUserName,
        profile.UpdatedAt
    FROM dbo.SyncProfiles profile
    INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
    WHERE profile.IsDeleted = 0
      AND (@CompanyId IS NULL OR profile.CompanyId = @CompanyId)
      AND (@IsActive IS NULL OR profile.IsActive = @IsActive)
      AND (@ExecutionMode IS NULL OR profile.ExecutionMode = @ExecutionMode)
      AND (@SearchPattern IS NULL OR profile.Code LIKE @SearchPattern OR profile.Name LIKE @SearchPattern OR company.CommercialName LIKE @SearchPattern)
      AND (@UserId IS NULL OR EXISTS (
          SELECT 1 FROM dbo.UserCompanies uc WHERE uc.UserId = @UserId AND uc.CompanyId = profile.CompanyId AND uc.IsActive = 1))
    ORDER BY company.CommercialName, profile.Code
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.SyncProfiles profile
    INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
    WHERE profile.IsDeleted = 0
      AND (@CompanyId IS NULL OR profile.CompanyId = @CompanyId)
      AND (@IsActive IS NULL OR profile.IsActive = @IsActive)
      AND (@ExecutionMode IS NULL OR profile.ExecutionMode = @ExecutionMode)
      AND (@SearchPattern IS NULL OR profile.Code LIKE @SearchPattern OR profile.Name LIKE @SearchPattern OR company.CommercialName LIKE @SearchPattern)
      AND (@UserId IS NULL OR EXISTS (
          SELECT 1 FROM dbo.UserCompanies uc WHERE uc.UserId = @UserId AND uc.CompanyId = profile.CompanyId AND uc.IsActive = 1));
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCCONFIGURATIONCOMPANYLOOKUPS
    @UserId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        company.Id,
        company.Code,
        company.CommercialName AS Name,
        company.IsActive,
        company.IsMaster,
        company.ParentCompanyId,
        company.SyncEnabled
    FROM dbo.Companies company
    WHERE company.IsDeleted = 0
      AND (@UserId IS NULL OR EXISTS (
          SELECT 1 FROM dbo.UserCompanies uc WHERE uc.UserId = @UserId AND uc.CompanyId = company.Id AND uc.IsActive = 1))
      AND (company.IsMaster = 1 OR company.ParentCompanyId IS NOT NULL)
    ORDER BY company.IsMaster DESC, company.CommercialName;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCPROFILEBUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        profile.Id, profile.CompanyId, company.Code AS CompanyCode, company.CommercialName AS CompanyName,
        profile.Code, profile.Name, profile.Description, profile.Direction, profile.ExecutionMode,
        profile.ConflictStrategy, profile.BatchSize, profile.MaxRetries, profile.RetryDelaySeconds,
        profile.TimeoutMinutes, profile.IsActive, profile.CreatedByUserId, profile.CreatedByUserName,
        profile.CreatedAt, profile.UpdatedByUserId, profile.UpdatedByUserName, profile.UpdatedAt
    FROM dbo.SyncProfiles profile
    INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
    WHERE profile.Id = @Id AND profile.IsDeleted = 0;

    SELECT
        branch.Id, branch.SyncProfileId, branch.BranchCompanyId,
        company.Code AS BranchCompanyCode, company.CommercialName AS BranchCompanyName,
        branch.BatchSize, branch.MaxRetries, branch.IsActive, branch.LastSynchronizationAt
    FROM dbo.SyncProfileBranches branch
    INNER JOIN dbo.Companies company ON company.Id = branch.BranchCompanyId
    WHERE branch.SyncProfileId = @Id AND branch.IsDeleted = 0
    ORDER BY company.CommercialName;

    SELECT
        entity.Id, entity.SyncProfileId, entity.EntityCode, entity.EntityName, entity.ExecutionOrder,
        entity.SyncMode, entity.KeyField, entity.ModifiedAtField, entity.VersionField, entity.ActiveField,
        entity.AllowInsert, entity.AllowUpdate, entity.AllowDeactivate, entity.ContinueOnError,
        entity.BatchSize, entity.IsActive
    FROM dbo.SyncProfileEntities entity
    WHERE entity.SyncProfileId = @Id AND entity.IsDeleted = 0
    ORDER BY entity.ExecutionOrder, entity.EntityCode;

    SELECT
        map.Id, map.SyncProfileEntityId, map.SyncProfileBranchId, map.SyncProfileId,
        entity.EntityCode, branch.BranchCompanyId, map.IsEnabled, map.BatchSize
    FROM dbo.SyncProfileEntityBranches map
    INNER JOIN dbo.SyncProfileEntities entity ON entity.Id = map.SyncProfileEntityId
    INNER JOIN dbo.SyncProfileBranches branch ON branch.Id = map.SyncProfileBranchId
    WHERE map.SyncProfileId = @Id AND map.IsDeleted = 0
    ORDER BY entity.ExecutionOrder, entity.EntityCode, branch.BranchCompanyId;

    SELECT TOP (1)
        Id, SyncProfileId, ScheduleType, IntervalMinutes, ExecutionTime, TimeZoneId,
        PreventConcurrentExecutions, IsActive
    FROM dbo.SyncSchedules
    WHERE SyncProfileId = @Id AND IsDeleted = 0
    ORDER BY IsActive DESC, ISNULL(UpdatedAt, CreatedAt) DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCPROFILEBUSCARPORCODIGO
    @CompanyId int,
    @Code nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Id int = (SELECT Id FROM dbo.SyncProfiles WHERE CompanyId = @CompanyId AND Code = @Code AND IsDeleted = 0);
    EXEC dbo.SP_NA_GET_SYNCPROFILEBUSCARPORID @Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SYNCPROFILECREAR
    @CompanyId int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @Direction nvarchar(30),
    @ExecutionMode nvarchar(20),
    @ConflictStrategy nvarchar(30),
    @BatchSize int,
    @MaxRetries int,
    @RetryDelaySeconds int,
    @TimeoutMinutes int,
    @IsActive bit,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL,
    @BranchesJson nvarchar(max) = N'[]',
    @EntitiesJson nvarchar(max) = N'[]',
    @EntityBranchesJson nvarchar(max) = N'[]',
    @ScheduleJson nvarchar(max) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId AND IsMaster = 1 AND SyncEnabled = 1 AND IsDeleted = 0)
        THROW 51000, 'La empresa maestra no existe o no tiene sincronizacion habilitada.', 1;
    IF EXISTS (SELECT 1 FROM dbo.SyncProfiles WHERE CompanyId = @CompanyId AND Code = @Code AND IsDeleted = 0)
        THROW 51001, 'Ya existe un perfil de sincronizacion con el mismo codigo para la empresa.', 1;
    IF @Direction <> N'MasterToBranch' OR @ConflictStrategy <> N'MasterWins' OR @ExecutionMode NOT IN (N'Incremental', N'Full', N'Manual')
        THROW 51002, 'Valores de perfil no soportados para la primera version.', 1;

    BEGIN TRANSACTION;
        INSERT INTO dbo.SyncProfiles
        (
            CompanyId, Code, Name, Description, Direction, ExecutionMode, ConflictStrategy,
            BatchSize, MaxRetries, RetryDelaySeconds, TimeoutMinutes, IsActive,
            CreatedByUserId, CreatedByUserName
        )
        VALUES
        (
            @CompanyId, @Code, @Name, @Description, @Direction, @ExecutionMode, @ConflictStrategy,
            @BatchSize, @MaxRetries, @RetryDelaySeconds, @TimeoutMinutes, @IsActive,
            @AuditUserId, @AuditUserName
        );

        DECLARE @ProfileId int = CONVERT(int, SCOPE_IDENTITY());
        EXEC dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR
            @Id = @ProfileId,
            @CompanyId = @CompanyId,
            @Code = @Code,
            @Name = @Name,
            @Description = @Description,
            @Direction = @Direction,
            @ExecutionMode = @ExecutionMode,
            @ConflictStrategy = @ConflictStrategy,
            @BatchSize = @BatchSize,
            @MaxRetries = @MaxRetries,
            @RetryDelaySeconds = @RetryDelaySeconds,
            @TimeoutMinutes = @TimeoutMinutes,
            @IsActive = @IsActive,
            @AuditUserId = @AuditUserId,
            @AuditUserName = @AuditUserName,
            @BranchesJson = @BranchesJson,
            @EntitiesJson = @EntitiesJson,
            @EntityBranchesJson = @EntityBranchesJson,
            @ScheduleJson = @ScheduleJson,
            @SuppressResult = 1;
    COMMIT TRANSACTION;

    SELECT @ProfileId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR
    @Id int,
    @CompanyId int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @Direction nvarchar(30),
    @ExecutionMode nvarchar(20),
    @ConflictStrategy nvarchar(30),
    @BatchSize int,
    @MaxRetries int,
    @RetryDelaySeconds int,
    @TimeoutMinutes int,
    @IsActive bit,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL,
    @BranchesJson nvarchar(max) = N'[]',
    @EntitiesJson nvarchar(max) = N'[]',
    @EntityBranchesJson nvarchar(max) = N'[]',
    @ScheduleJson nvarchar(max) = NULL,
    @SuppressResult bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.SyncProfiles WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        IF @SuppressResult = 0
            SELECT 0;
    END
    ELSE
    BEGIN
        DECLARE @Branches table (BranchCompanyId int NOT NULL PRIMARY KEY, BatchSize int NULL, MaxRetries int NULL, IsActive bit NOT NULL);
        DECLARE @Entities table (EntityCode nvarchar(80) NOT NULL PRIMARY KEY, EntityName nvarchar(120) NOT NULL, ExecutionOrder int NOT NULL, SyncMode nvarchar(20) NOT NULL, KeyField nvarchar(100) NULL, ModifiedAtField nvarchar(100) NULL, VersionField nvarchar(100) NULL, ActiveField nvarchar(100) NULL, AllowInsert bit NOT NULL, AllowUpdate bit NOT NULL, AllowDeactivate bit NOT NULL, ContinueOnError bit NOT NULL, BatchSize int NULL, IsActive bit NOT NULL);
        DECLARE @Matrix table (EntityCode nvarchar(80) NOT NULL, BranchCompanyId int NOT NULL, IsEnabled bit NOT NULL, BatchSize int NULL, PRIMARY KEY (EntityCode, BranchCompanyId));

        INSERT INTO @Branches
        SELECT BranchCompanyId, BatchSize, MaxRetries, IsActive
        FROM OPENJSON(ISNULL(@BranchesJson, N'[]'))
        WITH (BranchCompanyId int '$.branchCompanyId', BatchSize int '$.batchSize', MaxRetries int '$.maxRetries', IsActive bit '$.isActive');

        INSERT INTO @Entities
        SELECT EntityCode, EntityName, ExecutionOrder, SyncMode, KeyField, ModifiedAtField, VersionField, ActiveField, AllowInsert, AllowUpdate, AllowDeactivate, ContinueOnError, BatchSize, IsActive
        FROM OPENJSON(ISNULL(@EntitiesJson, N'[]'))
        WITH (EntityCode nvarchar(80) '$.entityCode', EntityName nvarchar(120) '$.entityName', ExecutionOrder int '$.executionOrder', SyncMode nvarchar(20) '$.syncMode', KeyField nvarchar(100) '$.keyField', ModifiedAtField nvarchar(100) '$.modifiedAtField', VersionField nvarchar(100) '$.versionField', ActiveField nvarchar(100) '$.activeField', AllowInsert bit '$.allowInsert', AllowUpdate bit '$.allowUpdate', AllowDeactivate bit '$.allowDeactivate', ContinueOnError bit '$.continueOnError', BatchSize int '$.batchSize', IsActive bit '$.isActive');

        INSERT INTO @Matrix
        SELECT EntityCode, BranchCompanyId, IsEnabled, BatchSize
        FROM OPENJSON(ISNULL(@EntityBranchesJson, N'[]'))
        WITH (EntityCode nvarchar(80) '$.entityCode', BranchCompanyId int '$.branchCompanyId', IsEnabled bit '$.isEnabled', BatchSize int '$.batchSize');

        IF EXISTS (SELECT 1 FROM @Branches branch WHERE NOT EXISTS (SELECT 1 FROM dbo.Companies company WHERE company.Id = branch.BranchCompanyId AND company.ParentCompanyId = @CompanyId AND company.IsMaster = 0 AND company.SyncEnabled = 1 AND company.IsDeleted = 0))
            THROW 51003, 'Una sucursal no pertenece a la empresa maestra o no tiene sincronizacion habilitada.', 1;
        IF EXISTS (SELECT 1 FROM @Entities entity WHERE entity.EntityCode NOT IN (N'Countries', N'Provinces', N'Cities', N'Currencies', N'BusinessPartnerPaymentTerms', N'SupplierGroups', N'SupplierClasses', N'EconomicActivities', N'Zones', N'SupplyMethods'))
            THROW 51004, 'Una entidad no pertenece al catalogo inicial permitido.', 1;
        IF EXISTS (SELECT 1 FROM @Matrix matrix WHERE NOT EXISTS (SELECT 1 FROM @Entities entity WHERE entity.EntityCode = matrix.EntityCode) OR NOT EXISTS (SELECT 1 FROM @Branches branch WHERE branch.BranchCompanyId = matrix.BranchCompanyId))
            THROW 51005, 'La matriz entidad-sucursal referencia una entidad o sucursal no incluida en el perfil.', 1;

        BEGIN TRANSACTION;
            UPDATE dbo.SyncProfiles
            SET CompanyId = @CompanyId,
                Code = @Code,
                Name = @Name,
                Description = @Description,
                Direction = @Direction,
                ExecutionMode = @ExecutionMode,
                ConflictStrategy = @ConflictStrategy,
                BatchSize = @BatchSize,
                MaxRetries = @MaxRetries,
                RetryDelaySeconds = @RetryDelaySeconds,
                TimeoutMinutes = @TimeoutMinutes,
                IsActive = @IsActive,
                UpdatedByUserId = @AuditUserId,
                UpdatedByUserName = @AuditUserName,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @Id AND IsDeleted = 0;

            UPDATE branch
            SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
            FROM dbo.SyncProfileBranches branch
            WHERE branch.SyncProfileId = @Id AND branch.IsDeleted = 0
              AND NOT EXISTS (SELECT 1 FROM @Branches source WHERE source.BranchCompanyId = branch.BranchCompanyId);

            MERGE dbo.SyncProfileBranches AS target
            USING @Branches AS source
            ON target.SyncProfileId = @Id AND target.BranchCompanyId = source.BranchCompanyId
            WHEN MATCHED THEN UPDATE SET BatchSize = source.BatchSize, MaxRetries = source.MaxRetries, IsActive = source.IsActive, IsDeleted = 0, UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME(), DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL
            WHEN NOT MATCHED THEN INSERT (SyncProfileId, BranchCompanyId, BatchSize, MaxRetries, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Id, source.BranchCompanyId, source.BatchSize, source.MaxRetries, source.IsActive, @AuditUserId, @AuditUserName);

            UPDATE entity
            SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
            FROM dbo.SyncProfileEntities entity
            WHERE entity.SyncProfileId = @Id AND entity.IsDeleted = 0
              AND NOT EXISTS (SELECT 1 FROM @Entities source WHERE source.EntityCode = entity.EntityCode);

            MERGE dbo.SyncProfileEntities AS target
            USING @Entities AS source
            ON target.SyncProfileId = @Id AND target.EntityCode = source.EntityCode
            WHEN MATCHED THEN UPDATE SET EntityName = source.EntityName, ExecutionOrder = source.ExecutionOrder, SyncMode = source.SyncMode, KeyField = source.KeyField, ModifiedAtField = source.ModifiedAtField, VersionField = source.VersionField, ActiveField = source.ActiveField, AllowInsert = source.AllowInsert, AllowUpdate = source.AllowUpdate, AllowDeactivate = source.AllowDeactivate, ContinueOnError = source.ContinueOnError, BatchSize = source.BatchSize, IsActive = source.IsActive, IsDeleted = 0, UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME(), DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL
            WHEN NOT MATCHED THEN INSERT (SyncProfileId, EntityCode, EntityName, ExecutionOrder, SyncMode, KeyField, ModifiedAtField, VersionField, ActiveField, AllowInsert, AllowUpdate, AllowDeactivate, ContinueOnError, BatchSize, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Id, source.EntityCode, source.EntityName, source.ExecutionOrder, source.SyncMode, source.KeyField, source.ModifiedAtField, source.VersionField, source.ActiveField, source.AllowInsert, source.AllowUpdate, source.AllowDeactivate, source.ContinueOnError, source.BatchSize, source.IsActive, @AuditUserId, @AuditUserName);

            UPDATE map
            SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
            FROM dbo.SyncProfileEntityBranches map
            INNER JOIN dbo.SyncProfileEntities entity ON entity.Id = map.SyncProfileEntityId
            INNER JOIN dbo.SyncProfileBranches branch ON branch.Id = map.SyncProfileBranchId
            WHERE map.SyncProfileId = @Id AND map.IsDeleted = 0
              AND NOT EXISTS (SELECT 1 FROM @Matrix source WHERE source.EntityCode = entity.EntityCode AND source.BranchCompanyId = branch.BranchCompanyId);

            MERGE dbo.SyncProfileEntityBranches AS target
            USING (
                SELECT entity.Id AS SyncProfileEntityId, branch.Id AS SyncProfileBranchId, matrix.IsEnabled, matrix.BatchSize
                FROM @Matrix matrix
                INNER JOIN dbo.SyncProfileEntities entity ON entity.SyncProfileId = @Id AND entity.EntityCode = matrix.EntityCode AND entity.IsDeleted = 0
                INNER JOIN dbo.SyncProfileBranches branch ON branch.SyncProfileId = @Id AND branch.BranchCompanyId = matrix.BranchCompanyId AND branch.IsDeleted = 0
            ) AS source
            ON target.SyncProfileEntityId = source.SyncProfileEntityId AND target.SyncProfileBranchId = source.SyncProfileBranchId
            WHEN MATCHED THEN UPDATE SET IsEnabled = source.IsEnabled, BatchSize = source.BatchSize, SyncProfileId = @Id, IsDeleted = 0, UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME(), DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL
            WHEN NOT MATCHED THEN INSERT (SyncProfileId, SyncProfileEntityId, SyncProfileBranchId, IsEnabled, BatchSize, CreatedByUserId, CreatedByUserName) VALUES (@Id, source.SyncProfileEntityId, source.SyncProfileBranchId, source.IsEnabled, source.BatchSize, @AuditUserId, @AuditUserName);

            IF @ScheduleJson IS NULL
            BEGIN
                UPDATE dbo.SyncSchedules
                SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
                WHERE SyncProfileId = @Id AND IsDeleted = 0;
            END
            ELSE
            BEGIN
                DECLARE @ScheduleType nvarchar(20), @IntervalMinutes int, @ExecutionTime time(0), @TimeZoneId nvarchar(100), @PreventConcurrentExecutions bit, @ScheduleIsActive bit;
                SELECT @ScheduleType = ScheduleType, @IntervalMinutes = IntervalMinutes, @ExecutionTime = TRY_CONVERT(time(0), ExecutionTime), @TimeZoneId = ISNULL(NULLIF(TimeZoneId, N''), N'America/Guayaquil'), @PreventConcurrentExecutions = PreventConcurrentExecutions, @ScheduleIsActive = IsActive
                FROM OPENJSON(@ScheduleJson)
                WITH (ScheduleType nvarchar(20) '$.scheduleType', IntervalMinutes int '$.intervalMinutes', ExecutionTime nvarchar(8) '$.executionTime', TimeZoneId nvarchar(100) '$.timeZoneId', PreventConcurrentExecutions bit '$.preventConcurrentExecutions', IsActive bit '$.isActive');

                UPDATE dbo.SyncSchedules SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME() WHERE SyncProfileId = @Id AND IsDeleted = 0;
                INSERT INTO dbo.SyncSchedules (SyncProfileId, ScheduleType, IntervalMinutes, ExecutionTime, TimeZoneId, PreventConcurrentExecutions, IsActive, CreatedByUserId, CreatedByUserName)
                VALUES (@Id, @ScheduleType, @IntervalMinutes, @ExecutionTime, @TimeZoneId, @PreventConcurrentExecutions, @ScheduleIsActive, @AuditUserId, @AuditUserName);
            END;
        COMMIT TRANSACTION;

        IF @SuppressResult = 0
            SELECT 1;
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SYNCPROFILEACTIVAR
    @Id int,
    @IsActive bit,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.SyncProfiles
    SET IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    DECLARE @Affected int = @@ROWCOUNT;

    IF @Affected > 0 AND @IsActive = 0
    BEGIN
        UPDATE dbo.SyncSchedules
        SET IsActive = 0,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE SyncProfileId = @Id
          AND IsDeleted = 0
          AND IsActive = 1;
    END;

    SELECT @Affected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_SYNCPROFILEELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.SyncProfiles WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        SELECT 0;
        RETURN;
    END;

    BEGIN TRANSACTION;
        UPDATE dbo.SyncProfileEntityBranches
        SET IsDeleted = 1, DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName, DeletedAt = SYSUTCDATETIME()
        WHERE SyncProfileId = @Id AND IsDeleted = 0;

        UPDATE dbo.SyncSchedules
        SET IsDeleted = 1, DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName, DeletedAt = SYSUTCDATETIME(), IsActive = 0
        WHERE SyncProfileId = @Id AND IsDeleted = 0;

        UPDATE dbo.SyncProfileEntities
        SET IsDeleted = 1, DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName, DeletedAt = SYSUTCDATETIME()
        WHERE SyncProfileId = @Id AND IsDeleted = 0;

        UPDATE dbo.SyncProfileBranches
        SET IsDeleted = 1, DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName, DeletedAt = SYSUTCDATETIME()
        WHERE SyncProfileId = @Id AND IsDeleted = 0;

        UPDATE dbo.SyncProfiles
        SET IsDeleted = 1,
            IsActive = 0,
            DeletedByUserId = @DeletedByUserId,
            DeletedByUserName = @DeletedByUserName,
            DeletedAt = SYSUTCDATETIME()
        WHERE Id = @Id AND IsDeleted = 0;

        DECLARE @Affected int = @@ROWCOUNT;
    COMMIT TRANSACTION;

    SELECT @Affected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SYNCPROFILEAUDITREGISTRAR
    @ProfileId int = NULL,
    @Action nvarchar(60),
    @FieldName nvarchar(120) = NULL,
    @OldValue nvarchar(max) = NULL,
    @NewValue nvarchar(max) = NULL,
    @UserId int = NULL,
    @UserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF OBJECT_ID(N'dbo.AuditSecurityChanges', N'U') IS NOT NULL
    BEGIN
        INSERT INTO dbo.AuditSecurityChanges
        (
            EntityName,
            RecordId,
            [Action],
            FieldName,
            OldValue,
            NewValue,
            UserId,
            UserName,
            [Source]
        )
        VALUES
        (
            N'SyncProfiles',
            COALESCE(CONVERT(nvarchar(80), @ProfileId), N'NEW'),
            @Action,
            @FieldName,
            @OldValue,
            @NewValue,
            @UserId,
            @UserName,
            N'API'
        );
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCPROFILETIENEHISTORIAL
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.SyncProfiles profile
    WHERE profile.Id = @Id
      AND profile.IsDeleted = 0
      AND (
          EXISTS (
              SELECT 1
              FROM dbo.SyncOutbox outbox
              INNER JOIN dbo.SyncProfileEntities entity ON entity.SyncProfileId = profile.Id AND entity.IsDeleted = 0
              WHERE outbox.CompanyId = profile.CompanyId
                AND outbox.EntityName IN (entity.EntityName, entity.EntityCode))
          OR EXISTS (
              SELECT 1
              FROM dbo.SyncAudit audit
              INNER JOIN dbo.SyncProfileEntities entity ON entity.SyncProfileId = profile.Id AND entity.IsDeleted = 0
              WHERE audit.EntityName IN (entity.EntityName, entity.EntityCode)));
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260711.01')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description, AppliedAt)
    VALUES (N'20260711.01', N'Sync master-branch configuration model', SYSUTCDATETIME());
END;
GO
