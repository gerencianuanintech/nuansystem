# Plan de validación — Importación TXT SRI

## Estado

El núcleo `TXT -> Staged -> Pending` ya fue validado en runtime bajo autorización separada. Este plan
se amplía exclusivamente para el CRUD paginado y el formulario WinForms de importaciones. No se deben
repetir los scripts `138`/`139` ni sus pruebas SQL acreditadas. Las nuevas migraciones `142`/`143`,
la API CRUD y WinForms no pueden ejecutarse sin otra autorización. SAP, worker y llamadas SRI siguen excluidos.
Los scripts `140`/`141` pertenecen exclusivamente a PriceList 8.6.

## Cierre de gates pendientes — 2026-07-28

Este cierre complementa y reemplaza exclusivamente los dos gates fallidos o bloqueados de la
evidencia runtime del 2026-07-27. No repite migraciones, fixtures, transiciones de cola ni llamadas
externas.

- **Validated — tamaño mínimo WinForms:** el commit `532545fe` establece `MinimumSize` en
  `1151 x 700` y conserva el tamaño inicial `1180 x 780`. Una medición funcional del formulario
  confirmó `985/985` px disponibles/requeridos en el ancho mínimo, `1178/985` en tamaño normal y
  `2558/985` maximizado. Las seis tarjetas KPI y los cuatro botones protegidos permanecen visibles.
  No se redujeron fuentes, no se ocultaron controles, no se creó layout en runtime y no se
  modificaron controles compartidos.
- **Validated — Visual Studio Designer:** `SriTxtImportForm.cs [Diseño]` se cerró y volvió a abrir
  correctamente en Visual Studio, sin errores de carga o serialización. La prueba estructural
  dedicada exige una sola fila, `WrapContents=false`, las seis tarjetas en orden y el ancho mínimo
  corporativo.
- **Validated — reconfirmación SQL posterior:** una nueva ejecución de solo lectura abrió
  conexiones con `Encrypt=true` y `TrustServerCertificate=false`, manteniendo conexiones y
  `EncryptionKey` únicamente en memoria. Master conservó una versión `143`, un formulario, un menú,
  una operación ENQUEUE, una OPEN_QUEUE y cero grants automáticos. DEMO conservó una versión `142`,
  0 importaciones, 0 filas, 0 auditorías TXT, 4 colas, 1 intento, 13 auditorías de cola y 1 documento.
  REMIGIO y CANARIS conservaron una versión `142` y todos los conteos SRI TXT/cola en cero.
  `QueueId=10004` permaneció `Authorized`, tipo `01`, con un intento, cinco auditorías, un documento
  de 10.027 bytes, SHA-256 de 32 bytes, tamaño coherente y sin lease.
- **Validated — aislamiento operativo:** API y workers permanecieron detenidos; no se ejecutó SQL,
  no hubo escrituras, no se crearon fixtures y no se llamó al SRI ni a SAP.
- **Validated — regresión final:** `git diff --check` correcto; build completo con 0 errores y
  0 advertencias; 31/31 pruebas SRI TXT; 90/90 pruebas SRI; suite completa con 577 aprobadas,
  5 diagnósticas omitidas y 0 fallidas.

Con estos resultados, los gates pendientes del CRUD SRI TXT quedan cerrados y la rama queda apta
para revisión final de integración. Este estado no autoriza push, PR ni integración a `master`.

## Evidencia runtime CRUD — 2026-07-27

Validación autorizada sobre la rama `refactor/codex-skills-v9-sri-txt-import`, partiendo de
`748324cf682cc225b3bbe21b9389629bc278b05b`:

- **Validated — respaldo:** se reutilizó y verificó con `RESTORE VERIFYONLY WITH CHECKSUM`
  `NuanSystem_Master_SRI_TXT_CRUD_PRE143_20260727_201157.bak`.
- **Validated — migración Master:** `143_master_sri_txt_import_crud_security.sql` se ejecutó dos
  veces exclusivamente en `NuanSystem_Master`. Quedó una versión `20260727.143`, un formulario,
  un menú, las operaciones compartidas `ACTION.REFRESH` y `ACTION.CONSULT`, y una sola instancia
  de `ACTION.SRI_TXT_IMPORTS.ENQUEUE` y `ACTION.SRI_TXT_IMPORTS.OPEN_QUEUE`. No quedaron códigos
  legacy, nombres duplicados ni concesiones automáticas.
- **Validated — seguridad HTTP:** 401 sin autenticación, 403 sin VIEW y 200 con VIEW; VIEW,
  UPLOAD y ENQUEUE se comprobaron por separado y los JWT se renovaron después de asignar permisos.
- **Validated — CRUD API:** listado, detalle, filas válidas/inválidas, filtros combinados, seis
  totales y páginas inicial, intermedia, final, vacía y fuera de rango.
- **Validated — tenant y saneamiento:** DEMO, REMIGIO y CANARIS permanecieron aislados. Respuestas
  y logs temporales no expusieron clave completa, línea TXT, XML, JWT, conexiones ni secretos.
- **Validated — enqueue controlado:** un fixture nuevo realizó una sola transición
  `Staged -> Pending`; la segunda solicitud fue idempotente y no agregó otra auditoría.
- **Validated — limpieza:** usuarios, roles, grants y fixtures temporales quedaron en cero. Los
  conteos finales coincidieron con la línea base: DEMO conservó 0 importaciones, 0 filas, 4 colas,
  1 intento, 1 documento y 13 auditorías de cola; REMIGIO y CANARIS conservaron todos esos conteos
  en cero. `QueueId=10004` continuó `Authorized` y su huella saneada coincidió exactamente.
- **Validated — WinForms parcial:** Visual Studio Designer abrió sin error; el formulario usa
  `AutoScaleMode.Font`, controles corporativos, filtros, paginación, estados vacío/busy/error,
  botones condicionados por permisos y navegación por `QueueId`. A DPI 96, el layout normal
  (`982/1011` px) y maximizado (`982/2560` px) no recortó los KPI.
- **Historical failure — tamaño mínimo WinForms:** al usar el mínimo efectivo, los KPI requieren 982 px dentro
  de 889 px disponibles. El último KPI queda recortado. No se corrigió durante la validación.
- **Historical blocker — reconfirmación SQL posterior:** la comparación final ya había aprobado dentro del
  arnés runtime y la limpieza; una reconfirmación de solo lectura posterior no pudo abrir una nueva
  conexión por un fallo TLS local. No hubo escritura ni evidencia de cambio de datos.
- **Validated — regresión:** `git diff --check`; build completo con 0 errores/advertencias;
  30/30 pruebas SRI TXT; 89/89 pruebas SRI; suite completa 569 aprobadas, 5 omitidas por requerir
  infraestructura explícita y 0 fallidas.
- **Not applicable:** no se iniciaron workers ni se realizaron llamadas al SRI o SAP.

Esta recomendación histórica queda reemplazada por el cierre documentado el 2026-07-28.

## Evidencia automática de implementación

Validación final del CRUD, sin ejecutar SQL ni runtime:

- `git diff --check`: correcto.
- `dotnet build NuanSystem.sln --no-restore -v minimal`: correcto, 0 errores y 0 advertencias.
- pruebas focalizadas SRI TXT: 30 aprobadas.
- pruebas SRI completas: 89 aprobadas.
- suite completa: 569 aprobadas, 5 omitidas por requerir infraestructura explícita, 0 fallidas.
- los contratos cubren permisos VIEW/UPLOAD/ENQUEUE separados, paginación de servidor, filtros,
  deserialización, saneamiento de DTO, conexión tenant y estructura corporativa/Designer del formulario.
- los scripts `142`/`143` fueron revisados estáticamente y no se ejecutaron.
- la secuencia `138`–`143` tiene un único propietario y una única versión por migración:
  SRI núcleo `138`/`139`, PriceList `140`/`141` y SRI CRUD `142`/`143`.

Evidencia previa del núcleo, que no se repitió:

- las tres muestras locales fueron procesadas de forma transitoria: 7.028, 2.722 y 1.322 filas, Windows-1252 y sin filas inválidas; no se copiaron al repositorio ni se imprimieron claves.
- contrato estático: claim de script `117` conserva exclusivamente `Pending` y `RetryScheduled`.
- contrato estático: script `139` no contiene grants a `RolePermissions`.

La evidencia histórica del núcleo incluye despliegue idempotente de `138`/`139`, concurrencia SQL,
rollback, permisos con JWT, multipart y aislamiento tenant. Esa evidencia se conserva y no se repite.
No validado todavía: scripts `142`/`143`, consultas CRUD reales, navegación WinForms y Designer del
nuevo formulario.

## Principios de evidencia

- Usar base tenant de prueba autorizada y datos sintéticos; nunca copiar claves reales a fixtures del repositorio.
- No imprimir claves completas, RUC, XML, credenciales, cookies ni JWT.
- Identificar archivos y filas por hashes saneados.
- Etiquetar cada comprobación como `Validated`, `Not validated`, `Blocked` o `Not applicable`.
- Separar compilación, pruebas unitarias, SQL real, runtime, Designer y servicios externos.
- Toda validación SRI o SAP real requiere autorización independiente y alcance controlado.

## Matriz de pruebas

| Área | Escenario | Resultado esperado | Nivel |
| --- | --- | --- | --- |
| Parser | TXT válido UTF-8 | Cabecera/filas aceptadas, conteos correctos | Unit/contract |
| Parser | TXT válido Windows-1252 | Aceptado bajo política aprobada | Unit/contract |
| Parser | TXT vacío | Error estable, sin cabecera persistida parcial | Unit/API |
| Parser | ISO-8859-1 u otra codificación no permitida | Rechazo saneado | Unit/API |
| Parser | Cabecera incorrecta | Rechazo con código, sin eco sensible | Unit/API |
| Parser | Columnas faltantes o extra | Fila/archivo inválido según contrato | Unit/API |
| Parser | Línea corrupta, NUL o muy larga | Rechazo temprano y acotado | Unit/API |
| Parser | Importe `.25` | Normalizado a `0.25` | Unit |
| Parser | Fecha/importe fuera de rango | Fila inválida | Unit |
| Clave | Longitud/no numérica/módulo 11 inválido | Fila inválida | Unit |
| Clave | Tipo o ambiente no soportado | Fila inválida | Unit |
| Coherencia | Fecha/tipo/RUC/serie no coinciden con clave | `Conflict` o inválida según contrato | Unit |
| Duplicados | Clave repetida en el mismo archivo | Primera candidata; posteriores `DuplicateInFile` | Unit/SQL |
| Duplicados | Clave existente en otra carga | Nueva fila trazada y vinculada | SQL/API |
| Duplicados | Archivo completo duplicado exitoso | No crea otra carga | SQL/API |
| Duplicados | Archivo duplicado fallido | Requiere reprocess explícito | API/security |
| Concurrencia | Dos cargas simultáneas del mismo hash | Una identidad; otra recibe resultado idempotente/conflicto estable | SQL integration |
| Persistencia | Rollback al fallar TVP/procedimiento | Sin cabecera/detalle/conteos parciales | SQL integration |
| Persistencia | Reejecutar script tenant/Master | Sin duplicados ni pérdida | SQL real |
| Encolado | Clave válida no existente | Una fila de cola y vínculo | SQL integration |
| Encolado | Upload de fila válida | Cola nueva queda `Staged`, sin intento | SQL integration |
| Encolado | Upload de fila inválida | No crea cola | SQL integration |
| Encolado | Claim con colas `Staged` | Ninguna fila `Staged` es reclamada | SQL contract/integration |
| Encolado | Encolar `Staged` | Transición atómica a `Pending` y auditoría | SQL integration |
| Encolado | Repetir enqueue | Misma cola, sin duplicado | SQL integration |
| Encolado | Enqueue concurrente | Una transición/auditoría; segunda ejecución idempotente | SQL integration |
| Encolado | Documento ya en cola | `LinkedExisting` | SQL integration |
| Encolado | Cola preexistente en estado posterior | Se vincula sin reiniciar estado, intento o retry | SQL integration |
| Encolado | Documento autorizado | `LinkedAuthorized`, XML intacto | SQL integration |
| Encolado | Conflicto de identidad | `Conflict`, sin adopción | SQL integration |
| SRI | Servicio temporalmente no disponible | Retry existente; carga no se revierte | Worker test autorizado |
| SAP | Integración deshabilitada | SRI completa sin outbox SAP | Integration |
| SAP | HANA/Service Layer no disponible | `Retry`, sin rollback SRI | Integration |
| SAP | Aplicación repetida mismo hash | `Unchanged` | Integration |
| SAP | Mismo ExternalId, contenido distinto | `Conflict` terminal | Integration |
| SAP | Error funcional no recuperable | Terminal visible/auditado | Integration |
| Eliminación | Carga elegible | `IsDeleted`, evidencia relacionada intacta | SQL/API |
| Eliminación | Carga con documento autorizado | No borra cola, XML ni auditoría | SQL/API |
| Historial | Acciones principales | Actor, fecha, motivo y estado; sin clave completa | API/UI |
| Seguridad | Sin autenticación | 401 | API |
| Seguridad | Autenticado sin operación | 403 | API |
| Seguridad | Autorizado | 200/resultado esperado | API |
| Seguridad | Ver errores sin permiso | 403 y UI oculta/deshabilitada | API/UI |
| Tenant | Misma identidad en empresas distintas | Aislamiento total y conexión correcta | Integration |
| Tenant | Empresa no permitida/manipulada | Rechazo por middleware/contexto | API |
| Archivo | Nombre con ruta o caracteres inseguros | Solo basename saneado | Unit/API |
| Límites | 10 MiB/50.000 filas exactos | Aceptación dentro del límite | Performance |
| Límites | Un byte/fila por encima | Rechazo temprano | Performance |
| UI | Loading/empty/error/busy/read-only | Estados usables y sin doble envío | Manual/UI |
| UI | Filtros y paginación | Conteos backend; grid no carga sin límite | UI/API |
| UI | Abrir Monitor SRI | Navega solo con permiso y vínculo válido | UI |
| UI | Historial corporativo | Usa `RecordHistoryForm` | UI |
| UI | Indicador SAP | Texto+estado, no solo color | UI |
| Designer | Abrir formulario y `.Designer.cs` | Sin error de serialización | Visual Studio |
| Layout | Resize, DPI, tab order, clipping | Sin solapamientos; acciones accesibles | Manual/UI |
| Build | Proyectos tocados | 0 errores; warnings reportados | Build |
| Regression | Suite completa | Sin regresiones | Automated |
| CRUD | Listado filtrado | Paginación estable y totales calculados sobre el mismo filtro | SQL/API |
| CRUD | Detalle tenant | Solo la empresa activa puede consultar la importación | SQL/API |
| CRUD | Filas válidas/inválidas | Filtro y paginación estables; ninguna clave o línea completa | SQL/API |
| CRUD | Saneamiento | DTO/JSON/logs omiten `AccessKey`, cabecera/línea original, XML y secretos | Contract/API |
| CRUD | Permisos independientes | VIEW no concede UPLOAD ni ENQUEUE; cada ruta aplica su permiso | API/security |
| CRUD | Navegación de fila | Solo filas con `QueueId` habilitan apertura del monitor | UI |

## Fases de validación propuestas

### Fase A — Contratos y parser

- Aprobar cabecera, codificaciones, tipos, campos opcionales y límites.
- Pruebas unitarias puras de decoding, parsing, normalización y redacción.
- No usar las claves reales adjuntas como fixtures.

### Fase B — Persistencia tenant

- Revisar estáticamente script idempotente y nombres contra el último número disponible.
- Ejecutar dos veces en una base tenant autorizada.
- Probar TVP, rollback, unicidad por hash, `rowversion`, locks e índices.
- Limpiar solo datos de prueba identificados y autorizados.

### Fase C — API, permisos y multitenancy

- Probar multipart, límites, cancelación y errores estables.
- Validar 401/403/200 con JWT renovado después del seed.
- Validar empresas permitidas y aislamiento tenant.

### Fase D — Vínculo con SRI

- Usar dobles/stubs para el importador.
- Comprobar que carga/validación no llaman al SRI.
- Verificar una sola cola, creación `Staged`, exclusión del claim, transición explícita, vínculo existente/autorizado y no alteración del XML.
- Probar que el claim conserva literalmente los únicos estados `Pending` y `RetryScheduled`.
- Una prueba real SRI no queda autorizada por este plan.

### Fase E — CRUD API y WinForms

- Ejecutar `142` dos veces solo en tenants autorizados y `143` dos veces solo en Master.
- Renovar JWT después de `143` y probar perfiles VIEW, UPLOAD y ENQUEUE por separado.
- Validar listado, detalle y filas con límites 1, página intermedia y página vacía.
- Repetir filtros por fecha, estado, archivo y ambiente y contrastar totales con SQL saneado.
- Probar aislamiento cruzado entre DEMO, REMIGIO y CANARIS sin revelar datos.
- Verificar por inspección de respuestas que no aparecen clave completa, línea/cabecera TXT, XML,
  JWT, conexión o secreto.
- Cliente tipado a través de `NuanApiClient`.
- Revisión de `.Designer.cs`, controles corporativos, FormKey y permisos.
- Abrir Visual Studio Designer y validar layout, resizing, DPI, tab order y estados empty/error/busy.
- Probar paginación de ambos grids, enqueue condicionado y navegación por `QueueId` al monitor.
- No cargar TXT ni volver a probar `Staged -> Pending` salvo que la autorización runtime lo incluya
  expresamente; la fase CRUD puede usar fixtures controlados ya preparados.

### Fase F — SAP, después de aprobación

- Validar UDT/UDO en ambiente SAP autorizado mediante mecanismos soportados.
- Probar Service Layer/DI API sin escrituras directas a tablas estándar.
- Simular indisponibilidad, retry, lease vencido, repetición y conflicto.
- No ejecutar una prueba real contra SAP por la sola aprobación del core TXT.

### Fase G — Regresión y evidencia

- Build de solución/proyectos afectados.
- Suites focalizadas y completas.
- Escaneo de secretos/datos sensibles y revisión de logs.
- Evidencia saneada con comandos, alcance, resultados y limitaciones.

## Gates de aprobación

1. Contrato TXT, codificaciones y límites aprobados.
2. Retención indefinida provisional y minimización de claves aprobadas.
3. Modelo SQL y política de concurrencia aprobados para implementación; ejecución SQL requiere autorización independiente.
4. Permisos aprobados sin grants automáticos a roles existentes.
5. WinForms, FormKey, menú y UX aprobados para implementación; ejecución de `142`/`143` y runtime diferidos.
6. UDT, payload mínimo y Service Layer aprobados como dirección futura; todo código SAP permanece diferido.
7. Plan de despliegue/recuperación documentado antes de ejecutar SQL.

## Criterio de terminación

La implementación solo se considera validada cuando la evidencia distingue claramente pruebas automatizadas, SQL real autorizado, runtime, Designer y servicios externos. Un build correcto no sustituye pruebas de concurrencia, tenant, permisos, SRI o SAP.
