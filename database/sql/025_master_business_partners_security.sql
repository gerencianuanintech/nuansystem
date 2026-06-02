/*
    Ejecutar este script en NuanSystem_Master.
    Registra los formularios y menus de Clientes y Proveedores basados en BusinessPartners.
*/

DECLARE @CatalogMenuId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CATALOGS' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.CATALOGS', N'Catalogos', N'Maestros comerciales y contables',
        1, NULL, N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg',
        20, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'Catalogos',
    Description = N'Maestros comerciales y contables',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 20,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.CATALOGS';

SET @CatalogMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CATALOGS' AND IsDeleted = 0);

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.CATALOGS.CUSTOMERS' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'FORM.CATALOGS.CUSTOMERS', N'Clientes', N'Mantenimiento empresarial de clientes', N'customers', 1, 1, 1, N'Sistema', SYSUTCDATETIME());
END;

UPDATE dbo.SecurityForms
SET Name = N'Clientes',
    Description = N'Mantenimiento empresarial de clientes',
    FormKey = N'customers',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.CATALOGS.CUSTOMERS';

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.CATALOGS.SUPPLIERS' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'FORM.CATALOGS.SUPPLIERS', N'Proveedores', N'Mantenimiento empresarial de proveedores', N'suppliers', 1, 1, 1, N'Sistema', SYSUTCDATETIME());
END;

UPDATE dbo.SecurityForms
SET Name = N'Proveedores',
    Description = N'Mantenimiento empresarial de proveedores',
    FormKey = N'suppliers',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.CATALOGS.SUPPLIERS';

DECLARE @CustomersFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.CATALOGS.CUSTOMERS' AND IsDeleted = 0);
DECLARE @SuppliersFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.CATALOGS.SUPPLIERS' AND IsDeleted = 0);

IF @CatalogMenuId IS NOT NULL AND @CustomersFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CATALOGS.CUSTOMERS' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @CatalogMenuId, N'MENU.CATALOGS.CUSTOMERS', N'Clientes',
            N'Mantenimiento empresarial de clientes', 3, @CustomersFormId, N'customers',
            N'Accordion/clientes_32.svg', N'Accordion/clientes_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @CatalogMenuId,
        Name = N'Clientes',
        Description = N'Mantenimiento empresarial de clientes',
        MenuType = 3,
        FormId = @CustomersFormId,
        FormKey = N'customers',
        DisplayOrder = 10,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.CATALOGS.CUSTOMERS';
END;

IF @CatalogMenuId IS NOT NULL AND @SuppliersFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CATALOGS.SUPPLIERS' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @CatalogMenuId, N'MENU.CATALOGS.SUPPLIERS', N'Proveedores',
            N'Mantenimiento empresarial de proveedores', 3, @SuppliersFormId, N'suppliers',
            N'Accordion/proveedores_32.svg', N'Accordion/proveedores_16.svg',
            20, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @CatalogMenuId,
        Name = N'Proveedores',
        Description = N'Mantenimiento empresarial de proveedores',
        MenuType = 3,
        FormId = @SuppliersFormId,
        FormKey = N'suppliers',
        DisplayOrder = 20,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.CATALOGS.SUPPLIERS';
END;

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @CustomersMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CATALOGS.CUSTOMERS' AND IsDeleted = 0);
DECLARE @SuppliersMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CATALOGS.SUPPLIERS' AND IsDeleted = 0);

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, menu.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityMenus menu
    WHERE menu.Id IN (@CatalogMenuId, @CustomersMenuId, @SuppliersMenuId)
      AND menu.Id IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleMenus existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.MenuId = menu.Id
      );

    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, formItem.FormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM (VALUES (@CustomersFormId), (@SuppliersFormId)) AS formItem(FormId)
    CROSS JOIN dbo.SecurityOperations operation
    WHERE formItem.FormId IS NOT NULL
      AND operation.IsDeleted = 0
      AND operation.IsActive = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = formItem.FormId
            AND existing.OperationId = operation.Id
      );
END;
GO
