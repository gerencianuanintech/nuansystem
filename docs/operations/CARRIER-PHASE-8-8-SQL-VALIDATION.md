# Transportistas 8.8 — evidencia SQL y Dapper

## Resultado

La persistencia y los contratos Dapper de Transportistas Matriz–Sucursal se
validaron el 1 de agosto de 2026. La configuración permanece deshabilitada y no
se ejecutó el relay ni ningún worker.

## Alcance exacto

- `162_tenant_carrier_transactional_outbox.sql` se ejecutó dos veces en
  `NuanSystem_DEMO` y `NuanSystem_DEMO_REMIGIO`.
- `163_master_carrier_transactional_registration.sql` se ejecutó dos veces en
  `NuanSystem_Master`.
- `NuanSystem_DEMO_CANARIS` fue consultada únicamente para comprobar que seguía
  sin la versión `162` y que sus conteos y huellas no cambiaron.
- No se llamó SAP ni SRI y no se inició API, WinForms o workers.

## Respaldos

Se crearon respaldos `COPY_ONLY WITH CHECKSUM` y se verificaron mediante
`RESTORE VERIFYONLY WITH CHECKSUM`:

- `NuanSystem_Master_Phase88_Carrier_20260801_220259.bak`
- `NuanSystem_DEMO_Phase88_Carrier_20260801_220259.bak`
- `NuanSystem_DEMO_REMIGIO_Phase88_Carrier_20260801_220259.bak`

## Gates aprobados

| Gate | Resultado |
|---|---|
| TLS SQL estricto | `Encrypt=true`, `TrustServerCertificate=false` |
| Historia idempotente | Una versión `20260801.162` por tenant desplegado y una `20260801.163` en Master |
| Identidad | `GlobalId` obligatorio y único en DEMO/Remigio |
| Código | Índice único sin filtro; el tombstone conserva la reserva |
| Configuración | `Carrier` registrado una vez, sin configuración u ownership activos y sin dependencias |
| Dapper | Detalle y fuente Full materializados contra SQL Server real |
| Atomicidad | Carrier y LocalOutbox coexistieron en una transacción y desaparecieron juntos al revertirla |
| Aplicación | Evento aplicado en Remigio por `GlobalId` |
| Idempotencia | Segundo uso del mismo `EventId` no duplicó el registro |
| Conflicto | Mismo `Code` con otro `GlobalId` terminó en conflicto sin adopción |
| Constraints | `DBCC CHECKCONSTRAINTS` sin violaciones en DEMO y Remigio |
| Limpieza | Cero fixtures residuales y conteos/huellas funcionales preservados |

El primer intento del validador se detuvo después del despliegue porque el
fixture temporal de colisión construyó una identidad incoherente. El
procedimiento la rechazó correctamente y la transacción fue revertida. Se
corrigió únicamente el fixture y se reanudaron los gates Dapper y de
preservación sin repetir respaldos ni migraciones.

## Regresión

- Build completo: 0 errores y 0 advertencias.
- Suite completa: 747 aprobadas, 5 diagnósticas omitidas y 0 fallidas.
- `git diff --check`: requerido antes del commit documental.

## Pendiente

Esta evidencia no declara completado el piloto end-to-end. Aún requiere una
autorización separada para habilitar temporalmente el relay y la ruta exclusiva
DEMO → Remigio, validar promoción, indisponibilidad de Master y el CRUD completo,
y restaurar exactamente la configuración original.
