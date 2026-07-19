/* Ejecutar en NuanSystem_Master. Registra Transportistas como mantenimiento independiente. */
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
    WHERE operation.IsDeleted = 0 AND operation.IsActive = 1 AND NOT EXISTS
    (SELECT 1 FROM dbo.SecurityRoleFormOperations existing WHERE existing.RoleId = @AdminRoleId AND existing.FormId = @FormId AND existing.OperationId = operation.Id);
END;
GO
