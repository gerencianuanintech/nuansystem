/*
    Countries: transactional LocalOutbox, external references and terminal apply.
    Target: tenant databases. Prerequisites: 065, 083 and 164.
    This script does not enable profiles, relays or workers.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Countries', N'U') IS NULL
    THROW 51168, 'Countries is required before migration 168.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL
    THROW 51168, 'LocalOutbox is required before migration 168.', 1;
IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL
    THROW 51168, 'SyncInbox is required before migration 168.', 1;
IF OBJECT_ID(N'dbo.SyncAudit', N'U') IS NULL
    THROW 51168, 'SyncAudit is required before migration 168.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51168, 'SchemaHistory is required before migration 168.', 1;
GO

IF COL_LENGTH(N'dbo.Countries', N'ExternalSystem') IS NULL
    ALTER TABLE dbo.Countries ADD ExternalSystem nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.Countries', N'ExternalCode') IS NULL
    ALTER TABLE dbo.Countries ADD ExternalCode nvarchar(100) NULL;
GO

IF EXISTS (SELECT Code FROM dbo.Countries GROUP BY Code HAVING COUNT_BIG(1) > 1)
    THROW 51168, 'Country codes, including tombstones, must be unique before migration 168.', 1;
GO

IF EXISTS
(
    SELECT ExternalSystem, ExternalCode
    FROM dbo.Countries
    WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL
    GROUP BY ExternalSystem, ExternalCode
    HAVING COUNT_BIG(1) > 1
)
    THROW 51168, 'Country external references must be unique before migration 168.', 1;
GO

IF EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Countries')
      AND name = N'UX_Countries_Code'
      AND (is_unique = 0 OR filter_definition IS NOT NULL)
)
    DROP INDEX UX_Countries_Code ON dbo.Countries;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Countries')
      AND name = N'UX_Countries_Code'
      AND is_unique = 1
      AND filter_definition IS NULL
)
    CREATE UNIQUE INDEX UX_Countries_Code ON dbo.Countries(Code);
GO

IF EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Countries')
      AND name = N'IX_Countries_ExternalRef'
)
    DROP INDEX IX_Countries_ExternalRef ON dbo.Countries;
GO

IF EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Countries')
      AND name = N'IX_Countries_ExternalReference'
)
    DROP INDEX IX_Countries_ExternalReference ON dbo.Countries;
GO

IF EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Countries')
      AND name = N'UX_Countries_ExternalRef'
      AND (is_unique = 0 OR filter_definition IS NULL)
)
    DROP INDEX UX_Countries_ExternalRef ON dbo.Countries;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Countries')
      AND name = N'UX_Countries_ExternalRef'
      AND is_unique = 1
      AND filter_definition IS NOT NULL
)
    CREATE UNIQUE INDEX UX_Countries_ExternalRef
        ON dbo.Countries(ExternalSystem, ExternalCode)
        WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CountryId AS Id, GlobalId, Code, Name, Iso2, Iso3, PhonePrefix,
           ExternalSystem, ExternalCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Countries WHERE IsDeleted = 0 ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_BUSCARPORID @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CountryId AS Id, GlobalId, Code, Name, Iso2, Iso3, PhonePrefix,
           ExternalSystem, ExternalCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Countries WHERE CountryId = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_BUSCARPORCODIGO
    @Code nvarchar(10), @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM dbo.Countries
    WHERE Code = @Code AND (@ExcluirId IS NULL OR CountryId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_COUNTRIES_CREAR
    @Id int = NULL, @GlobalId uniqueidentifier, @Code nvarchar(10), @Name nvarchar(120),
    @Iso2 nvarchar(2) = NULL, @Iso3 nvarchar(3) = NULL, @PhonePrefix nvarchar(10) = NULL,
    @IsActive bit = 1, @AuditUserId int = NULL, @AuditUserName nvarchar(100) = NULL,
    @ExternalSystem nvarchar(50) = NULL, @ExternalCode nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.Countries
        (GlobalId, Code, Name, Iso2, Iso3, PhonePrefix, IsActive, ExternalSystem, ExternalCode,
         CreatedByUserId, CreatedByUserName)
    VALUES
        (@GlobalId, @Code, @Name, @Iso2, @Iso3, @PhonePrefix, @IsActive, @ExternalSystem, @ExternalCode,
         @AuditUserId, @AuditUserName);
    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_COUNTRIES_ACTUALIZAR
    @Id int, @GlobalId uniqueidentifier, @Code nvarchar(10), @Name nvarchar(120),
    @Iso2 nvarchar(2) = NULL, @Iso3 nvarchar(3) = NULL, @PhonePrefix nvarchar(10) = NULL,
    @IsActive bit = 1, @AuditUserId int = NULL, @AuditUserName nvarchar(100) = NULL,
    @ExternalSystem nvarchar(50) = NULL, @ExternalCode nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Countries
    SET Code = @Code, Name = @Name, Iso2 = @Iso2, Iso3 = @Iso3,
        PhonePrefix = @PhonePrefix, IsActive = @IsActive, ExternalSystem = @ExternalSystem,
        ExternalCode = @ExternalCode, UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName
    WHERE CountryId = @Id AND IsDeleted = 0;
    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_COUNTRY_SYNC_APPLY_EVENT
    @EventId uniqueidentifier, @SourceCompanyId int, @EntityName nvarchar(80),
    @EntityGlobalId uniqueidentifier, @Operation nvarchar(30), @PayloadJson nvarchar(max),
    @GlobalId uniqueidentifier, @Code nvarchar(10), @Name nvarchar(120),
    @Iso2 nvarchar(2) = NULL, @Iso3 nvarchar(3) = NULL, @PhonePrefix nvarchar(10) = NULL,
    @IsActive bit, @IsDeleted bit, @ExternalSystem nvarchar(50) = NULL,
    @ExternalCode nvarchar(100) = NULL, @CreatedAt datetime2(0), @UpdatedAt datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @InboxId bigint, @InboxStatus nvarchar(30), @CountryId int;

        SELECT @InboxId = Id, @InboxStatus = Status
        FROM dbo.SyncInbox WITH (UPDLOCK, HOLDLOCK) WHERE EventId = @EventId;

        IF @InboxStatus = N'Applied'
        BEGIN
            SELECT @CountryId = CountryId FROM dbo.Countries WHERE GlobalId = @GlobalId;
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode, @CountryId AS CountryId;
            RETURN;
        END;

        IF @InboxStatus = N'DeadLetter'
        BEGIN
            COMMIT TRANSACTION;
            SELECT -2 AS ResultCode, CONVERT(int, NULL) AS CountryId;
            RETURN;
        END;

        IF @InboxId IS NULL
        BEGIN
            INSERT dbo.SyncInbox
                (EventId, SourceCompanyId, EntityName, EntityGlobalId, Operation, PayloadJson, Status)
            VALUES
                (@EventId, @SourceCompanyId, @EntityName, @EntityGlobalId, @Operation, @PayloadJson, N'Pending');
            SET @InboxId = CONVERT(bigint, SCOPE_IDENTITY());
        END;

        IF @EntityName <> N'Countries' OR @EntityGlobalId <> @GlobalId
            THROW 51168, 'Country event identity contract is invalid.', 1;

        IF EXISTS
        (
            SELECT 1 FROM dbo.Countries WITH (UPDLOCK, HOLDLOCK)
            WHERE Code = @Code AND GlobalId <> @GlobalId
        )
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status = N'DeadLetter', AttemptCount = AttemptCount + 1,
                ErrorMessage = N'Country code belongs to another GlobalId.',
                LastErrorMessage = N'Country code belongs to another GlobalId.', NextRetryAt = NULL
            WHERE Id = @InboxId;

            INSERT dbo.SyncAudit
                (CompanyId, EventId, EntityName, EntityGlobalId, [Action], PreviousStatus,
                 NewStatus, [Message], ErrorCode, CreatedBy)
            VALUES
                (@SourceCompanyId, @EventId, N'Countries', @GlobalId, N'DeadLetter',
                 @InboxStatus, N'DeadLetter', N'Country code conflict; no automatic adoption.',
                 N'SYNC_COUNTRY_CODE_CONFLICT', N'MasterBranchSyncWorker');

            COMMIT TRANSACTION;
            SELECT -2 AS ResultCode, CONVERT(int, NULL) AS CountryId;
            RETURN;
        END;

        IF @ExternalSystem IS NOT NULL AND @ExternalCode IS NOT NULL AND EXISTS
        (
            SELECT 1 FROM dbo.Countries WITH (UPDLOCK, HOLDLOCK)
            WHERE ExternalSystem = @ExternalSystem
              AND ExternalCode = @ExternalCode
              AND GlobalId <> @GlobalId
        )
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status = N'DeadLetter', AttemptCount = AttemptCount + 1,
                ErrorMessage = N'Country external reference belongs to another GlobalId.',
                LastErrorMessage = N'Country external reference belongs to another GlobalId.', NextRetryAt = NULL
            WHERE Id = @InboxId;

            INSERT dbo.SyncAudit
                (CompanyId, EventId, EntityName, EntityGlobalId, [Action], PreviousStatus,
                 NewStatus, [Message], ErrorCode, CreatedBy)
            VALUES
                (@SourceCompanyId, @EventId, N'Countries', @GlobalId, N'DeadLetter',
                 @InboxStatus, N'DeadLetter', N'Country external reference conflict.',
                 N'SYNC_COUNTRY_EXTERNAL_CONFLICT', N'MasterBranchSyncWorker');

            COMMIT TRANSACTION;
            SELECT -3 AS ResultCode, CONVERT(int, NULL) AS CountryId;
            RETURN;
        END;

        SELECT @CountryId = CountryId FROM dbo.Countries WITH (UPDLOCK, HOLDLOCK)
        WHERE GlobalId = @GlobalId;

        IF @CountryId IS NULL
        BEGIN
            INSERT dbo.Countries
                (GlobalId, Code, Name, Iso2, Iso3, PhonePrefix, IsActive, IsDeleted,
                 ExternalSystem, ExternalCode, CreatedAt, CreatedByUserName)
            VALUES
                (@GlobalId, @Code, @Name, @Iso2, @Iso3, @PhonePrefix, @IsActive, @IsDeleted,
                 @ExternalSystem, @ExternalCode, COALESCE(@CreatedAt, SYSUTCDATETIME()), N'MasterBranchSyncWorker');
            SET @CountryId = CONVERT(int, SCOPE_IDENTITY());
        END
        ELSE
        BEGIN
            UPDATE dbo.Countries
            SET Code = @Code, Name = @Name, Iso2 = @Iso2, Iso3 = @Iso3,
                PhonePrefix = @PhonePrefix, IsActive = @IsActive, IsDeleted = @IsDeleted,
                ExternalSystem = @ExternalSystem, ExternalCode = @ExternalCode,
                UpdatedAt = COALESCE(@UpdatedAt, SYSUTCDATETIME()),
                UpdatedByUserName = N'MasterBranchSyncWorker'
            WHERE CountryId = @CountryId;
        END;

        UPDATE dbo.SyncInbox
        SET Status = N'Applied', AppliedAt = SYSUTCDATETIME(), ErrorMessage = NULL,
            LastErrorMessage = NULL, NextRetryAt = NULL
        WHERE Id = @InboxId;

        INSERT dbo.SyncAudit
            (CompanyId, EventId, EntityName, EntityGlobalId, [Action], PreviousStatus,
             NewStatus, [Message], CreatedBy)
        VALUES
            (@SourceCompanyId, @EventId, N'Countries', @GlobalId, N'Applied',
             COALESCE(@InboxStatus, N'Pending'), N'Applied',
             N'Country event applied by GlobalId.', N'MasterBranchSyncWorker');

        COMMIT TRANSACTION;
        SELECT 1 AS ResultCode, @CountryId AS CountryId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260804.168')
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES (N'20260804.168', N'Countries transactional outbox and terminal GlobalId apply');
END;
GO
