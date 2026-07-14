SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncProfileEntities', N'U') IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_SyncProfileEntities_EntityCode'
          AND parent_object_id = OBJECT_ID(N'dbo.SyncProfileEntities')
    )
    BEGIN
        ALTER TABLE dbo.SyncProfileEntities DROP CONSTRAINT CK_SyncProfileEntities_EntityCode;
    END;

    ALTER TABLE dbo.SyncProfileEntities WITH CHECK
    ADD CONSTRAINT CK_SyncProfileEntities_EntityCode CHECK (EntityCode IN (
        N'Countries', N'Provinces', N'Cities', N'Currencies', N'BusinessPartnerPaymentTerms',
        N'SupplierGroups', N'SupplierClasses', N'EconomicActivities', N'Zones', N'SupplyMethods',
        N'BusinessPartner', N'Item', N'Warehouse'));
END;
GO

IF OBJECT_ID(N'dbo.SyncProfiles', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncProfiles_Routing' AND object_id = OBJECT_ID(N'dbo.SyncProfiles'))
BEGIN
    CREATE INDEX IX_SyncProfiles_Routing
        ON dbo.SyncProfiles (CompanyId, Direction, ExecutionMode, ConflictStrategy, IsActive)
        INCLUDE (Code, BatchSize, MaxRetries, RetryDelaySeconds, TimeoutMinutes)
        WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.SyncProfileEntities', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncProfileEntities_Routing' AND object_id = OBJECT_ID(N'dbo.SyncProfileEntities'))
BEGIN
    CREATE INDEX IX_SyncProfileEntities_Routing
        ON dbo.SyncProfileEntities (SyncProfileId, EntityCode, IsActive)
        INCLUDE (BatchSize, AllowInsert, AllowUpdate, AllowDeactivate, ContinueOnError, ExecutionOrder)
        WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.SyncProfileBranches', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncProfileBranches_Routing' AND object_id = OBJECT_ID(N'dbo.SyncProfileBranches'))
BEGIN
    CREATE INDEX IX_SyncProfileBranches_Routing
        ON dbo.SyncProfileBranches (SyncProfileId, BranchCompanyId, IsActive)
        INCLUDE (BatchSize, MaxRetries)
        WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.SyncProfileEntityBranches', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncProfileEntityBranches_Routing' AND object_id = OBJECT_ID(N'dbo.SyncProfileEntityBranches'))
BEGIN
    CREATE INDEX IX_SyncProfileEntityBranches_Routing
        ON dbo.SyncProfileEntityBranches (SyncProfileId, SyncProfileEntityId, SyncProfileBranchId, IsEnabled)
        INCLUDE (BatchSize)
        WHERE IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCROUTINGTARGETS
    @SourceCompanyId int,
    @EntityCode nvarchar(80)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedEntityCode nvarchar(80) = LTRIM(RTRIM(@EntityCode));

    SELECT DISTINCT
        profile.Id AS SyncProfileId,
        entity.Id AS SyncProfileEntityId,
        profile.Code AS SyncProfileCode,
        profile.CompanyId AS SourceCompanyId,
        profileBranch.BranchCompanyId,
        entity.EntityCode,
        COALESCE(matrix.BatchSize, entity.BatchSize, profileBranch.BatchSize, profile.BatchSize) AS BatchSize,
        COALESCE(profileBranch.MaxRetries, profile.MaxRetries) AS MaxRetries,
        profile.RetryDelaySeconds,
        profile.TimeoutMinutes,
        entity.AllowInsert,
        entity.AllowUpdate,
        entity.AllowDeactivate,
        entity.ContinueOnError
    FROM dbo.SyncProfiles AS profile
    INNER JOIN dbo.Companies AS sourceCompany
        ON sourceCompany.Id = profile.CompanyId
       AND sourceCompany.IsActive = 1
       AND sourceCompany.IsMaster = 1
       AND sourceCompany.SyncEnabled = 1
       AND sourceCompany.IsDeleted = 0
    INNER JOIN dbo.SyncProfileEntities AS entity
        ON entity.SyncProfileId = profile.Id
       AND entity.IsDeleted = 0
       AND entity.IsActive = 1
       AND entity.EntityCode = @NormalizedEntityCode
       AND entity.SyncMode = N'Incremental'
    INNER JOIN dbo.SyncProfileEntityBranches AS matrix
        ON matrix.SyncProfileId = profile.Id
       AND matrix.SyncProfileEntityId = entity.Id
       AND matrix.IsDeleted = 0
       AND matrix.IsEnabled = 1
    INNER JOIN dbo.SyncProfileBranches AS profileBranch
        ON profileBranch.Id = matrix.SyncProfileBranchId
       AND profileBranch.SyncProfileId = profile.Id
       AND profileBranch.IsDeleted = 0
       AND profileBranch.IsActive = 1
    INNER JOIN dbo.Companies AS branchCompany
        ON branchCompany.Id = profileBranch.BranchCompanyId
       AND branchCompany.IsActive = 1
       AND branchCompany.IsMaster = 0
       AND branchCompany.SyncEnabled = 1
       AND branchCompany.ParentCompanyId = profile.CompanyId
       AND branchCompany.IsDeleted = 0
    WHERE profile.CompanyId = @SourceCompanyId
      AND profile.IsDeleted = 0
      AND profile.IsActive = 1
      AND profile.Direction = N'MasterToBranch'
      AND profile.ExecutionMode = N'Incremental'
      AND profile.ConflictStrategy = N'MasterWins'
    ORDER BY profileBranch.BranchCompanyId, profile.Id, entity.Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCPROFILEACTIVECONFLICTS
    @ProfileId int = NULL,
    @CompanyId int,
    @CombinationsJson nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;

    IF ISJSON(@CombinationsJson) <> 1
    BEGIN
        THROW 51070, 'Las combinaciones de routing no tienen formato JSON valido.', 1;
    END;

    ;WITH RequestedCombinations AS
    (
        SELECT DISTINCT
            LTRIM(RTRIM(EntityCode)) AS EntityCode,
            BranchCompanyId
        FROM OPENJSON(@CombinationsJson)
        WITH
        (
            EntityCode nvarchar(80) '$.EntityCode',
            BranchCompanyId int '$.BranchCompanyId'
        )
        WHERE NULLIF(LTRIM(RTRIM(EntityCode)), N'') IS NOT NULL
          AND BranchCompanyId IS NOT NULL
    )
    SELECT DISTINCT
        profile.Id AS SyncProfileId,
        profile.Code AS SyncProfileCode,
        profileBranch.BranchCompanyId,
        entity.EntityCode
    FROM RequestedCombinations AS requested
    INNER JOIN dbo.SyncProfiles AS profile
        ON profile.CompanyId = @CompanyId
       AND profile.IsDeleted = 0
       AND profile.IsActive = 1
       AND profile.Direction = N'MasterToBranch'
       AND profile.ExecutionMode = N'Incremental'
       AND profile.ConflictStrategy = N'MasterWins'
       AND (@ProfileId IS NULL OR profile.Id <> @ProfileId)
    INNER JOIN dbo.SyncProfileEntities AS entity
        ON entity.SyncProfileId = profile.Id
       AND entity.IsDeleted = 0
       AND entity.IsActive = 1
       AND entity.SyncMode = N'Incremental'
       AND entity.EntityCode = requested.EntityCode
    INNER JOIN dbo.SyncProfileEntityBranches AS matrix
        ON matrix.SyncProfileId = profile.Id
       AND matrix.SyncProfileEntityId = entity.Id
       AND matrix.IsDeleted = 0
       AND matrix.IsEnabled = 1
    INNER JOIN dbo.SyncProfileBranches AS profileBranch
        ON profileBranch.Id = matrix.SyncProfileBranchId
       AND profileBranch.SyncProfileId = profile.Id
       AND profileBranch.BranchCompanyId = requested.BranchCompanyId
       AND profileBranch.IsDeleted = 0
       AND profileBranch.IsActive = 1
    ORDER BY entity.EntityCode, profileBranch.BranchCompanyId, profile.Id;
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260711.070')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description, AppliedAt)
    VALUES (N'20260711.070', N'Routing configurable Master-Branch sobre SyncOutboxTargets', SYSUTCDATETIME());
END;
GO
