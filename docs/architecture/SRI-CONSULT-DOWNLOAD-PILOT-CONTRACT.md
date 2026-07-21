# Contrato del piloto SRI — consulta y descarga de comprobantes autorizados

## Decision Record

- **Estado de la decision:** Aprobada por el propietario del proyecto.
- **Estado de implementacion:** Fases 5.2 a 5.5 implementadas y validadas segun su alcance. Fase 5.4 completo el recorrido end-to-end en `NuanSystem_DEMO` contra el ambiente oficial `Production`; Fase 5.5 desplego y valido descarga protegida, auditoria de acceso, aislamiento tenant y monitor sin repetir la llamada al SRI. Evidencia saneada en `SRI-WORKER-DEPLOYMENT.md` y [`../operations/SRI-DOCUMENT-MONITOR.md`](../operations/SRI-DOCUMENT-MONITOR.md).
- **Fecha:** 2026-07-20.
- **Direccion:** Consulta y descarga por clave de acceso de comprobantes previamente autorizados.
- **Excluido:** generacion, firma, recepcion, envio para autorizacion, anulacion y scraping del portal web.

## Motivo

El piloto debe validar primero la infraestructura transversal —cola durable, idempotencia, worker, almacenamiento XML, permisos, reintentos y observabilidad— sin introducir firma electronica ni emision tributaria. La integracion se mantiene independiente de SAP Business One.

## Autoridad externa

La implementacion debe verificarse contra la informacion tecnica vigente publicada por el Servicio de Rentas Internas antes de codificar el adaptador:

- Portal oficial: `https://www.sri.gob.ec/facturacion-electronica`.
- Ficha tecnica oficial del esquema offline publicada por el SRI.

La ficha confirma ambientes separados y consulta de autorizacion mediante servicios web. Los endpoints, WSDL y esquemas nunca se copian como constantes desde este documento: se validan nuevamente y se administran por configuracion protegida.

## Flujo funcional aprobado

```text
Origen autorizado entrega Environment + AccessKey + SourceReference
  -> API valida forma y permiso
  -> tenant DB crea/encuentra una solicitud idempotente
  -> worker reclama la solicitud
  -> adaptador oficial consulta autorizacion por AccessKey
  -> respuesta autorizada y XML se validan y almacenan durablemente
  -> solicitud pasa a Authorized
  -> API/WinForms permiten consultar metadatos y descargar XML por permiso
```

No existe dependencia obligatoria con un documento comercial local. Si se proporciona una referencia local, se conserva como relacion opcional y nunca redefine la identidad tributaria.

## Entrada minima

- `Environment`: `Test` o `Production`, resuelto contra configuracion tenant habilitada.
- `AccessKey`: 49 digitos numericos con digito verificador valido.
- `SourceType`: valor cerrado y versionado, por ejemplo `NuanSystem`, `Txt`, `SapAddOn`, `Manual` o `ExternalApi`.
- `SourceReference`: referencia inmutable del origen dentro de su tipo.
- `BranchCode`: opcional cuando el origen pertenece a una sucursal.
- `TraceId`: correlacion tecnica generada o normalizada por backend.

El backend obtiene empresa/tenant desde el contexto confiable. No acepta tenant, URL SRI, estado, intentos ni autorizacion como autoridad del cliente.

## Identidad e idempotencia

Dentro de cada base tenant, la identidad del piloto es:

```text
(Environment, AccessKey)
```

Debe existir una restriccion unica en base de datos. Una segunda solicitud concurrente devuelve el registro existente y no crea otro trabajo. `SourceType` y `SourceReference` quedan como trazabilidad; no forman una segunda identidad para la misma clave.

El worker vuelve a consultar solo mediante una accion de retry/reprocess permitida o una politica de reintento vigente; nunca crea una segunda fila logica.

## Estados aprobados para el piloto

| Estado | Significado |
|---|---|
| `Pending` | Solicitud durable, elegible para claim. |
| `Querying` | Worker posee lease vigente y consulta al proveedor. |
| `RetryScheduled` | Falla transitoria o respuesta aun no disponible; tiene proximo intento. |
| `Authorized` | Respuesta autorizada y XML durable con integridad verificada. |
| `NotFound` | El SRI no reporto autorizacion despues de agotar la ventana funcional aprobada. |
| `Failed` | Falla permanente funcional o de contrato que requiere revision. |
| `DeadLetter` | Falla tecnica agoto intentos y requiere accion manual auditada. |
| `Cancelled` | Cancelada antes de consulta efectiva por una accion permitida. |

Transiciones permitidas:

```text
Pending -> Querying | Cancelled
Querying -> Authorized | RetryScheduled | NotFound | Failed | DeadLetter
RetryScheduled -> Querying | Cancelled
Failed -> Pending        (reproceso manual con motivo)
DeadLetter -> Pending    (reproceso manual con permiso y motivo)
```

`Authorized`, `NotFound` y `Cancelled` son terminales para el piloto. Una descarga no cambia estado. `Authorized` solo se confirma después de persistir la respuesta y el XML.

## Resultado autorizado

Conservar como minimo:

- clave y ambiente;
- numero, fecha y estado de autorizacion devueltos por el proveedor;
- tipo de comprobante y RUC emisor obtenidos del XML cuando sean validos;
- bytes originales del XML autorizado o referencia inmutable al storage aprobado;
- `SHA-256`, longitud, content type y fecha de almacenamiento;
- identificador/correlacion remota disponible;
- intento que produjo el resultado.

El parser crea metadatos de consulta, pero los bytes originales son la evidencia documental. Un fallo de parseo nunca sustituye ni altera silenciosamente el XML recibido.

## Seguridad

- Capacidad `SRI_DOCUMENTS` e integracion `SRI` deben estar activas para el tenant y ambiente.
- Validacion TLS es obligatoria. No se admite `IgnoreSslErrors` en operacion.
- URLs y cualquier credencial se obtienen de configuracion protegida; no llegan desde WinForms ni desde el payload.
- La clave de acceso se enmascara en logs ordinarios; XML y datos tributarios completos no se registran.
- Ver metadatos, ver payload, descargar XML, cancelar y reprocesar son permisos backend independientes.
- Toda descarga y accion manual registra usuario/proceso, tenant, motivo cuando aplique y `TraceId`.
- El piloto no administra certificados de firma porque no genera ni firma comprobantes.

## Politica de errores y reintentos

- Timeout, red, indisponibilidad y throttling: transitorios, con backoff acotado y jitter.
- Respuesta valida sin autorizacion aun disponible: reintento funcional dentro de una ventana configurable.
- Clave invalida, ambiente incompatible o XML inconsistente: `Failed` sin retry automatico.
- Credencial/configuracion ausente o TLS invalido: falla tecnica visible; nunca se omite seguridad.
- Intentos tecnicos agotados: `DeadLetter`.

Limites aprobados: cinco intentos tecnicos, timeout de 30 segundos, backoff exponencial con jitter, lease de 120 segundos y tres respuestas sin autorizacion dentro de una ventana de 30 minutos antes de `NotFound`.

## Almacenamiento XML

La decision aprobada almacena el XML autorizado en la base tenant. `SriAuthorizedDocuments` conserva identidad unica por cola y por `(Environment, AccessKey)`, metadatos de autorizacion, intento, bytes XML, content type, tamano y SHA-256. La transicion a `Authorized` y el almacenamiento ocurren en la misma transaccion.

El tamano maximo es 5 MiB. No existe purga automatica ni eliminacion manual aprobada hasta definir retencion legal/operativa.

## Quality gates del piloto

1. Dos enqueue simultaneos producen una sola identidad.
2. Dos workers no consultan la misma solicitud con leases vigentes.
3. Reinicio y lease vencido recuperan trabajo sin duplicar XML.
4. Respuesta repetida produce el mismo documento y checksum.
5. `Authorized` nunca existe sin XML/referencia durable.
6. Not found, rechazo de contrato, timeout y TLS invalido quedan diferenciados.
7. Tenant deshabilitado no encola ni procesa.
8. JWT renovado refleja permisos de consulta, descarga y reproceso.
9. Logs y auditoria no exponen XML ni clave completa.
10. Solo un recorrido real contra ambiente oficial aprobado permite marcar E2E como validado.

## Cierre de Fases 5.4 y 5.5

La Fase 5.4 valido habilitacion temporal, recorrido oficial `Production`, persistencia autorizada, integridad, auditoria, liberacion de lease, redaccion, detencion e idempotencia en `NuanSystem_DEMO`. La evidencia detallada no sensible se conserva en `SRI-WORKER-DEPLOYMENT.md`; no debe repetirse la llamada real sin una nueva autorizacion expresa.

La Fase 5.5 desplego `118` en los tres tenants autorizados y `119` en Master, comprobo idempotencia SQL, permisos reales, aislamiento, descarga binaria, integridad, auditoria, estado inmutable y Designer. La evidencia detallada vive en [`../operations/SRI-DOCUMENT-MONITOR.md`](../operations/SRI-DOCUMENT-MONITOR.md).

Pendientes fuera del cierre de Fase 5.5:

1. Definir la retencion antes de introducir cualquier eliminacion.
2. Ampliar, si se aprueba, el catalogo inicial: `SourceType` (`NuanSystem`, `Txt`, `SapAddOn`, `Manual`, `ExternalApi`) y comprobantes `01`, `04`, `07`.

Estos pendientes no invalidan el E2E acotado de Fase 5.4 ni la experiencia operativa de consulta/descarga validada en Fase 5.5; permanecen fuera del alcance aprobado.

## Contrato implementado en Fase 5.5

- lista y resumen exigen `SRI.DOCUMENTS.VIEW` y usan proyecciones sin clave de acceso;
- detalle/auditoria exigen `SRI.DOCUMENTS.VIEW_PAYLOAD`; descarga exige `SRI.DOCUMENTS.DOWNLOAD_XML`;
- inexistencia responde `404`; estado y contenido faltante usan errores funcionales estables;
- cada exito entrega bytes `application/xml`, nombre seguro y `no-store`, y registra `DownloadXml` sin contenido ni clave;
- repetir la descarga conserva un solo documento, agrega otra auditoria y mantiene `Authorized`;
- SQL se resuelve mediante el tenant autenticado y WinForms usa exclusivamente el cliente tipado.

La ejecucion controlada de Fase 5.5 acredito tambien scripts instalados idempotentemente, permisos reales con JWT renovado, aislamiento tenant, descarga API, integridad, auditoria e inspeccion visual. La evidencia saneada se conserva en [`../operations/SRI-DOCUMENT-MONITOR.md`](../operations/SRI-DOCUMENT-MONITOR.md).
