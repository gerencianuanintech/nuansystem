/*
  Evoluciona dbo.ReplenishmentMethods creado por 044 sin recrear la tabla ni
  reinterpretar COMPRAR/FABRICAR/TRANSFERIR. Preserva Id, Code y referencias
  historicas en ItemMasterProfiles. Prerrequisitos: 043, 044, 065 y 106.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
IF DB_NAME()=N'NuanSystem_Master' THROW 51211,'Migration 211 must run only in tenant databases.',1;
IF OBJECT_ID(N'dbo.ReplenishmentMethods',N'U') IS NULL THROW 51211,'ReplenishmentMethods from migration 044 is required.',1;
IF OBJECT_ID(N'dbo.ItemMasterProfiles',N'U') IS NULL THROW 51211,'ItemMasterProfiles from migration 043 is required.',1;
IF OBJECT_ID(N'dbo.LocalOutbox',N'U') IS NULL THROW 51211,'LocalOutbox from migration 065 is required.',1;
IF OBJECT_ID(N'dbo.AuditCatalogChanges',N'U') IS NULL THROW 51211,'AuditCatalogChanges from migration 106 is required.',1;
IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NULL THROW 51211,'SchemaHistory is required.',1;
GO

IF EXISTS(SELECT Code FROM dbo.ReplenishmentMethods GROUP BY Code HAVING COUNT_BIG(1)>1)
 THROW 51211,'ReplenishmentMethods contains ambiguous duplicate codes, including tombstones.',1;
IF EXISTS(SELECT 1 FROM dbo.ReplenishmentMethods WHERE NULLIF(LTRIM(RTRIM(Code)),N'') IS NULL OR NULLIF(LTRIM(RTRIM(Name)),N'') IS NULL)
 THROW 51211,'ReplenishmentMethods contains blank codes or names.',1;
IF EXISTS(SELECT 1 FROM dbo.ItemMasterProfiles WHERE ISJSON(MasterDataJson)=1
 AND LEN(NULLIF(JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod'),N''))>50)
 THROW 51211,'A legacy replenishment method exceeds 50 characters; map it explicitly. Values are never truncated.',1;
IF EXISTS(SELECT 1 FROM dbo.ItemMasterProfiles WHERE ISJSON(MasterDataJson)=1
 AND NULLIF(JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod'),N'') IS NOT NULL
 AND JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod') COLLATE Latin1_General_100_BIN2<>
     LTRIM(RTRIM(JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod'))) COLLATE Latin1_General_100_BIN2)
 THROW 51211,'A legacy JSON replenishment method has leading or trailing whitespace; map it explicitly. Values are never normalized silently.',1;
IF EXISTS
(
 SELECT 1
 FROM
 (
  SELECT DISTINCT JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod') COLLATE Latin1_General_100_BIN2 Code
  FROM dbo.ItemMasterProfiles
  WHERE ISJSON(MasterDataJson)=1
    AND NULLIF(JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod'),N'') IS NOT NULL
 ) legacy
 GROUP BY legacy.Code COLLATE DATABASE_DEFAULT
 HAVING COUNT_BIG(1)>1
)
 THROW 51211,'Legacy JSON contains codes that are distinct exactly but collide under the database collation; reconcile them explicitly.',1;
IF EXISTS
(
 SELECT 1
 FROM dbo.ItemMasterProfiles profile
 JOIN dbo.ReplenishmentMethods method
   ON method.Code=JSON_VALUE(profile.MasterDataJson,N'$.inventory.replenishmentMethod')
 WHERE ISJSON(profile.MasterDataJson)=1
 AND method.Code COLLATE Latin1_General_100_BIN2<>
     JSON_VALUE(profile.MasterDataJson,N'$.inventory.replenishmentMethod') COLLATE Latin1_General_100_BIN2
)
 THROW 51211,'A legacy JSON code differs only by collation/casing from ReplenishmentMethods; reconcile explicitly.',1;
GO

IF OBJECT_ID(N'tempdb..#Migration211State',N'U') IS NOT NULL DROP TABLE #Migration211State;
CREATE TABLE #Migration211State(GlobalIdColumnAdded bit NOT NULL);
INSERT #Migration211State VALUES(CASE WHEN COL_LENGTH(N'dbo.ReplenishmentMethods',N'GlobalId') IS NULL THEN 1 ELSE 0 END);
IF COL_LENGTH(N'dbo.ReplenishmentMethods',N'GlobalId') IS NULL
 ALTER TABLE dbo.ReplenishmentMethods ADD GlobalId uniqueidentifier NULL;
IF COL_LENGTH(N'dbo.ReplenishmentMethods',N'SortOrder') IS NULL
 ALTER TABLE dbo.ReplenishmentMethods ADD SortOrder int NULL;
GO

/*
  Recuperación idempotente de una ejecución interrumpida después del ALTER:
  si 211 aún no está registrado y la columna no contiene ninguna identidad,
  puede tratarse como recién agregada sin sobrescribir un GlobalId real.
*/
UPDATE #Migration211State
SET GlobalIdColumnAdded=1
WHERE GlobalIdColumnAdded=0
  AND NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260813.211')
  AND NOT EXISTS(SELECT 1 FROM dbo.ReplenishmentMethods WHERE GlobalId IS NOT NULL);
GO

BEGIN TRY
 BEGIN TRANSACTION;
 UPDATE dbo.ReplenishmentMethods SET SortOrder=0 WHERE SortOrder IS NULL;

 DECLARE @Functional table(GlobalId uniqueidentifier,Code nvarchar(50),Name nvarchar(150),Description nvarchar(500),SortOrder int);
 INSERT @Functional VALUES
 ('5a9f26f7-c93a-4a27-b241-8ad7d3701001',N'MANUAL',N'Manual',N'Reposicion iniciada manualmente.',10),
 ('5a9f26f7-c93a-4a27-b241-8ad7d3701002',N'PUNTO_REORDEN',N'Punto de reorden',N'Reposicion al alcanzar el punto de reorden.',20),
 ('5a9f26f7-c93a-4a27-b241-8ad7d3701003',N'MIN_MAX',N'Minimo / maximo',N'Reposicion para recuperar el nivel maximo.',30),
 ('5a9f26f7-c93a-4a27-b241-8ad7d3701004',N'REVISION_PERIODICA',N'Revision periodica',N'Reposicion evaluada en intervalos definidos.',40),
 ('5a9f26f7-c93a-4a27-b241-8ad7d3701005',N'BAJO_DEMANDA',N'Bajo demanda',N'Reposicion disparada por demanda confirmada.',50),
 ('5a9f26f7-c93a-4a27-b241-8ad7d3701006',N'MRP',N'MRP',N'Reposicion calculada por planificacion de requerimientos.',60);

 IF EXISTS(SELECT 1 FROM @Functional seed JOIN dbo.ReplenishmentMethods existing ON existing.Code=seed.Code WHERE existing.Code COLLATE Latin1_General_100_BIN2<>seed.Code COLLATE Latin1_General_100_BIN2)
  THROW 51211,'An existing functional ReplenishmentMethod differs only by casing; reconcile it explicitly.',1;
 IF EXISTS(SELECT 1 FROM @Functional seed JOIN dbo.ReplenishmentMethods existing ON existing.GlobalId=seed.GlobalId WHERE existing.Code<>seed.Code)
  THROW 51211,'A deterministic ReplenishmentMethod GlobalId belongs to another code.',1;
 IF EXISTS(SELECT 1 FROM #Migration211State WHERE GlobalIdColumnAdded=0) AND EXISTS
 (
  SELECT 1 FROM @Functional seed JOIN dbo.ReplenishmentMethods existing ON existing.Code=seed.Code
  WHERE existing.GlobalId IS NULL OR existing.GlobalId<>seed.GlobalId
 )
  THROW 51211,'An existing functional ReplenishmentMethod already has a different or missing GlobalId; reconcile it explicitly.',1;
 IF EXISTS(SELECT 1 FROM #Migration211State WHERE GlobalIdColumnAdded=1)
 BEGIN
  UPDATE existing SET GlobalId=seed.GlobalId
  FROM dbo.ReplenishmentMethods existing JOIN @Functional seed
    ON seed.Code COLLATE Latin1_General_100_BIN2=existing.Code COLLATE Latin1_General_100_BIN2;
  UPDATE dbo.ReplenishmentMethods SET GlobalId=NEWID() WHERE GlobalId IS NULL;
 END;
 IF EXISTS(SELECT 1 FROM dbo.ReplenishmentMethods WHERE GlobalId IS NULL)
  THROW 51211,'ReplenishmentMethods has missing GlobalId values in a pre-existing GlobalId column; reconcile them explicitly.',1;
 INSERT dbo.ReplenishmentMethods(GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedByUserName,CreatedAt)
 SELECT seed.GlobalId,seed.Code,seed.Name,seed.Description,seed.SortOrder,1,0,N'Sistema',SYSUTCDATETIME()
 FROM @Functional seed WHERE NOT EXISTS(SELECT 1 FROM dbo.ReplenishmentMethods existing WHERE existing.Code=seed.Code);

 ;WITH Legacy AS
 (
  SELECT DISTINCT JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod') COLLATE Latin1_General_100_BIN2 Code
  FROM dbo.ItemMasterProfiles WHERE ISJSON(MasterDataJson)=1
  AND NULLIF(JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod'),N'') IS NOT NULL
 )
 INSERT dbo.ReplenishmentMethods(GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedByUserName,CreatedAt)
 SELECT NEWID(),legacy.Code,legacy.Code,N'Valor historico preservado desde ItemMasterProfiles.',1000,1,0,N'Migracion 211',SYSUTCDATETIME()
 FROM Legacy legacy WHERE NOT EXISTS(SELECT 1 FROM dbo.ReplenishmentMethods existing WHERE existing.Code=legacy.Code COLLATE Latin1_General_100_BIN2);

 IF EXISTS(SELECT GlobalId FROM dbo.ReplenishmentMethods GROUP BY GlobalId HAVING COUNT_BIG(1)>1) THROW 51211,'Duplicate ReplenishmentMethods GlobalId.',1;
 ALTER TABLE dbo.ReplenishmentMethods ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
 ALTER TABLE dbo.ReplenishmentMethods ALTER COLUMN SortOrder int NOT NULL;
 COMMIT;
END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH;
DROP TABLE #Migration211State;
GO

IF NOT EXISTS(SELECT 1 FROM sys.default_constraints d JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id WHERE d.parent_object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND c.name=N'GlobalId')
 ALTER TABLE dbo.ReplenishmentMethods ADD CONSTRAINT DF_ReplenishmentMethods_GlobalId DEFAULT NEWID() FOR GlobalId;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints d JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id WHERE d.parent_object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND c.name=N'SortOrder')
 ALTER TABLE dbo.ReplenishmentMethods ADD CONSTRAINT DF_ReplenishmentMethods_SortOrder DEFAULT(0) FOR SortOrder;
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND name=N'CK_ReplenishmentMethods_GlobalId')
 ALTER TABLE dbo.ReplenishmentMethods ADD CONSTRAINT CK_ReplenishmentMethods_GlobalId CHECK(GlobalId<>'00000000-0000-0000-0000-000000000000');
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND name=N'CK_ReplenishmentMethods_Code_NotBlank')
 ALTER TABLE dbo.ReplenishmentMethods ADD CONSTRAINT CK_ReplenishmentMethods_Code_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Code)),N'') IS NOT NULL);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND name=N'CK_ReplenishmentMethods_Name_NotBlank')
 ALTER TABLE dbo.ReplenishmentMethods ADD CONSTRAINT CK_ReplenishmentMethods_Name_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Name)),N'') IS NOT NULL);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND name=N'CK_ReplenishmentMethods_SortOrder')
 ALTER TABLE dbo.ReplenishmentMethods ADD CONSTRAINT CK_ReplenishmentMethods_SortOrder CHECK(SortOrder>=0);
IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND name=N'UX_ReplenishmentMethods_Code_Active')
 DROP INDEX UX_ReplenishmentMethods_Code_Active ON dbo.ReplenishmentMethods;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND name=N'UQ_ReplenishmentMethods_Code')
 CREATE UNIQUE INDEX UQ_ReplenishmentMethods_Code ON dbo.ReplenishmentMethods(Code);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND name=N'UQ_ReplenishmentMethods_GlobalId')
 CREATE UNIQUE INDEX UQ_ReplenishmentMethods_GlobalId ON dbo.ReplenishmentMethods(GlobalId);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ReplenishmentMethods') AND name=N'IX_ReplenishmentMethods_Active_SortOrder_Name')
 CREATE INDEX IX_ReplenishmentMethods_Active_SortOrder_Name ON dbo.ReplenishmentMethods(IsActive,SortOrder,Name) INCLUDE(Code,GlobalId) WHERE IsDeleted=0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ReplenishmentMethods_LISTAR AS
BEGIN SET NOCOUNT ON; SELECT Id,GlobalId,Code,Name,Description,SortOrder,IsActive,CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,DeletedByUserId,DeletedByUserName,DeletedAt FROM dbo.ReplenishmentMethods WHERE IsDeleted=0 ORDER BY SortOrder,Name,Code; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ReplenishmentMethods_LOOKUP @IncludeCode nvarchar(50)=NULL AS
BEGIN SET NOCOUNT ON; SELECT Id,GlobalId,Code,Name,Description,SortOrder,CAST(IsActive AS bit) IsActive FROM dbo.ReplenishmentMethods WHERE IsDeleted=0 AND (IsActive=1 OR Code=NULLIF(LTRIM(RTRIM(@IncludeCode)),N'') COLLATE Latin1_General_100_BIN2) ORDER BY SortOrder,Name,Code; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ReplenishmentMethods_BUSCARPORID @Id int AS
BEGIN SET NOCOUNT ON; SELECT TOP(1) Id,GlobalId,Code,Name,Description,SortOrder,IsActive,CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,DeletedByUserId,DeletedByUserName,DeletedAt FROM dbo.ReplenishmentMethods WHERE Id=@Id AND IsDeleted=0; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ReplenishmentMethods_BUSCARPORCODIGO @Code nvarchar(50),@ExcluirId int=NULL AS
BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.ReplenishmentMethods WITH(UPDLOCK,HOLDLOCK) WHERE Code=LTRIM(RTRIM(@Code)) AND (@ExcluirId IS NULL OR Id<>@ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ReplenishmentMethods_BUSCARPORCODIGO_DETALLE @Code nvarchar(50) AS
BEGIN SET NOCOUNT ON; SELECT TOP(1) Id,GlobalId,Code,Name,Description,SortOrder,IsActive,CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,DeletedByUserId,DeletedByUserName,DeletedAt FROM dbo.ReplenishmentMethods WHERE Code=LTRIM(RTRIM(@Code)) COLLATE Latin1_General_100_BIN2 AND IsDeleted=0; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ReplenishmentMethods_HISTORIAL @Id int AS
BEGIN SET NOCOUNT ON; SELECT Id,EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName,[Source],CreatedAt FROM dbo.AuditCatalogChanges WHERE EntityName=N'ReplenishmentMethod' AND RecordId=CONVERT(nvarchar(80),@Id) ORDER BY CreatedAt DESC,Id DESC; END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_GENERAL_INVENTORY_ReplenishmentMethods_CREAR @GlobalId uniqueidentifier=NULL,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,@CreatedByUserId int=NULL,@CreatedByUserName nvarchar(120)=NULL AS
BEGIN SET NOCOUNT ON; SET XACT_ABORT ON; SET @GlobalId=COALESCE(@GlobalId,NEWID()); SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name)); SET @Description=NULLIF(LTRIM(RTRIM(@Description)),N'');
 IF @GlobalId='00000000-0000-0000-0000-000000000000' OR NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL OR @SortOrder<0 THROW 51211,'Invalid ReplenishmentMethod data.',1;
 BEGIN TRY DECLARE @Own bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END; IF @Own=1 BEGIN TRANSACTION;
 IF EXISTS(SELECT 1 FROM dbo.ReplenishmentMethods WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code OR GlobalId=@GlobalId) BEGIN IF @Own=1 COMMIT; SELECT -1; RETURN; END;
 INSERT dbo.ReplenishmentMethods(GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedByUserId,CreatedByUserName,CreatedAt) VALUES(@GlobalId,@Code,@Name,@Description,@SortOrder,@IsActive,0,@CreatedByUserId,@CreatedByUserName,SYSUTCDATETIME()); DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName) SELECT N'ReplenishmentMethod',CONVERT(nvarchar(80),@Id),N'INSERT',FieldName,NULL,NewValue,@CreatedByUserId,@CreatedByUserName FROM(VALUES(N'GlobalId',CONVERT(nvarchar(max),@GlobalId)),(N'Code',@Code),(N'Name',@Name),(N'Description',@Description),(N'SortOrder',CONVERT(nvarchar(max),@SortOrder)),(N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@IsActive))))v(FieldName,NewValue);
 IF @Own=1 COMMIT; SELECT @Id; END TRY BEGIN CATCH IF @Own=1 AND XACT_STATE()<>0 ROLLBACK; IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END; THROW; END CATCH; END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_GENERAL_INVENTORY_ReplenishmentMethods_ACTUALIZAR @Id int,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,@UpdatedByUserId int=NULL,@UpdatedByUserName nvarchar(120)=NULL AS
BEGIN SET NOCOUNT ON; SET XACT_ABORT ON; SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name)); SET @Description=NULLIF(LTRIM(RTRIM(@Description)),N''); IF NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL OR @SortOrder<0 THROW 51211,'Invalid ReplenishmentMethod data.',1;
 BEGIN TRY DECLARE @Own bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END; IF @Own=1 BEGIN TRANSACTION; DECLARE @OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldSortOrder int,@OldIsActive bit;
 SELECT @OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSortOrder=SortOrder,@OldIsActive=IsActive FROM dbo.ReplenishmentMethods WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0; IF @OldCode IS NULL BEGIN IF @Own=1 COMMIT; SELECT 0; RETURN; END;
 IF @OldCode COLLATE Latin1_General_100_BIN2<>@Code COLLATE Latin1_General_100_BIN2 AND EXISTS(SELECT 1 FROM dbo.ItemMasterProfiles WHERE ISJSON(MasterDataJson)=1 AND JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod') COLLATE Latin1_General_100_BIN2=@OldCode COLLATE Latin1_General_100_BIN2) BEGIN IF @Own=1 COMMIT; SELECT -3; RETURN; END;
 IF EXISTS(SELECT 1 FROM dbo.ReplenishmentMethods WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND Id<>@Id) BEGIN IF @Own=1 COMMIT; SELECT -1; RETURN; END;
 UPDATE dbo.ReplenishmentMethods SET Code=@Code,Name=@Name,Description=@Description,SortOrder=@SortOrder,IsActive=@IsActive,UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName,UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
 INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName) SELECT N'ReplenishmentMethod',CONVERT(nvarchar(80),@Id),N'UPDATE',FieldName,OldValue,NewValue,@UpdatedByUserId,@UpdatedByUserName FROM(VALUES(N'Code',@OldCode,@Code),(N'Name',@OldName,@Name),(N'Description',@OldDescription,@Description),(N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),(N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))))v(FieldName,OldValue,NewValue) WHERE ISNULL(OldValue,N'')<>ISNULL(NewValue,N''); IF @Own=1 COMMIT; SELECT 1;
 END TRY BEGIN CATCH IF @Own=1 AND XACT_STATE()<>0 ROLLBACK; IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END; THROW; END CATCH; END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_ReplenishmentMethods_ELIMINAR @Id int,@DeletedByUserId int=NULL,@DeletedByUserName nvarchar(120)=NULL AS
BEGIN SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRY DECLARE @Own bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END; IF @Own=1 BEGIN TRANSACTION; DECLARE @Code nvarchar(50),@OldIsActive bit; SELECT @Code=Code,@OldIsActive=IsActive FROM dbo.ReplenishmentMethods WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0; IF @Code IS NULL BEGIN IF @Own=1 COMMIT; SELECT 0; RETURN; END;
 IF EXISTS(SELECT 1 FROM dbo.ItemMasterProfiles WITH(UPDLOCK,HOLDLOCK) WHERE ISJSON(MasterDataJson)=1 AND JSON_VALUE(MasterDataJson,N'$.inventory.replenishmentMethod') COLLATE Latin1_General_100_BIN2=@Code COLLATE Latin1_General_100_BIN2) BEGIN IF @Own=1 COMMIT; SELECT -3; RETURN; END;
 UPDATE dbo.ReplenishmentMethods SET IsActive=0,IsDeleted=1,DeletedByUserId=@DeletedByUserId,DeletedByUserName=@DeletedByUserName,DeletedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
 INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName) VALUES(N'ReplenishmentMethod',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),N'0',@DeletedByUserId,@DeletedByUserName),(N'ReplenishmentMethod',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName); IF @Own=1 COMMIT; SELECT 1; END TRY BEGIN CATCH IF @Own=1 AND XACT_STATE()<>0 ROLLBACK; THROW; END CATCH; END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_REPLENISHMENT_METHOD_SYNC_FULL @AfterId int=NULL,@BatchSize int=100 AS
BEGIN SET NOCOUNT ON; IF @BatchSize<1 OR @BatchSize>10001 THROW 51211,'Invalid Full BatchSize.',1; SELECT TOP(@BatchSize) Id,GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedAt,UpdatedAt FROM dbo.ReplenishmentMethods WHERE @AfterId IS NULL OR Id>@AfterId ORDER BY Id; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_REPLENISHMENT_METHOD_SYNC_APPLY @GlobalId uniqueidentifier,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit,@IsDeleted bit,@UpdatedAt datetime2(0)=NULL AS
BEGIN SET NOCOUNT ON; SET XACT_ABORT ON; SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name)); SET @Description=NULLIF(LTRIM(RTRIM(@Description)),N''); IF @GlobalId IS NULL OR @GlobalId='00000000-0000-0000-0000-000000000000' OR NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL OR @SortOrder<0 THROW 51211,'Invalid sync payload.',1;
 DECLARE @Id int,@ConflictId int,@WasNew bit=0,@OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldSortOrder int,@OldIsActive bit,@OldIsDeleted bit; SELECT @Id=Id,@OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSortOrder=SortOrder,@OldIsActive=IsActive,@OldIsDeleted=IsDeleted FROM dbo.ReplenishmentMethods WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId; SELECT @ConflictId=Id FROM dbo.ReplenishmentMethods WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND (@Id IS NULL OR Id<>@Id); IF @ConflictId IS NOT NULL BEGIN SELECT -2 ResultCode,CONVERT(int,NULL) ReplenishmentMethodId; RETURN; END;
 IF @Id IS NULL BEGIN SET @WasNew=1; INSERT dbo.ReplenishmentMethods(GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedAt,CreatedByUserName,DeletedAt,DeletedByUserName) VALUES(@GlobalId,@Code,@Name,@Description,@SortOrder,@IsActive,@IsDeleted,COALESCE(@UpdatedAt,SYSUTCDATETIME()),N'MasterBranchSyncWorker',CASE WHEN @IsDeleted=1 THEN COALESCE(@UpdatedAt,SYSUTCDATETIME()) END,CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END); SET @Id=CONVERT(int,SCOPE_IDENTITY()); END ELSE UPDATE dbo.ReplenishmentMethods SET Code=@Code,Name=@Name,Description=@Description,SortOrder=@SortOrder,IsActive=@IsActive,IsDeleted=@IsDeleted,UpdatedAt=COALESCE(@UpdatedAt,SYSUTCDATETIME()),UpdatedByUserName=N'MasterBranchSyncWorker',DeletedAt=CASE WHEN @IsDeleted=1 THEN COALESCE(@UpdatedAt,SYSUTCDATETIME()) END,DeletedByUserName=CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END WHERE Id=@Id;
 INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source]) SELECT N'ReplenishmentMethod',CONVERT(nvarchar(80),@Id),CASE WHEN @WasNew=1 THEN N'INSERT' WHEN @IsDeleted=1 AND ISNULL(@OldIsDeleted,0)=0 THEN N'DELETE' ELSE N'UPDATE' END,FieldName,CASE WHEN @WasNew=1 THEN NULL ELSE OldValue END,NewValue,N'MasterBranchSyncWorker',N'MasterBranchSyncWorker' FROM(VALUES(N'Code',@OldCode,@Code),(N'Name',@OldName,@Name),(N'Description',@OldDescription,@Description),(N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),(N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),(N'IsDeleted',CONVERT(nvarchar(max),CONVERT(int,@OldIsDeleted)),CONVERT(nvarchar(max),CONVERT(int,@IsDeleted))))v(FieldName,OldValue,NewValue) WHERE @WasNew=1 OR ISNULL(OldValue,N'')<>ISNULL(NewValue,N''); SELECT 1 ResultCode,@Id ReplenishmentMethodId; END;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260813.211') INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260813.211',N'Evoluciona ReplenishmentMethods con legacy JSON, auditoria, LocalOutbox y sync GlobalId');
GO
