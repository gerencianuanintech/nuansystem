/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos backend para maestros auxiliares de proveedores.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'GENERALSUPPLIER')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'GENERALSUPPLIER', N'General Proveedores', 45);
END;
GO

DECLARE @GeneralSupplierModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'GENERALSUPPLIER');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'GENERALSUPPLIER.SUPPLIERGROUPS.READ', N'Ver grupos de proveedor', N'Consultar grupos de proveedor.'),
    (N'GENERALSUPPLIER.SUPPLIERGROUPS.MANAGE', N'Gestionar grupos de proveedor', N'Crear, editar y eliminar grupos de proveedor.'),
    (N'GENERALSUPPLIER.SUPPLIERCLASSES.READ', N'Ver clases de proveedor', N'Consultar clases de proveedor.'),
    (N'GENERALSUPPLIER.SUPPLIERCLASSES.MANAGE', N'Gestionar clases de proveedor', N'Crear, editar y eliminar clases de proveedor.'),
    (N'GENERALSUPPLIER.ECONOMICACTIVITIES.READ', N'Ver actividades economicas', N'Consultar actividades economicas de proveedor.'),
    (N'GENERALSUPPLIER.ECONOMICACTIVITIES.MANAGE', N'Gestionar actividades economicas', N'Crear, editar y eliminar actividades economicas de proveedor.'),
    (N'GENERALSUPPLIER.ZONES.READ', N'Ver zonas de proveedor', N'Consultar zonas de proveedor.'),
    (N'GENERALSUPPLIER.ZONES.MANAGE', N'Gestionar zonas de proveedor', N'Crear, editar y eliminar zonas de proveedor.'),
    (N'GENERALSUPPLIER.SUPPLYMETHODS.READ', N'Ver formas de abastecimiento', N'Consultar formas de abastecimiento de proveedor.'),
    (N'GENERALSUPPLIER.SUPPLYMETHODS.MANAGE', N'Gestionar formas de abastecimiento', N'Crear, editar y eliminar formas de abastecimiento.'),
    (N'GENERALSUPPLIER.CONTACTTYPES.READ', N'Ver tipos de contacto', N'Consultar tipos de contacto de proveedor.'),
    (N'GENERALSUPPLIER.CONTACTTYPES.MANAGE', N'Gestionar tipos de contacto', N'Crear, editar y eliminar tipos de contacto.'),
    (N'GENERALSUPPLIER.CONTACTCHANNELS.READ', N'Ver canales de contacto', N'Consultar canales de contacto de proveedor.'),
    (N'GENERALSUPPLIER.CONTACTCHANNELS.MANAGE', N'Gestionar canales de contacto', N'Crear, editar y eliminar canales de contacto.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @GeneralSupplierModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @GeneralSupplierModuleId IS NOT NULL
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

DECLARE @GeneralSupplierMenuId int;
DECLARE @AdminRoleId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.GENERALSUPPLIER' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.GENERALSUPPLIER', N'General Proveedores', N'Maestros auxiliares de proveedores',
        1, NULL, N'Accordion/proveedores_32.svg', N'Accordion/proveedores_16.svg',
        25, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'General Proveedores',
    Description = N'Maestros auxiliares de proveedores',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 25,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.GENERALSUPPLIER';

SET @GeneralSupplierMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.GENERALSUPPLIER' AND IsDeleted = 0);

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
    (N'FORM.GENERALSUPPLIER.SUPPLIERGROUPS', N'Grupos de proveedor', N'Clasificacion principal de proveedores', N'supplier-groups', N'MENU.GENERALSUPPLIER.SUPPLIERGROUPS', 10),
    (N'FORM.GENERALSUPPLIER.SUPPLIERCLASSES', N'Clases de proveedor', N'Clasificacion operativa de proveedores', N'supplier-classes', N'MENU.GENERALSUPPLIER.SUPPLIERCLASSES', 20),
    (N'FORM.GENERALSUPPLIER.ECONOMICACTIVITIES', N'Actividades economicas', N'Actividades economicas de proveedores', N'economic-activities', N'MENU.GENERALSUPPLIER.ECONOMICACTIVITIES', 30),
    (N'FORM.GENERALSUPPLIER.ZONES', N'Zonas de proveedor', N'Zonas comerciales de proveedores', N'supplier-zones', N'MENU.GENERALSUPPLIER.ZONES', 40),
    (N'FORM.GENERALSUPPLIER.SUPPLYMETHODS', N'Formas de abastecimiento', N'Metodos de abastecimiento de proveedores', N'supply-methods', N'MENU.GENERALSUPPLIER.SUPPLYMETHODS', 50),
    (N'FORM.GENERALSUPPLIER.CONTACTTYPES', N'Tipos de contacto', N'Tipos de contacto para proveedores', N'supplier-contact-types', N'MENU.GENERALSUPPLIER.CONTACTTYPES', 60),
    (N'FORM.GENERALSUPPLIER.CONTACTCHANNELS', N'Canales de contacto', N'Canales de contacto para proveedores', N'supplier-contact-channels', N'MENU.GENERALSUPPLIER.CONTACTCHANNELS', 70);

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

IF @GeneralSupplierMenuId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    SELECT
        @GeneralSupplierMenuId,
        source.MenuCode,
        source.Name,
        source.Description,
        3,
        formItem.Id,
        source.FormKey,
        N'Accordion/proveedores_32.svg',
        N'Accordion/proveedores_16.svg',
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
    SET ParentId = @GeneralSupplierMenuId,
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
    WHERE (menu.Code = N'MENU.GENERALSUPPLIER'
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
