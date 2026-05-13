CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SEGURIDADOPERACIONESUSUARIO
    @UserId int,
    @FormKey nvarchar(120)
AS
BEGIN
    DECLARE @FormId int = (
        SELECT TOP (1) Id
        FROM dbo.SecurityForms
        WHERE FormKey = @FormKey
          AND IsDeleted = 0
    );

    IF @FormId IS NULL
    BEGIN
        SELECT
            CAST(0 AS int) AS OperationId,
            CAST(NULL AS nvarchar(80)) AS Code,
            CAST(NULL AS nvarchar(120)) AS Name,
            CAST(NULL AS nvarchar(300)) AS Description,
            CAST(NULL AS nvarchar(120)) AS ActionKey,
            CAST(NULL AS nvarchar(80)) AS RibbonPageName,
            CAST(NULL AS nvarchar(80)) AS RibbonGroupName,
            CAST(NULL AS nvarchar(200)) AS IconLarge,
            CAST(NULL AS nvarchar(200)) AS IconSmall,
            CAST(0 AS int) AS DisplayOrder,
            CAST(0 AS bit) AS IsAllowed
        WHERE 1 = 0;
        RETURN;
    END;

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
            INNER JOIN dbo.SecurityRoleFormOperations roleOperation ON roleOperation.RoleId = userRole.RoleId
            WHERE userRole.UserId = @UserId
              AND roleOperation.FormId = @FormId
              AND roleOperation.OperationId = operation.Id
              AND roleOperation.IsDeleted = 0
              AND roleOperation.IsAllowed = 1
        ) THEN 1 ELSE 0 END AS bit) AS IsAllowed
    FROM dbo.SecurityOperations operation
    WHERE operation.IsDeleted = 0
      AND operation.IsActive = 1
    ORDER BY operation.DisplayOrder, operation.Name;
END;
GO
