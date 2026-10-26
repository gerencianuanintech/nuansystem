# Capturas del monitor integradas con el paquete de evidencias

Implementación y validación offline. Ninguna conexión abierta, ejecución SQL ni SAP. La integridad estructural no verifica autenticidad humana ni cierra fase 7.

## Entrada recomendada

Desde el mismo host .NET de la infraestructura de pruebas:

```csharp
var package = Phase7SqlEvidencePackager.BuildFromMonitorCapture(
    runRoot, destination, trustedSuiteApproval, frozenCandidate,
    monitorCaptureDirectory, approvedCaptureIndexSha256, trustedMonitorApproval,
    cancellationToken);
```

La aprobación del monitor es `Phase7SqlControlledMonitorApproval`, suministrada por separado de la captura. La huella de `capture-index.json` debe provenir del resultado del ejecutor y del canal de revisión aprobado; recalcularla desde archivos recibidos no constituye aprobación. El método no ejecuta el monitor, procesos, SQL ni SAP. La entrada anterior `Build`, que recibe objetos, sigue disponible y compatible.

Las raíces son absolutas y separadas. El destino es nuevo y no puede incluir ni estar incluido en la captura o la corrida. No se sobrescriben archivos, no se borran datos ni se corrigen capturas parciales. Los archivos originales se conservan.

## Verificaciones antes de crear el paquete

El lector exige exactamente seis archivos planos: intent.json, observation.json, witness.json, inputs-index.json, report.json y capture-index.json. Rechaza extras, directorios inesperados, enlaces, nombres ambiguos, duplicados, rutas fuera de la raíz, tamaños excesivos, JSON inválido o campos no admitidos. Verifica el hash externo del índice y el tamaño/hash de sus cinco archivos.

El índice de entradas debe contener las seis rutas del ejecutor, con orden y hashes exactos: manifest.json, global/start.json, global/end.json, sql.trx y global/final-A/B.json. Se contrastan con los bytes de la corrida. Cambiar la corrida después de capturar exige una nueva revisión y captura; no se acepta vincularla silenciosamente a otro conjunto de entradas.

Se valida la aprobación de la suite contra su manifiesto y el vínculo de RunId, empresas, bases, PID, catálogo y tiempos del proceso/TRX. El monitor requiere aprobación propia, principal independiente, ventana y presupuestos válidos. Intención, testigo, observación, informe e índice deben coincidir en identidad, autorización y origen. La ventana de aislamiento de workers debe cubrir prueba y observación.

Se recalcula ObservationsConform a partir de los controles existentes de terminación, workers, retención exacta y estado final del protocolo. Un informe contradictorio se rechaza, aunque sus hashes hayan sido recalculados. Una captura negativa coherente se conserva con controles fallidos y bloqueos de revisión.

La captura y las seis entradas se mantienen en memoria tras validarlas. El empaquetador consume esos bytes para sus comprobaciones y artefactos, sin releerlos posteriormente del origen; el resto de evidencias usa sus controles existentes. Se preservan 14 clases de artefactos y 17 controles globales.

## Archivo y nueva verificación

El ZIP conserva los seis documentos originales bajo `monitor-capture/`, junto con `reference.json`, que contiene la huella externa y la aprobación utilizada. Todos forman parte del sello general. El artefacto Environment registra el vínculo con el índice de captura; SessionTermination contiene los objetos provenientes de esa captura.

`VerifyZip` conserva las verificaciones generales y, cuando hay captura vinculada, vuelve a comprobar su inventario, hashes internos, seis entradas, aprobación y coherencia. Contrasta sus objetos con Environment y SessionTermination, su origen y corrida con el informe general, y recalcula NoOrphanSessions, DataPreserved y WorkersAndRelayDisabled. Eliminar la captura dejando su vínculo, o alterar esos controles, no pasa aunque se recalcule el sello exterior. Los paquetes anteriores sin captura vinculada siguen siendo válidos con sus controles previos.

Antes de distribuir o consumir, usar el hash externo del ZIP devuelto por el empaquetador con `VerifyZip(..., expectedSha256: ...)` o `ReadIntegrityVerifiedReport`. Un sello autoconsistente prueba integridad/coherencia; no demuestra quién produjo los archivos. Una persona con capacidad para modificar todos los archivos y sustituir el hash aprobado puede fabricar un paquete nuevo: la revisión independiente de autenticidad sigue siendo obligatoria.

Las simulaciones permanecen Origin=Offline en captura, testigo y paquete; no se convierten a SqlServerLaboratory para aprobar controles. Las pruebas también fabrican formatos rotulados como laboratorio para verificar la compatibilidad estructural; esa etiqueta no acredita SQL real. EvidenceAuthenticityVerified y GlobalPhase7Closed permanecen false, incluso si ReadyForIndependentRunReview resulta true para un formato coherente.

## Pruebas y evidencia

`Phase7SqlMonitorPackagingTests` cubre captura generada por el simulador, alteraciones de cada archivo y entrada, capturas incompletas, inventarios inválidos, identidad/autorización/origen, negativos, cancelación, preservación y ZIP modificado con resellado exterior. Se ejecuta además la regresión offline existente. Evidencias en `artifacts/backend-solid-phase7-sql-monitor-packaging/`.

Resultado final offline: **33 pruebas nuevas y 1759 pruebas totales aprobadas, 0 fallidas y 0 omitidas**. Solución Release: 0 errores y 0 advertencias. Se excluyeron las integraciones SQL y SAP y se mantuvieron sus gates en 0. Revisión independiente final sin defectos importantes confirmados. Se preservaron los cambios actuales; solo se ajustó el empaquetador y se ampliaron las dos notas operativas relacionadas, con copias previas verificadas. No se abrieron conexiones, no se ejecutó SAP ni se modificaron bases de datos. La fase 7 permanece abierta a validación real y revisión de autenticidad.
