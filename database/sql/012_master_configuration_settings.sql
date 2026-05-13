IF COL_LENGTH('dbo.CompanyParameters', 'DataType') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD DataType nvarchar(30) NOT NULL CONSTRAINT DF_CompanyParameters_DataType DEFAULT N'Text';
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'Category') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD Category nvarchar(80) NULL;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'IsEncrypted') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD IsEncrypted bit NOT NULL CONSTRAINT DF_CompanyParameters_IsEncrypted DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'IsSystemParameter') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD IsSystemParameter bit NOT NULL CONSTRAINT DF_CompanyParameters_IsSystemParameter DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'IsEditable') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD IsEditable bit NOT NULL CONSTRAINT DF_CompanyParameters_IsEditable DEFAULT 1;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'DisplayOrder') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD DisplayOrder int NOT NULL CONSTRAINT DF_CompanyParameters_DisplayOrder DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'DefaultValue') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD DefaultValue nvarchar(max) NULL;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'ValidationExpression') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD ValidationExpression nvarchar(300) NULL;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'IsActive') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD IsActive bit NOT NULL CONSTRAINT DF_CompanyParameters_IsActive DEFAULT 1;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'CreatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD CreatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'CreatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD CreatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'UpdatedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD UpdatedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'UpdatedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD UpdatedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD IsDeleted bit NOT NULL CONSTRAINT DF_CompanyParameters_IsDeleted DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'DeletedByUserId') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD DeletedByUserId int NULL;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'DeletedByUserName') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD DeletedByUserName nvarchar(120) NULL;
END;
GO

IF COL_LENGTH('dbo.CompanyParameters', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyParameters ADD DeletedAt datetime2(0) NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CompanyParameters_Company_IsDeleted_DisplayOrder' AND object_id = OBJECT_ID(N'dbo.CompanyParameters'))
BEGIN
    CREATE INDEX IX_CompanyParameters_Company_IsDeleted_DisplayOrder ON dbo.CompanyParameters (CompanyId, IsDeleted, DisplayOrder, [Key]);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PARAMETROCONFIGURACIONLISTAR
    @CompanyId int
AS
BEGIN
    SELECT Id, CompanyId, [Key], [Value], Description, DataType, Category, IsEncrypted, IsSystemParameter,
           IsEditable, DisplayOrder, DefaultValue, ValidationExpression, IsActive,
           CreatedByUserId, CreatedByUserName, CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.CompanyParameters
    WHERE CompanyId = @CompanyId
      AND IsDeleted = 0
    ORDER BY DisplayOrder, [Key];
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PARAMETROCONFIGURACIONBUSCARPORID
    @CompanyId int,
    @Id int
AS
BEGIN
    SELECT Id, CompanyId, [Key], [Value], Description, DataType, Category, IsEncrypted, IsSystemParameter,
           IsEditable, DisplayOrder, DefaultValue, ValidationExpression, IsActive,
           CreatedByUserId, CreatedByUserName, CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.CompanyParameters
    WHERE CompanyId = @CompanyId
      AND Id = @Id
      AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PARAMETROCONFIGURACIONBUSCARPORCLAVE
    @CompanyId int,
    @Key nvarchar(120),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.CompanyParameters
    WHERE CompanyId = @CompanyId
      AND [Key] = @Key
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PARAMETROCONFIGURACIONCREAR
    @CompanyId int,
    @Key nvarchar(120),
    @Value nvarchar(max) = NULL,
    @Description nvarchar(300) = NULL,
    @DataType nvarchar(30) = N'Text',
    @Category nvarchar(80) = NULL,
    @IsEncrypted bit = 0,
    @IsSystemParameter bit = 0,
    @IsEditable bit = 1,
    @DisplayOrder int = 0,
    @DefaultValue nvarchar(max) = NULL,
    @ValidationExpression nvarchar(300) = NULL,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    INSERT INTO dbo.CompanyParameters
    (
        CompanyId, [Key], [Value], Description, DataType, Category, IsEncrypted, IsSystemParameter,
        IsEditable, DisplayOrder, DefaultValue, ValidationExpression, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @CompanyId, @Key, @Value, @Description, @DataType, @Category, @IsEncrypted, @IsSystemParameter,
        @IsEditable, @DisplayOrder, @DefaultValue, @ValidationExpression, @IsActive,
        @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME()
    );

    DECLARE @Id int = CAST(SCOPE_IDENTITY() AS int);

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'ConfigurationSettings', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
    FROM
    (
        VALUES
            (N'Key', CONVERT(nvarchar(max), @Key)),
            (N'Value', CASE WHEN @IsEncrypted = 1 THEN N'********' ELSE CONVERT(nvarchar(max), @Value) END),
            (N'Description', CONVERT(nvarchar(max), @Description)),
            (N'DataType', CONVERT(nvarchar(max), @DataType)),
            (N'Category', CONVERT(nvarchar(max), @Category)),
            (N'IsEncrypted', CONVERT(nvarchar(max), CONVERT(int, @IsEncrypted))),
            (N'IsSystemParameter', CONVERT(nvarchar(max), CONVERT(int, @IsSystemParameter))),
            (N'IsEditable', CONVERT(nvarchar(max), CONVERT(int, @IsEditable))),
            (N'DisplayOrder', CONVERT(nvarchar(max), @DisplayOrder)),
            (N'DefaultValue', CONVERT(nvarchar(max), @DefaultValue)),
            (N'ValidationExpression', CONVERT(nvarchar(max), @ValidationExpression)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, NewValue)
    WHERE NewValue IS NOT NULL;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PARAMETROCONFIGURACIONACTUALIZAR
    @CompanyId int,
    @Id int,
    @Key nvarchar(120),
    @Value nvarchar(max) = NULL,
    @Description nvarchar(300) = NULL,
    @DataType nvarchar(30) = N'Text',
    @Category nvarchar(80) = NULL,
    @IsEncrypted bit = 0,
    @IsSystemParameter bit = 0,
    @IsEditable bit = 1,
    @DisplayOrder int = 0,
    @DefaultValue nvarchar(max) = NULL,
    @ValidationExpression nvarchar(300) = NULL,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    DECLARE @OldKey nvarchar(120), @OldValue nvarchar(max), @OldDescription nvarchar(300), @OldDataType nvarchar(30),
            @OldCategory nvarchar(80), @OldIsEncrypted bit, @OldIsSystemParameter bit, @OldIsEditable bit,
            @OldDisplayOrder int, @OldDefaultValue nvarchar(max), @OldValidationExpression nvarchar(300), @OldIsActive bit;

    SELECT @OldKey = [Key], @OldValue = [Value], @OldDescription = Description, @OldDataType = DataType,
           @OldCategory = Category, @OldIsEncrypted = IsEncrypted, @OldIsSystemParameter = IsSystemParameter,
           @OldIsEditable = IsEditable, @OldDisplayOrder = DisplayOrder, @OldDefaultValue = DefaultValue,
           @OldValidationExpression = ValidationExpression, @OldIsActive = IsActive
    FROM dbo.CompanyParameters
    WHERE CompanyId = @CompanyId AND Id = @Id AND IsDeleted = 0;

    IF @OldKey IS NULL
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.CompanyParameters
    SET [Key] = @Key,
        [Value] = @Value,
        Description = @Description,
        DataType = @DataType,
        Category = @Category,
        IsEncrypted = @IsEncrypted,
        IsSystemParameter = @IsSystemParameter,
        IsEditable = @IsEditable,
        DisplayOrder = @DisplayOrder,
        DefaultValue = @DefaultValue,
        ValidationExpression = @ValidationExpression,
        IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @CompanyId AND Id = @Id AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'ConfigurationSettings', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM
    (
        VALUES
            (N'Key', CONVERT(nvarchar(max), @OldKey), CONVERT(nvarchar(max), @Key)),
            (N'Value', CASE WHEN @OldIsEncrypted = 1 THEN N'********' ELSE CONVERT(nvarchar(max), @OldValue) END, CASE WHEN @IsEncrypted = 1 THEN N'********' ELSE CONVERT(nvarchar(max), @Value) END),
            (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
            (N'DataType', CONVERT(nvarchar(max), @OldDataType), CONVERT(nvarchar(max), @DataType)),
            (N'Category', CONVERT(nvarchar(max), @OldCategory), CONVERT(nvarchar(max), @Category)),
            (N'IsEncrypted', CONVERT(nvarchar(max), CONVERT(int, @OldIsEncrypted)), CONVERT(nvarchar(max), CONVERT(int, @IsEncrypted))),
            (N'IsSystemParameter', CONVERT(nvarchar(max), CONVERT(int, @OldIsSystemParameter)), CONVERT(nvarchar(max), CONVERT(int, @IsSystemParameter))),
            (N'IsEditable', CONVERT(nvarchar(max), CONVERT(int, @OldIsEditable)), CONVERT(nvarchar(max), CONVERT(int, @IsEditable))),
            (N'DisplayOrder', CONVERT(nvarchar(max), @OldDisplayOrder), CONVERT(nvarchar(max), @DisplayOrder)),
            (N'DefaultValue', CONVERT(nvarchar(max), @OldDefaultValue), CONVERT(nvarchar(max), @DefaultValue)),
            (N'ValidationExpression', CONVERT(nvarchar(max), @OldValidationExpression), CONVERT(nvarchar(max), @ValidationExpression)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_PARAMETROCONFIGURACIONELIMINAR
    @CompanyId int,
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.CompanyParameters WHERE CompanyId = @CompanyId AND Id = @Id AND IsSystemParameter = 1 AND IsDeleted = 0)
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.CompanyParameters
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @CompanyId AND Id = @Id AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    IF @AffectedRows > 0
    BEGIN
        INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES (N'ConfigurationSettings', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);
    END;

    SELECT @AffectedRows;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.CONFIGURATION.SETTINGS')
BEGIN
    INSERT INTO dbo.SecurityForms (Code, Name, Description, FormKey, FormType, IsVisible, IsActive, CreatedByUserName, CreatedAt)
    VALUES (N'FORM.CONFIGURATION.SETTINGS', N'Parametros', N'Mantenimiento de parametros de configuracion', N'configuration-settings', 1, 1, 1, N'Sistema', SYSUTCDATETIME());
END;

UPDATE dbo.SecurityForms
SET Name = N'Parametros',
    Description = N'Mantenimiento de parametros de configuracion',
    FormKey = N'configuration-settings',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.CONFIGURATION.SETTINGS';
GO

DECLARE @ConfigurationMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION');
DECLARE @SettingsFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.CONFIGURATION.SETTINGS');

IF @ConfigurationMenuId IS NOT NULL AND @SettingsFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.SETTINGS')
    BEGIN
        INSERT INTO dbo.SecurityMenus (ParentId, Code, Name, Description, MenuType, FormId, FormKey, IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive, CreatedByUserName, CreatedAt)
        VALUES (@ConfigurationMenuId, N'MENU.CONFIGURATION.SETTINGS', N'Parametros', N'Administrar parametros de configuracion', 3, @SettingsFormId, N'configuration-settings', N'Accordion/settings_32.svg', N'Accordion/settings_16.svg', 20, 1, 1, N'Sistema', SYSUTCDATETIME());
    END;

    UPDATE dbo.SecurityMenus
    SET FormId = @SettingsFormId,
        FormKey = N'configuration-settings',
        IconLarge = N'Accordion/settings_32.svg',
        IconSmall = N'Accordion/settings_16.svg',
        DisplayOrder = 20,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.CONFIGURATION.SETTINGS';
END;
GO

DECLARE @AdminRoleIdForAccess int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');
DECLARE @SettingsMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.CONFIGURATION.SETTINGS');
DECLARE @SettingsFormIdForAccess int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.CONFIGURATION.SETTINGS');

IF @AdminRoleIdForAccess IS NOT NULL AND @SettingsMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleIdForAccess AND MenuId = @SettingsMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleIdForAccess, @SettingsMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleIdForAccess IS NOT NULL AND @SettingsFormIdForAccess IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleIdForAccess, @SettingsFormIdForAccess, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.Code IN (N'ACTION.REFRESH', N'ACTION.CREATE', N'ACTION.NEW', N'ACTION.COPY', N'ACTION.UPDATE', N'ACTION.EDIT', N'ACTION.DELETE', N'ACTION.CONSULT', N'ACTION.HISTORY')
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleIdForAccess
            AND existing.FormId = @SettingsFormIdForAccess
            AND existing.OperationId = operation.Id
      );
END;
GO
