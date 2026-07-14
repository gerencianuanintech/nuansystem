# Consolidacion de skills Codex para NuanSystem

Este documento orienta que skills usar para implementar la arquitectura objetivo sin duplicar reglas.

## Skill rector

Usar `nuansystem-project-rules` como entrada principal cuando el cambio toque mas de una capa.

## Skills por area

- Arquitectura comercial y capas: `nuansystem-commercial-architecture`.
- Capacidades por empresa/sucursal: `nuansystem-business-capabilities`.
- CRUD administrativo: `nuansystem-backend-crud`.
- Procesos operativos: `nuansystem-operational-usecase`.
- CQRS/Application: `nuansystem-mediatr-cqrs`.
- SQL Server y scripts: `nuansystem-sql-standards`.
- Compatibilidad futura MySQL: `nuansystem-database-provider-compatibility`.
- SAP Business One: `nuansystem-sap-business-one`.
- Seguridad y tenancy: `nuansystem-security-auth`.
- Errores y logging: `nuansystem-api-error-logging`.
- WinForms DevExpress: `nuansystem-winforms-devexpress` es el unico skill frontend principal.
- Cliente API WinForms: `nuansystem-frontend-api-client`.
- Tipografia WinForms: referencia interna `nuansystem-winforms-devexpress/references/enterprise-typography.md`; no activar como skill independiente.
- Produccion y workers: `nuansystem-deployment-production`.

## Nuevas areas objetivo

La arquitectura Master/Sucursal y SRI Worker se implementa combinando skills existentes:

- Master/Sucursal: `nuansystem-commercial-architecture`, `nuansystem-business-capabilities`, `nuansystem-sql-standards`, `nuansystem-security-auth`.
- Outbox/Inbox: `nuansystem-operational-usecase`, `nuansystem-sql-standards`, `nuansystem-api-error-logging`, `nuansystem-deployment-production`.
- SRI Worker: `nuansystem-operational-usecase`, `nuansystem-sql-standards`, `nuansystem-api-error-logging`, `nuansystem-deployment-production`.
- SAP opcional: `nuansystem-sap-business-one`, sin contaminar `Domain`.
- UI de configuracion: `nuansystem-winforms-devexpress`, `nuansystem-frontend-api-client` y referencias internas del skill frontend.

## Consolidacion frontend

`nuansystem-winforms-devexpress` es el unico skill frontend principal para WinForms DevExpress.

La tipografia empresarial ya no debe activarse como skill independiente. Los skills `nuansystem-enterprise-typography` y `devexpress-enterprise-typography` quedan como stubs de compatibilidad que apuntan a:

```text
.codex/skills/nuansystem-winforms-devexpress/references/enterprise-typography.md
```

Las reglas especializadas de frontend viven como referencias internas:

- `references/enterprise-typography.md`
- `references/designer-compatibility.md`
- `references/service-clients.md`
- `references/operational-forms.md`

No crear skills frontend por entidad o modulo, como `frontend-items`, `frontend-suppliers` o `frontend-sync`. Cuando una entidad necesite reglas especiales, documentarlas como referencia interna del skill principal o en documentacion de arquitectura si el alcance es transversal.

## Regla de aplicacion

Si un prompt de `docs/codex/prompts` toca varios dominios, leer primero:

- `AGENTS.md`
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`
- `docs/architecture/SRI-DOCUMENTS-WORKER.md`

Luego aplicar solo las skills necesarias para esa tarea concreta.
