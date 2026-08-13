/*
    Evoluciona dbo.ItemGroups con ocho cuentas contables canonicas,
    orden, proteccion de registros de sistema, auditoria e integracion
    con el contrato Matriz-Sucursal existente.

    Solo tenant. Prerrequisitos: 018, 020, 106, 129 y SchemaHistory.
    SalesAccountCode y PurchaseAccountCode se conservan como aliases legacy.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() = N'NuanSystem_Master' THROW 51186, 'Migration 186 must run only in tenant databases.', 1;
IF OBJECT_ID(N'dbo.ItemGroups', N'U') IS NULL THROW 51186, 'ItemGroups is required before migration 186.', 1;
IF OBJECT_ID(N'dbo.AuditInventoryChanges', N'U') IS NULL THROW 51186, 'AuditInventoryChanges is required before migration 186.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL THROW 51186, 'LocalOutbox from migration 129 is required before migration 186.', 1;
IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL THROW 51186, 'SyncInbox from migration 129 is required before migration 186.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL THROW 51186, 'SchemaHistory is required before migration 186.', 1;
IF EXISTS (SELECT Code FROM dbo.ItemGroups GROUP BY Code HAVING COUNT_BIG(1) > 1)
    THROW 51186, 'ItemGroups contains duplicate codes, including tombstones.', 1;
IF EXISTS (SELECT 1 FROM dbo.ItemGroups WHERE NULLIF(LTRIM(RTRIM(Code)),N'') IS NULL OR NULLIF(LTRIM(RTRIM(Name)),N'') IS NULL)
    THROW 51186, 'ItemGroups contains blank codes or names.', 1;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'IncomeAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD IncomeAccountCode nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.ItemGroups', N'SalesReturnAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD SalesReturnAccountCode nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.ItemGroups', N'PurchaseReturnAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD PurchaseReturnAccountCode nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.ItemGroups', N'CostVarianceAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD CostVarianceAccountCode nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.ItemGroups', N'InventoryAdjustmentAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD InventoryAdjustmentAccountCode nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.ItemGroups', N'PurchaseExpenseAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD PurchaseExpenseAccountCode nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.ItemGroups', N'SortOrder') IS NULL
    ALTER TABLE dbo.ItemGroups ADD SortOrder int NULL;
IF COL_LENGTH(N'dbo.ItemGroups', N'IsSystem') IS NULL
    ALTER TABLE dbo.ItemGroups ADD IsSystem bit NULL;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.ItemGroups
    SET IncomeAccountCode = COALESCE(IncomeAccountCode, SalesAccountCode),
        PurchaseExpenseAccountCode = COALESCE(PurchaseExpenseAccountCode, PurchaseAccountCode),
        SortOrder = COALESCE(SortOrder, 0),
        IsSystem = COALESCE(IsSystem, CASE WHEN Code = N'GENERAL' THEN 1 ELSE 0 END);

    ALTER TABLE dbo.ItemGroups ALTER COLUMN SortOrder int NOT NULL;
    ALTER TABLE dbo.ItemGroups ALTER COLUMN IsSystem bit NOT NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.default_constraints d JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id WHERE d.parent_object_id=OBJECT_ID(N'dbo.ItemGroups') AND c.name=N'SortOrder')
        ALTER TABLE dbo.ItemGroups ADD CONSTRAINT DF_ItemGroups_SortOrder DEFAULT(0) FOR SortOrder;
    IF NOT EXISTS (SELECT 1 FROM sys.default_constraints d JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id WHERE d.parent_object_id=OBJECT_ID(N'dbo.ItemGroups') AND c.name=N'IsSystem')
        ALTER TABLE dbo.ItemGroups ADD CONSTRAINT DF_ItemGroups_IsSystem DEFAULT(0) FOR IsSystem;
    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemGroups') AND name=N'CK_ItemGroups_Code_NotBlank')
        ALTER TABLE dbo.ItemGroups ADD CONSTRAINT CK_ItemGroups_Code_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Code)),N'') IS NOT NULL);
    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemGroups') AND name=N'CK_ItemGroups_Name_NotBlank')
        ALTER TABLE dbo.ItemGroups ADD CONSTRAINT CK_ItemGroups_Name_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Name)),N'') IS NOT NULL);
    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemGroups') AND name=N'CK_ItemGroups_SortOrder')
        ALTER TABLE dbo.ItemGroups ADD CONSTRAINT CK_ItemGroups_SortOrder CHECK(SortOrder >= 0);

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemGroups') AND name=N'UX_ItemGroups_Code_Active')
        DROP INDEX UX_ItemGroups_Code_Active ON dbo.ItemGroups;
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemGroups') AND name=N'UQ_ItemGroups_Code')
        CREATE UNIQUE INDEX UQ_ItemGroups_Code ON dbo.ItemGroups(Code);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemGroups') AND name=N'IX_ItemGroups_Active_SortOrder_Name')
        CREATE INDEX IX_ItemGroups_Active_SortOrder_Name ON dbo.ItemGroups(IsActive,SortOrder,Name) INCLUDE(Code) WHERE IsDeleted=0;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUPS_LISTAR AS
BEGIN
 SET NOCOUNT ON;
 SELECT Id,GlobalId,Code,Name,Description,IsActive,
  InventoryAccountCode,IncomeAccountCode,CostOfSalesAccountCode,SalesReturnAccountCode,
  PurchaseReturnAccountCode,CostVarianceAccountCode,InventoryAdjustmentAccountCode,PurchaseExpenseAccountCode,
  SalesAccountCode,PurchaseAccountCode,SortOrder,IsSystem,SapGroupCode,SapCode,ExternalSystem,ExternalCode,
  CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,
  DeletedByUserId,DeletedByUserName,DeletedAt
 FROM dbo.ItemGroups WHERE IsDeleted=0 ORDER BY SortOrder,Name,Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUPS_BUSCARPORID @Id int AS
BEGIN
 SET NOCOUNT ON;
 SELECT Id,GlobalId,Code,Name,Description,IsActive,
  InventoryAccountCode,IncomeAccountCode,CostOfSalesAccountCode,SalesReturnAccountCode,
  PurchaseReturnAccountCode,CostVarianceAccountCode,InventoryAdjustmentAccountCode,PurchaseExpenseAccountCode,
  SalesAccountCode,PurchaseAccountCode,SortOrder,IsSystem,SapGroupCode,SapCode,ExternalSystem,ExternalCode,
  CreatedByUserId,CreatedByUserName,CreatedAt,UpdatedByUserId,UpdatedByUserName,UpdatedAt,
  DeletedByUserId,DeletedByUserName,DeletedAt
 FROM dbo.ItemGroups WHERE Id=@Id AND IsDeleted=0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUPS_LOOKUP AS
BEGIN
 SET NOCOUNT ON;
 SELECT Id,GlobalId,Code,Name,SortOrder,IsSystem,CAST(IsActive AS bit) IsActive
 FROM dbo.ItemGroups WHERE IsDeleted=0 AND IsActive=1 ORDER BY SortOrder,Name,Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUPSBUSCARPORCODIGO @Code nvarchar(50),@ExcluirId int=NULL AS
BEGIN
 SET NOCOUNT ON;
 SELECT COUNT(1) FROM dbo.ItemGroups WITH(UPDLOCK,HOLDLOCK)
 WHERE Code=LTRIM(RTRIM(@Code)) AND (@ExcluirId IS NULL OR Id<>@ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUPS_HISTORIAL @Id int AS
BEGIN
 SET NOCOUNT ON;
 SELECT Id,EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName,[Source],CreatedAt
 FROM dbo.AuditInventoryChanges WHERE EntityName=N'ItemGroups' AND RecordId=CONVERT(nvarchar(80),@Id)
 ORDER BY CreatedAt DESC,Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_GROUPS_CREAR
 @GlobalId uniqueidentifier,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@IsActive bit,
 @InventoryAccountCode nvarchar(120)=NULL,@IncomeAccountCode nvarchar(120)=NULL,@CostOfSalesAccountCode nvarchar(120)=NULL,
 @SalesReturnAccountCode nvarchar(120)=NULL,@PurchaseReturnAccountCode nvarchar(120)=NULL,@CostVarianceAccountCode nvarchar(120)=NULL,
 @InventoryAdjustmentAccountCode nvarchar(120)=NULL,@PurchaseExpenseAccountCode nvarchar(120)=NULL,
 @SalesAccountCode nvarchar(120)=NULL,@PurchaseAccountCode nvarchar(120)=NULL,@SortOrder int=0,
 @SapGroupCode nvarchar(100)=NULL,@SapCode nvarchar(50)=NULL,@ExternalSystem nvarchar(50)=NULL,@ExternalCode nvarchar(100)=NULL,
 @CreatedByUserId int=NULL,@CreatedByUserName nvarchar(120)=NULL AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name));
 SET @IncomeAccountCode=COALESCE(@IncomeAccountCode,@SalesAccountCode);
 SET @PurchaseExpenseAccountCode=COALESCE(@PurchaseExpenseAccountCode,@PurchaseAccountCode);
 SET @SalesAccountCode=COALESCE(@SalesAccountCode,@IncomeAccountCode);
 SET @PurchaseAccountCode=COALESCE(@PurchaseAccountCode,@PurchaseExpenseAccountCode);
 IF @GlobalId IS NULL OR @GlobalId='00000000-0000-0000-0000-000000000000' THROW 51186,'ItemGroup GlobalId is required.',1;
 IF NULLIF(@Code,N'') IS NULL THROW 51002,'El codigo es obligatorio.',1;
 IF NULLIF(@Name,N'') IS NULL THROW 51003,'El nombre es obligatorio.',1;
 IF @SortOrder<0 THROW 51186,'ItemGroup SortOrder cannot be negative.',1;
 BEGIN TRY
  DECLARE @Own bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END; IF @Own=1 BEGIN TRANSACTION;
  IF EXISTS(SELECT 1 FROM dbo.ItemGroups WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code OR GlobalId=@GlobalId)
  BEGIN IF @Own=1 COMMIT; SELECT -1; RETURN; END;
  INSERT dbo.ItemGroups(GlobalId,Code,Name,Description,IsActive,
   InventoryAccountCode,IncomeAccountCode,CostOfSalesAccountCode,SalesReturnAccountCode,PurchaseReturnAccountCode,
   CostVarianceAccountCode,InventoryAdjustmentAccountCode,PurchaseExpenseAccountCode,SalesAccountCode,PurchaseAccountCode,
   SortOrder,IsSystem,SapGroupCode,SapCode,ExternalSystem,ExternalCode,CreatedByUserId,CreatedByUserName)
  VALUES(@GlobalId,@Code,@Name,@Description,@IsActive,
   @InventoryAccountCode,@IncomeAccountCode,@CostOfSalesAccountCode,@SalesReturnAccountCode,@PurchaseReturnAccountCode,
   @CostVarianceAccountCode,@InventoryAdjustmentAccountCode,@PurchaseExpenseAccountCode,@SalesAccountCode,@PurchaseAccountCode,
   @SortOrder,0,@SapGroupCode,@SapCode,@ExternalSystem,@ExternalCode,@CreatedByUserId,@CreatedByUserName);
  DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
  INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'ItemGroups',CONVERT(nvarchar(80),@Id),N'INSERT',FieldName,NULL,NewValue,@CreatedByUserId,@CreatedByUserName
  FROM(VALUES(N'Code',CONVERT(nvarchar(max),@Code)),(N'Name',CONVERT(nvarchar(max),@Name)),(N'Description',CONVERT(nvarchar(max),@Description)),
   (N'InventoryAccountCode',CONVERT(nvarchar(max),@InventoryAccountCode)),(N'IncomeAccountCode',CONVERT(nvarchar(max),@IncomeAccountCode)),
   (N'CostOfSalesAccountCode',CONVERT(nvarchar(max),@CostOfSalesAccountCode)),(N'SalesReturnAccountCode',CONVERT(nvarchar(max),@SalesReturnAccountCode)),
   (N'PurchaseReturnAccountCode',CONVERT(nvarchar(max),@PurchaseReturnAccountCode)),(N'CostVarianceAccountCode',CONVERT(nvarchar(max),@CostVarianceAccountCode)),
   (N'InventoryAdjustmentAccountCode',CONVERT(nvarchar(max),@InventoryAdjustmentAccountCode)),(N'PurchaseExpenseAccountCode',CONVERT(nvarchar(max),@PurchaseExpenseAccountCode)),
   (N'SortOrder',CONVERT(nvarchar(max),@SortOrder)),(N'SapGroupCode',CONVERT(nvarchar(max),@SapGroupCode)),(N'SapCode',CONVERT(nvarchar(max),@SapCode)),
   (N'ExternalSystem',CONVERT(nvarchar(max),@ExternalSystem)),(N'ExternalCode',CONVERT(nvarchar(max),@ExternalCode)),
   (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@IsActive))))v(FieldName,NewValue);
  IF @Own=1 COMMIT; SELECT @Id;
 END TRY BEGIN CATCH IF @Own=1 AND XACT_STATE()<>0 ROLLBACK; IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END; THROW; END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ITEM_GROUPS_ACTUALIZAR
 @Id int,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,@IsActive bit,
 @InventoryAccountCode nvarchar(120)=NULL,@IncomeAccountCode nvarchar(120)=NULL,@CostOfSalesAccountCode nvarchar(120)=NULL,
 @SalesReturnAccountCode nvarchar(120)=NULL,@PurchaseReturnAccountCode nvarchar(120)=NULL,@CostVarianceAccountCode nvarchar(120)=NULL,
 @InventoryAdjustmentAccountCode nvarchar(120)=NULL,@PurchaseExpenseAccountCode nvarchar(120)=NULL,
 @SalesAccountCode nvarchar(120)=NULL,@PurchaseAccountCode nvarchar(120)=NULL,@SortOrder int=0,
 @SapGroupCode nvarchar(100)=NULL,@SapCode nvarchar(50)=NULL,@ExternalSystem nvarchar(50)=NULL,@ExternalCode nvarchar(100)=NULL,
 @UpdatedByUserId int=NULL,@UpdatedByUserName nvarchar(120)=NULL AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name));
 SET @IncomeAccountCode=COALESCE(@IncomeAccountCode,@SalesAccountCode); SET @PurchaseExpenseAccountCode=COALESCE(@PurchaseExpenseAccountCode,@PurchaseAccountCode);
 SET @SalesAccountCode=COALESCE(@SalesAccountCode,@IncomeAccountCode); SET @PurchaseAccountCode=COALESCE(@PurchaseAccountCode,@PurchaseExpenseAccountCode);
 IF NULLIF(@Code,N'') IS NULL THROW 51002,'El codigo es obligatorio.',1; IF NULLIF(@Name,N'') IS NULL THROW 51003,'El nombre es obligatorio.',1;
 IF @SortOrder<0 THROW 51186,'ItemGroup SortOrder cannot be negative.',1;
 BEGIN TRY
  DECLARE @Own bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END; IF @Own=1 BEGIN TRANSACTION;
  DECLARE @OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldActive bit,@OldSystem bit,@OldSort int,
   @OldInv nvarchar(120),@OldIncome nvarchar(120),@OldCost nvarchar(120),@OldSalesReturn nvarchar(120),@OldPurchaseReturn nvarchar(120),
   @OldVariance nvarchar(120),@OldAdjustment nvarchar(120),@OldPurchaseExpense nvarchar(120),
   @OldSapGroup nvarchar(100),@OldSap nvarchar(50),@OldExternalSystem nvarchar(50),@OldExternalCode nvarchar(100);
  SELECT @OldCode=Code,@OldName=Name,@OldDescription=Description,@OldActive=IsActive,@OldSystem=IsSystem,@OldSort=SortOrder,
   @OldInv=InventoryAccountCode,@OldIncome=IncomeAccountCode,@OldCost=CostOfSalesAccountCode,@OldSalesReturn=SalesReturnAccountCode,
   @OldPurchaseReturn=PurchaseReturnAccountCode,@OldVariance=CostVarianceAccountCode,@OldAdjustment=InventoryAdjustmentAccountCode,
   @OldPurchaseExpense=PurchaseExpenseAccountCode,@OldSapGroup=SapGroupCode,@OldSap=SapCode,
   @OldExternalSystem=ExternalSystem,@OldExternalCode=ExternalCode FROM dbo.ItemGroups WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
  IF @OldCode IS NULL BEGIN IF @Own=1 COMMIT; SELECT 0; RETURN; END;
  IF @OldSystem=1 AND @OldCode<>@Code BEGIN IF @Own=1 COMMIT; SELECT -2; RETURN; END;
  IF EXISTS(SELECT 1 FROM dbo.ItemGroups WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND Id<>@Id) BEGIN IF @Own=1 COMMIT; SELECT -1; RETURN; END;
  UPDATE dbo.ItemGroups SET Code=@Code,Name=@Name,Description=@Description,IsActive=@IsActive,
   InventoryAccountCode=@InventoryAccountCode,IncomeAccountCode=@IncomeAccountCode,CostOfSalesAccountCode=@CostOfSalesAccountCode,
   SalesReturnAccountCode=@SalesReturnAccountCode,PurchaseReturnAccountCode=@PurchaseReturnAccountCode,CostVarianceAccountCode=@CostVarianceAccountCode,
   InventoryAdjustmentAccountCode=@InventoryAdjustmentAccountCode,PurchaseExpenseAccountCode=@PurchaseExpenseAccountCode,
   SalesAccountCode=@SalesAccountCode,PurchaseAccountCode=@PurchaseAccountCode,SortOrder=@SortOrder,SapGroupCode=@SapGroupCode,SapCode=@SapCode,
   ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,
   UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName,UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
  INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
  SELECT N'ItemGroups',CONVERT(nvarchar(80),@Id),N'UPDATE',FieldName,OldValue,NewValue,@UpdatedByUserId,@UpdatedByUserName FROM(VALUES
   (N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),(N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
   (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
   (N'InventoryAccountCode',CONVERT(nvarchar(max),@OldInv),CONVERT(nvarchar(max),@InventoryAccountCode)),(N'IncomeAccountCode',CONVERT(nvarchar(max),@OldIncome),CONVERT(nvarchar(max),@IncomeAccountCode)),
   (N'CostOfSalesAccountCode',CONVERT(nvarchar(max),@OldCost),CONVERT(nvarchar(max),@CostOfSalesAccountCode)),(N'SalesReturnAccountCode',CONVERT(nvarchar(max),@OldSalesReturn),CONVERT(nvarchar(max),@SalesReturnAccountCode)),
   (N'PurchaseReturnAccountCode',CONVERT(nvarchar(max),@OldPurchaseReturn),CONVERT(nvarchar(max),@PurchaseReturnAccountCode)),(N'CostVarianceAccountCode',CONVERT(nvarchar(max),@OldVariance),CONVERT(nvarchar(max),@CostVarianceAccountCode)),
   (N'InventoryAdjustmentAccountCode',CONVERT(nvarchar(max),@OldAdjustment),CONVERT(nvarchar(max),@InventoryAdjustmentAccountCode)),(N'PurchaseExpenseAccountCode',CONVERT(nvarchar(max),@OldPurchaseExpense),CONVERT(nvarchar(max),@PurchaseExpenseAccountCode)),
   (N'SortOrder',CONVERT(nvarchar(max),@OldSort),CONVERT(nvarchar(max),@SortOrder)),
   (N'SapGroupCode',CONVERT(nvarchar(max),@OldSapGroup),CONVERT(nvarchar(max),@SapGroupCode)),(N'SapCode',CONVERT(nvarchar(max),@OldSap),CONVERT(nvarchar(max),@SapCode)),
   (N'ExternalSystem',CONVERT(nvarchar(max),@OldExternalSystem),CONVERT(nvarchar(max),@ExternalSystem)),(N'ExternalCode',CONVERT(nvarchar(max),@OldExternalCode),CONVERT(nvarchar(max),@ExternalCode)),
   (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))))v(FieldName,OldValue,NewValue)
  WHERE ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
  IF @Own=1 COMMIT; SELECT 1;
 END TRY BEGIN CATCH IF @Own=1 AND XACT_STATE()<>0 ROLLBACK; IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1;RETURN;END; THROW; END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_ITEM_GROUPS_ELIMINAR @Id int,@DeletedByUserId int=NULL,@DeletedByUserName nvarchar(120)=NULL AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  DECLARE @Own bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END; IF @Own=1 BEGIN TRANSACTION;
  DECLARE @Active bit,@System bit; SELECT @Active=IsActive,@System=IsSystem FROM dbo.ItemGroups WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
  IF @Active IS NULL BEGIN IF @Own=1 COMMIT; SELECT 0;RETURN;END;
  IF @System=1 BEGIN IF @Own=1 COMMIT; SELECT -2;RETURN;END;
  IF EXISTS(SELECT 1 FROM dbo.Items WHERE ItemGroupId=@Id AND IsDeleted=0) OR EXISTS(SELECT 1 FROM dbo.ItemFamilies WHERE ItemGroupId=@Id AND IsDeleted=0)
  BEGIN IF @Own=1 COMMIT; SELECT -3;RETURN;END;
  UPDATE dbo.ItemGroups SET IsActive=0,IsDeleted=1,DeletedByUserId=@DeletedByUserId,DeletedByUserName=@DeletedByUserName,DeletedAt=SYSUTCDATETIME() WHERE Id=@Id AND IsDeleted=0;
  INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName) VALUES
   (N'ItemGroups',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@Active)),N'0',@DeletedByUserId,@DeletedByUserName),
   (N'ItemGroups',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);
  IF @Own=1 COMMIT; SELECT 1;
 END TRY BEGIN CATCH IF @Own=1 AND XACT_STATE()<>0 ROLLBACK; THROW; END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUP_SYNC_EXISTS_BY_GLOBAL_ID @GlobalId uniqueidentifier AS
BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.ItemGroups WHERE GlobalId=@GlobalId; END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_GROUP_SYNC_APPLY
 @GlobalId uniqueidentifier,@Code nvarchar(50),@Name nvarchar(150),@Description nvarchar(500)=NULL,
 @InventoryAccountCode nvarchar(120)=NULL,@IncomeAccountCode nvarchar(120)=NULL,@CostOfSalesAccountCode nvarchar(120)=NULL,
 @SalesReturnAccountCode nvarchar(120)=NULL,@PurchaseReturnAccountCode nvarchar(120)=NULL,@CostVarianceAccountCode nvarchar(120)=NULL,
 @InventoryAdjustmentAccountCode nvarchar(120)=NULL,@PurchaseExpenseAccountCode nvarchar(120)=NULL,
 @SalesAccountCode nvarchar(120)=NULL,@PurchaseAccountCode nvarchar(120)=NULL,@SortOrder int=0,@IsSystem bit=0,
 @SapGroupCode nvarchar(100)=NULL,@SapCode nvarchar(50)=NULL,@IsActive bit,@IsDeleted bit,
 @ExternalSystem nvarchar(50)=NULL,@ExternalCode nvarchar(100)=NULL,@CreatedAt datetime2(0),@UpdatedAt datetime2(0) AS
BEGIN
 SET NOCOUNT ON;
 /* Las cuentas contables son locales por tenant. Los parametros se conservan
    solo para tolerar productores anteriores; nunca se aplican en sucursales. */
 SET @IncomeAccountCode=COALESCE(@IncomeAccountCode,@SalesAccountCode); SET @PurchaseExpenseAccountCode=COALESCE(@PurchaseExpenseAccountCode,@PurchaseAccountCode);
 SET @SalesAccountCode=COALESCE(@SalesAccountCode,@IncomeAccountCode); SET @PurchaseAccountCode=COALESCE(@PurchaseAccountCode,@PurchaseExpenseAccountCode);
 DECLARE @Id int,@Conflict int,@WasNew bit=0;
 DECLARE @OldInventory nvarchar(120),@OldIncome nvarchar(120),@OldCost nvarchar(120),@OldSalesReturn nvarchar(120),
  @OldPurchaseReturn nvarchar(120),@OldVariance nvarchar(120),@OldAdjustment nvarchar(120),@OldPurchaseExpense nvarchar(120),
  @OldSort int,@OldSystem bit,@OldActive bit,@OldDeleted bit;
 SELECT @Id=Id,@OldInventory=InventoryAccountCode,@OldIncome=IncomeAccountCode,@OldCost=CostOfSalesAccountCode,
  @OldSalesReturn=SalesReturnAccountCode,@OldPurchaseReturn=PurchaseReturnAccountCode,@OldVariance=CostVarianceAccountCode,
  @OldAdjustment=InventoryAdjustmentAccountCode,@OldPurchaseExpense=PurchaseExpenseAccountCode,@OldSort=SortOrder,
  @OldSystem=IsSystem,@OldActive=IsActive,@OldDeleted=IsDeleted
 FROM dbo.ItemGroups WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
 SELECT @Conflict=Id FROM dbo.ItemGroups WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND (@Id IS NULL OR Id<>@Id);
 IF @Conflict IS NOT NULL BEGIN SELECT -2 ResultCode,CONVERT(int,NULL) ItemGroupId;RETURN;END;
 IF @Id IS NULL BEGIN
  SET @WasNew=1;
  INSERT dbo.ItemGroups(GlobalId,Code,Name,Description,InventoryAccountCode,IncomeAccountCode,CostOfSalesAccountCode,SalesReturnAccountCode,
   PurchaseReturnAccountCode,CostVarianceAccountCode,InventoryAdjustmentAccountCode,PurchaseExpenseAccountCode,SalesAccountCode,PurchaseAccountCode,
   SortOrder,IsSystem,SapGroupCode,SapCode,IsActive,IsDeleted,ExternalSystem,ExternalCode,CreatedAt,CreatedByUserName,DeletedAt,DeletedByUserName)
  VALUES(@GlobalId,@Code,@Name,@Description,NULL,NULL,NULL,NULL,
   NULL,NULL,NULL,NULL,NULL,NULL,
   @SortOrder,@IsSystem,@SapGroupCode,@SapCode,@IsActive,@IsDeleted,@ExternalSystem,@ExternalCode,@CreatedAt,N'MasterBranchSyncWorker',
   CASE WHEN @IsDeleted=1 THEN @UpdatedAt END,CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END); SET @Id=CONVERT(int,SCOPE_IDENTITY());
 END ELSE BEGIN
  UPDATE dbo.ItemGroups SET Code=@Code,Name=@Name,Description=@Description,
   SortOrder=@SortOrder,IsSystem=@IsSystem,SapGroupCode=@SapGroupCode,SapCode=@SapCode,
   IsActive=@IsActive,IsDeleted=@IsDeleted,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,UpdatedAt=@UpdatedAt,UpdatedByUserName=N'MasterBranchSyncWorker',
   DeletedAt=CASE WHEN @IsDeleted=1 THEN @UpdatedAt END,DeletedByUserName=CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END WHERE Id=@Id;
 END;
 INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
 SELECT N'ItemGroups',CONVERT(nvarchar(80),@Id),CASE WHEN @WasNew=1 THEN N'INSERT' WHEN @IsDeleted=1 AND ISNULL(@OldDeleted,0)=0 THEN N'DELETE' ELSE N'UPDATE' END,
  FieldName,CASE WHEN @WasNew=1 THEN NULL ELSE OldValue END,NewValue,N'MasterBranchSyncWorker',N'MasterBranchSyncWorker'
 FROM(VALUES
  (N'SortOrder',CONVERT(nvarchar(max),@OldSort),CONVERT(nvarchar(max),@SortOrder)),
  (N'IsSystem',CONVERT(nvarchar(max),CONVERT(int,@OldSystem)),CONVERT(nvarchar(max),CONVERT(int,@IsSystem))),
  (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
  (N'IsDeleted',CONVERT(nvarchar(max),CONVERT(int,@OldDeleted)),CONVERT(nvarchar(max),CONVERT(int,@IsDeleted))))v(FieldName,OldValue,NewValue)
 WHERE @WasNew=1 OR ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
 SELECT 1 ResultCode,@Id ItemGroupId;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260811.186')
 INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260811.186',N'ItemGroups con ocho cuentas contables, orden, sistema, auditoria y sync compatible');
GO
