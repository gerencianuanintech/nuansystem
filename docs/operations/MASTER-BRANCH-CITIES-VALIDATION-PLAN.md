# Plan de validación — Cities Matriz–Sucursal

## Estado

Plan preparado el 2026-08-04. Ningún paso SQL o runtime ha sido ejecutado por esta implementación.

## Gates estáticos

1. Compilar Application, Persistence, API y MasterBranchSyncWorker.
2. Ejecutar las pruebas dirigidas de Geography/Cities y Sync/Cities.
3. Confirmar que no existe `CitySyncPublisher` ni publicación directa desde los handlers.
4. Confirmar que CRUD, relectura y `ICityLocalOutboxWriter` reciben la misma conexión/transacción.
5. Confirmar que el Full transporta `IsDeleted`, referencias externas y ambos padres.
6. Confirmar que repositorio y migración no contienen adopción de `GlobalId` ni truncado.
7. Confirmar registro de `175` después de `172/173` en el inicializador tenant.

## Gate SQL futuro — requiere autorización

1. Respaldar y verificar cada tenant expresamente autorizado.
2. Comprobar cero duplicados por `(ProvinceId, Code)` y por referencia externa no nula.
3. Ejecutar `175` dos veces y verificar una sola versión `20260804.175`.
4. Confirmar columnas, índices y procedimientos esperados.
5. Probar dentro de transacción revertida: jerarquía inválida, código tombstone reservado y longitudes máximas.
6. Comparar conteos y huellas de Cities, LocalOutbox, SyncInbox y SyncAudit antes/después.

## Gate runtime futuro — requiere autorización independiente

1. Mantener SAP, SRI y workers ajenos apagados.
2. Configurar una única ruta piloto explícita; mantener Cities y relay deshabilitados fuera del intervalo controlado.
3. Validar create, update, disable y delete lógico con un evento durable por mutación.
4. Forzar fallo del outbox y comprobar rollback conjunto.
5. Promover dos veces el mismo `EventId` y comprobar idempotencia.
6. Aplicar en sucursal por `GlobalId`, verificando ids locales independientes.
7. Validar dependencia Country y Province faltante como reintentable.
8. Validar jerarquía incorrecta, cambio de padres, colisión de código, tombstone y referencia externa como terminales sin adopción.
9. Ejecutar Full dos veces; comprobar tombstones e idempotencia.
10. Limpiar fixtures, restaurar configuración exacta y confirmar cero procesos/locks/eventos reclamables.

## Criterio de cierre

Código compilable, pruebas dirigidas y suite afectada aprobadas; SQL desplegado dos veces con respaldo; piloto controlado limpio; configuraciones restauradas. Hasta completar todos esos gates, el estado runtime permanece pendiente.
