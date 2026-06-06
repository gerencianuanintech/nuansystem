/*
    Ejecutar este script en NuanSystem_Master.
    Registra permisos, formulario y menu para el mantenimiento de Series de Documentos.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'DOCUMENTS')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'DOCUMENTS', N'Documentos', 45);
END;
GO

DECLARE @DocumentsModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'DOCUMENTS');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'DOCUMENTS.SERIES.READ', N'Ver series de documentos', N'Consultar series de documentos por empresa.'),
    (N'DOCUMENTS.SERIES.CREATE', N'Crear series de documentos', N'Crear nuevas series de documentos por empresa.'),
    (N'DOCUMENTS.SERIES.UPDATE', N'Editar series de documentos', N'Actualizar series de documentos por empresa.'),
    (N'DOCUMENTS.SERIES.DELETE', N'Eliminar series de documentos', N'Eliminar logicamente series de documentos por empresa.'),
    (N'DOCUMENTS.SERIES.MANAGE', N'Gestionar series de documentos', N'Gestion integral y reserva de numeracion de series de documentos.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @DocumentsModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @DocumentsModuleId IS NOT NULL
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

DECLARE @ConfigurationMenuId int;
DECLARE @CatalogsMenuId int;
DECLARE @DocumentSeriesFormId int;
DECLARE @DocumentSeriesMenuId int;
DECLARE @AdminRoleId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.CONFIGURATION', N'Modulo de configuracion', N'Configuracion general del sistema',
        1, NULL, N'Accordion/configuracion_32.svg', N'Accordion/configuracion_16.svg',
        10, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'Modulo de configuracion',
    Description = N'Configuracion general del sistema',
    MenuType = 1,
    FormKey = NULL,
    DisplayOrder = 10,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'MENU.CONFIGURATION';

SET @ConfigurationMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION' AND IsDeleted = 0);

IF @ConfigurationMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.CATALOGS' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @ConfigurationMenuId, N'MENU.CONFIGURATION.CATALOGS', N'Catalogos',
            N'Catalogos de configuracion del sistema', 2, NULL,
            N'Accordion/catalogos_32.svg', N'Accordion/catalogos_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @ConfigurationMenuId,
        Name = N'Catalogos',
        Description = N'Catalogos de configuracion del sistema',
        MenuType = 2,
        FormKey = NULL,
        DisplayOrder = 10,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.CONFIGURATION.CATALOGS';
END;

SET @CatalogsMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.CATALOGS' AND IsDeleted = 0);

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.DOCUMENTS.SERIES' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.DOCUMENTS.SERIES', N'Series de Documentos', N'Mantenimiento de series de documentos',
        N'security-document-series', 1, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityForms
SET Name = N'Series de Documentos',
    Description = N'Mantenimiento de series de documentos',
    FormKey = N'security-document-series',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.DOCUMENTS.SERIES';

SET @DocumentSeriesFormId = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.DOCUMENTS.SERIES' AND IsDeleted = 0);

IF @CatalogsMenuId IS NOT NULL AND @DocumentSeriesFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.CATALOGS.DOCUMENT_SERIES' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @CatalogsMenuId, N'MENU.CONFIGURATION.CATALOGS.DOCUMENT_SERIES', N'Documentos',
            N'Mantenimiento de series de documentos', 3, @DocumentSeriesFormId, N'security-document-series',
            N'Accordion/documentos_32.svg', N'Accordion/documentos_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @CatalogsMenuId,
        Name = N'Documentos',
        Description = N'Mantenimiento de series de documentos',
        MenuType = 3,
        FormId = @DocumentSeriesFormId,
        FormKey = N'security-document-series',
        DisplayOrder = 10,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.CONFIGURATION.CATALOGS.DOCUMENT_SERIES';
END;

SET @AdminRoleId = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
SET @DocumentSeriesMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.CATALOGS.DOCUMENT_SERIES' AND IsDeleted = 0);

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, menu.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityMenus menu
    WHERE menu.Id IN (@ConfigurationMenuId, @CatalogsMenuId, @DocumentSeriesMenuId)
      AND menu.Id IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleMenus existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.MenuId = menu.Id
      );

    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, @DocumentSeriesFormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE @DocumentSeriesFormId IS NOT NULL
      AND operation.IsDeleted = 0
      AND operation.IsActive = 1
      AND operation.Code IN
      (
          N'refresh',
          N'create',
          N'update',
          N'delete',
          N'consult',
          N'history',
          N'customizecolumns',
          N'export'
      )
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = @DocumentSeriesFormId
            AND existing.OperationId = operation.Id
      );
END;
GO
