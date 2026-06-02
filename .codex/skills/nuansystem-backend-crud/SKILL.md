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

## Auxiliary Master Rules

- Administrable auxiliary masters must be implemented as independent backend features, not as helper arrays inside another feature.
- Each auxiliary master requires its own module/feature folder, DTOs, commands, queries, validators, repository contract, SQL Server Persistence repository, API endpoints, permission codes, and lookup query.
- Parent modules may reference auxiliary masters by stable Id/code values and consume lookup endpoints, but must not contain CRUD handlers for those auxiliary masters unless they are the owning module.
- Use a shared descriptor approach only to remove repetitive CRUD boilerplate; the descriptor must still preserve independent routes, permissions, forms, tables, and stored procedures per auxiliary master.
- Do not use hard-coded lookup values in backend handlers for catalogs that users can maintain, vary by company, or need permissions.
- When a consuming form has a `+` selector action, the backend must expose create permission and create endpoint for the owning auxiliary master.

## API Error Rules

- Do not return raw exceptions from endpoints.
- Handlers must return `Result<T>` with business errors.
- Unexpected exceptions must be handled by the global exception middleware.
- Validation failures must return a consistent validation response.
- Use stable error codes for frontend handling.
- Controllers or endpoints must not contain business logic.
- Controllers or endpoints must only receive request, call MediatR/use case, and return response.
- Do not expose SQL errors directly to the frontend.
- Do not expose stack traces in production.

## References

- Load `references/module-checklist.md` before implementing a new maintenance module.
- Use `$nuansystem-commercial-architecture` when deciding module boundaries or names.
- Use `$nuansystem-sql-standards` for database scripts or stored procedures.
- Use `$nuansystem-winforms-devexpress` when the task also touches the desktop UI.
