/* Master 234: company-scoped SAP business-partner identity policy. */
IF OBJECT_ID(N'dbo.Companies', N'U') IS NULL THROW 51234, 'Companies is required before migration 234.', 1;

IF OBJECT_ID(N'dbo.SapBusinessPartnerIdentityPolicies', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapBusinessPartnerIdentityPolicies
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapBusinessPartnerIdentityPolicies PRIMARY KEY,
        CompanyId int NOT NULL,
        CustomerNationalPrefix nvarchar(10) NOT NULL,
        CustomerForeignPrefix nvarchar(10) NOT NULL,
        SupplierNationalPrefix nvarchar(10) NOT NULL,
        SupplierForeignPrefix nvarchar(10) NOT NULL,
        PassportTaxpayerTypeCodes nvarchar(500) NOT NULL,
        PassportIdentificationTypeCode nvarchar(50) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_SapBusinessPartnerIdentityPolicies_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapBusinessPartnerIdentityPolicies_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SapBusinessPartnerIdentityPolicies_IsDeleted DEFAULT 0,
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_SapBusinessPartnerIdentityPolicies_Company FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_SapBusinessPartnerIdentityPolicies_Prefixes CHECK (
            LEN(LTRIM(RTRIM(CustomerNationalPrefix))) > 0 AND LEN(LTRIM(RTRIM(CustomerForeignPrefix))) > 0 AND
            LEN(LTRIM(RTRIM(SupplierNationalPrefix))) > 0 AND LEN(LTRIM(RTRIM(SupplierForeignPrefix))) > 0),
        CONSTRAINT CK_SapBusinessPartnerIdentityPolicies_Passport CHECK (
            LEN(LTRIM(RTRIM(PassportTaxpayerTypeCodes))) > 0 AND LEN(LTRIM(RTRIM(PassportIdentificationTypeCode))) > 0)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SapBusinessPartnerIdentityPolicies_Company_Active' AND object_id = OBJECT_ID(N'dbo.SapBusinessPartnerIdentityPolicies'))
    CREATE UNIQUE INDEX UX_SapBusinessPartnerIdentityPolicies_Company_Active ON dbo.SapBusinessPartnerIdentityPolicies(CompanyId) WHERE IsDeleted = 0 AND IsActive = 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPIDENTITYPOLICY_BUSCAR
    @CompanyId int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (1) Id, CompanyId, CustomerNationalPrefix, CustomerForeignPrefix, SupplierNationalPrefix, SupplierForeignPrefix,
        PassportTaxpayerTypeCodes, PassportIdentificationTypeCode, IsActive, RowVersion
    FROM dbo.SapBusinessPartnerIdentityPolicies
    WHERE CompanyId = @CompanyId AND IsDeleted = 0 AND IsActive = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPIDENTITYPOLICY_GUARDAR
    @CompanyId int,
    @CustomerNationalPrefix nvarchar(10), @CustomerForeignPrefix nvarchar(10),
    @SupplierNationalPrefix nvarchar(10), @SupplierForeignPrefix nvarchar(10),
    @PassportTaxpayerTypeCodes nvarchar(500), @PassportIdentificationTypeCode nvarchar(50),
    @AuditUserId int = NULL, @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    UPDATE dbo.SapBusinessPartnerIdentityPolicies
    SET IsActive = 0, UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @CompanyId AND IsDeleted = 0 AND IsActive = 1;
    INSERT dbo.SapBusinessPartnerIdentityPolicies
    (CompanyId, CustomerNationalPrefix, CustomerForeignPrefix, SupplierNationalPrefix, SupplierForeignPrefix,
     PassportTaxpayerTypeCodes, PassportIdentificationTypeCode, CreatedByUserId, CreatedByUserName)
    VALUES
    (@CompanyId, LTRIM(RTRIM(@CustomerNationalPrefix)), LTRIM(RTRIM(@CustomerForeignPrefix)), LTRIM(RTRIM(@SupplierNationalPrefix)), LTRIM(RTRIM(@SupplierForeignPrefix)),
     LTRIM(RTRIM(@PassportTaxpayerTypeCodes)), LTRIM(RTRIM(@PassportIdentificationTypeCode)), @AuditUserId, @AuditUserName);
    SELECT CONVERT(bigint, SCOPE_IDENTITY()) AS Id;
    COMMIT TRANSACTION;
END;
GO
