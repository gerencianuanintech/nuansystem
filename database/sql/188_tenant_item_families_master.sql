/*
    Funcionaliza Familias de articulos como maestro independiente.

    Solo tenant. Prerrequisitos: 021, 106, 127, 186 y SchemaHistory.
    Conserva la identidad (ItemGroupId, Code) para registros no eliminados,
    agrega orden, auditoria detallada y mantiene LocalOutbox/sync compatibles.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.ItemFamilies', N'U') IS NULL THROW 51188, 'ItemFamilies is required before migration 188.', 1;
IF OBJECT_ID(N'dbo.ItemGroups', N'U') IS NULL THROW 51188, 'ItemGroups is required before migration 188.', 1;
IF OBJECT_ID(N'dbo.Items', N'U') IS NULL THROW 51188, 'Items is required before migration 188.', 1;
IF OBJECT_ID(N'dbo.AuditInventoryChanges', N'U') IS NULL THROW 51188, 'AuditInventoryChanges is required before migration 188.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL THROW 51188, 'SchemaHistory is required before migration 188.', 1;
IF COL_LENGTH(N'dbo.ItemFamilies', N'GlobalId') IS NULL THROW 51188, 'ItemFamilies.GlobalId from migration 127 is required.', 1;
IF COL_LENGTH(N'dbo.ItemGroups', N'GlobalId') IS NULL THROW 51188, 'ItemGroups.GlobalId is required.', 1;
GO

IF COL_LENGTH(N'dbo.ItemFamilies', N'SortOrder') IS NULL
    ALTER TABLE dbo.ItemFamilies ADD SortOrder int NULL;
GO

UPDATE dbo.ItemFamilies SET SortOrder = 0 WHERE SortOrder IS NULL;
GO

IF EXISTS (SELECT 1 FROM dbo.ItemFamilies WHERE NULLIF(LTRIM(RTRIM(Code)), N'') IS NULL)
    THROW 51188, 'ItemFamilies contains blank codes; repair the data before migration 188.', 1;
IF EXISTS (SELECT 1 FROM dbo.ItemFamilies WHERE NULLIF(LTRIM(RTRIM(Name)), N'') IS NULL)
    THROW 51188, 'ItemFamilies contains blank names; repair the data before migration 188.', 1;
IF EXISTS (SELECT 1 FROM dbo.ItemFamilies WHERE SortOrder < 0)
    THROW 51188, 'ItemFamilies contains negative SortOrder values; repair the data before migration 188.', 1;
IF EXISTS
(
    SELECT 1
    FROM dbo.ItemFamilies family
    LEFT JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = family.ItemGroupId
    WHERE itemGroup.Id IS NULL
)
    THROW 51188, 'ItemFamilies contains orphan ItemGroupId values; repair the data before migration 188.', 1;
GO

IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.ItemFamilies')
      AND name = N'SortOrder' AND is_nullable = 1
)
    ALTER TABLE dbo.ItemFamilies ALTER COLUMN SortOrder int NOT NULL;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints d
    INNER JOIN sys.columns c
        ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
    WHERE d.parent_object_id = OBJECT_ID(N'dbo.ItemFamilies') AND c.name = N'SortOrder'
)
    ALTER TABLE dbo.ItemFamilies ADD CONSTRAINT DF_ItemFamilies_SortOrder DEFAULT (0) FOR SortOrder;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemFamilies') AND name = N'CK_ItemFamilies_Code_NotBlank')
    ALTER TABLE dbo.ItemFamilies ADD CONSTRAINT CK_ItemFamilies_Code_NotBlank CHECK (NULLIF(LTRIM(RTRIM(Code)), N'') IS NOT NULL);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemFamilies') AND name = N'CK_ItemFamilies_Name_NotBlank')
    ALTER TABLE dbo.ItemFamilies ADD CONSTRAINT CK_ItemFamilies_Name_NotBlank CHECK (NULLIF(LTRIM(RTRIM(Name)), N'') IS NOT NULL);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemFamilies') AND name = N'CK_ItemFamilies_SortOrder')
    ALTER TABLE dbo.ItemFamilies ADD CONSTRAINT CK_ItemFamilies_SortOrder CHECK (SortOrder >= 0);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.ItemFamilies')
      AND name = N'IX_ItemFamilies_Group_Active_SortOrder_Name'
)
    CREATE INDEX IX_ItemFamilies_Group_Active_SortOrder_Name
        ON dbo.ItemFamilies(ItemGroupId, IsActive, SortOrder, Name)
        INCLUDE(Code, GlobalId) WHERE IsDeleted = 0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT family.Id, family.GlobalId, family.ItemGroupId,
           itemGroup.GlobalId AS ItemGroupGlobalId,
           itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
           family.Code, family.Name, family.Description, family.SortOrder,
           family.IsActive, family.SapFamilyCode, family.SapCode,
           family.ExternalSystem, family.ExternalCode,
           family.CreatedByUserId, family.CreatedByUserName, family.CreatedAt,
           family.UpdatedByUserId, family.UpdatedByUserName, family.UpdatedAt,
           family.DeletedByUserId, family.DeletedByUserName, family.DeletedAt
    FROM dbo.ItemFamilies family
    INNER JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = family.ItemGroupId
    WHERE family.IsDeleted = 0
    ORDER BY itemGroup.Name, family.SortOrder, family.Name, family.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT family.Id, family.GlobalId, family.ItemGroupId,
           itemGroup.GlobalId AS ItemGroupGlobalId,
           itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
           family.Code, family.Name, family.Description, family.SortOrder,
           family.IsActive, family.SapFamilyCode, family.SapCode,
           family.ExternalSystem, family.ExternalCode,
           family.CreatedByUserId, family.CreatedByUserName, family.CreatedAt,
           family.UpdatedByUserId, family.UpdatedByUserName, family.UpdatedAt,
           family.DeletedByUserId, family.DeletedByUserName, family.DeletedAt
    FROM dbo.ItemFamilies family
    INNER JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = family.ItemGroupId
    WHERE family.Id = @Id AND family.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIES_BUSCARPORGRUPO
    @ItemGroupId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT family.Id, family.GlobalId, family.ItemGroupId,
           itemGroup.GlobalId AS ItemGroupGlobalId,
           itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
           family.Code, family.Name, family.Description, family.SortOrder,
           family.IsActive, family.SapFamilyCode, family.SapCode,
           family.ExternalSystem, family.ExternalCode,
           family.CreatedByUserId, family.CreatedByUserName, family.CreatedAt,
           family.UpdatedByUserId, family.UpdatedByUserName, family.UpdatedAt,
           family.DeletedByUserId, family.DeletedByUserName, family.DeletedAt
    FROM dbo.ItemFamilies family
    INNER JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = family.ItemGroupId
    WHERE family.ItemGroupId = @ItemGroupId
      AND family.IsDeleted = 0 AND family.IsActive = 1
    ORDER BY family.SortOrder, family.Name, family.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIES_LOOKUP
    @ItemGroupId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT family.Id, family.GlobalId, family.ItemGroupId,
           itemGroup.GlobalId AS ItemGroupGlobalId,
           itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
           family.Code, family.Name, family.SortOrder,
           CAST(family.IsActive AS bit) AS IsActive
    FROM dbo.ItemFamilies family
    INNER JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = family.ItemGroupId
    WHERE family.IsDeleted = 0 AND family.IsActive = 1
      AND itemGroup.IsDeleted = 0 AND itemGroup.IsActive = 1
      AND (@ItemGroupId IS NULL OR family.ItemGroupId = @ItemGroupId)
    ORDER BY itemGroup.Name, family.SortOrder, family.Name, family.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIESBUSCARPORCODIGO
    @ItemGroupId int,
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.ItemFamilies
    WHERE ItemGroupId = @ItemGroupId
      AND Code = LTRIM(RTRIM(@Code))
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIES_HISTORIAL
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, EntityName, RecordId, [Action], FieldName,
           OldValue, NewValue, UserId, UserName, [Source], CreatedAt
    FROM dbo.AuditInventoryChanges
    WHERE EntityName = N'ItemFamilies'
      AND RecordId = CONVERT(nvarchar(80), @Id)
    ORDER BY CreatedAt DESC, Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_FAMILIES_CREAR
    @GlobalId uniqueidentifier,
    @ItemGroupId int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @SortOrder int = 0,
    @IsActive bit,
    @SapFamilyCode nvarchar(100) = NULL,
    @SapCode nvarchar(50) = NULL,
    @ExternalSystem nvarchar(50) = NULL,
    @ExternalCode nvarchar(100) = NULL,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Code = LTRIM(RTRIM(@Code));
    SET @Name = LTRIM(RTRIM(@Name));
    IF @GlobalId IS NULL OR @GlobalId = '00000000-0000-0000-0000-000000000000' THROW 51188, 'ItemFamily GlobalId is required.', 1;
    IF NULLIF(@Code, N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(@Name, N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @SortOrder < 0 THROW 51188, 'ItemFamily SortOrder cannot be negative.', 1;

    BEGIN TRY
        DECLARE @OwnTransaction bit = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END;
        IF @OwnTransaction = 1 BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.ItemGroups WITH (UPDLOCK, HOLDLOCK)
            WHERE Id = @ItemGroupId AND IsDeleted = 0 AND IsActive = 1
        )
        BEGIN
            IF @OwnTransaction = 1 COMMIT;
            SELECT -2;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.ItemFamilies WITH (UPDLOCK, HOLDLOCK)
            WHERE (ItemGroupId = @ItemGroupId AND Code = @Code AND IsDeleted = 0)
               OR GlobalId = @GlobalId
        )
        BEGIN
            IF @OwnTransaction = 1 COMMIT;
            SELECT -1;
            RETURN;
        END;

        INSERT dbo.ItemFamilies
        (
            GlobalId, ItemGroupId, Code, Name, Description, SortOrder, IsActive,
            SapFamilyCode, SapCode, ExternalSystem, ExternalCode,
            CreatedByUserId, CreatedByUserName
        )
        VALUES
        (
            @GlobalId, @ItemGroupId, @Code, @Name, @Description, @SortOrder, @IsActive,
            @SapFamilyCode, @SapCode, @ExternalSystem, @ExternalCode,
            @CreatedByUserId, @CreatedByUserName
        );

        DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());

        INSERT dbo.AuditInventoryChanges
        (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        SELECT N'ItemFamilies', CONVERT(nvarchar(80), @Id), N'INSERT',
               FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
        FROM (VALUES
            (N'ItemGroupId', CONVERT(nvarchar(max), @ItemGroupId)),
            (N'Code', CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @Description)),
            (N'SortOrder', CONVERT(nvarchar(max), @SortOrder)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
            (N'SapFamilyCode', CONVERT(nvarchar(max), @SapFamilyCode)),
            (N'SapCode', CONVERT(nvarchar(max), @SapCode)),
            (N'ExternalSystem', CONVERT(nvarchar(max), @ExternalSystem)),
            (N'ExternalCode', CONVERT(nvarchar(max), @ExternalCode))
        ) valuesToAudit(FieldName, NewValue);

        IF @OwnTransaction = 1 COMMIT;
        SELECT @Id;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SELECT -1;
            RETURN;
        END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ITEM_FAMILIES_ACTUALIZAR
    @Id int,
    @ItemGroupId int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @SortOrder int = 0,
    @IsActive bit,
    @SapFamilyCode nvarchar(100) = NULL,
    @SapCode nvarchar(50) = NULL,
    @ExternalSystem nvarchar(50) = NULL,
    @ExternalCode nvarchar(100) = NULL,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Code = LTRIM(RTRIM(@Code));
    SET @Name = LTRIM(RTRIM(@Name));
    IF NULLIF(@Code, N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(@Name, N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @SortOrder < 0 THROW 51188, 'ItemFamily SortOrder cannot be negative.', 1;

    BEGIN TRY
        DECLARE @OwnTransaction bit = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END;
        IF @OwnTransaction = 1 BEGIN TRANSACTION;

        DECLARE @OldItemGroupId int, @OldCode nvarchar(50), @OldName nvarchar(150),
                @OldDescription nvarchar(500), @OldSortOrder int, @OldIsActive bit,
                @OldSapFamilyCode nvarchar(100), @OldSapCode nvarchar(50),
                @OldExternalSystem nvarchar(50), @OldExternalCode nvarchar(100);

        SELECT @OldItemGroupId = ItemGroupId, @OldCode = Code, @OldName = Name,
               @OldDescription = Description, @OldSortOrder = SortOrder, @OldIsActive = IsActive,
               @OldSapFamilyCode = SapFamilyCode, @OldSapCode = SapCode,
               @OldExternalSystem = ExternalSystem, @OldExternalCode = ExternalCode
        FROM dbo.ItemFamilies WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @Id AND IsDeleted = 0;

        IF @OldItemGroupId IS NULL
        BEGIN
            IF @OwnTransaction = 1 COMMIT;
            SELECT 0;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.ItemGroups WITH (UPDLOCK, HOLDLOCK)
            WHERE Id = @ItemGroupId AND IsDeleted = 0 AND IsActive = 1
        )
        BEGIN
            IF @OwnTransaction = 1 COMMIT;
            SELECT -2;
            RETURN;
        END;

        IF @OldItemGroupId <> @ItemGroupId
           AND EXISTS (SELECT 1 FROM dbo.Items WITH (UPDLOCK, HOLDLOCK) WHERE ItemFamilyId = @Id AND IsDeleted = 0)
        BEGIN
            IF @OwnTransaction = 1 COMMIT;
            SELECT -3;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.ItemFamilies WITH (UPDLOCK, HOLDLOCK)
            WHERE ItemGroupId = @ItemGroupId AND Code = @Code
              AND IsDeleted = 0 AND Id <> @Id
        )
        BEGIN
            IF @OwnTransaction = 1 COMMIT;
            SELECT -1;
            RETURN;
        END;

        UPDATE dbo.ItemFamilies
        SET ItemGroupId = @ItemGroupId,
            Code = @Code,
            Name = @Name,
            Description = @Description,
            SortOrder = @SortOrder,
            IsActive = @IsActive,
            SapFamilyCode = @SapFamilyCode,
            SapCode = @SapCode,
            ExternalSystem = @ExternalSystem,
            ExternalCode = @ExternalCode,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @Id AND IsDeleted = 0;

        INSERT dbo.AuditInventoryChanges
        (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        SELECT N'ItemFamilies', CONVERT(nvarchar(80), @Id), N'UPDATE',
               FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
        FROM (VALUES
            (N'ItemGroupId', CONVERT(nvarchar(max), @OldItemGroupId), CONVERT(nvarchar(max), @ItemGroupId)),
            (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
            (N'SortOrder', CONVERT(nvarchar(max), @OldSortOrder), CONVERT(nvarchar(max), @SortOrder)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
            (N'SapFamilyCode', CONVERT(nvarchar(max), @OldSapFamilyCode), CONVERT(nvarchar(max), @SapFamilyCode)),
            (N'SapCode', CONVERT(nvarchar(max), @OldSapCode), CONVERT(nvarchar(max), @SapCode)),
            (N'ExternalSystem', CONVERT(nvarchar(max), @OldExternalSystem), CONVERT(nvarchar(max), @ExternalSystem)),
            (N'ExternalCode', CONVERT(nvarchar(max), @OldExternalCode), CONVERT(nvarchar(max), @ExternalCode))
        ) valuesToAudit(FieldName, OldValue, NewValue)
        WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

        IF @OwnTransaction = 1 COMMIT;
        SELECT 1;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SELECT -1;
            RETURN;
        END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_ITEM_FAMILIES_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @OwnTransaction bit = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END;
        IF @OwnTransaction = 1 BEGIN TRANSACTION;

        DECLARE @OldIsActive bit;
        SELECT @OldIsActive = IsActive
        FROM dbo.ItemFamilies WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @Id AND IsDeleted = 0;

        IF @OldIsActive IS NULL
        BEGIN
            IF @OwnTransaction = 1 COMMIT;
            SELECT 0;
            RETURN;
        END;

        IF EXISTS (SELECT 1 FROM dbo.Items WITH (UPDLOCK, HOLDLOCK) WHERE ItemFamilyId = @Id AND IsDeleted = 0)
        BEGIN
            IF @OwnTransaction = 1 COMMIT;
            SELECT -4;
            RETURN;
        END;

        UPDATE dbo.ItemFamilies
        SET IsActive = 0,
            IsDeleted = 1,
            DeletedByUserId = @DeletedByUserId,
            DeletedByUserName = @DeletedByUserName,
            DeletedAt = SYSUTCDATETIME()
        WHERE Id = @Id AND IsDeleted = 0;

        INSERT dbo.AuditInventoryChanges
        (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES
        (N'ItemFamilies', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), N'0', @DeletedByUserId, @DeletedByUserName),
        (N'ItemFamilies', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);

        IF @OwnTransaction = 1 COMMIT;
        SELECT 1;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_FAMILY_SYNC_APPLY
    @GlobalId uniqueidentifier,
    @ItemGroupGlobalId uniqueidentifier,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @SortOrder int = 0,
    @IsActive bit,
    @IsDeleted bit,
    @SapFamilyCode nvarchar(100) = NULL,
    @SapCode nvarchar(50) = NULL,
    @ExternalSystem nvarchar(50) = NULL,
    @ExternalCode nvarchar(100) = NULL,
    @CreatedAt datetime2(0),
    @UpdatedAt datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Code = LTRIM(RTRIM(@Code));
    SET @Name = LTRIM(RTRIM(@Name));
    IF @GlobalId IS NULL OR @GlobalId = '00000000-0000-0000-0000-000000000000' THROW 51188, 'ItemFamily GlobalId is required for sync.', 1;
    IF @ItemGroupGlobalId IS NULL OR @ItemGroupGlobalId = '00000000-0000-0000-0000-000000000000' THROW 51188, 'ItemGroupGlobalId is required for ItemFamily sync.', 1;
    IF NULLIF(@Code, N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(@Name, N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @SortOrder < 0 THROW 51188, 'ItemFamily SortOrder cannot be negative.', 1;

    DECLARE @ItemGroupId int, @ItemFamilyId int, @ConflictingItemFamilyId int, @WasNew bit = 0;
    DECLARE @OldItemGroupId int, @OldCode nvarchar(50), @OldName nvarchar(150),
            @OldDescription nvarchar(500), @OldSortOrder int, @OldIsActive bit, @OldIsDeleted bit,
            @OldSapFamilyCode nvarchar(100), @OldSapCode nvarchar(50),
            @OldExternalSystem nvarchar(50), @OldExternalCode nvarchar(100);

    SELECT @ItemGroupId = Id
    FROM dbo.ItemGroups WITH (UPDLOCK, HOLDLOCK)
    WHERE GlobalId = @ItemGroupGlobalId AND IsDeleted = 0;

    IF @ItemGroupId IS NULL
        THROW 51188, 'ItemGroup dependency was not found for ItemFamily sync.', 1;

    SELECT @ItemFamilyId = Id,
           @OldItemGroupId = ItemGroupId, @OldCode = Code, @OldName = Name,
           @OldDescription = Description, @OldSortOrder = SortOrder,
           @OldIsActive = IsActive, @OldIsDeleted = IsDeleted,
           @OldSapFamilyCode = SapFamilyCode, @OldSapCode = SapCode,
           @OldExternalSystem = ExternalSystem, @OldExternalCode = ExternalCode
    FROM dbo.ItemFamilies WITH (UPDLOCK, HOLDLOCK)
    WHERE GlobalId = @GlobalId;

    SELECT @ConflictingItemFamilyId = Id
    FROM dbo.ItemFamilies WITH (UPDLOCK, HOLDLOCK)
    WHERE ItemGroupId = @ItemGroupId AND Code = @Code AND IsDeleted = 0
      AND (@ItemFamilyId IS NULL OR Id <> @ItemFamilyId);

    IF @ConflictingItemFamilyId IS NOT NULL
    BEGIN
        SELECT CONVERT(int, -2) AS ResultCode, CONVERT(int, NULL) AS ItemFamilyId;
        RETURN;
    END;

    IF @ItemFamilyId IS NULL
    BEGIN
        SET @WasNew = 1;
        INSERT dbo.ItemFamilies
        (
            GlobalId, ItemGroupId, Code, Name, Description, SortOrder,
            IsActive, IsDeleted, SapFamilyCode, SapCode, ExternalSystem, ExternalCode,
            CreatedAt, CreatedByUserName, DeletedAt, DeletedByUserName
        )
        VALUES
        (
            @GlobalId, @ItemGroupId, @Code, @Name, @Description, @SortOrder,
            @IsActive, @IsDeleted, @SapFamilyCode, @SapCode, @ExternalSystem, @ExternalCode,
            @CreatedAt, N'MasterBranchSyncWorker',
            CASE WHEN @IsDeleted = 1 THEN @UpdatedAt END,
            CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' END
        );
        SET @ItemFamilyId = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.ItemFamilies
        SET ItemGroupId = @ItemGroupId,
            Code = @Code,
            Name = @Name,
            Description = @Description,
            SortOrder = @SortOrder,
            IsActive = @IsActive,
            IsDeleted = @IsDeleted,
            SapFamilyCode = @SapFamilyCode,
            SapCode = @SapCode,
            ExternalSystem = @ExternalSystem,
            ExternalCode = @ExternalCode,
            UpdatedAt = @UpdatedAt,
            UpdatedByUserName = N'MasterBranchSyncWorker',
            DeletedAt = CASE WHEN @IsDeleted = 1 THEN @UpdatedAt END,
            DeletedByUserName = CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' END
        WHERE Id = @ItemFamilyId;
    END;

    INSERT dbo.AuditInventoryChanges
    (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserName, [Source])
    SELECT N'ItemFamilies', CONVERT(nvarchar(80), @ItemFamilyId),
           CASE WHEN @WasNew = 1 THEN N'INSERT'
                WHEN @IsDeleted = 1 AND ISNULL(@OldIsDeleted, 0) = 0 THEN N'DELETE'
                ELSE N'UPDATE' END,
           FieldName,
           CASE WHEN @WasNew = 1 THEN NULL ELSE OldValue END,
           NewValue, N'MasterBranchSyncWorker', N'MasterBranchSyncWorker'
    FROM (VALUES
        (N'ItemGroupId', CONVERT(nvarchar(max), @OldItemGroupId), CONVERT(nvarchar(max), @ItemGroupId)),
        (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
        (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
        (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
        (N'SortOrder', CONVERT(nvarchar(max), @OldSortOrder), CONVERT(nvarchar(max), @SortOrder)),
        (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
        (N'IsDeleted', CONVERT(nvarchar(max), CONVERT(int, @OldIsDeleted)), CONVERT(nvarchar(max), CONVERT(int, @IsDeleted))),
        (N'SapFamilyCode', CONVERT(nvarchar(max), @OldSapFamilyCode), CONVERT(nvarchar(max), @SapFamilyCode)),
        (N'SapCode', CONVERT(nvarchar(max), @OldSapCode), CONVERT(nvarchar(max), @SapCode)),
        (N'ExternalSystem', CONVERT(nvarchar(max), @OldExternalSystem), CONVERT(nvarchar(max), @ExternalSystem)),
        (N'ExternalCode', CONVERT(nvarchar(max), @OldExternalCode), CONVERT(nvarchar(max), @ExternalCode))
    ) valuesToAudit(FieldName, OldValue, NewValue)
    WHERE @WasNew = 1 OR ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    SELECT CONVERT(int, 1) AS ResultCode, @ItemFamilyId AS ItemFamilyId;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260812.188')
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES (N'20260812.188', N'ItemFamilies con orden, CRUD endurecido, auditoria e integracion Matriz-Sucursal compatible');
GO
