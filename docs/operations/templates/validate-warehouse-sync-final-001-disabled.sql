/*
    Validacion posterior del Disabled Warehouse Master -> Sucursal.
    Solo SELECT. No modifica datos.
*/

USE [NuanSystem_Master];
GO

DECLARE @NewDisableOutboxId bigint = 20005;
DECLARE @NewDisableEventId uniqueidentifier = 'fa452762-910a-40fc-9e79-72fb06a28eea';
DECLARE @EntityGlobalId uniqueidentifier = '383f9281-c05c-41ef-a9be-9fb1a57c9bd2';
DECLARE @EntityCode nvarchar(50) = N'BOD-SYNC-FINAL-001';

SELECT
    Id AS NewDisableOutboxId,
    EventId,
    EntityName,
    EntityCode,
    EntityGlobalId,
    Operation,
    Status,
    AttemptCount,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox
WHERE Id = @NewDisableOutboxId;

SELECT
    Id,
    OutboxId,
    BranchCompanyId,
    Status,
    AttemptCount,
    AppliedAt,
    LastErrorMessage
FROM dbo.SyncOutboxTargets
WHERE OutboxId = @NewDisableOutboxId;

SELECT
    Id,
    GlobalId,
    Code,
    Name,
    IsActive,
    UpdatedAt
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE Code = @EntityCode
ORDER BY Id;

SELECT
    Id,
    GlobalId,
    Code,
    Name,
    IsActive,
    UpdatedAt
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE GlobalId = @EntityGlobalId
ORDER BY Id;

SELECT
    Id,
    EventId AS NewDisableEventId,
    SourceCompanyId,
    EntityName,
    EntityGlobalId,
    Operation,
    Status,
    AppliedAt,
    LastErrorMessage
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.SyncInbox
WHERE EventId = @NewDisableEventId;

SELECT COUNT(*) AS WarehouseCountByGlobalId
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE GlobalId = @EntityGlobalId;

SELECT COUNT(*) AS WarehouseCountByCode
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE Code = @EntityCode;

SELECT
    Id,
    EventId AS NewDisableEventId,
    Action,
    PreviousStatus,
    NewStatus,
    Message,
    CreatedAt
FROM dbo.SyncAudit
WHERE EventId = @NewDisableEventId
ORDER BY Id;
