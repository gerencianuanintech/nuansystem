/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    Agrega persistencia versionada para el detalle funcional del Maestro de Items.
    Mantiene Application libre de SQL y evita duplicar las tablas base Items,
    ItemBarcodes e ItemWarehouses existentes.
*/

IF OBJECT_ID(N'dbo.ItemMasterProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemMasterProfiles
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemMasterProfiles PRIMARY KEY,
        ItemId int NOT NULL,
        MasterDataJson nvarchar(max) NOT NULL,
        Version int NOT NULL CONSTRAINT DF_ItemMasterProfiles_Version DEFAULT 1,
        IsActive bit NOT NULL CONSTRAINT DF_ItemMasterProfiles_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ItemMasterProfiles_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_ItemMasterProfiles_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_ItemMasterProfiles_Items FOREIGN KEY (ItemId) REFERENCES dbo.Items(Id),
        CONSTRAINT CK_ItemMasterProfiles_MasterDataJson_IsJson CHECK (ISJSON(MasterDataJson) = 1)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ItemMasterProfiles_Item_Active' AND object_id = OBJECT_ID(N'dbo.ItemMasterProfiles'))
BEGIN
    CREATE UNIQUE INDEX UX_ItemMasterProfiles_Item_Active
        ON dbo.ItemMasterProfiles (ItemId)
        WHERE IsDeleted = 0 AND IsActive = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMMASTERDATA_BUSCARPORITEMID
    @ItemId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        MasterDataJson
    FROM dbo.ItemMasterProfiles
    WHERE ItemId = @ItemId
      AND IsDeleted = 0
      AND IsActive = 1
    ORDER BY Version DESC, Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ITEMMASTERDATA_GUARDAR
    @ItemId int,
    @MasterDataJson nvarchar(max),
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Items WHERE Id = @ItemId AND IsDeleted = 0)
    BEGIN
        SELECT 0;
        RETURN;
    END;

    IF @MasterDataJson IS NULL OR ISJSON(@MasterDataJson) <> 1
    BEGIN
        SELECT 0;
        RETURN;
    END;

    BEGIN TRANSACTION;

    DECLARE @ProfileId bigint;
    DECLARE @NextVersion int = 1;

    SELECT TOP (1)
        @ProfileId = Id,
        @NextVersion = Version + 1
    FROM dbo.ItemMasterProfiles
    WHERE ItemId = @ItemId
      AND IsDeleted = 0
      AND IsActive = 1
    ORDER BY Version DESC, Id DESC;

    IF @ProfileId IS NULL
    BEGIN
        INSERT INTO dbo.ItemMasterProfiles
        (
            ItemId,
            MasterDataJson,
            Version,
            IsActive,
            CreatedByUserId,
            CreatedByUserName,
            CreatedAt
        )
        VALUES
        (
            @ItemId,
            @MasterDataJson,
            1,
            1,
            @UpdatedByUserId,
            @UpdatedByUserName,
            SYSUTCDATETIME()
        );
    END
    ELSE
    BEGIN
        UPDATE dbo.ItemMasterProfiles
        SET
            MasterDataJson = @MasterDataJson,
            Version = @NextVersion,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @ProfileId;
    END;

    INSERT INTO dbo.AuditInventoryChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    VALUES (N'ItemMasterProfiles', CONVERT(nvarchar(80), @ItemId), N'UPSERT', N'MasterDataJson', NULL, N'Actualizado', @UpdatedByUserId, @UpdatedByUserName);

    COMMIT TRANSACTION;

    SELECT 1;
END;
GO
