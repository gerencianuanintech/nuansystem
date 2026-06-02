/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    Integra dimensiones contables al maestro de proveedores sin cambiar el contrato modular.
    SQL Server es el motor principal; otros proveedores deben tener script equivalente.
*/

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'BranchId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD BranchId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'DepartmentId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD DepartmentId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'BusinessLineId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD BusinessLineId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'CostCenterId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CostCenterId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'ProjectId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD ProjectId int NULL;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Branches', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerAccountingSettings_Branches')
    ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CONSTRAINT FK_BusinessPartnerAccountingSettings_Branches FOREIGN KEY (BranchId) REFERENCES dbo.Branches(BranchId);
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Departments', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerAccountingSettings_Departments')
    ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CONSTRAINT FK_BusinessPartnerAccountingSettings_Departments FOREIGN KEY (DepartmentId) REFERENCES dbo.Departments(DepartmentId);
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.BusinessLines', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerAccountingSettings_BusinessLines')
    ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CONSTRAINT FK_BusinessPartnerAccountingSettings_BusinessLines FOREIGN KEY (BusinessLineId) REFERENCES dbo.BusinessLines(BusinessLineId);
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.CostCenters', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerAccountingSettings_CostCenters')
    ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CONSTRAINT FK_BusinessPartnerAccountingSettings_CostCenters FOREIGN KEY (CostCenterId) REFERENCES dbo.CostCenters(CostCenterId);
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Projects', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerAccountingSettings_Projects')
    ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CONSTRAINT FK_BusinessPartnerAccountingSettings_Projects FOREIGN KEY (ProjectId) REFERENCES dbo.Projects(ProjectId);
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
    SELECT CountryId AS Id, Code, Name, IsActive FROM dbo.Countries WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ProvinceId AS Id, Code, Name, IsActive, CountryId, CAST(NULL AS int) AS ProvinceId, CAST(NULL AS nvarchar(30)) AS PostalCode
    FROM dbo.Provinces
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
    SELECT CityId AS Id, Code, Name, IsActive, CountryId, ProvinceId, CAST(NULL AS nvarchar(30)) AS PostalCode
    FROM dbo.Cities
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
    SELECT BankId AS Id, Code, Name, IsActive FROM dbo.Banks WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT BankAccountTypeId AS Id, Code, Name, IsActive FROM dbo.BankAccountTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT CurrencyId AS Id, Code, Name, IsActive FROM dbo.Currencies WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Code;
    SELECT PriceListId AS Id, Code, Name, IsActive FROM dbo.PriceLists WHERE IsDeleted = 0 AND IsActive = 1 AND AppliesTo IN (N'Purchasing', N'Both') ORDER BY IsDefault DESC, Name;
    SELECT PurchasingAgentId AS Id, Code, Name, IsActive FROM dbo.PurchasingAgents WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT TaxRegimeId AS Id, Code, Name, IsActive FROM dbo.TaxRegimes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT TaxpayerTypeId AS Id, Code, Name, IsActive FROM dbo.TaxpayerTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT RetentionTypeId AS Id, Code, Name, IsActive FROM dbo.RetentionTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT RetentionConceptId AS Id, Code, Name, IsActive, SriCode, [Percent], AppliesIva, AppliesIncome, RetentionTypeId FROM dbo.RetentionConcepts WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY SriCode, Name;
    SELECT TaxSupportId AS Id, Code, Name, IsActive FROM dbo.TaxSupports WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT AccountingPaymentMethodId AS Id, Code, Name, IsActive FROM dbo.AccountingPaymentMethods WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT PaymentPriorityId AS Id, Code, Name, IsActive FROM dbo.PaymentPriorities WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ApprovalFlowId AS Id, Code, Name, IsActive FROM dbo.ApprovalFlows WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT PaymentDocumentTypeId AS Id, Code, Name, IsActive FROM dbo.PaymentDocumentTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT BranchId AS Id, Code, Name, IsActive FROM dbo.Branches WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT DepartmentId AS Id, Code, Name, IsActive FROM dbo.Departments WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT BusinessLineId AS Id, Code, Name, IsActive FROM dbo.BusinessLines WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT CostCenterId AS Id, Code, Name, IsActive FROM dbo.CostCenters WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ProjectId AS Id, Code, Name, IsActive FROM dbo.Projects WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
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
        credit.AllowsBackorder, credit.PreferredCurrencyCode, credit.PriceListCode, credit.AssignedSellerCode, credit.AssignedBuyerCode, credit.CreditStatus,
        accounting.CustomerAccountId, customerAccount.Code AS CustomerAccountCode, customerAccount.Name AS CustomerAccountName,
        accounting.SupplierAccountId, supplierAccount.Code AS SupplierAccountCode, supplierAccount.Name AS SupplierAccountName,
        accounting.CustomerAdvanceAccountId, accounting.SupplierAdvanceAccountId, accounting.RetentionAccountId,
        accounting.BranchId, branch.Name AS BranchName,
        accounting.DepartmentId, department.Name AS DepartmentName,
        accounting.BusinessLineId, businessLine.Name AS BusinessLineName,
        accounting.CostCenterId, costCenter.Name AS CostCenterName,
        accounting.ProjectId, project.Name AS ProjectName,
        accounting.CostCenterCode,
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
    LEFT JOIN dbo.BusinessPartnerPaymentTerms terms ON terms.Id = credit.PaymentTermId
    LEFT JOIN dbo.BusinessPartnerAccountingSettings accounting ON accounting.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.ChartOfAccounts customerAccount ON customerAccount.Id = accounting.CustomerAccountId
    LEFT JOIN dbo.ChartOfAccounts supplierAccount ON supplierAccount.Id = accounting.SupplierAccountId
    LEFT JOIN dbo.Branches branch ON branch.BranchId = accounting.BranchId
    LEFT JOIN dbo.Departments department ON department.DepartmentId = accounting.DepartmentId
    LEFT JOIN dbo.BusinessLines businessLine ON businessLine.BusinessLineId = accounting.BusinessLineId
    LEFT JOIN dbo.CostCenters costCenter ON costCenter.CostCenterId = accounting.CostCenterId
    LEFT JOIN dbo.Projects project ON project.ProjectId = accounting.ProjectId
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
        credit.AllowsBackorder, credit.PreferredCurrencyCode, credit.PriceListCode, credit.AssignedSellerCode, credit.AssignedBuyerCode, credit.CreditStatus,
        accounting.CustomerAccountId, customerAccount.Code AS CustomerAccountCode, customerAccount.Name AS CustomerAccountName,
        accounting.SupplierAccountId, supplierAccount.Code AS SupplierAccountCode, supplierAccount.Name AS SupplierAccountName,
        accounting.CustomerAdvanceAccountId, accounting.SupplierAdvanceAccountId, accounting.RetentionAccountId,
        accounting.BranchId, branch.Name AS BranchName,
        accounting.DepartmentId, department.Name AS DepartmentName,
        accounting.BusinessLineId, businessLine.Name AS BusinessLineName,
        accounting.CostCenterId, costCenter.Name AS CostCenterName,
        accounting.ProjectId, project.Name AS ProjectName,
        accounting.CostCenterCode,
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
    LEFT JOIN dbo.BusinessPartnerPaymentTerms terms ON terms.Id = credit.PaymentTermId
    LEFT JOIN dbo.BusinessPartnerAccountingSettings accounting ON accounting.BusinessPartnerId = bp.Id
    LEFT JOIN dbo.ChartOfAccounts customerAccount ON customerAccount.Id = accounting.CustomerAccountId
    LEFT JOIN dbo.ChartOfAccounts supplierAccount ON supplierAccount.Id = accounting.SupplierAccountId
    LEFT JOIN dbo.Branches branch ON branch.BranchId = accounting.BranchId
    LEFT JOIN dbo.Departments department ON department.DepartmentId = accounting.DepartmentId
    LEFT JOIN dbo.BusinessLines businessLine ON businessLine.BusinessLineId = accounting.BusinessLineId
    LEFT JOIN dbo.CostCenters costCenter ON costCenter.CostCenterId = accounting.CostCenterId
    LEFT JOIN dbo.Projects project ON project.ProjectId = accounting.ProjectId
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

IF OBJECT_ID(N'dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR', N'P') IS NOT NULL
   AND OBJECT_ID(N'dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR_LEGACY_ACCOUNTING_DIMENSIONS', N'P') IS NULL
BEGIN
    DECLARE @CreateDefinition nvarchar(max) = OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR'));
    SET @CreateDefinition = REPLACE(@CreateDefinition, N'SP_NA_POST_BUSINESSPARTNERS_CREAR', N'SP_NA_POST_BUSINESSPARTNERS_CREAR_LEGACY_ACCOUNTING_DIMENSIONS');
    IF @CreateDefinition IS NOT NULL
        EXEC sys.sp_executesql @CreateDefinition;
END;
GO

IF OBJECT_ID(N'dbo.SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR', N'P') IS NOT NULL
   AND OBJECT_ID(N'dbo.SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR_LEGACY_ACCOUNTING_DIMENSIONS', N'P') IS NULL
BEGIN
    DECLARE @UpdateDefinition nvarchar(max) = OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR'));
    SET @UpdateDefinition = REPLACE(@UpdateDefinition, N'SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR', N'SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR_LEGACY_ACCOUNTING_DIMENSIONS');
    IF @UpdateDefinition IS NOT NULL
        EXEC sys.sp_executesql @UpdateDefinition;
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
        @Code = @Code, @Name = @Name, @CommercialName = @CommercialName, @PartnerType = @PartnerType,
        @IdentificationTypeId = @IdentificationTypeId, @IdentificationNumber = @IdentificationNumber,
        @SupplierGroupId = @SupplierGroupId, @SupplierClassId = @SupplierClassId,
        @EconomicActivityId = @EconomicActivityId, @ZoneId = @ZoneId, @SupplyMethodId = @SupplyMethodId,
        @Email = @Email, @Phone = @Phone, @Website = @Website, @Remarks = @Remarks, @IsActive = @IsActive,
        @TaxpayerTypeId = @TaxpayerTypeId, @TaxRegimeId = @TaxRegimeId, @FiscalCountryId = @FiscalCountryId,
        @TaxpayerType = @TaxpayerType, @IsAccountingRequired = @IsAccountingRequired,
        @AppliesRetention = @AppliesRetention, @FiscalRegime = @FiscalRegime,
        @CountryCode = @CountryCode, @Province = @Province, @City = @City,
        @CustomerAccountId = @CustomerAccountId, @SupplierAccountId = @SupplierAccountId,
        @CustomerAdvanceAccountId = @CustomerAdvanceAccountId, @SupplierAdvanceAccountId = @SupplierAdvanceAccountId,
        @RetentionAccountId = @RetentionAccountId, @CostCenterCode = @CostCenterCode,
        @DefaultExpenseAccountId = @DefaultExpenseAccountId, @DifferenceAccountId = @DifferenceAccountId,
        @RoundingAccountId = @RoundingAccountId, @ClearingAccountId = @ClearingAccountId,
        @DiscountAccountId = @DiscountAccountId, @AccountingBySupplier = @AccountingBySupplier,
        @RequiresProvision = @RequiresProvision, @AllowsAdvance = @AllowsAdvance,
        @AllowsCompensation = @AllowsCompensation, @AllowsPartialPayments = @AllowsPartialPayments,
        @IsPaymentBlocked = @IsPaymentBlocked, @UsesWithholdingBase = @UsesWithholdingBase,
        @ConciliationRequired = @ConciliationRequired, @AccountingPaymentMethodId = @AccountingPaymentMethodId,
        @PaymentPriorityId = @PaymentPriorityId, @ApprovalFlowId = @ApprovalFlowId,
        @PaymentDocumentTypeId = @PaymentDocumentTypeId, @AccountingPaymentMethod = @AccountingPaymentMethod,
        @PaymentPriority = @PaymentPriority, @RequiredPaymentDay = @RequiredPaymentDay,
        @ApprovalFlow = @ApprovalFlow, @PaymentDocumentType = @PaymentDocumentType,
        @AveragePaymentDays = @AveragePaymentDays, @PaymentTolerancePercent = @PaymentTolerancePercent,
        @PaymentTermId = @PaymentTermId, @CreditDays = @CreditDays, @CreditLimit = @CreditLimit,
        @DeliveryDays = @DeliveryDays, @MinimumOrderAmount = @MinimumOrderAmount,
        @AllowsBackorder = @AllowsBackorder, @PreferredCurrencyCode = @PreferredCurrencyCode,
        @PriceListCode = @PriceListCode, @AssignedSellerCode = @AssignedSellerCode,
        @AssignedBuyerCode = @AssignedBuyerCode, @CreditStatus = @CreditStatus,
        @SapCardCode = @SapCardCode, @SapCardType = @SapCardType, @SapSyncStatus = @SapSyncStatus,
        @SapLastSyncAt = @SapLastSyncAt, @SapLastError = @SapLastError, @SapEnabled = @SapEnabled,
        @SapMode = @SapMode, @SapCompanyCode = @SapCompanyCode, @SapRetryCount = @SapRetryCount,
        @SyncAsSupplier = @SyncAsSupplier, @AllowManualSapRetry = @AllowManualSapRetry,
        @RequiresApprovalBeforeSapSync = @RequiresApprovalBeforeSapSync,
        @AddressesJson = @AddressesJson, @ContactsJson = @ContactsJson, @BankAccountsJson = @BankAccountsJson,
        @RetentionSettingsJson = @RetentionSettingsJson, @NotesJson = @NotesJson,
        @SapFieldMappingsJson = @SapFieldMappingsJson, @AttachmentsJson = @AttachmentsJson,
        @CreatedByUserId = @CreatedByUserId, @CreatedByUserName = @CreatedByUserName;

    DECLARE @Id int = (SELECT TOP (1) Id FROM @Created);

    UPDATE dbo.BusinessPartnerAccountingSettings
    SET BranchId = @BranchId,
        DepartmentId = @DepartmentId,
        BusinessLineId = @BusinessLineId,
        CostCenterId = @CostCenterId,
        ProjectId = @ProjectId
    WHERE BusinessPartnerId = @Id;

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
        @Id = @Id, @Code = @Code, @Name = @Name, @CommercialName = @CommercialName, @PartnerType = @PartnerType,
        @IdentificationTypeId = @IdentificationTypeId, @IdentificationNumber = @IdentificationNumber,
        @SupplierGroupId = @SupplierGroupId, @SupplierClassId = @SupplierClassId,
        @EconomicActivityId = @EconomicActivityId, @ZoneId = @ZoneId, @SupplyMethodId = @SupplyMethodId,
        @Email = @Email, @Phone = @Phone, @Website = @Website, @Remarks = @Remarks, @IsActive = @IsActive,
        @TaxpayerTypeId = @TaxpayerTypeId, @TaxRegimeId = @TaxRegimeId, @FiscalCountryId = @FiscalCountryId,
        @TaxpayerType = @TaxpayerType, @IsAccountingRequired = @IsAccountingRequired,
        @AppliesRetention = @AppliesRetention, @FiscalRegime = @FiscalRegime,
        @CountryCode = @CountryCode, @Province = @Province, @City = @City,
        @CustomerAccountId = @CustomerAccountId, @SupplierAccountId = @SupplierAccountId,
        @CustomerAdvanceAccountId = @CustomerAdvanceAccountId, @SupplierAdvanceAccountId = @SupplierAdvanceAccountId,
        @RetentionAccountId = @RetentionAccountId, @CostCenterCode = @CostCenterCode,
        @DefaultExpenseAccountId = @DefaultExpenseAccountId, @DifferenceAccountId = @DifferenceAccountId,
        @RoundingAccountId = @RoundingAccountId, @ClearingAccountId = @ClearingAccountId,
        @DiscountAccountId = @DiscountAccountId, @AccountingBySupplier = @AccountingBySupplier,
        @RequiresProvision = @RequiresProvision, @AllowsAdvance = @AllowsAdvance,
        @AllowsCompensation = @AllowsCompensation, @AllowsPartialPayments = @AllowsPartialPayments,
        @IsPaymentBlocked = @IsPaymentBlocked, @UsesWithholdingBase = @UsesWithholdingBase,
        @ConciliationRequired = @ConciliationRequired, @AccountingPaymentMethodId = @AccountingPaymentMethodId,
        @PaymentPriorityId = @PaymentPriorityId, @ApprovalFlowId = @ApprovalFlowId,
        @PaymentDocumentTypeId = @PaymentDocumentTypeId, @AccountingPaymentMethod = @AccountingPaymentMethod,
        @PaymentPriority = @PaymentPriority, @RequiredPaymentDay = @RequiredPaymentDay,
        @ApprovalFlow = @ApprovalFlow, @PaymentDocumentType = @PaymentDocumentType,
        @AveragePaymentDays = @AveragePaymentDays, @PaymentTolerancePercent = @PaymentTolerancePercent,
        @PaymentTermId = @PaymentTermId, @CreditDays = @CreditDays, @CreditLimit = @CreditLimit,
        @DeliveryDays = @DeliveryDays, @MinimumOrderAmount = @MinimumOrderAmount,
        @AllowsBackorder = @AllowsBackorder, @PreferredCurrencyCode = @PreferredCurrencyCode,
        @PriceListCode = @PriceListCode, @AssignedSellerCode = @AssignedSellerCode,
        @AssignedBuyerCode = @AssignedBuyerCode, @CreditStatus = @CreditStatus,
        @SapCardCode = @SapCardCode, @SapCardType = @SapCardType, @SapSyncStatus = @SapSyncStatus,
        @SapLastSyncAt = @SapLastSyncAt, @SapLastError = @SapLastError, @SapEnabled = @SapEnabled,
        @SapMode = @SapMode, @SapCompanyCode = @SapCompanyCode, @SapRetryCount = @SapRetryCount,
        @SyncAsSupplier = @SyncAsSupplier, @AllowManualSapRetry = @AllowManualSapRetry,
        @RequiresApprovalBeforeSapSync = @RequiresApprovalBeforeSapSync,
        @AddressesJson = @AddressesJson, @ContactsJson = @ContactsJson, @BankAccountsJson = @BankAccountsJson,
        @RetentionSettingsJson = @RetentionSettingsJson, @NotesJson = @NotesJson,
        @SapFieldMappingsJson = @SapFieldMappingsJson, @AttachmentsJson = @AttachmentsJson,
        @UpdatedByUserId = @UpdatedByUserId, @UpdatedByUserName = @UpdatedByUserName;

    DECLARE @AffectedRows int = (SELECT TOP (1) AffectedRows FROM @Updated);

    IF @AffectedRows > 0
    BEGIN
        UPDATE dbo.BusinessPartnerAccountingSettings
        SET BranchId = @BranchId,
            DepartmentId = @DepartmentId,
            BusinessLineId = @BusinessLineId,
            CostCenterId = @CostCenterId,
            ProjectId = @ProjectId
        WHERE BusinessPartnerId = @Id;
    END;

    SELECT @AffectedRows;
END;
GO
