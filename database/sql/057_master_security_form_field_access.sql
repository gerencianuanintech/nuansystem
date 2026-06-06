IF OBJECT_ID(N'dbo.SecurityRoleFormFields', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityRoleFormFields
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityRoleFormFields PRIMARY KEY,
        RoleId int NOT NULL,
        FormId int NOT NULL,
        FieldId int NOT NULL,
        IsVisible bit NOT NULL CONSTRAINT DF_SecurityRoleFormFields_IsVisible DEFAULT 1,
        IsEditable bit NOT NULL CONSTRAINT DF_SecurityRoleFormFields_IsEditable DEFAULT 1,
        IsRequired bit NOT NULL CONSTRAINT DF_SecurityRoleFormFields_IsRequired DEFAULT 0,
        IsReadOnly bit NOT NULL CONSTRAINT DF_SecurityRoleFormFields_IsReadOnly DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityRoleFormFields_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityRoleFormFields_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityRoleFormFields_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityRoleFormFields_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id),
        CONSTRAINT FK_SecurityRoleFormFields_SecurityForms FOREIGN KEY (FormId) REFERENCES dbo.SecurityForms(Id),
        CONSTRAINT FK_SecurityRoleFormFields_SecurityFields FOREIGN KEY (FieldId) REFERENCES dbo.SecurityFields(Id)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityRoleFormFields_Role_Form_Field_Active' AND object_id = OBJECT_ID(N'dbo.SecurityRoleFormFields'))
BEGIN
    CREATE UNIQUE INDEX UX_SecurityRoleFormFields_Role_Form_Field_Active
        ON dbo.SecurityRoleFormFields (RoleId, FormId, FieldId)
        WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SecurityRoleFormFields_Form_Role' AND object_id = OBJECT_ID(N'dbo.SecurityRoleFormFields'))
BEGIN
    CREATE INDEX IX_SecurityRoleFormFields_Form_Role
        ON dbo.SecurityRoleFormFields (FormId, RoleId, IsDeleted, IsActive);
END;
GO

IF OBJECT_ID(N'dbo.SecurityRoleDocumentSeriesField', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityRoleDocumentSeriesField
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityRoleDocumentSeriesField PRIMARY KEY,
        SecurityRoleDocumentSeriesId int NOT NULL,
        FieldId int NOT NULL,
        IsVisible bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesField_IsVisible DEFAULT 1,
        IsEditable bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesField_IsEditable DEFAULT 1,
        IsRequired bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesField_IsRequired DEFAULT 0,
        IsReadOnly bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesField_IsReadOnly DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesField_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesField_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesField_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityRoleDocumentSeriesField_Header FOREIGN KEY (SecurityRoleDocumentSeriesId) REFERENCES dbo.SecurityRoleDocumentSeries(Id),
        CONSTRAINT FK_SecurityRoleDocumentSeriesField_Field FOREIGN KEY (FieldId) REFERENCES dbo.SecurityFields(Id)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityRoleDocumentSeriesField_Header_Field_Active' AND object_id = OBJECT_ID(N'dbo.SecurityRoleDocumentSeriesField'))
BEGIN
    CREATE UNIQUE INDEX UX_SecurityRoleDocumentSeriesField_Header_Field_Active
        ON dbo.SecurityRoleDocumentSeriesField (SecurityRoleDocumentSeriesId, FieldId)
        WHERE IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMFIELDS_CAMPOS
    @RoleId int,
    @FormId int,
    @OnlyActive bit = 1,
    @Search nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        field.Id AS FieldId,
        field.FormId,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.FormKey,
        field.Code AS FieldCode,
        field.Name AS FieldName,
        field.FieldKey,
        field.Description,
        field.ControlType,
        field.DataType,
        field.IsVisible AS DefaultVisible,
        CASE WHEN field.IsReadOnly = 1 THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1) END AS DefaultEditable,
        field.IsRequired AS DefaultRequired,
        field.IsReadOnly AS DefaultReadOnly,
        field.DisplayOrder,
        COALESCE(access.IsVisible, field.IsVisible) AS IsVisible,
        COALESCE(access.IsEditable, CASE WHEN field.IsReadOnly = 1 THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1) END) AS IsEditable,
        COALESCE(access.IsRequired, field.IsRequired) AS IsRequired,
        COALESCE(access.IsReadOnly, field.IsReadOnly) AS IsReadOnly,
        COALESCE(access.IsActive, CONVERT(bit, 1)) AS IsActive,
        access.UpdatedByUserId,
        access.UpdatedByUserName,
        access.UpdatedAt,
        access.CreatedByUserId,
        access.CreatedByUserName,
        access.CreatedAt
    FROM dbo.SecurityFields field
    INNER JOIN dbo.SecurityForms form ON form.Id = field.FormId
        AND form.IsDeleted = 0
    LEFT JOIN dbo.SecurityRoleFormFields access ON access.RoleId = @RoleId
        AND access.FormId = field.FormId
        AND access.FieldId = field.Id
        AND access.IsDeleted = 0
    WHERE field.FormId = @FormId
      AND field.IsDeleted = 0
      AND (@OnlyActive = 0 OR field.IsActive = 1)
      AND
      (
          @Search IS NULL
          OR field.Code LIKE N'%' + @Search + N'%'
          OR field.Name LIKE N'%' + @Search + N'%'
          OR field.FieldKey LIKE N'%' + @Search + N'%'
          OR field.ControlType LIKE N'%' + @Search + N'%'
      )
    ORDER BY field.DisplayOrder, field.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEDOCUMENTSERIESFIELDS_CAMPOS
    @RoleId int,
    @CompanyCode nvarchar(50),
    @FormId int,
    @DocumentType nvarchar(50),
    @SecurityDocumentSeriesId int,
    @OnlyActive bit = 1,
    @Search nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FormKey nvarchar(120) =
    (
        SELECT TOP (1) FormKey
        FROM dbo.SecurityForms
        WHERE Id = @FormId
          AND IsDeleted = 0
    );

    DECLARE @HeaderId int =
    (
        SELECT TOP (1) Id
        FROM dbo.SecurityRoleDocumentSeries
        WHERE RoleId = @RoleId
          AND CompanyCode = @CompanyCode
          AND FormKey = @FormKey
          AND DocumentType = @DocumentType
          AND SecurityDocumentSeriesId = @SecurityDocumentSeriesId
          AND IsDeleted = 0
    );

    SELECT
        field.Id AS FieldId,
        field.FormId,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.FormKey,
        field.Code AS FieldCode,
        field.Name AS FieldName,
        field.FieldKey,
        field.Description,
        field.ControlType,
        field.DataType,
        field.IsVisible AS DefaultVisible,
        CASE WHEN field.IsReadOnly = 1 THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1) END AS DefaultEditable,
        field.IsRequired AS DefaultRequired,
        field.IsReadOnly AS DefaultReadOnly,
        field.DisplayOrder,
        COALESCE(access.IsVisible, field.IsVisible) AS IsVisible,
        COALESCE(access.IsEditable, CASE WHEN field.IsReadOnly = 1 THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1) END) AS IsEditable,
        COALESCE(access.IsRequired, field.IsRequired) AS IsRequired,
        COALESCE(access.IsReadOnly, field.IsReadOnly) AS IsReadOnly,
        COALESCE(access.IsActive, CONVERT(bit, 1)) AS IsActive,
        access.UpdatedByUserId,
        access.UpdatedByUserName,
        access.UpdatedAt,
        access.CreatedByUserId,
        access.CreatedByUserName,
        access.CreatedAt
    FROM dbo.SecurityFields field
    INNER JOIN dbo.SecurityForms form ON form.Id = field.FormId
        AND form.IsDeleted = 0
    LEFT JOIN dbo.SecurityRoleDocumentSeriesField access ON access.SecurityRoleDocumentSeriesId = @HeaderId
        AND access.FieldId = field.Id
        AND access.IsDeleted = 0
    WHERE field.FormId = @FormId
      AND field.IsDeleted = 0
      AND (@OnlyActive = 0 OR field.IsActive = 1)
      AND
      (
          @Search IS NULL
          OR field.Code LIKE N'%' + @Search + N'%'
          OR field.Name LIKE N'%' + @Search + N'%'
          OR field.FieldKey LIKE N'%' + @Search + N'%'
          OR field.ControlType LIKE N'%' + @Search + N'%'
      )
    ORDER BY field.DisplayOrder, field.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SECURITYROLEFORMFIELDS_GUARDAR
    @RoleId int,
    @FormId int,
    @FieldsJson nvarchar(max),
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now datetime2(0) = SYSUTCDATETIME();

    DECLARE @Fields TABLE
    (
        FieldId int NOT NULL PRIMARY KEY,
        IsVisible bit NOT NULL,
        IsEditable bit NOT NULL,
        IsRequired bit NOT NULL,
        IsReadOnly bit NOT NULL,
        IsActive bit NOT NULL
    );

    INSERT INTO @Fields (FieldId, IsVisible, IsEditable, IsRequired, IsReadOnly, IsActive)
    SELECT
        FieldId,
        IsVisible,
        IsEditable,
        IsRequired,
        IsReadOnly,
        IsActive
    FROM OPENJSON(@FieldsJson)
    WITH
    (
        FieldId int '$.fieldId',
        IsVisible bit '$.isVisible',
        IsEditable bit '$.isEditable',
        IsRequired bit '$.isRequired',
        IsReadOnly bit '$.isReadOnly',
        IsActive bit '$.isActive'
    )
    WHERE FieldId IS NOT NULL;

    MERGE dbo.SecurityRoleFormFields AS target
    USING
    (
        SELECT source.FieldId, source.IsVisible, source.IsEditable, source.IsRequired, source.IsReadOnly, source.IsActive
        FROM @Fields source
        INNER JOIN dbo.SecurityFields field ON field.Id = source.FieldId
            AND field.FormId = @FormId
            AND field.IsDeleted = 0
    ) AS source
    ON target.RoleId = @RoleId
        AND target.FormId = @FormId
        AND target.FieldId = source.FieldId
        AND target.IsDeleted = 0
    WHEN MATCHED THEN
        UPDATE SET
            IsVisible = source.IsVisible,
            IsEditable = source.IsEditable,
            IsRequired = source.IsRequired,
            IsReadOnly = source.IsReadOnly,
            IsActive = source.IsActive,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = @Now
    WHEN NOT MATCHED BY TARGET THEN
        INSERT
        (
            RoleId, FormId, FieldId, IsVisible, IsEditable, IsRequired, IsReadOnly, IsActive,
            CreatedByUserId, CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @RoleId, @FormId, source.FieldId, source.IsVisible, source.IsEditable, source.IsRequired, source.IsReadOnly, source.IsActive,
            @UpdatedByUserId, @UpdatedByUserName, @Now
        );

    SELECT CONVERT(bit, 1);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SECURITYROLEDOCUMENTSERIESFIELDS_GUARDAR
    @RoleId int,
    @CompanyCode nvarchar(50),
    @FormId int,
    @DocumentType nvarchar(50),
    @SecurityDocumentSeriesId int,
    @FieldsJson nvarchar(max),
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Now datetime2(0) = SYSUTCDATETIME();
    DECLARE @FormKey nvarchar(120) =
    (
        SELECT TOP (1) FormKey
        FROM dbo.SecurityForms
        WHERE Id = @FormId
          AND IsDeleted = 0
    );

    IF @FormKey IS NULL
    BEGIN
        SELECT CONVERT(bit, 0);
        RETURN;
    END;

    BEGIN TRANSACTION;

    DECLARE @HeaderId int;

    SELECT @HeaderId = Id
    FROM dbo.SecurityRoleDocumentSeries WITH (UPDLOCK, HOLDLOCK)
    WHERE RoleId = @RoleId
      AND CompanyCode = @CompanyCode
      AND FormKey = @FormKey
      AND DocumentType = @DocumentType
      AND SecurityDocumentSeriesId = @SecurityDocumentSeriesId;

    IF @HeaderId IS NULL
    BEGIN
        INSERT INTO dbo.SecurityRoleDocumentSeries
        (
            RoleId, CompanyCode, FormKey, SecurityDocumentSeriesId, DocumentType,
            IsActive, CreatedByUserId, CreatedByUserName, CreatedAt, IsDeleted
        )
        VALUES
        (
            @RoleId, @CompanyCode, @FormKey, @SecurityDocumentSeriesId, @DocumentType,
            1, @UpdatedByUserId, @UpdatedByUserName, @Now, 0
        );

        SET @HeaderId = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.SecurityRoleDocumentSeries
        SET IsActive = 1,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = @Now
        WHERE Id = @HeaderId;
    END;

    DECLARE @Fields TABLE
    (
        FieldId int NOT NULL PRIMARY KEY,
        IsVisible bit NOT NULL,
        IsEditable bit NOT NULL,
        IsRequired bit NOT NULL,
        IsReadOnly bit NOT NULL,
        IsActive bit NOT NULL
    );

    INSERT INTO @Fields (FieldId, IsVisible, IsEditable, IsRequired, IsReadOnly, IsActive)
    SELECT
        FieldId,
        IsVisible,
        IsEditable,
        IsRequired,
        IsReadOnly,
        IsActive
    FROM OPENJSON(@FieldsJson)
    WITH
    (
        FieldId int '$.fieldId',
        IsVisible bit '$.isVisible',
        IsEditable bit '$.isEditable',
        IsRequired bit '$.isRequired',
        IsReadOnly bit '$.isReadOnly',
        IsActive bit '$.isActive'
    )
    WHERE FieldId IS NOT NULL;

    MERGE dbo.SecurityRoleDocumentSeriesField AS target
    USING
    (
        SELECT source.FieldId, source.IsVisible, source.IsEditable, source.IsRequired, source.IsReadOnly, source.IsActive
        FROM @Fields source
        INNER JOIN dbo.SecurityFields field ON field.Id = source.FieldId
            AND field.FormId = @FormId
            AND field.IsDeleted = 0
    ) AS source
    ON target.SecurityRoleDocumentSeriesId = @HeaderId
        AND target.FieldId = source.FieldId
        AND target.IsDeleted = 0
    WHEN MATCHED THEN
        UPDATE SET
            IsVisible = source.IsVisible,
            IsEditable = source.IsEditable,
            IsRequired = source.IsRequired,
            IsReadOnly = source.IsReadOnly,
            IsActive = source.IsActive,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = @Now
    WHEN NOT MATCHED BY TARGET THEN
        INSERT
        (
            SecurityRoleDocumentSeriesId, FieldId, IsVisible, IsEditable, IsRequired, IsReadOnly, IsActive,
            CreatedByUserId, CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @HeaderId, source.FieldId, source.IsVisible, source.IsEditable, source.IsRequired, source.IsReadOnly, source.IsActive,
            @UpdatedByUserId, @UpdatedByUserName, @Now
        );

    COMMIT TRANSACTION;

    SELECT CONVERT(bit, 1);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMFIELDS_USUARIO
    @UserId int,
    @FormKey nvarchar(120),
    @FieldKey nvarchar(120)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        COALESCE(access.IsVisible, field.IsVisible) AS IsVisible,
        COALESCE(access.IsEditable, CASE WHEN field.IsReadOnly = 1 THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1) END) AS IsEditable,
        COALESCE(access.IsRequired, field.IsRequired) AS IsRequired,
        COALESCE(access.IsReadOnly, field.IsReadOnly) AS IsReadOnly
    FROM dbo.UserRoles userRole
    INNER JOIN dbo.Roles role ON role.Id = userRole.RoleId
        AND role.IsActive = 1
    INNER JOIN dbo.SecurityForms form ON form.FormKey = @FormKey
        AND form.IsActive = 1
        AND form.IsDeleted = 0
    INNER JOIN dbo.SecurityFields field ON field.FormId = form.Id
        AND field.FieldKey = @FieldKey
        AND field.IsActive = 1
        AND field.IsDeleted = 0
    LEFT JOIN dbo.SecurityRoleFormFields access ON access.RoleId = role.Id
        AND access.FormId = form.Id
        AND access.FieldId = field.Id
        AND access.IsActive = 1
        AND access.IsDeleted = 0
    WHERE userRole.UserId = @UserId
    ORDER BY
        COALESCE(access.IsVisible, field.IsVisible) DESC,
        COALESCE(access.IsEditable, CASE WHEN field.IsReadOnly = 1 THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1) END) DESC;
END;
GO

IF COL_LENGTH(N'dbo.SecurityForms', N'HasListView') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityForms
    ADD HasListView bit NOT NULL CONSTRAINT DF_SecurityForms_HasListView DEFAULT 1;
END;
GO

IF COL_LENGTH(N'dbo.SecurityForms', N'HasEditView') IS NULL
BEGIN
    ALTER TABLE dbo.SecurityForms
    ADD HasEditView bit NOT NULL CONSTRAINT DF_SecurityForms_HasEditView DEFAULT 1;
END;
GO

DECLARE @SecurityMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY');

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FIELDACCESS.MAINTENANCE')
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, HasListView, HasEditView,
        IsVisible, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.SECURITY.FIELDACCESS.MAINTENANCE',
        N'Accesos a Campos de Formularios de Mantenimiento',
        N'Configuracion de acceso a campos por rol para formularios de mantenimiento',
        N'security-field-access-maintenance',
        1, 1, 0, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FIELDACCESS.TRANSACTIONAL')
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, HasListView, HasEditView,
        IsVisible, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.SECURITY.FIELDACCESS.TRANSACTIONAL',
        N'Accesos a Campos de Formularios Transaccionales',
        N'Configuracion de acceso a campos por rol para formularios transaccionales',
        N'security-field-access-transactional',
        2, 1, 0, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityForms
SET Name = N'Accesos a Campos de Formularios de Mantenimiento',
    Description = N'Configuracion de acceso a campos por rol para formularios de mantenimiento',
    FormKey = N'security-field-access-maintenance',
    FormType = 1,
    HasListView = 1,
    HasEditView = 0,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.SECURITY.FIELDACCESS.MAINTENANCE';

UPDATE dbo.SecurityForms
SET Name = N'Accesos a Campos de Formularios Transaccionales',
    Description = N'Configuracion de acceso a campos por rol para formularios transaccionales',
    FormKey = N'security-field-access-transactional',
    FormType = 2,
    HasListView = 1,
    HasEditView = 0,
    IsVisible = 1,
    IsActive = 1
WHERE Code = N'FORM.SECURITY.FIELDACCESS.TRANSACTIONAL';
GO

DECLARE @SecurityMenuIdForFields int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY');

IF @SecurityMenuIdForFields IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.FIELDACCESS.MAINTENANCE')
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @SecurityMenuIdForFields,
            N'MENU.SECURITY.FIELDACCESS.MAINTENANCE',
            N'Campos Mant.',
            N'Accesos a campos de formularios de mantenimiento',
            3,
            N'security-field-access-maintenance',
            N'Accordion/campos_32.svg',
            N'Accordion/campos_16.svg',
            62,
            1,
            1,
            N'Sistema',
            SYSUTCDATETIME()
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.FIELDACCESS.TRANSACTIONAL')
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @SecurityMenuIdForFields,
            N'MENU.SECURITY.FIELDACCESS.TRANSACTIONAL',
            N'Campos Trans.',
            N'Accesos a campos de formularios transaccionales',
            3,
            N'security-field-access-transactional',
            N'Accordion/campos_32.svg',
            N'Accordion/campos_16.svg',
            63,
            1,
            1,
            N'Sistema',
            SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET Name = N'Campos Mant.',
        FormKey = N'security-field-access-maintenance',
        DisplayOrder = 62,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.SECURITY.FIELDACCESS.MAINTENANCE';

    UPDATE dbo.SecurityMenus
    SET Name = N'Campos Trans.',
        FormKey = N'security-field-access-transactional',
        DisplayOrder = 63,
        IsVisible = 1,
        IsActive = 1
    WHERE Code = N'MENU.SECURITY.FIELDACCESS.TRANSACTIONAL';
END;
GO

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');
DECLARE @MaintenanceMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.FIELDACCESS.MAINTENANCE');
DECLARE @TransactionalMenuId int = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.SECURITY.FIELDACCESS.TRANSACTIONAL');
DECLARE @MaintenanceFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FIELDACCESS.MAINTENANCE');
DECLARE @TransactionalFormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.SECURITY.FIELDACCESS.TRANSACTIONAL');

IF @AdminRoleId IS NOT NULL AND @MaintenanceMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleId AND MenuId = @MaintenanceMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, @MaintenanceMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleId IS NOT NULL AND @TransactionalMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityRoleMenus WHERE RoleId = @AdminRoleId AND MenuId = @TransactionalMenuId)
    BEGIN
        INSERT INTO dbo.SecurityRoleMenus (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, @TransactionalMenuId, 1, N'Sistema', SYSUTCDATETIME());
    END;
END;

IF @AdminRoleId IS NOT NULL AND @MaintenanceFormId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, @MaintenanceFormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.IsActive = 1
      AND operation.ActionKey IN (N'VIEW', N'EDIT', N'SAVE', N'REFRESH', N'EXPORT', N'HISTORY')
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = @MaintenanceFormId
            AND existing.OperationId = operation.Id
      );
END;

IF @AdminRoleId IS NOT NULL AND @TransactionalFormId IS NOT NULL
BEGIN
    INSERT INTO dbo.SecurityRoleFormOperations (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
    SELECT @AdminRoleId, @TransactionalFormId, operation.Id, 1, N'Sistema', SYSUTCDATETIME()
    FROM dbo.SecurityOperations operation
    WHERE operation.IsActive = 1
      AND operation.ActionKey IN (N'VIEW', N'EDIT', N'SAVE', N'REFRESH', N'EXPORT', N'HISTORY')
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations existing
          WHERE existing.RoleId = @AdminRoleId
            AND existing.FormId = @TransactionalFormId
            AND existing.OperationId = operation.Id
      );
END;
GO
