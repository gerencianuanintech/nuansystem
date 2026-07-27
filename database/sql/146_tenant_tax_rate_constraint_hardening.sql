/*
    Iteracion 8.7 - Endurecimiento del contrato decimal de Tax.
    Corrige hacia adelante la restriccion historica que solo exigia Rate >= 0.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Taxes', N'U') IS NULL
    THROW 51146, 'Taxes is required before migration 146.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51146, 'SchemaHistory is required before migration 146.', 1;
IF COL_LENGTH(N'dbo.Taxes', N'Rate') IS NULL
    THROW 51146, 'Taxes.Rate is required before migration 146.', 1;
IF EXISTS (SELECT 1 FROM dbo.Taxes WHERE Rate < 0 OR Rate > 1)
    THROW 51146, 'Tax Rate must use the decimal contract 0..1 before migration 146.', 1;
IF EXISTS
(
    SELECT 1
    FROM sys.check_constraints AS constraint_metadata
    WHERE constraint_metadata.parent_object_id = OBJECT_ID(N'dbo.Taxes')
      AND constraint_metadata.name <> N'CK_Taxes_Rate'
      AND LOWER(constraint_metadata.definition) LIKE N'%rate%'
)
    THROW 51146, 'An unexpected Tax Rate check constraint exists before migration 146.', 1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Taxes')
          AND name = N'CK_Taxes_Rate'
          AND
          (
              is_disabled = 1
              OR is_not_trusted = 1
              OR LOWER(
                    REPLACE(REPLACE(REPLACE(REPLACE(definition, N' ', N''), N'[', N''), N']', N''), N'(', N'')
                 ) NOT LIKE N'%rate>=0%rate<=1%'
          )
    )
        ALTER TABLE dbo.Taxes DROP CONSTRAINT CK_Taxes_Rate;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Taxes')
          AND name = N'CK_Taxes_Rate'
    )
        ALTER TABLE dbo.Taxes WITH CHECK
            ADD CONSTRAINT CK_Taxes_Rate CHECK (Rate >= 0 AND Rate <= 1);

    ALTER TABLE dbo.Taxes WITH CHECK CHECK CONSTRAINT CK_Taxes_Rate;

    IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260727.146')
        INSERT dbo.SchemaHistory(Version, Description)
        VALUES(N'20260727.146', N'Endurecer Tax Rate al contrato decimal cerrado 0..1');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
