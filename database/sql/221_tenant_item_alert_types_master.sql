SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
IF DB_NAME() = N'NuanSystem_Master' THROW 51221, 'Migration 221 must run only in tenant databases.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL THROW 51221, 'SchemaHistory is required.', 1;
IF OBJECT_ID(N'dbo.AuditCatalogChanges', N'U') IS NULL THROW 51221, 'AuditCatalogChanges is required.', 1;
GO
BEGIN TRY
 BEGIN TRANSACTION;
 IF OBJECT_ID(N'dbo.ItemAlertTypes', N'U') IS NULL
 BEGIN
  CREATE TABLE dbo.ItemAlertTypes(
        [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemAlertTypes PRIMARY KEY,
        [GlobalId] uniqueidentifier NOT NULL CONSTRAINT DF_ItemAlertTypes_GlobalId DEFAULT(NEWSEQUENTIALID()),
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [SortOrder] int NOT NULL CONSTRAINT DF_ItemAlertTypes_SortOrder DEFAULT(0),
        [IsActive] bit NOT NULL CONSTRAINT DF_ItemAlertTypes_IsActive DEFAULT(1),
        [CreatedByUserId] int NULL,
        [CreatedByUserName] nvarchar(120) NULL,
        [CreatedAt] datetime2(0) NOT NULL CONSTRAINT DF_ItemAlertTypes_CreatedAt DEFAULT(SYSUTCDATETIME()),
        [UpdatedByUserId] int NULL,
        [UpdatedByUserName] nvarchar(120) NULL,
        [UpdatedAt] datetime2(0) NULL,
        [IsDeleted] bit NOT NULL CONSTRAINT DF_ItemAlertTypes_IsDeleted DEFAULT(0),
        [DeletedByUserId] int NULL,
        [DeletedByUserName] nvarchar(120) NULL,
        [DeletedAt] datetime2(0) NULL
  );
 END;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemAlertTypes') AND name=N'UQ_ItemAlertTypes_GlobalId')
 CREATE UNIQUE INDEX UQ_ItemAlertTypes_GlobalId ON dbo.ItemAlertTypes([GlobalId]);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemAlertTypes') AND name=N'UQ_ItemAlertTypes_Code')
 CREATE UNIQUE INDEX UQ_ItemAlertTypes_Code ON dbo.ItemAlertTypes([Code]);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemAlertTypes') AND name=N'CK_ItemAlertTypes_Code_NotBlank')
 ALTER TABLE dbo.ItemAlertTypes ADD CONSTRAINT CK_ItemAlertTypes_Code_NotBlank CHECK(NULLIF(LTRIM(RTRIM([Code])),N'') IS NOT NULL);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemAlertTypes') AND name=N'CK_ItemAlertTypes_Name_NotBlank')
 ALTER TABLE dbo.ItemAlertTypes ADD CONSTRAINT CK_ItemAlertTypes_Name_NotBlank CHECK(NULLIF(LTRIM(RTRIM([Name])),N'') IS NOT NULL);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemAlertTypes') AND name=N'CK_ItemAlertTypes_SortOrder')
 ALTER TABLE dbo.ItemAlertTypes ADD CONSTRAINT CK_ItemAlertTypes_SortOrder CHECK([SortOrder] >= 0);
 IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260814.221')
  INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260814.221',N'Creates Tipos de alerta de artículos auxiliary master');
 COMMIT;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemAlertTypes_LISTAR
AS
BEGIN
 SET NOCOUNT ON;
 SELECT * FROM dbo.ItemAlertTypes WHERE IsDeleted=0 ORDER BY Code;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemAlertTypes_LOOKUP
AS
BEGIN
 SET NOCOUNT ON;
 SELECT * FROM dbo.ItemAlertTypes WHERE IsDeleted=0 AND IsActive=1 ORDER BY Code;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemAlertTypes_BUSCARPORID @Id int
AS
BEGIN
 SET NOCOUNT ON;
 SELECT * FROM dbo.ItemAlertTypes WHERE Id=@Id AND IsDeleted=0;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemAlertTypes_BUSCARPORCODIGO @Code nvarchar(50),@ExcluirId int=NULL
AS
BEGIN
 SET NOCOUNT ON;
 SELECT COUNT(1) FROM dbo.ItemAlertTypes WHERE Code=@Code AND IsDeleted=0 AND (@ExcluirId IS NULL OR Id<>@ExcluirId);
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ItemAlertTypes_HISTORIAL @Id int
AS
BEGIN
 SET NOCOUNT ON;
 SELECT Id,EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName,[Source],CreatedAt
 FROM dbo.AuditCatalogChanges WHERE EntityName=N'ItemAlertType' AND RecordId=CONVERT(nvarchar(80),@Id)
 ORDER BY CreatedAt DESC,Id DESC;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_GENERAL_INVENTORY_ItemAlertTypes_CREAR
 @GlobalId uniqueidentifier=NULL,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,@CreatedByUserId int=NULL,@CreatedByUserName nvarchar(120)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @GlobalId=COALESCE(@GlobalId,NEWID());
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  IF EXISTS(SELECT 1 FROM dbo.ItemAlertTypes WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code OR GlobalId=@GlobalId)
  BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
  INSERT dbo.ItemAlertTypes(GlobalId,[Code],[Name],[Description],[SortOrder],[IsActive],CreatedByUserId,CreatedByUserName,IsDeleted,CreatedAt)
  VALUES(@GlobalId,@Code,@Name,@Description,@SortOrder,@IsActive,@CreatedByUserId,@CreatedByUserName,0,SYSUTCDATETIME());
  DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'ItemAlertType',CONVERT(nvarchar(80),@Id),N'INSERT',FieldName,NULL,NewValue,@CreatedByUserId,@CreatedByUserName
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
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_GENERAL_INVENTORY_ItemAlertTypes_ACTUALIZAR
 @Id int,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,@UpdatedByUserId int=NULL,@UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  DECLARE @OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldSortOrder int,@OldIsActive bit;
  SELECT @OldCode=[Code],@OldName=[Name],@OldDescription=[Description],@OldSortOrder=[SortOrder],@OldIsActive=[IsActive] FROM dbo.ItemAlertTypes WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
  IF @OldCode IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;
  IF EXISTS(SELECT 1 FROM dbo.ItemAlertTypes WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND Id<>@Id AND IsDeleted=0)
  BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
  UPDATE dbo.ItemAlertTypes SET [Code]=@Code,[Name]=@Name,[Description]=@Description,[SortOrder]=@SortOrder,[IsActive]=@IsActive,UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName,UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
  INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'ItemAlertType',CONVERT(nvarchar(80),@Id),N'UPDATE',FieldName,OldValue,NewValue,@UpdatedByUserId,@UpdatedByUserName
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
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_ItemAlertTypes_ELIMINAR @Id int,@DeletedByUserId int=NULL,@DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
  IF @OwnTransaction=1 BEGIN TRANSACTION;
  UPDATE dbo.ItemAlertTypes SET IsActive=0,IsDeleted=1,DeletedByUserId=@DeletedByUserId,DeletedByUserName=@DeletedByUserName,DeletedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
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
