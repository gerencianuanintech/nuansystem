CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERACIONSEGURIDADLISTAR
AS
BEGIN
    SELECT
        Id,
        Code,
        Name,
        Description,
        RibbonPageName,
        RibbonGroupName,
        ActionKey,
        IconLarge,
        IconSmall,
        DisplayOrder,
        IsActive
    FROM dbo.SecurityOperations
    ORDER BY RibbonPageName, RibbonGroupName, DisplayOrder, Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERACIONSEGURIDADBUSCARPORID
    @Id int
AS
BEGIN
    SELECT
        Id,
        Code,
        Name,
        Description,
        RibbonPageName,
        RibbonGroupName,
        ActionKey,
        IconLarge,
        IconSmall,
        DisplayOrder,
        IsActive
    FROM dbo.SecurityOperations
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_OPERACIONSEGURIDADCREAR
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @RibbonPageName nvarchar(80) = NULL,
    @RibbonGroupName nvarchar(80) = NULL,
    @ActionKey nvarchar(120) = NULL,
    @IconLarge nvarchar(200) = NULL,
    @IconSmall nvarchar(200) = NULL,
    @DisplayOrder int = 0,
    @IsActive bit = 1
AS
BEGIN
    INSERT INTO dbo.SecurityOperations
    (
        Code,
        Name,
        Description,
        RibbonPageName,
        RibbonGroupName,
        ActionKey,
        IconLarge,
        IconSmall,
        DisplayOrder,
        IsActive
    )
    VALUES
    (
        @Code,
        @Name,
        @Description,
        @RibbonPageName,
        @RibbonGroupName,
        @ActionKey,
        @IconLarge,
        @IconSmall,
        @DisplayOrder,
        @IsActive
    );

    SELECT CAST(SCOPE_IDENTITY() AS int);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERACIONSEGURIDADBUSCARPORCODIGO
    @Code nvarchar(80),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.SecurityOperations
    WHERE Code = @Code
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERACIONSEGURIDADBUSCARPORNOMBRE
    @Name nvarchar(120),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.SecurityOperations
    WHERE Name = @Name
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_OPERACIONSEGURIDADACTUALIZAR
    @Id int,
    @Code nvarchar(80),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @RibbonPageName nvarchar(80) = NULL,
    @RibbonGroupName nvarchar(80) = NULL,
    @ActionKey nvarchar(120) = NULL,
    @IconLarge nvarchar(200) = NULL,
    @IconSmall nvarchar(200) = NULL,
    @DisplayOrder int = 0,
    @IsActive bit = 1
AS
BEGIN
    UPDATE dbo.SecurityOperations
    SET
        Code = @Code,
        Name = @Name,
        Description = @Description,
        RibbonPageName = @RibbonPageName,
        RibbonGroupName = @RibbonGroupName,
        ActionKey = @ActionKey,
        IconLarge = @IconLarge,
        IconSmall = @IconSmall,
        DisplayOrder = @DisplayOrder,
        IsActive = @IsActive
    WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_OPERACIONSEGURIDADELIMINAR
    @Id int
AS
BEGIN
    DELETE FROM dbo.SecurityOperations
    WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END;
GO
