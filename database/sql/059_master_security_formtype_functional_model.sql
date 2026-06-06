/*
    Ejecutar este script en NuanSystem_Master.
    Normaliza SecurityForms.FormType como naturaleza funcional del formulario:
    1 = Mantenimiento, 2 = Transaccional, 3 = Reporte, 4 = Dialogo, 5 = Proceso.
*/

UPDATE dbo.SecurityForms
SET FormType = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE FormKey IN
(
    N'security-document-series',
    N'operational-catalogs'
)
  AND FormType <> 1
  AND IsDeleted = 0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_FORMULARIOSEGURIDADLISTAR
AS
BEGIN
    SELECT
        Id, Code, Name, Description, FormKey, CAST(FormType AS int) AS FormType,
        CASE FormType
            WHEN 1 THEN N'Mantenimiento'
            WHEN 2 THEN N'Transaccional'
            WHEN 3 THEN N'Reporte'
            WHEN 4 THEN N'Dialogo'
            WHEN 5 THEN N'Proceso'
            ELSE N'Desconocido'
        END AS FormTypeName,
        IsVisible, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt,
        DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.SecurityForms
    WHERE IsDeleted = 0
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_FORMULARIOSEGURIDADBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        Id, Code, Name, Description, FormKey, CAST(FormType AS int) AS FormType,
        CASE FormType
            WHEN 1 THEN N'Mantenimiento'
            WHEN 2 THEN N'Transaccional'
            WHEN 3 THEN N'Reporte'
            WHEN 4 THEN N'Dialogo'
            WHEN 5 THEN N'Proceso'
            ELSE N'Desconocido'
        END AS FormTypeName,
        IsVisible, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt,
        DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.SecurityForms
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMACCESS_FORMULARIOS
    @FormType int = NULL,
    @OnlyActive bit = 1,
    @Search nvarchar(120) = NULL
AS
BEGIN
    SELECT
        form.Id,
        form.Code,
        form.Name,
        form.Description,
        form.FormKey,
        CAST(form.FormType AS int) AS FormType,
        CASE form.FormType
            WHEN 1 THEN N'Mantenimiento'
            WHEN 2 THEN N'Transaccional'
            WHEN 3 THEN N'Reporte'
            WHEN 4 THEN N'Dialogo'
            WHEN 5 THEN N'Proceso'
            ELSE N'Desconocido'
        END AS FormTypeName,
        form.IsVisible,
        form.IsActive
    FROM dbo.SecurityForms form
    WHERE form.IsDeleted = 0
      AND (@FormType IS NULL OR form.FormType = @FormType)
      AND (@OnlyActive = 0 OR form.IsActive = 1)
      AND
      (
          @Search IS NULL
          OR @Search = N''
          OR form.Code LIKE N'%' + @Search + N'%'
          OR form.Name LIKE N'%' + @Search + N'%'
          OR form.FormKey LIKE N'%' + @Search + N'%'
      )
    ORDER BY form.Name;
END;
GO
