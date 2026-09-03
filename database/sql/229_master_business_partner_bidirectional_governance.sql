/*
    BusinessPartner bidirectional governance - Master database.

    Prerequisites: 064, 069, 080, 092, 094 and 227.
    Adds policy, correlation, closed directions and permissions without
    activating profiles, routes, entity configurations, workers, SAP or SRI.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()<>N'NuanSystem_Master'
    THROW 52229, 'Migration 229 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.Companies',N'U') IS NULL
    THROW 52229, 'Companies is required before migration 229.', 1;
IF OBJECT_ID(N'dbo.SyncOutbox',N'U') IS NULL
    THROW 52229, 'SyncOutbox is required before migration 229.', 1;
IF OBJECT_ID(N'dbo.SyncProfiles',N'U') IS NULL
    THROW 52229, 'SyncProfiles is required before migration 229.', 1;
IF OBJECT_ID(N'dbo.SyncProfileBranches',N'U') IS NULL
    THROW 52229, 'SyncProfileBranches is required before migration 229.', 1;
IF OBJECT_ID(N'dbo.SyncProfileEntities',N'U') IS NULL
    THROW 52229, 'SyncProfileEntities is required before migration 229.', 1;
IF OBJECT_ID(N'dbo.SyncProfileEntityBranches',N'U') IS NULL
    THROW 52229, 'SyncProfileEntityBranches is required before migration 229.', 1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL
    THROW 52229, 'SyncEntityDefinitions is required before migration 229.', 1;
IF OBJECT_ID(N'dbo.AuditSyncConfigurationChanges',N'U') IS NULL
    THROW 52229, 'AuditSyncConfigurationChanges is required before migration 229.', 1;
IF OBJECT_ID(N'dbo.Modules',N'U') IS NULL OR OBJECT_ID(N'dbo.Permissions',N'U') IS NULL
    THROW 52229, 'Security permissions are required before migration 229.', 1;
IF OBJECT_ID(N'dbo.Roles',N'U') IS NULL OR OBJECT_ID(N'dbo.RolePermissions',N'U') IS NULL
    THROW 52229, 'Security roles are required before migration 229.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL
    THROW 52229, 'MasterSchemaHistory is required before migration 229.', 1;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerSapCodePolicies',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerSapCodePolicies
    (
        CompanyId int NOT NULL CONSTRAINT PK_BusinessPartnerSapCodePolicies PRIMARY KEY,
        IsEnabled bit NOT NULL CONSTRAINT DF_BusinessPartnerSapCodePolicies_IsEnabled DEFAULT (0),
        PrefixMode varchar(20) NOT NULL,
        PassportIdentificationTypeCode nvarchar(30) NOT NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerSapCodePolicies_UpdatedAt DEFAULT SYSUTCDATETIME(),
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_BusinessPartnerSapCodePolicies_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_BusinessPartnerSapCodePolicies_PrefixMode CHECK (PrefixMode IN ('NationalForeign','RoleOnly')),
        CONSTRAINT CK_BusinessPartnerSapCodePolicies_PassportCode CHECK (LEN(LTRIM(RTRIM(PassportIdentificationTypeCode))) BETWEEN 1 AND 30)
    );
END;
GO

IF EXISTS
(
    SELECT 1
    FROM
    (
        VALUES
            (N'CompanyId',56,4,0),
            (N'IsEnabled',104,1,0),
            (N'PrefixMode',167,20,0),
            (N'PassportIdentificationTypeCode',231,60,0),
            (N'UpdatedByUserId',56,4,1),
            (N'UpdatedByUserName',231,240,1),
            (N'UpdatedAt',42,6,0),
            (N'RowVersion',189,8,0)
    ) expected(Name,SystemTypeId,MaxLength,IsNullable)
    LEFT JOIN sys.columns actual
      ON actual.object_id=OBJECT_ID(N'dbo.BusinessPartnerSapCodePolicies')
     AND actual.name=expected.Name
    WHERE actual.column_id IS NULL
       OR actual.system_type_id<>expected.SystemTypeId
       OR actual.max_length<>expected.MaxLength
       OR actual.is_nullable<>expected.IsNullable
)
    THROW 52229, 'BusinessPartnerSapCodePolicies has an incompatible shape.', 1;

IF NOT EXISTS
(
    SELECT 1 FROM sys.key_constraints
    WHERE parent_object_id=OBJECT_ID(N'dbo.BusinessPartnerSapCodePolicies')
      AND name=N'PK_BusinessPartnerSapCodePolicies'
      AND [type]=N'PK'
)
    THROW 52229, 'BusinessPartnerSapCodePolicies primary key is missing.', 1;
IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE parent_object_id=OBJECT_ID(N'dbo.BusinessPartnerSapCodePolicies')
      AND referenced_object_id=OBJECT_ID(N'dbo.Companies')
      AND name=N'FK_BusinessPartnerSapCodePolicies_Companies'
      AND is_disabled=0
      AND is_not_trusted=0
)
    THROW 52229, 'BusinessPartnerSapCodePolicies company foreign key is missing or untrusted.', 1;
IF NOT EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id=OBJECT_ID(N'dbo.BusinessPartnerSapCodePolicies')
      AND name=N'CK_BusinessPartnerSapCodePolicies_PrefixMode'
      AND is_disabled=0
      AND is_not_trusted=0
)
    THROW 52229, 'BusinessPartnerSapCodePolicies prefix-mode check is missing or untrusted.', 1;
IF NOT EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id=OBJECT_ID(N'dbo.BusinessPartnerSapCodePolicies')
      AND name=N'CK_BusinessPartnerSapCodePolicies_PassportCode'
      AND is_disabled=0
      AND is_not_trusted=0
)
    THROW 52229, 'BusinessPartnerSapCodePolicies passport-code check is missing or untrusted.', 1;
GO

IF COL_LENGTH(N'dbo.SyncOutbox',N'CausationEventId') IS NULL
    ALTER TABLE dbo.SyncOutbox ADD CausationEventId uniqueidentifier NULL;
GO

IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id=OBJECT_ID(N'dbo.SyncOutbox')
      AND name=N'CausationEventId'
      AND (system_type_id<>36 OR max_length<>16 OR is_nullable<>1)
)
    THROW 52229, 'SyncOutbox.CausationEventId has an incompatible shape.', 1;
GO

IF OBJECT_ID(N'dbo.CK_SyncProfiles_Direction',N'C') IS NOT NULL
    ALTER TABLE dbo.SyncProfiles DROP CONSTRAINT CK_SyncProfiles_Direction;
IF OBJECT_ID(N'dbo.CK_SyncProfiles_ConflictStrategy',N'C') IS NOT NULL
    ALTER TABLE dbo.SyncProfiles DROP CONSTRAINT CK_SyncProfiles_ConflictStrategy;
IF OBJECT_ID(N'dbo.CK_SyncProfiles_DirectionPolicy',N'C') IS NOT NULL
    ALTER TABLE dbo.SyncProfiles DROP CONSTRAINT CK_SyncProfiles_DirectionPolicy;
GO

ALTER TABLE dbo.SyncProfiles WITH CHECK
    ADD CONSTRAINT CK_SyncProfiles_Direction CHECK (Direction IN (N'MasterToBranch',N'BranchToMaster'));
ALTER TABLE dbo.SyncProfiles CHECK CONSTRAINT CK_SyncProfiles_Direction;
ALTER TABLE dbo.SyncProfiles WITH CHECK
    ADD CONSTRAINT CK_SyncProfiles_ConflictStrategy CHECK (ConflictStrategy IN (N'MasterWins',N'CentralReview'));
ALTER TABLE dbo.SyncProfiles CHECK CONSTRAINT CK_SyncProfiles_ConflictStrategy;
ALTER TABLE dbo.SyncProfiles WITH CHECK
    ADD CONSTRAINT CK_SyncProfiles_DirectionPolicy CHECK
    (
        (Direction=N'MasterToBranch' AND ConflictStrategy=N'MasterWins')
        OR
        (Direction=N'BranchToMaster' AND ConflictStrategy=N'CentralReview' AND ExecutionMode=N'Incremental')
    );
ALTER TABLE dbo.SyncProfiles CHECK CONSTRAINT CK_SyncProfiles_DirectionPolicy;
GO

DECLARE @Definitions table
(
    Code nvarchar(80) PRIMARY KEY,
    Name nvarchar(120) NOT NULL,
    Description nvarchar(500) NOT NULL,
    DefaultExecutionOrder int NOT NULL
);

INSERT @Definitions(Code,Name,Description,DefaultExecutionOrder)
VALUES
    (N'BusinessPartnerProposal', N'Propuestas de socios de negocio', N'Contrato hijo a padre reservado para propuestas de socios; permanece inactivo hasta disponer de productor y aplicador.', 195),
    (N'BusinessPartnerProposalResult', N'Resultados de propuestas de socios de negocio', N'Contrato padre a hijo exacto reservado para rechazos y conflictos; permanece inactivo hasta disponer de productor y aplicador.', 205);

UPDATE target
SET Name=source.Name,
    Description=source.Description,
    DefaultExecutionOrder=source.DefaultExecutionOrder,
    SupportsIncremental=1,
    SupportsInsert=0,
    SupportsUpdate=0,
    SupportsDeactivate=0,
    DefaultKeyField=N'GlobalId',
    DefaultModifiedAtField=NULL,
    IsSystem=1,
    IsActive=0,
    IsDeleted=0,
    DeletedByUserId=NULL,
    DeletedByUserName=NULL,
    DeletedAt=NULL,
    UpdatedByUserName=N'Sistema',
    UpdatedAt=SYSUTCDATETIME()
FROM dbo.SyncEntityDefinitions target
INNER JOIN @Definitions source ON source.Code=target.Code;

INSERT dbo.SyncEntityDefinitions
(
    Code,Name,Description,DefaultExecutionOrder,SupportsIncremental,
    SupportsInsert,SupportsUpdate,SupportsDeactivate,DefaultKeyField,
    DefaultModifiedAtField,IsSystem,IsActive,CreatedByUserName
)
SELECT source.Code,source.Name,source.Description,source.DefaultExecutionOrder,1,
       0,0,0,N'GlobalId',NULL,1,0,N'Sistema'
FROM @Definitions source
WHERE NOT EXISTS
(
    SELECT 1 FROM dbo.SyncEntityDefinitions target WHERE target.Code=source.Code
);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNERSAPCODEPOLICY_BUSCARPOREMPRESAID
    @CompanyId int
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.Companies company
        WHERE company.Id=@CompanyId
          AND company.IsActive=1
          AND company.IsMaster=1
          AND company.IsDeleted=0
    )
        THROW 52229, 'BusinessPartner SAP code policy requires an active central company.', 1;

    SELECT
        policy.CompanyId,
        policy.IsEnabled,
        policy.PrefixMode,
        policy.PassportIdentificationTypeCode,
        policy.UpdatedByUserId,
        policy.UpdatedByUserName,
        policy.UpdatedAt,
        policy.RowVersion
    FROM dbo.BusinessPartnerSapCodePolicies policy
    INNER JOIN dbo.Companies company
      ON company.Id=policy.CompanyId
     AND company.IsActive=1
     AND company.IsMaster=1
     AND company.IsDeleted=0
    WHERE policy.CompanyId=@CompanyId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_BUSINESSPARTNERSAPCODEPOLICY_GUARDAR
    @CompanyId int,
    @IsEnabled bit,
    @PrefixMode varchar(20),
    @PassportIdentificationTypeCode nvarchar(30),
    @ExpectedRowVersion varbinary(8)=NULL,
    @UpdatedByUserId int=NULL,
    @UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @PrefixMode=LTRIM(RTRIM(@PrefixMode));
    SET @PassportIdentificationTypeCode=LTRIM(RTRIM(@PassportIdentificationTypeCode));

    IF @PrefixMode NOT IN ('NationalForeign','RoleOnly')
        THROW 52230, 'Unsupported BusinessPartner SAP code prefix mode.', 1;
    IF NULLIF(@PassportIdentificationTypeCode,N'') IS NULL OR LEN(@PassportIdentificationTypeCode)>30
        THROW 52231, 'Passport identification type code is required and cannot exceed 30 characters.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.Companies company WITH (UPDLOCK,HOLDLOCK)
            WHERE company.Id=@CompanyId
              AND company.IsActive=1
              AND company.IsMaster=1
              AND company.IsDeleted=0
        )
            THROW 52229, 'BusinessPartner SAP code policy requires an active central company.', 1;

        DECLARE @CurrentRowVersion varbinary(8);
        DECLARE @OldValue nvarchar(max);
        SELECT
            @CurrentRowVersion=policy.RowVersion,
            @OldValue=(
                SELECT policy.IsEnabled,policy.PrefixMode,policy.PassportIdentificationTypeCode
                FOR JSON PATH,WITHOUT_ARRAY_WRAPPER
            )
        FROM dbo.BusinessPartnerSapCodePolicies policy WITH (UPDLOCK,HOLDLOCK)
        WHERE policy.CompanyId=@CompanyId;

        IF @CurrentRowVersion IS NULL
        BEGIN
            IF @ExpectedRowVersion IS NOT NULL
                THROW 52232, 'BusinessPartner SAP code policy concurrency conflict.', 1;

            INSERT dbo.BusinessPartnerSapCodePolicies
                (CompanyId,IsEnabled,PrefixMode,PassportIdentificationTypeCode,UpdatedByUserId,UpdatedByUserName)
            VALUES
                (@CompanyId,@IsEnabled,@PrefixMode,@PassportIdentificationTypeCode,@UpdatedByUserId,@UpdatedByUserName);
        END
        ELSE
        BEGIN
            IF @ExpectedRowVersion IS NULL OR @ExpectedRowVersion<>@CurrentRowVersion
                THROW 52232, 'BusinessPartner SAP code policy concurrency conflict.', 1;

            UPDATE dbo.BusinessPartnerSapCodePolicies
            SET IsEnabled=@IsEnabled,
                PrefixMode=@PrefixMode,
                PassportIdentificationTypeCode=@PassportIdentificationTypeCode,
                UpdatedByUserId=@UpdatedByUserId,
                UpdatedByUserName=@UpdatedByUserName,
                UpdatedAt=SYSUTCDATETIME()
            WHERE CompanyId=@CompanyId AND RowVersion=@ExpectedRowVersion;

            IF @@ROWCOUNT<>1
                THROW 52232, 'BusinessPartner SAP code policy concurrency conflict.', 1;
        END;

        DECLARE @NewValue nvarchar(max)=(
            SELECT policy.IsEnabled,policy.PrefixMode,policy.PassportIdentificationTypeCode
            FROM dbo.BusinessPartnerSapCodePolicies policy
            WHERE policy.CompanyId=@CompanyId
            FOR JSON PATH,WITHOUT_ARRAY_WRAPPER
        );

        INSERT dbo.AuditSyncConfigurationChanges
            (EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName,[Source])
        VALUES
            (N'BusinessPartnerSapCodePolicies',CONVERT(nvarchar(80),@CompanyId),
             CASE WHEN @CurrentRowVersion IS NULL THEN N'Create' ELSE N'Update' END,
             N'Policy',@OldValue,@NewValue,@UpdatedByUserId,@UpdatedByUserName,N'API');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    SELECT
        policy.CompanyId,
        policy.IsEnabled,
        policy.PrefixMode,
        policy.PassportIdentificationTypeCode,
        policy.UpdatedByUserId,
        policy.UpdatedByUserName,
        policy.UpdatedAt,
        policy.RowVersion
    FROM dbo.BusinessPartnerSapCodePolicies policy
    WHERE policy.CompanyId=@CompanyId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SYNCPROFILECREAR
    @CompanyId int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500)=NULL,
    @Direction nvarchar(30),
    @ExecutionMode nvarchar(20),
    @ConflictStrategy nvarchar(30),
    @BatchSize int,
    @MaxRetries int,
    @RetryDelaySeconds int,
    @TimeoutMinutes int,
    @IsActive bit,
    @AuditUserId int=NULL,
    @AuditUserName nvarchar(120)=NULL,
    @BranchesJson nvarchar(max)=N'[]',
    @EntitiesJson nvarchar(max)=N'[]',
    @EntityBranchesJson nvarchar(max)=N'[]',
    @ScheduleJson nvarchar(max)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.Companies company
        WHERE company.Id=@CompanyId
          AND company.IsActive=1
          AND company.IsMaster=1
          AND company.SyncEnabled=1
          AND company.IsDeleted=0
    )
        THROW 51000, 'La empresa central no existe o no tiene sincronizacion habilitada.', 1;
    IF EXISTS (SELECT 1 FROM dbo.SyncProfiles WHERE CompanyId=@CompanyId AND Code=@Code AND IsDeleted=0)
        THROW 51001, 'Ya existe un perfil de sincronizacion con el mismo codigo para la empresa.', 1;
    IF @Direction NOT IN (N'MasterToBranch',N'BranchToMaster')
       OR @ExecutionMode NOT IN (N'Incremental',N'Full',N'Manual')
       OR (@Direction=N'MasterToBranch' AND @ConflictStrategy<>N'MasterWins')
       OR (@Direction=N'BranchToMaster' AND (@ConflictStrategy<>N'CentralReview' OR @ExecutionMode<>N'Incremental'))
        THROW 51002, 'Valores de perfil no soportados.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.SyncProfiles
        (
            CompanyId,Code,Name,Description,Direction,ExecutionMode,ConflictStrategy,
            BatchSize,MaxRetries,RetryDelaySeconds,TimeoutMinutes,IsActive,
            CreatedByUserId,CreatedByUserName
        )
        VALUES
        (
            @CompanyId,@Code,@Name,@Description,@Direction,@ExecutionMode,@ConflictStrategy,
            @BatchSize,@MaxRetries,@RetryDelaySeconds,@TimeoutMinutes,@IsActive,
            @AuditUserId,@AuditUserName
        );

        DECLARE @ProfileId int=CONVERT(int,SCOPE_IDENTITY());
        EXEC dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR
            @Id=@ProfileId,@CompanyId=@CompanyId,@Code=@Code,@Name=@Name,
            @Description=@Description,@Direction=@Direction,@ExecutionMode=@ExecutionMode,
            @ConflictStrategy=@ConflictStrategy,@BatchSize=@BatchSize,@MaxRetries=@MaxRetries,
            @RetryDelaySeconds=@RetryDelaySeconds,@TimeoutMinutes=@TimeoutMinutes,
            @IsActive=@IsActive,@AuditUserId=@AuditUserId,@AuditUserName=@AuditUserName,
            @BranchesJson=@BranchesJson,@EntitiesJson=@EntitiesJson,
            @EntityBranchesJson=@EntityBranchesJson,@ScheduleJson=@ScheduleJson,
            @SuppressResult=1;

        COMMIT TRANSACTION;
        SELECT @ProfileId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SYNCPROFILEACTIVAR
    @Id int,
    @IsActive bit,
    @UpdatedByUserId int=NULL,
    @UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @IsActive=1 AND EXISTS
    (
        SELECT 1
        FROM dbo.SyncProfiles profile
        INNER JOIN dbo.SyncProfileEntities entity
          ON entity.SyncProfileId=profile.Id
         AND entity.EntityCode=N'BusinessPartnerProposal'
         AND entity.IsActive=1
         AND entity.IsDeleted=0
        WHERE profile.Id=@Id
          AND profile.Direction=N'BranchToMaster'
          AND profile.IsDeleted=0
          AND NOT EXISTS
          (
              SELECT 1 FROM dbo.BusinessPartnerSapCodePolicies policy
              WHERE policy.CompanyId=profile.CompanyId AND policy.IsEnabled=1
          )
    )
        THROW 52233, 'An enabled central SAP code policy is required for BusinessPartner proposals.', 1;

    UPDATE dbo.SyncProfiles
    SET IsActive=@IsActive,
        UpdatedByUserId=@UpdatedByUserId,
        UpdatedByUserName=@UpdatedByUserName,
        UpdatedAt=SYSUTCDATETIME()
    WHERE Id=@Id AND IsDeleted=0;

    DECLARE @Affected int=@@ROWCOUNT;
    IF @Affected>0 AND @IsActive=0
    BEGIN
        UPDATE dbo.SyncSchedules
        SET IsActive=0,
            UpdatedByUserId=@UpdatedByUserId,
            UpdatedByUserName=@UpdatedByUserName,
            UpdatedAt=SYSUTCDATETIME()
        WHERE SyncProfileId=@Id AND IsDeleted=0 AND IsActive=1;
    END;

    SELECT @Affected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCROUTINGTARGETS
    @SourceCompanyId int,
    @EntityCode nvarchar(80),
    @SyncProfileId int=NULL,
    @TargetBranchCode nvarchar(50)=NULL,
    @RequireTargetBranchMatch bit=0,
    @TargetCompanyId int=NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedEntityCode nvarchar(80)=LTRIM(RTRIM(@EntityCode));
    DECLARE @NormalizedTargetBranchCode nvarchar(50)=NULLIF(LTRIM(RTRIM(@TargetBranchCode)),N'');

    SELECT DISTINCT
        profile.Id AS SyncProfileId,
        entity.Id AS SyncProfileEntityId,
        profile.Code AS SyncProfileCode,
        profile.CompanyId AS SourceCompanyId,
        branchCompany.Id AS BranchCompanyId,
        entity.EntityCode,
        COALESCE(matrix.BatchSize,entity.BatchSize,profileBranch.BatchSize,profile.BatchSize) AS BatchSize,
        COALESCE(profileBranch.MaxRetries,profile.MaxRetries) AS MaxRetries,
        profile.RetryDelaySeconds,
        profile.TimeoutMinutes,
        entity.AllowInsert,
        entity.AllowUpdate,
        entity.AllowDeactivate,
        entity.ContinueOnError
    FROM dbo.SyncProfiles profile
    INNER JOIN dbo.Companies sourceCompany
      ON sourceCompany.Id=profile.CompanyId
     AND sourceCompany.IsActive=1
     AND sourceCompany.IsMaster=1
     AND sourceCompany.SyncEnabled=1
     AND sourceCompany.IsDeleted=0
    INNER JOIN dbo.SyncProfileEntities entity
      ON entity.SyncProfileId=profile.Id
     AND entity.IsDeleted=0
     AND entity.IsActive=1
     AND entity.EntityCode=@NormalizedEntityCode
     AND (@SyncProfileId IS NOT NULL OR entity.SyncMode=N'Incremental')
    INNER JOIN dbo.SyncProfileEntityBranches matrix
      ON matrix.SyncProfileId=profile.Id
     AND matrix.SyncProfileEntityId=entity.Id
     AND matrix.IsDeleted=0
     AND matrix.IsEnabled=1
    INNER JOIN dbo.SyncProfileBranches profileBranch
      ON profileBranch.Id=matrix.SyncProfileBranchId
     AND profileBranch.SyncProfileId=profile.Id
     AND profileBranch.IsDeleted=0
     AND profileBranch.IsActive=1
    INNER JOIN dbo.Companies branchCompany
      ON branchCompany.Id=profileBranch.BranchCompanyId
     AND branchCompany.IsActive=1
     AND branchCompany.IsMaster=0
     AND branchCompany.SyncEnabled=1
     AND branchCompany.ParentCompanyId=profile.CompanyId
     AND branchCompany.IsDeleted=0
    WHERE profile.CompanyId=@SourceCompanyId
      AND profile.IsDeleted=0
      AND profile.IsActive=1
      AND profile.Direction=N'MasterToBranch'
      AND profile.ConflictStrategy=N'MasterWins'
      AND @NormalizedEntityCode<>N'BusinessPartnerProposal'
      AND
      (
          @NormalizedEntityCode<>N'BusinessPartnerProposalResult'
          OR
          (
              @NormalizedEntityCode=N'BusinessPartnerProposalResult'
              AND @TargetCompanyId IS NOT NULL
              AND branchCompany.Id=@TargetCompanyId
          )
      )
      AND (@TargetCompanyId IS NULL OR branchCompany.Id=@TargetCompanyId)
      AND
      (
          @RequireTargetBranchMatch=0
          OR (@NormalizedTargetBranchCode IS NOT NULL AND branchCompany.BranchCode=@NormalizedTargetBranchCode)
      )
      AND
      (
          (@SyncProfileId IS NULL AND profile.ExecutionMode=N'Incremental')
          OR (@SyncProfileId IS NOT NULL AND profile.Id=@SyncProfileId AND profile.ExecutionMode IN (N'Incremental',N'Full',N'Manual'))
      )

    UNION ALL

    SELECT DISTINCT
        profile.Id AS SyncProfileId,
        entity.Id AS SyncProfileEntityId,
        profile.Code AS SyncProfileCode,
        @SourceCompanyId AS SourceCompanyId,
        profile.CompanyId AS BranchCompanyId,
        entity.EntityCode,
        COALESCE(matrix.BatchSize,entity.BatchSize,profileBranch.BatchSize,profile.BatchSize) AS BatchSize,
        COALESCE(profileBranch.MaxRetries,profile.MaxRetries) AS MaxRetries,
        profile.RetryDelaySeconds,
        profile.TimeoutMinutes,
        entity.AllowInsert,
        entity.AllowUpdate,
        entity.AllowDeactivate,
        entity.ContinueOnError
    FROM dbo.SyncProfiles profile
    INNER JOIN dbo.Companies centralCompany
      ON centralCompany.Id=profile.CompanyId
     AND centralCompany.IsActive=1
     AND centralCompany.IsMaster=1
     AND centralCompany.SyncEnabled=1
     AND centralCompany.IsDeleted=0
    INNER JOIN dbo.SyncProfileEntities entity
      ON entity.SyncProfileId=profile.Id
     AND entity.IsDeleted=0
     AND entity.IsActive=1
     AND entity.EntityCode=@NormalizedEntityCode
     AND (@SyncProfileId IS NOT NULL OR entity.SyncMode=N'Incremental')
    INNER JOIN dbo.SyncProfileEntityBranches matrix
      ON matrix.SyncProfileId=profile.Id
     AND matrix.SyncProfileEntityId=entity.Id
     AND matrix.IsDeleted=0
     AND matrix.IsEnabled=1
    INNER JOIN dbo.SyncProfileBranches profileBranch
      ON profileBranch.Id=matrix.SyncProfileBranchId
     AND profileBranch.SyncProfileId=profile.Id
     AND profileBranch.BranchCompanyId=@SourceCompanyId
     AND profileBranch.IsDeleted=0
     AND profileBranch.IsActive=1
    INNER JOIN dbo.Companies sourceCompany
      ON sourceCompany.Id=profileBranch.BranchCompanyId
     AND sourceCompany.IsActive=1
     AND sourceCompany.IsMaster=0
     AND sourceCompany.SyncEnabled=1
     AND sourceCompany.ParentCompanyId=profile.CompanyId
     AND sourceCompany.IsDeleted=0
    WHERE profile.IsDeleted=0
      AND profile.IsActive=1
      AND profile.Direction=N'BranchToMaster'
      AND profile.ConflictStrategy=N'CentralReview'
      AND @NormalizedEntityCode=N'BusinessPartnerProposal'
      AND (@TargetCompanyId IS NULL OR profile.CompanyId=@TargetCompanyId)
      AND
      (
          @RequireTargetBranchMatch=0
          OR (@NormalizedTargetBranchCode IS NOT NULL AND sourceCompany.BranchCode=@NormalizedTargetBranchCode)
      )
      AND
      (
          (@SyncProfileId IS NULL AND profile.ExecutionMode=N'Incremental')
          OR (@SyncProfileId IS NOT NULL AND profile.Id=@SyncProfileId AND profile.ExecutionMode IN (N'Incremental',N'Full',N'Manual'))
      )
    ORDER BY BranchCompanyId,SyncProfileId,SyncProfileEntityId;
END;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @SyncModuleId int=(SELECT TOP(1) Id FROM dbo.Modules WHERE Code=N'SYNC');
    DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
    IF @SyncModuleId IS NULL OR @AdminRoleId IS NULL
        THROW 52229, 'SYNC module and ADMIN role are required before migration 229.', 1;

    DECLARE @Permissions table(Code nvarchar(120) PRIMARY KEY,Name nvarchar(160),Description nvarchar(300));
    INSERT @Permissions(Code,Name,Description)
    VALUES
        (N'SYNC.BUSINESS_PARTNER_CONFLICTS.VIEW',N'Ver conflictos de socios',N'Consultar conflictos de sincronizacion de socios de negocio.'),
        (N'SYNC.BUSINESS_PARTNER_CONFLICTS.RESOLVE',N'Resolver conflictos de socios',N'Resolver conflictos de sincronizacion de socios de negocio con motivo y auditoria.');

    UPDATE target
    SET ModuleId=@SyncModuleId,
        Name=source.Name,
        Description=source.Description,
        IsActive=1,
        UpdatedAt=SYSUTCDATETIME()
    FROM dbo.Permissions target
    INNER JOIN @Permissions source ON source.Code=target.Code;

    INSERT dbo.Permissions(ModuleId,Code,Name,Description)
    SELECT @SyncModuleId,source.Code,source.Name,source.Description
    FROM @Permissions source
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Permissions target WHERE target.Code=source.Code);

    INSERT dbo.RolePermissions(RoleId,PermissionId)
    SELECT @AdminRoleId,permission.Id
    FROM dbo.Permissions permission
    INNER JOIN @Permissions source ON source.Code=permission.Code
    WHERE NOT EXISTS
    (
        SELECT 1 FROM dbo.RolePermissions existing
        WHERE existing.RoleId=@AdminRoleId AND existing.PermissionId=permission.Id
    );

    IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260903.229')
        INSERT dbo.MasterSchemaHistory(Version,Description)
        VALUES(N'20260903.229',N'Gobierno bidireccional cerrado de socios de negocio en Master');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
