/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    Agrega al maestro de terceros los vinculos hacia catalogos GeneralSupplier
    sin mover reglas de negocio fuera de Application/Persistence.
*/

IF COL_LENGTH(N'dbo.BusinessPartners', N'SupplierGroupId') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD SupplierGroupId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartners', N'SupplierClassId') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD SupplierClassId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartners', N'EconomicActivityId') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD EconomicActivityId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartners', N'ZoneId') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD ZoneId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartners', N'SupplyMethodId') IS NULL
    ALTER TABLE dbo.BusinessPartners ADD SupplyMethodId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'ContactTypeId') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD ContactTypeId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerAddresses', N'CountryId') IS NULL
    ALTER TABLE dbo.BusinessPartnerAddresses ADD CountryId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerAddresses', N'ProvinceId') IS NULL
    ALTER TABLE dbo.BusinessPartnerAddresses ADD ProvinceId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerAddresses', N'CityId') IS NULL
    ALTER TABLE dbo.BusinessPartnerAddresses ADD CityId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'ContactChannelId') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD ContactChannelId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'Department') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD Department nvarchar(120) NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'Extension') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD Extension nvarchar(20) NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'Language') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD [Language] nvarchar(50) NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'ReceivesNotifications') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD ReceivesNotifications bit NOT NULL CONSTRAINT DF_BusinessPartnerContacts_ReceivesNotifications DEFAULT 0;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'Notes') IS NULL
    ALTER TABLE dbo.BusinessPartnerContacts ADD Notes nvarchar(500) NULL;
GO

IF OBJECT_ID(N'dbo.ContactTypes', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerContacts_ContactTypes')
BEGIN
    ALTER TABLE dbo.BusinessPartnerContacts
    ADD CONSTRAINT FK_BusinessPartnerContacts_ContactTypes
        FOREIGN KEY (ContactTypeId) REFERENCES dbo.ContactTypes(ContactTypeId);
END;
GO

IF OBJECT_ID(N'dbo.ContactChannels', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerContacts_ContactChannels')
BEGIN
    ALTER TABLE dbo.BusinessPartnerContacts
    ADD CONSTRAINT FK_BusinessPartnerContacts_ContactChannels
        FOREIGN KEY (ContactChannelId) REFERENCES dbo.ContactChannels(ContactChannelId);
END;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerCreditSettings', N'DeliveryDays') IS NULL
    ALTER TABLE dbo.BusinessPartnerCreditSettings ADD DeliveryDays int NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_DeliveryDays DEFAULT 0;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerCreditSettings', N'MinimumOrderAmount') IS NULL
    ALTER TABLE dbo.BusinessPartnerCreditSettings ADD MinimumOrderAmount decimal(19,6) NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_MinimumOrderAmount DEFAULT 0;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerCreditSettings', N'AllowsBackorder') IS NULL
    ALTER TABLE dbo.BusinessPartnerCreditSettings ADD AllowsBackorder bit NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_AllowsBackorder DEFAULT 0;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerCreditSettings', N'PreferredCurrencyCode') IS NULL
    ALTER TABLE dbo.BusinessPartnerCreditSettings ADD PreferredCurrencyCode nvarchar(3) NULL;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerPurchaseSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerPurchaseSettings
    (
        BusinessPartnerId int NOT NULL CONSTRAINT PK_BusinessPartnerPurchaseSettings PRIMARY KEY,
        Incoterm nvarchar(20) NULL,
        CommercialDiscountPercent decimal(9,4) NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_CommercialDiscountPercent DEFAULT 0,
        PurchaseCurrencyCode nvarchar(3) NULL,
        PreferredWarehouseId int NULL,
        PreferredWarehouseCode nvarchar(50) NULL,
        MinimumOrderQuantity decimal(19,6) NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_MinimumOrderQuantity DEFAULT 0,
        PurchaseSupplierType nvarchar(80) NULL,
        ActiveForImport bit NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_ActiveForImport DEFAULT 0,
        SubjectToEvaluation bit NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_SubjectToEvaluation DEFAULT 0,
        AllowsUrgentPurchases bit NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_AllowsUrgentPurchases DEFAULT 0,
        AverageDeliveryDays int NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_AverageDeliveryDays DEFAULT 0,
        LeadTimeDays int NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_LeadTimeDays DEFAULT 0,
        DeliveryToleranceDays int NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_DeliveryToleranceDays DEFAULT 0,
        RequiresPurchaseOrder bit NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_RequiresPurchaseOrder DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_BusinessPartnerPurchaseSettings_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id),
        CONSTRAINT CK_BusinessPartnerPurchaseSettings_Values CHECK (CommercialDiscountPercent >= 0 AND CommercialDiscountPercent <= 100 AND MinimumOrderQuantity >= 0 AND AverageDeliveryDays >= 0 AND LeadTimeDays >= 0 AND DeliveryToleranceDays >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerPurchaseSettings', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'Incoterm') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD Incoterm nvarchar(20) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'CommercialDiscountPercent') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD CommercialDiscountPercent decimal(9,4) NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_CommercialDiscountPercent DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'PurchaseCurrencyCode') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD PurchaseCurrencyCode nvarchar(3) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'PreferredWarehouseId') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD PreferredWarehouseId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'PreferredWarehouseCode') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD PreferredWarehouseCode nvarchar(50) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'MinimumOrderQuantity') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD MinimumOrderQuantity decimal(19,6) NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_MinimumOrderQuantity DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'PurchaseSupplierType') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD PurchaseSupplierType nvarchar(80) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'ActiveForImport') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD ActiveForImport bit NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_ActiveForImport DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'SubjectToEvaluation') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD SubjectToEvaluation bit NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_SubjectToEvaluation DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'AllowsUrgentPurchases') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD AllowsUrgentPurchases bit NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_AllowsUrgentPurchases DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'AverageDeliveryDays') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD AverageDeliveryDays int NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_AverageDeliveryDays DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'LeadTimeDays') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD LeadTimeDays int NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_LeadTimeDays DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'DeliveryToleranceDays') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD DeliveryToleranceDays int NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_DeliveryToleranceDays DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'RequiresPurchaseOrder') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD RequiresPurchaseOrder bit NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_RequiresPurchaseOrder DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'CreatedAt') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerPurchaseSettings_CreatedAt DEFAULT SYSUTCDATETIME();
    IF COL_LENGTH(N'dbo.BusinessPartnerPurchaseSettings', N'UpdatedAt') IS NULL
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings ADD UpdatedAt datetime2(0) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_BusinessPartnerPurchaseSettings_Values')
        ALTER TABLE dbo.BusinessPartnerPurchaseSettings WITH CHECK ADD CONSTRAINT CK_BusinessPartnerPurchaseSettings_Values CHECK (CommercialDiscountPercent >= 0 AND CommercialDiscountPercent <= 100 AND MinimumOrderQuantity >= 0 AND AverageDeliveryDays >= 0 AND LeadTimeDays >= 0 AND DeliveryToleranceDays >= 0);
END;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerFiscalData', N'TaxpayerTypeId') IS NULL
    ALTER TABLE dbo.BusinessPartnerFiscalData ADD TaxpayerTypeId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerFiscalData', N'TaxRegimeId') IS NULL
    ALTER TABLE dbo.BusinessPartnerFiscalData ADD TaxRegimeId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerFiscalData', N'FiscalCountryId') IS NULL
    ALTER TABLE dbo.BusinessPartnerFiscalData ADD FiscalCountryId int NULL;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerBankAccounts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerBankAccounts
    (
        BusinessPartnerBankAccountId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartnerBankAccounts PRIMARY KEY,
        BusinessPartnerId int NOT NULL,
        BankId int NULL,
        BankAccountTypeId int NULL,
        BankName nvarchar(160) NULL,
        AccountType nvarchar(60) NULL,
        AccountNumber nvarchar(80) NOT NULL,
        HolderName nvarchar(200) NULL,
        HolderIdentification nvarchar(50) NULL,
        CurrencyCode nvarchar(3) NULL,
        SwiftCode nvarchar(50) NULL,
        AbaRoutingCode nvarchar(50) NULL,
        Iban nvarchar(80) NULL,
        BankCountry nvarchar(120) NULL,
        BankCity nvarchar(120) NULL,
        Notes nvarchar(500) NULL,
        IsPrimary bit NOT NULL CONSTRAINT DF_BusinessPartnerBankAccounts_IsPrimary DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_BusinessPartnerBankAccounts_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerBankAccounts_CreatedAt DEFAULT SYSUTCDATETIME(),
        IsDeleted bit NOT NULL CONSTRAINT DF_BusinessPartnerBankAccounts_IsDeleted DEFAULT 0,
        CONSTRAINT FK_BusinessPartnerBankAccounts_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id)
    );
END;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerBankAccounts', N'BankId') IS NULL
    ALTER TABLE dbo.BusinessPartnerBankAccounts ADD BankId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerBankAccounts', N'BankAccountTypeId') IS NULL
    ALTER TABLE dbo.BusinessPartnerBankAccounts ADD BankAccountTypeId int NULL;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerSapFieldMappings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerSapFieldMappings
    (
        BusinessPartnerSapFieldMappingId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartnerSapFieldMappings PRIMARY KEY,
        BusinessPartnerId int NOT NULL,
        SystemField nvarchar(120) NOT NULL,
        SapField nvarchar(120) NOT NULL,
        Description nvarchar(300) NULL,
        IsRequired bit NOT NULL CONSTRAINT DF_BusinessPartnerSapFieldMappings_IsRequired DEFAULT 0,
        IsEnabled bit NOT NULL CONSTRAINT DF_BusinessPartnerSapFieldMappings_IsEnabled DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerSapFieldMappings_CreatedAt DEFAULT SYSUTCDATETIME(),
        IsDeleted bit NOT NULL CONSTRAINT DF_BusinessPartnerSapFieldMappings_IsDeleted DEFAULT 0,
        CONSTRAINT FK_BusinessPartnerSapFieldMappings_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerRetentionSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerRetentionSettings
    (
        BusinessPartnerRetentionSettingId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartnerRetentionSettings PRIMARY KEY,
        BusinessPartnerId int NOT NULL,
        RetentionTypeId int NULL,
        RetentionConceptId int NULL,
        TaxSupportId int NULL,
        RetentionType nvarchar(100) NULL,
        SriCode nvarchar(50) NULL,
        [Percent] decimal(9,4) NOT NULL CONSTRAINT DF_BusinessPartnerRetentionSettings_Percent DEFAULT 0,
        EntryAccountId int NULL,
        TaxSupport nvarchar(100) NULL,
        AppliesIva bit NOT NULL CONSTRAINT DF_BusinessPartnerRetentionSettings_AppliesIva DEFAULT 0,
        AppliesIncome bit NOT NULL CONSTRAINT DF_BusinessPartnerRetentionSettings_AppliesIncome DEFAULT 0,
        IsCurrent bit NOT NULL CONSTRAINT DF_BusinessPartnerRetentionSettings_IsCurrent DEFAULT 1,
        Notes nvarchar(500) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerRetentionSettings_CreatedAt DEFAULT SYSUTCDATETIME(),
        IsDeleted bit NOT NULL CONSTRAINT DF_BusinessPartnerRetentionSettings_IsDeleted DEFAULT 0,
        CONSTRAINT FK_BusinessPartnerRetentionSettings_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id)
    );
END;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerRetentionSettings', N'RetentionTypeId') IS NULL
    ALTER TABLE dbo.BusinessPartnerRetentionSettings ADD RetentionTypeId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerRetentionSettings', N'RetentionConceptId') IS NULL
    ALTER TABLE dbo.BusinessPartnerRetentionSettings ADD RetentionConceptId int NULL;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerRetentionSettings', N'TaxSupportId') IS NULL
    ALTER TABLE dbo.BusinessPartnerRetentionSettings ADD TaxSupportId int NULL;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerNotes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerNotes
    (
        BusinessPartnerId int NOT NULL CONSTRAINT PK_BusinessPartnerNotes PRIMARY KEY,
        InternalNotes nvarchar(max) NULL,
        PurchasingNotes nvarchar(max) NULL,
        PaymentNotes nvarchar(max) NULL,
        OperationalAlert nvarchar(500) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerNotes_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_BusinessPartnerNotes_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAttachmentsMetadata', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerAttachmentsMetadata
    (
        BusinessPartnerAttachmentId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartnerAttachmentsMetadata PRIMARY KEY,
        BusinessPartnerId int NOT NULL,
        AttachmentType nvarchar(80) NULL,
        FileName nvarchar(260) NOT NULL,
        Description nvarchar(300) NULL,
        ReferencePath nvarchar(500) NULL,
        FileSize bigint NULL,
        UploadedBy nvarchar(120) NULL,
        UploadedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerAttachmentsMetadata_UploadedAt DEFAULT SYSUTCDATETIME(),
        IsActive bit NOT NULL CONSTRAINT DF_BusinessPartnerAttachmentsMetadata_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_BusinessPartnerAttachmentsMetadata_IsDeleted DEFAULT 0,
        CONSTRAINT FK_BusinessPartnerAttachmentsMetadata_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id)
    );

    CREATE INDEX IX_BusinessPartnerAttachmentsMetadata_BusinessPartner
        ON dbo.BusinessPartnerAttachmentsMetadata (BusinessPartnerId, IsDeleted, IsActive);
END;
GO

IF OBJECT_ID(N'dbo.SupplierGroups', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartners_SupplierGroup')
BEGIN
    ALTER TABLE dbo.BusinessPartners WITH CHECK
    ADD CONSTRAINT FK_BusinessPartners_SupplierGroup FOREIGN KEY (SupplierGroupId) REFERENCES dbo.SupplierGroups(SupplierGroupId);
END;
GO

IF OBJECT_ID(N'dbo.SupplierClasses', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartners_SupplierClass')
BEGIN
    ALTER TABLE dbo.BusinessPartners WITH CHECK
    ADD CONSTRAINT FK_BusinessPartners_SupplierClass FOREIGN KEY (SupplierClassId) REFERENCES dbo.SupplierClasses(SupplierClassId);
END;
GO

IF OBJECT_ID(N'dbo.EconomicActivities', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartners_EconomicActivity')
BEGIN
    ALTER TABLE dbo.BusinessPartners WITH CHECK
    ADD CONSTRAINT FK_BusinessPartners_EconomicActivity FOREIGN KEY (EconomicActivityId) REFERENCES dbo.EconomicActivities(EconomicActivityId);
END;
GO

IF OBJECT_ID(N'dbo.Zones', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartners_Zone')
BEGIN
    ALTER TABLE dbo.BusinessPartners WITH CHECK
    ADD CONSTRAINT FK_BusinessPartners_Zone FOREIGN KEY (ZoneId) REFERENCES dbo.Zones(ZoneId);
END;
GO

IF OBJECT_ID(N'dbo.SupplyMethods', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartners_SupplyMethod')
BEGIN
    ALTER TABLE dbo.BusinessPartners WITH CHECK
    ADD CONSTRAINT FK_BusinessPartners_SupplyMethod FOREIGN KEY (SupplyMethodId) REFERENCES dbo.SupplyMethods(SupplyMethodId);
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'DefaultExpenseAccountId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD DefaultExpenseAccountId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'DifferenceAccountId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD DifferenceAccountId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'RoundingAccountId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD RoundingAccountId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'ClearingAccountId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD ClearingAccountId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'DiscountAccountId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD DiscountAccountId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'AccountingBySupplier') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD AccountingBySupplier bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AccountingBySupplier DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'RequiresProvision') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD RequiresProvision bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_RequiresProvision DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'AllowsAdvance') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD AllowsAdvance bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AllowsAdvance DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'AllowsCompensation') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD AllowsCompensation bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AllowsCompensation DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'AllowsPartialPayments') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD AllowsPartialPayments bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AllowsPartialPayments DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'IsPaymentBlocked') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD IsPaymentBlocked bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_IsPaymentBlocked DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'UsesWithholdingBase') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD UsesWithholdingBase bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_UsesWithholdingBase DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'ConciliationRequired') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD ConciliationRequired bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_ConciliationRequired DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'AccountingPaymentMethodId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD AccountingPaymentMethodId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'PaymentPriorityId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD PaymentPriorityId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'ApprovalFlowId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD ApprovalFlowId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'PaymentDocumentTypeId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD PaymentDocumentTypeId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'AccountingPaymentMethod') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD AccountingPaymentMethod nvarchar(80) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'PaymentPriority') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD PaymentPriority nvarchar(80) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'RequiredPaymentDay') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD RequiredPaymentDay nvarchar(80) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'ApprovalFlow') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD ApprovalFlow nvarchar(120) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'PaymentDocumentType') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD PaymentDocumentType nvarchar(80) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'AveragePaymentDays') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD AveragePaymentDays int NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AveragePaymentDays DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'PaymentTolerancePercent') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD PaymentTolerancePercent decimal(9,4) NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_PaymentTolerancePercent DEFAULT 0;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerSapMapping', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerSapMapping', N'SapEnabled') IS NULL
        ALTER TABLE dbo.BusinessPartnerSapMapping ADD SapEnabled bit NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_SapEnabled DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerSapMapping', N'SapMode') IS NULL
        ALTER TABLE dbo.BusinessPartnerSapMapping ADD SapMode nvarchar(50) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerSapMapping', N'SapCompanyCode') IS NULL
        ALTER TABLE dbo.BusinessPartnerSapMapping ADD SapCompanyCode nvarchar(80) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerSapMapping', N'SapRetryCount') IS NULL
        ALTER TABLE dbo.BusinessPartnerSapMapping ADD SapRetryCount int NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_SapRetryCount DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerSapMapping', N'SyncAsSupplier') IS NULL
        ALTER TABLE dbo.BusinessPartnerSapMapping ADD SyncAsSupplier bit NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_SyncAsSupplier DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerSapMapping', N'AllowManualSapRetry') IS NULL
        ALTER TABLE dbo.BusinessPartnerSapMapping ADD AllowManualSapRetry bit NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_AllowManualSapRetry DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerSapMapping', N'RequiresApprovalBeforeSapSync') IS NULL
        ALTER TABLE dbo.BusinessPartnerSapMapping ADD RequiresApprovalBeforeSapSync bit NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_RequiresApprovalBeforeSapSync DEFAULT 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNERS_LISTAR
    @PartnerType nvarchar(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        bp.Id, bp.Code, bp.Name, bp.CommercialName, bp.PartnerType,
        bp.IdentificationTypeId, idt.Code AS IdentificationTypeCode, idt.Name AS IdentificationTypeName,
        bp.IdentificationNumber,
        bp.SupplierGroupId, bp.SupplierClassId, bp.EconomicActivityId, bp.ZoneId, bp.SupplyMethodId,
        bp.Email, bp.Phone, bp.Website, bp.Remarks, bp.IsActive,
        fiscal.TaxpayerTypeId, fiscal.TaxRegimeId, fiscal.FiscalCountryId,
        fiscal.TaxpayerType, fiscal.IsAccountingRequired, fiscal.AppliesRetention, fiscal.FiscalRegime,
        fiscal.CountryCode, fiscal.Province, fiscal.City,
        credit.PaymentTermId, terms.Code AS PaymentTermCode, terms.Name AS PaymentTermName,
        credit.CreditDays, credit.CreditLimit, credit.DeliveryDays, credit.MinimumOrderAmount,
        credit.AllowsBackorder, credit.PreferredCurrencyCode, credit.PriceListCode, credit.AssignedSellerCode, credit.AssignedBuyerCode,
        purchase.Incoterm, purchase.CommercialDiscountPercent, purchase.PurchaseCurrencyCode,
        purchase.PreferredWarehouseId, purchase.PurchaseSupplierType, purchase.PreferredWarehouseCode,
        purchase.MinimumOrderQuantity, purchase.ActiveForImport, purchase.SubjectToEvaluation,
        purchase.AllowsUrgentPurchases, purchase.AverageDeliveryDays, purchase.LeadTimeDays,
        purchase.DeliveryToleranceDays, purchase.RequiresPurchaseOrder,
        credit.CreditStatus,
        accounting.CustomerAccountId, customerAccount.Code AS CustomerAccountCode, customerAccount.Name AS CustomerAccountName,
        accounting.SupplierAccountId, supplierAccount.Code AS SupplierAccountCode, supplierAccount.Name AS SupplierAccountName,
        accounting.CustomerAdvanceAccountId, accounting.SupplierAdvanceAccountId, accounting.RetentionAccountId, accounting.CostCenterCode,
        accounting.DefaultExpenseAccountId, accounting.DifferenceAccountId, accounting.RoundingAccountId,
        accounting.ClearingAccountId, accounting.DiscountAccountId,
        accounting.AccountingBySupplier, accounting.RequiresProvision, accounting.AllowsAdvance,
        accounting.AllowsCompensation, accounting.AllowsPartialPayments, accounting.IsPaymentBlocked,
        accounting.UsesWithholdingBase, accounting.ConciliationRequired,
        accounting.AccountingPaymentMethodId, accounting.PaymentPriorityId, accounting.ApprovalFlowId, accounting.PaymentDocumentTypeId,
        accounting.AccountingPaymentMethod, accounting.PaymentPriority, accounting.RequiredPaymentDay,
        accounting.ApprovalFlow, accounting.PaymentDocumentType, accounting.AveragePaymentDays,
        accounting.PaymentTolerancePercent,
        sap.SapCardCode, sap.SapCardType, sap.SapSyncStatus, sap.SapLastSyncAt, sap.SapLastError,
        sap.SapEnabled, sap.SapMode, sap.SapCompanyCode, sap.SapRetryCount,
        sap.SyncAsSupplier, sap.AllowManualSapRetry, sap.RequiresApprovalBeforeSapSync,
        bp.CreatedByUserId, bp.CreatedByUserName, bp.CreatedAt, bp.UpdatedByUserId, bp.UpdatedByUserName, bp.UpdatedAt
    FROM dbo.BusinessPartners bp
    INNER JOIN dbo.BusinessPartnerIdentificationTypes idt ON idt.Id = bp.IdentificationTypeId
    LEFT JOIN dbo.BusinessPartnerFiscalData fiscal ON fiscal.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.BusinessPartnerCreditSettings credit ON credit.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.BusinessPartnerPurchaseSettings purchase ON purchase.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.BusinessPartnerPaymentTerms terms ON terms.Id = credit.PaymentTermId
    LEFT JOIN dbo.BusinessPartnerAccountingSettings accounting ON accounting.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.ChartOfAccounts customerAccount ON customerAccount.Id = accounting.CustomerAccountId
    LEFT JOIN dbo.ChartOfAccounts supplierAccount ON supplierAccount.Id = accounting.SupplierAccountId
    LEFT JOIN dbo.BusinessPartnerSapMapping sap ON sap.BusinessPartnerId = bp.Id
    WHERE bp.IsDeleted = 0
      AND (@PartnerType IS NULL OR bp.PartnerType = @PartnerType OR bp.PartnerType = N'Both')
    ORDER BY bp.Code, bp.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        bp.Id, bp.Code, bp.Name, bp.CommercialName, bp.PartnerType,
        bp.IdentificationTypeId, idt.Code AS IdentificationTypeCode, idt.Name AS IdentificationTypeName,
        bp.IdentificationNumber,
        bp.SupplierGroupId, bp.SupplierClassId, bp.EconomicActivityId, bp.ZoneId, bp.SupplyMethodId,
        bp.Email, bp.Phone, bp.Website, bp.Remarks, bp.IsActive,
        fiscal.TaxpayerTypeId, fiscal.TaxRegimeId, fiscal.FiscalCountryId,
        fiscal.TaxpayerType, fiscal.IsAccountingRequired, fiscal.AppliesRetention, fiscal.FiscalRegime,
        fiscal.CountryCode, fiscal.Province, fiscal.City,
        credit.PaymentTermId, terms.Code AS PaymentTermCode, terms.Name AS PaymentTermName,
        credit.CreditDays, credit.CreditLimit, credit.DeliveryDays, credit.MinimumOrderAmount,
        credit.AllowsBackorder, credit.PreferredCurrencyCode, credit.PriceListCode, credit.AssignedSellerCode, credit.AssignedBuyerCode,
        purchase.Incoterm, purchase.CommercialDiscountPercent, purchase.PurchaseCurrencyCode,
        purchase.PreferredWarehouseId, purchase.PurchaseSupplierType, purchase.PreferredWarehouseCode,
        purchase.MinimumOrderQuantity, purchase.ActiveForImport, purchase.SubjectToEvaluation,
        purchase.AllowsUrgentPurchases, purchase.AverageDeliveryDays, purchase.LeadTimeDays,
        purchase.DeliveryToleranceDays, purchase.RequiresPurchaseOrder,
        credit.CreditStatus,
        accounting.CustomerAccountId, customerAccount.Code AS CustomerAccountCode, customerAccount.Name AS CustomerAccountName,
        accounting.SupplierAccountId, supplierAccount.Code AS SupplierAccountCode, supplierAccount.Name AS SupplierAccountName,
        accounting.CustomerAdvanceAccountId, accounting.SupplierAdvanceAccountId, accounting.RetentionAccountId, accounting.CostCenterCode,
        accounting.DefaultExpenseAccountId, accounting.DifferenceAccountId, accounting.RoundingAccountId,
        accounting.ClearingAccountId, accounting.DiscountAccountId,
        accounting.AccountingBySupplier, accounting.RequiresProvision, accounting.AllowsAdvance,
        accounting.AllowsCompensation, accounting.AllowsPartialPayments, accounting.IsPaymentBlocked,
        accounting.UsesWithholdingBase, accounting.ConciliationRequired,
        accounting.AccountingPaymentMethodId, accounting.PaymentPriorityId, accounting.ApprovalFlowId, accounting.PaymentDocumentTypeId,
        accounting.AccountingPaymentMethod, accounting.PaymentPriority, accounting.RequiredPaymentDay,
        accounting.ApprovalFlow, accounting.PaymentDocumentType, accounting.AveragePaymentDays,
        accounting.PaymentTolerancePercent,
        sap.SapCardCode, sap.SapCardType, sap.SapSyncStatus, sap.SapLastSyncAt, sap.SapLastError,
        sap.SapEnabled, sap.SapMode, sap.SapCompanyCode, sap.SapRetryCount,
        sap.SyncAsSupplier, sap.AllowManualSapRetry, sap.RequiresApprovalBeforeSapSync,
        bp.CreatedByUserId, bp.CreatedByUserName, bp.CreatedAt, bp.UpdatedByUserId, bp.UpdatedByUserName, bp.UpdatedAt
    FROM dbo.BusinessPartners bp
    INNER JOIN dbo.BusinessPartnerIdentificationTypes idt ON idt.Id = bp.IdentificationTypeId
    LEFT JOIN dbo.BusinessPartnerFiscalData fiscal ON fiscal.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.BusinessPartnerCreditSettings credit ON credit.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.BusinessPartnerPurchaseSettings purchase ON purchase.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.BusinessPartnerPaymentTerms terms ON terms.Id = credit.PaymentTermId
    LEFT JOIN dbo.BusinessPartnerAccountingSettings accounting ON accounting.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.ChartOfAccounts customerAccount ON customerAccount.Id = accounting.CustomerAccountId
    LEFT JOIN dbo.ChartOfAccounts supplierAccount ON supplierAccount.Id = accounting.SupplierAccountId
    LEFT JOIN dbo.BusinessPartnerSapMapping sap ON sap.BusinessPartnerId = bp.Id
    WHERE bp.Id = @Id
      AND bp.IsDeleted = 0;

    SELECT Id, BusinessPartnerId, CountryId, ProvinceId, CityId, AddressType, Line1, Line2, CountryCode, Province, City, PostalCode, Latitude, Longitude, IsPrimary, IsActive
    FROM dbo.BusinessPartnerAddresses
    WHERE BusinessPartnerId = @Id AND IsDeleted = 0
    ORDER BY IsPrimary DESC, AddressType, City;

    SELECT Id, BusinessPartnerId, ContactTypeId, ContactChannelId, Name, Position, Department, Phone, Extension, Mobile, Email, [Language], ReceivesNotifications, IsPrimary, IsActive, Notes
    FROM dbo.BusinessPartnerContacts
    WHERE BusinessPartnerId = @Id AND IsDeleted = 0
    ORDER BY IsPrimary DESC, Name;

    SELECT
        BusinessPartnerBankAccountId AS Id,
        BusinessPartnerId,
        BankId,
        BankAccountTypeId,
        BankName,
        AccountType,
        AccountNumber,
        HolderName,
        HolderIdentification,
        CurrencyCode,
        SwiftCode,
        AbaRoutingCode,
        Iban,
        BankCountry,
        BankCity,
        Notes,
        IsPrimary,
        IsActive
    FROM dbo.BusinessPartnerBankAccounts
    WHERE BusinessPartnerId = @Id AND IsDeleted = 0
    ORDER BY IsPrimary DESC, BankName, AccountNumber;

    SELECT
        BusinessPartnerRetentionSettingId AS Id,
        BusinessPartnerId,
        RetentionTypeId,
        RetentionConceptId,
        TaxSupportId,
        RetentionType,
        SriCode,
        [Percent],
        EntryAccountId,
        TaxSupport,
        AppliesIva,
        AppliesIncome,
        IsCurrent,
        Notes
    FROM dbo.BusinessPartnerRetentionSettings
    WHERE BusinessPartnerId = @Id AND IsDeleted = 0
    ORDER BY IsCurrent DESC, SriCode, RetentionType;

    SELECT BusinessPartnerId, InternalNotes, PurchasingNotes, PaymentNotes, OperationalAlert
    FROM dbo.BusinessPartnerNotes
    WHERE BusinessPartnerId = @Id;

    SELECT
        BusinessPartnerSapFieldMappingId AS Id,
        BusinessPartnerId,
        SystemField,
        SapField,
        Description,
        IsRequired,
        IsEnabled
    FROM dbo.BusinessPartnerSapFieldMappings
    WHERE BusinessPartnerId = @Id AND IsDeleted = 0
    ORDER BY SystemField, SapField;

    SELECT
        BusinessPartnerAttachmentId AS Id,
        BusinessPartnerId,
        AttachmentType,
        FileName,
        Description,
        ReferencePath,
        FileSize,
        UploadedBy,
        UploadedAt,
        IsActive
    FROM dbo.BusinessPartnerAttachmentsMetadata
    WHERE BusinessPartnerId = @Id AND IsDeleted = 0
    ORDER BY UploadedAt DESC, FileName;
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
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    INSERT INTO dbo.BusinessPartners
        (Code, Name, CommercialName, PartnerType, IdentificationTypeId, IdentificationNumber,
         SupplierGroupId, SupplierClassId, EconomicActivityId, ZoneId, SupplyMethodId,
         Email, Phone, Website, Remarks, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt)
    VALUES
        (@Code, @Name, @CommercialName, @PartnerType, @IdentificationTypeId, @IdentificationNumber,
         @SupplierGroupId, @SupplierClassId, @EconomicActivityId, @ZoneId, @SupplyMethodId,
         @Email, @Phone, @Website, @Remarks, @IsActive, @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME());

    DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());

    INSERT INTO dbo.BusinessPartnerFiscalData (BusinessPartnerId, TaxpayerTypeId, TaxRegimeId, FiscalCountryId, TaxpayerType, IsAccountingRequired, AppliesRetention, FiscalRegime, CountryCode, Province, City)
    VALUES (@Id, @TaxpayerTypeId, @TaxRegimeId, @FiscalCountryId, @TaxpayerType, @IsAccountingRequired, @AppliesRetention, @FiscalRegime, @CountryCode, @Province, @City);

    INSERT INTO dbo.BusinessPartnerAccountingSettings
        (BusinessPartnerId, CustomerAccountId, SupplierAccountId, CustomerAdvanceAccountId, SupplierAdvanceAccountId,
         RetentionAccountId, CostCenterCode, DefaultExpenseAccountId, DifferenceAccountId, RoundingAccountId,
         ClearingAccountId, DiscountAccountId, AccountingBySupplier, RequiresProvision, AllowsAdvance,
         AllowsCompensation, AllowsPartialPayments, IsPaymentBlocked, UsesWithholdingBase, ConciliationRequired,
         AccountingPaymentMethodId, PaymentPriorityId, ApprovalFlowId, PaymentDocumentTypeId,
         AccountingPaymentMethod, PaymentPriority, RequiredPaymentDay, ApprovalFlow, PaymentDocumentType,
         AveragePaymentDays, PaymentTolerancePercent)
    VALUES
        (@Id, @CustomerAccountId, @SupplierAccountId, @CustomerAdvanceAccountId, @SupplierAdvanceAccountId,
         @RetentionAccountId, @CostCenterCode, @DefaultExpenseAccountId, @DifferenceAccountId, @RoundingAccountId,
         @ClearingAccountId, @DiscountAccountId, @AccountingBySupplier, @RequiresProvision, @AllowsAdvance,
         @AllowsCompensation, @AllowsPartialPayments, @IsPaymentBlocked, @UsesWithholdingBase, @ConciliationRequired,
         @AccountingPaymentMethodId, @PaymentPriorityId, @ApprovalFlowId, @PaymentDocumentTypeId,
         @AccountingPaymentMethod, @PaymentPriority, @RequiredPaymentDay, @ApprovalFlow, @PaymentDocumentType,
         @AveragePaymentDays, @PaymentTolerancePercent);

    INSERT INTO dbo.BusinessPartnerCreditSettings
        (BusinessPartnerId, PaymentTermId, CreditDays, CreditLimit, DeliveryDays, MinimumOrderAmount,
         AllowsBackorder, PreferredCurrencyCode, PriceListCode, AssignedSellerCode, AssignedBuyerCode, CreditStatus)
    VALUES
        (@Id, @PaymentTermId, @CreditDays, @CreditLimit, @DeliveryDays, @MinimumOrderAmount,
         @AllowsBackorder, @PreferredCurrencyCode, @PriceListCode, @AssignedSellerCode, @AssignedBuyerCode, @CreditStatus);

    INSERT INTO dbo.BusinessPartnerPurchaseSettings
        (BusinessPartnerId, Incoterm, CommercialDiscountPercent, PurchaseCurrencyCode, PreferredWarehouseId,
         PreferredWarehouseCode, MinimumOrderQuantity, PurchaseSupplierType, ActiveForImport,
         SubjectToEvaluation, AllowsUrgentPurchases, AverageDeliveryDays, LeadTimeDays,
         DeliveryToleranceDays, RequiresPurchaseOrder)
    VALUES
        (@Id, @Incoterm, @CommercialDiscountPercent, @PurchaseCurrencyCode, @PreferredWarehouseId,
         @PreferredWarehouseCode, @MinimumOrderQuantity, @PurchaseSupplierType, @ActiveForImport,
         @SubjectToEvaluation, @AllowsUrgentPurchases, @AverageDeliveryDays, @LeadTimeDays,
         @DeliveryToleranceDays, @RequiresPurchaseOrder);

    INSERT INTO dbo.BusinessPartnerSapMapping
        (BusinessPartnerId, SapCardCode, SapCardType, SapSyncStatus, SapLastSyncAt, SapLastError,
         SapEnabled, SapMode, SapCompanyCode, SapRetryCount, SyncAsSupplier, AllowManualSapRetry, RequiresApprovalBeforeSapSync)
    VALUES
        (@Id, @SapCardCode, @SapCardType, @SapSyncStatus, @SapLastSyncAt, @SapLastError,
         @SapEnabled, @SapMode, @SapCompanyCode, @SapRetryCount, @SyncAsSupplier, @AllowManualSapRetry, @RequiresApprovalBeforeSapSync);

    INSERT INTO dbo.BusinessPartnerAddresses (BusinessPartnerId, CountryId, ProvinceId, CityId, AddressType, Line1, Line2, CountryCode, Province, City, PostalCode, Latitude, Longitude, IsPrimary, IsActive)
    SELECT @Id, CountryId, ProvinceId, CityId, AddressType, Line1, Line2, CountryCode, Province, City, PostalCode, Latitude, Longitude, IsPrimary, IsActive
    FROM OPENJSON(ISNULL(@AddressesJson, N'[]'))
    WITH (CountryId int '$.countryId', ProvinceId int '$.provinceId', CityId int '$.cityId', AddressType nvarchar(30) '$.addressType', Line1 nvarchar(300) '$.line1', Line2 nvarchar(300) '$.line2', CountryCode nvarchar(3) '$.countryCode', Province nvarchar(120) '$.province', City nvarchar(120) '$.city', PostalCode nvarchar(30) '$.postalCode', Latitude decimal(11,8) '$.latitude', Longitude decimal(11,8) '$.longitude', IsPrimary bit '$.isPrimary', IsActive bit '$.isActive')
    WHERE NULLIF(Line1, N'') IS NOT NULL;

    INSERT INTO dbo.BusinessPartnerContacts (BusinessPartnerId, ContactTypeId, ContactChannelId, Name, Position, Department, Phone, Extension, Mobile, Email, [Language], ReceivesNotifications, IsPrimary, IsActive, Notes)
    SELECT @Id, ContactTypeId, ContactChannelId, Name, Position, Department, Phone, Extension, Mobile, Email, [Language], ReceivesNotifications, IsPrimary, IsActive, Notes
    FROM OPENJSON(ISNULL(@ContactsJson, N'[]'))
    WITH (ContactTypeId int '$.contactTypeId', ContactChannelId int '$.contactChannelId', Name nvarchar(150) '$.name', Position nvarchar(120) '$.position', Department nvarchar(120) '$.department', Phone nvarchar(50) '$.phone', Extension nvarchar(20) '$.extension', Mobile nvarchar(50) '$.mobile', Email nvarchar(256) '$.email', [Language] nvarchar(50) '$.language', ReceivesNotifications bit '$.receivesNotifications', IsPrimary bit '$.isPrimary', IsActive bit '$.isActive', Notes nvarchar(500) '$.notes')
    WHERE NULLIF(Name, N'') IS NOT NULL;

    INSERT INTO dbo.BusinessPartnerBankAccounts
        (BusinessPartnerId, BankId, BankAccountTypeId, BankName, AccountType, AccountNumber, HolderName, HolderIdentification, CurrencyCode, SwiftCode, AbaRoutingCode, Iban, BankCountry, BankCity, Notes, IsPrimary, IsActive)
    SELECT
        @Id, BankId, BankAccountTypeId, BankName, AccountType, AccountNumber, HolderName, HolderIdentification, CurrencyCode, SwiftCode, AbaRoutingCode, Iban, BankCountry, BankCity, Notes, IsPrimary, IsActive
    FROM OPENJSON(ISNULL(@BankAccountsJson, N'[]'))
    WITH
    (
        BankId int '$.bankId',
        BankAccountTypeId int '$.bankAccountTypeId',
        BankName nvarchar(160) '$.bankName',
        AccountType nvarchar(60) '$.accountType',
        AccountNumber nvarchar(80) '$.accountNumber',
        HolderName nvarchar(200) '$.holderName',
        HolderIdentification nvarchar(50) '$.holderIdentification',
        CurrencyCode nvarchar(3) '$.currencyCode',
        SwiftCode nvarchar(50) '$.swiftCode',
        AbaRoutingCode nvarchar(50) '$.abaRoutingCode',
        Iban nvarchar(80) '$.iban',
        BankCountry nvarchar(120) '$.bankCountry',
        BankCity nvarchar(120) '$.bankCity',
        Notes nvarchar(500) '$.notes',
        IsPrimary bit '$.isPrimary',
        IsActive bit '$.isActive'
    )
    WHERE NULLIF(AccountNumber, N'') IS NOT NULL;

    INSERT INTO dbo.BusinessPartnerRetentionSettings
        (BusinessPartnerId, RetentionTypeId, RetentionConceptId, TaxSupportId, RetentionType, SriCode, [Percent], EntryAccountId, TaxSupport, AppliesIva, AppliesIncome, IsCurrent, Notes)
    SELECT @Id, RetentionTypeId, RetentionConceptId, TaxSupportId, RetentionType, SriCode, [Percent], EntryAccountId, TaxSupport, AppliesIva, AppliesIncome, IsCurrent, Notes
    FROM OPENJSON(ISNULL(@RetentionSettingsJson, N'[]'))
    WITH
    (
        RetentionTypeId int '$.retentionTypeId',
        RetentionConceptId int '$.retentionConceptId',
        TaxSupportId int '$.taxSupportId',
        RetentionType nvarchar(100) '$.retentionType',
        SriCode nvarchar(50) '$.sriCode',
        [Percent] decimal(9,4) '$.percent',
        EntryAccountId int '$.entryAccountId',
        TaxSupport nvarchar(100) '$.taxSupport',
        AppliesIva bit '$.appliesIva',
        AppliesIncome bit '$.appliesIncome',
        IsCurrent bit '$.isCurrent',
        Notes nvarchar(500) '$.notes'
    )
    WHERE NULLIF(RetentionType, N'') IS NOT NULL
       OR NULLIF(SriCode, N'') IS NOT NULL;

    INSERT INTO dbo.BusinessPartnerNotes (BusinessPartnerId, InternalNotes, PurchasingNotes, PaymentNotes, OperationalAlert)
    SELECT @Id, InternalNotes, PurchasingNotes, PaymentNotes, OperationalAlert
    FROM OPENJSON(CONCAT(N'[', ISNULL(NULLIF(@NotesJson, N'null'), N'{}'), N']'))
    WITH
    (
        InternalNotes nvarchar(max) '$.internalNotes',
        PurchasingNotes nvarchar(max) '$.purchasingNotes',
        PaymentNotes nvarchar(max) '$.paymentNotes',
        OperationalAlert nvarchar(500) '$.operationalAlert'
    )
    WHERE InternalNotes IS NOT NULL
       OR PurchasingNotes IS NOT NULL
       OR PaymentNotes IS NOT NULL
       OR OperationalAlert IS NOT NULL;

    INSERT INTO dbo.BusinessPartnerSapFieldMappings
        (BusinessPartnerId, SystemField, SapField, Description, IsRequired, IsEnabled)
    SELECT @Id, SystemField, SapField, Description, IsRequired, IsEnabled
    FROM OPENJSON(ISNULL(@SapFieldMappingsJson, N'[]'))
    WITH
    (
        SystemField nvarchar(120) '$.systemField',
        SapField nvarchar(120) '$.sapField',
        Description nvarchar(300) '$.description',
        IsRequired bit '$.isRequired',
        IsEnabled bit '$.isEnabled'
    )
    WHERE NULLIF(SystemField, N'') IS NOT NULL
      AND NULLIF(SapField, N'') IS NOT NULL;

    INSERT INTO dbo.BusinessPartnerAttachmentsMetadata
        (BusinessPartnerId, AttachmentType, FileName, Description, ReferencePath, FileSize, UploadedBy, IsActive)
    SELECT @Id, AttachmentType, FileName, Description, ReferencePath, FileSize, @CreatedByUserName, IsActive
    FROM OPENJSON(ISNULL(@AttachmentsJson, N'[]'))
    WITH
    (
        AttachmentType nvarchar(80) '$.attachmentType',
        FileName nvarchar(260) '$.fileName',
        Description nvarchar(300) '$.description',
        ReferencePath nvarchar(500) '$.referencePath',
        FileSize bigint '$.fileSize',
        IsActive bit '$.isActive'
    )
    WHERE NULLIF(FileName, N'') IS NOT NULL;

    INSERT INTO dbo.AuditCatalogChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    VALUES (N'BusinessPartners', CONVERT(nvarchar(80), @Id), N'INSERT', N'Code', NULL, @Code, @CreatedByUserId, @CreatedByUserName);

    COMMIT TRANSACTION;
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
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @OldCode nvarchar(50), @OldName nvarchar(200), @OldIsActive bit;
    SELECT @OldCode = Code, @OldName = Name, @OldIsActive = IsActive FROM dbo.BusinessPartners WHERE Id = @Id AND IsDeleted = 0;
    IF @OldCode IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.BusinessPartners
    SET Code = @Code, Name = @Name, CommercialName = @CommercialName, PartnerType = @PartnerType,
        IdentificationTypeId = @IdentificationTypeId, IdentificationNumber = @IdentificationNumber,
        SupplierGroupId = @SupplierGroupId, SupplierClassId = @SupplierClassId,
        EconomicActivityId = @EconomicActivityId, ZoneId = @ZoneId, SupplyMethodId = @SupplyMethodId,
        Email = @Email, Phone = @Phone, Website = @Website, Remarks = @Remarks, IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName, UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    UPDATE dbo.BusinessPartnerFiscalData
    SET TaxpayerTypeId = @TaxpayerTypeId, TaxRegimeId = @TaxRegimeId, FiscalCountryId = @FiscalCountryId,
        TaxpayerType = @TaxpayerType, IsAccountingRequired = @IsAccountingRequired, AppliesRetention = @AppliesRetention,
        FiscalRegime = @FiscalRegime, CountryCode = @CountryCode, Province = @Province, City = @City
    WHERE BusinessPartnerId = @Id;

    UPDATE dbo.BusinessPartnerAccountingSettings
    SET CustomerAccountId = @CustomerAccountId, SupplierAccountId = @SupplierAccountId,
        CustomerAdvanceAccountId = @CustomerAdvanceAccountId, SupplierAdvanceAccountId = @SupplierAdvanceAccountId,
        RetentionAccountId = @RetentionAccountId, CostCenterCode = @CostCenterCode,
        DefaultExpenseAccountId = @DefaultExpenseAccountId,
        DifferenceAccountId = @DifferenceAccountId,
        RoundingAccountId = @RoundingAccountId,
        ClearingAccountId = @ClearingAccountId,
        DiscountAccountId = @DiscountAccountId,
        AccountingBySupplier = @AccountingBySupplier,
        RequiresProvision = @RequiresProvision,
        AllowsAdvance = @AllowsAdvance,
        AllowsCompensation = @AllowsCompensation,
        AllowsPartialPayments = @AllowsPartialPayments,
        IsPaymentBlocked = @IsPaymentBlocked,
        UsesWithholdingBase = @UsesWithholdingBase,
        ConciliationRequired = @ConciliationRequired,
        AccountingPaymentMethodId = @AccountingPaymentMethodId,
        PaymentPriorityId = @PaymentPriorityId,
        ApprovalFlowId = @ApprovalFlowId,
        PaymentDocumentTypeId = @PaymentDocumentTypeId,
        AccountingPaymentMethod = @AccountingPaymentMethod,
        PaymentPriority = @PaymentPriority,
        RequiredPaymentDay = @RequiredPaymentDay,
        ApprovalFlow = @ApprovalFlow,
        PaymentDocumentType = @PaymentDocumentType,
        AveragePaymentDays = @AveragePaymentDays,
        PaymentTolerancePercent = @PaymentTolerancePercent
    WHERE BusinessPartnerId = @Id;

    UPDATE dbo.BusinessPartnerCreditSettings
    SET PaymentTermId = @PaymentTermId, CreditDays = @CreditDays, CreditLimit = @CreditLimit,
        DeliveryDays = @DeliveryDays, MinimumOrderAmount = @MinimumOrderAmount,
        AllowsBackorder = @AllowsBackorder, PreferredCurrencyCode = @PreferredCurrencyCode, PriceListCode = @PriceListCode, AssignedSellerCode = @AssignedSellerCode,
        AssignedBuyerCode = @AssignedBuyerCode,
        CreditStatus = @CreditStatus
    WHERE BusinessPartnerId = @Id;

    IF EXISTS (SELECT 1 FROM dbo.BusinessPartnerPurchaseSettings WHERE BusinessPartnerId = @Id)
    BEGIN
        UPDATE dbo.BusinessPartnerPurchaseSettings
        SET Incoterm = @Incoterm,
            CommercialDiscountPercent = @CommercialDiscountPercent,
            PurchaseCurrencyCode = @PurchaseCurrencyCode,
            PreferredWarehouseId = @PreferredWarehouseId,
            PreferredWarehouseCode = @PreferredWarehouseCode,
            MinimumOrderQuantity = @MinimumOrderQuantity,
            PurchaseSupplierType = @PurchaseSupplierType,
            ActiveForImport = @ActiveForImport,
            SubjectToEvaluation = @SubjectToEvaluation,
            AllowsUrgentPurchases = @AllowsUrgentPurchases,
            AverageDeliveryDays = @AverageDeliveryDays,
            LeadTimeDays = @LeadTimeDays,
            DeliveryToleranceDays = @DeliveryToleranceDays,
            RequiresPurchaseOrder = @RequiresPurchaseOrder,
            UpdatedAt = SYSUTCDATETIME()
        WHERE BusinessPartnerId = @Id;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.BusinessPartnerPurchaseSettings
            (BusinessPartnerId, Incoterm, CommercialDiscountPercent, PurchaseCurrencyCode, PreferredWarehouseId,
             PreferredWarehouseCode, MinimumOrderQuantity, PurchaseSupplierType, ActiveForImport,
             SubjectToEvaluation, AllowsUrgentPurchases, AverageDeliveryDays, LeadTimeDays,
             DeliveryToleranceDays, RequiresPurchaseOrder)
        VALUES
            (@Id, @Incoterm, @CommercialDiscountPercent, @PurchaseCurrencyCode, @PreferredWarehouseId,
             @PreferredWarehouseCode, @MinimumOrderQuantity, @PurchaseSupplierType, @ActiveForImport,
             @SubjectToEvaluation, @AllowsUrgentPurchases, @AverageDeliveryDays, @LeadTimeDays,
             @DeliveryToleranceDays, @RequiresPurchaseOrder);
    END;

    UPDATE dbo.BusinessPartnerSapMapping
    SET SapCardCode = @SapCardCode, SapCardType = @SapCardType, SapSyncStatus = @SapSyncStatus,
        SapLastSyncAt = @SapLastSyncAt, SapLastError = @SapLastError,
        SapEnabled = @SapEnabled,
        SapMode = @SapMode,
        SapCompanyCode = @SapCompanyCode,
        SapRetryCount = @SapRetryCount,
        SyncAsSupplier = @SyncAsSupplier,
        AllowManualSapRetry = @AllowManualSapRetry,
        RequiresApprovalBeforeSapSync = @RequiresApprovalBeforeSapSync
    WHERE BusinessPartnerId = @Id;

    UPDATE dbo.BusinessPartnerAddresses SET IsDeleted = 1, IsActive = 0 WHERE BusinessPartnerId = @Id AND IsDeleted = 0;
    INSERT INTO dbo.BusinessPartnerAddresses (BusinessPartnerId, CountryId, ProvinceId, CityId, AddressType, Line1, Line2, CountryCode, Province, City, PostalCode, Latitude, Longitude, IsPrimary, IsActive)
    SELECT @Id, CountryId, ProvinceId, CityId, AddressType, Line1, Line2, CountryCode, Province, City, PostalCode, Latitude, Longitude, IsPrimary, IsActive
    FROM OPENJSON(ISNULL(@AddressesJson, N'[]'))
    WITH (CountryId int '$.countryId', ProvinceId int '$.provinceId', CityId int '$.cityId', AddressType nvarchar(30) '$.addressType', Line1 nvarchar(300) '$.line1', Line2 nvarchar(300) '$.line2', CountryCode nvarchar(3) '$.countryCode', Province nvarchar(120) '$.province', City nvarchar(120) '$.city', PostalCode nvarchar(30) '$.postalCode', Latitude decimal(11,8) '$.latitude', Longitude decimal(11,8) '$.longitude', IsPrimary bit '$.isPrimary', IsActive bit '$.isActive')
    WHERE NULLIF(Line1, N'') IS NOT NULL;

    UPDATE dbo.BusinessPartnerContacts SET IsDeleted = 1, IsActive = 0 WHERE BusinessPartnerId = @Id AND IsDeleted = 0;
    INSERT INTO dbo.BusinessPartnerContacts (BusinessPartnerId, ContactTypeId, ContactChannelId, Name, Position, Department, Phone, Extension, Mobile, Email, [Language], ReceivesNotifications, IsPrimary, IsActive, Notes)
    SELECT @Id, ContactTypeId, ContactChannelId, Name, Position, Department, Phone, Extension, Mobile, Email, [Language], ReceivesNotifications, IsPrimary, IsActive, Notes
    FROM OPENJSON(ISNULL(@ContactsJson, N'[]'))
    WITH (ContactTypeId int '$.contactTypeId', ContactChannelId int '$.contactChannelId', Name nvarchar(150) '$.name', Position nvarchar(120) '$.position', Department nvarchar(120) '$.department', Phone nvarchar(50) '$.phone', Extension nvarchar(20) '$.extension', Mobile nvarchar(50) '$.mobile', Email nvarchar(256) '$.email', [Language] nvarchar(50) '$.language', ReceivesNotifications bit '$.receivesNotifications', IsPrimary bit '$.isPrimary', IsActive bit '$.isActive', Notes nvarchar(500) '$.notes')
    WHERE NULLIF(Name, N'') IS NOT NULL;

    UPDATE dbo.BusinessPartnerBankAccounts SET IsDeleted = 1, IsActive = 0 WHERE BusinessPartnerId = @Id AND IsDeleted = 0;
    INSERT INTO dbo.BusinessPartnerBankAccounts
        (BusinessPartnerId, BankId, BankAccountTypeId, BankName, AccountType, AccountNumber, HolderName, HolderIdentification, CurrencyCode, SwiftCode, AbaRoutingCode, Iban, BankCountry, BankCity, Notes, IsPrimary, IsActive)
    SELECT
        @Id, BankId, BankAccountTypeId, BankName, AccountType, AccountNumber, HolderName, HolderIdentification, CurrencyCode, SwiftCode, AbaRoutingCode, Iban, BankCountry, BankCity, Notes, IsPrimary, IsActive
    FROM OPENJSON(ISNULL(@BankAccountsJson, N'[]'))
    WITH
    (
        BankId int '$.bankId',
        BankAccountTypeId int '$.bankAccountTypeId',
        BankName nvarchar(160) '$.bankName',
        AccountType nvarchar(60) '$.accountType',
        AccountNumber nvarchar(80) '$.accountNumber',
        HolderName nvarchar(200) '$.holderName',
        HolderIdentification nvarchar(50) '$.holderIdentification',
        CurrencyCode nvarchar(3) '$.currencyCode',
        SwiftCode nvarchar(50) '$.swiftCode',
        AbaRoutingCode nvarchar(50) '$.abaRoutingCode',
        Iban nvarchar(80) '$.iban',
        BankCountry nvarchar(120) '$.bankCountry',
        BankCity nvarchar(120) '$.bankCity',
        Notes nvarchar(500) '$.notes',
        IsPrimary bit '$.isPrimary',
        IsActive bit '$.isActive'
    )
    WHERE NULLIF(AccountNumber, N'') IS NOT NULL;

    UPDATE dbo.BusinessPartnerRetentionSettings SET IsDeleted = 1, IsCurrent = 0 WHERE BusinessPartnerId = @Id AND IsDeleted = 0;
    INSERT INTO dbo.BusinessPartnerRetentionSettings
        (BusinessPartnerId, RetentionTypeId, RetentionConceptId, TaxSupportId, RetentionType, SriCode, [Percent], EntryAccountId, TaxSupport, AppliesIva, AppliesIncome, IsCurrent, Notes)
    SELECT @Id, RetentionTypeId, RetentionConceptId, TaxSupportId, RetentionType, SriCode, [Percent], EntryAccountId, TaxSupport, AppliesIva, AppliesIncome, IsCurrent, Notes
    FROM OPENJSON(ISNULL(@RetentionSettingsJson, N'[]'))
    WITH
    (
        RetentionTypeId int '$.retentionTypeId',
        RetentionConceptId int '$.retentionConceptId',
        TaxSupportId int '$.taxSupportId',
        RetentionType nvarchar(100) '$.retentionType',
        SriCode nvarchar(50) '$.sriCode',
        [Percent] decimal(9,4) '$.percent',
        EntryAccountId int '$.entryAccountId',
        TaxSupport nvarchar(100) '$.taxSupport',
        AppliesIva bit '$.appliesIva',
        AppliesIncome bit '$.appliesIncome',
        IsCurrent bit '$.isCurrent',
        Notes nvarchar(500) '$.notes'
    )
    WHERE NULLIF(RetentionType, N'') IS NOT NULL
       OR NULLIF(SriCode, N'') IS NOT NULL;

    DELETE FROM dbo.BusinessPartnerNotes WHERE BusinessPartnerId = @Id;
    INSERT INTO dbo.BusinessPartnerNotes (BusinessPartnerId, InternalNotes, PurchasingNotes, PaymentNotes, OperationalAlert, UpdatedAt)
    SELECT @Id, InternalNotes, PurchasingNotes, PaymentNotes, OperationalAlert, SYSUTCDATETIME()
    FROM OPENJSON(CONCAT(N'[', ISNULL(NULLIF(@NotesJson, N'null'), N'{}'), N']'))
    WITH
    (
        InternalNotes nvarchar(max) '$.internalNotes',
        PurchasingNotes nvarchar(max) '$.purchasingNotes',
        PaymentNotes nvarchar(max) '$.paymentNotes',
        OperationalAlert nvarchar(500) '$.operationalAlert'
    )
    WHERE InternalNotes IS NOT NULL
       OR PurchasingNotes IS NOT NULL
       OR PaymentNotes IS NOT NULL
       OR OperationalAlert IS NOT NULL;

    UPDATE dbo.BusinessPartnerSapFieldMappings SET IsDeleted = 1, IsEnabled = 0 WHERE BusinessPartnerId = @Id AND IsDeleted = 0;
    INSERT INTO dbo.BusinessPartnerSapFieldMappings
        (BusinessPartnerId, SystemField, SapField, Description, IsRequired, IsEnabled)
    SELECT @Id, SystemField, SapField, Description, IsRequired, IsEnabled
    FROM OPENJSON(ISNULL(@SapFieldMappingsJson, N'[]'))
    WITH
    (
        SystemField nvarchar(120) '$.systemField',
        SapField nvarchar(120) '$.sapField',
        Description nvarchar(300) '$.description',
        IsRequired bit '$.isRequired',
        IsEnabled bit '$.isEnabled'
    )
    WHERE NULLIF(SystemField, N'') IS NOT NULL
      AND NULLIF(SapField, N'') IS NOT NULL;

    IF @AttachmentsJson IS NOT NULL
    BEGIN
        UPDATE dbo.BusinessPartnerAttachmentsMetadata
        SET IsDeleted = 1, IsActive = 0
        WHERE BusinessPartnerId = @Id AND IsDeleted = 0;

        INSERT INTO dbo.BusinessPartnerAttachmentsMetadata
            (BusinessPartnerId, AttachmentType, FileName, Description, ReferencePath, FileSize, UploadedBy, IsActive)
        SELECT @Id, AttachmentType, FileName, Description, ReferencePath, FileSize, @UpdatedByUserName, IsActive
        FROM OPENJSON(ISNULL(@AttachmentsJson, N'[]'))
        WITH
        (
            AttachmentType nvarchar(80) '$.attachmentType',
            FileName nvarchar(260) '$.fileName',
            Description nvarchar(300) '$.description',
            ReferencePath nvarchar(500) '$.referencePath',
            FileSize bigint '$.fileSize',
            IsActive bit '$.isActive'
        )
        WHERE NULLIF(FileName, N'') IS NOT NULL;
    END;

    INSERT INTO dbo.AuditCatalogChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'BusinessPartners', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM (VALUES
        (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
        (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
        (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    COMMIT TRANSACTION;
    SELECT @AffectedRows;
END;
GO
