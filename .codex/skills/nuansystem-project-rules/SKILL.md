---
name: nuansystem-project-rules
description: Route cross-layer NuanSystem work to the correct backend, frontend, SQL, integration, worker, or deployment orchestrator while enforcing global boundaries. Use for architecture decisions or tasks spanning multiple projects or layers; do not load it for an isolated specialist-only review when a narrower skill fully owns the request.
---

# NuanSystem Project Rules

## Purpose and authority

Use this skill as a lightweight router for cross-layer work. It complements, but does not duplicate, the engineering core or specialist instructions.

Run `$nuansystem-framework-discovery` before designing a non-trivial change;
it owns loading and recording the applicable engineering-core evidence. Reuse
that record here, then route to the minimum NuanSystem skills and verified
repository implementations. Do not reread complete catalogs from this router.

## Progressive skill routing

Start with this router, Framework Discovery, and one primary orchestrator. Add a specialist only after the affected-layer map proves that its contract changes. Do not load every possible specialist in advance.

| Primary work | Start with | Add only when affected |
|---|---|---|
| Backend feature or CRUD | `$nuansystem-backend-architecture` | CRUD, CQRS, endpoints, validation, persistence, SQL, tenancy, errors, or tests specialist |
| Operational stock, money, documents, or workflow | `$nuansystem-operational-usecase` | SQL, security, errors, integration, and tests specialist |
| WinForms screen or frontend behavior | `$nuansystem-winforms-devexpress` | Forms, Designer, layout, controls, grids, lookups, navigation, or API-client specialist |
| Auxiliary administrative master | `$nuansystem-auxiliary-master-generator` | Specialists identified by the approved manifest and vertical impact |
| Master/branch replication | `$nuansystem-master-branch-sync` | SQL, persistence, security, worker, and tests specialist |
| SAP integration or SAP synchronization | `$nuansystem-sap-business-one` or `$nuansystem-sap-sync-orchestration` | Operational, errors, security, SQL, and tests specialist |
| SRI queue or worker | `$nuansystem-sri-document-queue` or `$nuansystem-sri-worker` | Security, SQL, API, frontend monitor, and tests specialist |
| Authentication | `$nuansystem-security-auth` | API client, tenancy, endpoints, errors, and tests specialist |
| Deployment | `$nuansystem-deployment-production` | Only the component-specific skills changed by deployment work |

Examples of selective activation:

- Changing only grid columns: frontend orchestrator plus grid specialist; no API-client skill unless the response contract changes.
- Adding only a typed frontend endpoint method: frontend API-client skill; add forms only if a screen also changes.
- Changing a backend validator: backend architecture plus validation and tests; no SQL skill unless database defenses or contracts change.

## Non-negotiable boundaries

- NuanSystem must operate independently without SAP; SAP is optional per company.
- Domain contains pure business rules and does not depend on SQL Server, Dapper, WinForms, SAP, SRI, or external services.
- WinForms consumes the REST API through the centralized client and never accesses SQL, SAP, or SRI directly.
- API endpoints remain thin; Application owns use-case orchestration and contracts.
- Persistence owns database access and provider details.
- Master owns companies, branches, capabilities, connections, integrations, and global configuration.
- Tenant databases own local operations and cannot know secrets from other tenants.
- Master/branch replication uses Outbox/Inbox, idempotency, audit, and observable retry behavior.
- SRI remains independent of SAP; capturers enqueue, and the SRI worker alone performs remote XML processing.
- Business truth, authorization, company access, and transaction results are validated server-side.
- Never log secrets, passwords, tokens, connection strings, or raw integration credentials.
- Preserve cancellation, standard errors, correlation, audit identity, and tenant isolation across affected flows.

## Classification and vertical impact

Before implementation, classify the request as one primary type:

- administrative CRUD;
- operational use case;
- synchronization;
- external integration;
- worker;
- security;
- deployment;
- shared framework;
- documentation or refactor.

Then mark each layer as **Change**, **Verify unchanged**, **Not applicable**, or **Blocked**:

```text
Domain
Application
Persistence
API
Database
Frontend services
Frontend forms/designer
Security/menu
Integration/synchronization/worker
Tests
Documentation/catalogs
```

Stock, money, pricing, cash, purchasing, document state, or external-system state always route through the operational pattern, even when the UI resembles CRUD.

## Reuse and ownership

- Preserve explicit domain ownership; a nearby entity is evidence of a pattern, not permission to merge aggregates.
- Reuse an exact approved component before configuring, extending, or creating infrastructure.
- Shared framework changes require consumer discovery and updates to the Framework Catalog and Knowledge Graph.
- Administrable masters retain independent backend, persistence, API, frontend, navigation, permissions, audit, and tests when their approved ownership requires it.
- Do not create parallel HTTP clients, result types, permission systems, base forms, grids, lookups, sync pipelines, or SQL abstractions.

## Validation and delivery

Validation must match risk and affected layers. At minimum:

- verify referenced files and symbols;
- inspect the scoped diff;
- run affected builds and tests when executable code changes;
- validate negative security, tenant, error, transaction, retry, or idempotency paths when applicable;
- report each check as **Validated**, **Not validated**, **Not applicable**, or **Blocked**.

Complete the work only when the requested outcome is present, unrelated changes are preserved, no silent placeholders remain, and every affected layer has an explicit disposition.

When auditing or updating project-local skills, run the UTF-8-safe validator:

```powershell
& .codex/skills/nuansystem-project-rules/scripts/Test-NuanSystemSkills.ps1
```
