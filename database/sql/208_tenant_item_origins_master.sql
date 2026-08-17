/*
    Crea ItemOrigins como maestro tenant independiente y preserva los valores
    historicos almacenados en ItemMasterProfiles.MasterDataJson.

    Prerrequisitos: 043, 065, 106 y SchemaHistory. No modifica el JSON de Item,
    no activa sincronizacion y no integra SAP/SRI.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()=N'NuanSystem_Master' THROW 51208,'Migration 208 must run only in tenant databases.',1;
IF OBJECT_ID(N'dbo.ItemMasterProfiles',N'U') IS NULL THROW 51208,'ItemMasterProfiles from migration 043 is required.',1;
IF OBJECT_ID(N'dbo.LocalOutbox',N'U') IS NULL THROW 51208,'LocalOutbox from migration 065 is required.',1;
IF OBJECT_ID(N'dbo.AuditCatalogChanges',N'U') IS NULL THROW 51208,'AuditCatalogChanges from migration 106 is required.',1;
IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NULL THROW 51208,'SchemaHistory is required.',1;
GO

IF EXISTS
(
    SELECT 1 FROM dbo.ItemMasterProfiles
    WHERE ISJSON(MasterDataJson)=1
      AND LEN(NULLIF(LTRIM(RTRIM(JSON_VALUE(MasterDataJson,N'$.general.origin'))),N''))>50
)
    THROW 51208,'A legacy Item origin exceeds 50 characters. Map it explicitly before migration 208; values are never truncated.',1;
GO

IF OBJECT_ID(N'dbo.ItemOrigins',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemOrigins
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemOrigins PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_ItemOrigins_GlobalId DEFAULT NEWID(),
        Code nvarchar(50) COLLATE Latin1_General_100_BIN2 NOT NULL,
        Name nvarchar(150) NOT NULL,
        Description nvarchar(500) NULL,
        SortOrder int NOT NULL CONSTRAINT DF_ItemOrigins_SortOrder DEFAULT(0),
        IsActive bit NOT NULL CONSTRAINT DF_ItemOrigins_IsActive DEFAULT(1),
        IsDeleted bit NOT NULL CONSTRAINT DF_ItemOrigins_IsDeleted DEFAULT(0),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ItemOrigins_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT CK_ItemOrigins_GlobalId CHECK(GlobalId<>'00000000-0000-0000-0000-000000000000'),
        CONSTRAINT CK_ItemOrigins_Code_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Code)),N'') IS NOT NULL),
        CONSTRAINT CK_ItemOrigins_Name_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Name)),N'') IS NOT NULL),
        CONSTRAINT CK_ItemOrigins_SortOrder CHECK(SortOrder>=0)
    );
END;
GO

BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @CorporateOrigins table
 (
     GlobalId uniqueidentifier NOT NULL, Code nvarchar(50) COLLATE Latin1_General_100_BIN2 NOT NULL,
     Name nvarchar(150) NOT NULL, Description nvarchar(500) NULL, SortOrder int NOT NULL
 );
 INSERT @CorporateOrigins VALUES
 ('6e24c5dd-6031-4a3d-9ed5-4a3aac71a001',N'Local',N'Local',N'Articulo de origen local.',10),
 ('6e24c5dd-6031-4a3d-9ed5-4a3aac71a002',N'Imported',N'Importado',N'Articulo de origen importado.',20),
 ('6e24c5dd-6031-4a3d-9ed5-4a3aac71a003',N'Mixed',N'Mixto',N'Articulo con origen local e importado.',30);

 IF EXISTS
 (
     SELECT 1 FROM @CorporateOrigins seed
     JOIN dbo.ItemOrigins existing ON existing.GlobalId=seed.GlobalId
     WHERE existing.Code<>seed.Code COLLATE Latin1_General_100_BIN2
 )
     THROW 51208,'A deterministic ItemOrigin GlobalId is already assigned to another exact code.',1;

 INSERT dbo.ItemOrigins(GlobalId,Code,Name,Description,SortOrder,IsActive,CreatedByUserName)
 SELECT seed.GlobalId,seed.Code,seed.Name,seed.Description,seed.SortOrder,1,N'Sistema'
 FROM @CorporateOrigins seed
 WHERE NOT EXISTS(SELECT 1 FROM dbo.ItemOrigins existing WHERE existing.Code=seed.Code);

 UPDATE existing SET GlobalId=CASE WHEN existing.GlobalId=seed.GlobalId THEN existing.GlobalId ELSE seed.GlobalId END,
     UpdatedAt=CASE WHEN existing.GlobalId=seed.GlobalId THEN existing.UpdatedAt ELSE SYSUTCDATETIME() END,
     UpdatedByUserName=CASE WHEN existing.GlobalId=seed.GlobalId THEN existing.UpdatedByUserName ELSE N'Sistema' END
 FROM dbo.ItemOrigins existing INNER JOIN @CorporateOrigins seed ON seed.Code=existing.Code;

 ;WITH LegacyOrigins AS
 (
     SELECT DISTINCT LTRIM(RTRIM(JSON_VALUE(profile.MasterDataJson,N'$.general.origin'))) COLLATE Latin1_General_100_BIN2 Code
     FROM dbo.ItemMasterProfiles profile
     WHERE ISJSON(profile.MasterDataJson)=1
       AND NULLIF(LTRIM(RTRIM(JSON_VALUE(profile.MasterDataJson,N'$.general.origin'))),N'') IS NOT NULL
 )
 INSERT dbo.ItemOrigins(GlobalId,Code,Name,Description,SortOrder,IsActive,CreatedByUserName)
 SELECT NEWID(),legacy.Code,legacy.Code,N'Valor historico preservado desde ItemMasterProfiles.',1000,1,N'Migracion 208'
 FROM LegacyOrigins legacy
 WHERE NOT EXISTS(SELECT 1 FROM dbo.ItemOrigins existing WHERE existing.Code=legacy.Code);

 IF EXISTS(SELECT GlobalId FROM dbo.ItemOrigins GROUP BY GlobalId HAVING COUNT_BIG(1)>1)
     THROW 51208,'ItemOrigins contains duplicate GlobalId values.',1;
 IF EXISTS(SELECT Code FROM dbo.ItemOrigins GROUP BY Code HAVING COUNT_BIG(1)>1)
     THROW 51208,'ItemOrigins contains duplicate exact codes.',1;
 COMMIT;
END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH;
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemOrigins') AND name=N'UQ_ItemOrigins_GlobalId')
 CREATE UNIQUE INDEX UQ_ItemOrigins_GlobalId ON dbo.ItemOrigins(GlobalId);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemOrigins') AND name=N'UQ_ItemOrigins_Code')
 CREATE UNIQUE INDEX UQ_ItemOrigins_Code ON dbo.ItemOrigins(Code);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemOrigins') AND name=N'IX_ItemOrigins_Active_SortOrder_Name')
 CREATE INDEX IX_ItemOrigins_Active_SortOrder_Name ON dbo.ItemOrigins(IsActive,SortOrder,Name)
 INCLUDE(Code,GlobalId) WHERE IsDeleted=0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemOrigins_LISTAR AS
BEGIN SET NOCOUNT ON;
 SELECT Id,GlobalId,Code,Name,Description,SortOrder,IsActive,
  CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,
  DeletedByUserId,DeletedByUserName,DeletedAt FROM dbo.ItemOrigins WHERE IsDeleted=0 ORDER BY SortOrder,Name,Code;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemOrigins_LOOKUP @IncludeCode nvarchar(50)=NULL AS
BEGIN SET NOCOUNT ON;
 SELECT Id,GlobalId,Code,Name,SortOrder,CAST(IsActive AS bit) IsActive
 FROM dbo.ItemOrigins WHERE IsDeleted=0 AND
 (IsActive=1 OR Code=NULLIF(LTRIM(RTRIM(@IncludeCode)),N'') COLLATE Latin1_General_100_BIN2)
 ORDER BY SortOrder,Name,Code;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemOrigins_BUSCARPORID @Id int AS
BEGIN SET NOCOUNT ON;
 SELECT TOP(1) Id,GlobalId,Code,Name,Description,SortOrder,IsActive,
  CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,
  DeletedByUserId,DeletedByUserName,DeletedAt FROM dbo.ItemOrigins WHERE Id=@Id AND IsDeleted=0;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemOrigins_BUSCARPORCODIGO @Code nvarchar(50),@ExcluirId int=NULL AS
BEGIN SET NOCOUNT ON;
 SELECT COUNT(1) FROM dbo.ItemOrigins WITH(UPDLOCK,HOLDLOCK)
 WHERE Code=LTRIM(RTRIM(@Code)) COLLATE Latin1_General_100_BIN2 AND (@ExcluirId IS NULL OR Id<>@ExcluirId);
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemOrigins_BUSCARPORCODIGO_DETALLE @Code nvarchar(50) AS
BEGIN SET NOCOUNT ON;
 SELECT TOP(1) Id,GlobalId,Code,Name,Description,SortOrder,IsActive,
  CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,
  DeletedByUserId,DeletedByUserName,DeletedAt
 FROM dbo.ItemOrigins
 WHERE Code=LTRIM(RTRIM(@Code)) COLLATE Latin1_General_100_BIN2 AND IsDeleted=0;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemOrigins_HISTORIAL @Id int AS
BEGIN SET NOCOUNT ON;
 SELECT Id,EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName,[Source],CreatedAt
 FROM dbo.AuditCatalogChanges WHERE EntityName=N'ItemOrigin' AND RecordId=CONVERT(nvarchar(80),@Id)
 ORDER BY CreatedAt DESC,Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_GENERAL_INVENTORY_ItemOrigins_CREAR
 @GlobalId uniqueidentifier=NULL,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,
 @SortOrder int=0,@IsActive bit=1,@CreatedByUserId int=NULL,@CreatedByUserName nvarchar(120)=NULL AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @GlobalId=COALESCE(@GlobalId,NEWID()); SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name)); SET @Description=NULLIF(LTRIM(RTRIM(@Description)),N'');
 IF @GlobalId='00000000-0000-0000-0000-000000000000' THROW 51208,'ItemOrigin GlobalId is invalid.',1;
 IF NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL OR @SortOrder<0 THROW 51208,'ItemOrigin data is invalid.',1;
 BEGIN TRY
  DECLARE @Own bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END; IF @Own=1 BEGIN TRANSACTION;
  IF EXISTS(SELECT 1 FROM dbo.ItemOrigins WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code COLLATE Latin1_General_100_BIN2 OR GlobalId=@GlobalId)
  BEGIN IF @Own=1 COMMIT; SELECT -1; RETURN; END;
  INSERT dbo.ItemOrigins(GlobalId,Code,Name,Description,SortOrder,IsActive,CreatedByUserId,CreatedByUserName)
  VALUES(@GlobalId,@Code,@Name,@Description,@SortOrder,@IsActive,@CreatedByUserId,@CreatedByUserName);
  DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'ItemOrigin',CONVERT(nvarchar(80),@Id),N'INSERT',FieldName,NULL,NewValue,@CreatedByUserId,@CreatedByUserName
  FROM(VALUES(N'GlobalId',CONVERT(nvarchar(max),@GlobalId)),(N'Code',@Code),(N'Name',@Name),(N'Description',@Description),
   (N'SortOrder',CONVERT(nvarchar(max),@SortOrder)),(N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@IsActive))))v(FieldName,NewValue);
  IF @Own=1 COMMIT; SELECT @Id;
 END TRY BEGIN CATCH IF @Own=1 AND XACT_STATE()<>0 ROLLBACK; IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END; THROW; END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_GENERAL_INVENTORY_ItemOrigins_ACTUALIZAR
 @Id int,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,
 @UpdatedByUserId int=NULL,@UpdatedByUserName nvarchar(120)=NULL AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name)); SET @Description=NULLIF(LTRIM(RTRIM(@Description)),N'');
 IF NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL OR @SortOrder<0 THROW 51208,'ItemOrigin data is invalid.',1;
 BEGIN TRY
  DECLARE @Own bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END; IF @Own=1 BEGIN TRANSACTION;
  DECLARE @OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldSortOrder int,@OldIsActive bit;
  SELECT @OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSortOrder=SortOrder,@OldIsActive=IsActive
  FROM dbo.ItemOrigins WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
  IF @OldCode IS NULL BEGIN IF @Own=1 COMMIT; SELECT 0; RETURN; END;
  IF EXISTS(SELECT 1 FROM dbo.ItemOrigins WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code COLLATE Latin1_General_100_BIN2 AND Id<>@Id)
  BEGIN IF @Own=1 COMMIT; SELECT -1; RETURN; END;
  UPDATE dbo.ItemOrigins SET Code=@Code,Name=@Name,Description=@Description,SortOrder=@SortOrder,IsActive=@IsActive,
   UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName,UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'ItemOrigin',CONVERT(nvarchar(80),@Id),N'UPDATE',FieldName,OldValue,NewValue,@UpdatedByUserId,@UpdatedByUserName
  FROM(VALUES(N'Code',@OldCode,@Code),(N'Name',@OldName,@Name),(N'Description',@OldDescription,@Description),
   (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
   (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))))v(FieldName,OldValue,NewValue)
  WHERE ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
  IF @Own=1 COMMIT; SELECT 1;
 END TRY BEGIN CATCH IF @Own=1 AND XACT_STATE()<>0 ROLLBACK; IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END; THROW; END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_ItemOrigins_ELIMINAR
 @Id int,@DeletedByUserId int=NULL,@DeletedByUserName nvarchar(120)=NULL AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  DECLARE @Own bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END; IF @Own=1 BEGIN TRANSACTION;
  DECLARE @Code nvarchar(50),@OldIsActive bit;
  SELECT @Code=Code,@OldIsActive=IsActive FROM dbo.ItemOrigins WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
  IF @Code IS NULL BEGIN IF @Own=1 COMMIT; SELECT 0; RETURN; END;
  IF EXISTS(SELECT 1 FROM dbo.ItemMasterProfiles profile WITH(UPDLOCK,HOLDLOCK)
    WHERE ISJSON(profile.MasterDataJson)=1
      AND JSON_VALUE(profile.MasterDataJson,N'$.general.origin') COLLATE Latin1_General_100_BIN2=@Code)
  BEGIN IF @Own=1 COMMIT; SELECT -3; RETURN; END;
  UPDATE dbo.ItemOrigins SET IsActive=0,IsDeleted=1,DeletedByUserId=@DeletedByUserId,DeletedByUserName=@DeletedByUserName,
   DeletedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
  DECLARE @Affected int=@@ROWCOUNT;
  IF @Affected>0
   INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
   VALUES(N'ItemOrigin',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),N'0',@DeletedByUserId,@DeletedByUserName),
         (N'ItemOrigin',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);
  IF @Own=1 COMMIT; SELECT @Affected;
 END TRY BEGIN CATCH IF @Own=1 AND XACT_STATE()<>0 ROLLBACK; THROW; END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_ORIGIN_SYNC_FULL @AfterId int=NULL,@BatchSize int=100 AS
BEGIN SET NOCOUNT ON;
 IF @BatchSize<1 OR @BatchSize>10001 THROW 51208,'ItemOrigin Full BatchSize must be between 1 and 10001.',1;
 SELECT TOP(@BatchSize) Id,GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedAt,UpdatedAt
 FROM dbo.ItemOrigins WHERE @AfterId IS NULL OR Id>@AfterId ORDER BY Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_ORIGIN_SYNC_APPLY
 @GlobalId uniqueidentifier,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,
 @IsActive bit,@IsDeleted bit,@UpdatedAt datetime2(0)=NULL AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name)); SET @Description=NULLIF(LTRIM(RTRIM(@Description)),N'');
 IF @GlobalId IS NULL OR @GlobalId='00000000-0000-0000-0000-000000000000' OR NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL OR @SortOrder<0
  THROW 51208,'ItemOrigin sync payload is invalid.',1;
 DECLARE @Id int,@ConflictId int,@WasNew bit=0,@OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),
  @OldSortOrder int,@OldIsActive bit,@OldIsDeleted bit;
 SELECT @Id=Id,@OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSortOrder=SortOrder,
  @OldIsActive=IsActive,@OldIsDeleted=IsDeleted FROM dbo.ItemOrigins WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
 SELECT @ConflictId=Id FROM dbo.ItemOrigins WITH(UPDLOCK,HOLDLOCK)
 WHERE Code=@Code COLLATE Latin1_General_100_BIN2 AND (@Id IS NULL OR Id<>@Id);
 IF @ConflictId IS NOT NULL BEGIN SELECT -2 ResultCode,CONVERT(int,NULL) ItemOriginId; RETURN; END;
 IF @Id IS NULL
 BEGIN
  SET @WasNew=1;
  INSERT dbo.ItemOrigins(GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedAt,CreatedByUserName,DeletedAt,DeletedByUserName)
  VALUES(@GlobalId,@Code,@Name,@Description,@SortOrder,@IsActive,@IsDeleted,COALESCE(@UpdatedAt,SYSUTCDATETIME()),N'MasterBranchSyncWorker',
   CASE WHEN @IsDeleted=1 THEN COALESCE(@UpdatedAt,SYSUTCDATETIME()) END,CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END);
  SET @Id=CONVERT(int,SCOPE_IDENTITY());
 END
 ELSE UPDATE dbo.ItemOrigins SET Code=@Code,Name=@Name,Description=@Description,SortOrder=@SortOrder,
  IsActive=@IsActive,IsDeleted=@IsDeleted,UpdatedAt=COALESCE(@UpdatedAt,SYSUTCDATETIME()),UpdatedByUserName=N'MasterBranchSyncWorker',
  DeletedAt=CASE WHEN @IsDeleted=1 THEN COALESCE(@UpdatedAt,SYSUTCDATETIME()) END,
  DeletedByUserName=CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END WHERE Id=@Id;
 INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
 SELECT N'ItemOrigin',CONVERT(nvarchar(80),@Id),CASE WHEN @WasNew=1 THEN N'INSERT' WHEN @IsDeleted=1 AND ISNULL(@OldIsDeleted,0)=0 THEN N'DELETE' ELSE N'UPDATE' END,
  FieldName,CASE WHEN @WasNew=1 THEN NULL ELSE OldValue END,NewValue,N'MasterBranchSyncWorker',N'MasterBranchSyncWorker'
 FROM(VALUES(N'Code',@OldCode,@Code),(N'Name',@OldName,@Name),(N'Description',@OldDescription,@Description),
  (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
  (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
  (N'IsDeleted',CONVERT(nvarchar(max),CONVERT(int,@OldIsDeleted)),CONVERT(nvarchar(max),CONVERT(int,@IsDeleted))))v(FieldName,OldValue,NewValue)
 WHERE @WasNew=1 OR ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
 SELECT 1 ResultCode,@Id ItemOriginId;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260813.208')
 INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260813.208',N'ItemOrigins con seeds deterministas, preservacion legacy, auditoria y sync por GlobalId');
GO
