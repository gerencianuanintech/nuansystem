/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos, formulario y menu para Ordenes de Compra.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'PURCHASING')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'PURCHASING', N'Compras', 55);
END;
GO

DECLARE @PurchasingModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'PURCHASING');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'PURCHASING.PURCHASEORDERS.READ', N'Ver ordenes de compra', N'Consultar ordenes de compra.'),
    (N'PURCHASING.PURCHASEORDERS.MANAGE', N'Gestionar ordenes de compra', N'Crear, editar y eliminar ordenes de compra.'),
    (N'PURCHASING.PURCHASEORDERS.APPROVE', N'Aprobar ordenes de compra', N'Enviar, aprobar y rechazar ordenes de compra.'),
    (N'PURCHASING.PURCHASEORDERS.SYNC_SAP', N'Sincronizar ordenes de compra con SAP', N'Enviar ordenes de compra aprobadas a SAP Business One.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @PurchasingModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @PurchasingModuleId IS NOT NULL
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

DECLARE @PurchasingMenuId int;
DECLARE @PurchaseOrdersFormId int;
DECLARE @PurchaseOrdersMenuId int;
DECLARE @AdminRoleId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.PURCHASING' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.PURCHASING', N'Compras', N'Procesos de abastecimiento y compras',
        1, NULL, N'Accordion/compras_32.svg', N'Accordion/compras_16.svg',
        35, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'Compras',
    Description = N'Procesos de abastecimiento y compras',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 35,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.PURCHASING';

SET @PurchasingMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.PURCHASING' AND IsDeleted = 0);

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.PURCHASING.PURCHASEORDERS' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'FORM.PURCHASING.PURCHASEORDERS', N'Ordenes de compra', N'Gestion de ordenes de compra', N'purchase-orders', 2, 1, 1, N'Sistema', SYSUTCDATETIME());
END;

UPDATE dbo.SecurityForms
SET Name = N'Ordenes de compra',
    Description = N'Gestion de ordenes de compra',
    FormKey = N'purchase-orders',
    FormType = 2,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.PURCHASING.PURCHASEORDERS';

SET @PurchaseOrdersFormId = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.PURCHASING.PURCHASEORDERS' AND IsDeleted = 0);

IF @PurchasingMenuId IS NOT NULL AND @PurchaseOrdersFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.PURCHASING.PURCHASEORDERS' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @PurchasingMenuId, N'MENU.PURCHASING.PURCHASEORDERS', N'Ordenes de compra',
            N'Gestion de ordenes de compra', 3, @PurchaseOrdersFormId, N'purchase-orders',
            N'Accordion/compras_32.svg', N'Accordion/compras_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @PurchasingMenuId,
        Name = N'Ordenes de compra',
        Description = N'Gestion de ordenes de compra',
        MenuType = 3,
        FormId = @PurchaseOrdersFormId,
        FormKey = N'purchase-orders',
        DisplayOrder = 10,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.PURCHASING.PURCHASEORDERS';
END;

SET @AdminRoleId = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
SET @PurchaseOrdersMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.PURCHASING.PURCHASEORDERS' AND IsDeleted = 0);

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, menu.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityMenus menu
    WHERE menu.Id IN (@PurchasingMenuId, @PurchaseOrdersMenuId)
      AND menu.Id IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleMenus existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.MenuId = menu.Id
      );

    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, @PurchaseOrdersFormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE @PurchaseOrdersFormId IS NOT NULL
      AND operation.IsDeleted = 0
      AND operation.IsActive = 1
      AND operation.Code IN
      (
          N'refresh',
          N'create',
          N'update',
          N'delete',
          N'consult',
          N'approve',
          N'syncsap',
          N'customizecolumns',
          N'export'
      )
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = @PurchaseOrdersFormId
            AND existing.OperationId = operation.Id
      );
END;
GO
