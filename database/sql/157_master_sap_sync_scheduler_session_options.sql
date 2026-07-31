/*
    Fase 10.4 - Reparacion forward-only de opciones de sesion del scheduler SAP.
    Prerrequisitos:
      - 155_master_sap_sync_scheduler.sql.
      - 156_master_sap_sync_scheduler_dapper_contract.sql.

    Alcance: vuelve autonoma la reserva de agendas ante conexiones con opciones
    SET distintas. No modifica datos funcionales ni cambia el contrato del SP.
*/
USE [NuanSystem_Master];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260731.156'
)
    THROW 51157, 'SAP scheduler migration 156 is required.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCSCHEDULERESERVAR
    @ScheduleId bigint,
    @ExpectedRowVersion varbinary(8),
    @UtcNow datetime2(0),
    @ObservedNextExecutionAtUtc datetime2(0) = NULL,
    @ScheduledAtUtc datetime2(0) = NULL,
    @NextExecutionAtUtc datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Requeridas para DML sobre indices filtrados, independientemente del cliente.
    SET ANSI_PADDING ON;
    SET ANSI_WARNINGS ON;
    SET ARITHABORT ON;
    SET CONCAT_NULL_YIELDS_NULL ON;
    SET NUMERIC_ROUNDABORT OFF;

    IF @ExpectedRowVersion IS NULL
       OR DATALENGTH(@ExpectedRowVersion) <> 8
       OR @NextExecutionAtUtc <= @UtcNow
       OR
       (
           (@ObservedNextExecutionAtUtc IS NULL AND @ScheduledAtUtc IS NOT NULL)
           OR
           (
               @ObservedNextExecutionAtUtc IS NOT NULL
               AND
               (
                   @ScheduledAtUtc IS NULL
                   OR @ScheduledAtUtc <> @ObservedNextExecutionAtUtc
               )
           )
       )
        THROW 51155, 'Invalid SAP schedule reservation contract.', 1;

    UPDATE schedule
    SET NextExecutionAtUtc = @NextExecutionAtUtc,
        LastScheduledAtUtc = COALESCE(@ScheduledAtUtc, schedule.LastScheduledAtUtc),
        UpdatedByUserName = N'NuanSystem.SyncWorker',
        UpdatedAt = @UtcNow
    FROM dbo.SapSyncSchedules schedule
    INNER JOIN dbo.SapSyncProfileEntities entity
        ON entity.Id = schedule.SapSyncProfileEntityId
    INNER JOIN dbo.SapSyncProfiles profile
        ON profile.Id = entity.SapSyncProfileId
    WHERE schedule.Id = @ScheduleId
      AND schedule.RowVersion = @ExpectedRowVersion
      AND schedule.IsDeleted = 0
      AND schedule.IsActive = 1
      AND schedule.ScheduleType <> 'Manual'
      AND entity.IsDeleted = 0
      AND entity.IsActive = 1
      AND profile.IsDeleted = 0
      AND profile.IsActive = 1
      AND
      (
          (@ObservedNextExecutionAtUtc IS NULL AND schedule.NextExecutionAtUtc IS NULL)
          OR
          (
              @ObservedNextExecutionAtUtc IS NOT NULL
              AND schedule.NextExecutionAtUtc = @ObservedNextExecutionAtUtc
              AND schedule.NextExecutionAtUtc <= @UtcNow
          )
      );

    SELECT @@ROWCOUNT;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260731.157'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260731.157',
        N'Repara opciones de sesion de la reserva del scheduler SAP'
    );
END;
GO
