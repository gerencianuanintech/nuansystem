/*
    Ejecutar este script en NuanSystem_Master.
    Registra Contabilidad > Plan de cuentas en menus, formularios, permisos y accesos admin.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.EXPORT_EXCEL')
BEGIN
    INSERT INTO dbo.SecurityOperations (Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey, DisplayOrder, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'ACTION.EXPORT_EXCEL', N'Excel', N'Exportar listado a Excel', N'Herramientas', N'Exportar', N'exportexcel', 100, 1, N'Sistema', SYSUTCDATETIME());
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.EXPORT_PDF')
BEGIN
    INSERT INTO dbo.SecurityOperations (Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey, DisplayOrder, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'ACTION.EXPORT_PDF', N'PDF', N'Exportar listado a PDF', N'Herramientas', N'Exportar', N'exportpdf', 110, 1, N'Sistema', SYSUTCDATETIME());
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.EXPORT_JSON')
BEGIN
    INSERT INTO dbo.SecurityOperations (Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey, DisplayOrder, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'ACTION.EXPORT_JSON', N'JSON', N'Exportar listado a JSON', N'Herramientas', N'Exportar', N'exportjson', 120, 1, N'Sistema', SYSUTCDATETIME());
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.EXPORT_XML')
BEGIN
    INSERT INTO dbo.SecurityOperations (Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey, DisplayOrder, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'ACTION.EXPORT_XML', N'XML', N'Exportar listado a XML', N'Herramientas', N'Exportar', N'exportxml', 130, 1, N'Sistema', SYSUTCDATETIME());
END;
GO

DECLARE @AccountingMenuId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.ACCOUNTING')
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.ACCOUNTING', N'Contabilidad', N'Modulo de contabilidad',
        1, NULL, N'Accordion/accounting_32.svg', N'Accordion/accounting_16.svg',
        60, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'Contabilidad',
    Description = N'Modulo de contabilidad',
    MenuType = 1,
    IconLarge = N'Accordion/accounting_32.svg',
    IconSmall = N'Accordion/accounting_16.svg',
    DisplayOrder = 60,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.ACCOUNTING';

SET @AccountingMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.ACCOUNTING' AND IsDeleted = 0);

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.ACCOUNTING.CHART_OF_ACCOUNTS')
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.ACCOUNTING.CHART_OF_ACCOUNTS', N'Plan de cuentas', N'Mantenimiento del plan de cuentas contable',
        N'chart-of-accounts', 1, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityForms
SET Name = N'Plan de cuentas',
    Description = N'Mantenimiento del plan de cuentas contable',
    FormKey = N'chart-of-accounts',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.ACCOUNTING.CHART_OF_ACCOUNTS';

DECLARE @ChartOfAccountsFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.ACCOUNTING.CHART_OF_ACCOUNTS' AND IsDeleted = 0);

IF @AccountingMenuId IS NOT NULL AND @ChartOfAccountsFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.ACCOUNTING.CHART_OF_ACCOUNTS')
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @AccountingMenuId, N'MENU.ACCOUNTING.CHART_OF_ACCOUNTS', N'Plan de cuentas',
            N'Mantenimiento del plan de cuentas contable',
            3, @ChartOfAccountsFormId, N'chart-of-accounts',
            N'Accordion/accounting_32.svg', N'Accordion/accounting_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @AccountingMenuId,
        Name = N'Plan de cuentas',
        Description = N'Mantenimiento del plan de cuentas contable',
        MenuType = 3,
        FormId = @ChartOfAccountsFormId,
        FormKey = N'chart-of-accounts',
        IconLarge = N'Accordion/accounting_32.svg',
        IconSmall = N'Accordion/accounting_16.svg',
        DisplayOrder = 10,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.ACCOUNTING.CHART_OF_ACCOUNTS';
END;

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @ChartOfAccountsMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.ACCOUNTING.CHART_OF_ACCOUNTS' AND IsDeleted = 0);

IF @AdminRoleId IS NOT NULL AND @AccountingMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleId AND MenuId = @AccountingMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, @AccountingMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleId IS NOT NULL AND @ChartOfAccountsMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleId AND MenuId = @ChartOfAccountsMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, @ChartOfAccountsMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleId IS NOT NULL AND @ChartOfAccountsFormId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, @ChartOfAccountsFormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.IsDeleted = 0
      AND operation.IsActive = 1
      AND operation.ActionKey IN
      (
          N'refresh',
          N'create',
          N'update',
          N'delete',
          N'consult',
          N'copy',
          N'history',
          N'customize-columns',
          N'customizecolumns',
          N'exportexcel',
          N'exportpdf',
          N'exportjson',
          N'exportxml'
      )
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = @ChartOfAccountsFormId
            AND existing.OperationId = operation.Id
      );
END;
GO
