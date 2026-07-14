CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SEGURIDADNAVEGACIONUSUARIO
    @UserId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        menu.Id,
        menu.ParentId,
        menu.Code,
        menu.Name,
        menu.Description,
        CAST(menu.MenuType AS int) AS MenuType,
        COALESCE(form.FormKey, menu.FormKey) AS FormKey,
        menu.IconLarge,
        menu.IconSmall,
        menu.DisplayOrder,
        menu.IsVisible,
        menu.IsActive
    FROM dbo.SecurityMenus menu
    LEFT JOIN dbo.SecurityForms form ON form.Id = menu.FormId AND form.IsDeleted = 0
    INNER JOIN dbo.SecurityRoleMenus roleMenu ON roleMenu.MenuId = menu.Id
        AND roleMenu.IsDeleted = 0
        AND roleMenu.IsAllowed = 1
    INNER JOIN dbo.UserRoles userRole ON userRole.RoleId = roleMenu.RoleId
        AND userRole.UserId = @UserId
    WHERE menu.IsDeleted = 0
      AND menu.IsVisible = 1
      AND menu.IsActive = 1
    ORDER BY menu.ParentId, menu.DisplayOrder, menu.Name;
END;
GO
