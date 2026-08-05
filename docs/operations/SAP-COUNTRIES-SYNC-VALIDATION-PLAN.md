# Plan de validación — SAP Países

## Gate A — Código

- Verificar reader paginado Full y ausencia de `$filter`.
- Probar mapper, normalización y límites de Code/Name/ISO2/ISO3.
- Probar create, update, unchanged, approval y conflict.
- Probar que `PhonePrefix`, `IsActive` y `GlobalId` se preservan.
- Probar snapshot tipado, hash, retry acotado, cancelación y timeout.
- Probar transacción Countries + LocalOutbox y rollback.
- Probar Full source, replay, tombstone y colisión terminal en sucursal.
- Verificar endpoints, DI, capability y navegación.
- Ejecutar build Release, suite dirigida, suite completa y `git diff --check`.

## Gate B — SQL autorizado

Requiere bases y ventana expresamente autorizadas:

1. Respaldos `COPY_ONLY WITH CHECKSUM` y `RESTORE VERIFYONLY WITH CHECKSUM`.
2. Instalar primero el contrato base de Countries en cada tenant que no lo
   tenga.
3. Ejecutar las migraciones nuevas dos veces.
4. Confirmar una sola versión, firmas Dapper, constraints e índices.
5. Confirmar perfiles, rutas, agendas, relay y workers deshabilitados.

## Gate C — SAP autorizado

1. Confirmar en `$metadata` que `Countries` representa el catálogo `OCRY` y
   validar los nombres reales de los campos ISO.
2. Ejecutar preview Full de solo lectura.
3. Confirmar que la petición no contiene `$filter` y recorre todas las páginas.
4. Ejecutar un Full controlado en DEMO.
5. Ejecutar un segundo Full y comprobar idempotencia.
6. Confirmar cero cambios de `GlobalId`, `PhonePrefix` o estado local.

## Gate D — Distribución autorizada

1. DEMO hacia Remigio como único target temporal.
2. Reconciliar Countries, Inbox/Outbox, auditoría, locks y errores.
3. Restaurar configuración.
4. DEMO hacia Cañaris como una oleada independiente.
5. Reconciliar y restaurar toda configuración temporal.

## Criterios de aborto

Abortar ante `$filter`, tenant incorrecto, cambio de `GlobalId`, adopción por
código, reutilización de tombstone, sobrescritura local, target no autorizado,
retry ilimitado, lock inconsistente, secretos visibles o conteos no
reconciliables. No borrar colas ni corregir datos en caliente.

## Gate para Provincias

Provincias no comienza hasta que Países esté idempotente y reconciliado en
DEMO, Remigio y Cañaris, con cero errores, eventos reclamables o locks.

La etapa de Provincias tomará `OCST` como tabla SAP de origen. Antes de codificar
se debe validar en `$metadata` su entity set de Service Layer, el código estable
de provincia/estado y la clave de relación con el país de `OCRY`. No se autoriza
acceso SQL/HANA directo desde Application, API ni WinForms.
