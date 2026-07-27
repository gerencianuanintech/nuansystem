/*
    Reserva permanentemente el codigo de Warehouse despues de una eliminacion
    logica. Debe ejecutarse despues de 133 en cada tenant habilitado.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NULL
    THROW 51135, 'Warehouses is required before migration 135.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51135, 'SchemaHistory is required before migration 135.', 1;
GO

IF EXISTS
(
    SELECT Code
    FROM dbo.Warehouses
    GROUP BY Code
    HAVING COUNT_BIG(1) > 1
)
    THROW 51135, 'Warehouse codes, including tombstones, must be unique before migration 135.', 1;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Warehouses')
      AND name = N'UX_Warehouses_Code_Active'
)
    DROP INDEX UX_Warehouses_Code_Active ON dbo.Warehouses;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Warehouses')
      AND name = N'UX_Warehouses_Code'
)
    CREATE UNIQUE INDEX UX_Warehouses_Code ON dbo.Warehouses(Code);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_WAREHOUSESBUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.Warehouses
    WHERE Code = @Code
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SchemaHistory
    WHERE Version = N'20260727.135'
)
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES
    (
        N'20260727.135',
        N'Reserva permanentemente codigos tombstone de Warehouse'
    );
GO
