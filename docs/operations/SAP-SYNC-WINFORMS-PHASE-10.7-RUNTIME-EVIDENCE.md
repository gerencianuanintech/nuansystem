# Fase 10.7 — Evidencia runtime de navegación SAP WinForms

## Alcance autorizado

La validación se limitó a `NuanSystem_Master`, API local y formularios WinForms de Perfiles SAP y Ejecuciones SAP. Todos los workers permanecieron detenidos y no se realizaron llamadas a SAP, Service Layer o SRI.

## Despliegue y seguridad

| Gate | Resultado |
|---|---|
| Respaldo Master | Validado mediante `COPY_ONLY WITH CHECKSUM` y `RESTORE VERIFYONLY WITH CHECKSUM`. |
| Migración `160` | Dos ejecuciones correctas; una sola versión `20260731.160`. |
| Migración `161` | Dos ejecuciones correctas; una sola versión `20260801.161`. |
| Formularios y menús | Dos formularios y dos menús SAP únicos. |
| Grants ADMIN | Dos menús, once operaciones de Perfiles SAP y siete operaciones de Ejecuciones SAP. |
| Iconos especializados | Reintentar, cancelar y liberar lock vencido enlazados a recursos SVG corporativos de 16 y 32 px. |
| Otros roles | Cero grants nuevos de operaciones SAP. |
| TLS SQL | `Encrypt=true` y `TrustServerCertificate=false`. |

Los respaldos verificados se conservan con los nombres `NuanSystem_Master_Phase107_20260731_201133.bak` y `NuanSystem_Master_Migration161_20260801_175342.bak`. No se registran credenciales, cadenas de conexión, claves, JWT ni datos personales.

## API y Ribbon

La API local se inició únicamente durante la prueba y quedó detenida al finalizar.

| Gate | Resultado |
|---|---|
| Sin autenticación | HTTP 401. |
| Usuario autenticado sin permiso | HTTP 403. |
| ADMIN con permisos vigentes | HTTP 200 en perfiles y ejecuciones. |
| Ribbon Perfiles SAP | Once acciones: consultar, crear, editar, eliminar, validar, activar, desactivar, actualizar, filtrar, columnas y ver ejecuciones. |
| Ribbon Ejecuciones SAP | Siete acciones: consultar, actualizar, filtrar, columnas, reintentar, cancelar y liberar lock vencido. |
| Ejecutar manualmente | No expuesto porque no existe endpoint backend autorizado. |

Los JWT temporales se construyeron y consumieron exclusivamente en memoria y no fueron impresos ni persistidos.

## Validación visual

Se abrieron y revisaron los seis componentes de la vertical SAP:

- listado, edición y filtro de Perfiles SAP;
- listado, detalle y filtro de Ejecuciones SAP.

Se comprobaron navegación independiente, títulos, grillas, acciones, tamaño mínimo, redimensionamiento y cierre. El editor de perfil quedó en una sola superficie, sin pestañas, con datos generales en la parte superior y las grillas de entidades y programación en la parte inferior. Durante la revisión se detectó que los botones heredados Guardar y Cancelar se superponían con el contenido del perfil. Se corrigió su ubicación en el pie del formulario y se añadió una prueba de regresión.

Los listados de perfiles, ejecuciones y detalles usan paginación de servidor integrada con `NuanDataGridControl`. Los filtros SAP y los diálogos corporativos revisados usan las acciones Cancelar, Limpiar y Aplicar con botones de `100 x 36` px.

La aprobación visual final confirmó que Consultar, Reintentar, Cancelar y Liberar lock vencido muestran sus iconos. Se corrigió además la resolución de acciones integradas del Ribbon para ignorar alias no autorizados antes de seleccionar la operación permitida y su icono.

Visual Studio Designer abrió correctamente `SapSyncProfileEditForm` y `SapSyncExecutionDetailForm`. Para mantener esa compatibilidad, las colecciones de pestañas y columnas usan tipos explícitos y se conservaron los recursos `.resx` generados por el diseñador.

## Regresión y estado final

| Comprobación | Resultado |
|---|---|
| Build completo | 0 errores y 0 advertencias. |
| Pruebas WinForms SAP dirigidas | 13 superadas, 0 fallidas. |
| Suite completa | 734 superadas, 5 diagnósticas omitidas y 0 fallidas. |
| Procesos finales | Cero procesos NuanSystem, API, host visual o Visual Studio usados por la validación. |
| Servicios externos | Cero llamadas SAP, Service Layer o SRI. |
| Workers | No iniciados. |

La Fase 10.7 queda validada para navegación, permisos, Ribbon, API y presentación WinForms. Esta aprobación no habilita perfiles, agendas, workers ni ejecución manual SAP.
