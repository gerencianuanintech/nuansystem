# Iteracion 5 — Blueprint de cola documental y worker SRI

## Estado

**Fase 5.2 desplegada y validada; Fase 5.3 implementada en codigo y pruebas.** Cola SQL, Application/API, permisos, idempotencia, concurrencia, auditoria y JWT renovado fueron comprobados en base real. El proveedor oficial de consulta, `NuanSystem.SriWorker`, claims/leases y almacenamiento XML tenant ya existen en codigo/script `117`, pero no se han desplegado ni ejecutado contra el SRI; por tanto todavia no hay validacion end-to-end.

## Discovery Record

- **Tipo:** integracion externa asincrona, multiempresa y transaccional.
- **Dominio:** documentos electronicos SRI, independiente de SAP Business One.
- **Propiedad:** Master configura capacidades/secretos; la base tenant conserva la operacion y trazabilidad de cada empresa.
- **Evidencia existente:** `SRI_DOCUMENTS`, integracion `SRI`, configuracion protegida y ambos deshabilitados por defecto en `062_master_tenant_configuration.sql`.
- **Referencias de ciclo de vida:** `NuanSystem.SyncWorker` y `NuanSystem.MasterBranchSyncWorker`, sin reutilizar sus colas ni sus responsabilidades.
- **Ausencias confirmadas al inicio:** tablas, repositorios, API, permisos, formularios, almacenamiento XML, cliente SRI y worker dedicados. Fases 5.2/5.3 cubren cola, API, permisos, proveedor, worker y XML store; monitor y descarga protegida siguen pendientes.

### Evidencia XML real revisada para Fase 5.2

Se inspeccionaron localmente 11 documentos descargados del SRI sin copiarlos al repositorio ni registrar claves, RUC o XML completos:

- 10 respuestas SOAP con raiz `Envelope` y 1 autorizacion con raiz `autorizacion`;
- 11 estados `AUTORIZADO`;
- 5 facturas `01` (`factura` v1.1.0);
- 4 notas de credito `04` (`notaCredito` v1.1.0);
- 2 comprobantes de retencion `07` (`comprobanteRetencion` v1.0.0 y v2.0.0);
- las 11 claves contenian 49 digitos, coincidian entre envoltura/documento y superaron modulo 11.

Esta evidencia cierra el catalogo inicial de tipos admitidos por la cola, pero no prueba llamadas al proveedor, almacenamiento XML ni procesamiento del worker.

## Limites de propiedad

```text
Capturador (NuanSystem/TXT/AddOn/formulario/API)
  -> Application/API: validar minimo y encolar durablemente
  -> Base tenant: cola + intentos + documento/XML + auditoria
  -> NuanSystem.SriWorker: claim + proveedor + XML + transiciones/reintentos
  -> Application/API: consultas y acciones manuales autorizadas
  -> WinForms: monitor sin SQL ni llamadas SRI
```

No se comparte `SyncOutbox`, SAP outbox, handlers SAP ni workers existentes. Una falla SAP no bloquea SRI y una falla SRI no revierte una transaccion comercial ya confirmada, salvo regla de negocio futura explicitamente aprobada.

## Contratos objetivo

### Persistencia tenant

El diseno detallado debe resolver, como minimo:

- cola con identidad estable, tenant/empresa/sucursal, origen, referencia de origen, prioridad, estado y version de concurrencia;
- clave de idempotencia protegida por indice unico;
- lease atomico con propietario, adquisicion, expiracion y recuperacion;
- historial inmutable de intentos, clasificacion de error y correlacion remota;
- documento/XML con checksum, tipo, tamano, referencia de storage y metadatos de autorizacion;
- auditoria de enqueue, cancelacion, reproceso, descarga y acceso sensible.

Los nombres de tablas, procedimientos e indices se aprueban en la fase de implementacion SQL y siguen `$nuansystem-sql-standards`.

### Application y API

Casos previstos: encolar, consultar lista/detalle, consultar intentos, cancelar si el estado lo permite, reprocesar con motivo, descargar XML autorizado y observar resumen operativo. Permisos provisionales:

- `SRI.DOCUMENTS.VIEW`
- `SRI.DOCUMENTS.ENQUEUE`
- `SRI.DOCUMENTS.CANCEL`
- `SRI.DOCUMENTS.REPROCESS`
- `SRI.DOCUMENTS.VIEW_PAYLOAD`
- `SRI.DOCUMENTS.DOWNLOAD_XML`

La nomenclatura definitiva debe alinearse con los aliases de seguridad existentes antes del script Master.

### Estados

La arquitectura base propone `Pending`, `Validating`, `Submitted`, `Authorized`, `Rejected`, `DownloadPending`, `Downloaded`, `Processed`, `Failed`, `RetryScheduled` y `DeadLetter`. Son candidatos, no un enum aprobado. El flujo piloto elegido debe definir transiciones legales, actor, precondicion, persistencia requerida y recuperacion.

## Fases de ejecucion

### 5.1 Contrato funcional y seguridad — direccion aprobada

La direccion aprobada es consulta y descarga por clave de acceso de comprobantes previamente autorizados. El contrato autoritativo es `SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md`. Las siete reglas operativas de proveedor, storage, tamano, retencion, TLS, retry y limites fueron aprobadas el 2026-07-20. No existe procesamiento remoto habilitado todavia.

### 5.2 Cola durable - desplegada y validada

Se implementaron contratos Application, repositorio Dapper, endpoints protegidos, scripts `115_tenant_sri_document_queue.sql` y `116_master_sri_document_queue_security.sql`, validacion de clave/ambiente, idempotencia `(Environment, AccessKey)`, auditoria y concurrencia por `rowversion`. El catalogo inicial comprobado con muestras reales autorizadas comprende factura `01`, nota de credito `04` y comprobante de retencion `07`.

Los XML reales usados para discovery no forman parte del repositorio. El 2026-07-20 se ejecutaron dos veces, sin error ni duplicados, el script `116` en `NuanSystem_Master` y el script `115` en `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS`. Se validaron 17 pruebas SRI, 411 pruebas de solucion, enqueue concurrente con una sola identidad, `rowversion`, transiciones manuales, auditoria, permisos ADMIN, rechazo HTTP 403 y JWT renovado.

`NuanSystem_DEMO` queda habilitada para el piloto en `Production`; Remigio y Canaris conservan SRI deshabilitado. La API y los usuarios efimeros de validacion fueron retirados al terminar. Esta fase no crea proveedor, worker, XML storage ni endpoint de descarga.

### 5.3 Worker, proveedor y almacenamiento XML - implementado; despliegue pendiente

Se implementaron `NuanSystem.SriWorker`, proveedor SOAP de `AutorizacionComprobantesOffline`, resolucion multiempresa, claims atomicos, lease de 120 segundos, recuperacion de leases, lote 10, concurrencia 2, timeout 30 segundos, cinco intentos tecnicos, backoff con jitter y cierre `NotFound` tras tres respuestas o 30 minutos. El XML autorizado se valida contra tipo, clave, RUC y ambiente y se almacena en la base tenant con SHA-256 y limite de 5 MiB. TLS es estricto y las URLs aceptadas se restringen a los hosts oficiales.

La compilacion del worker termino con 0 errores/advertencias y 33 pruebas SRI superaron. No se ejecuto `117`, no se habilito el worker y no se hicieron llamadas reales al SRI. El despliegue y la validacion de concurrencia/reinicio/proveedor siguen pendientes.

### 5.4 Almacenamiento y monitor

Completar descarga XML protegida, auditoria de acceso y monitor WinForms mediante `NuanApiClient` y framework corporativo. La UI no modifica estados arbitrariamente y no recibe rutas fisicas ni acceso SQL.

### 5.5 Validacion end-to-end

Ejecutar captura -> cola -> claim -> proveedor no productivo -> persistencia XML/resultado -> consulta UI/API, incluyendo duplicado, rechazo, timeout, retry, reinicio, lease vencido, dos workers, permiso y redaccion.

## Decisiones requeridas antes de 5.1

1. **Resuelto:** consulta/descarga de documentos ya autorizados por clave de acceso.
2. **Resuelto:** servicio offline oficial, con Test y Production configurables y validacion real solo bajo autorizacion.
3. **Resuelto:** XML en base tenant, inmutable, SHA-256, 5 MiB y sin purga automatica.
4. Alcance exacto de `AccessKey` y clave de idempotencia.
5. **Parcialmente resuelto:** 5 MiB, TLS/log redaction y sin eliminacion; descarga/auditoria de acceso se completa en 5.4.
6. Relacion con tipos e identidad de documentos comerciales locales.

## Quality gates

- Las decisiones anteriores tienen evidencia y propietario.
- SAP, Matriz-Sucursal y SRI permanecen separados.
- Scripts son idempotentes y forward-safe; concurrencia se prueba en base real.
- API aplica empresa confiable, permisos y errores estables.
- Dos workers no procesan el mismo trabajo; leases vencidos se recuperan.
- Reintentos son acotados y los terminales quedan visibles/auditados.
- Secretos, XML y datos tributarios no aparecen en logs ordinarios.
- Solo una prueba real contra el ambiente SRI aprobado permite declarar E2E validado.

## Skills vinculados

- `$nuansystem-sri-document-queue`: captura, persistencia, API, permisos y monitor.
- `$nuansystem-sri-worker`: claim, proveedor, XML, reintentos, dead letter y observabilidad.
- `$nuansystem-framework-discovery`, `$nuansystem-operational-usecase` y `$nuansystem-sql-standards`: obligatorios segun la capa.
