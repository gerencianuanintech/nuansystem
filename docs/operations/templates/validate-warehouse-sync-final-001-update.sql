/*
    Validacion posterior del Update Warehouse Master -> Sucursal.
    Solo SELECT. No modifica datos.
*/

USE [NuanSystem_Master];
GO

DECLARE @NewUpdateOutboxId bigint = 20004;
DECLARE @NewUpdateEventId uniqueidentifier = 'db68d25d-a22d-428f-a8db-554054527a12';
DECLARE @EntityGlobalId uniqueidentifier = '383f9281-c05c-41ef-a9be-9fb1a57c9bd2';
DECLARE @EntityCode nvarchar(50) = N'BOD-SYNC-FINAL-001';

SELECT
    Id,
    EventId,
    EntityName,
    EntityCode,
    EntityGlobalId,
    Operation,
    Status,
    AttemptCount,
    ProcessedAt,
    LastErrorMessage,
    LockedBy,
    LockedAt,
    LockExpiresAt
FROM dbo.SyncOutbox
WHERE Id = @NewUpdateOutboxId;

SELECT
    Id,
    OutboxId,
    BranchCompanyId,
    Status,
    AttemptCount,
    AppliedAt,
    LastErrorMessage
FROM dbo.SyncOutboxTargets
WHERE OutboxId = @NewUpdateOutboxId;

SELECT COUNT(*) AS WarehouseCountByCode
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE Code = @EntityCode;

SELECT COUNT(*) AS WarehouseCountByGlobalId
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE GlobalId = @EntityGlobalId;

SELECT
    Id,
    GlobalId,
    Code,
    Name,
    Description,
    City,
    Province,
    AllowsProduction,
    IsActive,
    UpdatedAt
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE GlobalId = @EntityGlobalId
   OR Code = @EntityCode;

SELECT COUNT(*) AS SyncInboxCountByEventId
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.SyncInbox
WHERE EventId = @NewUpdateEventId;

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
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.SyncInbox
WHERE EventId = @NewUpdateEventId;

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
WHERE EventId = @NewUpdateEventId
ORDER BY Id;
