/*
    Contrato incremental y Full para UnitOfMeasures.

    LocalOutbox se escribe desde Application mediante ILocalSyncOutboxRepository
    en la misma conexion/transaccion usada por los SP CRUD de 194. Este script
    entrega la proyeccion Full y el aplicador idempotente por GlobalId.

    ExternalSystem/ExternalCode son locales: Full no los publica, Apply crea en
    NULL y nunca los modifica. No existe adopcion automatica por Code.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.UnitOfMeasures',N'U') IS NULL
   OR COL_LENGTH(N'dbo.UnitOfMeasures',N'GlobalId') IS NULL
   OR COL_LENGTH(N'dbo.UnitOfMeasures',N'MagnitudeCode') IS NULL
    THROW 51196,'UnitOfMeasures from migration 194 is required.',1;
IF OBJECT_ID(N'dbo.LocalOutbox',N'U') IS NULL
    THROW 51196,'LocalOutbox is required for transactional CRUD publishing.',1;
IF OBJECT_ID(N'dbo.AuditInventoryChanges',N'U') IS NULL
    THROW 51196,'AuditInventoryChanges is required.',1;
IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NULL
    THROW 51196,'SchemaHistory is required.',1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_UNIT_OF_MEASURE_SYNC_FULL
    @AfterId int=NULL, @BatchSize int=100
AS
BEGIN
    SET NOCOUNT ON;
    IF @BatchSize<1 OR @BatchSize>10001
        THROW 51196,'UnitOfMeasure Full BatchSize must be between 1 and 10001.',1;
    SELECT TOP(@BatchSize)
           Id,GlobalId,Code,Name,Description,Symbol,MagnitudeCode,SortOrder,
           IsActive,IsDeleted,CreatedAt,UpdatedAt
    FROM dbo.UnitOfMeasures
    WHERE @AfterId IS NULL OR Id>@AfterId
    ORDER BY Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_UNIT_OF_MEASURE_SYNC_APPLY
    @GlobalId uniqueidentifier,
    @Code nvarchar(50), @Name nvarchar(150), @Description nvarchar(500)=NULL,
    @Symbol nvarchar(20)=NULL, @MagnitudeCode nvarchar(20), @SortOrder int=0,
    @IsActive bit, @IsDeleted bit, @UpdatedAt datetime2(0)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name));
    SET @Symbol=NULLIF(LTRIM(RTRIM(@Symbol)),N''); SET @MagnitudeCode=LTRIM(RTRIM(@MagnitudeCode));
    IF @GlobalId IS NULL OR @GlobalId='00000000-0000-0000-0000-000000000000'
        THROW 51196,'UnitOfMeasure GlobalId is required for sync.',1;
    IF NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL OR @SortOrder<0
        THROW 51196,'UnitOfMeasure sync payload is invalid.',1;
    IF @MagnitudeCode NOT IN(N'Quantity',N'Packaging',N'Mass',N'Volume',N'Length',N'Area',N'Time',N'Other')
        THROW 51196,'UnitOfMeasure sync MagnitudeCode is invalid.',1;

    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;

        DECLARE @UnitMeasureId int,@OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),
                @OldSymbol nvarchar(20),@OldMagnitudeCode nvarchar(20),@OldSortOrder int,
                @OldIsActive bit,@OldIsDeleted bit;

        IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WITH(UPDLOCK,HOLDLOCK)
                  WHERE Code=@Code AND GlobalId<>@GlobalId)
        BEGIN
            IF @OwnTransaction=1 COMMIT;
            SELECT -2 AS ResultCode,CONVERT(int,NULL) AS UnitMeasureId;
            RETURN;
        END;

        SELECT @UnitMeasureId=Id,@OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSymbol=Symbol,
               @OldMagnitudeCode=MagnitudeCode,@OldSortOrder=SortOrder,@OldIsActive=IsActive,@OldIsDeleted=IsDeleted
        FROM dbo.UnitOfMeasures WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;

        IF @UnitMeasureId IS NULL
        BEGIN
            INSERT dbo.UnitOfMeasures
            (GlobalId,Code,Name,Description,Symbol,MagnitudeCode,SortOrder,IsActive,IsDeleted,
             ExternalSystem,ExternalCode,CreatedAt,CreatedByUserName,DeletedAt,DeletedByUserName)
            VALUES
            (@GlobalId,@Code,@Name,@Description,@Symbol,@MagnitudeCode,@SortOrder,@IsActive,@IsDeleted,
             NULL,NULL,COALESCE(@UpdatedAt,SYSUTCDATETIME()),N'MasterBranchSyncWorker',
             CASE WHEN @IsDeleted=1 THEN COALESCE(@UpdatedAt,SYSUTCDATETIME()) END,
             CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END);
            SET @UnitMeasureId=CONVERT(int,SCOPE_IDENTITY());
        END
        ELSE
        BEGIN
            UPDATE dbo.UnitOfMeasures
            SET Code=@Code,Name=@Name,Description=@Description,Symbol=@Symbol,MagnitudeCode=@MagnitudeCode,
                SortOrder=@SortOrder,IsActive=@IsActive,IsDeleted=@IsDeleted,
                UpdatedAt=COALESCE(@UpdatedAt,SYSUTCDATETIME()),UpdatedByUserName=N'MasterBranchSyncWorker',
                DeletedAt=CASE WHEN @IsDeleted=1 THEN COALESCE(DeletedAt,@UpdatedAt,SYSUTCDATETIME()) ELSE NULL END,
                DeletedByUserId=CASE WHEN @IsDeleted=1 THEN DeletedByUserId ELSE NULL END,
                DeletedByUserName=CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' ELSE NULL END
            WHERE Id=@UnitMeasureId;
            /* Intencional: no modificar ExternalSystem/ExternalCode locales. */
        END;

        INSERT dbo.AuditInventoryChanges
        (EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
        SELECT N'UnitOfMeasures',CONVERT(nvarchar(80),@UnitMeasureId),
               CASE WHEN @OldCode IS NULL THEN N'INSERT'
                    WHEN @IsDeleted=1 AND ISNULL(@OldIsDeleted,0)=0 THEN N'DELETE'
                    ELSE N'UPDATE' END,
               FieldName,OldValue,NewValue,N'MasterBranchSyncWorker',N'MasterBranchSyncWorker'
        FROM(VALUES
            (N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),
            (N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
            (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
            (N'Symbol',CONVERT(nvarchar(max),@OldSymbol),CONVERT(nvarchar(max),@Symbol)),
            (N'MagnitudeCode',CONVERT(nvarchar(max),@OldMagnitudeCode),CONVERT(nvarchar(max),@MagnitudeCode)),
            (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
            (N'IsDeleted',CONVERT(nvarchar(max),CONVERT(int,@OldIsDeleted)),CONVERT(nvarchar(max),CONVERT(int,@IsDeleted)))
        ) valuesToAudit(FieldName,OldValue,NewValue)
        WHERE @OldCode IS NULL OR ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');

        IF @OwnTransaction=1 COMMIT;
        SELECT 1 AS ResultCode,@UnitMeasureId AS UnitMeasureId;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        IF ERROR_NUMBER() IN(2601,2627)
        BEGIN SELECT -2 AS ResultCode,CONVERT(int,NULL) AS UnitMeasureId; RETURN; END;
        THROW;
    END CATCH;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260812.196')
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260812.196',N'Agrega Full y Apply incremental local-safe para Unidades de medida');
GO
