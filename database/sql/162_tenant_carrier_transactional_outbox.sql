/*
    Iteracion 8.8 - Transportistas transaccional Matriz-Sucursal.

    - Transportistas es un maestro independiente de BusinessPartner y SAP.
    - GlobalId es la unica identidad entre tenants.
    - Code queda reservado incluso despues de eliminacion logica.
    - Create/Update/Delete y LocalOutbox comparten la transaccion tenant.
    - La aplicacion en sucursal es idempotente por EventId y GlobalId.
    - Una colision de Code con otro GlobalId es terminal, sin adopcion automatica.

    Ejecutar solo en bases tenant. Nunca en NuanSystem_Master.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() = N'NuanSystem_Master'
    THROW 51162, 'Migration 162 must run only in tenant databases.', 1;
IF OBJECT_ID(N'dbo.Carriers', N'U') IS NULL
    THROW 51162, 'Carriers is required before migration 162.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL
    THROW 51162, 'LocalOutbox is required before migration 162.', 1;
IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL
    THROW 51162, 'SyncInbox is required before migration 162.', 1;
IF OBJECT_ID(N'dbo.SyncAudit', N'U') IS NULL
    THROW 51162, 'SyncAudit is required before migration 162.', 1;
IF OBJECT_ID(N'dbo.AuditCatalogChanges', N'U') IS NULL
    THROW 51162, 'AuditCatalogChanges is required before migration 162.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51162, 'SchemaHistory is required before migration 162.', 1;
GO

IF EXISTS (SELECT Code FROM dbo.Carriers GROUP BY Code HAVING COUNT_BIG(1) > 1)
    THROW 51162, 'Carrier codes, including tombstones, must be unique before migration 162.', 1;
IF EXISTS (SELECT 1 FROM dbo.Carriers WHERE IdentificationTypeCode NOT IN (N'04', N'05', N'06'))
    THROW 51162, 'Carrier identification type must use SRI codes 04, 05 or 06.', 1;
GO

IF COL_LENGTH(N'dbo.Carriers', N'GlobalId') IS NULL
    ALTER TABLE dbo.Carriers ADD GlobalId uniqueidentifier NULL;
GO

UPDATE dbo.Carriers SET GlobalId = NEWID() WHERE GlobalId IS NULL;
GO

IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Carriers') AND name = N'GlobalId' AND is_nullable = 1
)
    ALTER TABLE dbo.Carriers ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints AS d
    INNER JOIN sys.columns AS c
        ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
    WHERE d.parent_object_id = OBJECT_ID(N'dbo.Carriers') AND c.name = N'GlobalId'
)
    ALTER TABLE dbo.Carriers ADD CONSTRAINT DF_Carriers_GlobalId DEFAULT NEWID() FOR GlobalId;
GO

IF EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Carriers') AND name = N'UX_Carriers_Code_ActiveRecord'
)
    DROP INDEX UX_Carriers_Code_ActiveRecord ON dbo.Carriers;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Carriers')
      AND name = N'UQ_Carriers_Code' AND is_unique = 1 AND filter_definition IS NULL
)
    CREATE UNIQUE INDEX UQ_Carriers_Code ON dbo.Carriers(Code);
GO

IF EXISTS (SELECT GlobalId FROM dbo.Carriers GROUP BY GlobalId HAVING COUNT_BIG(1) > 1)
    THROW 51162, 'Carrier GlobalId values must be unique before migration 162.', 1;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Carriers')
      AND name = N'UQ_Carriers_GlobalId' AND is_unique = 1 AND filter_definition IS NULL
)
    CREATE UNIQUE INDEX UQ_Carriers_GlobalId ON dbo.Carriers(GlobalId);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIERS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, GlobalId, Code, Name, IdentificationTypeCode, IdentificationNumber,
           Description, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt,
           UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.Carriers
    WHERE IsDeleted = 0
    ORDER BY Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIERS_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, GlobalId, Code, Name, IsActive
    FROM dbo.Carriers
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIERS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, GlobalId, Code, Name, IdentificationTypeCode, IdentificationNumber,
           Description, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt,
           UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.Carriers
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIERSBUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
    WHERE Code = @Code AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CARRIERS_CREAR
    @GlobalId uniqueidentifier,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @IdentificationTypeCode nvarchar(2),
    @IdentificationNumber nvarchar(30),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @GlobalId IS NULL OR @GlobalId = '00000000-0000-0000-0000-000000000000'
        THROW 51162, 'Carrier GlobalId is required.', 1;
    IF NULLIF(LTRIM(RTRIM(@Code)), N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(LTRIM(RTRIM(@Name)), N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @IdentificationTypeCode NOT IN (N'04', N'05', N'06') THROW 51000, 'Tipo de identificacion no permitido.', 1;
    IF NULLIF(LTRIM(RTRIM(@IdentificationNumber)), N'') IS NULL THROW 51001, 'La identificacion es obligatoria.', 1;

    DECLARE @OwnTransaction bit = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END;
    BEGIN TRY
        IF @OwnTransaction = 1 BEGIN TRANSACTION;

        IF EXISTS
        (
            SELECT 1 FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
            WHERE Code = @Code OR GlobalId = @GlobalId
        )
        BEGIN
            IF @OwnTransaction = 1 COMMIT TRANSACTION;
            SELECT -1;
            RETURN;
        END;

        INSERT dbo.Carriers
        (
            GlobalId, Code, Name, IdentificationTypeCode, IdentificationNumber,
            Description, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @GlobalId, @Code, @Name, @IdentificationTypeCode, @IdentificationNumber,
            @Description, @IsActive, @AuditUserId, @AuditUserName, SYSUTCDATETIME()
        );

        DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());
        INSERT dbo.AuditCatalogChanges
            (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        SELECT N'Carrier', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL,
               NewValue, @AuditUserId, @AuditUserName
        FROM (VALUES
            (N'Code', CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @Name)),
            (N'IdentificationTypeCode', CONVERT(nvarchar(max), @IdentificationTypeCode)),
            (N'IdentificationNumber', CONVERT(nvarchar(max), @IdentificationNumber)),
            (N'Description', CONVERT(nvarchar(max), @Description)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))) changes(FieldName, NewValue);

        IF @OwnTransaction = 1 COMMIT TRANSACTION;
        SELECT @Id;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SELECT -1;
            RETURN;
        END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CARRIERS_ACTUALIZAR
    @Id int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @IdentificationTypeCode nvarchar(2),
    @IdentificationNumber nvarchar(30),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NULLIF(LTRIM(RTRIM(@Code)), N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(LTRIM(RTRIM(@Name)), N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @IdentificationTypeCode NOT IN (N'04', N'05', N'06') THROW 51000, 'Tipo de identificacion no permitido.', 1;
    IF NULLIF(LTRIM(RTRIM(@IdentificationNumber)), N'') IS NULL THROW 51001, 'La identificacion es obligatoria.', 1;

    DECLARE @OwnTransaction bit = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END;
    BEGIN TRY
        IF @OwnTransaction = 1 BEGIN TRANSACTION;
        DECLARE @OldCode nvarchar(50), @OldName nvarchar(150), @OldType nvarchar(2),
                @OldNumber nvarchar(30), @OldDescription nvarchar(500), @OldIsActive bit;

        SELECT @OldCode = Code, @OldName = Name, @OldType = IdentificationTypeCode,
               @OldNumber = IdentificationNumber, @OldDescription = Description,
               @OldIsActive = IsActive
        FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @Id AND IsDeleted = 0;

        IF @OldCode IS NULL
        BEGIN
            IF @OwnTransaction = 1 COMMIT TRANSACTION;
            SELECT 0;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
            WHERE Code = @Code AND Id <> @Id
        )
        BEGIN
            IF @OwnTransaction = 1 COMMIT TRANSACTION;
            SELECT -1;
            RETURN;
        END;

        UPDATE dbo.Carriers
        SET Code = @Code, Name = @Name, IdentificationTypeCode = @IdentificationTypeCode,
            IdentificationNumber = @IdentificationNumber, Description = @Description,
            IsActive = @IsActive, UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @Id AND IsDeleted = 0;

        IF @@ROWCOUNT = 0
        BEGIN
            IF @OwnTransaction = 1 COMMIT TRANSACTION;
            SELECT 0;
            RETURN;
        END;

        INSERT dbo.AuditCatalogChanges
            (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        SELECT N'Carrier', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName,
               OldValue, NewValue, @AuditUserId, @AuditUserName
        FROM (VALUES
            (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
            (N'IdentificationTypeCode', CONVERT(nvarchar(max), @OldType), CONVERT(nvarchar(max), @IdentificationTypeCode)),
            (N'IdentificationNumber', CONVERT(nvarchar(max), @OldNumber), CONVERT(nvarchar(max), @IdentificationNumber)),
            (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive))))
            changes(FieldName, OldValue, NewValue)
        WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

        IF @OwnTransaction = 1 COMMIT TRANSACTION;
        SELECT 1;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SELECT -1;
            RETURN;
        END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_CARRIERS_ELIMINAR
    @Id int,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @OwnTransaction bit = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END;

    BEGIN TRY
        IF @OwnTransaction = 1 BEGIN TRANSACTION;
        DECLARE @OldIsActive bit;
        SELECT @OldIsActive = IsActive
        FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @Id AND IsDeleted = 0;

        IF @OldIsActive IS NULL
        BEGIN
            IF @OwnTransaction = 1 COMMIT TRANSACTION;
            SELECT 0;
            RETURN;
        END;

        UPDATE dbo.Carriers
        SET IsActive = 0, IsDeleted = 1, DeletedByUserId = @AuditUserId,
            DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
        WHERE Id = @Id AND IsDeleted = 0;

        IF @@ROWCOUNT = 0
        BEGIN
            IF @OwnTransaction = 1 COMMIT TRANSACTION;
            SELECT 0;
            RETURN;
        END;

        INSERT dbo.AuditCatalogChanges
            (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES
            (N'Carrier', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), N'0', @AuditUserId, @AuditUserName),
            (N'Carrier', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @AuditUserId, @AuditUserName);

        IF @OwnTransaction = 1 COMMIT TRANSACTION;
        SELECT 1;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CARRIER_SYNC_FULL
    @AfterId int = NULL,
    @BatchSize int = 100
AS
BEGIN
    SET NOCOUNT ON;
    IF @BatchSize < 1 OR @BatchSize > 10001
        THROW 51162, 'Carrier Full BatchSize must be between 1 and 10001.', 1;

    SELECT TOP (@BatchSize)
           Id, GlobalId, Code, Name, IdentificationTypeCode, IdentificationNumber,
           Description, IsActive, IsDeleted, CreatedAt, UpdatedAt
    FROM dbo.Carriers
    WHERE (@AfterId IS NULL OR Id > @AfterId)
    ORDER BY Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CARRIER_SYNC_APPLY_EVENT
    @EventId uniqueidentifier,
    @SourceCompanyId int,
    @EntityName nvarchar(80),
    @EntityGlobalId uniqueidentifier,
    @Operation nvarchar(30),
    @PayloadJson nvarchar(max),
    @GlobalId uniqueidentifier,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @IdentificationTypeCode nvarchar(2),
    @IdentificationNumber nvarchar(30),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @IsDeleted bit,
    @CreatedAt datetime2(0),
    @UpdatedAt datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @InboxId bigint, @InboxStatus nvarchar(30), @CarrierId int;
        DECLARE @OldCode nvarchar(50), @OldName nvarchar(150), @OldType nvarchar(2),
                @OldNumber nvarchar(30), @OldDescription nvarchar(500),
                @OldIsActive bit, @OldIsDeleted bit;

        SELECT @InboxId = Id, @InboxStatus = Status
        FROM dbo.SyncInbox WITH (UPDLOCK, HOLDLOCK)
        WHERE EventId = @EventId;

        IF @InboxStatus = N'Applied'
        BEGIN
            SELECT @CarrierId = Id FROM dbo.Carriers WHERE GlobalId = @GlobalId;
            COMMIT TRANSACTION;
            SELECT 2 AS ResultCode, @CarrierId AS CarrierId;
            RETURN;
        END;

        IF @InboxStatus = N'DeadLetter'
        BEGIN
            COMMIT TRANSACTION;
            SELECT -2 AS ResultCode, CONVERT(int, NULL) AS CarrierId;
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

        IF @EntityName <> N'Carrier' OR @EntityGlobalId <> @GlobalId
            THROW 51162, 'Carrier event identity contract is invalid.', 1;

        IF @IdentificationTypeCode NOT IN (N'04', N'05', N'06')
            THROW 51162, 'Carrier identification type is invalid.', 1;

        IF EXISTS
        (
            SELECT 1 FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
            WHERE Code = @Code AND GlobalId <> @GlobalId
        )
        BEGIN
            UPDATE dbo.SyncInbox
            SET Status = N'DeadLetter', AttemptCount = AttemptCount + 1,
                ErrorMessage = N'Carrier code belongs to another GlobalId.',
                LastErrorMessage = N'Carrier code belongs to another GlobalId.',
                NextRetryAt = NULL
            WHERE Id = @InboxId;

            INSERT dbo.SyncAudit
                (CompanyId, EventId, EntityName, EntityGlobalId, [Action], PreviousStatus,
                 NewStatus, [Message], ErrorCode, CreatedBy)
            VALUES
                (@SourceCompanyId, @EventId, N'Carrier', @GlobalId, N'DeadLetter',
                 @InboxStatus, N'DeadLetter', N'Carrier code conflict; no automatic adoption.',
                 N'SYNC_CARRIER_CODE_CONFLICT', N'MasterBranchSyncWorker');

            COMMIT TRANSACTION;
            SELECT -2 AS ResultCode, CONVERT(int, NULL) AS CarrierId;
            RETURN;
        END;

        SELECT @CarrierId = Id, @OldCode = Code, @OldName = Name,
               @OldType = IdentificationTypeCode, @OldNumber = IdentificationNumber,
               @OldDescription = Description, @OldIsActive = IsActive,
               @OldIsDeleted = IsDeleted
        FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
        WHERE GlobalId = @GlobalId;

        IF @CarrierId IS NULL
        BEGIN
            INSERT dbo.Carriers
            (
                GlobalId, Code, Name, IdentificationTypeCode, IdentificationNumber,
                Description, IsActive, IsDeleted, CreatedAt, CreatedByUserName,
                DeletedAt, DeletedByUserName
            )
            VALUES
            (
                @GlobalId, @Code, @Name, @IdentificationTypeCode, @IdentificationNumber,
                @Description, @IsActive, @IsDeleted, COALESCE(@CreatedAt, SYSUTCDATETIME()),
                N'MasterBranchSyncWorker',
                CASE WHEN @IsDeleted = 1 THEN COALESCE(@UpdatedAt, SYSUTCDATETIME()) END,
                CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' END
            );
            SET @CarrierId = CONVERT(int, SCOPE_IDENTITY());

            INSERT dbo.AuditCatalogChanges
                (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserName, [Source])
            SELECT N'Carrier', CONVERT(nvarchar(80), @CarrierId), N'INSERT', FieldName,
                   NULL, NewValue, N'MasterBranchSyncWorker', N'MasterBranchSyncWorker'
            FROM (VALUES
                (N'Code', CONVERT(nvarchar(max), @Code)),
                (N'Name', CONVERT(nvarchar(max), @Name)),
                (N'IdentificationTypeCode', CONVERT(nvarchar(max), @IdentificationTypeCode)),
                (N'IdentificationNumber', CONVERT(nvarchar(max), @IdentificationNumber)),
                (N'Description', CONVERT(nvarchar(max), @Description)),
                (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
                (N'IsDeleted', CONVERT(nvarchar(max), CONVERT(int, @IsDeleted)))) changes(FieldName, NewValue);
        END
        ELSE
        BEGIN
            UPDATE dbo.Carriers
            SET Code = @Code, Name = @Name, IdentificationTypeCode = @IdentificationTypeCode,
                IdentificationNumber = @IdentificationNumber, Description = @Description,
                IsActive = @IsActive, IsDeleted = @IsDeleted,
                UpdatedAt = COALESCE(@UpdatedAt, SYSUTCDATETIME()),
                UpdatedByUserName = N'MasterBranchSyncWorker',
                DeletedAt = CASE WHEN @IsDeleted = 1 THEN COALESCE(DeletedAt, @UpdatedAt, SYSUTCDATETIME()) ELSE NULL END,
                DeletedByUserName = CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END
            WHERE Id = @CarrierId;

            INSERT dbo.AuditCatalogChanges
                (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserName, [Source])
            SELECT N'Carrier', CONVERT(nvarchar(80), @CarrierId),
                   CASE WHEN @IsDeleted = 1 AND @OldIsDeleted = 0 THEN N'DELETE' ELSE N'UPDATE' END,
                   FieldName, OldValue, NewValue, N'MasterBranchSyncWorker', N'MasterBranchSyncWorker'
            FROM (VALUES
                (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
                (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
                (N'IdentificationTypeCode', CONVERT(nvarchar(max), @OldType), CONVERT(nvarchar(max), @IdentificationTypeCode)),
                (N'IdentificationNumber', CONVERT(nvarchar(max), @OldNumber), CONVERT(nvarchar(max), @IdentificationNumber)),
                (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
                (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
                (N'IsDeleted', CONVERT(nvarchar(max), CONVERT(int, @OldIsDeleted)), CONVERT(nvarchar(max), CONVERT(int, @IsDeleted))))
                changes(FieldName, OldValue, NewValue)
            WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');
        END;

        UPDATE dbo.SyncInbox
        SET Status = N'Applied', AppliedAt = SYSUTCDATETIME(),
            ErrorMessage = NULL, LastErrorMessage = NULL, NextRetryAt = NULL
        WHERE Id = @InboxId;

        INSERT dbo.SyncAudit
            (CompanyId, EventId, EntityName, EntityGlobalId, [Action], PreviousStatus,
             NewStatus, [Message], CreatedBy)
        VALUES
            (@SourceCompanyId, @EventId, N'Carrier', @GlobalId, N'Applied',
             COALESCE(@InboxStatus, N'Pending'), N'Applied',
             N'Carrier event applied by GlobalId.', N'MasterBranchSyncWorker');

        COMMIT TRANSACTION;
        SELECT 1 AS ResultCode, @CarrierId AS CarrierId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260801.162')
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES
    (
        N'20260801.162',
        N'Transportistas transaccional Matriz-Sucursal por GlobalId sin adopcion por codigo'
    );
END;
GO
