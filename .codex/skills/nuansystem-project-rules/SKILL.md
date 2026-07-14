---
name: nuansystem-project-rules
description: Orchestrate all NuanSystem project skills and enforce global architectural rules for C#/.NET backend, SQL Server-first persistence, future MySQL compatibility, WinForms DevExpress frontend, REST APIs, MediatR/CQRS, multi-company security, logging, SAP Business One, deployment, and production readiness. Use as the main project rule when a task touches NuanSystem architecture, spans multiple layers, or requires deciding which specialized skills to combine.
---

# NuanSystem Project Rules

## Purpose

This skill is the main NuanSystem project rule. It does not replace specialized skills; it tells Codex when to apply each one and how to combine them.

When a task touches several areas, apply all relevant skills instead of forcing the work through a single skill.

Examples:

- Backend CRUD: apply `$nuansystem-backend-crud`, `$nuansystem-mediatr-cqrs`, `$nuansystem-security-auth`, `$nuansystem-api-error-logging`, and `$nuansystem-sql-standards`.
- WinForms screen: apply `$nuansystem-winforms-devexpress`, `$nuansystem-frontend-api-client`, and `$nuansystem-security-auth`. Load `nuansystem-winforms-devexpress/references/enterprise-typography.md` only when typography details are needed.

## Global Non-Negotiable Rules

- No business logic in WinForms forms.
- No direct SQL Server access from WinForms.
- No direct MySQL access from WinForms.
- No direct SAP Business One access from WinForms.
- Every operation must pass through the REST API.
- No business logic in controllers or minimal API endpoints.
- Controllers/endpoints only receive requests, call MediatR/use cases, and return responses.
- Application orchestrates use cases.
- Domain contains business rules.
- Persistence contains database access.
- Infrastructure contains technical services.
- SAP integration must be optional and isolated.
- Security must always be validated in backend.
- Company access must always be validated in backend.
- Logs must not contain secrets, passwords, JWT tokens, SAP credentials, or connection strings.
- API errors must be standardized.
- Production code must include `CancellationToken` where applicable.
- Do not generate monolithic classes, giant services, or giant forms.
- Auxiliary masters must be independent in backend, frontend, and database. Do not hide an administrable catalog only inside the consuming form or inside a parent master.
- Each auxiliary master must have its own explicit WinForms list/edit form classes and its own menu/form key when it is administrable. Shared base classes, shared services, descriptors, or helper components are allowed only to remove boilerplate; they must not replace the concrete form per auxiliary master.

## Skill Selection Matrix

| Task type | Required skills |
|---|---|
| Backend CRUD | `$nuansystem-backend-crud`, `$nuansystem-mediatr-cqrs`, `$nuansystem-security-auth`, `$nuansystem-api-error-logging`, `$nuansystem-sql-standards` |
| Auxiliary master/catalog | `$nuansystem-commercial-architecture`, `$nuansystem-backend-crud`, `$nuansystem-sql-standards`, `$nuansystem-winforms-devexpress`, `$nuansystem-frontend-api-client`, `$nuansystem-security-auth` |
| Operational process | `$nuansystem-operational-usecase`, `$nuansystem-mediatr-cqrs`, `$nuansystem-sql-standards`, `$nuansystem-api-error-logging`, `$nuansystem-security-auth` |
| WinForms screen | `$nuansystem-winforms-devexpress`, `$nuansystem-frontend-api-client` |
| Login or authentication | `$nuansystem-security-auth`, `$nuansystem-frontend-api-client`, `$nuansystem-api-error-logging` |
| SQL Server scripts | `$nuansystem-sql-standards`, `$nuansystem-database-provider-compatibility` |
| MySQL future compatibility | `$nuansystem-database-provider-compatibility` |
| SAP integration | `$nuansystem-sap-business-one`, `$nuansystem-api-error-logging`, `$nuansystem-security-auth` |
| Logging/error handling | `$nuansystem-api-error-logging` |
| Production deployment | `$nuansystem-deployment-production` |
| Business capability configuration | `$nuansystem-business-capabilities`, `$nuansystem-commercial-architecture` |
| Architecture decision | `$nuansystem-commercial-architecture`, `$nuansystem-business-capabilities` |
| API client frontend | `$nuansystem-frontend-api-client`, `$nuansystem-winforms-devexpress` |
| DevExpress typography | `$nuansystem-winforms-devexpress` with `references/enterprise-typography.md` |

## Backend Rules

- Backend must use modular/clean architecture.
- Endpoints must be thin.
- Application logic must live in handlers or focused Application services.
- Use MediatR for use cases.
- Use FluentValidation for input validation.
- Use `Result<T>` or the standard API error response for business errors.
- Use `CancellationToken` where applicable.
- Do not access `HttpContext` from Domain.
- Do not couple Domain to Persistence.
- Do not couple Application directly to SQL Server, MySQL, Dapper, EF Core, or concrete connection classes.
- Do not return internal entities directly when DTOs are the correct contract.
- Keep authorization and active-company resolution in backend-controlled services, filters, middleware, or endpoint policies.
- Administrable auxiliary masters must have their own Application feature, DTOs, commands/queries, validators, repository contract, Persistence implementation, API endpoints, permissions, and lookup endpoint. A parent feature may consume them through lookups, but must not own their CRUD if the catalog is reusable or independently maintained.

## Frontend Rules

- Frontend uses WinForms with DevExpress.
- Forms must remain compatible with the Visual Studio Designer.
- Forms consume services; they do not call API mechanics directly.
- No form creates `HttpClient` directly.
- Use `NuanApiClient` or the approved centralized HTTP client.
- Use `ApiSession` for token, active company, and session state.
- Send JWT and `X-Company-Code` from the centralized client.
- Use `XtraMessageBox` or approved shared UI helpers for user-facing errors.
- Use `GridControl`/`GridView` for lists.
- For operational document edit screens, avoid `LayoutControl` and avoid `GroupControl`/group panels unless explicitly requested; prefer manually positioned `PanelControl` sections with explicit `LabelControl` titles. Maintenance screens may use their established local pattern.
- Apply official typography through `$nuansystem-winforms-devexpress` and its internal `references/enterprise-typography.md`.
- Keep main action buttons in the standard form area according to project design.
- Do not place business rules in button click events; events coordinate UI, call services, and render responses.
- Administrable auxiliary masters must have their own frontend module: service client, models, ViewModel when used by the local pattern, concrete list form, concrete edit form, `FormKey`, menu/security entry when applicable, and CRUD permissions. Consuming forms use `SearchLookUpEdit`/`LookUpEdit` and may expose a permission-controlled `+` button that opens the owner maintenance.
- Do not expose a single generic auxiliary-master form as the final user-facing maintenance for multiple masters. Navigation, permissions, form keys, and code references must point to concrete forms such as `SupplierGroupsForm` and `SupplierGroupEditForm`.
- Supplier auxiliary edit/list forms must inherit from the official common bases under `Forms/Common`, such as `BaseGridCrudListForm` and `BaseEditForm`. Do not create module-local form base classes under `Forms/GeneralSupplier/Catalogs`.
- Supplier auxiliary masters must live under `Forms/GeneralSupplier/{AuxiliaryMaster}`. Each auxiliary master folder must contain its own list and edit form classes. Example:

```text
Forms/GeneralSupplier
├── SupplierGroups
│   ├── SupplierGroupsForm.cs
│   └── SupplierGroupEditForm.cs
├── SupplierClasses
│   ├── SupplierClassesForm.cs
│   └── SupplierClassEditForm.cs
├── EconomicActivities
│   ├── EconomicActivitiesForm.cs
│   └── EconomicActivityEditForm.cs
├── Zones
│   ├── ZonesForm.cs
│   └── ZoneEditForm.cs
├── SupplyMethods
│   ├── SupplyMethodsForm.cs
│   └── SupplyMethodEditForm.cs
├── ContactTypes
│   ├── ContactTypesForm.cs
│   └── ContactTypeEditForm.cs
└── ContactChannels
    ├── ContactChannelsForm.cs
    └── ContactChannelEditForm.cs
```

## Database Rules

- SQL Server is the primary database engine and production standard.
- SQL Server scripts must follow `$nuansystem-sql-standards`.
- Future MySQL support must be isolated through `$nuansystem-database-provider-compatibility`.
- Do not expose SQL Server syntax in Application or Domain.
- Use repository contracts or persistence services.
- Use parameters always.
- Avoid SQL injection.
- Use versioned scripts.
- Maintain `SchemaVersions` or the repository's approved schema version mechanism when applicable.
- Use standard audit fields.
- Use soft delete where applicable.
- Keep SQL Server-specific features inside `database/sqlserver` scripts or SQL Server Persistence implementations.
- Administrable auxiliary masters must have independent database objects: their own table, unique code/indexes, audit fields, soft delete, lookup/list/detail/create/update/delete stored procedures, and seed/security scripts when needed. Do not store auxiliary master values only as free text in the parent table when the value is a maintained catalog.

## Multi-Company Rules

- Every company-data operation requires an active company.
- Frontend must send `X-Company-Code` through `NuanApiClient`.
- Backend must validate `X-Company-Code` against the master database.
- Backend must validate that the authenticated user has access to the company.
- Never trust `CompanyCode` without backend validation.
- Resolve `CompanyContext` per request.
- Resolve tenant database connections from Persistence/Infrastructure using trusted company configuration.
- Do not allow cross-company access.
- Company capabilities must condition functional behavior when rules vary by business type.

## Security Rules

- Security is governed by `$nuansystem-security-auth`.
- JWT is required for protected endpoints.
- Passwords must use secure hashing.
- Secrets must be encrypted, protected, or loaded from environment variables/secret providers.
- Do not log secrets.
- Do not log tokens.
- Validate permissions in backend.
- Roles and permissions must not depend on frontend trust.
- Protect or disable Swagger in production.
- HTTPS is mandatory in production.

## SAP Business One Rules

- SAP Business One is governed by `$nuansystem-sap-business-one`.
- SAP integration is optional per company.
- NuanSystem must work without SAP.
- Prefer Service Layer when viable.
- Use DI API only when the scenario requires it.
- WinForms never accesses SAP.
- SAP must not contaminate Domain.
- SAP mappings must be isolated.
- SAP errors must be registered.
- SAP sends must use `SapSyncLog`.
- Retries must be controlled.
- Do not resend successfully synced documents without validation and audit.

## Logging and Error Handling Rules

- Error handling is governed by `$nuansystem-api-error-logging`.
- Use Serilog.
- Use standard API errors.
- Include `TraceId`.
- Do not expose stack traces in production.
- Do not expose SQL errors directly to users.
- Do not expose raw SAP errors directly to end users without cleanup.
- Frontend must show clear user messages.
- Technical errors must be logged.
- Business errors must be understandable to the user.

## Production Rules

- Production readiness is governed by `$nuansystem-deployment-production`.
- Use environment-specific `appsettings`.
- Do not commit secrets.
- Use HTTPS.
- Configure logs and retention.
- Configure backups for master and tenant databases.
- Configure health checks.
- Protect Swagger.
- Validate SQL Server connectivity.
- Validate SAP connectivity when SAP is enabled.
- Compile in Release.
- Maintain a deployment checklist.

## When Multiple Skills Apply

Example 1: If the task is "crear CRUD de clientes completo", apply:

- `$nuansystem-backend-crud`
- `$nuansystem-mediatr-cqrs`
- `$nuansystem-sql-standards`
- `$nuansystem-security-auth`
- `$nuansystem-api-error-logging`
- `$nuansystem-winforms-devexpress`
- `$nuansystem-frontend-api-client`

Example 2: If the task is "crear proceso de venta", apply:

- `$nuansystem-operational-usecase`
- `$nuansystem-mediatr-cqrs`
- `$nuansystem-sql-standards`
- `$nuansystem-security-auth`
- `$nuansystem-api-error-logging`
- `$nuansystem-business-capabilities`

Example 3: If the task is "enviar factura a SAP", apply:

- `$nuansystem-sap-business-one`
- `$nuansystem-operational-usecase`
- `$nuansystem-security-auth`
- `$nuansystem-api-error-logging`
- `$nuansystem-sql-standards`

Example 4: If the task is "diseñar pantalla de artículos", apply:

- `$nuansystem-winforms-devexpress`
- `$nuansystem-frontend-api-client`
- `$nuansystem-business-capabilities`

## Anti-Patterns

- SQL queries inside WinForms.
- SAP DI API references inside WinForms.
- Business rules in button click events.
- Controllers with business logic.
- Handlers with massive unrelated logic.
- Static global `CompanyCode`.
- Trusting frontend permissions.
- Returning raw exceptions.
- Logging passwords.
- Logging JWT tokens.
- Hard-coding company-specific logic.
- Hard-coding SAP mode globally.
- Mixing SQL Server-specific code in Application.
- Creating one giant service for all modules.
- Creating one giant form for all operations.
- Using one user-facing generic maintenance form as the only form for several administrable auxiliary masters.
- Implementing auxiliary masters only as hard-coded combo items inside a form.
- Placing auxiliary master CRUD inside the consuming parent master instead of its owning module.
- Duplicating HTTP logic in every form.
- Ignoring cancellation tokens in backend operations.
- Ignoring transaction boundaries for stock or money operations.

## Required Output Behavior

When Codex generates or modifies code for NuanSystem:

- State which skills were applied.
- Keep modular structure.
- Create files in the correct folders.
- Do not mix layers.
- Use names consistent with NuanSystem.
- Include compilable examples when possible.
- Preserve Visual Studio Designer compatibility for WinForms forms.
- Do not introduce unnecessary dependencies.
- Do not change the base architecture without justification.
- Do not delete existing code unless necessary.
- If modifying a skill, keep Markdown valid.

## Final Checklist

- Were the correct skills applied?
- Is WinForms free of business logic?
- Does WinForms consume the API through the centralized client?
- Does backend validate the active company?
- Does backend validate permissions?
- Are controllers/endpoints thin?
- Are handlers clear and focused?
- Does Persistence contain data access?
- Is Domain free of infrastructure?
- Is SQL Server-specific code isolated?
- Is SAP decoupled?
- Are errors standardized?
- Are logs free of secrets?
- Is the code production-ready?
- Are DevExpress forms compatible with the Designer?
- Was official typography applied?
