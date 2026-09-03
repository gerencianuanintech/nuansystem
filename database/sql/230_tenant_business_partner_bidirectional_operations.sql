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
    @SapCardCode nvarchar(15) = NULL,
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

    IF EXISTS
    (
        SELECT 1 FROM dbo.BusinessPartners WITH (UPDLOCK, HOLDLOCK)
        WHERE GlobalId <> @GlobalId
          AND PartnerType = @PartnerType
          AND IdentificationTypeId = @IdentificationTypeId
          AND NormalizedIdentificationNumber = @NormalizedIdentificationNumber
          AND IsDeleted = 0 AND IsActive = 1
    )
        THROW 52030, 'Canonical identification belongs to another BusinessPartner.', 1;

    DECLARE @BusinessPartnerId int;
    SELECT @BusinessPartnerId = Id
    FROM dbo.BusinessPartners WITH (UPDLOCK, HOLDLOCK)
    WHERE GlobalId = @GlobalId;

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
        UPDATE dbo.BusinessPartnerSapMapping
        SET SapCardCode = @SapCardCode
        WHERE BusinessPartnerId = @BusinessPartnerId;
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

    SELECT bp.*, mapping.SapCardCode
    FROM dbo.BusinessPartners AS bp WITH (UPDLOCK, HOLDLOCK)
    LEFT JOIN dbo.BusinessPartnerSapMapping AS mapping WITH (UPDLOCK, HOLDLOCK)
        ON mapping.BusinessPartnerId = bp.Id
    WHERE bp.GlobalId = @GlobalId;

    SELECT addressItem.*
    FROM dbo.BusinessPartnerAddresses AS addressItem WITH (UPDLOCK, HOLDLOCK)
    INNER JOIN dbo.BusinessPartners AS bp ON bp.Id = addressItem.BusinessPartnerId
    WHERE bp.GlobalId = @GlobalId
    ORDER BY addressItem.Id;

    SELECT contactItem.*
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
    @SapCardCode nvarchar(15) = NULL,
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
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @InboxId bigint, @InboxStatus nvarchar(30), @BusinessPartnerId int;
        SELECT @InboxId = Id, @InboxStatus = Status
        FROM dbo.SyncInbox WITH (UPDLOCK, HOLDLOCK)
        WHERE EventId = @ProposalEventId;

        IF @InboxStatus = N'Applied'
        BEGIN
            SELECT @BusinessPartnerId = Id FROM dbo.BusinessPartners WHERE GlobalId = @BusinessPartnerGlobalId;
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode, @BusinessPartnerId AS BusinessPartnerId, @CanonicalVersion AS CanonicalVersion;
            RETURN;
        END;

        IF @InboxId IS NULL
        BEGIN
            INSERT dbo.SyncInbox
                (EventId, SourceCompanyId, EntityName, EntityGlobalId, Operation, PayloadJson, Status)
            VALUES
                (@ProposalEventId, @SourceCompanyId, N'BusinessPartnerProposal',
                 @BusinessPartnerGlobalId, @Operation, @ProposalPayloadJson, N'Pending');
            SET @InboxId = CONVERT(bigint, SCOPE_IDENTITY());
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

        IF NOT EXISTS (SELECT 1 FROM dbo.LocalOutbox WITH (UPDLOCK, HOLDLOCK) WHERE EventId=@CanonicalEventId)
            INSERT dbo.LocalOutbox
            (
                EventId, CompanyId, TargetCompanyId, CausationEventId, EntityName,
                EntityGlobalId, EntityCode, Operation, PayloadJson, Status
            )
            VALUES
            (
                @CanonicalEventId, @CompanyId, NULL, @ProposalEventId, N'BusinessPartner',
                @BusinessPartnerGlobalId, @Code, @Operation, @CanonicalPayloadJson, N'Pending'
            );

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

        DECLARE @InboxId bigint,@InboxStatus nvarchar(30);
        SELECT @InboxId=Id,@InboxStatus=Status FROM dbo.SyncInbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@ProposalEventId;
        IF @InboxStatus=N'Applied'
        BEGIN
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;
        IF @InboxId IS NULL
        BEGIN
            INSERT dbo.SyncInbox
                (EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
            VALUES
                (@ProposalEventId,@SourceCompanyId,N'BusinessPartnerProposal',@BusinessPartnerGlobalId,
                 N'Updated',@ProposedSnapshotJson,N'Pending');
            SET @InboxId=CONVERT(bigint,SCOPE_IDENTITY());
        END;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.BusinessPartnerSyncConflicts WITH (UPDLOCK,HOLDLOCK)
            WHERE ProposalEventId=@ProposalEventId
        )
            INSERT dbo.BusinessPartnerSyncConflicts
            (
                ProposalEventId,BusinessPartnerId,BusinessPartnerGlobalId,OriginCompanyId,
                BaseCanonicalVersion,CurrentCanonicalVersion,BaseSnapshotJson,
                ProposedSnapshotJson,CanonicalSnapshotJson,ConflictFieldsJson,
                CreatedByUserId,CreatedByUserName
            )
            VALUES
            (
                @ProposalEventId,@BusinessPartnerId,@BusinessPartnerGlobalId,@SourceCompanyId,
                @BaseCanonicalVersion,@CurrentCanonicalVersion,@BaseSnapshotJson,
                @ProposedSnapshotJson,@CanonicalSnapshotJson,@ConflictFieldsJson,
                @AuditUserId,@AuditUserName
            );

        UPDATE dbo.SyncInbox
        SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL
        WHERE Id=@InboxId;

        IF NOT EXISTS (SELECT 1 FROM dbo.LocalOutbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@ResultEventId)
            INSERT dbo.LocalOutbox
                (EventId,CompanyId,TargetCompanyId,CausationEventId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
            VALUES
                (@ResultEventId,@CompanyId,@SourceCompanyId,@ProposalEventId,N'BusinessPartnerProposalResult',
                 @BusinessPartnerGlobalId,NULL,N'Updated',@ResultPayloadJson,N'Pending');

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
        DECLARE @InboxId bigint,@InboxStatus nvarchar(30);
        SELECT @InboxId=Id,@InboxStatus=Status FROM dbo.SyncInbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@ProposalEventId;
        IF @InboxStatus=N'Applied'
        BEGIN
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;
        IF @InboxId IS NULL
        BEGIN
            INSERT dbo.SyncInbox
                (EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
            VALUES
                (@ProposalEventId,@SourceCompanyId,N'BusinessPartnerProposal',@BusinessPartnerGlobalId,
                 N'Updated',@ProposalPayloadJson,N'Pending');
            SET @InboxId=CONVERT(bigint,SCOPE_IDENTITY());
        END;
        UPDATE dbo.SyncInbox
        SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL
        WHERE Id=@InboxId;

        IF NOT EXISTS (SELECT 1 FROM dbo.LocalOutbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@ResultEventId)
            INSERT dbo.LocalOutbox
                (EventId,CompanyId,TargetCompanyId,CausationEventId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
            VALUES
                (@ResultEventId,@CompanyId,@SourceCompanyId,@ProposalEventId,N'BusinessPartnerProposalResult',
                 @BusinessPartnerGlobalId,NULL,N'Updated',@ResultPayloadJson,N'Pending');
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
    @SapCardCode nvarchar(15) = NULL,
    @CanonicalVersion bigint,
    @IsActive bit,
    @IsDeleted bit,
    @AddressesJson nvarchar(max),
    @ContactsJson nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@CurrentVersion bigint,@BusinessPartnerId int;
        SELECT @InboxId=Id,@InboxStatus=Status FROM dbo.SyncInbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@EventId;
        IF @InboxStatus=N'Applied'
        BEGIN
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;
        IF @InboxId IS NULL
        BEGIN
            INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
            VALUES(@EventId,@SourceCompanyId,N'BusinessPartner',@EntityGlobalId,@Operation,@PayloadJson,N'Pending');
            SET @InboxId=CONVERT(bigint,SCOPE_IDENTITY());
        END;

        SELECT @BusinessPartnerId=Id,@CurrentVersion=CanonicalVersion
        FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)
        WHERE GlobalId=@EntityGlobalId;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.BusinessPartners
            WHERE GlobalId=@EntityGlobalId
              AND CanonicalVersion > @CanonicalVersion
        )
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
    @SapCardCode nvarchar(15) = NULL,
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
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@BusinessPartnerId int;
        SELECT @InboxId=Id,@InboxStatus=Status FROM dbo.SyncInbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@EventId;
        IF @InboxStatus=N'Applied'
        BEGIN
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode;
            RETURN;
        END;
        IF @InboxId IS NULL
        BEGIN
            INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
            VALUES(@EventId,@SourceCompanyId,N'BusinessPartnerProposalResult',@EntityGlobalId,N'Updated',@PayloadJson,N'Pending');
            SET @InboxId=CONVERT(bigint,SCOPE_IDENTITY());
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
            CanonicalVersion=CASE WHEN @HasCanonical=1 THEN @CanonicalVersion ELSE CanonicalVersion END,
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
    SELECT conflict.Id,conflict.ProposalEventId,conflict.BusinessPartnerId,
           conflict.BusinessPartnerGlobalId,conflict.OriginCompanyId,
           conflict.BaseCanonicalVersion,conflict.CurrentCanonicalVersion,
           conflict.ConflictFieldsJson,conflict.Status,conflict.Resolution,
           conflict.ResolutionReason,conflict.CreatedByUserId,conflict.CreatedByUserName,
           conflict.CreatedAt,conflict.ResolvedByUserId,conflict.ResolvedByUserName,
           conflict.ResolvedAt,conflict.RowVersion,bp.Code,bp.Name
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
    SELECT conflict.*,bp.Code,bp.Name
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
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @ProposalEventId uniqueidentifier,@BusinessPartnerId int,@GlobalId uniqueidentifier,
                @OriginCompanyId int,@CurrentVersion bigint,@Status varchar(20),@ActualRowVersion binary(8);
        SELECT @ProposalEventId=ProposalEventId,@BusinessPartnerId=BusinessPartnerId,
               @GlobalId=BusinessPartnerGlobalId,@OriginCompanyId=OriginCompanyId,
               @CurrentVersion=CurrentCanonicalVersion,@Status=Status,@ActualRowVersion=RowVersion
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
            THROW 52030, 'BusinessPartner sync conflict has changed.', 1;

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
                @CanonicalVersion=@CurrentVersion+1,
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

        IF NOT EXISTS (SELECT 1 FROM dbo.LocalOutbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@OutboundEventId)
            INSERT dbo.LocalOutbox
                (EventId,CompanyId,TargetCompanyId,CausationEventId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
            VALUES
                (@OutboundEventId,@CompanyId,
                 CASE WHEN @Resolution='KeepCentral' THEN COALESCE(@TargetCompanyId,@OriginCompanyId) ELSE @TargetCompanyId END,
                 @ProposalEventId,@OutboundEntityName,@GlobalId,N'Updated',@OutboundPayloadJson,N'Pending');
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
