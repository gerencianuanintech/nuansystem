/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos, formulario y menu para Catalogos Operativos.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'OPERATIONALCATALOGS')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'OPERATIONALCATALOGS', N'Catalogos operativos', 46);
END;
GO

DECLARE @ModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'OPERATIONALCATALOGS');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @Permissions table (Code nvarchar(120), Name nvarchar(160), Description nvarchar(300));
INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'OPERATIONALCATALOGS.READ', N'Ver catalogos operativos', N'Consultar catalogos operativos por empresa.'),
    (N'OPERATIONALCATALOGS.MANAGE', N'Gestionar catalogos operativos', N'Crear, editar y eliminar catalogos operativos por empresa.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @ModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @ModuleId IS NOT NULL
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

DECLARE @ConfigurationMenuId int;
DECLARE @CatalogsMenuId int;
DECLARE @FormId int;
DECLARE @MenuId int;
DECLARE @AdminRoleId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus (ParentId, Code, Name, Description, MenuType, FormKey, IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (NULL, N'MENU.CONFIGURATION', N'Modulo de configuracion', N'Configuracion general del sistema', 1, NULL, N'Accordion/configuracion_32.svg', N'Accordion/configuracion_16.svg', 10, 1, 1, N'Sistema', SYSUTCDATETIME());
END;

SET @ConfigurationMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION' AND IsDeleted = 0);

IF @ConfigurationMenuId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.CATALOGS' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus (ParentId, Code, Name, Description, MenuType, FormKey, IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (@ConfigurationMenuId, N'MENU.CONFIGURATION.CATALOGS', N'Catalogos', N'Catalogos de configuracion del sistema', 2, NULL, N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg', 10, 1, 1, N'Sistema', SYSUTCDATETIME());
END;

SET @CatalogsMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.CATALOGS' AND IsDeleted = 0);

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.OPERATIONALCATALOGS' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'FORM.OPERATIONALCATALOGS', N'Catalogos operativos', N'Mantenimiento de catalogos operativos por empresa', N'operational-catalogs', 1, 1, 1, N'Sistema', SYSUTCDATETIME());
END;

UPDATE dbo.SecurityForms
SET Name = N'Catalogos operativos',
    Description = N'Mantenimiento de catalogos operativos por empresa',
    FormKey = N'operational-catalogs',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.OPERATIONALCATALOGS';

SET @FormId = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.OPERATIONALCATALOGS' AND IsDeleted = 0);

IF @CatalogsMenuId IS NOT NULL AND @FormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.CATALOGS.OPERATIONAL' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus (ParentId, Code, Name, Description, MenuType, FormId, FormKey, IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive, CreatedByUserName, CreatedAt)
        VALUES (@CatalogsMenuId, N'MENU.CONFIGURATION.CATALOGS.OPERATIONAL', N'Catalogos operativos', N'Mantenimiento de catalogos operativos', 3, @FormId, N'operational-catalogs', N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg', 20, 1, 1, N'Sistema', SYSUTCDATETIME());
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @CatalogsMenuId,
        Name = N'Catalogos operativos',
        Description = N'Mantenimiento de catalogos operativos',
        MenuType = 3,
        FormId = @FormId,
        FormKey = N'operational-catalogs',
        DisplayOrder = 20,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.CONFIGURATION.CATALOGS.OPERATIONAL';
END;

SET @AdminRoleId = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
SET @MenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.CATALOGS.OPERATIONAL' AND IsDeleted = 0);

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, menu.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityMenus menu
    WHERE menu.Id IN (@ConfigurationMenuId, @CatalogsMenuId, @MenuId)
      AND menu.Id IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleMenus existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.MenuId = menu.Id
      );

    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, @FormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE @FormId IS NOT NULL
      AND operation.IsDeleted = 0
      AND operation.IsActive = 1
      AND operation.Code IN (N'refresh', N'create', N'update', N'delete', N'consult', N'history', N'customizecolumns', N'export')
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = @FormId
            AND existing.OperationId = operation.Id
      );
END;
GO
