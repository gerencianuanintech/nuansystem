---
name: nuansystem-commercial-architecture
description: Guide NuanSystem architecture as a configurable multi-business commercial platform, not a single-giro product. Use when designing or refactoring modules, naming domains, deciding CRUD vs operational use cases, changing Clean Architecture boundaries, modularizing API endpoints, or evaluating whether behavior belongs in Domain, Application, Persistence, Configuration, or frontend.
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

- Use `$nuansystem-backend-crud` for maintenance modules.
- Use `$nuansystem-operational-usecase` for flows that affect stock, money, prices, purchases, cash, documents, or audit-sensitive state.
- Use `$nuansystem-business-capabilities` when rules may differ between supermarket, hardware store, distributor, condiments shop, services-with-inventory, or other businesses.
- Use `$nuansystem-sql-standards` for SQL scripts, stored procedures, seed data, and audit tables.
- Use `$nuansystem-winforms-devexpress` for desktop screens.

## References

- Load `references/module-boundaries.md` when deciding where a feature belongs.
