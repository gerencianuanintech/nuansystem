/* Provinces transactional LocalOutbox and terminal GlobalId apply. Tenant only; prerequisites 085, 164, 168. */
SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON; SET NOCOUNT ON; SET XACT_ABORT ON;
GO
IF OBJECT_ID(N'dbo.Provinces',N'U') IS NULL OR OBJECT_ID(N'dbo.Countries',N'U') IS NULL THROW 51172,'Countries and Provinces are required before 172.',1;
IF OBJECT_ID(N'dbo.LocalOutbox',N'U') IS NULL OR OBJECT_ID(N'dbo.SyncInbox',N'U') IS NULL OR OBJECT_ID(N'dbo.SyncAudit',N'U') IS NULL THROW 51172,'Sync tenant foundation is required before 172.',1;
IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NULL THROW 51172,'SchemaHistory is required before 172.',1;
GO
IF COL_LENGTH(N'dbo.Provinces',N'ExternalSystem') IS NULL ALTER TABLE dbo.Provinces ADD ExternalSystem nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.Provinces',N'ExternalCode') IS NULL ALTER TABLE dbo.Provinces ADD ExternalCode nvarchar(100) NULL;
GO
IF EXISTS(SELECT CountryId,Code FROM dbo.Provinces GROUP BY CountryId,Code HAVING COUNT_BIG(1)>1) THROW 51172,'Province codes, including tombstones, must be unique per country before 172.',1;
IF EXISTS(SELECT CountryId,ExternalSystem,ExternalCode FROM dbo.Provinces WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL GROUP BY CountryId,ExternalSystem,ExternalCode HAVING COUNT_BIG(1)>1) THROW 51172,'Province external references must be unique per country before 172.',1;
GO
IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Provinces') AND name=N'UX_Provinces_Country_Code' AND (is_unique=0 OR filter_definition IS NOT NULL)) DROP INDEX UX_Provinces_Country_Code ON dbo.Provinces;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Provinces') AND name=N'UX_Provinces_Country_Code' AND is_unique=1 AND filter_definition IS NULL) CREATE UNIQUE INDEX UX_Provinces_Country_Code ON dbo.Provinces(CountryId,Code);
GO
IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Provinces') AND name=N'IX_Provinces_ExternalRef') DROP INDEX IX_Provinces_ExternalRef ON dbo.Provinces;
IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Provinces') AND name=N'UX_Provinces_Country_ExternalRef' AND (is_unique=0 OR filter_definition IS NULL)) DROP INDEX UX_Provinces_Country_ExternalRef ON dbo.Provinces;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Provinces') AND name=N'UX_Provinces_Country_ExternalRef' AND is_unique=1 AND filter_definition IS NOT NULL)
    CREATE UNIQUE INDEX UX_Provinces_Country_ExternalRef ON dbo.Provinces(CountryId,ExternalSystem,ExternalCode) WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_LISTAR AS
BEGIN SET NOCOUNT ON;
SELECT p.ProvinceId Id,p.GlobalId,p.CountryId,c.GlobalId CountryGlobalId,c.Code CountryCode,c.Name CountryName,p.Code,p.Name,p.ExternalSystem,p.ExternalCode,p.IsActive,p.CreatedAt,p.UpdatedAt
FROM dbo.Provinces p INNER JOIN dbo.Countries c ON c.CountryId=p.CountryId WHERE p.IsDeleted=0 AND c.IsDeleted=0 ORDER BY c.Name,p.Name;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_BUSCARPORID @Id int AS
BEGIN SET NOCOUNT ON;
SELECT p.ProvinceId Id,p.GlobalId,p.CountryId,c.GlobalId CountryGlobalId,c.Code CountryCode,c.Name CountryName,p.Code,p.Name,p.ExternalSystem,p.ExternalCode,p.IsActive,p.CreatedAt,p.UpdatedAt
FROM dbo.Provinces p INNER JOIN dbo.Countries c ON c.CountryId=p.CountryId WHERE p.ProvinceId=@Id AND p.IsDeleted=0 AND c.IsDeleted=0;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_BUSCARPORCODIGO @CountryId int,@Code nvarchar(20),@ExcluirId int=NULL AS
BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.Provinces WHERE CountryId=@CountryId AND Code=@Code AND (@ExcluirId IS NULL OR ProvinceId<>@ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PROVINCES_CREAR
 @Id int=NULL,@GlobalId uniqueidentifier,@CountryId int,@Code nvarchar(20),@Name nvarchar(120),@IsActive bit=1,@AuditUserId int=NULL,@AuditUserName nvarchar(100)=NULL,@ExternalSystem nvarchar(50)=NULL,@ExternalCode nvarchar(100)=NULL AS
BEGIN SET NOCOUNT ON;
INSERT dbo.Provinces(GlobalId,CountryId,Code,Name,IsActive,ExternalSystem,ExternalCode,CreatedByUserId,CreatedByUserName)
VALUES(@GlobalId,@CountryId,@Code,@Name,@IsActive,@ExternalSystem,@ExternalCode,@AuditUserId,@AuditUserName); SELECT CONVERT(int,SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PROVINCES_ACTUALIZAR
 @Id int,@GlobalId uniqueidentifier,@CountryId int,@Code nvarchar(20),@Name nvarchar(120),@IsActive bit=1,@AuditUserId int=NULL,@AuditUserName nvarchar(100)=NULL,@ExternalSystem nvarchar(50)=NULL,@ExternalCode nvarchar(100)=NULL AS
BEGIN SET NOCOUNT ON;
UPDATE dbo.Provinces SET CountryId=@CountryId,Code=@Code,Name=@Name,IsActive=@IsActive,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,UpdatedAt=SYSUTCDATETIME(),UpdatedByUserId=@AuditUserId,UpdatedByUserName=@AuditUserName WHERE ProvinceId=@Id AND IsDeleted=0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PROVINCE_SYNC_APPLY_EVENT
 @EventId uniqueidentifier,@SourceCompanyId int,@EntityName nvarchar(80),@EntityGlobalId uniqueidentifier,@Operation nvarchar(30),@PayloadJson nvarchar(max),
 @GlobalId uniqueidentifier,@CountryGlobalId uniqueidentifier,@Code nvarchar(20),@Name nvarchar(120),@IsActive bit,@IsDeleted bit,
 @ExternalSystem nvarchar(50)=NULL,@ExternalCode nvarchar(100)=NULL,@CreatedAt datetime2(0),@UpdatedAt datetime2(0) AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  BEGIN TRANSACTION;
  DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@CountryId int,@ProvinceId int,@ExistingCountryId int;
  SELECT @InboxId=Id,@InboxStatus=Status FROM dbo.SyncInbox WITH(UPDLOCK,HOLDLOCK) WHERE EventId=@EventId;
  IF @InboxStatus=N'Applied' BEGIN SELECT @ProvinceId=ProvinceId FROM dbo.Provinces WHERE GlobalId=@GlobalId; COMMIT; SELECT 2 ResultCode,@ProvinceId ProvinceId; RETURN; END;
  IF @InboxStatus=N'DeadLetter' BEGIN COMMIT; SELECT -2 ResultCode,CONVERT(int,NULL) ProvinceId; RETURN; END;
  IF @InboxId IS NULL BEGIN INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status) VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,N'Pending'); SET @InboxId=CONVERT(bigint,SCOPE_IDENTITY()); END;
  IF @EntityName<>N'Provinces' OR @EntityGlobalId<>@GlobalId THROW 51172,'Province event identity is invalid.',1;
  SELECT @CountryId=CountryId FROM dbo.Countries WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@CountryGlobalId AND IsDeleted=0;
  IF @CountryId IS NULL THROW 51172,'CountryGlobalId dependency is not available for Province.',1;
  SELECT @ProvinceId=ProvinceId,@ExistingCountryId=CountryId FROM dbo.Provinces WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
  IF @ProvinceId IS NOT NULL AND @ExistingCountryId<>@CountryId
  BEGIN
   UPDATE dbo.SyncInbox SET Status=N'DeadLetter',AttemptCount=AttemptCount+1,ErrorMessage=N'Province parent country conflict.',LastErrorMessage=N'Province parent country conflict.',NextRetryAt=NULL WHERE Id=@InboxId;
   INSERT dbo.SyncAudit(CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],ErrorCode,CreatedBy) VALUES(@SourceCompanyId,@EventId,N'Provinces',@GlobalId,N'DeadLetter',@InboxStatus,N'DeadLetter',N'Province parent cannot be reassigned.',N'SYNC_PROVINCE_PARENT_CONFLICT',N'MasterBranchSyncWorker');
   COMMIT; SELECT -4 ResultCode,CONVERT(int,NULL) ProvinceId; RETURN;
  END;
  IF EXISTS(SELECT 1 FROM dbo.Provinces WITH(UPDLOCK,HOLDLOCK) WHERE CountryId=@CountryId AND Code=@Code AND GlobalId<>@GlobalId)
  BEGIN
   UPDATE dbo.SyncInbox SET Status=N'DeadLetter',AttemptCount=AttemptCount+1,ErrorMessage=N'Province code conflict.',LastErrorMessage=N'Province code conflict.',NextRetryAt=NULL WHERE Id=@InboxId;
   INSERT dbo.SyncAudit(CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],ErrorCode,CreatedBy) VALUES(@SourceCompanyId,@EventId,N'Provinces',@GlobalId,N'DeadLetter',@InboxStatus,N'DeadLetter',N'Province code conflict; no adoption.',N'SYNC_PROVINCE_CODE_CONFLICT',N'MasterBranchSyncWorker');
   COMMIT; SELECT -2 ResultCode,CONVERT(int,NULL) ProvinceId; RETURN;
  END;
  IF @ExternalSystem IS NOT NULL AND @ExternalCode IS NOT NULL AND EXISTS(SELECT 1 FROM dbo.Provinces WITH(UPDLOCK,HOLDLOCK) WHERE CountryId=@CountryId AND ExternalSystem=@ExternalSystem AND ExternalCode=@ExternalCode AND GlobalId<>@GlobalId)
  BEGIN
   UPDATE dbo.SyncInbox SET Status=N'DeadLetter',AttemptCount=AttemptCount+1,ErrorMessage=N'Province external reference conflict.',LastErrorMessage=N'Province external reference conflict.',NextRetryAt=NULL WHERE Id=@InboxId;
   INSERT dbo.SyncAudit(CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],ErrorCode,CreatedBy) VALUES(@SourceCompanyId,@EventId,N'Provinces',@GlobalId,N'DeadLetter',@InboxStatus,N'DeadLetter',N'Province external reference conflict.',N'SYNC_PROVINCE_EXTERNAL_CONFLICT',N'MasterBranchSyncWorker');
   COMMIT; SELECT -3 ResultCode,CONVERT(int,NULL) ProvinceId; RETURN;
  END;
  IF @ProvinceId IS NULL BEGIN INSERT dbo.Provinces(GlobalId,CountryId,Code,Name,IsActive,IsDeleted,ExternalSystem,ExternalCode,CreatedAt,CreatedByUserName) VALUES(@GlobalId,@CountryId,@Code,@Name,@IsActive,@IsDeleted,@ExternalSystem,@ExternalCode,COALESCE(@CreatedAt,SYSUTCDATETIME()),N'MasterBranchSyncWorker'); SET @ProvinceId=CONVERT(int,SCOPE_IDENTITY()); END
  ELSE UPDATE dbo.Provinces SET Code=@Code,Name=@Name,IsActive=@IsActive,IsDeleted=@IsDeleted,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,UpdatedAt=COALESCE(@UpdatedAt,SYSUTCDATETIME()),UpdatedByUserName=N'MasterBranchSyncWorker' WHERE ProvinceId=@ProvinceId;
  UPDATE dbo.SyncInbox SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL WHERE Id=@InboxId;
  INSERT dbo.SyncAudit(CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],CreatedBy) VALUES(@SourceCompanyId,@EventId,N'Provinces',@GlobalId,N'Applied',COALESCE(@InboxStatus,N'Pending'),N'Applied',N'Province applied by GlobalId.',N'MasterBranchSyncWorker');
  COMMIT; SELECT 1 ResultCode,@ProvinceId ProvinceId;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH;
END;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260804.172') INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260804.172',N'Provinces transactional outbox and terminal hierarchical apply');
GO
