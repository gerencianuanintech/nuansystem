/*
    Iteracion 8.5 - Currency transaccional y reserva permanente de codigo.

    - Currency y LocalOutbox se guardan en la misma transaccion de aplicacion.
    - La aplicacion Matriz-Sucursal usa GlobalId.
    - Una colision de Code con otro GlobalId es terminal; no adopta filas.
    - ExternalSystem y ExternalCode se conservan. SAP queda fuera de alcance.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Currencies', N'U') IS NULL
    THROW 51136, 'Currencies is required before migration 136.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL
    THROW 51136, 'LocalOutbox is required before migration 136.', 1;
IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL
    THROW 51136, 'SyncInbox is required before migration 136.', 1;
IF OBJECT_ID(N'dbo.SyncAudit', N'U') IS NULL
    THROW 51136, 'SyncAudit is required before migration 136.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51136, 'SchemaHistory is required before migration 136.', 1;
GO

IF EXISTS
(
    SELECT Code
    FROM dbo.Currencies
    GROUP BY Code
    HAVING COUNT_BIG(1) > 1
)
    THROW 51136, 'Currency codes, including tombstones, must be unique before migration 136.', 1;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Currencies')
      AND name = N'UX_Currencies_Code'
      AND (is_unique = 0 OR filter_definition IS NOT NULL)
)
    DROP INDEX UX_Currencies_Code ON dbo.Currencies;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Currencies')
      AND name = N'UX_Currencies_Code'
      AND is_unique = 1
      AND filter_definition IS NULL
)
    CREATE UNIQUE INDEX UX_Currencies_Code ON dbo.Currencies(Code);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CURRENCIES_BUSCARPORCODIGO
    @Code nvarchar(3),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.Currencies
    WHERE Code = @Code
      AND (@ExcluirId IS NULL OR CurrencyId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CURRENCY_SYNC_APPLY
    @GlobalId uniqueidentifier,
    @Code nvarchar(3),
    @Name nvarchar(120),
    @Symbol nvarchar(10) = NULL,
    @Description nvarchar(300) = NULL,
    @IsBaseCurrency bit,
    @IsActive bit,
    @ExternalSystem nvarchar(50) = NULL,
    @ExternalCode nvarchar(100) = NULL,
    @CreatedAt datetime2(0),
    @UpdatedAt datetime2(0) = NULL,
    @IsDeleted bit
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrencyId int;

    SELECT @CurrencyId = CurrencyId
    FROM dbo.Currencies WITH (UPDLOCK, HOLDLOCK)
    WHERE GlobalId = @GlobalId;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Currencies WITH (UPDLOCK, HOLDLOCK)
        WHERE Code = @Code
          AND GlobalId <> @GlobalId
    )
    BEGIN
        SELECT -2 AS ResultCode, CONVERT(int, NULL) AS CurrencyId;
        RETURN;
    END;

    IF @CurrencyId IS NULL
    BEGIN
        INSERT INTO dbo.Currencies
        (
            GlobalId, Code, Name, Symbol, Description, IsBaseCurrency,
            IsActive, IsDeleted, ExternalSystem, ExternalCode,
            CreatedAt, CreatedByUserName, DeletedAt, DeletedByUserName
        )
        VALUES
        (
            @GlobalId, @Code, @Name, @Symbol, @Description, @IsBaseCurrency,
            @IsActive, @IsDeleted, @ExternalSystem, @ExternalCode,
            COALESCE(@CreatedAt, SYSUTCDATETIME()), N'MasterBranchSyncWorker',
            CASE WHEN @IsDeleted = 1 THEN SYSUTCDATETIME() END,
            CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' END
        );

        SET @CurrencyId = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.Currencies
        SET Code = @Code,
            Name = @Name,
            Symbol = @Symbol,
            Description = @Description,
            IsBaseCurrency = @IsBaseCurrency,
            IsActive = @IsActive,
            IsDeleted = @IsDeleted,
            ExternalSystem = @ExternalSystem,
            ExternalCode = @ExternalCode,
            UpdatedAt = COALESCE(@UpdatedAt, SYSUTCDATETIME()),
            UpdatedByUserName = N'MasterBranchSyncWorker',
            DeletedAt = CASE
                WHEN @IsDeleted = 1 THEN COALESCE(DeletedAt, SYSUTCDATETIME())
                ELSE NULL
            END,
            DeletedByUserName = CASE
                WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker'
                ELSE NULL
            END
        WHERE CurrencyId = @CurrencyId;
    END;

    SELECT 1 AS ResultCode, @CurrencyId AS CurrencyId;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SchemaHistory
    WHERE Version = N'20260727.136'
)
BEGIN
    INSERT INTO dbo.SchemaHistory(Version, Description)
    VALUES
    (
        N'20260727.136',
        N'Currency transaccional, tombstone reservado y conflicto terminal sin adopcion'
    );
END;
GO
