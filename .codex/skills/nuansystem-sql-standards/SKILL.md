---
name: nuansystem-sql-standards
description: Create, review, or normalize NuanSystem SQL Server scripts, tenant/master database objects, stored procedures, Dapper procedure contracts, audit columns, logical deletes, indexes, CRUD and operational stored procedures, commercial seed data, capabilities, and SP_NA naming. Use for tasks touching database/sql, SQL migrations/scripts, persistence procedure names, CRUD stored procedures, operational transactions, configuration/capability seed data, or database audit behavior.
---

# NuanSystem SQL Standards

## Workflow

1. Read `docs/estandar-procedimientos-almacenados.md`, `docs/estandar-auditoria-base-datos.md`, and `docs/ARQUITECTURA-COMERCIAL.md` before editing SQL.
2. Place scripts under `database/sql` using the existing numeric prefix pattern.
3. Use idempotent scripts: `IF OBJECT_ID`, `IF COL_LENGTH`, `IF NOT EXISTS`, and `CREATE OR ALTER PROCEDURE`.
4. Route all CRUD persistence through stored procedures. Repositories should not contain inline CRUD SQL.
5. Name procedures with `SP_NA_{VERBO_HTTP}_{ENTIDAD}{ACCION}`.
6. Use logical delete for maintenance tables when audit columns exist.
7. Keep SQL parameter names aligned with C# DTO/data object property names where possible.
8. Update repository procedure constants when procedure names change.
9. For operational flows, design procedure boundaries around the Application transaction/use case, not around generic CRUD.

## Required CRUD Procedures

- `SP_NA_GET_{ENTITY}_LISTAR`
- `SP_NA_GET_{ENTITY}_BUSCARPORID`
- `SP_NA_GET_{ENTITY}BUSCARPORCODIGO` or the equivalent unique-field lookup
- `SP_NA_POST_{ENTITY}_CREAR`
- `SP_NA_PUT_{ENTITY}_ACTUALIZAR`
- `SP_NA_DELETE_{ENTITY}_ELIMINAR`

Follow nearby modules if an entity already uses plural names or a legacy naming variant.

## Audit Rules

- Add standard audit columns to maintenance tables.
- Fill create audit fields in `POST`.
- Fill update audit fields and `UpdatedAt = SYSUTCDATETIME()` in `PUT` and `PATCH`.
- Fill delete audit fields and `DeletedAt = SYSUTCDATETIME()` in `DELETE`.
- Filter `IsDeleted = 0` in list/detail/existence procedures.
- Return `CONVERT(int, SCOPE_IDENTITY())` after insert and `@@ROWCOUNT` after update/delete.

## References

- Load `references/sql-checklist.md` when creating or reviewing a script.
- Use `$nuansystem-operational-usecase` when SQL supports stock, money, pricing, purchases, cash, or document state changes.
- Use `$nuansystem-business-capabilities` when seeding or reading configurable behavior by company.
- Use `$nuansystem-backend-crud` when C# repository/API changes are also needed.
