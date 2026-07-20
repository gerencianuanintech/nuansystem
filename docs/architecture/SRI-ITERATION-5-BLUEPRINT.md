# Iteracion 5 — Blueprint de cola documental y worker SRI

## Estado

**Arquitectura y contratos solamente.** Al inicio de esta iteracion no existen una cola SRI operativa, un proveedor SRI productivo ni el proyecto `NuanSystem.SriWorker`. Este documento no autoriza a declarar implementado ningun nodo futuro.

## Discovery Record

- **Tipo:** integracion externa asincrona, multiempresa y transaccional.
- **Dominio:** documentos electronicos SRI, independiente de SAP Business One.
- **Propiedad:** Master configura capacidades/secretos; la base tenant conserva la operacion y trazabilidad de cada empresa.
- **Evidencia existente:** `SRI_DOCUMENTS`, integracion `SRI`, configuracion protegida y ambos deshabilitados por defecto en `062_master_tenant_configuration.sql`.
- **Referencias de ciclo de vida:** `NuanSystem.SyncWorker` y `NuanSystem.MasterBranchSyncWorker`, sin reutilizar sus colas ni sus responsabilidades.
- **Ausencias confirmadas:** tablas, repositorios, API, permisos, formularios, almacenamiento XML, cliente SRI y worker dedicados.

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

### 5.1 Contrato funcional y seguridad

Decidir direccion piloto, proveedor/ambiente, identidad idempotente, storage XML, retencion, privacidad, limites y relacion con documento local. Entregable: ADR/contrato aprobado, sin procesamiento remoto.

### 5.2 Cola durable

Crear contratos Application, SQL tenant versionado, repositorio Dapper, endpoints y permisos. Validar concurrencia, duplicados, tenant isolation, transiciones y JWT renovado. No crear aun un proveedor falso reportado como operativo.

### 5.3 Worker y proveedor

Crear `NuanSystem.SriWorker` con hosting, claims, leases, cancelacion, concurrencia acotada, clasificacion de fallas, retry/backoff, dead letter, health y observabilidad. El proveedor real requiere ambiente no productivo y politica de certificados/secretos aprobada.

### 5.4 Almacenamiento y monitor

Implementar abstraccion XML, integridad, acceso protegido y monitor WinForms mediante `NuanApiClient` y framework corporativo. La UI no modifica estados arbitrariamente.

### 5.5 Validacion end-to-end

Ejecutar captura -> cola -> claim -> proveedor no productivo -> persistencia XML/resultado -> consulta UI/API, incluyendo duplicado, rechazo, timeout, retry, reinicio, lease vencido, dos workers, permiso y redaccion.

## Decisiones requeridas antes de 5.1

1. Primer piloto: envio/autorizacion saliente o consulta/descarga de documentos ya autorizados.
2. Proveedor y ambiente oficial de pruebas SRI.
3. Almacenamiento XML: base de datos, archivos o storage externo.
4. Alcance exacto de `AccessKey` y clave de idempotencia.
5. Retencion, privacidad, tamano maximo y permisos de payload/XML.
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
