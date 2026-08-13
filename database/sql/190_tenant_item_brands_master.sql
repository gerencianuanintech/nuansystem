/*
    Funcionaliza Marcas de articulos como maestro independiente.

    Solo tenant. Prerrequisitos: 044, 065, 106 y SchemaHistory.
    GlobalId es la unica identidad entre tenants. Code queda reservado incluso
    despues del borrado logico para impedir adopciones automaticas.

    ExternalSystem, ExternalCode, SapManufacturerCode y SapCode son referencias
    locales de cada empresa/sucursal: no forman parte de Full ni de Sync Apply.
    Los procedimientos de escritura respetan una transaccion externa para que
    el backend confirme maestro + LocalOutbox en una sola unidad atomica.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.ItemBrands', N'U') IS NULL THROW 51190, 'ItemBrands from migration 044 is required.', 1;
IF OBJECT_ID(N'dbo.AuditInventoryChanges', N'U') IS NULL THROW 51190, 'AuditInventoryChanges is required.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL THROW 51190, 'LocalOutbox is required.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL THROW 51190, 'SchemaHistory is required.', 1;
GO

IF COL_LENGTH(N'dbo.ItemBrands', N'GlobalId') IS NULL
    ALTER TABLE dbo.ItemBrands ADD GlobalId uniqueidentifier NULL;
IF COL_LENGTH(N'dbo.ItemBrands', N'SortOrder') IS NULL
    ALTER TABLE dbo.ItemBrands ADD SortOrder int NULL;
IF COL_LENGTH(N'dbo.ItemBrands', N'ExternalSystem') IS NULL
    ALTER TABLE dbo.ItemBrands ADD ExternalSystem nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.ItemBrands', N'ExternalCode') IS NULL
    ALTER TABLE dbo.ItemBrands ADD ExternalCode nvarchar(100) NULL;
IF COL_LENGTH(N'dbo.ItemBrands', N'SapManufacturerCode') IS NULL
    ALTER TABLE dbo.ItemBrands ADD SapManufacturerCode nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.ItemBrands', N'SapCode') IS NULL
    ALTER TABLE dbo.ItemBrands ADD SapCode nvarchar(50) NULL;
GO

UPDATE dbo.ItemBrands SET GlobalId = NEWID() WHERE GlobalId IS NULL;
UPDATE dbo.ItemBrands SET SortOrder = 0 WHERE SortOrder IS NULL;
GO

IF EXISTS (SELECT 1 FROM dbo.ItemBrands WHERE NULLIF(LTRIM(RTRIM(Code)), N'') IS NULL)
    THROW 51190, 'ItemBrands contains blank codes; repair data before migration 190.', 1;
IF EXISTS (SELECT 1 FROM dbo.ItemBrands WHERE NULLIF(LTRIM(RTRIM(Name)), N'') IS NULL)
    THROW 51190, 'ItemBrands contains blank names; repair data before migration 190.', 1;
IF EXISTS (SELECT 1 FROM dbo.ItemBrands WHERE SortOrder < 0)
    THROW 51190, 'ItemBrands contains negative SortOrder values; repair data before migration 190.', 1;
IF EXISTS (SELECT Code FROM dbo.ItemBrands GROUP BY Code HAVING COUNT_BIG(*) > 1)
    THROW 51190, 'ItemBrands contains duplicated codes including tombstones; reconcile identities before migration 190.', 1;
IF EXISTS (SELECT GlobalId FROM dbo.ItemBrands GROUP BY GlobalId HAVING COUNT_BIG(*) > 1)
    THROW 51190, 'ItemBrands contains duplicated GlobalId values; repair data before migration 190.', 1;
IF EXISTS
(
    SELECT 1 FROM dbo.ItemBrands
    WHERE (NULLIF(LTRIM(RTRIM(ExternalSystem)), N'') IS NULL
           AND NULLIF(LTRIM(RTRIM(ExternalCode)), N'') IS NOT NULL)
       OR (NULLIF(LTRIM(RTRIM(ExternalSystem)), N'') IS NOT NULL
           AND NULLIF(LTRIM(RTRIM(ExternalCode)), N'') IS NULL)
)
    THROW 51190, 'ItemBrands external system and code must be both null or both informed.', 1;
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'GlobalId' AND is_nullable = 1)
    ALTER TABLE dbo.ItemBrands ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'SortOrder' AND is_nullable = 1)
    ALTER TABLE dbo.ItemBrands ALTER COLUMN SortOrder int NOT NULL;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.default_constraints d
    JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
    WHERE d.parent_object_id = OBJECT_ID(N'dbo.ItemBrands') AND c.name = N'GlobalId'
)
    ALTER TABLE dbo.ItemBrands ADD CONSTRAINT DF_ItemBrands_GlobalId DEFAULT NEWID() FOR GlobalId;
IF NOT EXISTS
(
    SELECT 1 FROM sys.default_constraints d
    JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
    WHERE d.parent_object_id = OBJECT_ID(N'dbo.ItemBrands') AND c.name = N'SortOrder'
)
    ALTER TABLE dbo.ItemBrands ADD CONSTRAINT DF_ItemBrands_SortOrder DEFAULT (0) FOR SortOrder;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'CK_ItemBrands_Code_NotBlank')
    ALTER TABLE dbo.ItemBrands ADD CONSTRAINT CK_ItemBrands_Code_NotBlank CHECK (NULLIF(LTRIM(RTRIM(Code)), N'') IS NOT NULL);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'CK_ItemBrands_Name_NotBlank')
    ALTER TABLE dbo.ItemBrands ADD CONSTRAINT CK_ItemBrands_Name_NotBlank CHECK (NULLIF(LTRIM(RTRIM(Name)), N'') IS NOT NULL);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'CK_ItemBrands_SortOrder')
    ALTER TABLE dbo.ItemBrands ADD CONSTRAINT CK_ItemBrands_SortOrder CHECK (SortOrder >= 0);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'CK_ItemBrands_ExternalReferencePair')
    ALTER TABLE dbo.ItemBrands ADD CONSTRAINT CK_ItemBrands_ExternalReferencePair CHECK
    (
        (NULLIF(LTRIM(RTRIM(ExternalSystem)), N'') IS NULL AND NULLIF(LTRIM(RTRIM(ExternalCode)), N'') IS NULL)
        OR
        (NULLIF(LTRIM(RTRIM(ExternalSystem)), N'') IS NOT NULL AND NULLIF(LTRIM(RTRIM(ExternalCode)), N'') IS NOT NULL)
    );
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'UX_ItemBrands_Code_Active')
    DROP INDEX UX_ItemBrands_Code_Active ON dbo.ItemBrands;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'UX_ItemBrands_Code')
    CREATE UNIQUE INDEX UX_ItemBrands_Code ON dbo.ItemBrands(Code);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'UX_ItemBrands_GlobalId')
    CREATE UNIQUE INDEX UX_ItemBrands_GlobalId ON dbo.ItemBrands(GlobalId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'IX_ItemBrands_Active_SortOrder_Name')
    CREATE INDEX IX_ItemBrands_Active_SortOrder_Name ON dbo.ItemBrands(IsActive, SortOrder, Name) INCLUDE(Code, GlobalId) WHERE IsDeleted = 0;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemBrands') AND name = N'IX_ItemBrands_ExternalRef')
    CREATE INDEX IX_ItemBrands_ExternalRef ON dbo.ItemBrands(ExternalSystem, ExternalCode)
        WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_BRANDS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, GlobalId, Code, Name, Description, SortOrder, IsActive,
           ExternalSystem, ExternalCode, SapManufacturerCode, SapCode,
           CreatedByUserId, CreatedByUserName, CreatedAt,
           UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.ItemBrands
    WHERE IsDeleted = 0
    ORDER BY SortOrder, Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_BRANDS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (1) Id, GlobalId, Code, Name, Description, SortOrder, IsActive,
           ExternalSystem, ExternalCode, SapManufacturerCode, SapCode,
           CreatedByUserId, CreatedByUserName, CreatedAt,
           UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.ItemBrands WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_BRANDS_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, GlobalId, Code, Name, SortOrder, IsActive
    FROM dbo.ItemBrands
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY SortOrder, Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_BRANDSBUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET @Code = LTRIM(RTRIM(@Code));
    SELECT COUNT(1) FROM dbo.ItemBrands
    WHERE Code = @Code AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_BRANDS_HISTORIAL
    @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, EntityName, RecordId, [Action], FieldName, OldValue, NewValue,
           UserId, UserName, [Source], CreatedAt
    FROM dbo.AuditInventoryChanges
    WHERE EntityName = N'ItemBrands' AND RecordId = CONVERT(nvarchar(80), @Id)
    ORDER BY CreatedAt DESC, Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_BRANDS_CREAR
    @GlobalId uniqueidentifier,
    @Code nvarchar(50), @Name nvarchar(150), @Description nvarchar(500) = NULL,
    @SortOrder int = 0, @IsActive bit,
    @ExternalSystem nvarchar(50) = NULL, @ExternalCode nvarchar(100) = NULL,
    @SapManufacturerCode nvarchar(50) = NULL, @SapCode nvarchar(50) = NULL,
    @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    SET @Code = LTRIM(RTRIM(@Code)); SET @Name = LTRIM(RTRIM(@Name));
    SET @ExternalSystem = NULLIF(LTRIM(RTRIM(@ExternalSystem)), N'');
    SET @ExternalCode = NULLIF(LTRIM(RTRIM(@ExternalCode)), N'');
    SET @SapManufacturerCode = NULLIF(LTRIM(RTRIM(@SapManufacturerCode)), N'');
    SET @SapCode = NULLIF(LTRIM(RTRIM(@SapCode)), N'');
    IF @GlobalId IS NULL OR @GlobalId = '00000000-0000-0000-0000-000000000000' THROW 51190, 'ItemBrand GlobalId is required.', 1;
    IF NULLIF(@Code, N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(@Name, N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @SortOrder < 0 THROW 51190, 'ItemBrand SortOrder cannot be negative.', 1;
    IF (@ExternalSystem IS NULL AND @ExternalCode IS NOT NULL) OR (@ExternalSystem IS NOT NULL AND @ExternalCode IS NULL)
        THROW 51190, 'External system and code must be informed together.', 1;
    BEGIN TRY
        DECLARE @OwnTransaction bit = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END;
        IF @OwnTransaction = 1 BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM dbo.ItemBrands WITH (UPDLOCK, HOLDLOCK) WHERE Code = @Code OR GlobalId = @GlobalId)
        BEGIN
            IF @OwnTransaction = 1 COMMIT; SELECT -1; RETURN;
        END;
        INSERT dbo.ItemBrands
        (GlobalId, Code, Name, Description, SortOrder, IsActive, IsDeleted,
         ExternalSystem, ExternalCode, SapManufacturerCode, SapCode,
         CreatedByUserId, CreatedByUserName, CreatedAt)
        VALUES
        (@GlobalId, @Code, @Name, @Description, @SortOrder, @IsActive, 0,
         @ExternalSystem, @ExternalCode, @SapManufacturerCode, @SapCode,
         @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME());
        DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());
        INSERT dbo.AuditInventoryChanges
        (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        SELECT N'ItemBrands', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
        FROM (VALUES
            (N'Code', CONVERT(nvarchar(max), @Code)), (N'Name', CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @Description)), (N'SortOrder', CONVERT(nvarchar(max), @SortOrder)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
            (N'ExternalSystem', CONVERT(nvarchar(max), @ExternalSystem)), (N'ExternalCode', CONVERT(nvarchar(max), @ExternalCode)),
            (N'SapManufacturerCode', CONVERT(nvarchar(max), @SapManufacturerCode)), (N'SapCode', CONVERT(nvarchar(max), @SapCode))
        ) auditValues(FieldName, NewValue);
        IF @OwnTransaction = 1 COMMIT; SELECT @Id;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK;
        IF ERROR_NUMBER() IN (2601, 2627) BEGIN SELECT -1; RETURN; END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ITEM_BRANDS_ACTUALIZAR
    @Id int, @Code nvarchar(50), @Name nvarchar(150), @Description nvarchar(500) = NULL,
    @SortOrder int = 0, @IsActive bit,
    @ExternalSystem nvarchar(50) = NULL, @ExternalCode nvarchar(100) = NULL,
    @SapManufacturerCode nvarchar(50) = NULL, @SapCode nvarchar(50) = NULL,
    @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    SET @Code = LTRIM(RTRIM(@Code)); SET @Name = LTRIM(RTRIM(@Name));
    SET @ExternalSystem = NULLIF(LTRIM(RTRIM(@ExternalSystem)), N'');
    SET @ExternalCode = NULLIF(LTRIM(RTRIM(@ExternalCode)), N'');
    SET @SapManufacturerCode = NULLIF(LTRIM(RTRIM(@SapManufacturerCode)), N'');
    SET @SapCode = NULLIF(LTRIM(RTRIM(@SapCode)), N'');
    IF NULLIF(@Code, N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(@Name, N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @SortOrder < 0 THROW 51190, 'ItemBrand SortOrder cannot be negative.', 1;
    IF (@ExternalSystem IS NULL AND @ExternalCode IS NOT NULL) OR (@ExternalSystem IS NOT NULL AND @ExternalCode IS NULL)
        THROW 51190, 'External system and code must be informed together.', 1;
    BEGIN TRY
        DECLARE @OwnTransaction bit = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END;
        IF @OwnTransaction = 1 BEGIN TRANSACTION;
        DECLARE @OldCode nvarchar(50), @OldName nvarchar(150), @OldDescription nvarchar(500), @OldSortOrder int, @OldIsActive bit,
                @OldExternalSystem nvarchar(50), @OldExternalCode nvarchar(100), @OldSapManufacturerCode nvarchar(50), @OldSapCode nvarchar(50);
        SELECT @OldCode=Code, @OldName=Name, @OldDescription=Description, @OldSortOrder=SortOrder, @OldIsActive=IsActive,
               @OldExternalSystem=ExternalSystem, @OldExternalCode=ExternalCode,
               @OldSapManufacturerCode=SapManufacturerCode, @OldSapCode=SapCode
        FROM dbo.ItemBrands WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
        IF @OldCode IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;
        IF EXISTS (SELECT 1 FROM dbo.ItemBrands WITH (UPDLOCK, HOLDLOCK) WHERE Code=@Code AND Id<>@Id)
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
        UPDATE dbo.ItemBrands SET Code=@Code, Name=@Name, Description=@Description, SortOrder=@SortOrder, IsActive=@IsActive,
               ExternalSystem=@ExternalSystem, ExternalCode=@ExternalCode, SapManufacturerCode=@SapManufacturerCode, SapCode=@SapCode,
               UpdatedByUserId=@UpdatedByUserId, UpdatedByUserName=@UpdatedByUserName, UpdatedAt=SYSUTCDATETIME()
        WHERE Id=@Id AND IsDeleted=0;
        INSERT dbo.AuditInventoryChanges
        (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        SELECT N'ItemBrands', CONVERT(nvarchar(80),@Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
        FROM (VALUES
            (N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),
            (N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
            (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
            (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
            (N'ExternalSystem',CONVERT(nvarchar(max),@OldExternalSystem),CONVERT(nvarchar(max),@ExternalSystem)),
            (N'ExternalCode',CONVERT(nvarchar(max),@OldExternalCode),CONVERT(nvarchar(max),@ExternalCode)),
            (N'SapManufacturerCode',CONVERT(nvarchar(max),@OldSapManufacturerCode),CONVERT(nvarchar(max),@SapManufacturerCode)),
            (N'SapCode',CONVERT(nvarchar(max),@OldSapCode),CONVERT(nvarchar(max),@SapCode))
        ) auditValues(FieldName,OldValue,NewValue)
        WHERE ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
        IF @OwnTransaction=1 COMMIT; SELECT 1;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        IF ERROR_NUMBER() IN (2601,2627) BEGIN SELECT -1; RETURN; END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_ITEM_BRANDS_ELIMINAR
    @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;
        DECLARE @OldIsActive bit;
        SELECT @OldIsActive=IsActive FROM dbo.ItemBrands WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
        IF @OldIsActive IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;
        UPDATE dbo.ItemBrands SET IsActive=0, IsDeleted=1, DeletedByUserId=@DeletedByUserId,
               DeletedByUserName=@DeletedByUserName, DeletedAt=SYSUTCDATETIME()
        WHERE Id=@Id AND IsDeleted=0;
        INSERT dbo.AuditInventoryChanges
        (EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        VALUES
        (N'ItemBrands',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),N'0',@DeletedByUserId,@DeletedByUserName),
        (N'ItemBrands',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);
        IF @OwnTransaction=1 COMMIT; SELECT 1;
    END TRY
    BEGIN CATCH IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK; THROW; END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_BRAND_SYNC_FULL
    @AfterId int = NULL, @BatchSize int = 100
AS
BEGIN
    SET NOCOUNT ON;
    IF @BatchSize < 1 OR @BatchSize > 10001 THROW 51190, 'ItemBrand Full BatchSize must be between 1 and 10001.', 1;
    SELECT TOP (@BatchSize) Id, GlobalId, Code, Name, Description, SortOrder, IsActive, IsDeleted,
           CreatedAt, UpdatedAt
    FROM dbo.ItemBrands WHERE @AfterId IS NULL OR Id>@AfterId ORDER BY Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_BRAND_SYNC_APPLY
    @GlobalId uniqueidentifier, @Code nvarchar(50), @Name nvarchar(150),
    @Description nvarchar(500) = NULL, @SortOrder int = 0,
    @IsActive bit, @IsDeleted bit, @UpdatedAt datetime2(0) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name));
    IF @GlobalId IS NULL OR @GlobalId='00000000-0000-0000-0000-000000000000' THROW 51190,'ItemBrand GlobalId is required for sync.',1;
    IF NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL OR @SortOrder<0 THROW 51190,'ItemBrand sync payload is invalid.',1;
    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;
        DECLARE @ItemBrandId int, @OldCode nvarchar(50), @OldName nvarchar(150), @OldDescription nvarchar(500),
                @OldSortOrder int, @OldIsActive bit, @OldIsDeleted bit;
        IF EXISTS (SELECT 1 FROM dbo.ItemBrands WITH (UPDLOCK,HOLDLOCK) WHERE Code=@Code AND GlobalId<>@GlobalId)
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -2 AS ResultCode,CONVERT(int,NULL) AS ItemBrandId; RETURN; END;
        SELECT @ItemBrandId=Id,@OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSortOrder=SortOrder,
               @OldIsActive=IsActive,@OldIsDeleted=IsDeleted
        FROM dbo.ItemBrands WITH (UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
        IF @ItemBrandId IS NULL
        BEGIN
            INSERT dbo.ItemBrands
            (GlobalId,Code,Name,Description,SortOrder,IsActive,IsDeleted,CreatedAt,CreatedByUserName,
             ExternalSystem,ExternalCode,SapManufacturerCode,SapCode,DeletedAt,DeletedByUserName)
            VALUES
            (@GlobalId,@Code,@Name,@Description,@SortOrder,@IsActive,@IsDeleted,COALESCE(@UpdatedAt,SYSUTCDATETIME()),N'MasterBranchSyncWorker',
             NULL,NULL,NULL,NULL,CASE WHEN @IsDeleted=1 THEN COALESCE(@UpdatedAt,SYSUTCDATETIME()) END,
             CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END);
            SET @ItemBrandId=CONVERT(int,SCOPE_IDENTITY());
        END
        ELSE
        BEGIN
            UPDATE dbo.ItemBrands SET Code=@Code,Name=@Name,Description=@Description,SortOrder=@SortOrder,
                   IsActive=@IsActive,IsDeleted=@IsDeleted,UpdatedAt=COALESCE(@UpdatedAt,SYSUTCDATETIME()),
                   UpdatedByUserName=N'MasterBranchSyncWorker',
                   DeletedAt=CASE WHEN @IsDeleted=1 THEN COALESCE(DeletedAt,@UpdatedAt,SYSUTCDATETIME()) ELSE NULL END,
                   DeletedByUserName=CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' ELSE NULL END
            WHERE Id=@ItemBrandId;
            /* Intencional: no modificar referencias External/SAP locales. */
        END;
        INSERT dbo.AuditInventoryChanges
        (EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
        SELECT N'ItemBrands',CONVERT(nvarchar(80),@ItemBrandId),
               CASE WHEN @OldCode IS NULL THEN N'INSERT' WHEN @IsDeleted=1 AND ISNULL(@OldIsDeleted,0)=0 THEN N'DELETE' ELSE N'UPDATE' END,
               FieldName,OldValue,NewValue,N'MasterBranchSyncWorker',N'MasterBranchSyncWorker'
        FROM (VALUES
            (N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),
            (N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
            (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
            (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
            (N'IsDeleted',CONVERT(nvarchar(max),CONVERT(int,@OldIsDeleted)),CONVERT(nvarchar(max),CONVERT(int,@IsDeleted)))
        ) auditValues(FieldName,OldValue,NewValue)
        WHERE @OldCode IS NULL OR ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
        IF @OwnTransaction=1 COMMIT;
        SELECT 1 AS ResultCode,@ItemBrandId AS ItemBrandId;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        IF ERROR_NUMBER() IN (2601,2627) BEGIN SELECT -2 AS ResultCode,CONVERT(int,NULL) AS ItemBrandId; RETURN; END;
        THROW;
    END CATCH;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260812.190')
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260812.190',N'Funcionaliza Marcas de articulos con GlobalId, CRUD, auditoria y sync local-safe');
GO
