SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncProfileEntityBranches', N'U') IS NULL
    THROW 51093, 'No existe SyncProfileEntityBranches. Ejecute primero el instalador Master Sync.', 1;
GO

IF COL_LENGTH(N'dbo.SyncProfileEntityBranches', N'DistributionMode') IS NULL
    ALTER TABLE dbo.SyncProfileEntityBranches ADD DistributionMode nvarchar(20) NOT NULL
        CONSTRAINT DF_SyncProfileEntityBranches_DistributionMode DEFAULT N'All';
IF COL_LENGTH(N'dbo.SyncProfileEntityBranches', N'OnNoMatch') IS NULL
    ALTER TABLE dbo.SyncProfileEntityBranches ADD OnNoMatch nvarchar(30) NOT NULL
        CONSTRAINT DF_SyncProfileEntityBranches_OnNoMatch DEFAULT N'KeepInMaster';
IF COL_LENGTH(N'dbo.SyncProfileEntityBranches', N'RuleExpressionJson') IS NULL
    ALTER TABLE dbo.SyncProfileEntityBranches ADD RuleExpressionJson nvarchar(max) NULL;
IF COL_LENGTH(N'dbo.SyncProfileEntityBranches', N'RuleVersion') IS NULL
    ALTER TABLE dbo.SyncProfileEntityBranches ADD RuleVersion int NOT NULL
        CONSTRAINT DF_SyncProfileEntityBranches_RuleVersion DEFAULT 1;
GO

UPDATE dbo.SyncProfileEntityBranches
SET DistributionMode = CASE WHEN IsEnabled = 1 THEN N'All' ELSE N'None' END
WHERE DistributionMode IS NULL OR DistributionMode NOT IN (N'None', N'All', N'Selected', N'Rule');
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_SyncProfileEntityBranches_DistributionMode')
    ALTER TABLE dbo.SyncProfileEntityBranches WITH CHECK ADD CONSTRAINT CK_SyncProfileEntityBranches_DistributionMode
        CHECK (DistributionMode IN (N'None', N'All', N'Selected', N'Rule'));
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_SyncProfileEntityBranches_OnNoMatch')
    ALTER TABLE dbo.SyncProfileEntityBranches WITH CHECK ADD CONSTRAINT CK_SyncProfileEntityBranches_OnNoMatch
        CHECK (OnNoMatch IN (N'KeepInMaster'));
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_SyncProfileEntityBranches_RuleJson')
    ALTER TABLE dbo.SyncProfileEntityBranches WITH CHECK ADD CONSTRAINT CK_SyncProfileEntityBranches_RuleJson
        CHECK (RuleExpressionJson IS NULL OR ISJSON(RuleExpressionJson) = 1);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_SyncProfileEntityBranches_RuleVersion')
    ALTER TABLE dbo.SyncProfileEntityBranches WITH CHECK ADD CONSTRAINT CK_SyncProfileEntityBranches_RuleVersion
        CHECK (RuleVersion > 0);
GO

IF OBJECT_ID(N'dbo.SyncDistributionSelections', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncDistributionSelections
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncDistributionSelections PRIMARY KEY,
        SyncProfileEntityBranchId int NOT NULL,
        EntityGlobalId uniqueidentifier NOT NULL,
        EntityCode nvarchar(120) NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncDistributionSelections_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SyncDistributionSelections_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SyncDistributionSelections_Matrix FOREIGN KEY (SyncProfileEntityBranchId)
            REFERENCES dbo.SyncProfileEntityBranches(Id)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncDistributionSelections_Matrix_GlobalId_Active' AND object_id = OBJECT_ID(N'dbo.SyncDistributionSelections'))
    CREATE UNIQUE INDEX UX_SyncDistributionSelections_Matrix_GlobalId_Active
        ON dbo.SyncDistributionSelections (SyncProfileEntityBranchId, EntityGlobalId) WHERE IsDeleted = 0;
GO

IF OBJECT_ID(N'dbo.SyncDistributionDecisionLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncDistributionDecisionLog
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncDistributionDecisionLog PRIMARY KEY,
        OutboxId bigint NOT NULL,
        SyncProfileEntityBranchId int NOT NULL,
        BranchCompanyId int NOT NULL,
        EntityGlobalId uniqueidentifier NOT NULL,
        DistributionMode nvarchar(20) NOT NULL,
        Matched bit NOT NULL,
        Reason nvarchar(500) NOT NULL,
        RuleVersion int NOT NULL,
        EvaluatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncDistributionDecisionLog_EvaluatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_SyncDistributionDecisionLog_Outbox FOREIGN KEY (OutboxId) REFERENCES dbo.SyncOutbox(Id),
        CONSTRAINT FK_SyncDistributionDecisionLog_Matrix FOREIGN KEY (SyncProfileEntityBranchId) REFERENCES dbo.SyncProfileEntityBranches(Id),
        CONSTRAINT FK_SyncDistributionDecisionLog_Branch FOREIGN KEY (BranchCompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT UQ_SyncDistributionDecisionLog_Outbox_Matrix UNIQUE (OutboxId, SyncProfileEntityBranchId)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncDistributionDecisionLog_GlobalId_Date' AND object_id = OBJECT_ID(N'dbo.SyncDistributionDecisionLog'))
    CREATE INDEX IX_SyncDistributionDecisionLog_GlobalId_Date
        ON dbo.SyncDistributionDecisionLog (EntityGlobalId, EvaluatedAt DESC);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCDISTRIBUTIONPOLICYBYMATRIXID
    @MatrixId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        matrix.Id AS SyncProfileEntityBranchId,
        profile.Id AS SyncProfileId,
        profile.Code AS SyncProfileCode,
        profile.CompanyId,
        masterCompany.Code AS CompanyCode,
        entity.EntityCode,
        profileBranch.BranchCompanyId,
        branchCompany.Code AS BranchCompanyCode,
        COALESCE(NULLIF(branchCompany.CommercialName, N''), NULLIF(branchCompany.LegalName, N''), branchCompany.Code) AS BranchCompanyName,
        matrix.DistributionMode,
        matrix.OnNoMatch,
        matrix.RuleExpressionJson,
        matrix.RuleVersion
    FROM dbo.SyncProfileEntityBranches matrix
    INNER JOIN dbo.SyncProfiles profile ON profile.Id = matrix.SyncProfileId AND profile.IsDeleted = 0
    INNER JOIN dbo.Companies masterCompany ON masterCompany.Id = profile.CompanyId AND masterCompany.IsDeleted = 0
    INNER JOIN dbo.SyncProfileEntities entity ON entity.Id = matrix.SyncProfileEntityId AND entity.IsDeleted = 0
    INNER JOIN dbo.SyncProfileBranches profileBranch ON profileBranch.Id = matrix.SyncProfileBranchId AND profileBranch.IsDeleted = 0
    INNER JOIN dbo.Companies branchCompany ON branchCompany.Id = profileBranch.BranchCompanyId AND branchCompany.IsDeleted = 0
    WHERE matrix.Id = @MatrixId AND matrix.IsDeleted = 0;

    SELECT EntityGlobalId, EntityCode
    FROM dbo.SyncDistributionSelections
    WHERE SyncProfileEntityBranchId = @MatrixId AND IsDeleted = 0
    ORDER BY EntityCode, EntityGlobalId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SYNCDISTRIBUTIONPOLICYACTUALIZAR
    @MatrixId int,
    @DistributionMode nvarchar(20),
    @OnNoMatch nvarchar(30),
    @RuleExpressionJson nvarchar(max) = NULL,
    @SelectionsJson nvarchar(max) = N'[]',
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @DistributionMode NOT IN (N'None', N'All', N'Selected', N'Rule')
        THROW 51094, 'Modo de distribucion no valido.', 1;
    IF @OnNoMatch <> N'KeepInMaster'
        THROW 51095, 'Accion OnNoMatch no valida.', 1;
    IF ISJSON(ISNULL(@SelectionsJson, N'')) <> 1
        THROW 51096, 'SelectionsJson no contiene JSON valido.', 1;
    IF @RuleExpressionJson IS NOT NULL AND ISJSON(@RuleExpressionJson) <> 1
        THROW 51097, 'RuleExpressionJson no contiene JSON valido.', 1;

    DECLARE @Selections TABLE (EntityGlobalId uniqueidentifier NOT NULL PRIMARY KEY, EntityCode nvarchar(120) NULL);
    INSERT INTO @Selections (EntityGlobalId, EntityCode)
    SELECT EntityGlobalId, NULLIF(LTRIM(RTRIM(EntityCode)), N'')
    FROM OPENJSON(@SelectionsJson)
    WITH (EntityGlobalId uniqueidentifier '$.entityGlobalId', EntityCode nvarchar(120) '$.entityCode')
    WHERE EntityGlobalId IS NOT NULL;

    BEGIN TRANSACTION;

    UPDATE dbo.SyncProfileEntityBranches
    SET DistributionMode = @DistributionMode,
        OnNoMatch = @OnNoMatch,
        RuleExpressionJson = CASE WHEN @DistributionMode = N'Rule' THEN @RuleExpressionJson ELSE NULL END,
        RuleVersion = RuleVersion + 1,
        IsEnabled = 1,
        UpdatedByUserId = @AuditUserId,
        UpdatedByUserName = @AuditUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @MatrixId AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        ROLLBACK TRANSACTION;
        SELECT 0;
        RETURN;
    END;

    UPDATE existing
    SET IsDeleted = 1,
        DeletedByUserId = @AuditUserId,
        DeletedByUserName = @AuditUserName,
        DeletedAt = SYSUTCDATETIME()
    FROM dbo.SyncDistributionSelections existing
    WHERE existing.SyncProfileEntityBranchId = @MatrixId
      AND existing.IsDeleted = 0
      AND NOT EXISTS (SELECT 1 FROM @Selections source WHERE source.EntityGlobalId = existing.EntityGlobalId);

    UPDATE existing
    SET EntityCode = source.EntityCode,
        UpdatedByUserId = @AuditUserId,
        UpdatedByUserName = @AuditUserName,
        UpdatedAt = SYSUTCDATETIME()
    FROM dbo.SyncDistributionSelections existing
    INNER JOIN @Selections source ON source.EntityGlobalId = existing.EntityGlobalId
    WHERE existing.SyncProfileEntityBranchId = @MatrixId AND existing.IsDeleted = 0;

    INSERT INTO dbo.SyncDistributionSelections
        (SyncProfileEntityBranchId, EntityGlobalId, EntityCode, CreatedByUserId, CreatedByUserName)
    SELECT @MatrixId, source.EntityGlobalId, source.EntityCode, @AuditUserId, @AuditUserName
    FROM @Selections source
    WHERE NOT EXISTS
    (
        SELECT 1 FROM dbo.SyncDistributionSelections existing
        WHERE existing.SyncProfileEntityBranchId = @MatrixId
          AND existing.EntityGlobalId = source.EntityGlobalId
          AND existing.IsDeleted = 0
    );

    COMMIT TRANSACTION;
    SELECT 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SYNCDISTRIBUTIONDECISIONREGISTRAR
    @OutboxId bigint,
    @SyncProfileEntityBranchId int,
    @BranchCompanyId int,
    @EntityGlobalId uniqueidentifier,
    @DistributionMode nvarchar(20),
    @Matched bit,
    @Reason nvarchar(500),
    @RuleVersion int
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.SyncDistributionDecisionLog
        WHERE OutboxId = @OutboxId AND SyncProfileEntityBranchId = @SyncProfileEntityBranchId
    )
    BEGIN
        INSERT INTO dbo.SyncDistributionDecisionLog
            (OutboxId, SyncProfileEntityBranchId, BranchCompanyId, EntityGlobalId, DistributionMode, Matched, Reason, RuleVersion)
        VALUES
            (@OutboxId, @SyncProfileEntityBranchId, @BranchCompanyId, @EntityGlobalId, @DistributionMode, @Matched, LEFT(@Reason, 500), @RuleVersion);
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCDISTRIBUTIONRULETARGETS
    @CompanyId int,
    @EntityName nvarchar(120),
    @EntityCode nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT distRule.BranchCompanyId
    FROM dbo.SyncDistributionRules AS distRule
    INNER JOIN dbo.Companies AS branch ON branch.Id = distRule.BranchCompanyId
    WHERE distRule.CompanyId = @CompanyId
      AND distRule.EntityName = @EntityName
      AND distRule.IsEnabled = 1
      AND branch.IsActive = 1
      AND branch.IsMaster = 0
      AND branch.SyncEnabled = 1
      AND branch.ParentCompanyId = @CompanyId
      AND branch.IsDeleted = 0
      AND
      (
          distRule.RuleType = N'All'
          OR (distRule.RuleType = N'ByEntityCode' AND distRule.RuleValue = @EntityCode)
          OR (distRule.RuleType = N'ByBranch' AND distRule.RuleValue = branch.BranchCode)
      )
    ORDER BY distRule.BranchCompanyId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCROUTINGTARGETS
    @SourceCompanyId int,
    @EntityCode nvarchar(80),
    @SyncProfileId int = NULL,
    @TargetBranchCode nvarchar(50) = NULL,
    @RequireTargetBranchMatch bit = 0,
    @EntityGlobalId uniqueidentifier = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedEntityCode nvarchar(80) = LTRIM(RTRIM(@EntityCode));
    DECLARE @NormalizedTargetBranchCode nvarchar(50) = NULLIF(LTRIM(RTRIM(@TargetBranchCode)), N'');

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
        entity.ContinueOnError,
        matrix.Id AS SyncProfileEntityBranchId,
        matrix.DistributionMode,
        matrix.OnNoMatch,
        matrix.RuleExpressionJson,
        matrix.RuleVersion,
        CONVERT(bit, CASE WHEN EXISTS
        (
            SELECT 1 FROM dbo.SyncDistributionSelections selection
            WHERE selection.SyncProfileEntityBranchId = matrix.Id
              AND selection.EntityGlobalId = @EntityGlobalId
              AND selection.IsDeleted = 0
        ) THEN 1 ELSE 0 END) AS IsSelected
    FROM dbo.SyncProfiles AS profile
    INNER JOIN dbo.Companies AS sourceCompany ON sourceCompany.Id = profile.CompanyId
       AND sourceCompany.IsActive = 1 AND sourceCompany.IsMaster = 1 AND sourceCompany.SyncEnabled = 1 AND sourceCompany.IsDeleted = 0
    INNER JOIN dbo.SyncProfileEntities AS entity ON entity.SyncProfileId = profile.Id
       AND entity.IsDeleted = 0 AND entity.IsActive = 1 AND entity.EntityCode = @NormalizedEntityCode
       AND (@SyncProfileId IS NOT NULL OR entity.SyncMode = N'Incremental')
    INNER JOIN dbo.SyncProfileEntityBranches AS matrix ON matrix.SyncProfileId = profile.Id
       AND matrix.SyncProfileEntityId = entity.Id AND matrix.IsDeleted = 0 AND matrix.IsEnabled = 1
    INNER JOIN dbo.SyncProfileBranches AS profileBranch ON profileBranch.Id = matrix.SyncProfileBranchId
       AND profileBranch.SyncProfileId = profile.Id AND profileBranch.IsDeleted = 0 AND profileBranch.IsActive = 1
    INNER JOIN dbo.Companies AS branchCompany ON branchCompany.Id = profileBranch.BranchCompanyId
       AND branchCompany.IsActive = 1 AND branchCompany.IsMaster = 0 AND branchCompany.SyncEnabled = 1
       AND branchCompany.ParentCompanyId = profile.CompanyId AND branchCompany.IsDeleted = 0
    WHERE profile.CompanyId = @SourceCompanyId
      AND profile.IsDeleted = 0 AND profile.IsActive = 1
      AND profile.Direction = N'MasterToBranch' AND profile.ConflictStrategy = N'MasterWins'
      AND (@RequireTargetBranchMatch = 0 OR (@NormalizedTargetBranchCode IS NOT NULL AND branchCompany.BranchCode = @NormalizedTargetBranchCode))
      AND ((@SyncProfileId IS NULL AND profile.ExecutionMode = N'Incremental')
        OR (@SyncProfileId IS NOT NULL AND profile.Id = @SyncProfileId AND profile.ExecutionMode IN (N'Incremental', N'Full', N'Manual')))
    ORDER BY profileBranch.BranchCompanyId, profile.Id, entity.Id;
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260716.093')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260716.093', N'Politicas genericas de distribucion por entidad y sucursal');
END;
GO
