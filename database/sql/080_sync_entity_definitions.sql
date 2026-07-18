/*
    Fase 1 del CRUD de definiciones de entidades de sincronizacion.

    Base propietaria: NuanSystem_Master.
    Responsabilidad: catalogo global administrable, dependencias y contratos
    SQL para Persistence. No crea endpoints ni formularios.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
BEGIN
    THROW 51100, 'Este script debe ejecutarse en NuanSystem_Master.', 1;
END;
GO

IF OBJECT_ID(N'dbo.SyncProfileEntities', N'U') IS NULL
BEGIN
    THROW 51101, 'No existe dbo.SyncProfileEntities. Ejecute primero 069_sync_master_branch_configuration.sql.', 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR
    @Id int,
    @CompanyId int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @Direction nvarchar(30),
    @ExecutionMode nvarchar(20),
    @ConflictStrategy nvarchar(30),
    @BatchSize int,
    @MaxRetries int,
    @RetryDelaySeconds int,
    @TimeoutMinutes int,
    @IsActive bit,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL,
    @BranchesJson nvarchar(max) = N'[]',
    @EntitiesJson nvarchar(max) = N'[]',
    @EntityBranchesJson nvarchar(max) = N'[]',
    @ScheduleJson nvarchar(max) = NULL,
    @SuppressResult bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.SyncProfiles WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        IF @SuppressResult = 0
            SELECT 0;
    END
    ELSE
    BEGIN
        DECLARE @Branches table (BranchCompanyId int NOT NULL PRIMARY KEY, BatchSize int NULL, MaxRetries int NULL, IsActive bit NOT NULL);
        DECLARE @Entities table (EntityCode nvarchar(80) NOT NULL PRIMARY KEY, EntityName nvarchar(120) NOT NULL, ExecutionOrder int NOT NULL, SyncMode nvarchar(20) NOT NULL, KeyField nvarchar(100) NULL, ModifiedAtField nvarchar(100) NULL, VersionField nvarchar(100) NULL, ActiveField nvarchar(100) NULL, AllowInsert bit NOT NULL, AllowUpdate bit NOT NULL, AllowDeactivate bit NOT NULL, ContinueOnError bit NOT NULL, BatchSize int NULL, IsActive bit NOT NULL);
        DECLARE @Matrix table (EntityCode nvarchar(80) NOT NULL, BranchCompanyId int NOT NULL, IsEnabled bit NOT NULL, BatchSize int NULL, PRIMARY KEY (EntityCode, BranchCompanyId));

        INSERT INTO @Branches
        SELECT BranchCompanyId, BatchSize, MaxRetries, IsActive
        FROM OPENJSON(ISNULL(@BranchesJson, N'[]'))
        WITH (BranchCompanyId int '$.branchCompanyId', BatchSize int '$.batchSize', MaxRetries int '$.maxRetries', IsActive bit '$.isActive');

        INSERT INTO @Entities
        SELECT EntityCode, EntityName, ExecutionOrder, SyncMode, KeyField, ModifiedAtField, VersionField, ActiveField, AllowInsert, AllowUpdate, AllowDeactivate, ContinueOnError, BatchSize, IsActive
        FROM OPENJSON(ISNULL(@EntitiesJson, N'[]'))
        WITH (EntityCode nvarchar(80) '$.entityCode', EntityName nvarchar(120) '$.entityName', ExecutionOrder int '$.executionOrder', SyncMode nvarchar(20) '$.syncMode', KeyField nvarchar(100) '$.keyField', ModifiedAtField nvarchar(100) '$.modifiedAtField', VersionField nvarchar(100) '$.versionField', ActiveField nvarchar(100) '$.activeField', AllowInsert bit '$.allowInsert', AllowUpdate bit '$.allowUpdate', AllowDeactivate bit '$.allowDeactivate', ContinueOnError bit '$.continueOnError', BatchSize int '$.batchSize', IsActive bit '$.isActive');

        INSERT INTO @Matrix
        SELECT EntityCode, BranchCompanyId, IsEnabled, BatchSize
        FROM OPENJSON(ISNULL(@EntityBranchesJson, N'[]'))
        WITH (EntityCode nvarchar(80) '$.entityCode', BranchCompanyId int '$.branchCompanyId', IsEnabled bit '$.isEnabled', BatchSize int '$.batchSize');

        IF EXISTS (SELECT 1 FROM @Branches branch WHERE NOT EXISTS (SELECT 1 FROM dbo.Companies company WHERE company.Id = branch.BranchCompanyId AND company.ParentCompanyId = @CompanyId AND company.IsMaster = 0 AND company.SyncEnabled = 1 AND company.IsDeleted = 0))
            THROW 51003, 'Una sucursal no pertenece a la empresa maestra o no tiene sincronizacion habilitada.', 1;
        IF EXISTS
        (
            SELECT 1
            FROM @Entities entity
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM dbo.SyncEntityDefinitions definition
                WHERE definition.Code = entity.EntityCode
                  AND definition.IsDeleted = 0
                  AND (definition.IsActive = 1 OR entity.IsActive = 0)
            )
        )
            THROW 51004, 'Una entidad no existe en el catalogo o esta inactiva.', 1;
        IF EXISTS (SELECT 1 FROM @Matrix matrix WHERE NOT EXISTS (SELECT 1 FROM @Entities entity WHERE entity.EntityCode = matrix.EntityCode) OR NOT EXISTS (SELECT 1 FROM @Branches branch WHERE branch.BranchCompanyId = matrix.BranchCompanyId))
            THROW 51005, 'La matriz entidad-sucursal referencia una entidad o sucursal no incluida en el perfil.', 1;

        BEGIN TRANSACTION;
            UPDATE dbo.SyncProfiles
            SET CompanyId = @CompanyId,
                Code = @Code,
                Name = @Name,
                Description = @Description,
                Direction = @Direction,
                ExecutionMode = @ExecutionMode,
                ConflictStrategy = @ConflictStrategy,
                BatchSize = @BatchSize,
                MaxRetries = @MaxRetries,
                RetryDelaySeconds = @RetryDelaySeconds,
                TimeoutMinutes = @TimeoutMinutes,
                IsActive = @IsActive,
                UpdatedByUserId = @AuditUserId,
                UpdatedByUserName = @AuditUserName,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @Id AND IsDeleted = 0;

            UPDATE branch
            SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
            FROM dbo.SyncProfileBranches branch
            WHERE branch.SyncProfileId = @Id AND branch.IsDeleted = 0
              AND NOT EXISTS (SELECT 1 FROM @Branches source WHERE source.BranchCompanyId = branch.BranchCompanyId);

            MERGE dbo.SyncProfileBranches AS target
            USING @Branches AS source
            ON target.SyncProfileId = @Id AND target.BranchCompanyId = source.BranchCompanyId
            WHEN MATCHED THEN UPDATE SET BatchSize = source.BatchSize, MaxRetries = source.MaxRetries, IsActive = source.IsActive, IsDeleted = 0, UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME(), DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL
            WHEN NOT MATCHED THEN INSERT (SyncProfileId, BranchCompanyId, BatchSize, MaxRetries, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Id, source.BranchCompanyId, source.BatchSize, source.MaxRetries, source.IsActive, @AuditUserId, @AuditUserName);

            UPDATE entity
            SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
            FROM dbo.SyncProfileEntities entity
            WHERE entity.SyncProfileId = @Id AND entity.IsDeleted = 0
              AND NOT EXISTS (SELECT 1 FROM @Entities source WHERE source.EntityCode = entity.EntityCode);

            MERGE dbo.SyncProfileEntities AS target
            USING @Entities AS source
            ON target.SyncProfileId = @Id AND target.EntityCode = source.EntityCode
            WHEN MATCHED THEN UPDATE SET EntityName = source.EntityName, ExecutionOrder = source.ExecutionOrder, SyncMode = source.SyncMode, KeyField = source.KeyField, ModifiedAtField = source.ModifiedAtField, VersionField = source.VersionField, ActiveField = source.ActiveField, AllowInsert = source.AllowInsert, AllowUpdate = source.AllowUpdate, AllowDeactivate = source.AllowDeactivate, ContinueOnError = source.ContinueOnError, BatchSize = source.BatchSize, IsActive = source.IsActive, IsDeleted = 0, UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME(), DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL
            WHEN NOT MATCHED THEN INSERT (SyncProfileId, EntityCode, EntityName, ExecutionOrder, SyncMode, KeyField, ModifiedAtField, VersionField, ActiveField, AllowInsert, AllowUpdate, AllowDeactivate, ContinueOnError, BatchSize, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Id, source.EntityCode, source.EntityName, source.ExecutionOrder, source.SyncMode, source.KeyField, source.ModifiedAtField, source.VersionField, source.ActiveField, source.AllowInsert, source.AllowUpdate, source.AllowDeactivate, source.ContinueOnError, source.BatchSize, source.IsActive, @AuditUserId, @AuditUserName);

            UPDATE map
            SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
            FROM dbo.SyncProfileEntityBranches map
            INNER JOIN dbo.SyncProfileEntities entity ON entity.Id = map.SyncProfileEntityId
            INNER JOIN dbo.SyncProfileBranches branch ON branch.Id = map.SyncProfileBranchId
            WHERE map.SyncProfileId = @Id AND map.IsDeleted = 0
              AND NOT EXISTS (SELECT 1 FROM @Matrix source WHERE source.EntityCode = entity.EntityCode AND source.BranchCompanyId = branch.BranchCompanyId);

            MERGE dbo.SyncProfileEntityBranches AS target
            USING (
                SELECT entity.Id AS SyncProfileEntityId, branch.Id AS SyncProfileBranchId, matrix.IsEnabled, matrix.BatchSize
                FROM @Matrix matrix
                INNER JOIN dbo.SyncProfileEntities entity ON entity.SyncProfileId = @Id AND entity.EntityCode = matrix.EntityCode AND entity.IsDeleted = 0
                INNER JOIN dbo.SyncProfileBranches branch ON branch.SyncProfileId = @Id AND branch.BranchCompanyId = matrix.BranchCompanyId AND branch.IsDeleted = 0
            ) AS source
            ON target.SyncProfileEntityId = source.SyncProfileEntityId AND target.SyncProfileBranchId = source.SyncProfileBranchId
            WHEN MATCHED THEN UPDATE SET IsEnabled = source.IsEnabled, BatchSize = source.BatchSize, SyncProfileId = @Id, IsDeleted = 0, UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME(), DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL
            WHEN NOT MATCHED THEN INSERT (SyncProfileId, SyncProfileEntityId, SyncProfileBranchId, IsEnabled, BatchSize, CreatedByUserId, CreatedByUserName) VALUES (@Id, source.SyncProfileEntityId, source.SyncProfileBranchId, source.IsEnabled, source.BatchSize, @AuditUserId, @AuditUserName);

            IF @ScheduleJson IS NULL
            BEGIN
                UPDATE dbo.SyncSchedules
                SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
                WHERE SyncProfileId = @Id AND IsDeleted = 0;
            END
            ELSE
            BEGIN
                DECLARE @ScheduleType nvarchar(20), @IntervalMinutes int, @ExecutionTime time(0), @TimeZoneId nvarchar(100), @PreventConcurrentExecutions bit, @ScheduleIsActive bit;
                SELECT @ScheduleType = ScheduleType, @IntervalMinutes = IntervalMinutes, @ExecutionTime = TRY_CONVERT(time(0), ExecutionTime), @TimeZoneId = ISNULL(NULLIF(TimeZoneId, N''), N'America/Guayaquil'), @PreventConcurrentExecutions = PreventConcurrentExecutions, @ScheduleIsActive = IsActive
                FROM OPENJSON(@ScheduleJson)
                WITH (ScheduleType nvarchar(20) '$.scheduleType', IntervalMinutes int '$.intervalMinutes', ExecutionTime nvarchar(8) '$.executionTime', TimeZoneId nvarchar(100) '$.timeZoneId', PreventConcurrentExecutions bit '$.preventConcurrentExecutions', IsActive bit '$.isActive');

                UPDATE dbo.SyncSchedules SET IsDeleted = 1, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME() WHERE SyncProfileId = @Id AND IsDeleted = 0;
                INSERT INTO dbo.SyncSchedules (SyncProfileId, ScheduleType, IntervalMinutes, ExecutionTime, TimeZoneId, PreventConcurrentExecutions, IsActive, CreatedByUserId, CreatedByUserName)
                VALUES (@Id, @ScheduleType, @IntervalMinutes, @ExecutionTime, @TimeZoneId, @PreventConcurrentExecutions, @ScheduleIsActive, @AuditUserId, @AuditUserName);
            END;
        COMMIT TRANSACTION;

        IF @SuppressResult = 0
            SELECT 1;
    END;
END;
GO

IF OBJECT_ID(N'dbo.SyncProfileEntities', N'U') IS NULL
BEGIN
    THROW 51101, 'No existe dbo.SyncProfileEntities. Ejecute primero 069_sync_master_branch_configuration.sql.', 1;
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncEntityDefinitions
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncEntityDefinitions PRIMARY KEY,
        Code nvarchar(80) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Description nvarchar(500) NULL,
        DefaultExecutionOrder int NOT NULL CONSTRAINT DF_SyncEntityDefinitions_DefaultExecutionOrder DEFAULT 100,
        SupportsIncremental bit NOT NULL CONSTRAINT DF_SyncEntityDefinitions_SupportsIncremental DEFAULT 1,
        SupportsInsert bit NOT NULL CONSTRAINT DF_SyncEntityDefinitions_SupportsInsert DEFAULT 0,
        SupportsUpdate bit NOT NULL CONSTRAINT DF_SyncEntityDefinitions_SupportsUpdate DEFAULT 0,
        SupportsDeactivate bit NOT NULL CONSTRAINT DF_SyncEntityDefinitions_SupportsDeactivate DEFAULT 0,
        DefaultKeyField nvarchar(100) NULL,
        DefaultModifiedAtField nvarchar(100) NULL,
        IsSystem bit NOT NULL CONSTRAINT DF_SyncEntityDefinitions_IsSystem DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_SyncEntityDefinitions_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncEntityDefinitions_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SyncEntityDefinitions_IsDeleted DEFAULT 0,
        CONSTRAINT UQ_SyncEntityDefinitions_Code UNIQUE (Code),
        CONSTRAINT CK_SyncEntityDefinitions_Code CHECK (LEN(LTRIM(RTRIM(Code))) BETWEEN 1 AND 80),
        CONSTRAINT CK_SyncEntityDefinitions_Name CHECK (LEN(LTRIM(RTRIM(Name))) BETWEEN 1 AND 120),
        CONSTRAINT CK_SyncEntityDefinitions_DefaultExecutionOrder CHECK (DefaultExecutionOrder >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncEntityDefinitionDependencies
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncEntityDefinitionDependencies PRIMARY KEY,
        EntityDefinitionId int NOT NULL,
        DependsOnEntityDefinitionId int NOT NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncEntityDefinitionDependencies_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SyncEntityDefinitionDependencies_IsDeleted DEFAULT 0,
        CONSTRAINT FK_SyncEntityDefinitionDependencies_Entity FOREIGN KEY (EntityDefinitionId) REFERENCES dbo.SyncEntityDefinitions(Id),
        CONSTRAINT FK_SyncEntityDefinitionDependencies_DependsOn FOREIGN KEY (DependsOnEntityDefinitionId) REFERENCES dbo.SyncEntityDefinitions(Id),
        CONSTRAINT CK_SyncEntityDefinitionDependencies_NotSelf CHECK (EntityDefinitionId <> DependsOnEntityDefinitionId)
    );
END;
GO

IF OBJECT_ID(N'dbo.AuditSyncConfigurationChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSyncConfigurationChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSyncConfigurationChanges PRIMARY KEY,
        EntityName nvarchar(120) NOT NULL,
        RecordId nvarchar(80) NOT NULL,
        [Action] nvarchar(20) NOT NULL,
        FieldName nvarchar(120) NOT NULL,
        OldValue nvarchar(max) NULL,
        NewValue nvarchar(max) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        [Source] nvarchar(60) NOT NULL CONSTRAINT DF_AuditSyncConfigurationChanges_Source DEFAULT N'API',
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditSyncConfigurationChanges_CreatedAt DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_SyncEntityDefinitionDependencies_Active'
      AND object_id = OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies')
)
BEGIN
    CREATE UNIQUE INDEX UX_SyncEntityDefinitionDependencies_Active
        ON dbo.SyncEntityDefinitionDependencies (EntityDefinitionId, DependsOnEntityDefinitionId)
        WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_SyncEntityDefinitions_List'
      AND object_id = OBJECT_ID(N'dbo.SyncEntityDefinitions')
)
BEGIN
    CREATE INDEX IX_SyncEntityDefinitions_List
        ON dbo.SyncEntityDefinitions (IsDeleted, IsActive, DefaultExecutionOrder, Name)
        INCLUDE (Code, IsSystem, SupportsIncremental, SupportsInsert, SupportsUpdate, SupportsDeactivate);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_AuditSyncConfigurationChanges_Record'
      AND object_id = OBJECT_ID(N'dbo.AuditSyncConfigurationChanges')
)
BEGIN
    CREATE INDEX IX_AuditSyncConfigurationChanges_Record
        ON dbo.AuditSyncConfigurationChanges (EntityName, RecordId, CreatedAt DESC);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_AuditSyncConfigurationChanges_User'
      AND object_id = OBJECT_ID(N'dbo.AuditSyncConfigurationChanges')
)
BEGIN
    CREATE INDEX IX_AuditSyncConfigurationChanges_User
        ON dbo.AuditSyncConfigurationChanges (UserId, CreatedAt DESC);
END;
GO

DECLARE @Definitions TABLE
(
    Code nvarchar(80) NOT NULL,
    Name nvarchar(120) NOT NULL,
    Description nvarchar(500) NULL,
    DefaultExecutionOrder int NOT NULL,
    SupportsIncremental bit NOT NULL,
    SupportsInsert bit NOT NULL,
    SupportsUpdate bit NOT NULL,
    SupportsDeactivate bit NOT NULL,
    DefaultKeyField nvarchar(100) NULL,
    DefaultModifiedAtField nvarchar(100) NULL
);

INSERT INTO @Definitions
(
    Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
    SupportsInsert, SupportsUpdate, SupportsDeactivate, DefaultKeyField, DefaultModifiedAtField
)
VALUES
    (N'Countries', N'Paises', N'Catalogo geografico. Sin productor/aplicador Master-Branch operativo.', 10, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'Provinces', N'Provincias', N'Catalogo geografico dependiente de paises. Sin productor/aplicador operativo.', 20, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'Cities', N'Ciudades', N'Catalogo geografico dependiente de paises y provincias. Sin productor/aplicador operativo.', 30, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'Currencies', N'Monedas', N'Catalogo comercial. Sin productor/aplicador Master-Branch operativo.', 40, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'BusinessPartnerPaymentTerms', N'Condiciones de pago', N'Catalogo de socios de negocio. Sin productor/aplicador operativo.', 50, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'SupplierGroups', N'Grupos de proveedor', N'Catalogo general de proveedores. Sin productor/aplicador operativo.', 60, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'SupplierClasses', N'Clases de proveedor', N'Catalogo general de proveedores. Sin productor/aplicador operativo.', 70, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'EconomicActivities', N'Actividades economicas', N'Catalogo general de proveedores. Sin productor/aplicador operativo.', 80, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'Zones', N'Zonas', N'Catalogo general de proveedores. Sin productor/aplicador operativo.', 90, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'SupplyMethods', N'Metodos de abastecimiento', N'Catalogo general de proveedores. Sin productor/aplicador operativo.', 100, 1, 0, 0, 0, N'Code', N'UpdatedAt'),
    (N'BusinessPartner', N'Socios de negocio', N'Productor y aplicador Master-Branch existentes con alcance maestro limitado.', 200, 1, 1, 1, 1, N'Code', N'UpdatedAt'),
    (N'ItemGroups', N'Grupos de articulos', N'Catalogo maestro con productor incremental, fuente Full y aplicador idempotente por GlobalId.', 205, 1, 1, 1, 1, N'Code', N'UpdatedAt'),
    (N'Item', N'Articulos', N'Productor y aplicador Master-Branch existentes con alcance maestro limitado.', 210, 1, 1, 1, 1, N'Code', N'UpdatedAt'),
    (N'Warehouse', N'Almacenes', N'Productor y aplicador Warehouse Master-Branch existentes.', 220, 1, 1, 1, 1, N'Code', N'UpdatedAt');

INSERT INTO dbo.SyncEntityDefinitions
(
    Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
    SupportsInsert, SupportsUpdate, SupportsDeactivate, DefaultKeyField,
    DefaultModifiedAtField, IsSystem, IsActive, CreatedByUserName
)
SELECT
    source.Code, source.Name, source.Description, source.DefaultExecutionOrder,
    source.SupportsIncremental, source.SupportsInsert, source.SupportsUpdate,
    source.SupportsDeactivate, source.DefaultKeyField, source.DefaultModifiedAtField,
    1, 1, N'Seed Sync Entity Definitions'
FROM @Definitions source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SyncEntityDefinitions target
    WHERE target.Code = source.Code
);
GO

DECLARE @SeedDependencies TABLE (EntityCode nvarchar(80) NOT NULL, DependsOnCode nvarchar(80) NOT NULL);
INSERT INTO @SeedDependencies (EntityCode, DependsOnCode)
VALUES
    (N'Provinces', N'Countries'),
    (N'Cities', N'Countries'),
    (N'Cities', N'Provinces'),
    (N'Item', N'ItemGroups');

INSERT INTO dbo.SyncEntityDefinitionDependencies
(
    EntityDefinitionId, DependsOnEntityDefinitionId, CreatedByUserName
)
SELECT
    entity.Id,
    dependency.Id,
    N'Seed Sync Entity Definitions'
FROM @SeedDependencies source
INNER JOIN dbo.SyncEntityDefinitions entity
    ON entity.Code = source.EntityCode AND entity.IsDeleted = 0
INNER JOIN dbo.SyncEntityDefinitions dependency
    ON dependency.Code = source.DependsOnCode AND dependency.IsDeleted = 0
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SyncEntityDefinitionDependencies existing
    WHERE existing.EntityDefinitionId = entity.Id
      AND existing.DependsOnEntityDefinitionId = dependency.Id
      AND existing.IsDeleted = 0
);
GO

IF OBJECT_ID(N'dbo.CK_SyncProfileEntities_EntityCode', N'C') IS NOT NULL
BEGIN
    ALTER TABLE dbo.SyncProfileEntities
        DROP CONSTRAINT CK_SyncProfileEntities_EntityCode;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_SyncProfileEntities_EntityDefinition'
      AND parent_object_id = OBJECT_ID(N'dbo.SyncProfileEntities')
)
BEGIN
    ALTER TABLE dbo.SyncProfileEntities WITH CHECK
        ADD CONSTRAINT FK_SyncProfileEntities_EntityDefinition
            FOREIGN KEY (EntityCode) REFERENCES dbo.SyncEntityDefinitions(Code);

    ALTER TABLE dbo.SyncProfileEntities
        CHECK CONSTRAINT FK_SyncProfileEntities_EntityDefinition;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCENTITYDEFINITIONPAGINAR
    @Search nvarchar(200) = NULL,
    @IsActive bit = NULL,
    @PageNumber int = 1,
    @PageSize int = 50
AS
BEGIN
    SET NOCOUNT ON;

    SET @PageNumber = CASE WHEN @PageNumber < 1 THEN 1 ELSE @PageNumber END;
    SET @PageSize = CASE WHEN @PageSize < 1 OR @PageSize > 500 THEN 50 ELSE @PageSize END;
    DECLARE @Offset int = (@PageNumber - 1) * @PageSize;
    DECLARE @Pattern nvarchar(202) = CASE WHEN NULLIF(LTRIM(RTRIM(@Search)), N'') IS NULL THEN NULL ELSE N'%' + LTRIM(RTRIM(@Search)) + N'%' END;

    SELECT
        definition.Id,
        definition.Code,
        definition.Name,
        definition.Description,
        definition.DefaultExecutionOrder,
        definition.SupportsIncremental,
        definition.SupportsInsert,
        definition.SupportsUpdate,
        definition.SupportsDeactivate,
        definition.DefaultKeyField,
        definition.DefaultModifiedAtField,
        definition.IsSystem,
        definition.IsActive,
        (SELECT COUNT(1) FROM dbo.SyncEntityDefinitionDependencies dependency WHERE dependency.EntityDefinitionId = definition.Id AND dependency.IsDeleted = 0) AS DependencyCount,
        CONVERT(bit, CASE WHEN EXISTS (SELECT 1 FROM dbo.SyncProfileEntities profileEntity WHERE profileEntity.EntityCode = definition.Code AND profileEntity.IsDeleted = 0) THEN 1 ELSE 0 END) AS IsInUse,
        definition.CreatedByUserId,
        definition.CreatedByUserName,
        definition.CreatedAt,
        definition.UpdatedByUserId,
        definition.UpdatedByUserName,
        definition.UpdatedAt
    FROM dbo.SyncEntityDefinitions definition
    WHERE definition.IsDeleted = 0
      AND (@IsActive IS NULL OR definition.IsActive = @IsActive)
      AND (@Pattern IS NULL OR definition.Code LIKE @Pattern OR definition.Name LIKE @Pattern OR definition.Description LIKE @Pattern)
    ORDER BY definition.DefaultExecutionOrder, definition.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.SyncEntityDefinitions definition
    WHERE definition.IsDeleted = 0
      AND (@IsActive IS NULL OR definition.IsActive = @IsActive)
      AND (@Pattern IS NULL OR definition.Code LIKE @Pattern OR definition.Name LIKE @Pattern OR definition.Description LIKE @Pattern);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCENTITYDEFINITIONLISTAR
    @IsActive bit = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, Code, Name, Description, DefaultExecutionOrder,
        SupportsIncremental, SupportsInsert, SupportsUpdate, SupportsDeactivate,
        DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.SyncEntityDefinitions
    WHERE IsDeleted = 0
      AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY DefaultExecutionOrder, Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCENTITYDEFINITIONBUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, Code, Name, Description, DefaultExecutionOrder,
        SupportsIncremental, SupportsInsert, SupportsUpdate, SupportsDeactivate,
        DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.SyncEntityDefinitions
    WHERE Id = @Id AND IsDeleted = 0;

    SELECT
        dependency.Id,
        dependency.DependsOnEntityDefinitionId AS DependencyDefinitionId,
        required.Code AS DependencyCode,
        required.Name AS DependencyName
    FROM dbo.SyncEntityDefinitionDependencies dependency
    INNER JOIN dbo.SyncEntityDefinitions required
        ON required.Id = dependency.DependsOnEntityDefinitionId
       AND required.IsDeleted = 0
    WHERE dependency.EntityDefinitionId = @Id
      AND dependency.IsDeleted = 0
    ORDER BY required.DefaultExecutionOrder, required.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCENTITYDEFINITIONBUSCARPORCODIGO
    @Code nvarchar(80)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id int =
    (
        SELECT Id
        FROM dbo.SyncEntityDefinitions
        WHERE Code = LTRIM(RTRIM(@Code)) AND IsDeleted = 0
    );

    EXEC dbo.SP_NA_GET_SYNCENTITYDEFINITIONBUSCARPORID @Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCENTITYDEFINITIONLOOKUP
    @IncludeId int = NULL,
    @IncludeInactive bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, Code, Name, Description, DefaultExecutionOrder,
        SupportsIncremental, SupportsInsert, SupportsUpdate, SupportsDeactivate,
        DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive
    FROM dbo.SyncEntityDefinitions
    WHERE IsDeleted = 0
      AND (@IncludeInactive = 1 OR IsActive = 1 OR Id = @IncludeId)
    ORDER BY DefaultExecutionOrder, Name;

    SELECT
        dependency.EntityDefinitionId,
        dependency.DependsOnEntityDefinitionId AS DependencyDefinitionId,
        required.Code AS DependencyCode,
        required.Name AS DependencyName
    FROM dbo.SyncEntityDefinitionDependencies dependency
    INNER JOIN dbo.SyncEntityDefinitions definition
        ON definition.Id = dependency.EntityDefinitionId
       AND definition.IsDeleted = 0
    INNER JOIN dbo.SyncEntityDefinitions required
        ON required.Id = dependency.DependsOnEntityDefinitionId
       AND required.IsDeleted = 0
    WHERE dependency.IsDeleted = 0
      AND (@IncludeInactive = 1 OR definition.IsActive = 1 OR definition.Id = @IncludeId)
    ORDER BY dependency.EntityDefinitionId, required.DefaultExecutionOrder, required.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCENTITYDEFINITIONTIENECICLO
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasCycle bit = 0;

    ;WITH DependencyPaths AS
    (
        SELECT
            dependency.EntityDefinitionId AS RootId,
            dependency.DependsOnEntityDefinitionId AS CurrentId,
            CAST(N'/' + CONVERT(nvarchar(20), dependency.EntityDefinitionId) + N'/' + CONVERT(nvarchar(20), dependency.DependsOnEntityDefinitionId) + N'/' AS nvarchar(max)) AS VisitedPath,
            CONVERT(bit, CASE WHEN dependency.EntityDefinitionId = dependency.DependsOnEntityDefinitionId THEN 1 ELSE 0 END) AS HasCycle
        FROM dbo.SyncEntityDefinitionDependencies dependency
        WHERE dependency.IsDeleted = 0

        UNION ALL

        SELECT
            path.RootId,
            dependency.DependsOnEntityDefinitionId,
            CAST(path.VisitedPath + CONVERT(nvarchar(20), dependency.DependsOnEntityDefinitionId) + N'/' AS nvarchar(max)),
            CONVERT(bit, CASE WHEN path.VisitedPath LIKE N'%/' + CONVERT(nvarchar(20), dependency.DependsOnEntityDefinitionId) + N'/%' THEN 1 ELSE 0 END)
        FROM DependencyPaths path
        INNER JOIN dbo.SyncEntityDefinitionDependencies dependency
            ON dependency.EntityDefinitionId = path.CurrentId
           AND dependency.IsDeleted = 0
        WHERE path.HasCycle = 0
    )
    SELECT TOP (1) @HasCycle = 1
    FROM DependencyPaths
    WHERE RootId = @Id AND HasCycle = 1
    OPTION (MAXRECURSION 32767);

    SELECT CONVERT(int, @HasCycle);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SYNCENTITYDEFINITIONCREAR
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(500) = NULL,
    @DefaultExecutionOrder int,
    @SupportsIncremental bit,
    @SupportsInsert bit,
    @SupportsUpdate bit,
    @SupportsDeactivate bit,
    @DefaultKeyField nvarchar(100) = NULL,
    @DefaultModifiedAtField nvarchar(100) = NULL,
    @IsActive bit,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL,
    @DependenciesJson nvarchar(max) = N'[]'
AS
BEGIN
    SET NOCOUNT ON;

    SET @Code = LTRIM(RTRIM(@Code));
    SET @Name = LTRIM(RTRIM(@Name));

    IF NULLIF(@Code, N'') IS NULL OR NULLIF(@Name, N'') IS NULL
        THROW 51102, 'El codigo y el nombre de la entidad son obligatorios.', 1;
    IF EXISTS (SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code = @Code)
        THROW 51103, 'Ya existe una definicion de entidad con el mismo codigo.', 1;

    DECLARE @Dependencies table (DependencyDefinitionId int NOT NULL PRIMARY KEY);
    INSERT INTO @Dependencies (DependencyDefinitionId)
    SELECT DependencyDefinitionId
    FROM OPENJSON(ISNULL(@DependenciesJson, N'[]'))
    WITH (DependencyDefinitionId int '$.dependencyDefinitionId')
    WHERE DependencyDefinitionId IS NOT NULL;

    IF EXISTS
    (
        SELECT 1
        FROM @Dependencies source
        WHERE NOT EXISTS
        (
            SELECT 1 FROM dbo.SyncEntityDefinitions dependency
            WHERE dependency.Id = source.DependencyDefinitionId
              AND dependency.IsDeleted = 0
              AND dependency.IsActive = 1
        )
    )
        THROW 51104, 'Una dependencia no existe o esta inactiva.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.SyncEntityDefinitions
        (
            Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
            SupportsInsert, SupportsUpdate, SupportsDeactivate, DefaultKeyField,
            DefaultModifiedAtField, IsSystem, IsActive, CreatedByUserId, CreatedByUserName
        )
        VALUES
        (
            @Code, @Name, NULLIF(LTRIM(RTRIM(@Description)), N''), @DefaultExecutionOrder,
            @SupportsIncremental, @SupportsInsert, @SupportsUpdate, @SupportsDeactivate,
            NULLIF(LTRIM(RTRIM(@DefaultKeyField)), N''),
            NULLIF(LTRIM(RTRIM(@DefaultModifiedAtField)), N''),
            0, @IsActive, @AuditUserId, @AuditUserName
        );

        DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());

        INSERT INTO dbo.SyncEntityDefinitionDependencies
        (
            EntityDefinitionId, DependsOnEntityDefinitionId,
            CreatedByUserId, CreatedByUserName
        )
        SELECT @Id, source.DependencyDefinitionId, @AuditUserId, @AuditUserName
        FROM @Dependencies source;

        DECLARE @Cycle table (HasCycle int NOT NULL);
        INSERT INTO @Cycle EXEC dbo.SP_NA_GET_SYNCENTITYDEFINITIONTIENECICLO @Id = @Id;
        IF EXISTS (SELECT 1 FROM @Cycle WHERE HasCycle = 1)
            THROW 51105, 'Las dependencias forman un ciclo.', 1;

        DECLARE @NewValue nvarchar(max) =
        (
            SELECT Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
                   SupportsInsert, SupportsUpdate, SupportsDeactivate,
                   DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive
            FROM dbo.SyncEntityDefinitions
            WHERE Id = @Id
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        INSERT INTO dbo.AuditSyncConfigurationChanges
        (
            EntityName, RecordId, [Action], FieldName, OldValue, NewValue,
            UserId, UserName, [Source]
        )
        VALUES
        (
            N'SyncEntityDefinitions', CONVERT(nvarchar(80), @Id), N'Create', N'Definition',
            NULL, @NewValue, @AuditUserId, @AuditUserName, N'API'
        );

        COMMIT TRANSACTION;
        SELECT @Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SYNCENTITYDEFINITIONACTUALIZAR
    @Id int,
    @Name nvarchar(120),
    @Description nvarchar(500) = NULL,
    @DefaultExecutionOrder int,
    @SupportsIncremental bit,
    @SupportsInsert bit,
    @SupportsUpdate bit,
    @SupportsDeactivate bit,
    @DefaultKeyField nvarchar(100) = NULL,
    @DefaultModifiedAtField nvarchar(100) = NULL,
    @IsActive bit,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL,
    @DependenciesJson nvarchar(max) = N'[]'
AS
BEGIN
    SET NOCOUNT ON;

    SET @Name = LTRIM(RTRIM(@Name));
    IF NULLIF(@Name, N'') IS NULL
        THROW 51106, 'El nombre de la entidad es obligatorio.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        SELECT 0;
        RETURN;
    END;

    DECLARE @Dependencies table (DependencyDefinitionId int NOT NULL PRIMARY KEY);
    INSERT INTO @Dependencies (DependencyDefinitionId)
    SELECT DependencyDefinitionId
    FROM OPENJSON(ISNULL(@DependenciesJson, N'[]'))
    WITH (DependencyDefinitionId int '$.dependencyDefinitionId')
    WHERE DependencyDefinitionId IS NOT NULL;

    IF EXISTS (SELECT 1 FROM @Dependencies WHERE DependencyDefinitionId = @Id)
        THROW 51107, 'Una entidad no puede depender de si misma.', 1;
    IF EXISTS
    (
        SELECT 1
        FROM @Dependencies source
        WHERE NOT EXISTS
        (
            SELECT 1 FROM dbo.SyncEntityDefinitions dependency
            WHERE dependency.Id = source.DependencyDefinitionId
              AND dependency.IsDeleted = 0
              AND dependency.IsActive = 1
        )
    )
        THROW 51104, 'Una dependencia no existe o esta inactiva.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OldValue nvarchar(max) =
        (
            SELECT Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
                   SupportsInsert, SupportsUpdate, SupportsDeactivate,
                   DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive
            FROM dbo.SyncEntityDefinitions
            WHERE Id = @Id
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        UPDATE dbo.SyncEntityDefinitions
        SET Name = @Name,
            Description = NULLIF(LTRIM(RTRIM(@Description)), N''),
            DefaultExecutionOrder = @DefaultExecutionOrder,
            SupportsIncremental = @SupportsIncremental,
            SupportsInsert = @SupportsInsert,
            SupportsUpdate = @SupportsUpdate,
            SupportsDeactivate = @SupportsDeactivate,
            DefaultKeyField = NULLIF(LTRIM(RTRIM(@DefaultKeyField)), N''),
            DefaultModifiedAtField = NULLIF(LTRIM(RTRIM(@DefaultModifiedAtField)), N''),
            IsActive = @IsActive,
            UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @Id AND IsDeleted = 0;

        UPDATE dependency
        SET IsDeleted = 1,
            DeletedByUserId = @AuditUserId,
            DeletedByUserName = @AuditUserName,
            DeletedAt = SYSUTCDATETIME()
        FROM dbo.SyncEntityDefinitionDependencies dependency
        WHERE dependency.EntityDefinitionId = @Id
          AND dependency.IsDeleted = 0
          AND NOT EXISTS
          (
              SELECT 1 FROM @Dependencies source
              WHERE source.DependencyDefinitionId = dependency.DependsOnEntityDefinitionId
          );

        MERGE dbo.SyncEntityDefinitionDependencies AS target
        USING @Dependencies AS source
        ON target.EntityDefinitionId = @Id
           AND target.DependsOnEntityDefinitionId = source.DependencyDefinitionId
        WHEN MATCHED THEN
            UPDATE SET IsDeleted = 0,
                       UpdatedByUserId = @AuditUserId,
                       UpdatedByUserName = @AuditUserName,
                       UpdatedAt = SYSUTCDATETIME(),
                       DeletedByUserId = NULL,
                       DeletedByUserName = NULL,
                       DeletedAt = NULL
        WHEN NOT MATCHED THEN
            INSERT (EntityDefinitionId, DependsOnEntityDefinitionId, CreatedByUserId, CreatedByUserName)
            VALUES (@Id, source.DependencyDefinitionId, @AuditUserId, @AuditUserName);

        DECLARE @Cycle table (HasCycle int NOT NULL);
        INSERT INTO @Cycle EXEC dbo.SP_NA_GET_SYNCENTITYDEFINITIONTIENECICLO @Id = @Id;
        IF EXISTS (SELECT 1 FROM @Cycle WHERE HasCycle = 1)
            THROW 51105, 'Las dependencias forman un ciclo.', 1;

        DECLARE @NewValue nvarchar(max) =
        (
            SELECT Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
                   SupportsInsert, SupportsUpdate, SupportsDeactivate,
                   DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive
            FROM dbo.SyncEntityDefinitions
            WHERE Id = @Id
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        INSERT INTO dbo.AuditSyncConfigurationChanges
        (
            EntityName, RecordId, [Action], FieldName, OldValue, NewValue,
            UserId, UserName, [Source]
        )
        VALUES
        (
            N'SyncEntityDefinitions', CONVERT(nvarchar(80), @Id), N'Update', N'Definition',
            @OldValue, @NewValue, @AuditUserId, @AuditUserName, N'API'
        );

        COMMIT TRANSACTION;
        SELECT 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_SYNCENTITYDEFINITIONELIMINAR
    @Id int,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        SELECT 0;
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Id = @Id AND IsSystem = 1 AND IsDeleted = 0)
        THROW 51108, 'Las entidades del sistema no pueden eliminarse; deben desactivarse.', 1;
    IF EXISTS
    (
        SELECT 1
        FROM dbo.SyncProfileEntities profileEntity
        INNER JOIN dbo.SyncEntityDefinitions definition ON definition.Code = profileEntity.EntityCode
        WHERE definition.Id = @Id AND profileEntity.IsDeleted = 0
    )
        THROW 51109, 'La entidad esta referenciada por uno o mas perfiles de sincronizacion.', 1;
    IF EXISTS
    (
        SELECT 1 FROM dbo.SyncEntityDefinitionDependencies dependency
        WHERE dependency.DependsOnEntityDefinitionId = @Id
          AND dependency.IsDeleted = 0
    )
        THROW 51110, 'La entidad es dependencia de otra definicion.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OldValue nvarchar(max) =
        (
            SELECT Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
                   SupportsInsert, SupportsUpdate, SupportsDeactivate,
                   DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive
            FROM dbo.SyncEntityDefinitions
            WHERE Id = @Id
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        UPDATE dbo.SyncEntityDefinitionDependencies
        SET IsDeleted = 1,
            DeletedByUserId = @AuditUserId,
            DeletedByUserName = @AuditUserName,
            DeletedAt = SYSUTCDATETIME()
        WHERE EntityDefinitionId = @Id AND IsDeleted = 0;

        UPDATE dbo.SyncEntityDefinitions
        SET IsDeleted = 1,
            IsActive = 0,
            DeletedByUserId = @AuditUserId,
            DeletedByUserName = @AuditUserName,
            DeletedAt = SYSUTCDATETIME()
        WHERE Id = @Id AND IsDeleted = 0;

        DECLARE @Affected int = @@ROWCOUNT;

        INSERT INTO dbo.AuditSyncConfigurationChanges
        (
            EntityName, RecordId, [Action], FieldName, OldValue, NewValue,
            UserId, UserName, [Source]
        )
        VALUES
        (
            N'SyncEntityDefinitions', CONVERT(nvarchar(80), @Id), N'Delete', N'IsDeleted',
            @OldValue, N'{"isDeleted":true,"isActive":false}',
            @AuditUserId, @AuditUserName, N'API'
        );

        COMMIT TRANSACTION;
        SELECT @Affected;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260715.080')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description, AppliedAt)
    VALUES (N'20260715.080', N'Catalogo administrable de definiciones de entidades Sync', SYSUTCDATETIME());
END;
GO
