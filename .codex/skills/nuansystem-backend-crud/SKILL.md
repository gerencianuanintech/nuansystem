---
name: nuansystem-backend-crud
description: Build or modify NuanSystem backend CRUD features for administrative maintenance modules, API endpoints, Application commands/queries/handlers/validators, repository contracts, Persistence repositories, permission codes, multi-company behavior, audit user fields, and Dapper stored-procedure integration. Use for administrative CRUD tasks touching src/Backend, minimal APIs, MediatR-style CQRS, tenant-aware data access, or backend module normalization. For processes affecting stock, money, prices, purchases, cash, or documents, use nuansystem-operational-usecase instead.
---

# NuanSystem Backend CRUD

## Workflow

1. Read `README.md`, `docs/ARCHITECTURE.md`, `docs/ARQUITECTURA-COMERCIAL.md`, and the closest existing feature before editing.
2. Confirm the request is administrative CRUD. If it affects stock, money, prices, purchases, cash, or document state, switch to `$nuansystem-operational-usecase`.
3. Keep dependency direction: `Api -> Application -> Domain`, `Application` defines contracts, `Persistence` implements repositories, `Infrastructure` implements technical services.
4. Model the feature under `src/Backend/NuanSystem.Application/Features/{Module}` with `Commands`, `Queries`, `Dtos`, and validators.
5. Add repository contracts to `Application/Abstractions` or the feature folder following nearby modules; implement them in `Persistence/Repositories`.
6. Use Dapper only through stored procedures with `CommandType.StoredProcedure`; do not add inline CRUD SQL in repositories.
7. Register new services in the existing dependency injection registration file for the owning project.
8. Add or update minimal API endpoints in `src/Backend/NuanSystem.Api/Endpoints/{Module}Endpoints.cs`; keep `Program.cs` as composition.
9. Protect endpoints with `RequirePermission(PermissionCodes.X)` or `RequireFormOperation(formKey, action)` as appropriate.
10. Preserve multi-company flow: tenant data must resolve through `ITenantConnectionFactory` and the active company context. Do not connect WinForms or API endpoints directly to tenant databases.
11. Pass audit user data from JWT/API context into commands and repository data objects for create, update, and delete operations.

## Backend Shape

- Commands and queries implement `ICommand<TResponse>` or `IQuery<TResponse>` from `Application/Abstractions/Messaging`.
- Handlers return `Result<T>` and keep business decisions in Application, not in Api.
- Validators use FluentValidation and should mirror field limits used by SQL scripts and DTOs.
- Repositories return DTOs or simple data objects already established by the feature. Avoid leaking persistence-only types upward.
- Delete operations are logical deletes when the table has audit columns.
- Use `CancellationToken` on async paths.

## References

- Load `references/module-checklist.md` before implementing a new maintenance module.
- Use `$nuansystem-commercial-architecture` when deciding module boundaries or names.
- Use `$nuansystem-sql-standards` for database scripts or stored procedures.
- Use `$nuansystem-winforms-devexpress` when the task also touches the desktop UI.
