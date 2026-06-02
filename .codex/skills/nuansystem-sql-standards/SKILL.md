---
name: nuansystem-sql-standards
description: Create, review, or normalize NuanSystem SQL Server scripts, tenant/master database objects, stored procedures, Dapper procedure contracts, audit columns, logical deletes, indexes, CRUD and operational stored procedures, commercial seed data, capabilities, and SP_NA naming. Use for tasks touching database/sqlserver, legacy database/sql scripts, SQL migrations/scripts, persistence procedure names, CRUD stored procedures, operational transactions, configuration/capability seed data, or database audit behavior.
---

# NuanSystem SQL Standards

## Workflow

1. Read `docs/estandar-procedimientos-almacenados.md`, `docs/estandar-auditoria-base-datos.md`, and `docs/ARQUITECTURA-COMERCIAL.md` before editing SQL.
2. Place SQL Server scripts under `database/sqlserver` using the existing numeric prefix pattern. If the repository still has legacy scripts under `database/sql`, keep legacy placement consistent until the project migrates that folder to `database/sqlserver`.
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

## Auxiliary Master Database Rules

- Administrable auxiliary masters must be independent database objects, not only free-text columns or hard-coded seed values in a parent table.
- Each auxiliary master must have its own table, primary key, unique `Code` constraint/index, `Name`, `Description` when useful, `IsActive`, `IsDeleted`, audit columns, and tenant/master placement according to ownership.
- Each auxiliary master must expose its own stored procedures for list, detail, lookup, create, update, and logical delete. A parent master can call lookup procedures or join by Id/code, but must not replace the auxiliary master's CRUD procedures.
- Use foreign keys from parent tables to auxiliary master tables when the relationship is stable and the catalog belongs to the same database scope.
- If an auxiliary master varies by company, create it in the tenant database and access it through `CompanyContext`. If it is global, document why it belongs to master.
- Seed data may provide defaults, but users must be able to maintain administrable auxiliary masters through the owning module.

## Audit Rules

- Add standard audit columns to maintenance tables.
- Fill create audit fields in `POST`.
- Fill update audit fields and `UpdatedAt = SYSUTCDATETIME()` in `PUT` and `PATCH`.
- Fill delete audit fields and `DeletedAt = SYSUTCDATETIME()` in `DELETE`.
- Filter `IsDeleted = 0` in list/detail/existence procedures.
- Return `CONVERT(int, SCOPE_IDENTITY())` after insert and `@@ROWCOUNT` after update/delete.

## Provider Isolation

- This skill defines SQL Server standards.
- SQL Server remains the production standard and must not be downgraded to a lowest-common-denominator SQL style for hypothetical provider portability.
- SQL Server-specific syntax must remain inside `database/sqlserver` scripts and Persistence implementations.
- Application and Domain must not depend on SQL Server-specific concepts.
- If MySQL support is added, create equivalent scripts under `database/mysql`.
- Keep repository contracts stable regardless of provider.
- Do not expose SQL Server stored procedure names directly in Application handlers.
- SQL Server scripts may use `CREATE OR ALTER`, `DATETIME2`, `SYSUTCDATETIME`, `SCOPE_IDENTITY` and `@@ROWCOUNT`.
- Those SQL Server-specific features must not leak outside SQL Server persistence code.

## References

- Load `references/sql-checklist.md` when creating or reviewing a script.
- Use `$nuansystem-operational-usecase` when SQL supports stock, money, pricing, purchases, cash, or document state changes.
- Use `$nuansystem-business-capabilities` when seeding or reading configurable behavior by company.
- Use `$nuansystem-backend-crud` when C# repository/API changes are also needed.
