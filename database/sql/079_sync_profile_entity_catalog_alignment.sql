/*
    Alinea la persistencia de perfiles con el catalogo de entidades expuesto
    por Application y habilitado por el routing Master-Sucursal.

    Corrige instalaciones donde SP_NA_PUT_SYNCPROFILEACTUALIZAR conserva el
    catalogo inicial de diez entidades y rechaza BusinessPartner, Item o Warehouse.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
BEGIN
    THROW 51079, 'Este script debe ejecutarse en NuanSystem_Master.', 1;
END;
GO

IF OBJECT_ID(N'dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR', N'P') IS NULL
BEGIN
    THROW 51080, 'No existe dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR. Ejecute primero 069_sync_master_branch_configuration.sql.', 1;
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
        IF EXISTS (SELECT 1 FROM @Entities entity WHERE entity.EntityCode NOT IN (N'Countries', N'Provinces', N'Cities', N'Currencies', N'BusinessPartnerPaymentTerms', N'SupplierGroups', N'SupplierClasses', N'EconomicActivities', N'Zones', N'SupplyMethods', N'BusinessPartner', N'ItemGroups', N'Item', N'Warehouse'))
            THROW 51004, 'Una entidad no pertenece al catalogo inicial permitido.', 1;
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

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260715.079')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description, AppliedAt)
    VALUES (N'20260715.079', N'Alineacion del catalogo de entidades para persistencia de perfiles Sync', SYSUTCDATETIME());
END;
GO
