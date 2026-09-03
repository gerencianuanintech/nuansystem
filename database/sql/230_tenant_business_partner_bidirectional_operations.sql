/*
    BusinessPartner bidirectional tenant operations.
    All cross-tenant intent is persisted in LocalOutbox; no external connection is opened.
    Applying canonical or proposal-result messages never creates a return event.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerSyncConflicts', N'U') IS NULL
    THROW 52030, 'BusinessPartnerSyncConflicts is required before migration 230.', 1;
IF COL_LENGTH(N'dbo.BusinessPartners', N'CanonicalVersion') IS NULL
    THROW 52030, 'BusinessPartner CanonicalVersion is required before migration 230.', 1;
IF COL_LENGTH(N'dbo.BusinessPartnerAddresses', N'GlobalId') IS NULL
    THROW 52030, 'BusinessPartner address GlobalId is required before migration 230.', 1;
IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'GlobalId') IS NULL
    THROW 52030, 'BusinessPartner contact GlobalId is required before migration 230.', 1;
IF COL_LENGTH(N'dbo.LocalOutbox', N'TargetCompanyId') IS NULL
    THROW 52030, 'LocalOutbox TargetCompanyId is required before migration 230.', 1;
IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL
    THROW 52030, 'SyncInbox is required before migration 230.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 52030, 'SchemaHistory is required before migration 230.', 1;
IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.SyncInbox')
      AND indexItem.name=N'UX_SyncInbox_EventId' AND indexItem.is_unique=1
      AND (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)=1
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'EventId')
)
    THROW 52030, 'SyncInbox EventId uniqueness is required before migration 230.', 1;
IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes AS indexItem
    WHERE indexItem.object_id=OBJECT_ID(N'dbo.LocalOutbox')
      AND indexItem.name=N'UX_LocalOutbox_EventId' AND indexItem.is_unique=1
      AND (SELECT COUNT_BIG(1) FROM sys.index_columns WHERE object_id=indexItem.object_id AND index_id=indexItem.index_id AND key_ordinal>0)=1
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic INNER JOIN sys.columns AS c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=indexItem.object_id AND ic.index_id=indexItem.index_id AND ic.key_ordinal=1 AND c.name=N'EventId')
)
    THROW 52030, 'LocalOutbox EventId uniqueness is required before migration 230.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE
    @EventId uniqueidentifier,
    @SourceCompanyId int,
    @EntityName nvarchar(120),
    @EntityGlobalId uniqueidentifier,
    @Operation nvarchar(30),
    @PayloadJson nvarchar(max),
    @InboxId bigint OUTPUT,
    @InboxStatus nvarchar(30) OUTPUT,
    @EnvelopeResult int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @@TRANCOUNT = 0
        THROW 52030, 'SyncInbox envelope guard requires an ambient transaction.', 1;
    IF @EventId IS NULL OR @SourceCompanyId IS NULL OR @EntityName IS NULL OR @EntityGlobalId IS NULL OR @Operation IS NULL OR @PayloadJson IS NULL
        THROW 52030, 'SyncInbox envelope required fields cannot be null.', 1;

    DECLARE @ExistingSourceCompanyId int,@ExistingEntityName nvarchar(120),
            @ExistingEntityGlobalId uniqueidentifier,@ExistingOperation nvarchar(30),
            @ExistingPayloadJson nvarchar(max),@ExistingLastErrorMessage nvarchar(max);
    SELECT @InboxId=Id,@InboxStatus=Status,@ExistingSourceCompanyId=SourceCompanyId,
           @ExistingEntityName=EntityName,@ExistingEntityGlobalId=EntityGlobalId,
           @ExistingOperation=Operation,@ExistingPayloadJson=PayloadJson,
           @ExistingLastErrorMessage=LastErrorMessage
    FROM dbo.SyncInbox WITH (UPDLOCK,HOLDLOCK)
    WHERE EventId=@EventId;

    IF @InboxId IS NULL
    BEGIN
        INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
        VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,N'Pending');
        SET @InboxId=CONVERT(bigint,SCOPE_IDENTITY());
        SET @InboxStatus=N'Pending';
        SET @EnvelopeResult=1;
        RETURN;
    END;

    IF CASE WHEN @ExistingSourceCompanyId=@SourceCompanyId OR (@ExistingSourceCompanyId IS NULL AND @SourceCompanyId IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingEntityName COLLATE Latin1_General_100_BIN2=@EntityName COLLATE Latin1_General_100_BIN2 OR (@ExistingEntityName IS NULL AND @EntityName IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingEntityGlobalId=@EntityGlobalId OR (@ExistingEntityGlobalId IS NULL AND @EntityGlobalId IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingOperation COLLATE Latin1_General_100_BIN2=@Operation COLLATE Latin1_General_100_BIN2 OR (@ExistingOperation IS NULL AND @Operation IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingPayloadJson COLLATE Latin1_General_100_BIN2=@PayloadJson COLLATE Latin1_General_100_BIN2 OR (@ExistingPayloadJson IS NULL AND @PayloadJson IS NULL) THEN 0 ELSE 1 END=1
    BEGIN
        UPDATE dbo.SyncInbox
        SET Status=N'DeadLetter',ErrorMessage=N'EventId collision: persisted inbox envelope differs.',
            LastErrorMessage=N'EventId collision: persisted inbox envelope differs.',NextRetryAt=NULL
        WHERE Id=@InboxId;
        SET @InboxStatus=N'DeadLetter';
        SET @EnvelopeResult=4;
        RETURN;
    END;

    SET @EnvelopeResult=CASE WHEN @InboxStatus=N'DeadLetter'
        AND @ExistingLastErrorMessage LIKE N'EventId collision:%' THEN 4 ELSE 2 END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE
    @EventId uniqueidentifier,
    @CompanyId int,
    @TargetCompanyId int = NULL,
    @CausationEventId uniqueidentifier = NULL,
    @EntityName nvarchar(120),
    @EntityGlobalId uniqueidentifier,
    @EntityCode nvarchar(100) = NULL,
    @Operation nvarchar(30),
    @PayloadJson nvarchar(max),
    @OutboxId bigint OUTPUT,
    @EnvelopeResult int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @@TRANCOUNT = 0
        THROW 52030, 'LocalOutbox envelope guard requires an ambient transaction.', 1;
    IF @EventId IS NULL OR @CompanyId IS NULL OR @EntityName IS NULL OR @EntityGlobalId IS NULL OR @Operation IS NULL OR @PayloadJson IS NULL
        THROW 52030, 'LocalOutbox envelope required fields cannot be null.', 1;

    DECLARE @ExistingCompanyId int,@ExistingTargetCompanyId int,
            @ExistingCausationEventId uniqueidentifier,@ExistingEntityName nvarchar(120),
            @ExistingEntityGlobalId uniqueidentifier,@ExistingEntityCode nvarchar(100),
            @ExistingOperation nvarchar(30),@ExistingPayloadJson nvarchar(max),
            @ExistingOutboxStatus nvarchar(30),@ExistingLastErrorMessage nvarchar(max);
    SELECT @OutboxId=Id,@ExistingCompanyId=CompanyId,@ExistingTargetCompanyId=TargetCompanyId,
           @ExistingCausationEventId=CausationEventId,@ExistingEntityName=EntityName,
           @ExistingEntityGlobalId=EntityGlobalId,@ExistingEntityCode=EntityCode,
           @ExistingOperation=Operation,@ExistingPayloadJson=PayloadJson,
           @ExistingOutboxStatus=Status,@ExistingLastErrorMessage=LastErrorMessage
    FROM dbo.LocalOutbox WITH (UPDLOCK,HOLDLOCK)
    WHERE EventId=@EventId;

    IF @OutboxId IS NULL
    BEGIN
        INSERT dbo.LocalOutbox
            (EventId,CompanyId,TargetCompanyId,CausationEventId,EntityName,EntityGlobalId,EntityCode,Operation,PayloadJson,Status)
        VALUES
            (@EventId,@CompanyId,@TargetCompanyId,@CausationEventId,@EntityName,@EntityGlobalId,@EntityCode,@Operation,@PayloadJson,N'Pending');
        SET @OutboxId=CONVERT(bigint,SCOPE_IDENTITY());
        SET @EnvelopeResult=1;
        RETURN;
    END;

    IF CASE WHEN @ExistingCompanyId=@CompanyId OR (@ExistingCompanyId IS NULL AND @CompanyId IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingTargetCompanyId=@TargetCompanyId OR (@ExistingTargetCompanyId IS NULL AND @TargetCompanyId IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingCausationEventId=@CausationEventId OR (@ExistingCausationEventId IS NULL AND @CausationEventId IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingEntityName COLLATE Latin1_General_100_BIN2=@EntityName COLLATE Latin1_General_100_BIN2 OR (@ExistingEntityName IS NULL AND @EntityName IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingEntityGlobalId=@EntityGlobalId OR (@ExistingEntityGlobalId IS NULL AND @EntityGlobalId IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingEntityCode COLLATE Latin1_General_100_BIN2=@EntityCode COLLATE Latin1_General_100_BIN2 OR (@ExistingEntityCode IS NULL AND @EntityCode IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingOperation COLLATE Latin1_General_100_BIN2=@Operation COLLATE Latin1_General_100_BIN2 OR (@ExistingOperation IS NULL AND @Operation IS NULL) THEN 0 ELSE 1 END=1
       OR CASE WHEN @ExistingPayloadJson COLLATE Latin1_General_100_BIN2=@PayloadJson COLLATE Latin1_General_100_BIN2 OR (@ExistingPayloadJson IS NULL AND @PayloadJson IS NULL) THEN 0 ELSE 1 END=1
    BEGIN
        UPDATE dbo.LocalOutbox
        SET Status=N'DeadLetter',LastErrorMessage=N'EventId collision: persisted outbox envelope differs.',NextRetryAt=NULL
        WHERE Id=@OutboxId;
        SET @EnvelopeResult=4;
        RETURN;
    END;

    SET @EnvelopeResult=CASE WHEN @ExistingOutboxStatus=N'DeadLetter'
        AND @ExistingLastErrorMessage LIKE N'EventId collision:%' THEN 4 ELSE 2 END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_CHILDREN_APPLY
    @BusinessPartnerId int,
    @AddressesJson nvarchar(max),
    @ContactsJson nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;

    IF ISJSON(@AddressesJson) <> 1 OR ISJSON(@ContactsJson) <> 1
        THROW 52030, 'BusinessPartner child collections must be valid JSON arrays.', 1;

    UPDATE dbo.BusinessPartnerAddresses
    SET IsDeleted = 1, IsActive = 0
    WHERE BusinessPartnerId = @BusinessPartnerId AND IsDeleted = 0;

    MERGE dbo.BusinessPartnerAddresses WITH (HOLDLOCK) AS targetItem
    USING
    (
        SELECT GlobalId, AddressType, Line1, Line2, CountryId, ProvinceId, CityId,
               CountryCode, Province, City, PostalCode, Latitude, Longitude,
               IsPrimary, IsActive, IsDeleted
        FROM OPENJSON(@AddressesJson)
        WITH
        (
            GlobalId uniqueidentifier '$.globalId',
            AddressType nvarchar(30) '$.addressType',
            Line1 nvarchar(300) '$.line1',
            Line2 nvarchar(300) '$.line2',
            CountryId int '$.countryId',
            ProvinceId int '$.provinceId',
            CityId int '$.cityId',
            CountryCode nvarchar(3) '$.countryCode',
            Province nvarchar(120) '$.province',
            City nvarchar(120) '$.city',
            PostalCode nvarchar(30) '$.postalCode',
            Latitude decimal(11,8) '$.latitude',
            Longitude decimal(11,8) '$.longitude',
            IsPrimary bit '$.isPrimary',
            IsActive bit '$.isActive',
            IsDeleted bit '$.isDeleted'
        )
    ) AS sourceItem
    ON targetItem.BusinessPartnerId = @BusinessPartnerId
       AND targetItem.GlobalId = sourceItem.GlobalId
    WHEN MATCHED THEN
        UPDATE SET AddressType = sourceItem.AddressType, Line1 = sourceItem.Line1,
                   Line2 = sourceItem.Line2, CountryId = sourceItem.CountryId,
                   ProvinceId = sourceItem.ProvinceId, CityId = sourceItem.CityId,
                   CountryCode = sourceItem.CountryCode, Province = sourceItem.Province,
                   City = sourceItem.City, PostalCode = sourceItem.PostalCode,
                   Latitude = sourceItem.Latitude, Longitude = sourceItem.Longitude,
                   IsPrimary = ISNULL(sourceItem.IsPrimary, 0),
                   IsActive = ISNULL(sourceItem.IsActive, 1),
                   IsDeleted = ISNULL(sourceItem.IsDeleted, 0)
    WHEN NOT MATCHED BY TARGET THEN
        INSERT
        (
            GlobalId, BusinessPartnerId, AddressType, Line1, Line2, CountryId,
            ProvinceId, CityId, CountryCode, Province, City, PostalCode,
            Latitude, Longitude, IsPrimary, IsActive, IsDeleted
        )
        VALUES
        (
            sourceItem.GlobalId, @BusinessPartnerId, sourceItem.AddressType,
            sourceItem.Line1, sourceItem.Line2, sourceItem.CountryId,
            sourceItem.ProvinceId, sourceItem.CityId, sourceItem.CountryCode,
            sourceItem.Province, sourceItem.City, sourceItem.PostalCode,
            sourceItem.Latitude, sourceItem.Longitude,
            ISNULL(sourceItem.IsPrimary, 0), ISNULL(sourceItem.IsActive, 1),
            ISNULL(sourceItem.IsDeleted, 0)
        );

    UPDATE dbo.BusinessPartnerContacts
    SET IsDeleted = 1, IsActive = 0
    WHERE BusinessPartnerId = @BusinessPartnerId AND IsDeleted = 0;

    MERGE dbo.BusinessPartnerContacts WITH (HOLDLOCK) AS targetItem
    USING
    (
        SELECT GlobalId, ContactTypeId, ContactChannelId, Name, Position, Department,
               Phone, Extension, Mobile, Email, [Language], ReceivesNotifications,
               IsPrimary, IsActive, Notes, IsDeleted
        FROM OPENJSON(@ContactsJson)
        WITH
        (
            GlobalId uniqueidentifier '$.globalId',
            ContactTypeId int '$.contactTypeId',
            ContactChannelId int '$.contactChannelId',
            Name nvarchar(150) '$.name',
            Position nvarchar(120) '$.position',
            Department nvarchar(120) '$.department',
            Phone nvarchar(50) '$.phone',
            Extension nvarchar(20) '$.extension',
            Mobile nvarchar(50) '$.mobile',
            Email nvarchar(256) '$.email',
            [Language] nvarchar(50) '$.language',
            ReceivesNotifications bit '$.receivesNotifications',
            IsPrimary bit '$.isPrimary',
            IsActive bit '$.isActive',
            Notes nvarchar(500) '$.notes',
            IsDeleted bit '$.isDeleted'
        )
    ) AS sourceItem
    ON targetItem.BusinessPartnerId = @BusinessPartnerId
       AND targetItem.GlobalId = sourceItem.GlobalId
    WHEN MATCHED THEN
        UPDATE SET ContactTypeId = sourceItem.ContactTypeId,
                   ContactChannelId = sourceItem.ContactChannelId,
                   Name = sourceItem.Name, Position = sourceItem.Position,
                   Department = sourceItem.Department, Phone = sourceItem.Phone,
                   Extension = sourceItem.Extension, Mobile = sourceItem.Mobile,
                   Email = sourceItem.Email, [Language] = sourceItem.[Language],
                   ReceivesNotifications = ISNULL(sourceItem.ReceivesNotifications, 0),
                   IsPrimary = ISNULL(sourceItem.IsPrimary, 0),
                   IsActive = ISNULL(sourceItem.IsActive, 1), Notes = sourceItem.Notes,
                   IsDeleted = ISNULL(sourceItem.IsDeleted, 0)
    WHEN NOT MATCHED BY TARGET THEN
        INSERT
        (
            GlobalId, BusinessPartnerId, ContactTypeId, ContactChannelId, Name,
            Position, Department, Phone, Extension, Mobile, Email, [Language],
            ReceivesNotifications, IsPrimary, IsActive, Notes, IsDeleted
        )
        VALUES
        (
            sourceItem.GlobalId, @BusinessPartnerId, sourceItem.ContactTypeId,
            sourceItem.ContactChannelId, sourceItem.Name, sourceItem.Position,
            sourceItem.Department, sourceItem.Phone, sourceItem.Extension,
            sourceItem.Mobile, sourceItem.Email, sourceItem.[Language],
            ISNULL(sourceItem.ReceivesNotifications, 0),
            ISNULL(sourceItem.IsPrimary, 0), ISNULL(sourceItem.IsActive, 1),
            sourceItem.Notes, ISNULL(sourceItem.IsDeleted, 0)
        );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT
    @GlobalId uniqueidentifier,
    @Code nvarchar(50),
    @Name nvarchar(200),
    @CommercialName nvarchar(200) = NULL,
    @PartnerType nvarchar(20),
    @IdentificationTypeId int,
    @IdentificationNumber nvarchar(50),
    @NormalizedIdentificationNumber nvarchar(50),
    @Email nvarchar(256) = NULL,
    @Phone nvarchar(50) = NULL,
    @SapCardCode nvarchar(50) = NULL,
    @CanonicalVersion bigint,
    @IsActive bit,
    @IsDeleted bit,
    @AddressesJson nvarchar(max),
    @ContactsJson nvarchar(max),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @PartnerType NOT IN (N'Customer', N'Supplier')
        THROW 52030, 'Only Customer or Supplier can become canonical.', 1;
    IF @CanonicalVersion < 1
        THROW 52030, 'CanonicalVersion must be positive.', 1;
    IF NULLIF(@NormalizedIdentificationNumber, N'') IS NULL
        THROW 52030, 'Normalized identification is required.', 1;
    IF DATALENGTH(@SapCardCode) > 30
        THROW 52030, 'SapCardCode cannot exceed 15 characters.', 1;

    DECLARE @ExpectedNormalizedIdentificationNumber nvarchar(50) = REPLACE(
        TRANSLATE(
            UPPER(LTRIM(RTRIM(@IdentificationNumber)) COLLATE Latin1_General_100_BIN2),
            N'.-' + NCHAR(9) + NCHAR(10) + NCHAR(11) + NCHAR(12) + NCHAR(13)
                + NCHAR(32) + NCHAR(133) + NCHAR(160) + NCHAR(5760)
                + NCHAR(8192) + NCHAR(8193) + NCHAR(8194) + NCHAR(8195)
                + NCHAR(8196) + NCHAR(8197) + NCHAR(8198) + NCHAR(8199)
                + NCHAR(8200) + NCHAR(8201) + NCHAR(8202) + NCHAR(8232)
                + NCHAR(8233) + NCHAR(8239) + NCHAR(8287) + NCHAR(12288),
            REPLICATE(N' ',27)),N' ',N'') COLLATE Latin1_General_100_BIN2;
    IF @NormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2
       <> @ExpectedNormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2
        THROW 52030, 'Normalized identification does not match the canonical policy.', 1;

    IF EXISTS
    (
        SELECT 1 FROM dbo.BusinessPartners WITH (UPDLOCK, HOLDLOCK)
        WHERE GlobalId <> @GlobalId
          AND PartnerType = @PartnerType
          AND IdentificationTypeId = @IdentificationTypeId
          AND NormalizedIdentificationNumber = @NormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2
          AND IsDeleted = 0 AND IsActive = 1
    )
        THROW 52030, 'Canonical identification belongs to another BusinessPartner.', 1;

    DECLARE @BusinessPartnerId int,@ExistingCode nvarchar(50),@ExistingPartnerType nvarchar(20),
            @ExistingIdentificationTypeId int,@ExistingIdentificationNumber nvarchar(50),
            @ExistingNormalizedIdentificationNumber nvarchar(50),@ExistingMasterSyncStatus varchar(20),
            @ExistingSapCardCode nvarchar(50);
    SELECT @BusinessPartnerId=Id,@ExistingCode=Code,@ExistingPartnerType=PartnerType,
           @ExistingIdentificationTypeId=IdentificationTypeId,
           @ExistingIdentificationNumber=IdentificationNumber,
           @ExistingNormalizedIdentificationNumber=NormalizedIdentificationNumber,
           @ExistingMasterSyncStatus=MasterSyncStatus
    FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)
    WHERE GlobalId = @GlobalId;

    IF @BusinessPartnerId IS NOT NULL
       AND
       (
           @ExistingPartnerType=N'Both' OR @ExistingMasterSyncStatus='LegacyReview'
           OR @ExistingCode COLLATE Latin1_General_100_BIN2<>@Code COLLATE Latin1_General_100_BIN2
           OR @ExistingPartnerType COLLATE Latin1_General_100_BIN2<>@PartnerType COLLATE Latin1_General_100_BIN2
           OR @ExistingIdentificationTypeId<>@IdentificationTypeId
           OR @ExistingIdentificationNumber COLLATE Latin1_General_100_BIN2<>@IdentificationNumber COLLATE Latin1_General_100_BIN2
           OR @ExistingNormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2<>@NormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2
       )
        THROW 52032, 'Immutable BusinessPartner identity conflict requires reconciliation.', 1;

    IF @BusinessPartnerId IS NOT NULL
    BEGIN
        SELECT @ExistingSapCardCode=SapCardCode
        FROM dbo.BusinessPartnerSapMapping WITH (UPDLOCK,HOLDLOCK)
        WHERE BusinessPartnerId=@BusinessPartnerId;
        IF NULLIF(LTRIM(RTRIM(@ExistingSapCardCode)),N'') IS NOT NULL
           AND NULLIF(LTRIM(RTRIM(@SapCardCode)),N'') IS NOT NULL
           AND @ExistingSapCardCode COLLATE Latin1_General_100_BIN2<>@SapCardCode COLLATE Latin1_General_100_BIN2
            THROW 52032, 'Confirmed SapCardCode conflict requires reconciliation.', 1;
    END;

    IF @BusinessPartnerId IS NULL
    BEGIN
        INSERT dbo.BusinessPartners
        (
            GlobalId, Code, Name, CommercialName, PartnerType, IdentificationTypeId,
            IdentificationNumber, NormalizedIdentificationNumber, Email, Phone,
            CanonicalVersion, MasterSyncStatus, MasterSyncMessage, IsActive,
            IsDeleted, CreatedByUserId, CreatedByUserName, CreatedAt,
            DeletedByUserId, DeletedByUserName, DeletedAt
        )
        VALUES
        (
            @GlobalId, @Code, @Name, @CommercialName, @PartnerType,
            @IdentificationTypeId, @IdentificationNumber,
            @NormalizedIdentificationNumber, @Email, @Phone,
            @CanonicalVersion, 'Accepted', NULL, @IsActive, @IsDeleted,
            @AuditUserId, @AuditUserName, SYSUTCDATETIME(),
            CASE WHEN @IsDeleted = 1 THEN @AuditUserId END,
            CASE WHEN @IsDeleted = 1 THEN @AuditUserName END,
            CASE WHEN @IsDeleted = 1 THEN SYSUTCDATETIME() END
        );
        SET @BusinessPartnerId = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.BusinessPartners
        SET Name = @Name, CommercialName = @CommercialName,
            Email = @Email, Phone = @Phone, CanonicalVersion = @CanonicalVersion,
            MasterSyncStatus = 'Accepted', MasterSyncMessage = NULL,
            IsActive = @IsActive, IsDeleted = @IsDeleted,
            UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME(),
            DeletedByUserId = CASE WHEN @IsDeleted = 1 THEN @AuditUserId END,
            DeletedByUserName = CASE WHEN @IsDeleted = 1 THEN @AuditUserName END,
            DeletedAt = CASE WHEN @IsDeleted = 1 THEN SYSUTCDATETIME() END
        WHERE Id = @BusinessPartnerId;
    END;

    IF OBJECT_ID(N'dbo.BusinessPartnerFiscalData', N'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerFiscalData WHERE BusinessPartnerId = @BusinessPartnerId)
        INSERT dbo.BusinessPartnerFiscalData (BusinessPartnerId) VALUES (@BusinessPartnerId);
    IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerAccountingSettings WHERE BusinessPartnerId = @BusinessPartnerId)
        INSERT dbo.BusinessPartnerAccountingSettings (BusinessPartnerId) VALUES (@BusinessPartnerId);
    IF OBJECT_ID(N'dbo.BusinessPartnerCreditSettings', N'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerCreditSettings WHERE BusinessPartnerId = @BusinessPartnerId)
        INSERT dbo.BusinessPartnerCreditSettings (BusinessPartnerId) VALUES (@BusinessPartnerId);
    IF OBJECT_ID(N'dbo.BusinessPartnerPurchaseSettings', N'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerPurchaseSettings WHERE BusinessPartnerId = @BusinessPartnerId)
        INSERT dbo.BusinessPartnerPurchaseSettings (BusinessPartnerId) VALUES (@BusinessPartnerId);

    IF EXISTS (SELECT 1 FROM dbo.BusinessPartnerSapMapping WHERE BusinessPartnerId = @BusinessPartnerId)
    BEGIN
        IF NULLIF(LTRIM(RTRIM(@ExistingSapCardCode)),N'') IS NULL
            UPDATE dbo.BusinessPartnerSapMapping
            SET SapCardCode = @SapCardCode
            WHERE BusinessPartnerId = @BusinessPartnerId;
    END
    ELSE
        INSERT dbo.BusinessPartnerSapMapping (BusinessPartnerId, SapCardCode)
        VALUES (@BusinessPartnerId, @SapCardCode);

    EXEC dbo.SP_NA_POST_BUSINESSPARTNER_CHILDREN_APPLY
        @BusinessPartnerId = @BusinessPartnerId,
        @AddressesJson = @AddressesJson,
        @ContactsJson = @ContactsJson;

    SELECT @BusinessPartnerId AS BusinessPartnerId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNER_CANONICAL_FORUPDATE
    @GlobalId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;

    SELECT bp.Id AS Id,bp.GlobalId AS GlobalId,bp.Code AS Code,bp.Name AS Name,
           bp.ExternalSystem AS ExternalSystem,bp.ExternalCode AS ExternalCode,
           bp.CommercialName AS CommercialName,bp.PartnerType AS PartnerType,
           bp.IdentificationTypeId AS IdentificationTypeId,
           bp.IdentificationNumber AS IdentificationNumber,
           bp.NormalizedIdentificationNumber AS NormalizedIdentificationNumber,
           bp.SupplierGroupId AS SupplierGroupId,bp.SupplierClassId AS SupplierClassId,
           bp.EconomicActivityId AS EconomicActivityId,bp.ZoneId AS ZoneId,
           bp.SupplyMethodId AS SupplyMethodId,bp.Email AS Email,bp.Phone AS Phone,
           bp.Website AS Website,bp.Remarks AS Remarks,bp.CanonicalVersion AS CanonicalVersion,
           bp.MasterSyncStatus AS MasterSyncStatus,bp.MasterSyncMessage AS MasterSyncMessage,
           bp.IsActive AS IsActive,bp.CreatedByUserId AS CreatedByUserId,
           bp.CreatedByUserName AS CreatedByUserName,bp.CreatedAt AS CreatedAt,
           bp.UpdatedByUserId AS UpdatedByUserId,bp.UpdatedByUserName AS UpdatedByUserName,
           bp.UpdatedAt AS UpdatedAt,bp.IsDeleted AS IsDeleted,
           bp.DeletedByUserId AS DeletedByUserId,bp.DeletedByUserName AS DeletedByUserName,
           bp.DeletedAt AS DeletedAt,bp.RowVersion AS RowVersion,
           mapping.SapCardCode AS SapCardCode
    FROM dbo.BusinessPartners AS bp WITH (UPDLOCK, HOLDLOCK)
    LEFT JOIN dbo.BusinessPartnerSapMapping AS mapping WITH (UPDLOCK, HOLDLOCK)
        ON mapping.BusinessPartnerId = bp.Id
    WHERE bp.GlobalId = @GlobalId;

    SELECT addressItem.Id AS Id,addressItem.GlobalId AS GlobalId,
           addressItem.BusinessPartnerId AS BusinessPartnerId,
           addressItem.AddressType AS AddressType,addressItem.Line1 AS Line1,
           addressItem.Line2 AS Line2,addressItem.CountryId AS CountryId,
           addressItem.ProvinceId AS ProvinceId,addressItem.CityId AS CityId,
           addressItem.CountryCode AS CountryCode,addressItem.Province AS Province,
           addressItem.City AS City,addressItem.PostalCode AS PostalCode,
           addressItem.Latitude AS Latitude,addressItem.Longitude AS Longitude,
           addressItem.IsPrimary AS IsPrimary,addressItem.IsActive AS IsActive,
           addressItem.CreatedAt AS CreatedAt,addressItem.IsDeleted AS IsDeleted
    FROM dbo.BusinessPartnerAddresses AS addressItem WITH (UPDLOCK, HOLDLOCK)
    INNER JOIN dbo.BusinessPartners AS bp ON bp.Id = addressItem.BusinessPartnerId
    WHERE bp.GlobalId = @GlobalId
    ORDER BY addressItem.Id;

    SELECT contactItem.Id AS Id,contactItem.GlobalId AS GlobalId,
           contactItem.BusinessPartnerId AS BusinessPartnerId,
           contactItem.ContactTypeId AS ContactTypeId,
           contactItem.ContactChannelId AS ContactChannelId,contactItem.Name AS Name,
           contactItem.Position AS Position,contactItem.Department AS Department,
           contactItem.Phone AS Phone,contactItem.Extension AS Extension,
           contactItem.Mobile AS Mobile,contactItem.Email AS Email,
           contactItem.[Language] AS [Language],
           contactItem.ReceivesNotifications AS ReceivesNotifications,
           contactItem.IsPrimary AS IsPrimary,contactItem.IsActive AS IsActive,
           contactItem.Notes AS Notes,contactItem.CreatedAt AS CreatedAt,
           contactItem.IsDeleted AS IsDeleted
    FROM dbo.BusinessPartnerContacts AS contactItem WITH (UPDLOCK, HOLDLOCK)
    INNER JOIN dbo.BusinessPartners AS bp ON bp.Id = contactItem.BusinessPartnerId
    WHERE bp.GlobalId = @GlobalId
    ORDER BY contactItem.Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT
    @ProposalEventId uniqueidentifier,
    @CompanyId int,
    @SourceCompanyId int,
    @BusinessPartnerGlobalId uniqueidentifier,
    @Operation nvarchar(30),
    @ProposalPayloadJson nvarchar(max),
    @Code nvarchar(50),
    @Name nvarchar(200),
    @CommercialName nvarchar(200) = NULL,
    @PartnerType nvarchar(20),
    @IdentificationTypeId int,
    @IdentificationNumber nvarchar(50),
    @NormalizedIdentificationNumber nvarchar(50),
    @Email nvarchar(256) = NULL,
    @Phone nvarchar(50) = NULL,
    @SapCardCode nvarchar(50) = NULL,
    @CanonicalVersion bigint,
    @IsActive bit,
    @IsDeleted bit,
    @AddressesJson nvarchar(max),
    @ContactsJson nvarchar(max),
    @CanonicalEventId uniqueidentifier,
    @CanonicalPayloadJson nvarchar(max),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF DATALENGTH(@SapCardCode) > 30
        THROW 52030, 'SapCardCode cannot exceed 15 characters.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@InboxEnvelopeResult int,
                @OutboxId bigint,@OutboxEnvelopeResult int,@BusinessPartnerId int,
                @LiveCode nvarchar(50),@LivePartnerType nvarchar(20),
                @LiveIdentificationTypeId int,@LiveIdentificationNumber nvarchar(50),
                @LiveNormalizedIdentificationNumber nvarchar(50),@LiveMasterSyncStatus varchar(20),
                @LiveSapCardCode nvarchar(50),@LiveCanonicalVersion bigint,
                @LiveBusinessPartnerRowVersion binary(8);
        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE
            @EventId=@ProposalEventId,@SourceCompanyId=@SourceCompanyId,
            @EntityName=N'BusinessPartnerProposal',@EntityGlobalId=@BusinessPartnerGlobalId,
            @Operation=@Operation,@PayloadJson=@ProposalPayloadJson,
            @InboxId=@InboxId OUTPUT,@InboxStatus=@InboxStatus OUTPUT,
            @EnvelopeResult=@InboxEnvelopeResult OUTPUT;

        IF @InboxEnvelopeResult=4
        BEGIN
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode,CAST(NULL AS int) AS BusinessPartnerId,
                   CAST(NULL AS bigint) AS CanonicalVersion;
            RETURN;
        END;

        IF @InboxStatus = N'Applied'
        BEGIN
            SELECT @BusinessPartnerId = Id FROM dbo.BusinessPartners WHERE GlobalId = @BusinessPartnerGlobalId;
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode, @BusinessPartnerId AS BusinessPartnerId, @CanonicalVersion AS CanonicalVersion;
            RETURN;
        END;

        SELECT @BusinessPartnerId=bp.Id,@LiveCode=bp.Code,@LivePartnerType=bp.PartnerType,
               @LiveIdentificationTypeId=bp.IdentificationTypeId,
               @LiveIdentificationNumber=bp.IdentificationNumber,
               @LiveNormalizedIdentificationNumber=bp.NormalizedIdentificationNumber,
               @LiveMasterSyncStatus=bp.MasterSyncStatus,@LiveCanonicalVersion=bp.CanonicalVersion,
               @LiveSapCardCode=mapping.SapCardCode,@LiveBusinessPartnerRowVersion=bp.RowVersion
        FROM dbo.BusinessPartners AS bp WITH (UPDLOCK,HOLDLOCK)
        LEFT JOIN dbo.BusinessPartnerSapMapping AS mapping WITH (UPDLOCK,HOLDLOCK)
            ON mapping.BusinessPartnerId=bp.Id
        WHERE bp.GlobalId=@BusinessPartnerGlobalId;

        IF @BusinessPartnerId IS NOT NULL
           AND
           (
               @LivePartnerType=N'Both' OR @LiveMasterSyncStatus='LegacyReview'
               OR @LiveCode COLLATE Latin1_General_100_BIN2<>@Code COLLATE Latin1_General_100_BIN2
               OR @LivePartnerType COLLATE Latin1_General_100_BIN2<>@PartnerType COLLATE Latin1_General_100_BIN2
               OR @LiveIdentificationTypeId<>@IdentificationTypeId
               OR @LiveIdentificationNumber COLLATE Latin1_General_100_BIN2<>@IdentificationNumber COLLATE Latin1_General_100_BIN2
               OR @LiveNormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2<>@NormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2
               OR (NULLIF(LTRIM(RTRIM(@LiveSapCardCode)),N'') IS NOT NULL
                   AND NULLIF(LTRIM(RTRIM(@SapCardCode)),N'') IS NOT NULL
                   AND @LiveSapCardCode COLLATE Latin1_General_100_BIN2<>@SapCardCode COLLATE Latin1_General_100_BIN2)
               OR @LiveCanonicalVersion>=@CanonicalVersion
           )
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerSyncConflicts WITH (UPDLOCK,HOLDLOCK) WHERE ProposalEventId=@ProposalEventId)
                INSERT dbo.BusinessPartnerSyncConflicts
                    (ProposalEventId,BusinessPartnerId,BusinessPartnerGlobalId,OriginCompanyId,
                     BaseCanonicalVersion,CurrentCanonicalVersion,PresentedBusinessPartnerRowVersion,BaseSnapshotJson,
                     ProposedSnapshotJson,CanonicalSnapshotJson,ConflictFieldsJson,
                     CreatedByUserId,CreatedByUserName)
                VALUES
                    (@ProposalEventId,@BusinessPartnerId,@BusinessPartnerGlobalId,@SourceCompanyId,
                     @LiveCanonicalVersion,@LiveCanonicalVersion,@LiveBusinessPartnerRowVersion,NULL,@ProposalPayloadJson,
                     @CanonicalPayloadJson,N'["immutableIdentityRoleSapOrVersion"]',
                     @AuditUserId,@AuditUserName);
            UPDATE dbo.SyncInbox
            SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,
                LastErrorMessage=N'Proposal moved to BusinessPartner conflict workflow.',NextRetryAt=NULL
            WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 5 AS ResultCode,@BusinessPartnerId AS BusinessPartnerId,
                   @LiveCanonicalVersion AS CanonicalVersion;
            RETURN;
        END;

        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE
            @EventId=@CanonicalEventId,@CompanyId=@CompanyId,@TargetCompanyId=NULL,
            @CausationEventId=@ProposalEventId,@EntityName=N'BusinessPartner',
            @EntityGlobalId=@BusinessPartnerGlobalId,@EntityCode=@Code,
            @Operation=@Operation,@PayloadJson=@CanonicalPayloadJson,
            @OutboxId=@OutboxId OUTPUT,@EnvelopeResult=@OutboxEnvelopeResult OUTPUT;
        IF @OutboxEnvelopeResult=4
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status=N'DeadLetter',ErrorMessage=N'Outbound EventId collision.',
                LastErrorMessage=N'Outbound EventId collision.',NextRetryAt=NULL
            WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode,CAST(NULL AS int) AS BusinessPartnerId,
                   CAST(NULL AS bigint) AS CanonicalVersion;
            RETURN;
        END;

        DECLARE @Saved table (BusinessPartnerId int NOT NULL);
        INSERT @Saved (BusinessPartnerId)
        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT
            @GlobalId=@BusinessPartnerGlobalId, @Code=@Code, @Name=@Name,
            @CommercialName=@CommercialName, @PartnerType=@PartnerType,
            @IdentificationTypeId=@IdentificationTypeId,
            @IdentificationNumber=@IdentificationNumber,
            @NormalizedIdentificationNumber=@NormalizedIdentificationNumber,
            @Email=@Email, @Phone=@Phone, @SapCardCode=@SapCardCode,
            @CanonicalVersion=@CanonicalVersion, @IsActive=@IsActive,
            @IsDeleted=@IsDeleted, @AddressesJson=@AddressesJson,
            @ContactsJson=@ContactsJson, @AuditUserId=@AuditUserId,
            @AuditUserName=@AuditUserName;
        SELECT @BusinessPartnerId = BusinessPartnerId FROM @Saved;

        UPDATE dbo.SyncInbox
        SET Status=N'Applied', AppliedAt=SYSUTCDATETIME(), ErrorMessage=NULL,
            LastErrorMessage=NULL, NextRetryAt=NULL
        WHERE Id=@InboxId;

        UPDATE dbo.BusinessPartnerSyncConflicts
        SET Status='Resolved', Resolution='AcceptBranch',
            ResolutionReason=COALESCE(ResolutionReason, N'Accepted during proposal reconciliation.'),
            ResolvedByUserId=@AuditUserId, ResolvedByUserName=@AuditUserName,
            ResolvedAt=SYSUTCDATETIME()
        WHERE ProposalEventId=@ProposalEventId AND Status='Open';

        COMMIT TRANSACTION;
        SELECT 1 AS ResultCode, @BusinessPartnerId AS BusinessPartnerId, @CanonicalVersion AS CanonicalVersion;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT
    @ProposalEventId uniqueidentifier,
    @CompanyId int,
    @SourceCompanyId int,
    @BusinessPartnerId int = NULL,
    @BusinessPartnerGlobalId uniqueidentifier,
    @BaseCanonicalVersion bigint,
    @CurrentCanonicalVersion bigint,
    @BaseSnapshotJson nvarchar(max) = NULL,
    @ProposedSnapshotJson nvarchar(max),
    @CanonicalSnapshotJson nvarchar(max),
    @ConflictFieldsJson nvarchar(max),
    @ResultEventId uniqueidentifier,
    @ResultPayloadJson nvarchar(max),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@InboxEnvelopeResult int,
                @OutboxId bigint,@OutboxEnvelopeResult int,
                @PresentedBusinessPartnerRowVersion binary(8),@LiveCurrentCanonicalVersion bigint;
        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE
            @EventId=@ProposalEventId,@SourceCompanyId=@SourceCompanyId,
            @EntityName=N'BusinessPartnerProposal',@EntityGlobalId=@BusinessPartnerGlobalId,
            @Operation=N'Updated',@PayloadJson=@ProposedSnapshotJson,
            @InboxId=@InboxId OUTPUT,@InboxStatus=@InboxStatus OUTPUT,
            @EnvelopeResult=@InboxEnvelopeResult OUTPUT;
        IF @InboxEnvelopeResult=4
        BEGIN
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode;
            RETURN;
        END;
        IF @InboxStatus=N'Applied'
        BEGIN
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;
        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE
            @EventId=@ResultEventId,@CompanyId=@CompanyId,@TargetCompanyId=@SourceCompanyId,
            @CausationEventId=@ProposalEventId,@EntityName=N'BusinessPartnerProposalResult',
            @EntityGlobalId=@BusinessPartnerGlobalId,@EntityCode=NULL,
            @Operation=N'Updated',@PayloadJson=@ResultPayloadJson,
            @OutboxId=@OutboxId OUTPUT,@EnvelopeResult=@OutboxEnvelopeResult OUTPUT;
        IF @OutboxEnvelopeResult=4
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status=N'DeadLetter',ErrorMessage=N'Outbound EventId collision.',
                LastErrorMessage=N'Outbound EventId collision.',NextRetryAt=NULL
            WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode;
            RETURN;
        END;

        SELECT @PresentedBusinessPartnerRowVersion=RowVersion,
               @LiveCurrentCanonicalVersion=CanonicalVersion
        FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)
        WHERE Id=@BusinessPartnerId AND GlobalId=@BusinessPartnerGlobalId;
        IF @LiveCurrentCanonicalVersion IS NOT NULL
            SET @CurrentCanonicalVersion=@LiveCurrentCanonicalVersion;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.BusinessPartnerSyncConflicts WITH (UPDLOCK,HOLDLOCK)
            WHERE ProposalEventId=@ProposalEventId
        )
            INSERT dbo.BusinessPartnerSyncConflicts
            (
                ProposalEventId,BusinessPartnerId,BusinessPartnerGlobalId,OriginCompanyId,
                BaseCanonicalVersion,CurrentCanonicalVersion,PresentedBusinessPartnerRowVersion,BaseSnapshotJson,
                ProposedSnapshotJson,CanonicalSnapshotJson,ConflictFieldsJson,
                CreatedByUserId,CreatedByUserName
            )
            VALUES
            (
                @ProposalEventId,@BusinessPartnerId,@BusinessPartnerGlobalId,@SourceCompanyId,
                @BaseCanonicalVersion,@CurrentCanonicalVersion,@PresentedBusinessPartnerRowVersion,@BaseSnapshotJson,
                @ProposedSnapshotJson,@CanonicalSnapshotJson,@ConflictFieldsJson,
                @AuditUserId,@AuditUserName
            );

        UPDATE dbo.SyncInbox
        SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL
        WHERE Id=@InboxId;

        COMMIT TRANSACTION;
        SELECT 1 AS ResultCode;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT
    @ProposalEventId uniqueidentifier,
    @CompanyId int,
    @SourceCompanyId int,
    @BusinessPartnerGlobalId uniqueidentifier,
    @ProposalPayloadJson nvarchar(max),
    @ResultEventId uniqueidentifier,
    @ResultPayloadJson nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@InboxEnvelopeResult int,
                @OutboxId bigint,@OutboxEnvelopeResult int;
        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE
            @EventId=@ProposalEventId,@SourceCompanyId=@SourceCompanyId,
            @EntityName=N'BusinessPartnerProposal',@EntityGlobalId=@BusinessPartnerGlobalId,
            @Operation=N'Updated',@PayloadJson=@ProposalPayloadJson,
            @InboxId=@InboxId OUTPUT,@InboxStatus=@InboxStatus OUTPUT,
            @EnvelopeResult=@InboxEnvelopeResult OUTPUT;
        IF @InboxEnvelopeResult=4
        BEGIN
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode;
            RETURN;
        END;
        IF @InboxStatus=N'Applied'
        BEGIN
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;
        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE
            @EventId=@ResultEventId,@CompanyId=@CompanyId,@TargetCompanyId=@SourceCompanyId,
            @CausationEventId=@ProposalEventId,@EntityName=N'BusinessPartnerProposalResult',
            @EntityGlobalId=@BusinessPartnerGlobalId,@EntityCode=NULL,
            @Operation=N'Updated',@PayloadJson=@ResultPayloadJson,
            @OutboxId=@OutboxId OUTPUT,@EnvelopeResult=@OutboxEnvelopeResult OUTPUT;
        IF @OutboxEnvelopeResult=4
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status=N'DeadLetter',ErrorMessage=N'Outbound EventId collision.',
                LastErrorMessage=N'Outbound EventId collision.',NextRetryAt=NULL
            WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode;
            RETURN;
        END;
        UPDATE dbo.SyncInbox
        SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL
        WHERE Id=@InboxId;

        COMMIT TRANSACTION;
        SELECT 1 AS ResultCode;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY
    @EventId uniqueidentifier,
    @SourceCompanyId int,
    @EntityGlobalId uniqueidentifier,
    @Operation nvarchar(30),
    @PayloadJson nvarchar(max),
    @Code nvarchar(50),
    @Name nvarchar(200),
    @CommercialName nvarchar(200) = NULL,
    @PartnerType nvarchar(20),
    @IdentificationTypeId int,
    @IdentificationNumber nvarchar(50),
    @NormalizedIdentificationNumber nvarchar(50),
    @Email nvarchar(256) = NULL,
    @Phone nvarchar(50) = NULL,
    @SapCardCode nvarchar(50) = NULL,
    @CanonicalVersion bigint,
    @IsActive bit,
    @IsDeleted bit,
    @AddressesJson nvarchar(max),
    @ContactsJson nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF DATALENGTH(@SapCardCode) > 30
        THROW 52030, 'SapCardCode cannot exceed 15 characters.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@InboxEnvelopeResult int,
                @CurrentVersion bigint,@BusinessPartnerId int,@LiveCode nvarchar(50),
                @LivePartnerType nvarchar(20),@LiveIdentificationTypeId int,
                @LiveIdentificationNumber nvarchar(50),@LiveNormalizedIdentificationNumber nvarchar(50),
                @LiveMasterSyncStatus varchar(20),@LiveSapCardCode nvarchar(50);
        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE
            @EventId=@EventId,@SourceCompanyId=@SourceCompanyId,
            @EntityName=N'BusinessPartner',@EntityGlobalId=@EntityGlobalId,
            @Operation=@Operation,@PayloadJson=@PayloadJson,
            @InboxId=@InboxId OUTPUT,@InboxStatus=@InboxStatus OUTPUT,
            @EnvelopeResult=@InboxEnvelopeResult OUTPUT;
        IF @InboxEnvelopeResult=4
        BEGIN
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode;
            RETURN;
        END;
        IF @InboxStatus=N'Applied'
        BEGIN
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;
        SELECT @BusinessPartnerId=bp.Id,@CurrentVersion=bp.CanonicalVersion,@LiveCode=bp.Code,
               @LivePartnerType=bp.PartnerType,@LiveIdentificationTypeId=bp.IdentificationTypeId,
               @LiveIdentificationNumber=bp.IdentificationNumber,
               @LiveNormalizedIdentificationNumber=bp.NormalizedIdentificationNumber,
               @LiveMasterSyncStatus=bp.MasterSyncStatus,@LiveSapCardCode=mapping.SapCardCode
        FROM dbo.BusinessPartners AS bp WITH (UPDLOCK,HOLDLOCK)
        LEFT JOIN dbo.BusinessPartnerSapMapping AS mapping WITH (UPDLOCK,HOLDLOCK)
            ON mapping.BusinessPartnerId=bp.Id
        WHERE bp.GlobalId=@EntityGlobalId;

        IF @CurrentVersion > @CanonicalVersion
        BEGIN
            UPDATE dbo.SyncInbox SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 3 AS ResultCode;
            RETURN;
        END;

        IF @CurrentVersion = @CanonicalVersion
        BEGIN
            UPDATE dbo.SyncInbox SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;

        IF @BusinessPartnerId IS NOT NULL
           AND
           (
               @LivePartnerType=N'Both' OR @LiveMasterSyncStatus='LegacyReview'
               OR @LiveCode COLLATE Latin1_General_100_BIN2<>@Code COLLATE Latin1_General_100_BIN2
               OR @LivePartnerType COLLATE Latin1_General_100_BIN2<>@PartnerType COLLATE Latin1_General_100_BIN2
               OR @LiveIdentificationTypeId<>@IdentificationTypeId
               OR @LiveIdentificationNumber COLLATE Latin1_General_100_BIN2<>@IdentificationNumber COLLATE Latin1_General_100_BIN2
               OR @LiveNormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2<>@NormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2
               OR (NULLIF(LTRIM(RTRIM(@LiveSapCardCode)),N'') IS NOT NULL
                   AND NULLIF(LTRIM(RTRIM(@SapCardCode)),N'') IS NOT NULL
                   AND @LiveSapCardCode COLLATE Latin1_General_100_BIN2<>@SapCardCode COLLATE Latin1_General_100_BIN2)
           )
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status=N'DeadLetter',ErrorMessage=N'Canonical event conflicts with immutable BusinessPartner state.',
                LastErrorMessage=N'Canonical event conflicts with immutable BusinessPartner state.',NextRetryAt=NULL
            WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 5 AS ResultCode;
            RETURN;
        END;

        DECLARE @Saved table(BusinessPartnerId int NOT NULL);
        INSERT @Saved(BusinessPartnerId)
        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT
            @GlobalId=@EntityGlobalId,@Code=@Code,@Name=@Name,@CommercialName=@CommercialName,
            @PartnerType=@PartnerType,@IdentificationTypeId=@IdentificationTypeId,
            @IdentificationNumber=@IdentificationNumber,
            @NormalizedIdentificationNumber=@NormalizedIdentificationNumber,
            @Email=@Email,@Phone=@Phone,@SapCardCode=@SapCardCode,
            @CanonicalVersion=@CanonicalVersion,@IsActive=@IsActive,@IsDeleted=@IsDeleted,
            @AddressesJson=@AddressesJson,@ContactsJson=@ContactsJson,
            @AuditUserName=N'MasterBranchSyncWorker';

        UPDATE dbo.SyncInbox
        SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL
        WHERE Id=@InboxId;
        COMMIT TRANSACTION;
        SELECT 1 AS ResultCode;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY
    @EventId uniqueidentifier,
    @SourceCompanyId int,
    @EntityGlobalId uniqueidentifier,
    @PayloadJson nvarchar(max),
    @Status varchar(20),
    @Message nvarchar(500) = NULL,
    @CanonicalVersion bigint,
    @HasCanonical bit = 0,
    @Code nvarchar(50) = NULL,
    @Name nvarchar(200) = NULL,
    @CommercialName nvarchar(200) = NULL,
    @PartnerType nvarchar(20) = NULL,
    @IdentificationTypeId int = NULL,
    @IdentificationNumber nvarchar(50) = NULL,
    @NormalizedIdentificationNumber nvarchar(50) = NULL,
    @Email nvarchar(256) = NULL,
    @Phone nvarchar(50) = NULL,
    @SapCardCode nvarchar(50) = NULL,
    @IsActive bit = 1,
    @IsDeleted bit = 0,
    @AddressesJson nvarchar(max) = N'[]',
    @ContactsJson nvarchar(max) = N'[]'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @Status NOT IN ('Accepted','Rejected','Conflict')
        THROW 52030, 'Proposal result status is invalid.', 1;
    IF DATALENGTH(@SapCardCode) > 30
        THROW 52030, 'SapCardCode cannot exceed 15 characters.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@InboxEnvelopeResult int,
                @BusinessPartnerId int,@CurrentVersion bigint,@LiveCode nvarchar(50),
                @LivePartnerType nvarchar(20),@LiveIdentificationTypeId int,
                @LiveIdentificationNumber nvarchar(50),@LiveNormalizedIdentificationNumber nvarchar(50),
                @LiveMasterSyncStatus varchar(20),@LiveSapCardCode nvarchar(50);
        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE
            @EventId=@EventId,@SourceCompanyId=@SourceCompanyId,
            @EntityName=N'BusinessPartnerProposalResult',@EntityGlobalId=@EntityGlobalId,
            @Operation=N'Updated',@PayloadJson=@PayloadJson,
            @InboxId=@InboxId OUTPUT,@InboxStatus=@InboxStatus OUTPUT,
            @EnvelopeResult=@InboxEnvelopeResult OUTPUT;
        IF @InboxEnvelopeResult=4
        BEGIN
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode;
            RETURN;
        END;
        IF @InboxStatus=N'Applied'
        BEGIN
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;
        SELECT @BusinessPartnerId=Id,@CurrentVersion=CanonicalVersion,@LiveCode=Code,
               @LivePartnerType=PartnerType,@LiveIdentificationTypeId=IdentificationTypeId,
               @LiveIdentificationNumber=IdentificationNumber,
               @LiveNormalizedIdentificationNumber=NormalizedIdentificationNumber,
               @LiveMasterSyncStatus=MasterSyncStatus
        FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)
        WHERE GlobalId=@EntityGlobalId;
        IF @BusinessPartnerId IS NOT NULL
            SELECT @LiveSapCardCode=SapCardCode
            FROM dbo.BusinessPartnerSapMapping WITH (UPDLOCK,HOLDLOCK)
            WHERE BusinessPartnerId=@BusinessPartnerId;

        IF @CurrentVersion > @CanonicalVersion
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status=N'Ignored',AppliedAt=SYSUTCDATETIME(),
                ErrorMessage=NULL,LastErrorMessage=N'Stale canonical proposal result ignored.',NextRetryAt=NULL
            WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 3 AS ResultCode;
            RETURN;
        END;

        IF @HasCanonical=1 AND @CurrentVersion = @CanonicalVersion
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL
            WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;

        IF @HasCanonical=0 AND (@BusinessPartnerId IS NULL OR @CurrentVersion < @CanonicalVersion)
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status=N'DeadLetter',ErrorMessage=N'Proposal result advanced without a canonical snapshot.',
                LastErrorMessage=N'Proposal result advanced without a canonical snapshot.',NextRetryAt=NULL
            WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode;
            RETURN;
        END;

        IF @HasCanonical=1 AND @BusinessPartnerId IS NOT NULL
           AND
           (
               @LivePartnerType=N'Both' OR @LiveMasterSyncStatus='LegacyReview'
               OR @LiveCode COLLATE Latin1_General_100_BIN2<>@Code COLLATE Latin1_General_100_BIN2
               OR @LivePartnerType COLLATE Latin1_General_100_BIN2<>@PartnerType COLLATE Latin1_General_100_BIN2
               OR @LiveIdentificationTypeId<>@IdentificationTypeId
               OR @LiveIdentificationNumber COLLATE Latin1_General_100_BIN2<>@IdentificationNumber COLLATE Latin1_General_100_BIN2
               OR @LiveNormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2<>@NormalizedIdentificationNumber COLLATE Latin1_General_100_BIN2
               OR (NULLIF(LTRIM(RTRIM(@LiveSapCardCode)),N'') IS NOT NULL
                   AND NULLIF(LTRIM(RTRIM(@SapCardCode)),N'') IS NOT NULL
                   AND @LiveSapCardCode COLLATE Latin1_General_100_BIN2<>@SapCardCode COLLATE Latin1_General_100_BIN2)
           )
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status=N'DeadLetter',ErrorMessage=N'Proposal result conflicts with immutable BusinessPartner state.',
                LastErrorMessage=N'Proposal result conflicts with immutable BusinessPartner state.',NextRetryAt=NULL
            WHERE Id=@InboxId;
            COMMIT TRANSACTION;
            SELECT 5 AS ResultCode;
            RETURN;
        END;

        IF @HasCanonical=1
        BEGIN
            DECLARE @Saved table(BusinessPartnerId int NOT NULL);
            INSERT @Saved(BusinessPartnerId)
            EXEC dbo.SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT
                @GlobalId=@EntityGlobalId,@Code=@Code,@Name=@Name,@CommercialName=@CommercialName,
                @PartnerType=@PartnerType,@IdentificationTypeId=@IdentificationTypeId,
                @IdentificationNumber=@IdentificationNumber,
                @NormalizedIdentificationNumber=@NormalizedIdentificationNumber,
                @Email=@Email,@Phone=@Phone,@SapCardCode=@SapCardCode,
                @CanonicalVersion=@CanonicalVersion,@IsActive=@IsActive,@IsDeleted=@IsDeleted,
                @AddressesJson=@AddressesJson,@ContactsJson=@ContactsJson,
                @AuditUserName=N'MasterBranchSyncWorker';
        END;

        UPDATE dbo.BusinessPartners
        SET MasterSyncStatus=@Status,MasterSyncMessage=@Message,
            UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'MasterBranchSyncWorker'
        WHERE GlobalId=@EntityGlobalId;
        UPDATE dbo.SyncInbox
        SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL
        WHERE Id=@InboxId;
        COMMIT TRANSACTION;
        SELECT 1 AS ResultCode;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICTS_LISTAR
    @Status varchar(20) = 'Open'
AS
BEGIN
    SET NOCOUNT ON;
    SELECT conflict.Id AS Id,conflict.ProposalEventId AS ProposalEventId,
           conflict.BusinessPartnerId AS BusinessPartnerId,
           conflict.BusinessPartnerGlobalId AS BusinessPartnerGlobalId,
           conflict.OriginCompanyId AS OriginCompanyId,
           conflict.BaseCanonicalVersion AS BaseCanonicalVersion,
           conflict.CurrentCanonicalVersion AS CurrentCanonicalVersion,
           conflict.PresentedBusinessPartnerRowVersion AS PresentedBusinessPartnerRowVersion,
           conflict.ConflictFieldsJson AS ConflictFieldsJson,conflict.Status AS Status,
           conflict.Resolution AS Resolution,conflict.ResolutionReason AS ResolutionReason,
           conflict.CreatedByUserId AS CreatedByUserId,
           conflict.CreatedByUserName AS CreatedByUserName,
           conflict.CreatedAt AS CreatedAt,conflict.ResolvedByUserId AS ResolvedByUserId,
           conflict.ResolvedByUserName AS ResolvedByUserName,
           conflict.ResolvedAt AS ResolvedAt,conflict.RowVersion AS RowVersion,
           bp.Code AS Code,bp.Name AS Name
    FROM dbo.BusinessPartnerSyncConflicts AS conflict
    LEFT JOIN dbo.BusinessPartners AS bp ON bp.Id=conflict.BusinessPartnerId
    WHERE @Status IS NULL OR conflict.Status=@Status
    ORDER BY conflict.CreatedAt DESC,conflict.Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICT_BUSCARPORID
    @Id bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT conflict.Id AS Id,conflict.ProposalEventId AS ProposalEventId,
           conflict.BusinessPartnerId AS BusinessPartnerId,
           conflict.BusinessPartnerGlobalId AS BusinessPartnerGlobalId,
           conflict.OriginCompanyId AS OriginCompanyId,
           conflict.BaseCanonicalVersion AS BaseCanonicalVersion,
           conflict.CurrentCanonicalVersion AS CurrentCanonicalVersion,
           conflict.PresentedBusinessPartnerRowVersion AS PresentedBusinessPartnerRowVersion,
           conflict.BaseSnapshotJson AS BaseSnapshotJson,
           conflict.ProposedSnapshotJson AS ProposedSnapshotJson,
           conflict.CanonicalSnapshotJson AS CanonicalSnapshotJson,
           conflict.ConflictFieldsJson AS ConflictFieldsJson,
           conflict.Status AS Status,conflict.Resolution AS Resolution,
           conflict.ResolutionReason AS ResolutionReason,
           conflict.CreatedByUserId AS CreatedByUserId,
           conflict.CreatedByUserName AS CreatedByUserName,
           conflict.CreatedAt AS CreatedAt,conflict.ResolvedByUserId AS ResolvedByUserId,
           conflict.ResolvedByUserName AS ResolvedByUserName,
           conflict.ResolvedAt AS ResolvedAt,conflict.RowVersion AS RowVersion,
           bp.Code AS Code,bp.Name AS Name
    FROM dbo.BusinessPartnerSyncConflicts AS conflict
    LEFT JOIN dbo.BusinessPartners AS bp ON bp.Id=conflict.BusinessPartnerId
    WHERE conflict.Id=@Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER
    @Id bigint,
    @Resolution varchar(20),
    @ResolutionReason nvarchar(500),
    @ExpectedRowVersion binary(8),
    @CompanyId int,
    @ResolvedSnapshotJson nvarchar(max) = NULL,
    @AddressesJson nvarchar(max) = N'[]',
    @ContactsJson nvarchar(max) = N'[]',
    @OutboundEventId uniqueidentifier,
    @OutboundEntityName nvarchar(120),
    @OutboundPayloadJson nvarchar(max),
    @TargetCompanyId int = NULL,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @Resolution NOT IN ('AcceptBranch','KeepCentral')
        THROW 52030, 'Conflict resolution is invalid.', 1;
    IF NULLIF(LTRIM(RTRIM(@ResolutionReason)),N'') IS NULL
        THROW 52030, 'Conflict resolution reason is required.', 1;
    IF @ExpectedRowVersion IS NULL
        THROW 52030, 'Expected conflict row version is required.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @ProposalEventId uniqueidentifier,@BusinessPartnerId int,@GlobalId uniqueidentifier,
                @OriginCompanyId int,@CurrentVersion bigint,@Status varchar(20),
                @ActualRowVersion binary(8),@PresentedBusinessPartnerRowVersion binary(8),
                @LiveBusinessPartnerRowVersion binary(8),@LiveCanonicalVersion bigint,@LiveBusinessPartnerId int,
                @OutboxId bigint,@OutboxEnvelopeResult int,@ResolvedTargetCompanyId int;
        SELECT @ProposalEventId=ProposalEventId,@BusinessPartnerId=BusinessPartnerId,
               @GlobalId=BusinessPartnerGlobalId,@OriginCompanyId=OriginCompanyId,
               @CurrentVersion=CurrentCanonicalVersion,
               @PresentedBusinessPartnerRowVersion=PresentedBusinessPartnerRowVersion,
               @Status=Status,@ActualRowVersion=RowVersion
        FROM dbo.BusinessPartnerSyncConflicts WITH (UPDLOCK,HOLDLOCK)
        WHERE Id=@Id;
        IF @ProposalEventId IS NULL THROW 52030, 'BusinessPartner sync conflict was not found.', 1;
        IF @Status='Resolved'
        BEGIN
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode,@Id AS ConflictId;
            RETURN;
        END;
        IF @ActualRowVersion<>@ExpectedRowVersion
        BEGIN
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode,@Id AS ConflictId;
            RETURN;
        END;

        SELECT @LiveBusinessPartnerId=Id,@LiveCanonicalVersion=CanonicalVersion,
               @LiveBusinessPartnerRowVersion=RowVersion
        FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)
        WHERE GlobalId=@GlobalId;
        IF (@LiveBusinessPartnerId IS NULL AND @BusinessPartnerId IS NOT NULL)
           OR (@BusinessPartnerId IS NOT NULL AND @LiveBusinessPartnerId<>@BusinessPartnerId)
           OR (@LiveBusinessPartnerId IS NOT NULL AND @LiveCanonicalVersion <> @CurrentVersion)
           OR (@LiveBusinessPartnerId IS NOT NULL AND @PresentedBusinessPartnerRowVersion IS NULL)
           OR (@LiveBusinessPartnerId IS NOT NULL AND @LiveBusinessPartnerRowVersion<>@PresentedBusinessPartnerRowVersion)
        BEGIN
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode,@Id AS ConflictId;
            RETURN;
        END;
        SET @LiveCanonicalVersion=COALESCE(@LiveCanonicalVersion,0);
        SET @ResolvedTargetCompanyId=CASE WHEN @Resolution='KeepCentral'
            THEN COALESCE(@TargetCompanyId,@OriginCompanyId) ELSE @TargetCompanyId END;

        EXEC dbo.SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE
            @EventId=@OutboundEventId,@CompanyId=@CompanyId,
            @TargetCompanyId=@ResolvedTargetCompanyId,@CausationEventId=@ProposalEventId,
            @EntityName=@OutboundEntityName,@EntityGlobalId=@GlobalId,@EntityCode=NULL,
            @Operation=N'Updated',@PayloadJson=@OutboundPayloadJson,
            @OutboxId=@OutboxId OUTPUT,@EnvelopeResult=@OutboxEnvelopeResult OUTPUT;
        IF @OutboxEnvelopeResult=4
        BEGIN
            COMMIT TRANSACTION;
            SELECT 4 AS ResultCode,@Id AS ConflictId;
            RETURN;
        END;

        IF @Resolution='AcceptBranch'
        BEGIN
            IF ISJSON(@ResolvedSnapshotJson)<>1
                THROW 52030, 'Resolved BusinessPartner snapshot is required.', 1;
            DECLARE @Saved table(BusinessPartnerId int NOT NULL);
            INSERT @Saved(BusinessPartnerId)
            EXEC dbo.SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT
                @GlobalId=@GlobalId,
                @Code=JSON_VALUE(@ResolvedSnapshotJson,'$.code'),
                @Name=JSON_VALUE(@ResolvedSnapshotJson,'$.name'),
                @CommercialName=JSON_VALUE(@ResolvedSnapshotJson,'$.commercialName'),
                @PartnerType=JSON_VALUE(@ResolvedSnapshotJson,'$.partnerType'),
                @IdentificationTypeId=TRY_CONVERT(int,JSON_VALUE(@ResolvedSnapshotJson,'$.identificationTypeId')),
                @IdentificationNumber=JSON_VALUE(@ResolvedSnapshotJson,'$.identificationNumber'),
                @NormalizedIdentificationNumber=JSON_VALUE(@ResolvedSnapshotJson,'$.normalizedIdentificationNumber'),
                @Email=JSON_VALUE(@ResolvedSnapshotJson,'$.email'),
                @Phone=JSON_VALUE(@ResolvedSnapshotJson,'$.phone'),
                @SapCardCode=JSON_VALUE(@ResolvedSnapshotJson,'$.sapCardCode'),
                @CanonicalVersion=@LiveCanonicalVersion+1,
                @IsActive=ISNULL(TRY_CONVERT(bit,JSON_VALUE(@ResolvedSnapshotJson,'$.isActive')),1),
                @IsDeleted=ISNULL(TRY_CONVERT(bit,JSON_VALUE(@ResolvedSnapshotJson,'$.isDeleted')),0),
                @AddressesJson=@AddressesJson,@ContactsJson=@ContactsJson,
                @AuditUserId=@AuditUserId,@AuditUserName=@AuditUserName;
            SELECT @BusinessPartnerId=BusinessPartnerId FROM @Saved;
        END;

        UPDATE dbo.BusinessPartnerSyncConflicts
        SET BusinessPartnerId=@BusinessPartnerId,Status='Resolved',Resolution=@Resolution,
            ResolutionReason=@ResolutionReason,ResolvedByUserId=@AuditUserId,
            ResolvedByUserName=@AuditUserName,ResolvedAt=SYSUTCDATETIME()
        WHERE Id=@Id;
        UPDATE dbo.SyncInbox
        SET Status=N'Applied',AppliedAt=COALESCE(AppliedAt,SYSUTCDATETIME()),
            ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL
        WHERE EventId=@ProposalEventId;

        COMMIT TRANSACTION;
        SELECT 1 AS ResultCode,@Id AS ConflictId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260903.230')
BEGIN
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260903.230',N'BusinessPartner tenant proposal, canonical apply and conflict operations');
END;
GO
