# Blueprint — Importación TXT de documentos electrónicos SRI

## Estado y alcance

**Estado:** discovery y contrato del núcleo TXT aprobados por el propietario el 2026-07-27. La implementación autorizada se limita a `TXT -> SriDocumentQueue`; WinForms y SAP permanecen fuera de alcance.

Este documento define un módulo tenant para registrar cargas TXT, validar sus filas y vincular las filas válidas con la cola durable SRI existente. También presenta alternativas para una integración posterior con SAP Business One sobre HANA.

### Estado de implementación del núcleo

Implementado en la rama `refactor/codex-skills-v9-sri-txt-import`:

- contratos, handlers y validadores Application;
- parser streaming UTF-8 estricto/Windows-1252;
- script tenant `138` y repositorio Dapper;
- estado `Staged` y transición explícita `Staged -> Pending`;
- script Master `139` sin grants automáticos;
- API multipart y permisos independientes de upload/enqueue;
- redacción de claves y estados legibles en DTO;
- pruebas automatizadas y contractuales.

Los scripts `138_tenant_sri_txt_import.sql` y `139_master_sri_txt_import_security.sql` y el runtime del núcleo
`TXT -> Staged -> Pending` fueron validados bajo autorización separada en Master y los tres tenants piloto.
Esa evidencia no se repite en la fase CRUD.

Autorizado el 2026-07-27: cerrar la consulta paginada de importaciones y filas, el detalle saneado,
la navegación hacia `SriDocumentQueue` y un formulario WinForms independiente. Las nuevas migraciones
propuestas son `140_tenant_sri_txt_import_crud.sql` y
`141_master_sri_txt_import_crud_security.sql`; se generan, pero no se ejecutan sin una autorización
posterior. SAP, llamadas SRI y ejecución del worker continúan excluidos.

## 1. Discovery Record

| Aspecto | Resultado |
| --- | --- |
| Clasificación | Administración/monitor de cargas + proceso operativo de validación y encolado + integración externa opcional |
| Dominio propietario | SRI |
| Persistencia operacional | Base tenant seleccionada por el contexto autenticado de empresa |
| Captura | WinForms consume únicamente la API REST |
| Autorización SRI | Cola y `NuanSystem.SriWorker` existentes |
| SAP | Integración opcional por empresa; no condiciona la operación SRI |
| SAP HANA | No existe un contrato aprobado para la UDT/UDO ni un escritor SRI |
| Transporte WinForms | `INuanApiClient`/`NuanApiClient`; no se encontró soporte multipart reutilizable |
| Patrón UI más cercano | `SriDocumentMonitorForm`, `NuanDataGridControl`, KPI corporativos, historial corporativo |
| Patrón SQL | Scripts versionados e idempotentes, procedimientos almacenados, Dapper, `rowversion` |
| Restricciones de seguridad | No exponer claves completas, XML, credenciales, SQL ni datos de otro tenant |

### Límites de propiedad

```text
WinForms
  -> API/Application: carga, validación, comandos y consultas autorizadas
  -> Persistence/base tenant: cabecera, filas, auditoría y vínculo con cola SRI
  -> SriDocumentQueue existente: idempotencia documental
  -> NuanSystem.SriWorker existente: única llamada al SRI y almacenamiento XML
  -> Monitor SRI existente: consulta del documento/resultado autorizado

Salida SAP opcional
  -> outbox durable tenant
  -> procesador de integración SAP fuera de SriWorker
  -> Service Layer hacia UDT/UDO aprobado
```

No se crea una segunda cola SRI, otro worker SRI, otro almacén XML, otro cliente SRI, otro mecanismo de retry SRI ni un monitor documental sustituto.

### Evidencia revisada

- Constitución, kernel, catálogos, grafo y checklist de ingeniería.
- Arquitectura general, comercial, multiempresa, SAP, Matriz/Sucursal y SRI.
- Blueprints y evidencia operacional de las iteraciones SRI 5, 6 y 7.
- Capas Application, Persistence, API, Worker y WinForms del recorrido SRI.
- Scripts SRI `115`, `117`, `118`, `119`, `120`, `121`, `122` y posteriores.
- Contratos SAP/HANA y estado del outbox SAP.
- Tres archivos TXT proporcionados, inspeccionados sin modificarlos.

El discovery inicial observó hasta `135_tenant_warehouse_tombstone_code_reservation.sql`. Después de integrar Currency 8.5 en `master`, el segundo escaneo confirmó `136_tenant_currency_transactional_outbox.sql` y `137_master_currency_transactional_registration.sql`. Para esta implementación se proponen:

- `138_tenant_sri_txt_import.sql`;
- `139_master_sri_txt_import_security.sql`.

La numeración debe verificarse una vez más inmediatamente antes de crear cada script. Este contrato no autoriza su ejecución.

## 2. Análisis estructural saneado de los TXT

### Estructura común

| Propiedad | Resultado |
| --- | --- |
| Codificación | UTF-8 estricto falla. Las muestras se aceptan como Windows-1252. ISO-8859-1 queda expresamente rechazado por política |
| BOM | No |
| Separador | Tabulación |
| Cabecera | Sí |
| Columnas | 12 exactas en todas las filas |
| Filas de datos | 11.072 |
| Filas vacías internas | 0 |
| Duplicados exactos | 0 |
| Claves repetidas globales | 0 |
| Claves | 49 dígitos, módulo 11 válido en las 11.072 filas |
| Ambiente inferido | `2`, Producción, en todas las claves |
| Información adicional | Sí: emisor, tipo, serie, fechas, receptor, importes y documento modificado |
| Información sensible | Sí |

Cabecera observada:

1. `RUC_EMISOR`: identificación tributaria del emisor.
2. `RAZON_SOCIAL_EMISOR`: nombre o razón social.
3. `TIPO_COMPROBANTE`: código y etiqueta del documento.
4. `SERIE_COMPROBANTE`: establecimiento, punto de emisión y secuencial.
5. `CLAVE_ACCESO`: clave SRI.
6. `FECHA_AUTORIZACION`: fecha y hora de autorización.
7. `FECHA_EMISION`: fecha de emisión.
8. `IDENTIFICACION_RECEPTOR`: identificación del receptor.
9. `VALOR_SIN_IMPUESTOS`: base imponible/valor sin impuestos.
10. `IVA`: impuesto.
11. `IMPORTE_TOTAL`: total cuando aplica.
12. `NUMERO_DOCUMENTO_MODIFICADO`: referencia documental cuando aplica.

### Resultado por archivo

| Archivo saneado | SHA-256 | Filas | Tipo | Rango de emisión | Emisores |
| --- | --- | ---: | --- | --- | ---: |
| `0190386252001_Recibidos.txt` | `446221…f6228` | 7.028 | `01` Factura | 2026-07-01 a 2026-07-26 | 505 |
| `0190386252001_Recibidos (1).txt` | `28e52f…904c` | 2.722 | `04` Nota de crédito | 2026-07-01 a 2026-07-25 | 210 |
| `0190386252001_Recibidos (2).txt` | `cc4a89…4458d` | 1.322 | `07` Comprobante de retención | 2026-07-01 a 2026-07-26 | 280 |

Validaciones cruzadas superadas en todas las filas:

- fecha de emisión, tipo, RUC emisor y serie coinciden con los segmentos de la clave;
- receptor coincide con el identificador que da nombre a las muestras;
- los formatos de RUC, receptor, serie y fechas son consistentes;
- no faltan los ocho campos nucleares;
- no existen filas incompletas bajo las reglas observadas por tipo.

Los vacíos de importes no son errores por sí mismos:

- factura: documento modificado vacío;
- nota de crédito: total vacío y documento modificado informado;
- retención: base, IVA y total vacíos; documento modificado informado.

Los importes usan punto decimal; existen valores como `.25`, que deben normalizarse a `0.25`. No se observó coma decimal.

### Datos que deben protegerse

Son sensibles la clave completa, RUC/identificación, razón social, serie/secuencial, importes, fechas de autorización y referencias modificadas. La evidencia ordinaria debe usar:

- `SHA-256` para identidad técnica;
- máscara con sufijo corto para soporte;
- mensajes de validación sin eco de la línea original;
- permisos específicos para detalle y errores.

No debe persistirse una segunda copia legible de la clave en el detalle si la cola ya necesita conservarla para consultar al SRI. La propuesta guarda `AccessKeyHash`, máscara y vínculo a `SriDocumentQueue`; la clave completa existe de forma transitoria durante validación/encolado.

## 3. Inventario reutilizable

| Componente existente | Reutilización |
| --- | --- |
| `SriDocumentCodes` | Tipos `01`, `04`, `07`, ambientes, formato y módulo 11 |
| Política/validator de enqueue | Capacidad `SRI_DOCUMENTS`, integración `SRI`, ambiente y reglas básicas |
| `SriDocumentQueueRepository` | Cola tenant y vínculo idempotente por ambiente/clave |
| `SriDocumentQueue` | Única cola de autorización |
| Intentos, leases y retry/dead letter | Sin cambios de propiedad |
| `NuanSystem.SriWorker` | Único actor que consulta el servicio oficial |
| `SriAuthorizedDocuments` | Único almacén protegido de XML autorizado |
| Endpoints SRI actuales | Apertura del documento relacionado, detalle, intentos, auditoría y XML |
| `SriDocumentMonitorForm` | Monitor documental; se abre desde una fila vinculada |
| `NuanDataGridControl`, KPI y acciones corporativas | Pantalla de cargas |
| `RecordHistoryForm` | Historial de una carga |
| `NuanApiClient` | Autenticación, empresa, serialización y errores |
| Conexión HANA protegida y clientes SAP | Base técnica, no contrato de escritura SRI |

Brechas confirmadas:

- no hay transporte multipart en el cliente común;
- el outbox SAP general y su worker están marcados como no implementados;
- no hay UDT/UDO SRI aprobada;
- no hay escritor soportado hacia una UDT/UDO SRI;
- no existe modelo tenant de cargas TXT.

## 4. Clasificación funcional

La pantalla no es un CRUD estándar aunque administre cabeceras. Es un **monitor operacional de cargas** porque valida documentos, crea vínculos idempotentes, dispara encolado y expone recuperación.

Se separan dos responsabilidades:

1. **Administración de cargas:** registrar archivo, consultar estado, detalle, errores, historial y eliminación lógica restringida.
2. **Proceso operativo:** validar en servidor, persistir resultados, vincular/encolar en la cola SRI y preparar eventos de integración SAP.

El procesamiento SRI sigue siendo responsabilidad exclusiva del worker existente.

## 5. Modelo funcional

### Agregados

#### `SriTxtImport`

Representa una carga por empresa. Conserva identidad global, nombre seguro, hash del contenido, metadatos, conteos autoritativos, estado, auditoría y concurrencia.

#### `SriTxtImportRow`

Representa una línea inmutable del archivo: número de línea, hash de fila/clave, datos normalizados permitidos, resultado de validación, estado de encolado, errores saneados y vínculo opcional a la cola.

Una fila válida crea o vincula durante el upload una única `SriDocumentQueue`:

- si la identidad tributaria no existe, se crea con estado `Staged`;
- si ya existe en cualquier estado, solo se vincula;
- una fila inválida nunca crea cola;
- `SriTxtImportRows` conserva hash, máscara y `QueueId`, no la clave completa.

#### `SriTxtImportAudit`

Registra acciones de negocio de la carga que no pueden depender de una cola todavía inexistente.

#### `SriSapOutbox` — condicionado a aprobación

Registra intención durable de enviar a SAP solamente cuando exista un contrato UDT/UDO aprobado. No forma parte del worker SRI.

### Reglas principales

- El hash del archivo identifica una carga tenant aunque el nombre cambie.
- Un archivo exitoso no crea otra cabecera.
- Un archivo fallido o incompleto se recupera mediante acción explícita sobre la misma identidad.
- Cada fila conserva su número original.
- La API vuelve a validar extensión, tamaño, cabecera, codificación, columnas y contenido.
- El frontend nunca suministra conteos o estados autoritativos.
- Una clave existente vincula la fila con la cola/documento existente.
- `Staged` significa preparado por una carga validada, pero todavía no elegible para consulta.
- Solo la acción explícita Encolar puede ejecutar `Staged -> Pending`.
- Encolar una fila ya vinculada a un estado distinto de `Staged` es idempotente y no reinicia el documento.
- Una discrepancia de identidad produce `Conflict`; no se adopta por coincidencia parcial.
- Eliminar lógicamente una carga no elimina cola, XML, auditoría ni outbox.
- Reprocesar no borra evidencia previa; crea un nuevo intento/auditoría y respeta idempotencia.

## 6. Modelo SQL Server tenant propuesto

Los nombres son candidatos sujetos a revisión final de convenciones.

### `SriTxtImports`

- `Id bigint identity` PK.
- `GlobalId uniqueidentifier` con restricción única.
- `OriginalFileName nvarchar(260)` ya saneado; nunca ruta local.
- `FileSha256 binary(32)` único, sin filtro por borrado lógico.
- `FileSizeBytes bigint`.
- `EncodingCode varchar(20)`, `DelimiterCode varchar(10)`, `HeaderHash binary(32)`.
- `Status varchar(32)`.
- conteos `Total`, `Valid`, `Invalid`, `Duplicate`, `Linked`, `Enqueued`, `Rejected`.
- resumen SAP opcional, derivado de outbox, no aceptado desde UI.
- `StartedAt`, `CompletedAt`.
- `LastErrorCode`, `LastErrorMessage` saneado.
- auditoría estándar de creación/modificación y `IsDeleted` con actor/fecha/motivo.
- `RowVersion rowversion`.

La unicidad de `FileSha256` no se filtra por `IsDeleted`: la eliminación lógica no habilita una importación accidentalmente duplicada.

### `SriTxtImportRows`

- `Id bigint identity` PK.
- `ImportId bigint` FK a cabecera.
- `RowGlobalId uniqueidentifier` único.
- `LineNumber int`.
- `RowSha256 binary(32)`.
- `AccessKeySha256 binary(32)`.
- `MaskedAccessKey varchar(20)`.
- `Environment varchar(16)`, `DocumentType char(2)`.
- campos tributarios normalizados según política de minimización.
- fechas `date`/`datetime2`.
- importes `decimal(19,6)` nullable.
- `ValidationStatus`, `EnqueueStatus`.
- `ValidationCode`, `ValidationMessage` saneado.
- `QueueId bigint null` FK a `SriDocumentQueue`.
- auditoría de creación/modificación y `RowVersion`.

Índices:

- único `(ImportId, LineNumber)`;
- índice `(ImportId, ValidationStatus, EnqueueStatus)`;
- índice `(AccessKeySha256)`;
- índice `(QueueId)` donde no sea nulo;
- único `RowGlobalId`.

No se propone unicidad global de `AccessKeySha256` en detalle: un archivo diferente puede contener una clave ya registrada y debe quedar trazado, vinculado y clasificado.

### Auditoría

`AuditSriTxtImportChanges` mantiene eventos inmutables: `UploadRegistered`, `ValidationStarted`, `ValidationCompleted`, `EnqueueStarted`, `RowLinked`, `RowEnqueued`, `ReprocessRequested`, `SoftDeleted`, `SapScheduled` y cambios terminales. Conserva actor confiable, fecha UTC, IP/correlación cuando el patrón lo permita, código de evento y datos saneados.

Se reutiliza `RecordHistoryForm` como superficie, no se crea infraestructura visual paralela. La tabla específica es necesaria porque la auditoría SRI actual exige `QueueId` y una carga puede existir antes de cualquier cola.

### Procedimientos y transacciones

Contratos candidatos:

- lista paginada y filtros;
- detalle de cabecera;
- detalle paginado de filas;
- registrar carga por hash con bloqueo seguro;
- insertar filas mediante TVP;
- finalizar validación y conteos;
- vincular/encolar filas válidas en lote;
- reprocesar con `rowversion` y motivo;
- eliminación lógica restringida;
- historial;
- resumen/estado SAP.

El registro concurrente usa índice único y tratamiento explícito de violación de unicidad. La operación de vincular/encolar se ejecuta en una transacción tenant, con locks acotados y la restricción existente `(Environment, AccessKey)` como autoridad. No se hace una transacción distribuida con SRI o SAP.

Para 11.000+ filas se propone TVP y procedimiento por lote, no un endpoint ni SP por fila. La transacción debe insertar/vincular filas, actualizar conteos y auditoría de forma atómica; llamadas remotas quedan fuera.

### Retención

- Cabecera, detalle y auditoría tienen retención indefinida provisional hasta que se apruebe una política legal definitiva.
- Cola y XML conservan su política vigente.
- No se borra evidencia por eliminación lógica.
- El archivo binario original no se almacena. Se conservan hash, cabecera, filas normalizadas y auditoría.

## 7. Alternativas SAP Business One/HANA

| Alternativa | Ventajas | Riesgos/límites | Evaluación |
| --- | --- | --- | --- |
| UDT de SAP B1 | Soportada para datos simples, visible al ecosistema SAP | Requiere definición y creación por mecanismos SAP; límites de campos | Recomendada si solo se requiere registro |
| UDO sobre UDT | Comportamiento, validación y UI empresarial SAP | Mayor complejidad y gobierno SAP | Solo si SAP necesita ciclo de vida propio |
| Tabla técnica HANA independiente | Flexibilidad técnica | Puede quedar fuera del soporte/backup/gobierno B1 | Solo con aprobación arquitectónica explícita |
| Service Layer | Transporte soportado, desacoplado de HANA directo | Requiere objeto SAP publicado y manejo de sesión | Transporte recomendado |
| DI API | Amplia compatibilidad histórica | Dependencia Windows/COM y operación más compleja | Alternativa si Service Layer no soporta el contrato |
| Escritura directa a tablas estándar | Ninguna admisible | Riesgo de integridad y soporte | Prohibida |

### Dato y momento recomendados

SAP no necesita la carga cruda ni cada clave antes de autorización. La recomendación es publicar, después de `Authorized`, un registro mínimo:

- `ImportRowGlobalId` como reconciliador global;
- identificador de cola/documento;
- ambiente y hash de clave;
- tipo, fechas y metadatos tributarios aprobados;
- estado SRI y fecha de autorización;
- hash/referencia del XML, no el XML completo, salvo requisito aprobado;
- `PayloadSha256` y versión del contrato.

Esto evita convertir SAP en una dependencia del ingreso TXT o de la autorización SRI y evita duplicar XML sin necesidad.

### Alternativa recomendada

1. Aprobar una UDT SAP B1 para el registro mínimo; usar UDO únicamente si SAP requiere comportamiento empresarial.
2. Crear una intención durable tenant `SriSapOutbox` cuando la cola alcance `Authorized`.
3. Procesarla desde el ámbito de sincronización SAP, mediante Service Layer y un adaptador tipado.
4. No alojar este proceso en `NuanSystem.SriWorker`.
5. No afirmar que se reutiliza el outbox SAP actual: su runner está `NotImplemented`. El propietario debe aprobar entre:
   - completar primero el framework genérico de outbox SAP; o
   - implementar un vertical acotado SRI dentro del worker de sincronización SAP.

El segundo camino reduce alcance inicial, pero crea un contrato especializado. El primero mejora reutilización transversal, pero amplía considerablemente el proyecto. Esta decisión bloquea la implementación SAP, no la carga TXT ni el encolado SRI.

### Idempotencia y conflicto SAP

- `ExternalId = RowGlobalId` con formato estable.
- `PayloadSha256` representa el contenido publicado.
- mismo `ExternalId` + mismo hash: `Unchanged`;
- mismo `ExternalId` + hash distinto: `Conflict` terminal;
- no se adopta un registro SAP por RUC, clave parcial, serie o coincidencia de campos;
- estados: `Pending`, `InProcess`, `Applied`, `Unchanged`, `Retry`, `Conflict`, `Failed`/`DeadLetter`;
- leases, límite de intentos y backoff;
- ninguna credencial, cookie o JWT en outbox o auditoría.

## 8. Flujo de estados

### Carga

```text
Uploading
  -> Uploaded
  -> Validating
       -> Validated
       -> ValidatedWithErrors
       -> Failed
  -> Enqueueing
       -> Completed
       -> CompletedWithErrors
       -> Failed
  -> SoftDeleted (solo si las reglas lo permiten)
```

Una carga `Failed` o incompleta se retoma mediante `ReprocessRequested`, con motivo, permiso y concurrencia; no se crea una identidad nueva para el mismo hash.

### Fila

```text
Parsed
  -> Valid / Invalid / DuplicateInFile / Conflict
Valid
  -> Staged                 (cola nueva, aún no reclamable)
  -> LinkedExisting         (cola preexistente)
  -> LinkedAuthorized       (cola/documento autorizado)
Staged
  -> Enqueued               (transición atómica de cola Staged -> Pending)
  -> Staged                 (segunda acción idempotente)
LinkedExisting/Authorized
  -> sin reinicio ni cambio de estado de cola
```

La cola SRI conserva sus estados vigentes. El importador no redefine ni duplica ese ciclo.

La cola amplía su contrato con `Staged`:

```text
Staged -> Pending           (única transición aprobada en esta fase)
Pending -> Querying | Cancelled
RetryScheduled -> Querying | Cancelled
```

`NuanSystem.SriWorker` continúa reclamando exclusivamente `Pending` y `RetryScheduled`. No se aprueba cancelación, edición o transición arbitraria desde `Staged`.

### SAP condicionado

```text
Pending -> InProcess -> Applied
                    -> Unchanged
                    -> Retry -> InProcess
                    -> Conflict
                    -> Failed/DeadLetter
```

## 9. Árboles de decisión

### Archivo duplicado

```text
¿FileSha256 existe en el tenant?
  No -> crear cabecera y validar.
  Sí -> ¿la carga terminó con éxito?
          Sí -> no crear otra carga; devolver identidad y estado existentes.
          No -> exigir Reprocess con permiso, motivo y RowVersion.

¿Archivo diferente contiene claves conocidas?
  Sí -> registrar la nueva carga y clasificar cada fila:
          vínculo existente / autorizado / conflicto.
```

### Fila válida

```text
¿Formato, módulo 11, ambiente, tipo y coherencia interna son válidos?
  No -> Invalid con código saneado.
  Sí -> ¿misma clave se repite dentro del archivo?
          Sí -> primera fila candidata; posteriores DuplicateInFile.
          No -> buscar por Environment + AccessKey en cola.
                  No existe -> crear cola Staged y vincular.
                  Existe -> ¿identidad coherente?
                              Sí, Staged -> vincular y esperar acción Encolar.
                              Sí, no autorizada -> LinkedExisting sin reiniciar.
                              Sí, autorizada -> LinkedAuthorized.
                              No -> Conflict sin adopción automática.
```

### Integración SAP

```text
¿Integración SAP está aprobada/habilitada para la empresa?
  No -> SRI termina sin SAP.
  Sí -> crear/obtener outbox por ExternalId.
        ¿SAP disponible?
          No -> Retry.
          Sí -> ¿ExternalId no existe?
                  Sí -> aplicar -> Applied.
                  No -> ¿PayloadSha256 coincide?
                          Sí -> Unchanged.
                          No -> Conflict terminal.
        ¿Error funcional no recuperable?
          Sí -> Conflict/Failed terminal con diagnóstico saneado.
```

## 10. Diseño API

Grupo candidato: `/api/sri/txt-imports`.

| Método/ruta | Propósito |
| --- | --- |
| `GET /` | Lista paginada con fecha, estado, nombre de archivo y ambiente; incluye totales filtrados |
| `GET /{id}` | Cabecera y conteos saneados |
| `GET /{id}/rows` | Filas paginadas por validez, con máscara y vínculo opcional a cola |
| `POST /upload` | Multipart; registra y analiza archivo |
| `POST /{id}/validate` | Revalidación explícita cuando el estado lo permita |
| `POST /{id}/enqueue` | Cambia atómicamente colas propias `Staged -> Pending`; ignora de forma idempotente colas ya avanzadas |
| `POST /{id}/reprocess` | Recuperación autorizada con motivo y `RowVersion` |
| `DELETE /{id}` | Eliminación lógica restringida |
| `GET /{id}/history` | Historial corporativo |
| `GET /{id}/sap-status` | Resumen de integración SAP |
| `GET /{id}/rows/{rowId}/sri-document` | Identidad/ruta autorizada para abrir Monitor SRI |

La subida requiere multipart en API. El CRUD WinForms autorizado no incorpora carga en esta fase y,
por tanto, no amplía `INuanApiClient`: consume únicamente consultas JSON y el enqueue ya existente.
JSON Base64 se descarta por costo de memoria y expansión del contenido.

Las proyecciones públicas nunca incluyen `AccessKey`, `HeaderLine`, la línea TXT original, XML,
conexiones, JWT ni secretos. Una fila vinculada expone únicamente `QueueId`, estado visible,
hash y máscara; `QueueId` permite abrir el monitor documental, cuya autorización sigue siendo
independiente.

### Límites iniciales propuestos

| Control | Propuesta |
| --- | --- |
| Tamaño | 10 MiB por archivo |
| Filas | 50.000 filas de datos |
| Extensión | `.txt` |
| Codificación | UTF-8 estricto y Windows-1252; ISO-8859-1 rechazado |
| Cabecera | 12 nombres exactos, orden versionado |
| Separador | TAB |
| Longitud de línea | 8 KiB |
| Tiempo | 60 s por validación síncrona inicial |
| Nombre | solo `Path.GetFileName`; caracteres/longitud saneados |
| Procesamiento | streaming, límites antes de materializar, cancelación |

La API rechaza archivos comprimidos, binarios, NUL, líneas excesivas, columna extra/faltante, fechas/importes fuera de rango y contenido posterior al límite. Los errores usan códigos estables y no reproducen la línea.

Si el piloto demuestra que 50.000 filas no caben de forma segura en el límite, debe diseñarse procesamiento durable separado; no se debe aumentar el timeout indefinidamente ni usar el worker SRI para parsear TXT.

## 11. Diseño WinForms

### Identidad y lifecycle

- FormKey candidato: `sri-txt-imports`.
- Formulario dedicado de monitor operacional, no `BaseGridCrudListForm`.
- Reutiliza el módulo/menú SRI existente.
- `SriDocumentMonitorForm` sigue siendo el monitor documental; la nueva pantalla solo lo abre con el vínculo autorizado.

### Superficie

- filtros: rango de fecha, estado, archivo y ambiente;
- acciones de esta fase: refrescar, limpiar filtros, encolar y abrir la cola vinculada;
- KPI: filas totales, válidas, inválidas, vinculadas, `Staged` y `Pending`;
- grid principal de cargas con `NuanDataGridControl`;
- panel/pestaña de resumen y grid de filas paginado;
- errores saneados y máscara de clave;
- indicador SAP con texto y estado, no solo color;
- acción por fila para abrir el documento relacionado en el Monitor SRI;
- detalle de cabecera sin la línea TXT original.

Se usan `NuanActionButton`, `BrandResources`, `AppTypography`, `FormStyler` y controles corporativos. Los catálogos cerrados de estado pueden usar el editor corporativo aplicable; `NuanLookupEdit` se usa solamente si el contrato de datos lo justifica.

El layout se declara en `.Designer.cs`, con regiones explícitas, Segoe UI, alineación numérica, `MinimumSize`, anchors/dock deliberados y estados loading, empty, filtered-empty, error, busy, degraded y read-only. No se construye la jerarquía principal en runtime.

El formulario/ViewModel conserva estado de UI y evita doble envío; el cliente tipado conoce rutas; el backend decide permisos, empresa, conteos, validación, transacciones, SRI y SAP.

## 12. Permisos

Permisos aprobados, sin concesión automática a ningún rol existente:

- `SRI.TXT_IMPORTS.VIEW`
- `SRI.TXT_IMPORTS.UPLOAD`
- `SRI.TXT_IMPORTS.VALIDATE`
- `SRI.TXT_IMPORTS.ENQUEUE`
- `SRI.TXT_IMPORTS.REPROCESS`
- `SRI.TXT_IMPORTS.DELETE`
- `SRI.TXT_IMPORTS.VIEW_ERRORS`
- `SRI.TXT_IMPORTS.VIEW_HISTORY`
- `SRI.TXT_IMPORTS.VIEW_SAP_STATUS`

Abrir/descargar un documento SRI conserva los permisos existentes del monitor/XML. El script `139`
ya sembró los códigos en `Permissions` sin concesiones. El script `141` registra el formulario,
menú y operaciones de esta fase, también sin insertar concesiones en `RolePermissions`,
`SecurityRoleMenus` ni `SecurityRoleFormOperations`. Consulta, carga y enqueue conservan permisos
independientes; una concesión posterior requiere aprobación separada y un JWT renovado.

## 13. Idempotencia, auditoría y recuperación

### Idempotencia

- archivo: `FileSha256` único por base tenant;
- fila: `(ImportId, LineNumber)` y `RowSha256`;
- documento: restricción vigente de cola `(Environment, AccessKey)`;
- preparación: upload crea o encuentra la cola bajo la misma restricción y nunca crea cola para filas inválidas;
- enqueue: actualización condicionada `Status = 'Staged'`, auditada; repetirla no crea cola, intento ni evento;
- SAP: `ExternalId` + `PayloadSha256`;
- acciones: `RowVersion`, estados legales y auditoría de cada intento.

### Auditoría

- actor/empresa provienen de claims/contexto confiable;
- correlación de petición y motivo obligatorio en reprocess/delete;
- no se registran claves completas, XML, líneas crudas, secretos ni respuestas SAP sensibles;
- los conteos se calculan en servidor;
- descargas XML siguen la auditoría actual;
- historial presenta eventos de carga y enlaces con eventos documentales.

### Recuperación

- fallo antes de persistir cabecera: no queda carga;
- fallo durante inserción de detalle: rollback de la transacción tenant;
- carga incompleta: acción explícita sobre la misma cabecera/hash;
- fallo al encolar: rollback del lote o estado recuperable claramente delimitado;
- fallo del upload: no deja importación, detalle o colas `Staged` parcialmente confirmadas;
- SRI no disponible: retry/dead letter existentes;
- SAP no disponible: outbox conserva `Retry`, sin revertir SRI;
- lease vencido SAP: recuperable por el procesador aprobado;
- conflicto: terminal y visible, sin adopción automática;
- borrado lógico: reversible solo si se aprueba, sin borrar evidencias protegidas.

## 14. Archivos estimados

Los nombres son orientativos.

### Documentación/catálogos

- `docs/architecture/SRI-TXT-IMPORT-BLUEPRINT.md`
- `docs/operations/SRI-TXT-IMPORT-VALIDATION-PLAN.md`
- actualizaciones futuras de catálogos/grafo si se extiende `INuanApiClient`.

### Backend

- feature Application `Sri/TxtImports`: comandos, queries, DTOs, validators y contratos.
- repositorio Dapper tenant y mapeos.
- endpoints `SriTxtImportEndpoints`.
- extensión multipart del transporte compartido, solo tras consumer review.
- pruebas unitarias, contractuales, persistencia, concurrencia, autorización y tenant.

### SQL

- un script tenant para tablas, TVP, procedimientos, índices y auditoría.
- un script Master para permisos/formulario/menú.
- scripts posteriores de reparación únicamente si una versión desplegada lo requiere.
- SQL/outbox SAP solo después de la decisión SAP.

### WinForms

- modelos y cliente tipado SRI TXT.
- ViewModel.
- formulario y `.Designer.cs`.
- registro DI/factory/navegación.
- pruebas de ViewModel/cliente cuando el patrón lo permita.

### SAP condicionado

- contrato Application de publicación.
- repositorio outbox tenant.
- adaptador Service Layer UDT/UDO.
- procesador en el ámbito de sincronización SAP.
- pruebas de idempotencia, retry, conflicto y tenant.

## 15. Riesgos

| Riesgo | Mitigación |
| --- | --- |
| Confundir Windows-1252 e ISO-8859-1 | Aceptar solo UTF-8 estricto y Windows-1252; rechazar ISO-8859-1 |
| Exponer claves/RUC en UI o logs | hash, máscara, permisos y errores saneados |
| Duplicar cola/worker/XML | vínculo obligatorio a componentes existentes |
| Agotar memoria con archivos grandes | streaming y límites tempranos |
| Carrera de dos cargas iguales | índice único + manejo de conflicto |
| Transacción demasiado larga | TVP/lotes acotados y medición real |
| Estado parcial entre importación y cola | transacción tenant y estados recuperables |
| Acoplar SRI a SAP | outbox posterior, integración opcional |
| Usar outbox SAP supuesto | reconocer `NotImplemented` y aprobar ownership |
| Escribir directo en SAP estándar | prohibición y Service Layer/DI API |
| Duplicar XML en SAP | publicar metadatos/hash salvo requisito |
| Permisos solo visuales | autorización backend y pruebas 401/403/200 |
| Número de migración obsoleto | reescaneo justo antes de implementar |

## 16. Decisiones aprobadas por el propietario

Decisiones aprobadas el 2026-07-27:

1. Aceptar UTF-8 estricto y Windows-1252. Rechazar ISO-8859-1.
2. Aplicar límites de 10 MiB, 50.000 filas de datos, 8 KiB por línea y 60 segundos.
3. No conservar el archivo binario. Persistir en la base tenant el hash, la cabecera, las filas normalizadas y la auditoría.
4. Persistir los 12 campos tributarios normalizados. `SriTxtImportRows` conserva hash, máscara y `QueueId`, pero no una segunda copia de la clave completa. `SriDocumentQueue` mantiene la clave completa bajo su contrato vigente.
5. Usar eliminación lógica sin restauración en esta fase. Nunca eliminar `SriDocumentQueue`, XML o auditoría relacionada.
6. `Upload` registra y valida en una sola operación. El encolado es una acción explícita posterior.
7. Crear los permisos propuestos sin concederlos automáticamente a roles existentes.
8. Una integración SAP futura recibe únicamente documentos que alcancen `Authorized`.
9. La integración futura usa una UDT de SAP Business One mediante Service Layer. Se prohíbe escribir directamente en tablas estándar o técnicas de HANA.
10. SAP queda aplazado. La fase actual implementa y valida únicamente `TXT -> SriDocumentQueue`.
11. La UDT futura recibe metadatos mínimos y hash/referencia del XML, nunca el XML completo.
12. Cabecera, detalle y auditoría mantienen retención indefinida provisional hasta aprobar una política legal definitiva.
13. Incorporar `Staged` en `SriDocumentQueue`. Upload crea o vincula filas válidas como `Staged`; la acción Encolar es la única transición aprobada `Staged -> Pending`.
14. El worker conserva el claim exclusivo sobre `Pending` y `RetryScheduled`; `Staged` nunca es reclamable.
15. Cualquier cola preexistente se vincula sin reiniciar estado. Una segunda acción Encolar es idempotente.
16. Los contratos API distinguen `Staged` como “Preparado” y `Pending` como “Pendiente de consulta”, sin exponer la clave.
17. El CRUD propio de importaciones expone listado, detalle y filas mediante paginación de servidor,
    con filtros por fecha, estado, nombre de archivo y ambiente.
18. Los totales filtrados distinguen filas totales, válidas, inválidas, vinculadas, `Staged` y `Pending`.
19. Ninguna respuesta pública incluye clave completa, cabecera/línea TXT original, XML, JWT,
    conexiones o secretos. La navegación al monitor se hace solo mediante `QueueId`.
20. El formulario WinForms es un monitor operacional independiente, usa controles corporativos y
    Designer, y no implementa SAP ni acceso directo a SQL/SRI.

### Alcance autorizado de implementación

- contratos, casos de uso y validadores Application;
- parser streaming UTF-8 estricto/Windows-1252;
- persistencia tenant idempotente mediante procedimientos almacenados;
- upload multipart que registra y valida;
- acción explícita de vinculación/encolado con `SriDocumentQueue`;
- ampliación forward-only del constraint de estados para agregar `Staged`, sin modificar filas existentes;
- permisos Master sin grants automáticos;
- pruebas automáticas.
- consultas CRUD paginadas y saneadas, formulario WinForms y navegación autorizada a la cola.

Quedan excluidos SAP, ejecución SQL, runtime API/WinForms, workers y cualquier llamada externa.
Los scripts `140`/`141` o el runtime CRUD requieren autorizaciones independientes.

## 17. Plan de implementación en commits pequeños

1. `docs(sri): aprobar contrato TXT y decisiones de seguridad`
2. `feat(sri): agregar modelo y validadores Application de cargas`
3. `feat(sql): agregar persistencia tenant SRI TXT idempotente`
4. `feat(sri): implementar parser streaming, repositorio Dapper y transacción de importación`
5. `feat(api): agregar endpoint multipart de carga`
6. `feat(sri): activar Staged de forma explícita e idempotente`
7. `feat(security): agregar permisos y navegación SRI TXT`
8. `feat(winforms): agregar cliente, ViewModel y monitor de cargas`
9. `test(sri): cubrir parsing, concurrencia, tenant, permisos y rollback`
10. `docs(sri): registrar evidencia de validación`
11. `feat(sap): agregar contrato UDT y outbox` — fase futura, no autorizada
12. `feat(sap): agregar procesador Service Layer, retry y conflictos` — fase futura
13. `test(sap): validar idempotencia, indisponibilidad y colisión` — fase futura

Cada commit debe compilar y probar su alcance, no modificar scripts históricos desplegados y permanecer separado de Matriz/Sucursal, Warehouse y Currency.

## 18. Criterio de salida del blueprint

El núcleo `TXT -> Staged -> Pending` está implementado y fue validado bajo su autorización previa.
El CRUD de consulta y su formulario quedan autorizados para implementación, pero no para ejecutar
sus nuevas migraciones ni su runtime. SAP permanece diferido. La ausencia del contrato técnico
detallado de la UDT no autoriza una integración SAP improvisada.
