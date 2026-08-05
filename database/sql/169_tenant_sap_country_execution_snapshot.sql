/*
    Agrega CountryV1 al contrato tipado de snapshots de ejecución SAP.
    Ejecutar en tenants fuente SAP después de 153 y 158.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SapSyncExecutionDetails', N'U') IS NULL
    THROW 51169, 'Migration 153 is required before migration 169.', 1;
IF OBJECT_ID(N'dbo.SchemaVersions', N'U') IS NULL
    THROW 51169, 'SchemaVersions is required before migration 169.', 1;
GO

IF EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.SapSyncExecutionDetails')
      AND name = N'CK_SapSyncExecutionDetails_ApprovedSnapshotType'
)
BEGIN
    ALTER TABLE dbo.SapSyncExecutionDetails
        DROP CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType;
END;
GO

ALTER TABLE dbo.SapSyncExecutionDetails WITH CHECK
ADD CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType CHECK
(
    ApprovedSnapshotType IS NULL
    OR ApprovedSnapshotType IN
       ('SupplierV1', 'ItemV1', 'PaymentTermV1', 'WarehouseV1', 'CountryV1')
);
GO

ALTER TABLE dbo.SapSyncExecutionDetails
    CHECK CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLEGUARDAR
    @Id bigint = NULL,
    @ExecutionUid uniqueidentifier,
    @SourceRecordKey nvarchar(120),
    @SourceVersion nvarchar(120) = NULL,
    @LocalEntityId bigint = NULL,
    @LocalGlobalId uniqueidentifier = NULL,
    @Action varchar(20),
    @Status varchar(30),
    @AttemptCount int,
    @MaxAttempts int,
    @NextAttemptAtUtc datetime2(0) = NULL,
    @ErrorClass varchar(20) = NULL,
    @ResultCode nvarchar(120) = NULL,
    @SafeMessage nvarchar(1000) = NULL,
    @ApprovedSnapshotType varchar(40) = NULL,
    @ApprovedSnapshotJson nvarchar(max) = NULL,
    @SnapshotHash binary(32) = NULL,
    @StartedAtUtc datetime2(0) = NULL,
    @FinishedAtUtc datetime2(0) = NULL,
    @RowVersion varbinary(8) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @SourceRecordKey = NULLIF(LTRIM(RTRIM(@SourceRecordKey)), N'');
    IF @SourceRecordKey IS NULL
       OR (@ApprovedSnapshotType IS NULL AND (@ApprovedSnapshotJson IS NOT NULL OR @SnapshotHash IS NOT NULL))
       OR (@ApprovedSnapshotType IS NOT NULL AND (@ApprovedSnapshotJson IS NULL OR @SnapshotHash IS NULL))
       OR (@ApprovedSnapshotJson IS NOT NULL AND ISJSON(@ApprovedSnapshotJson) <> 1)
        THROW 51169, 'Invalid SAP execution detail snapshot contract.', 1;

    IF @ApprovedSnapshotJson IS NOT NULL
       AND
       (
           LOWER(@ApprovedSnapshotJson) LIKE N'%password%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%token%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%cookie%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%authorization%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%connectionstring%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%b1session%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%routeid%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%login%'
       )
        THROW 51169, 'Sensitive keys are forbidden in ApprovedSnapshotJson.', 1;

    IF @ApprovedSnapshotJson IS NOT NULL
       AND
       (
           @ApprovedSnapshotType NOT IN
               ('SupplierV1', 'ItemV1', 'PaymentTermV1', 'WarehouseV1', 'CountryV1')
           OR EXISTS
              (
                  SELECT 1
                  FROM OPENJSON(@ApprovedSnapshotJson) property
                  WHERE
                      (@ApprovedSnapshotType = 'CountryV1'
                       AND property.[key] NOT IN ('countryCode', 'countryName', 'iso2', 'iso3'))
                      OR
                      (@ApprovedSnapshotType = 'WarehouseV1'
                       AND property.[key] NOT IN ('warehouseCode', 'warehouseName', 'street', 'city', 'province', 'country', 'isActive'))
                      OR
                      (@ApprovedSnapshotType = 'SupplierV1'
                       AND property.[key] NOT IN ('cardCode', 'cardName', 'taxIdentification', 'cardType', 'groupCode', 'phone', 'email', 'currency', 'isActive', 'createdAt', 'updatedAt'))
                      OR
                      (@ApprovedSnapshotType = 'ItemV1'
                       AND property.[key] NOT IN ('itemCode', 'itemName', 'itemGroupCode', 'inventoryUnitCode', 'purchaseUnitCode', 'salesUnitCode', 'barcode', 'purchaseTaxCode', 'salesTaxCode', 'isPurchaseItem', 'isSalesItem', 'isInventoryItem', 'manageSerialNumbers', 'manageBatchNumbers', 'itemType', 'isActive'))
                      OR
                      (@ApprovedSnapshotType = 'PaymentTermV1'
                       AND property.[key] NOT IN ('groupNumber', 'name', 'additionalDays', 'additionalMonths', 'numberOfInstallments'))
              )
       )
        THROW 51169, 'ApprovedSnapshotJson contains fields outside its typed allowlist.', 1;

    DECLARE @ExecutionId bigint =
    (
        SELECT Id FROM dbo.SapSyncExecutions WHERE ExecutionUid = @ExecutionUid
    );
    IF @ExecutionId IS NULL
    BEGIN
        SELECT CAST(NULL AS bigint) AS Id,
               N'ExecutionNotFound' AS ResultCode,
               CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    DECLARE @DetailId bigint =
    (
        SELECT Id
        FROM dbo.SapSyncExecutionDetails
        WHERE SapSyncExecutionId = @ExecutionId
          AND SourceRecordKey = @SourceRecordKey
    );
    DECLARE @WasCreated bit = 0;

    BEGIN TRANSACTION;

    IF @DetailId IS NULL
    BEGIN
        INSERT dbo.SapSyncExecutionDetails
        (
            SapSyncExecutionId, SourceRecordKey, SourceVersion,
            LocalEntityId, LocalGlobalId, Action, Status,
            AttemptCount, MaxAttempts, NextAttemptAtUtc, ErrorClass,
            ResultCode, SafeMessage, ApprovedSnapshotType,
            ApprovedSnapshotJson, SnapshotHash, StartedAtUtc, FinishedAtUtc
        )
        VALUES
        (
            @ExecutionId, @SourceRecordKey, NULLIF(LTRIM(RTRIM(@SourceVersion)), N''),
            @LocalEntityId, @LocalGlobalId, @Action, @Status,
            @AttemptCount, @MaxAttempts, @NextAttemptAtUtc, @ErrorClass,
            NULLIF(LTRIM(RTRIM(@ResultCode)), N''), NULLIF(LTRIM(RTRIM(@SafeMessage)), N''),
            @ApprovedSnapshotType, @ApprovedSnapshotJson, @SnapshotHash,
            @StartedAtUtc, @FinishedAtUtc
        );
        SET @DetailId = SCOPE_IDENTITY();
        SET @WasCreated = 1;

        INSERT dbo.AuditSapSyncExecutionChanges
        (
            SapSyncExecutionId, SapSyncExecutionDetailId, Action, NewStatus
        )
        VALUES(@ExecutionId, @DetailId, 'DetailCreated', @Status);
    END
    ELSE
    BEGIN
        DECLARE @PreviousStatus varchar(30) =
        (
            SELECT Status FROM dbo.SapSyncExecutionDetails WHERE Id = @DetailId
        );

        UPDATE dbo.SapSyncExecutionDetails WITH (UPDLOCK)
        SET SourceVersion = NULLIF(LTRIM(RTRIM(@SourceVersion)), N''),
            LocalEntityId = @LocalEntityId,
            LocalGlobalId = @LocalGlobalId,
            Action = @Action,
            Status = @Status,
            AttemptCount = @AttemptCount,
            MaxAttempts = @MaxAttempts,
            NextAttemptAtUtc = @NextAttemptAtUtc,
            ErrorClass = @ErrorClass,
            ResultCode = NULLIF(LTRIM(RTRIM(@ResultCode)), N''),
            SafeMessage = NULLIF(LTRIM(RTRIM(@SafeMessage)), N''),
            ApprovedSnapshotType = @ApprovedSnapshotType,
            ApprovedSnapshotJson = @ApprovedSnapshotJson,
            SnapshotHash = @SnapshotHash,
            StartedAtUtc = @StartedAtUtc,
            FinishedAtUtc = @FinishedAtUtc,
            WorkerInstance = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN WorkerInstance ELSE NULL END,
            OwnerToken = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN OwnerToken ELSE NULL END,
            LockedAtUtc = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN LockedAtUtc ELSE NULL END,
            RenewedAtUtc = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN RenewedAtUtc ELSE NULL END,
            LockExpiresAtUtc = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN LockExpiresAtUtc ELSE NULL END,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @DetailId
          AND (@RowVersion IS NULL OR RowVersion = @RowVersion);

        IF @@ROWCOUNT = 0
        BEGIN
            ROLLBACK;
            SELECT @DetailId AS Id,
                   N'ConcurrencyConflict' AS ResultCode,
                   CAST(NULL AS varbinary(8)) AS RowVersion;
            RETURN;
        END;

        INSERT dbo.AuditSapSyncExecutionChanges
        (
            SapSyncExecutionId, SapSyncExecutionDetailId, Action,
            PreviousStatus, NewStatus
        )
        VALUES(@ExecutionId, @DetailId, 'DetailUpdated', @PreviousStatus, @Status);
    END;

    UPDATE dbo.SapSyncExecutions
    SET LastProgressAtUtc = SYSUTCDATETIME(), UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @ExecutionId;

    COMMIT;

    SELECT Id,
           CASE WHEN @WasCreated = 1 THEN N'Created' ELSE N'Updated' END AS ResultCode,
           RowVersion
    FROM dbo.SapSyncExecutionDetails
    WHERE Id = @DetailId;
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM dbo.SchemaVersions
    WHERE Version = N'20260804.169'
)
BEGIN
    INSERT dbo.SchemaVersions(Version, Description)
    VALUES(N'20260804.169', N'Agrega snapshot CountryV1 para ejecuciones SAP de Países');
END;
GO
