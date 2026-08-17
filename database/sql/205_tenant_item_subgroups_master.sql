/*
    Funcionaliza Subgrupos de articulos como maestro dependiente de Familias.

    Solo tenant. Evoluciona el catalogo generico creado en 044 sin adoptar
    relaciones ambiguas. Los subgrupos historicos usados por articulos se
    separan por familia; el seed GENERAL se replica por familia activa.
    Cualquier otro registro historico sin familia detiene la migracion para
    evitar una asignacion inventada.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() = N'NuanSystem_Master' THROW 51205, 'Migration 205 must run only in tenant databases.', 1;
IF OBJECT_ID(N'dbo.ItemSubgroups', N'U') IS NULL THROW 51205, 'ItemSubgroups from migration 044 is required.', 1;
IF OBJECT_ID(N'dbo.ItemFamilies', N'U') IS NULL THROW 51205, 'ItemFamilies is required.', 1;
IF OBJECT_ID(N'dbo.Items', N'U') IS NULL THROW 51205, 'Items is required.', 1;
IF OBJECT_ID(N'dbo.ItemMasterProfiles', N'U') IS NULL THROW 51205, 'ItemMasterProfiles is required.', 1;
IF OBJECT_ID(N'dbo.AuditInventoryChanges', N'U') IS NULL THROW 51205, 'AuditInventoryChanges is required.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL THROW 51205, 'LocalOutbox is required.', 1;
IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL THROW 51205, 'SyncInbox is required.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL THROW 51205, 'SchemaHistory is required.', 1;
IF COL_LENGTH(N'dbo.ItemFamilies', N'GlobalId') IS NULL THROW 51205, 'ItemFamilies.GlobalId is required.', 1;
GO

IF COL_LENGTH(N'dbo.ItemSubgroups', N'GlobalId') IS NULL
    ALTER TABLE dbo.ItemSubgroups ADD GlobalId uniqueidentifier NULL;
IF COL_LENGTH(N'dbo.ItemSubgroups', N'ItemFamilyId') IS NULL
    ALTER TABLE dbo.ItemSubgroups ADD ItemFamilyId int NULL;
IF COL_LENGTH(N'dbo.ItemSubgroups', N'SortOrder') IS NULL
    ALTER TABLE dbo.ItemSubgroups ADD SortOrder int NULL;
GO

/* El indice legado imponia Code global; el nuevo dominio permite repetirlo
   entre familias y reserva el codigo solamente dentro de cada familia. */
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemSubgroups') AND name = N'UX_ItemSubgroups_Code_Active')
    DROP INDEX UX_ItemSubgroups_Code_Active ON dbo.ItemSubgroups;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    CREATE TABLE #Assignments
    (
        ItemSubgroupId int NOT NULL,
        ItemFamilyId int NOT NULL,
        CONSTRAINT PK_Assignments PRIMARY KEY(ItemSubgroupId, ItemFamilyId)
    );

    /* La relacion historica se obtiene solo cuando el propio articulo aporta
       simultaneamente familia y codigo de subgrupo. */
    INSERT #Assignments(ItemSubgroupId, ItemFamilyId)
    SELECT DISTINCT subgroup.Id, item.ItemFamilyId
    FROM dbo.ItemSubgroups subgroup
    INNER JOIN dbo.ItemMasterProfiles profile
        ON profile.IsDeleted = 0 AND profile.IsActive = 1
       AND NULLIF(LTRIM(RTRIM(JSON_VALUE(profile.MasterDataJson, N'$.general.subGroup'))), N'') = subgroup.Code
    INNER JOIN dbo.Items item
        ON item.Id = profile.ItemId AND item.IsDeleted = 0 AND item.ItemFamilyId IS NOT NULL
    INNER JOIN dbo.ItemFamilies family
        ON family.Id = item.ItemFamilyId AND family.IsDeleted = 0
    WHERE subgroup.ItemFamilyId IS NULL;

    /* GENERAL era el unico seed tecnico de 044: se convierte en un GENERAL
       propio por cada familia activa cuando todavia no tiene uso historico. */
    INSERT #Assignments(ItemSubgroupId, ItemFamilyId)
    SELECT subgroup.Id, family.Id
    FROM dbo.ItemSubgroups subgroup
    CROSS JOIN dbo.ItemFamilies family
    WHERE subgroup.ItemFamilyId IS NULL
      AND subgroup.Code = N'GENERAL'
      AND family.IsDeleted = 0
      AND NOT EXISTS (SELECT 1 FROM #Assignments a WHERE a.ItemSubgroupId = subgroup.Id)
      AND NOT EXISTS
      (
          SELECT 1 FROM #Assignments existing
          WHERE existing.ItemSubgroupId = subgroup.Id AND existing.ItemFamilyId = family.Id
      );

    /* En una base nueva puede existir solamente el seed tecnico GENERAL de
       044 y todavia ninguna familia. Como no tiene consumidores ni relacion
       posible, se retira para no inventar una familia ni bloquear el alta
       posterior del primer maestro. */
    DELETE subgroup
    FROM dbo.ItemSubgroups subgroup
    WHERE subgroup.ItemFamilyId IS NULL
      AND subgroup.Code = N'GENERAL'
      AND NOT EXISTS (SELECT 1 FROM #Assignments a WHERE a.ItemSubgroupId = subgroup.Id)
      AND NOT EXISTS (SELECT 1 FROM dbo.ItemFamilies family WHERE family.IsDeleted = 0)
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.ItemMasterProfiles profile
          WHERE profile.IsDeleted = 0
            AND NULLIF(LTRIM(RTRIM(JSON_VALUE(profile.MasterDataJson, N'$.general.subGroup'))), N'') = subgroup.Code
      );

    IF EXISTS
    (
        SELECT 1
        FROM dbo.ItemSubgroups subgroup
        WHERE subgroup.ItemFamilyId IS NULL
          AND NOT EXISTS (SELECT 1 FROM #Assignments a WHERE a.ItemSubgroupId = subgroup.Id)
    )
        THROW 51205, 'ItemSubgroups contains legacy rows without an unambiguous ItemFamily. Map them before migration 205.', 1;

    /* Conserva el Id original para la primera familia y clona los casos donde
       el antiguo codigo global se utilizaba en mas de una familia. */
    UPDATE subgroup
    SET ItemFamilyId = assignment.ItemFamilyId
    FROM dbo.ItemSubgroups subgroup
    INNER JOIN
    (
        SELECT ItemSubgroupId, MIN(ItemFamilyId) ItemFamilyId
        FROM #Assignments GROUP BY ItemSubgroupId
    ) assignment ON assignment.ItemSubgroupId = subgroup.Id
    WHERE subgroup.ItemFamilyId IS NULL;

    INSERT dbo.ItemSubgroups
    (
        GlobalId, ItemFamilyId, Code, Name, Description, SortOrder,
        IsActive, IsDeleted, CreatedAt, CreatedByUserId, CreatedByUserName,
        UpdatedAt, UpdatedByUserId, UpdatedByUserName,
        DeletedAt, DeletedByUserId, DeletedByUserName
    )
    SELECT NEWID(), assignment.ItemFamilyId, source.Code, source.Name,
           source.Description, COALESCE(source.SortOrder, 0),
           source.IsActive, source.IsDeleted, source.CreatedAt,
           source.CreatedByUserId, source.CreatedByUserName,
           source.UpdatedAt, source.UpdatedByUserId, source.UpdatedByUserName,
           source.DeletedAt, source.DeletedByUserId, source.DeletedByUserName
    FROM #Assignments assignment
    INNER JOIN dbo.ItemSubgroups source ON source.Id = assignment.ItemSubgroupId
    WHERE assignment.ItemFamilyId <> source.ItemFamilyId
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.ItemSubgroups target
          WHERE target.ItemFamilyId = assignment.ItemFamilyId AND target.Code = source.Code
      );

    UPDATE dbo.ItemSubgroups SET GlobalId = NEWID() WHERE GlobalId IS NULL;
    UPDATE dbo.ItemSubgroups SET SortOrder = 0 WHERE SortOrder IS NULL;

    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    THROW;
END CATCH;
GO

IF EXISTS (SELECT 1 FROM dbo.ItemSubgroups WHERE ItemFamilyId IS NULL)
    THROW 51205, 'ItemSubgroups.ItemFamilyId backfill is incomplete.', 1;
IF EXISTS (SELECT 1 FROM dbo.ItemSubgroups WHERE GlobalId IS NULL)
    THROW 51205, 'ItemSubgroups.GlobalId backfill is incomplete.', 1;
IF EXISTS (SELECT 1 FROM dbo.ItemSubgroups WHERE NULLIF(LTRIM(RTRIM(Code)), N'') IS NULL)
    THROW 51205, 'ItemSubgroups contains blank codes.', 1;
IF EXISTS (SELECT 1 FROM dbo.ItemSubgroups WHERE NULLIF(LTRIM(RTRIM(Name)), N'') IS NULL)
    THROW 51205, 'ItemSubgroups contains blank names.', 1;
IF EXISTS (SELECT 1 FROM dbo.ItemSubgroups WHERE SortOrder < 0)
    THROW 51205, 'ItemSubgroups contains negative SortOrder values.', 1;
IF EXISTS
(
    SELECT 1 FROM dbo.ItemSubgroups subgroup
    LEFT JOIN dbo.ItemFamilies family ON family.Id = subgroup.ItemFamilyId
    WHERE family.Id IS NULL
)
    THROW 51205, 'ItemSubgroups contains orphan ItemFamilyId values.', 1;
IF EXISTS
(
    SELECT ItemFamilyId, Code FROM dbo.ItemSubgroups
    GROUP BY ItemFamilyId, Code HAVING COUNT(1) > 1
)
    THROW 51205, 'ItemSubgroups contains duplicate codes inside a family, including tombstones.', 1;
GO

ALTER TABLE dbo.ItemSubgroups ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
ALTER TABLE dbo.ItemSubgroups ALTER COLUMN ItemFamilyId int NOT NULL;
ALTER TABLE dbo.ItemSubgroups ALTER COLUMN SortOrder int NOT NULL;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.default_constraints d
    INNER JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id
    WHERE d.parent_object_id=OBJECT_ID(N'dbo.ItemSubgroups') AND c.name=N'GlobalId'
)
    ALTER TABLE dbo.ItemSubgroups ADD CONSTRAINT DF_ItemSubgroups_GlobalId DEFAULT(NEWSEQUENTIALID()) FOR GlobalId;
IF NOT EXISTS
(
    SELECT 1 FROM sys.default_constraints d
    INNER JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id
    WHERE d.parent_object_id=OBJECT_ID(N'dbo.ItemSubgroups') AND c.name=N'SortOrder'
)
    ALTER TABLE dbo.ItemSubgroups ADD CONSTRAINT DF_ItemSubgroups_SortOrder DEFAULT(0) FOR SortOrder;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE parent_object_id=OBJECT_ID(N'dbo.ItemSubgroups') AND name=N'FK_ItemSubgroups_ItemFamilies')
    ALTER TABLE dbo.ItemSubgroups WITH CHECK ADD CONSTRAINT FK_ItemSubgroups_ItemFamilies FOREIGN KEY(ItemFamilyId) REFERENCES dbo.ItemFamilies(Id);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemSubgroups') AND name=N'CK_ItemSubgroups_Code_NotBlank')
    ALTER TABLE dbo.ItemSubgroups ADD CONSTRAINT CK_ItemSubgroups_Code_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Code)),N'') IS NOT NULL);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemSubgroups') AND name=N'CK_ItemSubgroups_Name_NotBlank')
    ALTER TABLE dbo.ItemSubgroups ADD CONSTRAINT CK_ItemSubgroups_Name_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Name)),N'') IS NOT NULL);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ItemSubgroups') AND name=N'CK_ItemSubgroups_SortOrder')
    ALTER TABLE dbo.ItemSubgroups ADD CONSTRAINT CK_ItemSubgroups_SortOrder CHECK(SortOrder >= 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemSubgroups') AND name=N'UX_ItemSubgroups_GlobalId')
    CREATE UNIQUE INDEX UX_ItemSubgroups_GlobalId ON dbo.ItemSubgroups(GlobalId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemSubgroups') AND name=N'UX_ItemSubgroups_Family_Code')
    CREATE UNIQUE INDEX UX_ItemSubgroups_Family_Code ON dbo.ItemSubgroups(ItemFamilyId, Code);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.ItemSubgroups') AND name=N'IX_ItemSubgroups_Family_Active_Order')
    CREATE INDEX IX_ItemSubgroups_Family_Active_Order
        ON dbo.ItemSubgroups(ItemFamilyId, IsActive, SortOrder, Name)
        INCLUDE(GlobalId, Code) WHERE IsDeleted=0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_SUBGROUPS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT subgroup.Id, subgroup.GlobalId, subgroup.ItemFamilyId,
           family.GlobalId ItemFamilyGlobalId, family.Code ItemFamilyCode, family.Name ItemFamilyName,
           subgroup.Code, subgroup.Name, subgroup.Description, subgroup.SortOrder,
           subgroup.IsActive, subgroup.CreatedByUserId, subgroup.CreatedByUserName, subgroup.CreatedAt,
           subgroup.UpdatedByUserId, subgroup.UpdatedByUserName, subgroup.UpdatedAt,
           subgroup.DeletedByUserId, subgroup.DeletedByUserName, subgroup.DeletedAt
    FROM dbo.ItemSubgroups subgroup
    INNER JOIN dbo.ItemFamilies family ON family.Id=subgroup.ItemFamilyId
    WHERE subgroup.IsDeleted=0
    ORDER BY family.Name, subgroup.SortOrder, subgroup.Name, subgroup.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_SUBGROUPS_LOOKUP @ItemFamilyId int=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT subgroup.Id, subgroup.GlobalId, subgroup.ItemFamilyId,
           family.GlobalId ItemFamilyGlobalId, family.Code ItemFamilyCode, family.Name ItemFamilyName,
           subgroup.Code, subgroup.Name, subgroup.SortOrder, CAST(subgroup.IsActive AS bit) IsActive
    FROM dbo.ItemSubgroups subgroup
    INNER JOIN dbo.ItemFamilies family ON family.Id=subgroup.ItemFamilyId
    WHERE subgroup.IsDeleted=0 AND subgroup.IsActive=1
      AND family.IsDeleted=0 AND family.IsActive=1
      AND (@ItemFamilyId IS NULL OR subgroup.ItemFamilyId=@ItemFamilyId)
    ORDER BY family.Name, subgroup.SortOrder, subgroup.Name, subgroup.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_SUBGROUPS_BUSCARPORID @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT subgroup.Id, subgroup.GlobalId, subgroup.ItemFamilyId,
           family.GlobalId ItemFamilyGlobalId, family.Code ItemFamilyCode, family.Name ItemFamilyName,
           subgroup.Code, subgroup.Name, subgroup.Description, subgroup.SortOrder,
           subgroup.IsActive, subgroup.CreatedByUserId, subgroup.CreatedByUserName, subgroup.CreatedAt,
           subgroup.UpdatedByUserId, subgroup.UpdatedByUserName, subgroup.UpdatedAt,
           subgroup.DeletedByUserId, subgroup.DeletedByUserName, subgroup.DeletedAt
    FROM dbo.ItemSubgroups subgroup
    INNER JOIN dbo.ItemFamilies family ON family.Id=subgroup.ItemFamilyId
    WHERE subgroup.Id=@Id AND subgroup.IsDeleted=0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_SUBGROUPSBUSCARPORCODIGO
    @ItemFamilyId int, @Code nvarchar(50), @ExcluirId int=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM dbo.ItemSubgroups
    WHERE ItemFamilyId=@ItemFamilyId AND Code=LTRIM(RTRIM(@Code))
      AND IsDeleted=0 AND (@ExcluirId IS NULL OR Id<>@ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_SUBGROUPS_HISTORIAL @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName,[Source],CreatedAt
    FROM dbo.AuditInventoryChanges
    WHERE EntityName=N'ItemSubgroups' AND RecordId=CONVERT(nvarchar(80),@Id)
    ORDER BY CreatedAt DESC,Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_SUBGROUPS_CREAR
    @GlobalId uniqueidentifier,@ItemFamilyId int,@Code nvarchar(50),@Name nvarchar(150),
    @Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,
    @CreatedByUserId int=NULL,@CreatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name));
    IF @GlobalId IS NULL OR @GlobalId='00000000-0000-0000-0000-000000000000' THROW 51205,'ItemSubgroup GlobalId is required.',1;
    IF NULLIF(@Code,N'') IS NULL THROW 51002,'El codigo es obligatorio.',1;
    IF NULLIF(@Name,N'') IS NULL THROW 51003,'El nombre es obligatorio.',1;
    IF @SortOrder<0 THROW 51205,'SortOrder cannot be negative.',1;
    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;
        IF NOT EXISTS(SELECT 1 FROM dbo.ItemFamilies WITH(UPDLOCK,HOLDLOCK) WHERE Id=@ItemFamilyId AND IsDeleted=0 AND IsActive=1)
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -2; RETURN; END;
        IF EXISTS(SELECT 1 FROM dbo.ItemSubgroups WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId OR (ItemFamilyId=@ItemFamilyId AND Code=@Code))
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
        INSERT dbo.ItemSubgroups(GlobalId,ItemFamilyId,Code,Name,Description,SortOrder,IsActive,CreatedByUserId,CreatedByUserName)
        VALUES(@GlobalId,@ItemFamilyId,@Code,@Name,@Description,@SortOrder,@IsActive,@CreatedByUserId,@CreatedByUserName);
        DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        SELECT N'ItemSubgroups',CONVERT(nvarchar(80),@Id),N'INSERT',FieldName,NULL,NewValue,@CreatedByUserId,@CreatedByUserName
        FROM(VALUES
            (N'ItemFamilyId',CONVERT(nvarchar(max),@ItemFamilyId)),(N'Code',@Code),(N'Name',@Name),
            (N'Description',@Description),(N'SortOrder',CONVERT(nvarchar(max),@SortOrder)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@IsActive))))v(FieldName,NewValue);
        IF @OwnTransaction=1 COMMIT; SELECT @Id;
    END TRY BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ITEM_SUBGROUPS_ACTUALIZAR
    @Id int,@ItemFamilyId int,@Code nvarchar(50),@Name nvarchar(150),
    @Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit=1,
    @UpdatedByUserId int=NULL,@UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name));
    IF NULLIF(@Code,N'') IS NULL THROW 51002,'El codigo es obligatorio.',1;
    IF NULLIF(@Name,N'') IS NULL THROW 51003,'El nombre es obligatorio.',1;
    IF @SortOrder<0 THROW 51205,'SortOrder cannot be negative.',1;
    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;
        DECLARE @OldFamilyId int,@OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldSortOrder int,@OldIsActive bit;
        SELECT @OldFamilyId=ItemFamilyId,@OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSortOrder=SortOrder,@OldIsActive=IsActive
        FROM dbo.ItemSubgroups WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
        IF @OldFamilyId IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;
        IF NOT EXISTS(SELECT 1 FROM dbo.ItemFamilies WITH(UPDLOCK,HOLDLOCK) WHERE Id=@ItemFamilyId AND IsDeleted=0 AND IsActive=1)
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -2; RETURN; END;
        IF EXISTS(SELECT 1 FROM dbo.ItemSubgroups WITH(UPDLOCK,HOLDLOCK) WHERE ItemFamilyId=@ItemFamilyId AND Code=@Code AND Id<>@Id)
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
        IF (@OldFamilyId<>@ItemFamilyId OR @OldCode<>@Code) AND EXISTS
        (
            SELECT 1 FROM dbo.Items item WITH(UPDLOCK,HOLDLOCK)
            INNER JOIN dbo.ItemMasterProfiles profile ON profile.ItemId=item.Id AND profile.IsDeleted=0 AND profile.IsActive=1
            WHERE item.IsDeleted=0 AND item.ItemFamilyId=@OldFamilyId
              AND JSON_VALUE(profile.MasterDataJson,N'$.general.subGroup')=@OldCode
        )
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -3; RETURN; END;
        UPDATE dbo.ItemSubgroups SET ItemFamilyId=@ItemFamilyId,Code=@Code,Name=@Name,Description=@Description,
            SortOrder=@SortOrder,IsActive=@IsActive,UpdatedByUserId=@UpdatedByUserId,
            UpdatedByUserName=@UpdatedByUserName,UpdatedAt=SYSUTCDATETIME()
        WHERE Id=@Id AND IsDeleted=0;
        INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        SELECT N'ItemSubgroups',CONVERT(nvarchar(80),@Id),N'UPDATE',FieldName,OldValue,NewValue,@UpdatedByUserId,@UpdatedByUserName
        FROM(VALUES
            (N'ItemFamilyId',CONVERT(nvarchar(max),@OldFamilyId),CONVERT(nvarchar(max),@ItemFamilyId)),
            (N'Code',@OldCode,@Code),(N'Name',@OldName,@Name),(N'Description',@OldDescription,@Description),
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_ITEM_SUBGROUPS_ELIMINAR
    @Id int,@DeletedByUserId int=NULL,@DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;
        DECLARE @FamilyId int,@Code nvarchar(50),@OldIsActive bit;
        SELECT @FamilyId=ItemFamilyId,@Code=Code,@OldIsActive=IsActive FROM dbo.ItemSubgroups WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
        IF @FamilyId IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;
        IF EXISTS
        (
            SELECT 1 FROM dbo.Items item WITH(UPDLOCK,HOLDLOCK)
            INNER JOIN dbo.ItemMasterProfiles profile ON profile.ItemId=item.Id AND profile.IsDeleted=0 AND profile.IsActive=1
            WHERE item.IsDeleted=0 AND item.ItemFamilyId=@FamilyId
              AND JSON_VALUE(profile.MasterDataJson,N'$.general.subGroup')=@Code
        )
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -4; RETURN; END;
        UPDATE dbo.ItemSubgroups SET IsActive=0,IsDeleted=1,DeletedAt=SYSUTCDATETIME(),
            DeletedByUserId=@DeletedByUserId,DeletedByUserName=@DeletedByUserName WHERE Id=@Id AND IsDeleted=0;
        INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        VALUES(N'ItemSubgroups',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),N'0',@DeletedByUserId,@DeletedByUserName),
              (N'ItemSubgroups',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);
        IF @OwnTransaction=1 COMMIT; SELECT 1;
    END TRY BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK; THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_SUBGROUP_SYNC_FULL @AfterId int=NULL,@BatchSize int=100
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP(@BatchSize) subgroup.Id,subgroup.GlobalId,family.GlobalId ItemFamilyGlobalId,
           family.Code ItemFamilyCode,subgroup.Code,subgroup.Name,subgroup.Description,subgroup.SortOrder,
           subgroup.IsActive,subgroup.IsDeleted,subgroup.CreatedAt,subgroup.UpdatedAt
    FROM dbo.ItemSubgroups subgroup
    INNER JOIN dbo.ItemFamilies family ON family.Id=subgroup.ItemFamilyId
    WHERE (@AfterId IS NULL OR subgroup.Id>@AfterId)
    ORDER BY subgroup.Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_SUBGROUP_SYNC_APPLY
    @GlobalId uniqueidentifier,@ItemFamilyGlobalId uniqueidentifier,@Code nvarchar(50),@Name nvarchar(150),
    @Description nvarchar(500)=NULL,@SortOrder int=0,@IsActive bit,@IsDeleted bit,
    @CreatedAt datetime2(0),@UpdatedAt datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name));
    DECLARE @FamilyId int=(SELECT Id FROM dbo.ItemFamilies WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@ItemFamilyGlobalId);
    IF @FamilyId IS NULL THROW 51205,'ItemFamily dependency was not found for ItemSubgroup sync.',1;
    DECLARE @Id int,@ConflictId int,@WasNew bit=0,@OldFamilyId int,@OldCode nvarchar(50),@OldName nvarchar(150),
            @OldDescription nvarchar(500),@OldSortOrder int,@OldIsActive bit,@OldIsDeleted bit;
    SELECT @Id=Id,@OldFamilyId=ItemFamilyId,@OldCode=Code,@OldName=Name,@OldDescription=Description,
           @OldSortOrder=SortOrder,@OldIsActive=IsActive,@OldIsDeleted=IsDeleted
    FROM dbo.ItemSubgroups WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
    SELECT @ConflictId=Id FROM dbo.ItemSubgroups WITH(UPDLOCK,HOLDLOCK)
    WHERE ItemFamilyId=@FamilyId AND Code=@Code AND (@Id IS NULL OR Id<>@Id);
    IF @ConflictId IS NOT NULL BEGIN SELECT -2 ResultCode,CONVERT(int,NULL) ItemSubgroupId; RETURN; END;
    IF @Id IS NULL
    BEGIN
        SET @WasNew=1;
        INSERT dbo.ItemSubgroups(GlobalId,ItemFamilyId,Code,Name,Description,SortOrder,IsActive,IsDeleted,
            CreatedAt,CreatedByUserName,DeletedAt,DeletedByUserName)
        VALUES(@GlobalId,@FamilyId,@Code,@Name,@Description,@SortOrder,@IsActive,@IsDeleted,@CreatedAt,N'MasterBranchSyncWorker',
            CASE WHEN @IsDeleted=1 THEN @UpdatedAt END,CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END);
        SET @Id=CONVERT(int,SCOPE_IDENTITY());
    END
    ELSE
        UPDATE dbo.ItemSubgroups SET ItemFamilyId=@FamilyId,Code=@Code,Name=@Name,Description=@Description,
            SortOrder=@SortOrder,IsActive=@IsActive,IsDeleted=@IsDeleted,UpdatedAt=@UpdatedAt,
            UpdatedByUserName=N'MasterBranchSyncWorker',DeletedAt=CASE WHEN @IsDeleted=1 THEN @UpdatedAt END,
            DeletedByUserName=CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END WHERE Id=@Id;
    INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
    SELECT N'ItemSubgroups',CONVERT(nvarchar(80),@Id),
           CASE WHEN @WasNew=1 THEN N'INSERT' WHEN @IsDeleted=1 AND ISNULL(@OldIsDeleted,0)=0 THEN N'DELETE' ELSE N'UPDATE' END,
           FieldName,CASE WHEN @WasNew=1 THEN NULL ELSE OldValue END,NewValue,N'MasterBranchSyncWorker',N'MasterBranchSyncWorker'
    FROM(VALUES
        (N'ItemFamilyId',CONVERT(nvarchar(max),@OldFamilyId),CONVERT(nvarchar(max),@FamilyId)),
        (N'Code',@OldCode,@Code),(N'Name',@OldName,@Name),(N'Description',@OldDescription,@Description),
        (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
        (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
        (N'IsDeleted',CONVERT(nvarchar(max),CONVERT(int,@OldIsDeleted)),CONVERT(nvarchar(max),CONVERT(int,@IsDeleted))))v(FieldName,OldValue,NewValue)
    WHERE @WasNew=1 OR ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
    SELECT 1 ResultCode,@Id ItemSubgroupId;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260813.205')
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260813.205',N'ItemSubgroups dependiente de ItemFamilies con CRUD, auditoria y sync por GlobalId');
GO
