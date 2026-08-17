/*
    Migration 223 - Forward hardening for auxiliary item-master deletes.

    Target: tenant databases only.
    Prerequisite: 106. The tenant may be partially evolved; SQL Server deferred
    name resolution keeps each hardened procedure ready for its feature table,
    and the initializer reapplies this script after later feature migrations.
    Recreates the three logical-delete procedures so the row count is captured
    before audit and the data change plus audit share one transaction.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()=N'NuanSystem_Master'
    THROW 51223,'Migration 223 must run only in tenant databases.',1;
IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NULL
    THROW 51223,'SchemaHistory is required.',1;
IF OBJECT_ID(N'dbo.AuditCatalogChanges',N'U') IS NULL
    THROW 51223,'AuditCatalogChanges from migration 106 is required.',1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_ItemOrigins_ELIMINAR
    @Id int,
    @DeletedByUserId int=NULL,
    @DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;

        DECLARE @Code nvarchar(50),@OldIsActive bit;
        SELECT @Code=Code,@OldIsActive=IsActive
        FROM dbo.ItemOrigins WITH(UPDLOCK,HOLDLOCK)
        WHERE Id=@Id AND IsDeleted=0;

        IF @Code IS NULL
        BEGIN
            IF @OwnTransaction=1 COMMIT;
            SELECT 0;
            RETURN;
        END;

        IF EXISTS(
            SELECT 1
            FROM dbo.ItemMasterProfiles profile WITH(UPDLOCK,HOLDLOCK)
            WHERE ISJSON(profile.MasterDataJson)=1
              AND JSON_VALUE(profile.MasterDataJson,N'$.general.origin') COLLATE Latin1_General_100_BIN2=@Code)
        BEGIN
            IF @OwnTransaction=1 COMMIT;
            SELECT -3;
            RETURN;
        END;

        UPDATE dbo.ItemOrigins
        SET IsActive=0,IsDeleted=1,DeletedByUserId=@DeletedByUserId,
            DeletedByUserName=@DeletedByUserName,DeletedAt=SYSUTCDATETIME()
        WHERE Id=@Id AND IsDeleted=0;

        DECLARE @Affected int=@@ROWCOUNT;
        IF @Affected>0
            INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
            VALUES
            (N'ItemOrigin',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),N'0',@DeletedByUserId,@DeletedByUserName),
            (N'ItemOrigin',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);

        IF @OwnTransaction=1 COMMIT;
        SELECT @Affected;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_ItemCommercialSegments_ELIMINAR
    @Id int,
    @DeletedByUserId int=NULL,
    @DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;

        UPDATE dbo.ItemCommercialSegments
        SET IsActive=0,IsDeleted=1,DeletedByUserId=@DeletedByUserId,
            DeletedByUserName=@DeletedByUserName,DeletedAt=SYSUTCDATETIME()
        WHERE Id=@Id AND IsDeleted=0;

        DECLARE @Affected int=@@ROWCOUNT;
        IF @Affected>0
            INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
            VALUES(N'ItemCommercialSegment',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);

        IF @OwnTransaction=1 COMMIT;
        SELECT @Affected;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_ItemAlertTypes_ELIMINAR
    @Id int,
    @DeletedByUserId int=NULL,
    @DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;

        UPDATE dbo.ItemAlertTypes
        SET IsActive=0,IsDeleted=1,DeletedByUserId=@DeletedByUserId,
            DeletedByUserName=@DeletedByUserName,DeletedAt=SYSUTCDATETIME()
        WHERE Id=@Id AND IsDeleted=0;

        DECLARE @Affected int=@@ROWCOUNT;
        IF @Affected>0
            INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
            VALUES(N'ItemAlertType',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);

        IF @OwnTransaction=1 COMMIT;
        SELECT @Affected;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260815.223')
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260815.223',N'Hardens ItemOrigins, ItemCommercialSegments and ItemAlertTypes logical delete audit transactions');
GO
