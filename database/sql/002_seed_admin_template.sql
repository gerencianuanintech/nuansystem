USE [NuanSystem_Master];
GO

/*
    Reemplace los valores marcados antes de ejecutar.

    PasswordHash debe generarse con el algoritmo PBKDF2-SHA256 usado por
    NuanSystem.Infrastructure.Authentication.Pbkdf2PasswordHasher.
*/

DECLARE @userName nvarchar(120) = N'admin';
DECLARE @email nvarchar(256) = N'admin@nuansystem.local';
DECLARE @displayName nvarchar(200) = N'Administrador';
DECLARE @passwordHash nvarchar(max) = N'<PASSWORD_HASH>';

IF @passwordHash = N'<PASSWORD_HASH>'
BEGIN
    THROW 50001, 'Debe reemplazar <PASSWORD_HASH> antes de ejecutar este script.', 1;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE NormalizedUserName = UPPER(@userName))
BEGIN
    INSERT INTO dbo.Users
    (
        UserName,
        NormalizedUserName,
        Email,
        NormalizedEmail,
        DisplayName,
        PasswordHash,
        IsActive
    )
    VALUES
    (
        @userName,
        UPPER(@userName),
        @email,
        UPPER(@email),
        @displayName,
        @passwordHash,
        1
    );
END;

DECLARE @userId int = (SELECT Id FROM dbo.Users WHERE NormalizedUserName = UPPER(@userName));
DECLARE @adminRoleId int = (SELECT Id FROM dbo.Roles WHERE Code = N'ADMIN');

IF @adminRoleId IS NULL
BEGIN
    THROW 50002, 'No existe el rol ADMIN. Ejecute primero 001_master_database.sql.', 1;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.UserRoles WHERE UserId = @userId AND RoleId = @adminRoleId)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId)
    VALUES (@userId, @adminRoleId);
END;
GO
