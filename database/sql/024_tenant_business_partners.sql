/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    Reemplaza los mantenimientos tempranos de Customers/Documents por un maestro
    empresarial de terceros comerciales para clientes y proveedores.
*/

IF OBJECT_ID(N'dbo.DocumentLines', N'U') IS NOT NULL
    DROP TABLE dbo.DocumentLines;
GO

IF OBJECT_ID(N'dbo.Documents', N'U') IS NOT NULL
    DROP TABLE dbo.Documents;
GO

IF OBJECT_ID(N'dbo.Customers', N'U') IS NOT NULL
    DROP TABLE dbo.Customers;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerIdentificationTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerIdentificationTypes
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartnerIdentificationTypes PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(120) NOT NULL,
        CountryCode nvarchar(3) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_BusinessPartnerIdentificationTypes_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerIdentificationTypes_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_BusinessPartnerIdentificationTypes_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_BusinessPartnerIdentificationTypes_Code_Active' AND object_id = OBJECT_ID(N'dbo.BusinessPartnerIdentificationTypes'))
BEGIN
    CREATE UNIQUE INDEX UX_BusinessPartnerIdentificationTypes_Code_Active ON dbo.BusinessPartnerIdentificationTypes (Code) WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerPaymentTerms', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerPaymentTerms
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartnerPaymentTerms PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Days int NOT NULL CONSTRAINT DF_BusinessPartnerPaymentTerms_Days DEFAULT 0,
        IsCredit bit NOT NULL CONSTRAINT DF_BusinessPartnerPaymentTerms_IsCredit DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_BusinessPartnerPaymentTerms_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerPaymentTerms_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_BusinessPartnerPaymentTerms_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT CK_BusinessPartnerPaymentTerms_Days CHECK (Days >= 0)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_BusinessPartnerPaymentTerms_Code_Active' AND object_id = OBJECT_ID(N'dbo.BusinessPartnerPaymentTerms'))
BEGIN
    CREATE UNIQUE INDEX UX_BusinessPartnerPaymentTerms_Code_Active ON dbo.BusinessPartnerPaymentTerms (Code) WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartners
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartners PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(200) NOT NULL,
        CommercialName nvarchar(200) NULL,
        PartnerType nvarchar(20) NOT NULL,
        IdentificationTypeId int NOT NULL,
        IdentificationNumber nvarchar(50) NOT NULL,
        Email nvarchar(256) NULL,
        Phone nvarchar(50) NULL,
        Website nvarchar(200) NULL,
        Remarks nvarchar(1000) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_BusinessPartners_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartners_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_BusinessPartners_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_BusinessPartners_IdentificationType FOREIGN KEY (IdentificationTypeId) REFERENCES dbo.BusinessPartnerIdentificationTypes(Id),
        CONSTRAINT CK_BusinessPartners_PartnerType CHECK (PartnerType IN (N'Customer', N'Supplier', N'Both'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_BusinessPartners_Code_Active' AND object_id = OBJECT_ID(N'dbo.BusinessPartners'))
    CREATE UNIQUE INDEX UX_BusinessPartners_Code_Active ON dbo.BusinessPartners (Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_BusinessPartners_Identification_Active' AND object_id = OBJECT_ID(N'dbo.BusinessPartners'))
    CREATE UNIQUE INDEX UX_BusinessPartners_Identification_Active ON dbo.BusinessPartners (IdentificationTypeId, IdentificationNumber) WHERE IsDeleted = 0;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAddresses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerAddresses
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartnerAddresses PRIMARY KEY,
        BusinessPartnerId int NOT NULL,
        AddressType nvarchar(30) NOT NULL,
        Line1 nvarchar(300) NOT NULL,
        Line2 nvarchar(300) NULL,
        CountryId int NULL,
        ProvinceId int NULL,
        CityId int NULL,
        CountryCode nvarchar(3) NULL,
        Province nvarchar(120) NULL,
        City nvarchar(120) NULL,
        PostalCode nvarchar(30) NULL,
        Latitude decimal(11,8) NULL,
        Longitude decimal(11,8) NULL,
        IsPrimary bit NOT NULL CONSTRAINT DF_BusinessPartnerAddresses_IsPrimary DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_BusinessPartnerAddresses_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerAddresses_CreatedAt DEFAULT SYSUTCDATETIME(),
        IsDeleted bit NOT NULL CONSTRAINT DF_BusinessPartnerAddresses_IsDeleted DEFAULT 0,
        CONSTRAINT FK_BusinessPartnerAddresses_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id),
        CONSTRAINT CK_BusinessPartnerAddresses_AddressType CHECK (AddressType IN (N'Main', N'Billing', N'Shipping', N'Other'))
    );
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerContacts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerContacts
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessPartnerContacts PRIMARY KEY,
        BusinessPartnerId int NOT NULL,
        ContactTypeId int NULL,
        ContactChannelId int NULL,
        Name nvarchar(150) NOT NULL,
        Position nvarchar(120) NULL,
        Department nvarchar(120) NULL,
        Phone nvarchar(50) NULL,
        Extension nvarchar(20) NULL,
        Mobile nvarchar(50) NULL,
        Email nvarchar(256) NULL,
        [Language] nvarchar(50) NULL,
        ReceivesNotifications bit NOT NULL CONSTRAINT DF_BusinessPartnerContacts_ReceivesNotifications DEFAULT 0,
        IsPrimary bit NOT NULL CONSTRAINT DF_BusinessPartnerContacts_IsPrimary DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_BusinessPartnerContacts_IsActive DEFAULT 1,
        Notes nvarchar(500) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerContacts_CreatedAt DEFAULT SYSUTCDATETIME(),
        IsDeleted bit NOT NULL CONSTRAINT DF_BusinessPartnerContacts_IsDeleted DEFAULT 0,
        CONSTRAINT FK_BusinessPartnerContacts_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerFiscalData', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerFiscalData
    (
        BusinessPartnerId int NOT NULL CONSTRAINT PK_BusinessPartnerFiscalData PRIMARY KEY,
        TaxpayerTypeId int NULL,
        TaxRegimeId int NULL,
        FiscalCountryId int NULL,
        TaxpayerType nvarchar(60) NULL,
        IsAccountingRequired bit NOT NULL CONSTRAINT DF_BusinessPartnerFiscalData_IsAccountingRequired DEFAULT 0,
        AppliesRetention bit NOT NULL CONSTRAINT DF_BusinessPartnerFiscalData_AppliesRetention DEFAULT 0,
        FiscalRegime nvarchar(80) NULL,
        CountryCode nvarchar(3) NULL,
        Province nvarchar(120) NULL,
        City nvarchar(120) NULL,
        CONSTRAINT FK_BusinessPartnerFiscalData_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerAccountingSettings
    (
        BusinessPartnerId int NOT NULL CONSTRAINT PK_BusinessPartnerAccountingSettings PRIMARY KEY,
        CustomerAccountId int NULL,
        SupplierAccountId int NULL,
        CustomerAdvanceAccountId int NULL,
        SupplierAdvanceAccountId int NULL,
        RetentionAccountId int NULL,
        CostCenterCode nvarchar(50) NULL,
        DefaultExpenseAccountId int NULL,
        DifferenceAccountId int NULL,
        RoundingAccountId int NULL,
        ClearingAccountId int NULL,
        DiscountAccountId int NULL,
        AccountingBySupplier bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AccountingBySupplier DEFAULT 0,
        RequiresProvision bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_RequiresProvision DEFAULT 0,
        AllowsAdvance bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AllowsAdvance DEFAULT 0,
        AllowsCompensation bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AllowsCompensation DEFAULT 0,
        AllowsPartialPayments bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AllowsPartialPayments DEFAULT 0,
        IsPaymentBlocked bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_IsPaymentBlocked DEFAULT 0,
        UsesWithholdingBase bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_UsesWithholdingBase DEFAULT 0,
        ConciliationRequired bit NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_ConciliationRequired DEFAULT 0,
        AccountingPaymentMethodId int NULL,
        PaymentPriorityId int NULL,
        ApprovalFlowId int NULL,
        PaymentDocumentTypeId int NULL,
        AccountingPaymentMethod nvarchar(80) NULL,
        PaymentPriority nvarchar(80) NULL,
        RequiredPaymentDay nvarchar(80) NULL,
        ApprovalFlow nvarchar(120) NULL,
        PaymentDocumentType nvarchar(80) NULL,
        AveragePaymentDays int NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_AveragePaymentDays DEFAULT 0,
        PaymentTolerancePercent decimal(9,4) NOT NULL CONSTRAINT DF_BusinessPartnerAccountingSettings_PaymentTolerancePercent DEFAULT 0,
        CONSTRAINT FK_BusinessPartnerAccountingSettings_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerCreditSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerCreditSettings
    (
        BusinessPartnerId int NOT NULL CONSTRAINT PK_BusinessPartnerCreditSettings PRIMARY KEY,
        PaymentTermId int NULL,
        CreditDays int NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_CreditDays DEFAULT 0,
        CreditLimit decimal(19,6) NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_CreditLimit DEFAULT 0,
        DeliveryDays int NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_DeliveryDays DEFAULT 0,
        MinimumOrderAmount decimal(19,6) NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_MinimumOrderAmount DEFAULT 0,
        AllowsBackorder bit NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_AllowsBackorder DEFAULT 0,
        PreferredCurrencyCode nvarchar(3) NULL,
        PriceListCode nvarchar(50) NULL,
        AssignedSellerCode nvarchar(50) NULL,
        AssignedBuyerCode nvarchar(50) NULL,
        CreditStatus nvarchar(30) NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_CreditStatus DEFAULT N'Normal',
        CONSTRAINT FK_BusinessPartnerCreditSettings_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id),
        CONSTRAINT FK_BusinessPartnerCreditSettings_PaymentTerms FOREIGN KEY (PaymentTermId) REFERENCES dbo.BusinessPartnerPaymentTerms(Id),
        CONSTRAINT CK_BusinessPartnerCreditSettings_Credit CHECK (CreditDays >= 0 AND CreditLimit >= 0)
    );
END;
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

IF OBJECT_ID(N'dbo.BusinessPartnerSapMapping', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessPartnerSapMapping
    (
        BusinessPartnerId int NOT NULL CONSTRAINT PK_BusinessPartnerSapMapping PRIMARY KEY,
        SapCardCode nvarchar(50) NULL,
        SapCardType nvarchar(1) NULL,
        SapSyncStatus nvarchar(30) NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_SapSyncStatus DEFAULT N'Pending',
        SapLastSyncAt datetime2(0) NULL,
        SapLastError nvarchar(max) NULL,
        SapEnabled bit NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_SapEnabled DEFAULT 0,
        SapMode nvarchar(50) NULL,
        SapCompanyCode nvarchar(80) NULL,
        SapRetryCount int NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_SapRetryCount DEFAULT 0,
        SyncAsSupplier bit NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_SyncAsSupplier DEFAULT 0,
        AllowManualSapRetry bit NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_AllowManualSapRetry DEFAULT 0,
        RequiresApprovalBeforeSapSync bit NOT NULL CONSTRAINT DF_BusinessPartnerSapMapping_RequiresApprovalBeforeSapSync DEFAULT 0,
        CONSTRAINT FK_BusinessPartnerSapMapping_BusinessPartners FOREIGN KEY (BusinessPartnerId) REFERENCES dbo.BusinessPartners(Id),
        CONSTRAINT CK_BusinessPartnerSapMapping_CardType CHECK (SapCardType IS NULL OR SapCardType IN (N'C', N'S', N'L'))
    );
END;
GO

IF OBJECT_ID(N'dbo.AuditCatalogChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditCatalogChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditCatalogChanges PRIMARY KEY,
        EntityName nvarchar(120) NOT NULL,
        RecordId nvarchar(80) NOT NULL,
        [Action] nvarchar(30) NOT NULL,
        FieldName nvarchar(120) NOT NULL,
        OldValue nvarchar(max) NULL,
        NewValue nvarchar(max) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        [Source] nvarchar(60) NOT NULL CONSTRAINT DF_AuditCatalogChanges_Source DEFAULT N'API',
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditCatalogChanges_CreatedAt DEFAULT SYSUTCDATETIME()
    );
    CREATE INDEX IX_AuditCatalogChanges_Entity_Record_CreatedAt ON dbo.AuditCatalogChanges (EntityName, RecordId, CreatedAt DESC);
    CREATE INDEX IX_AuditCatalogChanges_User_CreatedAt ON dbo.AuditCatalogChanges (UserId, CreatedAt DESC);
    CREATE INDEX IX_AuditCatalogChanges_CreatedAt ON dbo.AuditCatalogChanges (CreatedAt DESC);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerIdentificationTypes WHERE Code = N'RUC' AND IsDeleted = 0)
    INSERT INTO dbo.BusinessPartnerIdentificationTypes (Code, Name, CountryCode, CreatedByUserName) VALUES (N'RUC', N'RUC', N'ECU', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerIdentificationTypes WHERE Code = N'CEDULA' AND IsDeleted = 0)
    INSERT INTO dbo.BusinessPartnerIdentificationTypes (Code, Name, CountryCode, CreatedByUserName) VALUES (N'CEDULA', N'Cedula', N'ECU', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerIdentificationTypes WHERE Code = N'PASAPORTE' AND IsDeleted = 0)
    INSERT INTO dbo.BusinessPartnerIdentificationTypes (Code, Name, CountryCode, CreatedByUserName) VALUES (N'PASAPORTE', N'Pasaporte', NULL, N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerPaymentTerms WHERE Code = N'CONTADO' AND IsDeleted = 0)
    INSERT INTO dbo.BusinessPartnerPaymentTerms (Code, Name, Days, IsCredit, CreatedByUserName) VALUES (N'CONTADO', N'Contado', 0, 0, N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.BusinessPartnerPaymentTerms WHERE Code = N'CREDITO30' AND IsDeleted = 0)
    INSERT INTO dbo.BusinessPartnerPaymentTerms (Code, Name, Days, IsCredit, CreatedByUserName) VALUES (N'CREDITO30', N'Credito 30 dias', 30, 1, N'Sistema');
GO

IF OBJECT_ID(N'dbo.BusinessPartnerFiscalData', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerFiscalData', N'TaxpayerTypeId') IS NULL
        ALTER TABLE dbo.BusinessPartnerFiscalData ADD TaxpayerTypeId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerFiscalData', N'TaxRegimeId') IS NULL
        ALTER TABLE dbo.BusinessPartnerFiscalData ADD TaxRegimeId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerFiscalData', N'FiscalCountryId') IS NULL
        ALTER TABLE dbo.BusinessPartnerFiscalData ADD FiscalCountryId int NULL;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAddresses', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerAddresses', N'CountryId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAddresses ADD CountryId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAddresses', N'ProvinceId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAddresses ADD ProvinceId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAddresses', N'CityId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAddresses ADD CityId int NULL;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerContacts', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'ContactTypeId') IS NULL
        ALTER TABLE dbo.BusinessPartnerContacts ADD ContactTypeId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'ContactChannelId') IS NULL
        ALTER TABLE dbo.BusinessPartnerContacts ADD ContactChannelId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'Department') IS NULL
        ALTER TABLE dbo.BusinessPartnerContacts ADD Department nvarchar(120) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'Extension') IS NULL
        ALTER TABLE dbo.BusinessPartnerContacts ADD Extension nvarchar(20) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'Language') IS NULL
        ALTER TABLE dbo.BusinessPartnerContacts ADD [Language] nvarchar(50) NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'ReceivesNotifications') IS NULL
        ALTER TABLE dbo.BusinessPartnerContacts ADD ReceivesNotifications bit NOT NULL CONSTRAINT DF_BusinessPartnerContacts_ReceivesNotifications DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'Notes') IS NULL
        ALTER TABLE dbo.BusinessPartnerContacts ADD Notes nvarchar(500) NULL;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerCreditSettings', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerCreditSettings', N'DeliveryDays') IS NULL
        ALTER TABLE dbo.BusinessPartnerCreditSettings ADD DeliveryDays int NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_DeliveryDays DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerCreditSettings', N'MinimumOrderAmount') IS NULL
        ALTER TABLE dbo.BusinessPartnerCreditSettings ADD MinimumOrderAmount decimal(19,6) NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_MinimumOrderAmount DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerCreditSettings', N'AllowsBackorder') IS NULL
        ALTER TABLE dbo.BusinessPartnerCreditSettings ADD AllowsBackorder bit NOT NULL CONSTRAINT DF_BusinessPartnerCreditSettings_AllowsBackorder DEFAULT 0;
    IF COL_LENGTH(N'dbo.BusinessPartnerCreditSettings', N'PreferredCurrencyCode') IS NULL
        ALTER TABLE dbo.BusinessPartnerCreditSettings ADD PreferredCurrencyCode nvarchar(3) NULL;
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
        bp.IdentificationNumber, bp.Email, bp.Phone, bp.Website, bp.Remarks, bp.IsActive,
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
        bp.IdentificationNumber, bp.Email, bp.Phone, bp.Website, bp.Remarks, bp.IsActive,
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
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.BusinessPartners
    WHERE Code = @Code AND IsDeleted = 0 AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORIDENTIFICACION
    @IdentificationTypeId int,
    @IdentificationNumber nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.BusinessPartners
    WHERE IdentificationTypeId = @IdentificationTypeId
      AND IdentificationNumber = @IdentificationNumber
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNERS_LOOKUPS
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Code, Name, CountryCode FROM dbo.BusinessPartnerIdentificationTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, Days, IsCredit FROM dbo.BusinessPartnerPaymentTerms WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Days, Name;
    SELECT Id, Code, Name, IsActive FROM dbo.ChartOfAccounts WHERE IsDeleted = 0 AND IsActive = 1 AND AllowsMovement = 1 ORDER BY Code;
    SELECT N'Customer' AS Code, N'Cliente' AS Name UNION ALL SELECT N'Supplier', N'Proveedor' UNION ALL SELECT N'Both', N'Cliente y proveedor';
    SELECT N'Active' AS Code, N'Activo' AS Name UNION ALL SELECT N'Inactive', N'Inactivo';
    SELECT N'Pending' AS Code, N'Pendiente' AS Name UNION ALL SELECT N'Synced', N'Sincronizado' UNION ALL SELECT N'Error', N'Error';
    SELECT SupplierGroupId AS Id, Code, Name, IsActive FROM dbo.SupplierGroups WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT SupplierClassId AS Id, Code, Name, IsActive FROM dbo.SupplierClasses WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT EconomicActivityId AS Id, Code, Name, IsActive FROM dbo.EconomicActivities WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ZoneId AS Id, Code, Name, IsActive FROM dbo.Zones WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT SupplyMethodId AS Id, Code, Name, IsActive FROM dbo.SupplyMethods WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ContactTypeId AS Id, Code, Name, IsActive FROM dbo.ContactTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ContactChannelId AS Id, Code, Name, IsActive FROM dbo.ContactChannels WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR
    @Code nvarchar(50),
    @Name nvarchar(200),
    @CommercialName nvarchar(200) = NULL,
    @PartnerType nvarchar(20),
    @IdentificationTypeId int,
    @IdentificationNumber nvarchar(50),
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
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    INSERT INTO dbo.BusinessPartners (Code, Name, CommercialName, PartnerType, IdentificationTypeId, IdentificationNumber, Email, Phone, Website, Remarks, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt)
    VALUES (@Code, @Name, @CommercialName, @PartnerType, @IdentificationTypeId, @IdentificationNumber, @Email, @Phone, @Website, @Remarks, @IsActive, @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME());

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

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_BUSINESSPARTNERS_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.BusinessPartners
    SET IsDeleted = 1, IsActive = 0, DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME(), UpdatedByUserId = @DeletedByUserId, UpdatedByUserName = @DeletedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;
    IF @AffectedRows > 0
        INSERT INTO dbo.AuditCatalogChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES (N'BusinessPartners', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);

    SELECT @AffectedRows;
END;
GO
