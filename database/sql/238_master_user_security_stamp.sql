SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;

IF COL_LENGTH(N'dbo.Users', N'SecurityStamp') IS NULL
BEGIN
    ALTER TABLE dbo.Users
        ADD SecurityStamp uniqueidentifier NOT NULL
            CONSTRAINT DF_Users_SecurityStamp DEFAULT NEWID() WITH VALUES;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_Users_RotateSecurityStamp
ON dbo.Users
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(SecurityStamp) RETURN;

    UPDATE users
    SET SecurityStamp = NEWID()
    FROM dbo.Users AS users
    INNER JOIN inserted AS currentUser ON currentUser.Id = users.Id
    INNER JOIN deleted AS previousUser ON previousUser.Id = users.Id
    WHERE currentUser.PasswordHash <> previousUser.PasswordHash
       OR currentUser.IsActive <> previousUser.IsActive
       OR currentUser.IsLocked <> previousUser.IsLocked
       OR currentUser.MustChangePassword <> previousUser.MustChangePassword
       OR ISNULL(currentUser.LockoutEndAt, CONVERT(datetime2, '19000101'))
          <> ISNULL(previousUser.LockoutEndAt, CONVERT(datetime2, '19000101'))
       OR currentUser.IsDeleted <> previousUser.IsDeleted;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_UserRoles_RotateSecurityStamp
ON dbo.UserRoles
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE users
    SET SecurityStamp = NEWID()
    FROM dbo.Users AS users
    INNER JOIN
    (
        SELECT UserId FROM inserted
        UNION
        SELECT UserId FROM deleted
    ) AS affected ON affected.UserId = users.Id;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_RolePermissions_RotateSecurityStamp
ON dbo.RolePermissions
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE users
    SET SecurityStamp = NEWID()
    FROM dbo.Users AS users
    INNER JOIN dbo.UserRoles AS userRole ON userRole.UserId = users.Id
    INNER JOIN
    (
        SELECT RoleId FROM inserted
        UNION
        SELECT RoleId FROM deleted
    ) AS affected ON affected.RoleId = userRole.RoleId;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_Roles_RotateSecurityStamp
ON dbo.Roles
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(IsActive) RETURN;

    UPDATE users
    SET SecurityStamp = NEWID()
    FROM dbo.Users AS users
    INNER JOIN dbo.UserRoles AS userRole ON userRole.UserId = users.Id
    INNER JOIN inserted AS currentRole ON currentRole.Id = userRole.RoleId
    INNER JOIN deleted AS previousRole ON previousRole.Id = currentRole.Id
    WHERE currentRole.IsActive <> previousRole.IsActive;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_Permissions_RotateSecurityStamp
ON dbo.Permissions
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(IsActive) RETURN;

    UPDATE users
    SET SecurityStamp = NEWID()
    FROM dbo.Users AS users
    INNER JOIN dbo.UserRoles AS userRole ON userRole.UserId = users.Id
    INNER JOIN dbo.RolePermissions AS rolePermission ON rolePermission.RoleId = userRole.RoleId
    INNER JOIN inserted AS currentPermission ON currentPermission.Id = rolePermission.PermissionId
    INNER JOIN deleted AS previousPermission ON previousPermission.Id = currentPermission.Id
    WHERE currentPermission.IsActive <> previousPermission.IsActive;
END;
GO
