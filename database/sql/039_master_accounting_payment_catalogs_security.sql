/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos, formularios y menus independientes para catalogos contables de pago.
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
    (N'FINANCIAL.ACCOUNTINGPAYMENTMETHODS.READ', N'Ver metodos de pago contable', N'Consultar metodos de pago contable.'),
    (N'FINANCIAL.ACCOUNTINGPAYMENTMETHODS.MANAGE', N'Gestionar metodos de pago contable', N'Crear, editar y eliminar metodos de pago contable.'),
    (N'FINANCIAL.PAYMENTPRIORITIES.READ', N'Ver prioridades de pago', N'Consultar prioridades de pago.'),
    (N'FINANCIAL.PAYMENTPRIORITIES.MANAGE', N'Gestionar prioridades de pago', N'Crear, editar y eliminar prioridades de pago.'),
    (N'FINANCIAL.APPROVALFLOWS.READ', N'Ver flujos de aprobacion', N'Consultar flujos de aprobacion.'),
    (N'FINANCIAL.APPROVALFLOWS.MANAGE', N'Gestionar flujos de aprobacion', N'Crear, editar y eliminar flujos de aprobacion.'),
    (N'FINANCIAL.PAYMENTDOCUMENTTYPES.READ', N'Ver tipos de documento de pago', N'Consultar tipos de documento de pago.'),
    (N'FINANCIAL.PAYMENTDOCUMENTTYPES.MANAGE', N'Gestionar tipos de documento de pago', N'Crear, editar y eliminar tipos de documento de pago.');

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
          SELECT 1 FROM dbo.RolePermissions existing
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
    (N'FORM.FINANCIALCATALOGS.ACCOUNTINGPAYMENTMETHODS', N'Metodos de pago contable', N'Mantenimiento de metodos de pago contable', N'accounting-payment-methods', N'MENU.FINANCIALCATALOGS.ACCOUNTINGPAYMENTMETHODS', 60),
    (N'FORM.FINANCIALCATALOGS.PAYMENTPRIORITIES', N'Prioridades de pago', N'Mantenimiento de prioridades de pago', N'payment-priorities', N'MENU.FINANCIALCATALOGS.PAYMENTPRIORITIES', 70),
    (N'FORM.FINANCIALCATALOGS.APPROVALFLOWS', N'Flujos de aprobacion', N'Mantenimiento de flujos de aprobacion', N'approval-flows', N'MENU.FINANCIALCATALOGS.APPROVALFLOWS', 80),
    (N'FORM.FINANCIALCATALOGS.PAYMENTDOCUMENTTYPES', N'Tipos de documento de pago', N'Mantenimiento de tipos de documento de pago', N'payment-document-types', N'MENU.FINANCIALCATALOGS.PAYMENTDOCUMENTTYPES', 90);

INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
SELECT source.Code, source.Name, source.Description, source.FormKey, 1, 1, 1, N'Sistema', SYSUTCDATETIME()
FROM @Forms source
WHERE NOT EXISTS
(
    SELECT 1 FROM dbo.SecurityForms existing
    WHERE existing.Code = source.Code AND existing.IsDeleted = 0
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
        SELECT 1 FROM dbo.SecurityMenus existing
        WHERE existing.Code = source.MenuCode AND existing.IsDeleted = 0
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
    WHERE menu.Code IN (SELECT MenuCode FROM @Forms)
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.SecurityRoleMenus existing
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
          SELECT 1 FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = formItem.Id
            AND existing.OperationId = operation.Id
      );
END;
GO
