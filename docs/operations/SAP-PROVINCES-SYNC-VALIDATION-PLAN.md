# Validación — Provincias SAP y Matriz–Sucursal

## Gates estáticos

1. Reader `States?$orderby=Country,Code`, paginación completa y ausencia de `$filter`.
2. Mapper y snapshot `ProvinceV1` limitado a país, código y nombre.
3. CRUD + `LocalOutbox` atómicos; rollback conjunto ante fallo del writer.
4. Full incluye tombstones y referencias externas.
5. Aplicación solo por `CountryGlobalId`/`GlobalId`; sin búsquedas de adopción por código.
6. Colisiones de código, referencia externa y padre terminales; dependencia padre ausente reintentable.
7. Scripts 172/173/174 idempotentes, registrados y sin perfiles/agendas.

## Gate SQL/runtime

- Nombrar Master, DEMO y cada sucursal autorizada.
- Respaldos `COPY_ONLY WITH CHECKSUM` y `RESTORE VERIFYONLY`.
- Ejecutar cada script dos veces y confirmar una sola versión.
- Mantener relay/workers deshabilitados.
- Dos Full SAP→DEMO idempotentes; después DEMO→Remigio y DEMO→Cañaris en oleadas independientes.
- Verificar mismo `GlobalId` y `CountryGlobalId`, cero huérfanos, replay idempotente, tombstone y conflictos terminales.
- Restaurar configuración temporal, eliminar fixtures y confirmar cero procesos/locks.

## Aborto

Detener ante `$filter`, adopción por código, cambio de `GlobalId`, reasignación silenciosa de país, huérfanos, targets no autorizados, retry infinito, secretos o configuración activa residual.

Estado actual: despliegue SQL, preview, dos Full SAP→DEMO y distribución de 95
provincias a Remigio/Cañaris validados el 2026-08-04. Todos los eventos y
targets están `Applied`; el perfil temporal quedó inactivo y el worker está
detenido. Ver
[`SAP-PROVINCES-SYNC-RUNTIME-EVIDENCE.md`](SAP-PROVINCES-SYNC-RUNTIME-EVIDENCE.md).
