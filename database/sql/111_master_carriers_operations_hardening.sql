/*
    Ejecutar en NuanSystem_Master despues de 108 y 109.
    Limita Transportistas a las operaciones reales del CRUD/grilla corporativa.
    Revoca solo grants automaticos de Sistema que nunca fueron modificados.
*/
DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @FormId int = (SELECT TOP (1) Id FROM dbo.SecurityForms WHERE Code = N'FORM.CATALOGS.CARRIERS' AND IsDeleted = 0);

DECLARE @AllowedActions table (ActionKey nvarchar(120) NOT NULL PRIMARY KEY);
INSERT @AllowedActions (ActionKey)
VALUES
    (N'refresh'), (N'create'), (N'update'), (N'delete'), (N'consult'), (N'copy'), (N'history'),
    (N'customize-columns'), (N'customizecolumns'),
    (N'exportexcel'), (N'exportpdf'), (N'exportjson'), (N'exportxml');

IF @AdminRoleId IS NOT NULL AND @FormId IS NOT NULL
BEGIN
    UPDATE roleOperation
    SET IsAllowed = 0,
        IsDeleted = 1,
        DeletedByUserName = N'Sistema',
        DeletedAt = SYSUTCDATETIME()
    FROM dbo.SecurityRoleFormOperations roleOperation
    INNER JOIN dbo.SecurityOperations operation ON operation.Id = roleOperation.OperationId
    WHERE roleOperation.RoleId = @AdminRoleId
      AND roleOperation.FormId = @FormId
      AND roleOperation.IsDeleted = 0
      AND roleOperation.CreatedByUserName = N'Sistema'
      AND roleOperation.UpdatedAt IS NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM @AllowedActions allowed
          WHERE allowed.ActionKey = LOWER(LTRIM(RTRIM(operation.ActionKey)))
      );

    MERGE dbo.SecurityRoleFormOperations AS target
    USING
    (
        SELECT @AdminRoleId AS RoleId, @FormId AS FormId, operation.Id AS OperationId
        FROM dbo.SecurityOperations operation
        INNER JOIN @AllowedActions allowed
            ON allowed.ActionKey = LOWER(LTRIM(RTRIM(operation.ActionKey)))
        WHERE operation.IsDeleted = 0
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
