/*
    Forward-only repair for the BusinessPartner persistence contract.

    Scope:
    - Restores dbo.BusinessPartnerPurchaseSettings when an older tenant omitted it.
    - Aligns create/update procedures with the current Dapper parameter contract.
    - Preserves the accounting-dimension wrapper introduced by script 042.

    Run only in a tenant database. Never run in NuanSystem_Master.
*/

SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NULL
    THROW 51000, 'BusinessPartners is required before migration 126.', 1;

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NULL
    THROW 51000, 'BusinessPartnerAccountingSettings is required before migration 126.', 1;

IF OBJECT_ID(N'dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR_LEGACY_ACCOUNTING_DIMENSIONS', N'P') IS NULL
    THROW 51000, 'Legacy create procedure required by migration 126 is missing.', 1;

IF OBJECT_ID(N'dbo.SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR_LEGACY_ACCOUNTING_DIMENSIONS', N'P') IS NULL
    THROW 51000, 'Legacy update procedure required by migration 126 is missing.', 1;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerPurchaseSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerPurchaseSettings
    (
        BusinessPartnerId int NOT NULL CONSTRAINT PK_BusinessPartnerPurchaseSettings PRIMARY KEY,
        Incoterm nvarchar(20) NULL,
        CommercialDiscountPercent decimal(9,4) NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_CommercialDiscountPercent DEFAULT 0,
        PurchaseCurrencyCode nvarchar(3) NULL,
        PreferredWarehouseId int NULL,
        PreferredWarehouseCode nvarchar(50) NULL,
        MinimumOrderQuantity decimal(19,6) NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_MinimumOrderQuantity DEFAULT 0,
        PurchaseSupplierType nvarchar(80) NULL,
        ActiveForImport bit NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_ActiveForImport DEFAULT 0,
        SubjectToEvaluation bit NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_SubjectToEvaluation DEFAULT 0,
        AllowsUrgentPurchases bit NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_AllowsUrgentPurchases DEFAULT 0,
        AverageDeliveryDays int NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_AverageDeliveryDays DEFAULT 0,
        LeadTimeDays int NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_LeadTimeDays DEFAULT 0,
        DeliveryToleranceDays int NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_DeliveryToleranceDays DEFAULT 0,
        RequiresPurchaseOrder bit NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_RequiresPurchaseOrder DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL
            CONSTRAINT DF_BusinessPartnerPurchaseSettings_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_BusinessPartnerPurchaseSettings_BusinessPartners
            FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id),
        CONSTRAINT CK_BusinessPartnerPurchaseSettings_Values
            CHECK
            (
                CommercialDiscountPercent >= 0
                AND CommercialDiscountPercent <= 100
                AND MinimumOrderQuantity >= 0
                AND AverageDeliveryDays >= 0
                AND LeadTimeDays >= 0
                AND DeliveryToleranceDays >= 0
            )
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR
    @Code nvarchar(50),
    @Name nvarchar(200),
    @CommercialName nvarchar(200) = NULL,
    @PartnerType nvarchar(20),
    @IdentificationTypeId int,
    @IdentificationNumber nvarchar(50),
    @SupplierGroupId int = NULL,
    @SupplierClassId int = NULL,
    @EconomicActivityId int = NULL,
    @ZoneId int = NULL,
    @SupplyMethodId int = NULL,
    @Email nvarchar(256) = NULL,
    @Phone nvarchar(50) = NULL,
    @Website nvarchar(200) = NULL,
    @Remarks nvarchar(1000) = NULL,
    @IsActive bit,
    @TaxpayerTypeId int = NULL,
    @TaxRegimeId int = NULL,
    @FiscalCountryId int = NULL,
    @TaxpayerType nvarchar(60) = NULL,
    @IsAccountingRequired bit = 0,
    @AppliesRetention bit = 0,
    @FiscalRegime nvarchar(80) = NULL,
    @CountryCode nvarchar(3) = NULL,
    @Province nvarchar(120) = NULL,
    @City nvarchar(120) = NULL,
    @CustomerAccountId int = NULL,
    @SupplierAccountId int = NULL,
    @CustomerAdvanceAccountId int = NULL,
    @SupplierAdvanceAccountId int = NULL,
    @RetentionAccountId int = NULL,
    @BranchId int = NULL,
    @DepartmentId int = NULL,
    @BusinessLineId int = NULL,
    @CostCenterId int = NULL,
    @ProjectId int = NULL,
    @CostCenterCode nvarchar(50) = NULL,
    @DefaultExpenseAccountId int = NULL,
    @DifferenceAccountId int = NULL,
    @RoundingAccountId int = NULL,
    @ClearingAccountId int = NULL,
    @DiscountAccountId int = NULL,
    @AccountingBySupplier bit = 0,
    @RequiresProvision bit = 0,
    @AllowsAdvance bit = 0,
    @AllowsCompensation bit = 0,
    @AllowsPartialPayments bit = 0,
    @IsPaymentBlocked bit = 0,
    @UsesWithholdingBase bit = 0,
    @ConciliationRequired bit = 0,
    @AccountingPaymentMethodId int = NULL,
    @PaymentPriorityId int = NULL,
    @ApprovalFlowId int = NULL,
    @PaymentDocumentTypeId int = NULL,
    @AccountingPaymentMethod nvarchar(80) = NULL,
    @PaymentPriority nvarchar(80) = NULL,
    @RequiredPaymentDay nvarchar(80) = NULL,
    @ApprovalFlow nvarchar(120) = NULL,
    @PaymentDocumentType nvarchar(80) = NULL,
    @AveragePaymentDays int = 0,
    @PaymentTolerancePercent decimal(9,4) = 0,
    @PaymentTermId int = NULL,
    @CreditDays int = 0,
    @CreditLimit decimal(19,6) = 0,
    @DeliveryDays int = 0,
    @MinimumOrderAmount decimal(19,6) = 0,
    @AllowsBackorder bit = 0,
    @PreferredCurrencyCode nvarchar(3) = NULL,
    @PriceListCode nvarchar(50) = NULL,
    @AssignedSellerCode nvarchar(50) = NULL,
    @AssignedBuyerCode nvarchar(50) = NULL,
    @Incoterm nvarchar(20) = NULL,
    @CommercialDiscountPercent decimal(9,4) = 0,
    @PurchaseCurrencyCode nvarchar(3) = NULL,
    @PreferredWarehouseId int = NULL,
    @PurchaseSupplierType nvarchar(80) = NULL,
    @PreferredWarehouseCode nvarchar(50) = NULL,
    @MinimumOrderQuantity decimal(19,6) = 0,
    @ActiveForImport bit = 0,
    @SubjectToEvaluation bit = 0,
    @AllowsUrgentPurchases bit = 0,
    @AverageDeliveryDays int = 0,
    @LeadTimeDays int = 0,
    @DeliveryToleranceDays int = 0,
    @RequiresPurchaseOrder bit = 0,
    @CreditStatus nvarchar(30) = N'Normal',
    @SapCardCode nvarchar(50) = NULL,
    @SapCardType nvarchar(1) = NULL,
    @SapSyncStatus nvarchar(30) = N'Pending',
    @SapLastSyncAt datetime2(0) = NULL,
    @SapLastError nvarchar(max) = NULL,
    @SapEnabled bit = 0,
    @SapMode nvarchar(50) = NULL,
    @SapCompanyCode nvarchar(80) = NULL,
    @SapRetryCount int = 0,
    @SyncAsSupplier bit = 0,
    @AllowManualSapRetry bit = 0,
    @RequiresApprovalBeforeSapSync bit = 0,
    @AddressesJson nvarchar(max) = NULL,
    @ContactsJson nvarchar(max) = NULL,
    @BankAccountsJson nvarchar(max) = NULL,
    @RetentionSettingsJson nvarchar(max) = NULL,
    @NotesJson nvarchar(max) = NULL,
    @SapFieldMappingsJson nvarchar(max) = NULL,
    @AttachmentsJson nvarchar(max) = NULL,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Created table (Id int NOT NULL);

    INSERT INTO @Created (Id)
    EXEC dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR_LEGACY_ACCOUNTING_DIMENSIONS
        @Code=@Code, @Name=@Name, @CommercialName=@CommercialName, @PartnerType=@PartnerType,
        @IdentificationTypeId=@IdentificationTypeId, @IdentificationNumber=@IdentificationNumber,
        @SupplierGroupId=@SupplierGroupId, @SupplierClassId=@SupplierClassId,
        @EconomicActivityId=@EconomicActivityId, @ZoneId=@ZoneId, @SupplyMethodId=@SupplyMethodId,
        @Email=@Email, @Phone=@Phone, @Website=@Website, @Remarks=@Remarks, @IsActive=@IsActive,
        @TaxpayerTypeId=@TaxpayerTypeId, @TaxRegimeId=@TaxRegimeId, @FiscalCountryId=@FiscalCountryId,
        @TaxpayerType=@TaxpayerType, @IsAccountingRequired=@IsAccountingRequired,
        @AppliesRetention=@AppliesRetention, @FiscalRegime=@FiscalRegime,
        @CountryCode=@CountryCode, @Province=@Province, @City=@City,
        @CustomerAccountId=@CustomerAccountId, @SupplierAccountId=@SupplierAccountId,
        @CustomerAdvanceAccountId=@CustomerAdvanceAccountId,
        @SupplierAdvanceAccountId=@SupplierAdvanceAccountId, @RetentionAccountId=@RetentionAccountId,
        @CostCenterCode=@CostCenterCode, @DefaultExpenseAccountId=@DefaultExpenseAccountId,
        @DifferenceAccountId=@DifferenceAccountId, @RoundingAccountId=@RoundingAccountId,
        @ClearingAccountId=@ClearingAccountId, @DiscountAccountId=@DiscountAccountId,
        @AccountingBySupplier=@AccountingBySupplier, @RequiresProvision=@RequiresProvision,
        @AllowsAdvance=@AllowsAdvance, @AllowsCompensation=@AllowsCompensation,
        @AllowsPartialPayments=@AllowsPartialPayments, @IsPaymentBlocked=@IsPaymentBlocked,
        @UsesWithholdingBase=@UsesWithholdingBase, @ConciliationRequired=@ConciliationRequired,
        @AccountingPaymentMethodId=@AccountingPaymentMethodId, @PaymentPriorityId=@PaymentPriorityId,
        @ApprovalFlowId=@ApprovalFlowId, @PaymentDocumentTypeId=@PaymentDocumentTypeId,
        @AccountingPaymentMethod=@AccountingPaymentMethod, @PaymentPriority=@PaymentPriority,
        @RequiredPaymentDay=@RequiredPaymentDay, @ApprovalFlow=@ApprovalFlow,
        @PaymentDocumentType=@PaymentDocumentType, @AveragePaymentDays=@AveragePaymentDays,
        @PaymentTolerancePercent=@PaymentTolerancePercent, @PaymentTermId=@PaymentTermId,
        @CreditDays=@CreditDays, @CreditLimit=@CreditLimit, @DeliveryDays=@DeliveryDays,
        @MinimumOrderAmount=@MinimumOrderAmount, @AllowsBackorder=@AllowsBackorder,
        @PreferredCurrencyCode=@PreferredCurrencyCode, @PriceListCode=@PriceListCode,
        @AssignedSellerCode=@AssignedSellerCode, @AssignedBuyerCode=@AssignedBuyerCode,
        @CreditStatus=@CreditStatus, @SapCardCode=@SapCardCode, @SapCardType=@SapCardType,
        @SapSyncStatus=@SapSyncStatus, @SapLastSyncAt=@SapLastSyncAt, @SapLastError=@SapLastError,
        @SapEnabled=@SapEnabled, @SapMode=@SapMode, @SapCompanyCode=@SapCompanyCode,
        @SapRetryCount=@SapRetryCount, @SyncAsSupplier=@SyncAsSupplier,
        @AllowManualSapRetry=@AllowManualSapRetry,
        @RequiresApprovalBeforeSapSync=@RequiresApprovalBeforeSapSync,
        @AddressesJson=@AddressesJson, @ContactsJson=@ContactsJson,
        @BankAccountsJson=@BankAccountsJson, @RetentionSettingsJson=@RetentionSettingsJson,
        @NotesJson=@NotesJson, @SapFieldMappingsJson=@SapFieldMappingsJson,
        @AttachmentsJson=@AttachmentsJson, @CreatedByUserId=@CreatedByUserId,
        @CreatedByUserName=@CreatedByUserName;

    DECLARE @Id int=(SELECT TOP(1) Id FROM @Created);

    UPDATE dbo.BusinessPartnerAccountingSettings
    SET BranchId=@BranchId,
        DepartmentId=@DepartmentId,
        BusinessLineId=@BusinessLineId,
        CostCenterId=@CostCenterId,
        ProjectId=@ProjectId
    WHERE BusinessPartnerId=@Id;

    IF EXISTS(SELECT 1 FROM dbo.BusinessPartnerPurchaseSettings WHERE BusinessPartnerId=@Id)
    BEGIN
        UPDATE dbo.BusinessPartnerPurchaseSettings
        SET Incoterm=@Incoterm,
            CommercialDiscountPercent=@CommercialDiscountPercent,
            PurchaseCurrencyCode=@PurchaseCurrencyCode,
            PreferredWarehouseId=@PreferredWarehouseId,
            PurchaseSupplierType=@PurchaseSupplierType,
            PreferredWarehouseCode=@PreferredWarehouseCode,
            MinimumOrderQuantity=@MinimumOrderQuantity,
            ActiveForImport=@ActiveForImport,
            SubjectToEvaluation=@SubjectToEvaluation,
            AllowsUrgentPurchases=@AllowsUrgentPurchases,
            AverageDeliveryDays=@AverageDeliveryDays,
            LeadTimeDays=@LeadTimeDays,
            DeliveryToleranceDays=@DeliveryToleranceDays,
            RequiresPurchaseOrder=@RequiresPurchaseOrder,
            UpdatedAt=SYSUTCDATETIME()
        WHERE BusinessPartnerId=@Id;
    END
    ELSE
    BEGIN
        INSERT dbo.BusinessPartnerPurchaseSettings
        (
            BusinessPartnerId, Incoterm, CommercialDiscountPercent, PurchaseCurrencyCode,
            PreferredWarehouseId, PurchaseSupplierType, PreferredWarehouseCode,
            MinimumOrderQuantity, ActiveForImport, SubjectToEvaluation, AllowsUrgentPurchases,
            AverageDeliveryDays, LeadTimeDays, DeliveryToleranceDays, RequiresPurchaseOrder
        )
        VALUES
        (
            @Id, @Incoterm, @CommercialDiscountPercent, @PurchaseCurrencyCode,
            @PreferredWarehouseId, @PurchaseSupplierType, @PreferredWarehouseCode,
            @MinimumOrderQuantity, @ActiveForImport, @SubjectToEvaluation, @AllowsUrgentPurchases,
            @AverageDeliveryDays, @LeadTimeDays, @DeliveryToleranceDays, @RequiresPurchaseOrder
        );
    END;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR
    @Id int,
    @Code nvarchar(50),
    @Name nvarchar(200),
    @CommercialName nvarchar(200) = NULL,
    @PartnerType nvarchar(20),
    @IdentificationTypeId int,
    @IdentificationNumber nvarchar(50),
    @SupplierGroupId int = NULL,
    @SupplierClassId int = NULL,
    @EconomicActivityId int = NULL,
    @ZoneId int = NULL,
    @SupplyMethodId int = NULL,
    @Email nvarchar(256) = NULL,
    @Phone nvarchar(50) = NULL,
    @Website nvarchar(200) = NULL,
    @Remarks nvarchar(1000) = NULL,
    @IsActive bit,
    @TaxpayerTypeId int = NULL,
    @TaxRegimeId int = NULL,
    @FiscalCountryId int = NULL,
    @TaxpayerType nvarchar(60) = NULL,
    @IsAccountingRequired bit = 0,
    @AppliesRetention bit = 0,
    @FiscalRegime nvarchar(80) = NULL,
    @CountryCode nvarchar(3) = NULL,
    @Province nvarchar(120) = NULL,
    @City nvarchar(120) = NULL,
    @CustomerAccountId int = NULL,
    @SupplierAccountId int = NULL,
    @CustomerAdvanceAccountId int = NULL,
    @SupplierAdvanceAccountId int = NULL,
    @RetentionAccountId int = NULL,
    @BranchId int = NULL,
    @DepartmentId int = NULL,
    @BusinessLineId int = NULL,
    @CostCenterId int = NULL,
    @ProjectId int = NULL,
    @CostCenterCode nvarchar(50) = NULL,
    @DefaultExpenseAccountId int = NULL,
    @DifferenceAccountId int = NULL,
    @RoundingAccountId int = NULL,
    @ClearingAccountId int = NULL,
    @DiscountAccountId int = NULL,
    @AccountingBySupplier bit = 0,
    @RequiresProvision bit = 0,
    @AllowsAdvance bit = 0,
    @AllowsCompensation bit = 0,
    @AllowsPartialPayments bit = 0,
    @IsPaymentBlocked bit = 0,
    @UsesWithholdingBase bit = 0,
    @ConciliationRequired bit = 0,
    @AccountingPaymentMethodId int = NULL,
    @PaymentPriorityId int = NULL,
    @ApprovalFlowId int = NULL,
    @PaymentDocumentTypeId int = NULL,
    @AccountingPaymentMethod nvarchar(80) = NULL,
    @PaymentPriority nvarchar(80) = NULL,
    @RequiredPaymentDay nvarchar(80) = NULL,
    @ApprovalFlow nvarchar(120) = NULL,
    @PaymentDocumentType nvarchar(80) = NULL,
    @AveragePaymentDays int = 0,
    @PaymentTolerancePercent decimal(9,4) = 0,
    @PaymentTermId int = NULL,
    @CreditDays int = 0,
    @CreditLimit decimal(19,6) = 0,
    @DeliveryDays int = 0,
    @MinimumOrderAmount decimal(19,6) = 0,
    @AllowsBackorder bit = 0,
    @PreferredCurrencyCode nvarchar(3) = NULL,
    @PriceListCode nvarchar(50) = NULL,
    @AssignedSellerCode nvarchar(50) = NULL,
    @AssignedBuyerCode nvarchar(50) = NULL,
    @Incoterm nvarchar(20) = NULL,
    @CommercialDiscountPercent decimal(9,4) = 0,
    @PurchaseCurrencyCode nvarchar(3) = NULL,
    @PreferredWarehouseId int = NULL,
    @PurchaseSupplierType nvarchar(80) = NULL,
    @PreferredWarehouseCode nvarchar(50) = NULL,
    @MinimumOrderQuantity decimal(19,6) = 0,
    @ActiveForImport bit = 0,
    @SubjectToEvaluation bit = 0,
    @AllowsUrgentPurchases bit = 0,
    @AverageDeliveryDays int = 0,
    @LeadTimeDays int = 0,
    @DeliveryToleranceDays int = 0,
    @RequiresPurchaseOrder bit = 0,
    @CreditStatus nvarchar(30) = N'Normal',
    @SapCardCode nvarchar(50) = NULL,
    @SapCardType nvarchar(1) = NULL,
    @SapSyncStatus nvarchar(30) = N'Pending',
    @SapLastSyncAt datetime2(0) = NULL,
    @SapLastError nvarchar(max) = NULL,
    @SapEnabled bit = 0,
    @SapMode nvarchar(50) = NULL,
    @SapCompanyCode nvarchar(80) = NULL,
    @SapRetryCount int = 0,
    @SyncAsSupplier bit = 0,
    @AllowManualSapRetry bit = 0,
    @RequiresApprovalBeforeSapSync bit = 0,
    @AddressesJson nvarchar(max) = NULL,
    @ContactsJson nvarchar(max) = NULL,
    @BankAccountsJson nvarchar(max) = NULL,
    @RetentionSettingsJson nvarchar(max) = NULL,
    @NotesJson nvarchar(max) = NULL,
    @SapFieldMappingsJson nvarchar(max) = NULL,
    @AttachmentsJson nvarchar(max) = NULL,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Updated table (AffectedRows int NOT NULL);

    INSERT INTO @Updated (AffectedRows)
    EXEC dbo.SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR_LEGACY_ACCOUNTING_DIMENSIONS
        @Id=@Id, @Code=@Code, @Name=@Name, @CommercialName=@CommercialName,
        @PartnerType=@PartnerType, @IdentificationTypeId=@IdentificationTypeId,
        @IdentificationNumber=@IdentificationNumber, @SupplierGroupId=@SupplierGroupId,
        @SupplierClassId=@SupplierClassId, @EconomicActivityId=@EconomicActivityId,
        @ZoneId=@ZoneId, @SupplyMethodId=@SupplyMethodId, @Email=@Email, @Phone=@Phone,
        @Website=@Website, @Remarks=@Remarks, @IsActive=@IsActive,
        @TaxpayerTypeId=@TaxpayerTypeId, @TaxRegimeId=@TaxRegimeId,
        @FiscalCountryId=@FiscalCountryId, @TaxpayerType=@TaxpayerType,
        @IsAccountingRequired=@IsAccountingRequired, @AppliesRetention=@AppliesRetention,
        @FiscalRegime=@FiscalRegime, @CountryCode=@CountryCode, @Province=@Province, @City=@City,
        @CustomerAccountId=@CustomerAccountId, @SupplierAccountId=@SupplierAccountId,
        @CustomerAdvanceAccountId=@CustomerAdvanceAccountId,
        @SupplierAdvanceAccountId=@SupplierAdvanceAccountId, @RetentionAccountId=@RetentionAccountId,
        @CostCenterCode=@CostCenterCode, @DefaultExpenseAccountId=@DefaultExpenseAccountId,
        @DifferenceAccountId=@DifferenceAccountId, @RoundingAccountId=@RoundingAccountId,
        @ClearingAccountId=@ClearingAccountId, @DiscountAccountId=@DiscountAccountId,
        @AccountingBySupplier=@AccountingBySupplier, @RequiresProvision=@RequiresProvision,
        @AllowsAdvance=@AllowsAdvance, @AllowsCompensation=@AllowsCompensation,
        @AllowsPartialPayments=@AllowsPartialPayments, @IsPaymentBlocked=@IsPaymentBlocked,
        @UsesWithholdingBase=@UsesWithholdingBase, @ConciliationRequired=@ConciliationRequired,
        @AccountingPaymentMethodId=@AccountingPaymentMethodId, @PaymentPriorityId=@PaymentPriorityId,
        @ApprovalFlowId=@ApprovalFlowId, @PaymentDocumentTypeId=@PaymentDocumentTypeId,
        @AccountingPaymentMethod=@AccountingPaymentMethod, @PaymentPriority=@PaymentPriority,
        @RequiredPaymentDay=@RequiredPaymentDay, @ApprovalFlow=@ApprovalFlow,
        @PaymentDocumentType=@PaymentDocumentType, @AveragePaymentDays=@AveragePaymentDays,
        @PaymentTolerancePercent=@PaymentTolerancePercent, @PaymentTermId=@PaymentTermId,
        @CreditDays=@CreditDays, @CreditLimit=@CreditLimit, @DeliveryDays=@DeliveryDays,
        @MinimumOrderAmount=@MinimumOrderAmount, @AllowsBackorder=@AllowsBackorder,
        @PreferredCurrencyCode=@PreferredCurrencyCode, @PriceListCode=@PriceListCode,
        @AssignedSellerCode=@AssignedSellerCode, @AssignedBuyerCode=@AssignedBuyerCode,
        @CreditStatus=@CreditStatus, @SapCardCode=@SapCardCode, @SapCardType=@SapCardType,
        @SapSyncStatus=@SapSyncStatus, @SapLastSyncAt=@SapLastSyncAt, @SapLastError=@SapLastError,
        @SapEnabled=@SapEnabled, @SapMode=@SapMode, @SapCompanyCode=@SapCompanyCode,
        @SapRetryCount=@SapRetryCount, @SyncAsSupplier=@SyncAsSupplier,
        @AllowManualSapRetry=@AllowManualSapRetry,
        @RequiresApprovalBeforeSapSync=@RequiresApprovalBeforeSapSync,
        @AddressesJson=@AddressesJson, @ContactsJson=@ContactsJson,
        @BankAccountsJson=@BankAccountsJson, @RetentionSettingsJson=@RetentionSettingsJson,
        @NotesJson=@NotesJson, @SapFieldMappingsJson=@SapFieldMappingsJson,
        @AttachmentsJson=@AttachmentsJson, @UpdatedByUserId=@UpdatedByUserId,
        @UpdatedByUserName=@UpdatedByUserName;

    DECLARE @AffectedRows int=(SELECT TOP(1) AffectedRows FROM @Updated);

    IF @AffectedRows>0
    BEGIN
        UPDATE dbo.BusinessPartnerAccountingSettings
        SET BranchId=@BranchId,
            DepartmentId=@DepartmentId,
            BusinessLineId=@BusinessLineId,
            CostCenterId=@CostCenterId,
            ProjectId=@ProjectId
        WHERE BusinessPartnerId=@Id;

        IF EXISTS(SELECT 1 FROM dbo.BusinessPartnerPurchaseSettings WHERE BusinessPartnerId=@Id)
        BEGIN
            UPDATE dbo.BusinessPartnerPurchaseSettings
            SET Incoterm=@Incoterm,
                CommercialDiscountPercent=@CommercialDiscountPercent,
                PurchaseCurrencyCode=@PurchaseCurrencyCode,
                PreferredWarehouseId=@PreferredWarehouseId,
                PurchaseSupplierType=@PurchaseSupplierType,
                PreferredWarehouseCode=@PreferredWarehouseCode,
                MinimumOrderQuantity=@MinimumOrderQuantity,
                ActiveForImport=@ActiveForImport,
                SubjectToEvaluation=@SubjectToEvaluation,
                AllowsUrgentPurchases=@AllowsUrgentPurchases,
                AverageDeliveryDays=@AverageDeliveryDays,
                LeadTimeDays=@LeadTimeDays,
                DeliveryToleranceDays=@DeliveryToleranceDays,
                RequiresPurchaseOrder=@RequiresPurchaseOrder,
                UpdatedAt=SYSUTCDATETIME()
            WHERE BusinessPartnerId=@Id;
        END
        ELSE
        BEGIN
            INSERT dbo.BusinessPartnerPurchaseSettings
            (
                BusinessPartnerId, Incoterm, CommercialDiscountPercent, PurchaseCurrencyCode,
                PreferredWarehouseId, PurchaseSupplierType, PreferredWarehouseCode,
                MinimumOrderQuantity, ActiveForImport, SubjectToEvaluation, AllowsUrgentPurchases,
                AverageDeliveryDays, LeadTimeDays, DeliveryToleranceDays, RequiresPurchaseOrder
            )
            VALUES
            (
                @Id, @Incoterm, @CommercialDiscountPercent, @PurchaseCurrencyCode,
                @PreferredWarehouseId, @PurchaseSupplierType, @PreferredWarehouseCode,
                @MinimumOrderQuantity, @ActiveForImport, @SubjectToEvaluation,
                @AllowsUrgentPurchases, @AverageDeliveryDays, @LeadTimeDays,
                @DeliveryToleranceDays, @RequiresPurchaseOrder
            );
        END;
    END;

    SELECT @AffectedRows;
END;
GO

DECLARE @ListDefinition nvarchar(max)=
    OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_GET_BUSINESSPARTNERS_LISTAR'));

IF @ListDefinition IS NULL
    THROW 51000, 'BusinessPartner list procedure is required by migration 126.', 1;

IF @ListDefinition NOT LIKE N'%bp.GlobalId%'
BEGIN
    IF @ListDefinition NOT LIKE N'%bp.Id, bp.Code, bp.Name, bp.CommercialName%'
        THROW 51000, 'Unexpected BusinessPartner list projection in migration 126.', 1;

    SET @ListDefinition=REPLACE(
        @ListDefinition,
        N'bp.Id, bp.Code, bp.Name, bp.CommercialName',
        N'bp.Id, bp.GlobalId, bp.Code, bp.Name, bp.ExternalSystem, bp.ExternalCode, bp.CommercialName');
    DECLARE @ListProcedureAt int=CHARINDEX(N'PROCEDURE',UPPER(@ListDefinition));
    IF @ListProcedureAt=0
        THROW 51000, 'BusinessPartner list definition has no procedure declaration.', 1;
    SET @ListDefinition=N'ALTER '+SUBSTRING(
        @ListDefinition,
        @ListProcedureAt,
        LEN(@ListDefinition)-@ListProcedureAt+1);
    EXEC sys.sp_executesql @ListDefinition;
END;
GO

DECLARE @GetDefinition nvarchar(max)=
    OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORID'));

IF @GetDefinition IS NULL
    THROW 51000, 'BusinessPartner get-by-id procedure is required by migration 126.', 1;

IF @GetDefinition NOT LIKE N'%bp.GlobalId%'
BEGIN
    IF @GetDefinition NOT LIKE N'%bp.Id, bp.Code, bp.Name, bp.CommercialName%'
        THROW 51000, 'Unexpected BusinessPartner get-by-id projection in migration 126.', 1;

    SET @GetDefinition=REPLACE(
        @GetDefinition,
        N'bp.Id, bp.Code, bp.Name, bp.CommercialName',
        N'bp.Id, bp.GlobalId, bp.Code, bp.Name, bp.ExternalSystem, bp.ExternalCode, bp.CommercialName');
    DECLARE @GetProcedureAt int=CHARINDEX(N'PROCEDURE',UPPER(@GetDefinition));
    IF @GetProcedureAt=0
        THROW 51000, 'BusinessPartner get-by-id definition has no procedure declaration.', 1;
    SET @GetDefinition=N'ALTER '+SUBSTRING(
        @GetDefinition,
        @GetProcedureAt,
        LEN(@GetDefinition)-@GetProcedureAt+1);
    EXEC sys.sp_executesql @GetDefinition;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51000, 'SchemaHistory is required before recording migration 126.', 1;

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260725.126')
BEGIN
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260725.126',N'Restaura contrato de compras de BusinessPartner y procedimientos Dapper');
END;
GO
