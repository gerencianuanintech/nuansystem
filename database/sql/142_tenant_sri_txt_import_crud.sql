/*
    Migracion 142 - SRI TXT Import, consultas paginadas y saneadas (tenant).

    No modifica el flujo TXT -> Staged -> Pending.
    No devuelve AccessKey, HeaderLine, linea TXT, XML ni secretos.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51142, 'SchemaHistory is required before migration 142.', 1;
IF OBJECT_ID(N'dbo.SriTxtImports', N'U') IS NULL
    THROW 51142, 'SriTxtImports is required before migration 142.', 1;
IF OBJECT_ID(N'dbo.SriTxtImportRows', N'U') IS NULL
    THROW 51142, 'SriTxtImportRows is required before migration 142.', 1;
IF OBJECT_ID(N'dbo.SriDocumentQueue', N'U') IS NULL
    THROW 51142, 'SriDocumentQueue is required before migration 142.', 1;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SriTxtImportRows')
      AND name = N'IX_SriTxtImportRows_Environment_Import'
)
BEGIN
    CREATE INDEX IX_SriTxtImportRows_Environment_Import
        ON dbo.SriTxtImportRows(Environment, ImportId)
        INCLUDE (ValidationStatus, EnqueueStatus, QueueId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRITXTIMPORT_LISTAR
    @CreatedFrom datetime2(0) = NULL,
    @CreatedTo datetime2(0) = NULL,
    @Status varchar(30) = NULL,
    @FileName nvarchar(260) = NULL,
    @Environment nvarchar(20) = NULL,
    @Page int = 1,
    @PageSize int = 50
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 OR @PageSize NOT BETWEEN 1 AND 500
        THROW 51142, 'Invalid SRI TXT import paging.', 1;

    DECLARE @EscapedFileName nvarchar(780) =
        REPLACE(REPLACE(REPLACE(NULLIF(LTRIM(RTRIM(@FileName)), N''), N'\', N'\\'), N'%', N'\%'), N'_', N'\_');

    CREATE TABLE #FilteredImports
    (
        Id bigint NOT NULL PRIMARY KEY
    );

    INSERT #FilteredImports(Id)
    SELECT import.Id
    FROM dbo.SriTxtImports import
    WHERE import.IsDeleted = 0
      AND (@CreatedFrom IS NULL OR import.CreatedAt >= @CreatedFrom)
      AND (@CreatedTo IS NULL OR import.CreatedAt <= @CreatedTo)
      AND (@Status IS NULL OR import.Status = @Status)
      AND (@EscapedFileName IS NULL OR import.OriginalFileName LIKE N'%' + @EscapedFileName + N'%' ESCAPE N'\')
      AND
      (
          @Environment IS NULL
          OR EXISTS
          (
              SELECT 1
              FROM dbo.SriTxtImportRows row
              WHERE row.ImportId = import.Id
                AND row.Environment = @Environment
          )
      );

    SELECT
        CONVERT(int, COUNT_BIG(1)) AS TotalCount,
        COALESCE(SUM(CONVERT(bigint, import.TotalRows)), 0) AS TotalRows,
        COALESCE(SUM(CONVERT(bigint, import.ValidRows)), 0) AS ValidRows,
        COALESCE(SUM(CONVERT(bigint, import.InvalidRows + import.DuplicateRows)), 0) AS InvalidRows,
        COALESCE(SUM(CONVERT(bigint, import.LinkedRows)), 0) AS LinkedRows,
        COALESCE
        (
            (
                SELECT COUNT_BIG(1)
                FROM dbo.SriTxtImportRows row
                INNER JOIN #FilteredImports filtered ON filtered.Id = row.ImportId
                INNER JOIN dbo.SriDocumentQueue queue ON queue.Id = row.QueueId
                WHERE queue.Status = N'Staged'
            ),
            0
        ) AS StagedRows,
        COALESCE
        (
            (
                SELECT COUNT_BIG(1)
                FROM dbo.SriTxtImportRows row
                INNER JOIN #FilteredImports filtered ON filtered.Id = row.ImportId
                INNER JOIN dbo.SriDocumentQueue queue ON queue.Id = row.QueueId
                WHERE queue.Status = N'Pending'
            ),
            0
        ) AS PendingRows
    FROM dbo.SriTxtImports import
    INNER JOIN #FilteredImports filtered ON filtered.Id = import.Id;

    SELECT
        import.Id,
        import.GlobalId,
        import.OriginalFileName,
        CONVERT(varchar(64), import.FileSha256, 2) AS FileSha256Hex,
        import.FileSizeBytes,
        import.EncodingCode,
        import.Status,
        import.TotalRows,
        import.ValidRows,
        import.InvalidRows + import.DuplicateRows AS InvalidRows,
        import.LinkedRows,
        COALESCE(currentState.StagedRows, 0) AS StagedRows,
        COALESCE(currentState.PendingRows, 0) AS PendingRows,
        import.CreatedByUserName,
        import.CreatedAt,
        import.CompletedAt,
        import.RowVersion
    FROM dbo.SriTxtImports import
    INNER JOIN #FilteredImports filtered ON filtered.Id = import.Id
    OUTER APPLY
    (
        SELECT
            SUM(CASE WHEN queue.Status = N'Staged' THEN 1 ELSE 0 END) AS StagedRows,
            SUM(CASE WHEN queue.Status = N'Pending' THEN 1 ELSE 0 END) AS PendingRows
        FROM dbo.SriTxtImportRows row
        LEFT JOIN dbo.SriDocumentQueue queue ON queue.Id = row.QueueId
        WHERE row.ImportId = import.Id
    ) currentState
    ORDER BY import.CreatedAt DESC, import.Id DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRITXTIMPORT_PORID
    @ImportId bigint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        import.Id,
        import.GlobalId,
        import.OriginalFileName,
        CONVERT(varchar(64), import.FileSha256, 2) AS FileSha256Hex,
        import.FileSizeBytes,
        import.EncodingCode,
        import.Status,
        import.TotalRows,
        import.ValidRows,
        import.InvalidRows,
        import.DuplicateRows,
        COALESCE(currentState.StagedRows, 0) AS StagedRows,
        COALESCE(currentState.PendingRows, 0) AS PendingRows,
        import.LinkedRows,
        import.EnqueuedRows,
        import.ConflictRows,
        import.CreatedByUserId,
        import.CreatedByUserName,
        import.CreatedAt,
        import.CompletedAt,
        import.RowVersion
    FROM dbo.SriTxtImports import
    OUTER APPLY
    (
        SELECT
            SUM(CASE WHEN queue.Status = N'Staged' THEN 1 ELSE 0 END) AS StagedRows,
            SUM(CASE WHEN queue.Status = N'Pending' THEN 1 ELSE 0 END) AS PendingRows
        FROM dbo.SriTxtImportRows row
        LEFT JOIN dbo.SriDocumentQueue queue ON queue.Id = row.QueueId
        WHERE row.ImportId = import.Id
    ) currentState
    WHERE import.Id = @ImportId
      AND import.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRITXTIMPORT_FILAS
    @ImportId bigint,
    @Validity varchar(10) = 'All',
    @Page int = 1,
    @PageSize int = 100
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 OR @PageSize NOT BETWEEN 1 AND 500
        THROW 51142, 'Invalid SRI TXT row paging.', 1;
    IF @Validity NOT IN ('All', 'Valid', 'Invalid')
        THROW 51142, 'Invalid SRI TXT row validity filter.', 1;

    DECLARE @Exists bit =
        CASE WHEN EXISTS
        (
            SELECT 1
            FROM dbo.SriTxtImports
            WHERE Id = @ImportId
              AND IsDeleted = 0
        ) THEN 1 ELSE 0 END;

    SELECT
        @Exists AS [Exists],
        CASE WHEN @Exists = 0 THEN 0 ELSE
            (
                SELECT COUNT(1)
                FROM dbo.SriTxtImportRows row
                WHERE row.ImportId = @ImportId
                  AND
                  (
                      @Validity = 'All'
                      OR (@Validity = 'Valid' AND row.ValidationStatus = 'Valid')
                      OR (@Validity = 'Invalid' AND row.ValidationStatus <> 'Valid')
                  )
            )
        END AS TotalCount;

    IF @Exists = 0
        RETURN;

    SELECT
        row.Id,
        row.RowGlobalId,
        row.LineNumber,
        CONVERT(varchar(64), row.RowSha256, 2) AS RowSha256Hex,
        CONVERT(varchar(64), row.AccessKeySha256, 2) AS AccessKeySha256Hex,
        row.MaskedAccessKey,
        row.IssuerRuc,
        row.IssuerLegalName,
        row.DocumentTypeCode,
        row.DocumentTypeName,
        row.DocumentSeries,
        row.Environment,
        row.AuthorizationAt,
        row.EmissionDate,
        row.ReceiverIdentification,
        row.ValueWithoutTaxes,
        row.VatAmount,
        row.TotalAmount,
        row.ModifiedDocumentNumber,
        row.ValidationStatus,
        row.EnqueueStatus,
        row.QueueId,
        queue.Status AS QueueStatus,
        row.CreatedAt,
        row.RowVersion
    FROM dbo.SriTxtImportRows row
    LEFT JOIN dbo.SriDocumentQueue queue ON queue.Id = row.QueueId
    WHERE row.ImportId = @ImportId
      AND
      (
          @Validity = 'All'
          OR (@Validity = 'Valid' AND row.ValidationStatus = 'Valid')
          OR (@Validity = 'Invalid' AND row.ValidationStatus <> 'Valid')
      )
    ORDER BY row.LineNumber, row.Id
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SchemaHistory
    WHERE Version = N'20260727.142'
)
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES
    (
        N'20260727.142',
        N'Consultas paginadas y saneadas del CRUD SRI TXT Import'
    );
END;
GO
