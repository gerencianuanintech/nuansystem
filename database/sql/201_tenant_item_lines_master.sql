/*
    Evoluciona dbo.ItemLines como maestro tenant independiente.

    - Solo bases tenant; prerrequisitos 044, 065, 106 y SchemaHistory.
    - Conserva todas las filas existentes y no crea nuevas semillas.
    - No agrega referencias SAP/externas ni IsSystem.
    - LocalOutbox se escribe desde Application dentro de la misma transaccion CRUD.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()=N'NuanSystem_Master' THROW 51201,'Migration 201 must run only in tenant databases.',1;
IF OBJECT_ID(N'dbo.ItemLines',N'U') IS NULL THROW 51201,'ItemLines from migration 044 is required.',1;
IF OBJECT_ID(N'dbo.LocalOutbox',N'U') IS NULL THROW 51201,'LocalOutbox from migration 065 is required.',1;
IF OBJECT_ID(N'dbo.AuditCatalogChanges',N'U') IS NULL THROW 51201,'AuditCatalogChanges from migration 106 is required.',1;
IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NULL THROW 51201,'SchemaHistory is required.',1;
GO

IF EXISTS(SELECT UPPER(LTRIM(RTRIM(Code))) FROM dbo.ItemLines GROUP BY UPPER(LTRIM(RTRIM(Code))) HAVING COUNT_BIG(1)>1)
    THROW 51201,'ItemLines contains duplicate codes, including deleted records.',1;
IF EXISTS(SELECT 1 FROM dbo.ItemLines WHERE NULLIF(LTRIM(RTRIM(Code)),N'') IS NULL OR NULLIF(LTRIM(RTRIM(Name)),N'') IS NULL)
    THROW 51201,'ItemLines contains blank codes or names.',1;
GO

BEGIN TRY
    BEGIN TRANSACTION;
    IF COL_LENGTH(N'dbo.ItemLines',N'GlobalId') IS NULL ALTER TABLE dbo.ItemLines ADD GlobalId uniqueidentifier NULL;
    IF COL_LENGTH(N'dbo.ItemLines',N'SortOrder') IS NULL ALTER TABLE dbo.ItemLines ADD SortOrder int NULL;

    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK;
    THROW;
END CATCH;
GO

BEGIN TRY
    BEGIN TRANSACTION;
    UPDATE dbo.ItemLines
    SET Code=LTRIM(RTRIM(Code)),Name=LTRIM(RTRIM(Name)),Description=NULLIF(LTRIM(RTRIM(Description)),N'');
    UPDATE dbo.ItemLines SET GlobalId=NEWID() WHERE GlobalId IS NULL;
    UPDATE dbo.ItemLines SET SortOrder=0 WHERE SortOrder IS NULL;

    IF EXISTS(SELECT 1 FROM dbo.ItemLines WHERE GlobalId IS NULL OR SortOrder IS NULL)
        THROW 51201,'ItemLines backfill did not resolve all required values.',1;
    IF EXISTS(SELECT GlobalId FROM dbo.ItemLines GROUP BY GlobalId HAVING COUNT_BIG(1)>1)
        THROW 51201,'ItemLines contains duplicate GlobalId values.',1;

    ALTER TABLE dbo.ItemLines ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
    ALTER TABLE dbo.ItemLines ALTER COLUMN SortOrder int NOT NULL;

    IF NOT EXISTS(SELECT 1 FROM sys.default_constraints d JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id WHERE d.parent_object_id=OBJECT_ID(N'dbo.ItemLines') AND c.name=N'GlobalId')
        ALTER TABLE dbo.ItemLines ADD CONSTRAINT DF_ItemLines_GlobalId DEFAULT NEWID() FOR GlobalId;
    IF NOT EXISTS(SELECT 1 FROM sys.default_constraints d JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id WHERE d.parent_object_id=OBJECT_ID(N'dbo.ItemLines') AND c.name=N'SortOrder')
        ALTER TABLE dbo.ItemLines ADD CONSTRAINT DF_ItemLines_SortOrder DEFAULT(0) FOR SortOrder;

    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemLines') AND name=N'CK_ItemLines_Code_NotBlank')
        ALTER TABLE dbo.ItemLines ADD CONSTRAINT CK_ItemLines_Code_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Code)),N'') IS NOT NULL);
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemLines') AND name=N'CK_ItemLines_Name_NotBlank')
        ALTER TABLE dbo.ItemLines ADD CONSTRAINT CK_ItemLines_Name_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Name)),N'') IS NOT NULL);
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemLines') AND name=N'CK_ItemLines_SortOrder')
        ALTER TABLE dbo.ItemLines ADD CONSTRAINT CK_ItemLines_SortOrder CHECK(SortOrder>=0);

    IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemLines') AND name=N'UX_ItemLines_Code_Active')
        DROP INDEX UX_ItemLines_Code_Active ON dbo.ItemLines;
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemLines') AND name=N'UQ_ItemLines_Code')
        CREATE UNIQUE INDEX UQ_ItemLines_Code ON dbo.ItemLines(Code);
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemLines') AND name=N'UQ_ItemLines_GlobalId')
        CREATE UNIQUE INDEX UQ_ItemLines_GlobalId ON dbo.ItemLines(GlobalId);
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemLines') AND name=N'IX_ItemLines_Active_SortOrder_Name')
        CREATE INDEX IX_ItemLines_Active_SortOrder_Name ON dbo.ItemLines(IsActive,SortOrder,Name) INCLUDE(Code) WHERE IsDeleted=0;
    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK;
    THROW;
END CATCH;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMLINES_LISTAR AS
BEGIN SET NOCOUNT ON;
 SELECT Id,GlobalId,Code,Name,Description,SortOrder,IsActive,
        CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,
        DeletedByUserId,DeletedByUserName,DeletedAt
 FROM dbo.ItemLines WHERE IsDeleted=0 ORDER BY SortOrder,Name,Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMLINES_BUSCARPORID @Id int AS
BEGIN SET NOCOUNT ON;
 SELECT TOP(1) Id,GlobalId,Code,Name,Description,SortOrder,IsActive,
        CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,
        DeletedByUserId,DeletedByUserName,DeletedAt
 FROM dbo.ItemLines WHERE Id=@Id AND IsDeleted=0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMLINES_LOOKUP AS
BEGIN SET NOCOUNT ON;
 SELECT Id,GlobalId,Code,Name,SortOrder,CAST(IsActive AS bit) IsActive
 FROM dbo.ItemLines WHERE IsDeleted=0 AND IsActive=1 ORDER BY SortOrder,Name,Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMLINES_BUSCARPORCODIGO @Code nvarchar(50),@ExcluirId int=NULL AS
BEGIN SET NOCOUNT ON;
 SELECT COUNT(1) FROM dbo.ItemLines WITH(UPDLOCK,HOLDLOCK)
 WHERE Code=LTRIM(RTRIM(@Code)) AND (@ExcluirId IS NULL OR Id<>@ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMLINES_HISTORIAL @Id int AS
BEGIN SET NOCOUNT ON;
 SELECT Id,EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName,[Source],CreatedAt
 FROM dbo.AuditCatalogChanges WHERE EntityName=N'ItemLine' AND RecordId=CONVERT(nvarchar(80),@Id)
 ORDER BY CreatedAt DESC,Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_GENERAL_INVENTORY_ITEMLINES_CREAR
 @Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,
 @GlobalId uniqueidentifier=NULL,@CreatedByUserId int=NULL,@CreatedByUserName nvarchar(120)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name)); SET @Description=NULLIF(LTRIM(RTRIM(@Description)),N'');
 SET @GlobalId=COALESCE(@GlobalId,NEWID());
 IF NULLIF(@Code,N'') IS NULL THROW 51002,'El codigo es obligatorio.',1;
 IF NULLIF(@Name,N'') IS NULL THROW 51003,'El nombre es obligatorio.',1;
 IF @SortOrder<0 THROW 51201,'ItemLine SortOrder cannot be negative.',1;
 IF @GlobalId='00000000-0000-0000-0000-000000000000' THROW 51201,'ItemLine GlobalId is invalid.',1;
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  IF EXISTS(SELECT 1 FROM dbo.ItemLines WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code OR GlobalId=@GlobalId)
  BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
  INSERT dbo.ItemLines(GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedAt,CreatedByUserId,CreatedByUserName)
  VALUES(@GlobalId,@Code,@Name,@Description,@SortOrder,@IsActive,0,SYSUTCDATETIME(),@CreatedByUserId,@CreatedByUserName);
  DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'ItemLine',CONVERT(nvarchar(80),@Id),N'INSERT',FieldName,NULL,NewValue,@CreatedByUserId,@CreatedByUserName
  FROM(VALUES(N'GlobalId',CONVERT(nvarchar(max),@GlobalId)),(N'Code',CONVERT(nvarchar(max),@Code)),
       (N'Name',CONVERT(nvarchar(max),@Name)),(N'Description',CONVERT(nvarchar(max),@Description)),
       (N'SortOrder',CONVERT(nvarchar(max),@SortOrder)),(N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@IsActive))))v(FieldName,NewValue);
  IF @OwnTransaction=1 COMMIT; SELECT @Id;
 END TRY BEGIN CATCH
  IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
  IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END;
  THROW;
 END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_GENERAL_INVENTORY_ITEMLINES_ACTUALIZAR
 @Id int,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,
 @UpdatedByUserId int=NULL,@UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name)); SET @Description=NULLIF(LTRIM(RTRIM(@Description)),N'');
 IF @Id<=0 THROW 51201,'ItemLine Id is invalid.',1;
 IF NULLIF(@Code,N'') IS NULL THROW 51002,'El codigo es obligatorio.',1;
 IF NULLIF(@Name,N'') IS NULL THROW 51003,'El nombre es obligatorio.',1;
 IF @SortOrder<0 THROW 51201,'ItemLine SortOrder cannot be negative.',1;
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  DECLARE @OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldSortOrder int,@OldIsActive bit;
  SELECT @OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSortOrder=SortOrder,@OldIsActive=IsActive
  FROM dbo.ItemLines WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
  IF @OldCode IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;
  IF EXISTS(SELECT 1 FROM dbo.ItemLines WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND Id<>@Id)
  BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
  UPDATE dbo.ItemLines SET Code=@Code,Name=@Name,Description=@Description,SortOrder=@SortOrder,IsActive=@IsActive,
      UpdatedAt=SYSUTCDATETIME(),UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName WHERE Id=@Id AND IsDeleted=0;
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'ItemLine',CONVERT(nvarchar(80),@Id),N'UPDATE',FieldName,OldValue,NewValue,@UpdatedByUserId,@UpdatedByUserName
  FROM(VALUES(N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),
      (N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
      (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
      (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
      (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))))v(FieldName,OldValue,NewValue)
  WHERE ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
  IF @OwnTransaction=1 COMMIT; SELECT 1;
 END TRY BEGIN CATCH
  IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
  IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END;
  THROW;
 END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_ITEMLINES_ELIMINAR
 @Id int,@DeletedByUserId int=NULL,@DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  DECLARE @OldIsActive bit,@IsInUse bit=0;
  SELECT @OldIsActive=IsActive FROM dbo.ItemLines WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
  IF @OldIsActive IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;
  DECLARE @SchemaName sysname,@TableName sysname,@ColumnName sysname,@Sql nvarchar(max),@Referenced bit;
  DECLARE reference_cursor CURSOR LOCAL FAST_FORWARD FOR
  SELECT OBJECT_SCHEMA_NAME(fk.parent_object_id),OBJECT_NAME(fk.parent_object_id),parentColumn.name
  FROM sys.foreign_keys fk JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id=fk.object_id
  JOIN sys.columns parentColumn ON parentColumn.object_id=fk.parent_object_id AND parentColumn.column_id=fkc.parent_column_id
  WHERE fk.referenced_object_id=OBJECT_ID(N'dbo.ItemLines') AND 1=(SELECT COUNT(1) FROM sys.foreign_key_columns x WHERE x.constraint_object_id=fk.object_id);
  OPEN reference_cursor; FETCH NEXT FROM reference_cursor INTO @SchemaName,@TableName,@ColumnName;
  WHILE @@FETCH_STATUS=0 AND @IsInUse=0
  BEGIN
   SET @Referenced=0; SET @Sql=N'SELECT @Found=CASE WHEN EXISTS(SELECT 1 FROM '+QUOTENAME(@SchemaName)+N'.'+QUOTENAME(@TableName)+N' WHERE '+QUOTENAME(@ColumnName)+N'=@ItemLineId) THEN 1 ELSE 0 END;';
   EXEC sys.sp_executesql @Sql,N'@ItemLineId int,@Found bit OUTPUT',@ItemLineId=@Id,@Found=@Referenced OUTPUT;
   IF @Referenced=1 SET @IsInUse=1;
   FETCH NEXT FROM reference_cursor INTO @SchemaName,@TableName,@ColumnName;
  END;
  CLOSE reference_cursor; DEALLOCATE reference_cursor;
  IF @IsInUse=1 BEGIN IF @OwnTransaction=1 COMMIT; SELECT -3; RETURN; END;
  UPDATE dbo.ItemLines SET IsActive=0,IsDeleted=1,DeletedAt=SYSUTCDATETIME(),DeletedByUserId=@DeletedByUserId,DeletedByUserName=@DeletedByUserName WHERE Id=@Id AND IsDeleted=0;
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  VALUES(N'ItemLine',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),N'0',@DeletedByUserId,@DeletedByUserName),
        (N'ItemLine',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);
  IF @OwnTransaction=1 COMMIT; SELECT 1;
 END TRY BEGIN CATCH
  IF CURSOR_STATUS('local','reference_cursor')>=0 CLOSE reference_cursor;
  IF CURSOR_STATUS('local','reference_cursor')>-3 DEALLOCATE reference_cursor;
  IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
  THROW;
 END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_LINE_SYNC_FULL @AfterId int=NULL,@BatchSize int=100 AS
BEGIN
 SET NOCOUNT ON;
 IF @BatchSize<1 OR @BatchSize>10001 THROW 51201,'ItemLine Full BatchSize must be between 1 and 10001.',1;
 SELECT TOP(@BatchSize) Id,GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedAt,UpdatedAt
 FROM dbo.ItemLines WHERE @AfterId IS NULL OR Id>@AfterId ORDER BY Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_LINE_SYNC_APPLY
 @GlobalId uniqueidentifier,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,
 @SortOrder int=0,@IsActive bit,@IsDeleted bit,@UpdatedAt datetime2(0)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name)); SET @Description=NULLIF(LTRIM(RTRIM(@Description)),N'');
 IF @GlobalId IS NULL OR @GlobalId='00000000-0000-0000-0000-000000000000' THROW 51201,'ItemLine GlobalId is required for sync.',1;
 IF NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL OR @SortOrder<0 THROW 51201,'ItemLine sync payload is invalid.',1;
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  IF EXISTS(SELECT 1 FROM dbo.ItemLines WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND GlobalId<>@GlobalId)
  BEGIN IF @OwnTransaction=1 COMMIT; SELECT -2 ResultCode,CONVERT(int,NULL) ItemLineId; RETURN; END;
  DECLARE @ItemLineId int,@OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldSortOrder int,@OldIsActive bit,@OldIsDeleted bit;
  SELECT @ItemLineId=Id,@OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSortOrder=SortOrder,@OldIsActive=IsActive,@OldIsDeleted=IsDeleted
  FROM dbo.ItemLines WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
  IF @ItemLineId IS NULL
  BEGIN
   INSERT dbo.ItemLines(GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedAt,CreatedByUserName,DeletedAt,DeletedByUserName)
   VALUES(@GlobalId,@Code,@Name,@Description,@SortOrder,@IsActive,@IsDeleted,COALESCE(@UpdatedAt,SYSUTCDATETIME()),N'MasterBranchSyncWorker',
          CASE WHEN @IsDeleted=1 THEN COALESCE(@UpdatedAt,SYSUTCDATETIME()) END,CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END);
   SET @ItemLineId=CONVERT(int,SCOPE_IDENTITY());
  END
  ELSE
   UPDATE dbo.ItemLines SET Code=@Code,Name=@Name,Description=@Description,SortOrder=@SortOrder,
      IsActive=@IsActive,IsDeleted=@IsDeleted,UpdatedAt=COALESCE(@UpdatedAt,SYSUTCDATETIME()),UpdatedByUserName=N'MasterBranchSyncWorker',
      DeletedAt=CASE WHEN @IsDeleted=1 THEN COALESCE(DeletedAt,@UpdatedAt,SYSUTCDATETIME()) ELSE NULL END,
      DeletedByUserId=CASE WHEN @IsDeleted=1 THEN DeletedByUserId ELSE NULL END,
      DeletedByUserName=CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' ELSE NULL END WHERE Id=@ItemLineId;
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
  SELECT N'ItemLine',CONVERT(nvarchar(80),@ItemLineId),CASE WHEN @OldCode IS NULL THEN N'INSERT' WHEN @IsDeleted=1 AND ISNULL(@OldIsDeleted,0)=0 THEN N'DELETE' ELSE N'UPDATE' END,
      FieldName,OldValue,NewValue,N'MasterBranchSyncWorker',N'MasterBranchSyncWorker'
  FROM(VALUES(N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),
      (N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
      (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
      (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
      (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
      (N'IsDeleted',CONVERT(nvarchar(max),CONVERT(int,@OldIsDeleted)),CONVERT(nvarchar(max),CONVERT(int,@IsDeleted))))v(FieldName,OldValue,NewValue)
  WHERE @OldCode IS NULL OR ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
  IF @OwnTransaction=1 COMMIT; SELECT 1 ResultCode,@ItemLineId ItemLineId;
 END TRY BEGIN CATCH
  IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
  IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -2 ResultCode,CONVERT(int,NULL) ItemLineId; RETURN; END;
  THROW;
 END CATCH;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260813.201')
 INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260813.201',N'Funcionaliza ItemLines con GlobalId, auditoria, LocalOutbox y apply por GlobalId');
GO
