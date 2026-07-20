/* Ejecutar en NuanSystem_Master. Registra Transportistas como mantenimiento independiente. */
IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'CATALOG')
BEGIN
    INSERT dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'CATALOG', N'Catalogos', 20);
END;
GO

DECLARE @CatalogModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'CATALOG');
DECLARE @PermissionAdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @CarrierPermissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT @CarrierPermissions (Code, Name, Description)
VALUES
    (N'CATALOG.CARRIERS.READ', N'Consultar transportistas', N'Listar y consultar el maestro independiente de transportistas.'),
    (N'CATALOG.CARRIERS.MANAGE', N'Gestionar transportistas', N'Crear, editar y eliminar transportistas.');

INSERT dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @CatalogModuleId, source.Code, source.Name, source.Description
FROM @CarrierPermissions source
WHERE @CatalogModuleId IS NOT NULL
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.Permissions existing
      WHERE existing.Code = source.Code
  );

UPDATE permission
SET permission.ModuleId = @CatalogModuleId,
    permission.Name = source.Name,
    permission.Description = source.Description,
    permission.IsActive = 1,
    permission.UpdatedAt = SYSUTCDATETIME()
FROM dbo.Permissions permission
INNER JOIN @CarrierPermissions source ON source.Code = permission.Code
WHERE @CatalogModuleId IS NOT NULL;

IF @PermissionAdminRoleId IS NOT NULL
BEGIN
    INSERT dbo.RolePermissions (RoleId, PermissionId)
    SELECT @PermissionAdminRoleId, permission.Id
    FROM dbo.Permissions permission
    WHERE permission.Code IN (SELECT Code FROM @CarrierPermissions)
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.RolePermissions existing
          WHERE existing.RoleId = @PermissionAdminRoleId
            AND existing.PermissionId = permission.Id
      );
END;
GO

DECLARE @CatalogMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CATALOGS' AND IsDeleted = 0);

IF @CatalogMenuId IS NULL
BEGIN
    INSERT dbo.SecurityMenus (ParentId, Code, Name, Description, MenuType, FormKey, IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (NULL, N'MENU.CATALOGS', N'Catalogos', N'Maestros comerciales y contables', 1, NULL, N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg', 20, 1, 1, N'Sistema', SYSUTCDATETIME());
    SET @CatalogMenuId = CONVERT(int, SCOPE_IDENTITY());
END;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.CATALOGS.CARRIERS' AND IsDeleted = 0)
    INSERT dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'FORM.CATALOGS.CARRIERS', N'Transportistas', N'Mantenimiento independiente de transportistas', N'carriers', 1, 1, 1, N'Sistema', SYSUTCDATETIME());

UPDATE dbo.SecurityForms
SET Name = N'Transportistas', Description = N'Mantenimiento independiente de transportistas', FormKey = N'carriers', FormType = 1, IsVisible = 1, IsActive = 1
WHERE Code = N'FORM.CATALOGS.CARRIERS';

DECLARE @FormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.CATALOGS.CARRIERS' AND IsDeleted = 0);

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CATALOGS.CARRIERS' AND IsDeleted = 0)
    INSERT dbo.SecurityMenus (ParentId, Code, Name, Description, MenuType, FormId, FormKey, IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (@CatalogMenuId, N'MENU.CATALOGS.CARRIERS', N'Transportistas', N'Mantenimiento independiente de transportistas', 3, @FormId, N'carriers', N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg', 30, 1, 1, N'Sistema', SYSUTCDATETIME());

UPDATE dbo.SecurityMenus
SET ParentId = @CatalogMenuId, Name = N'Transportistas', Description = N'Mantenimiento independiente de transportistas', MenuType = 3,
    FormId = @FormId, FormKey = N'carriers', DisplayOrder = 30, IsVisible = 1, IsActive = 1
WHERE Code = N'MENU.CATALOGS.CARRIERS';

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @MenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CATALOGS.CARRIERS' AND IsDeleted = 0);

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, menu.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityMenus menu
    WHERE menu.Id IN (@CatalogMenuId, @MenuId) AND NOT EXISTS
    (SELECT 1 FROM dbo.SecurityRoleMenus existing WHERE existing.RoleId = @AdminRoleId AND existing.MenuId = menu.Id);

    INSERT dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, @FormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.IsDeleted = 0
      AND operation.IsActive = 1
      AND LOWER(LTRIM(RTRIM(operation.ActionKey))) IN
      (
          N'refresh', N'create', N'update', N'delete', N'consult', N'copy', N'history',
          N'customize-columns', N'customizecolumns',
          N'export-excel', N'exportexcel', N'export-pdf', N'exportpdf',
          N'export-json', N'exportjson', N'export-xml', N'exportxml'
      )
      AND NOT EXISTS
    (SELECT 1 FROM dbo.SecurityRoleFormOperations existing WHERE existing.RoleId = @AdminRoleId AND existing.FormId = @FormId AND existing.OperationId = operation.Id);
END;
GO
