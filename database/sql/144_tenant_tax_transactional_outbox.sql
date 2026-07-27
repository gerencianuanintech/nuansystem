/*
    Iteracion 8.7 - Tax transaccional Matriz-Sucursal.
    No ejecuta workers ni modifica configuracion Master.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Taxes', N'U') IS NULL THROW 51144, 'Taxes is required before migration 144.', 1;
IF OBJECT_ID(N'dbo.Items', N'U') IS NULL THROW 51144, 'Items is required before migration 144.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL THROW 51144, 'LocalOutbox is required before migration 144.', 1;
IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL THROW 51144, 'SyncInbox is required before migration 144.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL THROW 51144, 'SchemaHistory is required before migration 144.', 1;
IF OBJECT_ID(N'dbo.AuditCatalogChanges', N'U') IS NULL THROW 51144, 'AuditCatalogChanges from migration 106 is required before migration 144.', 1;
IF COL_LENGTH(N'dbo.Taxes', N'GlobalId') IS NULL OR COL_LENGTH(N'dbo.Taxes', N'Description') IS NULL
    OR COL_LENGTH(N'dbo.Taxes', N'ExternalSystem') IS NULL OR COL_LENGTH(N'dbo.Taxes', N'ExternalCode') IS NULL
    THROW 51144, 'Taxes sync columns are required before migration 144.', 1;
GO

IF EXISTS (SELECT Code FROM dbo.Taxes GROUP BY Code HAVING COUNT_BIG(1) > 1)
    THROW 51144, 'Tax codes, including tombstones, must be unique before migration 144.', 1;
IF EXISTS (SELECT 1 FROM dbo.Taxes WHERE Rate < 0 OR Rate > 1)
    THROW 51144, 'Tax Rate must use the decimal contract 0..1 before migration 144.', 1;
IF EXISTS (SELECT GlobalId FROM dbo.Taxes GROUP BY GlobalId HAVING COUNT_BIG(1) > 1)
    THROW 51144, 'Tax GlobalId values must be unique before migration 144.', 1;
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Taxes') AND name=N'UX_Taxes_Code_Active')
    DROP INDEX UX_Taxes_Code_Active ON dbo.Taxes;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Taxes') AND name=N'UQ_Taxes_Code' AND is_unique=1 AND filter_definition IS NULL)
    CREATE UNIQUE INDEX UQ_Taxes_Code ON dbo.Taxes(Code);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Taxes') AND name=N'UQ_Taxes_GlobalId' AND is_unique=1 AND filter_definition IS NULL)
    CREATE UNIQUE INDEX UQ_Taxes_GlobalId ON dbo.Taxes(GlobalId);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, GlobalId, Code, Name, Description, Rate, IsActive,
           ExternalSystem, ExternalCode, CreatedByUserId, CreatedByUserName,
           CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.Taxes WHERE IsDeleted=0 ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXES_BUSCARPORID @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, GlobalId, Code, Name, Description, Rate, IsActive,
           ExternalSystem, ExternalCode, CreatedByUserId, CreatedByUserName,
           CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.Taxes WHERE Id=@Id AND IsDeleted=0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXES_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Code, Name, Rate, IsActive
    FROM dbo.Taxes WHERE IsDeleted=0 AND IsActive=1 ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXES_HISTORIAL @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName,CreatedAt
    FROM dbo.AuditCatalogChanges
    WHERE EntityName=N'Tax' AND RecordId=CONVERT(nvarchar(80),@Id)
    ORDER BY CreatedAt DESC,Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXES_CODIGORESERVADO
    @Code nvarchar(50), @ExcluirId int=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM dbo.Taxes WITH (UPDLOCK,HOLDLOCK)
    WHERE Code=@Code AND (@ExcluirId IS NULL OR Id<>@ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXES_REFERENCIASITEMSACTIVOS @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM dbo.Items
    WHERE IsDeleted=0 AND IsActive=1 AND (PurchaseTaxId=@Id OR SalesTaxId=@Id);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_TAXES_CREAR
    @GlobalId uniqueidentifier, @Code nvarchar(50), @Name nvarchar(150),
    @Description nvarchar(500)=NULL, @Rate decimal(9,6), @IsActive bit,
    @CreatedByUserId int=NULL, @CreatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    IF @Rate < 0 OR @Rate > 1 THROW 51144, 'Tax Rate must be between 0 and 1.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT dbo.Taxes(GlobalId,Code,Name,Description,Rate,IsActive,CreatedByUserId,CreatedByUserName)
        VALUES(@GlobalId,@Code,@Name,@Description,@Rate,@IsActive,@CreatedByUserId,@CreatedByUserName);
        DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        SELECT N'Tax',CONVERT(nvarchar(80),@Id),N'INSERT',FieldName,NULL,NewValue,@CreatedByUserId,@CreatedByUserName
        FROM (VALUES
            (N'Code',CONVERT(nvarchar(max),@Code)),
            (N'Name',CONVERT(nvarchar(max),@Name)),
            (N'Description',CONVERT(nvarchar(max),@Description)),
            (N'Rate',CONVERT(nvarchar(max),@Rate)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@IsActive)))) changes(FieldName,NewValue);
        COMMIT; SELECT @Id;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_TAXES_ACTUALIZAR
    @Id int, @Code nvarchar(50), @Name nvarchar(150),
    @Description nvarchar(500)=NULL, @Rate decimal(9,6), @IsActive bit,
    @UpdatedByUserId int=NULL, @UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    IF @Rate < 0 OR @Rate > 1 THROW 51144, 'Tax Rate must be between 0 and 1.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),
            @OldRate decimal(9,6),@OldIsActive bit;
        SELECT @OldCode=Code,@OldName=Name,@OldDescription=Description,@OldRate=Rate,@OldIsActive=IsActive
        FROM dbo.Taxes WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
        IF @OldCode IS NULL BEGIN ROLLBACK; SELECT 0; RETURN; END;
        UPDATE dbo.Taxes SET Code=@Code,Name=@Name,Description=@Description,Rate=@Rate,IsActive=@IsActive,
            UpdatedAt=SYSUTCDATETIME(),UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName
        WHERE Id=@Id AND IsDeleted=0;
        IF @@ROWCOUNT=0 BEGIN ROLLBACK; SELECT 0; RETURN; END;
        INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        SELECT N'Tax',CONVERT(nvarchar(80),@Id),N'UPDATE',FieldName,OldValue,NewValue,@UpdatedByUserId,@UpdatedByUserName
        FROM (VALUES
            (N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),
            (N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
            (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
            (N'Rate',CONVERT(nvarchar(max),@OldRate),CONVERT(nvarchar(max),@Rate)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))))
            changes(FieldName,OldValue,NewValue)
        WHERE ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
        COMMIT; SELECT 1;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_TAXES_ELIMINAR
    @Id int, @DeletedByUserId int=NULL, @DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @OldIsActive bit;
        SELECT @OldIsActive=IsActive FROM dbo.Taxes WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
        IF @OldIsActive IS NULL BEGIN ROLLBACK; SELECT 0; RETURN; END;
        IF EXISTS(SELECT 1 FROM dbo.Items WITH(UPDLOCK,HOLDLOCK)
                  WHERE IsDeleted=0 AND IsActive=1 AND (PurchaseTaxId=@Id OR SalesTaxId=@Id))
            THROW 51144, 'Tax has active Item references.', 1;
        UPDATE dbo.Taxes SET IsDeleted=1,IsActive=0,DeletedAt=SYSUTCDATETIME(),
            DeletedByUserId=@DeletedByUserId,DeletedByUserName=@DeletedByUserName
        WHERE Id=@Id AND IsDeleted=0;
        IF @@ROWCOUNT=0 BEGIN ROLLBACK; SELECT 0; RETURN; END;
        INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        VALUES
            (N'Tax',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),N'0',@DeletedByUserId,@DeletedByUserName),
            (N'Tax',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);
        COMMIT; SELECT 1;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_TAX_SYNC_APPLY_EVENT
    @EventId uniqueidentifier, @SourceCompanyId int, @EntityName nvarchar(80),
    @EntityGlobalId uniqueidentifier, @Operation nvarchar(30), @PayloadJson nvarchar(max),
    @GlobalId uniqueidentifier, @Code nvarchar(50), @Name nvarchar(150),
    @Description nvarchar(500)=NULL, @Rate decimal(9,6), @IsActive bit, @IsDeleted bit,
    @ExternalSystem nvarchar(50)=NULL, @ExternalCode nvarchar(100)=NULL,
    @CreatedAt datetime2(0), @UpdatedAt datetime2(0)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRANSACTION;
    DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@TaxId int;
    DECLARE @OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),
        @OldRate decimal(9,6),@OldIsActive bit,@OldIsDeleted bit;
    SELECT @InboxId=Id,@InboxStatus=Status FROM dbo.SyncInbox WITH(UPDLOCK,HOLDLOCK) WHERE EventId=@EventId;
    IF @InboxStatus=N'Applied'
    BEGIN
        SELECT @TaxId=Id FROM dbo.Taxes WHERE GlobalId=@GlobalId;
        COMMIT; SELECT 2 ResultCode,@TaxId TaxId; RETURN;
    END;
    IF @InboxStatus=N'DeadLetter'
    BEGIN
        COMMIT; SELECT -2 ResultCode,CONVERT(int,NULL) TaxId; RETURN;
    END;
    IF @InboxId IS NULL
    BEGIN
        INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
        VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,N'Pending');
        SET @InboxId=CONVERT(bigint,SCOPE_IDENTITY());
    END;
    IF @Rate < 0 OR @Rate > 1
    BEGIN
        UPDATE dbo.SyncInbox SET Status=N'DeadLetter',AttemptCount=AttemptCount+1,
            ErrorMessage=N'Tax rate is outside decimal contract.',LastErrorMessage=N'Tax rate is outside decimal contract.',NextRetryAt=NULL
        WHERE Id=@InboxId;
        COMMIT; SELECT -3 ResultCode,CONVERT(int,NULL) TaxId; RETURN;
    END;
    IF EXISTS(SELECT 1 FROM dbo.Taxes WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND GlobalId<>@GlobalId)
    BEGIN
        UPDATE dbo.SyncInbox SET Status=N'DeadLetter',AttemptCount=AttemptCount+1,
            ErrorMessage=N'Tax code belongs to another GlobalId.',LastErrorMessage=N'Tax code belongs to another GlobalId.',NextRetryAt=NULL
        WHERE Id=@InboxId;
        COMMIT; SELECT -2 ResultCode,CONVERT(int,NULL) TaxId; RETURN;
    END;
    SELECT @TaxId=Id,@OldCode=Code,@OldName=Name,@OldDescription=Description,
        @OldRate=Rate,@OldIsActive=IsActive,@OldIsDeleted=IsDeleted
    FROM dbo.Taxes WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
    IF @TaxId IS NULL
    BEGIN
        INSERT dbo.Taxes(GlobalId,Code,Name,Description,Rate,IsActive,IsDeleted,ExternalSystem,ExternalCode,
            CreatedAt,CreatedByUserName,DeletedAt,DeletedByUserName)
        VALUES(@GlobalId,@Code,@Name,@Description,@Rate,@IsActive,@IsDeleted,@ExternalSystem,@ExternalCode,
            COALESCE(@CreatedAt,SYSUTCDATETIME()),N'MasterBranchSyncWorker',
            CASE WHEN @IsDeleted=1 THEN SYSUTCDATETIME() END,
            CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END);
        SET @TaxId=CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
        SELECT N'Tax',CONVERT(nvarchar(80),@TaxId),N'INSERT',FieldName,NULL,NewValue,
            N'MasterBranchSyncWorker',N'MasterBranchSyncWorker'
        FROM (VALUES
            (N'Code',CONVERT(nvarchar(max),@Code)),
            (N'Name',CONVERT(nvarchar(max),@Name)),
            (N'Description',CONVERT(nvarchar(max),@Description)),
            (N'Rate',CONVERT(nvarchar(max),@Rate)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
            (N'IsDeleted',CONVERT(nvarchar(max),CONVERT(int,@IsDeleted)))) changes(FieldName,NewValue);
    END
    ELSE
    BEGIN
        UPDATE dbo.Taxes SET Code=@Code,Name=@Name,Description=@Description,Rate=@Rate,
            IsActive=@IsActive,IsDeleted=@IsDeleted,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,
            UpdatedAt=COALESCE(@UpdatedAt,SYSUTCDATETIME()),UpdatedByUserName=N'MasterBranchSyncWorker',
            DeletedAt=CASE WHEN @IsDeleted=1 THEN COALESCE(DeletedAt,SYSUTCDATETIME()) ELSE NULL END,
            DeletedByUserName=CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' ELSE NULL END
        WHERE Id=@TaxId;
        INSERT dbo.AuditCatalogChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
        SELECT N'Tax',CONVERT(nvarchar(80),@TaxId),
            CASE WHEN @IsDeleted=1 AND @OldIsDeleted=0 THEN N'DELETE' ELSE N'UPDATE' END,
            FieldName,OldValue,NewValue,N'MasterBranchSyncWorker',N'MasterBranchSyncWorker'
        FROM (VALUES
            (N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),
            (N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
            (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
            (N'Rate',CONVERT(nvarchar(max),@OldRate),CONVERT(nvarchar(max),@Rate)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
            (N'IsDeleted',CONVERT(nvarchar(max),CONVERT(int,@OldIsDeleted)),CONVERT(nvarchar(max),CONVERT(int,@IsDeleted))))
            changes(FieldName,OldValue,NewValue)
        WHERE ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
    END;
    UPDATE dbo.SyncInbox SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),
        ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL WHERE Id=@InboxId;
    COMMIT; SELECT 1 ResultCode,@TaxId TaxId;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260727.144')
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260727.144',N'Tax transaccional, tasa decimal y conflicto terminal sin adopcion');
GO
