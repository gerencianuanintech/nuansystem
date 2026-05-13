/*
    Ejecutar este script en NuanSystem_Master.
    Registra el mantenimiento de Items en menus, formularios y accesos base.
*/

DECLARE @InventoryMenuId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.INVENTORY')
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.INVENTORY', N'Inventario', N'Modulo de inventario',
        1, NULL, N'Accordion/inventario_32.svg', N'Accordion/inventario_16.svg',
        30, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'Inventario',
    Description = N'Modulo de inventario',
    MenuType = 1,
    IconLarge = N'Accordion/inventario_32.svg',
    IconSmall = N'Accordion/inventario_16.svg',
    DisplayOrder = 30,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.INVENTORY';

SET @InventoryMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.INVENTORY' AND IsDeleted = 0);

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.INVENTORY.ITEMS')
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.INVENTORY.ITEMS', N'Items', N'Mantenimiento de maestro de items',
        N'items', 1, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityForms
SET Name = N'Items',
    Description = N'Mantenimiento de maestro de items',
    FormKey = N'items',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.INVENTORY.ITEMS';

DECLARE @ItemsFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.INVENTORY.ITEMS' AND IsDeleted = 0);

IF @InventoryMenuId IS NOT NULL AND @ItemsFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.INVENTORY.ITEMS')
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @InventoryMenuId, N'MENU.INVENTORY.ITEMS', N'Items',
            N'Mantenimiento de maestro de items',
            3, @ItemsFormId, N'items',
            N'Accordion/productos_32.svg', N'Accordion/productos_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @InventoryMenuId,
        Name = N'Items',
        Description = N'Mantenimiento de maestro de items',
        MenuType = 3,
        FormId = @ItemsFormId,
        FormKey = N'items',
        IconLarge = N'Accordion/productos_32.svg',
        IconSmall = N'Accordion/productos_16.svg',
        DisplayOrder = 10,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.INVENTORY.ITEMS';
END;

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @ItemsMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.INVENTORY.ITEMS' AND IsDeleted = 0);

IF @AdminRoleId IS NOT NULL AND @InventoryMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleId AND MenuId = @InventoryMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, @InventoryMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleId IS NOT NULL AND @ItemsMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleId AND MenuId = @ItemsMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, @ItemsMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleId IS NOT NULL AND @ItemsFormId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, @ItemsFormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.IsDeleted = 0
      AND operation.IsActive = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = @ItemsFormId
            AND existing.OperationId = operation.Id
      );
END;
GO
