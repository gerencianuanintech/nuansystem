/*
    Migracion 179 - Aplicabilidad de operaciones por formulario.

    Separa el catalogo global de operaciones de las operaciones que realmente
    pertenecen a cada formulario. Canonicaliza create/update sin sobreescribir
    decisiones explicitas y limita las acciones especializadas a su propietario.
*/
USE [NuanSystem_Master];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51179, 'Migration 179 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51179, 'MasterSchemaHistory is required before migration 179.', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260805.178')
    THROW 51179, 'Migration 178 is required before migration 179.', 1;
IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityRoleFormOperations', N'U') IS NULL
    THROW 51179, 'Security form-operation schema is required before migration 179.', 1;
GO

IF OBJECT_ID(N'dbo.SecurityFormOperations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityFormOperations
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityFormOperations PRIMARY KEY,
        FormId int NOT NULL,
        OperationId int NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityFormOperations_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityFormOperations_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityFormOperations_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityFormOperations_Form FOREIGN KEY (FormId) REFERENCES dbo.SecurityForms(Id),
        CONSTRAINT FK_SecurityFormOperations_Operation FOREIGN KEY (OperationId) REFERENCES dbo.SecurityOperations(Id),
        CONSTRAINT UQ_SecurityFormOperations_Form_Operation UNIQUE (FormId, OperationId)
    );

    CREATE INDEX IX_SecurityFormOperations_Operation
        ON dbo.SecurityFormOperations(OperationId, FormId)
        INCLUDE(IsActive, IsDeleted);
END;
GO

BEGIN TRY
BEGIN TRANSACTION;

DECLARE @CreateOperationId int =
(
    SELECT TOP (1) Id FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.CREATE' AND IsDeleted = 0
);
DECLARE @NewOperationId int =
(
    SELECT TOP (1) Id FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.NEW' AND IsDeleted = 0
);
DECLARE @UpdateOperationId int =
(
    SELECT TOP (1) Id FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.UPDATE' AND IsDeleted = 0
);
DECLARE @EditOperationId int =
(
    SELECT TOP (1) Id FROM dbo.SecurityOperations
    WHERE Code = N'ACTION.EDIT' AND IsDeleted = 0
);

IF @CreateOperationId IS NULL OR @UpdateOperationId IS NULL
    THROW 51179, 'Canonical CREATE and UPDATE operations are required.', 1;

/*
    Copia aliases legados solo cuando no existe una decision canonica previa.
    Una denegacion explicita en CREATE/UPDATE tiene precedencia.
*/
IF @NewOperationId IS NOT NULL
BEGIN
    INSERT dbo.SecurityRoleFormOperations
    (
        RoleId, FormId, OperationId, IsAllowed,
        CreatedByUserId, CreatedByUserName, CreatedAt
    )
    SELECT
        legacy.RoleId, legacy.FormId, @CreateOperationId, legacy.IsAllowed,
        legacy.CreatedByUserId, COALESCE(legacy.CreatedByUserName, N'Sistema'), SYSUTCDATETIME()
    FROM dbo.SecurityRoleFormOperations legacy
    WHERE legacy.OperationId = @NewOperationId
      AND legacy.IsDeleted = 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations canonical
          WHERE canonical.RoleId = legacy.RoleId
            AND canonical.FormId = legacy.FormId
            AND canonical.OperationId = @CreateOperationId
      );
END;

IF @EditOperationId IS NOT NULL
BEGIN
    INSERT dbo.SecurityRoleFormOperations
    (
        RoleId, FormId, OperationId, IsAllowed,
        CreatedByUserId, CreatedByUserName, CreatedAt
    )
    SELECT
        legacy.RoleId, legacy.FormId, @UpdateOperationId, legacy.IsAllowed,
        legacy.CreatedByUserId, COALESCE(legacy.CreatedByUserName, N'Sistema'), SYSUTCDATETIME()
    FROM dbo.SecurityRoleFormOperations legacy
    WHERE legacy.OperationId = @EditOperationId
      AND legacy.IsDeleted = 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SecurityRoleFormOperations canonical
          WHERE canonical.RoleId = legacy.RoleId
            AND canonical.FormId = legacy.FormId
            AND canonical.OperationId = @UpdateOperationId
      );
END;

DECLARE @Applicable table
(
    FormId int NOT NULL,
    OperationId int NOT NULL,
    PRIMARY KEY(FormId, OperationId)
);

/*
    Acciones corporativas comunes con evidencia historica de asignacion.
    La lista cerrada evita heredar los antiguos CROSS JOIN con todo el catalogo global.
*/
INSERT @Applicable(FormId, OperationId)
SELECT DISTINCT roleOperation.FormId, roleOperation.OperationId
FROM dbo.SecurityRoleFormOperations roleOperation
INNER JOIN dbo.SecurityForms form
    ON form.Id = roleOperation.FormId
   AND form.IsDeleted = 0
INNER JOIN dbo.SecurityOperations operation
    ON operation.Id = roleOperation.OperationId
   AND operation.IsDeleted = 0
WHERE roleOperation.IsDeleted = 0
  AND (roleOperation.IsAllowed = 1 OR roleOperation.CreatedByUserName = N'Sistema')
  AND operation.Code IN
  (
      N'ACTION.REFRESH', N'ACTION.CONSULT', N'ACTION.CREATE',
      N'ACTION.UPDATE', N'ACTION.DELETE', N'ACTION.COPY', N'ACTION.HISTORY',
      N'ACTION.CUSTOMIZE_COLUMNS', N'ACTION.EXPORT_EXCEL', N'ACTION.EXPORT_PDF',
      N'ACTION.EXPORT_JSON', N'ACTION.EXPORT_XML'
  )
  AND form.FormKey NOT IN
  (
      N'security-access',
      N'security-form-access-transactional',
      N'security-field-access-maintenance',
      N'security-field-access-transactional',
      N'users',
      N'security-roles',
      N'security-operations',
      N'security-menus',
      N'security-forms',
      N'security-fields'
  );

/* Los formularios de administracion de accesos usan solamente acciones CRUD comunes. */
INSERT @Applicable(FormId, OperationId)
SELECT form.Id, operation.Id
FROM dbo.SecurityForms form
CROSS JOIN dbo.SecurityOperations operation
WHERE form.FormKey IN
(
    N'security-access',
    N'security-form-access-transactional',
    N'security-field-access-maintenance',
    N'security-field-access-transactional',
    N'users',
    N'security-roles',
    N'security-operations',
    N'security-menus',
    N'security-forms',
    N'security-fields'
)
AND form.IsDeleted = 0
AND operation.IsDeleted = 0
AND operation.Code IN
(
    N'ACTION.REFRESH', N'ACTION.CONSULT', N'ACTION.CREATE',
    N'ACTION.UPDATE', N'ACTION.DELETE', N'ACTION.HISTORY'
)
AND NOT EXISTS
(
    SELECT 1 FROM @Applicable existing
    WHERE existing.FormId = form.Id AND existing.OperationId = operation.Id
);

/* Scopes especializados declarados por su formulario propietario. */
DECLARE @Scoped table
(
    FormKey nvarchar(120) NOT NULL,
    OperationCode nvarchar(120) NOT NULL,
    PRIMARY KEY(FormKey, OperationCode)
);

INSERT @Scoped(FormKey, OperationCode)
VALUES
    (N'sap-sync-profiles', N'ACTION.SAP_SYNC_PROFILES.VALIDATE'),
    (N'sap-sync-profiles', N'ACTION.SAP_SYNC_PROFILES.ACTIVATE'),
    (N'sap-sync-profiles', N'ACTION.SAP_SYNC_PROFILES.DEACTIVATE'),
    (N'sap-sync-executions', N'ACTION.SAP_SYNC_EXECUTIONS.RETRY'),
    (N'sap-sync-executions', N'ACTION.SAP_SYNC_EXECUTIONS.CANCEL'),
    (N'sap-sync-executions', N'ACTION.SAP_SYNC_EXECUTIONS.RELEASE_EXPIRED_LOCK'),
    (N'sri-txt-imports', N'ACTION.SRI_TXT_IMPORTS.UPLOAD'),
    (N'sri-txt-imports', N'ACTION.SRI_TXT_IMPORTS.ENQUEUE'),
    (N'sri-txt-imports', N'ACTION.SRI_TXT_IMPORTS.OPEN_QUEUE'),
    (N'sri-document-monitor', N'ACTION.DOWNLOAD_XML'),
    (N'sri-document-monitor', N'ACTION.VIEW_WORKER_HEALTH'),
    (N'sri-document-monitor', N'ACTION.FILTER'),
    (N'sri-txt-imports', N'ACTION.FILTER'),
    (N'sap-sync-profiles', N'ACTION.FILTER'),
    (N'sap-sync-profiles', N'ACTION.VIEWEXECUTIONS'),
    (N'sap-sync-executions', N'ACTION.FILTER'),
    (N'sync-profiles', N'ACTION.ACTIVATE'),
    (N'sync-profiles', N'ACTION.DEACTIVATE'),
    (N'sync-profiles', N'ACTION.EXECUTE'),
    (N'sync-profiles', N'ACTION.VIEWEXECUTIONS'),
    (N'sync-profiles', N'ACTION.FILTER'),
    (N'sync-profiles', N'ACTION.VALIDATE'),
    (N'sync-executions', N'ACTION.CANCEL'),
    (N'sync-executions', N'ACTION.RETRY'),
    (N'sync-monitor', N'ACTION.EXPORT'),
    (N'purchase-orders', N'ACTION.APPROVE'),
    (N'purchase-orders', N'ACTION.REJECT'),
    (N'purchase-orders', N'ACTION.SYNC_SAP'),
    (N'purchase-orders', N'ACTION.CANCEL'),
    (N'purchase-orders', N'ACTION.REOPEN'),
    (N'purchase-orders', N'ACTION.PRINT'),
    (N'purchase-orders', N'ACTION.EXPORT'),
    (N'purchase-orders-edit', N'ACTION.APPROVE'),
    (N'purchase-orders-edit', N'ACTION.REJECT'),
    (N'purchase-orders-edit', N'ACTION.SYNC_SAP'),
    (N'purchase-orders-edit', N'ACTION.CANCEL'),
    (N'purchase-orders-edit', N'ACTION.REOPEN'),
    (N'purchase-orders-edit', N'ACTION.PRINT'),
    (N'purchase-orders-edit', N'ACTION.EXPORT');

INSERT @Applicable(FormId, OperationId)
SELECT form.Id, operation.Id
FROM @Scoped scoped
INNER JOIN dbo.SecurityForms form
    ON form.FormKey = scoped.FormKey
   AND form.IsDeleted = 0
INNER JOIN dbo.SecurityOperations operation
    ON operation.Code = scoped.OperationCode
   AND operation.IsDeleted = 0
WHERE NOT EXISTS
(
    SELECT 1 FROM @Applicable existing
    WHERE existing.FormId = form.Id AND existing.OperationId = operation.Id
);

MERGE dbo.SecurityFormOperations AS target
USING @Applicable AS source
    ON target.FormId = source.FormId
   AND target.OperationId = source.OperationId
WHEN MATCHED THEN
    UPDATE SET
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT(FormId, OperationId, IsActive, CreatedByUserName, CreatedAt)
    VALUES(source.FormId, source.OperationId, 1, N'Sistema', SYSUTCDATETIME());

/* Desactiva las concesiones SAP que 178 asocio por ActionKey a Geografia. */
UPDATE roleOperation
SET IsAllowed = 0,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
FROM dbo.SecurityRoleFormOperations roleOperation
INNER JOIN dbo.SecurityForms form ON form.Id = roleOperation.FormId
INNER JOIN dbo.SecurityOperations operation ON operation.Id = roleOperation.OperationId
WHERE form.FormKey IN (N'countries', N'provinces', N'cities')
  AND operation.Code LIKE N'ACTION.SAP[_]SYNC[_]%'
  AND roleOperation.IsDeleted = 0
  AND roleOperation.IsAllowed = 1;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260805.179'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES(N'20260805.179', N'Aplicabilidad canonica de operaciones por formulario');
END;

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SEGURIDADOPERACIONESUSUARIO
    @UserId int,
    @FormKey nvarchar(120)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        operation.Id AS OperationId,
        operation.Code,
        operation.Name,
        operation.Description,
        operation.ActionKey,
        operation.RibbonPageName,
        operation.RibbonGroupName,
        operation.IconLarge,
        operation.IconSmall,
        operation.DisplayOrder,
        CAST(CASE WHEN EXISTS
        (
            SELECT 1
            FROM dbo.UserRoles userRole
            INNER JOIN dbo.SecurityRoleFormOperations roleOperation
                ON roleOperation.RoleId = userRole.RoleId
               AND roleOperation.FormId = form.Id
               AND roleOperation.OperationId = operation.Id
               AND roleOperation.IsDeleted = 0
               AND roleOperation.IsAllowed = 1
            WHERE userRole.UserId = @UserId
        ) THEN 1 ELSE 0 END AS bit) AS IsAllowed
    FROM dbo.SecurityForms form
    INNER JOIN dbo.SecurityFormOperations formOperation
        ON formOperation.FormId = form.Id
       AND formOperation.IsActive = 1
       AND formOperation.IsDeleted = 0
    INNER JOIN dbo.SecurityOperations operation
        ON operation.Id = formOperation.OperationId
       AND operation.IsActive = 1
       AND operation.IsDeleted = 0
    WHERE form.FormKey = @FormKey
      AND form.IsActive = 1
      AND form.IsDeleted = 0
    ORDER BY operation.DisplayOrder, operation.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ACCESOROLCARGAR
    @RoleId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        menu.Id AS MenuId,
        menu.ParentId,
        menu.Code,
        menu.Name,
        CAST(menu.MenuType AS int) AS MenuType,
        COALESCE(form.FormKey, menu.FormKey) AS FormKey,
        CAST(CASE WHEN roleMenu.IsAllowed = 1 THEN 1 ELSE 0 END AS bit) AS IsAllowed
    FROM dbo.SecurityMenus menu
    LEFT JOIN dbo.SecurityForms form ON form.Id = menu.FormId AND form.IsDeleted = 0
    LEFT JOIN dbo.SecurityRoleMenus roleMenu ON roleMenu.MenuId = menu.Id
        AND roleMenu.RoleId = @RoleId
        AND roleMenu.IsDeleted = 0
    WHERE menu.IsDeleted = 0
    ORDER BY menu.ParentId, menu.DisplayOrder, menu.Name;

    SELECT
        form.Id AS FormId,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.FormKey,
        operation.Id AS OperationId,
        operation.Code AS OperationCode,
        operation.Name AS OperationName,
        operation.ActionKey,
        CAST(CASE WHEN roleOperation.IsAllowed = 1 THEN 1 ELSE 0 END AS bit) AS IsAllowed
    FROM dbo.SecurityForms form
    INNER JOIN dbo.SecurityFormOperations formOperation
        ON formOperation.FormId = form.Id
       AND formOperation.IsActive = 1
       AND formOperation.IsDeleted = 0
    INNER JOIN dbo.SecurityOperations operation
        ON operation.Id = formOperation.OperationId
       AND operation.IsDeleted = 0
       AND operation.IsActive = 1
    LEFT JOIN dbo.SecurityRoleFormOperations roleOperation ON roleOperation.FormId = form.Id
        AND roleOperation.OperationId = operation.Id
        AND roleOperation.RoleId = @RoleId
        AND roleOperation.IsDeleted = 0
    WHERE form.IsDeleted = 0
      AND form.IsActive = 1
    ORDER BY form.Name, operation.DisplayOrder, operation.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ACCESOROLGUARDAR
    @RoleId int,
    @MenusJson nvarchar(max),
    @OperationsJson nvarchar(max),
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @Menus table(MenuId int NOT NULL PRIMARY KEY, IsAllowed bit NOT NULL);
    INSERT @Menus(MenuId, IsAllowed)
    SELECT MenuId, IsAllowed
    FROM OPENJSON(@MenusJson)
    WITH(MenuId int '$.menuId', IsAllowed bit '$.isAllowed')
    WHERE MenuId IS NOT NULL;

    DECLARE @Operations table
    (
        FormId int NOT NULL,
        OperationId int NOT NULL,
        IsAllowed bit NOT NULL,
        PRIMARY KEY(FormId, OperationId)
    );
    INSERT @Operations(FormId, OperationId, IsAllowed)
    SELECT FormId, OperationId, IsAllowed
    FROM OPENJSON(@OperationsJson)
    WITH(FormId int '$.formId', OperationId int '$.operationId', IsAllowed bit '$.isAllowed')
    WHERE FormId IS NOT NULL AND OperationId IS NOT NULL;

    IF EXISTS
    (
        SELECT 1
        FROM @Operations requested
        LEFT JOIN dbo.SecurityFormOperations applicable
            ON applicable.FormId = requested.FormId
           AND applicable.OperationId = requested.OperationId
           AND applicable.IsActive = 1
           AND applicable.IsDeleted = 0
        WHERE applicable.Id IS NULL
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 51179, 'The payload contains an operation that is not applicable to its form.', 1;
    END;

    UPDATE dbo.SecurityRoleMenus
    SET IsDeleted = 1, DeletedByUserId = @UpdatedByUserId,
        DeletedByUserName = @UpdatedByUserName, DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE RoleId = @RoleId AND IsDeleted = 0;

    MERGE dbo.SecurityRoleMenus AS target
    USING @Menus AS source ON target.RoleId = @RoleId AND target.MenuId = source.MenuId
    WHEN MATCHED THEN UPDATE SET IsAllowed = source.IsAllowed, IsDeleted = 0,
        DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL,
        UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName, UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN INSERT(RoleId, MenuId, IsAllowed, CreatedByUserId, CreatedByUserName, CreatedAt)
        VALUES(@RoleId, source.MenuId, source.IsAllowed, @UpdatedByUserId, @UpdatedByUserName, SYSUTCDATETIME());

    UPDATE roleOperation
    SET IsDeleted = 1, DeletedByUserId = @UpdatedByUserId,
        DeletedByUserName = @UpdatedByUserName, DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    FROM dbo.SecurityRoleFormOperations roleOperation
    INNER JOIN dbo.SecurityFormOperations applicable
        ON applicable.FormId = roleOperation.FormId
       AND applicable.OperationId = roleOperation.OperationId
       AND applicable.IsActive = 1
       AND applicable.IsDeleted = 0
    WHERE roleOperation.RoleId = @RoleId AND roleOperation.IsDeleted = 0;

    MERGE dbo.SecurityRoleFormOperations AS target
    USING @Operations AS source
        ON target.RoleId = @RoleId AND target.FormId = source.FormId AND target.OperationId = source.OperationId
    WHEN MATCHED THEN UPDATE SET IsAllowed = source.IsAllowed, IsDeleted = 0,
        DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL,
        UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName, UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN INSERT(RoleId, FormId, OperationId, IsAllowed, CreatedByUserId, CreatedByUserName, CreatedAt)
        VALUES(@RoleId, source.FormId, source.OperationId, source.IsAllowed, @UpdatedByUserId, @UpdatedByUserName, SYSUTCDATETIME());

    COMMIT TRANSACTION;
    SELECT CAST(1 AS bit);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMACCESS_OPERACIONES
    @RoleId int,
    @FormId int,
    @OnlyActive bit = 1,
    @Search nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        form.Id AS FormId, form.Code AS FormCode, form.Name AS FormName, form.FormKey,
        operation.Id AS OperationId, operation.Code AS OperationCode,
        operation.Name AS OperationName, operation.Description AS OperationDescription,
        operation.ActionKey, operation.RibbonPageName, operation.RibbonGroupName,
        operation.IconLarge, operation.IconSmall, operation.DisplayOrder,
        CAST(CASE WHEN roleOperation.IsAllowed = 1 THEN 1 ELSE 0 END AS bit) AS IsAllowed,
        roleOperation.UpdatedByUserId, roleOperation.UpdatedByUserName, roleOperation.UpdatedAt,
        roleOperation.CreatedByUserId, roleOperation.CreatedByUserName, roleOperation.CreatedAt
    FROM dbo.SecurityForms form
    INNER JOIN dbo.SecurityFormOperations formOperation
        ON formOperation.FormId = form.Id
       AND formOperation.IsActive = 1
       AND formOperation.IsDeleted = 0
    INNER JOIN dbo.SecurityOperations operation
        ON operation.Id = formOperation.OperationId
       AND operation.IsDeleted = 0
    LEFT JOIN dbo.SecurityRoleFormOperations roleOperation ON roleOperation.FormId = form.Id
        AND roleOperation.OperationId = operation.Id
        AND roleOperation.RoleId = @RoleId
        AND roleOperation.IsDeleted = 0
    WHERE form.Id = @FormId
      AND form.IsDeleted = 0
      AND (@OnlyActive = 0 OR operation.IsActive = 1)
      AND (@Search IS NULL OR @Search = N'' OR operation.Code LIKE N'%' + @Search + N'%'
           OR operation.Name LIKE N'%' + @Search + N'%'
           OR operation.ActionKey LIKE N'%' + @Search + N'%')
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
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @Operations table(OperationId int NOT NULL PRIMARY KEY, IsAllowed bit NOT NULL);
    INSERT @Operations(OperationId, IsAllowed)
    SELECT OperationId, IsAllowed
    FROM OPENJSON(@OperationsJson)
    WITH(OperationId int '$.operationId', IsAllowed bit '$.isAllowed')
    WHERE OperationId IS NOT NULL;

    IF EXISTS
    (
        SELECT 1
        FROM @Operations requested
        LEFT JOIN dbo.SecurityFormOperations applicable
            ON applicable.FormId = @FormId
           AND applicable.OperationId = requested.OperationId
           AND applicable.IsActive = 1
           AND applicable.IsDeleted = 0
        WHERE applicable.Id IS NULL
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 51179, 'The payload contains an operation that is not applicable to the form.', 1;
    END;

    MERGE dbo.SecurityRoleFormOperations AS target
    USING @Operations AS source
        ON target.RoleId = @RoleId AND target.FormId = @FormId AND target.OperationId = source.OperationId
    WHEN MATCHED THEN UPDATE SET IsAllowed = source.IsAllowed, IsDeleted = 0,
        DeletedByUserId = NULL, DeletedByUserName = NULL, DeletedAt = NULL,
        UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName, UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN INSERT(RoleId, FormId, OperationId, IsAllowed, CreatedByUserId, CreatedByUserName, CreatedAt)
        VALUES(@RoleId, @FormId, source.OperationId, source.IsAllowed, @UpdatedByUserId, @UpdatedByUserName, SYSUTCDATETIME());

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
    SET NOCOUNT ON;
    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.SecurityForms form
        INNER JOIN dbo.SecurityFormOperations applicable
            ON applicable.FormId = form.Id AND applicable.IsActive = 1 AND applicable.IsDeleted = 0
        INNER JOIN dbo.SecurityOperations operation
            ON operation.Id = applicable.OperationId AND operation.IsDeleted = 0 AND operation.IsActive = 1
        INNER JOIN dbo.SecurityRoleFormOperations roleOperation
            ON roleOperation.FormId = form.Id AND roleOperation.OperationId = operation.Id
           AND roleOperation.RoleId = @RoleId AND roleOperation.IsDeleted = 0 AND roleOperation.IsAllowed = 1
        WHERE form.FormKey = @FormKey AND form.IsDeleted = 0 AND form.IsActive = 1
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
    SET NOCOUNT ON;
    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.UserRoles userRole
        INNER JOIN dbo.SecurityForms form
            ON form.FormKey = @FormKey AND form.IsDeleted = 0 AND form.IsActive = 1
        INNER JOIN dbo.SecurityFormOperations applicable
            ON applicable.FormId = form.Id AND applicable.IsActive = 1 AND applicable.IsDeleted = 0
        INNER JOIN dbo.SecurityOperations operation
            ON operation.Id = applicable.OperationId AND operation.IsDeleted = 0 AND operation.IsActive = 1
           AND operation.ActionKey = @ActionKey
        INNER JOIN dbo.SecurityRoleFormOperations roleOperation
            ON roleOperation.FormId = form.Id AND roleOperation.OperationId = operation.Id
           AND roleOperation.RoleId = userRole.RoleId AND roleOperation.IsDeleted = 0 AND roleOperation.IsAllowed = 1
        WHERE userRole.UserId = @UserId
    ) THEN 1 ELSE 0 END AS bit) AS IsAllowed;
END;
GO
