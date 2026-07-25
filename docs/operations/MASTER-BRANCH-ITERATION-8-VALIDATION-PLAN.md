# Plan de validación — Iteración 8 transactional outbox

## Propósito

Validar el límite tenant `LocalOutbox` → Master `SyncOutbox` sin habilitar
entidades adicionales ni confundir promoción con aplicación en sucursal.

Fuente arquitectónica:
[MASTER-BRANCH-ITERATION-8-TRANSACTIONAL-OUTBOX-BLUEPRINT.md](../architecture/MASTER-BRANCH-ITERATION-8-TRANSACTIONAL-OUTBOX-BLUEPRINT.md).

## Separación de gates

### Gate A — Código y contratos

- Build completo sin errores ni advertencias.
- Pruebas unitarias de handler, writer, relay y promoción.
- Pruebas contractuales de SQL, DI, configuración segura y ausencia de SAP/SRI.
- Confirmar que `BusinessPartner` usa un solo camino de publicación.

### Gate B — SQL autorizado

Requiere autorización que nombre las bases.

- Backup/restore policy acordada antes de migrar.
- Script tenant ejecutado dos veces.
- Script Master ejecutado dos veces.
- Historia de schema sin duplicados.
- Objetos, índices y constraints confiables.
- Claims concurrentes exclusivos.
- Lease vencido recuperable.
- Completion por owner.
- Retry y `DeadLetter` acotados.
- Promoción repetida con mismo contenido: idempotente.
- Mismo `EventId` con contenido distinto: conflicto.

### Gate C — Transacción tenant

Usar un fixture creado para la prueba; no reutilizar datos protegidos.

1. Forzar fallo antes de commit: no existe maestro ni `LocalOutbox`.
2. Confirmar éxito: existe maestro y exactamente una intención.
3. Forzar fallo del insert de intención: se revierte el maestro.
4. Create/update/delete lógico conservan operación y snapshot correctos.
5. Dos solicitudes equivalentes no comparten `EventId`.

### Gate D — Master no disponible

1. Confirmar que el CRUD tenant termina con éxito durable.
2. `LocalOutbox` queda elegible y sin lock residual.
3. Retry aumenta de forma acotada y registra error saneado.
4. Restablecer Master y promover el mismo `EventId`.
5. Confirmar una fila `SyncOutbox`, targets únicos y cierre local.

### Gate E — Crash window

Simular commit Master exitoso antes de actualizar `LocalOutbox`.

- El segundo intento recupera `SyncOutbox` por `EventId`.
- No duplica targets, decisiones ni auditoría incompatible.
- Cierra la intención local como `Applied`.

### Gate F — Runtime controlado

Requiere autorización separada.

- Solo una empresa Master y una entidad piloto.
- `SkeletonMode` y relay parten deshabilitados.
- `BusinessPartner` es la única entidad del piloto.
- Una sola sucursal autorizada.
- Cero llamadas SAP y SRI.
- Capturar estados LocalOutbox, SyncOutbox, target, SyncInbox y auditoría.
- Detener procesos y retirar fixtures al terminar.

## Evidencia mínima

```text
Branch/commit:
Bases autorizadas:
Configuración efectiva saneada:
Conteos iniciales/finales:
EventId del fixture:
GlobalId del fixture:
Transiciones LocalOutbox:
Resultado promoción Master:
Targets:
Resultado sucursal:
Auditorías:
Procesos finales:
Git final:
```

No imprimir payload completo, conexiones, tokens, claves, certificados ni datos
personales reales.

## Criterios de aborto

Detener sin avanzar al gate siguiente cuando:

- una segunda ejecución SQL no es idempotente;
- maestro e intención local divergen;
- hay dos owners de un lease vigente;
- una colisión de payload se acepta como idempotente;
- aparecen targets duplicados;
- el relay llama SAP o SRI;
- se habilita otra entidad/empresa/sucursal;
- la limpieza no puede preservar evidencia y datos ajenos.

## Estado

Gate A de Fase 8.1 ejecutado para contratos y código. Build y pruebas
automatizadas quedan registrados en el cierre de la rama. Los gates B a F no
han sido ejecutados: requieren las autorizaciones separadas ya definidas.
