/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos, menus y formularios independientes para catalogos financieros y de compras.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'FINANCIALCATALOGS')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'FINANCIALCATALOGS', N'Catalogos Financieros', 46);
END;
GO

DECLARE @FinancialCatalogsModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'FINANCIALCATALOGS');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'FINANCIAL.CATALOGS.READ', N'Ver catalogos financieros', N'Consultar catalogos financieros y de compras.'),
    (N'FINANCIAL.CATALOGS.MANAGE', N'Gestionar catalogos financieros', N'Crear, editar y eliminar catalogos financieros y de compras.'),
    (N'FINANCIAL.BANKS.READ', N'Ver bancos', N'Consultar bancos.'),
    (N'FINANCIAL.BANKS.MANAGE', N'Gestionar bancos', N'Crear, editar y eliminar bancos.'),
    (N'FINANCIAL.BANKACCOUNTTYPES.READ', N'Ver tipos de cuenta bancaria', N'Consultar tipos de cuenta bancaria.'),
    (N'FINANCIAL.BANKACCOUNTTYPES.MANAGE', N'Gestionar tipos de cuenta bancaria', N'Crear, editar y eliminar tipos de cuenta bancaria.'),
    (N'FINANCIAL.CURRENCIES.READ', N'Ver monedas', N'Consultar monedas.'),
    (N'FINANCIAL.CURRENCIES.MANAGE', N'Gestionar monedas', N'Crear, editar y eliminar monedas.'),
    (N'FINANCIAL.PRICELISTS.READ', N'Ver listas de precios', N'Consultar listas de precios.'),
    (N'FINANCIAL.PRICELISTS.MANAGE', N'Gestionar listas de precios', N'Crear, editar y eliminar listas de precios.'),
    (N'FINANCIAL.PURCHASINGAGENTS.READ', N'Ver compradores', N'Consultar compradores o agentes de compras.'),
    (N'FINANCIAL.PURCHASINGAGENTS.MANAGE', N'Gestionar compradores', N'Crear, editar y eliminar compradores o agentes de compras.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @FinancialCatalogsModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @FinancialCatalogsModuleId IS NOT NULL
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

UPDATE dbo.SecurityMenus
SET Name = N'Catalogos Financieros',
    Description = N'Maestros auxiliares financieros y de compras',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 26,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.FINANCIALCATALOGS';

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
    (N'FORM.FINANCIALCATALOGS.BANKS', N'Bancos', N'Mantenimiento de bancos', N'banks', N'MENU.FINANCIALCATALOGS.BANKS', 10),
    (N'FORM.FINANCIALCATALOGS.BANKACCOUNTTYPES', N'Tipos de cuenta bancaria', N'Mantenimiento de tipos de cuenta bancaria', N'bank-account-types', N'MENU.FINANCIALCATALOGS.BANKACCOUNTTYPES', 20),
    (N'FORM.FINANCIALCATALOGS.CURRENCIES', N'Monedas', N'Mantenimiento de monedas', N'currencies', N'MENU.FINANCIALCATALOGS.CURRENCIES', 30),
    (N'FORM.FINANCIALCATALOGS.PRICELISTS', N'Listas de precios', N'Mantenimiento de listas de precios', N'price-lists', N'MENU.FINANCIALCATALOGS.PRICELISTS', 40),
    (N'FORM.FINANCIALCATALOGS.PURCHASINGAGENTS', N'Compradores', N'Mantenimiento de compradores y agentes de compras', N'purchasing-agents', N'MENU.FINANCIALCATALOGS.PURCHASINGAGENTS', 50);

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
