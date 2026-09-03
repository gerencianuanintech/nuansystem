/*
    BusinessPartner bidirectional tenant foundation.
    Forward-only and idempotent. Capabilities remain disabled by default.
    Prerequisites: tenant BusinessPartner schema, LocalOutbox, SyncInbox and SchemaHistory.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NULL
    THROW 52028, 'BusinessPartners is required before migration 228.', 1;
IF OBJECT_ID(N'dbo.BusinessPartnerAddresses', N'U') IS NULL
    THROW 52028, 'BusinessPartnerAddresses is required before migration 228.', 1;
IF OBJECT_ID(N'dbo.BusinessPartnerContacts', N'U') IS NULL
    THROW 52028, 'BusinessPartnerContacts is required before migration 228.', 1;
IF OBJECT_ID(N'dbo.BusinessPartnerSapMapping', N'U') IS NULL
    THROW 52028, 'BusinessPartnerSapMapping is required before migration 228.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL
    THROW 52028, 'LocalOutbox is required before migration 228.', 1;
IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL
    THROW 52028, 'SyncInbox is required before migration 228.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 52028, 'SchemaHistory is required before migration 228.', 1;
GO

IF COL_LENGTH(N'dbo.BusinessPartners', N'NormalizedIdentificationNumber') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD NormalizedIdentificationNumber nvarchar(50) NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartners', N'CanonicalVersion') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD CanonicalVersion bigint NOT NULL
        CONSTRAINT DF_BusinessPartners_CanonicalVersion DEFAULT (1);
GO

IF COL_LENGTH(N'dbo.BusinessPartners', N'MasterSyncStatus') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD MasterSyncStatus varchar(20) NOT NULL
        CONSTRAINT DF_BusinessPartners_MasterSyncStatus DEFAULT ('Accepted');
GO

IF COL_LENGTH(N'dbo.BusinessPartners', N'MasterSyncMessage') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD MasterSyncMessage nvarchar(500) NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartners', N'RowVersion') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD RowVersion rowversion NOT NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerAddresses', N'GlobalId') IS NULL
    ALTER TABLE dbo.BusinessPartnerAddresses ADD GlobalId uniqueidentifier NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'GlobalId') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD GlobalId uniqueidentifier NULL;
GO

IF COL_LENGTH(N'dbo.LocalOutbox', N'TargetCompanyId') IS NULL
    ALTER TABLE dbo.LocalOutbox ADD TargetCompanyId int NULL;
GO

IF COL_LENGTH(N'dbo.LocalOutbox', N'CausationEventId') IS NULL
    ALTER TABLE dbo.LocalOutbox ADD CausationEventId uniqueidentifier NULL;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS (SELECT 1 FROM dbo.BusinessPartners WHERE GlobalId IS NULL)
        THROW 52028, 'BusinessPartner rows without GlobalId require manual remediation.', 1;

    UPDATE dbo.BusinessPartners
    SET NormalizedIdentificationNumber = REPLACE(
        TRANSLATE(
            UPPER(LTRIM(RTRIM(IdentificationNumber))),
            N'.-' + NCHAR(9) + NCHAR(10) + NCHAR(11) + NCHAR(12) + NCHAR(13)
                + NCHAR(32) + NCHAR(133) + NCHAR(160) + NCHAR(5760)
                + NCHAR(8192) + NCHAR(8193) + NCHAR(8194) + NCHAR(8195)
                + NCHAR(8196) + NCHAR(8197) + NCHAR(8198) + NCHAR(8199)
                + NCHAR(8200) + NCHAR(8201) + NCHAR(8202) + NCHAR(8232)
                + NCHAR(8233) + NCHAR(8239) + NCHAR(8287) + NCHAR(12288),
            REPLICATE(N' ', 27)),
        N' ', N'');

    UPDATE dbo.BusinessPartnerAddresses
    SET GlobalId = NEWID()
    WHERE GlobalId IS NULL;

    UPDATE dbo.BusinessPartnerContacts
    SET GlobalId = NEWID()
    WHERE GlobalId IS NULL;

    UPDATE dbo.BusinessPartners
    SET MasterSyncStatus = 'LegacyReview',
        MasterSyncMessage = N'Legacy Both role requires manual review.'
    WHERE PartnerType = N'Both'
      AND MasterSyncStatus <> 'LegacyReview';

    IF EXISTS
    (
        SELECT 1
        FROM dbo.BusinessPartners
        WHERE NULLIF(NormalizedIdentificationNumber, N'') IS NULL
    )
        THROW 52028, 'Normalized BusinessPartner identification cannot be empty.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.BusinessPartners
        WHERE IsDeleted = 0 AND IsActive = 1
        GROUP BY PartnerType, IdentificationTypeId, NormalizedIdentificationNumber
        HAVING COUNT_BIG(1) > 1
    )
        THROW 52028, 'Duplicate normalized BusinessPartner identification exists within the same role.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.BusinessPartnerSapMapping
        WHERE NULLIF(LTRIM(RTRIM(SapCardCode)), N'') IS NOT NULL
        GROUP BY SapCardCode
        HAVING COUNT_BIG(1) > 1
    )
        THROW 52028, 'Duplicate nonblank SapCardCode exists.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.BusinessPartnerSapMapping
        WHERE LEN(SapCardCode) > 15
    )
        THROW 52028, 'SapCardCode longer than 15 characters requires manual remediation.', 1;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND name = N'NormalizedIdentificationNumber'
      AND is_nullable = 1
)
    ALTER TABLE dbo.BusinessPartners ALTER COLUMN NormalizedIdentificationNumber nvarchar(50) NOT NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerAddresses')
      AND name = N'GlobalId'
      AND is_nullable = 1
)
    ALTER TABLE dbo.BusinessPartnerAddresses ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerContacts')
      AND name = N'GlobalId'
      AND is_nullable = 1
)
    ALTER TABLE dbo.BusinessPartnerContacts ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
GO

IF OBJECT_ID(N'dbo.DF_BusinessPartnerAddresses_GlobalId', N'D') IS NULL
    ALTER TABLE dbo.BusinessPartnerAddresses ADD CONSTRAINT DF_BusinessPartnerAddresses_GlobalId DEFAULT NEWID() FOR GlobalId;
GO

IF OBJECT_ID(N'dbo.DF_BusinessPartnerContacts_GlobalId', N'D') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD CONSTRAINT DF_BusinessPartnerContacts_GlobalId DEFAULT NEWID() FOR GlobalId;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND name = N'CK_BusinessPartners_PartnerType'
)
    ALTER TABLE dbo.BusinessPartners WITH CHECK
    ADD CONSTRAINT CK_BusinessPartners_PartnerType
    CHECK (PartnerType IN (N'Customer', N'Supplier', N'Both'));
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND name = N'CK_BusinessPartners_MasterSyncStatus'
)
    ALTER TABLE dbo.BusinessPartners WITH CHECK
    ADD CONSTRAINT CK_BusinessPartners_MasterSyncStatus
    CHECK (MasterSyncStatus IN ('PendingMaster', 'Accepted', 'Rejected', 'Conflict', 'LegacyReview'));
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND name = N'CK_BusinessPartners_CanonicalVersion'
)
    ALTER TABLE dbo.BusinessPartners WITH CHECK
    ADD CONSTRAINT CK_BusinessPartners_CanonicalVersion CHECK (CanonicalVersion >= 0);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND name = N'CK_BusinessPartners_NormalizedIdentificationNumber'
)
    ALTER TABLE dbo.BusinessPartners WITH CHECK
    ADD CONSTRAINT CK_BusinessPartners_NormalizedIdentificationNumber
    CHECK (NULLIF(NormalizedIdentificationNumber, N'') IS NOT NULL);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.BusinessPartnerSapMapping')
      AND name = N'CK_BusinessPartnerSapMapping_SapCardCodeLength'
)
    ALTER TABLE dbo.BusinessPartnerSapMapping WITH CHECK
    ADD CONSTRAINT CK_BusinessPartnerSapMapping_SapCardCodeLength
    CHECK (SapCardCode IS NULL OR LEN(SapCardCode) <= 15);
GO

IF EXISTS
(
    SELECT 1
    FROM sys.indexes AS indexItem
    WHERE indexItem.object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND indexItem.name = N'UX_BusinessPartners_Identification_Active'
      AND NOT EXISTS
      (
          SELECT 1
          FROM sys.index_columns AS indexColumn
          INNER JOIN sys.columns AS columnItem
              ON columnItem.object_id = indexColumn.object_id
             AND columnItem.column_id = indexColumn.column_id
          WHERE indexColumn.object_id = indexItem.object_id
            AND indexColumn.index_id = indexItem.index_id
            AND columnItem.name = N'NormalizedIdentificationNumber'
      )
)
    DROP INDEX UX_BusinessPartners_Identification_Active ON dbo.BusinessPartners;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND name = N'UX_BusinessPartners_Identification_Active'
)
    CREATE UNIQUE INDEX UX_BusinessPartners_Identification_Active
    ON dbo.BusinessPartners (PartnerType, IdentificationTypeId, NormalizedIdentificationNumber)
    WHERE IsDeleted = 0 AND IsActive = 1;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerSapMapping')
      AND name = N'UX_BusinessPartners_SapCardCode_Active'
)
    CREATE UNIQUE INDEX UX_BusinessPartners_SapCardCode_Active
    ON dbo.BusinessPartnerSapMapping (SapCardCode)
    WHERE SapCardCode IS NOT NULL AND SapCardCode <> N'';
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerAddresses')
      AND name = N'UX_BusinessPartnerAddresses_GlobalId'
)
    CREATE UNIQUE INDEX UX_BusinessPartnerAddresses_GlobalId
    ON dbo.BusinessPartnerAddresses (GlobalId);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerContacts')
      AND name = N'UX_BusinessPartnerContacts_GlobalId'
)
    CREATE UNIQUE INDEX UX_BusinessPartnerContacts_GlobalId
    ON dbo.BusinessPartnerContacts (GlobalId);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.LocalOutbox')
      AND name = N'IX_LocalOutbox_TargetCompany_Status'
)
    CREATE INDEX IX_LocalOutbox_TargetCompany_Status
    ON dbo.LocalOutbox (TargetCompanyId, Status, CreatedAt)
    INCLUDE (EventId, EntityName, EntityGlobalId, CausationEventId);
GO

IF OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerSyncConflicts
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartnerSyncConflicts PRIMARY KEY,
        ProposalEventId uniqueidentifier NOT NULL,
        BusinessPartnerId int NULL,
        BusinessPartnerGlobalId uniqueidentifier NOT NULL,
        OriginCompanyId int NOT NULL,
        BaseCanonicalVersion bigint NOT NULL,
        CurrentCanonicalVersion bigint NOT NULL,
        BaseSnapshotJson nvarchar(max) NULL,
        ProposedSnapshotJson nvarchar(max) NOT NULL,
        CanonicalSnapshotJson nvarchar(max) NOT NULL,
        ConflictFieldsJson nvarchar(max) NOT NULL,
        Status varchar(20) NOT NULL CONSTRAINT DF_BusinessPartnerSyncConflicts_Status DEFAULT ('Open'),
        Resolution varchar(20) NULL,
        ResolutionReason nvarchar(500) NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerSyncConflicts_CreatedAt DEFAULT SYSUTCDATETIME(),
        ResolvedByUserId int NULL,
        ResolvedByUserName nvarchar(120) NULL,
        ResolvedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_BusinessPartnerSyncConflicts_BusinessPartners
            FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id),
        CONSTRAINT CK_BusinessPartnerSyncConflicts_Versions
            CHECK (BaseCanonicalVersion >= 0 AND CurrentCanonicalVersion >= 0),
        CONSTRAINT CK_BusinessPartnerSyncConflicts_Status
            CHECK (Status IN ('Open', 'Resolved')),
        CONSTRAINT CK_BusinessPartnerSyncConflicts_Resolution
            CHECK (Resolution IS NULL OR Resolution IN ('AcceptBranch', 'KeepCentral')),
        CONSTRAINT CK_BusinessPartnerSyncConflicts_ResolutionState
            CHECK
            (
                (Status = 'Open' AND Resolution IS NULL AND ResolvedAt IS NULL)
                OR
                (Status = 'Resolved' AND Resolution IS NOT NULL
                 AND NULLIF(LTRIM(RTRIM(ResolutionReason)), N'') IS NOT NULL
                 AND ResolvedAt IS NOT NULL)
            ),
        CONSTRAINT CK_BusinessPartnerSyncConflicts_BaseSnapshotJson
            CHECK (BaseSnapshotJson IS NULL OR ISJSON(BaseSnapshotJson) = 1),
        CONSTRAINT CK_BusinessPartnerSyncConflicts_ProposedSnapshotJson
            CHECK (ISJSON(ProposedSnapshotJson) = 1),
        CONSTRAINT CK_BusinessPartnerSyncConflicts_CanonicalSnapshotJson
            CHECK (ISJSON(CanonicalSnapshotJson) = 1),
        CONSTRAINT CK_BusinessPartnerSyncConflicts_ConflictFieldsJson
            CHECK (ISJSON(ConflictFieldsJson) = 1)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND name = N'UX_BusinessPartnerSyncConflicts_ProposalEventId'
)
    CREATE UNIQUE INDEX UX_BusinessPartnerSyncConflicts_ProposalEventId
    ON dbo.BusinessPartnerSyncConflicts (ProposalEventId);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND name = N'IX_BusinessPartnerSyncConflicts_Status_CreatedAt'
)
    CREATE INDEX IX_BusinessPartnerSyncConflicts_Status_CreatedAt
    ON dbo.BusinessPartnerSyncConflicts (Status, CreatedAt DESC)
    INCLUDE (BusinessPartnerGlobalId, OriginCompanyId, CurrentCanonicalVersion);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260903.228')
BEGIN
    INSERT dbo.SchemaHistory (Version, Description)
    VALUES (N'20260903.228', N'BusinessPartner tenant bidirectional identity and conflict foundation');
END;
GO
