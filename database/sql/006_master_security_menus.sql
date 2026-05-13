IF OBJECT_ID(N'dbo.SecurityMenus', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityMenus
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityMenus PRIMARY KEY,
        ParentId int NULL,
        Code nvarchar(80) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Description nvarchar(300) NULL,
        MenuType tinyint NOT NULL,
        FormKey nvarchar(120) NULL,
        IconLarge nvarchar(200) NULL,
        IconSmall nvarchar(200) NULL,
        DisplayOrder int NOT NULL CONSTRAINT DF_SecurityMenus_DisplayOrder DEFAULT 0,
        IsVisible bit NOT NULL CONSTRAINT DF_SecurityMenus_IsVisible DEFAULT 1,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityMenus_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityMenus_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityMenus_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityMenus_Parent FOREIGN KEY (ParentId) REFERENCES dbo.SecurityMenus(Id),
        CONSTRAINT UQ_SecurityMenus_Code UNIQUE (Code),
        CONSTRAINT CK_SecurityMenus_MenuType CHECK (MenuType IN (1, 2, 3))
    );
END;
GO

IF OBJECT_ID(N'dbo.AuditSecurityChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSecurityChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSecurityChanges PRIMARY KEY,
        EntityName nvarchar(120) NOT NULL,
        RecordId nvarchar(80) NOT NULL,
        [Action] nvarchar(30) NOT NULL,
        FieldName nvarchar(120) NOT NULL,
        OldValue nvarchar(max) NULL,
        NewValue nvarchar(max) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        Source nvarchar(60) NOT NULL CONSTRAINT DF_AuditSecurityChanges_Source DEFAULT N'API',
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditSecurityChanges_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AuditSecurityChanges_Entity_Record_CreatedAt ON dbo.AuditSecurityChanges (EntityName, RecordId, CreatedAt DESC);
    CREATE INDEX IX_AuditSecurityChanges_User_CreatedAt ON dbo.AuditSecurityChanges (UserId, CreatedAt DESC);
    CREATE INDEX IX_AuditSecurityChanges_CreatedAt ON dbo.AuditSecurityChanges (CreatedAt DESC);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_MENUSEGURIDADLISTAR
AS
BEGIN
    SELECT
        menu.Id,
        menu.ParentId,
        parent.Code AS ParentCode,
        parent.Name AS ParentName,
        menu.Code,
        menu.Name,
        menu.Description,
        CAST(menu.MenuType AS int) AS MenuType,
        CASE menu.MenuType
            WHEN 1 THEN N'Grupo'
            WHEN 2 THEN N'Submenu'
            WHEN 3 THEN N'Formulario'
            ELSE N'Desconocido'
        END AS MenuTypeName,
        menu.FormKey,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.Description AS FormDescription,
        menu.IconLarge,
        menu.IconSmall,
        menu.DisplayOrder,
        menu.IsVisible,
        menu.IsActive,
        menu.CreatedByUserId,
        menu.CreatedByUserName,
        menu.CreatedAt,
        menu.UpdatedByUserId,
        menu.UpdatedByUserName,
        menu.UpdatedAt,
        menu.DeletedByUserId,
        menu.DeletedByUserName,
        menu.DeletedAt
    FROM dbo.SecurityMenus menu
    LEFT JOIN dbo.SecurityMenus parent ON parent.Id = menu.ParentId
        AND parent.IsDeleted = 0
    LEFT JOIN dbo.SecurityForms form ON form.FormKey = menu.FormKey
        AND form.IsDeleted = 0
    WHERE menu.IsDeleted = 0
    ORDER BY menu.ParentId, menu.DisplayOrder, menu.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_MENUSEGURIDADBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        menu.Id,
        menu.ParentId,
        parent.Code AS ParentCode,
        parent.Name AS ParentName,
        menu.Code,
        menu.Name,
        menu.Description,
        CAST(menu.MenuType AS int) AS MenuType,
        CASE menu.MenuType
            WHEN 1 THEN N'Grupo'
            WHEN 2 THEN N'Submenu'
            WHEN 3 THEN N'Formulario'
            ELSE N'Desconocido'
        END AS MenuTypeName,
        menu.FormKey,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.Description AS FormDescription,
        menu.IconLarge,
        menu.IconSmall,
        menu.DisplayOrder,
        menu.IsVisible,
        menu.IsActive,
        menu.CreatedByUserId,
        menu.CreatedByUserName,
        menu.CreatedAt,
        menu.UpdatedByUserId,
        menu.UpdatedByUserName,
        menu.UpdatedAt,
        menu.DeletedByUserId,
        menu.DeletedByUserName,
        menu.DeletedAt
    FROM dbo.SecurityMenus menu
    LEFT JOIN dbo.SecurityMenus parent ON parent.Id = menu.ParentId
        AND parent.IsDeleted = 0
    LEFT JOIN dbo.SecurityForms form ON form.FormKey = menu.FormKey
        AND form.IsDeleted = 0
    WHERE menu.Id = @Id AND menu.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_MENUSEGURIDADBUSCARPORCODIGO
    @Code nvarchar(80),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.SecurityMenus
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_MENUSEGURIDADCREAR
    @ParentId int = NULL,
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @MenuType int,
    @FormKey nvarchar(120) = NULL,
    @IconLarge nvarchar(200) = NULL,
    @IconSmall nvarchar(200) = NULL,
    @DisplayOrder int = 0,
    @IsVisible bit = 1,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey, IconLarge, IconSmall,
        DisplayOrder, IsVisible, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @ParentId, @Code, @Name, @Description, @MenuType, @FormKey, @IconLarge, @IconSmall,
        @DisplayOrder, @IsVisible, @IsActive, @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME()
    );

    DECLARE @Id int = CAST(SCOPE_IDENTITY() AS int);

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityMenus', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
    FROM
    (
        VALUES
            (N'ParentId', CONVERT(nvarchar(max), @ParentId)),
            (N'Code', @Code),
            (N'Name', @Name),
            (N'Description', @Description),
            (N'MenuType', CONVERT(nvarchar(max), @MenuType)),
            (N'FormKey', @FormKey),
            (N'IconLarge', @IconLarge),
            (N'IconSmall', @IconSmall),
            (N'DisplayOrder', CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsVisible', CONVERT(nvarchar(max), CONVERT(int, @IsVisible))),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, NewValue);

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_MENUSEGURIDADACTUALIZAR
    @Id int,
    @ParentId int = NULL,
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @MenuType int,
    @FormKey nvarchar(120) = NULL,
    @IconLarge nvarchar(200) = NULL,
    @IconSmall nvarchar(200) = NULL,
    @DisplayOrder int = 0,
    @IsVisible bit = 1,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    DECLARE
        @OldParentId int,
        @OldCode nvarchar(80),
        @OldName nvarchar(120),
        @OldDescription nvarchar(300),
        @OldMenuType tinyint,
        @OldFormKey nvarchar(120),
        @OldIconLarge nvarchar(200),
        @OldIconSmall nvarchar(200),
        @OldDisplayOrder int,
        @OldIsVisible bit,
        @OldIsActive bit;

    SELECT
        @OldParentId = ParentId,
        @OldCode = Code,
        @OldName = Name,
        @OldDescription = Description,
        @OldMenuType = MenuType,
        @OldFormKey = FormKey,
        @OldIconLarge = IconLarge,
        @OldIconSmall = IconSmall,
        @OldDisplayOrder = DisplayOrder,
        @OldIsVisible = IsVisible,
        @OldIsActive = IsActive
    FROM dbo.SecurityMenus
    WHERE Id = @Id AND IsDeleted = 0;

    DECLARE @Rows int;

    UPDATE dbo.SecurityMenus
    SET
        ParentId = @ParentId,
        Code = @Code,
        Name = @Name,
        Description = @Description,
        MenuType = @MenuType,
        FormKey = @FormKey,
        IconLarge = @IconLarge,
        IconSmall = @IconSmall,
        DisplayOrder = @DisplayOrder,
        IsVisible = @IsVisible,
        IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    SET @Rows = @@ROWCOUNT;

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityMenus', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM
    (
        VALUES
            (N'ParentId', CONVERT(nvarchar(max), @OldParentId), CONVERT(nvarchar(max), @ParentId)),
            (N'Code', @OldCode, @Code),
            (N'Name', @OldName, @Name),
            (N'Description', @OldDescription, @Description),
            (N'MenuType', CONVERT(nvarchar(max), @OldMenuType), CONVERT(nvarchar(max), @MenuType)),
            (N'FormKey', @OldFormKey, @FormKey),
            (N'IconLarge', @OldIconLarge, @IconLarge),
            (N'IconSmall', @OldIconSmall, @IconSmall),
            (N'DisplayOrder', CONVERT(nvarchar(max), @OldDisplayOrder), CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsVisible', CONVERT(nvarchar(max), CONVERT(int, @OldIsVisible)), CONVERT(nvarchar(max), CONVERT(int, @IsVisible))),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    SELECT @Rows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_MENUSEGURIDADELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    DECLARE @Rows int;

    UPDATE dbo.SecurityMenus
    SET
        IsDeleted = 1,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    SET @Rows = @@ROWCOUNT;

    IF @Rows > 0
    BEGIN
        INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES (N'SecurityMenus', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);
    END;

    SELECT @Rows;
END;
GO
