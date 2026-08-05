/* Cities transactional LocalOutbox and terminal GlobalId apply. Tenant only; prerequisites 087, 164, 168, 172. */
SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON; SET NOCOUNT ON; SET XACT_ABORT ON;
GO
IF OBJECT_ID(N'dbo.Cities',N'U') IS NULL OR OBJECT_ID(N'dbo.Provinces',N'U') IS NULL OR OBJECT_ID(N'dbo.Countries',N'U') IS NULL THROW 51175,'Countries, Provinces and Cities are required before 175.',1;
IF OBJECT_ID(N'dbo.LocalOutbox',N'U') IS NULL OR OBJECT_ID(N'dbo.SyncInbox',N'U') IS NULL OR OBJECT_ID(N'dbo.SyncAudit',N'U') IS NULL THROW 51175,'Sync tenant foundation is required before 175.',1;
IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NULL THROW 51175,'SchemaHistory is required before 175.',1;
GO
IF COL_LENGTH(N'dbo.Cities',N'ExternalSystem') IS NULL ALTER TABLE dbo.Cities ADD ExternalSystem nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.Cities',N'ExternalCode') IS NULL ALTER TABLE dbo.Cities ADD ExternalCode nvarchar(100) NULL;
GO
IF EXISTS(SELECT ProvinceId,Code FROM dbo.Cities GROUP BY ProvinceId,Code HAVING COUNT_BIG(1)>1) THROW 51175,'City codes, including tombstones, must be unique per province before 175.',1;
IF EXISTS(SELECT ProvinceId,ExternalSystem,ExternalCode FROM dbo.Cities WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL GROUP BY ProvinceId,ExternalSystem,ExternalCode HAVING COUNT_BIG(1)>1) THROW 51175,'City external references must be unique per province before 175.',1;
GO
IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Cities') AND name=N'UX_Cities_Province_Code' AND (is_unique=0 OR filter_definition IS NOT NULL)) DROP INDEX UX_Cities_Province_Code ON dbo.Cities;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Cities') AND name=N'UX_Cities_Province_Code' AND is_unique=1 AND filter_definition IS NULL) CREATE UNIQUE INDEX UX_Cities_Province_Code ON dbo.Cities(ProvinceId,Code);
GO
IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Cities') AND name=N'IX_Cities_ExternalRef') DROP INDEX IX_Cities_ExternalRef ON dbo.Cities;
IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Cities') AND name=N'UX_Cities_Province_ExternalRef' AND (is_unique=0 OR filter_definition IS NULL)) DROP INDEX UX_Cities_Province_ExternalRef ON dbo.Cities;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Cities') AND name=N'UX_Cities_Province_ExternalRef' AND is_unique=1 AND filter_definition IS NOT NULL)
    CREATE UNIQUE INDEX UX_Cities_Province_ExternalRef ON dbo.Cities(ProvinceId,ExternalSystem,ExternalCode) WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_LISTAR AS
BEGIN SET NOCOUNT ON;
SELECT city.CityId Id,city.GlobalId,city.CountryId,country.GlobalId CountryGlobalId,country.Code CountryCode,country.Name CountryName,city.ProvinceId,province.GlobalId ProvinceGlobalId,province.Code ProvinceCode,province.Name ProvinceName,city.Code,city.Name,city.ExternalSystem,city.ExternalCode,city.IsActive,city.CreatedAt,city.UpdatedAt
FROM dbo.Cities city INNER JOIN dbo.Countries country ON country.CountryId=city.CountryId INNER JOIN dbo.Provinces province ON province.ProvinceId=city.ProvinceId
WHERE city.IsDeleted=0 AND country.IsDeleted=0 AND province.IsDeleted=0 AND province.CountryId=country.CountryId ORDER BY country.Name,province.Name,city.Name;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_BUSCARPORID @Id int AS
BEGIN SET NOCOUNT ON;
SELECT city.CityId Id,city.GlobalId,city.CountryId,country.GlobalId CountryGlobalId,country.Code CountryCode,country.Name CountryName,city.ProvinceId,province.GlobalId ProvinceGlobalId,province.Code ProvinceCode,province.Name ProvinceName,city.Code,city.Name,city.ExternalSystem,city.ExternalCode,city.IsActive,city.CreatedAt,city.UpdatedAt
FROM dbo.Cities city INNER JOIN dbo.Countries country ON country.CountryId=city.CountryId INNER JOIN dbo.Provinces province ON province.ProvinceId=city.ProvinceId
WHERE city.CityId=@Id AND city.IsDeleted=0 AND country.IsDeleted=0 AND province.IsDeleted=0 AND province.CountryId=country.CountryId;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_BUSCARPORCODIGO @ProvinceId int,@Code nvarchar(20),@ExcluirId int=NULL AS
BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.Cities WHERE ProvinceId=@ProvinceId AND Code=@Code AND (@ExcluirId IS NULL OR CityId<>@ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CITIES_CREAR
 @Id int=NULL,@GlobalId uniqueidentifier,@CountryId int,@ProvinceId int,@Code nvarchar(20),@Name nvarchar(120),@IsActive bit=1,@AuditUserId int=NULL,@AuditUserName nvarchar(100)=NULL,@ExternalSystem nvarchar(50)=NULL,@ExternalCode nvarchar(100)=NULL AS
BEGIN SET NOCOUNT ON;
IF NOT EXISTS(SELECT 1 FROM dbo.Provinces WHERE ProvinceId=@ProvinceId AND CountryId=@CountryId AND IsDeleted=0) THROW 51090,'La provincia no pertenece al pais indicado.',1;
INSERT dbo.Cities(GlobalId,CountryId,ProvinceId,Code,Name,IsActive,ExternalSystem,ExternalCode,CreatedByUserId,CreatedByUserName)
VALUES(@GlobalId,@CountryId,@ProvinceId,@Code,@Name,@IsActive,@ExternalSystem,@ExternalCode,@AuditUserId,@AuditUserName); SELECT CONVERT(int,SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CITIES_ACTUALIZAR
 @Id int,@GlobalId uniqueidentifier,@CountryId int,@ProvinceId int,@Code nvarchar(20),@Name nvarchar(120),@IsActive bit=1,@AuditUserId int=NULL,@AuditUserName nvarchar(100)=NULL,@ExternalSystem nvarchar(50)=NULL,@ExternalCode nvarchar(100)=NULL AS
BEGIN SET NOCOUNT ON;
IF NOT EXISTS(SELECT 1 FROM dbo.Provinces WHERE ProvinceId=@ProvinceId AND CountryId=@CountryId AND IsDeleted=0) THROW 51090,'La provincia no pertenece al pais indicado.',1;
IF EXISTS(SELECT 1 FROM dbo.Cities WHERE CityId=@Id AND IsDeleted=0 AND (CountryId<>@CountryId OR ProvinceId<>@ProvinceId)) THROW 51091,'No se puede reasignar la ciudad a otro pais o provincia.',1;
UPDATE dbo.Cities SET CountryId=@CountryId,ProvinceId=@ProvinceId,Code=@Code,Name=@Name,IsActive=@IsActive,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,UpdatedAt=SYSUTCDATETIME(),UpdatedByUserId=@AuditUserId,UpdatedByUserName=@AuditUserName WHERE CityId=@Id AND IsDeleted=0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CITY_SYNC_APPLY_EVENT
 @EventId uniqueidentifier,@SourceCompanyId int,@EntityName nvarchar(80),@EntityGlobalId uniqueidentifier,@Operation nvarchar(30),@PayloadJson nvarchar(max),
 @GlobalId uniqueidentifier,@CountryGlobalId uniqueidentifier,@ProvinceGlobalId uniqueidentifier,@Code nvarchar(20),@Name nvarchar(120),@IsActive bit,@IsDeleted bit,
 @ExternalSystem nvarchar(50)=NULL,@ExternalCode nvarchar(100)=NULL,@CreatedAt datetime2(0),@UpdatedAt datetime2(0) AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  BEGIN TRANSACTION;
  DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@CountryId int,@ProvinceId int,@ProvinceCountryId int,@CityId int,@ExistingCountryId int,@ExistingProvinceId int;
  SELECT @InboxId=Id,@InboxStatus=Status FROM dbo.SyncInbox WITH(UPDLOCK,HOLDLOCK) WHERE EventId=@EventId;
  IF @InboxStatus=N'Applied' BEGIN SELECT @CityId=CityId FROM dbo.Cities WHERE GlobalId=@GlobalId; COMMIT; SELECT 2 ResultCode,@CityId CityId; RETURN; END;
  IF @InboxStatus=N'DeadLetter' BEGIN COMMIT; SELECT -6 ResultCode,CONVERT(int,NULL) CityId; RETURN; END;
  IF @InboxId IS NULL BEGIN INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status) VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,N'Pending'); SET @InboxId=CONVERT(bigint,SCOPE_IDENTITY()); END;
  IF @EntityName<>N'Cities' OR @EntityGlobalId<>@GlobalId THROW 51175,'City event identity is invalid.',1;
  SELECT @CountryId=CountryId FROM dbo.Countries WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@CountryGlobalId AND IsDeleted=0;
  IF @CountryId IS NULL THROW 51175,'CountryGlobalId dependency is not available for City.',1;
  SELECT @ProvinceId=ProvinceId,@ProvinceCountryId=CountryId FROM dbo.Provinces WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@ProvinceGlobalId AND IsDeleted=0;
  IF @ProvinceId IS NULL THROW 51175,'ProvinceGlobalId dependency is not available for City.',1;
  IF @ProvinceCountryId<>@CountryId
  BEGIN
   UPDATE dbo.SyncInbox SET Status=N'DeadLetter',AttemptCount=AttemptCount+1,ErrorMessage=N'City payload hierarchy conflict.',LastErrorMessage=N'City payload hierarchy conflict.',NextRetryAt=NULL WHERE Id=@InboxId;
   INSERT dbo.SyncAudit(CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],ErrorCode,CreatedBy) VALUES(@SourceCompanyId,@EventId,N'Cities',@GlobalId,N'DeadLetter',@InboxStatus,N'DeadLetter',N'ProvinceGlobalId does not belong to CountryGlobalId.',N'SYNC_CITY_HIERARCHY_CONFLICT',N'MasterBranchSyncWorker');
   COMMIT; SELECT -4 ResultCode,CONVERT(int,NULL) CityId; RETURN;
  END;
  SELECT @CityId=CityId,@ExistingCountryId=CountryId,@ExistingProvinceId=ProvinceId FROM dbo.Cities WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
  IF @CityId IS NOT NULL AND (@ExistingCountryId<>@CountryId OR @ExistingProvinceId<>@ProvinceId)
  BEGIN
   UPDATE dbo.SyncInbox SET Status=N'DeadLetter',AttemptCount=AttemptCount+1,ErrorMessage=N'City parent conflict.',LastErrorMessage=N'City parent conflict.',NextRetryAt=NULL WHERE Id=@InboxId;
   INSERT dbo.SyncAudit(CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],ErrorCode,CreatedBy) VALUES(@SourceCompanyId,@EventId,N'Cities',@GlobalId,N'DeadLetter',@InboxStatus,N'DeadLetter',N'City parent cannot be reassigned.',N'SYNC_CITY_PARENT_CONFLICT',N'MasterBranchSyncWorker');
   COMMIT; SELECT -5 ResultCode,CONVERT(int,NULL) CityId; RETURN;
  END;
  IF EXISTS(SELECT 1 FROM dbo.Cities WITH(UPDLOCK,HOLDLOCK) WHERE ProvinceId=@ProvinceId AND Code=@Code AND GlobalId<>@GlobalId)
  BEGIN
   UPDATE dbo.SyncInbox SET Status=N'DeadLetter',AttemptCount=AttemptCount+1,ErrorMessage=N'City code conflict.',LastErrorMessage=N'City code conflict.',NextRetryAt=NULL WHERE Id=@InboxId;
   INSERT dbo.SyncAudit(CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],ErrorCode,CreatedBy) VALUES(@SourceCompanyId,@EventId,N'Cities',@GlobalId,N'DeadLetter',@InboxStatus,N'DeadLetter',N'City code conflict; no adoption.',N'SYNC_CITY_CODE_CONFLICT',N'MasterBranchSyncWorker');
   COMMIT; SELECT -2 ResultCode,CONVERT(int,NULL) CityId; RETURN;
  END;
  IF @ExternalSystem IS NOT NULL AND @ExternalCode IS NOT NULL AND EXISTS(SELECT 1 FROM dbo.Cities WITH(UPDLOCK,HOLDLOCK) WHERE ProvinceId=@ProvinceId AND ExternalSystem=@ExternalSystem AND ExternalCode=@ExternalCode AND GlobalId<>@GlobalId)
  BEGIN
   UPDATE dbo.SyncInbox SET Status=N'DeadLetter',AttemptCount=AttemptCount+1,ErrorMessage=N'City external reference conflict.',LastErrorMessage=N'City external reference conflict.',NextRetryAt=NULL WHERE Id=@InboxId;
   INSERT dbo.SyncAudit(CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],ErrorCode,CreatedBy) VALUES(@SourceCompanyId,@EventId,N'Cities',@GlobalId,N'DeadLetter',@InboxStatus,N'DeadLetter',N'City external reference conflict.',N'SYNC_CITY_EXTERNAL_CONFLICT',N'MasterBranchSyncWorker');
   COMMIT; SELECT -3 ResultCode,CONVERT(int,NULL) CityId; RETURN;
  END;
  IF @CityId IS NULL BEGIN INSERT dbo.Cities(GlobalId,CountryId,ProvinceId,Code,Name,IsActive,IsDeleted,ExternalSystem,ExternalCode,CreatedAt,CreatedByUserName) VALUES(@GlobalId,@CountryId,@ProvinceId,@Code,@Name,@IsActive,@IsDeleted,@ExternalSystem,@ExternalCode,COALESCE(@CreatedAt,SYSUTCDATETIME()),N'MasterBranchSyncWorker'); SET @CityId=CONVERT(int,SCOPE_IDENTITY()); END
  ELSE UPDATE dbo.Cities SET Code=@Code,Name=@Name,IsActive=@IsActive,IsDeleted=@IsDeleted,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,UpdatedAt=COALESCE(@UpdatedAt,SYSUTCDATETIME()),UpdatedByUserName=N'MasterBranchSyncWorker' WHERE CityId=@CityId;
  UPDATE dbo.SyncInbox SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL WHERE Id=@InboxId;
  INSERT dbo.SyncAudit(CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],CreatedBy) VALUES(@SourceCompanyId,@EventId,N'Cities',@GlobalId,N'Applied',COALESCE(@InboxStatus,N'Pending'),N'Applied',N'City applied by GlobalId.',N'MasterBranchSyncWorker');
  COMMIT; SELECT 1 ResultCode,@CityId CityId;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH;
END;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260804.175') INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260804.175',N'Cities transactional outbox and terminal hierarchical apply');
GO
