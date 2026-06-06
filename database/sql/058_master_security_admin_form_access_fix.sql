DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @SecurityAccessBypassPermissionId int = (
    SELECT TOP (1) Id
    FROM dbo.Permissions
    WHERE Code = N'SECURITY.ACCESS.BYPASS'
);

IF @AdminRoleId IS NOT NULL AND @SecurityAccessBypassPermissionId IS NOT NULL
BEGIN
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
    SELECT @AdminRoleId, @SecurityAccessBypassPermissionId
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.RolePermissions rolePermission
        WHERE rolePermission.RoleId = @AdminRoleId
          AND rolePermission.PermissionId = @SecurityAccessBypassPermissionId
    );
END;
GO

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);

IF @AdminRoleId IS NOT NULL
BEGIN
    DECLARE @SecurityAdminForms table
    (
        FormKey nvarchar(120) NOT NULL PRIMARY KEY
    );

    INSERT INTO @SecurityAdminForms (FormKey)
    VALUES
        (N'users'),
        (N'security-roles'),
        (N'security-operations'),
        (N'security-menus'),
        (N'security-forms'),
        (N'security-fields'),
        (N'security-access'),
        (N'security-form-access-transactional');

    MERGE dbo.SecurityRoleFormOperations AS target
    USING
    (
        SELECT
            @AdminRoleId AS RoleId,
            form.Id AS FormId,
            operation.Id AS OperationId
        FROM dbo.SecurityForms form
        INNER JOIN @SecurityAdminForms adminForm ON adminForm.FormKey = form.FormKey
        CROSS JOIN dbo.SecurityOperations operation
        WHERE form.IsDeleted = 0
          AND form.IsActive = 1
          AND operation.IsDeleted = 0
          AND operation.IsActive = 1
    ) AS source
        ON target.RoleId = source.RoleId
       AND target.FormId = source.FormId
       AND target.OperationId = source.OperationId
    WHEN MATCHED THEN
        UPDATE SET
            IsAllowed = 1,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserName = N'Sistema',
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (source.RoleId, source.FormId, source.OperationId, 1, N'Sistema', SYSUTCDATETIME());
END;
GO
