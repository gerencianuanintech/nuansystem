/*
    Iteracion 8.6 - PriceList transaccional Matriz-Sucursal.

    No ejecuta workers ni modifica configuracion Master.
    Reserva Code incluso despues de eliminacion logica.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.PriceLists', N'U') IS NULL
    THROW 51140, 'PriceLists is required before migration 140.', 1;
IF OBJECT_ID(N'dbo.Currencies', N'U') IS NULL
    THROW 51140, 'Currencies is required before migration 140.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL
    THROW 51140, 'LocalOutbox is required before migration 140.', 1;
IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL
    THROW 51140, 'SyncInbox is required before migration 140.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51140, 'SchemaHistory is required before migration 140.', 1;
GO

IF EXISTS (SELECT Code FROM dbo.PriceLists GROUP BY Code HAVING COUNT_BIG(1) > 1)
    THROW 51140, 'PriceList codes, including tombstones, must be unique before migration 140.', 1;
IF EXISTS (SELECT 1 FROM dbo.PriceLists WHERE AppliesTo NOT IN (N'Sales', N'Purchasing', N'Both'))
    THROW 51140, 'Unexpected PriceList AppliesTo value before migration 140.', 1;
IF EXISTS
(
    SELECT 1
    FROM dbo.PriceLists AS priceList
    LEFT JOIN dbo.Currencies AS currency
      ON currency.Code = priceList.CurrencyCode
     AND currency.IsDeleted = 0
    WHERE priceList.IsDeleted = 0
      AND currency.CurrencyId IS NULL
)
    THROW 51140, 'PriceList has an unresolved CurrencyCode before migration 140.', 1;
IF
(
    SELECT COUNT_BIG(1)
    FROM dbo.PriceLists
    WHERE IsDeleted = 0 AND IsActive = 1 AND IsDefault = 1
      AND AppliesTo IN (N'Sales', N'Both')
) > 1
    THROW 51140, 'More than one effective Sales default PriceList exists.', 1;
IF
(
    SELECT COUNT_BIG(1)
    FROM dbo.PriceLists
    WHERE IsDeleted = 0 AND IsActive = 1 AND IsDefault = 1
      AND AppliesTo IN (N'Purchasing', N'Both')
) > 1
    THROW 51140, 'More than one effective Purchasing default PriceList exists.', 1;
GO

IF EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.PriceLists')
      AND name = N'UX_PriceLists_Code'
      AND (is_unique = 0 OR filter_definition IS NOT NULL)
)
    DROP INDEX UX_PriceLists_Code ON dbo.PriceLists;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.PriceLists')
      AND name = N'UX_PriceLists_Code'
      AND is_unique = 1
      AND filter_definition IS NULL
)
    CREATE UNIQUE INDEX UX_PriceLists_Code ON dbo.PriceLists(Code);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.PriceListId AS Id, p.GlobalId, p.Code, p.Name, p.Description,
        p.CurrencyCode, c.Name AS CurrencyName, c.GlobalId AS CurrencyGlobalId,
        p.AppliesTo, p.IsDefault, p.IsActive,
        p.ExternalSystem, p.ExternalCode, p.SapCode,
        p.CreatedByUserId, p.CreatedByUserName, p.CreatedAt,
        p.UpdatedByUserId, p.UpdatedByUserName, p.UpdatedAt
    FROM dbo.PriceLists AS p
    INNER JOIN dbo.Currencies AS c ON c.Code = p.CurrencyCode AND c.IsDeleted = 0
    WHERE p.IsDeleted = 0
    ORDER BY p.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.PriceListId AS Id, p.GlobalId, p.Code, p.Name, p.Description,
        p.CurrencyCode, c.Name AS CurrencyName, c.GlobalId AS CurrencyGlobalId,
        p.AppliesTo, p.IsDefault, p.IsActive,
        p.ExternalSystem, p.ExternalCode, p.SapCode,
        p.CreatedByUserId, p.CreatedByUserName, p.CreatedAt,
        p.UpdatedByUserId, p.UpdatedByUserName, p.UpdatedAt
    FROM dbo.PriceLists AS p
    INNER JOIN dbo.Currencies AS c ON c.Code = p.CurrencyCode AND c.IsDeleted = 0
    WHERE p.PriceListId = @Id AND p.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_LOOKUP
    @AppliesTo nvarchar(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT PriceListId AS Id, Code, Name, IsActive
    FROM dbo.PriceLists
    WHERE IsDeleted = 0 AND IsActive = 1
      AND (@AppliesTo IS NULL OR AppliesTo = @AppliesTo OR AppliesTo = N'Both')
    ORDER BY IsDefault DESC, Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_MONEDAPORCODIGO
    @CurrencyCode nvarchar(3)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Code, Name, GlobalId
    FROM dbo.Currencies WITH (UPDLOCK, HOLDLOCK)
    WHERE Code = @CurrencyCode AND IsDeleted = 0 AND IsActive = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_CODIGORESERVADO
    @Code nvarchar(30),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.PriceLists WITH (UPDLOCK, HOLDLOCK)
    WHERE Code = @Code
      AND (@ExcluirId IS NULL OR PriceListId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_PREDETERMINADACONFLICTO
    @AppliesTo nvarchar(20),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.PriceLists WITH (UPDLOCK, HOLDLOCK)
    WHERE IsDeleted = 0 AND IsActive = 1 AND IsDefault = 1
      AND (@ExcluirId IS NULL OR PriceListId <> @ExcluirId)
      AND (AppliesTo = N'Both' OR @AppliesTo = N'Both' OR AppliesTo = @AppliesTo);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_REFERENCIASACTIVAS
    @Id int,
    @Code nvarchar(30)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Count int = 0;

    IF OBJECT_ID(N'dbo.BusinessPartnerCreditSettings', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NOT NULL
    BEGIN
        SELECT @Count += COUNT(1)
        FROM dbo.BusinessPartnerCreditSettings AS credit
        INNER JOIN dbo.BusinessPartners AS partner ON partner.Id = credit.BusinessPartnerId
        WHERE credit.PriceListCode = @Code
          AND partner.IsDeleted = 0
          AND partner.IsActive = 1;
    END;

    IF OBJECT_ID(N'dbo.PurchaseOrderHeaders', N'U') IS NOT NULL
    BEGIN
        SELECT @Count += COUNT(1)
        FROM dbo.PurchaseOrderHeaders
        WHERE PriceListId = @Id AND IsDeleted = 0;
    END;

    SELECT @Count;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PRICELISTS_CREAR
    @GlobalId uniqueidentifier,
    @Code nvarchar(30),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @CurrencyCode nvarchar(3),
    @AppliesTo nvarchar(20),
    @IsDefault bit,
    @IsActive bit,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.PriceLists
    (
        GlobalId, Code, Name, Description, CurrencyCode, AppliesTo,
        IsDefault, IsActive, CreatedByUserId, CreatedByUserName
    )
    VALUES
    (
        @GlobalId, @Code, @Name, @Description, @CurrencyCode, @AppliesTo,
        @IsDefault, @IsActive, @CreatedByUserId, @CreatedByUserName
    );
    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PRICELISTS_ACTUALIZAR
    @Id int,
    @Code nvarchar(30),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @CurrencyCode nvarchar(3),
    @AppliesTo nvarchar(20),
    @IsDefault bit,
    @IsActive bit,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.PriceLists
    SET Code = @Code, Name = @Name, Description = @Description,
        CurrencyCode = @CurrencyCode, AppliesTo = @AppliesTo,
        IsDefault = @IsDefault, IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
    WHERE PriceListId = @Id AND IsDeleted = 0;
    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_PRICELISTS_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.PriceLists
    SET IsDeleted = 1, IsActive = 0, IsDefault = 0,
        DeletedAt = SYSUTCDATETIME(),
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName
    WHERE PriceListId = @Id AND IsDeleted = 0;
    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PRICELIST_SYNC_APPLY_EVENT
    @EventId uniqueidentifier,
    @SourceCompanyId int,
    @EntityName nvarchar(80),
    @EntityGlobalId uniqueidentifier,
    @Operation nvarchar(30),
    @PayloadJson nvarchar(max),
    @GlobalId uniqueidentifier,
    @Code nvarchar(30),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @CurrencyGlobalId uniqueidentifier,
    @CurrencyCodeEvidence nvarchar(3),
    @AppliesTo nvarchar(20),
    @IsDefault bit,
    @IsActive bit,
    @IsDeleted bit,
    @ExternalSystem nvarchar(50) = NULL,
    @ExternalCode nvarchar(100) = NULL,
    @SapCode nvarchar(100) = NULL,
    @CreatedAt datetime2(0),
    @UpdatedAt datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @InboxId bigint;
    DECLARE @InboxStatus nvarchar(30);
    DECLARE @PriceListId int;
    DECLARE @CurrencyCode nvarchar(3);

    SELECT @InboxId = Id, @InboxStatus = Status
    FROM dbo.SyncInbox WITH (UPDLOCK, HOLDLOCK)
    WHERE EventId = @EventId;

    IF @InboxStatus = N'Applied'
    BEGIN
        SELECT @PriceListId = PriceListId FROM dbo.PriceLists WHERE GlobalId = @GlobalId;
        COMMIT TRANSACTION;
        SELECT 2 AS ResultCode, @PriceListId AS PriceListId;
        RETURN;
    END;
    IF @InboxStatus = N'DeadLetter'
    BEGIN
        COMMIT TRANSACTION;
        SELECT -2 AS ResultCode, CONVERT(int, NULL) AS PriceListId;
        RETURN;
    END;

    IF @InboxId IS NULL
    BEGIN
        INSERT INTO dbo.SyncInbox
        (
            EventId, SourceCompanyId, EntityName, EntityGlobalId,
            Operation, PayloadJson, Status
        )
        VALUES
        (
            @EventId, @SourceCompanyId, @EntityName, @EntityGlobalId,
            @Operation, @PayloadJson, N'Pending'
        );
        SET @InboxId = CONVERT(bigint, SCOPE_IDENTITY());
    END;

    SELECT @CurrencyCode = Code
    FROM dbo.Currencies WITH (UPDLOCK, HOLDLOCK)
    WHERE GlobalId = @CurrencyGlobalId AND IsDeleted = 0 AND IsActive = 1;

    IF @CurrencyCode IS NULL
    BEGIN
        UPDATE dbo.SyncInbox
        SET Status = N'Error', AttemptCount = AttemptCount + 1,
            ErrorMessage = N'Currency dependency is not available.',
            LastErrorMessage = N'Currency dependency is not available.',
            NextRetryAt = DATEADD(second, 30, SYSUTCDATETIME())
        WHERE Id = @InboxId;
        COMMIT TRANSACTION;
        SELECT -3 AS ResultCode, CONVERT(int, NULL) AS PriceListId;
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1 FROM dbo.PriceLists WITH (UPDLOCK, HOLDLOCK)
        WHERE Code = @Code AND GlobalId <> @GlobalId
    )
    BEGIN
        UPDATE dbo.SyncInbox
        SET Status = N'DeadLetter', AttemptCount = AttemptCount + 1,
            ErrorMessage = N'PriceList code belongs to another GlobalId.',
            LastErrorMessage = N'PriceList code belongs to another GlobalId.',
            NextRetryAt = NULL
        WHERE Id = @InboxId;
        COMMIT TRANSACTION;
        SELECT -2 AS ResultCode, CONVERT(int, NULL) AS PriceListId;
        RETURN;
    END;

    SELECT @PriceListId = PriceListId
    FROM dbo.PriceLists WITH (UPDLOCK, HOLDLOCK)
    WHERE GlobalId = @GlobalId;

    IF @IsDefault = 1 AND @IsDeleted = 0 AND @IsActive = 1
       AND EXISTS
       (
           SELECT 1 FROM dbo.PriceLists WITH (UPDLOCK, HOLDLOCK)
           WHERE IsDeleted = 0 AND IsActive = 1 AND IsDefault = 1
             AND (@PriceListId IS NULL OR PriceListId <> @PriceListId)
             AND (AppliesTo = N'Both' OR @AppliesTo = N'Both' OR AppliesTo = @AppliesTo)
       )
    BEGIN
        UPDATE dbo.SyncInbox
        SET Status = N'DeadLetter', AttemptCount = AttemptCount + 1,
            ErrorMessage = N'PriceList default scope conflict.',
            LastErrorMessage = N'PriceList default scope conflict.',
            NextRetryAt = NULL
        WHERE Id = @InboxId;
        COMMIT TRANSACTION;
        SELECT -4 AS ResultCode, CONVERT(int, NULL) AS PriceListId;
        RETURN;
    END;

    IF @PriceListId IS NULL
    BEGIN
        INSERT INTO dbo.PriceLists
        (
            GlobalId, Code, Name, Description, CurrencyCode, AppliesTo,
            IsDefault, IsActive, IsDeleted, ExternalSystem, ExternalCode,
            SapCode, CreatedAt, CreatedByUserName, DeletedAt, DeletedByUserName
        )
        VALUES
        (
            @GlobalId, @Code, @Name, @Description, @CurrencyCode, @AppliesTo,
            @IsDefault, @IsActive, @IsDeleted, @ExternalSystem, @ExternalCode,
            @SapCode, COALESCE(@CreatedAt, SYSUTCDATETIME()), N'MasterBranchSyncWorker',
            CASE WHEN @IsDeleted = 1 THEN SYSUTCDATETIME() END,
            CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' END
        );
        SET @PriceListId = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.PriceLists
        SET Code = @Code, Name = @Name, Description = @Description,
            CurrencyCode = @CurrencyCode, AppliesTo = @AppliesTo,
            IsDefault = CASE WHEN @IsDeleted = 1 THEN 0 ELSE @IsDefault END,
            IsActive = @IsActive, IsDeleted = @IsDeleted,
            ExternalSystem = @ExternalSystem, ExternalCode = @ExternalCode,
            SapCode = @SapCode, UpdatedAt = COALESCE(@UpdatedAt, SYSUTCDATETIME()),
            UpdatedByUserName = N'MasterBranchSyncWorker',
            DeletedAt = CASE WHEN @IsDeleted = 1 THEN COALESCE(DeletedAt, SYSUTCDATETIME()) ELSE NULL END,
            DeletedByUserName = CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END
        WHERE PriceListId = @PriceListId;
    END;

    UPDATE dbo.SyncInbox
    SET Status = N'Applied', AppliedAt = SYSUTCDATETIME(),
        ErrorMessage = NULL, LastErrorMessage = NULL, NextRetryAt = NULL
    WHERE Id = @InboxId;

    COMMIT TRANSACTION;
    SELECT 1 AS ResultCode, @PriceListId AS PriceListId;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260727.140')
BEGIN
    INSERT INTO dbo.SchemaHistory(Version, Description)
    VALUES (N'20260727.140', N'PriceList transaccional, Currency por GlobalId y conflictos terminales');
END;
GO
