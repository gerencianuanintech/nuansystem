/*
    Migracion 154 - Hardening de la API de perfiles SAP en NuanSystem_Master.

    Prerrequisito: 152_master_sap_sync_profiles.sql.
    Alcance:
      - consulta parametrizada de empresas accesibles para perfiles SAP;
      - sin secretos de SapCompanySettings;
      - reparacion forward-only e idempotente.

    No crea formularios, menus, permisos ni perfiles. No habilita agendas.
    SapSyncEntitySettings se conserva sin cambios.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Companies', N'U') IS NULL
    THROW 51154, 'Companies is required before migration 154.', 1;
IF OBJECT_ID(N'dbo.UserCompanies', N'U') IS NULL
    THROW 51154, 'UserCompanies is required before migration 154.', 1;
IF OBJECT_ID(N'dbo.SapCompanySettings', N'U') IS NULL
    THROW 51154, 'SapCompanySettings is required before migration 154.', 1;
IF OBJECT_ID(N'dbo.SapSyncProfiles', N'U') IS NULL
    THROW 51154, 'Migration 152 is required before migration 154.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51154, 'MasterSchemaHistory is required before migration 154.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCPROFILEEMPRESASACCESIBLES
    @UserId int,
    @CompanyId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        company.Id AS CompanyId,
        company.Code AS CompanyCode,
        company.CommercialName AS CompanyName,
        company.IsActive AS IsCompanyActive,
        company.SapIntegrationMode,
        CONVERT(bit, CASE WHEN settings.Id IS NULL THEN 0 ELSE 1 END) AS HasSapSettings,
        CONVERT(bit, COALESCE(settings.IsEnabled, 0)) AS IsSapEnabled,
        COALESCE(settings.IntegrationMode, 0) AS SapSettingsIntegrationMode,
        CONVERT(bit, CASE WHEN userCompany.UserId IS NULL THEN 0 ELSE 1 END) AS IsUserAuthorized
    FROM dbo.Companies company
    LEFT JOIN dbo.SapCompanySettings settings
        ON settings.CompanyId = company.Id
    LEFT JOIN dbo.UserCompanies userCompany
        ON userCompany.CompanyId = company.Id
       AND userCompany.UserId = @UserId
       AND userCompany.IsActive = 1
    WHERE
        (@CompanyId IS NULL AND userCompany.UserId IS NOT NULL)
        OR
        (@CompanyId IS NOT NULL AND company.Id = @CompanyId)
    ORDER BY company.CommercialName, company.Code;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260730.154'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260730.154',
        N'Hardening de acceso y propiedad de perfiles SAP'
    );
END;
GO
