# Evidencia runtime — Fase 8.9 aislamiento del relay

## Alcance

- Fecha: 2026-08-01, zona `America/Guayaquil`.
- Rama: `refactor/codex-skills-v8-9-relay-hardening`.
- Bases tenant: `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y
  `NuanSystem_DEMO_CANARIS`.
- Migración: `164_tenant_local_outbox_entity_scope.sql`.
- Master, SAP y SRI: fuera del alcance.
- API y workers: detenidos durante toda la validación.

## Respaldos

Se crearon con `COPY_ONLY WITH CHECKSUM` y se verificaron mediante
`RESTORE VERIFYONLY WITH CHECKSUM`:

- `NuanSystem_DEMO_Phase89_164_20260802_023700.bak`;
- `NuanSystem_DEMO_REMIGIO_Phase89_164_20260802_023700.bak`;
- `NuanSystem_DEMO_CANARIS_Phase89_164_20260802_023700.bak`.

## Despliegue

La migración se ejecutó dos veces en cada tenant, cinco lotes por pase. Los
seis pases terminaron correctamente y cada base conserva exactamente una fila
`20260801.164` en `SchemaHistory` y una instancia de cada procedimiento.

Los procedimientos de claim y liberación exponen
`@EnabledEntityNamesJson`; una lista vacía es un no-op y el filtro se aplica
por `EntityName` antes de modificar `LocalOutbox`.

## Validación Dapper real

En cada base se abrieron fixtures identificables dentro de una transacción que
fue revertida:

| Gate | DEMO | Remigio | Cañaris |
|---|---:|---:|---:|
| Release con lista vacía | 0 | 0 | 0 |
| Claims con lista vacía | 0 | 0 | 0 |
| Lease vencido permitido liberado | 1 | 1 | 1 |
| Eventos permitidos materializados | 2 | 2 | 2 |
| Pending bloqueado preservado | Sí | Sí | Sí |
| Lease bloqueado preservado | Sí | Sí | Sí |
| Materialización Dapper | Aprobada | Aprobada | Aprobada |

Las entidades de prueba fueron `Phase89Allowed` y `Phase89Blocked`. No se
usaron entidades funcionales reales. Finalizada cada transacción, ambas
entidades quedaron con cero filas y los conteos y checksums funcionales de
`LocalOutbox` y `SyncAudit` coincidieron con la línea base.

## Seguridad y estado final

- Conexiones SQL: `Encrypt=true`, `TrustServerCertificate=false`.
- Credenciales y cadenas: cargadas únicamente en memoria, sin mostrarse ni
  persistirse en la evidencia.
- Workers NuanSystem: cero durante el despliegue y la validación.
- Llamadas SAP/SRI: cero.
- Configuración de perfiles, rutas y entidades: sin cambios.
- Fixtures residuales: cero.

La migración SQL y el contrato Dapper quedan aprobados. La activación real del
relay continúa siendo una decisión operativa separada y debe realizarse con
allowlist explícita y ventana controlada.
