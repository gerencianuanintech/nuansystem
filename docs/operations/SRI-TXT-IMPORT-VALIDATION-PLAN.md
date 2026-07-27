# Plan de validación — Importación TXT SRI

## Estado

Plan aprobado para validar el núcleo `TXT -> SriDocumentQueue`. La aprobación no autoriza ejecutar API, SQL Server, SAP HANA, workers, WinForms ni llamadas SRI.

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
| Encolado | Repetir enqueue | Misma cola, sin duplicado | SQL integration |
| Encolado | Documento ya en cola | `LinkedExisting` | SQL integration |
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
- Verificar una sola cola, vínculo existente/autorizado y no alteración del XML.
- Una prueba real SRI no queda autorizada por este plan.

### Fase E — WinForms

- Cliente tipado a través de `NuanApiClient`.
- Revisión de `.Designer.cs`, controles corporativos, FormKey y permisos.
- Abrir Visual Studio Designer y validar layout/resizing/DPI.
- Probar monitor, historial y navegación al documento.

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
5. WinForms, FormKey, menú y UX diferidos.
6. UDT, payload mínimo y Service Layer aprobados como dirección futura; todo código SAP permanece diferido.
7. Plan de despliegue/recuperación documentado antes de ejecutar SQL.

## Criterio de terminación

La implementación solo se considera validada cuando la evidencia distingue claramente pruebas automatizadas, SQL real autorizado, runtime, Designer y servicios externos. Un build correcto no sustituye pruebas de concurrencia, tenant, permisos, SRI o SAP.
