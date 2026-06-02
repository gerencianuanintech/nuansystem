/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos, menus y formularios independientes para maestros auxiliares de inventario/items.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'GENERALINVENTORY')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'GENERALINVENTORY', N'General Inventario', 47);
END;
GO

DECLARE @GeneralInventoryModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'GENERALINVENTORY');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'GENERALINVENTORY.UNITMEASURES.READ', N'Ver unidades de medida', N'Consultar unidades de medida de inventario.'),
    (N'GENERALINVENTORY.UNITMEASURES.MANAGE', N'Gestionar unidades de medida', N'Crear, editar y eliminar unidades de medida.'),
    (N'GENERALINVENTORY.WAREHOUSES.READ', N'Ver bodegas', N'Consultar bodegas.'),
    (N'GENERALINVENTORY.WAREHOUSES.MANAGE', N'Gestionar bodegas', N'Crear, editar y eliminar bodegas.'),
    (N'GENERALINVENTORY.ITEMBRANDS.READ', N'Ver marcas de articulos', N'Consultar marcas de articulos.'),
    (N'GENERALINVENTORY.ITEMBRANDS.MANAGE', N'Gestionar marcas de articulos', N'Crear, editar y eliminar marcas de articulos.'),
    (N'GENERALINVENTORY.ITEMTYPES.READ', N'Ver tipos de item', N'Consultar tipos de item.'),
    (N'GENERALINVENTORY.ITEMTYPES.MANAGE', N'Gestionar tipos de item', N'Crear, editar y eliminar tipos de item.'),
    (N'GENERALINVENTORY.PRODUCTTYPES.READ', N'Ver tipos de producto', N'Consultar tipos de producto.'),
    (N'GENERALINVENTORY.PRODUCTTYPES.MANAGE', N'Gestionar tipos de producto', N'Crear, editar y eliminar tipos de producto.'),
    (N'GENERALINVENTORY.ITEMLINES.READ', N'Ver lineas de articulos', N'Consultar lineas de articulos.'),
    (N'GENERALINVENTORY.ITEMLINES.MANAGE', N'Gestionar lineas de articulos', N'Crear, editar y eliminar lineas de articulos.'),
    (N'GENERALINVENTORY.ITEMSUBGROUPS.READ', N'Ver subgrupos de articulos', N'Consultar subgrupos de articulos.'),
    (N'GENERALINVENTORY.ITEMSUBGROUPS.MANAGE', N'Gestionar subgrupos de articulos', N'Crear, editar y eliminar subgrupos de articulos.'),
    (N'GENERALINVENTORY.SALESCHANNELS.READ', N'Ver canales de venta', N'Consultar canales de venta.'),
    (N'GENERALINVENTORY.SALESCHANNELS.MANAGE', N'Gestionar canales de venta', N'Crear, editar y eliminar canales de venta.'),
    (N'GENERALINVENTORY.WAREHOUSELOCATIONS.READ', N'Ver ubicaciones de bodega', N'Consultar ubicaciones de bodega.'),
    (N'GENERALINVENTORY.WAREHOUSELOCATIONS.MANAGE', N'Gestionar ubicaciones de bodega', N'Crear, editar y eliminar ubicaciones de bodega.'),
    (N'GENERALINVENTORY.STORAGEZONES.READ', N'Ver zonas de almacenamiento', N'Consultar zonas de almacenamiento.'),
    (N'GENERALINVENTORY.STORAGEZONES.MANAGE', N'Gestionar zonas de almacenamiento', N'Crear, editar y eliminar zonas de almacenamiento.'),
    (N'GENERALINVENTORY.STORAGECONDITIONS.READ', N'Ver condiciones de almacenamiento', N'Consultar condiciones de almacenamiento.'),
    (N'GENERALINVENTORY.STORAGECONDITIONS.MANAGE', N'Gestionar condiciones de almacenamiento', N'Crear, editar y eliminar condiciones de almacenamiento.'),
    (N'GENERALINVENTORY.REPLENISHMENTMETHODS.READ', N'Ver metodos de reposicion', N'Consultar metodos de reposicion.'),
    (N'GENERALINVENTORY.REPLENISHMENTMETHODS.MANAGE', N'Gestionar metodos de reposicion', N'Crear, editar y eliminar metodos de reposicion.'),
    (N'GENERALINVENTORY.VARIANTATTRIBUTES.READ', N'Ver atributos de variantes', N'Consultar atributos de variantes.'),
    (N'GENERALINVENTORY.VARIANTATTRIBUTES.MANAGE', N'Gestionar atributos de variantes', N'Crear, editar y eliminar atributos de variantes.'),
    (N'GENERALINVENTORY.ATTACHMENTDOCUMENTTYPES.READ', N'Ver tipos de documento de anexos', N'Consultar tipos de documento de anexos.'),
    (N'GENERALINVENTORY.ATTACHMENTDOCUMENTTYPES.MANAGE', N'Gestionar tipos de documento de anexos', N'Crear, editar y eliminar tipos de documento de anexos.'),
    (N'GENERALINVENTORY.ATTACHMENTCATEGORIES.READ', N'Ver categorias de anexos', N'Consultar categorias de anexos.'),
    (N'GENERALINVENTORY.ATTACHMENTCATEGORIES.MANAGE', N'Gestionar categorias de anexos', N'Crear, editar y eliminar categorias de anexos.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @GeneralInventoryModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @GeneralInventoryModuleId IS NOT NULL
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

DECLARE @GeneralInventoryMenuId int;
DECLARE @AdminRoleId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.GENERALINVENTORY' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.GENERALINVENTORY', N'General Inventario', N'Maestros auxiliares de inventario e items',
        1, NULL, N'Accordion/inventario_32.svg', N'Accordion/inventario_16.svg',
        27, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'General Inventario',
    Description = N'Maestros auxiliares de inventario e items',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 27,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.GENERALINVENTORY';

SET @GeneralInventoryMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.GENERALINVENTORY' AND IsDeleted = 0);

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
    (N'FORM.GENERALINVENTORY.UNITMEASURES', N'Unidades de medida', N'Mantenimiento de unidades de medida', N'inventory-unit-measures', N'MENU.GENERALINVENTORY.UNITMEASURES', 10),
    (N'FORM.GENERALINVENTORY.WAREHOUSES', N'Bodegas', N'Mantenimiento de bodegas', N'inventory-warehouses', N'MENU.GENERALINVENTORY.WAREHOUSES', 20),
    (N'FORM.GENERALINVENTORY.ITEMBRANDS', N'Marcas de articulos', N'Mantenimiento de marcas de articulos', N'inventory-item-brands', N'MENU.GENERALINVENTORY.ITEMBRANDS', 30),
    (N'FORM.GENERALINVENTORY.ITEMTYPES', N'Tipos de item', N'Mantenimiento de tipos de item', N'inventory-item-types', N'MENU.GENERALINVENTORY.ITEMTYPES', 40),
    (N'FORM.GENERALINVENTORY.PRODUCTTYPES', N'Tipos de producto', N'Mantenimiento de tipos de producto', N'inventory-product-types', N'MENU.GENERALINVENTORY.PRODUCTTYPES', 50),
    (N'FORM.GENERALINVENTORY.ITEMLINES', N'Lineas de articulos', N'Mantenimiento de lineas de articulos', N'inventory-item-lines', N'MENU.GENERALINVENTORY.ITEMLINES', 60),
    (N'FORM.GENERALINVENTORY.ITEMSUBGROUPS', N'Subgrupos de articulos', N'Mantenimiento de subgrupos de articulos', N'inventory-item-subgroups', N'MENU.GENERALINVENTORY.ITEMSUBGROUPS', 70),
    (N'FORM.GENERALINVENTORY.SALESCHANNELS', N'Canales de venta', N'Mantenimiento de canales de venta', N'inventory-sales-channels', N'MENU.GENERALINVENTORY.SALESCHANNELS', 80),
    (N'FORM.GENERALINVENTORY.WAREHOUSELOCATIONS', N'Ubicaciones de bodega', N'Mantenimiento de ubicaciones de bodega', N'inventory-warehouse-locations', N'MENU.GENERALINVENTORY.WAREHOUSELOCATIONS', 90),
    (N'FORM.GENERALINVENTORY.STORAGEZONES', N'Zonas de almacenamiento', N'Mantenimiento de zonas de almacenamiento', N'inventory-storage-zones', N'MENU.GENERALINVENTORY.STORAGEZONES', 100),
    (N'FORM.GENERALINVENTORY.STORAGECONDITIONS', N'Condiciones de almacenamiento', N'Mantenimiento de condiciones de almacenamiento', N'inventory-storage-conditions', N'MENU.GENERALINVENTORY.STORAGECONDITIONS', 110),
    (N'FORM.GENERALINVENTORY.REPLENISHMENTMETHODS', N'Metodos de reposicion', N'Mantenimiento de metodos de reposicion', N'inventory-replenishment-methods', N'MENU.GENERALINVENTORY.REPLENISHMENTMETHODS', 120),
    (N'FORM.GENERALINVENTORY.VARIANTATTRIBUTES', N'Atributos de variantes', N'Mantenimiento de atributos de variantes', N'inventory-variant-attributes', N'MENU.GENERALINVENTORY.VARIANTATTRIBUTES', 130),
    (N'FORM.GENERALINVENTORY.ATTACHMENTDOCUMENTTYPES', N'Tipos de documento de anexos', N'Mantenimiento de tipos de documento de anexos', N'inventory-attachment-document-types', N'MENU.GENERALINVENTORY.ATTACHMENTDOCUMENTTYPES', 140),
    (N'FORM.GENERALINVENTORY.ATTACHMENTCATEGORIES', N'Categorias de anexos', N'Mantenimiento de categorias de anexos', N'inventory-attachment-categories', N'MENU.GENERALINVENTORY.ATTACHMENTCATEGORIES', 150);

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

IF @GeneralInventoryMenuId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    SELECT
        @GeneralInventoryMenuId,
        source.MenuCode,
        source.Name,
        source.Description,
        3,
        formItem.Id,
        source.FormKey,
        N'Accordion/inventario_32.svg',
        N'Accordion/inventario_16.svg',
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
    SET ParentId = @GeneralInventoryMenuId,
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
    WHERE (menu.Code = N'MENU.GENERALINVENTORY'
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
