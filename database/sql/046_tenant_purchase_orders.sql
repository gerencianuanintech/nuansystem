SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.PurchaseTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseTypes
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseTypes PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(500) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_PurchaseTypes_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_PurchaseTypes_IsDeleted DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(256) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseTypes_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(256) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(256) NULL,
        DeletedAt datetime2(0) NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PurchaseTypes_Code_Active' AND object_id = OBJECT_ID(N'dbo.PurchaseTypes'))
    CREATE UNIQUE INDEX UX_PurchaseTypes_Code_Active ON dbo.PurchaseTypes (Code) WHERE IsDeleted = 0;

IF NOT EXISTS (SELECT 1 FROM dbo.PurchaseTypes WHERE Code = N'LOCAL' AND IsDeleted = 0)
    INSERT INTO dbo.PurchaseTypes (Code, Name, Description, CreatedByUserName) VALUES (N'LOCAL', N'Compra local', N'Orden de compra local.', N'Sistema');

IF OBJECT_ID(N'dbo.DocumentSeries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DocumentSeries
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_DocumentSeries PRIMARY KEY,
        DocumentType nvarchar(50) NOT NULL,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(200) NOT NULL,
        Prefix nvarchar(20) NULL,
        CurrentNumber int NOT NULL CONSTRAINT DF_DocumentSeries_CurrentNumber DEFAULT 0,
        IsDefault bit NOT NULL CONSTRAINT DF_DocumentSeries_IsDefault DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_DocumentSeries_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_DocumentSeries_IsDeleted DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(256) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_DocumentSeries_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(256) NULL,
        UpdatedAt datetime2(0) NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_DocumentSeries_Type_Code_Active' AND object_id = OBJECT_ID(N'dbo.DocumentSeries'))
    CREATE UNIQUE INDEX UX_DocumentSeries_Type_Code_Active ON dbo.DocumentSeries (DocumentType, Code) WHERE IsDeleted = 0;

IF NOT EXISTS (SELECT 1 FROM dbo.DocumentSeries WHERE DocumentType = N'PurchaseOrder' AND Code = N'OC-2026' AND IsDeleted = 0)
    INSERT INTO dbo.DocumentSeries (DocumentType, Code, Name, Prefix, IsDefault, CreatedByUserName) VALUES (N'PurchaseOrder', N'OC-2026', N'Ordenes de compra 2026', N'OC', 1, N'Sistema');

IF OBJECT_ID(N'dbo.PurchaseOrderHeaders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseOrderHeaders
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseOrderHeaders PRIMARY KEY,
        BranchId int NULL,
        DocumentSeriesId int NULL,
        SeriesCode nvarchar(50) NOT NULL,
        DocumentNumber nvarchar(50) NOT NULL,
        SupplierId int NOT NULL,
        SupplierCode nvarchar(50) NOT NULL,
        SupplierName nvarchar(200) NOT NULL,
        SupplierTaxId nvarchar(50) NULL,
        ContactName nvarchar(200) NULL,
        Phone nvarchar(80) NULL,
        Email nvarchar(200) NULL,
        DocumentDate date NOT NULL,
        DeliveryDate date NOT NULL,
        CurrencyCode nvarchar(10) NOT NULL,
        ExchangeRate decimal(19,6) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_ExchangeRate DEFAULT 1,
        PaymentTermId int NULL,
        PriceListId int NULL,
        BuyerId int NULL,
        MainWarehouseId int NULL,
        ProjectId int NULL,
        CostCenterId int NULL,
        PurchaseTypeId int NULL,
        Comments nvarchar(2000) NULL,
        Status nvarchar(40) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_Status DEFAULT N'Draft',
        Subtotal decimal(19,6) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_Subtotal DEFAULT 0,
        DiscountPercent decimal(9,6) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_DiscountPercent DEFAULT 0,
        DiscountAmount decimal(19,6) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_DiscountAmount DEFAULT 0,
        TaxAmount decimal(19,6) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_TaxAmount DEFAULT 0,
        TotalAmount decimal(19,6) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_TotalAmount DEFAULT 0,
        TotalItems int NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_TotalItems DEFAULT 0,
        TotalQuantity decimal(19,6) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_TotalQuantity DEFAULT 0,
        TotalWeight decimal(19,6) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_TotalWeight DEFAULT 0,
        SapObjectType nvarchar(20) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_SapObjectType DEFAULT N'22',
        SapStatus nvarchar(40) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_SapStatus DEFAULT N'Pending',
        SapDocEntry int NULL,
        SapDocNum int NULL,
        SapSyncDate datetime2(0) NULL,
        SapMessage nvarchar(max) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_IsDeleted DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(256) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(256) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(256) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT CK_PurchaseOrderHeaders_SapObjectType CHECK (SapObjectType = N'22')
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PurchaseOrderHeaders_Series_Number_Active' AND object_id = OBJECT_ID(N'dbo.PurchaseOrderHeaders'))
    CREATE UNIQUE INDEX UX_PurchaseOrderHeaders_Series_Number_Active ON dbo.PurchaseOrderHeaders (SeriesCode, DocumentNumber) WHERE IsDeleted = 0;

IF OBJECT_ID(N'dbo.PurchaseOrderLines', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseOrderLines
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseOrderLines PRIMARY KEY,
        PurchaseOrderId int NOT NULL,
        LineNumber int NOT NULL,
        ItemId int NOT NULL,
        ItemCode nvarchar(50) NOT NULL,
        ItemName nvarchar(200) NOT NULL,
        UnitId int NULL,
        UnitCode nvarchar(50) NULL,
        Quantity decimal(19,6) NOT NULL,
        OpenQuantity decimal(19,6) NOT NULL,
        UnitPrice decimal(19,6) NOT NULL,
        DiscountPercent decimal(9,6) NOT NULL,
        DiscountAmount decimal(19,6) NOT NULL,
        TaxId int NULL,
        TaxCode nvarchar(50) NOT NULL,
        TaxRate decimal(9,6) NOT NULL,
        TaxAmount decimal(19,6) NOT NULL,
        WarehouseId int NOT NULL,
        WarehouseCode nvarchar(50) NOT NULL,
        DeliveryDate date NOT NULL,
        CostCenterId int NULL,
        ProjectId int NULL,
        LineSubtotal decimal(19,6) NOT NULL,
        LineTotal decimal(19,6) NOT NULL,
        Status nvarchar(40) NOT NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseOrderLines_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_PurchaseOrderLines_Header FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrderHeaders(Id)
    );
END;

IF OBJECT_ID(N'dbo.PurchaseOrderAddresses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseOrderAddresses
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseOrderAddresses PRIMARY KEY,
        PurchaseOrderId int NOT NULL,
        AddressType nvarchar(20) NOT NULL,
        SourceAddressId int NULL,
        AddressName nvarchar(200) NULL,
        Street nvarchar(500) NULL,
        Reference nvarchar(500) NULL,
        City nvarchar(120) NULL,
        State nvarchar(120) NULL,
        ZipCode nvarchar(40) NULL,
        Country nvarchar(120) NULL,
        Phone nvarchar(80) NULL,
        Email nvarchar(200) NULL,
        IsModified bit NOT NULL CONSTRAINT DF_PurchaseOrderAddresses_IsModified DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseOrderAddresses_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_PurchaseOrderAddresses_Header FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrderHeaders(Id),
        CONSTRAINT CK_PurchaseOrderAddresses_Type CHECK (AddressType IN (N'Delivery', N'Billing'))
    );
END;

IF OBJECT_ID(N'dbo.PurchaseOrderApprovals', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseOrderApprovals
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseOrderApprovals PRIMARY KEY,
        PurchaseOrderId int NOT NULL,
        ApprovalLevel int NOT NULL,
        RoleName nvarchar(200) NULL,
        UserName nvarchar(256) NULL,
        RequestedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseOrderApprovals_RequestedAt DEFAULT SYSUTCDATETIME(),
        RespondedAt datetime2(0) NULL,
        Status nvarchar(40) NOT NULL,
        Observation nvarchar(1000) NULL,
        CONSTRAINT FK_PurchaseOrderApprovals_Header FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrderHeaders(Id)
    );
END;

IF OBJECT_ID(N'dbo.PurchaseOrderRelatedDocuments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseOrderRelatedDocuments
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseOrderRelatedDocuments PRIMARY KEY,
        PurchaseOrderId int NOT NULL,
        RelatedDocumentType nvarchar(80) NOT NULL,
        RelatedDocumentId int NULL,
        Series nvarchar(50) NULL,
        Number nvarchar(50) NULL,
        Date date NULL,
        Status nvarchar(40) NULL,
        Reference nvarchar(200) NULL,
        Comment nvarchar(1000) NULL,
        Total decimal(19,6) NOT NULL CONSTRAINT DF_PurchaseOrderRelatedDocuments_Total DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(256) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseOrderRelatedDocuments_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_PurchaseOrderRelatedDocuments_Header FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrderHeaders(Id)
    );
END;

IF OBJECT_ID(N'dbo.PurchaseOrderAttachments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseOrderAttachments
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseOrderAttachments PRIMARY KEY,
        PurchaseOrderId int NOT NULL,
        FileName nvarchar(260) NOT NULL,
        OriginalFileName nvarchar(260) NOT NULL,
        FileExtension nvarchar(20) NULL,
        MimeType nvarchar(120) NULL,
        FileSize bigint NOT NULL CONSTRAINT DF_PurchaseOrderAttachments_FileSize DEFAULT 0,
        StoragePath nvarchar(1000) NULL,
        Status nvarchar(40) NOT NULL CONSTRAINT DF_PurchaseOrderAttachments_Status DEFAULT N'Active',
        Comment nvarchar(1000) NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(256) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseOrderAttachments_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_PurchaseOrderAttachments_Header FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrderHeaders(Id)
    );
END;

IF OBJECT_ID(N'dbo.PurchaseOrderSapSyncLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseOrderSapSyncLogs
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseOrderSapSyncLogs PRIMARY KEY,
        PurchaseOrderId int NOT NULL,
        Process nvarchar(80) NOT NULL,
        Status nvarchar(40) NOT NULL,
        Message nvarchar(max) NULL,
        TechnicalRequest nvarchar(max) NULL,
        TechnicalResponse nvarchar(max) NULL,
        UserId int NULL,
        UserName nvarchar(256) NULL,
        AttemptNumber int NOT NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseOrderSapSyncLogs_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_PurchaseOrderSapSyncLogs_Header FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrderHeaders(Id)
    );
END;

GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PURCHASEORDERS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        SeriesCode,
        DocumentNumber,
        SupplierCode,
        SupplierName,
        DocumentDate,
        DeliveryDate,
        CurrencyCode,
        Status,
        TotalAmount,
        SapStatus
    FROM dbo.PurchaseOrderHeaders
    WHERE IsDeleted = 0
    ORDER BY DocumentDate DESC, Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PURCHASEORDERS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM dbo.PurchaseOrderHeaders WHERE Id = @Id AND IsDeleted = 0;
    SELECT * FROM dbo.PurchaseOrderLines WHERE PurchaseOrderId = @Id ORDER BY LineNumber;
    SELECT * FROM dbo.PurchaseOrderAddresses WHERE PurchaseOrderId = @Id ORDER BY AddressType;
    SELECT * FROM dbo.PurchaseOrderApprovals WHERE PurchaseOrderId = @Id ORDER BY ApprovalLevel, Id;
    SELECT * FROM dbo.PurchaseOrderRelatedDocuments WHERE PurchaseOrderId = @Id ORDER BY Id DESC;
    SELECT * FROM dbo.PurchaseOrderAttachments WHERE PurchaseOrderId = @Id ORDER BY CreatedAt DESC, Id DESC;
    SELECT Id, CreatedAt, Process, Status, Message, UserName, AttemptNumber FROM dbo.PurchaseOrderSapSyncLogs WHERE PurchaseOrderId = @Id ORDER BY CreatedAt DESC, Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PURCHASEORDERS_LOOKUPS
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Code, Name, IsActive FROM dbo.BusinessPartners WHERE IsDeleted = 0 AND IsActive = 1 AND PartnerType IN (N'Supplier', N'Both') ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.Items WHERE IsDeleted = 0 AND IsActive = 1 AND IsPurchaseItem = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.UnitMeasures WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.Warehouses WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, Rate, IsActive FROM dbo.Taxes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.Currencies WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.PaymentTerms WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.PriceLists WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.PurchasingAgents WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.CostCenters WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.Projects WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.PurchaseTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, IsActive FROM dbo.DocumentSeries WHERE IsDeleted = 0 AND IsActive = 1 AND DocumentType = N'PurchaseOrder' ORDER BY IsDefault DESC, Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PURCHASEORDERS_CREAR
    @BranchId int = NULL,
    @DocumentSeriesId int = NULL,
    @SeriesCode nvarchar(50),
    @DocumentNumber nvarchar(50),
    @SupplierId int,
    @SupplierCode nvarchar(50),
    @SupplierName nvarchar(200),
    @SupplierTaxId nvarchar(50) = NULL,
    @ContactName nvarchar(200) = NULL,
    @Phone nvarchar(80) = NULL,
    @Email nvarchar(200) = NULL,
    @DocumentDate date,
    @DeliveryDate date,
    @CurrencyCode nvarchar(10),
    @ExchangeRate decimal(19,6),
    @PaymentTermId int = NULL,
    @PriceListId int = NULL,
    @BuyerId int = NULL,
    @MainWarehouseId int = NULL,
    @ProjectId int = NULL,
    @CostCenterId int = NULL,
    @PurchaseTypeId int = NULL,
    @Comments nvarchar(2000) = NULL,
    @Status nvarchar(40),
    @Subtotal decimal(19,6),
    @DiscountPercent decimal(9,6),
    @DiscountAmount decimal(19,6),
    @TaxAmount decimal(19,6),
    @TotalAmount decimal(19,6),
    @TotalItems int,
    @TotalQuantity decimal(19,6),
    @TotalWeight decimal(19,6),
    @SapObjectType nvarchar(20),
    @SapStatus nvarchar(40),
    @LinesJson nvarchar(max),
    @AddressesJson nvarchar(max),
    @RelatedDocumentsJson nvarchar(max) = NULL,
    @AttachmentsJson nvarchar(max) = NULL,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.PurchaseOrderHeaders
        (BranchId, DocumentSeriesId, SeriesCode, DocumentNumber, SupplierId, SupplierCode, SupplierName, SupplierTaxId,
         ContactName, Phone, Email, DocumentDate, DeliveryDate, CurrencyCode, ExchangeRate, PaymentTermId, PriceListId,
         BuyerId, MainWarehouseId, ProjectId, CostCenterId, PurchaseTypeId, Comments, Status, Subtotal, DiscountPercent,
         DiscountAmount, TaxAmount, TotalAmount, TotalItems, TotalQuantity, TotalWeight, SapObjectType, SapStatus,
         CreatedByUserId, CreatedByUserName)
    VALUES
        (@BranchId, @DocumentSeriesId, @SeriesCode, @DocumentNumber, @SupplierId, @SupplierCode, @SupplierName, @SupplierTaxId,
         @ContactName, @Phone, @Email, @DocumentDate, @DeliveryDate, @CurrencyCode, @ExchangeRate, @PaymentTermId, @PriceListId,
         @BuyerId, @MainWarehouseId, @ProjectId, @CostCenterId, @PurchaseTypeId, @Comments, @Status, @Subtotal, @DiscountPercent,
         @DiscountAmount, @TaxAmount, @TotalAmount, @TotalItems, @TotalQuantity, @TotalWeight, @SapObjectType, @SapStatus,
         @AuditUserId, @AuditUserName);

    DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());

    EXEC dbo.SP_NA_INTERNAL_PURCHASEORDERS_REPLACE_CHILDREN @Id, @LinesJson, @AddressesJson, @RelatedDocumentsJson, @AttachmentsJson, @AuditUserId, @AuditUserName;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PURCHASEORDERS_ACTUALIZAR
    @Id int,
    @BranchId int = NULL,
    @DocumentSeriesId int = NULL,
    @SeriesCode nvarchar(50),
    @DocumentNumber nvarchar(50),
    @SupplierId int,
    @SupplierCode nvarchar(50),
    @SupplierName nvarchar(200),
    @SupplierTaxId nvarchar(50) = NULL,
    @ContactName nvarchar(200) = NULL,
    @Phone nvarchar(80) = NULL,
    @Email nvarchar(200) = NULL,
    @DocumentDate date,
    @DeliveryDate date,
    @CurrencyCode nvarchar(10),
    @ExchangeRate decimal(19,6),
    @PaymentTermId int = NULL,
    @PriceListId int = NULL,
    @BuyerId int = NULL,
    @MainWarehouseId int = NULL,
    @ProjectId int = NULL,
    @CostCenterId int = NULL,
    @PurchaseTypeId int = NULL,
    @Comments nvarchar(2000) = NULL,
    @Status nvarchar(40),
    @Subtotal decimal(19,6),
    @DiscountPercent decimal(9,6),
    @DiscountAmount decimal(19,6),
    @TaxAmount decimal(19,6),
    @TotalAmount decimal(19,6),
    @TotalItems int,
    @TotalQuantity decimal(19,6),
    @TotalWeight decimal(19,6),
    @SapObjectType nvarchar(20),
    @SapStatus nvarchar(40),
    @LinesJson nvarchar(max),
    @AddressesJson nvarchar(max),
    @RelatedDocumentsJson nvarchar(max) = NULL,
    @AttachmentsJson nvarchar(max) = NULL,
    @ExpectedStatusesJson nvarchar(max) = NULL,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- @ExpectedStatusesJson protege edicion concurrente.
    -- NULL queda solo por compatibilidad legacy; el backend nuevo debe enviar siempre estados esperados.
    UPDATE h
    SET BranchId = @BranchId,
        DocumentSeriesId = @DocumentSeriesId,
        SeriesCode = @SeriesCode,
        DocumentNumber = @DocumentNumber,
        SupplierId = @SupplierId,
        SupplierCode = @SupplierCode,
        SupplierName = @SupplierName,
        SupplierTaxId = @SupplierTaxId,
        ContactName = @ContactName,
        Phone = @Phone,
        Email = @Email,
        DocumentDate = @DocumentDate,
        DeliveryDate = @DeliveryDate,
        CurrencyCode = @CurrencyCode,
        ExchangeRate = @ExchangeRate,
        PaymentTermId = @PaymentTermId,
        PriceListId = @PriceListId,
        BuyerId = @BuyerId,
        MainWarehouseId = @MainWarehouseId,
        ProjectId = @ProjectId,
        CostCenterId = @CostCenterId,
        PurchaseTypeId = @PurchaseTypeId,
        Comments = @Comments,
        Status = @Status,
        Subtotal = @Subtotal,
        DiscountPercent = @DiscountPercent,
        DiscountAmount = @DiscountAmount,
        TaxAmount = @TaxAmount,
        TotalAmount = @TotalAmount,
        TotalItems = @TotalItems,
        TotalQuantity = @TotalQuantity,
        TotalWeight = @TotalWeight,
        SapObjectType = @SapObjectType,
        SapStatus = @SapStatus,
        UpdatedByUserId = @AuditUserId,
        UpdatedByUserName = @AuditUserName,
        UpdatedAt = SYSUTCDATETIME()
    FROM dbo.PurchaseOrderHeaders AS h
    WHERE h.Id = @Id
      AND h.IsDeleted = 0
      AND (
          @ExpectedStatusesJson IS NULL
          OR EXISTS (
              SELECT 1
              FROM OPENJSON(@ExpectedStatusesJson)
              WHERE [value] = h.Status
          )
      );

    DECLARE @Rows int = @@ROWCOUNT;
    IF @Rows > 0
    BEGIN
        EXEC dbo.SP_NA_INTERNAL_PURCHASEORDERS_REPLACE_CHILDREN @Id, @LinesJson, @AddressesJson, @RelatedDocumentsJson, @AttachmentsJson, @AuditUserId, @AuditUserName;
    END

    SELECT @Rows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_INTERNAL_PURCHASEORDERS_REPLACE_CHILDREN
    @Id int,
    @LinesJson nvarchar(max),
    @AddressesJson nvarchar(max),
    @RelatedDocumentsJson nvarchar(max) = NULL,
    @AttachmentsJson nvarchar(max) = NULL,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.PurchaseOrderLines WHERE PurchaseOrderId = @Id;
    DELETE FROM dbo.PurchaseOrderAddresses WHERE PurchaseOrderId = @Id;
    DELETE FROM dbo.PurchaseOrderRelatedDocuments WHERE PurchaseOrderId = @Id;
    DELETE FROM dbo.PurchaseOrderAttachments WHERE PurchaseOrderId = @Id;

    INSERT INTO dbo.PurchaseOrderLines
        (PurchaseOrderId, LineNumber, ItemId, ItemCode, ItemName, UnitId, UnitCode, Quantity, OpenQuantity,
         UnitPrice, DiscountPercent, DiscountAmount, TaxId, TaxCode, TaxRate, TaxAmount, WarehouseId, WarehouseCode,
         DeliveryDate, CostCenterId, ProjectId, LineSubtotal, LineTotal, Status)
    SELECT
        @Id, LineNumber, ItemId, ItemCode, ItemName, UnitId, UnitCode, Quantity, OpenQuantity,
        UnitPrice, DiscountPercent, DiscountAmount, TaxId, TaxCode, TaxRate, TaxAmount, WarehouseId, WarehouseCode,
        DeliveryDate, CostCenterId, ProjectId, LineSubtotal, LineTotal, Status
    FROM OPENJSON(ISNULL(@LinesJson, N'[]'))
    WITH
    (
        LineNumber int '$.lineNumber',
        ItemId int '$.itemId',
        ItemCode nvarchar(50) '$.itemCode',
        ItemName nvarchar(200) '$.itemName',
        UnitId int '$.unitId',
        UnitCode nvarchar(50) '$.unitCode',
        Quantity decimal(19,6) '$.quantity',
        OpenQuantity decimal(19,6) '$.openQuantity',
        UnitPrice decimal(19,6) '$.unitPrice',
        DiscountPercent decimal(9,6) '$.discountPercent',
        DiscountAmount decimal(19,6) '$.discountAmount',
        TaxId int '$.taxId',
        TaxCode nvarchar(50) '$.taxCode',
        TaxRate decimal(9,6) '$.taxRate',
        TaxAmount decimal(19,6) '$.taxAmount',
        WarehouseId int '$.warehouseId',
        WarehouseCode nvarchar(50) '$.warehouseCode',
        DeliveryDate date '$.deliveryDate',
        CostCenterId int '$.costCenterId',
        ProjectId int '$.projectId',
        LineSubtotal decimal(19,6) '$.lineSubtotal',
        LineTotal decimal(19,6) '$.lineTotal',
        Status nvarchar(40) '$.status'
    );

    INSERT INTO dbo.PurchaseOrderAddresses
        (PurchaseOrderId, AddressType, SourceAddressId, AddressName, Street, Reference, City, State, ZipCode, Country, Phone, Email, IsModified)
    SELECT @Id, AddressType, SourceAddressId, AddressName, Street, Reference, City, State, ZipCode, Country, Phone, Email, IsModified
    FROM OPENJSON(ISNULL(@AddressesJson, N'[]'))
    WITH
    (
        AddressType nvarchar(20) '$.addressType',
        SourceAddressId int '$.sourceAddressId',
        AddressName nvarchar(200) '$.addressName',
        Street nvarchar(500) '$.street',
        Reference nvarchar(500) '$.reference',
        City nvarchar(120) '$.city',
        State nvarchar(120) '$.state',
        ZipCode nvarchar(40) '$.zipCode',
        Country nvarchar(120) '$.country',
        Phone nvarchar(80) '$.phone',
        Email nvarchar(200) '$.email',
        IsModified bit '$.isModified'
    );

    INSERT INTO dbo.PurchaseOrderRelatedDocuments
        (PurchaseOrderId, RelatedDocumentType, RelatedDocumentId, Series, Number, Date, Status, Reference, Comment, Total, CreatedByUserId, CreatedByUserName)
    SELECT @Id, RelatedDocumentType, RelatedDocumentId, Series, Number, Date, Status, Reference, Comment, Total, @AuditUserId, @AuditUserName
    FROM OPENJSON(ISNULL(@RelatedDocumentsJson, N'[]'))
    WITH
    (
        RelatedDocumentType nvarchar(80) '$.relatedDocumentType',
        RelatedDocumentId int '$.relatedDocumentId',
        Series nvarchar(50) '$.series',
        Number nvarchar(50) '$.number',
        Date date '$.date',
        Status nvarchar(40) '$.status',
        Reference nvarchar(200) '$.reference',
        Comment nvarchar(1000) '$.comment',
        Total decimal(19,6) '$.total'
    );

    INSERT INTO dbo.PurchaseOrderAttachments
        (PurchaseOrderId, FileName, OriginalFileName, FileExtension, MimeType, FileSize, StoragePath, Status, Comment, CreatedByUserId, CreatedByUserName)
    SELECT @Id, FileName, OriginalFileName, FileExtension, MimeType, FileSize, StoragePath, Status, Comment, @AuditUserId, @AuditUserName
    FROM OPENJSON(ISNULL(@AttachmentsJson, N'[]'))
    WITH
    (
        FileName nvarchar(260) '$.fileName',
        OriginalFileName nvarchar(260) '$.originalFileName',
        FileExtension nvarchar(20) '$.fileExtension',
        MimeType nvarchar(120) '$.mimeType',
        FileSize bigint '$.fileSize',
        StoragePath nvarchar(1000) '$.storagePath',
        Status nvarchar(40) '$.status',
        Comment nvarchar(1000) '$.comment'
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_PURCHASEORDERS_ELIMINAR
    @Id int,
    @ExpectedStatusesJson nvarchar(max) = NULL,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- @ExpectedStatusesJson protege anulacion concurrente.
    -- NULL queda solo por compatibilidad legacy; el backend nuevo debe enviar siempre estados esperados.
    UPDATE h
    SET IsDeleted = 1,
        Status = N'Cancelled',
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME()
    FROM dbo.PurchaseOrderHeaders AS h
    WHERE h.Id = @Id
      AND h.IsDeleted = 0
      AND (
          @ExpectedStatusesJson IS NULL
          OR EXISTS (
              SELECT 1
              FROM OPENJSON(@ExpectedStatusesJson)
              WHERE [value] = h.Status
          )
      );

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_PURCHASEORDERS_ESTADO
    @Id int,
    @Status nvarchar(40),
    @ExpectedStatusesJson nvarchar(max) = NULL,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- @ExpectedStatusesJson protege transiciones concurrentes.
    -- NULL queda solo por compatibilidad legacy; el backend nuevo debe enviar siempre estados esperados.
    UPDATE h
    SET Status = @Status,
        SapStatus = CASE WHEN @Status = N'SapPending' THEN N'Pending' WHEN @Status = N'SapSynced' THEN N'Synced' WHEN @Status = N'SapError' THEN N'Error' ELSE SapStatus END,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    FROM dbo.PurchaseOrderHeaders AS h
    WHERE h.Id = @Id
      AND h.IsDeleted = 0
      AND (
          @ExpectedStatusesJson IS NULL
          OR EXISTS (
              SELECT 1
              FROM OPENJSON(@ExpectedStatusesJson)
              WHERE [value] = h.Status
          )
      );

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PURCHASEORDERS_SAPLOG
    @PurchaseOrderId int,
    @Process nvarchar(80),
    @Status nvarchar(40),
    @Message nvarchar(max) = NULL,
    @UserId int = NULL,
    @UserName nvarchar(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AttemptNumber int = ISNULL((SELECT MAX(AttemptNumber) FROM dbo.PurchaseOrderSapSyncLogs WHERE PurchaseOrderId = @PurchaseOrderId), 0) + 1;

    INSERT INTO dbo.PurchaseOrderSapSyncLogs (PurchaseOrderId, Process, Status, Message, UserId, UserName, AttemptNumber)
    VALUES (@PurchaseOrderId, @Process, @Status, @Message, @UserId, @UserName, @AttemptNumber);

    SELECT Id, CreatedAt, Process, Status, Message, UserName, AttemptNumber
    FROM dbo.PurchaseOrderSapSyncLogs
    WHERE Id = CONVERT(bigint, SCOPE_IDENTITY());
END;
GO
