IF COL_LENGTH('dbo.Roles', 'DisplayOrder') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD DisplayOrder int NOT NULL CONSTRAINT DF_Roles_DisplayOrder DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Roles', 'IsSystemRole') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD IsSystemRole bit NOT NULL CONSTRAINT DF_Roles_IsSystemRole DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Roles', 'IsAssignable') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD IsAssignable bit NOT NULL CONSTRAINT DF_Roles_IsAssignable DEFAULT 1;
END;
GO

IF COL_LENGTH('dbo.Roles', 'CreatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD CreatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.Roles', 'CreatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD CreatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Roles', 'UpdatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD UpdatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.Roles', 'UpdatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD UpdatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Roles', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD IsDeleted bit NOT NULL CONSTRAINT DF_Roles_IsDeleted DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Roles', 'DeletedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD DeletedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.Roles', 'DeletedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD DeletedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Roles', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Roles ADD DeletedAt datetime2(0) NULL;
END;
GO

UPDATE dbo.Roles
SET IsSystemRole = 1,
    IsAssignable = 1
WHERE Code = N'ADMIN';
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Roles_IsDeleted_DisplayOrder' AND object_id = OBJECT_ID(N'dbo.Roles'))
BEGIN
    CREATE INDEX IX_Roles_IsDeleted_DisplayOrder ON dbo.Roles (IsDeleted, DisplayOrder, Name);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Roles_Code_Active' AND object_id = OBJECT_ID(N'dbo.Roles'))
BEGIN
    CREATE UNIQUE INDEX UX_Roles_Code_Active ON dbo.Roles (Code) WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Roles_Name_Active' AND object_id = OBJECT_ID(N'dbo.Roles'))
BEGIN
    CREATE UNIQUE INDEX UX_Roles_Name_Active ON dbo.Roles (Name) WHERE IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ROLSEGURIDADLISTAR
AS
BEGIN
    SELECT
        r.Id,
        r.Code,
        r.Name,
        r.Description,
        r.DisplayOrder,
        r.IsSystemRole,
        r.IsAssignable,
        r.IsActive,
        COALESCE(permissionInfo.Permissions, N'') AS PermissionsText,
        r.CreatedByUserId,
        r.CreatedByUserName,
        r.CreatedAt,
        r.UpdatedByUserId,
        r.UpdatedByUserName,
        r.UpdatedAt,
        r.DeletedByUserId,
        r.DeletedByUserName,
        r.DeletedAt
    FROM dbo.Roles r
    OUTER APPLY
    (
        SELECT STRING_AGG(CONVERT(nvarchar(max), p.Code), N',') WITHIN GROUP (ORDER BY p.Code) AS Permissions
        FROM dbo.RolePermissions rp
        INNER JOIN dbo.Permissions p ON p.Id = rp.PermissionId
        WHERE rp.RoleId = r.Id
    ) permissionInfo
    WHERE r.IsDeleted = 0
    ORDER BY r.DisplayOrder, r.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ROLSEGURIDADBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        r.Id,
        r.Code,
        r.Name,
        r.Description,
        r.DisplayOrder,
        r.IsSystemRole,
        r.IsAssignable,
        r.IsActive,
        COALESCE(permissionInfo.Permissions, N'') AS PermissionsText,
        r.CreatedByUserId,
        r.CreatedByUserName,
        r.CreatedAt,
        r.UpdatedByUserId,
        r.UpdatedByUserName,
        r.UpdatedAt,
        r.DeletedByUserId,
        r.DeletedByUserName,
        r.DeletedAt
    FROM dbo.Roles r
    OUTER APPLY
    (
        SELECT STRING_AGG(CONVERT(nvarchar(max), p.Code), N',') WITHIN GROUP (ORDER BY p.Code) AS Permissions
        FROM dbo.RolePermissions rp
        INNER JOIN dbo.Permissions p ON p.Id = rp.PermissionId
        WHERE rp.RoleId = r.Id
    ) permissionInfo
    WHERE r.Id = @Id
      AND r.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ROLSEGURIDADBUSCARPORCODIGO
    @Code nvarchar(80),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.Roles
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ROLSEGURIDADBUSCARPORNOMBRE
    @Name nvarchar(120),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.Roles
    WHERE Name = @Name
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ROLSEGURIDADCREAR
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @DisplayOrder int = 0,
    @IsSystemRole bit = 0,
    @IsAssignable bit = 1,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    INSERT INTO dbo.Roles
    (
        Code, Name, Description, DisplayOrder, IsSystemRole, IsAssignable, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @Code, @Name, @Description, @DisplayOrder, @IsSystemRole, @IsAssignable, @IsActive,
        @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME()
    );

    DECLARE @Id int = CAST(SCOPE_IDENTITY() AS int);

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityRoles', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
    FROM
    (
        VALUES
            (N'Code', CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @Description)),
            (N'DisplayOrder', CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsSystemRole', CONVERT(nvarchar(max), CONVERT(int, @IsSystemRole))),
            (N'IsAssignable', CONVERT(nvarchar(max), CONVERT(int, @IsAssignable))),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, NewValue)
    WHERE NewValue IS NOT NULL;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ROLSEGURIDADACTUALIZAR
    @Id int,
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @DisplayOrder int = 0,
    @IsSystemRole bit = 0,
    @IsAssignable bit = 1,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    DECLARE
        @OldCode nvarchar(80),
        @OldName nvarchar(120),
        @OldDescription nvarchar(300),
        @OldDisplayOrder int,
        @OldIsSystemRole bit,
        @OldIsAssignable bit,
        @OldIsActive bit;

    SELECT
        @OldCode = Code,
        @OldName = Name,
        @OldDescription = Description,
        @OldDisplayOrder = DisplayOrder,
        @OldIsSystemRole = IsSystemRole,
        @OldIsAssignable = IsAssignable,
        @OldIsActive = IsActive
    FROM dbo.Roles
    WHERE Id = @Id
      AND IsDeleted = 0;

    IF @OldCode IS NULL
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.Roles
    SET
        Code = @Code,
        Name = @Name,
        Description = @Description,
        DisplayOrder = @DisplayOrder,
        IsSystemRole = @IsSystemRole,
        IsAssignable = @IsAssignable,
        IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityRoles', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM
    (
        VALUES
            (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
            (N'DisplayOrder', CONVERT(nvarchar(max), @OldDisplayOrder), CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsSystemRole', CONVERT(nvarchar(max), CONVERT(int, @OldIsSystemRole)), CONVERT(nvarchar(max), CONVERT(int, @IsSystemRole))),
            (N'IsAssignable', CONVERT(nvarchar(max), CONVERT(int, @OldIsAssignable)), CONVERT(nvarchar(max), CONVERT(int, @IsAssignable))),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_ROLSEGURIDADELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = @Id AND IsSystemRole = 1 AND IsDeleted = 0)
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.Roles
    SET
        IsDeleted = 1,
        IsActive = 0,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    IF @AffectedRows > 0
    BEGIN
        INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES (N'SecurityRoles', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);
    END;

    SELECT @AffectedRows;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.ROLES')
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.SECURITY.ROLES', N'Roles', N'Mantenimiento de roles de seguridad',
        N'security-roles', 1, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityForms
SET Name = N'Roles',
    Description = N'Mantenimiento de roles de seguridad',
    FormKey = N'security-roles',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.SECURITY.ROLES';
GO

DECLARE @SecurityMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY');
DECLARE @RolesFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.ROLES');

IF @SecurityMenuId IS NOT NULL AND @RolesFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.ROLES')
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @SecurityMenuId, N'MENU.SECURITY.ROLES', N'Roles',
            N'Administrar roles de seguridad',
            3, @RolesFormId, N'security-roles',
            N'Accordion/roles_32.svg', N'Accordion/roles_16.svg',
            20, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET FormId = @RolesFormId,
        FormKey = N'security-roles',
        IconLarge = N'Accordion/roles_32.svg',
        IconSmall = N'Accordion/roles_16.svg',
        DisplayOrder = 20
    WHERE Code = N'MENU.SECURITY.ROLES';
END;
GO

DECLARE @SecurityModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'SECURITY');

IF @SecurityModuleId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Code = N'SECURITY.ROLES.MANAGE')
BEGIN
    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    VALUES (@SecurityModuleId, N'SECURITY.ROLES.MANAGE', N'Gestionar roles', N'Crear, editar, eliminar y consultar roles');
END;
GO

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');
DECLARE @RolesPermissionId int = (SELECT TOP (1) Id FROM dbo.Permissions WHERE Code = N'SECURITY.ROLES.MANAGE');

IF @AdminRoleId IS NOT NULL AND @RolesPermissionId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RolesPermissionId)
    BEGIN
        INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
        VALUES (@AdminRoleId, @RolesPermissionId);
    END;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.CREATE' OR Name = N'Nuevo')
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'ACTION.CREATE', N'Nuevo', N'Crear registros',
        N'Inicio', N'Acciones', N'create',
        N'Operaciones/nuevo_32.svg', N'Operaciones/nuevo_16.svg',
        20, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.UPDATE' OR Name = N'Editar')
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'ACTION.UPDATE', N'Editar', N'Editar registros existentes',
        N'Inicio', N'Acciones', N'update',
        N'Operaciones/editar_32.svg', N'Operaciones/editar_16.svg',
        30, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityOperations WHERE Code = N'ACTION.COPY')
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        IconLarge, IconSmall, DisplayOrder, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'ACTION.COPY', N'Copiar', N'Crear un registro desde otro existente',
        N'Inicio', N'Acciones', N'copy',
        N'Operaciones/copiar_32.svg', N'Operaciones/copiar_16.svg',
        40, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

UPDATE dbo.SecurityOperations
SET IconLarge = CASE Code
        WHEN N'ACTION.REFRESH' THEN N'Operaciones/actualizar_32.svg'
        WHEN N'ACTION.DELETE' THEN N'Operaciones/eliminar_32.svg'
        WHEN N'ACTION.CONSULT' THEN N'Operaciones/consultar_32.svg'
        WHEN N'ACTION.HISTORY' THEN N'Operaciones/historia_32.svg'
        ELSE IconLarge
    END,
    IconSmall = CASE Code
        WHEN N'ACTION.REFRESH' THEN N'Operaciones/actualizar_16.svg'
        WHEN N'ACTION.DELETE' THEN N'Operaciones/eliminar_16.svg'
        WHEN N'ACTION.CONSULT' THEN N'Operaciones/consultar_16.svg'
        WHEN N'ACTION.HISTORY' THEN N'Operaciones/historia_16.svg'
        ELSE IconSmall
    END
WHERE Code IN (N'ACTION.REFRESH', N'ACTION.DELETE', N'ACTION.CONSULT', N'ACTION.HISTORY');
GO

DECLARE @AdminRoleIdForAccess int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');
DECLARE @RolesMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.ROLES');
DECLARE @RolesFormIdForAccess int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.ROLES');

IF @AdminRoleIdForAccess IS NOT NULL AND @RolesMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleIdForAccess AND MenuId = @RolesMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleIdForAccess, @RolesMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleIdForAccess IS NOT NULL AND @RolesFormIdForAccess IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleIdForAccess, @RolesFormIdForAccess, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.Code IN
    (
        N'ACTION.REFRESH',
        N'ACTION.CREATE',
        N'ACTION.NEW',
        N'ACTION.COPY',
        N'ACTION.UPDATE',
        N'ACTION.EDIT',
        N'ACTION.DELETE',
        N'ACTION.CONSULT',
        N'ACTION.HISTORY'
    )
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleIdForAccess
            AND existing.FormId = @RolesFormIdForAccess
            AND existing.OperationId = operation.Id
      );
END;
GO
