IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SECURITY')
BEGIN
    INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
    VALUES (N'SECURITY', N'Seguridad', 10);
END;
GO

DECLARE @SecurityModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'SECURITY');
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @Permissions table
(
    Code nvarchar(120) NOT NULL,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NULL
);

INSERT INTO @Permissions (Code, Name, Description)
VALUES
    (N'SECURITY.FORMACCESS.MAINTENANCE.MANAGE', N'Gestionar accesos a formularios de mantenimiento', N'Permite configurar operaciones dinamicas por rol para formularios de mantenimiento.'),
    (N'SECURITY.FORMACCESS.TRANSACTIONAL.MANAGE', N'Gestionar accesos a formularios transaccionales', N'Permite configurar operaciones dinamicas por rol para formularios transaccionales.'),
    (N'SECURITY.DOCUMENTSERIESACCESS.MANAGE', N'Gestionar accesos por series de documentos', N'Permite configurar accesos por rol, documento, serie y operacion.'),
    (N'SECURITY.FIELDACCESS.MAINTENANCE.MANAGE', N'Gestionar accesos a campos de mantenimiento', N'Permite configurar campos visibles, editables, requeridos y de solo lectura por rol.'),
    (N'SECURITY.FIELDACCESS.TRANSACTIONAL.MANAGE', N'Gestionar accesos a campos transaccionales', N'Permite configurar campos transaccionales por rol, formulario, documento y serie.');

INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
SELECT @SecurityModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE @SecurityModuleId IS NOT NULL
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.Permissions permission
      WHERE permission.Code = source.Code
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
          FROM dbo.RolePermissions rolePermission
          WHERE rolePermission.RoleId = @AdminRoleId
            AND rolePermission.PermissionId = permission.Id
      );
END;
GO

DECLARE @SecurityMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY' AND IsDeleted = 0);
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.ACCESS' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES
    (
        N'FORM.SECURITY.ACCESS',
        N'Accesos a Formularios de Mantenimiento',
        N'Configuracion dinamica de operaciones por rol para formularios de mantenimiento.',
        N'security-access',
        1,
        1,
        1,
        N'Sistema',
        SYSUTCDATETIME()
    );
END
ELSE
BEGIN
    UPDATE dbo.SecurityForms
    SET Name = N'Accesos a Formularios de Mantenimiento',
        Description = N'Configuracion dinamica de operaciones por rol para formularios de mantenimiento.',
        FormKey = N'security-access',
        FormType = 1,
        IsVisible = 1,
        IsActive = 1,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Code = N'FORM.SECURITY.ACCESS'
      AND IsDeleted = 0;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FORMACCESS.TRANSACTIONAL' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES
    (
        N'FORM.SECURITY.FORMACCESS.TRANSACTIONAL',
        N'Accesos a Formularios Transaccionales',
        N'Configuracion dinamica de operaciones por rol para formularios transaccionales.',
        N'security-form-access-transactional',
        2,
        1,
        1,
        N'Sistema',
        SYSUTCDATETIME()
    );
END
ELSE
BEGIN
    UPDATE dbo.SecurityForms
    SET Name = N'Accesos a Formularios Transaccionales',
        Description = N'Configuracion dinamica de operaciones por rol para formularios transaccionales.',
        FormKey = N'security-form-access-transactional',
        FormType = 2,
        IsVisible = 1,
        IsActive = 1,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Code = N'FORM.SECURITY.FORMACCESS.TRANSACTIONAL'
      AND IsDeleted = 0;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FIELDACCESS.MAINTENANCE' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES
    (
        N'FORM.SECURITY.FIELDACCESS.MAINTENANCE',
        N'Accesos a Campos de Formularios de Mantenimiento',
        N'Configuracion dinamica de campos por rol para formularios de mantenimiento.',
        N'security-field-access-maintenance',
        1,
        0,
        1,
        N'Sistema',
        SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FIELDACCESS.TRANSACTIONAL' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES
    (
        N'FORM.SECURITY.FIELDACCESS.TRANSACTIONAL',
        N'Accesos a Campos de Formularios Transaccionales',
        N'Configuracion dinamica de campos por rol para formularios transaccionales.',
        N'security-field-access-transactional',
        2,
        0,
        1,
        N'Sistema',
        SYSUTCDATETIME()
    );
END;
GO

DECLARE @SecurityMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY' AND IsDeleted = 0);
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

DECLARE @AccessMenus table
(
    MenuCode nvarchar(80) NOT NULL,
    MenuName nvarchar(120) NOT NULL,
    MenuDescription nvarchar(300) NULL,
    FormCode nvarchar(80) NOT NULL,
    FormKey nvarchar(120) NOT NULL,
    DisplayOrder int NOT NULL,
    IsVisible bit NOT NULL
);

INSERT INTO @AccessMenus (MenuCode, MenuName, MenuDescription, FormCode, FormKey, DisplayOrder, IsVisible)
VALUES
    (N'MENU.SECURITY.ACCESS', N'Accesos', N'Accesos a formularios de mantenimiento por rol', N'FORM.SECURITY.ACCESS', N'security-access', 60, 1),
    (N'MENU.SECURITY.FORMACCESS.TRANSACTIONAL', N'Accesos transaccionales', N'Accesos a formularios transaccionales por rol', N'FORM.SECURITY.FORMACCESS.TRANSACTIONAL', N'security-form-access-transactional', 61, 1),
    (N'MENU.SECURITY.FIELDACCESS.MAINTENANCE', N'Campos mantenimiento', N'Accesos a campos de formularios de mantenimiento', N'FORM.SECURITY.FIELDACCESS.MAINTENANCE', N'security-field-access-maintenance', 62, 0),
    (N'MENU.SECURITY.FIELDACCESS.TRANSACTIONAL', N'Campos transaccionales', N'Accesos a campos de formularios transaccionales', N'FORM.SECURITY.FIELDACCESS.TRANSACTIONAL', N'security-field-access-transactional', 63, 0);

MERGE dbo.SecurityMenus AS target
USING
(
    SELECT
        source.MenuCode,
        source.MenuName,
        source.MenuDescription,
        form.Id AS FormId,
        source.FormKey,
        source.DisplayOrder,
        source.IsVisible
    FROM @AccessMenus source
    INNER JOIN dbo.SecurityForms form ON form.Code = source.FormCode
        AND form.IsDeleted = 0
    WHERE @SecurityMenuId IS NOT NULL
) AS source
    ON target.Code = source.MenuCode
WHEN MATCHED THEN
    UPDATE SET
        ParentId = @SecurityMenuId,
        Name = source.MenuName,
        Description = source.MenuDescription,
        MenuType = 3,
        FormId = source.FormId,
        FormKey = source.FormKey,
        DisplayOrder = source.DisplayOrder,
        IsVisible = source.IsVisible,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME(),
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL
WHEN NOT MATCHED THEN
    INSERT
    (
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @SecurityMenuId, source.MenuCode, source.MenuName, source.MenuDescription,
        3, source.FormId, source.FormKey,
        N'Security_32x32.svg', N'Security_16x16.svg', source.DisplayOrder,
        source.IsVisible, 1, N'Sistema', SYSUTCDATETIME()
    );

IF @AdminRoleId IS NOT NULL
BEGIN
    MERGE dbo.SecurityRoleMenus AS target
    USING
    (
        SELECT menu.Id AS MenuId
        FROM dbo.SecurityMenus menu
        WHERE menu.Code IN (SELECT MenuCode FROM @AccessMenus)
          AND menu.IsDeleted = 0
    ) AS source
        ON target.RoleId = @AdminRoleId
       AND target.MenuId = source.MenuId
    WHEN MATCHED THEN
        UPDATE SET
            IsAllowed = 1,
            IsDeleted = 0,
            UpdatedByUserName = N'Sistema',
            UpdatedAt = SYSUTCDATETIME(),
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL
    WHEN NOT MATCHED THEN
        INSERT (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, source.MenuId, 1, N'Sistema', SYSUTCDATETIME());

    MERGE dbo.SecurityRoleFormOperations AS target
    USING
    (
        SELECT form.Id AS FormId, operation.Id AS OperationId
        FROM dbo.SecurityForms form
        CROSS JOIN dbo.SecurityOperations operation
        WHERE form.Code IN
        (
            N'FORM.SECURITY.ACCESS',
            N'FORM.SECURITY.FORMACCESS.TRANSACTIONAL',
            N'FORM.SECURITY.FIELDACCESS.MAINTENANCE',
            N'FORM.SECURITY.FIELDACCESS.TRANSACTIONAL'
        )
          AND form.IsDeleted = 0
          AND operation.IsDeleted = 0
          AND operation.IsActive = 1
    ) AS source
        ON target.RoleId = @AdminRoleId
       AND target.FormId = source.FormId
       AND target.OperationId = source.OperationId
    WHEN MATCHED THEN
        UPDATE SET
            IsAllowed = 1,
            IsDeleted = 0,
            UpdatedByUserName = N'Sistema',
            UpdatedAt = SYSUTCDATETIME(),
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL
    WHEN NOT MATCHED THEN
        INSERT (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, source.FormId, source.OperationId, 1, N'Sistema', SYSUTCDATETIME());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMACCESS_FORMULARIOS
    @FormType int = NULL,
    @OnlyActive bit = 1,
    @Search nvarchar(120) = NULL
AS
BEGIN
    SELECT
        form.Id,
        form.Code,
        form.Name,
        form.Description,
        form.FormKey,
        CAST(form.FormType AS int) AS FormType,
        CASE form.FormType
            WHEN 1 THEN N'Mantenimiento'
            WHEN 2 THEN N'Transaccional'
            WHEN 3 THEN N'Reporte'
            WHEN 4 THEN N'Dialogo'
            WHEN 5 THEN N'Proceso'
            ELSE N'Desconocido'
        END AS FormTypeName,
        form.IsVisible,
        form.IsActive
    FROM dbo.SecurityForms form
    WHERE form.IsDeleted = 0
      AND (@FormType IS NULL OR form.FormType = @FormType)
      AND (@OnlyActive = 0 OR form.IsActive = 1)
      AND
      (
          @Search IS NULL
          OR @Search = N''
          OR form.Code LIKE N'%' + @Search + N'%'
          OR form.Name LIKE N'%' + @Search + N'%'
          OR form.FormKey LIKE N'%' + @Search + N'%'
      )
    ORDER BY form.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMACCESS_OPERACIONES
    @RoleId int,
    @FormId int,
    @OnlyActive bit = 1,
    @Search nvarchar(120) = NULL
AS
BEGIN
    SELECT
        form.Id AS FormId,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.FormKey,
        operation.Id AS OperationId,
        operation.Code AS OperationCode,
        operation.Name AS OperationName,
        operation.Description AS OperationDescription,
        operation.ActionKey,
        operation.RibbonPageName,
        operation.RibbonGroupName,
        operation.IconLarge,
        operation.IconSmall,
        operation.DisplayOrder,
        CAST(CASE WHEN roleOperation.IsAllowed = 1 THEN 1 ELSE 0 END AS bit) AS IsAllowed,
        roleOperation.UpdatedByUserId,
        roleOperation.UpdatedByUserName,
        roleOperation.UpdatedAt,
        roleOperation.CreatedByUserId,
        roleOperation.CreatedByUserName,
        roleOperation.CreatedAt
    FROM dbo.SecurityForms form
    CROSS JOIN dbo.SecurityOperations operation
    LEFT JOIN dbo.SecurityRoleFormOperations roleOperation ON roleOperation.FormId = form.Id
        AND roleOperation.OperationId = operation.Id
        AND roleOperation.RoleId = @RoleId
        AND roleOperation.IsDeleted = 0
    WHERE form.Id = @FormId
      AND form.IsDeleted = 0
      AND operation.IsDeleted = 0
      AND (@OnlyActive = 0 OR operation.IsActive = 1)
      AND
      (
          @Search IS NULL
          OR @Search = N''
          OR operation.Code LIKE N'%' + @Search + N'%'
          OR operation.Name LIKE N'%' + @Search + N'%'
          OR operation.ActionKey LIKE N'%' + @Search + N'%'
      )
    ORDER BY operation.DisplayOrder, operation.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SECURITYROLEFORMACCESS_GUARDAR
    @RoleId int,
    @FormId int,
    @OperationsJson nvarchar(max),
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @Operations table
    (
        OperationId int NOT NULL PRIMARY KEY,
        IsAllowed bit NOT NULL
    );

    INSERT INTO @Operations (OperationId, IsAllowed)
    SELECT OperationId, IsAllowed
    FROM OPENJSON(@OperationsJson)
    WITH
    (
        OperationId int '$.operationId',
        IsAllowed bit '$.isAllowed'
    )
    WHERE OperationId IS NOT NULL;

    MERGE dbo.SecurityRoleFormOperations AS target
    USING @Operations AS source
        ON target.RoleId = @RoleId
       AND target.FormId = @FormId
       AND target.OperationId = source.OperationId
    WHEN MATCHED THEN
        UPDATE SET
            IsAllowed = source.IsAllowed,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (RoleId, FormId, OperationId, IsAllowed, CreatedByUserId, CreatedByUserName, CreatedAt)
        VALUES (@RoleId, @FormId, source.OperationId, source.IsAllowed, @UpdatedByUserId, @UpdatedByUserName, SYSUTCDATETIME());

    COMMIT TRANSACTION;

    SELECT CAST(1 AS bit);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMACCESS_VALIDAR
    @RoleId int,
    @FormKey nvarchar(120),
    @ActionKey nvarchar(120)
AS
BEGIN
    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.SecurityForms form
        INNER JOIN dbo.SecurityRoleFormOperations roleOperation ON roleOperation.FormId = form.Id
            AND roleOperation.RoleId = @RoleId
            AND roleOperation.IsDeleted = 0
            AND roleOperation.IsAllowed = 1
        INNER JOIN dbo.SecurityOperations operation ON operation.Id = roleOperation.OperationId
            AND operation.IsDeleted = 0
            AND operation.IsActive = 1
        WHERE form.FormKey = @FormKey
          AND form.IsDeleted = 0
          AND form.IsActive = 1
          AND operation.ActionKey = @ActionKey
    ) THEN 1 ELSE 0 END AS bit) AS IsAllowed;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMACCESS_USUARIO
    @UserId int,
    @FormKey nvarchar(120),
    @ActionKey nvarchar(120)
AS
BEGIN
    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.UserRoles userRole
        INNER JOIN dbo.SecurityForms form ON form.FormKey = @FormKey
            AND form.IsDeleted = 0
            AND form.IsActive = 1
        INNER JOIN dbo.SecurityRoleFormOperations roleOperation ON roleOperation.FormId = form.Id
            AND roleOperation.RoleId = userRole.RoleId
            AND roleOperation.IsDeleted = 0
            AND roleOperation.IsAllowed = 1
        INNER JOIN dbo.SecurityOperations operation ON operation.Id = roleOperation.OperationId
            AND operation.IsDeleted = 0
            AND operation.IsActive = 1
            AND operation.ActionKey = @ActionKey
        WHERE userRole.UserId = @UserId
    ) THEN 1 ELSE 0 END AS bit) AS IsAllowed;
END;
GO
