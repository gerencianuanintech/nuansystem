IF OBJECT_ID(N'dbo.SecurityFields', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityFields
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityFields PRIMARY KEY,
        FormId int NOT NULL,
        Code nvarchar(80) NOT NULL,
        Name nvarchar(120) NOT NULL,
        FieldKey nvarchar(120) NOT NULL,
        Description nvarchar(300) NULL,
        ControlType nvarchar(60) NOT NULL,
        DataType nvarchar(40) NOT NULL,
        IsRequired bit NOT NULL CONSTRAINT DF_SecurityFields_IsRequired DEFAULT 0,
        ValidationMessage nvarchar(300) NULL,
        IsReadOnly bit NOT NULL CONSTRAINT DF_SecurityFields_IsReadOnly DEFAULT 0,
        IsVisible bit NOT NULL CONSTRAINT DF_SecurityFields_IsVisible DEFAULT 1,
        IsCustom bit NOT NULL CONSTRAINT DF_SecurityFields_IsCustom DEFAULT 0,
        DisplayOrder int NOT NULL CONSTRAINT DF_SecurityFields_DisplayOrder DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityFields_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityFields_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityFields_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityFields_SecurityForms FOREIGN KEY (FormId) REFERENCES dbo.SecurityForms(Id),
        CONSTRAINT CK_SecurityFields_DisplayOrder CHECK (DisplayOrder >= 0),
        CONSTRAINT CK_SecurityFields_DataType CHECK (DataType IN (N'string', N'int', N'decimal', N'date', N'datetime', N'bool', N'image', N'guid'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityFields_Code_Active' AND object_id = OBJECT_ID(N'dbo.SecurityFields'))
BEGIN
    CREATE UNIQUE INDEX UX_SecurityFields_Code_Active ON dbo.SecurityFields (Code) WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityFields_Form_FieldKey_Active' AND object_id = OBJECT_ID(N'dbo.SecurityFields'))
BEGIN
    CREATE UNIQUE INDEX UX_SecurityFields_Form_FieldKey_Active ON dbo.SecurityFields (FormId, FieldKey) WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SecurityFields_Form_Order' AND object_id = OBJECT_ID(N'dbo.SecurityFields'))
BEGIN
    CREATE INDEX IX_SecurityFields_Form_Order ON dbo.SecurityFields (FormId, IsDeleted, DisplayOrder, Name);
END;
GO

IF OBJECT_ID(N'dbo.AuditSecurityChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSecurityChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSecurityChanges PRIMARY KEY,
        EntityName nvarchar(120) NOT NULL,
        RecordId nvarchar(80) NOT NULL,
        [Action] nvarchar(30) NOT NULL,
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CAMPOSEGURIDADLISTAR
AS
BEGIN
    SELECT
        field.Id,
        field.FormId,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.FormKey,
        field.Code,
        field.Name,
        field.FieldKey,
        field.Description,
        field.ControlType,
        field.DataType,
        field.IsRequired,
        field.ValidationMessage,
        field.IsReadOnly,
        field.IsVisible,
        field.IsCustom,
        field.DisplayOrder,
        field.IsActive,
        field.CreatedByUserId,
        field.CreatedByUserName,
        field.CreatedAt,
        field.UpdatedByUserId,
        field.UpdatedByUserName,
        field.UpdatedAt,
        field.DeletedByUserId,
        field.DeletedByUserName,
        field.DeletedAt
    FROM dbo.SecurityFields field
    INNER JOIN dbo.SecurityForms form ON form.Id = field.FormId
        AND form.IsDeleted = 0
    WHERE field.IsDeleted = 0
    ORDER BY form.Name, field.DisplayOrder, field.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CAMPOSEGURIDADBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        field.Id,
        field.FormId,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.FormKey,
        field.Code,
        field.Name,
        field.FieldKey,
        field.Description,
        field.ControlType,
        field.DataType,
        field.IsRequired,
        field.ValidationMessage,
        field.IsReadOnly,
        field.IsVisible,
        field.IsCustom,
        field.DisplayOrder,
        field.IsActive,
        field.CreatedByUserId,
        field.CreatedByUserName,
        field.CreatedAt,
        field.UpdatedByUserId,
        field.UpdatedByUserName,
        field.UpdatedAt,
        field.DeletedByUserId,
        field.DeletedByUserName,
        field.DeletedAt
    FROM dbo.SecurityFields field
    INNER JOIN dbo.SecurityForms form ON form.Id = field.FormId
        AND form.IsDeleted = 0
    WHERE field.Id = @Id
      AND field.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CAMPOSEGURIDADBUSCARPORCODIGO
    @Code nvarchar(80),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.SecurityFields
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CAMPOSEGURIDADBUSCARPORCLAVE
    @FormId int,
    @FieldKey nvarchar(120),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.SecurityFields
    WHERE FormId = @FormId
      AND FieldKey = @FieldKey
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CAMPOSEGURIDADCREAR
    @FormId int,
    @Code nvarchar(80),
    @Name nvarchar(120),
    @FieldKey nvarchar(120),
    @Description nvarchar(300) = NULL,
    @ControlType nvarchar(60),
    @DataType nvarchar(40),
    @IsRequired bit = 0,
    @ValidationMessage nvarchar(300) = NULL,
    @IsReadOnly bit = 0,
    @IsVisible bit = 1,
    @IsCustom bit = 0,
    @DisplayOrder int = 0,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    INSERT INTO dbo.SecurityFields
    (
        FormId, Code, Name, FieldKey, Description, ControlType, DataType,
        IsRequired, ValidationMessage, IsReadOnly, IsVisible, IsCustom,
        DisplayOrder, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @FormId, @Code, @Name, @FieldKey, @Description, @ControlType, @DataType,
        @IsRequired, @ValidationMessage, @IsReadOnly, @IsVisible, @IsCustom,
        @DisplayOrder, @IsActive, @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME()
    );

    DECLARE @Id int = CAST(SCOPE_IDENTITY() AS int);

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityFields', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @CreatedByUserId, @CreatedByUserName
    FROM
    (
        VALUES
            (N'FormId', CONVERT(nvarchar(max), @FormId)),
            (N'Code', CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @Name)),
            (N'FieldKey', CONVERT(nvarchar(max), @FieldKey)),
            (N'Description', CONVERT(nvarchar(max), @Description)),
            (N'ControlType', CONVERT(nvarchar(max), @ControlType)),
            (N'DataType', CONVERT(nvarchar(max), @DataType)),
            (N'IsRequired', CONVERT(nvarchar(max), CONVERT(int, @IsRequired))),
            (N'ValidationMessage', CONVERT(nvarchar(max), @ValidationMessage)),
            (N'IsReadOnly', CONVERT(nvarchar(max), CONVERT(int, @IsReadOnly))),
            (N'IsVisible', CONVERT(nvarchar(max), CONVERT(int, @IsVisible))),
            (N'IsCustom', CONVERT(nvarchar(max), CONVERT(int, @IsCustom))),
            (N'DisplayOrder', CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, NewValue)
    WHERE NewValue IS NOT NULL;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CAMPOSEGURIDADACTUALIZAR
    @Id int,
    @FormId int,
    @Code nvarchar(80),
    @Name nvarchar(120),
    @FieldKey nvarchar(120),
    @Description nvarchar(300) = NULL,
    @ControlType nvarchar(60),
    @DataType nvarchar(40),
    @IsRequired bit = 0,
    @ValidationMessage nvarchar(300) = NULL,
    @IsReadOnly bit = 0,
    @IsVisible bit = 1,
    @IsCustom bit = 0,
    @DisplayOrder int = 0,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    DECLARE
        @OldFormId int,
        @OldCode nvarchar(80),
        @OldName nvarchar(120),
        @OldFieldKey nvarchar(120),
        @OldDescription nvarchar(300),
        @OldControlType nvarchar(60),
        @OldDataType nvarchar(40),
        @OldIsRequired bit,
        @OldValidationMessage nvarchar(300),
        @OldIsReadOnly bit,
        @OldIsVisible bit,
        @OldIsCustom bit,
        @OldDisplayOrder int,
        @OldIsActive bit;

    SELECT
        @OldFormId = FormId,
        @OldCode = Code,
        @OldName = Name,
        @OldFieldKey = FieldKey,
        @OldDescription = Description,
        @OldControlType = ControlType,
        @OldDataType = DataType,
        @OldIsRequired = IsRequired,
        @OldValidationMessage = ValidationMessage,
        @OldIsReadOnly = IsReadOnly,
        @OldIsVisible = IsVisible,
        @OldIsCustom = IsCustom,
        @OldDisplayOrder = DisplayOrder,
        @OldIsActive = IsActive
    FROM dbo.SecurityFields
    WHERE Id = @Id
      AND IsDeleted = 0;

    IF @OldCode IS NULL
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.SecurityFields
    SET
        FormId = @FormId,
        Code = @Code,
        Name = @Name,
        FieldKey = @FieldKey,
        Description = @Description,
        ControlType = @ControlType,
        DataType = @DataType,
        IsRequired = @IsRequired,
        ValidationMessage = @ValidationMessage,
        IsReadOnly = @IsReadOnly,
        IsVisible = @IsVisible,
        IsCustom = @IsCustom,
        DisplayOrder = @DisplayOrder,
        IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    INSERT INTO dbo.AuditSecurityChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'SecurityFields', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM
    (
        VALUES
            (N'FormId', CONVERT(nvarchar(max), @OldFormId), CONVERT(nvarchar(max), @FormId)),
            (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
            (N'FieldKey', CONVERT(nvarchar(max), @OldFieldKey), CONVERT(nvarchar(max), @FieldKey)),
            (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
            (N'ControlType', CONVERT(nvarchar(max), @OldControlType), CONVERT(nvarchar(max), @ControlType)),
            (N'DataType', CONVERT(nvarchar(max), @OldDataType), CONVERT(nvarchar(max), @DataType)),
            (N'IsRequired', CONVERT(nvarchar(max), CONVERT(int, @OldIsRequired)), CONVERT(nvarchar(max), CONVERT(int, @IsRequired))),
            (N'ValidationMessage', CONVERT(nvarchar(max), @OldValidationMessage), CONVERT(nvarchar(max), @ValidationMessage)),
            (N'IsReadOnly', CONVERT(nvarchar(max), CONVERT(int, @OldIsReadOnly)), CONVERT(nvarchar(max), CONVERT(int, @IsReadOnly))),
            (N'IsVisible', CONVERT(nvarchar(max), CONVERT(int, @OldIsVisible)), CONVERT(nvarchar(max), CONVERT(int, @IsVisible))),
            (N'IsCustom', CONVERT(nvarchar(max), CONVERT(int, @OldIsCustom)), CONVERT(nvarchar(max), CONVERT(int, @IsCustom))),
            (N'DisplayOrder', CONVERT(nvarchar(max), @OldDisplayOrder), CONVERT(nvarchar(max), @DisplayOrder)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_CAMPOSEGURIDADELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    UPDATE dbo.SecurityFields
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
        VALUES (N'SecurityFields', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);
    END;

    SELECT @AffectedRows;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FIELDS')
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.SECURITY.FIELDS', N'Campos', N'Mantenimiento de campos de seguridad por formulario',
        N'security-fields', 1, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityForms
SET Name = N'Campos',
    Description = N'Mantenimiento de campos de seguridad por formulario',
    FormKey = N'security-fields',
    FormType = 1,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.SECURITY.FIELDS';
GO

DECLARE @SecurityMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY');
DECLARE @FieldsFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FIELDS');

IF @SecurityMenuId IS NOT NULL AND @FieldsFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.FIELDS')
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @SecurityMenuId, N'MENU.SECURITY.FIELDS', N'Campos',
            N'Administrar campos de seguridad por formulario',
            3, N'security-fields',
            N'Accordion/campos_32.svg', N'Accordion/campos_16.svg',
            45, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET FormKey = N'security-fields',
        IconLarge = N'Accordion/campos_32.svg',
        IconSmall = N'Accordion/campos_16.svg',
        DisplayOrder = 45,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.SECURITY.FIELDS';
END;
GO

DECLARE @AdminRoleIdForAccess int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');
DECLARE @FieldsMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.FIELDS');
DECLARE @FieldsFormIdForAccess int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FIELDS');

IF @AdminRoleIdForAccess IS NOT NULL AND @FieldsMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleIdForAccess AND MenuId = @FieldsMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleIdForAccess, @FieldsMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleIdForAccess IS NOT NULL AND @FieldsFormIdForAccess IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleIdForAccess, @FieldsFormIdForAccess, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.IsActive = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleIdForAccess
            AND existing.FormId = @FieldsFormIdForAccess
            AND existing.OperationId = operation.Id
      );
END;
GO
