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
    ALTER TABLE dbo.BusinessPartners ADD NormalizedIdentificationNumber nvarchar(50) COLLATE Latin1_General_100_BIN2 NULL;
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

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND name = N'NormalizedIdentificationNumber'
      AND (system_type_id <> 231 OR max_length <> 100
           OR collation_name <> N'Latin1_General_100_BIN2')
)
    THROW 52028, 'BusinessPartners.NormalizedIdentificationNumber has an incompatible shape.', 1;
IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartners') AND name = N'CanonicalVersion'
      AND (system_type_id <> 127 OR max_length <> 8 OR is_nullable <> 0)
)
    THROW 52028, 'BusinessPartners.CanonicalVersion has an incompatible shape.', 1;
IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartners') AND name = N'MasterSyncStatus'
      AND (system_type_id <> 167 OR max_length <> 20 OR is_nullable <> 0)
)
    THROW 52028, 'BusinessPartners.MasterSyncStatus has an incompatible shape.', 1;
IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartners') AND name = N'MasterSyncMessage'
      AND (system_type_id <> 231 OR max_length <> 1000 OR is_nullable <> 1)
)
    THROW 52028, 'BusinessPartners.MasterSyncMessage has an incompatible shape.', 1;
IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartners') AND name = N'RowVersion'
      AND (system_type_id <> 189 OR max_length <> 8 OR is_nullable <> 0)
)
    THROW 52028, 'BusinessPartners.RowVersion has an incompatible shape.', 1;
IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerAddresses') AND name = N'GlobalId'
      AND (system_type_id <> 36 OR max_length <> 16)
)
    THROW 52028, 'BusinessPartnerAddresses.GlobalId has an incompatible shape.', 1;
IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerContacts') AND name = N'GlobalId'
      AND (system_type_id <> 36 OR max_length <> 16)
)
    THROW 52028, 'BusinessPartnerContacts.GlobalId has an incompatible shape.', 1;
IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.LocalOutbox') AND name = N'TargetCompanyId'
      AND (system_type_id <> 56 OR max_length <> 4 OR is_nullable <> 1)
)
    THROW 52028, 'LocalOutbox.TargetCompanyId has an incompatible shape.', 1;
IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.LocalOutbox') AND name = N'CausationEventId'
      AND (system_type_id <> 36 OR max_length <> 16 OR is_nullable <> 1)
)
    THROW 52028, 'LocalOutbox.CausationEventId has an incompatible shape.', 1;
GO

-- Fixed casing and binary comparison keep the result independent of tenant collation.
IF UPPER(NCHAR(233) COLLATE Latin1_General_100_BIN2) <> NCHAR(201) COLLATE Latin1_General_100_BIN2
   OR UPPER(NCHAR(304) COLLATE Latin1_General_100_BIN2) <> NCHAR(304) COLLATE Latin1_General_100_BIN2
   OR UPPER(NCHAR(101) COLLATE Latin1_General_100_BIN2) = UPPER(NCHAR(233) COLLATE Latin1_General_100_BIN2)
    THROW 52028, 'Required invariant normalization collation behavior is unavailable.', 1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS (SELECT 1 FROM dbo.BusinessPartners WHERE GlobalId IS NULL)
        THROW 52028, 'BusinessPartner rows without GlobalId require manual remediation.', 1;

    UPDATE dbo.BusinessPartners
    SET NormalizedIdentificationNumber = REPLACE(
        TRANSLATE(
            UPPER(LTRIM(RTRIM(IdentificationNumber)) COLLATE Latin1_General_100_BIN2),
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
    ALTER TABLE dbo.BusinessPartners ALTER COLUMN NormalizedIdentificationNumber nvarchar(50) COLLATE Latin1_General_100_BIN2 NOT NULL;
GO

IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND name = N'NormalizedIdentificationNumber'
      AND (system_type_id <> 231 OR max_length <> 100 OR is_nullable <> 0
           OR collation_name <> N'Latin1_General_100_BIN2')
)
    THROW 52028, 'BusinessPartners.NormalizedIdentificationNumber final shape is incompatible.', 1;
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

IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerAddresses') AND name = N'GlobalId'
      AND (system_type_id <> 36 OR max_length <> 16 OR is_nullable <> 0)
)
    THROW 52028, 'BusinessPartnerAddresses.GlobalId final shape is incompatible.', 1;
IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.BusinessPartnerContacts') AND name = N'GlobalId'
      AND (system_type_id <> 36 OR max_length <> 16 OR is_nullable <> 0)
)
    THROW 52028, 'BusinessPartnerContacts.GlobalId final shape is incompatible.', 1;
GO

IF OBJECT_ID(N'dbo.DF_BusinessPartnerAddresses_GlobalId', N'D') IS NULL
    ALTER TABLE dbo.BusinessPartnerAddresses ADD CONSTRAINT DF_BusinessPartnerAddresses_GlobalId DEFAULT NEWID() FOR GlobalId;
GO

IF OBJECT_ID(N'dbo.DF_BusinessPartnerContacts_GlobalId', N'D') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD CONSTRAINT DF_BusinessPartnerContacts_GlobalId DEFAULT NEWID() FOR GlobalId;
GO

IF EXISTS
(
    SELECT GlobalId
    FROM dbo.BusinessPartnerAddresses
    GROUP BY GlobalId
    HAVING COUNT_BIG(1)>1
)
    THROW 52028, 'Duplicate BusinessPartnerAddresses.GlobalId prevents unique index creation.', 1;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints AS defaultItem
    INNER JOIN sys.columns AS columnItem
        ON columnItem.object_id = defaultItem.parent_object_id
       AND columnItem.column_id = defaultItem.parent_column_id
    WHERE defaultItem.parent_object_id = OBJECT_ID(N'dbo.BusinessPartnerAddresses')
      AND defaultItem.name = N'DF_BusinessPartnerAddresses_GlobalId'
      AND columnItem.name = N'GlobalId'
      AND LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(defaultItem.definition COLLATE Latin1_General_100_BIN2,N'(',N''),N')',N''),N' ',N''),NCHAR(13),N''),NCHAR(10),N'')) = N'newid'
)
    THROW 52028, 'DF_BusinessPartnerAddresses_GlobalId has an incompatible shape.', 1;
IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints AS defaultItem
    INNER JOIN sys.columns AS columnItem
        ON columnItem.object_id = defaultItem.parent_object_id
       AND columnItem.column_id = defaultItem.parent_column_id
    WHERE defaultItem.parent_object_id = OBJECT_ID(N'dbo.BusinessPartnerContacts')
      AND defaultItem.name = N'DF_BusinessPartnerContacts_GlobalId'
      AND columnItem.name = N'GlobalId'
      AND LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(defaultItem.definition COLLATE Latin1_General_100_BIN2,N'(',N''),N')',N''),N' ',N''),NCHAR(13),N''),NCHAR(10),N'')) = N'newid'
)
    THROW 52028, 'DF_BusinessPartnerContacts_GlobalId has an incompatible shape.', 1;
GO

IF EXISTS
(
    SELECT GlobalId
    FROM dbo.BusinessPartnerContacts
    GROUP BY GlobalId
    HAVING COUNT_BIG(1)>1
)
    THROW 52028, 'Duplicate BusinessPartnerContacts.GlobalId prevents unique index creation.', 1;
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

DECLARE @ExpectedBaseChecks table
(
    ParentObjectId int NOT NULL,
    ConstraintName sysname NOT NULL,
    ExpectedDefinition nvarchar(max) NOT NULL
);
INSERT @ExpectedBaseChecks(ParentObjectId,ConstraintName,ExpectedDefinition)
VALUES
    (OBJECT_ID(N'dbo.BusinessPartners'),N'CK_BusinessPartners_PartnerType',N'(partnertypein(n''customer'',n''supplier'',n''both''))'),
    (OBJECT_ID(N'dbo.BusinessPartners'),N'CK_BusinessPartners_MasterSyncStatus',N'(mastersyncstatusin(''pendingmaster'',''accepted'',''rejected'',''conflict'',''legacyreview''))'),
    (OBJECT_ID(N'dbo.BusinessPartners'),N'CK_BusinessPartners_CanonicalVersion',N'(canonicalversion>=(0))'),
    (OBJECT_ID(N'dbo.BusinessPartners'),N'CK_BusinessPartners_NormalizedIdentificationNumber',N'(nullif(normalizedidentificationnumber,n'''')isnotnull)'),
    (OBJECT_ID(N'dbo.BusinessPartnerSapMapping'),N'CK_BusinessPartnerSapMapping_SapCardCodeLength',N'(sapcardcodeisnullorlen(sapcardcode)<=(15))');

IF EXISTS
(
    SELECT 1
    FROM @ExpectedBaseChecks AS required
    LEFT JOIN sys.check_constraints AS checkItem
        ON checkItem.parent_object_id=required.ParentObjectId
       AND checkItem.name=required.ConstraintName
    CROSS APPLY
    (
        VALUES (LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(checkItem.definition COLLATE Latin1_General_100_BIN2,N'[',N''),N']',N''),N' ',N''),NCHAR(9),N''),NCHAR(13),N''),NCHAR(10),N'')))
    ) AS normalized(NormalizedDefinition)
    WHERE checkItem.object_id IS NULL OR checkItem.is_disabled=1 OR checkItem.is_not_trusted=1
       OR normalized.NormalizedDefinition<>required.ExpectedDefinition
)
    THROW 52028, 'BusinessPartner check constraints have an incompatible shape.', 1;

DECLARE @ExpectedBaseDefaults table
(
    ParentObjectId int NOT NULL,
    ConstraintName sysname NOT NULL,
    ColumnName sysname NOT NULL,
    ExpectedDefinition nvarchar(100) NOT NULL
);
INSERT @ExpectedBaseDefaults(ParentObjectId,ConstraintName,ColumnName,ExpectedDefinition)
VALUES
    (OBJECT_ID(N'dbo.BusinessPartners'),N'DF_BusinessPartners_CanonicalVersion',N'CanonicalVersion',N'1'),
    (OBJECT_ID(N'dbo.BusinessPartners'),N'DF_BusinessPartners_MasterSyncStatus',N'MasterSyncStatus',N'''accepted''');

IF EXISTS
(
    SELECT 1
    FROM @ExpectedBaseDefaults AS required
    LEFT JOIN sys.default_constraints AS defaultItem
        ON defaultItem.parent_object_id=required.ParentObjectId
       AND defaultItem.name=required.ConstraintName
    LEFT JOIN sys.columns AS columnItem
        ON columnItem.object_id=defaultItem.parent_object_id
       AND columnItem.column_id=defaultItem.parent_column_id
    CROSS APPLY
    (
        VALUES (LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(defaultItem.definition COLLATE Latin1_General_100_BIN2,N'(',N''),N')',N''),N' ',N''),NCHAR(13),N''),NCHAR(10),N'')))
    ) AS normalized(NormalizedDefinition)
    WHERE defaultItem.object_id IS NULL OR columnItem.name<>required.ColumnName
       OR normalized.NormalizedDefinition<>required.ExpectedDefinition
)
    THROW 52028, 'BusinessPartner default constraints have an incompatible shape.', 1;
GO

IF EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    CROSS APPLY
    (
        VALUES (LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(indexItem.filter_definition COLLATE Latin1_General_100_BIN2,N'[',N''),N']',N''),N'(',N''),N')',N''),N' ',N''),NCHAR(9),N''),NCHAR(13),N''),NCHAR(10),N'')))
    ) AS filterShape(NormalizedFilterDefinition)
    WHERE indexItem.object_id = OBJECT_ID(N'dbo.BusinessPartners')
      AND indexItem.name = N'UX_BusinessPartners_Identification_Active'
      AND
      (
          indexItem.is_unique <> 1 OR indexItem.has_filter <> 1
          OR filterShape.NormalizedFilterDefinition<>N'isdeleted=0andisactive=1'
          OR (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0) <> 3
          OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'PartnerType')
          OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=2 AND c.name=N'IdentificationTypeId')
          OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=3 AND c.name=N'NormalizedIdentificationNumber')
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
    SELECT 1 FROM sys.indexes AS indexItem
    CROSS APPLY
    (
        VALUES (LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(indexItem.filter_definition COLLATE Latin1_General_100_BIN2,N'[',N''),N']',N''),N'(',N''),N')',N''),N' ',N''),NCHAR(9),N''),NCHAR(13),N''),NCHAR(10),N'')))
    ) AS filterShape(NormalizedFilterDefinition)
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartners')
      AND indexItem.name=N'UX_BusinessPartners_Identification_Active'
      AND indexItem.is_unique=1 AND indexItem.has_filter=1
      AND filterShape.NormalizedFilterDefinition=N'isdeleted=0andisactive=1'
      AND (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)=3
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'PartnerType')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=2 AND c.name=N'IdentificationTypeId')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=3 AND c.name=N'NormalizedIdentificationNumber')
)
    THROW 52028, 'UX_BusinessPartners_Identification_Active has an incompatible shape.', 1;
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


IF EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    CROSS APPLY
    (
        VALUES (LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(indexItem.filter_definition COLLATE Latin1_General_100_BIN2,N'[',N''),N']',N''),N'(',N''),N')',N''),N' ',N''),NCHAR(9),N''),NCHAR(13),N''),NCHAR(10),N'')))
    ) AS filterShape(NormalizedFilterDefinition)
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerSapMapping')
      AND indexItem.name=N'UX_BusinessPartners_SapCardCode_Active'
      AND
      (
          indexItem.is_unique<>1 OR indexItem.has_filter<>1
          OR filterShape.NormalizedFilterDefinition<>N'sapcardcodeisnotnullandsapcardcode<>n'''''
          OR (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)<>1
          OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'SapCardCode')
      )
)
BEGIN
    DROP INDEX UX_BusinessPartners_SapCardCode_Active ON dbo.BusinessPartnerSapMapping;
    CREATE UNIQUE INDEX UX_BusinessPartners_SapCardCode_Active
    ON dbo.BusinessPartnerSapMapping (SapCardCode)
    WHERE SapCardCode IS NOT NULL AND SapCardCode <> N'';
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    CROSS APPLY
    (
        VALUES (LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(indexItem.filter_definition COLLATE Latin1_General_100_BIN2,N'[',N''),N']',N''),N'(',N''),N')',N''),N' ',N''),NCHAR(9),N''),NCHAR(13),N''),NCHAR(10),N'')))
    ) AS filterShape(NormalizedFilterDefinition)
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerSapMapping')
      AND indexItem.name=N'UX_BusinessPartners_SapCardCode_Active'
      AND indexItem.is_unique=1 AND indexItem.has_filter=1
      AND filterShape.NormalizedFilterDefinition=N'sapcardcodeisnotnullandsapcardcode<>n'''''
      AND (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)=1
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'SapCardCode')
)
    THROW 52028, 'UX_BusinessPartners_SapCardCode_Active has an incompatible shape.', 1;
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


IF EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerAddresses')
      AND indexItem.name=N'UX_BusinessPartnerAddresses_GlobalId'
      AND (indexItem.is_unique<>1 OR indexItem.has_filter<>0
           OR (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)<>1
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'GlobalId'))
)
BEGIN
    DROP INDEX UX_BusinessPartnerAddresses_GlobalId ON dbo.BusinessPartnerAddresses;
    CREATE UNIQUE INDEX UX_BusinessPartnerAddresses_GlobalId ON dbo.BusinessPartnerAddresses(GlobalId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerAddresses')
      AND indexItem.name=N'UX_BusinessPartnerAddresses_GlobalId'
      AND indexItem.is_unique=1 AND indexItem.has_filter=0
      AND (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)=1
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'GlobalId')
)
    THROW 52028, 'UX_BusinessPartnerAddresses_GlobalId has an incompatible shape.', 1;
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


IF EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerContacts')
      AND indexItem.name=N'UX_BusinessPartnerContacts_GlobalId'
      AND (indexItem.is_unique<>1 OR indexItem.has_filter<>0
           OR (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)<>1
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'GlobalId'))
)
BEGIN
    DROP INDEX UX_BusinessPartnerContacts_GlobalId ON dbo.BusinessPartnerContacts;
    CREATE UNIQUE INDEX UX_BusinessPartnerContacts_GlobalId ON dbo.BusinessPartnerContacts(GlobalId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerContacts')
      AND indexItem.name=N'UX_BusinessPartnerContacts_GlobalId'
      AND indexItem.is_unique=1 AND indexItem.has_filter=0
      AND (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)=1
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'GlobalId')
)
    THROW 52028, 'UX_BusinessPartnerContacts_GlobalId has an incompatible shape.', 1;
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

IF EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.LocalOutbox')
      AND indexItem.name=N'IX_LocalOutbox_TargetCompany_Status'
      AND (indexItem.is_unique<>0 OR indexItem.has_filter<>0
           OR (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)<>3
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'TargetCompanyId')
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=2 AND c.name=N'Status')
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=3 AND c.name=N'CreatedAt')
           OR (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND is_included_column=1)<>4
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'EventId')
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'EntityName')
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'EntityGlobalId')
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'CausationEventId'))
)
BEGIN
    DROP INDEX IX_LocalOutbox_TargetCompany_Status ON dbo.LocalOutbox;
    CREATE INDEX IX_LocalOutbox_TargetCompany_Status
    ON dbo.LocalOutbox(TargetCompanyId,Status,CreatedAt)
    INCLUDE(EventId,EntityName,EntityGlobalId,CausationEventId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.LocalOutbox')
      AND indexItem.name=N'IX_LocalOutbox_TargetCompany_Status'
      AND indexItem.is_unique=0 AND indexItem.has_filter=0
      AND (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)=3
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'TargetCompanyId')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=2 AND c.name=N'Status')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=3 AND c.name=N'CreatedAt')
      AND (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND is_included_column=1)=4
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'EventId')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'EntityName')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'EntityGlobalId')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'CausationEventId')
)
    THROW 52028, 'IX_LocalOutbox_TargetCompany_Status has an incompatible shape.', 1;
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
        PresentedBusinessPartnerRowVersion binary(8) NULL,
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

IF COL_LENGTH(N'dbo.BusinessPartnerSyncConflicts',N'PresentedBusinessPartnerRowVersion') IS NULL
    ALTER TABLE dbo.BusinessPartnerSyncConflicts ADD PresentedBusinessPartnerRowVersion binary(8) NULL;
GO

DECLARE @ExpectedConflictColumns table
(
    ColumnName sysname NOT NULL,
    UserTypeId int NOT NULL,
    SystemTypeId tinyint NOT NULL,
    MaxLength smallint NOT NULL,
    PrecisionValue tinyint NOT NULL,
    ScaleValue tinyint NOT NULL,
    CollationName sysname NULL,
    IsNullable bit NOT NULL
);
DECLARE @DatabaseCollation sysname=CONVERT(sysname,DATABASEPROPERTYEX(DB_NAME(),N'Collation'));
INSERT @ExpectedConflictColumns
    (ColumnName,UserTypeId,SystemTypeId,MaxLength,PrecisionValue,ScaleValue,CollationName,IsNullable)
VALUES
    (N'Id',127,127,8,19,0,NULL,0),(N'ProposalEventId',36,36,16,0,0,NULL,0),
    (N'BusinessPartnerId',56,56,4,10,0,NULL,1),(N'BusinessPartnerGlobalId',36,36,16,0,0,NULL,0),
    (N'OriginCompanyId',56,56,4,10,0,NULL,0),(N'BaseCanonicalVersion',127,127,8,19,0,NULL,0),
    (N'CurrentCanonicalVersion',127,127,8,19,0,NULL,0),(N'PresentedBusinessPartnerRowVersion',173,173,8,0,0,NULL,1),
    (N'BaseSnapshotJson',231,231,-1,0,0,@DatabaseCollation,1),(N'ProposedSnapshotJson',231,231,-1,0,0,@DatabaseCollation,0),
    (N'CanonicalSnapshotJson',231,231,-1,0,0,@DatabaseCollation,0),(N'ConflictFieldsJson',231,231,-1,0,0,@DatabaseCollation,0),
    (N'Status',167,167,20,0,0,@DatabaseCollation,0),(N'Resolution',167,167,20,0,0,@DatabaseCollation,1),
    (N'ResolutionReason',231,231,1000,0,0,@DatabaseCollation,1),(N'CreatedByUserId',56,56,4,10,0,NULL,1),
    (N'CreatedByUserName',231,231,240,0,0,@DatabaseCollation,1),(N'CreatedAt',42,42,6,19,0,NULL,0),
    (N'ResolvedByUserId',56,56,4,10,0,NULL,1),(N'ResolvedByUserName',231,231,240,0,0,@DatabaseCollation,1),
    (N'ResolvedAt',42,42,6,19,0,NULL,1),(N'RowVersion',189,189,8,0,0,NULL,0);

IF (SELECT COUNT_BIG(1) FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')) <> 22
   OR EXISTS
   (
       SELECT 1
       FROM @ExpectedConflictColumns AS expected
       LEFT JOIN sys.columns AS actual
           ON actual.object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
          AND actual.name=expected.ColumnName
       WHERE actual.column_id IS NULL OR actual.user_type_id<>expected.UserTypeId
          OR actual.system_type_id<>expected.SystemTypeId
          OR actual.max_length<>expected.MaxLength
          OR actual.precision<>expected.PrecisionValue
          OR actual.scale<>expected.ScaleValue
          OR CASE WHEN actual.collation_name=expected.CollationName
                       OR (actual.collation_name IS NULL AND expected.CollationName IS NULL)
                  THEN 0 ELSE 1 END=1
          OR actual.is_nullable<>expected.IsNullable
   )
   OR COLUMNPROPERTY(OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts'),N'Id',N'IsIdentity')<>1
    THROW 52028, 'BusinessPartnerSyncConflicts has an incompatible shape.', 1;

IF NOT EXISTS
(
    SELECT 1 FROM sys.key_constraints AS keyItem
    INNER JOIN sys.indexes AS indexItem
        ON indexItem.object_id=keyItem.parent_object_id AND indexItem.index_id=keyItem.unique_index_id
    WHERE keyItem.parent_object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND keyItem.name=N'PK_BusinessPartnerSyncConflicts' AND keyItem.type=N'PK'
      AND indexItem.is_unique=1
      AND EXISTS
      (
          SELECT 1 FROM sys.index_columns AS ic
          INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id
          WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id
            AND ic.key_ordinal=1 AND c.name=N'Id'
      )
)
    THROW 52028, 'BusinessPartnerSyncConflicts primary key has an incompatible shape.', 1;

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys AS foreignItem
    INNER JOIN sys.foreign_key_columns AS link
        ON link.constraint_object_id=foreignItem.object_id
    INNER JOIN sys.columns AS parentColumn
        ON parentColumn.object_id=link.parent_object_id AND parentColumn.column_id=link.parent_column_id
    INNER JOIN sys.columns AS referencedColumn
        ON referencedColumn.object_id=link.referenced_object_id AND referencedColumn.column_id=link.referenced_column_id
    WHERE foreignItem.parent_object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND foreignItem.name=N'FK_BusinessPartnerSyncConflicts_BusinessPartners'
      AND foreignItem.referenced_object_id=OBJECT_ID(N'dbo.BusinessPartners')
      AND foreignItem.is_disabled=0 AND foreignItem.is_not_trusted=0
      AND parentColumn.name=N'BusinessPartnerId' AND referencedColumn.name=N'Id'
)
    THROW 52028, 'BusinessPartnerSyncConflicts foreign key has an incompatible shape.', 1;

DECLARE @ExpectedConflictChecks table
(
    ConstraintName sysname NOT NULL,
    ExpectedDefinition nvarchar(max) NOT NULL
);
INSERT @ExpectedConflictChecks(ConstraintName,ExpectedDefinition)
VALUES
    (N'CK_BusinessPartnerSyncConflicts_Versions',N'(basecanonicalversion>=(0)andcurrentcanonicalversion>=(0))'),
    (N'CK_BusinessPartnerSyncConflicts_Status',N'(statusin(''open'',''resolved''))'),
    (N'CK_BusinessPartnerSyncConflicts_Resolution',N'(resolutionisnullorresolutionin(''acceptbranch'',''keepcentral''))'),
    (N'CK_BusinessPartnerSyncConflicts_ResolutionState',N'((status=''open''andresolutionisnullandresolvedatisnull)or(status=''resolved''andresolutionisnotnullandnullif(ltrim(rtrim(resolutionreason)),n'''')isnotnullandresolvedatisnotnull))'),
    (N'CK_BusinessPartnerSyncConflicts_BaseSnapshotJson',N'(basesnapshotjsonisnullorisjson(basesnapshotjson)=(1))'),
    (N'CK_BusinessPartnerSyncConflicts_ProposedSnapshotJson',N'(isjson(proposedsnapshotjson)=(1))'),
    (N'CK_BusinessPartnerSyncConflicts_CanonicalSnapshotJson',N'(isjson(canonicalsnapshotjson)=(1))'),
    (N'CK_BusinessPartnerSyncConflicts_ConflictFieldsJson',N'(isjson(conflictfieldsjson)=(1))');

IF EXISTS
(
    SELECT 1
    FROM @ExpectedConflictChecks AS required
    LEFT JOIN sys.check_constraints AS checkItem
        ON checkItem.parent_object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
       AND checkItem.name=required.ConstraintName
    CROSS APPLY
    (
        VALUES (LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(checkItem.definition COLLATE Latin1_General_100_BIN2,N'[',N''),N']',N''),N' ',N''),NCHAR(9),N''),NCHAR(13),N''),NCHAR(10),N'')))
    ) AS normalized(NormalizedDefinition)
    WHERE checkItem.object_id IS NULL OR checkItem.is_disabled=1 OR checkItem.is_not_trusted=1
       OR normalized.NormalizedDefinition<>required.ExpectedDefinition
)
    THROW 52028, 'BusinessPartnerSyncConflicts constraints have an incompatible shape.', 1;

IF NOT EXISTS
(
    SELECT 1 FROM sys.default_constraints AS defaultItem
    INNER JOIN sys.columns AS columnItem
        ON columnItem.object_id=defaultItem.parent_object_id AND columnItem.column_id=defaultItem.parent_column_id
    WHERE defaultItem.parent_object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND defaultItem.name=N'DF_BusinessPartnerSyncConflicts_Status'
      AND columnItem.name=N'Status'
      AND LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(defaultItem.definition COLLATE Latin1_General_100_BIN2,N'(',N''),N')',N''),N' ',N''),NCHAR(13),N''),NCHAR(10),N''))=N'''open'''
)
   OR NOT EXISTS
(
    SELECT 1 FROM sys.default_constraints AS defaultItem
    INNER JOIN sys.columns AS columnItem
        ON columnItem.object_id=defaultItem.parent_object_id AND columnItem.column_id=defaultItem.parent_column_id
    WHERE defaultItem.parent_object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND defaultItem.name=N'DF_BusinessPartnerSyncConflicts_CreatedAt'
      AND columnItem.name=N'CreatedAt'
      AND LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(defaultItem.definition COLLATE Latin1_General_100_BIN2,N'(',N''),N')',N''),N' ',N''),NCHAR(13),N''),NCHAR(10),N''))=N'sysutcdatetime'
)
    THROW 52028, 'BusinessPartnerSyncConflicts defaults have an incompatible shape.', 1;
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


IF EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND indexItem.name=N'UX_BusinessPartnerSyncConflicts_ProposalEventId'
      AND (indexItem.is_unique<>1 OR indexItem.has_filter<>0
           OR (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)<>1
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'ProposalEventId'))
)
BEGIN
    DROP INDEX UX_BusinessPartnerSyncConflicts_ProposalEventId ON dbo.BusinessPartnerSyncConflicts;
    CREATE UNIQUE INDEX UX_BusinessPartnerSyncConflicts_ProposalEventId ON dbo.BusinessPartnerSyncConflicts(ProposalEventId);
END;
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


IF EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND indexItem.name=N'IX_BusinessPartnerSyncConflicts_Status_CreatedAt'
      AND (indexItem.is_unique<>0 OR indexItem.has_filter<>0
           OR (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)<>2
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'Status' AND ic.is_descending_key=0)
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=2 AND c.name=N'CreatedAt' AND ic.is_descending_key=1)
           OR (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND is_included_column=1)<>3
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'BusinessPartnerGlobalId')
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'OriginCompanyId')
           OR NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'CurrentCanonicalVersion'))
)
BEGIN
    DROP INDEX IX_BusinessPartnerSyncConflicts_Status_CreatedAt ON dbo.BusinessPartnerSyncConflicts;
    CREATE INDEX IX_BusinessPartnerSyncConflicts_Status_CreatedAt
    ON dbo.BusinessPartnerSyncConflicts(Status,CreatedAt DESC)
    INCLUDE(BusinessPartnerGlobalId,OriginCompanyId,CurrentCanonicalVersion);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND indexItem.name=N'UX_BusinessPartnerSyncConflicts_ProposalEventId'
      AND indexItem.is_unique=1 AND indexItem.has_filter=0
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'ProposalEventId')
)
    THROW 52028, 'UX_BusinessPartnerSyncConflicts_ProposalEventId has an incompatible shape.', 1;
IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts')
      AND indexItem.name=N'IX_BusinessPartnerSyncConflicts_Status_CreatedAt'
      AND indexItem.is_unique=0 AND indexItem.has_filter=0
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'Status')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=2 AND c.name=N'CreatedAt' AND ic.is_descending_key=1)
      AND (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND is_included_column=1)=3
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'BusinessPartnerGlobalId')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'OriginCompanyId')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.is_included_column=1 AND c.name=N'CurrentCanonicalVersion')
)
    THROW 52028, 'IX_BusinessPartnerSyncConflicts_Status_CreatedAt has an incompatible shape.', 1;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260903.228')
BEGIN
    INSERT dbo.SchemaHistory (Version, Description)
    VALUES (N'20260903.228', N'BusinessPartner tenant bidirectional identity and conflict foundation');
END;
GO
