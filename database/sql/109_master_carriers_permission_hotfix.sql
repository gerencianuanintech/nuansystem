/*
    Ejecutar en NuanSystem_Master despues de 108_master_carriers_security.sql.
    Repara instalaciones donde el menu y las operaciones de Transportistas fueron
    creados antes que sus permisos de API.
*/
IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'CATALOG')
BEGIN
    INSERT dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'CATALOG', N'Catalogos', 20);
END;
GO

DECLARE @CatalogModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'CATALOG');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

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

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT dbo.RolePermissions (RoleId, PermissionId)
    SELECT @AdminRoleId, permission.Id
    FROM dbo.Permissions permission
    WHERE permission.Code IN (SELECT Code FROM @CarrierPermissions)
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.RolePermissions existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.PermissionId = permission.Id
      );
END;
GO

