/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos, menus y formularios independientes para catalogos geograficos.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'GEOGRAPHY')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'GEOGRAPHY', N'Geografia', 47);
END;
GO

DECLARE @GeographyModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'GEOGRAPHY');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'GEOGRAPHY.COUNTRIES.READ', N'Ver paises', N'Consultar paises.'),
    (N'GEOGRAPHY.COUNTRIES.MANAGE', N'Gestionar paises', N'Crear, editar y eliminar paises.'),
    (N'GEOGRAPHY.PROVINCES.READ', N'Ver provincias', N'Consultar provincias o estados.'),
    (N'GEOGRAPHY.PROVINCES.MANAGE', N'Gestionar provincias', N'Crear, editar y eliminar provincias o estados.'),
    (N'GEOGRAPHY.CITIES.READ', N'Ver ciudades', N'Consultar ciudades.'),
    (N'GEOGRAPHY.CITIES.MANAGE', N'Gestionar ciudades', N'Crear, editar y eliminar ciudades.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @GeographyModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @GeographyModuleId IS NOT NULL
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.Permissions existing
      WHERE existing.Code = source.Code
  );

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
    SELECT @AdminRoleId, permission.Id
    FROM dbo.Permissions permission
    WHERE permission.Code IN (SELECT Code FROM @Permissions)
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.RolePermissions existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.PermissionId = permission.Id
      );
END;
GO

DECLARE @GeographyMenuId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.GEOGRAPHY' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.GEOGRAPHY', N'Geografia', N'Maestros geograficos',
        1, NULL, N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg',
        27, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'Geografia',
    Description = N'Maestros geograficos',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 27,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.GEOGRAPHY';

SET @GeographyMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.GEOGRAPHY' AND IsDeleted = 0);

DECLARE @Forms table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL,
    FormKey nvarchar(120) NOT NULL,
    MenuCode nvarchar(120) NOT NULL,
    DisplayOrder int NOT NULL
);

INSERT INTO @Forms (Code, Name, Description, FormKey, MenuCode, DisplayOrder)
VALUES
    (N'FORM.GEOGRAPHY.COUNTRIES', N'Paises', N'Mantenimiento de paises', N'countries', N'MENU.GEOGRAPHY.COUNTRIES', 10),
    (N'FORM.GEOGRAPHY.PROVINCES', N'Provincias', N'Mantenimiento de provincias o estados', N'provinces', N'MENU.GEOGRAPHY.PROVINCES', 20),
    (N'FORM.GEOGRAPHY.CITIES', N'Ciudades', N'Mantenimiento de ciudades', N'cities', N'MENU.GEOGRAPHY.CITIES', 30);

INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
SELECT source.Code, source.Name, source.Description, source.FormKey, 1, 1, 1, N'Sistema', SYSUTCDATETIME()
FROM @Forms source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SecurityForms existing
    WHERE existing.Code = source.Code
      AND existing.IsDeleted = 0
);

UPDATE formTarget
SET Name = source.Name,
    Description = source.Description,
    FormKey = source.FormKey,
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
FROM dbo.SecurityForms formTarget
INNER JOIN @Forms source ON source.Code = formTarget.Code;

IF @GeographyMenuId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    SELECT
        @GeographyMenuId,
        source.MenuCode,
        source.Name,
        source.Description,
        3,
        formItem.Id,
        source.FormKey,
        N'Accordion/catalogos_32.svg',
        N'Accordion/catalogos_16.svg',
        source.DisplayOrder,
        1,
        1,
        N'Sistema',
        SYSUTCDATETIME()
    FROM @Forms source
    INNER JOIN dbo.SecurityForms formItem ON formItem.Code = source.Code AND formItem.IsDeleted = 0
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.SecurityMenus existing
        WHERE existing.Code = source.MenuCode
          AND existing.IsDeleted = 0
    );
END;
GO
