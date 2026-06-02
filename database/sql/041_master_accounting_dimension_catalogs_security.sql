/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos, menus y formularios independientes para dimensiones contables.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'FINANCIALCATALOGS')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'FINANCIALCATALOGS', N'Catalogos Financieros', 46);
END;
GO

DECLARE @FinancialCatalogsModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'FINANCIALCATALOGS');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'FINANCIAL.BRANCHES.READ', N'Ver sucursales', N'Consultar sucursales.'),
    (N'FINANCIAL.BRANCHES.MANAGE', N'Gestionar sucursales', N'Crear, editar y eliminar sucursales.'),
    (N'FINANCIAL.DEPARTMENTS.READ', N'Ver departamentos', N'Consultar departamentos.'),
    (N'FINANCIAL.DEPARTMENTS.MANAGE', N'Gestionar departamentos', N'Crear, editar y eliminar departamentos.'),
    (N'FINANCIAL.BUSINESSLINES.READ', N'Ver lineas de negocio', N'Consultar lineas de negocio.'),
    (N'FINANCIAL.BUSINESSLINES.MANAGE', N'Gestionar lineas de negocio', N'Crear, editar y eliminar lineas de negocio.'),
    (N'FINANCIAL.COSTCENTERS.READ', N'Ver centros de costo', N'Consultar centros de costo.'),
    (N'FINANCIAL.COSTCENTERS.MANAGE', N'Gestionar centros de costo', N'Crear, editar y eliminar centros de costo.'),
    (N'FINANCIAL.PROJECTS.READ', N'Ver proyectos', N'Consultar proyectos.'),
    (N'FINANCIAL.PROJECTS.MANAGE', N'Gestionar proyectos', N'Crear, editar y eliminar proyectos.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @FinancialCatalogsModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @FinancialCatalogsModuleId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.Permissions existing WHERE existing.Code = source.Code);

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

DECLARE @FinancialCatalogsMenuId int;
DECLARE @AdminRoleId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.FINANCIALCATALOGS' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.FINANCIALCATALOGS', N'Catalogos Financieros', N'Maestros auxiliares financieros y de compras',
        1, NULL, N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg',
        26, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

SET @FinancialCatalogsMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.FINANCIALCATALOGS' AND IsDeleted = 0);

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
    (N'FORM.FINANCIALCATALOGS.BRANCHES', N'Sucursales', N'Mantenimiento de sucursales', N'branches', N'MENU.FINANCIALCATALOGS.BRANCHES', 100),
    (N'FORM.FINANCIALCATALOGS.DEPARTMENTS', N'Departamentos', N'Mantenimiento de departamentos', N'departments', N'MENU.FINANCIALCATALOGS.DEPARTMENTS', 110),
    (N'FORM.FINANCIALCATALOGS.BUSINESSLINES', N'Lineas de negocio', N'Mantenimiento de lineas de negocio', N'business-lines', N'MENU.FINANCIALCATALOGS.BUSINESSLINES', 120),
    (N'FORM.FINANCIALCATALOGS.COSTCENTERS', N'Centros de costo', N'Mantenimiento de centros de costo', N'cost-centers', N'MENU.FINANCIALCATALOGS.COSTCENTERS', 130),
    (N'FORM.FINANCIALCATALOGS.PROJECTS', N'Proyectos', N'Mantenimiento de proyectos', N'projects', N'MENU.FINANCIALCATALOGS.PROJECTS', 140);

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

IF @FinancialCatalogsMenuId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    SELECT
        @FinancialCatalogsMenuId,
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

    UPDATE menuTarget
    SET ParentId = @FinancialCatalogsMenuId,
        Name = source.Name,
        Description = source.Description,
        MenuType = 3,
        FormId = formItem.Id,
        FormKey = source.FormKey,
        DisplayOrder = source.DisplayOrder,
        IsVisible = 1,
        IsActive = 1
    FROM dbo.SecurityMenus menuTarget
    INNER JOIN @Forms source ON source.MenuCode = menuTarget.Code
    INNER JOIN dbo.SecurityForms formItem ON formItem.Code = source.Code AND formItem.IsDeleted = 0;
END;

SET @AdminRoleId = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, menu.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityMenus menu
    WHERE (menu.Code = N'MENU.FINANCIALCATALOGS'
       OR menu.Code IN (SELECT MenuCode FROM @Forms))
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleMenus existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.MenuId = menu.Id
      );

    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, formItem.Id, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityForms formItem
    CROSS JOIN dbo.SecurityOperations operation
    WHERE formItem.Code IN (SELECT Code FROM @Forms)
      AND formItem.IsDeleted = 0
      AND operation.IsDeleted = 0
      AND operation.IsActive = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = formItem.Id
            AND existing.OperationId = operation.Id
      );
END;
GO
