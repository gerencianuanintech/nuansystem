USE [NuanSystem_Master];
GO

IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaServer') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaServer nvarchar(200) NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaPort') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaPort int NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaSchema') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaSchema nvarchar(128) NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaUser') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaUser nvarchar(128) NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'HanaPasswordEncrypted') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD HanaPasswordEncrypted nvarchar(max) NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'MaxRetryCount') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD MaxRetryCount int NOT NULL CONSTRAINT DF_SapCompanySettings_MaxRetryCount DEFAULT 3;
GO

IF COL_LENGTH(N'dbo.SapCompanySettings', N'CreatedByUserId') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD CreatedByUserId int NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'CreatedByUserName') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD CreatedByUserName nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'UpdatedByUserId') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD UpdatedByUserId int NULL;
IF COL_LENGTH(N'dbo.SapCompanySettings', N'UpdatedByUserName') IS NULL
    ALTER TABLE dbo.SapCompanySettings ADD UpdatedByUserName nvarchar(120) NULL;
GO

IF OBJECT_ID(N'dbo.SapCompanySettingsAudit', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapCompanySettingsAudit
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapCompanySettingsAudit PRIMARY KEY,
        CompanyId int NOT NULL,
        [Action] nvarchar(20) NOT NULL,
        ChangedFields nvarchar(500) NOT NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapCompanySettingsAudit_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_SapCompanySettingsAudit_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id)
    );

    CREATE INDEX IX_SapCompanySettingsAudit_Company_CreatedAt
        ON dbo.SapCompanySettingsAudit (CompanyId, CreatedAt DESC);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPCOMPANYSETTINGS_BUSCARPOREMPRESAID
    @CompanyId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        s.Id,
        s.CompanyId,
        c.Code AS CompanyCode,
        s.IsEnabled,
        s.IntegrationMode,
        s.ServiceLayerUrl,
        s.SapCompanyDb,
        s.SapUser,
        s.SapPasswordEncrypted,
        s.DiApiServer,
        s.LicenseServer,
        s.Language,
        s.HanaServer,
        s.HanaPort,
        s.HanaSchema,
        s.HanaUser,
        s.HanaPasswordEncrypted,
        s.MaxRetryCount,
        s.UpdatedAt
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
        s.Id,
        s.CompanyId,
        c.Code AS CompanyCode,
        s.IsEnabled,
        s.IntegrationMode,
        s.ServiceLayerUrl,
        s.SapCompanyDb,
        s.SapUser,
        s.SapPasswordEncrypted,
        s.DiApiServer,
        s.LicenseServer,
        s.Language,
        s.HanaServer,
        s.HanaPort,
        s.HanaSchema,
        s.HanaUser,
        s.HanaPasswordEncrypted,
        s.MaxRetryCount,
        s.UpdatedAt
    FROM dbo.SapCompanySettings s
    INNER JOIN dbo.Companies c ON c.Id = s.CompanyId
    WHERE c.Code = @CompanyCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SAPCOMPANYSETTINGS_SERVICELAYER
    @CompanyId int,
    @IsEnabled bit,
    @ServiceLayerUrl nvarchar(500),
    @SapCompanyDb nvarchar(128),
    @SapUser nvarchar(128),
    @SapPasswordEncrypted nvarchar(max) = NULL,
    @MaxRetryCount int,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId AND IsActive = 1)
            THROW 50001, 'La empresa no existe o esta inactiva.', 1;

        DECLARE @SettingsId int;
        DECLARE @Action nvarchar(20);

        SELECT @SettingsId = Id
        FROM dbo.SapCompanySettings WITH (UPDLOCK, HOLDLOCK)
        WHERE CompanyId = @CompanyId;

        IF @SettingsId IS NULL
        BEGIN
            IF @IsEnabled = 1 AND NULLIF(@SapPasswordEncrypted, N'') IS NULL
                THROW 50002, 'La credencial SAP protegida es obligatoria al activar Service Layer.', 1;

            INSERT INTO dbo.SapCompanySettings
            (
                CompanyId,
                IsEnabled,
                IntegrationMode,
                ServiceLayerUrl,
                SapCompanyDb,
                SapUser,
                SapPasswordEncrypted,
                MaxRetryCount,
                CreatedByUserId,
                CreatedByUserName
            )
            VALUES
            (
                @CompanyId,
                @IsEnabled,
                1,
                @ServiceLayerUrl,
                @SapCompanyDb,
                @SapUser,
                @SapPasswordEncrypted,
                @MaxRetryCount,
                @UpdatedByUserId,
                @UpdatedByUserName
            );

            SET @SettingsId = CONVERT(int, SCOPE_IDENTITY());
            SET @Action = N'Create';
        END
        ELSE
        BEGIN
            UPDATE dbo.SapCompanySettings
            SET IsEnabled = @IsEnabled,
                IntegrationMode = 1,
                ServiceLayerUrl = @ServiceLayerUrl,
                SapCompanyDb = @SapCompanyDb,
                SapUser = @SapUser,
                SapPasswordEncrypted = COALESCE(NULLIF(@SapPasswordEncrypted, N''), SapPasswordEncrypted),
                MaxRetryCount = @MaxRetryCount,
                UpdatedByUserId = @UpdatedByUserId,
                UpdatedByUserName = @UpdatedByUserName,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @SettingsId;

            SET @Action = N'Update';
        END;

        UPDATE dbo.Companies
        SET SapIntegrationMode = CASE WHEN @IsEnabled = 1 THEN 1 ELSE 0 END,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @CompanyId;

        INSERT INTO dbo.SapCompanySettingsAudit
        (
            CompanyId,
            [Action],
            ChangedFields,
            UserId,
            UserName
        )
        VALUES
        (
            @CompanyId,
            @Action,
            N'IsEnabled,IntegrationMode,ServiceLayerUrl,SapCompanyDb,SapUser,PasswordPresence,MaxRetryCount',
            @UpdatedByUserId,
            @UpdatedByUserName
        );

        COMMIT TRANSACTION;
        SELECT @SettingsId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260716.01')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260716.01', N'Configuracion segura SAP Service Layer por empresa');
END;
GO
