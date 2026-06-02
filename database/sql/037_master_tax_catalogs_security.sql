/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos, menus y formularios independientes para catalogos tributarios.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'TAXCATALOGS')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'TAXCATALOGS', N'Catalogos Tributarios', 48);
END;
GO

DECLARE @TaxCatalogsModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'TAXCATALOGS');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'TAX.REGIMES.READ', N'Ver regimenes tributarios', N'Consultar regimenes tributarios.'),
    (N'TAX.REGIMES.MANAGE', N'Gestionar regimenes tributarios', N'Crear, editar y eliminar regimenes tributarios.'),
    (N'TAX.TAXPAYERTYPES.READ', N'Ver tipos de contribuyente', N'Consultar tipos de contribuyente.'),
    (N'TAX.TAXPAYERTYPES.MANAGE', N'Gestionar tipos de contribuyente', N'Crear, editar y eliminar tipos de contribuyente.'),
    (N'TAX.RETENTIONTYPES.READ', N'Ver tipos de retencion', N'Consultar tipos de retencion.'),
    (N'TAX.RETENTIONTYPES.MANAGE', N'Gestionar tipos de retencion', N'Crear, editar y eliminar tipos de retencion.'),
    (N'TAX.RETENTIONCONCEPTS.READ', N'Ver conceptos de retencion', N'Consultar conceptos de retencion.'),
    (N'TAX.RETENTIONCONCEPTS.MANAGE', N'Gestionar conceptos de retencion', N'Crear, editar y eliminar conceptos de retencion.'),
    (N'TAX.TAXSUPPORTS.READ', N'Ver sustentos tributarios', N'Consultar sustentos tributarios.'),
    (N'TAX.TAXSUPPORTS.MANAGE', N'Gestionar sustentos tributarios', N'Crear, editar y eliminar sustentos tributarios.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @TaxCatalogsModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @TaxCatalogsModuleId IS NOT NULL
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

DECLARE @TaxCatalogsMenuId int;
DECLARE @AdminRoleId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.TAXCATALOGS' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.TAXCATALOGS', N'Catalogos Tributarios', N'Maestros auxiliares tributarios para proveedores, compras y retenciones',
        1, NULL, N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg',
        28, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'Catalogos Tributarios',
    Description = N'Maestros auxiliares tributarios para proveedores, compras y retenciones',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 28,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.TAXCATALOGS';

SET @TaxCatalogsMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.TAXCATALOGS' AND IsDeleted = 0);

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
    (N'FORM.TAXCATALOGS.TAXREGIMES', N'Regimenes tributarios', N'Mantenimiento de regimenes tributarios', N'tax-regimes', N'MENU.TAXCATALOGS.TAXREGIMES', 10),
    (N'FORM.TAXCATALOGS.TAXPAYERTYPES', N'Tipos de contribuyente', N'Mantenimiento de tipos de contribuyente', N'taxpayer-types', N'MENU.TAXCATALOGS.TAXPAYERTYPES', 20),
    (N'FORM.TAXCATALOGS.RETENTIONTYPES', N'Tipos de retencion', N'Mantenimiento de tipos de retencion', N'retention-types', N'MENU.TAXCATALOGS.RETENTIONTYPES', 30),
    (N'FORM.TAXCATALOGS.RETENTIONCONCEPTS', N'Conceptos de retencion', N'Mantenimiento de conceptos de retencion', N'retention-concepts', N'MENU.TAXCATALOGS.RETENTIONCONCEPTS', 40),
    (N'FORM.TAXCATALOGS.TAXSUPPORTS', N'Sustentos tributarios', N'Mantenimiento de sustentos tributarios', N'tax-supports', N'MENU.TAXCATALOGS.TAXSUPPORTS', 50);

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

IF @TaxCatalogsMenuId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    SELECT
        @TaxCatalogsMenuId,
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
    SET ParentId = @TaxCatalogsMenuId,
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
    WHERE (menu.Code = N'MENU.TAXCATALOGS'
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
