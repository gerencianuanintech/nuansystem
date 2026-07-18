/*
    Fase 3: preparar entidades replicables para sincronizacion Master/Sucursal.

    Reglas:
    - Id sigue siendo local de cada base.
    - GlobalId identifica la misma entidad entre Master/Sucursal.
    - Code sigue siendo el codigo funcional.
    - ExternalSystem, ExternalCode y SapCode son opcionales.
    - SAP no es obligatorio.
    - Este script no crea Outbox/Inbox ni workers.
*/

/* Master database objects */

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Users', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Users_GlobalId DEFAULT NEWID();
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Users', N'ExternalSystem') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD ExternalSystem nvarchar(50) NULL;
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Users', N'ExternalCode') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD ExternalCode nvarchar(100) NULL;
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Users_GlobalId' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
    CREATE UNIQUE INDEX UX_Users_GlobalId ON dbo.Users (GlobalId);
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_ExternalRef' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
    CREATE INDEX IX_Users_ExternalRef ON dbo.Users (ExternalSystem, ExternalCode) WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.UserRoles', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.UserCompanies', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Companies', N'U') IS NOT NULL
BEGIN
    EXEC(N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_USUARIOSEGURIDADLISTAR
AS
BEGIN
    SELECT
        u.Id,
        u.UserName,
        u.Email,
        u.PhoneNumber,
        u.EmailConfirmed,
        u.PhoneNumberConfirmed,
        u.FirstName,
        u.LastName,
        u.DisplayName,
        u.IsActive,
        u.IsLocked,
        u.CanUseWeb,
        u.CanUseMobile,
        u.FailedAccessCount,
        u.LastLoginAt,
        u.MustChangePassword,
        u.LockoutEndAt,
        u.TwoFactorEnabled,
        u.ProfileImageUrl,
        u.ProfileImage,
        u.ProfileImageContentType,
        u.ProfileImageFileName,
        roleInfo.RoleId,
        COALESCE(roleInfo.Roles, N'''') AS RolesText,
        COALESCE(companyInfo.Companies, N'''') AS CompaniesText,
        u.CreatedByUserId,
        u.CreatedByUserName,
        u.CreatedAt,
        u.UpdatedByUserId,
        u.UpdatedByUserName,
        u.UpdatedAt,
        u.DeletedByUserId,
        u.DeletedByUserName,
        u.DeletedAt,
        u.GlobalId,
        u.ExternalSystem,
        u.ExternalCode
    FROM dbo.Users u
    OUTER APPLY
    (
        SELECT
            MIN(ur.RoleId) AS RoleId,
            STRING_AGG(CONVERT(nvarchar(max), r.Code), N'','') WITHIN GROUP (ORDER BY r.Code) AS Roles
        FROM dbo.UserRoles ur
        INNER JOIN dbo.Roles r ON r.Id = ur.RoleId
        WHERE ur.UserId = u.Id
    ) roleInfo
    OUTER APPLY
    (
        SELECT STRING_AGG(CONVERT(nvarchar(max), c.Code), N'','') WITHIN GROUP (ORDER BY c.Code) AS Companies
        FROM dbo.UserCompanies uc
        INNER JOIN dbo.Companies c ON c.Id = uc.CompanyId
        WHERE uc.UserId = u.Id
          AND uc.IsActive = 1
    ) companyInfo
    WHERE u.IsDeleted = 0
    ORDER BY u.UserName;
END;');

    EXEC(N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_USUARIOSEGURIDADBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        u.Id,
        u.UserName,
        u.Email,
        u.PhoneNumber,
        u.EmailConfirmed,
        u.PhoneNumberConfirmed,
        u.FirstName,
        u.LastName,
        u.DisplayName,
        u.IsActive,
        u.IsLocked,
        u.CanUseWeb,
        u.CanUseMobile,
        u.FailedAccessCount,
        u.LastLoginAt,
        u.MustChangePassword,
        u.LockoutEndAt,
        u.TwoFactorEnabled,
        u.ProfileImageUrl,
        u.ProfileImage,
        u.ProfileImageContentType,
        u.ProfileImageFileName,
        roleInfo.RoleId,
        COALESCE(roleInfo.Roles, N'''') AS RolesText,
        COALESCE(companyInfo.Companies, N'''') AS CompaniesText,
        u.CreatedByUserId,
        u.CreatedByUserName,
        u.CreatedAt,
        u.UpdatedByUserId,
        u.UpdatedByUserName,
        u.UpdatedAt,
        u.DeletedByUserId,
        u.DeletedByUserName,
        u.DeletedAt,
        u.GlobalId,
        u.ExternalSystem,
        u.ExternalCode
    FROM dbo.Users u
    OUTER APPLY
    (
        SELECT
            MIN(ur.RoleId) AS RoleId,
            STRING_AGG(CONVERT(nvarchar(max), r.Code), N'','') WITHIN GROUP (ORDER BY r.Code) AS Roles
        FROM dbo.UserRoles ur
        INNER JOIN dbo.Roles r ON r.Id = ur.RoleId
        WHERE ur.UserId = u.Id
    ) roleInfo
    OUTER APPLY
    (
        SELECT STRING_AGG(CONVERT(nvarchar(max), c.Code), N'','') WITHIN GROUP (ORDER BY c.Code) AS Companies
        FROM dbo.UserCompanies uc
        INNER JOIN dbo.Companies c ON c.Id = uc.CompanyId
        WHERE uc.UserId = u.Id
          AND uc.IsActive = 1
    ) companyInfo
    WHERE u.Id = @Id
      AND u.IsDeleted = 0;
END;');
END;
GO

IF OBJECT_ID(N'dbo.CompanyParameters', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.CompanyParameters', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_CompanyParameters_GlobalId DEFAULT NEWID();
END;
GO

IF OBJECT_ID(N'dbo.CompanyParameters', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.CompanyParameters', N'ExternalSystem') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD ExternalSystem nvarchar(50) NULL;
END;
GO

IF OBJECT_ID(N'dbo.CompanyParameters', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.CompanyParameters', N'ExternalCode') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD ExternalCode nvarchar(100) NULL;
END;
GO

IF OBJECT_ID(N'dbo.CompanyParameters', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_CompanyParameters_GlobalId' AND object_id = OBJECT_ID(N'dbo.CompanyParameters'))
BEGIN
    CREATE UNIQUE INDEX UX_CompanyParameters_GlobalId ON dbo.CompanyParameters (GlobalId);
END;
GO

IF OBJECT_ID(N'dbo.CompanyParameters', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CompanyParameters_ExternalRef' AND object_id = OBJECT_ID(N'dbo.CompanyParameters'))
BEGIN
    CREATE INDEX IX_CompanyParameters_ExternalRef ON dbo.CompanyParameters (CompanyId, ExternalSystem, ExternalCode)
    WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
END;
GO

/* Tenant business partners */

IF OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.BusinessPartners', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.BusinessPartners ADD GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_BusinessPartners_GlobalId DEFAULT NEWID();
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.BusinessPartners', N'ExternalSystem') IS NULL
BEGIN
    ALTER TABLE dbo.BusinessPartners ADD ExternalSystem nvarchar(50) NULL;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.BusinessPartners', N'ExternalCode') IS NULL
BEGIN
    ALTER TABLE dbo.BusinessPartners ADD ExternalCode nvarchar(100) NULL;
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_BusinessPartners_GlobalId' AND object_id = OBJECT_ID(N'dbo.BusinessPartners'))
BEGIN
    CREATE UNIQUE INDEX UX_BusinessPartners_GlobalId ON dbo.BusinessPartners (GlobalId);
END;
GO

IF OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_BusinessPartners_ExternalRef' AND object_id = OBJECT_ID(N'dbo.BusinessPartners'))
BEGIN
    CREATE INDEX IX_BusinessPartners_ExternalRef ON dbo.BusinessPartners (ExternalSystem, ExternalCode)
    WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
END;
GO

/* Tenant inventory and pricing masters */

IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Items', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.Items ADD GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Items_GlobalId DEFAULT NEWID();
END;
GO

IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Items', N'ExternalSystem') IS NULL
BEGIN
    ALTER TABLE dbo.Items ADD ExternalSystem nvarchar(50) NULL;
END;
GO

IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Items', N'ExternalCode') IS NULL
BEGIN
    ALTER TABLE dbo.Items ADD ExternalCode nvarchar(100) NULL;
END;
GO

IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Items', N'SapCode') IS NULL
BEGIN
    ALTER TABLE dbo.Items ADD SapCode nvarchar(100) NULL;
END;
GO

IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Items_GlobalId' AND object_id = OBJECT_ID(N'dbo.Items'))
BEGIN
    CREATE UNIQUE INDEX UX_Items_GlobalId ON dbo.Items (GlobalId);
END;
GO

IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Items_ExternalRef' AND object_id = OBJECT_ID(N'dbo.Items'))
BEGIN
    CREATE INDEX IX_Items_ExternalRef ON dbo.Items (ExternalSystem, ExternalCode)
    WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
END;
GO

IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Items_SapCode' AND object_id = OBJECT_ID(N'dbo.Items'))
BEGIN
    CREATE INDEX IX_Items_SapCode ON dbo.Items (SapCode) WHERE SapCode IS NOT NULL;
END;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Warehouses', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.Warehouses ADD GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Warehouses_GlobalId DEFAULT NEWID();
END;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Warehouses', N'ExternalSystem') IS NULL
BEGIN
    ALTER TABLE dbo.Warehouses ADD ExternalSystem nvarchar(50) NULL;
END;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Warehouses', N'ExternalCode') IS NULL
BEGIN
    ALTER TABLE dbo.Warehouses ADD ExternalCode nvarchar(100) NULL;
END;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Warehouses', N'SapCode') IS NULL
BEGIN
    ALTER TABLE dbo.Warehouses ADD SapCode nvarchar(100) NULL;
END;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Warehouses_GlobalId' AND object_id = OBJECT_ID(N'dbo.Warehouses'))
BEGIN
    CREATE UNIQUE INDEX UX_Warehouses_GlobalId ON dbo.Warehouses (GlobalId);
END;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Warehouses_ExternalRef' AND object_id = OBJECT_ID(N'dbo.Warehouses'))
BEGIN
    CREATE INDEX IX_Warehouses_ExternalRef ON dbo.Warehouses (ExternalSystem, ExternalCode)
    WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
END;
GO

IF OBJECT_ID(N'dbo.PriceLists', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.PriceLists', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.PriceLists ADD GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_PriceLists_GlobalId DEFAULT NEWID();
END;
GO

IF OBJECT_ID(N'dbo.PriceLists', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.PriceLists', N'ExternalSystem') IS NULL
BEGIN
    ALTER TABLE dbo.PriceLists ADD ExternalSystem nvarchar(50) NULL;
END;
GO

IF OBJECT_ID(N'dbo.PriceLists', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.PriceLists', N'ExternalCode') IS NULL
BEGIN
    ALTER TABLE dbo.PriceLists ADD ExternalCode nvarchar(100) NULL;
END;
GO

IF OBJECT_ID(N'dbo.PriceLists', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.PriceLists', N'SapCode') IS NULL
BEGIN
    ALTER TABLE dbo.PriceLists ADD SapCode nvarchar(100) NULL;
END;
GO

IF OBJECT_ID(N'dbo.PriceLists', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PriceLists_GlobalId' AND object_id = OBJECT_ID(N'dbo.PriceLists'))
BEGIN
    CREATE UNIQUE INDEX UX_PriceLists_GlobalId ON dbo.PriceLists (GlobalId);
END;
GO

IF OBJECT_ID(N'dbo.PriceLists', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PriceLists_ExternalRef' AND object_id = OBJECT_ID(N'dbo.PriceLists'))
BEGIN
    CREATE INDEX IX_PriceLists_ExternalRef ON dbo.PriceLists (ExternalSystem, ExternalCode)
    WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
END;
GO

/* Tenant administrative catalogs */

DECLARE @CatalogTables TABLE
(
    TableName sysname NOT NULL,
    PrimaryKeyName sysname NOT NULL,
    HasSapCode bit NOT NULL
);

INSERT INTO @CatalogTables (TableName, PrimaryKeyName, HasSapCode)
VALUES
    (N'UnitOfMeasures', N'Id', 0),
    (N'ItemGroups', N'Id', 1),
    (N'ItemFamilies', N'ItemFamilyId', 1),
    (N'Taxes', N'Id', 0),
    (N'Currencies', N'CurrencyId', 0),
    (N'BusinessPartnerIdentificationTypes', N'Id', 0),
    (N'BusinessPartnerPaymentTerms', N'Id', 0),
    (N'SupplierGroups', N'SupplierGroupId', 0),
    (N'SupplierClasses', N'SupplierClassId', 0),
    (N'EconomicActivities', N'EconomicActivityId', 0),
    (N'Zones', N'ZoneId', 0),
    (N'SupplyMethods', N'SupplyMethodId', 0),
    (N'ContactTypes', N'ContactTypeId', 0),
    (N'ContactChannels', N'ContactChannelId', 0),
    (N'Countries', N'CountryId', 0),
    (N'Provinces', N'ProvinceId', 0),
    (N'Cities', N'CityId', 0),
    (N'Banks', N'BankId', 0),
    (N'BankAccountTypes', N'BankAccountTypeId', 0),
    (N'OperationalCatalog', N'OperationalCatalogId', 0);

DECLARE
    @TableName sysname,
    @PrimaryKeyName sysname,
    @HasSapCode bit,
    @Sql nvarchar(max);

DECLARE catalog_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT TableName, PrimaryKeyName, HasSapCode
FROM @CatalogTables;

OPEN catalog_cursor;
FETCH NEXT FROM catalog_cursor INTO @TableName, @PrimaryKeyName, @HasSapCode;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF OBJECT_ID(N'dbo.' + @TableName, N'U') IS NOT NULL
    BEGIN
        IF COL_LENGTH(N'dbo.' + @TableName, N'GlobalId') IS NULL
        BEGIN
            SET @Sql = N'ALTER TABLE dbo.' + QUOTENAME(@TableName) + N' ADD GlobalId uniqueidentifier NOT NULL CONSTRAINT '
                + QUOTENAME(N'DF_' + @TableName + N'_GlobalId') + N' DEFAULT NEWID();';
            EXEC sys.sp_executesql @Sql;
        END;

        IF COL_LENGTH(N'dbo.' + @TableName, N'ExternalSystem') IS NULL
        BEGIN
            SET @Sql = N'ALTER TABLE dbo.' + QUOTENAME(@TableName) + N' ADD ExternalSystem nvarchar(50) NULL;';
            EXEC sys.sp_executesql @Sql;
        END;

        IF COL_LENGTH(N'dbo.' + @TableName, N'ExternalCode') IS NULL
        BEGIN
            SET @Sql = N'ALTER TABLE dbo.' + QUOTENAME(@TableName) + N' ADD ExternalCode nvarchar(100) NULL;';
            EXEC sys.sp_executesql @Sql;
        END;

        IF @HasSapCode = 1 AND COL_LENGTH(N'dbo.' + @TableName, N'SapCode') IS NULL
        BEGIN
            SET @Sql = N'ALTER TABLE dbo.' + QUOTENAME(@TableName) + N' ADD SapCode nvarchar(100) NULL;';
            EXEC sys.sp_executesql @Sql;
        END;

        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_' + @TableName + N'_GlobalId' AND object_id = OBJECT_ID(N'dbo.' + @TableName))
        BEGIN
            SET @Sql = N'CREATE UNIQUE INDEX ' + QUOTENAME(N'UX_' + @TableName + N'_GlobalId')
                + N' ON dbo.' + QUOTENAME(@TableName) + N' (GlobalId);';
            EXEC sys.sp_executesql @Sql;
        END;

        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_' + @TableName + N'_ExternalRef' AND object_id = OBJECT_ID(N'dbo.' + @TableName))
        BEGIN
            SET @Sql = N'CREATE INDEX ' + QUOTENAME(N'IX_' + @TableName + N'_ExternalRef')
                + N' ON dbo.' + QUOTENAME(@TableName) + N' (ExternalSystem, ExternalCode) WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;';
            EXEC sys.sp_executesql @Sql;
        END;
    END;

    FETCH NEXT FROM catalog_cursor INTO @TableName, @PrimaryKeyName, @HasSapCode;
END;

CLOSE catalog_cursor;
DEALLOCATE catalog_cursor;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NOT NULL
BEGIN
    EXEC sys.sp_executesql N'
        IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N''20260709.03'')
        BEGIN
            INSERT INTO dbo.SchemaHistory (Version, Description)
            VALUES (N''20260709.03'', N''Fase 3: GlobalId y referencias externas opcionales en entidades replicables'');
        END;';
END;
GO
