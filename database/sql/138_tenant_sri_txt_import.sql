/*
    SRI TXT Import - persistencia tenant y preparacion Staged.

    Prerrequisitos: 115, 117 y 118.
    Este script no consulta al SRI, no inicia workers y no conserva el archivo binario.
    Amplia el estado de SriDocumentQueue sin modificar filas existentes.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SriDocumentQueue', N'U') IS NULL
    THROW 51138, 'SriDocumentQueue is required before migration 138.', 1;
IF OBJECT_ID(N'dbo.AuditSriDocumentChanges', N'U') IS NULL
    THROW 51138, 'AuditSriDocumentChanges is required before migration 138.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51138, 'SchemaHistory is required before migration 138.', 1;
GO

IF EXISTS
(
    SELECT 1
    FROM dbo.SriDocumentQueue
    WHERE Status NOT IN
    (
        N'Staged', N'Pending', N'Querying', N'RetryScheduled',
        N'Authorized', N'NotFound', N'Failed', N'DeadLetter', N'Cancelled'
    )
)
    THROW 51138, 'SriDocumentQueue contains an unknown status; migration 138 stopped without changing the constraint.', 1;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.SriDocumentQueue')
      AND name = N'CK_SriDocumentQueue_Status'
      AND definition NOT LIKE N'%Staged%'
)
BEGIN
    BEGIN TRANSACTION;
    ALTER TABLE dbo.SriDocumentQueue DROP CONSTRAINT CK_SriDocumentQueue_Status;
    ALTER TABLE dbo.SriDocumentQueue WITH CHECK
    ADD CONSTRAINT CK_SriDocumentQueue_Status
        CHECK
        (
            Status IN
            (
                N'Staged', N'Pending', N'Querying', N'RetryScheduled',
                N'Authorized', N'NotFound', N'Failed', N'DeadLetter', N'Cancelled'
            )
        );
    COMMIT;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.SriDocumentQueue')
      AND name = N'CK_SriDocumentQueue_Status'
)
BEGIN
    ALTER TABLE dbo.SriDocumentQueue WITH CHECK
    ADD CONSTRAINT CK_SriDocumentQueue_Status
        CHECK
        (
            Status IN
            (
                N'Staged', N'Pending', N'Querying', N'RetryScheduled',
                N'Authorized', N'NotFound', N'Failed', N'DeadLetter', N'Cancelled'
            )
        );
END;
GO

IF OBJECT_ID(N'dbo.SriTxtImports', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SriTxtImports
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SriTxtImports PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL,
        OriginalFileName nvarchar(260) NOT NULL,
        FileSha256 binary(32) NOT NULL,
        FileSizeBytes bigint NOT NULL,
        EncodingCode varchar(20) NOT NULL,
        HeaderLine nvarchar(1000) NOT NULL,
        Status varchar(30) NOT NULL,
        TotalRows int NOT NULL,
        ValidRows int NOT NULL,
        InvalidRows int NOT NULL,
        DuplicateRows int NOT NULL,
        StagedRows int NOT NULL CONSTRAINT DF_SriTxtImports_StagedRows DEFAULT 0,
        LinkedRows int NOT NULL CONSTRAINT DF_SriTxtImports_LinkedRows DEFAULT 0,
        EnqueuedRows int NOT NULL CONSTRAINT DF_SriTxtImports_EnqueuedRows DEFAULT 0,
        ConflictRows int NOT NULL CONSTRAINT DF_SriTxtImports_ConflictRows DEFAULT 0,
        TraceId uniqueidentifier NOT NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(150) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SriTxtImports_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(150) NULL,
        UpdatedAt datetime2(0) NULL,
        CompletedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SriTxtImports_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(150) NULL,
        DeletedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT UQ_SriTxtImports_GlobalId UNIQUE (GlobalId),
        CONSTRAINT CK_SriTxtImports_FileSize CHECK (FileSizeBytes BETWEEN 1 AND 10485760),
        CONSTRAINT CK_SriTxtImports_Encoding CHECK (EncodingCode IN ('UTF-8', 'Windows-1252')),
        CONSTRAINT CK_SriTxtImports_Status CHECK
        (
            Status IN ('Validated', 'ValidatedWithErrors', 'Completed', 'CompletedWithErrors')
        ),
        CONSTRAINT CK_SriTxtImports_Counts CHECK
        (
            TotalRows BETWEEN 1 AND 50000
            AND ValidRows >= 0
            AND InvalidRows >= 0
            AND DuplicateRows >= 0
            AND StagedRows >= 0
            AND LinkedRows >= 0
            AND EnqueuedRows >= 0
            AND ConflictRows >= 0
        )
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SriTxtImports')
      AND name = N'UX_SriTxtImports_FileSha256'
)
    CREATE UNIQUE INDEX UX_SriTxtImports_FileSha256 ON dbo.SriTxtImports(FileSha256);
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SriTxtImports')
      AND name = N'IX_SriTxtImports_Status_CreatedAt'
)
    CREATE INDEX IX_SriTxtImports_Status_CreatedAt
        ON dbo.SriTxtImports(IsDeleted, Status, CreatedAt DESC);
GO

IF OBJECT_ID(N'dbo.SriTxtImportRows', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SriTxtImportRows
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SriTxtImportRows PRIMARY KEY,
        ImportId bigint NOT NULL,
        RowGlobalId uniqueidentifier NOT NULL,
        LineNumber int NOT NULL,
        RowSha256 binary(32) NOT NULL,
        AccessKeySha256 binary(32) NULL,
        MaskedAccessKey varchar(20) NULL,
        IssuerRuc varchar(13) NULL,
        IssuerLegalName nvarchar(200) NULL,
        DocumentTypeCode char(2) NULL,
        DocumentTypeName nvarchar(50) NULL,
        DocumentSeries varchar(17) NULL,
        Environment nvarchar(20) NULL,
        AuthorizationAt datetime2(0) NULL,
        EmissionDate date NULL,
        ReceiverIdentification varchar(20) NULL,
        ValueWithoutTaxes decimal(19,6) NULL,
        VatAmount decimal(19,6) NULL,
        TotalAmount decimal(19,6) NULL,
        ModifiedDocumentNumber nvarchar(50) NULL,
        ValidationStatus varchar(30) NOT NULL,
        EnqueueStatus varchar(30) NOT NULL,
        QueueId bigint NULL,
        ValidationCode nvarchar(100) NULL,
        ValidationMessage nvarchar(500) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SriTxtImportRows_CreatedAt DEFAULT SYSUTCDATETIME(),
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_SriTxtImportRows_Import FOREIGN KEY (ImportId) REFERENCES dbo.SriTxtImports(Id),
        CONSTRAINT FK_SriTxtImportRows_Queue FOREIGN KEY (QueueId) REFERENCES dbo.SriDocumentQueue(Id),
        CONSTRAINT UQ_SriTxtImportRows_GlobalId UNIQUE (RowGlobalId),
        CONSTRAINT UQ_SriTxtImportRows_Import_Line UNIQUE (ImportId, LineNumber),
        CONSTRAINT CK_SriTxtImportRows_Line CHECK (LineNumber >= 2),
        CONSTRAINT CK_SriTxtImportRows_ValidationStatus CHECK
        (
            ValidationStatus IN ('Valid', 'Invalid', 'DuplicateInFile', 'Conflict')
        ),
        CONSTRAINT CK_SriTxtImportRows_EnqueueStatus CHECK
        (
            EnqueueStatus IN
            (
                'NotEligible', 'Staged', 'LinkedExisting',
                'LinkedAuthorized', 'Enqueued', 'Conflict'
            )
        ),
        CONSTRAINT CK_SriTxtImportRows_Environment CHECK
        (
            Environment IS NULL OR Environment IN (N'Test', N'Production')
        ),
        CONSTRAINT CK_SriTxtImportRows_DocumentType CHECK
        (
            DocumentTypeCode IS NULL OR DocumentTypeCode IN ('01', '04', '07')
        )
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SriTxtImportRows')
      AND name = N'IX_SriTxtImportRows_Import_Status'
)
    CREATE INDEX IX_SriTxtImportRows_Import_Status
        ON dbo.SriTxtImportRows(ImportId, ValidationStatus, EnqueueStatus, LineNumber)
        INCLUDE (QueueId, MaskedAccessKey, DocumentTypeCode, Environment);
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SriTxtImportRows')
      AND name = N'IX_SriTxtImportRows_AccessKeySha256'
)
    CREATE INDEX IX_SriTxtImportRows_AccessKeySha256
        ON dbo.SriTxtImportRows(AccessKeySha256)
        WHERE AccessKeySha256 IS NOT NULL;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SriTxtImportRows')
      AND name = N'IX_SriTxtImportRows_QueueId'
)
    CREATE INDEX IX_SriTxtImportRows_QueueId
        ON dbo.SriTxtImportRows(QueueId)
        WHERE QueueId IS NOT NULL;
GO

IF OBJECT_ID(N'dbo.AuditSriTxtImportChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSriTxtImportChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSriTxtImportChanges PRIMARY KEY,
        ImportId bigint NOT NULL,
        Action nvarchar(50) NOT NULL,
        PreviousStatus varchar(30) NULL,
        NewStatus varchar(30) NOT NULL,
        Reason nvarchar(500) NULL,
        UserId int NULL,
        UserName nvarchar(150) NULL,
        TraceId uniqueidentifier NOT NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditSriTxtImportChanges_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_AuditSriTxtImportChanges_Import FOREIGN KEY (ImportId) REFERENCES dbo.SriTxtImports(Id)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.AuditSriTxtImportChanges')
      AND name = N'IX_AuditSriTxtImportChanges_Import_CreatedAt'
)
    CREATE INDEX IX_AuditSriTxtImportChanges_Import_CreatedAt
        ON dbo.AuditSriTxtImportChanges(ImportId, CreatedAt DESC);
GO

IF TYPE_ID(N'dbo.SriTxtImportRowTableType') IS NULL
BEGIN
    EXEC(N'
        CREATE TYPE dbo.SriTxtImportRowTableType AS TABLE
        (
            RowGlobalId uniqueidentifier NOT NULL,
            LineNumber int NOT NULL,
            RowSha256 binary(32) NOT NULL,
            AccessKey char(49) NULL,
            AccessKeySha256 binary(32) NULL,
            MaskedAccessKey varchar(20) NULL,
            IssuerRuc varchar(13) NULL,
            IssuerLegalName nvarchar(200) NULL,
            DocumentTypeCode char(2) NULL,
            DocumentTypeName nvarchar(50) NULL,
            DocumentSeries varchar(17) NULL,
            Environment nvarchar(20) NULL,
            AuthorizationAt datetime2(0) NULL,
            EmissionDate date NULL,
            ReceiverIdentification varchar(20) NULL,
            ValueWithoutTaxes decimal(19,6) NULL,
            VatAmount decimal(19,6) NULL,
            TotalAmount decimal(19,6) NULL,
            ModifiedDocumentNumber nvarchar(50) NULL,
            ValidationStatus varchar(30) NOT NULL,
            ValidationCode nvarchar(100) NULL,
            ValidationMessage nvarchar(500) NULL,
            PRIMARY KEY (LineNumber)
        );
    ');
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRITXTIMPORT_REGISTRARVALIDADO
    @GlobalId uniqueidentifier,
    @OriginalFileName nvarchar(260),
    @FileSha256 binary(32),
    @FileSizeBytes bigint,
    @EncodingCode varchar(20),
    @HeaderLine nvarchar(1000),
    @Rows dbo.SriTxtImportRowTableType READONLY,
    @TraceId uniqueidentifier,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    IF NOT EXISTS (SELECT 1 FROM @Rows)
        THROW 51138, 'At least one TXT row is required.', 1;
    IF (SELECT COUNT_BIG(1) FROM @Rows) > 50000
        THROW 51138, 'TXT row limit exceeded.', 1;

    BEGIN TRANSACTION;

    DECLARE @ImportId bigint;
    SELECT @ImportId = Id
    FROM dbo.SriTxtImports WITH (UPDLOCK, HOLDLOCK)
    WHERE FileSha256 = @FileSha256;

    IF @ImportId IS NOT NULL
    BEGIN
        COMMIT;
        SELECT 1 AS ResultCode, CONVERT(bit, 0) AS IsCreated;
        SELECT
            i.Id, i.GlobalId, i.OriginalFileName,
            CONVERT(varchar(64), i.FileSha256, 2) AS FileSha256Hex,
            i.FileSizeBytes, i.EncodingCode, i.HeaderLine, i.Status,
            i.TotalRows, i.ValidRows, i.InvalidRows, i.DuplicateRows,
            i.StagedRows, i.LinkedRows, i.EnqueuedRows, i.ConflictRows,
            i.CreatedByUserId, i.CreatedByUserName, i.CreatedAt, i.CompletedAt,
            i.RowVersion
        FROM dbo.SriTxtImports i
        WHERE i.Id = @ImportId;
        RETURN;
    END;

    DECLARE @TotalRows int = (SELECT COUNT(1) FROM @Rows);
    DECLARE @ValidRows int = (SELECT COUNT(1) FROM @Rows WHERE ValidationStatus = 'Valid');
    DECLARE @InvalidRows int = (SELECT COUNT(1) FROM @Rows WHERE ValidationStatus = 'Invalid');
    DECLARE @DuplicateRows int = (SELECT COUNT(1) FROM @Rows WHERE ValidationStatus = 'DuplicateInFile');
    DECLARE @InitialStatus varchar(30) =
        CASE WHEN @InvalidRows + @DuplicateRows > 0 THEN 'ValidatedWithErrors' ELSE 'Validated' END;

    INSERT dbo.SriTxtImports
    (
        GlobalId, OriginalFileName, FileSha256, FileSizeBytes, EncodingCode,
        HeaderLine, Status, TotalRows, ValidRows, InvalidRows, DuplicateRows,
        TraceId, CreatedByUserId, CreatedByUserName
    )
    VALUES
    (
        @GlobalId, @OriginalFileName, @FileSha256, @FileSizeBytes, @EncodingCode,
        @HeaderLine, @InitialStatus, @TotalRows, @ValidRows, @InvalidRows, @DuplicateRows,
        @TraceId, @AuditUserId, @AuditUserName
    );
    SET @ImportId = SCOPE_IDENTITY();

    DECLARE @CreatedQueues table(QueueId bigint PRIMARY KEY);
    ;WITH validKeys AS
    (
        SELECT
            source.RowGlobalId,
            source.LineNumber,
            source.AccessKey,
            source.DocumentTypeCode,
            source.Environment,
            ROW_NUMBER() OVER
            (
                PARTITION BY source.Environment, source.AccessKey
                ORDER BY source.LineNumber
            ) AS IdentityOrder
        FROM @Rows source
        WHERE source.ValidationStatus = 'Valid'
          AND source.AccessKey IS NOT NULL
          AND source.DocumentTypeCode IS NOT NULL
          AND source.Environment IS NOT NULL
    )
    INSERT dbo.SriDocumentQueue
    (
        Environment, AccessKey, DocumentTypeCode, SourceType, SourceReference,
        Status, Priority, TraceId, CreatedByUserId, CreatedByUserName
    )
    OUTPUT inserted.Id INTO @CreatedQueues(QueueId)
    SELECT
        source.Environment,
        source.AccessKey,
        source.DocumentTypeCode,
        N'Txt',
        CONCAT(N'SriTxtImport/', CONVERT(nvarchar(36), @GlobalId), N'/', source.LineNumber),
        N'Staged',
        CONVERT(tinyint, 5),
        source.RowGlobalId,
        @AuditUserId,
        @AuditUserName
    FROM validKeys source
    WHERE source.IdentityOrder = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SriDocumentQueue existing WITH (UPDLOCK, HOLDLOCK)
          WHERE existing.Environment = source.Environment
            AND existing.AccessKey = source.AccessKey
      );

    INSERT dbo.AuditSriDocumentChanges
    (
        QueueId, Action, PreviousStatus, NewStatus,
        UserId, UserName, TraceId
    )
    SELECT
        created.QueueId, N'StageFromTxt', NULL, N'Staged',
        @AuditUserId, @AuditUserName, queue.TraceId
    FROM @CreatedQueues created
    INNER JOIN dbo.SriDocumentQueue queue ON queue.Id = created.QueueId;

    INSERT dbo.SriTxtImportRows
    (
        ImportId, RowGlobalId, LineNumber, RowSha256,
        AccessKeySha256, MaskedAccessKey, IssuerRuc, IssuerLegalName,
        DocumentTypeCode, DocumentTypeName, DocumentSeries, Environment,
        AuthorizationAt, EmissionDate, ReceiverIdentification,
        ValueWithoutTaxes, VatAmount, TotalAmount, ModifiedDocumentNumber,
        ValidationStatus, EnqueueStatus, QueueId,
        ValidationCode, ValidationMessage
    )
    SELECT
        @ImportId,
        source.RowGlobalId,
        source.LineNumber,
        source.RowSha256,
        source.AccessKeySha256,
        source.MaskedAccessKey,
        source.IssuerRuc,
        source.IssuerLegalName,
        source.DocumentTypeCode,
        source.DocumentTypeName,
        source.DocumentSeries,
        source.Environment,
        source.AuthorizationAt,
        source.EmissionDate,
        source.ReceiverIdentification,
        source.ValueWithoutTaxes,
        source.VatAmount,
        source.TotalAmount,
        source.ModifiedDocumentNumber,
        CASE
            WHEN source.ValidationStatus = 'Valid'
             AND queue.Id IS NOT NULL
             AND queue.DocumentTypeCode <> source.DocumentTypeCode
                THEN 'Conflict'
            ELSE source.ValidationStatus
        END,
        CASE
            WHEN source.ValidationStatus <> 'Valid' THEN 'NotEligible'
            WHEN queue.Id IS NULL OR queue.DocumentTypeCode <> source.DocumentTypeCode THEN 'Conflict'
            WHEN created.QueueId IS NOT NULL THEN 'Staged'
            WHEN queue.Status = N'Authorized' THEN 'LinkedAuthorized'
            WHEN queue.Status = N'Staged' THEN 'Staged'
            ELSE 'LinkedExisting'
        END,
        CASE
            WHEN source.ValidationStatus = 'Valid'
             AND queue.DocumentTypeCode = source.DocumentTypeCode
                THEN queue.Id
            ELSE NULL
        END,
        CASE
            WHEN source.ValidationStatus = 'Valid'
             AND queue.Id IS NOT NULL
             AND queue.DocumentTypeCode <> source.DocumentTypeCode
                THEN N'SRI_TXT_QUEUE_IDENTITY_CONFLICT'
            ELSE source.ValidationCode
        END,
        CASE
            WHEN source.ValidationStatus = 'Valid'
             AND queue.Id IS NOT NULL
             AND queue.DocumentTypeCode <> source.DocumentTypeCode
                THEN N'La identidad de la cola existente no coincide con el tipo documental.'
            ELSE source.ValidationMessage
        END
    FROM @Rows source
    LEFT JOIN dbo.SriDocumentQueue queue
      ON queue.Environment = source.Environment
     AND queue.AccessKey = source.AccessKey
    LEFT JOIN @CreatedQueues created ON created.QueueId = queue.Id;

    DECLARE @StagedRows int =
        (SELECT COUNT(1) FROM dbo.SriTxtImportRows WHERE ImportId = @ImportId AND EnqueueStatus = 'Staged');
    DECLARE @LinkedRows int =
        (SELECT COUNT(1) FROM dbo.SriTxtImportRows WHERE ImportId = @ImportId AND EnqueueStatus IN ('LinkedExisting', 'LinkedAuthorized'));
    DECLARE @ConflictRows int =
        (SELECT COUNT(1) FROM dbo.SriTxtImportRows WHERE ImportId = @ImportId AND EnqueueStatus = 'Conflict');
    DECLARE @FinalValidationStatus varchar(30) =
        CASE
            WHEN @InvalidRows + @DuplicateRows + @ConflictRows > 0
                THEN 'ValidatedWithErrors'
            ELSE 'Validated'
        END;

    UPDATE dbo.SriTxtImports
    SET Status = @FinalValidationStatus,
        StagedRows = @StagedRows,
        LinkedRows = @LinkedRows,
        ConflictRows = @ConflictRows,
        ValidRows = @ValidRows - @ConflictRows,
        InvalidRows = @InvalidRows + @ConflictRows
    WHERE Id = @ImportId;

    INSERT dbo.AuditSriTxtImportChanges
    (
        ImportId, Action, PreviousStatus, NewStatus,
        Reason, UserId, UserName, TraceId
    )
    VALUES
    (
        @ImportId, N'UploadValidated', NULL, @FinalValidationStatus,
        CONCAT(N'Filas=', @TotalRows, N';Validas=', @ValidRows - @ConflictRows,
               N';Invalidas=', @InvalidRows + @ConflictRows, N';Duplicadas=', @DuplicateRows),
        @AuditUserId, @AuditUserName, @TraceId
    );

    COMMIT;

    SELECT 1 AS ResultCode, CONVERT(bit, 1) AS IsCreated;
    SELECT
        i.Id, i.GlobalId, i.OriginalFileName,
        CONVERT(varchar(64), i.FileSha256, 2) AS FileSha256Hex,
        i.FileSizeBytes, i.EncodingCode, i.HeaderLine, i.Status,
        i.TotalRows, i.ValidRows, i.InvalidRows, i.DuplicateRows,
        i.StagedRows, i.LinkedRows, i.EnqueuedRows, i.ConflictRows,
        i.CreatedByUserId, i.CreatedByUserName, i.CreatedAt, i.CompletedAt,
        i.RowVersion
    FROM dbo.SriTxtImports i
    WHERE i.Id = @ImportId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRITXTIMPORT_AMBIENTESPREPARADOS
    @ImportId bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT queue.Environment
    FROM dbo.SriTxtImportRows row
    INNER JOIN dbo.SriTxtImports import ON import.Id = row.ImportId
    INNER JOIN dbo.SriDocumentQueue queue ON queue.Id = row.QueueId
    WHERE row.ImportId = @ImportId
      AND import.IsDeleted = 0
      AND row.ValidationStatus = 'Valid'
      AND queue.Status = N'Staged';
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRITXTIMPORT_ENCOLAR
    @ImportId bigint,
    @RowVersion binary(8),
    @TraceId uniqueidentifier,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    DECLARE @CurrentStatus varchar(30);
    DECLARE @CurrentRowVersion binary(8);
    SELECT
        @CurrentStatus = Status,
        @CurrentRowVersion = RowVersion
    FROM dbo.SriTxtImports WITH (UPDLOCK, HOLDLOCK)
    WHERE Id = @ImportId
      AND IsDeleted = 0;

    IF @CurrentStatus IS NULL
    BEGIN
        ROLLBACK;
        SELECT 0 AS ResultCode;
        RETURN;
    END;

    DECLARE @HasStaged bit =
        CASE WHEN EXISTS
        (
            SELECT 1
            FROM dbo.SriTxtImportRows row
            INNER JOIN dbo.SriDocumentQueue queue WITH (UPDLOCK, HOLDLOCK)
                ON queue.Id = row.QueueId
            WHERE row.ImportId = @ImportId
              AND row.ValidationStatus = 'Valid'
              AND queue.Status = N'Staged'
        ) THEN 1 ELSE 0 END;

    IF @RowVersion IS NULL OR DATALENGTH(@RowVersion) <> 8
    BEGIN
        ROLLBACK;
        SELECT -2 AS ResultCode;
        RETURN;
    END;

    IF @CurrentRowVersion <> @RowVersion AND @HasStaged = 1
    BEGIN
        ROLLBACK;
        SELECT -2 AS ResultCode;
        RETURN;
    END;

    IF @CurrentStatus NOT IN ('Validated', 'ValidatedWithErrors', 'Completed', 'CompletedWithErrors')
    BEGIN
        ROLLBACK;
        SELECT -3 AS ResultCode;
        RETURN;
    END;

    DECLARE @Transitioned table(QueueId bigint PRIMARY KEY, TraceId uniqueidentifier);
    UPDATE queue
       SET Status = N'Pending',
           UpdatedByUserId = @AuditUserId,
           UpdatedByUserName = @AuditUserName,
           UpdatedAt = SYSUTCDATETIME()
    OUTPUT inserted.Id, inserted.TraceId INTO @Transitioned(QueueId, TraceId)
    FROM dbo.SriDocumentQueue queue
    WHERE queue.Status = N'Staged'
      AND EXISTS
      (
          SELECT 1
          FROM dbo.SriTxtImportRows row
          WHERE row.ImportId = @ImportId
            AND row.QueueId = queue.Id
            AND row.ValidationStatus = 'Valid'
      );

    INSERT dbo.AuditSriDocumentChanges
    (
        QueueId, Action, PreviousStatus, NewStatus,
        UserId, UserName, TraceId
    )
    SELECT
        transitioned.QueueId, N'EnqueueFromTxt', N'Staged', N'Pending',
        @AuditUserId, @AuditUserName, transitioned.TraceId
    FROM @Transitioned transitioned;

    UPDATE row
       SET EnqueueStatus =
           CASE
               WHEN transitioned.QueueId IS NOT NULL THEN 'Enqueued'
               WHEN queue.Status = N'Authorized' THEN 'LinkedAuthorized'
               WHEN queue.Status = N'Staged' THEN 'Staged'
               ELSE 'LinkedExisting'
           END
    FROM dbo.SriTxtImportRows row
    INNER JOIN dbo.SriDocumentQueue queue ON queue.Id = row.QueueId
    LEFT JOIN @Transitioned transitioned ON transitioned.QueueId = row.QueueId
    WHERE row.ImportId = @ImportId
      AND row.ValidationStatus = 'Valid';

    DECLARE @InvalidOrConflict int =
        (SELECT COUNT(1) FROM dbo.SriTxtImportRows WHERE ImportId = @ImportId AND ValidationStatus IN ('Invalid', 'Conflict'));
    DECLARE @DuplicateRows int =
        (SELECT COUNT(1) FROM dbo.SriTxtImportRows WHERE ImportId = @ImportId AND ValidationStatus = 'DuplicateInFile');
    DECLARE @NewStatus varchar(30) =
        CASE WHEN @InvalidOrConflict + @DuplicateRows > 0 THEN 'CompletedWithErrors' ELSE 'Completed' END;
    DECLARE @TransitionCount int = (SELECT COUNT(1) FROM @Transitioned);

    IF @CurrentStatus NOT IN ('Completed', 'CompletedWithErrors') OR @TransitionCount > 0
    BEGIN
        UPDATE dbo.SriTxtImports
        SET Status = @NewStatus,
            StagedRows = (SELECT COUNT(1) FROM dbo.SriTxtImportRows WHERE ImportId = @ImportId AND EnqueueStatus = 'Staged'),
            LinkedRows = (SELECT COUNT(1) FROM dbo.SriTxtImportRows WHERE ImportId = @ImportId AND EnqueueStatus IN ('LinkedExisting', 'LinkedAuthorized')),
            EnqueuedRows = (SELECT COUNT(1) FROM dbo.SriTxtImportRows WHERE ImportId = @ImportId AND EnqueueStatus = 'Enqueued'),
            ConflictRows = (SELECT COUNT(1) FROM dbo.SriTxtImportRows WHERE ImportId = @ImportId AND EnqueueStatus = 'Conflict'),
            UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME(),
            CompletedAt = SYSUTCDATETIME()
        WHERE Id = @ImportId;

        INSERT dbo.AuditSriTxtImportChanges
        (
            ImportId, Action, PreviousStatus, NewStatus,
            Reason, UserId, UserName, TraceId
        )
        VALUES
        (
            @ImportId, N'EnqueueCompleted', @CurrentStatus, @NewStatus,
            CONCAT(N'Colas activadas=', @TransitionCount),
            @AuditUserId, @AuditUserName, @TraceId
        );
    END;

    COMMIT;

    SELECT 1 AS ResultCode;
    SELECT
        i.Id, i.GlobalId, i.OriginalFileName,
        CONVERT(varchar(64), i.FileSha256, 2) AS FileSha256Hex,
        i.FileSizeBytes, i.EncodingCode, i.HeaderLine, i.Status,
        i.TotalRows, i.ValidRows, i.InvalidRows, i.DuplicateRows,
        i.StagedRows, i.LinkedRows, i.EnqueuedRows, i.ConflictRows,
        i.CreatedByUserId, i.CreatedByUserName, i.CreatedAt, i.CompletedAt,
        i.RowVersion
    FROM dbo.SriTxtImports i
    WHERE i.Id = @ImportId;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SchemaHistory
    WHERE Version = N'20260727.138'
)
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES
    (
        N'20260727.138',
        N'SRI TXT Import tenant con Staged, detalle normalizado y enqueue explicito'
    );
END;
GO
