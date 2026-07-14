/*
    Ejecutar en NuanSystem_Master.
    Endurecimiento incremental Etapa 7 para sincronizacion Maestro/Sucursal.

    Alcance:
    - Reserva atomica de ejecuciones administrativas cuando PreventConcurrentExecutions = 1.
    - No crea nuevas tablas, workers, schedulers, outbox ni inbox.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_CREATE_SYNCPROFILEEXECUTION
    @SyncProfileId int,
    @ExecutionType nvarchar(20),
    @RequestedBy nvarchar(120) = NULL,
    @CorrelationId nvarchar(100),
    @EntityCodesJson nvarchar(max) = NULL,
    @FromKey nvarchar(200) = NULL,
    @MaxRecords int = NULL,
    @TotalEntities int = 0,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @PreventConcurrent bit = 1;
    DECLARE @CreatedId int;

    BEGIN TRANSACTION;

    SELECT @PreventConcurrent = ISNULL(schedule.PreventConcurrentExecutions, 1)
    FROM dbo.SyncSchedules AS schedule WITH (UPDLOCK, HOLDLOCK)
    WHERE schedule.SyncProfileId = @SyncProfileId
      AND schedule.IsDeleted = 0;

    IF @PreventConcurrent = 1
       AND EXISTS
       (
           SELECT 1
           FROM dbo.SyncProfileExecutions WITH (UPDLOCK, HOLDLOCK)
           WHERE SyncProfileId = @SyncProfileId
             AND Status IN (N'Pending', N'Running', N'Cancelling')
       )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 51071, 'Ya existe una ejecucion activa para el perfil.', 1;
    END;

    INSERT INTO dbo.SyncProfileExecutions
    (
        SyncProfileId, ExecutionType, Status, CorrelationId, RequestedBy,
        EntityCodesJson, FromKey, MaxRecords, TotalEntities,
        CreatedByUserId, CreatedByUserName
    )
    VALUES
    (
        @SyncProfileId, @ExecutionType, N'Pending', @CorrelationId, @RequestedBy,
        @EntityCodesJson, @FromKey, @MaxRecords, ISNULL(@TotalEntities, 0),
        @CreatedByUserId, @CreatedByUserName
    );

    SET @CreatedId = CONVERT(int, SCOPE_IDENTITY());

    COMMIT TRANSACTION;

    SELECT @CreatedId;
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260711.073')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description, AppliedAt)
    VALUES (N'20260711.073', N'Hardening concurrencia ejecuciones Sync Maestro-Sucursal', SYSUTCDATETIME());
END;
GO
