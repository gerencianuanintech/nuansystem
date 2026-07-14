/*
    Validacion final Warehouse Master -> Sucursal para BOD-SYNC-FINAL-001.

    Solo SELECT. No modifica datos.
    Ejecutar contra SQL Server con permisos de lectura sobre:
    - NuanSystem_Master
    - NuanSystem_SYNC_WH_BRANCH_TEST
*/

USE [NuanSystem_Master];
GO

DECLARE @SyncOutboxId bigint = 20003;
DECLARE @TargetId bigint = 20003;
DECLARE @EventId uniqueidentifier = 'e3600237-9167-4ee8-a6de-b8e62931a38a';
DECLARE @EntityGlobalId uniqueidentifier = '383f9281-c05c-41ef-a9be-9fb1a57c9bd2';
DECLARE @EntityCode nvarchar(50) = N'BOD-SYNC-FINAL-001';

SELECT
    Id,
    EventId,
    CompanyId,
    EntityName,
    EntityCode,
    EntityGlobalId,
    Operation,
    Status,
    AttemptCount,
    LockedBy,
    LockedAt,
    LockExpiresAt,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox
WHERE Id = @SyncOutboxId;

SELECT
    Id,
    OutboxId,
    BranchCompanyId,
    Status,
    AttemptCount,
    AppliedAt,
    LastErrorMessage
FROM dbo.SyncOutboxTargets
WHERE Id = @TargetId;

SELECT
    COUNT(1) AS WarehouseCountByCode
FROM [NuanSystem_SYNC_WH_BRANCH_TEST].dbo.Warehouses
WHERE Code = @EntityCode
  AND IsDeleted = 0;

SELECT
    COUNT(1) AS WarehouseCountByGlobalId
FROM [NuanSystem_SYNC_WH_BRANCH_TEST].dbo.Warehouses
WHERE GlobalId = @EntityGlobalId
  AND IsDeleted = 0;

SELECT
    Id,
    GlobalId,
    Code,
    Name,
    City,
    Province,
    AllowsProduction,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM [NuanSystem_SYNC_WH_BRANCH_TEST].dbo.Warehouses
WHERE (Code = @EntityCode OR GlobalId = @EntityGlobalId)
  AND IsDeleted = 0;

SELECT
    COUNT(1) AS SyncInboxCountByEventId
FROM [NuanSystem_SYNC_WH_BRANCH_TEST].dbo.SyncInbox
WHERE EventId = @EventId;

SELECT
    Id,
    EventId,
    SourceCompanyId,
    EntityName,
    EntityGlobalId,
    Operation,
    Status,
    AttemptCount,
    ReceivedAt,
    AppliedAt,
    LastErrorMessage
FROM [NuanSystem_SYNC_WH_BRANCH_TEST].dbo.SyncInbox
WHERE EventId = @EventId;

SELECT
    Id,
    EventId,
    CompanyId,
    BranchCompanyId,
    EntityName,
    EntityGlobalId,
    Action,
    PreviousStatus,
    NewStatus,
    Message,
    ErrorCode,
    CreatedAt,
    CreatedBy
FROM dbo.SyncAudit
WHERE EventId = @EventId
ORDER BY CreatedAt, Id;
