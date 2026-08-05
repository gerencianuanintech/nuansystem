/* Configures the per-company read-only HANA SELECT for Cities and registers the handler capability. Master only. */
USE [NuanSystem_Master];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51176, 'Migration 176 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SapCompanySettings', N'U') IS NULL
    OR OBJECT_ID(N'dbo.SapCompanySettingsAudit', N'U') IS NULL
    OR OBJECT_ID(N'dbo.SapSyncHandlerCapabilities', N'U') IS NULL
    OR OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51176, 'Master SAP settings, audit and profile capability foundations are required before 176.', 1;
GO

IF COL_LENGTH(N'dbo.SapCompanySettings', N'CitiesSelectQuery') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD CitiesSelectQuery nvarchar(max) NULL;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPCOMPANYSETTINGS_BUSCARPOREMPRESAID
    @CompanyId int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (1)
        s.Id, s.CompanyId, c.Code AS CompanyCode, s.IsEnabled, s.IntegrationMode,
        s.ServiceLayerUrl, s.SapCompanyDb, s.SapUser, s.SapPasswordEncrypted,
        s.DiApiServer, s.LicenseServer, s.Language,
        s.HanaServer, s.HanaPort, s.HanaSchema, s.HanaUser, s.HanaPasswordEncrypted,
        s.CitiesSelectQuery, s.MaxRetryCount, s.UpdatedAt
    FROM dbo.SapCompanySettings s
    INNER JOIN dbo.Companies c ON c.Id = s.CompanyId
    WHERE s.CompanyId = @CompanyId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPCOMPANYSETTINGS_BUSCARPOREMPRESACODIGO
    @CompanyCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (1)
        s.Id, s.CompanyId, c.Code AS CompanyCode, s.IsEnabled, s.IntegrationMode,
        s.ServiceLayerUrl, s.SapCompanyDb, s.SapUser, s.SapPasswordEncrypted,
        s.DiApiServer, s.LicenseServer, s.Language,
        s.HanaServer, s.HanaPort, s.HanaSchema, s.HanaUser, s.HanaPasswordEncrypted,
        s.CitiesSelectQuery, s.MaxRetryCount, s.UpdatedAt
    FROM dbo.SapCompanySettings s
    INNER JOIN dbo.Companies c ON c.Id = s.CompanyId
    WHERE c.Code = @CompanyCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SAPCOMPANYSETTINGS_CITIESQUERY
    @CompanyId int,
    @CitiesSelectQuery nvarchar(max) = NULL,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @CitiesSelectQuery = NULLIF(LTRIM(RTRIM(@CitiesSelectQuery)), N'');
    IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId AND IsActive = 1)
        THROW 51176, 'La empresa no existe o esta inactiva.', 1;

    IF @CitiesSelectQuery IS NOT NULL
    BEGIN
        IF LEN(@CitiesSelectQuery) > 12000
            THROW 51176, 'La consulta SAP de ciudades supera 12000 caracteres.', 1;
        IF UPPER(LEFT(@CitiesSelectQuery, 6)) <> N'SELECT'
            THROW 51176, 'La consulta SAP de ciudades debe iniciar con SELECT.', 1;
        IF CHARINDEX(N';', @CitiesSelectQuery) > 0
            OR CHARINDEX(N'--', @CitiesSelectQuery) > 0
            OR CHARINDEX(N'/*', @CitiesSelectQuery) > 0
            OR CHARINDEX(N'*/', @CitiesSelectQuery) > 0
            THROW 51176, 'La consulta SAP de ciudades debe ser una sola sentencia SELECT sin comentarios.', 1;
        IF @CitiesSelectQuery NOT LIKE N'%AS%CountryCode%'
            OR @CitiesSelectQuery NOT LIKE N'%AS%ProvinceCode%'
            OR @CitiesSelectQuery NOT LIKE N'%AS%CityCode%'
            OR @CitiesSelectQuery NOT LIKE N'%AS%CityName%'
            THROW 51176, 'La consulta SAP de ciudades debe exponer CountryCode, ProvinceCode, CityCode y CityName.', 1;
    END;

    BEGIN TRANSACTION;
    DECLARE @SettingsId int;
    DECLARE @AuditAction nvarchar(20) = N'Update';
    SELECT @SettingsId = Id
    FROM dbo.SapCompanySettings WITH (UPDLOCK, HOLDLOCK)
    WHERE CompanyId = @CompanyId;

    IF @SettingsId IS NULL
    BEGIN
        SET @AuditAction = N'Create';
        INSERT dbo.SapCompanySettings
        (
            CompanyId, IsEnabled, IntegrationMode, CitiesSelectQuery, MaxRetryCount,
            CreatedByUserId, CreatedByUserName
        )
        VALUES
        (
            @CompanyId, 0, 0, @CitiesSelectQuery, 3,
            @UpdatedByUserId, @UpdatedByUserName
        );
        SET @SettingsId = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.SapCompanySettings
        SET CitiesSelectQuery = @CitiesSelectQuery,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @SettingsId;
    END;

    INSERT dbo.SapCompanySettingsAudit(CompanyId,[Action],ChangedFields,UserId,UserName)
    VALUES(@CompanyId,@AuditAction,N'CitiesSelectQuery',@UpdatedByUserId,@UpdatedByUserName);

    COMMIT;
    SELECT @SettingsId;
END;
GO

BEGIN TRANSACTION;
UPDATE dbo.SapSyncHandlerCapabilities
SET DisplayName = N'Ciudades', SupportsSapToErp = 1, SupportsErpToSap = 0,
    SupportsFull = 1, SupportsIncremental = 0, IsImplemented = 1, IsActive = 1,
    UpdatedByUserName = N'Sistema', UpdatedAt = SYSUTCDATETIME()
WHERE EntityCode = N'Cities';

IF NOT EXISTS (SELECT 1 FROM dbo.SapSyncHandlerCapabilities WHERE EntityCode = N'Cities')
    INSERT dbo.SapSyncHandlerCapabilities
    (
        EntityCode,DisplayName,SupportsSapToErp,SupportsErpToSap,
        SupportsFull,SupportsIncremental,IsImplemented,IsActive,CreatedByUserName
    )
    VALUES(N'Cities',N'Ciudades',1,0,1,0,1,1,N'Sistema');

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260805.176')
    INSERT dbo.MasterSchemaHistory(Version,Description)
    VALUES(N'20260805.176',N'Configura SELECT HANA por empresa y registra sincronizacion SAP Full de Ciudades');
COMMIT;
GO
