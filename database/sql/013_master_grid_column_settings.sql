IF OBJECT_ID(N'dbo.SecurityGridColumnSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityGridColumnSettings
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityGridColumnSettings PRIMARY KEY,
        FormKey nvarchar(120) NOT NULL,
        GridName nvarchar(120) NOT NULL,
        ColumnFieldName nvarchar(160) NOT NULL,
        DefaultCaption nvarchar(160) NOT NULL,
        CustomCaption nvarchar(160) NULL,
        IsVisible bit NOT NULL CONSTRAINT DF_SecurityGridColumnSettings_IsVisible DEFAULT 1,
        VisibleIndex int NOT NULL CONSTRAINT DF_SecurityGridColumnSettings_VisibleIndex DEFAULT 0,
        Width int NOT NULL CONSTRAINT DF_SecurityGridColumnSettings_Width DEFAULT 100,
        UserId int NULL,
        RoleId int NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityGridColumnSettings_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityGridColumnSettings_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityGridColumnSettings_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_SecurityGridColumnSettings_Role FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityGridColumnSettings_User' AND object_id = OBJECT_ID(N'dbo.SecurityGridColumnSettings'))
BEGIN
    CREATE UNIQUE INDEX UX_SecurityGridColumnSettings_User
    ON dbo.SecurityGridColumnSettings (FormKey, GridName, ColumnFieldName, UserId)
    WHERE UserId IS NOT NULL AND IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.CUSTOMIZE_COLUMNS')
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'ACTION.CUSTOMIZE_COLUMNS',
        N'Columnas',
        N'Personalizar visibilidad, orden y nombre de columnas del listado',
        N'Inicio',
        N'Acciones',
        N'customize-columns',
        N'Operaciones/columnas_32.svg',
        N'Operaciones/columnas_16.svg',
        39,
        1,
        N'Sistema',
        SYSUTCDATETIME()
    );
END;
GO

UPDATE dbo.SecurityOperations
SET Name = N'Columnas',
    Description = N'Personalizar visibilidad, orden y nombre de columnas del listado',
    RibbonPageName = N'Inicio',
    RibbonGroupName = N'Acciones',
    ActionKey = N'customize-columns',
    DisplayOrder = 39,
    IsActive = 1
WHERE Code = N'ACTION.CUSTOMIZE_COLUMNS';
GO

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');
DECLARE @CustomizeColumnsOperationId int = (SELECT TOP (1) Id FROM dbo.SecurityOperations WHERE Code = N'ACTION.CUSTOMIZE_COLUMNS');

IF @AdminRoleId IS NOT NULL AND @CustomizeColumnsOperationId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, form.Id, @CustomizeColumnsOperationId, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityForms form
    WHERE form.IsDeleted = 0
      AND form.IsActive = 1
      AND form.FormKey IN
      (
          N'configuration-companies',
          N'configuration-settings',
          N'users',
          N'security-roles',
          N'security-operations',
          N'security-menus',
          N'security-forms'
      )
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = form.Id
            AND existing.OperationId = @CustomizeColumnsOperationId
      );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SEGURIDADCOLUMNASLISTADOUSUARIO
    @UserId int,
    @FormKey nvarchar(120),
    @GridName nvarchar(120)
AS
BEGIN
    SELECT
        ColumnFieldName AS FieldName,
        DefaultCaption,
        ISNULL(CustomCaption, DefaultCaption) AS Caption,
        IsVisible,
        VisibleIndex,
        Width
    FROM dbo.SecurityGridColumnSettings
    WHERE UserId = @UserId
      AND FormKey = @FormKey
      AND GridName = @GridName
      AND IsDeleted = 0
    ORDER BY VisibleIndex, ColumnFieldName;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SEGURIDADCOLUMNASLISTADOGUARDAR
    @UserId int,
    @FormKey nvarchar(120),
    @GridName nvarchar(120),
    @ColumnsJson nvarchar(max),
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @Columns table
    (
        FieldName nvarchar(160) NOT NULL,
        DefaultCaption nvarchar(160) NOT NULL,
        Caption nvarchar(160) NOT NULL,
        IsVisible bit NOT NULL,
        VisibleIndex int NOT NULL,
        Width int NOT NULL
    );

    INSERT INTO @Columns (FieldName, DefaultCaption, Caption, IsVisible, VisibleIndex, Width)
    SELECT
        FieldName,
        DefaultCaption,
        Caption,
        IsVisible,
        VisibleIndex,
        Width
    FROM OPENJSON(@ColumnsJson)
    WITH
    (
        FieldName nvarchar(160) '$.fieldName',
        DefaultCaption nvarchar(160) '$.defaultCaption',
        Caption nvarchar(160) '$.caption',
        IsVisible bit '$.isVisible',
        VisibleIndex int '$.visibleIndex',
        Width int '$.width'
    )
    WHERE FieldName IS NOT NULL;

    UPDATE dbo.SecurityGridColumnSettings
    SET IsDeleted = 1,
        DeletedByUserId = @UpdatedByUserId,
        DeletedByUserName = @UpdatedByUserName,
        DeletedAt = SYSUTCDATETIME()
    WHERE UserId = @UserId
      AND FormKey = @FormKey
      AND GridName = @GridName
      AND IsDeleted = 0
      AND ColumnFieldName NOT IN (SELECT FieldName FROM @Columns);

    MERGE dbo.SecurityGridColumnSettings AS target
    USING @Columns AS source
        ON target.UserId = @UserId
       AND target.FormKey = @FormKey
       AND target.GridName = @GridName
       AND target.ColumnFieldName = source.FieldName
    WHEN MATCHED THEN
        UPDATE SET
            DefaultCaption = source.DefaultCaption,
            CustomCaption = NULLIF(source.Caption, source.DefaultCaption),
            IsVisible = source.IsVisible,
            VisibleIndex = source.VisibleIndex,
            Width = source.Width,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT
        (
            FormKey, GridName, ColumnFieldName, DefaultCaption, CustomCaption,
            IsVisible, VisibleIndex, Width, UserId,
            CreatedByUserId, CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @FormKey, @GridName, source.FieldName, source.DefaultCaption, NULLIF(source.Caption, source.DefaultCaption),
            source.IsVisible, source.VisibleIndex, source.Width, @UserId,
            @UpdatedByUserId, @UpdatedByUserName, SYSUTCDATETIME()
        );

    COMMIT TRANSACTION;

    SELECT CAST(1 AS bit);
END;
GO
