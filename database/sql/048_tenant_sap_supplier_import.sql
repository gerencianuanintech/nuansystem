CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNERS_IMPORTARSAP
    @CardCode nvarchar(50),
    @CardName nvarchar(200),
    @TaxIdentification nvarchar(50) = NULL,
    @CardType nvarchar(1) = N'S',
    @Phone nvarchar(50) = NULL,
    @Email nvarchar(256) = NULL,
    @Currency nvarchar(3) = NULL,
    @IsActive bit = 1,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @NormalizedCardCode nvarchar(50) = NULLIF(LTRIM(RTRIM(@CardCode)), N'');
    DECLARE @NormalizedCardName nvarchar(200) = NULLIF(LTRIM(RTRIM(@CardName)), N'');
    DECLARE @NormalizedTaxIdentification nvarchar(50) = NULLIF(LTRIM(RTRIM(@TaxIdentification)), N'');
    DECLARE @NormalizedCardType nvarchar(1) = ISNULL(NULLIF(UPPER(LTRIM(RTRIM(@CardType))), N''), N'S');

    IF @NormalizedCardCode IS NULL OR @NormalizedCardName IS NULL
        THROW 51000, 'El proveedor SAP debe tener CardCode y CardName.', 1;

    DECLARE @IdentificationTypeId int;
    SELECT TOP (1) @IdentificationTypeId = Id
    FROM dbo.BusinessPartnerIdentificationTypes
    WHERE IsDeleted = 0
      AND IsActive = 1
    ORDER BY
        CASE
            WHEN Code IN (N'RUC', N'TAXID') THEN 0
            WHEN Code IN (N'CI', N'CEDULA') THEN 1
            WHEN Code IN (N'OTRO', N'OTHER') THEN 2
            ELSE 3
        END,
        Id;

    IF @IdentificationTypeId IS NULL
        THROW 51001, 'No existe un tipo de identificacion activo para crear proveedores.', 1;

    DECLARE @BusinessPartnerId int;
    DECLARE @Action nvarchar(20) = N'Updated';

    SELECT TOP (1) @BusinessPartnerId = bp.Id
    FROM dbo.BusinessPartners bp
    INNER JOIN dbo.BusinessPartnerSapMapping sap ON sap.BusinessPartnerId = bp.Id
    WHERE bp.IsDeleted = 0
      AND sap.SapCardCode = @NormalizedCardCode
    ORDER BY bp.Id;

    IF @BusinessPartnerId IS NULL
    BEGIN
        SELECT TOP (1) @BusinessPartnerId = Id
        FROM dbo.BusinessPartners
        WHERE IsDeleted = 0
          AND Code = @NormalizedCardCode
        ORDER BY Id;
    END;

    BEGIN TRANSACTION;

    IF @BusinessPartnerId IS NULL
    BEGIN
        SET @Action = N'Created';

        INSERT INTO dbo.BusinessPartners
            (Code, Name, CommercialName, PartnerType, IdentificationTypeId, IdentificationNumber,
             Email, Phone, Website, Remarks, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt)
        VALUES
            (@NormalizedCardCode, @NormalizedCardName, @NormalizedCardName, N'Supplier', @IdentificationTypeId,
             ISNULL(@NormalizedTaxIdentification, @NormalizedCardCode), @Email, @Phone, NULL,
             N'Creado desde sincronizacion SAP Business One.', @IsActive, @AuditUserId, @AuditUserName, SYSUTCDATETIME());

        SET @BusinessPartnerId = CONVERT(int, SCOPE_IDENTITY());

        INSERT INTO dbo.BusinessPartnerFiscalData
            (BusinessPartnerId, CountryCode, IsAccountingRequired, AppliesRetention)
        VALUES
            (@BusinessPartnerId, NULL, 0, 0);

        INSERT INTO dbo.BusinessPartnerAccountingSettings
            (BusinessPartnerId, AccountingBySupplier, RequiresProvision, AllowsAdvance, AllowsCompensation,
             AllowsPartialPayments, IsPaymentBlocked, UsesWithholdingBase, ConciliationRequired, AveragePaymentDays,
             PaymentTolerancePercent)
        VALUES
            (@BusinessPartnerId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        INSERT INTO dbo.BusinessPartnerCreditSettings
            (BusinessPartnerId, CreditDays, CreditLimit, DeliveryDays, MinimumOrderAmount, AllowsBackorder,
             PreferredCurrencyCode, CreditStatus)
        VALUES
            (@BusinessPartnerId, 0, 0, 0, 0, 0, @Currency, N'Normal');

        INSERT INTO dbo.BusinessPartnerSapMapping
            (BusinessPartnerId, SapCardCode, SapCardType, SapSyncStatus, SapLastSyncAt, SapLastError,
             SapEnabled, SapMode, SapCompanyCode, SapRetryCount, SyncAsSupplier, AllowManualSapRetry,
             RequiresApprovalBeforeSapSync)
        VALUES
            (@BusinessPartnerId, @NormalizedCardCode, @NormalizedCardType, N'Synced', SYSUTCDATETIME(), NULL,
             1, N'HANA_IMPORT', NULL, 0, 1, 1, 0);
    END
    ELSE
    BEGIN
        UPDATE dbo.BusinessPartners
        SET Name = @NormalizedCardName,
            CommercialName = @NormalizedCardName,
            PartnerType = CASE WHEN PartnerType = N'Customer' THEN N'Both' ELSE PartnerType END,
            Email = @Email,
            Phone = @Phone,
            IsActive = @IsActive,
            UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @BusinessPartnerId;

        IF NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerSapMapping WHERE BusinessPartnerId = @BusinessPartnerId)
        BEGIN
            INSERT INTO dbo.BusinessPartnerSapMapping
                (BusinessPartnerId, SapCardCode, SapCardType, SapSyncStatus, SapLastSyncAt, SapLastError,
                 SapEnabled, SapMode, SapCompanyCode, SapRetryCount, SyncAsSupplier, AllowManualSapRetry,
                 RequiresApprovalBeforeSapSync)
            VALUES
                (@BusinessPartnerId, @NormalizedCardCode, @NormalizedCardType, N'Synced', SYSUTCDATETIME(), NULL,
                 1, N'HANA_IMPORT', NULL, 0, 1, 1, 0);
        END
        ELSE
        BEGIN
            UPDATE dbo.BusinessPartnerSapMapping
            SET SapCardCode = @NormalizedCardCode,
                SapCardType = @NormalizedCardType,
                SapSyncStatus = N'Synced',
                SapLastSyncAt = SYSUTCDATETIME(),
                SapLastError = NULL,
                SapEnabled = 1,
                SapMode = N'HANA_IMPORT',
                SyncAsSupplier = 1,
                AllowManualSapRetry = 1
            WHERE BusinessPartnerId = @BusinessPartnerId;
        END;

        UPDATE dbo.BusinessPartnerCreditSettings
        SET PreferredCurrencyCode = @Currency
        WHERE BusinessPartnerId = @BusinessPartnerId;
    END;

    COMMIT TRANSACTION;

    SELECT
        @BusinessPartnerId AS BusinessPartnerId,
        @Action AS [Action],
        CASE
            WHEN @Action = N'Created' THEN N'Proveedor creado desde SAP.'
            ELSE N'Proveedor actualizado desde SAP.'
        END AS [Message];
END;
GO
