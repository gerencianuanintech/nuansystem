---
name: nuansystem-commercial-architecture
description: Guide NuanSystem as a configurable multi-business commercial platform. Use for module/domain design, CRUD-versus-operational classification, Clean Architecture boundaries, API modularization, or deciding whether behavior belongs in Domain, Application, Persistence, Configuration, or frontend.
---

# NuanSystem Commercial Architecture

## Workflow

1. Read `docs/ARQUITECTURA-COMERCIAL.md` and `docs/ARCHITECTURE.md`.
2. Classify the request as administrative CRUD or operational process.
3. Use neutral commercial module names: `Catalogs`, `Inventory`, `Pricing`, `Sales`, `Purchasing`, `Cash`, `Documents`, `Security`, `Configuration`, `Integrations`.
4. Avoid giro-specific names in code unless the concept truly exists only for that integration or preset.
5. If behavior can vary by business type, model it as a company capability/configuration instead of hard-coding it.
6. Keep dependency direction: Api -> Application -> Domain; Application defines contracts; Persistence and integrations implement them.
7. Keep `Program.cs` as composition and map endpoints through `Endpoints/Map...Endpoints()` modules.

## Decisions

- Use `$nuansystem-backend-architecture` as the backend orchestrator.
- Through that orchestrator, use `$nuansystem-backend-crud` for maintenance modules.
- Through that orchestrator, use `$nuansystem-operational-usecase` for flows that affect stock, money, prices, purchases, cash, documents, workflow, synchronization, or external state.
- Use `$nuansystem-business-capabilities` when rules may differ between supermarket, hardware store, distributor, condiments shop, services-with-inventory, or other businesses.
- Use `$nuansystem-sql-standards` for SQL scripts, stored procedures, seed data, and audit tables.
- Use `$nuansystem-winforms-devexpress` for desktop screens.

## Auxiliary Master Boundaries

- Auxiliary masters are first-class maintenance modules when users can administer them or when more than one feature consumes them.
- Each auxiliary master must be independent in backend, frontend, and database:
  - Backend: own Application feature, DTOs, commands/queries, validators, repository contract, Persistence repository, API endpoints, permissions, and lookup endpoint.
  - Frontend: own service client/models, ViewModel when the local pattern uses one, list/edit forms, `FormKey`, menu/security registration when applicable, and CRUD permissions.
  - Database: own table, unique code/indexes, audit fields, soft delete, stored procedures, and seed/security script when needed.
- Consuming modules such as `BusinessPartners` or `InventoryItems` must consume auxiliary masters through lookup contracts and stable Id/code values. They must not implement the auxiliary master's CRUD inside the parent form or parent endpoint.
- Related-record creation from a selector must open the owning maintenance module and refresh the lookup after save. The `+` action must depend on create permission for that auxiliary master.
- Hard-coded combo values are only acceptable for fixed enumerations that are not administrable and are not expected to vary by company.

## References

- Load `references/module-boundaries.md` when deciding where a feature belongs.
