/*
    Iteracion 8.9 - Aislamiento por entidad del relay LocalOutbox.

    - Claim y liberacion de leases se limitan a las entidades habilitadas en el worker.
    - Una lista vacia o invalida no muta LocalOutbox.
    - No activa workers, perfiles, rutas ni entidades.

    Ejecutar solo en bases tenant. Nunca en NuanSystem_Master.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() = N'NuanSystem_Master'
    THROW 51164, 'Migration 164 must run only in tenant databases.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL
    THROW 51164, 'LocalOutbox is required before migration 164.', 1;
IF OBJECT_ID(N'dbo.SyncAudit', N'U') IS NULL
    THROW 51164, 'SyncAudit is required before migration 164.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51164, 'SchemaHistory is required before migration 164.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_LOCALOUTBOX_LIBERARLEASESVENCIDOS
    @WorkerInstance nvarchar(120),
    @EnabledEntityNamesJson nvarchar(max) = N'[]'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF ISJSON(@EnabledEntityNamesJson) <> 1
        THROW 51164, 'Enabled entity names must be a valid JSON array.', 1;

    DECLARE @EnabledEntities table(EntityName nvarchar(120) NOT NULL PRIMARY KEY);
    INSERT @EnabledEntities(EntityName)
    SELECT DISTINCT LTRIM(RTRIM(CONVERT(nvarchar(120), [value])))
    FROM OPENJSON(@EnabledEntityNamesJson)
    WHERE [type] = 1 AND NULLIF(LTRIM(RTRIM(CONVERT(nvarchar(120), [value]))), N'') IS NOT NULL;

    IF NOT EXISTS (SELECT 1 FROM @EnabledEntities)
    BEGIN
        SELECT 0;
        RETURN;
    END;

    DECLARE @Now datetime2(0) = SYSUTCDATETIME();
    DECLARE @Released table(Id bigint PRIMARY KEY);

    UPDATE item
    SET Status = CASE WHEN item.AttemptCount >= item.MaxAttempts THEN N'DeadLetter' ELSE N'Error' END,
        NextRetryAt = CASE WHEN item.AttemptCount >= item.MaxAttempts THEN NULL ELSE @Now END,
        LockedBy = NULL,
        LockedAt = NULL,
        LockExpiresAt = NULL,
        LastErrorMessage = N'Lease vencido liberado por relay.'
    OUTPUT inserted.Id INTO @Released(Id)
    FROM dbo.LocalOutbox AS item
    WHERE item.Status = N'InProcess'
      AND item.LockExpiresAt < @Now
      AND EXISTS (SELECT 1 FROM @EnabledEntities enabled WHERE enabled.EntityName = item.EntityName);

    INSERT dbo.SyncAudit
        (CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],CreatedBy)
    SELECT item.CompanyId,item.EventId,item.EntityName,item.EntityGlobalId,N'Failed',N'InProcess',item.Status,
           N'Lease local vencido liberado.',@WorkerInstance
    FROM dbo.LocalOutbox AS item
    INNER JOIN @Released AS released ON released.Id = item.Id;

    SELECT COUNT(1) FROM @Released;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_LOCALOUTBOX_RECLAMAR
    @WorkerInstance nvarchar(120),
    @BatchSize int,
    @LeaseSeconds int,
    @EnabledEntityNamesJson nvarchar(max) = N'[]'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @BatchSize < 1 OR @BatchSize > 500
        THROW 51164, 'LocalOutbox BatchSize must be between 1 and 500.', 1;
    IF @LeaseSeconds < 30 OR @LeaseSeconds > 14400
        THROW 51164, 'LocalOutbox LeaseSeconds must be between 30 and 14400.', 1;
    IF ISJSON(@EnabledEntityNamesJson) <> 1
        THROW 51164, 'Enabled entity names must be a valid JSON array.', 1;

    DECLARE @EnabledEntities table(EntityName nvarchar(120) NOT NULL PRIMARY KEY);
    INSERT @EnabledEntities(EntityName)
    SELECT DISTINCT LTRIM(RTRIM(CONVERT(nvarchar(120), [value])))
    FROM OPENJSON(@EnabledEntityNamesJson)
    WHERE [type] = 1 AND NULLIF(LTRIM(RTRIM(CONVERT(nvarchar(120), [value]))), N'') IS NOT NULL;

    IF NOT EXISTS (SELECT 1 FROM @EnabledEntities)
        RETURN;

    DECLARE @Now datetime2(0) = SYSUTCDATETIME();
    DECLARE @Claimed table(Id bigint PRIMARY KEY);

    BEGIN TRANSACTION;
    ;WITH Candidates AS
    (
        SELECT TOP (@BatchSize) item.Id
        FROM dbo.LocalOutbox AS item WITH (UPDLOCK,READPAST,ROWLOCK)
        WHERE item.Status IN (N'Pending',N'Error')
          AND (item.NextRetryAt IS NULL OR item.NextRetryAt <= @Now)
          AND (item.LockExpiresAt IS NULL OR item.LockExpiresAt <= @Now)
          AND item.AttemptCount < item.MaxAttempts
          AND EXISTS (SELECT 1 FROM @EnabledEntities enabled WHERE enabled.EntityName = item.EntityName)
        ORDER BY item.CreatedAt,item.Id
    )
    UPDATE item
    SET Status = N'InProcess',
        AttemptCount = AttemptCount + 1,
        LockedBy = @WorkerInstance,
        LockedAt = @Now,
        LockExpiresAt = DATEADD(SECOND,@LeaseSeconds,@Now),
        LastErrorMessage = NULL
    OUTPUT inserted.Id INTO @Claimed(Id)
    FROM dbo.LocalOutbox AS item
    INNER JOIN Candidates AS candidate ON candidate.Id = item.Id;

    INSERT dbo.SyncAudit
        (CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],CreatedBy)
    SELECT item.CompanyId,item.EventId,item.EntityName,item.EntityGlobalId,N'Claimed',
           CASE WHEN item.AttemptCount = 1 THEN N'Pending' ELSE N'Error' END,N'InProcess',
           N'Evento LocalOutbox reclamado por relay.',@WorkerInstance
    FROM dbo.LocalOutbox AS item
    INNER JOIN @Claimed AS claimed ON claimed.Id = item.Id;
    COMMIT TRANSACTION;

    SELECT item.*
    FROM dbo.LocalOutbox AS item
    INNER JOIN @Claimed AS claimed ON claimed.Id = item.Id
    ORDER BY item.CreatedAt,item.Id;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260801.164')
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES(N'20260801.164', N'Aisla claim y leases LocalOutbox por entidades habilitadas');
END;
GO
