/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos granulares para el Maestro de Items / Articulos.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'CATALOG')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'CATALOG', N'Catalogos', 20);
END;
GO

DECLARE @CatalogModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'CATALOG');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'CATALOG.ITEMS.CREATE', N'Crear items', N'Crear articulos en el maestro de items.'),
    (N'CATALOG.ITEMS.UPDATE', N'Editar items', N'Editar datos generales del maestro de items.'),
    (N'CATALOG.ITEMS.DELETE', N'Eliminar items', N'Eliminar logicamente articulos del maestro de items.'),
    (N'CATALOG.ITEMS.ACTIVATE', N'Activar items', N'Activar o inactivar articulos.'),
    (N'CATALOG.ITEMS.EXPORT', N'Exportar items', N'Exportar informacion del maestro de items.'),
    (N'CATALOG.ITEMS.CONFIGURE_INVENTORY', N'Configurar inventario de items', N'Editar unidades, bodegas, lotes, series y reglas de inventario.'),
    (N'CATALOG.ITEMS.CONFIGURE_PRICES', N'Configurar precios de items', N'Editar ventas, costos, listas de precio y margenes.'),
    (N'CATALOG.ITEMS.CONFIGURE_ACCOUNTING', N'Configurar contabilidad de items', N'Editar cuentas contables y dimensiones del item.'),
    (N'CATALOG.ITEMS.CONFIGURE_TAXES', N'Configurar impuestos de items', N'Editar reglas tributarias del item.'),
    (N'CATALOG.ITEMS.CONFIGURE_SAP', N'Configurar SAP de items', N'Editar configuracion de sincronizacion SAP del item.'),
    (N'CATALOG.ITEMS.MANAGE_ATTACHMENTS', N'Gestionar anexos de items', N'Cargar, descargar, abrir y marcar imagenes o anexos del item.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @CatalogModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @CatalogModuleId IS NOT NULL
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
