# Contrato del piloto SRI — consulta y descarga de comprobantes autorizados

## Decision Record

- **Estado de la decision:** Aprobada por el propietario del proyecto.
- **Estado de implementacion:** Fase 5.2 desplegada y validada en Master y tres bases tenant; proveedor, worker, XML y E2E SRI pendientes.
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

Los limites concretos de intentos, tiempos y ventana funcional se configuran y se aprueban antes de habilitar el worker.

## Almacenamiento XML

Para no fijar prematuramente infraestructura, el contrato exige `ISriDocumentStorage` con referencia inmutable e integridad. La eleccion entre tenant DB y storage externo queda pendiente antes de Fase 5.4. Ningun consumidor conoce el proveedor fisico.

No se implementa purga automatica hasta aprobar retencion legal/operativa. La ausencia de una politica de purga no autoriza borrado manual ni almacenamiento sin limites de tamano.

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

## Pendientes para Fases 5.3 y 5.4

1. Elegir almacenamiento fisico del XML.
2. Aprobar retencion, tamano maximo y politica de purga.
3. Aprobar proveedor/endpoints y ambiente oficial de validacion.
4. Aprobar limites de retry, ventana funcional y expiracion de lease.
5. Ampliar, si se aprueba, el catalogo inicial ya cerrado para Fase 5.2: `SourceType` (`NuanSystem`, `Txt`, `SapAddOn`, `Manual`, `ExternalApi`) y comprobantes `01`, `04`, `07`.

Estos pendientes bloquean proveedor, worker y almacenamiento XML, pero no la cola durable de Fase 5.2. La implementacion del proveedor permanece bloqueada hasta cerrarlos.
