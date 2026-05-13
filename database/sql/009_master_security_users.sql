IF COL_LENGTH('dbo.Users', 'CreatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD CreatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'CreatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD CreatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'UpdatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD UpdatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'UpdatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD UpdatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD IsDeleted bit NOT NULL CONSTRAINT DF_Users_IsDeleted DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Users', 'DeletedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD DeletedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'DeletedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD DeletedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD DeletedAt datetime2(0) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'PhoneNumber') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD PhoneNumber nvarchar(30) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'NormalizedPhoneNumber') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD NormalizedPhoneNumber nvarchar(30) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'EmailConfirmed') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD EmailConfirmed bit NOT NULL CONSTRAINT DF_Users_EmailConfirmed DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Users', 'PhoneNumberConfirmed') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD PhoneNumberConfirmed bit NOT NULL CONSTRAINT DF_Users_PhoneNumberConfirmed DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Users', 'FirstName') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD FirstName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'LastName') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD LastName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'LastLoginAt') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD LastLoginAt datetime2(0) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'MustChangePassword') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD MustChangePassword bit NOT NULL CONSTRAINT DF_Users_MustChangePassword DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Users', 'LockoutEndAt') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD LockoutEndAt datetime2(0) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'TwoFactorEnabled') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD TwoFactorEnabled bit NOT NULL CONSTRAINT DF_Users_TwoFactorEnabled DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Users', 'IsLocked') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD IsLocked bit NOT NULL CONSTRAINT DF_Users_IsLocked DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Users', 'CanUseWeb') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD CanUseWeb bit NOT NULL CONSTRAINT DF_Users_CanUseWeb DEFAULT 1;
END;
GO

IF COL_LENGTH('dbo.Users', 'CanUseMobile') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD CanUseMobile bit NOT NULL CONSTRAINT DF_Users_CanUseMobile DEFAULT 1;
END;
GO

IF COL_LENGTH('dbo.Users', 'ProfileImageUrl') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD ProfileImageUrl nvarchar(500) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'ProfileImage') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD ProfileImage varbinary(max) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'ProfileImageContentType') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD ProfileImageContentType nvarchar(100) NULL;
END;
GO

IF COL_LENGTH('dbo.Users', 'ProfileImageFileName') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD ProfileImageFileName nvarchar(260) NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_IsDeleted_UserName' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
    CREATE INDEX IX_Users_IsDeleted_UserName ON dbo.Users (IsDeleted, UserName);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_NormalizedEmail' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
    CREATE INDEX IX_Users_NormalizedEmail ON dbo.Users (NormalizedEmail) WHERE NormalizedEmail IS NOT NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_NormalizedPhoneNumber' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
    CREATE INDEX IX_Users_NormalizedPhoneNumber ON dbo.Users (NormalizedPhoneNumber) WHERE NormalizedPhoneNumber IS NOT NULL;
END;
GO

IF OBJECT_ID(N'dbo.AuditSecurityChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSecurityChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSecurityChanges PRIMARY KEY,
        EntityName nvarchar(120) NOT NULL,
        RecordId nvarchar(80) NOT NULL,
        [Action] nvarchar(20) NOT NULL,
        FieldName nvarchar(120) NOT NULL,
        OldValue nvarchar(max) NULL,
        NewValue nvarchar(max) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        Source nvarchar(60) NOT NULL CONSTRAINT DF_AuditSecurityChanges_Source DEFAULT N'API',
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditSecurityChanges_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AuditSecurityChanges_Entity_Record_CreatedAt ON dbo.AuditSecurityChanges (EntityName, RecordId, CreatedAt DESC);
    CREATE INDEX IX_AuditSecurityChanges_User_CreatedAt ON dbo.AuditSecurityChanges (UserId, CreatedAt DESC);
    CREATE INDEX IX_AuditSecurityChanges_CreatedAt ON dbo.AuditSecurityChanges (CreatedAt DESC);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_USUARIOSEGURIDADLISTAR
AS
BEGIN
    SELECT
        u.Id,
        u.UserName,
        u.Email,
        u.PhoneNumber,
        u.EmailConfirmed,
        u.PhoneNumberConfirmed,
        u.FirstName,
        u.LastName,
        u.DisplayName,
        u.IsActive,
        u.IsLocked,
        u.CanUseWeb,
        u.CanUseMobile,
        u.FailedAccessCount,
        u.LastLoginAt,
        u.MustChangePassword,
        u.LockoutEndAt,
        u.TwoFactorEnabled,
        u.ProfileImageUrl,
        u.ProfileImage,
        u.ProfileImageContentType,
        u.ProfileImageFileName,
        roleInfo.RoleId,
        COALESCE(roleInfo.Roles, N'') AS RolesText,
        COALESCE(companyInfo.Companies, N'') AS CompaniesText,
        u.CreatedByUserId,
        u.CreatedByUserName,
        u.CreatedAt,
        u.UpdatedByUserId,
        u.UpdatedByUserName,
        u.UpdatedAt,
        u.DeletedByUserId,
        u.DeletedByUserName,
        u.DeletedAt
    FROM dbo.Users u
    OUTER APPLY
    (
        SELECT
            MIN(ur.RoleId) AS RoleId,
            STRING_AGG(CONVERT(nvarchar(max), r.Code), N',') WITHIN GROUP (ORDER BY r.Code) AS Roles
        FROM dbo.UserRoles ur
        INNER JOIN dbo.Roles r ON r.Id = ur.RoleId
        WHERE ur.UserId = u.Id
    ) roleInfo
    OUTER APPLY
    (
        SELECT STRING_AGG(CONVERT(nvarchar(max), c.Code), N',') WITHIN GROUP (ORDER BY c.Code) AS Companies
        FROM dbo.UserCompanies uc
        INNER JOIN dbo.Companies c ON c.Id = uc.CompanyId
        WHERE uc.UserId = u.Id
          AND uc.IsActive = 1
    ) companyInfo
    WHERE u.IsDeleted = 0
    ORDER BY u.UserName;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_USUARIOSEGURIDADBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        u.Id,
        u.UserName,
        u.Email,
        u.PhoneNumber,
        u.EmailConfirmed,
        u.PhoneNumberConfirmed,
        u.FirstName,
        u.LastName,
        u.DisplayName,
        u.IsActive,
        u.IsLocked,
        u.CanUseWeb,
        u.CanUseMobile,
        u.FailedAccessCount,
        u.LastLoginAt,
        u.MustChangePassword,
        u.LockoutEndAt,
        u.TwoFactorEnabled,
        u.ProfileImageUrl,
        u.ProfileImage,
        u.ProfileImageContentType,
        u.ProfileImageFileName,
        roleInfo.RoleId,
        COALESCE(roleInfo.Roles, N'') AS RolesText,
        COALESCE(companyInfo.Companies, N'') AS CompaniesText,
        u.CreatedByUserId,
        u.CreatedByUserName,
        u.CreatedAt,
        u.UpdatedByUserId,
        u.UpdatedByUserName,
        u.UpdatedAt,
        u.DeletedByUserId,
        u.DeletedByUserName,
        u.DeletedAt
    FROM dbo.Users u
    OUTER APPLY
    (
        SELECT
            MIN(ur.RoleId) AS RoleId,
            STRING_AGG(CONVERT(nvarchar(max), r.Code), N',') WITHIN GROUP (ORDER BY r.Code) AS Roles
        FROM dbo.UserRoles ur
        INNER JOIN dbo.Roles r ON r.Id = ur.RoleId
        WHERE ur.UserId = u.Id
    ) roleInfo
    OUTER APPLY
    (
        SELECT STRING_AGG(CONVERT(nvarchar(max), c.Code), N',') WITHIN GROUP (ORDER BY c.Code) AS Companies
        FROM dbo.UserCompanies uc
        INNER JOIN dbo.Companies c ON c.Id = uc.CompanyId
        WHERE uc.UserId = u.Id
          AND uc.IsActive = 1
    ) companyInfo
    WHERE u.Id = @Id
      AND u.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_USUARIOSEGURIDADBUSCARPORNOMBRE
    @UserName nvarchar(120),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.Users
    WHERE NormalizedUserName = UPPER(@UserName)
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_USUARIOSEGURIDADROLES
AS
BEGIN
    SELECT Id, Code, Name, Description, IsActive
    FROM dbo.Roles
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_USUARIOSEGURIDADCREAR
    @UserName nvarchar(120),
    @Email nvarchar(256) = NULL,
    @PhoneNumber nvarchar(30) = NULL,
    @EmailConfirmed bit = 0,
    @PhoneNumberConfirmed bit = 0,
    @FirstName nvarchar(120) = NULL,
    @LastName nvarchar(120) = NULL,
    @DisplayName nvarchar(200),
    @PasswordHash nvarchar(max),
    @RoleId int = NULL,
    @IsActive bit = 1,
    @IsLocked bit = 0,
    @CanUseWeb bit = 1,
    @CanUseMobile bit = 1,
    @MustChangePassword bit = 0,
    @LockoutEndAt datetime2(0) = NULL,
    @TwoFactorEnabled bit = 0,
    @ProfileImageUrl nvarchar(500) = NULL,
    @ProfileImage varbinary(max) = NULL,
    @ProfileImageContentType nvarchar(100) = NULL,
    @ProfileImageFileName nvarchar(260) = NULL,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    INSERT INTO dbo.Users
    (
        UserName, NormalizedUserName, Email, NormalizedEmail, PhoneNumber, NormalizedPhoneNumber,
        EmailConfirmed, PhoneNumberConfirmed, FirstName, LastName, DisplayName, PasswordHash,
        IsActive, IsLocked, CanUseWeb, CanUseMobile, FailedAccessCount, MustChangePassword, LockoutEndAt,
        TwoFactorEnabled, ProfileImageUrl, ProfileImage, ProfileImageContentType, ProfileImageFileName,
        CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @UserName, UPPER(@UserName), @Email, UPPER(@Email), @PhoneNumber, UPPER(@PhoneNumber),
        @EmailConfirmed, @PhoneNumberConfirmed, @FirstName, @LastName, @DisplayName, @PasswordHash,
        @IsActive, @IsLocked, @CanUseWeb, @CanUseMobile, 0, @MustChangePassword, @LockoutEndAt,
        @TwoFactorEnabled, @ProfileImageUrl, @ProfileImage, @ProfileImageContentType, @ProfileImageFileName,
        @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME()
    );

    DECLARE @Id int = CAST(SCOPE_IDENTITY() AS int);

    IF @RoleId IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = @RoleId AND IsActive = 1)
    BEGIN
        INSERT INTO dbo.UserRoles (UserId, RoleId)
        VALUES (@Id, @RoleId);
    END;

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityUsers', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
    FROM
    (
        VALUES
            (N'UserName', CONVERT(nvarchar(max), @UserName)),
            (N'Email', CONVERT(nvarchar(max), @Email)),
            (N'PhoneNumber', CONVERT(nvarchar(max), @PhoneNumber)),
            (N'EmailConfirmed', CONVERT(nvarchar(max), CONVERT(int, @EmailConfirmed))),
            (N'PhoneNumberConfirmed', CONVERT(nvarchar(max), CONVERT(int, @PhoneNumberConfirmed))),
            (N'FirstName', CONVERT(nvarchar(max), @FirstName)),
            (N'LastName', CONVERT(nvarchar(max), @LastName)),
            (N'DisplayName', CONVERT(nvarchar(max), @DisplayName)),
            (N'PasswordHash', CONVERT(nvarchar(max), N'CREATED')),
            (N'RoleId', CONVERT(nvarchar(max), @RoleId)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
            (N'IsLocked', CONVERT(nvarchar(max), CONVERT(int, @IsLocked))),
            (N'CanUseWeb', CONVERT(nvarchar(max), CONVERT(int, @CanUseWeb))),
            (N'CanUseMobile', CONVERT(nvarchar(max), CONVERT(int, @CanUseMobile))),
            (N'MustChangePassword', CONVERT(nvarchar(max), CONVERT(int, @MustChangePassword))),
            (N'LockoutEndAt', CONVERT(nvarchar(max), @LockoutEndAt)),
            (N'TwoFactorEnabled', CONVERT(nvarchar(max), CONVERT(int, @TwoFactorEnabled))),
            (N'ProfileImageUrl', CONVERT(nvarchar(max), @ProfileImageUrl)),
            (N'ProfileImage', CASE WHEN @ProfileImage IS NULL THEN NULL ELSE N'LOADED' END),
            (N'ProfileImageContentType', CONVERT(nvarchar(max), @ProfileImageContentType)),
            (N'ProfileImageFileName', CONVERT(nvarchar(max), @ProfileImageFileName))
    ) AS Changes(FieldName, NewValue)
    WHERE NewValue IS NOT NULL;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_USUARIOSEGURIDADACTUALIZAR
    @Id int,
    @UserName nvarchar(120),
    @Email nvarchar(256) = NULL,
    @PhoneNumber nvarchar(30) = NULL,
    @EmailConfirmed bit = 0,
    @PhoneNumberConfirmed bit = 0,
    @FirstName nvarchar(120) = NULL,
    @LastName nvarchar(120) = NULL,
    @DisplayName nvarchar(200),
    @PasswordHash nvarchar(max) = NULL,
    @RoleId int = NULL,
    @IsActive bit = 1,
    @IsLocked bit = 0,
    @CanUseWeb bit = 1,
    @CanUseMobile bit = 1,
    @MustChangePassword bit = 0,
    @LockoutEndAt datetime2(0) = NULL,
    @TwoFactorEnabled bit = 0,
    @ProfileImageUrl nvarchar(500) = NULL,
    @ProfileImage varbinary(max) = NULL,
    @ProfileImageContentType nvarchar(100) = NULL,
    @ProfileImageFileName nvarchar(260) = NULL,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    DECLARE
        @OldUserName nvarchar(120),
        @OldEmail nvarchar(256),
        @OldPhoneNumber nvarchar(30),
        @OldEmailConfirmed bit,
        @OldPhoneNumberConfirmed bit,
        @OldFirstName nvarchar(120),
        @OldLastName nvarchar(120),
        @OldDisplayName nvarchar(200),
        @OldRoleId int,
        @OldIsActive bit,
        @OldIsLocked bit,
        @OldCanUseWeb bit,
        @OldCanUseMobile bit,
        @OldMustChangePassword bit,
        @OldLockoutEndAt datetime2(0),
        @OldTwoFactorEnabled bit,
        @OldProfileImageUrl nvarchar(500),
        @OldProfileImage varbinary(max),
        @OldProfileImageContentType nvarchar(100),
        @OldProfileImageFileName nvarchar(260);

    SELECT
        @OldUserName = u.UserName,
        @OldEmail = u.Email,
        @OldPhoneNumber = u.PhoneNumber,
        @OldEmailConfirmed = u.EmailConfirmed,
        @OldPhoneNumberConfirmed = u.PhoneNumberConfirmed,
        @OldFirstName = u.FirstName,
        @OldLastName = u.LastName,
        @OldDisplayName = u.DisplayName,
        @OldIsActive = u.IsActive,
        @OldIsLocked = u.IsLocked,
        @OldCanUseWeb = u.CanUseWeb,
        @OldCanUseMobile = u.CanUseMobile,
        @OldMustChangePassword = u.MustChangePassword,
        @OldLockoutEndAt = u.LockoutEndAt,
        @OldTwoFactorEnabled = u.TwoFactorEnabled,
        @OldProfileImageUrl = u.ProfileImageUrl,
        @OldProfileImage = u.ProfileImage,
        @OldProfileImageContentType = u.ProfileImageContentType,
        @OldProfileImageFileName = u.ProfileImageFileName
    FROM dbo.Users u
    WHERE u.Id = @Id
      AND u.IsDeleted = 0;

    SELECT TOP (1) @OldRoleId = ur.RoleId
    FROM dbo.UserRoles ur
    WHERE ur.UserId = @Id
    ORDER BY ur.RoleId;

    IF @OldUserName IS NULL
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.Users
    SET
        UserName = @UserName,
        NormalizedUserName = UPPER(@UserName),
        Email = @Email,
        NormalizedEmail = UPPER(@Email),
        PhoneNumber = @PhoneNumber,
        NormalizedPhoneNumber = UPPER(@PhoneNumber),
        EmailConfirmed = @EmailConfirmed,
        PhoneNumberConfirmed = @PhoneNumberConfirmed,
        FirstName = @FirstName,
        LastName = @LastName,
        DisplayName = @DisplayName,
        PasswordHash = COALESCE(@PasswordHash, PasswordHash),
        IsActive = @IsActive,
        IsLocked = @IsLocked,
        CanUseWeb = @CanUseWeb,
        CanUseMobile = @CanUseMobile,
        MustChangePassword = @MustChangePassword,
        LockoutEndAt = @LockoutEndAt,
        TwoFactorEnabled = @TwoFactorEnabled,
        ProfileImageUrl = @ProfileImageUrl,
        ProfileImage = @ProfileImage,
        ProfileImageContentType = @ProfileImageContentType,
        ProfileImageFileName = @ProfileImageFileName,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    DELETE FROM dbo.UserRoles
    WHERE UserId = @Id;

    IF @RoleId IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = @RoleId AND IsActive = 1)
    BEGIN
        INSERT INTO dbo.UserRoles (UserId, RoleId)
        VALUES (@Id, @RoleId);
    END;

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityUsers', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM
    (
        VALUES
            (N'UserName', CONVERT(nvarchar(max), @OldUserName), CONVERT(nvarchar(max), @UserName)),
            (N'Email', CONVERT(nvarchar(max), @OldEmail), CONVERT(nvarchar(max), @Email)),
            (N'PhoneNumber', CONVERT(nvarchar(max), @OldPhoneNumber), CONVERT(nvarchar(max), @PhoneNumber)),
            (N'EmailConfirmed', CONVERT(nvarchar(max), CONVERT(int, @OldEmailConfirmed)), CONVERT(nvarchar(max), CONVERT(int, @EmailConfirmed))),
            (N'PhoneNumberConfirmed', CONVERT(nvarchar(max), CONVERT(int, @OldPhoneNumberConfirmed)), CONVERT(nvarchar(max), CONVERT(int, @PhoneNumberConfirmed))),
            (N'FirstName', CONVERT(nvarchar(max), @OldFirstName), CONVERT(nvarchar(max), @FirstName)),
            (N'LastName', CONVERT(nvarchar(max), @OldLastName), CONVERT(nvarchar(max), @LastName)),
            (N'DisplayName', CONVERT(nvarchar(max), @OldDisplayName), CONVERT(nvarchar(max), @DisplayName)),
            (N'RoleId', CONVERT(nvarchar(max), @OldRoleId), CONVERT(nvarchar(max), @RoleId)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive))),
            (N'IsLocked', CONVERT(nvarchar(max), CONVERT(int, @OldIsLocked)), CONVERT(nvarchar(max), CONVERT(int, @IsLocked))),
            (N'CanUseWeb', CONVERT(nvarchar(max), CONVERT(int, @OldCanUseWeb)), CONVERT(nvarchar(max), CONVERT(int, @CanUseWeb))),
            (N'CanUseMobile', CONVERT(nvarchar(max), CONVERT(int, @OldCanUseMobile)), CONVERT(nvarchar(max), CONVERT(int, @CanUseMobile))),
            (N'MustChangePassword', CONVERT(nvarchar(max), CONVERT(int, @OldMustChangePassword)), CONVERT(nvarchar(max), CONVERT(int, @MustChangePassword))),
            (N'LockoutEndAt', CONVERT(nvarchar(max), @OldLockoutEndAt), CONVERT(nvarchar(max), @LockoutEndAt)),
            (N'TwoFactorEnabled', CONVERT(nvarchar(max), CONVERT(int, @OldTwoFactorEnabled)), CONVERT(nvarchar(max), CONVERT(int, @TwoFactorEnabled))),
            (N'ProfileImageUrl', CONVERT(nvarchar(max), @OldProfileImageUrl), CONVERT(nvarchar(max), @ProfileImageUrl)),
            (N'ProfileImage', CASE WHEN @OldProfileImage IS NULL THEN NULL ELSE N'LOADED' END, CASE WHEN @ProfileImage IS NULL THEN NULL ELSE N'LOADED' END),
            (N'ProfileImageContentType', CONVERT(nvarchar(max), @OldProfileImageContentType), CONVERT(nvarchar(max), @ProfileImageContentType)),
            (N'ProfileImageFileName', CONVERT(nvarchar(max), @OldProfileImageFileName), CONVERT(nvarchar(max), @ProfileImageFileName)),
            (N'PasswordHash', CASE WHEN @PasswordHash IS NULL THEN N'UNCHANGED' ELSE N'********' END, CASE WHEN @PasswordHash IS NULL THEN N'UNCHANGED' ELSE N'UPDATED' END)
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_USUARIOSEGURIDADELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    UPDATE dbo.Users
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
        VALUES (N'SecurityUsers', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);
    END;

    SELECT @AffectedRows;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.USERS')
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.SECURITY.USERS', N'Usuarios', N'Mantenimiento de usuarios de seguridad',
        N'users', 1, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;
GO

DECLARE @SecurityMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY');
DECLARE @UsersFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.USERS');

IF @SecurityMenuId IS NOT NULL AND @UsersFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.USERS')
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @SecurityMenuId, N'MENU.SECURITY.USERS', N'Usuarios',
            N'Administrar usuarios de seguridad',
            3, @UsersFormId, N'users',
            N'Accordion/usuarios_32.svg', N'Accordion/usuarios_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET FormId = @UsersFormId,
        FormKey = N'users',
        IconLarge = COALESCE(IconLarge, N'Accordion/usuarios_32.svg'),
        IconSmall = COALESCE(IconSmall, N'Accordion/usuarios_16.svg')
    WHERE Code = N'MENU.SECURITY.USERS';
END;
GO
