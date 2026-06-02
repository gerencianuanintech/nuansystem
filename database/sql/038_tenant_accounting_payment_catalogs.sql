/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    Crea catalogos auxiliares contables usados por proveedores.
    SQL Server es el motor principal; otros proveedores deben tener script equivalente.
*/

IF OBJECT_ID(N'dbo.AccountingPaymentMethods', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AccountingPaymentMethods
    (
        AccountingPaymentMethodId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AccountingPaymentMethods PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_AccountingPaymentMethods_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_AccountingPaymentMethods_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AccountingPaymentMethods_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.PaymentPriorities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PaymentPriorities
    (
        PaymentPriorityId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PaymentPriorities PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_PaymentPriorities_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_PaymentPriorities_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PaymentPriorities_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.ApprovalFlows', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApprovalFlows
    (
        ApprovalFlowId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ApprovalFlows PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_ApprovalFlows_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_ApprovalFlows_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ApprovalFlows_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.PaymentDocumentTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PaymentDocumentTypes
    (
        PaymentDocumentTypeId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PaymentDocumentTypes PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_PaymentDocumentTypes_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_PaymentDocumentTypes_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PaymentDocumentTypes_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_AccountingPaymentMethods_Code' AND object_id = OBJECT_ID(N'dbo.AccountingPaymentMethods'))
    CREATE UNIQUE INDEX UX_AccountingPaymentMethods_Code ON dbo.AccountingPaymentMethods(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PaymentPriorities_Code' AND object_id = OBJECT_ID(N'dbo.PaymentPriorities'))
    CREATE UNIQUE INDEX UX_PaymentPriorities_Code ON dbo.PaymentPriorities(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ApprovalFlows_Code' AND object_id = OBJECT_ID(N'dbo.ApprovalFlows'))
    CREATE UNIQUE INDEX UX_ApprovalFlows_Code ON dbo.ApprovalFlows(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PaymentDocumentTypes_Code' AND object_id = OBJECT_ID(N'dbo.PaymentDocumentTypes'))
    CREATE UNIQUE INDEX UX_PaymentDocumentTypes_Code ON dbo.PaymentDocumentTypes(Code) WHERE IsDeleted = 0;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'AccountingPaymentMethodId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD AccountingPaymentMethodId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'PaymentPriorityId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD PaymentPriorityId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'ApprovalFlowId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD ApprovalFlowId int NULL;
    IF COL_LENGTH(N'dbo.BusinessPartnerAccountingSettings', N'PaymentDocumentTypeId') IS NULL
        ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD PaymentDocumentTypeId int NULL;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerAccountingSettings_AccountingPaymentMethods')
    ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CONSTRAINT FK_BusinessPartnerAccountingSettings_AccountingPaymentMethods FOREIGN KEY (AccountingPaymentMethodId) REFERENCES dbo.AccountingPaymentMethods(AccountingPaymentMethodId);
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerAccountingSettings_PaymentPriorities')
    ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CONSTRAINT FK_BusinessPartnerAccountingSettings_PaymentPriorities FOREIGN KEY (PaymentPriorityId) REFERENCES dbo.PaymentPriorities(PaymentPriorityId);
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerAccountingSettings_ApprovalFlows')
    ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CONSTRAINT FK_BusinessPartnerAccountingSettings_ApprovalFlows FOREIGN KEY (ApprovalFlowId) REFERENCES dbo.ApprovalFlows(ApprovalFlowId);
GO

IF OBJECT_ID(N'dbo.BusinessPartnerAccountingSettings', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BusinessPartnerAccountingSettings_PaymentDocumentTypes')
    ALTER TABLE dbo.BusinessPartnerAccountingSettings ADD CONSTRAINT FK_BusinessPartnerAccountingSettings_PaymentDocumentTypes FOREIGN KEY (PaymentDocumentTypeId) REFERENCES dbo.PaymentDocumentTypes(PaymentDocumentTypeId);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.AccountingPaymentMethods WHERE Code = N'TRANSFER')
    INSERT INTO dbo.AccountingPaymentMethods (Code, Name, Description, CreatedByUserName) VALUES (N'TRANSFER', N'Transferencia bancaria', N'Pago mediante transferencia bancaria.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.AccountingPaymentMethods WHERE Code = N'CHEQUE')
    INSERT INTO dbo.AccountingPaymentMethods (Code, Name, Description, CreatedByUserName) VALUES (N'CHEQUE', N'Cheque', N'Pago mediante cheque.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.AccountingPaymentMethods WHERE Code = N'CASH')
    INSERT INTO dbo.AccountingPaymentMethods (Code, Name, Description, CreatedByUserName) VALUES (N'CASH', N'Efectivo proveedor', N'Pago de proveedor en efectivo controlado.', N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PaymentPriorities WHERE Code = N'NORMAL')
    INSERT INTO dbo.PaymentPriorities (Code, Name, Description, CreatedByUserName) VALUES (N'NORMAL', N'Normal', N'Prioridad normal de pago.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.PaymentPriorities WHERE Code = N'HIGH')
    INSERT INTO dbo.PaymentPriorities (Code, Name, Description, CreatedByUserName) VALUES (N'HIGH', N'Alta', N'Prioridad alta de pago.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.PaymentPriorities WHERE Code = N'HELD')
    INSERT INTO dbo.PaymentPriorities (Code, Name, Description, CreatedByUserName) VALUES (N'HELD', N'Retenida', N'Pago retenido hasta revision.', N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ApprovalFlows WHERE Code = N'GT5000')
    INSERT INTO dbo.ApprovalFlows (Code, Name, Description, CreatedByUserName) VALUES (N'GT5000', N'Pago > 5,000 requiere aprobacion', N'Flujo para pagos altos a proveedor.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.ApprovalFlows WHERE Code = N'ALWAYS')
    INSERT INTO dbo.ApprovalFlows (Code, Name, Description, CreatedByUserName) VALUES (N'ALWAYS', N'Siempre requiere aprobacion', N'Todos los pagos requieren aprobacion.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.ApprovalFlows WHERE Code = N'NONE')
    INSERT INTO dbo.ApprovalFlows (Code, Name, Description, CreatedByUserName) VALUES (N'NONE', N'Sin aprobacion', N'No requiere aprobacion previa.', N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PaymentDocumentTypes WHERE Code = N'PAYMENT')
    INSERT INTO dbo.PaymentDocumentTypes (Code, Name, Description, CreatedByUserName) VALUES (N'PAYMENT', N'Egreso proveedor', N'Documento de egreso para proveedor.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.PaymentDocumentTypes WHERE Code = N'DEBITNOTE')
    INSERT INTO dbo.PaymentDocumentTypes (Code, Name, Description, CreatedByUserName) VALUES (N'DEBITNOTE', N'Nota de debito', N'Documento de debito relacionado al proveedor.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.PaymentDocumentTypes WHERE Code = N'SETTLEMENT')
    INSERT INTO dbo.PaymentDocumentTypes (Code, Name, Description, CreatedByUserName) VALUES (N'SETTLEMENT', N'Liquidacion', N'Documento de liquidacion de pago.', N'Sistema');
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ACCOUNTINGPAYMENTMETHODS_LISTAR AS BEGIN SET NOCOUNT ON; SELECT AccountingPaymentMethodId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName FROM dbo.AccountingPaymentMethods WHERE IsDeleted = 0 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ACCOUNTINGPAYMENTMETHODS_BUSCARPORID @Id int AS BEGIN SET NOCOUNT ON; SELECT AccountingPaymentMethodId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName FROM dbo.AccountingPaymentMethods WHERE AccountingPaymentMethodId = @Id AND IsDeleted = 0; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ACCOUNTINGPAYMENTMETHODS_LOOKUP AS BEGIN SET NOCOUNT ON; SELECT AccountingPaymentMethodId AS Id, Code, Name, IsActive FROM dbo.AccountingPaymentMethods WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ACCOUNTINGPAYMENTMETHODS_BUSCARPORCODIGO @Code nvarchar(30), @ExcluirId int = NULL AS BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.AccountingPaymentMethods WHERE IsDeleted = 0 AND Code = @Code AND (@ExcluirId IS NULL OR AccountingPaymentMethodId <> @ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ACCOUNTINGPAYMENTMETHODS_CREAR @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; INSERT INTO dbo.AccountingPaymentMethods (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName); SELECT CONVERT(int, SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ACCOUNTINGPAYMENTMETHODS_ACTUALIZAR @Id int, @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; UPDATE dbo.AccountingPaymentMethods SET Code = @Code, Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName WHERE AccountingPaymentMethodId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_ACCOUNTINGPAYMENTMETHODS_ELIMINAR @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; UPDATE dbo.AccountingPaymentMethods SET IsDeleted = 1, IsActive = 0, DeletedAt = SYSUTCDATETIME(), DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName WHERE AccountingPaymentMethodId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PAYMENTPRIORITIES_LISTAR AS BEGIN SET NOCOUNT ON; SELECT PaymentPriorityId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName FROM dbo.PaymentPriorities WHERE IsDeleted = 0 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PAYMENTPRIORITIES_BUSCARPORID @Id int AS BEGIN SET NOCOUNT ON; SELECT PaymentPriorityId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName FROM dbo.PaymentPriorities WHERE PaymentPriorityId = @Id AND IsDeleted = 0; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PAYMENTPRIORITIES_LOOKUP AS BEGIN SET NOCOUNT ON; SELECT PaymentPriorityId AS Id, Code, Name, IsActive FROM dbo.PaymentPriorities WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PAYMENTPRIORITIES_BUSCARPORCODIGO @Code nvarchar(30), @ExcluirId int = NULL AS BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.PaymentPriorities WHERE IsDeleted = 0 AND Code = @Code AND (@ExcluirId IS NULL OR PaymentPriorityId <> @ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PAYMENTPRIORITIES_CREAR @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; INSERT INTO dbo.PaymentPriorities (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName); SELECT CONVERT(int, SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PAYMENTPRIORITIES_ACTUALIZAR @Id int, @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; UPDATE dbo.PaymentPriorities SET Code = @Code, Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName WHERE PaymentPriorityId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_PAYMENTPRIORITIES_ELIMINAR @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; UPDATE dbo.PaymentPriorities SET IsDeleted = 1, IsActive = 0, DeletedAt = SYSUTCDATETIME(), DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName WHERE PaymentPriorityId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_APPROVALFLOWS_LISTAR AS BEGIN SET NOCOUNT ON; SELECT ApprovalFlowId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName FROM dbo.ApprovalFlows WHERE IsDeleted = 0 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_APPROVALFLOWS_BUSCARPORID @Id int AS BEGIN SET NOCOUNT ON; SELECT ApprovalFlowId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName FROM dbo.ApprovalFlows WHERE ApprovalFlowId = @Id AND IsDeleted = 0; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_APPROVALFLOWS_LOOKUP AS BEGIN SET NOCOUNT ON; SELECT ApprovalFlowId AS Id, Code, Name, IsActive FROM dbo.ApprovalFlows WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_APPROVALFLOWS_BUSCARPORCODIGO @Code nvarchar(30), @ExcluirId int = NULL AS BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.ApprovalFlows WHERE IsDeleted = 0 AND Code = @Code AND (@ExcluirId IS NULL OR ApprovalFlowId <> @ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_APPROVALFLOWS_CREAR @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; INSERT INTO dbo.ApprovalFlows (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName); SELECT CONVERT(int, SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_APPROVALFLOWS_ACTUALIZAR @Id int, @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; UPDATE dbo.ApprovalFlows SET Code = @Code, Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName WHERE ApprovalFlowId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_APPROVALFLOWS_ELIMINAR @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; UPDATE dbo.ApprovalFlows SET IsDeleted = 1, IsActive = 0, DeletedAt = SYSUTCDATETIME(), DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName WHERE ApprovalFlowId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PAYMENTDOCUMENTTYPES_LISTAR AS BEGIN SET NOCOUNT ON; SELECT PaymentDocumentTypeId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName FROM dbo.PaymentDocumentTypes WHERE IsDeleted = 0 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PAYMENTDOCUMENTTYPES_BUSCARPORID @Id int AS BEGIN SET NOCOUNT ON; SELECT PaymentDocumentTypeId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName FROM dbo.PaymentDocumentTypes WHERE PaymentDocumentTypeId = @Id AND IsDeleted = 0; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PAYMENTDOCUMENTTYPES_LOOKUP AS BEGIN SET NOCOUNT ON; SELECT PaymentDocumentTypeId AS Id, Code, Name, IsActive FROM dbo.PaymentDocumentTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PAYMENTDOCUMENTTYPES_BUSCARPORCODIGO @Code nvarchar(30), @ExcluirId int = NULL AS BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.PaymentDocumentTypes WHERE IsDeleted = 0 AND Code = @Code AND (@ExcluirId IS NULL OR PaymentDocumentTypeId <> @ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PAYMENTDOCUMENTTYPES_CREAR @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; INSERT INTO dbo.PaymentDocumentTypes (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName); SELECT CONVERT(int, SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PAYMENTDOCUMENTTYPES_ACTUALIZAR @Id int, @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; UPDATE dbo.PaymentDocumentTypes SET Code = @Code, Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName WHERE PaymentDocumentTypeId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_PAYMENTDOCUMENTTYPES_ELIMINAR @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(100) = NULL AS BEGIN SET NOCOUNT ON; UPDATE dbo.PaymentDocumentTypes SET IsDeleted = 1, IsActive = 0, DeletedAt = SYSUTCDATETIME(), DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName WHERE PaymentDocumentTypeId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
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
    SELECT ProvinceId AS Id, Code, Name, IsActive FROM dbo.Provinces WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT CityId AS Id, Code, Name, IsActive FROM dbo.Cities WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
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
END;
GO
