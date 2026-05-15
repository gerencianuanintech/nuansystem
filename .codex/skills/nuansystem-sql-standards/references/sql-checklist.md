# SQL Script Checklist

## Table

- Define `Id int IDENTITY(1,1)` unless a nearby module establishes another key.
- Define business unique indexes with `WHERE IsDeleted = 0`.
- Add default constraints for `CreatedAt` and `IsDeleted`.
- Use `nvarchar` lengths consistent with validators and DTOs.
- Add nullable SAP columns only when the module needs SAP mapping.

## Procedures

- Start each procedure with `SET NOCOUNT ON;`.
- Use `CREATE OR ALTER PROCEDURE`.
- Do not return deleted rows from list/detail procedures.
- Use `@ExcluirId int = NULL` for uniqueness checks used by create and update validators.
- Return scalar values expected by repositories: created `Id`, count, or affected row count.

## C# Alignment

- Repository constants must match SQL procedure names exactly.
- Dapper parameter object property names must match SQL parameters.
- Validators must match SQL required fields and max lengths.
