/*
  Evoluciona dbo.SalesChannels creado por 044 sin recrear la tabla.
  Preserva Id, Code, Name, Description y los valores usados por ItemEdit.
  Prerrequisitos: 044, 106 y SchemaHistory.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
IF DB_NAME()=N'NuanSystem_Master' THROW 51226,'Migration 226 must run only in tenant databases.',1;
IF OBJECT_ID(N'dbo.SalesChannels',N'U') IS NULL THROW 51226,'SalesChannels from migration 044 is required.',1;
IF OBJECT_ID(N'dbo.AuditCatalogChanges',N'U') IS NULL THROW 51226,'AuditCatalogChanges is required.',1;
IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NULL THROW 51226,'SchemaHistory is required.',1;
IF EXISTS(SELECT Code FROM dbo.SalesChannels GROUP BY Code HAVING COUNT_BIG(1)>1)
 THROW 51226,'SalesChannels contains duplicate codes; reconcile them before migration.',1;
IF EXISTS(SELECT 1 FROM dbo.SalesChannels WHERE NULLIF(LTRIM(RTRIM(Code)),N'') IS NULL OR NULLIF(LTRIM(RTRIM(Name)),N'') IS NULL)
 THROW 51226,'SalesChannels contains blank codes or names.',1;
GO

IF COL_LENGTH(N'dbo.SalesChannels',N'GlobalId') IS NULL
 ALTER TABLE dbo.SalesChannels ADD GlobalId uniqueidentifier NULL;
IF COL_LENGTH(N'dbo.SalesChannels',N'SortOrder') IS NULL
 ALTER TABLE dbo.SalesChannels ADD SortOrder int NULL;
GO

BEGIN TRY
 BEGIN TRANSACTION;
 UPDATE dbo.SalesChannels
 SET GlobalId=CASE Code
  WHEN N'LOCAL' THEN 'ebf9d7fd-d4bd-4c1d-94e0-9d7f22601001'
  WHEN N'ECOMMERCE' THEN 'ebf9d7fd-d4bd-4c1d-94e0-9d7f22601002'
  WHEN N'MAYORISTA' THEN 'ebf9d7fd-d4bd-4c1d-94e0-9d7f22601003'
  ELSE NEWID()
 END
 WHERE GlobalId IS NULL;
 UPDATE dbo.SalesChannels
 SET SortOrder=CASE Code WHEN N'LOCAL' THEN 10 WHEN N'ECOMMERCE' THEN 20 WHEN N'MAYORISTA' THEN 30 ELSE 1000+Id END
 WHERE SortOrder IS NULL;
 IF EXISTS(SELECT GlobalId FROM dbo.SalesChannels GROUP BY GlobalId HAVING COUNT_BIG(1)>1)
  THROW 51226,'SalesChannels contains duplicate GlobalId values.',1;
 IF EXISTS(SELECT 1 FROM dbo.SalesChannels WHERE GlobalId IS NULL OR SortOrder IS NULL OR SortOrder<0)
  THROW 51226,'SalesChannels could not be backfilled safely.',1;
 ALTER TABLE dbo.SalesChannels ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
 ALTER TABLE dbo.SalesChannels ALTER COLUMN SortOrder int NOT NULL;
 COMMIT;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
GO

IF OBJECT_ID(N'DF_SalesChannels_GlobalId',N'D') IS NULL
 ALTER TABLE dbo.SalesChannels ADD CONSTRAINT DF_SalesChannels_GlobalId DEFAULT(NEWSEQUENTIALID()) FOR GlobalId;
IF OBJECT_ID(N'DF_SalesChannels_SortOrder',N'D') IS NULL
 ALTER TABLE dbo.SalesChannels ADD CONSTRAINT DF_SalesChannels_SortOrder DEFAULT(0) FOR SortOrder;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.SalesChannels') AND name=N'UQ_SalesChannels_GlobalId')
 CREATE UNIQUE INDEX UQ_SalesChannels_GlobalId ON dbo.SalesChannels(GlobalId);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.SalesChannels') AND name=N'CK_SalesChannels_Code_NotBlank')
 ALTER TABLE dbo.SalesChannels ADD CONSTRAINT CK_SalesChannels_Code_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Code)),N'') IS NOT NULL);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.SalesChannels') AND name=N'CK_SalesChannels_Name_NotBlank')
 ALTER TABLE dbo.SalesChannels ADD CONSTRAINT CK_SalesChannels_Name_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Name)),N'') IS NOT NULL);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.SalesChannels') AND name=N'CK_SalesChannels_SortOrder')
 ALTER TABLE dbo.SalesChannels ADD CONSTRAINT CK_SalesChannels_SortOrder CHECK(SortOrder>=0);
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260817.226')
 INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260817.226',N'Evolves SalesChannels into an independent auxiliary master');
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_SalesChannels_LISTAR
AS
BEGIN
 SET NOCOUNT ON;
 SELECT * FROM dbo.SalesChannels WHERE IsDeleted=0 ORDER BY Code;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_SalesChannels_LOOKUP
AS
BEGIN
 SET NOCOUNT ON;
 SELECT * FROM dbo.SalesChannels WHERE IsDeleted=0 AND IsActive=1 ORDER BY Code;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_SalesChannels_BUSCARPORID @Id int
AS
BEGIN
 SET NOCOUNT ON;
 SELECT * FROM dbo.SalesChannels WHERE Id=@Id AND IsDeleted=0;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_SalesChannels_BUSCARPORCODIGO @Code nvarchar(50),@ExcluirId int=NULL
AS
BEGIN
 SET NOCOUNT ON;
 SELECT COUNT(1) FROM dbo.SalesChannels WHERE Code=@Code AND IsDeleted=0 AND (@ExcluirId IS NULL OR Id<>@ExcluirId);
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_SalesChannels_HISTORIAL @Id int
AS
BEGIN
 SET NOCOUNT ON;
 SELECT Id,EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName,[Source],CreatedAt
 FROM dbo.AuditCatalogChanges WHERE EntityName=N'SalesChannel' AND RecordId=CONVERT(nvarchar(80),@Id)
 ORDER BY CreatedAt DESC,Id DESC;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_GENERAL_INVENTORY_SalesChannels_CREAR
 @GlobalId uniqueidentifier=NULL,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,@CreatedByUserId int=NULL,@CreatedByUserName nvarchar(120)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @GlobalId=COALESCE(@GlobalId,NEWID());
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  IF EXISTS(SELECT 1 FROM dbo.SalesChannels WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code OR GlobalId=@GlobalId)
  BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
  INSERT dbo.SalesChannels(GlobalId,[Code],[Name],[Description],[SortOrder],[IsActive],CreatedByUserId,CreatedByUserName,IsDeleted,CreatedAt)
  VALUES(@GlobalId,@Code,@Name,@Description,@SortOrder,@IsActive,@CreatedByUserId,@CreatedByUserName,0,SYSUTCDATETIME());
  DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'SalesChannel',CONVERT(nvarchar(80),@Id),N'INSERT',FieldName,NULL,NewValue,@CreatedByUserId,@CreatedByUserName
  FROM(VALUES (N'Code',CONVERT(nvarchar(max),@Code)),
       (N'Name',CONVERT(nvarchar(max),@Name)),
       (N'Description',CONVERT(nvarchar(max),@Description)),
       (N'SortOrder',CONVERT(nvarchar(max),@SortOrder)),
       (N'IsActive',CONVERT(nvarchar(max),@IsActive)))v(FieldName,NewValue);
  IF @OwnTransaction=1 COMMIT; SELECT @Id;
 END TRY BEGIN CATCH
  IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
  IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END;
  THROW;
 END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_GENERAL_INVENTORY_SalesChannels_ACTUALIZAR
 @Id int,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,@UpdatedByUserId int=NULL,@UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  DECLARE @OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldSortOrder int,@OldIsActive bit;
  SELECT @OldCode=[Code],@OldName=[Name],@OldDescription=[Description],@OldSortOrder=[SortOrder],@OldIsActive=[IsActive] FROM dbo.SalesChannels WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
  IF @OldCode IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;
  IF EXISTS(SELECT 1 FROM dbo.SalesChannels WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND Id<>@Id AND IsDeleted=0)
  BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
  UPDATE dbo.SalesChannels SET [Code]=@Code,[Name]=@Name,[Description]=@Description,[SortOrder]=@SortOrder,[IsActive]=@IsActive,UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName,UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'SalesChannel',CONVERT(nvarchar(80),@Id),N'UPDATE',FieldName,OldValue,NewValue,@UpdatedByUserId,@UpdatedByUserName
  FROM(VALUES (N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),
       (N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
       (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
       (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
       (N'IsActive',CONVERT(nvarchar(max),@OldIsActive),CONVERT(nvarchar(max),@IsActive)))v(FieldName,OldValue,NewValue)
  WHERE ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
  IF @OwnTransaction=1 COMMIT; SELECT 1;
 END TRY BEGIN CATCH
  IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
  IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END;
  THROW;
 END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_SalesChannels_ELIMINAR @Id int,@DeletedByUserId int=NULL,@DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  UPDATE dbo.SalesChannels SET IsActive=0,IsDeleted=1,DeletedByUserId=@DeletedByUserId,DeletedByUserName=@DeletedByUserName,DeletedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
  DECLARE @Affected int=@@ROWCOUNT;
  IF @Affected>0
   INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
   VALUES(N'SalesChannel',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);
  IF @OwnTransaction=1 COMMIT;
  SELECT @Affected;
 END TRY BEGIN CATCH
  IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
  THROW;
 END CATCH;
END;
GO

