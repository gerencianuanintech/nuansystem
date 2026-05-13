IF COL_LENGTH('dbo.SecurityMenus', 'FormId') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityMenus ADD FormId int NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_SecurityMenus_Form'
      AND parent_object_id = OBJECT_ID(N'dbo.SecurityMenus')
)
BEGIN
    ALTER TABLE dbo.SecurityMenus
    ADD CONSTRAINT FK_SecurityMenus_Form FOREIGN KEY (FormId) REFERENCES dbo.SecurityForms(Id);
END;
GO

UPDATE menu
SET FormId = form.Id
FROM dbo.SecurityMenus menu
INNER JOIN dbo.SecurityForms form ON form.FormKey = menu.FormKey
WHERE menu.FormId IS NULL
  AND menu.FormKey IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.REFRESH')
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'ACTION.REFRESH', N'Actualizar', N'Refrescar datos de la pantalla activa',
        N'Inicio', N'Acciones', N'refresh', NULL, NULL,
        10, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.DELETE')
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'ACTION.DELETE', N'Eliminar', N'Eliminar o inactivar registros existentes',
        N'Inicio', N'Acciones', N'delete', NULL, NULL,
        40, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.CONSULT')
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'ACTION.CONSULT', N'Consultar', N'Abrir registros en modo solo lectura',
        N'Inicio', N'Acciones', N'consult', NULL, NULL,
        35, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.HISTORY')
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'ACTION.HISTORY', N'Historial', N'Consultar las modificaciones realizadas al registro seleccionado',
        N'Inicio', N'Acciones', N'history', NULL, NULL,
        38, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.RELOAD_ACCESS')
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'ACTION.RELOAD_ACCESS', N'Cargar accesos', N'Recargar permisos, menús y operaciones sin cerrar sesión',
        N'Inicio', N'Sesion', N'reload-access', N'Operaciones/actualizar_32.svg', N'Operaciones/actualizar_16.svg',
        90, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

UPDATE dbo.SecurityOperations
SET IconLarge = NULL
WHERE IconLarge IN
(
    N'Add_32x32.svg',
    N'Edit_32x32.svg',
    N'Delete_32x32.svg',
    N'Refresh_32x32.svg',
    N'Find_32x32.svg',
    N'History_32x32.svg'
);
GO

UPDATE dbo.SecurityOperations
SET IconSmall = NULL
WHERE IconSmall IN
(
    N'Add_16x16.svg',
    N'Edit_16x16.svg',
    N'Delete_16x16.svg',
    N'Refresh_16x16.svg',
    N'Find_16x16.svg',
    N'History_16x16.svg'
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.ACCESS')
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.SECURITY.ACCESS', N'Accesos', N'Asignación de menús y operaciones por rol',
        N'security-access', 1, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

DECLARE @SecurityMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY');
DECLARE @AccessFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.ACCESS');

IF @SecurityMenuId IS NOT NULL AND @AccessFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.ACCESS')
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @SecurityMenuId, N'MENU.SECURITY.ACCESS', N'Accesos',
            N'Configurar accesos por rol a menús y operaciones',
            3, @AccessFormId, N'security-access',
            N'Security_32x32.svg', N'Security_16x16.svg',
            60, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET FormId = @AccessFormId,
        FormKey = N'security-access'
    WHERE Code = N'MENU.SECURITY.ACCESS';
END;
GO

IF OBJECT_ID(N'dbo.SecurityRoleMenus', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityRoleMenus
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityRoleMenus PRIMARY KEY,
        RoleId int NOT NULL,
        MenuId int NOT NULL,
        IsAllowed bit NOT NULL CONSTRAINT DF_SecurityRoleMenus_IsAllowed DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityRoleMenus_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityRoleMenus_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityRoleMenus_Role FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id),
        CONSTRAINT FK_SecurityRoleMenus_Menu FOREIGN KEY (MenuId) REFERENCES dbo.SecurityMenus(Id),
        CONSTRAINT UQ_SecurityRoleMenus_Role_Menu UNIQUE (RoleId, MenuId)
    );
END;
GO

IF OBJECT_ID(N'dbo.SecurityRoleFormOperations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityRoleFormOperations
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityRoleFormOperations PRIMARY KEY,
        RoleId int NOT NULL,
        FormId int NOT NULL,
        OperationId int NOT NULL,
        IsAllowed bit NOT NULL CONSTRAINT DF_SecurityRoleFormOperations_IsAllowed DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityRoleFormOperations_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityRoleFormOperations_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityRoleFormOperations_Role FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id),
        CONSTRAINT FK_SecurityRoleFormOperations_Form FOREIGN KEY (FormId) REFERENCES dbo.SecurityForms(Id),
        CONSTRAINT FK_SecurityRoleFormOperations_Operation FOREIGN KEY (OperationId) REFERENCES dbo.SecurityOperations(Id),
        CONSTRAINT UQ_SecurityRoleFormOperations_Role_Form_Operation UNIQUE (RoleId, FormId, OperationId)
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SEGURIDADNAVEGACIONUSUARIO
    @UserId int
AS
BEGIN
    DECLARE @HasConfiguredAccess bit = 0;

    IF EXISTS (
        SELECT 1
        FROM dbo.UserRoles userRole
        INNER JOIN dbo.SecurityRoleMenus roleMenu ON roleMenu.RoleId = userRole.RoleId
        WHERE userRole.UserId = @UserId
          AND roleMenu.IsDeleted = 0
    )
    BEGIN
        SET @HasConfiguredAccess = 1;
    END;

    IF @HasConfiguredAccess = 1
    BEGIN
        SELECT DISTINCT
            menu.Id,
            menu.ParentId,
            menu.Code,
            menu.Name,
            menu.Description,
            CAST(menu.MenuType AS int) AS MenuType,
            COALESCE(form.FormKey, menu.FormKey) AS FormKey,
            menu.IconLarge,
            menu.IconSmall,
            menu.DisplayOrder,
            menu.IsVisible,
            menu.IsActive
        FROM dbo.SecurityMenus menu
        LEFT JOIN dbo.SecurityForms form ON form.Id = menu.FormId AND form.IsDeleted = 0
        INNER JOIN dbo.SecurityRoleMenus roleMenu ON roleMenu.MenuId = menu.Id
            AND roleMenu.IsDeleted = 0
            AND roleMenu.IsAllowed = 1
        INNER JOIN dbo.UserRoles userRole ON userRole.RoleId = roleMenu.RoleId
            AND userRole.UserId = @UserId
        WHERE menu.IsDeleted = 0
          AND menu.IsVisible = 1
          AND menu.IsActive = 1
        ORDER BY menu.ParentId, menu.DisplayOrder, menu.Name;

        RETURN;
    END;

    SELECT
        menu.Id,
        menu.ParentId,
        menu.Code,
        menu.Name,
        menu.Description,
        CAST(menu.MenuType AS int) AS MenuType,
        COALESCE(form.FormKey, menu.FormKey) AS FormKey,
        menu.IconLarge,
        menu.IconSmall,
        menu.DisplayOrder,
        menu.IsVisible,
        menu.IsActive
    FROM dbo.SecurityMenus menu
    LEFT JOIN dbo.SecurityForms form ON form.Id = menu.FormId AND form.IsDeleted = 0
    WHERE menu.IsDeleted = 0
      AND menu.IsVisible = 1
      AND menu.IsActive = 1
    ORDER BY menu.ParentId, menu.DisplayOrder, menu.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SEGURIDADOPERACIONESUSUARIO
    @UserId int,
    @FormKey nvarchar(120)
AS
BEGIN
    DECLARE @FormId int = (
        SELECT TOP (1) Id
        FROM dbo.SecurityForms
        WHERE FormKey = @FormKey
          AND IsDeleted = 0
    );

    IF @FormId IS NULL
    BEGIN
        SELECT
            CAST(0 AS int) AS OperationId,
            CAST(NULL AS nvarchar(80)) AS Code,
            CAST(NULL AS nvarchar(120)) AS Name,
            CAST(NULL AS nvarchar(300)) AS Description,
            CAST(NULL AS nvarchar(120)) AS ActionKey,
            CAST(NULL AS nvarchar(80)) AS RibbonPageName,
            CAST(NULL AS nvarchar(80)) AS RibbonGroupName,
            CAST(NULL AS nvarchar(200)) AS IconLarge,
            CAST(NULL AS nvarchar(200)) AS IconSmall,
            CAST(0 AS int) AS DisplayOrder,
            CAST(0 AS bit) AS IsAllowed
        WHERE 1 = 0;
        RETURN;
    END;

    SELECT
        operation.Id AS OperationId,
        operation.Code,
        operation.Name,
        operation.Description,
        operation.ActionKey,
        operation.RibbonPageName,
        operation.RibbonGroupName,
        operation.IconLarge,
        operation.IconSmall,
        operation.DisplayOrder,
        CAST(CASE WHEN EXISTS
        (
            SELECT 1
            FROM dbo.UserRoles userRole
            INNER JOIN dbo.SecurityRoleFormOperations roleOperation ON roleOperation.RoleId = userRole.RoleId
            WHERE userRole.UserId = @UserId
              AND roleOperation.FormId = @FormId
              AND roleOperation.OperationId = operation.Id
              AND roleOperation.IsDeleted = 0
              AND roleOperation.IsAllowed = 1
        ) THEN 1 ELSE 0 END AS bit) AS IsAllowed
    FROM dbo.SecurityOperations operation
    WHERE operation.IsDeleted = 0
      AND operation.IsActive = 1
    ORDER BY operation.DisplayOrder, operation.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ACCESOROLCARGAR
    @RoleId int
AS
BEGIN
    SELECT
        menu.Id AS MenuId,
        menu.ParentId,
        menu.Code,
        menu.Name,
        CAST(menu.MenuType AS int) AS MenuType,
        COALESCE(form.FormKey, menu.FormKey) AS FormKey,
        CAST(CASE WHEN roleMenu.IsAllowed = 1 THEN 1 ELSE 0 END AS bit) AS IsAllowed
    FROM dbo.SecurityMenus menu
    LEFT JOIN dbo.SecurityForms form ON form.Id = menu.FormId AND form.IsDeleted = 0
    LEFT JOIN dbo.SecurityRoleMenus roleMenu ON roleMenu.MenuId = menu.Id
        AND roleMenu.RoleId = @RoleId
        AND roleMenu.IsDeleted = 0
    WHERE menu.IsDeleted = 0
    ORDER BY menu.ParentId, menu.DisplayOrder, menu.Name;

    SELECT
        form.Id AS FormId,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.FormKey,
        operation.Id AS OperationId,
        operation.Code AS OperationCode,
        operation.Name AS OperationName,
        operation.ActionKey,
        CAST(CASE WHEN roleOperation.IsAllowed = 1 THEN 1 ELSE 0 END AS bit) AS IsAllowed
    FROM dbo.SecurityForms form
    CROSS JOIN dbo.SecurityOperations operation
    LEFT JOIN dbo.SecurityRoleFormOperations roleOperation ON roleOperation.FormId = form.Id
        AND roleOperation.OperationId = operation.Id
        AND roleOperation.RoleId = @RoleId
        AND roleOperation.IsDeleted = 0
    WHERE form.IsDeleted = 0
      AND form.IsActive = 1
      AND operation.IsDeleted = 0
      AND operation.IsActive = 1
    ORDER BY form.Name, operation.DisplayOrder, operation.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ACCESOROLGUARDAR
    @RoleId int,
    @MenusJson nvarchar(max),
    @OperationsJson nvarchar(max),
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @Menus table
    (
        MenuId int NOT NULL,
        IsAllowed bit NOT NULL
    );

    INSERT INTO @Menus (MenuId, IsAllowed)
    SELECT MenuId, IsAllowed
    FROM OPENJSON(@MenusJson)
    WITH
    (
        MenuId int '$.menuId',
        IsAllowed bit '$.isAllowed'
    );

    UPDATE dbo.SecurityRoleMenus
    SET IsDeleted = 1,
        DeletedByUserId = @UpdatedByUserId,
        DeletedByUserName = @UpdatedByUserName,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE RoleId = @RoleId
      AND IsDeleted = 0;

    MERGE dbo.SecurityRoleMenus AS target
    USING @Menus AS source
        ON target.RoleId = @RoleId
       AND target.MenuId = source.MenuId
    WHEN MATCHED THEN
        UPDATE SET
            IsAllowed = source.IsAllowed,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (RoleId, MenuId, IsAllowed, CreatedByUserId, CreatedByUserName, CreatedAt)
        VALUES (@RoleId, source.MenuId, source.IsAllowed, @UpdatedByUserId, @UpdatedByUserName, SYSUTCDATETIME());

    DECLARE @Operations table
    (
        FormId int NOT NULL,
        OperationId int NOT NULL,
        IsAllowed bit NOT NULL
    );

    INSERT INTO @Operations (FormId, OperationId, IsAllowed)
    SELECT FormId, OperationId, IsAllowed
    FROM OPENJSON(@OperationsJson)
    WITH
    (
        FormId int '$.formId',
        OperationId int '$.operationId',
        IsAllowed bit '$.isAllowed'
    );

    UPDATE dbo.SecurityRoleFormOperations
    SET IsDeleted = 1,
        DeletedByUserId = @UpdatedByUserId,
        DeletedByUserName = @UpdatedByUserName,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE RoleId = @RoleId
      AND IsDeleted = 0;

    MERGE dbo.SecurityRoleFormOperations AS target
    USING @Operations AS source
        ON target.RoleId = @RoleId
       AND target.FormId = source.FormId
       AND target.OperationId = source.OperationId
    WHEN MATCHED THEN
        UPDATE SET
            IsAllowed = source.IsAllowed,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (RoleId, FormId, OperationId, IsAllowed, CreatedByUserId, CreatedByUserName, CreatedAt)
        VALUES (@RoleId, source.FormId, source.OperationId, source.IsAllowed, @UpdatedByUserId, @UpdatedByUserName, SYSUTCDATETIME());

    COMMIT TRANSACTION;

    SELECT CAST(1 AS bit);
END;
GO
